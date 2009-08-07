using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PcapDotNet.Base;
using PcapDotNet.Packets.Ethernet;

namespace PcapDotNet.Packets
{
    /// <summary>
    /// Represents a packet datagram.
    /// A datagram is part of the packet bytes that can be treated as a specific protocol data (usually header + payload).
    /// Never copies the given buffer.
    /// </summary>
    public class Datagram : IEquatable<Datagram>, IEnumerable<byte>
    {
        /// <summary>
        /// Take all the bytes as a datagram.
        /// </summary>
        /// <param name="buffer">The buffer to take as a datagram.</param>
        public Datagram(byte[] buffer)
            : this(buffer, 0, buffer.Length)
        {
        }

        /// <summary>
        /// Take only part of the bytes as a datagarm.
        /// </summary>
        /// <param name="buffer">The bytes to take the datagram from.</param>
        /// <param name="offset">The offset in the buffer to start taking the bytes from.</param>
        /// <param name="length">The number of bytes to take.</param>
        public Datagram(byte[] buffer, int offset, int length)
        {
            _buffer = buffer;
            _startOffset = offset;
            _length = length;
        }

        /// <summary>
        /// An empty datagram.
        /// Useful for empty payloads.
        /// </summary>
        public static Datagram Empty
        {
            get { return _empty; }
        }

        /// <summary>
        /// The number of bytes in this datagram.
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// The value of the byte in the given offset in the datagram.
        /// </summary>
        /// <param name="offset">The offset in the datagram to take the byte from.</param>
        public byte this[int offset]
        {
            get { return _buffer[StartOffset + offset]; }
        }

        /// <summary>
        /// A datagram is checked for validity according to the protocol.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (_isValid == null)
                    _isValid = CalculateIsValid();
                return _isValid.Value;
            }
        }

        /// <summary>
        /// Iterate through all the bytes in the datagram.
        /// </summary>
        public IEnumerator<byte> GetEnumerator()
        {
            for (int i = 0; i != Length; ++i)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Two datagrams are equal if the have the same data.
        /// </summary>
        public bool Equals(Datagram other)
        {
            if (Length != other.Length)
                return false;

            for (int i = 0; i != Length; ++i)
            {
                if (this[i] != other[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Two datagrams are equal if the have the same data.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as Datagram);
        }

        /// <summary>
        /// The hash code of a datagram is the hash code of its length xored with all its bytes (each byte is xored with the next byte in the integer).
        /// </summary>
        public override int GetHashCode()
        {
            return Length.GetHashCode() ^ this.BytesSequenceGetHashCode();
        }

        internal void Write(byte[] buffer, int offset)
        {
            _buffer.BlockCopy(StartOffset, buffer, offset, Length);
        }

        /// <summary>
        /// The original buffer that holds all the data for the datagram.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        protected byte[] Buffer
        {
            get { return _buffer; }
        }

        /// <summary>
        /// The offset of the first byte of the datagram in the buffer.
        /// </summary>
        protected int StartOffset
        {
            get { return _startOffset; }
        }

        /// <summary>
        /// The default validity check always returns true.
        /// </summary>
        protected virtual bool CalculateIsValid()
        {
            return true;
        }

        /// <summary>
        /// Reads 2 bytes from a specific offset in the datagram as a ushort with a given endianity.
        /// </summary>
        /// <param name="offset">The offset in the datagram to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "ushort")]
        protected ushort ReadUShort(int offset, Endianity endianity)
        {
            return Buffer.ReadUShort(StartOffset + offset, endianity);
        }

        /// <summary>
        /// Reads 4 bytes from a specific offset in the datagram as a uint with a given endianity.
        /// </summary>
        /// <param name="offset">The offset in the datagram to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "uint")]
        protected uint ReadUInt(int offset, Endianity endianity)
        {
            return Buffer.ReadUInt(StartOffset + offset, endianity);
        }

        /// <summary>
        /// Reads 6 bytes from a specific offset in the datagram as a MacAddress with a given endianity.
        /// </summary>
        /// <param name="offset">The offset in the datagram to start reading.</param>
        /// <param name="endianity">The endianity to use to translate the bytes to the value.</param>
        /// <returns>The value converted from the read bytes according to the endianity.</returns>
        protected MacAddress ReadMacAddress(int offset, Endianity endianity)
        {
            return Buffer.ReadMacAddress(StartOffset + offset, endianity);
        }

        private static readonly Datagram _empty = new Datagram(new byte[0], 0, 0);
        private readonly byte[] _buffer;
        private readonly int _startOffset;
        private readonly int _length;
        private bool? _isValid;
    }
}