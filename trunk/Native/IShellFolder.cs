using System;	//IntPtr
using System.Runtime.InteropServices;	//StructLayout,MarshalAs
using System.Text;	//StringBuilder

namespace shell32
{
	[Flags()]
	public enum SHCONTF
	{
		SHCONTF_FOLDERS = 0x0020,
		SHCONTF_NONFOLDERS = 0x0040,
		SHCONTF_INCLUDEHIDDEN = 0x0080,
		SHCONTF_INIT_ON_FIRST_NEXT = 0x0100,
		SHCONTF_NETPRINTERSRCH = 0x0200,
		SHCONTF_SHAREABLE = 0x0400,
		SHCONTF_STORAGE = 0x0800
	}

	[Flags()]
	public enum SFGAO : uint
	{
		SFGAO_CANCOPY = 0x000000001,
		SFGAO_CANMOVE = 0x000000002,
		SFGAO_CANLINK = 0x000000004,
		SFGAO_STORAGE = 0x000000008,
		SFGAO_CANRENAME = 0x00000010,
		SFGAO_CANDELETE = 0x00000020,
		SFGAO_HASPROPSHEET = 0x00000040,
		SFGAO_DROPTARGET = 0x00000100,
		SFGAO_CAPABILITYMASK = 0x00000177,
		SFGAO_ENCRYPTED = 0x00002000,
		SFGAO_ISSLOW = 0x00004000,
		SFGAO_GHOSTED = 0x00008000,
		SFGAO_LINK = 0x00010000,
		SFGAO_SHARE = 0x00020000,
		SFGAO_READONLY = 0x00040000,
		SFGAO_HIDDEN = 0x00080000,
		SFGAO_DISPLAYATTRMASK = 0x000FC000,
		SFGAO_FILESYSANCESTOR = 0x10000000,
		SFGAO_FOLDER = 0x20000000,
		SFGAO_FILESYSTEM = 0x40000000,
		SFGAO_HASSUBFOLDER = 0x80000000,
		SFGAO_CONTENTSMASK = 0x80000000,
		SFGAO_VALIDATE = 0x01000000,
		SFGAO_REMOVABLE = 0x02000000,
		SFGAO_COMPRESSED = 0x04000000,
		SFGAO_BROWSABLE = 0x08000000,
		SFGAO_NONENUMERATED = 0x00100000,
		SFGAO_NEWCONTENT = 0x00200000,
		SFGAO_CANMONIKER = 0x00400000,
		SFGAO_HASSTORAGE = 0x00400000,
		SFGAO_STREAM = 0x00400000,
		SFGAO_STORAGEANCESTOR = 0x00800000,
		SFGAO_STORAGECAPMASK = 0x70C50008
	}

	[Flags()]
	public enum SHGDN
	{
		SHGDN_NORMAL = 0,
		SHGDN_INFOLDER = 1,
		SHGDN_FORADDRESSBAR = 16384,
		SHGDN_FORPARSING = 32768
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct STRRET
	{
		[FieldOffset(0)]
		UInt32 uType;
		[FieldOffset(4)]
		IntPtr pOleStr;
		[FieldOffset(4)]
		IntPtr pStr;
		[FieldOffset(4)]
		UInt32 uOffset;
		[FieldOffset(4)]
		IntPtr cStr;
	}


	[ComImportAttribute()]
	[GuidAttribute("000214E6-0000-0000-C000-000000000046")]
	[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IShellFolder
	{
		[PreserveSig]
		int ParseDisplayName(IntPtr hwndOwner, IntPtr pbcReserved, [MarshalAs(UnmanagedType.LPWStr)] string lpszDisplayName, ref int pchEaten, out IntPtr ppidl, out int pdwAttributes);
		[PreserveSig]
		int EnumObjects(IntPtr hwndOwner, [MarshalAs(UnmanagedType.U4)] SHCONTF grfFlags, ref IEnumIDList ppenumIDList);
		[PreserveSig]
		int BindToObject(IntPtr pidl, IntPtr pbcReserved, ref Guid riid, ref IShellFolder ppvOut);
		[PreserveSig]
		int BindToStorage(IntPtr pidl, IntPtr pbcReserved, ref Guid riid, IntPtr ppvObj);
		[PreserveSig]
		int CompareIDs(IntPtr lParam, IntPtr pidl1, IntPtr pidl2);
		[PreserveSig]
		int CreateViewObject(IntPtr hwndOwner, ref Guid riid, IntPtr ppvOut);
		[PreserveSig]
		int GetAttributesOf(int cidl, IntPtr apidl, [MarshalAs(UnmanagedType.U4)] ref SFGAO rgfInOut);
		[PreserveSig]
		int GetUIObjectOf(IntPtr hwndOwner, int cidl, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, ref Guid riid, int prgfInOut, out IUnknown ppvOut);
		[PreserveSig]
		int GetDisplayNameOf(IntPtr pidl, [MarshalAs(UnmanagedType.U4)] SHGDN uFlags, ref STRRET lpName);
		[PreserveSig]
		int SetNameOf(IntPtr hwndOwner, IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] string lpszName, [MarshalAs(UnmanagedType.U4)] SHCONTF uFlags, ref IntPtr ppidlOut);
	};
}
