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
using System;

namespace Starksoft.Net.Http
{

	public class HttpResponse
	{

		private string _raw = System.String.Empty;
		private string _html = System.String.Empty;
		private ResponseCodeOptions _responseCode;
		private ResponseStatusOptions _responseStatus;
		private string _responseCodeDescription;

		private HttpHeaderCollection _webHeaders = new HttpHeaderCollection();
		private HttpCookieCollection _webCookies = new HttpCookieCollection();

		public enum ResponseStatusOptions
		{
			SwitchingProtocols = 1,
			Success = 2,
			Redirection = 3,
			ClientError = 4,
			ServerError = 5
		}

		public enum ResponseCodeOptions
		{
			None = 0,
			@Continue = 100,
			SwitchingProtocols = 101,
			OK = 200,
			Created = 201,
			Accepted = 202,
			NonAuthoritiveInformation = 203,
			NoContent = 204,
			ResetContent = 205,
			PartialContent = 206,
			MultipleChoices = 300,
			MovedPermanetly = 301,
			Found = 302,
			SeeOther = 303,
			NotModified = 304,
			UserProxy = 305,
			TemporaryRedirect = 307,
			BadRequest = 400,
			Unauthorized = 401,
			PaymentRequired = 402,
			Forbidden = 403,
			NotFound = 404,
			MethodNotAllowed = 405,
			NotAcceptable = 406,
			ProxyAuthenticantionRequired = 407,
			RequestTimeout = 408,
			Conflict = 409,
			Gone = 410,
			PreconditionFailed = 411,
			RequestEntityTooLarge = 413,
			RequestURITooLong = 414,
			UnsupportedMediaType = 415,
			RequestedRangeNotSatisfied = 416,
			ExpectationFailed = 417,
			InternalServerError = 500,
			NotImplemented = 501,
			BadGateway = 502,
			ServiceUnavailable = 503,
			GatewayTimeout = 504,
			HttpVersionNotSupported = 505
		}

		public HttpResponse(string request)
		{
			_raw = request;
			Parse();
		}

		public HttpResponse()
		{ }

		public string ResponseCodeDescription
		{ 
            get {	return _responseCodeDescription; }	
        }

		public ResponseCodeOptions ResponseCode
		{
			get	{ return _responseCode;	}
		}

		public ResponseStatusOptions ResponseStatus
		{
			get	{ return _responseStatus; }
		}

		public HttpCookieCollection Cookies
		{
			get	{ return _webCookies; }
		}

		public string Raw
		{
			get	{ return _raw;	}
			set	{ _raw = value;	}
		}

		public string Html
		{
			get { return _html;	}
		}

		public string FormVariables(string name)
		{

			int tagBeg = 0;
			int tagEnd = 0;
			string tagData = null;
			string token = null;
			string tagname = null;

			//  locate the tag by name
			token = new StringBuilder().Append("name=").Append((char)(34)).Append(name).Append((char)(34)).ToString();

			tagBeg = (_html.ToUpper().IndexOf(token.ToUpper(), 0) + 1);
			if (tagBeg == 0)
			{
				//  tag name could not be found
				return System.String.Empty;
			}

			//  get the beginning position of the HTML tag
			tagBeg = _html.IndexOf("<", tagBeg);

			//  get the name of tag type
			tagname = _html.Substring(tagBeg + 1 - 1, 10);

			int posBeg = 0;
			int posEnd = 0;

			if ((tagname.ToUpper().IndexOf(HTTP.TAG_INPUT.ToUpper(), 0) + 1) > 0)
			{
				tagEnd = (_html.ToUpper().IndexOf(">".ToUpper(), tagBeg - 1) + 1);
				if (tagEnd == 0)
				{
					return string.Empty;
				}
				tagData = _html.Substring(tagBeg - 1, tagEnd - tagBeg + 1);
    			return ExtractValue(tagData);
			}

			if ((tagname.ToUpper().IndexOf(HTTP.TAG_SELECT.ToUpper(), 0) + 1) > 0)
			{
				tagEnd = (_html.ToUpper().IndexOf("</SELECT>".ToUpper(), tagBeg - 1) + 1);
				if (tagEnd == 0)
				{
					return string.Empty;
				}

				tagData = _html.Substring(tagBeg - 1, tagEnd - tagBeg + 1);

				//  find the selected option tag
				posBeg = (tagData.ToUpper().IndexOf("SELECTED".ToUpper(), 0) + 1);

				//  go backwards to find the beginning of the selected option tag
				posBeg = tagData.IndexOf("<OPTION", posBeg);

				// find the end of the option tag
				posEnd = (tagData.ToUpper().IndexOf(">".ToUpper(), posBeg - 1) + 1);

				// extract the tag
				tagData = tagData.Substring(posBeg, posEnd - posBeg + 1);

				//  extract the value and return
				return ExtractValue(tagData);
			}

			if ((tagname.ToUpper().IndexOf(HTTP.TAG_TEXTAREA.ToUpper(), 0) + 1) > 0)
			{
				tagEnd = (_html.ToUpper().IndexOf("</TEXTAREA>".ToUpper(), tagBeg - 1) + 1);

				if (tagEnd == 0)
				{
					return string.Empty;
				}

				tagData = _html.Substring(tagBeg - 1, tagEnd - tagBeg);

				//  find the closing part of the first tag
				posBeg = (tagData.ToUpper().IndexOf(">".ToUpper(), 0) + 1);

				// extract the value and return
				return tagData.Substring(posBeg);
			}

			return null;
		}

