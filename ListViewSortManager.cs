////////////////////////////////////////////////////////////////////////////////
// Slightly modified from CodeProject article "Adding Multicolumn-sorting to 
//	ListViewSortManager" By Luis Alonso Ramos
// licensed under the CPOL by Luis in his message from 11:10 7 Apr '08

///////////////////////////////////////////////////////////////////////////////
// Intelectix Utility Framework
//
// Copyright (c) 2005-07, Intelectix, S.A. de C.V.  All rights reserved.
// Portions Copyright (c) 2002-04 by Eddie Velasquez
// This file is Intelectix confidential. Do not distribute.
//
// Created: Monday, April 22, 2002 by Eddie Velasquez
// Version 2.0 July 23, 2005 by Luis Alonso Ramos
//

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;

namespace ListView_SortManager
{
    #region ListViewSortManager

    /// <summary>
    /// Provides easy multiple-column sorting for <c>ListView</c> controls.
    /// </summary>
    public class ListViewSortManager
    {
        #region Constructors

        /// <summary>
        /// Creates a ListViewSortManager object, initially unsorted.
        /// </summary>
        /// <param name="list">ListView that this manager will provide
        /// sorting to.</param>
        /// <param name="comparers">Array of Types of comparers (one for each
        /// column).</param>
        public ListViewSortManager(ListView list, Type[] comparers)
            :
            this(list, comparers, -1, SortOrder.None)
        {
        }

        /// <summary>
        /// Constructor. Creates a ListViewSortManager object with initial
        /// sorting on one column.
        /// </summary>
        /// <param name="list">ListView that this manager will provide
        /// sorting to.</param>
        /// <param name="comparers">Array of Types of comparers (one for each
        /// column).</param>
        /// <param name="column">Initial column to sort.</param>
        /// <param name="order">Initial sort order.</param>
        public ListViewSortManager(ListView list, Type[] comparers, int column,
            SortOrder order)
            : this(list, comparers, new int[] { column }, new SortOrder[] { order })
        {
        }

