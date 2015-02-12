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

namespace Esuli.MiPai.Datatypes.MPEG7
{
    using System;
    using System.Text;
    using System.Xml;
    using Esuli.MiPai.DistanceFunctions;

    [Serializable]
    public class ColorLayout
        : IEquatable<ColorLayout>,
        IDistance<ColorLayout>
    {
        private static readonly int[][] m_weight = new int[][] {
				new int [] { 3, 3, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
						1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
						1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
						1, 1, 1, 1, 1, 1, 1, 1 },
				new int [] { 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
						1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
						1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
						1, 1, 1, 1, 1, 1, 1, 1 },
				new int [] { 4, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
						1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
						1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
						1, 1, 1, 1, 1, 1, 1, 1 }
				
        };

        private static readonly double[][] m_weight_sqrt;

        static ColorLayout()
        {
            m_weight_sqrt = new double[3][];
            for (int i = 0; i < m_weight.Length; i++)
            {
                m_weight_sqrt[i] = new double[m_weight[i].Length];
                for (int j = 0; j < m_weight[i].Length; j++)
                {
                    m_weight_sqrt[i][j] = Math.Sqrt(m_weight[i][j]);
                }
            }
        }

        private byte yDCCoeff;
        private byte cbDCCoeff;
        private byte crDCCoeff;
        private byte[] yACCoeff;
        private byte[] cbACCoeff;
        private byte[] crACCoeff;

        public ColorLayout(XmlReader reader)
        {
            int missingFields = 6;
            bool go = true;
            while (go)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "YDCCoeff":
                                yDCCoeff = (byte)reader.ReadElementContentAsInt();
                                --missingFields;
                                break;
                            case "CbDCCoeff":
                                cbDCCoeff = (byte)reader.ReadElementContentAsInt();
                                --missingFields;
                                break;
                            case "CrDCCoeff":
                                crDCCoeff = (byte)reader.ReadElementContentAsInt();
                                --missingFields;
                                break;
                            case "YACCoeff2":
                            case "YACCoeff5":
                            case "YACCoeff9":
                            case "YACCoeff14":
                            case "YACCoeff20":
                            case "YACCoeff27":
                            case "YACCoeff63":
                                yACCoeff = XmlUtils.ReadByteArray(reader.ReadElementContentAsString());
                                --missingFields;
                                break;
                            case "CbACCoeff2":
                            case "CbACCoeff5":
                            case "CbACCoeff9":
                            case "CbACCoeff14":
                            case "CbACCoeff20":
                            case "CbACCoeff27":
                            case "CbACCoeff63":
                                cbACCoeff = XmlUtils.ReadByteArray(reader.ReadElementContentAsString());
                                --missingFields;
                                break;
                            case "CrACCoeff2":
                            case "CrACCoeff5":
                            case "CrACCoeff9":
                            case "CrACCoeff14":
                            case "CrACCoeff20":
                            case "CrACCoeff27":
                            case "CrACCoeff63":
                                crACCoeff = XmlUtils.ReadByteArray(reader.ReadElementContentAsString());
                                --missingFields;
                                break;
                            default:
                                go = reader.Read();
                                break;
                        }
                        break;
                    case (XmlNodeType.EndElement):
                        if (reader.Name == "VisualDescriptor")
                        {
                            go = false;
                        }
                        else
                        {
                            go = reader.Read();
                        }
                        break;
                    default:
                        reader.Read();
                        break;
                }
            }
            if (missingFields != 0)
            {
                throw new Exception("Not all the values have been specified");
            }
        }

        public ColorLayout(byte yDCCoeff, byte cbDCCoeff, byte crDCCoeff, byte[] yACCoeff, byte[] cbACCoeff, byte[] crACCoeff)
        {
            this.yDCCoeff = yDCCoeff;
            this.cbDCCoeff = cbDCCoeff;
            this.crDCCoeff = crDCCoeff;
            this.yACCoeff = yACCoeff;
            this.cbACCoeff = cbACCoeff;
            this.crACCoeff = crACCoeff;
        }

        public byte YDCCoeff
        {
            get
            {
                return yDCCoeff;
            }
        }

        public byte CbDCCoeff
        {
            get
            {
                return cbDCCoeff;
            }
        }

        public byte CrDCCoeff
        {
            get
            {
                return crDCCoeff;
            }
        }

        public byte[] YACCoeff
        {
            get
            {
                return yACCoeff;
            }
        }

        public byte[] CbACCoeff
        {
            get
            {
                return cbACCoeff;
            }
        }

        public byte[] CrACCoeff
        {
            get
            {
                return crACCoeff;
            }
        }

        public override int GetHashCode()
        {
            int tempHashCode = 0;
            tempHashCode = 31 * tempHashCode + yDCCoeff;
            tempHashCode = 31 * tempHashCode + cbDCCoeff;
            tempHashCode = 31 * tempHashCode + crDCCoeff;

            for (int i = 0; i < yACCoeff.Length; ++i)
            {
                tempHashCode = 31 * tempHashCode + yACCoeff[i];
            }
            for (int i = 0; i < cbACCoeff.Length; ++i)
            {
                tempHashCode = 31 * tempHashCode + cbACCoeff[i];
            }
            for (int i = 0; i < crACCoeff.Length; ++i)
            {
                tempHashCode = 31 * tempHashCode + crACCoeff[i];
            }

            return tempHashCode;
        }

        public double Distance(ColorLayout item)
        {
            int NY = Math.Min(yACCoeff.Length, item.yACCoeff.Length);
            int NC = Math.Min(cbACCoeff.Length, item.cbACCoeff.Length);

            int diff = yDCCoeff - item.yDCCoeff;
            int[] sum = new int[3];
            sum[0] = m_weight[0][0] * diff * diff;
            for (int i = 0; i < NY; ++i)
            {
                diff = (yACCoeff[i] - item.yACCoeff[i]);
                sum[0] += (m_weight[0][i + 1] * diff * diff);
            }

            diff = (cbDCCoeff - item.cbDCCoeff);
            sum[1] = m_weight[1][0] * diff * diff;
            for (int i = 0; i < NC; ++i)
            {
                diff = (cbACCoeff[i] - item.cbACCoeff[i]);
                sum[1] += (m_weight[1][i + 1] * diff * diff);
            }

            diff = (crDCCoeff - item.crDCCoeff);
            sum[2] = m_weight[2][0] * diff * diff;
            for (int i = 0; i < NC; ++i)
            {
                diff = (crACCoeff[i] - item.crACCoeff[i]);
                sum[2] += (m_weight[2][i + 1] * diff * diff);
            }

            return Math.Sqrt(sum[0]) + Math.Sqrt(sum[1])
                    + Math.Sqrt(sum[2]);
        }

        public override string ToString()
        {

            StringBuilder str = new StringBuilder("ColorLayout");

            str.Append("\n  YDCCoeff: ");
            str.Append(yDCCoeff);
            str.Append("\n  CbDCCoeff: ");
            str.Append(cbDCCoeff);
            str.Append("\n  CrDCCoeff: ");
            str.Append(crDCCoeff);

            str.Append("\n  YACCoeff:");
            for (int i = 0; i < yACCoeff.Length; ++i)
            {
                str.Append(" ");
                str.Append(yACCoeff[i]);
            }

            str.Append("\n  CbACCoeff:");
            for (int i = 0; i < cbACCoeff.Length; ++i)
            {
                str.Append(" ");
                str.Append(cbACCoeff[i]);
            }

            str.Append("\n  CrACCoeff:");
            for (int i = 0; i < crACCoeff.Length; ++i)
            {
                str.Append(" ");
                str.Append(crACCoeff[i]);
            }

            str.Append("\n");

            return str.ToString();
        }

        public bool Equals(ColorLayout other)
        {
            if (yDCCoeff != other.yDCCoeff)
            {
                return false;
            }
            if (cbDCCoeff != other.cbDCCoeff)
            {
                return false;
            }
            if (crDCCoeff != other.crDCCoeff)
            {
                return false;
            }
            for (int i = 0; i < yACCoeff.Length; ++i)
            {
                if (yACCoeff[i] != other.yACCoeff[i])
                    return false;
            }
            for (int i = 0; i < cbACCoeff.Length; ++i)
            {
                if (cbACCoeff[i] != other.cbACCoeff[i])
                    return false;
            }
            for (int i = 0; i < crACCoeff.Length; ++i)
            {
                if (crACCoeff[i] != other.crACCoeff[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return ((IEquatable<ColorLayout>)this).Equals(obj as ColorLayout);
        }


        public int GetFastDistanceValues(float[] values, int offset)
        {
            int NY = yACCoeff.Length;
            int NC = cbACCoeff.Length;

            int i = offset;

            values[i++] = (float)m_weight_sqrt[0][0] * yDCCoeff;
            for (int j = 0; j < NY; j++)
            {
                values[i++] = (float)m_weight_sqrt[0][j + 1] * yACCoeff[j];
            }

            values[i++] = (float)m_weight_sqrt[1][0] * cbDCCoeff;
            for (int j = 0; j < NC; j++)
            {
                values[i++] = (float)m_weight_sqrt[1][j + 1] * cbACCoeff[j];
            }

            values[i++] = (float)m_weight_sqrt[2][0] * crDCCoeff;
            for (int j = 0; j < NC; j++)
            {
                values[i++] = (float)m_weight_sqrt[2][j + 1] * crACCoeff[j];
            }
            return i;
        }
    }
}