		private string ExtractValue(string data)
		{
			int valBeg = 0;
			int valEnd = 0;
			string valData = null;

			//valBeg = InStr(data, "value=" & Chr(34), CompareMethod.Text)
			//valBeg = InStr(valBeg, data, Chr(34), CompareMethod.Text)
			//valEnd = InStr(valBeg + 1, data, Chr(34), CompareMethod.Text)

            valBeg = data.ToUpper().IndexOf("VALUE=" + System.Convert.ToChar(0x42));
			valBeg = data.ToUpper().IndexOf("VALUE=" + System.Convert.ToChar(0x42),valBeg);
			valEnd = data.ToUpper().IndexOf("VALUE=" + System.Convert.ToChar(0x42),valBeg + 1);
			valData = data.Substring(valBeg, valEnd - valBeg - 1);

			return valData;
		}

		private int ExtractCode(string line)
		{
			int beg_idx = 0;
			int end_idx = 0;
			string val = null;

			if (line.IndexOf("HTTP") == -1)
				throw new System.ApplicationException("No HTTP response found.");

			beg_idx = line.IndexOf(" ") + 1;
			end_idx = line.IndexOf(" ", beg_idx);

			val = line.Substring(beg_idx, end_idx - beg_idx);
			Int32 code;

			try
			{
				code = Convert.ToInt32(val);
			} 
			catch
			{			
				throw new ApplicationException("Invalid response code received.");
			}

			return code;
	    }

		public void Parse()
		{
			string[] data = null;
			string line = null;
			int beg_idx = 0;
			int end_idx = 0;
			string value = null;
			ResponseCodeOptions code = ResponseCodeOptions.None;
			int idx = 0;
			int i = 0;

			//  get rid of the LF character if it exists and then split the string on all CR
			data = _raw.Replace('\n',' ').Split('\r');
			idx = 0;
			line = data[idx];
			idx += 1;
			code = (ResponseCodeOptions)ExtractCode(line);

			if (code == ResponseCodeOptions.Continue)
			{
				int tempFor1 = data.GetUpperBound(0);
				for (i = idx; i <= tempFor1; i++)
				{
					line = data[i];

					if (line.IndexOf("HTTP") > -1)
					{
						code = (ResponseCodeOptions) ExtractCode(line);
						idx = i + 1;
						break;
					}
				}
			}

			_responseCode = code;
			_responseStatus = (ResponseStatusOptions) System.Convert.ToInt32((int)_responseCode / 100);

			beg_idx = line.IndexOf(" ", end_idx) + 1;
			end_idx = line.Length;

			value = line.Substring(beg_idx, end_idx - beg_idx);
			_responseCodeDescription = value;

			int tempFor2 = data.GetUpperBound(0);
			for (i = idx; i <= tempFor2; i++)
			{
				line = data[i];
				if (line.IndexOf(":") == -1)
				{
					break;
				}

				if (line.IndexOf(HTTP.HEAD_SET_COOKIE) > -1)
				{
					_webCookies.Add(new HttpCookie(line.Substring(HTTP.HEAD_SET_COOKIE.Length + 1)));
				}
				_webHeaders.Add(new HttpHeader(line));
			}
		}
	}

}