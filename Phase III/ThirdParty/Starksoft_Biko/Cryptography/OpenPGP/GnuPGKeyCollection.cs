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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace Starksoft.Cryptography.OpenPGP
{
    /// <summary>
    /// Collection of PGP keys stored in the GnuPGP application.
    /// </summary>
    public class GnuPGKeyCollection : IEnumerable<GnuPGKey>
    {
        private List<GnuPGKey> _keyList = new List<GnuPGKey>();
        private string _raw;

        private static string COL_KEY = "Key";
        private static string COL_KEY_EXPIRATION = "KeyExpiration";
        private static string COL_USER_ID = "UserId";
        private static string COL_USER_NAME = "UserName";
        private static string COL_SUB_KEY = "SubKey";
        private static string COL_SUB_KEY_EXPIRATION = "SubKeyExpiration";
        private static string COL_RAW = "Raw";
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="keys">StreamReader object containing GnuPG raw key stream data.</param>
        public GnuPGKeyCollection(StreamReader keys)
        {
            Fill(keys);
            GetRaw(keys);
        }

        /// <summary>
        /// Raw key stream text data.
        /// </summary>
        public string Raw
        {
            get { return _raw; }
        }

        private void GetRaw(StreamReader keys)
        {
            keys.BaseStream.Position = 0;
            _raw = keys.ReadToEnd();
        }

        //private void Fill(StreamReader data)
        //{
        //    string line = null;
        //    line = data.ReadLine();
        //    line = data.ReadLine();
        //    do
        //    {
        //        string line1 = data.ReadLine();
        //        string line2 = data.ReadLine();
        //        string line3 = data.ReadLine();
        //        while (!data.EndOfStream && !(line3.StartsWith("sub") || line3.StartsWith("ssb")))
        //        {
        //            line3 = data.ReadLine();
        //        }
        //        GnuPGKey key = new GnuPGKey(String.Format("{0}\r\n{1}\r\n{2}", line1, line2, line3));
        //        _keyList.Add(key);
        //        data.ReadLine();
        //    }
        //    while (!data.EndOfStream);
        //}

        private void Fill(StreamReader data)
        {
            string text = "";
                        
            while (!data.EndOfStream)
            {
                // read a line from the output stream
                string line = data.ReadLine();

                // skip lines we are not interested in
                if (!line.StartsWith("pub") && !line.StartsWith("sec") && !line.StartsWith("uid"))
                {
                    // make sure this isn't the end of a key parsing operation
                    if (text.Length != 0)
                    {
                        _keyList.Add(new GnuPGKey(text));
                        text = "";   // clear the key variable
                    }
                    continue;
                }
                text += line;
            }
        }

        /// <summary>
        ///  Searches for the specified GnuPGKey object and returns the zero-based index of the
        ///  first occurrence within the entire GnuPGKeyCollection colleciton.
        /// </summary>
        /// <param name="item">The GnuPGKeyobject to locate in the GnuPGKeyCollection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire GnuPGKeyCollection, if found; otherwise, –1.</returns>
        public int IndexOf(GnuPGKey item)
        {
            return _keyList.IndexOf(item);
        }

        /// <summary>
        ///  Retrieves the specified GnuPGKey object by zero-based index from the GnuPGKeyCollection.        
        /// </summary>
        /// <param name="index">Zero-based index integer value.</param>
        /// <returns>The GnuPGKey object corresponding to the index position.</returns>
        public GnuPGKey GetKey(int index)
        { 
            return _keyList[index]; 
        }

        /// <summary>
        /// Adds a GnuPGKey object to the end of the GnuPGKeyCollection.
        /// </summary>
        /// <param name="item">GnuPGKey item to add to the GnuPGKeyCollection.</param>
        public void AddKey(GnuPGKey item)
        {
            _keyList.Add(item);
        }

        /// <summary>
        /// Gets the number of elements actually contained in the GnuPGKeyCollection.
        /// </summary>
        public int Count
        { 
            get { return _keyList.Count; } 
        }

        IEnumerator<GnuPGKey> IEnumerable<GnuPGKey>.GetEnumerator()
        { 
            return _keyList.GetEnumerator(); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _keyList.GetEnumerator();
        }

        /// <summary>
        /// Indexer for the GnuPGKeyCollection collection.
        /// </summary>
        /// <param name="index">Zero-based index value.</param>
        /// <returns></returns>
        public GnuPGKey this[int index]
        {
            get
            {
                return _keyList[index];
            }

        }

        /// <summary>
        /// Convert current GnuPGKeyCollection to a DataTable object to make data binding a minpulation of key data easier.
        /// </summary>
        /// <returns>Data table object.</returns>
        public DataTable ToDataTable()
        {
            DataTable dataTbl = new DataTable();
            CreateColumns(dataTbl);

            foreach (GnuPGKey item in _keyList)
            {
                DataRow row = dataTbl.NewRow();
                row[COL_USER_ID] = item.UserId;
                row[COL_USER_NAME] = item.UserName;
                row[COL_KEY] = item.Key;
                row[COL_KEY_EXPIRATION] = item.KeyExpiration;
                row[COL_SUB_KEY] = item.SubKey;
                row[COL_SUB_KEY_EXPIRATION] = item.SubKeyExpiration;
                dataTbl.Rows.Add(row);
            }

            return dataTbl;
        }

        private void CreateColumns(DataTable dataTbl)
        {
            dataTbl.Columns.Add(new DataColumn(COL_USER_ID, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_USER_NAME, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_KEY, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_KEY_EXPIRATION, typeof(DateTime)));
            dataTbl.Columns.Add(new DataColumn(COL_SUB_KEY, typeof(string)));
            dataTbl.Columns.Add(new DataColumn(COL_SUB_KEY_EXPIRATION, typeof(DateTime)));
            dataTbl.Columns.Add(new DataColumn(COL_RAW, typeof(string)));
        }

    }
}
