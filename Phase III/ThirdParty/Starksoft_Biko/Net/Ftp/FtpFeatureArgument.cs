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
using System.Collections.Generic;
using System.Text;

namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// Ftp feature argument item class.
    /// </summary>
    public class FtpFeatureArgument
    {
        private string _name;
        private bool _isDefault;

        /// <summary>
        /// Constructor to create a new ftp feature.
        /// </summary>
        /// <param name="name">Feature name.</param>
        public FtpFeatureArgument(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "must have a value");

            _name = name.Trim();
            Parse();
        }

        /// <summary>
        /// Feature name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Default value indicator. 
        /// </summary>
        /// <remarks>Some feature arguments may be marked as a default argument by the FTP server.</remarks>
        public bool IsDefault
        {
            get { return _isDefault; }
        }

        private void Parse()
        {
            char[] a = _name.ToCharArray();
            int last = a.Length - 1;
            if (a[last] == '*')
            {
                _isDefault = true;
                _name = _name.Substring(0, last);
            }

        }

    }
}
