/*
Copyright (c) 2007-2012 Benton Stark, Starksoft LLC (http://www.starksoft.com) 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;

namespace Starksoft.Net.Ftp
{
    /// <summary>
    /// Event arguments to facilitate the transfer progress event.
    /// </summary>
    public class TransferProgressEventArgs : EventArgs
    {

        private int _bytesTransferred;
        private long _totalBytesTransferred;
        private long _transferSize;
        private int _bytesPerSecond;
        private TimeSpan _elapsedTime;
        private int _percentComplete;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="bytesTransferred">The number of bytes transferred in the last transfer block.</param>
        /// <param name="totalBytesTransferred">Total number of bytes transferred since transfer began.</param>
        /// <param name="transferSize">Size of the data transfer.</param>
        /// <param name="bytesPerSecond">The data transfer speed in bytes per second over the entire duration of the data transfer.</param>
        /// <param name="elapsedTime">The time that has elapsed since the data transfer started.</param>
        /// <param name="percentComplete">The percentage that the transfer has completed.</param>
        public TransferProgressEventArgs(int bytesTransferred, long totalBytesTransferred, long transferSize, int bytesPerSecond, TimeSpan elapsedTime, int percentComplete)
        {
            _bytesTransferred = bytesTransferred;
            _totalBytesTransferred = totalBytesTransferred;
            _transferSize = transferSize;
            _bytesPerSecond = bytesPerSecond;
            _elapsedTime = elapsedTime;
            _percentComplete = percentComplete;
        }

        /// <summary>
        /// The number of bytes transferred in the last transfer block.  
        /// </summary>
        public int BytesTransferred
        {
            get { return _bytesTransferred; }
        }

        /// <summary>
        /// The total number of bytes transferred for a particular transfer event.
        /// </summary>
        public long TotalBytesTransferred
        {
            get { return _totalBytesTransferred; }
        }

        /// <summary>
        /// Gets the data transfer speed in bytes per second.
        /// </summary>
        public int BytesPerSecond
        {
            get { return _bytesPerSecond; }
        }

        /// <summary>
        /// Gets the data transfer speed in Kilobytes per second.
        /// </summary>
        public int KilobytesPerSecond
        {
            get { return _bytesPerSecond / 1024; }
        }

        /// <summary>
        /// Gets the data transfer speed in Megabytes per second.
        /// </summary>
        public int MegabytesPerSecond
        {
            get { return _bytesPerSecond / 1024 / 1024; }
        }

        /// <summary>
        /// Gets the data transfer speed in Gigabytes per second.
        /// </summary>
        public int GigabytesPerSecond
        {
            get { return _bytesPerSecond / 1024 / 1024 / 1024; }
        }
        
        /// <summary>
        /// Gets the time that has elapsed since the data transfer started.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
        }

        /// <summary>
        /// Gets the percentage the transfer is complete if data is available.
        /// </summary>
        /// <remarks>
        /// This data is not available for PutFile() stream transfers that do not support seeking.  This data
        /// will not be accurately represented when zlib compression is enabled.  This data is not available
        /// for non-file data transfer events.
        /// </remarks>
        public int PercentComplete
        {
            get { return _percentComplete; }
        }
        
        /// <summary>
        /// Gets the size of the data transfer if data is available.
        /// </summary>
        /// <remarks>
        /// This data is not available for PutFile() stream transfers that do not support seeking.  This data
        /// will not be accurately represented when zlib compression is enabled.  This data is not available
        /// for non-file data transfer events.
        /// </remarks>
        public long TransferSize
        {
            get { return _transferSize; }
        }

        /// <summary>
        /// The number of bytes remaining in the transfer.
        /// </summary>
        /// <remarks>
        /// This data is not available for PutFile() stream transfers that do not support seeking.  This data
        /// will not be accurately represented when zlib compression is enabled.  This data is not available
        /// for non-file data transfer events.
        /// </remarks>
        public long BytesRemaining
        {
            get
            {
                if (_transferSize < 0)
                    return 0;
                return (_transferSize - _totalBytesTransferred);
            }

        }

        /// <summary>
        /// Gets the estimated time that remains for the data transfer.
        /// </summary>
        /// <remarks>
        /// This data is not available for PutFile() stream transfers that do not support seeking.  This data
        /// will not be accurately represented when zlib compression is enabled.  This data is not available
        /// for non-file data transfer events.
        /// </remarks>
        public TimeSpan TimeRemaining
        {
            get 
            {
                if (_transferSize < 0)
                    return new TimeSpan();
                int sec = (int)(BytesRemaining / _bytesPerSecond);
                return new TimeSpan(0, 0, sec); 
            }
        }

    }
}
