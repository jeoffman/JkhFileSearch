namespace jkhFileSearch
{
	partial class FormMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.label1 = new System.Windows.Forms.Label();
			this.buttonStart = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.comboLookIn = new System.Windows.Forms.ComboBox();
			this.checkIncludeSubdirs = new System.Windows.Forms.CheckBox();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.listFiles = new jkhFileSearch.FormMain.FlickerFreeListView();
			this.richTextBox = new System.Windows.Forms.RichTextBox();
			this.comboContainingText = new System.Windows.Forms.ComboBox();
			this.comboFileName = new System.Windows.Forms.ComboBox();
			this.buttonStop = new System.Windows.Forms.Button();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonStop = new System.Windows.Forms.ToolStripButton();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelBlankSpacer = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelDuration = new System.Windows.Forms.ToolStripStatusLabel();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBoxRegularExpressionFileName = new System.Windows.Forms.CheckBox();
			this.timerDurationDisplayUpdate = new System.Windows.Forms.Timer(this.components);
			this.numericUpDownMin = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownMax = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.dateTimePickerMin = new System.Windows.Forms.DateTimePicker();
			this.comboBoxDateSearchType = new System.Windows.Forms.ComboBox();
			this.dateTimePickerMax = new System.Windows.Forms.DateTimePicker();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMax)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "File &name:";
			// 
			// buttonStart
			// 
			this.buttonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonStart.Location = new System.Drawing.Point(752, 30);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new System.Drawing.Size(75, 23);
			this.buttonStart.TabIndex = 3;
			this.buttonStart.Text = "Start Search";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(1, 89);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Look in:";
			// 
			// comboLookIn
			// 
			this.comboLookIn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboLookIn.FormattingEnabled = true;
			this.comboLookIn.Location = new System.Drawing.Point(52, 86);
			this.comboLookIn.Name = "comboLookIn";
			this.comboLookIn.Size = new System.Drawing.Size(672, 21);
			this.comboLookIn.TabIndex = 4;
			// 
			// checkIncludeSubdirs
			// 
			this.checkIncludeSubdirs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIncludeSubdirs.AutoSize = true;
			this.checkIncludeSubdirs.Location = new System.Drawing.Point(730, 88);
			this.checkIncludeSubdirs.Name = "checkIncludeSubdirs";
			this.checkIncludeSubdirs.Size = new System.Drawing.Size(97, 17);
			this.checkIncludeSubdirs.TabIndex = 5;
			this.checkIncludeSubdirs.Text = "Include subdirs";
			this.checkIncludeSubdirs.UseVisualStyleBackColor = true;
			// 
			// splitContainer
			// 
			this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer.Location = new System.Drawing.Point(0, 136);
			this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.listFiles);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.richTextBox);
			this.splitContainer.Size = new System.Drawing.Size(835, 403);
			this.splitContainer.SplitterDistance = 316;
			this.splitContainer.TabIndex = 6;
			// 
			// listFiles
			// 
			this.listFiles.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listFiles.FullRowSelect = true;
			this.listFiles.HideSelection = false;
			this.listFiles.Location = new System.Drawing.Point(0, 0);
			this.listFiles.Margin = new System.Windows.Forms.Padding(0);
			this.listFiles.Name = "listFiles";
			this.listFiles.Size = new System.Drawing.Size(316, 403);
			this.listFiles.TabIndex = 0;
			this.listFiles.UseCompatibleStateImageBehavior = false;
			this.listFiles.View = System.Windows.Forms.View.Details;
			this.listFiles.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listFiles_MouseClick);
			this.listFiles.DoubleClick += new System.EventHandler(this.listFiles_MouseDoubleClick);
			this.listFiles.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listFiles_ItemSelectionChanged);
			this.listFiles.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listFiles_ItemDrag);
			// 
			// richTextBox
			// 
			this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richTextBox.Location = new System.Drawing.Point(0, 0);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.Size = new System.Drawing.Size(515, 403);
			this.richTextBox.TabIndex = 0;
			this.richTextBox.Text = "";
			this.richTextBox.WordWrap = false;
			// 
			// comboContainingText
			// 
			this.comboContainingText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboContainingText.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.comboContainingText.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.comboContainingText.FormattingEnabled = true;
			this.comboContainingText.Location = new System.Drawing.Point(78, 57);
			this.comboContainingText.Name = "comboContainingText";
			this.comboContainingText.Size = new System.Drawing.Size(668, 21);
			this.comboContainingText.TabIndex = 1;
			// 
			// comboFileName
			// 
			this.comboFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboFileName.FormattingEnabled = true;
			this.comboFileName.Location = new System.Drawing.Point(78, 32);
			this.comboFileName.Name = "comboFileName";
			this.comboFileName.Size = new System.Drawing.Size(601, 21);
			this.comboFileName.TabIndex = 0;
			this.comboFileName.TextChanged += new System.EventHandler(this.comboFileName_TextChanged);
			// 
			// buttonStop
			// 
			this.buttonStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonStop.Enabled = false;
			this.buttonStop.Location = new System.Drawing.Point(752, 55);
			this.buttonStop.Name = "buttonStop";
			this.buttonStop.Size = new System.Drawing.Size(75, 23);
			this.buttonStop.TabIndex = 3;
			this.buttonStop.Text = "Stop";
			this.buttonStop.UseVisualStyleBackColor = true;
			this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
			// 
			// toolStrip
			// 
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSettings,
            this.toolStripButtonStop});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(835, 25);
			this.toolStrip.TabIndex = 7;
			this.toolStrip.Text = "toolStrip";
			// 
			// toolStripButtonSettings
			// 
			this.toolStripButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSettings.Image")));
			this.toolStripButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonSettings.Name = "toolStripButtonSettings";
			this.toolStripButtonSettings.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonSettings.Text = "Settings";
			this.toolStripButtonSettings.Click += new System.EventHandler(this.toolStripButtonSettings_Click);
			// 
			// toolStripButtonStop
			// 
			this.toolStripButtonStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonStop.Enabled = false;
			this.toolStripButtonStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonStop.Image")));
			this.toolStripButtonStop.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonStop.Name = "toolStripButtonStop";
			this.toolStripButtonStop.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonStop.Text = "Stop search";
			this.toolStripButtonStop.Click += new System.EventHandler(this.toolStripButtonStop_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripStatusLabelBlankSpacer,
            this.toolStripStatusLabelDuration});
			this.statusStrip.Location = new System.Drawing.Point(0, 539);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(835, 22);
			this.statusStrip.TabIndex = 8;
			this.statusStrip.Text = "statusStrip";
			// 
			// toolStripStatusLabel
			// 
			this.toolStripStatusLabel.Name = "toolStripStatusLabel";
			this.toolStripStatusLabel.Size = new System.Drawing.Size(48, 17);
			this.toolStripStatusLabel.Text = "Waiting";
			// 
			// toolStripStatusLabelBlankSpacer
			// 
			this.toolStripStatusLabelBlankSpacer.Name = "toolStripStatusLabelBlankSpacer";
			this.toolStripStatusLabelBlankSpacer.Size = new System.Drawing.Size(723, 17);
			this.toolStripStatusLabelBlankSpacer.Spring = true;
			// 
			// toolStripStatusLabelDuration
			// 
			this.toolStripStatusLabelDuration.Name = "toolStripStatusLabelDuration";
			this.toolStripStatusLabelDuration.Size = new System.Drawing.Size(49, 17);
			this.toolStripStatusLabelDuration.Text = "00:00:00";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 60);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 13);
			this.label3.TabIndex = 1;
			this.label3.Text = "Containing:";
			// 
			// checkBoxRegularExpressionFileName
			// 
			this.checkBoxRegularExpressionFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxRegularExpressionFileName.AutoSize = true;
			this.checkBoxRegularExpressionFileName.Location = new System.Drawing.Point(685, 34);
			this.checkBoxRegularExpressionFileName.Name = "checkBoxRegularExpressionFileName";
			this.checkBoxRegularExpressionFileName.Size = new System.Drawing.Size(61, 17);
			this.checkBoxRegularExpressionFileName.TabIndex = 9;
			this.checkBoxRegularExpressionFileName.Text = "Reg Ex";
			this.checkBoxRegularExpressionFileName.UseVisualStyleBackColor = true;
			// 
			// timerDurationDisplayUpdate
			// 
			this.timerDurationDisplayUpdate.Interval = 1000;
			// 
			// numericUpDownMin
			// 
			this.numericUpDownMin.Location = new System.Drawing.Point(13, 112);
			this.numericUpDownMin.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
			this.numericUpDownMin.Name = "numericUpDownMin";
			this.numericUpDownMin.Size = new System.Drawing.Size(87, 20);
			this.numericUpDownMin.TabIndex = 10;
			this.numericUpDownMin.ThousandsSeparator = true;
			// 
			// numericUpDownMax
			// 
			this.numericUpDownMax.Location = new System.Drawing.Point(184, 112);
			this.numericUpDownMax.Maximum = new decimal(new int[] {
            0,
            1,
            0,
            0});
			this.numericUpDownMax.Name = "numericUpDownMax";
			this.numericUpDownMax.Size = new System.Drawing.Size(100, 20);
			this.numericUpDownMax.TabIndex = 10;
			this.numericUpDownMax.ThousandsSeparator = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(102, 116);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(79, 13);
			this.label4.TabIndex = 11;
			this.label4.Text = "≤ Size (bytes) ≤";
			// 
			// dateTimePickerMin
			// 
			this.dateTimePickerMin.CustomFormat = "";
			this.dateTimePickerMin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimePickerMin.Location = new System.Drawing.Point(320, 112);
			this.dateTimePickerMin.Name = "dateTimePickerMin";
			this.dateTimePickerMin.Size = new System.Drawing.Size(81, 20);
			this.dateTimePickerMin.TabIndex = 12;
			// 
			// comboBoxDateSearchType
			// 
			this.comboBoxDateSearchType.FormattingEnabled = true;
			this.comboBoxDateSearchType.Location = new System.Drawing.Point(419, 112);
			this.comboBoxDateSearchType.Name = "comboBoxDateSearchType";
			this.comboBoxDateSearchType.Size = new System.Drawing.Size(94, 21);
			this.comboBoxDateSearchType.TabIndex = 13;
			this.comboBoxDateSearchType.Text = "No Date";
			// 
			// dateTimePickerMax
			// 
			this.dateTimePickerMax.CustomFormat = "";
			this.dateTimePickerMax.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimePickerMax.Location = new System.Drawing.Point(530, 112);
			this.dateTimePickerMax.Name = "dateTimePickerMax";
			this.dateTimePickerMax.Size = new System.Drawing.Size(81, 20);
			this.dateTimePickerMax.TabIndex = 12;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(403, 116);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(13, 13);
			this.label5.TabIndex = 11;
			this.label5.Text = "≤";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(515, 116);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(13, 13);
			this.label6.TabIndex = 11;
			this.label6.Text = "≤";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(835, 561);
			this.Controls.Add(this.comboBoxDateSearchType);
			this.Controls.Add(this.dateTimePickerMax);
			this.Controls.Add(this.dateTimePickerMin);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.numericUpDownMax);
			this.Controls.Add(this.numericUpDownMin);
			this.Controls.Add(this.checkBoxRegularExpressionFileName);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.checkIncludeSubdirs);
			this.Controls.Add(this.comboFileName);
			this.Controls.Add(this.comboContainingText);
			this.Controls.Add(this.comboLookIn);
			this.Controls.Add(this.buttonStop);
			this.Controls.Add(this.buttonStart);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMain";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "JKH File Search";
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMax)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonStart;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboLookIn;
		private System.Windows.Forms.CheckBox checkIncludeSubdirs;
		private System.Windows.Forms.SplitContainer splitContainer;
		private FlickerFreeListView listFiles;
		private System.Windows.Forms.ComboBox comboContainingText;
		private System.Windows.Forms.ComboBox comboFileName;
		private System.Windows.Forms.Button buttonStop;
		private System.Windows.Forms.RichTextBox richTextBox;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton toolStripButtonSettings;
		private System.Windows.Forms.ToolStripButton toolStripButtonStop;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBoxRegularExpressionFileName;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelBlankSpacer;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDuration;
		private System.Windows.Forms.Timer timerDurationDisplayUpdate;
		private System.Windows.Forms.NumericUpDown numericUpDownMin;
		private System.Windows.Forms.NumericUpDown numericUpDownMax;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DateTimePicker dateTimePickerMin;
		private System.Windows.Forms.ComboBox comboBoxDateSearchType;
		private System.Windows.Forms.DateTimePicker dateTimePickerMax;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;

	}
}

