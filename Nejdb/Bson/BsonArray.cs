// ============================================================================================
//   .NET API for EJDB database library http://ejdb.org
//   Copyright (C) 2012-2013 Softmotions Ltd <info@softmotions.com>
//
//   This file is part of EJDB.
//   EJDB is free software; you can redistribute it and/or modify it under the terms of
//   the GNU Lesser General Public License as published by the Free Software Foundation; either
//   version 2.1 of the License or any later version.  EJDB is distributed in the hope
//   that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public
//   License for more details.
//   You should have received a copy of the GNU Lesser General Public License along with EJDB;
//   if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330,
//   Boston, MA 02111-1307 USA.
// ============================================================================================

using System;

namespace Nejdb.Bson {

	[Serializable]
	public class BsonArray : BsonDocument {

		public override BsonType BsonType {
			get {
				return BsonType.ARRAY;
			}
		}

		public object this[int key] {
			get {
				return GetObjectValue(key.ToString());
			}
		}

		public BsonArray() {
		}

		public BsonArray(BsonUndefined[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetUndefined(i);
			}
		}

		public BsonArray(Bsonull[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNull(i);
			}
		}

		public BsonArray(ushort[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNumber(i, (int) arr[i]);
			}
		}

		public BsonArray(uint[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNumber(i, (long) arr[i]);
			}
		}

		public BsonArray(ulong[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNumber(i, (long) arr[i]);
			}
		}

		public BsonArray(short[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNumber(i, (int) arr[i]);
			}
		}

		public BsonArray(string[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetString(i, arr[i]);
			}
		}

		public BsonArray(int[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNumber(i, arr[i]);
			}
		}

		public BsonArray(long[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNumber(i, arr[i]);
			}
		}

		public BsonArray(float[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNumber(i, arr[i]);
			}
		}

		public BsonArray(double[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetNumber(i, arr[i]);
			}
		}

		public BsonArray(bool[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetBool(i, arr[i]);
			}
		}

		public BsonArray(BsonOid[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetOID(i, arr[i]);
			}
		}

		public BsonArray(DateTime[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetDate(i, arr[i]);
			}
		}

		public BsonArray(BsonDocument[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetObject(i, arr[i]);
			}
		}

		public BsonArray(BsonArray[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetArray(i, arr[i]);
			}
		}

		public BsonArray(BsonRegexp[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetRegexp(i, arr[i]);
			}
		}

		public BsonArray(BsonTimestamp[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetTimestamp(i, arr[i]);
			}
		}

		public BsonArray(BsonCodeWScope[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetCodeWScope(i, arr[i]);
			}
		}

		public BsonArray(BsonBinData[] arr) {
			for (var i = 0; i < arr.Length; ++i) {
				SetBinData(i, arr[i]);
			}
		}

		public BsonDocument SetNull(int idx) {
			return base.SetNull(idx.ToString());
		}

		public BsonDocument SetUndefined(int idx) {
			return base.SetUndefined(idx.ToString());
		}

		public BsonDocument SetMaxKey(int idx) {
			return base.SetMaxKey(idx.ToString());
		}

		public BsonDocument SetMinKey(int idx) {
			return base.SetMinKey(idx.ToString());
		}

		public BsonDocument SetOID(int idx, string oid) {
			return base.SetOID(idx.ToString(), oid);
		}

		public BsonDocument SetOID(int idx, BsonOid oid) {
			return base.SetOID(idx.ToString(), oid); 
		}

		public BsonDocument SetBool(int idx, bool val) {
			return base.SetBool(idx.ToString(), val);
		}

		public BsonDocument SetNumber(int idx, int val) {
			return base.SetNumber(idx.ToString(), val);
		}

		public BsonDocument SetNumber(int idx, long val) {
			return base.SetNumber(idx.ToString(), val);
		}

		public BsonDocument SetNumber(int idx, double val) {
			return base.SetNumber(idx.ToString(), val); 
		}

		public BsonDocument SetNumber(int idx, float val) {
			return base.SetNumber(idx.ToString(), val); 
		}

		public BsonDocument SetString(int idx, string val) {
			return base.SetString(idx.ToString(), val); 
		}

		public BsonDocument SetCode(int idx, string val) {
			return base.SetCode(idx.ToString(), val);
		}

		public BsonDocument SetSymbol(int idx, string val) {
			return base.SetSymbol(idx.ToString(), val);
		}

		public BsonDocument SetDate(int idx, DateTime val) {
			return base.SetDate(idx.ToString(), val);
		}

		public BsonDocument SetRegexp(int idx, BsonRegexp val) {
			return base.SetRegexp(idx.ToString(), val);
		}

		public BsonDocument SetBinData(int idx, BsonBinData val) {
			return base.SetBinData(idx.ToString(), val);
		}

		public BsonDocument SetObject(int idx, BsonDocument val) {
			return base.SetDocument(idx.ToString(), val);
		}

		public BsonDocument SetArray(int idx, BsonArray val) {
			return base.SetArray(idx.ToString(), val);
		}

		public BsonDocument SetTimestamp(int idx, BsonTimestamp val) {
			return base.SetTimestamp(idx.ToString(), val);
		}

		public BsonDocument SetCodeWScope(int idx, BsonCodeWScope val) {
			return base.SetCodeWScope(idx.ToString(), val);
		}

		protected override void CheckKey(string key) {
			int idx;
			if (key == null || !int.TryParse(key, out idx) || idx < 0) {
				throw new InvalidBsonDataException(string.Format("Invalid array key: {0}", key));
			}
		}
	}
}

