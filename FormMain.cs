using System;	//EventArgs
using System.ComponentModel;	//ThreadPool
using System.Diagnostics;	//Debug
using System.Drawing;	//Color
using System.Globalization;	//CultureInfo
using System.IO;	//DirectoryInfo
using System.Runtime.InteropServices;	//Marshall
using System.Security.Permissions;	//SecurityPermission,PermissionSet
using System.Text;	//StringBuilder
using System.Text.RegularExpressions;	//Regex
using System.Windows.Forms;	//Form
using System.Collections.Specialized;	//StringCollection

using Microsoft.VisualBasic.FileIO;	//FileSystem

// using ShellDll;
// using CG.Core;

using shell32;
using user32;
using ListView_SortManager;
//using jkhIContextMenu;

using CustomSettingsXML;
using QuickLogging;
using Microsoft.VisualBasic.Devices;	//CustomSettings

// [assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true)]
// [assembly: PermissionSet(SecurityAction.RequestOptional, Name = "Nothing")]

// [assembly: IsolatedStorageFilePermission(SecurityAction.RequestMinimum, UserQuota = 1048576)]
// [assembly: SecurityPermission(SecurityAction.RequestRefuse, UnmanagedCode = true)]
// [assembly: FileIOPermission(SecurityAction.RequestOptional, Unrestricted = true)]

[assembly: FileIOPermission(SecurityAction.RequestMinimum, Unrestricted = true)]
namespace jkhFileSearch
{
	public partial class FormMain : Form
	{
		public class FlickerFreeListView : ListView
		{
			public FlickerFreeListView() : base()
			{
				this.DoubleBuffered = true;
			}
		}

		public enum DateSearchType
		{
			NoDate,
			DateCreated,
			DateModified,
			DateAccessed
		}

		public struct SearchParams
		{
			public string lookIn;
			public string filename;
			public string containingText;
			public bool recurseSubdirs;
			public long minSize;
			public long maxSize;
			public DateTime modifiedAfter;
			public DateTime modifiedBefore;
			public DateSearchType dateSearchType;
			public bool regEx;
		}

		public struct TextSearchParams
		{
			public string lookIn;
			public string containingText;
		}

		private static BackgroundWorker _bwSearcher;
		private static BackgroundWorker _bwTextSearcher;
		private FileTypeIconManager _ftim = new FileTypeIconManager();
		private ListViewSortManager _sortManager;
		private bool _UserAborted;// = false;
		Stopwatch _durationStopwatch = new Stopwatch();
		private bool _closePending;// = false;
		private const int _maxComboSize = 10;
		ColumnHeader _headerModified;

		public FormMain()
		{
			InitializeComponent();
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			//global::jkhFileSearch.Properties.Settings.Default.Upgrade();
			//global::jkhFileSearch.Properties.Settings.Default.Reload();

			this.AcceptButton = buttonStart;

			listFiles.SmallImageList = _ftim.SmallImageList;
			listFiles.LargeImageList = _ftim.LargeImageList;

			/*ColumnHeader headerFileName =*/ listFiles.Columns.Add("File Name");
			/*ColumnHeader headerPath =*/ listFiles.Columns.Add("Path");
			ColumnHeader headerSize = listFiles.Columns.Add("Size");
			headerSize.TextAlign = HorizontalAlignment.Right;
			/*ColumnHeader headerExtension =*/ listFiles.Columns.Add("Extension");
			_headerModified = listFiles.Columns.Add("Modified");

			Type[] sortTypes = new Type[]{	typeof(ListViewTextSort), 
											typeof(ListViewTextSort),
											typeof(ListViewInt32MetricPrefixesSort),
											typeof(ListViewTextSort),
											typeof(ListViewDateSort)	};
			_sortManager = new ListViewSortManager(listFiles, sortTypes, -1, System.Windows.Forms.SortOrder.Ascending);  // Sort descending by second column

			CustomSettings settings = new CustomSettings();

			settings.RestoreColumnWidths(listFiles);
			splitContainer.SplitterDistance = settings.GetSetting("SplitterDistance", 120);
			checkIncludeSubdirs.Checked = settings.GetSetting("IncludeSubdirs", true);
			checkBoxRegularExpressionFileName.Checked = settings.GetSetting("RegularExpressionFileName", true);
			comboBoxResultView.SelectedIndex = settings.GetSetting("ResultView", 4);

			string[] fileNames = settings.GetSetting("comboFileNames", new string[0]);
			comboFileName.Items.AddRange(fileNames);

			string[] containingTexts = settings.GetSetting("comboContainingText", new string[0]);
			comboContainingText.Items.AddRange(containingTexts);

			numericUpDownMin.Value = numericUpDownMin.Minimum;
			numericUpDownMax.Value = numericUpDownMax.Maximum;
			dateTimePickerMin.Value = dateTimePickerMin.MinDate;
			dateTimePickerMax.Value = dateTimePickerMax.MaxDate;

			if(Environment.GetCommandLineArgs().Length > 1)
			{
				string path = Environment.GetCommandLineArgs()[1];
				int beginning = 0;
				if(path[0] == '\"')
					beginning = 1;
				int ending = path.Length;
				if(path[path.Length - 1] == '\"')
					ending = path.Length - 1;
				path = path.Substring(beginning, ending);
				if(path[path.Length - 1] != '\\')
					path += '\\';
				comboLookIn.Text = Path.GetFullPath(path);
				comboFileName.Text = "*.*";
			}
			else
			{
				comboFileName.Text = settings.GetSetting("filename", "");
				comboLookIn.Text = settings.GetSetting("lookIn", "");
			}

			comboBoxDateSearchType.DataSource = Enum.GetValues(typeof(DateSearchType)); 

			timerDurationDisplayUpdate.Tick += new EventHandler(DurationTimerTicks);

			settings.RestoreWindowPlacement(this);
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(BusySearching)
			{
				e.Cancel = true;	// "defer" closing until "SearchFinished_Final" (the search thread dies), which will hopefully be soon!
				_closePending = true;
				StopSearch();
			}
			else
			{
				CustomSettings settings = new CustomSettings();
				settings.PutSetting("SplitterDistance", splitContainer.SplitterDistance);
				settings.PutSetting("filename", comboFileName.Text);
				settings.PutSetting("lookIn", comboLookIn.Text);
				settings.PutSetting("IncludeSubdirs", checkIncludeSubdirs.Checked);
				settings.PutSetting("RegularExpressionFileName", checkBoxRegularExpressionFileName.Checked);
				settings.PutSetting("ResultView", comboBoxResultView.SelectedIndex);

				// trim list of fileNames so it is manageable
				while(comboFileName.Items.Count > _maxComboSize)
					comboFileName.Items.RemoveAt(comboFileName.Items.Count - 1);
				string[] fileNames = new string[comboFileName.Items.Count];
				comboFileName.Items.CopyTo(fileNames, 0);
				settings.PutSetting("comboFileNames", fileNames);

				// trim list of containingTexts so it is manageable
				while(comboContainingText.Items.Count > _maxComboSize)
					comboContainingText.Items.RemoveAt(comboContainingText.Items.Count - 1);
				string[] containingTexts = new string[comboContainingText.Items.Count];
				comboContainingText.Items.CopyTo(containingTexts, 0);
				settings.PutSetting("comboContainingText", containingTexts);

				settings.SaveColumnWidths(listFiles);
				settings.SaveWindowPlacement(this);
			}
		}
		#region Tool Strip Button
		private void toolStripButtonSettings_Click(object sender, EventArgs e)
		{
			FormSettings settings = new FormSettings();
			settings.ShowDialog();
		}

