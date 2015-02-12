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
    using Esuli.Base;
    using Esuli.Base.IO;
    using Esuli.Base.IO.Storage;
    using Esuli.MiPai.DistanceFunctions;
    using Esuli.MiPai.Indexing;
    using Esuli.MiPai.IO;
    using Esuli.Scheggia.Core;
    using Esuli.Scheggia.Enumerators;
    
    public class PermutationPrefixIndex<Tfeatures> 
        : IPermutationPrefixIndex<Tfeatures>, IDisposable
        where Tfeatures : IIntId,IComparable<Tfeatures>
    {
        private ReadOnlyFixedSizeStorage<Tfeatures> featuresStorageReader;

        private IDistanceFunction<Tfeatures> distanceFunction;

        private int lastAccessed;
        private int lastSequences;

        private PermutationPrefixTree<Tfeatures> tree;

        public PermutationPrefixIndex(string name, string location, int pruneLevel)
        {
            tree = PermutationPrefixTreeSerialization<Tfeatures>.Read(name, location, pruneLevel);
            distanceFunction = tree.PermutationPrefixGenerator.DistanceFunction;
            featuresStorageReader = new ReadOnlyFixedSizeStorage<Tfeatures>(name, location);
        }

        public int Count
        {
            get
            {
                return featuresStorageReader.Count;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
               tree = null;
               featuresStorageReader.Dispose();
            }
        }
        
        public PermutationPrefixTree<Tfeatures> PrefixTree
        {
            get
            {
                return tree;
            }
        }
        
        public int Depth
        {
            get
            {
                return tree.Depth;
            }
        }
        
        public PermutationPrefixGenerator<Tfeatures> PermutationPrefixGenerator
        {
            get
            {
                return tree.PermutationPrefixGenerator;
            }
        }

        public int LastAccessed
        {
            get
            {
                return lastAccessed;
            }
        }

        public int LastSequences
        {
            get
            {
                return lastSequences;
            }
        }

        public KeyValuePair<double, KeyValuePair<int, int>>[] Search(Tfeatures query, int resultsCount, int minCount,
            int maxCount, int permutations, PermutationType permutationType,int timeout)
        {
            return Search(query, distanceFunction, resultsCount, minCount, maxCount, permutations, permutationType,timeout);
        }

        public KeyValuePair<double, KeyValuePair<int, int>>[] Search(Tfeatures query, 
            IDistanceFunction<Tfeatures> distanceFunction, int resultsCount, int minCount, int maxCount, 
            int permutations, PermutationType permutationType,int timeout)
        {
            List<KeyValuePair<double, KeyValuePair<int, int>>> results = new List<KeyValuePair<double, KeyValuePair<int, int>>>();
            using (IPostingEnumerator<Tfeatures> candidates = GetCandidateEnumerator(query, distanceFunction, minCount, maxCount, permutations, permutationType))
            {
                lastAccessed = candidates.Count;

                using (ScoreSortedPostingEnumerator<Tfeatures> resultEnumerator = new ScoreSortedPostingEnumerator<Tfeatures>(candidates, resultsCount, false))
                {

                    while (resultEnumerator.MoveNext())
                    {
                        using (var specializedEnumerator = resultEnumerator.GetSpecializedCurrentHitEnumerator())
                        {
                            specializedEnumerator.MoveNext();
                            Tfeatures result = specializedEnumerator.Current;
                            results.Add(new KeyValuePair<double, KeyValuePair<int, int>>(resultEnumerator.ScoreFunction(), new KeyValuePair<int, int>(resultEnumerator.CurrentPostingId, result.Id)));
                        }
                    }
                }
            }

            return results.ToArray();
        }

        public IPostingEnumerator<Tfeatures> GetCandidateEnumerator(Tfeatures query, int minCount, 
            int maxCount, int permutations, PermutationType permutationType)
        {
            return GetCandidateEnumerator(query, distanceFunction, minCount, maxCount, permutations, permutationType);
        }

        public IPostingEnumerator<Tfeatures> GetCandidateEnumerator(Tfeatures query, IDistanceFunction<Tfeatures> distanceFunction, int minCount, int maxCount, int permutations, PermutationType permutationType)
        {
            List<KeyValuePair<int, int>> ranges = tree.SearchRanges(query, minCount, maxCount, permutations, permutationType);
            lastSequences = ranges.Count;
            return CandidateResultEnumerator<Tfeatures>.Build(query, ranges.ToArray(), featuresStorageReader, distanceFunction);
        }
    }
}
