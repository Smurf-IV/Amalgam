/*
 *  Authors:  Benton Stark
 * 
 *  Copyright (c) 2012 Starksoft, LLC (http://www.starksoft.com) 
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


namespace Starksoft.Net.Http
{
    /// <summary>
    /// Http header collection.
    /// </summary>
    public class HttpHeaderCollection : IEnumerable<HttpHeader>
    {
        private List<HttpHeader> _list = new List<HttpHeader>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public HttpHeaderCollection()
        { }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire WebHeaderCollection list.
        /// </summary>
        /// <param name="item">The WebHeader object to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire if found; otherwise, -1.</returns>
        public int IndexOf(HttpHeader item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Adds an WebHeader to the end of the WebHeaderCollection list.
        /// </summary>
        /// <param name="item">WebHeader object to add.</param>
        public void Add(HttpHeader item)
        {
            _list.Add(item);
        }

        /// <summary>
        ///  Gets the number of elements actually contained in the WebHeaderCollection list.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        IEnumerator<HttpHeader> IEnumerable<HttpHeader>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Gets an WebHeader from the WebHeaderCollection list based on index value.
        /// </summary>
        /// <param name="index">Numeric index of item to retrieve.</param>
        /// <returns>WebHeader object.</returns>
        public HttpHeader this[int index]
        {
            get { return _list[index]; }
        }

        /// <summary>
        /// Remove all elements from the WebHeaderCollection list.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        public string GetHttpFormat()
        {
            if (this.Count == 0)
                return string.Empty;
            StringBuilder rtn = new StringBuilder();
            foreach(HttpHeader header in this)
            {
                rtn.Append(this.GetHttpFormat());
            }
            return rtn.ToString();
        }

    }
}