		private void toolStripButtonStop_Click(object sender, EventArgs e)
		{
			StopSearch();
		}
		#endregion	//Tool Strip Button


		#region File Search
		private void buttonStart_Click(object sender, EventArgs e)
		{
			StartSearch();
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			bool retval = true;
			if(keyData == Keys.Enter)// && checkContainingText.Focused == true)
				StartSearch();
			else if(keyData == Keys.Delete && listFiles.Focused == true)	// keep listFiles.Focused or you can't use DEL in file name and search text
				DeleteSelectedFiles(true);
			else if(keyData == (Keys.Delete | Keys.Shift) && listFiles.Focused == true)	// keep listFiles.Focused or you can't use DEL in file name and search text
				DeleteSelectedFiles(false);
			else
			{
				//Debug.WriteLine(keyData.ToString());
				retval = base.ProcessDialogKey(keyData);
			}
			return retval;
		}

		private void comboFileName_TextChanged(object sender, EventArgs e)
		{
			try
			{
				Regex test = new Regex(comboFileName.Text);
				checkBoxRegularExpressionFileName.Enabled = true;
			}
			catch(Exception /*exc*/)
			{
				checkBoxRegularExpressionFileName.Enabled = false;
				checkBoxRegularExpressionFileName.Checked = false;
			}
		}

		SearchParams BuildSearchParams()
		{
			SearchParams retval = new SearchParams();

			retval.lookIn = comboLookIn.Text;
			if(string.IsNullOrEmpty(retval.lookIn) || retval.lookIn[retval.lookIn.Length - 1] != '\\')
				retval.lookIn += '\\';
			retval.filename = comboFileName.Text;
			if(!checkBoxRegularExpressionFileName.Checked)
			{
				if(retval.filename.Length == 0 || retval.filename[0] != '*')
					retval.filename = '*' + retval.filename;
				if(retval.filename[retval.filename.Length - 1] != '*')
					retval.filename += '*';
			}
			retval.containingText = comboContainingText.Text;
			retval.recurseSubdirs = checkIncludeSubdirs.Checked;
			retval.minSize = (long)numericUpDownMin.Value;
			retval.maxSize = (long)numericUpDownMax.Value;
			retval.modifiedAfter = dateTimePickerMin.Value;// DateTime.MinValue;
			retval.modifiedBefore = dateTimePickerMax.Value;//DateTime.MaxValue;
			retval.dateSearchType = (DateSearchType)this.comboBoxDateSearchType.SelectedValue; 
			retval.regEx = checkBoxRegularExpressionFileName.Enabled && checkBoxRegularExpressionFileName.Checked;

			// quick GUI hack to change the text of the file results "Date Modified" column
			string dateHeader;
			switch(retval.dateSearchType)
			{
				case DateSearchType.DateAccessed:
					dateHeader = "Accessed";
					break;
				case DateSearchType.DateCreated:
					dateHeader = "Created";
					break;
				case DateSearchType.DateModified:
				case DateSearchType.NoDate:
				default:
					dateHeader = "Modified";
					break;
			}
			_headerModified.Text = dateHeader;

			return retval;
		}

		protected void EnableSearchGUI(bool enableSearch)
		{
			buttonStart.Enabled = enableSearch;
			comboContainingText.Enabled = enableSearch;
			comboFileName.Enabled = enableSearch;
			checkBoxRegularExpressionFileName.Enabled = enableSearch;
			checkIncludeSubdirs.Enabled = enableSearch;

			buttonStop.Enabled = !enableSearch;
			toolStripButtonStop.Enabled = !enableSearch;

			if(enableSearch)
				this.Cursor = Cursors.Default;
			else
				this.Cursor = Cursors.AppStarting;
		}

		static protected void AppendComboBoxText(ComboBox combo)
		{
			if(combo != null)
			{
				if(!string.IsNullOrEmpty(combo.Text))
				{
					int indexExistingItem = combo.Items.IndexOf(combo.Text);
					if(indexExistingItem != 0)
					{
						if(indexExistingItem > 0)
						{	// sort to top (most recently used)
							string textCopy = combo.Text;
							combo.Items.RemoveAt(indexExistingItem);
							combo.Items.Insert(0, textCopy);
							combo.Text = textCopy;
						}
						else
						{	// new item
							combo.Items.Insert(0, combo.Text);
						}
					}
				}
			}
		}

