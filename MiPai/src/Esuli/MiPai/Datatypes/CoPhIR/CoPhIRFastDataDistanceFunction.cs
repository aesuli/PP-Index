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
    using Esuli.MiPai.DistanceFunctions;

    public class CoPhIRFastDataDistanceFunction
        : IDistanceFunction<CoPhIRFastData>
    {
        private static readonly int clStart = CoPhIRFastData.ValuesLength - CoPhIRFastData.L2ValueLength;

        public CoPhIRFastDataDistanceFunction()
        {
        }

        public double Distance(CoPhIRFastData o1, CoPhIRFastData o2)
        {
            double dist = 0;

            float[] v1 = o1.Values;
            float[] v2 = o2.Values;

            int i;
            for (i = 0; i < clStart; ++i)
            {
                dist += Math.Abs(v1[i] - v2[i]);
            }

            double acc = 0;
            for (; i < clStart + 6; ++i)
            {
                double diff = v1[i] - v2[i];
                acc += diff * diff;
            }
            dist += Math.Sqrt(acc);

            acc = 0;
            for (; i < clStart + 6 + 3; ++i)
            {
                double diff = v1[i] - v2[i];
                acc += diff * diff;
            }
            dist += Math.Sqrt(acc);

            acc = 0;
            for (; i < clStart + 6 + 3 + 3; ++i)
            {
                double diff = v1[i] - v2[i];
                acc += diff * diff;
            }
            dist += Math.Sqrt(acc);

            return dist;
        }
    }
}
