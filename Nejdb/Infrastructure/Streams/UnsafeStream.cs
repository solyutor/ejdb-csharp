using System;
using System.IO;
using Nejdb.Internals;

namespace Nejdb.Infrastructure.Streams
{
    internal sealed unsafe class UnsafeStream : Stream
    {
        private readonly BsonHandle _bson;
        private readonly byte* _origin;
        private byte* _current;
        //private int _length;

        public UnsafeStream(byte* origin)
        {
            _origin = origin;
            _current = _origin;
        }

        public UnsafeStream(BsonHandle bson) : this(bson.GetBsonBuffer())
        {
            if (bson.IsInvalid)
            {
                throw new InvalidOperationException();
            }
            _bson = bson;
        }

        public override void Flush()
        {

        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return (long) (_current = _origin + offset);
                case SeekOrigin.Current:
                    return (long) (_current + offset);
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int index = offset; index < offset + count; index++)
            {
                buffer[index] = (byte) ReadByte();
            }
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            for (int index = offset; index < offset + count; index++)
            {
                WriteByte(buffer[index]);
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { return _current - _origin; }
            set { _current = value + _origin; }
        }

        public override int ReadByte()
        {
            var result = *(_current);
            _current++;
            return result;
        }

        public override void WriteByte(byte value)
        {
            *(_current) = value;
            _current++;
        }

        protected override void Dispose(bool disposing)
        {
            if(_bson == null) return;
            
            _bson.Dispose();
        }
    }
}