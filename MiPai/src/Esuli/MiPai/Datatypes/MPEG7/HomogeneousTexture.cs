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
    public class HomogeneousTexture
         : IEquatable<HomogeneousTexture>,
        IDistance<HomogeneousTexture>
    {
        //private static readonly int Nray = 128;
        //private static readonly int Nview = 180;
        //private static readonly int Npixel = 11520;
        public static readonly int NUMofFEATURE = 62;
        public static readonly int Quant_level = 255;
        public static readonly int RadialDivision = 5;
        public static readonly int AngularDivision = 6;

        private static readonly double[] wm = new double[] { 0.42, 1.00, 1.00, 0.08, 1.00 };
        private static readonly double[] wd = new double[] { 0.32, 1.00, 1.00, 1.00, 1.00 };

        private static readonly double wdc = 0.28;
        private static readonly double wstd = 0.22;

        private static readonly double dcmin = 0.0;
        private static readonly double dcmax = 255.0;
        private static readonly double stdmin = 1.309462;
        private static readonly double stdmax = 109.476530;

        private static readonly double[][] mmax = new double[][]		
	{new double []{18.392888,18.014313,18.002143,18.083845,18.046575,17.962099},
	 new double [] {19.368960,18.628248,18.682786,19.785603,18.714615,18.879544},
	 new double [] {20.816939,19.093605,20.837982,20.488190,20.763511,19.262577},
	 new double [] {22.298871,20.316787,20.659550,21.463502,20.159304,20.280403},
	 new double [] {21.516125,19.954733,20.381041,22.129800,20.184864,19.999331}};

        private static readonly double[][] mmin = new double[][]		
	{new double []{ 6.549734, 8.886816, 8.885367, 6.155831, 8.810013, 8.888925},
	 new double [] { 6.999376, 7.859269, 7.592031, 6.754764, 7.807377, 7.635503},
	 new double [] { 8.299334, 8.067422, 7.955684, 7.939576, 8.518458, 8.672599},
	 new double [] { 9.933642, 9.732479, 9.725933, 9.802238,10.076958,10.428015},
	 new double [] {11.704927,11.690975,11.896972,11.996963,11.977944,11.944282}};

        private static readonly double[][] dmax = new double[][]		
	{new double [] {21.099482,20.749788,20.786944,20.847705,20.772294,20.747129},
	 new double [] {22.658359,21.334119,21.283285,22.621111,21.773690,21.702166},
	 new double [] {24.317046,21.618960,24.396872,23.797967,24.329333,21.688523},
	 new double [] {25.638742,24.102725,22.687910,25.216958,22.334769,22.234942},
	 new double [] {24.692990,22.978804,23.891302,25.244315,24.281915,22.699811}};

        private static readonly double[][] dmin = new double[][]		
	{new double []{ 9.052970,11.754891,11.781252, 8.649997,11.674788,11.738701},
	 new double [] { 9.275178,10.386329,10.066189, 8.914539,10.292868,10.152977},
	 new double [] {10.368594,10.196313,10.211122,10.112823,10.648101,10.801070},
	 new double [] {11.737487,11.560674,11.551509,11.608201,11.897524,12.246614},
	 new double [] {13.303207,13.314553,13.450340,13.605001,13.547492,13.435994}};

        private static readonly double dcnorm = 122.331353;
        private static readonly double stdnorm = 51.314701;

        private static readonly double[][] mmean = new double[][]	
	{new double []{13.948462, 15.067986, 15.077915, 13.865536, 15.031283, 15.145633},
	 new double [] {15.557970, 15.172251, 15.357618, 15.166167, 15.414601, 15.414378},
	 new double [] {17.212408, 16.173027, 16.742651, 16.913837, 16.911480, 16.582123},
	 new double [] {17.911104, 16.761711, 17.065447, 17.867548, 17.250889, 17.050728},
	 new double [] {17.942741, 16.891190, 17.101770, 18.032434, 17.295305, 17.202160}};

        private static readonly double[][] dmean = new double[][]		
	{new double []{16.544933, 17.845844, 17.849176, 16.484509, 17.803377, 17.928810},
	 new double [] {18.054886, 17.617800, 17.862095, 17.627794, 17.935352, 17.887453},
	 new double [] {19.771456, 18.512341, 19.240444, 19.410559, 19.373478, 18.962496},
	 new double [] {20.192045, 18.763544, 19.202494, 20.098207, 19.399082, 19.032280},
	 new double [] {19.857040, 18.514065, 18.831860, 19.984838, 18.971045, 18.863575}};

        private byte average;
        private byte standardDeviation;
        private byte[] energy;
        private byte[] energyDeviation;
        private bool hasEnergyDeviation;
        private float[] features;

        public HomogeneousTexture(XmlReader reader)
        {
            int missingFields = 2;
            energy = null;
            energyDeviation = null;

            bool go = true;
            while (go)
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "Average":
                                average = (byte)reader.ReadElementContentAsInt();
                                --missingFields;
                                break;
                            case "StandardDeviation":
                                standardDeviation = (byte)reader.ReadElementContentAsInt();
                                --missingFields;
                                break;
                            case "Energy":
                                energy = XmlUtils.ReadByteArray(reader.ReadElementContentAsString());
                                break;
                            case "EnergyDeviation":
                                energyDeviation = XmlUtils.ReadByteArray(reader.ReadElementContentAsString());
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
                        go = reader.Read();
                        break;
                }
            }
            if (missingFields != 0 || energy == null)
            {
                throw new Exception("Not all the values have been specified");
            }
            if (energyDeviation == null)
            {
                hasEnergyDeviation = false;
                energyDeviation = new byte[energy.Length];
                for (int i = 0; i < energyDeviation.Length; ++i)
                {
                    energyDeviation[i] = 0;
                }
            }
            else
            {
                hasEnergyDeviation = true;
            }
            features = null;
        }

        public HomogeneousTexture(byte average, byte standardDeviation, byte[] energy, byte[] energyDeviation, bool hasEnergyDeviation)
        {
            this.average = average;
            this.standardDeviation = standardDeviation;
            this.energy = energy;
            this.energyDeviation = energyDeviation;
            if (energyDeviation == null)
            {
                this.energyDeviation = new byte[energy.Length];
                for (int i = 0; i < this.energyDeviation.Length; ++i)
                {
                    this.energyDeviation[i] = 0;
                }
            }
            this.hasEnergyDeviation = hasEnergyDeviation;
            features = null;
        }

        public HomogeneousTexture(float[] features, bool hasEnergyDeviation)
        {
            average = 0;
            standardDeviation = 0;
            energy = null;
            energyDeviation = null;
            this.hasEnergyDeviation = hasEnergyDeviation;
            this.features = features;
        }

        public byte Average
        {
            get
            {
                return average;
            }
        }

        public byte StandardDeviation
        {
            get
            {
                return standardDeviation;
            }
        }

        public byte[] Energy
        {
            get
            {
                return energy;
            }
        }

        public byte[] EnergyDeviation
        {
            get
            {
                return energyDeviation;
            }
        }

        public bool HasEnergyDeviation
        {
            get
            {
                return hasEnergyDeviation;
            }
        }

        public float[] Features
        {
            get
            {
                if (features == null)
                {
                    int[] RefFeature = new int[NUMofFEATURE];
                    RefFeature.Initialize();
                    RefFeature[0] = average;
                    RefFeature[1] = standardDeviation;

                    for (int i = 0; i < 30; ++i)
                    {
                        RefFeature[i + 2] = energy[i];

                        if (hasEnergyDeviation)
                        {
                            RefFeature[i + 32] = energyDeviation[i];
                        }
                    }

                    features = new float[NUMofFEATURE];
                    features.Initialize();

                    HT_dequantization(RefFeature, features);
                    HT_Normalization(features);
                }
                return features;
            }
        }
        public override int GetHashCode()
        {
            int tempHashCode = 0;

            for (int i = 0; i < energy.Length; ++i)
            {
                tempHashCode = 31 * tempHashCode + energy[i];
            }
            for (int i = 0; i < energyDeviation.Length; ++i)
            {
                tempHashCode = 31 * tempHashCode + energyDeviation[i];
            }

            return tempHashCode;
        }

        public double Distance(HomogeneousTexture item)
        {
            return Distance(this, item, 0);
        }

        private double Distance(HomogeneousTexture d1, HomogeneousTexture d2, int option)
        {
            double temp_distance, distance = 0;

            float[] fRefFeature = d1.Features;
            float[] fQueryFeature = d2.Features;

            bool flag = d1.hasEnergyDeviation && d2.hasEnergyDeviation;

            distance = (wdc * Math.Abs(fRefFeature[0] - fQueryFeature[0]));
            distance += (wstd * Math.Abs(fRefFeature[1] - fQueryFeature[1]));

            if (option == 0)
            {
                if (flag)
                {
                    for (int n = 0; n < RadialDivision; ++n)
                    {
                        for (int m = 0; m < AngularDivision; ++m)
                        {
                            distance += (wm[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 2] - fQueryFeature[n * AngularDivision + m + 2]))
                                       + (wd[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 32] - fQueryFeature[n * AngularDivision + m + 32]));
                        }
                    }
                }
                else
                {
                    for (int n = 0; n < RadialDivision; ++n)
                    {
                        for (int m = 0; m < AngularDivision; ++m)
                        {
                            distance += (wm[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 2] - fQueryFeature[n * AngularDivision + m + 2]));
                        }
                    }
                }
                return distance;
            }
            else if (option == 1) //r
            {
                double min = Double.MaxValue;
                if (flag)
                {
                    for (int i = AngularDivision; i > 0; --i)
                    {
                        temp_distance = 0.0;
                        for (int n = 0; n < RadialDivision; ++n)
                        {
                            for (int m = 0; m < AngularDivision; ++m)
                            {
                                if (m >= i)
                                {
                                    temp_distance += (wm[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 2 - i] - fQueryFeature[n * AngularDivision + m + 2]))
                                                    + (wd[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 30 + 2 - i] - fQueryFeature[n * AngularDivision + m + 30 + 2]));
                                }
                                else
                                {
                                    temp_distance += (wm[n] * Math.Abs(fRefFeature[(n + 1) * AngularDivision + m + 2 - i] - fQueryFeature[n * AngularDivision + m + 2]))
                                                    + (wd[n] * Math.Abs(fRefFeature[(n + 1) * AngularDivision + m + 30 + 2 - i] - fQueryFeature[n * AngularDivision + m + 30 + 2]));
                                }
                            }
                        }
                        if (temp_distance < min)
                        {
                            min = temp_distance;
                        }
                    }
                }
                else
                {
                    for (int i = AngularDivision; i > 0; --i)
                    {
                        temp_distance = 0.0;
                        for (int n = 0; n < RadialDivision; ++n)
                        {
                            for (int m = 0; m < AngularDivision; ++m)
                            {
                                if (m >= i)
                                {
                                    temp_distance += (wm[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 2 - i] - fQueryFeature[n * AngularDivision + m + 2]));
                                }
                                else
                                {
                                    temp_distance += (wm[n] * Math.Abs(fRefFeature[(n + 1) * AngularDivision + m + 2 - i] - fQueryFeature[n * AngularDivision + m + 2]));
                                }
                            }
                        }

                        if (temp_distance < min)
                        {
                            min = temp_distance;
                        }
                    }
                }

                distance += min;
                return distance;
            }
            else if (option == 2) //s
            {
                double min = Double.MaxValue;
                {
                    if (flag)
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            temp_distance = 0.0;
                            for (int n = 2; n < RadialDivision; ++n)
                            {
                                for (int m = 0; m < AngularDivision; ++m)
                                {
                                    temp_distance += (wm[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 2] - fQueryFeature[(n - i) * AngularDivision + m + 2]))
                                                    + (wd[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 30 + 2] - fQueryFeature[(n - i) * AngularDivision + m + 30 + 2]));
                                }
                            }

                            if (temp_distance < min)
                            {
                                min = temp_distance;
                            }
                        }

                        for (int i = 1; i < 3; ++i)
                        {
                            temp_distance = 0.0;
                            temp_distance = (wdc * Math.Abs(fRefFeature[0] - fQueryFeature[0]));
                            temp_distance += (wstd * Math.Abs(fRefFeature[1] - fQueryFeature[1]));
                            for (int n = 2; n < RadialDivision; ++n)
                            {
                                for (int m = 0; m < AngularDivision; ++m)
                                {
                                    temp_distance += (wm[n] * Math.Abs(fRefFeature[(n - i) * AngularDivision + m + 2] - fQueryFeature[n * AngularDivision + m + 2]))
                                                    + (wd[n] * Math.Abs(fRefFeature[(n - i) * AngularDivision + m + 30 + 2] - fQueryFeature[n * AngularDivision + m + 30 + 2]));
                                }
                            }

                            if (temp_distance < min)
                            {
                                min = temp_distance;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 3; ++i)
                        {
                            temp_distance = 0.0;
                            for (int n = 2; n < RadialDivision; ++n)
                            {
                                for (int m = 0; m < AngularDivision; ++m)
                                {
                                    temp_distance += (wm[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 2] - fQueryFeature[(n - i) * AngularDivision + m + 2]));
                                }
                            }
                            if (temp_distance < min)
                            {
                                min = temp_distance;
                            }
                        }

                        for (int i = 1; i < 3; ++i)
                        {
                            temp_distance = 0.0;
                            temp_distance = (wdc * Math.Abs(fRefFeature[0] - fQueryFeature[0]));
                            temp_distance += (wstd * Math.Abs(fRefFeature[1] - fQueryFeature[1]));
                            for (int n = 2; n < RadialDivision; ++n)
                            {
                                for (int m = 0; m < AngularDivision; ++m)
                                {
                                    temp_distance += (wm[n] * Math.Abs(fRefFeature[(n - i) * AngularDivision + m + 2] - fQueryFeature[n * AngularDivision + m + 2]));
                                }
                            }
                            if (temp_distance < min)
                            {
                                min = temp_distance;
                            }
                        }
                    }
                    distance += min;
                    return distance;
                }
            }
            else if (option == 3)//rs||sr
            {
                double min = Double.MaxValue;
                if (flag)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int i = AngularDivision; i > 0; --i)
                        {
                            temp_distance = 0.0;
                            for (int n = 2; n < RadialDivision; ++n)
                            {
                                for (int m = 0; m < AngularDivision; ++m)
                                {
                                    if (m >= i)
                                    {
                                        temp_distance += (wm[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 2 - i] - fQueryFeature[(n - j) * AngularDivision + m + 2]))
                                                        + (wd[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 30 + 2 - i] - fQueryFeature[(n - j) * AngularDivision + m + 30 + 2]));
                                    }
                                    else
                                    {
                                        temp_distance += (wm[n] * Math.Abs(fRefFeature[(n + 1) * AngularDivision + m + 2 - i] - fQueryFeature[(n - j) * AngularDivision + m + 2]))
                                                        + (wd[n] * Math.Abs(fRefFeature[(n + 1) * AngularDivision + m + 30 + 2 - i] - fQueryFeature[(n - j) * AngularDivision + m + 30 + 2]));
                                    }
                                }
                            }
                            if (temp_distance < min)
                            {
                                min = temp_distance;
                            }
                        }
                    }
                    for (int j = 1; j < 3; j++)
                    {
                        for (int i = AngularDivision; i > 0; --i)
                        {
                            temp_distance = 0.0;
                            for (int n = 2; n < RadialDivision; ++n)
                            {
                                for (int m = 0; m < AngularDivision; ++m)
                                {
                                    if (m >= i)
                                    {
                                        temp_distance += (wm[n] * Math.Abs(fRefFeature[(n - j) * AngularDivision + m + 2 - i] - fQueryFeature[n * AngularDivision + m + 2]))
                                                       + (wd[n] * Math.Abs(fRefFeature[(n - j) * AngularDivision + m + 30 + 2 - i] - fQueryFeature[n * AngularDivision + m + 30 + 2]));
                                    }
                                    else
                                    {
                                        temp_distance += (wm[n] * Math.Abs(fRefFeature[(n + 1 - j) * AngularDivision + m + 2 - i] - fQueryFeature[n * AngularDivision + m + 2]))
                                                       + (wd[n] * Math.Abs(fRefFeature[(n + 1 - j) * AngularDivision + m + 30 + 2 - i] - fQueryFeature[n * AngularDivision + m + 30 + 2]));
                                    }
                                }
                            }

                            if (temp_distance < min)
                            {
                                min = temp_distance;
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int i = AngularDivision; i > 0; --i)
                        {
                            temp_distance = 0.0;
                            for (int n = 2; n < RadialDivision; ++n)
                            {
                                for (int m = 0; m < AngularDivision; ++m)
                                {
                                    if (m >= i)
                                    {
                                        temp_distance += (wm[n] * Math.Abs(fRefFeature[n * AngularDivision + m + 2 - i] - fQueryFeature[(n - j) * AngularDivision + m + 2]));
                                    }
                                    else
                                    {
                                        temp_distance += (wm[n] * Math.Abs(fRefFeature[(n + 1) * AngularDivision + m + 2 - i] - fQueryFeature[(n - j) * AngularDivision + m + 2]));
                                    }
                                }
                            }
                            if (temp_distance < min)
                            {
                                min = temp_distance;
                            }
                        }
                    }
                    for (int j = 1; j < 3; j++)
                    {
                        for (int i = AngularDivision; i > 0; --i)
                        {
                            temp_distance = 0.0;
                            for (int n = 2; n < RadialDivision; ++n)
                            {
                                for (int m = 0; m < AngularDivision; ++m)
                                {
                                    if (m >= i)
                                    {
                                        temp_distance += (wm[n] * Math.Abs(fRefFeature[(n - j) * AngularDivision + m + 2 - i] - fQueryFeature[n * AngularDivision + m + 2]));
                                    }
                                    else
                                    {
                                        temp_distance += (wm[n] * Math.Abs(fRefFeature[(n + 1 - j) * AngularDivision + m + 2 - i] - fQueryFeature[n * AngularDivision + m + 2]));
                                    }
                                }
                            }

                            if (temp_distance < min)
                            {
                                min = temp_distance;
                            }
                        }
                    }
                }
                distance = min + distance;
                return distance;
            }
            else
            {
                throw new Exception("Unknown distance option " + option);
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder("HomogeneousTexture");
            str.Append("\n  Average: ");
            str.Append(average);
            str.Append("\n  StandardDeviation: ");
            str.Append(standardDeviation);
            str.Append("\n  Energy:");

            for (int i = 0; i < energy.Length; ++i)
            {
                str.Append(" ");
                str.Append(energy[i]);
            }
            str.Append("\n  EnergyDeviation:");
            for (int i = 0; i < energyDeviation.Length; ++i)
            {
                str.Append(" ");
                str.Append(energyDeviation[i]);
            }
            str.Append("\n");
            return str.ToString();
        }

        public bool Equals(HomogeneousTexture other)
        {
            if (average != other.average)
            {
                return false;
            }
            if (standardDeviation != other.standardDeviation)
            {
                return false;
            }

            for (int i = 0; i < energy.Length; ++i)
            {
                if (energy[i] != other.energy[i])
                {
                    return false;
                }
            }
            for (int i = 0; i < energyDeviation.Length; ++i)
            {
                if (energyDeviation[i] != other.energyDeviation[i])
                {
                    return false;
                }
            }

            return true;
        }

        private void HT_dequantization(int[] intFeature, float[] floatFeature)
        {
            double dcstep = (dcmax - dcmin) / Quant_level;
            floatFeature[0] = (float)(dcmin + intFeature[0] * dcstep);

            double stdstep = (stdmax - stdmin) / Quant_level;
            floatFeature[1] = (float)(stdmin + intFeature[1] * stdstep);

            double val;
            for (int n = 0; n < RadialDivision; ++n)
            {
                for (int m = 0; m < AngularDivision; ++m)
                {
                    val = (mmax[n][m] - mmin[n][m]) / Quant_level;
                    floatFeature[n * AngularDivision + m + 2] = (float)(mmin[n][m] + intFeature[n * AngularDivision + m + 2] * val);
                }
            }
            for (int n = 0; n < RadialDivision; ++n)
            {
                for (int m = 0; m < AngularDivision; ++m)
                {
                    val = (dmax[n][m] - dmin[n][m]) / Quant_level;
                    floatFeature[n * AngularDivision + m + 32] = (float)(dmin[n][m] + intFeature[n * AngularDivision + m + 32] * val);
                }
            }
        }

        private void HT_Normalization(float[] feature)
        {
            feature[0] /= (float)dcnorm;
            feature[1] /= (float)stdnorm;

            for (int n = 0; n < RadialDivision; ++n)
            {
                for (int m = 0; m < AngularDivision; ++m)
                {
                    feature[n * AngularDivision + m + 2] /= (float)mmean[n][m];
                }
            }
            for (int n = 0; n < RadialDivision; ++n)
            {
                for (int m = 0; m < AngularDivision; ++m)
                {
                    feature[n * AngularDivision + m + 32] /= (float)dmean[n][m];
                }
            }
        }

        public int GetFastDistanceValues(float[] dest, int offSet)
        {
            float[] pre1 = Features;

            int i = offSet;

            dest[i++] = (float)(wdc * pre1[0]);
            dest[i++] = (float)(wstd * pre1[1]);

            for (int n = 0; n < RadialDivision; n++)
            {
                for (int m = 0; m < AngularDivision; m++)
                {
                    dest[i++] = (float)(wm[n] * pre1[n * AngularDivision + m + 2]);
                    dest[i++] = (float)(wd[n] * pre1[n * AngularDivision + m + 32]);
                }
            }

            return i;
        }
    }
}