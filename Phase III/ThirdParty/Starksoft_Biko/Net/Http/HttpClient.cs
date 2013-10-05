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
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using System.Threading;
using System.Security.Authentication;
using System.IO.Compression;
using System.Globalization;
using Starksoft.Net.Proxy;


namespace Starksoft.Net.Http
{
    /// <summary>
    /// Http response codes.
    /// </summary>
    /// <remarks>
    /// Informational 1xx.   This class of status code indicates a provisional response, consisting only of the Status-Line and optional headers, and is terminated by an empty line. There are no required headers for this class of status code. Since HTTP/1.0 did not define any 1xx status codes, servers MUST NOT send a 1xx response to an HTTP/1.0 client except under experimental conditions.  A client MUST be prepared to accept one or more 1xx status responses prior to a regular response, even if the client does not expect a 100 (Continue) status message. Unexpected 1xx status responses MAY be ignored by a user agent. 
    /// Successful 2xx.  This class of status code indicates that the client's request was successfully received, understood, and accepted. 
    /// Redirection 3xx. This class of status code indicates that further action needs to be taken by the user agent in order to fulfill the request. The action required MAY be carried out by the user agent without interaction with the user if and only if the method used in the second request is GET or HEAD. A client SHOULD detect infinite redirection loops, since such loops generate network traffic for each redirection.  
    /// Client Error 4xx.  The 4xx class of status code is intended for cases in which the client seems to have erred. Except when responding to a HEAD request, the server SHOULD include an entity containing an explanation of the error situation, and whether it is a temporary or permanent condition. These status codes are applicable to any request method. User agents SHOULD display any included entity to the user. 
    /// ServerError 5xx.  Response status codes beginning with the digit "5" indicate cases in which the server is aware that it has erred or is incapable of performing the request. Except when responding to a HEAD request, the server SHOULD include an entity containing an explanation of the error situation, and whether it is a temporary or permanent condition. User agents SHOULD display any included entity to the user. These response codes are applicable to any request method.
    /// </remarks>
    public enum HttpResponseCode : int
    {
        /// <summary>
        /// (100) The client SHOULD continue with its request. This interim response is used to inform the client that the initial part of the request has been received and has not yet been rejected by the server. The client SHOULD continue by sending the remainder of the request or, if the request has already been completed, ignore this response. The server MUST send a final response after the request has been completed. 
        /// </summary>
        Continue = 100,
        /// <summary>
        /// (101) The client SHOULD continue with its request. This interim response is used to inform the client that the initial part of the request has been received and has not yet been rejected by the server. The client SHOULD continue by sending the remainder of the request or, if the request has already been completed, ignore this response. The server MUST send a final response after the request has been completed
        /// </summary>
        SwitchingProtocols = 101,
        /// <summary>
        /// (102) The server will switch protocols to those defined by the response's Upgrade header field immediately after the empty line which terminates the 101 response. The protocol SHOULD be switched only when it is advantageous to do so. For example, switching to a newer version of HTTP is advantageous over older versions, and switching to a real-time, synchronous protocol might be advantageous when delivering resources that use such features. 
        /// </summary>
        /// <remarks>
        /// WebDAV specific code since a WebDAV request may contina many
        /// sub-requests involving file operations.  See RFC 2518 for more information.
        /// </remarks>
        Processing = 102,
        /// <summary>
        /// (200) The request has succeeded. The information returned with the response is dependent on the method used in the request.
        /// Send after a GET, HEAD, POST, or TRACE command.
        /// </summary>
        Ok = 200,
        /// <summary>
        /// (201) The request has been fulfilled and resulted in a new resource being created. The newly created resource can be referenced by the URI(s) returned in the entity of the response, with the most specific URI for the resource given by a Location header field. The response SHOULD include an entity containing a list of resource characteristics and location(s) from which the user or user agent can choose the one most appropriate. The entity format is specified by the media type given in the Content-Type header field
        /// </summary>
        /// <remarks>
        /// A 201 response MAY contain an ETag response header field indicating the current value of the entity tag for the requested variant just created.
        /// </remarks>
        Created = 201,
        /// <summary>
        /// (202) The request has been accepted for processing, but the processing has not been completed. The request might or might not eventually be acted upon, as it might be disallowed when processing actually takes place. There is no facility for re-sending a status code from an asynchronous operation such as this. 
        /// </summary>
        /// <remarks>
        /// The 202 response is intentionally non-committal. Its purpose is to allow a server to accept a request for some other process (perhaps a batch-oriented process that is only run once per day) without requiring that the user agent's connection to the server persist until the process is completed. The entity returned with this response SHOULD include an indication of the request's current status and either a pointer to a status monitor or some estimate of when the user can expect the request to be fulfilled.
        /// </remarks>
        Accepted = 202,
        /// <summary>
        /// (203) The returned metainformation in the entity-header is not the definitive set as available from the origin server, but is gathered from a local or a third-party copy. The set presented MAY be a subset or superset of the original version. For example, including local annotation information about the resource might result in a superset of the metainformation known by the origin server. Use of this response code is not required and is only appropriate when the response would otherwise be 200 (OK).
        /// </summary>
        NonAuthoritativeInformation = 203,
        /// <summary>
        /// (204) The server has fulfilled the request but does not need to return an entity-body, and might want to return updated metainformation. The response MAY include new or updated metainformation in the form of entity-headers, which if present SHOULD be associated with the requested variant.  
        /// </summary>
        /// <remarks>HTTP/1.1 code.</remarks>
        /// <remarks>The 204 response MUST NOT include a message-body, and thus is always terminated by the first empty line after the header fields. </remarks>
        NoContent = 204,
        /// <summary>
        /// (205) The server has fulfilled the request and the user agent SHOULD reset the document view which caused the request to be sent. This response is primarily intended to allow input for actions to take place via user input, followed by a clearing of the form in which the input is given so that the user can easily initiate another input action. The response MUST NOT include an entity.
        /// </summary>
        ResetContent = 205,
        /// <summary>
        /// (206)  The server has fulfilled the partial GET request for the resource. The request MUST have included a Range header field (section 14.35) indicating the desired range, and MAY have included an If-Range header field (section 14.27) to make the request conditional. 
        /// </summary>
        /// <remarks>
        /// The response MUST include the following header fields: 
        /// - Either a Content-Range header field (section 14.16) indicating
        /// the range included with this response, or a multipart/byteranges
        /// Content-Type including Content-Range fields for each part. If a
        /// Content-Length header field is present in the response, its
        /// value MUST match the actual number of OCTETs transmitted in the
        /// message-body.
        /// - Date
        /// - ETag and/or Content-Location, if the header would have been sent
        /// in a 200 response to the same request
        /// - Expires, Cache-Control, and/or Vary, if the field-value might
        /// differ from that sent in any previous response for the same variant
        /// </remarks>
        PartialContent = 206,
        /// <summary>
        /// (207) The message body that follows is an XML message and can contain a number of separate response codes, depending on how many sub-requests were made.
        /// </summary>
        /// <remarks>This is a WebDAV specific code.  See RFC 2518 for more information.</remarks>
        MultiStatus = 207,
        /// <summary>
        /// (300) The requested resource corresponds to any one of a set of representations, each with its own specific location, and agent- driven negotiation information (section 12) is being provided so that the user (or user agent) can select a preferred representation and redirect its request to that location. 
        /// </summary>
        /// <remarks>
        /// Unless it was a HEAD request, the response SHOULD include an entity containing a list of resource characteristics and location(s) from which the user or user agent can choose the one most appropriate. The entity format is specified by the media type given in the Content- Type header field. Depending upon the format and the capabilities of 
        /// the user agent, selection of the most appropriate choice MAY be performed automatically. However, this specification does not define any standard for such automatic selection. 
        /// If the server has a preferred choice of representation, it SHOULD include the specific URI for that representation in the Location field; user agents MAY use the Location field value for automatic redirection. This response is cacheable unless indicated otherwise. 
        /// </remarks>
        MultipleChoices = 300,
        /// <summary>
        /// (301) The requested resource has been assigned a new permanent URI and any future references to this resource SHOULD use one of the returned URIs. Clients with link editing capabilities ought to automatically re-link references to the Request-URI to one or more of the new references returned by the server, where possible. This response is cacheable unless indicated otherwise. 
        /// </summary>
        /// <remarks>
        /// The new permanent URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s). 
        /// If the 301 status code is received in response to a request other than GET or HEAD, the user agent MUST NOT automatically redirect the request unless it can be confirmed by the user, since this might change the conditions under which the request was issued. 
        /// </remarks>
        MovedPermanently = 301,
        /// <summary>
        /// (302) The requested resource resides temporarily under a different URI. Since the redirection might be altered on occasion, the client SHOULD continue to use the Request-URI for future requests. This response is only cacheable if indicated by a Cache-Control or Expires header field. 
        /// </summary>
        /// <remarks>
        /// The temporary URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s). 
        /// If the 302 status code is received in response to a request other than GET or HEAD, the user agent MUST NOT automatically redirect the request unless it can be confirmed by the user, since this might change the conditions under which the request was issued. 
        /// Note: RFC 1945 and RFC 2068 specify that the client is not allowed
        /// to change the method on the redirected request.  However, most
        /// existing user agent implementations treat 302 as if it were a 303
        /// response, performing a GET on the Location field-value regardless
        /// of the original request method. The status codes 303 and 307 have
        /// been added for servers that wish to make unambiguously clear which
        /// kind of reaction is expected of the client.
        /// </remarks>
        Found = 302,
        /// <summary>
        /// (303) The response to the request can be found under a different URI and SHOULD be retrieved using a GET method on that resource. This method exists primarily to allow the output of a POST-activated script to redirect the user agent to a selected resource. The new URI is not a substitute reference for the originally requested resource. The 303 response MUST NOT be cached, but the response to the second (redirected) request might be cacheable. 
        /// </summary>
        /// <remarks>
        /// HTTP/1.1 code.
        /// The different URI SHOULD be given by the Location field in the response. Unless the request method was HEAD, the entity of the response SHOULD contain a short hypertext note with a hyperlink to the new URI(s). 
        /// Note: Many pre-HTTP/1.1 user agents do not understand the 303
        /// status. When interoperability with such clients is a concern, the
        /// 302 status code may be used instead, since most user agents react
        /// to a 302 response as described here for 303.
        /// </remarks>
        SeeOther = 303,
        /// <summary>
        /// (305) If the client has performed a conditional GET request and access is allowed, but the document has not been modified, the server SHOULD respond with this status code. The 304 response MUST NOT contain a message-body, and thus is always terminated by the first empty line after the header fields. 
        /// </summary>
        /// <remarks>
        ///  The response MUST include the following header fields:
        ///      - Date, unless its omission is required by section 14.18.1
        ///      If a clockless origin server obeys these rules, and proxies and clients add their own Date to any response received without one (as already specified by [RFC 2068], section 14.19), caches will operate correctly.
        ///      - ETag and/or Content-Location, if the header would have been sent
        ///      in a 200 response to the same request
        ///      - Expires, Cache-Control, and/or Vary, if the field-value might
        ///      differ from that sent in any previous response for the same
        ///      variant
        ///If the conditional GET used a strong cache validator (see section 13.3.3), the response SHOULD NOT include other entity-headers. Otherwise (i.e., the conditional GET used a weak validator), the response MUST NOT include other entity-headers; this prevents inconsistencies between cached entity-bodies and updated headers.
        ///If a 304 response indicates an entity not currently cached, then the cache MUST disregard the response and repeat the request without the conditional.
        ///If a cache uses a received 304 response to update a cache entry, the cache MUST update the entry to reflect any new field values given in the response. 
        /// </remarks>
        NotModified = 304,
        /// <summary>
        /// (305) 
        /// </summary>
        UseProxy = 305,
        /// <summary>
        /// (306)
        /// </summary>
        Reserved1 = 306,
        /// <summary>
        /// (307) 
        /// </summary>
        TemporaryRedirect = 307,
        /// <summary>
        /// (400) 
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// (401) 
        /// </summary>
        Unauthorized = 401,
        /// <summary>
        /// (402) 
        /// </summary>
        PaymentRequired = 402,
        /// <summary>
        /// (403) 
        /// </summary>
        Forbidden = 403,
        /// <summary>
        /// (404) 
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// (405) 
        /// </summary>
        MethodNotAllowed = 405,
        /// <summary>
        /// (406) 
        /// </summary>
        NotAcceptable = 406,
        /// <summary>
        /// (407) 
        /// </summary>
        ProxyAuthenticationRequired = 407,
        /// <summary>
        /// (408) 
        /// </summary>
        RequestTimeout = 408,
        /// <summary>
        /// (409) 
        /// </summary>
        Conflict = 409,
        /// <summary>
        /// (410) 
        /// </summary>
        Gone = 410,
        /// <summary>
        /// (411) 
        /// </summary>
        LengthRequired = 411,
        /// <summary>
        /// (412) 
        /// </summary>
        PreconditionFailed = 412,
        /// <summary>
        /// (413) 
        /// </summary>
        RequestEntityTooLarge = 413,
        /// <summary>
        /// (414) 
        /// </summary>
        RequestUriTooLong = 414,
        /// <summary>
        /// (415) 
        /// </summary>
        UnsupportedMediaType = 415,
        /// <summary>
        /// (416) 
        /// </summary>
        RequestedRangeNotSatisfiable = 416,
        /// <summary>
        /// (417) 
        /// </summary>
        ExpectationFailed = 417,
        /// <summary>
        /// (500) 
        /// </summary>
        InternalServerError = 500,
        /// <summary>
        /// (501) 
        /// </summary>
        NotImplemented = 501,
        /// <summary>
        /// (502) 
        /// </summary>
        BadGateway = 502,
        /// <summary>
        /// (503) 
        /// </summary>
        ServiceUnavailable = 503,
        /// <summary>
        /// (504) 
        /// </summary>
        GatewayTimeout = 504,
        /// <summary>
        /// (505) 
        /// </summary>
        HttpVersionNotSupported = 505
    }
    
