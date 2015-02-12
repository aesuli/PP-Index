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
    using Esuli.MiPai.Datatypes.MPEG7;
    using Esuli.MiPai.DistanceFunctions;

    public class CoPhIRDataDistanceFunction<Tselector>
        : IDistanceFunction<CoPhIRData>
        where Tselector : ISelector, new()
    {
        public static readonly double SCW = 2.5 * 1.0 / 3000.0;
        public static readonly double CLW = 1.5 * 1.0 / 300.0;
        public static readonly double CSW = 2.5 * 1.0 / 40.0;
        public static readonly double EHW = 4.5 * 1.0 / 68.0;
        public static readonly double HTW = 0.5 * 1.0 / 25.0;

        private static readonly Tselector selector;

        static CoPhIRDataDistanceFunction()
        {
            selector = new Tselector();
        }

        public CoPhIRDataDistanceFunction()
        {
        }

        public Tselector Selector
        {
            get
            {
                return selector;
            }
        }

        public double Distance(CoPhIRData o1, CoPhIRData o2)
        {
            double distance = 0.0;
            if (selector.UseColorLayout && o1.ColorLayout != null && o2.ColorLayout != null && selector.ColorLayoutWeight != 0.0)
            {
                distance += CLW * o1.ColorLayout.Distance(o2.ColorLayout) * selector.ColorLayoutWeight;
            }
            if (selector.UseColorStructure && o1.ColorStructure != null && o2.ColorStructure != null && selector.ColorStructureWeight != 0.0)
            {
                distance += CSW * o1.ColorStructure.Distance(o2.ColorStructure) * selector.ColorStructureWeight;
            }
            if (selector.UseEdgeHistogram && o1.EdgeHistogram != null && o2.EdgeHistogram != null && selector.EdgeHistogramWeight != 0.0)
            {
                distance += EHW * o1.EdgeHistogram.Distance(o2.EdgeHistogram) * selector.EdgeHistogramWeight;
            }
            if (selector.UseHomogeneousTexture && o1.HomogeneousTexture != null && o2.HomogeneousTexture != null && selector.HomogeneousTextureWeight != 0.0)
            {
                distance += HTW * o1.HomogeneousTexture.Distance(o2.HomogeneousTexture) * selector.HomogeneousTextureWeight;
            }
            if (selector.UseScalableColor && o1.ScalableColor != null && o2.ScalableColor != null && selector.ScalableColorWeight != 0.0)
            {
                distance += SCW * o1.ScalableColor.Distance(o2.ScalableColor) * selector.ScalableColorWeight;
            }

            return distance;
        }
    }
}
