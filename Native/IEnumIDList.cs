using System;	//IntPtr
using System.Runtime.InteropServices;	//PreserveSig

namespace shell32
{
	[ComImportAttribute()]
	[GuidAttribute("000214F2-0000-0000-C000-000000000046")]
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IEnumIDList
	{
		[PreserveSig]
		int Next(int celt, ref IntPtr rgelt, out int pceltFetched);
		[PreserveSig]
		int Skip(int celt);
		[PreserveSig]
		int Reset();
		[PreserveSig]
		int Clone(ref IEnumIDList ppenum);
	}
}