		static private bool BusySearching
		{
			get
			{
				return (_bwSearcher != null && _bwSearcher.IsBusy) || (_bwTextSearcher != null && _bwTextSearcher.IsBusy);
			}
		}

		protected void StartSearch()
		{
			if(_bwSearcher == null || !_bwSearcher.IsBusy)
			{
				EnableSearchGUI(false);

				AppendComboBoxText(comboFileName);
				AppendComboBoxText(comboContainingText);

				_UserAborted = false;

				listFiles.Items.Clear();
				_ftim.Clear();
				richTextBox.Text = "";
				_durationStopwatch.Reset();
				UpdateDurationDisplay();
				_durationStopwatch.Start();

				timerDurationDisplayUpdate.Start();

				try
				{
					toolStripSplitButtonLogPopUp.Image = jkhFileSearch.Properties.Resources.LogPopUpGood;	//reset the pop-up log indicator
					toolStripButtonLogging.Image = jkhFileSearch.Properties.Resources.LogPopUpGood;

					AddPopUpMessage("Searching: " + comboLookIn.Text, Color.DarkGreen);

					_bwSearcher = new BackgroundWorker();
					_bwSearcher.WorkerSupportsCancellation = true;
					_bwSearcher.DoWork += SearchAsync;
					_bwSearcher.RunWorkerCompleted += SearchCompleted;
					_bwSearcher.RunWorkerAsync(BuildSearchParams());
				}
				catch(System.ArgumentException exc)
				{
					MessageBox.Show("Invalid argument: " + exc.Message);
				}
				catch(Exception exc)
				{
					MessageBox.Show("Exception(1): " + exc.Message);
				}
			}
		}

		private void DurationTimerTicks(Object obj, EventArgs evtArgs)
		{
			UpdateDurationDisplay();
		}

		private delegate void UpdateDurationDisplayDelegate();
		private void UpdateDurationDisplay()
		{
			if(listFiles.InvokeRequired)
			{
				this.Invoke(new UpdateDurationDisplayDelegate(UpdateDurationDisplay));
			}
			else
			{
				toolStripStatusLabelDuration.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", _durationStopwatch.Elapsed.Hours, _durationStopwatch.Elapsed.Minutes, _durationStopwatch.Elapsed.Seconds);
				if(_UserAborted)
					toolStripStatusLabelDuration.Text += "(Aborted)";
			}
		}

		void SearchAsync(object sender, DoWorkEventArgs e)
		{
			SearchParams sp = (SearchParams)e.Argument;
			SearchFolder(sp.lookIn, sp);
		}

		protected void SearchFolder(string lookIn, SearchParams sp)
		{
			if(!string.IsNullOrEmpty(sp.filename))
			{
				if(!_bwSearcher.CancellationPending)
				{
					SetStatusPath(lookIn);

					DirectoryInfo di = new DirectoryInfo(lookIn);
					try
					{
						// only add dirs to list if not searching for text
						if(string.IsNullOrEmpty(sp.containingText))
						{
							DirectoryInfo[] dirs;
							if(sp.regEx)
								dirs = di.GetDirectories();
							else
								dirs = di.GetDirectories(sp.filename);
							foreach(DirectoryInfo dirsInfo in dirs)
							{
								if(_bwSearcher.CancellationPending)
									break;
								bool bDateOK = true;
								switch(sp.dateSearchType)
								{
									case DateSearchType.NoDate:
										break;
									case DateSearchType.DateAccessed:
										bDateOK = (sp.modifiedAfter <= dirsInfo.LastAccessTime) && (dirsInfo.LastAccessTime <= sp.modifiedBefore);
										break;
									case DateSearchType.DateCreated:
										bDateOK = (sp.modifiedAfter <= dirsInfo.CreationTime) && (dirsInfo.CreationTime <= sp.modifiedBefore);
										break;
									case DateSearchType.DateModified:
										bDateOK = (sp.modifiedAfter <= dirsInfo.LastWriteTime) && (dirsInfo.LastWriteTime <= sp.modifiedBefore);
										break;
								}
								if(bDateOK && (!sp.regEx || Regex.IsMatch(dirsInfo.Name, sp.filename)))
								{
									AddDirectory(sp, dirsInfo);
								}
							}
						}

						FileInfo[] files;
						if(sp.regEx)
							files = di.GetFiles();
						else
							files = di.GetFiles(sp.filename);
						foreach(FileInfo fi in files)
						{
							if(_bwSearcher.CancellationPending)
								break;

							//Debug.WriteLine(fi.Name);

							if((fi.Length <= sp.maxSize) && (fi.Length >= sp.minSize))
							{
								bool bDateOK = true;
								switch(sp.dateSearchType)
								{
									case DateSearchType.NoDate:
										break;
									case DateSearchType.DateAccessed:
										bDateOK = (sp.modifiedAfter <= fi.LastAccessTime) && (fi.LastAccessTime <= sp.modifiedBefore);
										break;
									case DateSearchType.DateCreated:
										bDateOK = (sp.modifiedAfter <= fi.CreationTime) && (fi.CreationTime <= sp.modifiedBefore);
										break;
									case DateSearchType.DateModified:
										bDateOK = (sp.modifiedAfter <= fi.LastWriteTime) && (fi.LastWriteTime <= sp.modifiedBefore);
										break;
								}

								if(bDateOK && (!sp.regEx || Regex.IsMatch(fi.Name, sp.filename)))
								{
									if(string.IsNullOrEmpty(sp.containingText))
									{
										AddFile(sp, fi);
									}
									else
									{
										StreamReader sr = File.OpenText(Path.Combine(fi.DirectoryName, fi.Name));
										string line = sr.ReadLine();
										while(line != null && !_bwSearcher.CancellationPending)
										{
											if(line.IndexOf(sp.containingText, StringComparison.CurrentCultureIgnoreCase) >= 0)
											{
												try
												{
													AddFile(sp, fi);
												}
												catch(Exception exc)
												{
													AddPopUpMessage("Exception adding \"" + fi.FullName + "\":" + exc.Message, Color.DarkRed);
													Exception copy = exc;
												}
												break;
											}
											if(!_bwSearcher.CancellationPending)
												line = sr.ReadLine();
										}
										sr.Close();
									}
								}
							}
						}
					}
					catch(UnauthorizedAccessException exc)
					{
						AddPopUpMessage("UnauthorizedAccessException in SearchFolder: " + exc.Message, Color.DarkRed);
					}
					catch(Exception exc)
					{
						AddPopUpMessage("Exception in SearchFolder: " + exc.Message, Color.DarkRed);
// 						if(MessageBox.Show("Exception in SearchFolder: " + exc.Message, "Exception processing folder", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
// 							_bwSearcher.CancelAsync();
					}

					if(sp.recurseSubdirs && !_bwSearcher.CancellationPending)
					{
						try
						{
							DirectoryInfo[] subs = di.GetDirectories();
							foreach(DirectoryInfo sub in subs)
							{
								if(_bwSearcher.CancellationPending)
									break;

								try
								{
									SearchFolder(sub.FullName, sp);
								}
								catch(UnauthorizedAccessException exc)
								{
									AddPopUpMessage("UnauthorizedAccessException in SearchFolder for Directories: " + exc.Message, Color.DarkRed);
								}
								catch(Exception exc)
								{
									AddPopUpMessage("Exception in SearchFolder for Directories: " + exc.Message, Color.DarkRed);
// 									if(MessageBox.Show("Exception in SearchFolder for Directories: " + exc.Message, "Exception processing folder", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
// 										_bwSearcher.CancelAsync();
								}
							}
						}
						catch(UnauthorizedAccessException exc)
						{
							AddPopUpMessage("UnauthorizedAccessException after GetDirectories: " + exc.Message, Color.DarkRed);
						}
						catch(Exception exc)
						{
							AddPopUpMessage("Exception after GetDirectories: " + exc.Message, Color.DarkRed);
// 							if(MessageBox.Show("Exception after GetDirectories: " + exc.Message, "Exception processing Directories", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
// 								_bwSearcher.CancelAsync();
						}
					}
				}
			}
		}
		#endregion	//File Search

