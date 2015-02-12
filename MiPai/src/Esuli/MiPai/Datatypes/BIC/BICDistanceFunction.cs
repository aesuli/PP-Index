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

namespace Esuli.MiPai.Datatypes.BIC
{
    using System;
    using Esuli.MiPai.DistanceFunctions;

    public class BICDistanceFunction : IDistanceFunction<BICDescriptor>
    {
        public double Distance(BICDescriptor obj1,BICDescriptor obj2)
        {
            int distance = 0;
            var data1 = obj1.Data;
            var data2 = obj2.Data;
            for (int i = 0; i < BICDescriptor.DataSize; ++i)
            {
                byte val1 = data1[i];
                byte val2 = data2[i];
                distance += Math.Abs((val1&0xf)-(val2&0xf))+Math.Abs((val1>>4)-(val2>>4));
            }
            return distance;
        }
    }
}
