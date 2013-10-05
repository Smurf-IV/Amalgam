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
	public class HttpRequest
	{
		private HttpHeaderCollection _webHeaders = new HttpHeaderCollection();
		private HttpHeaderCollection _userHeaders = new HttpHeaderCollection();
		private HttpCookieCollection _webCookies = new HttpCookieCollection();
		private HttpParameterCollection _webParams = new HttpParameterCollection();

		private string _host;
		private string _abspath;
		private MethodOptions _method = MethodOptions.Post;
		private string _referer;
		private string _raw;
		private int _contentLength;
		private HttpAuthorization _webauth = new HttpAuthorization();

		public enum MethodOptions
		{
			Post,
			Get,
			Delete,
            Put
		}

		public string Host
		{
			get	{ return _host;	}
			set	{ _host = value; }
		}

		public string AbsPath
		{
			get { return _abspath;	}
			set	{ _abspath = value;	}
		}

		public HttpHeaderCollection Headers
		{
			get	{ return _userHeaders;	}
		}

		public HttpParameterCollection Parameters
		{
			get { return _webParams; }
		}

		public MethodOptions Method
		{
			get	{ return _method; }
			set { _method = value; }
		}

		public string Referer
		{
			get	{ return _referer; }
			set	{ _referer = value;	}
		}

		public string Raw
		{
			get	
            {
				if (_raw.Length == 0)
				{
					this.Build();
				}

				return _raw;
			}
		}

		public HttpCookieCollection Cookies
		{
			get	{ return _webCookies; }
		}

		public int ContentLength
		{
			get { return _contentLength; }
		}

		public HttpAuthorization Authorization
		{
			get { return _webauth; }
		}

		public void Build()
		{
			_raw = BuildRequest();
		}

        public byte[] GetBytes()
        {
            if (_raw.Length == 0)
            {
                this.Build();
            }

            return System.Text.ASCIIEncoding.ASCII.GetBytes(_raw);
        }

		private string BuildRequest()
		{

			AddDefaultHeaders();

			StringBuilder request = new StringBuilder();
			string wparams = _webParams.GetHttpFormat();

            switch (_method)
            {
                case MethodOptions.Post:
				    _contentLength = wparams.Length;

				    //  add the content length header
				    _webHeaders.Add(new HttpHeader(HTTP.HEAD_CONTENT_LENGTH, _contentLength.ToString()));
				    _webHeaders.Add(new HttpHeader(HTTP.HEAD_CONTENT_TYPE, HTTP.HEAD_CONTENT_TYPE_X_WWW_FORM_VALUE));
				    _webHeaders.Add(new HttpHeader(HTTP.HEAD_CONNECTION, HTTP.HEAD_CONNECTION_CLOSE));

				    request.Append(GetHttpVerb());
				    request.Append(_webHeaders.GetHttpFormat());
				    request.Append(_userHeaders.GetHttpFormat());
				    request.Append(_webCookies.GetHttpFormat());
				    request.Append(System.Environment.NewLine).Append(System.Environment.NewLine);
				    request.Append(wparams);
                    break;

                case MethodOptions.Get:
				    _webHeaders.Add(new HttpHeader(HTTP.HEAD_CONNECTION, HTTP.HEAD_CONNECTION_CLOSE));

				    BuildQueryString(wparams);

				    request.Append(GetHttpVerb());
				    request.Append(_webHeaders.GetHttpFormat());
				    request.Append(_userHeaders.GetHttpFormat());
				    request.Append(_webCookies.GetHttpFormat());
				    request.Append(System.Environment.NewLine); 
                    break;
            }

			return request.ToString();
		}

		private string GetHttpVerb()
		{
			StringBuilder header = new StringBuilder();

            switch (_method)
            {
                case MethodOptions.Get:
                    header.Append(HTTP.GET);
                    break;

                case MethodOptions.Post:
                    header.Append(HTTP.POST);
                    break;
            }

			header.Append(" ").Append(_abspath).Append(" HTTP/").Append(HTTP.VERSION).Append(System.Environment.NewLine);

			return header.ToString();
		}

		private void AddDefaultHeaders()
		{
			_webHeaders.Clear();

			_webHeaders.Add(new HttpHeader(HTTP.HEAD_ACCEPT_LANGUAGE, HTTP.HEAD_ACCEPT_LANGUAGE_VALUE));
			_webHeaders.Add(new HttpHeader(HTTP.HEAD_USER_AGENT, HTTP.HEAD_USER_AGENT_VALUE));
			_webHeaders.Add(new HttpHeader(HTTP.HEAD_HOST, _host));
			_webHeaders.Add(new HttpHeader(HTTP.HEAD_CACHE, HTTP.HEAD_CACHE_VALUE));

			if (_referer.Length > 0)
			{
				_webHeaders.Add(new HttpHeader(HTTP.HEAD_REFERER, _referer));
			}

			//  if we using authenticatio then get the authentication header information
			if (_webauth.AuthenicationType != HttpAuthorization.AuthType.None)
			{
				_webHeaders.Add(new HttpHeader(_webauth.HttpFormat()));
			}
		}

		private string BuildQueryString(string paramStr)
		{
			//  only append the querystring parameters if there are any to append
			if (paramStr.Length > 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(_abspath).Append(HTTP.HEAD_QUERYSTRING).Append(paramStr);
				_abspath = sb.ToString();
			}

			return null;
		}


	}
}