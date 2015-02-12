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
    public class ColorStructure
        : IEquatable<ColorStructure>,
        IDistance<ColorStructure>
    {
        private byte[] values;

        public ColorStructure(XmlReader reader)
        {
            values = null;
            bool go = true;
            while (go)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "Values")
                        {
                            values = XmlUtils.ReadByteArray(reader.ReadElementContentAsString());
                        }
                        else
                        {
                            go = reader.Read();
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
                        go = reader.Read();
                        break;
                }
            }
            if (values == null)
            {
                throw new Exception("Not all the values have been specified");
            }
        }

        public ColorStructure(byte[] values)
        {
            this.values = values;
        }

        public byte[] Values
        {
            get
            {
                return values;
            }
        }
        
        public override int GetHashCode()
        {
            int tempHashCode = 0;

            for (int i = 0; i < values.Length; ++i)
            {
                tempHashCode = 31 * tempHashCode + values[i];
            }

            return tempHashCode;
        }

        public double Distance(ColorStructure item)
        {
            int sum = 0;
            for (int i = 0; i < values.Length; ++i)
            {
                sum += Math.Abs(values[i] - item.values[i]);
            }
            return sum / 255.0;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder("ColorStructure");
            str.Append("  Values:");
            for (int i = 0; i < values.Length; i++)
            {
                str.Append(" ");
                str.Append(values[i]);
            }
            str.Append("\n");
            return str.ToString();
        }

        public bool Equals(ColorStructure other)
        {
            for (int i = 0; i < values.Length; ++i)
            {
                if (values[i] != other.values[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return ((ColorStructure)this).Equals(obj as ColorStructure);
        }
    }
}