        /// <summary>
        /// Constructor. Creates a ListViewSortManager object with initial
        /// sorting on multiple columns.
        /// </summary>
        /// <param name="list">ListView that this manager will provide
        /// sorting to.</param>
        /// <param name="comparers">Array of Types of comparers (one for each
        /// column).</param>
        /// <param name="columns">Array of columns to sort by.</param>
        /// <param name="orders">Array of SortOrders to sort by (must be of the
        /// same length than <c>columns</c>.</param>
        public ListViewSortManager(ListView list, Type[] comparers, int[] columns,
            SortOrder[] orders)
        {
            if(list == null)
            {
                throw new ArgumentNullException("list",
                    "list parameter must be a valid ListView object.");
            }

            if(columns.Length != orders.Length)
            {
                throw new ArgumentException("columns, orders",
                    "The length of the columns and orders array must be the same.");
            }

            listView = list;
            comparerTypes = comparers;

            foreach(Type type in comparers)
            {
                this.comparers.Add((IListViewSorter)Activator.CreateInstance(type));
            }

            if(!nativeArrows)
            {
                SetHeaderImageList(listView, headerImageList);
            }

            list.ColumnClick += new ColumnClickEventHandler(listView_ColumnClick);

            for(int i = 0; i < columns.Length; i++)
            {
                int column = columns[i];

                if(column != -1)
                {
                    if(orders[i] == SortOrder.Ascending)
                        sortColumns.Add(column + 1);
                    else if(orders[i] == SortOrder.Descending)
                        sortColumns.Add(-(column + 1));
                }
            }

            Sort();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether list sorting is enabled.
        /// </summary>
        /// <value>true to enable sorting, false to disable.</value>
        /// <remarks>Disable sorting to speed up big updates to the list
        /// view contents, so the list view is not resorted everytime a new
        /// item is added.</remarks>
        public bool SortEnabled
        {
            get 
            { 
                return sortEnabled; 
            }
            set
            {
                // Only enable/disable the sorting once, even if the property
                // is set several times in a row.
                if(value)
                {
                    if(!sortEnabled)
                    {
                        listView.ColumnClick +=
                            new ColumnClickEventHandler(listView_ColumnClick);
                        Sort();
                    }
                }
                else
                {
                    if(sortEnabled)
                    {
                        listView.ColumnClick -=
                            new ColumnClickEventHandler(listView_ColumnClick);
                        listView.ListViewItemSorter = null;

                        // Sorting disabled? remove all the header arrows
                        for(int i = 0; i < listView.Columns.Count; i++)
                            ShowHeaderIcon(listView, i, SortOrder.None);
                    }
                }

                sortEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the first column on which the list view will
        /// be sorted.
        /// </summary>
        /// <value>A 0-based index for the first column to sort by, or -1
        /// if no column will be sorted.</value>
        /// <exception cref="ArgumentOutOfRangeException">Column index is
        /// greater than the number of columns in the list view.</exception>
        public int Column
        {
            get
            {
                if(sortColumns.Count == 0)
                    return -1;

                return Math.Abs((int) sortColumns[0]) - 1;
            }
            set
            {
                // Get current sort order, and the clear the columns
                SortOrder sortOrder = SortOrder;
                sortColumns.Clear();
                
                if(value >= 0)
                {
                    if(value > listView.Columns.Count - 1)
                        throw new ArgumentOutOfRangeException("Column property",
                            value, "Column index greater than number of columns in ListView.");

                    if(sortOrder == SortOrder.Descending)
                        sortColumns.Add(-(value + 1));
                    else
                        sortColumns.Add(value + 1);
                }

                Sort();
            }
        }

        /// <summary>
        /// Gets or sets the sort order for the first column.
        /// </summary>
        /// <value>One of the values in the SortOrder enum.</value>
        /// <remarks>If Column is -1, setting this property will have
        /// no effect.</remarks>
        public SortOrder SortOrder
        {
            get
            {
                if(sortColumns.Count == 0)
                    return SortOrder.None;

                if(Math.Sign((int) sortColumns[0]) > 0)
                    return SortOrder.Ascending;
                else
                    return SortOrder.Descending;
            }
            set
            {
                if(value != SortOrder.None)
                {
                    int column = Column;
                    if(column == -1)
                        return;

                    sortColumns.RemoveAt(0);
                    sortColumns.Insert(0, value == SortOrder.Ascending ?
                        (column + 1) : -(column + 1));
                }
                else
                {
                    // No sorting? clear the column sequence
                    sortColumns.Clear();                    
                }

                Sort();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the sorting of the list view, based on selected columns.
        /// </summary>
        public void Sort()
        {
            // Display the arrow for each column according to its order in
            // the current sequence
            for(int i = 0; i < listView.Columns.Count; i++)
            {
                if(sortColumns.Contains(i + 1))
                    ShowHeaderIcon(listView, i, SortOrder.Ascending);
                else if(sortColumns.Contains(-(i + 1)))
                    ShowHeaderIcon(listView, i, SortOrder.Descending);
                else
                    ShowHeaderIcon(listView, i, SortOrder.None);

            }

            if(sortColumns.Count > 0)
            {
                // A new sorting object should be created, since the column
                // ArrayList is converted to a regular array in the
                // constructor, in order to save unboxing time

                listView.ListViewItemSorter =
                    new ListViewSorter(sortColumns, comparers);
            }
            else
            {
                listView.ListViewItemSorter = null;
            }
        }

        /// <summary>
        /// Returns the type of the comparer for the given column.
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <returns></returns>
        public Type GetColumnComparerType(int column)
        {
            return comparerTypes[column];
        }
        
        /// <summary>
        /// Sets the type of the comparer for the given column.
        /// </summary>
        /// <param name="column">Column index.</param>
        /// <param name="comparerType">Comparer type.</param>
        public void SetColumnComparerType(int column, Type comparerType)
        {
            comparerTypes[column] = comparerType;
            
            // Create the new comparer object
            comparers.RemoveAt(column);
            comparers.Insert(column,
                (IListViewSorter) Activator.CreateInstance(comparerType));
        }

        /// <summary>
        /// Reassigns the comparer types for all the columns
        /// </summary>
        /// <param name="comparers">Array of Types of comparers (one for
        /// each column)</param>
        public void SetComparerTypes(Type[] comparers)
        {
            comparerTypes = comparers;

            // Create new comparer objects for every column
            this.comparers.Clear();
            foreach(Type type in comparers)
            {
                this.comparers.Add((IListViewSorter) Activator.CreateInstance(type));
            }
        }

        #endregion

        #region Implementation

        private ListView  listView;
        private Type[]    comparerTypes;

        private bool      sortEnabled = true;
        private ArrayList sortColumns = new ArrayList();
        private ArrayList comparers   = new ArrayList();

        /// <summary>
        /// Handles ColumnClick for ListView control.
        /// </summary>
        /// <param name="sender">Object raising the event.</param>
        /// <param name="e">Event arguments.</param>
        private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if((Control.ModifierKeys & Keys.Control) > 0)
            {
                // If Ctrl key is down, add this column to the sequence
                AddToSequence(e.Column);
            }
            else
            {
                // If only once column is in the sequence, and it is this colum,
                // don't clear the sequence, but toggle the order
                if(!(sortColumns.Count == 1 &&
                    FindInSequence(e.Column) != SortOrder.None))
                    sortColumns.Clear();

                AddToSequence(e.Column);
            }

            Sort();
        }

        /// <summary>
        /// Finds if a specified column is in the sort sequence.
        /// </summary>
        /// <param name="column">0-based index of column.</param>
        /// <returns>Whether the column is in the sequence, and its sorting
        /// order.</returns>
        private SortOrder FindInSequence(int column)
        {
            if(sortColumns.Contains(column + 1))
                return SortOrder.Ascending;
            else if(sortColumns.Contains(-(column + 1)))
                return SortOrder.Descending;
            else
                return SortOrder.None;
        }

        /// <summary>
        /// Adds a column to the sequence.
        /// </summary>
        /// <param name="column"></param>
        private void AddToSequence(int column)
        {
            if(sortColumns.Count > 0 &&
                Math.Abs((int) sortColumns[sortColumns.Count - 1]) == column + 1)
            {
                // The column was clicked twice or more in a row, toggle its
                // order
                if(sortColumns.Contains(column + 1))
                {
                    sortColumns.Remove(column + 1);
                    sortColumns.Add(-(column + 1));
                }
                else if(sortColumns.Contains(-(column + 1)))
                {
                    sortColumns.Remove(-(column + 1));
                    sortColumns.Add(column + 1);
                }
            }
            else if(sortColumns.Contains(column + 1))
            {
                // The column was clicked previously. remove it from the
                // sequence and add it at the end, in ascending order
                sortColumns.Remove(column + 1);
                sortColumns.Add(column + 1);
            }
            else if(sortColumns.Contains(-(column + 1)))
            {
                // The column was clicked previously. remove it from the
                // sequence and add it at the end, in ascending order
                sortColumns.Remove(-(column + 1));
                sortColumns.Add(column + 1);
            }
            else
            {
                // Column was not in the sequence, add it in ascending order
                sortColumns.Add(column + 1);
            }
        }

        #region Graphics

        private static bool nativeArrows = ComCtlDllSupportsArrows();
        private static ImageList headerImageList = InitImageList();

        /// <summary>
        /// Contains items for each type of arrow a header can have.
        /// </summary>
        private enum ArrowType
        {
            Ascending,
            Descending
        }

        /// <summary>
        /// Creates the image list, if native arrows are not suppoerted.
        /// </summary>
        /// <returns>A new ImageList object, or null if native arrows are
        /// supported.</returns>
        private static ImageList InitImageList()
        {
            if(!nativeArrows)
            {
                headerImageList = new ImageList();
                headerImageList.ImageSize = new Size(8, 8);
                headerImageList.TransparentColor = Color.Magenta;

                headerImageList.Images.Add(GetArrowBitmap(ArrowType.Ascending));
                headerImageList.Images.Add(GetArrowBitmap(ArrowType.Descending));
            }

            return headerImageList;
        }

        /// <summary>
        /// Draws an arrow on a bitmap.
        /// </summary>
        /// <param name="type">Type of arrow to </param>
        /// <returns></returns>
        private static Bitmap GetArrowBitmap(ArrowType type)
        {
            Bitmap bmp = new Bitmap(8, 8);
            Graphics gfx = Graphics.FromImage(bmp);

            Pen lightPen = SystemPens.ControlLightLight;
            Pen shadowPen = SystemPens.ControlDark;

            gfx.FillRectangle(Brushes.Magenta, 0, 0, 8, 8);

            if(type == ArrowType.Ascending)     
            {
                gfx.DrawLine(lightPen, 0, 7, 7, 7);
                gfx.DrawLine(lightPen, 7, 7, 4, 0);
                gfx.DrawLine(shadowPen, 3, 0, 0, 7);
            }

            else if(type == ArrowType.Descending)
            {
                gfx.DrawLine(lightPen, 4, 7, 7, 0);
                gfx.DrawLine(shadowPen, 3, 7, 0, 0);
                gfx.DrawLine(shadowPen, 0, 0, 7, 0);
            }
            
            gfx.Dispose();

            return bmp;
        }

        /// <summary>
        /// Shows the arrow on the specified header, according to the
        /// sorting order.
        /// </summary>
        /// <param name="list">ListView control.</param>
        /// <param name="columnIndex">Index of column.</param>
        /// <param name="sortOrder">Sorting order. SortOrder.None to hide the
        /// arrow.</param>
        private static void ShowHeaderIcon(ListView list, int columnIndex, SortOrder sortOrder)
        {
            if(columnIndex < 0 || columnIndex >= list.Columns.Count)
                return;

            IntPtr hHeader = NativeMethods.SendMessage(list.Handle,
                NativeMethods.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

            ColumnHeader colHdr = list.Columns[columnIndex];

            NativeMethods.HDITEM hd = new NativeMethods.HDITEM();
            hd.mask = NativeMethods.HDI_FORMAT;

            HorizontalAlignment align = colHdr.TextAlign;

            if(align == HorizontalAlignment.Left)
                hd.fmt = NativeMethods.HDF_LEFT | NativeMethods.HDF_STRING | 
                    NativeMethods.HDF_BITMAP_ON_RIGHT;
            else if(align == HorizontalAlignment.Center)
                hd.fmt = NativeMethods.HDF_CENTER | NativeMethods.HDF_STRING |
                    NativeMethods.HDF_BITMAP_ON_RIGHT;
            else
                hd.fmt = NativeMethods.HDF_RIGHT | NativeMethods. HDF_STRING;

            if(nativeArrows)
            {
                if(sortOrder == SortOrder.Ascending)
                    hd.fmt |= NativeMethods.HDF_SORTUP;
                else if(sortOrder == SortOrder.Descending)
                    hd.fmt |= NativeMethods.HDF_SORTDOWN;
            }
            else
            {
                hd.mask |= NativeMethods.HDI_IMAGE;

                if(sortOrder != SortOrder.None)
                    hd.fmt |= NativeMethods.HDF_IMAGE;

                hd.iImage = (int) sortOrder - 1;
            }

            NativeMethods.SendMessage2(hHeader, NativeMethods.HDM_SETITEM,
                new IntPtr(columnIndex), ref hd);
        }

        /// <summary>
        /// Uses P/Invoke to set the image list to the header of a list view.
        /// </summary>
        /// <param name="list">List view control.</param>
        /// <param name="imgList">Image list.</param>
        static private void SetHeaderImageList(ListView list, ImageList imgList)
        {
            IntPtr hHeader = NativeMethods.SendMessage(list.Handle,
                NativeMethods.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            NativeMethods.SendMessage(hHeader, NativeMethods.HDM_SETIMAGELIST,
                IntPtr.Zero, imgList.Handle);
        }

        /// <summary>
        /// Finds out if COMCTL32.DLL version is new enough to support native
        /// arrows in headers.
        /// </summary>
        /// <returns>true if arrows are supported, false otherwise.</returns>
        static private bool ComCtlDllSupportsArrows()
        {
            IntPtr hModule = IntPtr.Zero;

            try
            {
                hModule = NativeMethods.LoadLibrary("comctl32.dll");
                if(hModule != IntPtr.Zero)
                {
                    IntPtr proc = NativeMethods.GetProcAddress(hModule, "DllGetVersion");
                    if(proc == IntPtr.Zero)
                        return false;
                }

                NativeMethods.DLLVERSIONINFO vi = new NativeMethods.DLLVERSIONINFO();
                vi.cbSize = Marshal.SizeOf(typeof(NativeMethods.DLLVERSIONINFO));

                NativeMethods.DllGetVersion(ref vi);

                return vi.dwMajorVersion >= 6;
            }
            finally
            {
                if(hModule != IntPtr.Zero)
                    NativeMethods.FreeLibrary(hModule);
            }
        }

        #endregion

        #region Native methods

        /// <summary>
        /// Contains native methods.
        /// </summary>
        internal class NativeMethods
        {
            /// <summary>
            /// Private constructor to prevent instantiation of this class.
            /// </summary>
            private NativeMethods()
            {
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct DLLVERSIONINFO
            {
                public int cbSize;
                public int dwMajorVersion;
                public int dwMinorVersion;
                public int dwBuildNumber;
                public int dwPlatformID;
            }

            [DllImport("kernel32.dll")]
            internal static extern IntPtr LoadLibrary(string fileName);

            [DllImport("kernel32.dll", CharSet=CharSet.Ansi, ExactSpelling=true)]
            internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("comctl32.dll")]
            internal static extern int DllGetVersion(ref DLLVERSIONINFO pdvi);

            [StructLayout(LayoutKind.Sequential)]
            internal struct HDITEM 
            {
                public Int32     mask; 
                public Int32     cxy;   
                [MarshalAs(UnmanagedType.LPTStr)] 
                public String    pszText; 
                public IntPtr    hbm; 
                public Int32     cchTextMax; 
                public Int32     fmt; 
                public Int32     lParam; 
                public Int32     iImage;
                public Int32     iOrder;
            };

            [DllImport("user32")]
            internal static extern IntPtr SendMessage(IntPtr Handle, Int32 msg,
                IntPtr wParam, IntPtr lParam);

            [DllImport("user32", EntryPoint="SendMessage")]
            internal static extern IntPtr SendMessage2(IntPtr Handle, Int32 msg,
                IntPtr wParam, ref HDITEM lParam);

            internal const Int32 HDI_WIDTH          = 0x0001;
            internal const Int32 HDI_HEIGHT         = HDI_WIDTH;
            internal const Int32 HDI_TEXT           = 0x0002;
            internal const Int32 HDI_FORMAT         = 0x0004;
            internal const Int32 HDI_LPARAM         = 0x0008;
            internal const Int32 HDI_BITMAP         = 0x0010;
            internal const Int32 HDI_IMAGE          = 0x0020;
            internal const Int32 HDI_DI_SETITEM     = 0x0040;
            internal const Int32 HDI_ORDER          = 0x0080;
            internal const Int32 HDI_FILTER         = 0x0100;       // 0x0500

            internal const Int32 HDF_LEFT           = 0x0000;
            internal const Int32 HDF_RIGHT          = 0x0001;
            internal const Int32 HDF_CENTER         = 0x0002;
            internal const Int32 HDF_JUSTIFYMASK        = 0x0003;
            internal const Int32 HDF_RTLREADING     = 0x0004;
            internal const Int32 HDF_OWNERDRAW      = 0x8000;
            internal const Int32 HDF_STRING         = 0x4000;
            internal const Int32 HDF_BITMAP         = 0x2000;
            internal const Int32 HDF_BITMAP_ON_RIGHT = 0x1000;
            internal const Int32 HDF_IMAGE          = 0x0800;
            internal const Int32 HDF_SORTUP         = 0x0400;       // 0x0501
            internal const Int32 HDF_SORTDOWN       = 0x0200;       // 0x0501

            internal const Int32 LVM_FIRST           = 0x1000;      // List messages
            internal const Int32 LVM_GETHEADER      = LVM_FIRST + 31;

            internal const Int32 HDM_FIRST          = 0x1200;      // Header messages
            internal const Int32 HDM_SETIMAGELIST   = HDM_FIRST + 8;
            internal const Int32 HDM_GETIMAGELIST   = HDM_FIRST + 9;
            internal const Int32 HDM_GETITEM        = HDM_FIRST + 11;
            internal const Int32 HDM_SETITEM        = HDM_FIRST + 12;
        }

        #endregion

        #endregion
    }

    #endregion
    
    #region ListViewSorter

    /// <summary>
    /// Implements the IComparer interface to perform sorting of ListViewItems.
    /// </summary>
    internal class ListViewSorter : IComparer
    {
        /// <summary>
        /// Constructor. Initializes a ListViewSorter object..
        /// </summary>
        /// <param name="sortColumns">Contains the order in which columns are
        /// to be sorted.</param>
        /// <param name="comparers">Contains a comparer object for each
        /// column.</param>
        public ListViewSorter(ArrayList sortColumns, ArrayList comparers)
        {
            this.sortColumns = (int[]) sortColumns.ToArray(typeof(int));
            this.comparers = comparers;
        }

        /// <summary>
        /// Compares two ListViewItem objects according to the order in which
        /// the user clicked on the columns.
        /// </summary>
        /// <param name="lhs">First item to compare.</param>
        /// <param name="rhs">Second item to compare.</param>
        /// <returns>-1 to indicate that the first item should go first, 1 if
        /// the second item goes first, 0 if they are equal.</returns>
        public int Compare(Object lhs, Object rhs)
        {
            ListViewItem lhsLvi = lhs as ListViewItem;
            ListViewItem rhsLvi = rhs as ListViewItem;

            // Check for total row, and always sort it at the end
            if(lhs is ListViewBottomItem)
                return 1;
            else if(rhs is ListViewBottomItem)
                return -1;

            // We only know how to sort ListViewItems, so return equal
            if(lhsLvi == null || rhsLvi == null)
                return 0;

            ListViewItem.ListViewSubItemCollection lhsItems = lhsLvi.SubItems;
            ListViewItem.ListViewSubItemCollection rhsItems = rhsLvi.SubItems;

            foreach(int i in sortColumns)
            {
                int column = Math.Abs(i) - 1;

                string lhsText = (lhsItems.Count > column) ?
                    lhsItems[column].Text : String.Empty;
                string rhsText = (rhsItems.Count > column) ?
                    rhsItems[column].Text : String.Empty;

                int result = 0;
                if(lhsText.Length == 0 || rhsText.Length == 0)
                    result = lhsText.CompareTo(rhsText);
                else
                    result = ((IListViewSorter) comparers[column]).Compare(lhsText, rhsText);

                if(result != 0)
                    return result * Math.Sign(i);
            }

            return 0;
        }

        private int[] sortColumns;
        private ArrayList comparers;
    }

    #endregion

    #region IListViewSorter

    /// <summary>
    /// Interface for different sorting classes to derive from.
    /// </summary>
    public interface IListViewSorter
    {
        /// <summary>
        /// Compares two string values. Derived class must convert the
        /// string to the type it sorts.
        /// </summary>
        /// <param name="lhs">Value 1.</param>
        /// <param name="rhs">Value 2.</param>
        /// <returns>-1 if <c>lhs</c> is should go first, 1 if <c>rhs</c> should
        /// go first, 0 if both numbers are equal.</returns>
        int Compare(string lhs, string rhs);
    }

    #endregion

    #region Comparers

    /// <summary>
    /// Provides text sorting (case sensitive) for the ListViewSortManager
    /// class.
    /// </summary>
    public class ListViewTextSort : IListViewSorter
    {
        /// <summary>
        /// Overridden to do type-specific comparision.
        /// </summary>
        /// <param name="lhs">First Object to compare</param>
        /// <param name="rhs">Second Object to compare</param>
        /// <returns>Less that zero if lhs is less than rhs. Greater than zero if lhs greater that rhs. Zero if they are equal</returns>
        public int Compare(String lhs, String rhs)
        {
            return String.Compare(lhs, rhs, false, CultureInfo.CurrentCulture);
        }
    }

    /// <summary>
    /// Provides text sorting (case insensitive) for ListViewSortManager class.
    /// </summary>
    public class ListViewTextCaseInsensitiveSort : IListViewSorter
    {
        /// <summary>
        /// Compares two case-insensitive string values.
        /// </summary>
        /// <param name="lhs">Value 1.</param>
        /// <param name="rhs">Value 2.</param>
        /// <returns>-1 if <c>lhs</c> is should go first, 1 if <c>rhs</c> should
        /// go first, 0 if both numbers are equal.</returns>
        public int Compare(String lhs, String rhs)
        {
            return String.Compare(lhs, rhs, true, CultureInfo.CurrentCulture);
        }
    }

    /// <summary>
    /// Provides date sorting for the ListViewSortManager class.
    /// </summary>
    public class ListViewDateSort : IListViewSorter
    {
        /// <summary>
        /// Compares two string values, parsing them as dates.
        /// </summary>
        /// <param name="lhs">Value 1.</param>
        /// <param name="rhs">Value 2.</param>
        /// <returns>-1 if <c>lhs</c> is should go first, 1 if <c>rhs</c> should
        /// go first, 0 if both numbers are equal.</returns>
        public int Compare(String lhs, String rhs)
        {
            return DateTime.Parse(lhs, CultureInfo.CurrentCulture).CompareTo(DateTime.Parse(rhs, CultureInfo.CurrentCulture));
        }
    }

    /// <summary>
    /// Provides integer (32 bits) sorting for the ListViewSortManager class.
    /// </summary>
    public class ListViewInt32Sort : IListViewSorter
    {
        /// <summary>
        /// Compares two string values, parsing them as integers.
        /// </summary>
        /// <param name="lhs">Value 1.</param>
        /// <param name="rhs">Value 2.</param>
        /// <returns>-1 if <c>lhs</c> is should go first, 1 if <c>rhs</c> should
        /// go first, 0 if both numbers are equal.</returns>
        public int Compare(String lhs, String rhs)
        {
            return int.Parse(lhs, NumberStyles.Number, CultureInfo.CurrentCulture) -
                int.Parse(rhs, NumberStyles.Number, CultureInfo.CurrentCulture);
        }
    }

    /// <summary>
    /// Provides integer (64 bits) sorting for the ListViewSortManager class.
    /// </summary>
    public class ListViewInt64Sort : IListViewSorter
    {
        /// <summary>
        /// Compares two string values, parsing them as 64-bit integers.
        /// </summary>
        /// <param name="lhs">Value 1.</param>
        /// <param name="rhs">Value 2.</param>
        /// <returns>-1 if <c>lhs</c> is should go first, 1 if <c>rhs</c> should
        /// go first, 0 if both numbers are equal.</returns>
        public int Compare(String lhs, String rhs)
        {
            return (int) (Int64.Parse(lhs, NumberStyles.Number, CultureInfo.CurrentCulture) -
                Int64.Parse(rhs, NumberStyles.Number, CultureInfo.CurrentCulture));
        }
    }

	public class ListViewInt32MetricPrefixesSort : IListViewSorter
	{
		/// <summary>
		/// Compares two numeric string values with a metric prefix (kb, mb, b etc.), parsing them as integers.
		/// </summary>
		/// <param name="lhs">Value 1.</param>
		/// <param name="rhs">Value 2.</param>
		/// <returns>-1 if <c>lhs</c> is should go first, 1 if <c>rhs</c> should
		/// go first, 0 if both numbers are equal.</returns>
		public int Compare(String lhs, String rhs)
		{
			long lhsLong = 0;
			string[] lhsParts = lhs.Split(' ');
			if(lhsParts.Length == 2)
			{
				lhsLong = long.Parse(lhsParts[0], NumberStyles.Number, CultureInfo.CurrentCulture);
				switch(char.ToUpper(lhsParts[1][0]))
				{
					case 'K':
						lhsLong *= 1024;
						break;
					case 'M':
						lhsLong *= 1024 * 1024;
						break;
					case 'G':
						lhsLong *= 1024 * 1024 * 1024;
						break;
				}
			}
			else
			{
				throw new ArgumentException("Bad left hand side");
			}

			long rhsLong = 0;
			string[] rhsParts = rhs.Split(' ');
			if(rhsParts.Length == 2)
			{
				rhsLong = long.Parse(rhsParts[0], NumberStyles.Number, CultureInfo.CurrentCulture);
				switch(char.ToUpper(rhsParts[1][0]))
				{
					case 'K':
						rhsLong *= 1024;
						break;
					case 'M':
						rhsLong *= 1024 * 1024;
						break;
					case 'G':
						rhsLong *= 1024 * 1024 * 1024;
						break;
				}
			}
			else
			{
				throw new ArgumentException("Bad right hand side");
			}
			return (lhsLong > rhsLong) ? 1 : (lhsLong < rhsLong) ? -1 : 0;
		}
	}

    /// <summary>
    /// Provides floating-point sorting for the ListViewSortManager class.
    /// </summary>
    public class ListViewDoubleSort : IListViewSorter
    {
        /// <summary>
        /// Compares two string values, parsing them as doubles.
        /// </summary>
        /// <param name="lhs">Value 1.</param>
        /// <param name="rhs">Value 2.</param>
        /// <returns>-1 if <c>lhs</c> is should go first, 1 if <c>rhs</c> should
        /// go first, 0 if both numbers are equal.</returns>
        public int Compare(String lhs, String rhs)
        {
            Double result = Double.Parse(lhs, CultureInfo.CurrentCulture) -
                Double.Parse(rhs, CultureInfo.CurrentCulture);

            if(result > 0)
                return 1;
            else if(result < 0)
                return -1;
            else
                return 0;
        }
    }
    
    /// <summary>
    /// Provides currency sorting for the ListViewSortManager class.
    /// </summary>
    public class ListViewCurrencySort : IListViewSorter
    {
        /// <summary>
        /// Called by base class to compare two currency numbers.
        /// </summary>
        /// <param name="lhs">First number to compare, as a string.</param>
        /// <param name="rhs">Second number to compare, as a string.</param>
        /// <returns>-1 if <c>lhs</c> is should go first, 1 if <c>rhs</c> should
        /// go first, 0 if both numbers are equal.</returns>
        public int Compare(String lhs, String rhs)
        {
            decimal result =
                decimal.Parse(lhs, NumberStyles.Currency, CultureInfo.CurrentCulture) -
                decimal.Parse(rhs, NumberStyles.Currency, CultureInfo.CurrentCulture);

            if(result > 0)
                return 1;
            else if(result < 0)
                return -1;
            else
                return 0;
        }
    }

    /// <summary>
    /// Provides IP comparisons for the ListViewSortManager class.
    /// </summary>
    public class ListViewIPSort : IListViewSorter
    {
        /// <summary>
        /// Called by base class to compare two currency numbers.
        /// </summary>
        /// <param name="lhs">First number to compare, as a string.</param>
        /// <param name="rhs">Second number to compare, as a string.</param>
        /// <returns>-1 if <c>lhs</c> is should go first, 1 if <c>rhs</c> should
        /// go first, 0 if both numbers are equal.</returns>
        public int Compare(String lhs, String rhs)
        {
            IPAddress lhsip = IPAddress.Parse(lhs);
            IPAddress rhsip = IPAddress.Parse(rhs);

            byte[] lhsIpBytes = lhsip.GetAddressBytes();
            byte[] rhsIpBytes = rhsip.GetAddressBytes();

            for(int i = 0; i < lhsIpBytes.Length; i++)
            {
                if(lhsIpBytes[i] < rhsIpBytes[i])
                    return -1;
                else if(lhsIpBytes[i] > rhsIpBytes[i])
                    return 1;
            }

            return 0;
        }
    }

    /// <summary>
    /// Provides percentage sorting for the ListViewSortManager class.
    /// </summary>
    public class ListViewPercentSort : IListViewSorter
    {
        /// <summary>
        /// Called by base class to compare two percentage numbers.
        /// </summary>
        /// <param name="lhs">First number to compare, as a string.</param>
        /// <param name="rhs">Second number to compare, as a string.</param>
        /// <returns>-1 if <c>lhs</c> is should go first, 1 if <c>rhs</c> should
        /// go first, 0 if both numbers are equal.</returns>
        public int Compare(String lhs, String rhs)
        {
            string leftNum = lhs.Trim();
            if(leftNum.EndsWith("%"))
                leftNum = leftNum.Substring(0, leftNum.IndexOf("%"));

            string rightNum = rhs.Trim();
            if(rightNum.EndsWith("%"))
                rightNum = rightNum.Substring(0, rightNum.IndexOf("%"));

            decimal result =
                decimal.Parse(leftNum, CultureInfo.CurrentCulture) -
                decimal.Parse(rightNum, CultureInfo.CurrentCulture);

            if(result > 0)
                return 1;
            else if(result < 0)
                return -1;
            else
                return 0;
        }
    }

    #endregion

    #region Special Rows

    /// <summary>
    /// Represents a row that will be sorted always at the end.
    /// </summary>
    [Serializable]
    public class ListViewBottomItem : ListViewItem
    {
        /// <summary>
        /// Default constructor. Initializes a new empty ListViewBottomItem
        /// object.
        /// </summary>
        public ListViewBottomItem()
            : base()
        {
        }

        /// <summary>
        /// Constructor. Initializes a new ListViewBottomItem object.
        /// </summary>
        /// <param name="text">Text to display in the first column.</param>
        public ListViewBottomItem(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Constructor. Initializes a new ListViewBottomItem object.
        /// </summary>
        /// <param name="items">Array of string elements for the different
        /// subitems.</param>
        public ListViewBottomItem(string[] items)
            : base(items)
        {
        }
    }

    #endregion
}
