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

namespace Esuli.MiPai.IO
{
    using System.IO;
    using Esuli.Base.IO;
    using Esuli.MiPai.Index;

    public class SortedPermutationPrefixSerialization
        : ISequentialObjectSerialization<PermutationPrefix> 
    {
        public SortedPermutationPrefixSerialization()
        {
        }

        public void WriteFirst(PermutationPrefix obj, Stream stream)
        {
            int[] data = obj.Data;
            VariableByteCoding.Write(data.Length, stream);
            for (int i = 0; i < data.Length; ++i)
            {
                VariableByteCoding.Write(data[i], stream);
            }
        }

        public void Write(PermutationPrefix obj, PermutationPrefix previousObj, Stream stream)
        {
            int[] data = obj.Data;
            for (int i = 0; i < data.Length; ++i)
            {
                VariableByteCoding.Write(data[i], stream);
            }
        }

        public PermutationPrefix ReadFirst(Stream stream)
        {
            int length = (int) VariableByteCoding.Read(stream);
            int[] data = new int[length];
            for (int i = 0; i < length; ++i)
            {
                data[i] = (int)VariableByteCoding.Read(stream);
            }

            return new PermutationPrefix(data);
        }

        public PermutationPrefix Read(PermutationPrefix previousObj, Stream stream)
        {
            int length = previousObj.Data.Length;
            int[] data = new int[length];
            for (int i = 0; i < length; ++i)
            {
                data[i] = (int)VariableByteCoding.Read(stream);
            }

            return new PermutationPrefix(data);
        }

        public PermutationPrefix ReadFirst(byte [] buffer, ref long position)
        {
            int length = (int)VariableByteCoding.Read(buffer, ref position);
            int[] data = new int[length];
            for (int i = 0; i < length; ++i)
            {
                data[i] = (int)VariableByteCoding.Read(buffer, ref position);
            }

            return new PermutationPrefix(data);
        }

        public PermutationPrefix Read(PermutationPrefix previousObj, byte[] buffer, ref long position)
        {
            int length = previousObj.Data.Length;
            int[] data = new int[length];
            for (int i = 0; i < length; ++i)
            {
                data[i] = (int)VariableByteCoding.Read(buffer, ref position);
            }

            return new PermutationPrefix(data);
        }
    }
}
