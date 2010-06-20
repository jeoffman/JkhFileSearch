using System;	//IntPtr
using System.Diagnostics;	//Debug
using System.Runtime.InteropServices;	//StructLayout,MarshalAs
using System.IO;	//FileAttributes

namespace kernel32
{
	class FindFile
	{
		public const int MAX_PATH = 260;
		public const int MAX_ALTERNATE = 14;

		[StructLayout(LayoutKind.Sequential)]
		public struct FILETIME
		{
			public uint dwLowDateTime;
			public uint dwHighDateTime;
		};

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WIN32_FIND_DATA
		{
			public FileAttributes dwFileAttributes;
			public FILETIME ftCreationTime;
			public FILETIME ftLastAccessTime;
			public FILETIME ftLastWriteTime;
			public int nFileSizeHigh;
			public int nFileSizeLow;
			public int dwReserved0;
			public int dwReserved1;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
			public string cFileName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
			public string cAlternate;
		}

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32", CharSet = CharSet.Unicode)]
		public static extern bool FindClose(IntPtr hFindFile);

		public FileInfo[] GetFiles(string fileName, out WIN32_FIND_DATA lpFindFileData)
		{
			Debug.Assert(false);
			lpFindFileData = new WIN32_FIND_DATA();
			return null;
		}
		
		private long RecurseDirectory(string directory, int level, out int files, out int folders)
		{
			IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
			long size = 0;
			files = 0;
			folders = 0;
			WIN32_FIND_DATA findData;

			IntPtr findHandle;

			// please note that the following line won't work if you try this on a network folder, like \\Machine\C$
			// simply remove the \\?\ part in this case or use \\?\UNC\ prefix
			findHandle = FindFirstFile(@"\\?\" + directory + @"\*", out findData);
			if(findHandle != INVALID_HANDLE_VALUE)
			{

				do
				{
					if((findData.dwFileAttributes & FileAttributes.Directory) != 0)
					{

						if(findData.cFileName != "." && findData.cFileName != "..")
						{
							folders++;

							int subfiles, subfolders;
							string subdirectory = directory + (directory.EndsWith(@"\") ? "" : @"\") +
							findData.cFileName;
							if(level != 0)  // allows -1 to do complete search.
							{
								size += RecurseDirectory(subdirectory, level - 1, out subfiles, out subfolders);

								folders += subfolders;
								files += subfiles;
							}
						}
					}
					else
					{
						// File
						files++;

						size += (long)findData.nFileSizeLow + (long)findData.nFileSizeHigh * 4294967296;
					}
				} while(FindNextFile(findHandle, out findData));
				FindClose(findHandle);
			}
			return size;
		}
	}
}



