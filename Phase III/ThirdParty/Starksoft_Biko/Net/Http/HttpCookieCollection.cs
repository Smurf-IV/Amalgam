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
    /// Http cookie collection.
    /// </summary>
    public class HttpCookieCollection : IEnumerable<HttpCookie>
    {
        private List<HttpCookie> _list = new List<HttpCookie>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public HttpCookieCollection()
        { }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the
        /// first occurrence within the entire WebCookieCollection list.
        /// </summary>
        /// <param name="item">The WebCookie object to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire if found; otherwise, -1.</returns>
        public int IndexOf(HttpCookie item)
        {
            return _list.IndexOf(item);
        }



        /// <summary>
        /// Adds an WebCookie to the end of the WebCookieCollection list.
        /// </summary>
        /// <param name="item">WebCookie object to add.</param>
        public void Add(HttpCookie item)
        {
            _list.Add(item);
        }

        /// <summary>
        ///  Gets the number of elements actually contained in the WebCookieCollection list.
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        IEnumerator<HttpCookie> IEnumerable<HttpCookie>.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Gets an WebCookie from the WebCookieCollection list based on index value.
        /// </summary>
        /// <param name="index">Numeric index of item to retrieve.</param>
        /// <returns>WebCookie object.</returns>
        public HttpCookie this[int index]
        {
            get { return _list[index]; }
        }

        /// <summary>
        /// Remove all elements from the WebCookieCollection list.
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

            sb.Append(HTTP.HEAD_COOKIE).Append(": ");
            foreach (HttpCookie cookie in this)
            {
                sb.Append(cookie.HttpFormat()).Append("; ");
            }

            //  remove the final semi-colon and spacing
            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }



    }
}














        //public string HttpFormat()
        //{
        //    int cnt = 0;
        //    StringBuilder rtn = new StringBuilder();

        //    if (this.Count == 0)
        //    {
        //        return string.Empty;
        //    }

        //    rtn.Append(HTTP.HEAD_COOKIE).Append(": ");

        //    int tempFor1 = this.Count;
        //    for (cnt = 0; cnt < tempFor1; cnt++)
        //    {
        //        //  append the other name-value pairs
        //        rtn.Append(this.GetItem(cnt).HttpFormat()).Append("; ");

        //    }

        //    //  remove the final semi-colon and spacing
        //    rtn.Remove(rtn.Length - 2, 2);

        //    return rtn.ToString();
        //}