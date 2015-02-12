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

namespace Esuli.MiPai.Datatypes.Corel
{
    using System;
    using System.IO;
    using Esuli.Base.IO;

    public class CorelDataObjectSerialization : IFixedSizeObjectSerialization<CorelData>
    {
        private static readonly int length = 32;
        private static readonly int size = (32 * 8)+4;

        public CorelDataObjectSerialization()
        {
        }

        public void Write(CorelData referencePoint, Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(referencePoint.Id);
            for (int i = 0; i < referencePoint.Features.Length; ++i)
            {
                bw.Write(referencePoint.Features[i]);
            }
        }

        public CorelData Read(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            int id = br.ReadInt32();
            double[] data = new double[length];
            for (int i = 0; i < length; ++i)
            {
                data[i] = br.ReadDouble();
            }
            return new CorelData(id, data);
        }

        public CorelData Read(byte[] buffer, ref long position)
        {
            int id = BitConverter.ToInt32(buffer, (int)position);
            position += sizeof(Int32);
            double[] data = new double[length];
            for (int i = 0; i < length; ++i)
            {
                data[i] = BitConverter.ToInt64(buffer, (int)position);
                position += sizeof(Int64);
            }
            return new CorelData(id, data);
        }

        public int ObjectSize
        {
            get
            {
                return size;
            }
        }
    }
}
