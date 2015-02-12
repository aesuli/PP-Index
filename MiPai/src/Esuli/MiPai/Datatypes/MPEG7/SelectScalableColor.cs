﻿// Copyright (C) 2013 Andrea Esuli
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
    public class SelectScalableColor : ISelector
    {
        public SelectScalableColor()
        {
        }

        public bool UseColorLayout
        {
            get
            {
                return false;
            }
        }

        public bool UseColorStructure
        {
            get
            {
                return false;
            }
        }

        public bool UseEdgeHistogram
        {
            get
            {
                return false;
            }
        }

        public bool UseHomogeneousTexture
        {
            get
            {
                return false;
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
                return 1.0;
            }
        }

        public double ColorStructureWeight
        {
            get
            {
                return 1.0;
            }
        }

        public double EdgeHistogramWeight
        {
            get
            {
                return 1.0;
            }
        }

        public double HomogeneousTextureWeight
        {
            get
            {
                return 1.0;
            }
        }

        public double ScalableColorWeight
        {
            get
            {
                return 1.0;
            }
        }
    }
}
