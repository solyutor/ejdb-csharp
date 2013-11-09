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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using Ejdb.IO;
using Ejdb.Bson;
using Ejdb.Utils;
using System.Reflection;
using System.Linq;

namespace Ejdb.Bson {

	/// <summary>
	/// Bson document deserialized data wrapper.
	/// </summary>
	[Serializable]
	public class BsonDocument : IBsonValue, IEnumerable<BsonValue>, ICloneable 
	{
		public static readonly BsonDocument Empty = new BsonDocument();


		static Dictionary<Type, Action<BsonDocument, string, object>> TYPE_SETTERS = 
		new Dictionary<Type, Action<BsonDocument, string, object>> {
			{typeof(bool), (d, k, v) => d.SetBool(k, (bool) v)},
			{typeof(bool[]), (d, k, v) => d.SetArray(k, new BsonArray((bool[]) v))},
			{typeof(byte), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(sbyte), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(ushort), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(ushort[]), (d, k, v) => d.SetArray(k, new BsonArray((ushort[]) v))},
			{typeof(short), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(short[]), (d, k, v) => d.SetArray(k, new BsonArray((short[]) v))},
			{typeof(uint), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(uint[]), (d, k, v) => d.SetArray(k, new BsonArray((uint[]) v))},
			{typeof(int), (d, k, v) => d.SetNumber(k, (int) v)},
			{typeof(int[]), (d, k, v) => d.SetArray(k, new BsonArray((int[]) v))},
			{typeof(ulong), (d, k, v) => d.SetNumber(k, (long) v)},
			{typeof(ulong[]), (d, k, v) => d.SetArray(k, new BsonArray((ulong[]) v))},
			{typeof(long), (d, k, v) => d.SetNumber(k, (long) v)},
			{typeof(long[]), (d, k, v) => d.SetArray(k, new BsonArray((long[]) v))},
			{typeof(float), (d, k, v) => d.SetNumber(k, (float) v)},
			{typeof(float[]), (d, k, v) => d.SetArray(k, new BsonArray((float[]) v))},
			{typeof(double), (d, k, v) => d.SetNumber(k, (double) v)},
			{typeof(double[]), (d, k, v) => d.SetArray(k, new BsonArray((double[]) v))},
			{typeof(char), (d, k, v) => d.SetString(k, v.ToString())},
			{typeof(string), (d, k, v) => d.SetString(k, (string) v)},
			{typeof(string[]), (d, k, v) => d.SetArray(k, new BsonArray((string[]) v))},
			{typeof(BsonOid), (d, k, v) => d.SetOID(k, (BsonOid) v)},
			{typeof(BsonOid[]), (d, k, v) => d.SetArray(k, new BsonArray((BsonOid[]) v))},
			{typeof(BsonRegexp), (d, k, v) => d.SetRegexp(k, (BsonRegexp) v)},
			{typeof(BsonRegexp[]), (d, k, v) => d.SetArray(k, new BsonArray((BsonRegexp[]) v))},
			{typeof(BsonValue), (d, k, v) => d.SetBsonValue((BsonValue) v)},
			{typeof(BsonTimestamp), (d, k, v) => d.SetTimestamp(k, (BsonTimestamp) v)},
			{typeof(BsonTimestamp[]), (d, k, v) => d.SetArray(k, new BsonArray((BsonTimestamp[]) v))},
			{typeof(BsonCodeWScope), (d, k, v) => d.SetCodeWScope(k, (BsonCodeWScope) v)},
			{typeof(BsonCodeWScope[]), (d, k, v) => d.SetArray(k, new BsonArray((BsonCodeWScope[]) v))},
			{typeof(BsonBinData), (d, k, v) => d.SetBinData(k, (BsonBinData) v)},
			{typeof(BsonBinData[]), (d, k, v) => d.SetArray(k, new BsonArray((BsonBinData[]) v))},
			{typeof(BsonDocument), (d, k, v) => d.SetDocument(k, (BsonDocument) v)},
			{typeof(BsonDocument[]), (d, k, v) => d.SetArray(k, new BsonArray((BsonDocument[]) v))},
			{typeof(BsonArray), (d, k, v) => d.SetArray(k, (BsonArray) v)},
			{typeof(BsonArray[]), (d, k, v) => d.SetArray(k, new BsonArray((BsonArray[]) v))},
			{typeof(DateTime), (d, k, v) => d.SetDate(k, (DateTime) v)},
			{typeof(DateTime[]), (d, k, v) => d.SetArray(k, new BsonArray((DateTime[]) v))},
			{typeof(BsonUndefined), (d, k, v) => d.SetUndefined(k)},
			{typeof(BsonUndefined[]), (d, k, v) => d.SetArray(k, new BsonArray((BsonUndefined[]) v))},
			{typeof(Bsonull), (d, k, v) => d.SetNull(k)},
			{typeof(Bsonull[]), (d, k, v) => d.SetArray(k, new BsonArray((Bsonull[]) v))}
		};

