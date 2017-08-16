/*
 *  Authors:  Benton Stark
 * 
 *  Copyright (c) 2007-2012 Starksoft, LLC (http://www.starksoft.com) 
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 * 
 */

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// Ftp item list.
    /// </summary>
    /// <exception cref="FtpFeatureException"></exception>
    public class FtpItemCollection : IEnumerable<FtpItem> 
	{
        private List<FtpItem> _list = new List<FtpItem>();

        private long _totalSize;

        // standard data columns
        private static string COL_NAME = "Name";
        private static string COL_MODIFIED = "Modified";
        private static string COL_SIZE = "Size";
        private static string COL_SYMBOLIC_LINK = "SymbolicLink";
        private static string COL_TYPE = "Type";
        private static string COL_ATTRIBUTES = "Attributes";
        private static string COL_MODE = "Mode";
        private static string COL_RAW_TEXT = "RawText";

        // MLSx specific data columns
        private static string COL_MLSX_CREATED = "Created";
        private static string COL_MLSX_UNIQUE_ID = "UniqueId";
        private static string COL_MLSX_PERMISSIONS = "Permissions";
        private static string COL_MLSX_LANGUAGE = "Language";
        private static string COL_MLSX_MEDIA_TYPE = "MediaType";
        private static string COL_MLSX_CHARACTER_SET = "CharacterSet";
        private static string COL_MLSX_GROUP = "Group";
        private static string COL_MLSX_OWNER = "Owner";

        /// <summary>
        /// Default constructor for FtpItemCollection.
        /// </summary>
        public FtpItemCollection()
        {  }

        /// <summary>
        /// Split a multi-line file list text response and add the parsed items to the collection.
        /// </summary>
        /// <param name="path">Path to the item on the FTP server.</param>
        /// <param name="fileList">The multi-line file list text from the FTP server.</param>
        /// <param name="itemParser">Line item parser object used to parse each line of fileList data.</param>
        public FtpItemCollection(string path, string fileList, IFtpItemParser itemParser)
        {
            Parse(path, fileList, itemParser);
        }

        /// <summary>
        /// Merges two FtpItemCollection together into a single collection.
        /// </summary>
        /// <param name="items">Collection to merge with.</param>
        public void Merge(FtpItemCollection items)
        {
            if (items == null)
                throw new ArgumentNullException("items", "must have a value");

            foreach (FtpItem item in items)
            {
                FtpItem n = new FtpItem(item.Name, item.Modified, item.Size, item.ItemType, item.Attributes, item.Mode, item.SymbolicLink, item.RawText);
                n.SetParentPath(item.ParentPath);
                this.Add(n);
            }
        }

        private void Parse(string path, string fileList, IFtpItemParser itemParser)
        {
            string[] lines = SplitFileList(fileList);

            int length = lines.Length - 1;
            for (int i = 0; i <= length; i++)
            {
                FtpItem item = itemParser.ParseLine(lines[i]);
                if (item != null)
                {
                    // set the parent path to the value passed in
                    item.SetParentPath(path);
                    _list.Add(item);
                    _totalSize += item.Size;
                }
            }
        }

        private string[] SplitFileList(string response)
        {
            return response.Split(new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Convert current FtpCollection to a DataTable object.
        /// </summary>
        /// <returns>Data table object.</returns>
        public DataTable ToDataTable()
        {
            DataTable dataTbl = new DataTable();
            dataTbl.Locale = CultureInfo.InvariantCulture;
            bool mlsx = false;

            if (_list.Count > 0)
            {
                if (_list[0] is FtpMlsxItem)
                    mlsx = true;
            }

            CreateColumns(dataTbl, mlsx);

            foreach (FtpItem item in _list)
            {
                // skip parent and current directory items to get consistent data
                if (item.ItemType == FtpItemType.ParentDirectory
                    || item.ItemType == FtpItemType.CurrentDirectory)
                    continue;

                DataRow row = dataTbl.NewRow();

                // set standard row columns for FtpItem object
                row[COL_NAME] = item.Name;
                row[COL_MODIFIED] = item.Modified == null ? DateTime.MinValue : item.Modified;
                row[COL_SIZE] = item.Size;
                row[COL_TYPE] = item.ItemType.ToString();
                row[COL_ATTRIBUTES] = item.Attributes == null ? String.Empty : item.Attributes;
                row[COL_SYMBOLIC_LINK] = item.SymbolicLink == null ? String.Empty : item.SymbolicLink;
                row[COL_RAW_TEXT] = item.RawText == null ? String.Empty : item.RawText;
                row[COL_MODE] = item.Mode == null ? 0 : item.Mode;

                // set MLSx specific columns if the object is a FtpMlsxItem object
                if (mlsx)
                {
                    FtpMlsxItem mitem = (FtpMlsxItem)item;
                    row[COL_MLSX_CREATED] = mitem.Created == null ? DateTime.MinValue : mitem.Created; ;
                    row[COL_MLSX_UNIQUE_ID] = mitem.UniqueId == null ? String.Empty : mitem.UniqueId;
                    row[COL_MLSX_PERMISSIONS] = mitem.Permissions.ToString();
                    row[COL_MLSX_LANGUAGE] = mitem.Language == null ? String.Empty : mitem.Language;
                    row[COL_MLSX_MEDIA_TYPE] = mitem.MediaType == null ? String.Empty : mitem.MediaType;
                    row[COL_MLSX_CHARACTER_SET] = mitem.CharacterSet == null ? String.Empty : mitem.CharacterSet;
                    row[COL_MLSX_GROUP] = mitem.Group == null ? 0 : mitem.Group;
                    row[COL_MLSX_OWNER] = mitem.Owner == null ? 0 : mitem.Owner;
                }

                dataTbl.Rows.Add(row);
            }

            return dataTbl;
        }
        
        private void CreateColumns(DataTable dataTbl, bool mlsx)
        {
            // standard data columns
            dataTbl.Columns.Add(new DataColumn(COL_NAME, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_MODIFIED, typeof(DateTime)));
            dataTbl.Columns.Add(new DataColumn(COL_SIZE, typeof(long)));
            dataTbl.Columns.Add(new DataColumn(COL_TYPE, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_ATTRIBUTES, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_SYMBOLIC_LINK, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_RAW_TEXT, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_MODE, typeof(Int32)));

            if (!mlsx)
                return;

            // MLSx specific data columns
            dataTbl.Columns.Add(new DataColumn(COL_MLSX_CREATED, typeof(DateTime)));
            dataTbl.Columns.Add(new DataColumn(COL_MLSX_UNIQUE_ID, typeof(String)));
            dataTbl.Columns.Add(new DataColumn(COL_MLSX_PERMISSIONS, typeof(String)));
            dataTbl.Columns.Add(new DataColumn(COL_MLSX_LANGUAGE, typeof(String)));
            dataTbl.Columns.Add(new DataColumn(COL_MLSX_MEDIA_TYPE, typeof(String)));
            dataTbl.Columns.Add(new DataColumn(COL_MLSX_CHARACTER_SET, typeof(String)));
            dataTbl.Columns.Add(new DataColumn(COL_MLSX_GROUP, typeof(Int32)));
            dataTbl.Columns.Add(new DataColumn(COL_MLSX_OWNER, typeof(Int32)));
        }

        /// <summary>
        /// Gets the size, in bytes, of all files in the collection as reported by the FTP server.
        /// </summary>
        public long TotalSize
        {
            get { return _totalSize; }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire FtpItemCollection list.
        /// </summary>
        /// <param name="item">The FtpItem to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire if found; otherwise, -1.</returns>
        public int IndexOf(FtpItem item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Adds an FtpItem to the end of the FtpItemCollection list.
        /// </summary>
        /// <param name="item">FtpItem object to add.</param>
        public void Add(FtpItem item)
        {
            _list.Add(item);
        }

        /// <summary>
        ///  Gets the number of elements actually contained in the FtpItemCollection list.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        IEnumerator<FtpItem> IEnumerable<FtpItem>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Gets an FtpItem from the FtpItemCollection based on index value.
        /// </summary>
        /// <param name="index">Numeric index of item to retrieve.</param>
        /// <returns>FtpItem</returns>
        public FtpItem this[int index]
        {
            get { return _list[index];  }
        }

        /// <summary>
        /// Linearly searches for the specified object based on the 'name' parameter value
        /// and returns true if an object with the name is found; otherwise false.
        /// </summary>
        /// <param name="name">The name of the FtpItem to locate in the collection.</param>
        /// <returns>True if the name if found; otherwise false.</returns>
        public bool Contains(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name", "must have a value");

            foreach (FtpItem item in _list)
            {
                if (name == item.Name)
                    return true;
            }

            return false;
        }
    }
} 