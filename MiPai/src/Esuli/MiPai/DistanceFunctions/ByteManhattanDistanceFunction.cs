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

namespace Esuli.MiPai.DistanceFunctions
{
    using System;

    public class ByteManhattanDistanceFunction : IDistanceFunction<byte []>
    {
        public double Distance(byte[] o1, byte[] o2)
        {
            double dist = 0.0;
            for (int i = 0; i < o1.Length; ++i)
            {
                dist += Math.Abs(o1[i] - o2[i]);
            }
            return dist/(byte.MaxValue*o1.Length);
        }
    }
}
