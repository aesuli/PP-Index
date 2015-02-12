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

namespace Esuli.MiPai.Datatypes.Corel
{
    using System;
    using Esuli.Base;

    public class CorelData : IComparable<CorelData>, IIntId
    {
        private int id;
        private double[] features;

        public CorelData(int id, double[] features)
        {
            this.id = id;
            this.features = features;
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public double[] Features
        {
            get
            {
                return features;
            }
        }

        public int CompareTo(CorelData other)
        {
            return id - other.id;
        }
    }
}
