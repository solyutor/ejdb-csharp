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
using System.IO;
using Nejdb.Internals;

namespace Nejdb.Bson
{

	[Serializable]
	public struct ObjectId : IBsonValue
	{
		public readonly byte Byte01;
		public readonly byte Byte02;
		public readonly byte Byte03;
		public readonly byte Byte04;
		public readonly byte Byte05;
		public readonly byte Byte06;
		public readonly byte Byte07;
		public readonly byte Byte08;
		public readonly byte Byte09;
		public readonly byte Byte10;
		public readonly byte Byte11;
		public readonly byte Byte12;

		public BsonType BsonType
		{
			get { return BsonType.OID; }
		}

		public ObjectId(string val)
		{
			if (!IsValidOid(val))
			{
				throw new ArgumentException("Invalid oid string");
			}

			Byte01 = val.ToByteFromHex(0);
			Byte02 = val.ToByteFromHex(2);
			Byte03 = val.ToByteFromHex(4);
			Byte04 = val.ToByteFromHex(6);
			Byte05 = val.ToByteFromHex(8);
			Byte06 = val.ToByteFromHex(10);
			Byte07 = val.ToByteFromHex(12);
			Byte08 = val.ToByteFromHex(14);
			Byte09 = val.ToByteFromHex(16);
			Byte10 = val.ToByteFromHex(18);
			Byte11 = val.ToByteFromHex(20);
			Byte12 = val.ToByteFromHex(22);
		}

		public ObjectId(byte[] val)
		{
			Byte01 = val[0];
			Byte02 = val[1];
			Byte03 = val[2];
			Byte04 = val[3];
			Byte05 = val[4];
			Byte06 = val[5];
			Byte07 = val[6];
			Byte08 = val[7];
			Byte09 = val[8];
			Byte10 = val[9];
			Byte11 = val[10];
			Byte12 = val[11];
		}

		public ObjectId(BinaryReader reader)
		{
			Byte01 = reader.ReadByte();
			Byte02 = reader.ReadByte();
			Byte03 = reader.ReadByte();
			Byte04 = reader.ReadByte();
			Byte05 = reader.ReadByte();
			Byte06 = reader.ReadByte();
			Byte07 = reader.ReadByte();
			Byte08 = reader.ReadByte();
			Byte09 = reader.ReadByte();
			Byte10 = reader.ReadByte();
			Byte11 = reader.ReadByte();
			Byte12 = reader.ReadByte();

		}

		public static bool IsValidOid(string oid)
		{
			if (oid.Length != 24)
			{
				return false;

			}
			for (int j = 0; j < oid.Length; j++)
			{
				var symbol = Char.ToLowerInvariant(oid[j]);

				if ((symbol >= '0' && symbol <= '9') || ((symbol >= 'a' && symbol <= 'f')))
				{
					continue;
				}
				return false;
			}
			return true;
		}


		public byte[] ToBytes()
		{
			return new[] { Byte01, Byte02, Byte03, Byte04, Byte05, Byte06, Byte07, Byte08, Byte09, Byte10, Byte11, Byte12 };
		}

		public override string ToString()
		{
			return BitConverter.ToString(ToBytes()).Replace("-", "").ToLower();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!(obj is ObjectId))
			{
				return false;
			}
			var other = (ObjectId)obj;

			return
				Byte01 == other.Byte01 &&
				Byte02 == other.Byte02 &&
				Byte03 == other.Byte03 &&
				Byte04 == other.Byte04 &&
				Byte05 == other.Byte05 &&
				Byte06 == other.Byte06 &&
				Byte07 == other.Byte07 &&
				Byte08 == other.Byte08 &&
				Byte09 == other.Byte09 &&
				Byte10 == other.Byte10 &&
				Byte11 == other.Byte11 &&
				Byte12 == other.Byte12;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public static bool operator ==(ObjectId a, ObjectId b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ObjectId a, ObjectId b)
		{
			return !(a == b);
		}

		//public static bool operator >(ObjectId a, ObjectId b)
		//{
		//    return a.CompareTo(b) > 0;
		//}

		//public static bool operator <(ObjectId a, ObjectId b)
		//{
		//    return a.CompareTo(b) < 0;
		//}

		public static implicit operator ObjectId(string val)
		{
			return new ObjectId(val);
		}
	}
}
