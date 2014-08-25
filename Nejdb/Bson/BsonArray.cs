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

namespace Nejdb.Bson
{

    /// <summary>
    /// Implementes an array of bson values. 
    /// </summary>
    [Serializable]
    public class BsonArray : BsonDocument
    {
        /// <summary>
        /// Bson Type this document. 
        /// </summary>
        /// <remarks>
        /// Type is always <see cref="Bson.BsonType.ARRAY"/>
        /// </remarks>
        /// <value>The type of the Bson.</value>
        public override BsonType BsonType
        {
            get {  return BsonType.ARRAY; }
        }

        /// <summary>
        /// Gets bson value at specified position
        /// </summary>
        /// <param name="index"></param>
        public object this[int index]
        {
            get { return GetObjectValue(index.ToString()); }
        }

        /// <summary>
        /// Creates empty instance of <see cref="BsonArray"/>
        /// </summary>
        public BsonArray()
        {
        }

        
        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Values in array</param>
        public BsonArray(BsonUndefined[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetUndefined(i);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of null values to place in <see cref="BsonArray"/></param>
        public BsonArray(BsonNull[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetNull(i);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Placeholders for values</param>
        public BsonArray(ushort[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetNumber(i, array[i]);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="uint"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(uint[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetNumber(i, array[i]);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="ulong"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(ulong[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetNumber(i, (long)array[i]);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="short"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(short[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetNumber(i, array[i]);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="string"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(string[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetString(i, array[i]);
            }
        }
        
        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="int"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(int[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetNumber(i, array[i]);
            }
        }
        
        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="long"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(long[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetNumber(i, array[i]);
            }
        }
        
        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="float"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(float[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetNumber(i, array[i]);
            }
        }
        
        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="double"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(double[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetNumber(i, array[i]);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="bool"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(bool[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetBool(i, array[i]);
            }
        }
        
        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="ObjectId"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(ObjectId[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetObjectId(i, array[i]);
            }
        }
        
        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="DateTime"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(DateTime[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetDate(i, array[i]);
            }
        }
        
        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="BsonDocument"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(BsonDocument[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetObject(i, array[i]);
            }
        }
        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="BsonArray"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(BsonArray[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetArray(i, array[i]);
            }
        }
        
        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="BsonRegexp"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(BsonRegexp[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetRegexp(i, array[i]);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="BsonTimestamp"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(BsonTimestamp[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetTimestamp(i, array[i]);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="BsonCodeWScope"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(BsonCodeWScope[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetCodeWScope(i, array[i]);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="BsonArray"/>
        /// </summary>
        /// <param name="array">Array of <seealso cref="BsonBinData"/> values to place in <see cref="BsonArray"/></param>
        public BsonArray(BsonBinData[] array)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                SetBinData(i, array[i]);
            }
        }

        /// <summary>
        /// Sets null at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetNull(int index)
        {
            return SetNull(index.ToString());
        }

        /// <summary>
        /// Sets null at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetUndefined(int index)
        {
            return SetUndefined(index.ToString());
        }

        /// <summary>
        /// Sets bson max key at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetMaxKey(int index)
        {
            return SetMaxKey(index.ToString());
        }

        /// <summary>
        /// Sets bson min key specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetMinKey(int index)
        {
            return SetMinKey(index.ToString());
        }

        /// <summary>
        /// Sets <see cref="ObjectId"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">String representation of <see cref="ObjectId"/></param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetObjectId(int index, string value)
        {
            return SetOID(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="ObjectId"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An <see cref="ObjectId"/> value</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetObjectId(int index, ObjectId value)
        {
            return SetOID(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="bool"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An <see cref="bool"/> value</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetBool(int index, bool value)
        {
            return SetBool(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="int"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An <see cref="int"/> value</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetNumber(int index, int value)
        {
            return SetNumber(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="long"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An <see cref="long"/> value</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetNumber(int index, long value)
        {
            return SetNumber(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="double"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An <see cref="double"/> value</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetNumber(int index, double value)
        {
            return SetNumber(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="float"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An <see cref="float"/> value</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetNumber(int index, float value)
        {
            return SetNumber(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="string"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An <see cref="string"/> value</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetString(int index, string value)
        {
            return SetString(index.ToString(), value);
        }

        /// <summary>
        /// Sets javascript code at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">Javascript code as string</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetCode(int index, string value)
        {
            return SetCode(index.ToString(), value);
        }

        /// <summary>
        /// Sets bson symbol at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">Bson symbol</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetSymbol(int index, string value)
        {
            return SetSymbol(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="DateTime"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An <see cref="DateTime"/> value</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetDate(int index, DateTime value)
        {
            return SetDate(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="BsonRegexp"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An <see cref="BsonRegexp"/> value</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetRegexp(int index, BsonRegexp value)
        {
            return SetRegexp(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="BsonBinData"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An <see cref="BsonBinData"/> value</param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetBinData(int index, BsonBinData value)
        {
            return SetBinData(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="BsonDocument"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An instance of <see cref="BsonDocument"/></param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetObject(int index, BsonDocument value)
        {
            return SetDocument(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="BsonArray"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An instance of <see cref="BsonArray"/></param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetArray(int index, BsonArray value)
        {
            return SetArray(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="BsonTimestamp"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An instance of <see cref="BsonTimestamp"/></param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetTimestamp(int index, BsonTimestamp value)
        {
            return SetTimestamp(index.ToString(), value);
        }

        /// <summary>
        /// Sets <see cref="BsonCodeWScope"/> value at specified position.
        /// </summary>
        /// <param name="index">Index to place value at</param>
        /// <param name="value">An instance of <see cref="BsonCodeWScope"/></param>
        /// <returns>Returns itself</returns>
        public BsonDocument SetCodeWScope(int index, BsonCodeWScope value)
        {
            return SetCodeWScope(index.ToString(), value);
        }

        /// <summary>
        /// Ensures that specified index exists. 
        /// </summary>
        /// <param name="index">Index to check</param>
        /// <exception cref="InvalidBsonDataException">If index does not exists</exception>
        protected override void CheckKey(string index)
        {
            int parsedIndex;
            if (index == null || !int.TryParse(index, out parsedIndex) || parsedIndex < 0)
            {
                throw new InvalidBsonDataException(string.Format("Invalid array index: {0}", index));
            }
        }
    }
}

