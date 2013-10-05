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
    /// Ftp feature item class.
    /// </summary>
    public class FtpFeature
    {
        private string _name;
        private FtpFeatureArgumentCollection _argList;
        

        /// <summary>
        /// Constructor to create a new ftp feature.
        /// </summary>
        /// <param name="name">Feature name (required).</param>
        /// <param name="arguments">Argument list (can empty).</param>
        public FtpFeature(string name, string arguments)
		{
            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name", "must have a value");
            if (arguments == null)
                throw new ArgumentNullException("arguments", "cannot be null");
            _name = name;
            if (arguments.Length != 0)
                _argList = new FtpFeatureArgumentCollection(arguments);
            else
                _argList = new FtpFeatureArgumentCollection();
		}

        /// <summary>
        /// Feature name.
        /// </summary>
		public string Name
		{
			get	{ return _name;	}
		}

        /// <summary>
        /// Get the FTP feature arguments collection.
        /// </summary>
        public FtpFeatureArgumentCollection Arguments
        {
            get { return _argList; }
        }

    }
}
