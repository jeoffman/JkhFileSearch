using System;	//IntPtr
using System.Runtime.InteropServices;	//StructLayout,MarshalAs
using System.Text;	//StringBuilder

namespace shell32
{
	/// <summary>QueryContextMenu shortcut menu changing flags</summary>
	[Flags]
	public enum CMF : uint
	{
		CMF_NORMAL = 0x00000000,
		CMF_DEFAULTONLY = 0x00000001,
		CMF_VERBSONLY = 0x00000002,
		CMF_EXPLORE = 0x00000004,
		CMF_NOVERBS = 0x00000008,
		CMF_CANRENAME = 0x00000010,
		CMF_NODEFAULT = 0x00000020,
		CMF_INCLUDESTATIC = 0x00000040,
		CMF_EXTENDEDVERBS = 0x00000100,
		CMF_RESERVED = 0xffff0000
	}

	/// <summary>CMINVOKECOMMANDINFOEX structure flags</summary>
	[Flags]
	public enum CMIC : uint
	{
		CMIC_MASK_HOTKEY = 0x00000020,
		CMIC_MASK_ICON = 0x00000010,
		CMIC_MASK_FLAG_NO_UI = 0x00000400,
		CMIC_MASK_UNICODE = 0x00004000,
		CMIC_MASK_NO_CONSOLE = 0x00008000,
		CMIC_MASK_ASYNCOK = 0x00100000,
		CMIC_MASK_NOZONECHECKS = 0x00800000,
		CMIC_MASK_SHIFT_DOWN = 0x10000000,
		CMIC_MASK_CONTROL_DOWN = 0x40000000,
		CMIC_MASK_FLAG_LOG_USAGE = 0x04000000,
		CMIC_MASK_PTINVOKE = 0x20000000
	}

	/// <summary>Show Window flags</summary>
	[Flags]
	public enum SW
	{
		SW_HIDE = 0,
		SW_SHOWNORMAL = 1,
		SW_NORMAL = 1,
		SW_SHOWMINIMIZED = 2,
		SW_SHOWMAXIMIZED = 3,
		SW_MAXIMIZE = 3,
		SW_SHOWNOACTIVATE = 4,
		SW_SHOW = 5,
		SW_MINIMIZE = 6,
		SW_SHOWMINNOACTIVE = 7,
		SW_SHOWNA = 8,
		SW_RESTORE = 9,
		SW_SHOWDEFAULT = 10,
	}


	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct POINT
	{
		public POINT(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public static POINT FromPoint(System.Drawing.Point pt)
		{
			return new POINT(pt.X, pt.Y);
		}

		public int x;
		public int y;
	}

	// Contains extended information about a shortcut menu command
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct CMINVOKECOMMANDINFOEX
	{
		public int cbSize;
		public CMIC fMask;
		public IntPtr hwnd;
		public IntPtr lpVerb;
		[MarshalAs(UnmanagedType.LPStr)]
		public string lpParameters;
		[MarshalAs(UnmanagedType.LPStr)]
		public string lpDirectory;
		public SW nShow;
		public int dwHotKey;
		public IntPtr hIcon;
		[MarshalAs(UnmanagedType.LPStr)]
		public string lpTitle;
		public IntPtr lpVerbW;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpParametersW;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpDirectoryW;
		[MarshalAs(UnmanagedType.LPWStr)]
		public string lpTitleW;
		public POINT ptInvoke;
	}

	[Flags]
	public enum GCS : uint
	{
		GCS_VERBA = 0,
		GCS_HELPTEXTA = 1,
		GCS_VALIDATEA = 2,
		GCS_VERBW = 4,
		GCS_HELPTEXTW = 5,
		GCS_VALIDATEW = 6
	}


	[ComImport()]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[GuidAttribute("000214e4-0000-0000-c000-000000000046")]
	public interface IContextMenu
	{
		// Adds commands to a shortcut menu
		[PreserveSig()]
		Int32 QueryContextMenu(IntPtr hmenu, uint iMenu, uint idCmdFirst, uint idCmdLast, CMF uFlags);

		// Carries out the command associated with a shortcut menu item
		[PreserveSig()]
		Int32 InvokeCommand(ref CMINVOKECOMMANDINFOEX info);

		// Retrieves information about a shortcut menu command, 
		// including the help string and the language-independent, 
		// or canonical, name for the command
		[PreserveSig()]
		Int32 GetCommandString(uint idcmd, GCS uflags, uint reserved, [MarshalAs(UnmanagedType.LPArray)] byte[] commandstring, int cch);
		[PreserveSig()]
		//Int32 GetCommandString(uint idcmd, GCS uflags, uint reserved, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder commandstring, int cch);
		Int32 GetCommandString(uint idcmd, GCS uflags, uint reserved, StringBuilder commandstring, int cch);
		[PreserveSig()]
		Int32 GetCommandString(uint idcmd, GCS uflags, uint reserved, [MarshalAs(UnmanagedType.LPWStr)] string commandstring, int cch);

	}

	[ComImport, Guid("000214f4-0000-0000-c000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IContextMenu2
	{
		// Adds commands to a shortcut menu
		[PreserveSig()]
		Int32 QueryContextMenu(IntPtr hmenu, uint iMenu, uint idCmdFirst, uint idCmdLast, CMF uFlags);

		// Carries out the command associated with a shortcut menu item
		[PreserveSig()]
		Int32 InvokeCommand(ref CMINVOKECOMMANDINFOEX info);

		// Retrieves information about a shortcut menu command, 
		// including the help string and the language-independent, 
		// or canonical, name for the command
		[PreserveSig()]
		Int32 GetCommandString(uint idcmd, GCS uflags, uint reserved, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder commandstring, int cch);

		// Allows client objects of the IContextMenu interface to 
		// handle messages associated with owner-drawn menu items
		[PreserveSig]
		Int32 HandleMenuMsg(uint uMsg, IntPtr wParam, IntPtr lParam);
	}

	[ComImport, Guid("bcfce0a0-ec17-11d0-8d10-00a0c90f2719")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IContextMenu3
	{
		// Adds commands to a shortcut menu
		[PreserveSig()]
		Int32 QueryContextMenu(IntPtr hmenu, uint iMenu, uint idCmdFirst, uint idCmdLast, CMF uFlags);

		// Carries out the command associated with a shortcut menu item
		[PreserveSig()]
		Int32 InvokeCommand(ref CMINVOKECOMMANDINFOEX info);

		// Retrieves information about a shortcut menu command, 
		// including the help string and the language-independent, 
		// or canonical, name for the command
		[PreserveSig()]
		Int32 GetCommandString(uint idcmd, GCS uflags, uint reserved, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder commandstring, int cch);

		// Allows client objects of the IContextMenu interface to 
		// handle messages associated with owner-drawn menu items
		[PreserveSig]
		Int32 HandleMenuMsg(uint uMsg, IntPtr wParam, IntPtr lParam);

		// Allows client objects of the IContextMenu3 interface to 
		// handle messages associated with owner-drawn menu items
		[PreserveSig]
		Int32 HandleMenuMsg2(uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr plResult);
	}
}