		private delegate void SetStatusPathDelegate(string path);
		private void SetStatusPath(string path)
		{
			if(listFiles.InvokeRequired)
			{
				this.Invoke(new SetStatusPathDelegate(SetStatusPath), new Object[] { path });
			}
			else
			{
				toolStripStatusLabel.Text = "Searching: " + path;
			}
		}

		private delegate void AddFileDelegate(SearchParams sp, FileInfo fi);
		private void AddFile(SearchParams sp, FileInfo fi)
		{
			if(listFiles.InvokeRequired)
			{
				this.Invoke(new AddFileDelegate(AddFile), new Object[] { sp, fi });
			}
			else
			{
				ListViewItem lvi = new ListViewItem(fi.Name);
				lvi.SubItems.Add(fi.DirectoryName);
				lvi.SubItems.Add(BuildFileSizeText(fi.Length));
				lvi.SubItems.Add(fi.Extension);
				string DateText;
				switch(sp.dateSearchType)
				{
					case DateSearchType.DateAccessed:
						DateText = fi.LastAccessTime.ToShortDateString() + " " + fi.LastAccessTime.ToShortTimeString();
						break;
					case DateSearchType.DateCreated:
						DateText = fi.CreationTime.ToShortDateString() + " " + fi.CreationTime.ToShortTimeString();
						break;
					case DateSearchType.NoDate:
					case DateSearchType.DateModified:
					default:
						DateText = fi.LastWriteTime.ToShortDateString() + " " + fi.LastWriteTime.ToShortTimeString();
						break;
				}
				lvi.SubItems.Add(DateText);
				lvi.ImageIndex = _ftim.GetIconIndex(fi.FullName);

				listFiles.Items.Add(lvi);
			}
		}

		private delegate void AddDirectoryDelegate(SearchParams sp, DirectoryInfo di);
		private void AddDirectory(SearchParams sp, DirectoryInfo di)
		{
			if(listFiles.InvokeRequired)
			{
				this.Invoke(new AddDirectoryDelegate(AddDirectory), new Object[] { sp, di });
			}
			else
			{
				ListViewItem lvi = new ListViewItem(di.Name);
				lvi.SubItems.Add(BuildDirPath(di));
				lvi.SubItems.Add("");
				lvi.SubItems.Add("[Dir]");
				string DateText;
				switch(sp.dateSearchType)
				{
					case DateSearchType.DateAccessed:
						DateText = di.LastAccessTime.ToShortDateString() + " " + di.LastAccessTime.ToShortTimeString();
						break;
					case DateSearchType.DateCreated:
						DateText = di.CreationTime.ToShortDateString() + " " + di.CreationTime.ToShortTimeString();
						break;
					case DateSearchType.NoDate:
					case DateSearchType.DateModified:
					default:
						DateText = di.LastWriteTime.ToShortDateString() + " " + di.LastWriteTime.ToShortTimeString();
						break;
				}
				lvi.SubItems.Add(DateText);
				lvi.ImageIndex = _ftim.GetIconIndexDir(di.FullName);
				listFiles.Items.Add(lvi);
			}
		}