    /// <summary>
    /// Defines the possible versions of HttpSecurityProtocol.
    /// </summary>
    public enum HttpSecurityProtocol : int
    {
        /// <summary>
        /// No security protocol specified.
        /// </summary>
        None,
        /// <summary>
        /// Specifies Transport Layer Security (TLS) version 1.0 is required to secure communciations.  The TLS protocol is defined in IETF RFC 2246 and supercedes the SSL 3.0 protocol.
        /// </summary>
        Tls1,
        /// <summary>
        /// Specifies Transport Layer Security (TLS) version 1.0. or Secure Socket Layer (SSL) version 3.0 is acceptable to secure communications
        /// </summary>
        Tls1OrSsl3,
        /// <summary>
        /// Specifies Secure Socket Layer (SSL) version 3.0 is required to secure communications.  SSL 3.0 has been superseded by the TLS protocol
        /// and is provided for backward compatibility only
        /// </summary>
        Ssl3,
        /// <summary>
        /// Specifies Secure Socket Layer (SSL) version 2.0 is required to secure communications.  SSL 2.0 has been superseded by the TLS protocol
        /// and is provided for backward compatibility only.  SSL 2.0 has several weaknesses and should only be used with legacy Http server that require it.
        /// </summary>
        Ssl2
    }

    public enum TransferDirection
    {
        ToServer,
        ToClient
    }

