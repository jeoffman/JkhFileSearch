using System;	//IntPtr
using System.Collections;	//Hashtable
//using System.Text;

using System.IO;	//Path
using System.Runtime.InteropServices;	//MarshalAs
using System.Windows.Forms;	//ImageList

namespace jkhFileSearch
{
	[StructLayout(LayoutKind.Sequential)]
	public struct SHFILEINFO
	{
		public IntPtr hIcon;
		public IntPtr iIcon;
		public uint dwAttributes;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	};

	public class shell32
	{
		public const uint SHGFI_LARGEICON = 0x0;					//Large icon
		public const uint SHGFI_SMALLICON = 0x1;					//Small icon
		public const uint SHGFI_SHELLICONSIZE = 0x000000004;     /* get shell size icon */
		public const uint SHGFI_PIDL = 0x000000008;     /* pszPath is a pidl */
		public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;	//
		public const uint SHGFI_ADDOVERLAYS = 0x000000020;
		public const uint SHGFI_OVERLAYINDEX = 0x000000040;
		public const uint SHGFI_ICON = 0x000000100;					//get icon
		public const uint SHGFI_DISPLAYNAME = 0x000000200;     /* get display name */
		public const uint SHGFI_TYPENAME = 0x000000400;     /* get type name */

		[DllImport("shell32.dll")]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
	}


	class FileTypeIconManager
	{
		private Hashtable _fileTypeIndecies = new Hashtable(StringComparer.CurrentCultureIgnoreCase);
		private const string _SECRET_MAGICAL_DIRECTORY_STRING = "!@#$MAGICALDIRECTORYSTRING%^&*()";
		ImageList il = new ImageList();

		public ImageList SmallImageList
		{
			get
			{
				return il;
			}
		}

		public void Clear()
		{
			il.Images.Clear();
			_fileTypeIndecies.Clear();
		}

		public int GetIconIndex(string filename)
		{
			int retval = -1;
			string ext = Path.GetExtension(filename);
			if(ext.Equals(".exe", StringComparison.CurrentCultureIgnoreCase) || ext.Equals(".lnk", StringComparison.CurrentCultureIgnoreCase))
			{
				SHFILEINFO shinfo = new SHFILEINFO();
				/*IntPtr hImgSmall =*/ shell32.SHGetFileInfo(filename, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), shell32.SHGFI_ICON | shell32.SHGFI_SMALLICON);
				System.Drawing.Icon tmp = System.Drawing.Icon.FromHandle(shinfo.hIcon);
				il.Images.Add(tmp);
				retval = il.Images.Count - 1;
				_fileTypeIndecies.Add(filename, retval);
			}
			else
			{
				if(!_fileTypeIndecies.ContainsKey(ext))
				{
					SHFILEINFO shinfo = new SHFILEINFO();
					/*IntPtr hImgSmall =*/ shell32.SHGetFileInfo(ext, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), shell32.SHGFI_ICON | shell32.SHGFI_SMALLICON | shell32.SHGFI_USEFILEATTRIBUTES);
					System.Drawing.Icon tmp = System.Drawing.Icon.FromHandle(shinfo.hIcon);
					il.Images.Add(tmp);
					retval = il.Images.Count - 1;
					_fileTypeIndecies.Add(ext, retval);
				}
				else
				{
					retval = (int)_fileTypeIndecies[ext];
				}
			}
			return retval;
		}

		public int GetIconIndexDir(string filename)
		{
			int retval = -1;

			if(!_fileTypeIndecies.ContainsKey(_SECRET_MAGICAL_DIRECTORY_STRING))
			{
				SHFILEINFO shinfo = new SHFILEINFO();
				/*IntPtr hImgSmall =*/ shell32.SHGetFileInfo(filename, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), shell32.SHGFI_ICON | shell32.SHGFI_SMALLICON);
				System.Drawing.Icon tmp = System.Drawing.Icon.FromHandle(shinfo.hIcon);
				il.Images.Add(tmp);
				retval = il.Images.Count - 1;
				_fileTypeIndecies.Add(_SECRET_MAGICAL_DIRECTORY_STRING, retval);
			}
			else
			{
				retval = (int)_fileTypeIndecies[_SECRET_MAGICAL_DIRECTORY_STRING];
			}
			return retval;
		}
	}
}