		private string BuildDirPath(DirectoryInfo di)
		{
			string retval = "";
			DirectoryInfo diSeek = di.Parent;
			while(diSeek != null)
			{
				if(diSeek.Name.EndsWith("\\") || string.IsNullOrEmpty(retval))
					retval = diSeek.Name + retval;
				else
					retval = diSeek.Name + "\\" + retval;
				diSeek = diSeek.Parent;
			}
			return retval;
		}

		static private string BuildFileSizeText(long fileSize)
		{
			string retval;
			if(fileSize < 1024)
			{	//bytes
				retval = fileSize.ToString(CultureInfo.CurrentCulture) + " B";
			}
			else if(fileSize < 1024 * 1024)
			{	//Kilobytes
				retval = (fileSize / 1024).ToString(CultureInfo.CurrentCulture) + " KB";
			}
			else if(fileSize < 1024 * 1024 * 1024)
			{	//Megabytes
				retval = (fileSize / (1024 * 1024)).ToString(CultureInfo.CurrentCulture) + " MB";
			}
			else //if (fileSize < 1024 * 1024 * 1024 * 1024)
			{	//Gigabytes
				retval = (fileSize / (1024 * 1024 * 1024)).ToString(CultureInfo.CurrentCulture) + " GB";
			}
			return retval;
		}

		#region Stop File Search
		private void buttonStop_Click(object sender, EventArgs e)
		{
			StopSearch();
		}

		protected void StopSearch()
		{
			_UserAborted = true;
			buttonStop.Enabled = false;
			toolStripButtonStop.Enabled = false;

			if(_bwSearcher != null && _bwSearcher.IsBusy)
				_bwSearcher.CancelAsync();
			else
				SearchFinished_Final();

			if(_bwTextSearcher != null && _bwTextSearcher.IsBusy)
				_bwTextSearcher.CancelAsync();
		}

		void SearchCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.Invoke(new SearchFinished_FinalDelegate(SearchFinished_Final));
		}

