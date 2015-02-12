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

namespace Esuli.MiPai.Index
{
    using System.Collections.Generic;
    using Esuli.MiPai.DistanceFunctions;

    public interface IPermutationPrefixIndex<Tfeatures> : IPermutationPrefixIndex
    {

        KeyValuePair<double, KeyValuePair<int, int>>[] Search(Tfeatures query, int resultsCount, 
            int minCount, int maxCount, int permutations, PermutationType permutationType, int timeout);

        KeyValuePair<double, KeyValuePair<int, int>>[] Search(Tfeatures query, IDistanceFunction<Tfeatures> distanceFunction, 
            int resultsCount, int minCount, int maxCount, int permutations, PermutationType permutationType, int timeout);
    }
}