    public class HttpClient : IDisposable
	{
		private HttpRequest _webRequest = new HttpRequest();
		private HttpResponse _webResponse = new HttpResponse();

		private string _url;
		private int _port;
		private string _host;
		private string _abspath;

		private string TARGET_DEFAULT = "/";
		private string HTTP_PREFIX = "http://";

        #region Public Events

        /// <summary>
        /// Server response event.
        /// </summary>
        public event EventHandler<HttpResponseEventArgs> ServerResponse;

        /// <summary>
        /// Server request event.
        /// </summary>
        public event EventHandler<HttpRequestEventArgs> ClientRequest;

        /// <summary>
        /// Data transfer progress event.
        /// </summary>
        public event EventHandler<TransferProgressEventArgs> TransferProgress;

        /// <summary>
        /// Data transfer complete event.
        /// </summary>
        public event EventHandler<TransferCompleteEventArgs> TransferComplete;

        /// <summary>
        /// Security certificate authentication event.
        /// </summary>
        public event EventHandler<ValidateServerCertificateEventArgs> ValidateServerCertificate;

        /// <summary>
        /// Connection closed event.
        /// </summary>
        public event EventHandler<ConnectionClosedEventArgs> ConnectionClosed;

        #endregion

        #region Private Variables and Constants

        private Stream _commandStream;
        private TcpClient _commandConn;

