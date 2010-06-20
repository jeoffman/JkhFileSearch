using System;	//IntPtr
using System.Runtime.InteropServices;	//PreserveSig

namespace shell32
{
	public class Shell32Methods
	{
		public static Guid IID_IShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
		public static Guid IID_IContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");
		public static Guid IID_IContextMenu2 = new Guid("{000214f4-0000-0000-c000-000000000046}");
		public static Guid IID_IContextMenu3 = new Guid("{bcfce0a0-ec17-11d0-8d10-00a0c90f2719}");

		[DllImport("shell32.dll")]
		public static extern Int32 SHGetDesktopFolder(out IShellFolder ppshf2);

		[DllImport("shell32.dll")]
		public static extern Int32 SHGetDesktopFolder(out IntPtr ppshf);

		[DllImport("shell32.dll")]
		public static extern Int32 SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, [Out()] out IntPtr pidl, uint sfgaoIn, [Out()] out uint psfgaoOut);
	}
}
