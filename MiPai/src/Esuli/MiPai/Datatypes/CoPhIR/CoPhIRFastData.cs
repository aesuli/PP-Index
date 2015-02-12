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
    using Esuli.Base;
    using Esuli.MiPai.Datatypes.MPEG7;

    [Serializable]
    public class CoPhIRFastData
      : IIntId, IComparable<CoPhIRFastData>
    {
        private static readonly int l2ValuesSize1 = 6;
        private static readonly int l2ValuesSize2 = 3;
        private static readonly int l2ValuesSize3 = 3;
        public static readonly int L2ValueLength = l2ValuesSize1 + l2ValuesSize2 + l2ValuesSize3;

        private static readonly int l1ValuesN_EH = 150;
        private static readonly int l1ValuesN_CS = 64;
        private static readonly int l1ValuesN_HT = 2 + HomogeneousTexture.RadialDivision * HomogeneousTexture.AngularDivision * 2;
        private static readonly int l1ValuesN_SC = 64;
        private static readonly int l1ValuesN_CL = l2ValuesSize1 + l2ValuesSize2 + l2ValuesSize3;
        public static readonly int ValuesLength = l1ValuesN_EH + l1ValuesN_CS + l1ValuesN_HT + l1ValuesN_SC + l1ValuesN_CL;

        private int id;
        private float[] values;

        public CoPhIRFastData(CoPhIRData orig)
        {
            values = new float[ValuesLength];
            id = orig.Id;

            int offset = 0;

            // EH
            double[] ehValues = orig.EdgeHistogram.TotalEdgeHist;
            for (int i = 0; i < 150; i++)
            {
                values[offset++] = (float)(CoPhIRDataDistanceFunction<SelectAll>.EHW * ehValues[i]);
            }

            // CS
            byte[] csBytes = orig.ColorStructure.Values;
            for (int i = 0; i < 64; i++)
            {
                values[offset++] = (float)(CoPhIRDataDistanceFunction<SelectAll>.CSW * csBytes[i] / 255.0);
            }

            // SC
            short[] scCoeff = orig.ScalableColor.Coeff;
            for (int i = 0; i < 64; i++)
            {
                values[offset++] = (float)(CoPhIRDataDistanceFunction<SelectAll>.SCW * scCoeff[i]);
            }

            // HT
            int newOffset = orig.HomogeneousTexture.GetFastDistanceValues(values, offset);
            for (int i = offset; i < newOffset; ++i)
            {
                values[i] *= (float)CoPhIRDataDistanceFunction<SelectAll>.HTW;
            }
            offset = newOffset;

            //CL
            newOffset = orig.ColorLayout.GetFastDistanceValues(values, offset);
            for (int i = offset; i < newOffset; ++i)
            {
                values[i] *= (float)CoPhIRDataDistanceFunction<SelectAll>.CLW;
            }
            offset = newOffset;
        }

        public CoPhIRFastData(int id, float[] values)
        {
            this.id = id;
            this.values = values;
        }

        public CoPhIRFastData(int id, CoPhIRData original) :
            this(original)
        {
            this.id = id;
        }

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public float[] Values
        {
            get
            {
                return values;
            }
            set
            {
                values = value;
            }
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public int CompareTo(CoPhIRFastData other)
        {
            return id - other.id;
        }
    }
}