        private IProxyClient _proxy;

        private int _tcpBufferSize = TCP_BUFFER_SIZE;
        private int _tcpTimeout = TCP_TIMEOUT;
        private int _maxUploadSpeed;
        private int _maxDownloadSpeed;

        private int _transferTimeout = TRANSFER_TIMEOUT;

        // secure communications specific 
        private HttpSecurityProtocol _securityProtocol = HttpSecurityProtocol.None;
        private X509Certificate2 _serverCertificate;
        private X509CertificateCollection _clientCertificates = new X509CertificateCollection();
        
        // data compresion specific
        private bool _isCompressionEnabled;

        // async background worker event-based object
        private BackgroundWorker _asyncWorker;
        private bool _asyncCanceled;

        private const int TCP_BUFFER_SIZE = 8192;
        private const int TCP_TIMEOUT = 30000; // 30 seconds

        private const int WAIT_FOR_DATA_INTERVAL = 10; // 10 ms
        private const int TRANSFER_TIMEOUT = 15000; // 15 seconds


        #endregion        

        #region Constructors
        public HttpClient(string url, int port, string username, string password) 
		{
            _url = url;
            _port = port;
            extractHost();
            _webRequest.Host = _host;
			_webRequest.AbsPath = _abspath;
			_webRequest.Authorization.Password = password;
			_webRequest.Authorization.UserName = username;
			_webRequest.Authorization.AuthenicationType = HttpAuthorization.AuthType.Basic;
        }
        

        public HttpClient(string url, int port) : this(url, port, "", "")
		{	}

		public HttpClient(string url, string username, string password) : this(url, HTTP.PORT, username, password)
        {	}

		public HttpClient(string url) : this(url, HTTP.PORT, "", "")
		{	}
        
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether an asynchronous operation is canceled.
        /// </summary>
        /// <remarks>Returns true if an asynchronous operation is canceled; otherwise, false.
        /// </remarks>
        public bool IsAsyncCanceled
        {
            get { return _asyncCanceled; }
        }

        /// <summary>
        /// Gets a value indicating whether an asynchronous operation is running.
        /// </summary>
        /// <remarks>Returns true if an asynchronous operation is running; otherwise, false.
        /// </remarks>
        public bool IsBusy
        {
            get { return _asyncWorker == null ? false : _asyncWorker.IsBusy; }
        }

        /// <summary>
        /// Gets or sets the current port number used by the FtpClient to make a connection to the Http server.
        /// </summary>
        /// <remarks>
        /// The default value is '80'.  This setting can only be changed when the 
        /// connection to the Http server is closed.  And HttpException is thrown if this 
        /// setting is changed when the Http server connection is open.
        /// 
        /// Returns an integer representing the port number used to connect to a remote server.
        /// </remarks>
        public int Port
        {
            get { return _port; }
            set
            {
                if (this.IsConnected)
                    throw new HttpException("Port property value can not be changed when connection is open.");

                _port = value;
            }
        }

