using System;	//IntPtr
using System.Runtime.InteropServices;	//PreserveSig

namespace user32
{
	[StructLayout(LayoutKind.Sequential)]
	public struct CMINVOKECOMMANDINFO
	{
		public int cbSize;
		public int fMask;
		public IntPtr hwnd;
		public IntPtr lpVerb;
		public IntPtr lpParameters;
		public IntPtr lpDirectory;
		public int nShow;
		public int dwHotKey;
		public IntPtr hIcon;
	}

	[Flags()]
	public enum TPM
	{
		TPM_LEFTBUTTON = 0x0000,
		TPM_RIGHTBUTTON = 0x0002,
		TPM_LEFTALIGN = 0x0000,
		TPM_CENTERALIGN = 0x0004,
		TPM_RIGHTALIGN = 0x0008,
		TPM_TOPALIGN = 0x0000,
		TPM_VCENTERALIGN = 0x0010,
		TPM_BOTTOMALIGN = 0x0020,
		TPM_HORIZONTAL = 0x0000,
		TPM_VERTICAL = 0x0040,
		TPM_RETURNCMD = 0x0100
	}

	public enum MF	//Menu Flags
	{
		MF_INSERT = 0x00000000,
		MF_CHANGE = 0x00000080,
		MF_APPEND = 0x00000100,
		MF_DELETE = 0x00000200,
		MF_REMOVE = 0x00001000,

		MF_BYCOMMAND = 0x00000000,
		MF_BYPOSITION = 0x00000400,

		MF_SEPARATOR = 0x00000800,

		MF_ENABLED = 0x00000000,
		MF_GRAYED = 0x00000001,
		MF_DISABLED = 0x00000002,

		MF_UNCHECKED = 0x00000000,
		MF_CHECKED = 0x00000008,
		MF_USECHECKBITMAPS = 0x00000200,

		MF_STRING = 0x00000000,
		MF_BITMAP = 0x00000004,
		MF_OWNERDRAW = 0x00000100,

		MF_POPUP = 0x00000010,
		MF_MENUBARBREAK = 0x00000020,
		MF_MENUBREAK = 0x00000040,

		MF_UNHILITE = 0x00000000,
		MF_HILITE = 0x00000080
	}


	class User32Methods
	{
		[DllImport("user32.dll")]
		public static extern UInt32 TrackPopupMenu(IntPtr hMenu, TPM uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

		[DllImport("USER32", EntryPoint = "InsertMenuW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
		public static extern int InsertMenu(IntPtr hMenu, int Position, MF Flags, int NewId, string text);
	}
}
