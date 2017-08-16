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
	public class HttpHeader
	{
		private string _Name;
		private string _Value;

		public HttpHeader(string name, string value) : base()
		{
			_Name = name;
			_Value = value;
		}

		public HttpHeader(string line)
		{
			Parse(line);
		}

		public string Name
		{
			get	{ return _Name;	}
		}

		public string Value
		{
			get	{ return _Value; }
		}

		public string HttpFormat()
		{
			return new StringBuilder().Append(_Name).Append(": ").Append(_Value).Append(System.Environment.NewLine).ToString();
		}

		private void Parse(string line)
		{
			_Name = line.Substring(0, line.IndexOf(":")).Trim();
			_Value = line.Substring(line.IndexOf(":") + 1).Trim();
		}

	}

}