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
    public class CustomizableSelectAll : ISelector
    {
        private double colorLayoutWeight;
        private double colorStructureWeight;
        private double edgeHistogramWeight;
        private double homogeneousTextureWeight;
        private double scalableColorWeight;

        public CustomizableSelectAll()
        {
            colorLayoutWeight = 1.0;
            colorStructureWeight = 1.0;
            edgeHistogramWeight = 1.0;
            homogeneousTextureWeight = 1.0;
            scalableColorWeight = 1.0;
        }

        public CustomizableSelectAll(double colorLayoutWeight, double colorStructureWeight, double edgeHistogramWeight,
            double homogeneousTextureWeight, double scalableColorWeight)
        {
            this.colorLayoutWeight = colorLayoutWeight;
            this.colorStructureWeight = colorStructureWeight;
            this.edgeHistogramWeight = edgeHistogramWeight;
            this.homogeneousTextureWeight = homogeneousTextureWeight;
            this.scalableColorWeight = scalableColorWeight;
        }

        public bool UseColorLayout
        {
            get
            {
                return true;
            }
        }

        public bool UseColorStructure
        {
            get
            {
                return true;
            }
        }

        public bool UseEdgeHistogram
        {
            get
            {
                return true;
            }
        }

        public bool UseHomogeneousTexture
        {
            get
            {
                return true;
            }
        }

        public bool UseScalableColor
        {
            get
            {
                return true;
            }
        }

        public double ColorLayoutWeight
        {
            get
            {
                return colorLayoutWeight;
            }
            set
            {
                colorLayoutWeight = value;
            }
        }

        public double ColorStructureWeight
        {
            get
            {
                return colorStructureWeight;
            }
            set
            {
                colorStructureWeight = value;
            }
        }

        public double EdgeHistogramWeight
        {
            get
            {
                return edgeHistogramWeight;
            }
            set
            {
                edgeHistogramWeight = value;
            }
        }

        public double HomogeneousTextureWeight
        {
            get
            {
                return homogeneousTextureWeight;
            }
            set
            {
                homogeneousTextureWeight = value;
            }
        }

        public double ScalableColorWeight
        {
            get
            {
                return scalableColorWeight;
            }
            set
            {
                scalableColorWeight = value;
            }
        }
    }
}
