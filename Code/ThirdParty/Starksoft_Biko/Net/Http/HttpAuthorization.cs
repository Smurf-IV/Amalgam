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
using System.Text;

namespace Starksoft.Net.Http
{
	/// <summary>
	/// Summary description for WebAuthorization.
	/// </summary>
	public class HttpAuthorization
	{
		private string _username = System.String.Empty;
		private string _password = System.String.Empty;
		private AuthType _authtype = AuthType.None;

		public enum AuthType
		{
			None,
			Basic
		}

		public string UserName
		{
			get	{ return _username;	}
			set	{ _username = value; }
		}

		public string Password
		{
			get	{ return _password;	}
			set { _password = value; }
		}

		public AuthType AuthenicationType
		{
			get	{ return _authtype;	}
			set	{ _authtype = value; }
		}

		public string HttpFormat()
		{
			switch (_authtype)
			{
				case AuthType.None:
                    return "";
                case AuthType.Basic:
                    return String.Format("{0}: {1} {2}", HTTP.HEAD_AUTHORIZATION, HTTP.HEAD_AUTHORIZATION_TYPE_BASIC, Base64Encode(String.Format("{0}:{1}", _username, _password)));
                default:
                    throw new HttpException("unknown authentication type");
			}
		}

		private string Base64Encode(string val)
		{
			return Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(val));
		}

	}

}
