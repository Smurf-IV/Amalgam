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

namespace Starksoft.Net.Http
{
	internal sealed class HTTP
	{
		internal const string POST = "Post";
		internal const string GET = "Get";
		internal const string TAG_INPUT = "INPUT";
		internal const string TAG_SELECT = "SELECT";
		internal const string TAG_TEXTAREA = "TEXTAREA";

		//  HTTP header values
		internal const int PORT = 80;
		internal const string VERSION = "1.1";

		internal const string HEAD_HOST = "Host";
		internal const string HEAD_ACCEPT_LANGUAGE = "Accept-Language";
		internal const string HEAD_CONTENT_TYPE = "Content-Type";
		internal const string HEAD_USER_AGENT = "User-Agent";
		internal const string HEAD_CACHE = "Cache-Control";
		internal const string HEAD_REFERER = "Referer";
		internal const string HEAD_CONTENT_LENGTH = "Content-Length";
		internal const string HEAD_COOKIE = "Cookie";
		internal const string HEAD_CONNECTION = "Connection";
		internal const string HEAD_CONNECTION_CLOSE = "Close";
		internal const string HEAD_SET_COOKIE = "Set-Cookie";
		internal const string HEAD_QUERYSTRING = "?";

		internal const string HEAD_ACCEPT_LANGUAGE_VALUE = "en-us";
		internal const string HEAD_CONTENT_TYPE_X_WWW_FORM_VALUE = "application/x-www-form-urlencoded";
		internal const string HEAD_CONTENT_TYPE_HTML = "text/HTML";
		internal const string HEAD_USER_AGENT_VALUE = "Starksoft.Net.Http/1.0"; 
		internal const string HEAD_CACHE_VALUE = "no-cache";
		internal const string HEAD_AUTHORIZATION = "Authorization";
		internal const string HEAD_AUTHORIZATION_TYPE_BASIC = "Basic";

	}
}

