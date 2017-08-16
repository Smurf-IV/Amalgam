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


namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// Ftp response collection.
    /// </summary>
    public class FtpFeatureArgumentCollection : IEnumerable<FtpFeatureArgument>
    {
        private List<FtpFeatureArgument> _list = new List<FtpFeatureArgument>();
        private string _text;

        /// <summary>
        /// Default constructor for no feature arguments.
        /// </summary>
        public FtpFeatureArgumentCollection()
        {

        }

        /// <summary>
        /// Default constructor with features.
        /// </summary>
        /// <param name="text">Raw feature list text.</param>
        public FtpFeatureArgumentCollection(string text)
        {
            if (String.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            _text = text;
            Parse();
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire FtpFeatureArgumentCollection list.
        /// </summary>
        /// <param name="item">The FtpFeatureArgument object to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire if found; otherwise, -1.</returns>
        public int IndexOf(FtpFeatureArgument item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Adds an FtpFeatureArgument to the end of the FtpFeatureArgumentCollection list.
        /// </summary>
        /// <param name="item">FtpFeatureArgument object to add.</param>
        public void Add(FtpFeatureArgument item)
        {
            _list.Add(item);
        }

        /// <summary>
        ///  Gets the number of elements actually contained in the FtpFeatureArgumentCollection list.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        IEnumerator<FtpFeatureArgument> IEnumerable<FtpFeatureArgument>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Gets an FtpFeatureArgument from the FtpFeatureArgumentCollection list based on index value.
        /// </summary>
        /// <param name="index">Numeric index of item to retrieve.</param>
        /// <returns>FtpFeatureArgument object.</returns>
        public FtpFeatureArgument this[int index]
        {
            get { return _list[index]; }
        }

        /// <summary>
        /// Gets an FtpFeature from the FtpFeatureCollection list based on name.
        /// </summary>
        /// <param name="name">Name of the feature.</param>
        /// <returns>FtpFeature object.</returns>
        public FtpFeatureArgument this[string name]
        {
            get { return Find(name); }
        }

        /// <summary>
        /// Remove all elements from the FtpFeatureArgumentCollection list.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Get the raw FTP server supplied reponse text for features.
        /// </summary>
        /// <returns>A string containing the FTP feature list.</returns>
        public string GetRawText()
        {
            return _text;
        }

        /// <summary>
        /// Linearly searches for the specified object based on the feature 'name' parameter value
        /// and returns the corresponding object with the name is found; otherwise null.  Search is case insensitive.
        /// </summary>
        /// <param name="name">The name of the FtpFeatureArgument to locate in the collection.</param>
        /// <returns>FtpFeatureArgument object if the name if found; otherwise null.</returns>
        public FtpFeatureArgument Find(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "must have a value");

            foreach (FtpFeatureArgument item in _list)
            {
                if (String.Compare(name, item.Name, true) == 0)
                    return item;
            }

            return null;
        }

        /// <summary>
        /// Linearly searches for the specified object based on the feature 'name' parameter value
        /// and returns true if an object with the name is found; otherwise false.  Search is case insensitive.
        /// </summary>
        /// <remarks>
        /// example:  col.Contains("UTF8");
        /// </remarks>
        /// <param name="name">The name of the FtpFeatureArgument to locate in the collection.</param>
        /// <returns>True if the name if found; otherwise false.</returns>
        public bool Contains(string name)
        {
            return Find(name) != null ? true : false;
        }

        private void Parse()
        {
            string[] args = GetArgumentArray();
            foreach (string arg in args)
            {
                _list.Add(new FtpFeatureArgument(arg));
            }
        }

        /// <summary>
        /// Gets the FTP feature arguments as a string array.
        /// </summary>
        /// <returns>Array of strings containing arguments; otherwise null.</returns>
        public string[] GetArgumentArray()
        {
            if (String.IsNullOrEmpty(_text))
                return null;
            else
                if (_text.Contains(";"))
                    return SplitSemi(_text);
                else
                    if (_text.Contains(" "))
                        return SplitSpace(_text);
                    else
                    {
                        string[] a = new string[1];
                        a[0] = _text;
                        return a;
                    }
        }

        private string[] SplitSemi(string list)
        {
            return list.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string[] SplitSpace(string list)
        {
            return list.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }



    }
}