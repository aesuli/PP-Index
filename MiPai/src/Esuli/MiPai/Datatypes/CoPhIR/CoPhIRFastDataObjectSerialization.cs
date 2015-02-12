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

namespace Esuli.MiPai.Datatypes.CoPhIR
{
    using System;
    using System.IO;
    using Esuli.Base.IO;

    public class CoPhIRFastDataObjectSerialization
        : IFixedSizeObjectSerialization<CoPhIRFastData>
    {
        private static readonly int size = sizeof(int) + CoPhIRFastData.ValuesLength * sizeof(float);

        public CoPhIRFastDataObjectSerialization()
        {
        }

        public int ObjectSize
        {
            get
            {
                return size;
            }
        }

        public void Write(CoPhIRFastData obj, Stream stream)
        {
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            binaryWriter.Write(obj.Id);

            float[] values = obj.Values;
            for (int i = 0; i < CoPhIRFastData.ValuesLength; ++i)
            {
                binaryWriter.Write(values[i]);
            }
        }

        public CoPhIRFastData Read(Stream stream)
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            int id = binaryReader.ReadInt32();

            float[] values = new float[CoPhIRFastData.ValuesLength];
            for (int i = 0; i < CoPhIRFastData.ValuesLength; ++i)
            {
                values[i] = binaryReader.ReadSingle();
            }

            CoPhIRFastData data = new CoPhIRFastData(id, values);

            return data;
        }

        public CoPhIRFastData Read(byte[] buffer, ref long position)
        {
            int id = BitConverter.ToInt32(buffer, (int)position);
            position += sizeof(Int32);

            float[] values = new float[CoPhIRFastData.ValuesLength];
            for (int i = 0; i < CoPhIRFastData.ValuesLength; ++i)
            {
                values[i] = BitConverter.ToSingle(buffer, (int)position);
                position += sizeof(float);
            }

            CoPhIRFastData data = new CoPhIRFastData(id, values);

            return data;
        }
    }
}
