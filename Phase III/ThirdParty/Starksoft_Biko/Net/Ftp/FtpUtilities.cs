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
using System.Text;

namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// FTP utility methods.
    /// </summary>
    public class FtpUtilities
    {
        private const int MODE_VAL_MIN_VALUE = 0;
        private const int MODE_VAL_MAX_VALUE = 777;

        private const int MODE_STR_MIN_LEN = 9;
        private const int MODE_STR_MAX_LEN = 10;

        private const string MODE_STR_NONE = "---";
        private const string MODE_STR___x = "--x";
        private const string MODE_STR__w_ = "-w-";
        private const string MODE_STR__wx = "-wx";
        private const string MODE_STR_r__ = "r--";
        private const string MODE_STR_r_x = "r-x";
        private const string MODE_STR_rw_ = "rw-";
        private const string MODE_STR_rwx = "rwx";

        private const char MODE_CHAR_NONE = '0';
        private const char MODE_CHAR___x = '1';
        private const char MODE_CHAR__w_ = '2';
        private const char MODE_CHAR__wx = '3';
        private const char MODE_CHAR_r__ = '4';
        private const char MODE_CHAR_r_x = '5';
        private const char MODE_CHAR_rw_ = '6';
        private const char MODE_CHAR_rwx = '7';

        private const int MODE_INT_NONE = 0;
        private const int MODE_INT___x = 1;
        private const int MODE_INT__w_ = 2;
        private const int MODE_INT__wx = 3;
        private const int MODE_INT_r__ = 4;
        private const int MODE_INT_r_x = 5;
        private const int MODE_INT_rw_ = 6;
        private const int MODE_INT_rwx = 7;

        /// <summary>
        /// Converts the UNIX 3 digit mode value to UNIX attribute string.
        /// </summary>
        /// <param name="mode">Three digit mode value.</param>
        /// <returns>Attribute string.</returns>
        public static string ModeToAttribute(int mode)
        {
            // check that the mode value is within the correct size limits
            if (mode < MODE_VAL_MIN_VALUE || mode > MODE_VAL_MAX_VALUE)
                return String.Empty;

            string s = mode.ToString();
            char[] ary = s.ToCharArray();
            StringBuilder b = new StringBuilder();

            foreach (char c in ary)
            {
                switch (c)
                {
                    case MODE_CHAR_NONE:
                        b.Append(MODE_STR_NONE);
                        break;
                    case MODE_CHAR___x:
                        b.Append(MODE_STR___x);
                        break;
                    case MODE_CHAR__w_:
                        b.Append(MODE_CHAR__w_);
                        break;
                    case MODE_CHAR__wx:
                        b.Append(MODE_STR__wx);
                        break;
                    case MODE_CHAR_r__:
                        b.Append(MODE_STR_r__);
                        break;
                    case MODE_CHAR_r_x:
                        b.Append(MODE_STR_r_x);
                        break;
                    case MODE_CHAR_rw_:
                        b.Append(MODE_STR_rw_);
                        break;
                    case MODE_CHAR_rwx:
                        b.Append(MODE_STR_rwx);
                        break;
                    default:
                        b.Append(MODE_STR_NONE);
                        break;
                }
            }
            return b.ToString();
        }

        /// <summary>
        /// Convert UNIX attribute to UNIX three digit mode value.
        /// </summary>
        /// <param name="s">Attribute string.</param>
        /// <returns>Three digit mode integer value.</returns>
        public static int AttributeToMode(string s)
        {
            if (s.Length < MODE_STR_MIN_LEN || s.Length > MODE_STR_MAX_LEN)
                return 0;

            // strip off the leading unix attribute 
            if (s.Length == MODE_STR_MAX_LEN)
                s = s.Substring(1);

            string user = s.Substring(0, 3);
            string group = s.Substring(3, 3);
            string other = s.Substring(6, 3);

            int mode = ParseAttrib(user) * 100;
            mode += ParseAttrib(group) * 10;
            mode += ParseAttrib(other);
            return mode;
        }

        private static int ParseAttrib(string a)
        {
            switch (a)
            {
                case MODE_STR_NONE:
                    return MODE_INT_NONE;
                case MODE_STR___x:
                    return MODE_INT___x;
                case MODE_STR__w_:
                    return MODE_INT__w_;
                case MODE_STR__wx:
                    return MODE_INT__wx;
                case MODE_STR_r__:
                    return MODE_INT_r__;
                case MODE_STR_r_x:
                    return MODE_INT_r_x;
                case MODE_STR_rw_:
                    return MODE_INT_rw_;
                case MODE_STR_rwx:
                    return MODE_INT_rwx;
                default:
                    return MODE_INT_NONE;
            }

        }

    }
}
