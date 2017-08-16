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
using System.IO;
using NUnit.Framework;
using System.Diagnostics;
using Starksoft.Net.Ftp;

namespace Starksoft.Tests
{
    /// <summary>
    /// Test fixture for Starksoft.Net.FtpClient.
    /// </summary>
    [TestFixture]

    public class TestFtpMlsx
    {
        /// <summary>
        /// Test the MSLx item class.
        /// </summary>
        /// <param name="line">MLSx line text to parse and load.</param>
        [CLSCompliant(false)]
        [TestCase("Type=file;Size=1830;Modify=19940916055648;Perm=r; hatch.c")]
        [TestCase("modify=20120822211414;perm=adfr;size=2101;type=file;unique=16UF3F5;UNIX.group=49440;UNIX.mode=0744;UNIX.owner=49440; iphone_settings_icon.jpg")]
        [TestCase("modify=20030225143801;perm=adfr;size=503;type=file;unique=12U24470006;UNIX.group=0;UNIX.mode=0644;UNIX.owner=0; welcome.msg")]
        [TestCase("type=dir;modify=20120825130005; /Test")]
        [TestCase("create=20120825130005;lang=en;media-type=TEXT;charset=UTF-8;modify=20030225143801;perm=acdeflmprw;size=3243243332343503;type=file;unique=12U24470006;UNIX.group=100;UNIX.mode=0644;UNIX.owner=1000; wel come.msg")]
        [TestCase("create=20120825130005;lang=fr;media-type=TEXT;charset=UTF-7;modify=20030225143801;perm=acdeflmprw;size=3243243332343503;type=pdir;unique=12U24470006;UNIX.group=100;UNIX.mode=0644;UNIX.owner=1000; wel come.msg")]
        [TestCase("create=20120825130005;lang=sp;media-type=TEXT;charset=UTF-8;modify=20030225143801;perm=acdeflmprw;size=3243243332343503;type=cdir;unique=12U24470006;UNIX.group=100;UNIX.mode=0644;UNIX.owner=1000; wel come.msg")]
        [TestCase("create=20120825130005;lang=en;media-type=TEXT;charset=UTF-8;modify=20030225143801;perm=acdeflmprw;size=3243243332343503;type=dir;unique=12U24470006;UNIX.group=100;UNIX.mode=0644;UNIX.owner=1000; wel come.msg")]
        public void TestMlsxItem(string line)
        {
            FtpMlsxItemParser p = new FtpMlsxItemParser();
            p.ParseLine(line);
            Debug.WriteLine(p.ToString());
        }

        /// <summary>
        /// Test the get MLST method.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="protocol"></param>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <param name="method"></param>
        [CLSCompliant(false)]
        [TestCase("127.0.0.1", 21, FtpSecurityProtocol.None, "test", "test", ListingMethod.Automatic)]
        [TestCase("127.0.0.1", 21, FtpSecurityProtocol.None, "test", "test", ListingMethod.List)]
        [TestCase("127.0.0.1", 21, FtpSecurityProtocol.None, "test", "test", ListingMethod.ListAl)]
        [TestCase("127.0.0.1", 21, FtpSecurityProtocol.None, "test", "test", ListingMethod.Mlsx)]
        public void TestGetFileInfo(string host, int port, FtpSecurityProtocol protocol,
            string user, string pwd, ListingMethod method)
        {
            using (FtpClient c = new FtpClient(host, port, protocol))
            {
                Debug.WriteLine("********** BEGINNING **********");
                c.AlwaysAcceptServerCertificate = true;
                c.DirListingMethod = method;
                c.Open(user, pwd);
                Assert.IsTrue(c.IsConnected);
                // get information about the root directory
                FtpItem m = c.GetFileInfo(".");
                if (m is FtpMlsxItem)
                    Debug.Write(((FtpMlsxItem)m).ToString());
                else
                    Debug.Write(m.ToString());
                Debug.WriteLine("********** ENDING **********");
            }
        }

    }
}
