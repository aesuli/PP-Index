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
    using System;
    using System.IO;
    using Esuli.Base.IO;
    using Esuli.MiPai.Index;
    
    public class PermutationPrefixSerialization : IObjectSerialization<PermutationPrefix>
    {

        public void Write(PermutationPrefix obj, Stream stream)
        {
            var writer = new BinaryWriter(stream);
            var data = obj.Data;
            var length = data.Length;
            writer.Write(length);
            for (var i = 0; i < length; ++i)
            {
                writer.Write(data[i]);
            }
        }

        public PermutationPrefix Read(Stream stream)
        {
            var reader = new BinaryReader(stream);
            var length = reader.ReadInt32();
            var data  = new int [length];
            for (var i = 0; i < length; ++i)
            {
                data[i] = reader.ReadInt32();
            }
            return new PermutationPrefix(data);
        }

        public PermutationPrefix Read(byte[] buffer, ref long position)
        {
            var length = BitConverter.ToInt32(buffer,(int) position);
            position += sizeof(int);
            var data = new int[length];
            for (var i = 0; i < length; ++i)
            {
                data[i] = BitConverter.ToInt32(buffer, (int)position);
                position += sizeof(int);
            }
            return new PermutationPrefix(data);
        }
    }
}
