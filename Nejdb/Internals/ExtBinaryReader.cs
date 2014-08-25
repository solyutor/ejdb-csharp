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

using System.Collections.Generic;
using System.IO;
using System.Text;
using Nejdb.Bson;

namespace Nejdb.Internals
{

    public class ExtBinaryReader : BinaryReader
    {
        private readonly bool _leaveopen;

        public ExtBinaryReader(Stream input) : this(input, BsonConstants.Encoding)
        {
        }

        public ExtBinaryReader(Stream input, Encoding encoding) : this(input, encoding, false)
        {
        }

        public ExtBinaryReader(Stream input, bool leaveOpen) : this(input, BsonConstants.Encoding, leaveOpen)
        {
        }

        public ExtBinaryReader(Stream input, Encoding encoding, bool leaveopen) : base(input, encoding)
        {
            _leaveopen = leaveopen;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(!_leaveopen);
        }

        public string ReadCString()
        {
            List<byte> sb = new List<byte>(64);
            byte bv;
            while ((bv = ReadByte()) != 0x00)
            {
                sb.Add(bv);
            }

            return BsonConstants.Encoding.GetString(sb.ToArray());
        }

        public void SkipCString()
        {
            while ((ReadByte()) != 0x00);
        }
    }
}