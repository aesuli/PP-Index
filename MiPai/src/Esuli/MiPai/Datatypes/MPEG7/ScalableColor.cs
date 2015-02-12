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
    public class ScalableColor
                 : IEquatable<ScalableColor>,
        IDistance<ScalableColor>
    {
        private int numOfCoeff;
        private int numOfBitplanesDiscarded;
        private short[] coeff;

        public ScalableColor(XmlReader reader)
        {
            numOfCoeff = int.Parse(reader["numOfCoeff"]);
            numOfBitplanesDiscarded = int.Parse(reader["numOfBitplanesDiscarded"]);
            coeff = null;
            bool go = true;
            while (go)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "Coeff")
                        {
                            coeff = XmlUtils.ReadShortArray(reader.ReadElementContentAsString());
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
            if (coeff == null)
            {
                throw new Exception("Not all the values have been specified");
            }
        }
        
        public ScalableColor(int numOfCoeff,int numOfBitplanesDiscarded,short[] coeff)
        {
            this.numOfCoeff = numOfCoeff;
            this.numOfBitplanesDiscarded = numOfBitplanesDiscarded;
            this.coeff = coeff;
        }

        public int NumOfCoeff
        {
            get
            {
                return numOfCoeff;
            }
        }

        public int NumOfBitplanesDiscarded
        {
            get
            {
                return numOfBitplanesDiscarded;
            }
        }

        public short[] Coeff
        {
            get
            {
                return coeff;
            }
        }
        
        public double Distance(ScalableColor item)
        {
            int sum = 0;

            for (int i = 0; i < coeff.Length; ++i)
            {
                sum += Math.Abs(coeff[i] - item.coeff[i]);
            }

            return sum;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder("ScalableColor");
            str.Append("\n  numOfCoeff:");
            str.Append(numOfCoeff);
            str.Append("\n  numOfBitplanesDiscarded:");
            str.Append(numOfBitplanesDiscarded);
            str.Append("\n  Coeff:");
            for (int i = 0; i < coeff.Length; ++i)
            {
                str.Append(" ");
                str.Append(coeff[i]);
            }
            str.Append("\n");
            return str.ToString();
            ;
        }

        public bool Equals(ScalableColor other)
        {
            for (int i = 0; i < coeff.Length; ++i)
            {
                if (coeff[i] != other.coeff[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(Object obj)
        {
            return ((IEquatable<ScalableColor>)this).Equals(obj as ScalableColor);
        }

        public override int GetHashCode()
        {
            int tempHashCode = 0;

            for (int i = 0; i < coeff.Length; ++i)
            {
                tempHashCode = 31 * tempHashCode + coeff[i];
            }

            return tempHashCode;
        }

    }
}