		readonly List<BsonValue> _fieldslist;

		[NonSerializedAttribute]
		Dictionary<string, BsonValue> _fields;

		[NonSerializedAttribute]
		int? _cachedhash;

		/// <summary>
		/// Bson Type this document. 
		/// </summary>
		/// <remarks>
		/// Type can be either <see cref="Ejdb.Bson.BsonType.OBJECT"/> or <see cref="Ejdb.Bson.BsonType.ARRAY"/>
		/// </remarks>
		/// <value>The type of the Bson.</value>
		public virtual BsonType BsonType {
			get { 
				return BsonType.OBJECT;
			}
		}

		/// <summary>
		/// Gets the document keys.
		/// </summary>
		public ICollection<string> Keys {
			get {
				CheckFields();
				return _fields.Keys;
			}
		}

		/// <summary>
		/// Gets count of document keys.
		/// </summary>
		public int KeysCount {
			get {
				return _fieldslist.Count;
			}
		}

		public BsonDocument() {
			this._fields = null;
			this._fieldslist = new List<BsonValue>();
		}

		public BsonDocument(BsonIterator it) : this() {
			while (it.Next() != BsonType.EOO) {
				Add(it.FetchCurrentValue());
			}
		}

		public BsonDocument(BsonIterator it, string[] fields) : this() {
			Array.Sort(fields);
			BsonType bt;
			int ind = -1;
			int nfc = 0;
			foreach (string f in fields) {
				if (f != null) {
					nfc++;
				}
			}
			while ((bt = it.Next()) != BsonType.EOO) {
				if (nfc < 1) {
					continue;
				}
				string kk = it.CurrentKey;
				if ((ind = Array.IndexOf(fields, kk)) != -1) {
					Add(it.FetchCurrentValue());
					fields[ind] = null;
					nfc--;
				} else if (bt == BsonType.OBJECT || bt == BsonType.ARRAY) {
					string[] narr = null;
					for (var i = 0; i < fields.Length; ++i) {
						var f = fields[i];
						if (f == null) {
							continue;
						}
						if (f.IndexOf(kk, StringComparison.Ordinal) == 0 && 
							f.Length > kk.Length + 1 && 
							f[kk.Length] == '.') {
							if (narr == null) {
								narr = new string[fields.Length];
							}
							narr[i] = f.Substring(kk.Length + 1);
							fields[i] = null;
							nfc--;
						}
					}
					if (narr != null) {
						BsonIterator nit = new BsonIterator(it);
						BsonDocument ndoc = new BsonDocument(nit, narr);
						if (ndoc.KeysCount > 0) {
							Add(new BsonValue(bt, kk, ndoc));
						}
					}
				}
			}
			it.Dispose();
		}

		public BsonDocument(byte[] bsdata) : this() {
			using (BsonIterator it = new BsonIterator(bsdata)) {
				while (it.Next() != BsonType.EOO) {
					Add(it.FetchCurrentValue());
				}
			}
		}

