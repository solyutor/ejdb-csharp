//// ============================================================================================
////   .NET API for EJDB database library http://ejdb.org
////   Copyright (C) 2012-2013 Softmotions Ltd <info@softmotions.com>
////
////   This file is part of EJDB.
////   EJDB is free software; you can redistribute it and/or modify it under the terms of
////   the GNU Lesser General Public License as published by the Free Software Foundation; either
////   version 2.1 of the License or any later version.  EJDB is distributed in the hope
////   that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
////   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
////   License for more details.
////   You should have received a copy of the GNU Lesser General Public License along with EJDB;
////   if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330,
////   Boston, MA 02111-1307 USA.
//// ============================================================================================
//using System;
//using System.Runtime.InteropServices;
//using Ejdb.BSON;
//using Ejdb.Utils;

//namespace Ejdb.DB {
//	/// <summary>
//	/// EJDB database native wrapper.
//	/// </summary>
//	public class EJDB  {
		
//		bool _throwonfail = true;
//		//.//////////////////////////////////////////////////////////////////
//		//   				Native functions refs
//		//.//////////////////////////////////////////////////////////////////

//		#region NativeRefs
		
		





		
		
//		//EJDB_EXPORT bson* json2bson(const char *jsonstr);
//		[DllImport(EJDB_LIB_NAME, EntryPoint = "json2bson", CallingConvention = CallingConvention.Cdecl)]
//		internal static extern IntPtr _json2bson([In] IntPtr jsonstr);

//		internal static IntPtr _json2bson(string jsonstr) {
//			IntPtr jsonptr = Native.NativeUtf8FromString(jsonstr);
//			try {
//				return _json2bson(jsonptr);
//			} finally {
//				Marshal.FreeHGlobal(jsonptr); //UnixMarshal.FreeHeap(jsonptr);
//			}
//		}

//		internal static bool _ejdbsetindex(IntPtr coll, string ipath, int flags) {
//			IntPtr ipathptr = Native.NativeUtf8FromString(ipath); //UnixMarshal.StringToHeap(ipath, Encoding.UTF8);
//			try {
//				return _ejdbsetindex(coll, ipathptr, flags);
//			} finally {
//				Marshal.FreeHGlobal(ipathptr); //UnixMarshal.FreeHeap(ipathptr);
//			}
//		}

//		/// <summary>
//		/// DROP indexes of all types for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool DropIndexes(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXDROPALL);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// OPTIMIZE indexes of all types for JSON field path.
//		/// </summary>
//		/// <remarks>
//		/// Performs B+ tree index file optimization.
//		/// </remarks>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool OptimizeIndexes(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXOP);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Ensure index presence of String type for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool EnsureStringIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXSTR);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Rebuild index of String type for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool RebuildStringIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXSTR | JBIDXREBLD);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Drop index of String type for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool DropStringIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXSTR | JBIDXDROP);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Ensure case insensitive String index for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool EnsureIStringIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXISTR);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Rebuild case insensitive String index for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool RebuildIStringIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXISTR | JBIDXREBLD);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Drop case insensitive String index for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool DropIStringIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXISTR | JBIDXDROP);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Ensure index presence of Number type for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool EnsureNumberIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXNUM);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Rebuild index of Number type for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool RebuildNumberIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXNUM | JBIDXREBLD);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Drop index of Number type for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool DropNumberIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXNUM | JBIDXDROP);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Ensure index presence of Array type for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool EnsureArrayIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXARR);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Rebuild index of Array type for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool RebuildArrayIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXARR | JBIDXREBLD);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Drop index of Array type for JSON field path.
//		/// </summary>
//		/// <returns><c>false</c>, if error occurred.</returns>
//		/// <param name="cname">Name of collection.</param>
//		/// <param name="ipath">JSON indexed field path</param>
//		public bool DropArrayIndex(string cname, string ipath) {
//			bool rv = IndexOperation(cname, ipath, JBIDXARR | JBIDXDROP);
//			if (_throwonfail && !rv) {
//				throw new EJDBException(this);
//			}
//			return rv;
//		}

//		/// <summary>
//		/// Creates the query.
//		/// </summary>
//		/// <returns>The query object.</returns>
//		/// <param name="qdoc">BSON query spec.</param>
//		/// <param name="defaultcollection">Name of the collection used by default.</param>
//		public EJDBQuery CreateQuery(object qv = null, string defaultcollection = null) {
//			CheckDisposed();
//			return new EJDBQuery(this, BSONDocument.ValueOf(qv), defaultcollection);
//		}

//		public EJDBQuery CreateQueryFor(string defaultcollection) {
//			CheckDisposed();
//			return new EJDBQuery(this, new BSONDocument(), defaultcollection);
//		}

//		/// <summary>
//		/// Convert JSON string into BSONDocument.
//		/// Returns `null` if conversion failed.
//		/// </summary>
//		/// <returns>The BSONDocument instance on success.</returns>
//		/// <param name="json">JSON string</param>
//		public BSONDocument Json2Bson(string json) {
//			IntPtr bsonret = _json2bson(json);
//			if (bsonret == IntPtr.Zero) {
//				return null;
//			}
//			byte[] bsdata = BsonPtrIntoByteArray(bsonret);
//			if (bsdata.Length == 0) {
//				return null;
//			}
//			BSONIterator it = new BSONIterator(bsdata);
//			return it.ToBSONDocument();
//		}





//		internal void CheckDisposed(bool checkopen = false) {
//			if (_db == IntPtr.Zero) {
//				throw new ObjectDisposedException("Database is disposed");
//			}
//			if (checkopen) {
//				if (!IsOpen) {
//					throw new ObjectDisposedException("Operation on closed EJDB instance"); 
//				}
//			}
//		}
//	}
//}