		private delegate void SearchFinished_FinalDelegate();
		private void SearchFinished_Final()
		{
			_durationStopwatch.Stop();
			timerDurationDisplayUpdate.Stop();

			if(_closePending)
			{
				Close();
			}
			else
			{
				EnableSearchGUI(true);
				if(listFiles.Items.Count == 1)
					toolStripStatusLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0} file found", listFiles.Items.Count);
				else
					toolStripStatusLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0} files found", listFiles.Items.Count);
				if(_UserAborted)
					toolStripStatusLabel.Text += " (User Aborted)";
				UpdateDurationDisplay();
			}
		}
		#endregion Stop File Search


		private void listFiles_MouseDoubleClick(object sender, EventArgs e)
		{
			foreach(ListViewItem lvi in listFiles.SelectedItems)
			{
				System.Diagnostics.Process p = new System.Diagnostics.Process();
				p.StartInfo.FileName = Path.Combine(lvi.SubItems[1].Text, lvi.SubItems[0].Text);
				try
				{
					p.Start();
				}
				catch(Exception exc)
				{
					MessageBox.Show(exc.Message);
				}
			}
		}

		//private delegate void listFiles_MouseClickDelegate(object sender, MouseEventArgs e);
		private void listFiles_MouseClick(object sender, MouseEventArgs e)
		{
			ListViewItem lvi = listFiles.GetItemAt(e.X, e.Y);
			if(lvi != null)
			{
				if(e.Button == MouseButtons.Right)
				{
					IShellFolder desktopFolder = null;
					IShellFolder[] folders = new IShellFolder[listFiles.SelectedItems.Count];
					IntPtr[] folderPidls = new IntPtr[listFiles.SelectedItems.Count];
					IntPtr[] filePidls = new IntPtr[listFiles.SelectedItems.Count];

					for(int countItems = 0; countItems < listFiles.SelectedItems.Count; countItems++)
					{
						filePidls[countItems] = IntPtr.Zero;
						folderPidls[countItems] = IntPtr.Zero;
						folders[countItems] = null;
					}

					try
					{
						int attr = 0;
						int pchEaten = 0;
						int hr;

						// Get a reference to the desktop folder object.
						Shell32Methods.SHGetDesktopFolder(out desktopFolder);

						// Build an ARRAY of filePidl for each selected item
						for(int countSelections = 0; countSelections < listFiles.SelectedItems.Count; countSelections++)
						{
							string path = Path.Combine(listFiles.SelectedItems[countSelections].SubItems[1].Text, listFiles.SelectedItems[countSelections].SubItems[0].Text);

							// Get the PIDL for the file's folder.
							hr = desktopFolder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero, Path.GetDirectoryName(path), ref pchEaten, out folderPidls[countSelections], out attr);
							if(hr != 0)
								Marshal.ThrowExceptionForHR(hr);

							// Get a reference to the file's folder object.
							hr = desktopFolder.BindToObject(folderPidls[countSelections], IntPtr.Zero, ref Shell32Methods.IID_IShellFolder, ref folders[countSelections]);
							if(hr != 0)
								Marshal.ThrowExceptionForHR(hr);

							// Get the PIDL for the file - relative to the parent folder.
							hr = folders[countSelections].ParseDisplayName(IntPtr.Zero, IntPtr.Zero, Path.GetFileName(path), ref pchEaten, out filePidls[countSelections], out attr);
							if(hr != 0)
								Marshal.ThrowExceptionForHR(hr);
						}
						
						int reserved = 0;
						IUnknown unk = null;
						// Get the IID_IContextMenu interface associated with the file object.
						hr = folders[0].GetUIObjectOf(IntPtr.Zero, filePidls.Length, filePidls, ref Shell32Methods.IID_IContextMenu, reserved, out unk);
						if(hr != 0)
							Marshal.ThrowExceptionForHR(hr);

						ContextMenu menu = new ContextMenu();
						IContextMenu cm = (IContextMenu)unk;
						//menu.MenuItems.Add(new MenuItem("TEST"));
						cm.QueryContextMenu(menu.Handle, 0, uint.MinValue, uint.MaxValue, CMF.CMF_NORMAL);

						User32Methods.InsertMenu(menu.Handle, 0, MF.MF_BYPOSITION, 57575, "Explore To:");

						System.Drawing.Point clickPoint = listFiles.PointToScreen(e.Location);
						UInt32 menuResult = User32Methods.TrackPopupMenu(menu.Handle, TPM.TPM_RETURNCMD, clickPoint.X, clickPoint.Y, 0, this.Handle, IntPtr.Zero);
						if(menuResult != 0)
						{
							if(menuResult == 57575)
							{
								// just the path of the item under the mouse!
								string path = Path.Combine(lvi.SubItems[1].Text, lvi.SubItems[0].Text);

								System.Diagnostics.Process p = new System.Diagnostics.Process();
								p.StartInfo.FileName = "explorer.exe";
								p.StartInfo.Arguments = "/e,/select," + path;

								Debug.WriteLine(p.StartInfo.FileName + " " + p.StartInfo.Arguments);
								try
								{
									p.Start();
								}
								catch(Exception exc)
								{
									MessageBox.Show(exc.ToString());
								}
							}
							else
							{
								StringBuilder sb = new StringBuilder(256);
								cm.GetCommandString(menuResult, GCS.GCS_VERBW, 0, sb, sb.Length);
								//byte[] bytes = new byte[256];
								//cm.GetCommandString(menuResult, GCS.GCS_VERBW, 0, bytes, bytes.Length);

								CMINVOKECOMMANDINFOEX cmici = new CMINVOKECOMMANDINFOEX();
								cmici.cbSize = Marshal.SizeOf(cmici);
								cmici.fMask = 0;
								cmici.hwnd = this.Handle;
								cmici.lpVerb = (IntPtr)((menuResult) & 0x0000FFFF);
								cmici.lpParameters = null;
								cmici.lpDirectory = null;
								cmici.nShow = SW.SW_SHOWNORMAL;
								cmici.dwHotKey = 0;
								cmici.hIcon = IntPtr.Zero;
								cmici.lpTitle = null;
								cmici.lpVerbW = (IntPtr)((menuResult) & 0x0000FFFF);
								cmici.lpParametersW = null;
								cmici.lpDirectoryW = null;
								cmici.lpTitleW = null;
								cmici.ptInvoke = POINT.FromPoint(clickPoint);
								Int32 debugRetval = cm.InvokeCommand(ref cmici);
							}
						}
					}
					catch(Exception exc)
					{
						MessageBox.Show(exc.ToString());
					}
					finally
					{
						foreach(IntPtr intptr in filePidls)
							if(intptr != IntPtr.Zero)
								Marshal.FreeCoTaskMem(intptr);

						foreach(IntPtr intptr in folderPidls)
							if(intptr != IntPtr.Zero)
								Marshal.FreeCoTaskMem(intptr);

						foreach(IShellFolder fldr in folders)
						{
							if(fldr != null)
								Marshal.ReleaseComObject(fldr);
						}

						if(desktopFolder != null)
							Marshal.ReleaseComObject(desktopFolder);
					}
				}
			}
		}

		private delegate void AppendText_ColorDelegate(string text, Color color);
		private void AppendText_Color(string text, Color color)
		{
			if(listFiles.InvokeRequired)
			{
				this.Invoke(new AppendText_ColorDelegate(AppendText_Color), new Object[] { text, color });
			}
			else
			{
				if(!string.IsNullOrEmpty(text))
				{
					richTextBox.SuspendLayout();
					int originalTextEnd = richTextBox.Text.Length;
					richTextBox.AppendText(text);
					richTextBox.Select(originalTextEnd, text.Length);
					richTextBox.SelectionColor = color;
					richTextBox.SelectionLength = 0;
					richTextBox.ScrollToCaret();
					richTextBox.ResumeLayout();
					richTextBox.Update();
				}
			}
		}

		private void SetTextColor(int indexBegin, int length)
		{
			//if(indexBegin < indexEnd)
			{
				richTextBox.Select(indexBegin, length);
				richTextBox.SelectionColor = Color.Red;
				richTextBox.SelectionLength = 0;
			}
		}

		private delegate void AppendLineDelegate(string text);
		private void AppendLine(string text)
		{
			if(listFiles.InvokeRequired)
			{
				this.Invoke(new AppendLineDelegate(AppendLine), new Object[] { text });
			}
			else
			{
				if(!string.IsNullOrEmpty(text))
				{
					richTextBox.SuspendLayout();
					int originalTextEnd = richTextBox.Text.Length;
					richTextBox.AppendText(text);
					richTextBox.Select(originalTextEnd, text.Length);

					int index = 0;
					while(index >= 0)
					{
						int foundIndex = text.IndexOf(comboContainingText.Text, index, StringComparison.CurrentCultureIgnoreCase);
						if(foundIndex >= 0)
						{
							SetTextColor(originalTextEnd + foundIndex, comboContainingText.Text.Length);
							index = foundIndex + comboContainingText.Text.Length;
						}
						else
						{
							break;
						}
					}

					richTextBox.SelectionLength = 0;
					richTextBox.ScrollToCaret();
					richTextBox.ResumeLayout();
					richTextBox.Update();
				}
			}
		}

		private void AppendText_FontStyle(string text, FontStyle fs)
		{
			if(!string.IsNullOrEmpty(text))
			{
				//speed up GUI
				richTextBox.SuspendLayout();

				// append and font-ize
				int originalTextEnd = richTextBox.Text.Length;
				richTextBox.AppendText(text);
				richTextBox.Select(originalTextEnd, text.Length);
				Font oldFont = richTextBox.SelectionFont;
				richTextBox.SelectionFont = new Font(richTextBox.SelectionFont, fs);
				richTextBox.SelectionLength = 0;

				// put the old font back
				richTextBox.Select(text.Length, 0);
				richTextBox.SelectionFont = oldFont;
				
				richTextBox.ScrollToCaret();
				richTextBox.ResumeLayout();
				richTextBox.Update();
			}
		}

		#region Search for Text (Right Pane)
		TextSearchParams BuildTextSearchParams(string path)
		{
			TextSearchParams retval = new TextSearchParams();

			retval.lookIn = path;
			retval.containingText = comboContainingText.Text;

			return retval;
		}

		private void StartTextSearch(string path)
		{
			this.Cursor = Cursors.AppStarting;

			if(_bwTextSearcher != null && _bwTextSearcher.IsBusy)
				_bwTextSearcher.CancelAsync();
			// I would LOVE to wait right here for the thread to complete, but
			//	this is a race condition because "this" thread is needed by the
			//	_bwTextSearcher thread to do AppendText_Color calls

			try
			{
				_bwTextSearcher = new BackgroundWorker();
				_bwTextSearcher.WorkerSupportsCancellation = true;
				_bwTextSearcher.DoWork += TextSearchAsync;
				_bwTextSearcher.RunWorkerCompleted += TextSearchCompleted;
				_bwTextSearcher.RunWorkerAsync(BuildTextSearchParams(path));
			}
			catch(System.ArgumentException exc)
			{
				MessageBox.Show("Invalid argument: " + exc.Message);
			}
			catch(Exception exc)
			{
				MessageBox.Show("Exception(1): " + exc.Message);
			}
		}

		private void TextSearchAsync(object sender, DoWorkEventArgs e)
		{
			TextSearchParams sp = (TextSearchParams)e.Argument;
			SearchFileForText(sp.lookIn, sp.containingText);
		}

		private void SearchFileForText(string lookIn, string lookFor)
		{
			if(!string.IsNullOrEmpty(lookIn))
			{
				int lineCount = 0;
				StreamReader sr;
				string line;
				sr = File.OpenText(lookIn);
				line = sr.ReadLine();
				while(line != null && !_bwTextSearcher.CancellationPending)
				{
					lineCount++;
					if(line.IndexOf(lookFor, StringComparison.CurrentCultureIgnoreCase) >= 0)
					{
						if(line.Length > 1024)
							line = line.Substring(0, 1024);
						string newLineText = string.Format("{0}. {1}\n", lineCount, line);
#if USE_BEGIN_INVOKE_JEOFF
						// trying to fix that deadlock issue with the GUI thread
						//	canceling this thread (CancelAsync = CancellationPending)
						//	but this thread waiting for GUI to complete and GUI
						//	thread waiting for this thread to cancel.
						// Using "BeginInvoke" in this way did NOT work!
						//IAsyncResult result = this.BeginInvoke(new AppendLineDelegate(AppendLine), new Object[] { newLineText });
						//IAsyncResult result = this.BeginInvoke(new MethodInvoker(AppendLine(newLineText)));
						//while(!result.IsCompleted)
						//{
						//	if(_bwTextSearcher.CancellationPending)
						//		break;
						//	System.Threading.Thread.Sleep(1);
						//}
						//this.EndInvoke(result);
#else
						AppendLine(newLineText);
#endif
					}
					if(!_bwTextSearcher.CancellationPending)
						line = sr.ReadLine();
				}
				sr.Close();
			}
		}

		private void TextSearchCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.Invoke(new TextSearchCompletedDelegate(TextSearchCompleted));
		}

		private delegate void TextSearchCompletedDelegate();
		private void TextSearchCompleted()
		{
			this.Cursor = Cursors.Default;
		}
		#endregion Search for Text (Right Pane)


		private void listFiles_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if(_bwTextSearcher != null && _bwTextSearcher.IsBusy)
			{	// the thread is already busy loading text from another file, cancel and QUIT!
				_bwTextSearcher.CancelAsync();

				// Smells like a race condition here! We should wait for the 
				//	_bwTextSearcher thread to be done before we start the next
				//	search, but yet it doesn't crash in my experience. When it 
				//	becomes a real problem then I'll debug it.
			}
			richTextBox.Text = "";

			string path = Path.Combine(e.Item.SubItems[1].Text, e.Item.SubItems[0].Text);
			FileInfo fi = new FileInfo(path);
			StringBuilder fileInfoText = new StringBuilder();
			fileInfoText.Append(path);
			fileInfoText.Append(" (");
			if(fi.Exists)
			{
				fileInfoText.Append(BuildFileSizeText(fi.Length));
				fileInfoText.Append(", ");
			}
			fileInfoText.Append(fi.LastWriteTime.ToShortDateString());
			fileInfoText.Append(" ");
			fileInfoText.Append(fi.LastWriteTime.ToShortTimeString());
			fileInfoText.Append(")\n");
			AppendText_FontStyle(fileInfoText.ToString(), FontStyle.Bold | FontStyle.Underline);

			if(comboContainingText.Text.Length > 0)
				StartTextSearch(path);
		}

		private void listFiles_ItemDrag(object sender, ItemDragEventArgs e)
		{	// ignore (ListViewItem)e.Item and just use listFiles.SelectedItems for multi-select
			// WARNING: how you do this is important. Goofy .NET!

			// FIRST we have to build the list of files being dragged
			StringCollection sc = new StringCollection();
			foreach(ListViewItem lvi in listFiles.SelectedItems)
				sc.Add(Path.Combine(lvi.SubItems[1].Text, lvi.SubItems[0].Text));

			//	SECOND: You MUST use the 2-parameter constructor of DataObject AND call SetFileDropList!!!
			//	Otherwise I get unexpected results when dropping files into NOTEPAD.exe
			//	(like it shows the text "FileDrop" instead of opening the files in notepad. Argh!)
			DataObject dragStuff = new DataObject(DataFormats.FileDrop, sc);
			dragStuff.SetFileDropList(sc);

			listFiles.DoDragDrop(dragStuff, DragDropEffects.Copy);
		}

		private void DeleteSelectedFiles(bool useRecycler)
		{
			if(MessageBox.Show("Are you sure?") == DialogResult.OK)
			{
				string statusInfo = null;
				foreach(ListViewItem lvi in listFiles.SelectedItems)
				{
					string fileToDelete = Path.Combine(lvi.SubItems[1].Text, lvi.SubItems[0].Text);

					try
					{
						if(lvi.SubItems.Count > 3 && lvi.SubItems[3].Text == "[Dir]")
						{
							if(useRecycler)
								FileSystem.DeleteDirectory(fileToDelete, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
							//MessageBox.Show("Recycle here!");
							else
								Directory.Delete(fileToDelete, true);
							listFiles.Items.Remove(lvi);
						}
						else
						{
							if(useRecycler)
								FileSystem.DeleteFile(fileToDelete, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
							//MessageBox.Show("Recycle here!");
							else
								System.IO.File.Delete(fileToDelete);
							listFiles.Items.Remove(lvi);
						}
					}
					catch(Exception exc)
					{
						QuickLog.ErrorException("Exception deleting files", exc);
						lvi.ForeColor = Color.Red;
						if(string.IsNullOrEmpty(statusInfo))
							statusInfo = "Exception deleting ";
						else
							statusInfo += ',';
						statusInfo += lvi.SubItems[0].Text;
					}
				}
				if(!string.IsNullOrEmpty(statusInfo))
					MessageBox.Show(statusInfo);
			}
		}

		private void comboBoxResultView_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch(comboBoxResultView.SelectedIndex)
			{
				case 0:	//Large Icons
					listFiles.View = View.LargeIcon;
					break;
				case 1:	//Small Icons
					listFiles.View = View.SmallIcon;
					break;
				case 2:	//List
					listFiles.View = View.List;
					break;
				case 3:	//Tile
					listFiles.View = View.Tile;
					break;
				case 4://Details
					listFiles.View = View.Details;
					break;
			}
		}

		private void AddPopUpMessage(string text, Color color)
		{
			if(!string.IsNullOrEmpty(text))
			{
				if(InvokeRequired)
				{
					Invoke(new MethodInvoker(delegate() { AddPopUpMessage(text, color); }));
				}
				else
				{
					if(!text.EndsWith("\n"))
						text += "\n";
					richTextBoxPopUp.SuspendLayout();
					int originalTextEnd = richTextBoxPopUp.Text.Length;
					richTextBoxPopUp.AppendText(text);
					richTextBoxPopUp.Select(originalTextEnd, text.Length);
					richTextBoxPopUp.SelectionColor = color;
					richTextBoxPopUp.SelectionLength = 0;
					richTextBoxPopUp.ScrollToCaret();
					richTextBoxPopUp.ResumeLayout();
					richTextBoxPopUp.Update();
					if(color == Color.DarkRed && toolStripSplitButtonLogPopUp.Image != jkhFileSearch.Properties.Resources.LogPopUpBad)
						toolStripSplitButtonLogPopUp.Image = jkhFileSearch.Properties.Resources.LogPopUpBad;
					if(color == Color.DarkRed && toolStripButtonLogging.Image != jkhFileSearch.Properties.Resources.LogPopUpBad)
						toolStripButtonLogging.Image = jkhFileSearch.Properties.Resources.LogPopUpBad;
				}
			}
		}

		private void toolStripSplitButtonLogPopUp_ButtonClick(object sender, EventArgs e)
		{
			if(richTextBoxPopUp.Visible)
				richTextBoxPopUp.Hide();
			else
				richTextBoxPopUp.Show();
		}

		private void richTextBoxPopUp_MouseMove(object sender, MouseEventArgs e)
		{
			if(_tracking)
			{
				Debug.WriteLine(string.Format("++{0},{1}", e.Location.X, e.Location.Y));

// 				richTextBoxPopUp.Height = richTextBoxPopUp.Top + e.Y;
// 				richTextBoxPopUp.Width = richTextBoxPopUp.Top + e.X;

				Size sizeTemp = new Size((Point)richTextBoxPopUp.Size);
				sizeTemp.Width -= e.Location.X;
				sizeTemp.Height -= e.Location.Y;
				richTextBoxPopUp.Size = sizeTemp;

				Point locTemp = new Point(richTextBoxPopUp.Location.X, richTextBoxPopUp.Location.Y);
				locTemp.X += e.Location.X;
				locTemp.Y += e.Location.Y;
				richTextBoxPopUp.Location = locTemp;
			}
			else
			{
				if(e.Location.X <= 4 && e.Location.Y <= 4)
				{
					Cursor = Cursors.SizeNWSE;
				}
				else
				{
					Cursor = Cursors.Default;
					Debug.WriteLine(string.Format("{0},{1}", e.Location.X, e.Location.Y));
				}
			}
		}

		bool _tracking = false;
		private void richTextBoxPopUp_MouseDown(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				if(e.Location.X <= 4 && e.Location.Y <= 4)
				{
					richTextBoxPopUp.Capture = true;
					Cursor = Cursors.SizeNWSE;
					_tracking = true;
				}				
			}
			else
			{
				Cursor = Cursors.Default;
				Debug.WriteLine(string.Format("{0},{1}", e.Location.X, e.Location.Y));
			}

		}

		private void richTextBoxPopUp_MouseUp(object sender, MouseEventArgs e)
		{
			_tracking = false;
		}

		private void toolStripButtonLogging_Click(object sender, EventArgs e)
		{
			if(richTextBoxPopUp.Visible)
				richTextBoxPopUp.Hide();
			else
				richTextBoxPopUp.Show();
		}
	}
}