		public BsonDocument(Stream bstream) : this() {
			using (BsonIterator it = new BsonIterator(bstream)) {
				while (it.Next() != BsonType.EOO) {
					Add(it.FetchCurrentValue());
				}
			}
		}

		public BsonDocument(BsonDocument doc) : this() {
			foreach (var bv in doc) {
				Add((BsonValue) bv.Clone());
			}
		}

		public IEnumerator<BsonValue> GetEnumerator() {
			return _fieldslist.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		/// <summary>
		/// Convert Bson document object into Bson binary data byte array.
		/// </summary>
		/// <returns>The byte array.</returns>
		public byte[] ToByteArray() {
			byte[] res;
			using (var ms = new MemoryStream()) {
				Serialize(ms);
				res = ms.ToArray();
			}
			return res;
		}

		public string ToDebugDataString() {
			return BitConverter.ToString(ToByteArray());
		}

		/// <summary>
		/// Gets the field value.
		/// </summary>
		/// <returns>The Bson value.</returns>
		/// <see cref="Ejdb.Bson.BsonValue"/>
		/// <param name="key">Document field name</param>
		public BsonValue GetBsonValue(string key) {
			CheckFields();
			BsonValue ov;
			if (_fields.TryGetValue(key, out ov)) {
				return ov;
			} else {
				return null;
			}
		}

		/// <summary>
		/// Gets the field value object. 
		/// </summary>
		/// <remarks>
		/// Hierarchical field paths are NOT supported. Use <c>[]</c> operator instead.
		/// </remarks>
		/// <param name="key">Bson document key</param>
		/// <see cref="Ejdb.Bson.BsonValue"/>
		public object GetObjectValue(string key) {
			var bv = GetBsonValue(key);
			return bv != null ? bv.Value : null;
		}

		/// <summary>
		/// Determines whether this document has the specified key.
		/// </summary>
		/// <returns><c>true</c> if this document has the specified key; otherwise, <c>false</c>.</returns>
		/// <param name="key">Key.</param>
		public bool HasKey(string key) {
			return (GetBsonValue(key) != null);
		}

		/// <summary>
		/// Gets the <see cref="Ejdb.Bson.BsonDocument"/> with the specified key.
		/// </summary>
		/// <remarks>
		/// Getter for hierarchical field paths are supported.
		/// </remarks>
		/// <param name="key">Key.</param>
		/// <returns>Key object </c> or <c>null</c> if the key is not exists or value type is either 
		/// <see cref="Ejdb.Bson.BsonType.NULL"/> or <see cref="Ejdb.Bson.BsonType.UNDEFINED"/></returns>
		public object this[string key] {
			get {
				int ind;
				if ((ind = key.IndexOf(".", StringComparison.Ordinal)) == -1) {
					return GetObjectValue(key);
				} else {
					string prefix = key.Substring(0, ind);
					BsonDocument doc = GetObjectValue(prefix) as BsonDocument;
					if (doc == null || key.Length < ind + 2) {
						return null;
					}
					return doc[key.Substring(ind + 1)];
				}
			}
			set {
				object v = value;
				if (v == null) {
					SetNull(key);
					return;
				}
				Action<BsonDocument, string, object> setter;
				Type vtype = v.GetType();
				TYPE_SETTERS.TryGetValue(vtype, out setter);
				if (setter == null) {
					if (vtype.IsAnonymousType()) {
						setter = SetAnonType;
					} else {
						throw new Exception(string.Format("Unsupported value type: {0} for doc[key] assign operation", v.GetType()));
					}
				}
				setter(this, key, v);
			}
		}

		public static void SetAnonType(BsonDocument doc, string key, object val) {
			BsonDocument ndoc = (key == null) ? doc : new BsonDocument();
			Type vtype = val.GetType();
			foreach (PropertyInfo pi in vtype.GetProperties()) {
				if (!pi.CanRead) {
					continue;
				}
				ndoc[pi.Name] = pi.GetValue(val, null);
			}
			if (key != null) {
				doc.SetDocument(key, ndoc);
			}
		}

		public BsonDocument SetNull(string key) {
			return SetBsonValue(new BsonValue(BsonType.NULL, key));
		}

		public BsonDocument SetUndefined(string key) {
			return SetBsonValue(new BsonValue(BsonType.UNKNOWN, key));
		}

		public BsonDocument SetMaxKey(string key) {
			return SetBsonValue(new BsonValue(BsonType.MAXKEY, key));
		}

		public BsonDocument SetMinKey(string key) {
			return SetBsonValue(new BsonValue(BsonType.MINKEY, key));
		}

		public BsonDocument SetOID(string key, string oid) {
			return SetBsonValue(new BsonValue(BsonType.OID, key, new BsonOid(oid)));
		}

		public BsonDocument SetOID(string key, BsonOid oid) {
			return SetBsonValue(new BsonValue(BsonType.OID, key, oid));
		}

		public BsonDocument SetBool(string key, bool val) {
			return SetBsonValue(new BsonValue(BsonType.BOOL, key, val));
		}

		public BsonDocument SetNumber(string key, int val) {
			return SetBsonValue(new BsonValue(BsonType.INT, key, val));
		}

		public BsonDocument SetNumber(string key, long val) {
			return SetBsonValue(new BsonValue(BsonType.LONG, key, val));
		}

		public BsonDocument SetNumber(string key, double val) {
			return SetBsonValue(new BsonValue(BsonType.DOUBLE, key, val));
		}

		public BsonDocument SetNumber(string key, float val) {
			return SetBsonValue(new BsonValue(BsonType.DOUBLE, key, val));
		}

		public BsonDocument SetString(string key, string val) {
			return SetBsonValue(new BsonValue(BsonType.STRING, key, val));
		}

		public BsonDocument SetCode(string key, string val) {
			return SetBsonValue(new BsonValue(BsonType.CODE, key, val));
		}

		public BsonDocument SetSymbol(string key, string val) {
			return SetBsonValue(new BsonValue(BsonType.SYMBOL, key, val));
		}

		public BsonDocument SetDate(string key, DateTime val) {
			return SetBsonValue(new BsonValue(BsonType.DATE, key, val));
		}

		public BsonDocument SetRegexp(string key, BsonRegexp val) {
			return SetBsonValue(new BsonValue(BsonType.REGEX, key, val));
		}

		public BsonDocument SetBinData(string key, BsonBinData val) {
			return SetBsonValue(new BsonValue(BsonType.BINDATA, key, val));
		}

		public BsonDocument SetDocument(string key, BsonDocument val) {
			return SetBsonValue(new BsonValue(BsonType.OBJECT, key, val));
		}

		public BsonDocument SetArray(string key, BsonArray val) {
			return SetBsonValue(new BsonValue(BsonType.ARRAY, key, val));
		}

		public BsonDocument SetTimestamp(string key, BsonTimestamp val) {
			return SetBsonValue(new BsonValue(BsonType.TIMESTAMP, key, val));
		}

		public BsonDocument SetCodeWScope(string key, BsonCodeWScope val) {
			return SetBsonValue(new BsonValue(BsonType.CODEWSCOPE, key, val));
		}

		public BsonValue DropValue(string key) {
			var bv = GetBsonValue(key);
			if (bv == null) {
				return bv;
			}
			_cachedhash = null;
			_fields.Remove(key);
			_fieldslist.RemoveAll(x => x.Key == key);
			return bv;
		}

		/// <summary>
		/// Removes all data from document.
		/// </summary>
		public void Clear() {
			_cachedhash = null;
			_fieldslist.Clear();
			if (_fields != null) {
				_fields.Clear();
				_fields = null;
			}
		}

		public void Serialize(Stream os) {
			if (os.CanSeek) {
				long start = os.Position;
				os.Position += 4; //skip int32 document size
				using (var bw = new ExtBinaryWriter(os, Encoding.UTF8, true)) {
					foreach (BsonValue bv in _fieldslist) {
						WriteBsonValue(bv, bw);
					}
					bw.Write((byte) 0x00);
					long end = os.Position;
					os.Position = start;
					bw.Write((int) (end - start));
					os.Position = end; //go to the end
				}
			} else {
				byte[] darr;
				var ms = new MemoryStream();
				using (var bw = new ExtBinaryWriter(ms)) {
					foreach (BsonValue bv in _fieldslist) {
						WriteBsonValue(bv, bw);
					}
					darr = ms.ToArray();
				}	
				using (var bw = new ExtBinaryWriter(os, Encoding.UTF8, true)) {
					bw.Write(darr.Length + 4/*doclen*/ + 1/*0x00*/);
					bw.Write(darr);
					bw.Write((byte) 0x00); 
				}
			}
			os.Flush();
		}

		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (!(obj is BsonDocument)) {
				return false;
			}
			BsonDocument d1 = this;
			BsonDocument d2 = ((BsonDocument) obj);
			if (d1.KeysCount != d2.KeysCount) {
				return false;
			}
			foreach (BsonValue bv1 in d1._fieldslist) {
				BsonValue bv2 = d2.GetBsonValue(bv1.Key);
				if (bv1 != bv2) {
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode() {
			if (_cachedhash != null) {
				return (int) _cachedhash;
			}
			unchecked {
				int hash = 1;
				foreach (var bv in _fieldslist) {
					hash = (hash * 31) + bv.GetHashCode(); 
				}
				_cachedhash = hash;
			}
			return (int) _cachedhash;
		}

		public static bool operator ==(BsonDocument d1, BsonDocument d2) {
			return Equals(d1, d2);
		}

		public static bool operator !=(BsonDocument d1, BsonDocument d2) {
			return !(d1 == d2);
		}

		public static BsonDocument ValueOf(object val) {
			if (val == null) {
				return new BsonDocument();
			}
			Type vtype = val.GetType();
			if (val is BsonDocument) {
				return (BsonDocument) val;
			} else if (vtype == typeof(byte[])) {
				return new BsonDocument((byte[]) val);
			} else if (vtype.IsAnonymousType()) {
				BsonDocument doc = new BsonDocument();
				SetAnonType(doc, null, val);
				return doc;
			} 
			throw new InvalidCastException(string.Format("Unsupported cast type: {0}", vtype));
		}

		public object Clone() {
			return new BsonDocument(this);
		}

		public override string ToString() {
			return string.Format("[{0}: {1}]", GetType().Name, 
			                     string.Join(", ", from bv in _fieldslist select bv.ToString())); 
		}
		//.//////////////////////////////////////////////////////////////////
		// 						Private staff										  
		//.//////////////////////////////////////////////////////////////////
		internal BsonDocument Add(BsonValue bv) {
			_cachedhash = null;
			_fieldslist.Add(bv);
			if (_fields != null) {
				_fields[bv.Key] = bv;
			}
			return this;
		}

		internal BsonDocument SetBsonValue(BsonValue val) {
			_cachedhash = null;
			CheckFields();
			if (val.BsonType == BsonType.STRING && val.Key == "_id") {
				val = new BsonValue(BsonType.OID, val.Key, new BsonOid((string) val.Value));
			}
			BsonValue ov;
			if (_fields.TryGetValue(val.Key, out ov)) {
				ov.Key = val.Key;
				ov.BsonType = val.BsonType;
				ov.Value = val.Value;
			} else {
				_fieldslist.Add(val);
				_fields.Add(val.Key, val);
			}
			return this;
		}

		protected virtual void CheckKey(string key) {
		}

		protected void WriteBsonValue(BsonValue bv, ExtBinaryWriter bw) {
			BsonType bt = bv.BsonType;
			switch (bt) {
				case BsonType.EOO:
					break;
				case BsonType.NULL:
				case BsonType.UNDEFINED:	
				case BsonType.MAXKEY:
				case BsonType.MINKEY:
					WriteTypeAndKey(bv, bw);
					break;
				case BsonType.OID:
					{
						WriteTypeAndKey(bv, bw);
						BsonOid oid = (BsonOid) bv.Value;
						Debug.Assert(oid._bytes.Length == 12);
						bw.Write(oid._bytes);
						break;
					}
				case BsonType.STRING:
				case BsonType.CODE:
				case BsonType.SYMBOL:										
					WriteTypeAndKey(bv, bw);
					bw.WriteBsonString((string) bv.Value);
					break;
				case BsonType.BOOL:
					WriteTypeAndKey(bv, bw);
					bw.Write((bool) bv.Value);
					break;
				case BsonType.INT:
					WriteTypeAndKey(bv, bw);
					bw.Write((int) bv.Value);
					break;
				case BsonType.LONG:
					WriteTypeAndKey(bv, bw);
					bw.Write((long) bv.Value);
					break;
				case BsonType.ARRAY:
				case BsonType.OBJECT:
					{					
						BsonDocument doc = (BsonDocument) bv.Value;
						WriteTypeAndKey(bv, bw);
						doc.Serialize(bw.BaseStream);
						break;
					}
				case BsonType.DATE:
					{	
						DateTime dt = (DateTime) bv.Value;
						var diff = dt.ToLocalTime() - BsonConstants.Epoch;
						long time = (long) Math.Floor(diff.TotalMilliseconds);
						WriteTypeAndKey(bv, bw);
						bw.Write(time);
						break;
					}
				case BsonType.DOUBLE:
					WriteTypeAndKey(bv, bw);
					bw.Write((double) bv.Value);
					break;	
				case BsonType.REGEX:
					{
						BsonRegexp rv = (BsonRegexp) bv.Value;		
						WriteTypeAndKey(bv, bw);
						bw.WriteCString(rv.Re ?? "");
						bw.WriteCString(rv.Opts ?? "");						
						break;
					}				
				case BsonType.BINDATA:
					{						
						BsonBinData bdata = (BsonBinData) bv.Value;
						WriteTypeAndKey(bv, bw);
						bw.Write(bdata.Data.Length);
						bw.Write(bdata.Subtype);
						bw.Write(bdata.Data);						
						break;		
					}
				case BsonType.DBREF:
					//Unsupported DBREF!
					break;
				case BsonType.TIMESTAMP:
					{
						BsonTimestamp ts = (BsonTimestamp) bv.Value;
						WriteTypeAndKey(bv, bw);
						bw.Write(ts.Inc);
						bw.Write(ts.Ts);
						break;										
					}		
				case BsonType.CODEWSCOPE:
					{
						BsonCodeWScope cw = (BsonCodeWScope) bv.Value;						
						WriteTypeAndKey(bv, bw);						
						using (var cwwr = new ExtBinaryWriter(new MemoryStream())) {							
							cwwr.WriteBsonString(cw.Code);
							cw.Scope.Serialize(cwwr.BaseStream);					
							byte[] cwdata = ((MemoryStream) cwwr.BaseStream).ToArray();
							bw.Write(cwdata.Length);
							bw.Write(cwdata);
						}
						break;
					}
				default:
					throw new InvalidBsonDataException("Unknown entry type: " + bt);											
			}		
		}

		protected void WriteTypeAndKey(BsonValue bv, ExtBinaryWriter bw) {
			bw.Write((byte) bv.BsonType);
			bw.WriteCString(bv.Key);
		}

		protected void CheckFields() {
			if (_fields != null) {
				return;
			}
			_fields = new Dictionary<string, BsonValue>(Math.Max(_fieldslist.Count + 1, 32));
			foreach (var bv in _fieldslist) {
				_fields.Add(bv.Key, bv);
			}
		}
	}
}
