using System;
using System.IO;

namespace CR2ToJPG
{
    internal class PartialStream : Stream
    {
        private FileStream m_f;

        private int m_length;

        private uint m_start;

        internal PartialStream(FileStream p_f, uint p_start, int p_length)
        {
            m_f = p_f;
            m_start = p_start;
            m_length = p_length;

            m_f.Seek(p_start, SeekOrigin.Begin);
        }

        public override bool CanRead
        {
            get { return m_f.CanRead; }
        }

        public override bool CanSeek
        {
            get { return m_f.CanSeek; }
        }

        public override bool CanTimeout
        {
            get { return m_f.CanTimeout; }
        }

        public override bool CanWrite
        {
            get { return m_f.CanWrite; }
        }

        public override long Length
        {
            get { return m_length; }
        }

        public override long Position
        {
            get { return m_f.Position - m_start; }
            set { m_f.Position = value + m_start; }
        }

        public override int ReadTimeout
        {
            get { return m_f.ReadTimeout; }
            set { m_f.ReadTimeout = value; }
        }

        public override int WriteTimeout
        {
            get { return m_f.WriteTimeout; }
            set { m_f.WriteTimeout = value; }
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return m_f.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return m_f.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void Close()
        {
            m_f.Close();
        }

        public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
        {
            return m_f.CreateObjRef(requestedType);
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return m_f.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            m_f.EndWrite(asyncResult);
        }

        public override bool Equals(object obj)
        {
            return m_f.Equals(obj);
        }

        public override void Flush()
        {
            m_f.Flush();
        }

        public override int GetHashCode()
        {
            return m_f.GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return m_f.InitializeLifetimeService();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long maxRead = Length - Position;
            return m_f.Read(buffer, offset, (count <= maxRead) ? count : (int)maxRead);
        }

        public override int ReadByte()
        {
            if (Position < Length)
                return m_f.ReadByte();
            else
                return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return m_f.Seek(offset + m_start, origin);
        }

        public override void SetLength(long value)
        {
            m_f.SetLength(value);
        }

        public override string ToString()
        {
            return m_f.ToString();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_f.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            m_f.WriteByte(value);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}