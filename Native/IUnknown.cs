using System;	//IntPtr
using System.Runtime.InteropServices;	//StructLayout,MarshalAs

namespace shell32
{
	[ComImport, Guid("00000000-0000-0000-C000-000000000046")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IUnknown
	{
		[PreserveSig]
		IntPtr QueryInterface(ref Guid riid, out IntPtr pVoid);
		[PreserveSig]
		IntPtr AddRef();
		[PreserveSig]
		IntPtr Release();
	}
}
