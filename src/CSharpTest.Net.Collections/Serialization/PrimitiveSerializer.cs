﻿#region Copyright 2011-2014 by Roger Knapp, Licensed under the Apache License, Version 2.0
/* Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *   http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion
using System;
using System.Collections.Generic;
using System.IO;

namespace CSharpTest.Net.Serialization
{
    /// <summary>
    /// Provides simple implementations of ISerializer&lt;T> for the primitive .Net types.
    /// </summary>
    public class PrimitiveSerializer :
        ISerializer<string>,
        ISerializer<bool>,
        ISerializer<byte>,
        ISerializer<sbyte>,
        ISerializer<byte[]>,
        ISerializer<byte[][]>,
        ISerializer<char>,
        ISerializer<DateTime>,
        ISerializer<TimeSpan>,
        ISerializer<short>,
        ISerializer<ushort>,
        ISerializer<int>,
        ISerializer<int[]>,
        ISerializer<uint>,
        ISerializer<long>,
        ISerializer<long[]>,
        ISerializer<ulong>,
        ISerializer<double>,
        ISerializer<float>,
        ISerializer<Guid>,
        ISerializer<Guid[]>,
        ISerializer<IntPtr>,
        ISerializer<UIntPtr>
    {
        #region Static singleton accessors
        /// <summary> Gets a singleton of the PrimitiveSerializer </summary>
        public static readonly PrimitiveSerializer Instance = new PrimitiveSerializer();
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<string> String = LimitedSerializer.Unlimited;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<bool> Boolean = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<byte> Byte = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<sbyte> SByte = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<byte[]> Bytes = LimitedSerializer.Unlimited;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<char> Char = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<DateTime> DateTime = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<TimeSpan> TimeSpan = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<short> Int16 = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<ushort> UInt16 = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<int> Int32 = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<int[]> Int32Array = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<uint> UInt32 = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<long> Int64 = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<long[]> Int64Array = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<ulong> UInt64 = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<double> Double = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<float> Float = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<Guid> Guid = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<Guid[]> GuidArray = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<IntPtr> IntPtr = Instance;
        /// <summary> Gets a typed version of the PrimitiveSerializer </summary>
        public static readonly ISerializer<UIntPtr> UIntPtr = Instance;
        #endregion

        #region ISerializer<string> Members

        void ISerializer<string>.WriteTo(string value, Stream stream)
        {
            String.WriteTo(value, stream);
        }

        string ISerializer<string>.ReadFrom(Stream stream)
        {
            return String.ReadFrom(stream);
        }

        #endregion
        #region ISerializer<bool> Members

        void ISerializer<bool>.WriteTo(bool value, Stream stream)
        {
            const byte bTrue = 1;
            const byte bFalse = 0;
            stream.WriteByte(value ? bTrue : bFalse);
        }

        bool ISerializer<bool>.ReadFrom(Stream stream)
        {
            int result = stream.ReadByte();
            Check.Assert<InvalidDataException>(result != -1);
            return result == 1;
        }

        #endregion
        #region ISerializer<byte> Members

        void ISerializer<byte>.WriteTo(byte value, Stream stream)
        {
            stream.WriteByte(value);
        }

        byte ISerializer<byte>.ReadFrom(Stream stream)
        {
            int result = stream.ReadByte();
            Check.Assert<InvalidDataException>(result != -1);
            return unchecked((byte)result);
        }

        #endregion
        #region ISerializer<sbyte> Members

        void ISerializer<sbyte>.WriteTo(sbyte value, Stream stream)
        {
            stream.WriteByte(unchecked((byte)value));
        }

        sbyte ISerializer<sbyte>.ReadFrom(Stream stream)
        {
            int result = stream.ReadByte();
            Check.Assert<InvalidDataException>(result != -1);
            return unchecked((sbyte)result);
        }

        #endregion
        #region ISerializer<byte[]> Members

        void ISerializer<byte[]>.WriteTo(byte[] value, Stream stream)
        {
            Bytes.WriteTo(value, stream);
        }
        byte[] ISerializer<byte[]>.ReadFrom(Stream stream)
        {
            return Bytes.ReadFrom(stream);
        }

        #endregion
        #region ISerializer<byte[][]> Members

        /// <summary>
        /// Allows arrays of arrays of bytes up to 255 bytes in length per array
        /// </summary>
        /// <param name="values"></param>
        /// <param name="stream"></param>
        void ISerializer<byte[][]>.WriteTo(byte[][] values, Stream stream)
        {
            List<byte> bytes = new List<byte>();

            if (values != null)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    var val = values[i];

                    if (val == null) continue;

                    bytes.Add((byte)val.Length);

                    for (var j = 0; j < val.Length; j++)
                    {
                        bytes.Add(val[j]);
                    }
                }
            }

            ((ISerializer<byte[]>)this).WriteTo(bytes.ToArray(), stream);
        }

        byte[][] ISerializer<byte[][]>.ReadFrom(Stream stream)
        {
            byte[] bytes = ((ISerializer<byte[]>)this).ReadFrom(stream);
            List<byte[]> bytesList = new List<byte[]>();

            var offset = 0;
            
            if (bytes != null)
            {
                while (offset < bytes.Length)
                {
                    var dataOffset = offset + 1;
                    var length = bytes[offset];
                    var arr = new byte[length];

                    for (var i = 0; i < length; i++)
                    {
                        arr[i] = bytes[dataOffset + i];
                    }

                    bytesList.Add(arr);
                    offset = dataOffset + length;
                }
            }

            return bytesList.ToArray();
        }

        #endregion
        #region ISerializer<char> Members

        void ISerializer<char>.WriteTo(char value, Stream stream)
        {
            VariantNumberSerializer.Int32.WriteTo(value, stream);
        }

        char ISerializer<char>.ReadFrom(Stream stream)
        {
            return unchecked((char)VariantNumberSerializer.Int32.ReadFrom(stream));
        }

        #endregion
        #region ISerializer<DateTime> Members

        void ISerializer<DateTime>.WriteTo(DateTime value, Stream stream)
        {
            ((ISerializer<long>)this).WriteTo(value.ToBinary(), stream);
        }

        DateTime ISerializer<DateTime>.ReadFrom(Stream stream)
        {
            return System.DateTime.FromBinary(((ISerializer<long>)this).ReadFrom(stream));
        }

        #endregion
        #region ISerializer<TimeSpan> Members

        void ISerializer<TimeSpan>.WriteTo(TimeSpan value, Stream stream)
        {
            ((ISerializer<long>)this).WriteTo(value.Ticks, stream);
        }

        TimeSpan ISerializer<TimeSpan>.ReadFrom(Stream stream)
        {
            return new TimeSpan(((ISerializer<long>)this).ReadFrom(stream));
        }

        #endregion
        #region ISerializer<short> Members

        void ISerializer<short>.WriteTo(short value, Stream stream)
        {
            ((ISerializer<ushort>)this).WriteTo(unchecked((ushort)value), stream);
        }

        short ISerializer<short>.ReadFrom(Stream stream)
        {
            return unchecked((short)((ISerializer<ushort>)this).ReadFrom(stream));
        }

        #endregion
        #region ISerializer<ushort> Members

        void ISerializer<ushort>.WriteTo(ushort value, Stream stream)
        {
            unchecked
            {
                stream.WriteByte((byte)(value >> 8));
                stream.WriteByte((byte)value);
            }
        }

        ushort ISerializer<ushort>.ReadFrom(Stream stream)
        {
            unchecked
            {
                int b1 = stream.ReadByte();
                int b2 = stream.ReadByte();
                Check.Assert<InvalidDataException>(b2 != -1);
                return (ushort)((b1 << 8) | b2);
            }
        }

        #endregion
        #region ISerializer<int> Members

        void ISerializer<int>.WriteTo(int value, Stream stream)
        {
            ((ISerializer<uint>)this).WriteTo(unchecked((uint)value), stream);
        }

        int ISerializer<int>.ReadFrom(Stream stream)
        {
            return unchecked((int)((ISerializer<uint>)this).ReadFrom(stream));
        }

        #endregion
        #region ISerializer<int[]> Members

        void ISerializer<int[]>.WriteTo(int[] values, Stream stream)
        {
            byte[] bytes = new byte[values.Length * 4];
            Buffer.BlockCopy(values, 0, bytes, 0, bytes.Length);

            ((ISerializer<byte[]>)this).WriteTo(bytes, stream);
        }

        int[] ISerializer<int[]>.ReadFrom(Stream stream)
        {
            byte[] bytes = ((ISerializer<byte[]>)this).ReadFrom(stream);
            int[] ints = new int[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, ints, 0, bytes.Length);

            return ints;
        }

        #endregion
        #region ISerializer<uint> Members

        void ISerializer<uint>.WriteTo(uint value, Stream stream)
        {
            unchecked
            {
                stream.WriteByte((byte)(value >> 24));
                stream.WriteByte((byte)(value >> 16));
                stream.WriteByte((byte)(value >> 8));
                stream.WriteByte((byte)value);
            }
        }

        uint ISerializer<uint>.ReadFrom(Stream stream)
        {
            unchecked
            {
                int b1 = stream.ReadByte();
                int b2 = stream.ReadByte();
                int b3 = stream.ReadByte();
                int b4 = stream.ReadByte();

                Check.Assert<InvalidDataException>(b4 != -1);
                return (
                    (((uint)b1) << 24) |
                    (((uint)b2) << 16) |
                    (((uint)b3) << 8) |
                    (((uint)b4) << 0)
                    );
            }
        }

        #endregion
        #region ISerializer<long> Members

        void ISerializer<long>.WriteTo(long value, Stream stream)
        {
            ((ISerializer<ulong>)this).WriteTo(unchecked((ulong)value), stream);
        }

        long ISerializer<long>.ReadFrom(Stream stream)
        {
            return unchecked((long)((ISerializer<ulong>)this).ReadFrom(stream));
        }

        #endregion
        #region ISerializer<long[]> Members

        void ISerializer<long[]>.WriteTo(long[] values, Stream stream)
        {
            byte[] bytes = new byte[values.Length * 8];
            Buffer.BlockCopy(values, 0, bytes, 0, bytes.Length);

            ((ISerializer<byte[]>)this).WriteTo(bytes, stream);
        }

        long[] ISerializer<long[]>.ReadFrom(Stream stream)
        {
            byte[] bytes = ((ISerializer<byte[]>)this).ReadFrom(stream);
            long[] longs = new long[bytes.Length / 8];
            Buffer.BlockCopy(bytes, 0, longs, 0, bytes.Length);

            return longs;
        }

        #endregion
        #region ISerializer<ulong> Members

        void ISerializer<ulong>.WriteTo(ulong value, Stream stream)
        {
            unchecked
            {
                stream.WriteByte((byte)(value >> 56));
                stream.WriteByte((byte)(value >> 48));
                stream.WriteByte((byte)(value >> 40));
                stream.WriteByte((byte)(value >> 32));
                stream.WriteByte((byte)(value >> 24));
                stream.WriteByte((byte)(value >> 16));
                stream.WriteByte((byte)(value >> 8));
                stream.WriteByte((byte)value);
            }
        }

        ulong ISerializer<ulong>.ReadFrom(Stream stream)
        {
            unchecked
            {
                int b1 = stream.ReadByte();
                int b2 = stream.ReadByte();
                int b3 = stream.ReadByte();
                int b4 = stream.ReadByte();
                int b5 = stream.ReadByte();
                int b6 = stream.ReadByte();
                int b7 = stream.ReadByte();
                int b8 = stream.ReadByte();
                Check.Assert<InvalidDataException>(b8 != -1);
                return (
                    (((ulong)b1) << 56) |
                    (((ulong)b2) << 48) |
                    (((ulong)b3) << 40) |
                    (((ulong)b4) << 32) |
                    (((ulong)b5) << 24) |
                    (((ulong)b6) << 16) |
                    (((ulong)b7) << 8) |
                    (((ulong)b8) << 0)
                    );
            }
        }

        #endregion
        #region ISerializer<double> Members

        void ISerializer<double>.WriteTo(double value, Stream stream)
        {
            ((ISerializer<long>)this).WriteTo(BitConverter.DoubleToInt64Bits(value), stream);
        }

        double ISerializer<double>.ReadFrom(Stream stream)
        {
            return BitConverter.Int64BitsToDouble(((ISerializer<long>)this).ReadFrom(stream));
        }

        #endregion
        #region ISerializer<float> Members

        void ISerializer<float>.WriteTo(float value, Stream stream)
        {
            ((ISerializer<long>)this).WriteTo(BitConverter.DoubleToInt64Bits(value), stream);
        }

        float ISerializer<float>.ReadFrom(Stream stream)
        {
            return unchecked((float)BitConverter.Int64BitsToDouble(((ISerializer<long>)this).ReadFrom(stream)));
        }

        #endregion
        #region ISerializer<Guid> Members

        void ISerializer<Guid>.WriteTo(Guid value, Stream stream)
        {
            stream.Write(value.ToByteArray(), 0, 16);
        }

        Guid ISerializer<Guid>.ReadFrom(Stream stream)
        {
            byte[] tmp = new byte[16];

            int len, bytesRead = 0;
            while (bytesRead < 16 && 0 != (len = stream.Read(tmp, bytesRead, 16 - bytesRead)))
                bytesRead += len;

            Check.Assert<InvalidDataException>(16 == bytesRead);
            return new Guid(tmp);
        }

        #endregion
        #region ISerializer<Guid[]> Members

        void ISerializer<Guid[]>.WriteTo(Guid[] values, Stream stream)
        {
            byte[] bytes = new byte[values.Length * 16];

            for (var i = 0; i < values.Length; i++)
            {
                var guidBytes = values[i].ToByteArray();
                for (var j = 0; j < 16; j++)
                {
                    bytes[i * 16 + j] = guidBytes[j];
                }
            }

            ((ISerializer<byte[]>)this).WriteTo(bytes, stream);
        }

        Guid[] ISerializer<Guid[]>.ReadFrom(Stream stream)
        {
            byte[] bytes = ((ISerializer<byte[]>)this).ReadFrom(stream);
            Guid[] guids = new Guid[bytes.Length / 16];
            
            for (var i = 0; i < guids.Length; i++)
            {
                var guidBytes = new byte[16];
                for (var j = 0; j < 16; j++)
                {
                    guidBytes[j] = bytes[i * 16 + j];
                }
                guids[i] = new System.Guid(guidBytes);
            }

            return guids;
        }

        #endregion
        #region ISerializer<IntPtr> Members

        void ISerializer<IntPtr>.WriteTo(IntPtr value, Stream stream)
        {
            ((ISerializer<long>)this).WriteTo(value.ToInt64(), stream);
        }

        IntPtr ISerializer<IntPtr>.ReadFrom(Stream stream)
        {
            return new IntPtr(((ISerializer<long>)this).ReadFrom(stream));
        }

        #endregion
        #region ISerializer<UIntPtr> Members

        void ISerializer<UIntPtr>.WriteTo(UIntPtr value, Stream stream)
        {
            ((ISerializer<ulong>)this).WriteTo(value.ToUInt64(), stream);
        }

        UIntPtr ISerializer<UIntPtr>.ReadFrom(Stream stream)
        {
            return new UIntPtr(((ISerializer<ulong>)this).ReadFrom(stream));
        }

        #endregion
    }
}