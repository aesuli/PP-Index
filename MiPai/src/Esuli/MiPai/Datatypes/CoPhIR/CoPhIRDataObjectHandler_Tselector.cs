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
    using Esuli.MiPai.Datatypes.MPEG7;

    public class CoPhIRDataObjectSerialization<Tselector> 
        : IFixedSizeObjectSerialization<CoPhIRData>
        where Tselector : ISelector, new ()
    {
        private static readonly int size;
        private static readonly Tselector selector;

        static CoPhIRDataObjectSerialization()
        {
            size = sizeof(int)*3;
            size += sizeof(long);
            selector = new Tselector();
            if (selector.UseColorLayout)
            {
                size += 12;
            }
            if (selector.UseColorStructure)
            {
                size += 64;
            }
            if (selector.UseEdgeHistogram)
            {
                size += 80;
            }
            if (selector.UseHomogeneousTexture)
            {
                size += 63;
            }
            if (selector.UseScalableColor)
            {
                size += 136;
            }
        }

        public CoPhIRDataObjectSerialization()
        {
        }

        public int ObjectSize
        {
            get
            {
                return size;
            }
        }

        public void Write(CoPhIRData obj, Stream stream)
        {
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            binaryWriter.Write(obj.Id);
            binaryWriter.Write(obj.FarmId);
            binaryWriter.Write(obj.ServerId);
            binaryWriter.Write(obj.Secret);

            if (selector.UseColorLayout)
            {
                binaryWriter.Write(obj.ColorLayout.YDCCoeff);
                binaryWriter.Write(obj.ColorLayout.CbDCCoeff);
                binaryWriter.Write(obj.ColorLayout.CrDCCoeff);
                binaryWriter.Write(obj.ColorLayout.YACCoeff);
                binaryWriter.Write(obj.ColorLayout.CbACCoeff);
                binaryWriter.Write(obj.ColorLayout.CrACCoeff);
            }

            if (selector.UseColorStructure)
            {
                binaryWriter.Write(obj.ColorStructure.Values);
            }

            if (selector.UseEdgeHistogram)
            {
                binaryWriter.Write(obj.EdgeHistogram.BinCounts);
            }

            if (selector.UseHomogeneousTexture)
            {
                binaryWriter.Write(obj.HomogeneousTexture.Average);
                binaryWriter.Write(obj.HomogeneousTexture.StandardDeviation);
                binaryWriter.Write(obj.HomogeneousTexture.Energy);
                binaryWriter.Write(obj.HomogeneousTexture.EnergyDeviation);
                binaryWriter.Write(obj.HomogeneousTexture.HasEnergyDeviation);
            }

            if (selector.UseScalableColor)
            {
                binaryWriter.Write(obj.ScalableColor.NumOfCoeff);
                binaryWriter.Write(obj.ScalableColor.NumOfBitplanesDiscarded);
                for (int i = 0; i < obj.ScalableColor.NumOfCoeff; ++i)
                {
                    binaryWriter.Write(obj.ScalableColor.Coeff[i]);
                }
            }
        }

        public CoPhIRData Read(Stream stream)
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            int id = binaryReader.ReadInt32();
            int farmId = binaryReader.ReadInt32();
            int serverId = binaryReader.ReadInt32();
            long secret = binaryReader.ReadInt64();

            ColorLayout colorLayout = null;

            if (selector.UseColorLayout)
            {
                byte yDCCoeff = binaryReader.ReadByte();
                byte cbDCCoeff = binaryReader.ReadByte();
                byte crDCCoeff = binaryReader.ReadByte();
                byte[] yACCoeff = binaryReader.ReadBytes(5);
                byte[] cbACCoeff = binaryReader.ReadBytes(2);
                byte[] crACCoeff = binaryReader.ReadBytes(2);

                colorLayout = new ColorLayout(yDCCoeff, cbDCCoeff, crDCCoeff, yACCoeff, cbACCoeff, crACCoeff);
            }

            ColorStructure colorStructure = null;

            if (selector.UseColorStructure)
            {
                byte[] values = binaryReader.ReadBytes(64);

                colorStructure = new ColorStructure(values);
            }

            EdgeHistogram edgeHistogram = null;
            if (selector.UseEdgeHistogram)
            {
                byte[] binCounts = binaryReader.ReadBytes(80);

                edgeHistogram = new EdgeHistogram(binCounts);
            }

            HomogeneousTexture homogeneousTexture = null;
            if (selector.UseHomogeneousTexture)
            {
                byte average = binaryReader.ReadByte();
                byte standardDeviation = binaryReader.ReadByte();
                byte[] energy = binaryReader.ReadBytes(30);
                byte[] energyDeviation = binaryReader.ReadBytes(30);
                bool hasEnergyDeviation = binaryReader.ReadBoolean();

                homogeneousTexture = new HomogeneousTexture(average, standardDeviation, energy, energyDeviation, hasEnergyDeviation);
            }

            ScalableColor scalableColor = null;
            if (selector.UseScalableColor)
            {
                int numOfCoeff = binaryReader.ReadInt32();
                int numOfBitplanesDiscarded = binaryReader.ReadInt32();

                short[] coeff = new short[numOfCoeff];
                for (int i = 0; i < numOfCoeff; ++i)
                {
                    coeff[i] = binaryReader.ReadInt16();
                }

                scalableColor = new ScalableColor(numOfCoeff, numOfBitplanesDiscarded, coeff);
            }

            CoPhIRData data = new CoPhIRData(id,farmId,serverId,secret, colorLayout, colorStructure, edgeHistogram, homogeneousTexture, scalableColor);

            return data;
        }

        public CoPhIRData Read(byte [] buffer, ref long position)
        {
            int id = BitConverter.ToInt32(buffer, (int) position);
            position += sizeof(Int32);
            int farmId = BitConverter.ToInt32(buffer, (int)position);
            position += sizeof(Int32);
            int serverId = BitConverter.ToInt32(buffer, (int)position);
            position += sizeof(Int32);
            long secret = BitConverter.ToInt64(buffer, (int)position);
            position += sizeof(Int64);

            ColorLayout colorLayout = null;

            if (selector.UseColorLayout)
            {
                byte yDCCoeff = buffer[position++];
                byte cbDCCoeff = buffer[position++];
                byte crDCCoeff = buffer[position++];
                byte[] yACCoeff = new byte[5];
                Array.Copy(buffer, position, yACCoeff, 0, 5);
                position += 5;
                byte[] cbACCoeff = new byte[2];
                Array.Copy(buffer, position, cbACCoeff, 0, 2);
                position += 2;
                byte[] crACCoeff = new byte[2];
                Array.Copy(buffer, position, crACCoeff, 0, 2);
                position += 2;

                colorLayout = new ColorLayout(yDCCoeff, cbDCCoeff, crDCCoeff, yACCoeff, cbACCoeff, crACCoeff);
            }

            ColorStructure colorStructure = null;

            if (selector.UseColorStructure)
            {
                byte[] values = new byte[64];
                Array.Copy(buffer, position, values, 0, 64);
                position += 64;

                colorStructure = new ColorStructure(values);
            }

            EdgeHistogram edgeHistogram = null;
            if (selector.UseEdgeHistogram)
            {
                byte[] binCounts = new byte[80];
                Array.Copy(buffer, position, binCounts, 0, 80);
                position += 80;

                edgeHistogram = new EdgeHistogram(binCounts);
            }

            HomogeneousTexture homogeneousTexture = null;
            if (selector.UseHomogeneousTexture)
            {
                byte average = buffer[position++];
                byte standardDeviation = buffer[position++];
                byte[] energy = new byte[30];
                Array.Copy(buffer, position, energy, 0, 30);
                position += 30;
                byte[] energyDeviation = new byte[30];
                Array.Copy(buffer, position, energyDeviation, 0, 30);
                position += 30;
                bool hasEnergyDeviation = BitConverter.ToBoolean(buffer, (int)position);
                position += sizeof(bool);

                homogeneousTexture = new HomogeneousTexture(average, standardDeviation, energy, energyDeviation, hasEnergyDeviation);
            }

            ScalableColor scalableColor = null;
            if (selector.UseScalableColor)
            {
                int numOfCoeff = BitConverter.ToInt32(buffer, (int)position);
                position += sizeof(Int32);
                int numOfBitplanesDiscarded = BitConverter.ToInt32(buffer, (int)position);
                position += sizeof(Int32);

                short[] coeff = new short[numOfCoeff];
                for (int i = 0; i < numOfCoeff; ++i)
                {
                    coeff[i] = BitConverter.ToInt16(buffer, (int)position);
                    position += sizeof(Int16);
                }
                scalableColor = new ScalableColor(numOfCoeff, numOfBitplanesDiscarded, coeff);
            }

            CoPhIRData data = new CoPhIRData(id, farmId, serverId, secret, colorLayout, colorStructure, edgeHistogram, homogeneousTexture, scalableColor);

            return data;
        }
    }
}
