using System;
using Ejdb.BSON;

namespace Ejdb.DB
{
	public class Hints
	{
		private BSONDocument _hints;

		public Hints()
		{
			_hints = new BSONDocument();
		}
		public Hints Max(int max)
		{
			if (max < 0)
			{
				throw new ArgumentException("Max limit cannot be negative");
			}
			
			_hints["$max"] = max;
			return this;
		}

		public Hints Skip(int skip)
		{
			if (skip < 0)
			{
				throw new ArgumentException("Skip value cannot be negative");
			}
			if (_hints == null)
			{
				_hints = new BSONDocument();
			}
			_hints["$skip"] = skip;
			return this;
		}

		public Hints OrderBy(string field, bool asc = true)
		{
			if (_hints == null)
			{
				_hints = new BSONDocument();
			}
			BSONDocument oby = _hints["$orderby"] as BSONDocument;
			if (oby == null)
			{
				oby = new BSONDocument();
				_hints["$orderby"] = oby;
			}
			oby[field] = (asc) ? 1 : -1;
			
			return this;
		}

		public Hints IncludeFields(params string[] fields)
		{
			return IncExFields(fields, 1);
		}

		public Hints ExcludeFields(params string[] fields)
		{
			return IncExFields(fields, 0);
		}

		public bool HasHints
		{
			get { return _hints.KeysCount > 0; }
		}

		public bool IsEmpty
		{
			get { return _hints.KeysCount == 0; }
		}

		private Hints IncExFields(string[] fields, int inc)
		{
			if (_hints == null)
			{
				_hints = new BSONDocument();
			}
			BSONDocument fdoc = _hints["$fields"] as BSONDocument;
			if (fdoc == null)
			{
				fdoc = new BSONDocument();
				_hints["$fields"] = fdoc;
			}
			foreach (var fn in fields)
			{
				fdoc[fn] = inc;
			}
			
			return this;
		}

		public byte[] ToByteArray()
		{
			return _hints.ToByteArray();
		}
	}
}