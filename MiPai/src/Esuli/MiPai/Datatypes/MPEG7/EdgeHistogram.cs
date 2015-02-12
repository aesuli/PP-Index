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
    public class EdgeHistogram
        : IEquatable<EdgeHistogram>,
        IDistance<EdgeHistogram>
    {
        private static readonly double[][] quantTable = new double[][] { 
			  new double [] {0.010867,0.057915,0.099526,0.144849,0.195573,0.260504,0.358031,0.530128}, 
			  new double [] {0.012266,0.069934,0.125879,0.182307,0.243396,0.314563,0.411728,0.564319},
			  new double [] {0.004193,0.025852,0.046860,0.068519,0.093286,0.123490,0.161505,0.228960},
			  new double [] {0.004174,0.025924,0.046232,0.067163,0.089655,0.115391,0.151904,0.217745},
			 new double [] {0.006778,0.051667,0.108650,0.166257,0.224226,0.285691,0.356375,0.450972},
        };

        private byte[] binCounts;
        private double[] totalEdgeHist;

        public EdgeHistogram(XmlReader reader)
        {
            binCounts = null;
            bool go = true;
            while (go)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name == "BinCounts")
                        {
                            binCounts = XmlUtils.ReadByteArray(reader.ReadElementContentAsString());
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
            if (binCounts == null)
            {
                throw new Exception("Not all the values have been specified");
            }
            totalEdgeHist = null;
        }

        public EdgeHistogram(byte[] binCounts)
        {
            this.binCounts = binCounts;
            totalEdgeHist = null;
        }

        public EdgeHistogram(double[] totalEdgeHist)
        {
            binCounts = null;
            this.totalEdgeHist = totalEdgeHist;
        }
        
        public byte[] BinCounts
        {
            get
            {
                return binCounts;
            }
        }

        public double[] TotalEdgeHist
        {
            get
            {
                if (totalEdgeHist == null)
                {
                    double[] localEdgeHist = new double[80];

                    for (int i = 0; i < 80; ++i)
                    {
                        localEdgeHist[i] = quantTable[i % 5][binCounts[i]];
                    }

                    totalEdgeHist = new double[150];
                    EHD_Make_Global_SemiGlobal(localEdgeHist, totalEdgeHist);
                }
                return totalEdgeHist;
            }
        }

        public override int GetHashCode()
        {
            int tempHashCode = 0;

            for (int i = 0; i < binCounts.Length; ++i)
            {
                tempHashCode = 31 * tempHashCode + binCounts[i];
            }

            return tempHashCode;
        }

        public double Distance(EdgeHistogram item)
        {
            return Distance(this.TotalEdgeHist,item.TotalEdgeHist);
        }

        private double Distance(double[] totalEdgeHistRef,double[] totalEdgeHistQuery) {
            double dist = 0.0;
            for (int i = 0; i < 150; ++i)
            {
                dist += Math.Abs(totalEdgeHistRef[i] - totalEdgeHistQuery[i]);
            }

            return dist;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder("EdgeHistogram");
            str.Append("  BinCounts:");
            for (int i = 0; i < binCounts.Length; ++i)
            {
                str.Append(" ");
                str.Append(binCounts[i]);
            }
            str.Append("\n");
            return str.ToString();
        }

        public bool Equals(EdgeHistogram item)
        {
            for (int i = 0; i < binCounts.Length; ++i)
            {
                if (binCounts[i] != item.binCounts[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return ((IEquatable<EdgeHistogram>)this).Equals(obj);
        }

        private void EHD_Make_Global_SemiGlobal(double[] LocalHistogramOnly, double[] TotalHistogram)
        {
            for (int l = 0; l < 80; ++l)
            {
                TotalHistogram[l + 5] = LocalHistogramOnly[l];
            }

            for (int i = 0; i < 5; ++i)
            {
                TotalHistogram[i] = 0.0;
            }

            for (int j = 0; j < 80; j += 5)
            {
                for (int i = 0; i < 5; ++i)
                {
                    TotalHistogram[i] += TotalHistogram[5 + i + j];
                }
            }

            for (int i = 0; i < 5; ++i)
            {
                TotalHistogram[i] = TotalHistogram[i] * 5 / 16.0;
            }

            for (int i = 85,j=0; i < 105; ++i,++j)
            {
                TotalHistogram[i] =
                    (TotalHistogram[5 + j]
                    + TotalHistogram[25 + j]
                    + TotalHistogram[45 + j]
                    + TotalHistogram[65 + j]) / 4.0;
            }

            for (int i = 105,j=0; i < 125; ++i,++j)
            {
                TotalHistogram[i] =
                    (TotalHistogram[5 + 20 * (j / 5) + j % 5]
                    + TotalHistogram[5 + 20 * (j / 5) + j % 5 + 5]
                    + TotalHistogram[5 + 20 * (j / 5) + j % 5 + 10]
                + TotalHistogram[5 + 20 * (j / 5) + j % 5 + 15]) / 4.0;
            }

            for (int i = 125,j=0; i < 135; ++i,++j)
            {
                TotalHistogram[i] =
                    (TotalHistogram[5 + 10 * (j / 5) + 0 + j % 5]
                           + TotalHistogram[5 + 10 * (j / 5) + 5 + j % 5]
                           + TotalHistogram[5 + 10 * (j / 5) + 20 + j % 5]
                           + TotalHistogram[5 + 10 * (j / 5) + 25 + j % 5]) / 4.0;
            }

            for (int i = 135,j=0; i < 145; ++i,++j)
            {
                TotalHistogram[i] =
                    (TotalHistogram[5 + 10 * (j / 5) + 40 + j % 5]
                           + TotalHistogram[5 + 10 * (j / 5) + 45 + j % 5]
                           + TotalHistogram[5 + 10 * (j / 5) + 60 + j % 5]
                           + TotalHistogram[5 + 10 * (j / 5) + 65 + j % 5]) / 4.0;
            }

            for (int i = 145,j=0; i < 150; ++i,++j)
            {
                TotalHistogram[i] =
                    (TotalHistogram[30 + j % 5]
                           + TotalHistogram[35 + j % 5]
                           + TotalHistogram[50 + j % 5]
                           + TotalHistogram[55 + j % 5]) / 4.0;
            }
        }
    }
}