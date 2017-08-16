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
    /// Http parameter collection.
    /// </summary>
    public class HttpParameterCollection : IEnumerable<HttpParameter>
    {
        private List<HttpParameter> _list = new List<HttpParameter>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public HttpParameterCollection()
        { }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire WebParameterCollection list.
        /// </summary>
        /// <param name="item">The WebParameter object to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire if found; otherwise, -1.</returns>
        public int IndexOf(HttpParameter item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Adds an WebParameter to the end of the WebParameterCollection list.
        /// </summary>
        /// <param name="item">WebParameter object to add.</param>
        public void Add(HttpParameter item)
        {
            _list.Add(item);
        }

        /// <summary>
        ///  Gets the number of elements actually contained in the WebParameterCollection list.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        IEnumerator<HttpParameter> IEnumerable<HttpParameter>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Gets an WebParameter from the WebParameterCollection list based on index value.
        /// </summary>
        /// <param name="index">Numeric index of item to retrieve.</param>
        /// <returns>WebParameter object.</returns>
        public HttpParameter this[int index]
        {
            get { return _list[index]; }
        }

        /// <summary>
        /// Remove all elements from the WebParameterCollection list.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        public string GetHttpFormat()
        {
            if (this.Count == 0)
                return string.Empty;
            
            StringBuilder sb = new StringBuilder();

            foreach(HttpParameter param in this)
            {
                //  append the other name-value pairs
                sb.Append(param.HttpFormat());
                sb.Append("&");
            }

            sb.Remove(sb.Length - 1, 1);

            return System.Web.HttpUtility.UrlEncode(sb.ToString());
        }

    }
}

