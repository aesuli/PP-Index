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

namespace Esuli.MiPai.Datatypes.BIC
{
    using System;
    using System.IO;
    using Esuli.Base.IO;

    public class BICSerialization : IFixedSizeObjectSerialization<BICDescriptor>
    {
        public BICSerialization()
        {
        }

        public void Write(BICDescriptor referencePoint, Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(referencePoint.Id);
            bw.Write(referencePoint.Data);
        }

        public BICDescriptor Read(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            int id = br.ReadInt32();
            byte[] data = br.ReadBytes(BICDescriptor.DataSize);
            return new BICDescriptor(id,data);
        }

        public BICDescriptor Read(byte [] buffer, ref long position)
        {
            int id = BitConverter.ToInt32(buffer,(int) position);
            position += sizeof(Int32);
            byte[] data = new byte[BICDescriptor.DataSize];
            Array.Copy(buffer, position, data, 0, BICDescriptor.DataSize);
            return new BICDescriptor(id, data);
        }
        
        public int ObjectSize
        {
            get
            {
                return BICDescriptor.DataSize + sizeof(Int32);
            }
        }
    }
}
