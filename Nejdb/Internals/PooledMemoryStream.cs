using System.IO;
using System.Threading;

namespace Nejdb.Internals
{
    /// <summary>
    /// Contains additional functionality to make it possible to use in pool
    /// </summary>
    public class PooledMemoryStream : MemoryStream
    {
        private const int Busy = 1;
        private const int Free = 0;
        private int _isFree;

        /// <summary>
        /// Creates new instance of <see cref="PooledMemoryStream"/>
        /// </summary>
        public PooledMemoryStream(int capacity) :  base(capacity)
        {
            
        }
        /// <summary>
        /// Owns stream in a thread safe manner
        /// </summary>
        /// <returns>True if the stream owned by caller. False otherwise</returns>
        public bool TryOwn()
        {
            var result = Interlocked.CompareExchange(ref _isFree, Busy, Free);
            return result == Free;
        }

        protected override void Dispose(bool disposing)
        {
            SetLength(0);

            Thread.VolatileWrite(ref _isFree, Free);
        }
    }
}