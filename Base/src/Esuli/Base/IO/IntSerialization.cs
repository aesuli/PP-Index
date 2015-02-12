// Copyright (C) 2013 Andrea Esuli
// http://www.esuli.it
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace Esuli.Base.IO
{
    using System;
    using System.IO;

    /// <summary>
    /// Serialization of ints.
    /// </summary>
    public class IntSerialization : IObjectSerialization<int>
    {
        void IObjectSerialization<int>.Write(int obj, Stream stream)
        {
            IntSerialization.Write(obj, stream);
        }

        int IObjectSerialization<int>.Read(Stream stream)
        {
            return IntSerialization.Read(stream);
        }

        int IObjectSerialization<int>.Read(byte[] buffer, ref long position)
        {
            return IntSerialization.Read(buffer, ref position);
        }

        public static void Write(int value, Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static int Read(Stream stream)
        {
            byte[] buffer = new byte[sizeof(Int32)];
            stream.Read(buffer, 0, sizeof(Int32));
            return BitConverter.ToInt32(buffer,0);
        }

        public static int Read(byte[] buffer, ref long position)
        {
            int value = BitConverter.ToInt32(buffer,(int) position);
            position += sizeof(Int32);
            return value;
        }

        public static void Skip(Stream s)
        {
            s.Position += sizeof(Int32);
        }
    }
}
