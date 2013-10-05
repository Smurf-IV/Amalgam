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

using System.Text;

namespace Starksoft.Net.Http
{
	public class HttpCookie
	{
		private string _name;
		private string _value;

		public HttpCookie(string name, string value) : base()
		{
			_name = name;
			_value = value;
		}

		public HttpCookie(string data) : base()
		{
			Parse(data);
		}
        
		public string Name
		{
			get { return _name;	}
		}

		public string Value
		{
			get { return _value; }
		}

		public string HttpFormat()
		{
			return new StringBuilder().Append(_name).Append("=").Append(_value).ToString();
		}

		private void Parse(string data)
		{
			int beg_idx = 0;
			int end_idx = 0;

			beg_idx = 0;
			end_idx = data.IndexOf("=");

			_name = data.Substring(beg_idx, end_idx).Trim();

			beg_idx = end_idx + 1;
			end_idx = data.IndexOf(";", beg_idx) - beg_idx;

			_value = data.Substring(beg_idx, end_idx).Trim();
		}

	}

}