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
    using System;
    using System.Collections.Generic;
    using Esuli.MiPai.DistanceFunctions;

    public class MultiplePermutationPrefixIndex<Tfeatures>
        : IPermutationPrefixIndex<Tfeatures>
    {
        private IPermutationPrefixIndex<Tfeatures> [] indexes;
        private int[] offsets;
        private int count;

        public MultiplePermutationPrefixIndex(IPermutationPrefixIndex<Tfeatures>[] indexes,int [] offsets)
        {
            this.indexes = indexes;
            this.offsets = offsets;
            count = 0;
            foreach (var index in indexes)
            {
                count += index.Count;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public KeyValuePair<double, KeyValuePair<int, int>>[] Search(Tfeatures query, int resultsCount, int minCount, int maxCount, int permutations, PermutationType permutationType, int timeout)
        {
            KeyValuePair<double, KeyValuePair<int, int>>[][] results = new KeyValuePair<double,KeyValuePair<int,int>>[indexes.Length][];
            DateTime end = DateTime.Now.AddSeconds(timeout);
            for (int i = 0; i < indexes.Length; ++i)
            {
                results[i] = indexes[i].Search(query, resultsCount, minCount, maxCount, permutations, permutationType, (int)((end - DateTime.Now).TotalSeconds + 1));
                if (DateTime.Now > end)
                {
                    break;
                }
            }

            return Merge(results,resultsCount);
        }

        public KeyValuePair<double, KeyValuePair<int, int>>[] Search(Tfeatures query, IDistanceFunction<Tfeatures> distanceFunction, int resultsCount, int minCount, int maxCount, int permutations, PermutationType permutationType, int timeout)
        {
            KeyValuePair<double, KeyValuePair<int, int>>[][] results = new KeyValuePair<double,KeyValuePair<int,int>>[indexes.Length][];
            DateTime end = DateTime.Now.AddSeconds(timeout);
            for (int i = 0; i < indexes.Length; ++i)
            {
                results[i] = indexes[i].Search(query, distanceFunction, resultsCount, minCount, maxCount, permutations, permutationType,(int)((end-DateTime.Now).TotalSeconds+1));
                if (DateTime.Now > end)
                {
                    break;
                }
            }
            return Merge(results, resultsCount);
        }

        private KeyValuePair<double, KeyValuePair<int, int>>[] Merge(KeyValuePair<double, KeyValuePair<int, int>>[][] results, int resultsCount)
        {
            List<KeyValuePair<double, KeyValuePair<int, int>>> result = new List<KeyValuePair<double, KeyValuePair<int, int>>>(resultsCount);
            HashSet<int> ids = new HashSet<int>();

            int setSize = 0;
            while (setSize < results.Length && results[setSize] != null)
            {
                ++setSize;
            }

            int[] positions = new int[setSize];
            for (int i = 0; i < setSize; ++i)
            {
                positions[i] = 0;
            }

            while(result.Count< resultsCount)
            {
                int minIndex = 0;
                while (minIndex < positions.Length && positions[minIndex] == -1)
                {
                    ++minIndex;
                }

                if (minIndex == positions.Length)
                {
                    return result.ToArray();
                }

                double minValue = results[minIndex][positions[minIndex]].Key;
                for (int j = minIndex + 1; j < setSize; ++j)
                {
                    if (positions[j] >= 0)
                    {
                        if (results[j][positions[j]].Key < minValue)
                        {
                            minIndex = j;
                            minValue = results[j][positions[j]].Key;
                        }
                    }
                }

                int id = results[minIndex][positions[minIndex]].Value.Value;

                if (!ids.Contains(id))
                {
                    ids.Add(id);
                    KeyValuePair<double, KeyValuePair<int, int>> entry = results[minIndex][positions[minIndex]];
                    result.Add(new KeyValuePair<double, KeyValuePair<int, int>>(entry.Key,new KeyValuePair<int,int>(entry.Value.Key+offsets[minIndex],id)));
                }

                ++positions[minIndex];
                if (positions[minIndex] == results[minIndex].Length)
                {
                    positions[minIndex] = -1;
                }
            }

            return result.ToArray();
        }
    }
}
