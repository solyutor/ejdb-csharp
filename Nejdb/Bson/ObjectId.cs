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
using System.Runtime.InteropServices;
using Nejdb.Internals;

namespace Nejdb.Bson
{

    /// <summary>
    /// Encapsulates value and operations over BSON id object.
    /// <remarks>Look at http://bsonspec.org/ for details</remarks>
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct ObjectId : IBsonValue
    {
        /// <summary>
        /// Represents and empty identifier
        /// </summary>
        public static readonly ObjectId Empty = new ObjectId();

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

        /// <summary>
        /// Returns bson type for ObjectId
        /// </summary>
        public BsonType BsonType
        {
            get { return BsonType.OID; }
        }

        /// <summary>
        /// Return true if the instance is empty, false otherwise.
        /// </summary>
        public bool IsEmpty
        {
            get { return this.Equals(Empty); }
        }

        /// <summary>
        /// Creates new instance of <see cref="ObjectId"/> from specified <seealso cref="byte"/> values.
        /// </summary>
        public ObjectId(byte byte01, byte byte02, byte byte03, byte byte04, byte byte05, byte byte06, byte byte07, byte byte08, byte byte09, byte byte10, byte byte11, byte byte12)
        {
            Byte01 = byte01;
            Byte02 = byte02;
            Byte03 = byte03;
            Byte04 = byte04;
            Byte05 = byte05;
            Byte06 = byte06;
            Byte07 = byte07;
            Byte08 = byte08;
            Byte09 = byte09;
            Byte10 = byte10;
            Byte11 = byte11;
            Byte12 = byte12;
        }

        /// <summary>
        /// Creates new instance of <see cref="ObjectId"/> from its string representation
        /// </summary>
        /// <param name="value">A string representation of <see cref="ObjectId"/></param>
        /// <exception cref="ArgumentException"></exception>
        public ObjectId(string value)
        {
            if (!IsValidObjectId(value))
            {
                throw new ArgumentException("Invalid objectId string");
            }

            Byte01 = value.ToByteFromHex(0);
            Byte02 = value.ToByteFromHex(2);
            Byte03 = value.ToByteFromHex(4);
            Byte04 = value.ToByteFromHex(6);
            Byte05 = value.ToByteFromHex(8);
            Byte06 = value.ToByteFromHex(10);
            Byte07 = value.ToByteFromHex(12);
            Byte08 = value.ToByteFromHex(14);
            Byte09 = value.ToByteFromHex(16);
            Byte10 = value.ToByteFromHex(18);
            Byte11 = value.ToByteFromHex(20);
            Byte12 = value.ToByteFromHex(22);
        }

        /// <summary>
        /// Creates new instance of <see cref="ObjectId"/> from its byte array representation
        /// </summary>
        /// <param name="value"><see cref="ObjectId"/> as byte array</param>
        public ObjectId(byte[] value)
        {
            Byte01 = value[0];
            Byte02 = value[1];
            Byte03 = value[2];
            Byte04 = value[3];
            Byte05 = value[4];
            Byte06 = value[5];
            Byte07 = value[6];
            Byte08 = value[7];
            Byte09 = value[8];
            Byte10 = value[9];
            Byte11 = value[10];
            Byte12 = value[11];
        }

        /// <summary>
        /// Creates new instance of <see cref="ObjectId"/> from its byte array representation in array segment
        /// </summary>
        /// <param name="value"><see cref="ObjectId"/> as byte array</param>
        public ObjectId(ArraySegment<byte> value)
        {
            var val = value.Array;
            Byte01 = val[value.Offset + 0];
            Byte02 = val[value.Offset + 1];
            Byte03 = val[value.Offset + 2];
            Byte04 = val[value.Offset + 3];
            Byte05 = val[value.Offset + 4];
            Byte06 = val[value.Offset + 5];
            Byte07 = val[value.Offset + 6];
            Byte08 = val[value.Offset + 7];
            Byte09 = val[value.Offset + 8];
            Byte10 = val[value.Offset + 9];
            Byte11 = val[value.Offset + 10];
            Byte12 = val[value.Offset + 11];
        }

        /// <summary>
        /// Creates new instance of <see cref="ObjectId"/> from binary reader
        /// </summary>
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

        /// <summary>
        /// Check validity of string representation of <see cref="ObjectId"/>
        /// </summary>
        /// <param name="objectId"><see cref="ObjectId"/> as string</param>
        /// <returns>True if object id is valid. False otherwise.</returns>
        public static bool IsValidObjectId(string objectId)
        {
            if (objectId.Length != 24)
            {
                return false;

            }
            for (int j = 0; j < objectId.Length; j++)
            {
                var symbol = Char.ToLowerInvariant(objectId[j]);

                if ((symbol >= '0' && symbol <= '9') || ((symbol >= 'a' && symbol <= 'f')))
                {
                    continue;
                }
                return false;
            }
            return true;
        }


        /// <summary>
        /// Converts <see cref="ObjectId"/> to byte array representation
        /// </summary>
        public byte[] ToBytes()
        {
            return new[] { Byte01, Byte02, Byte03, Byte04, Byte05, Byte06, Byte07, Byte08, Byte09, Byte10, Byte11, Byte12 };
        }

        /// <summary>
        /// Returns <see cref="string"/> representation of <see cref="ObjectId"/> using hex numbers.
        /// </summary>
        public override string ToString()
        {
            return BitConverter.ToString(ToBytes()).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
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
            return Equals((ObjectId)obj);
        }

        /// <summary>
        /// Check if identifier is equal to other one
        /// </summary>
        public bool Equals(ObjectId other)
        {
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

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Byte01.GetHashCode();
                hashCode = (hashCode*397) ^ Byte02.GetHashCode();
                hashCode = (hashCode*397) ^ Byte03.GetHashCode();
                hashCode = (hashCode*397) ^ Byte04.GetHashCode();
                hashCode = (hashCode*397) ^ Byte05.GetHashCode();
                hashCode = (hashCode*397) ^ Byte06.GetHashCode();
                hashCode = (hashCode*397) ^ Byte07.GetHashCode();
                hashCode = (hashCode*397) ^ Byte08.GetHashCode();
                hashCode = (hashCode*397) ^ Byte09.GetHashCode();
                hashCode = (hashCode*397) ^ Byte10.GetHashCode();
                hashCode = (hashCode*397) ^ Byte11.GetHashCode();
                hashCode = (hashCode*397) ^ Byte12.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Checks equality of two <see cref="ObjectId"/>
        /// </summary>
        /// <returns>True if object id are equal, False otherwise</returns>
        public static bool operator ==(ObjectId a, ObjectId b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Checks inequality of two <see cref="ObjectId"/>
        /// </summary>
        /// <returns>True if object id are not equal, False otherwise</returns>
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

        /// <summary>
        /// Converts a hex string to ObjectId
        /// </summary>
        public static explicit operator ObjectId(string value)
        {
            return new ObjectId(value);
        }
        
        /// <summary>
        /// Converts byte array to an instanse of  <see cref="ObjectId"/>
        /// </summary>
        public static explicit operator ObjectId(byte[] value)
        {
            return new ObjectId(value);
        }
    }
}