        /// <summary>
        /// Gets or sets a text value containing the current host used by the FtpClient to make a connection to the Http server.
        /// </summary>
        /// <remarks>
        /// This value may be in the form of either a host name or IP address.
        /// This setting can only be changed when the 
        /// connection to the Http server is closed.  And HttpException is thrown if this 
        /// setting is changed when the Http server connection is open.
        /// 
        /// Returns a string with either the host name or host ip address.
        /// </remarks>
        public string Host
        {
            get { return _host; }
            set
            {
                if (this.IsConnected)
                    throw new HttpException("Host property value can not be changed when connection is open.");

                _host = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating what security protocol such as Secure Sock Layer (SSL) should be used.
        /// </summary>
        /// <remarks>
        /// The default value is 'None'.  This setting can only be changed when the 
        /// connection to the Http server is closed.  An HttpException is thrown if this 
        /// setting is changed when the Http server connection is open.
        /// 
        /// Returns an enumerator specifying the choosen security protocol of either TLS v1.0, SSL v3.0 or SSL v2.0.
        /// </remarks>
        /// <seealso cref="SecurityCertificates"/>
        /// <seealso cref="ValidateServerCertificate" />
        public HttpSecurityProtocol SecurityProtocol
        {
            get { return _securityProtocol; }
            set
            {
                if (this.IsConnected)
                    throw new HttpException("SecurityProtocol property value can not be changed when connection is open.");

                _securityProtocol = value;
            }
        }

        /// <summary>
        /// Get Client certificate collection used when connection with a secured SSL/TSL protocol.  Add your client certificates 
        /// if required to connect to the remote Http server.
        /// </summary>
        /// <remarks>Returns a X509CertificateCollection list contains X.509 security certificates.</remarks>
        /// <seealso cref="SecurityProtocol"/>
        /// <seealso cref="ValidateServerCertificate" />
        public X509CertificateCollection SecurityCertificates
        {
            get { return _clientCertificates; }
        }

        /// <summary>
        /// Gets or sets a value indicating that the client will use compression when uploading and downloading
        /// data.
        /// </summary>
        /// <remarks>
        /// This value turns on or off the compression algorithm DEFLATE to facility FTP data compression which is compatible with
        /// Http servers that implement compression via the zLib compression software library.  The default value is 'False'.  
        /// This setting can only be changed when the system is not busy conducting other operations.  
        /// 
        /// Returns True if compression is enabled; otherwise False;
        /// </remarks>
        public bool IsCompressionEnabled
        {
            get { return _isCompressionEnabled; }
            set { _isCompressionEnabled = value;  }
        }

        /// <summary>
        /// Gets or sets an Integer value representing the maximum upload speed allowed 
        /// for data transfers in kilobytes per second.
        /// </summary>
        /// <remarks>
        /// Set this value when you would like to throttle back any upload data transfers.
        /// A value of zero means there is no restriction on how fast data uploads are 
        /// conducted.  The default value is zero.  This setting is used to throttle data traffic so the FtpClient does
        /// not consume all available network bandwidth.
        /// </remarks>
        /// <seealso cref="MaxDownloadSpeed"/>
        public int MaxUploadSpeed
        {
            get { return _maxUploadSpeed; }
            set
            {
                if (value * 1024 > Int32.MaxValue || value < 0)
                    throw new ArgumentOutOfRangeException("value", "The MaxUploadSpeed property must have a range of 0 to 2,097,152.");

                _maxUploadSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets an Integer value representing the maximum download speed allowed 
        /// for data transfers in kilobytes per second.
        /// </summary>
        /// <remarks>
        /// Set this value when you would like to throttle back any download data transfers.
        /// A value of zero means there is no restriction on how fast data uploads are 
        /// conducted.  The default value is zero.  This setting is used to throttle data traffic so the FtpClient does
        /// not consume all available network bandwidth.
        /// </remarks>
        /// <seealso cref="MaxUploadSpeed"/>
        public int MaxDownloadSpeed
        {
            get { return _maxDownloadSpeed; }
            set
            {
                if (value * 1024 > Int32.MaxValue || value < 0)
                    throw new ArgumentOutOfRangeException("value", "must have a range of 0 to 2,097,152.");

                _maxDownloadSpeed = value;
            }
        }

        /// <summary>
        /// Gets or sets the TCP buffer size used when communicating with the Http server in bytes.
        /// </summary>
        /// <remarks>Returns an integer value representing the buffer size.  The default value is 8192.</remarks>
        public int TcpBufferSize
        {
            get { return _tcpBufferSize; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "must be greater than 0.");

                _tcpBufferSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the TCP timeout used when communciating with the Http server in milliseconds.
        /// </summary>
        /// <remarks>
        /// Default value is 30000 (30 seconds).
        /// </remarks>
        /// <seealso cref="TransferTimeout"/>
        public int TcpTimeout
        {
            get { return _tcpTimeout; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "must be greater than or equal to 0.");

                _tcpTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the data transfer timeout used when communicating with the Http server in milliseconds.
        /// </summary>
        /// <remarks>
        /// Default value is 15000 (15 seconds).
        /// </remarks>
        /// <seealso cref="TcpTimeout"/>
        public int TransferTimeout
        {
            get { return _transferTimeout; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "must be greater than 0.");

                _transferTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the the proxy object to use when establishing a connection to the remote Http server.
        /// </summary>
        /// <remarks>Create a proxy object when traversing a firewall.</remarks>
        /// <code>
        /// FtpClient ftp = new FtpClient();
        ///
        /// // create an instance of the client proxy factory for the an ftp client
        /// ftp.Proxy = (new ProxyClientFactory()).CreateProxyClient(ProxyType.Http, "localhost", 6588);
        ///        
        /// </code>
        /// <seealso cref="Starksoft.Net.Proxy.ProxyClientFactory"/>
        public IProxyClient Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        /// <summary>
        /// Gets the connection status to the Http server.
        /// </summary>
        /// <remarks>Returns True if the connection is open; otherwise False.</remarks>
        /// <seealso cref="ConnectionClosed"/>
        public bool IsConnected
        {
            get
            {
                if (_commandConn == null || _commandConn.Client == null)
                    return false;

                Socket client = _commandConn.Client;

                if (!client.Connected)
                    return false;

                // this is how you can determine whether a socket is still connected.
                bool blockingState = client.Blocking;
                bool connected = true;
                try
                {
                    byte[] tmp = new byte[1];

                    client.Blocking = false;
                    client.Send(tmp, 0, 0);
                }
                catch (SocketException e)
                {
                    // 10035 == WSAEWOULDBLOCK
                    if (!e.NativeErrorCode.Equals(10035))
                    {
                        connected = false;
                    }
                }
                catch (ObjectDisposedException)
                {
                    connected = false;
                }
                finally
                {
                    client.Blocking = blockingState;
                }

                return connected;

            }
        }

		public HttpRequest Request
		{
			get	{ return _webRequest; }
		}

		public HttpResponse Response
		{
			get	{ return _webResponse;	}
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Cancels any asychronous operation that is currently active.
        /// </summary>
        public void CancelAsync()
        {
            if (_asyncWorker != null && !_asyncWorker.CancellationPending && _asyncWorker.IsBusy)
            {
                _asyncCanceled = true;
                _asyncWorker.CancelAsync();
            }
        }

        //public void Send()
        //{
        //    SendHttpData();
        //}

        #endregion
        
        #region Private Methods

        //private void SendHttpData()
        //{
        //    byte[] request = Encoding.ASCII.GetBytes(_webRequest.Raw);
        //    byte[] buffer = new byte[257];
        //    string response = null;

        //    //   IPAddress and IPEndPoint represent the endpoint that will
        //    //   receive the request.
        //    //   Gets the first IPAddress in the list using DNS.
        //    IPAddress hostadd = Dns.GetHostByName(_host).AddressList[0];
        //    IPEndPoint EPhost = new IPEndPoint(hostadd, _port);

        //    //   Creates the Socket for sending data over TCP.
        //    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //    //   Connects to the host using IPEndPoint.
        //    s.Connect(EPhost);

        //    if (!s.Connected)
        //    {
        //        throw new HttpException("Unable to connect to host '" + _host + "' on port " + _port);
        //    }

        //    //   Sends the GET text to the host
        //    s.Send(request, request.Length, 0);

        //    //   Receives the page, looping until all bytes are received
        //    Int32 bytes = s.Receive(buffer, buffer.Length, 0);

        //    response = response + Encoding.ASCII.GetString(buffer, 0, bytes);

        //    while (bytes > 0)
        //    {
        //        bytes = s.Receive(buffer, buffer.Length, 0);
        //        response = response + Encoding.ASCII.GetString(buffer, 0, bytes);
        //    }

        //    s.Close();
        //    _webResponse.Raw = response;
        //    _webResponse.Parse();
        //}
        
        private void extractHost()
        {
            string temp = _url;

            if (temp.ToLower().IndexOf(HTTP_PREFIX) > -1)
            {
                temp = temp.Substring(HTTP_PREFIX.Length);
            }

            //  find the separator
            int pos = temp.IndexOf("/");

            if (pos > -1)
            {
                _host = temp.Substring(0, pos);
                _abspath = temp.Substring(pos);
            }
            else
            {
                //  if there is no target specified the use the default.
                _host = temp;
                _abspath = TARGET_DEFAULT;
            }
        }
        
        /// <summary>
        /// Send a Http command request to the server.
        /// </summary>
        /// <param name="request"></param>
        private void SendRequest(HttpRequest request)
        {
            if (_commandConn == null || _commandConn.Connected == false)
                throw new HttpConnectionClosedException("Connection is closed.");

            // clear out any responses that might have been pending from a previous
            // failed operation
            //DontWaitForHappyCodes();

            if (ClientRequest != null)
                ClientRequest(this, new HttpRequestEventArgs(request));

            byte[] buffer = request.GetBytes();

            try
            {
                _commandStream.Write(buffer, 0, buffer.Length);
            }
            catch (IOException ex)
            {
                throw new HttpConnectionBrokenException("Connection is broken.  Failed to send command.", ex);
            }


        }
        
        /// <summary>
        /// creates a new async worker object for the async events to use.
        /// </summary>
        private void CreateAsyncWorker()
        {
            if (_asyncWorker != null)
                _asyncWorker.Dispose();
            _asyncWorker = null;
            _asyncCanceled = false;
            _asyncWorker = new BackgroundWorker();
        }

        //  open a connection to the server
        private void OpenCommandConn()
        {
            //  create a new tcp client object 
            CreateCommandConnection();

            //StartCommandMonitorThread();

            if (_securityProtocol != HttpSecurityProtocol.None)
                CreateSslCommandStream();

            // test to see if this is an asychronous operation and if so make sure 
            // the user has not requested the operation to be canceled
            if (IsAsyncCancellationPending())
                return;

            //// this check screws up secure connections so we have to ignore it when secure connections are enabled
            //if (_securityProtocol == HttpSecurityProtocol.None)
            //    WaitForHappyCodes(HttpResponseCode.ServiceReadyForNewUser);

        }

        private void TransferData(TransferDirection direction, HttpRequest request, Stream data, long restartPosition)
        {
            if (_commandConn == null || _commandConn.Connected == false)
                throw new HttpConnectionClosedException("Connection is closed.");

            if (request == null)
                throw new ArgumentNullException("request", "value is required");

            if (data == null)
                throw new ArgumentNullException("data", "value is required");

            switch (direction)
            {
                case TransferDirection.ToClient:
                    if (!data.CanWrite)
                        throw new HttpDataTransferException("Data transfer error.  Data conn does not allow write operation.");
                    break;
                case TransferDirection.ToServer:
                    if (!data.CanRead)
                        throw new HttpDataTransferException("Data transfer error.  Data conn does not allow read operation.");
                    break;
            }

            try
            {
                // create a thread to begin the process of opening a data connection to the remote server
                OpenCommandConn();

                // send the data transfer command that requires a separate data connection to be established to transmit data
                SendRequest(request);

                // test to see if we need to enable compression by using the DeflateStream
                if (_isCompressionEnabled)
                {
                    _commandStream = CreateZlibStream(direction, _commandStream);
                }

                // based on the direction of the data transfer we need to handle the input and output streams
                switch (direction)
                {
                    case TransferDirection.ToClient:
                        TransferBytes(_commandStream, data, _maxDownloadSpeed * 1024);
                        break;
                    case TransferDirection.ToServer:
                        TransferBytes(data, _commandStream, _maxUploadSpeed * 1024);
                        break;
                }
            }
            finally
            {
                // attempt to close the data connection
                CloseCommandConn();
            }

            // if no errors occurred and this is not a quoted command then we will wait for the server to send a closing connection message
            //WaitForHappyCodes(HttpResponseCode.ClosingDataConnection);
        }

        private Stream CreateZlibStream(TransferDirection direction, Stream stream)
        {
            DeflateStream deflateStream = null;

            switch (direction)
            {
                case TransferDirection.ToClient:
                    deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true);
                    // zlib fix to ignore first two bytes of header data 
                    deflateStream.BaseStream.ReadByte();
                    deflateStream.BaseStream.ReadByte();
                    break;

                case TransferDirection.ToServer:
                    deflateStream = new DeflateStream(stream, CompressionMode.Compress, true);
                    // this is a fix for the DeflateStream class only when sending compressed data to the server.  
                    // Zlib has two bytes of data attached to the header that we have to write before processing the data stream.
                    deflateStream.BaseStream.WriteByte(120);
                    deflateStream.BaseStream.WriteByte(218);
                    break;
            }
            stream = deflateStream;
            return stream;
        }

        private bool IsAsyncCancellationPending()
        {
            if (_asyncWorker != null && _asyncWorker.CancellationPending)
            {
                _asyncCanceled = true;
                return true;
            }
            return false;
        }

        private void TransferBytes(Stream input, Stream output, int maxBytesPerSecond)
        {
            int bufferSize = _tcpBufferSize > maxBytesPerSecond && maxBytesPerSecond != 0 ? maxBytesPerSecond : _tcpBufferSize;
            byte[] buffer = new byte[bufferSize];
            long bytesTotal = 0;
            int bytesRead = 0;
            DateTime start = DateTime.Now;
            TimeSpan elapsed;
            int bytesPerSec = 0;

            do
            {
                bytesRead = input.Read(buffer, 0, bufferSize);
                bytesTotal += bytesRead;
                output.Write(buffer, 0, bytesRead);

                // calculate some statistics
                elapsed = DateTime.Now.Subtract(start);
                bytesPerSec = (int)(elapsed.TotalSeconds < 1 ? bytesTotal : bytesTotal / elapsed.TotalSeconds);

                //  if the consumer subscribes to transfer progress event then fire it
                if (TransferProgress != null)
                    TransferProgress(this, new TransferProgressEventArgs(bytesRead, bytesPerSec, elapsed));

                // test to see if this is an asychronous operation and if so make sure 
                // the user has not requested the operation to be canceled
                if (IsAsyncCancellationPending())
                    throw new HttpAsynchronousOperationException("Asynchronous operation canceled by user.");

                // throttle the transfer if necessary
                ThrottleByteTransfer(maxBytesPerSecond, bytesTotal, elapsed, bytesPerSec);

            } while (bytesRead > 0);

            //  if the consumer subscribes to transfer complete event then fire it
            if (TransferComplete != null)
                TransferComplete(this, new TransferCompleteEventArgs(bytesTotal, bytesPerSec, elapsed));
        }

        private void ThrottleByteTransfer(int maxBytesPerSecond, long bytesTotal, TimeSpan elapsed, int bytesPerSec)
        {
            // we only throttle if the maxBytesPerSecond is not zero (zero turns off the throttle)
            if (maxBytesPerSecond > 0)
            {
                // we only throttle if our through-put is higher than what we want
                if (bytesPerSec > maxBytesPerSecond)
                {
                    double elapsedMilliSec = elapsed.TotalSeconds == 0 ? elapsed.TotalMilliseconds : elapsed.TotalSeconds * 1000;

                    // need to calc a delay in milliseconds for the throttle wait based on how fast the 
                    // transfer is relative to the speed it needs to be
                    double millisecDelay = (bytesTotal / (maxBytesPerSecond / 1000) - elapsedMilliSec);
                    
                    // can only sleep to a max of an Int32 so we need to check this since bytesTotal is a long value
                    // this should never be an issue but never say never
                    if (millisecDelay > Int32.MaxValue)
                        millisecDelay = Int32.MaxValue;

                    // go to sleep
                    Thread.Sleep((int)millisecDelay);
                }
            }
        }
        
        private void RaiseServerResponseEvent(HttpResponse response)
        {
            if (ServerResponse != null)
                ServerResponse(this, new HttpResponseEventArgs(response));
        }

        private void RaiseConnectionClosedEvent()
        {
            if (ConnectionClosed != null)
                ConnectionClosed(this, new ConnectionClosedEventArgs());
        }

        

        private string[] SplitResponse(string response)
        {
            char[] crlfSplit = new char[2];
            crlfSplit[0] = '\r';
            crlfSplit[1] = '\n';
            return response.Split(crlfSplit, StringSplitOptions.RemoveEmptyEntries);
        }


        private void CreateCommandConnection()
        {
            if (_host == null || _host.Length == 0)
                throw new HttpException("An FTP Host must be specified before opening connection to FTP destination.  Set the appropriate value using the Host property on the FtpClient object.");

            try
            {
                //  test to see if we should use the user supplied proxy object
                //  to create the connection
                if (_proxy != null)
                    _commandConn = _proxy.CreateConnection(_host, _port);
                else
                    _commandConn = new TcpClient(_host, _port);
            }
            catch (ProxyException pex)
            {
                if (_commandConn != null)
                    _commandConn.Close();

                throw new ProxyException(String.Format(CultureInfo.InvariantCulture, "A proxy error occurred while creating connection to FTP destination {0} on port {1}.", _host, _port.ToString(CultureInfo.InvariantCulture)), pex);
            }
            catch (Exception ex)
            {
                if (_commandConn != null)
                    _commandConn.Close();

                throw new HttpConnectionOpenException(String.Format(CultureInfo.InvariantCulture, "An error occurred while opening a connection to FTP destination {0} on port {1}.", _host, _port.ToString(CultureInfo.InvariantCulture)), ex);
            }

            // set command connection buffer sizes and timeouts
            _commandConn.ReceiveBufferSize = _tcpBufferSize;
            _commandConn.ReceiveTimeout = _tcpTimeout;
            _commandConn.SendBufferSize = _tcpBufferSize;
            _commandConn.SendTimeout = _tcpTimeout;

            // set the command stream object
            _commandStream = _commandConn.GetStream();
        }

        /// <summary>
        /// Creates an SSL or TLS secured stream.
        /// </summary>
        /// <param name="stream">Unsecured stream.</param>
        /// <returns>Secured stream</returns>
        private Stream CreateSslStream(Stream stream)
        {
            // create an SSL or TLS stream that will close the client's stream
            SslStream ssl = new SslStream(stream, true, new RemoteCertificateValidationCallback(secureStream_ValidateServerCertificate), null);

            // choose the protocol
            SslProtocols protocol = SslProtocols.None;
            switch (_securityProtocol)
            {
                case HttpSecurityProtocol.Tls1OrSsl3:
                    protocol = SslProtocols.Default;
                    break;
                case HttpSecurityProtocol.Ssl2:
                    protocol = SslProtocols.Ssl2;
                    break;
                case HttpSecurityProtocol.Ssl3:
                    protocol = SslProtocols.Ssl3;
                    break;
                case HttpSecurityProtocol.Tls1:
                    protocol = SslProtocols.Tls;
                    break;
                default:
                    throw new HttpSecureConnectionException(String.Format("Unhandled HttpSecurityProtocol type '{0}'.", _securityProtocol.ToString()));
            }

            // The server name must match the name on the server certificate.
            try
            {
                // authenicate the client
                ssl.AuthenticateAsClient(_host, _clientCertificates, protocol, true);
            }
            catch (AuthenticationException authEx)
            {
                throw new HttpAuthenticationException("Secure FTP session certificate authentication failed.", authEx);
            }

            return ssl;
        }

        // the following method is invoked by the RemoteCertificateValidationDelegate.
        private bool secureStream_ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //// if it is the same certificate we have already approved then don't validate it again
            if (_serverCertificate != null && certificate.GetCertHashString() == _serverCertificate.GetCertHashString())
                return true;

            // invoke the ValidateServerCertificate event if the user is subscribing to it
            // ignore our own logic and let the user decide if the certificate is valid or not
            if (ValidateServerCertificate != null)
            {
                ValidateServerCertificateEventArgs args = new ValidateServerCertificateEventArgs(new X509Certificate2(certificate.GetRawCertData()), chain, sslPolicyErrors);
                ValidateServerCertificate(this, args);
                // make a copy of the certificate due to sharing violations
                if (args.IsCertificateValid)
                    _serverCertificate = new X509Certificate2(certificate.GetRawCertData());
                return args.IsCertificateValid;
            }
            else
            {
                // analyze the policy errors and decide if the certificate should be accepted or not.
                if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateNameMismatch) == SslPolicyErrors.RemoteCertificateNameMismatch)
                    throw new HttpCertificateValidationException(String.Format("Certificate validation failed.  The host name '{0}' does not match the name on the security certificate '{1}'.  To override this behavior, subscribe to the ValidateServerCertificate event to validate certificates.", _host, certificate.Issuer));

                if (sslPolicyErrors == SslPolicyErrors.None || (sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    // make a copy of the server certificate due to sharing violations
                    _serverCertificate = new X509Certificate2(certificate.GetRawCertData());
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        
        private void CreateSslCommandStream()
        {
 
                // set the active command stream to the ssl command stream object
                _commandStream = CreateSslStream(_commandConn.GetStream());
        }

        private void CloseCommandConn()
        {
            if (_commandConn == null)
                return;
            try
            {
                _commandConn.Close();
            }
            catch { }

            _commandConn = null;
        }

        #endregion

        #region Destructors

        /// <summary>
        /// Disposes all objects and connections.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Dispose Method.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_asyncWorker != null && _asyncWorker.IsBusy)
                    _asyncWorker.CancelAsync();

                if (_commandConn != null && _commandConn.Connected)
                    _commandConn.Close();
            }
        }

        /// <summary>
        /// Dispose deconstructor.
        /// </summary>
        ~HttpClient()
        {
            Dispose(false);
        }

        #endregion


	}

}