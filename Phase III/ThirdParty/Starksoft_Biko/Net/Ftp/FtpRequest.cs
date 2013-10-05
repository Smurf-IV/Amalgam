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
using System.Globalization;

namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// FTP server commands.
    /// </summary>
    public enum FtpCmd
    {
        /// <summary>
        /// Unknown command issued.
        /// </summary>
        Unknown,
        /// <summary>
        /// The USER command.
        /// </summary>
        User,
        /// <summary>
        /// The PASS command.
        /// </summary>
        Pass,
        /// <summary>
        /// The MKD command.  Make new directory.
        /// </summary>
        Mkd,
        /// <summary>
        /// The RMD command.  Remove directory.
        /// </summary>
        Rmd,
        /// <summary>
        /// The RETR command.  Retrieve file.
        /// </summary>
        Retr,
        /// <summary>
        /// The PWD command.  Print working directory.
        /// </summary>
        Pwd,
        /// <summary>
        /// The SYST command.  System status.
        /// </summary>
        Syst,
        /// <summary>
        /// The CDUP command.  Change directory up.
        /// </summary>
        Cdup,
        /// <summary>
        /// The DELE command.  Delete file or directory.
        /// </summary>
        Dele,
        /// <summary>
        /// The TYPE command.  Transfer type.
        /// </summary>
        Type,
        /// <summary>
        /// The CWD command.  Change working directory.
        /// </summary>
        Cwd,
        /// <summary>
        /// The PORT command.  Data port.
        /// </summary>
        Port,
        /// <summary>
        /// The PASV command.  Passive port.
        /// </summary>
        Pasv,
        /// <summary>
        /// The STOR command.  Store file.
        /// </summary>
        Stor,
        /// <summary>
        /// The STOU command.  Store file unique.
        /// </summary>
        Stou,
        /// <summary>
        /// The APPE command.  Append file.
        /// </summary>
        Appe,
        /// <summary>
        /// The RNFR command.  Rename file from.
        /// </summary>
        Rnfr,
        /// <summary>
        /// The RFTO command.  Rename file to.
        /// </summary>
        Rnto,
        /// <summary>
        /// The ABOR command.  Abort current operation.
        /// </summary>
        Abor,
        /// <summary>
        /// The LIST command.  List files.
        /// </summary>
        List,
        /// <summary>
        /// The NLST command.  Namelist files.
        /// </summary>
        Nlst,
        /// <summary>
        /// The SITE command.  Site.
        /// </summary>
        Site,
        /// <summary>
        /// The STAT command.  Status.
        /// </summary>
        Stat,
        /// <summary> 
        /// The NOOP command.  No operation.
        /// </summary>
        Noop,
        /// <summary>
        /// The HELP command.  Help.
        /// </summary>
        Help,
        /// <summary>
        /// The ALLO command.  Allocate space.
        /// </summary>
        Allo,
        /// <summary>
        /// The QUIT command.  Quite session.
        /// </summary>
        Quit,
        /// <summary>
        /// The REST command.  Restart transfer.
        /// </summary>
        Rest,
        /// <summary>
        /// The AUTH command.  Initialize authentication.
        /// </summary>
        Auth,
        /// <summary>
        /// The PBSZ command.
        /// </summary>
        Pbsz,
        /// <summary>
        /// The PROT command.  Security protocol.
        /// </summary>
        Prot,
        /// <summary>
        /// The MODE command.  Data transfer mode.
        /// </summary>
        Mode,
        /// <summary>
        /// The MDTM command.  Month Day Time command.
        /// </summary>
        Mdtm,
        /// <summary>
        /// The SIZE command.  File size.
        /// </summary>
        /// <remarks>
        /// This command retrieves the size of the file as stored on the FTP server.  Not all FTP servers
        /// support this command.
        /// </remarks>
        Size,
        /// <summary>
        /// The FEAT command.  Supported features.
        /// </summary>
        /// <remarks>
        /// This command gets a list of supported features from the FTP server.  The feature list may contain
        /// extended commands in addition to proprietrary commands that are not defined in RFC documents.
        /// </remarks>
        Feat,
        /// <summary>
        /// The XCRC command.  CRC file accuracy testing.
        /// </summary>
        /// <remarks>
        /// This is a non-standard command not supported by all FTP servers and not defined in RFC documents.
        /// </remarks>
        Xcrc,
        /// <summary>
        /// The XMD5 command.  MD5 file integrity hashing.
        /// </summary>
        /// <remarks>
        /// This is a non-standard command not supported by all FTP servers and not defined in RFC documents.
        /// </remarks>
        Xmd5,
        /// <summary>
        /// The XSHA1 command.  SHA1 file integerity hashing.
        /// </summary>
        /// <remarks>
        /// This is a non-standard command not supported by all FTP servers and not defined in RFC documents.
        /// </remarks>
        Xsha1,
        /// <summary>
        /// The XSHA256 command.  SHA-256 file integerity hashing.
        /// </summary>
        /// <remarks>
        /// This is a non-standard command not supported by all FTP servers and not defined in RFC documents.
        /// </remarks>
        Xsha256,
        /// <summary>
        /// The XSHA512 command.  SHA-512 file integerity hashing.
        /// </summary>
        /// <remarks>
        /// This is a non-standard command not supported by all FTP servers and not defined in RFC documents.
        /// </remarks>
        Xsha512,
        /// <summary>
        /// The EPSV command.  Extended passive command.
        /// </summary>
        /// <remarks>
        /// The EPSV command is an extended PASV command that supports both IPv4 and IPv6 network addressing and is defined
        /// in the RFC 2428 document.
        /// </remarks>
        Epsv,
        /// <summary>
        /// The EPRT command.  Extended port command.
        /// </summary>
        /// <remarks>
        /// The EPRT command is an extended PORT command that supports both IPv4 and IPv6 network addressing and is defined
        /// in the RFC 2428 document.
        /// </remarks>
        Eprt,
        /// <summary>
        /// The MFMT command. Modify File Modification Time command.
        /// </summary>
        Mfmt,
        /// <summary>
        /// The MFCT command. Modify File Creation Time command.
        /// </summary>
        Mfct,
        /// <summary>
        /// The OPTS command.  This command allows an FTP client to define a parameter that will be used by a subsequent command. 
        /// </summary>
        Opts,
        /// <summary>
        /// The HASH command.  This command is not supported by all FTP servers.
        /// </summary>
        /// <remarks>
        /// This command is in the RFC draft phase and is used to generate file hashes on FTP server.  
        /// More information can be found searching the document named
        /// "File Transfer Protocol HASH Command for Cryptographic Hashes" draft-ietf-ftpext2-hash-03.
        /// </remarks>
        Hash,
        /// <summary>
        /// The RANG command.  This command is not supported by all FTP servers.
        /// </summary>
        /// <remarks>
        /// This command is in the RFC draft phase and is used specify a byte range for partical file hashes.  
        /// More information can be found searching the document named
        /// "File Transfer Protocol HASH Command for Cryptographic Hashes" draft-ietf-ftpext2-hash-03.
        /// </remarks>
        Rang,
        /// <summary>
        /// The CLNT command.  This command is not supported by all FTP servers.
        /// </summary>
        /// <remarks>
        /// The CLieNT command is a command whereby the FTP client can identify itself to the FTP server.
        /// Many FTP servers ignore this command.  The ServU FTP server requires this command to be sent
        /// prior to other important commands. 
        /// </remarks>
        Clnt,
        /// <summary>
        /// The MLST command.  This command is not supported by all FTP servers.
        /// </summary>
        /// <remarks>
        /// The MLST and MLSD commands are intended to standardize the file and
        /// directory information returned by the server-FTP process.  These
        /// commands differ from the LIST command in that the format of the
        /// replies is strictly defined although extensible.
        /// </remarks>
        Mlst,
        /// <summary>
        /// The MLSD command.  This command is not supported by all FTP servers.
        /// </summary>
        /// <remarks>
        /// The MLST and MLSD commands are intended to standardize the file and
        /// directory information returned by the server-FTP process.  These
        /// commands differ from the LIST command in that the format of the
        /// replies is strictly defined although extensible.
        /// </remarks>
        Mlsd
    }
 
    /// <summary>
    /// FTP request object which contains the command, arguments and text or an FTP request.
    /// </summary>
    public class FtpRequest
    {
        private FtpCmd _command;
        private string[] _arguments;
        private string _text;
        private Encoding _encoding;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FtpRequest()
        {
            _encoding = Encoding.UTF8;
            _command = new FtpCmd();
            _text = string.Empty;
        }

        /// <summary>
        /// FTP request constructor.
        /// </summary>
        /// <param name="encoding">Text encoding object to use.</param>
        /// <param name="command">FTP request command.</param>
        /// <param name="arguments">Parameters for the request</param>
        internal FtpRequest(Encoding encoding, FtpCmd command, params string[] arguments)
        {
            _encoding = encoding;
            _command = command;
            _arguments = arguments;
            _text = BuildCommandText();
        }

        /// <summary>
        /// FTP request constructor.
        /// </summary>
        /// <param name="encoding">Text encoding object to use.</param>
        /// <param name="command">FTP request command.</param>
        internal FtpRequest(Encoding encoding, FtpCmd command) : this(encoding, command, null)
        { }

        /// <summary>
        /// Get the FTP command enumeration value.
        /// </summary>
        public FtpCmd Command
        {
            get { return _command; }
        }

        /// <summary>
        /// Get the FTP command arguments (if any).
        /// </summary>
        public List<string> Arguments
        {
            get 
            {  
                return new List<string>(_arguments); 
            }
        }

        /// <summary>
        /// Get the FTP command text with any arguments.
        /// </summary>
        public string Text
        {
            get { return _text; }
        }

        /// <summary>
        /// Gets a boolean value indicating if the command is a file transfer or not.
        /// </summary>
        public bool IsFileTransfer
        {
            get
            {
                return ((_command == FtpCmd.Retr)
                  || (_command == FtpCmd.Stor)
                  || (_command == FtpCmd.Stou)
                  || (_command == FtpCmd.Appe)
                  );
            }
        }

        internal string BuildCommandText()
        {
            string commandText = _command.ToString().ToUpper(CultureInfo.InvariantCulture);

            if (_arguments == null)
            {
                return commandText;
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                foreach (string arg in _arguments)
                {
                    builder.Append(arg);
                    builder.Append(" ");
                }
                string argText = builder.ToString().TrimEnd();

                if (_command == FtpCmd.Unknown)
                    return argText;
                else
                    return String.Format("{0} {1}", commandText, argText).TrimEnd();
            }
        }

        internal byte[] GetBytes()
        {
            return _encoding.GetBytes(String.Format("{0}\r\n", _text));
        }

        internal bool HasHappyCodes
        {
            get { return GetHappyCodes().Length == 0 ? false : true; }
        }


        internal FtpResponseCode[]  GetHappyCodes()
        {
            switch(_command)
            {
                case FtpCmd.Unknown:
                    return BuildResponseArray();
                case FtpCmd.Allo:
                    return BuildResponseArray(FtpResponseCode.CommandOkay, FtpResponseCode.CommandNotImplementedSuperfluousAtThisSite);
                case FtpCmd.User:
                    return BuildResponseArray(FtpResponseCode.UserNameOkayButNeedPassword, FtpResponseCode.ServiceReadyForNewUser, FtpResponseCode.UserLoggedIn);
                case FtpCmd.Pass:
                    return BuildResponseArray(FtpResponseCode.UserLoggedIn, FtpResponseCode.ServiceReadyForNewUser, FtpResponseCode.NotLoggedIn);
                case FtpCmd.Cwd:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Pwd:
                    return BuildResponseArray(FtpResponseCode.PathNameCreated);
                case FtpCmd.Dele:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Abor:
                    return BuildResponseArray();
                case FtpCmd.Mkd:
                    return BuildResponseArray(FtpResponseCode.PathNameCreated);
                case FtpCmd.Rmd:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Help:
                    return BuildResponseArray(FtpResponseCode.SystemStatusOrHelpReply, FtpResponseCode.HelpMessage, FtpResponseCode.FileStatus);
                case FtpCmd.Mdtm:
                    return BuildResponseArray(FtpResponseCode.FileStatus, FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Stat:
                    return BuildResponseArray(FtpResponseCode.SystemStatusOrHelpReply, FtpResponseCode.DirectoryStatus, FtpResponseCode.FileStatus);
                case FtpCmd.Cdup:
                    return BuildResponseArray(FtpResponseCode.CommandOkay, FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Size:
                    return BuildResponseArray(FtpResponseCode.FileStatus);
                case FtpCmd.Feat:
                    return BuildResponseArray(FtpResponseCode.SystemStatusOrHelpReply);
                case FtpCmd.Syst:
                    return BuildResponseArray(FtpResponseCode.NameSystemType);
                case FtpCmd.Rnfr:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionPendingFurtherInformation);
                case FtpCmd.Rnto:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Noop:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);                
                case FtpCmd.Site:
                    return BuildResponseArray(FtpResponseCode.CommandOkay, FtpResponseCode.CommandNotImplementedSuperfluousAtThisSite, FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Pasv:
                    return BuildResponseArray(FtpResponseCode.EnteringPassiveMode);
                case FtpCmd.Port:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Type:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Rest:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionPendingFurtherInformation);
                case FtpCmd.Mode:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Quit:
                    return BuildResponseArray();
                case FtpCmd.Auth:
                    return BuildResponseArray(FtpResponseCode.AuthenticationCommandOkay, FtpResponseCode.AuthenticationCommandOkaySecurityDataOptional);
                case FtpCmd.Pbsz:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Prot:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.List:
                case FtpCmd.Nlst:
                case FtpCmd.Mlsd:
                    return BuildResponseArray(FtpResponseCode.DataConnectionAlreadyOpenSoTransferStarting,
                                FtpResponseCode.FileStatusOkaySoAboutToOpenDataConnection,
                                FtpResponseCode.ClosingDataConnection,
                                FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Appe:
                case FtpCmd.Stor:
                case FtpCmd.Stou:
                case FtpCmd.Retr:
                    return BuildResponseArray(FtpResponseCode.DataConnectionAlreadyOpenSoTransferStarting, 
                                FtpResponseCode.FileStatusOkaySoAboutToOpenDataConnection, 
                                FtpResponseCode.ClosingDataConnection,
                                FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Mlst:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Xcrc:
                case FtpCmd.Xmd5:
                case FtpCmd.Xsha1:
                case FtpCmd.Xsha256:
                case FtpCmd.Xsha512:
                    return BuildResponseArray(FtpResponseCode.RequestedFileActionOkayAndCompleted);
                case FtpCmd.Epsv:
                    return BuildResponseArray();
                case FtpCmd.Eprt:
                    return BuildResponseArray();
                case FtpCmd.Mfmt:
                    return BuildResponseArray(FtpResponseCode.FileStatus);
                case FtpCmd.Mfct:
                    return BuildResponseArray(FtpResponseCode.FileStatus);
                case FtpCmd.Opts:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                case FtpCmd.Hash:
                    return BuildResponseArray(FtpResponseCode.FileStatus);
                case FtpCmd.Clnt:
                    return BuildResponseArray(FtpResponseCode.CommandOkay);
                default:
                    throw new FtpException(String.Format("No response code(s) defined for FtpCmd {0}.", _command.ToString()));
            }
        }
        
        private FtpResponseCode[] BuildResponseArray(params FtpResponseCode[] codes)
        {
            return codes;
        }
    
    }
}
