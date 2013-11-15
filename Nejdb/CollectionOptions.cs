using System.Runtime.InteropServices;

namespace Nejdb
{
    /// <summary>
    /// Corresponds to <c>EJCOLLOPTS</c> in ejdb.h
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CollectionOptions
    {
        public CollectionOptions(bool large, bool compressed, long records, int cachedRecords)
        {
            Large = large;
            Compressed = compressed;
            Records = records;
            CachedRecords = cachedRecords;
        }

        /// <summary>
        /// Large collection. It can be larger than 2GB. Default false
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool Large;

        /// <summary>
        /// Collection records will be compressed with DEFLATE compression. Default: false
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public readonly bool Compressed;

        /// <summary>
        /// Expected records number in the collection. Default: 128K
        /// </summary>
        public readonly long Records;

        /// <summary>
        /// Maximum number of cached records. Default: 0
        /// </summary>
        public readonly int CachedRecords;

        /// <summary>
        /// Default options
        /// </summary>
        public static readonly CollectionOptions None = new CollectionOptions();
    }
}