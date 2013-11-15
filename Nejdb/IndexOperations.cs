using System;

namespace Nejdb
{
    /// <summary>
    /// Represents possible index operations and types (ejdb.h)
    /// </summary>
    [Flags]
    public enum IndexOperations : int
    {
        /// <summary>
        /// Drop index. (JBIDXDROP)
        /// </summary>
        DropIndex = 1 << 0,

        /// <summary>
        /// Drop index for all types. (JBIDXDROPALL)
        /// </summary>
        DropIndexAllTypes = 1 << 1,

        /// <summary>
        /// Optimize indexes. (JBIDXOP)
        /// </summary>
        Optimize = 1 << 2,

        /// <summary>
        /// Rebuild index. (JBIDXREBLD)
        /// </summary>
        Rebuild = 1 << 3,

        /// <summary>
        /// Number index. (JBIDXNUM)
        /// </summary>
        Number = 1 << 4,

        /// <summary>
        /// String index. (JBIDXSTR)
        /// </summary>
        String = 1 << 5,

        /// <summary>
        /// Array token index. (JBIDXARR)
        /// </summary>
        ArrayToken = 1 << 6,

        /// <summary>
        /// Case insensitive string index. (JBIDXISTR)
        /// </summary>
        CaseInsensitiveString = 1 << 7
    }
}