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
    using Esuli.Base.Enumerators;
    using Esuli.Base.IO.Storage;
    using Esuli.MiPai.DistanceFunctions;
    using Esuli.Scheggia.Core;
    using Esuli.Scheggia.Enumerators;

    public class CandidateResultEnumerator<Tfeatures>
        : IPostingEnumerator<Tfeatures>
    {
        public static IPostingEnumerator<Tfeatures> Build(Tfeatures query, KeyValuePair<int, int>[] ranges, ReadOnlyFixedSizeStorage<Tfeatures> storageReader, IDistanceFunction<Tfeatures> distanceFunction)
        {
            if (ranges.Length == 0)
            {
                return new EmptyPostingEnumerator<Tfeatures>();
            }

            return new CandidateResultEnumerator<Tfeatures>(query, ranges, storageReader, distanceFunction);
        }

        private KeyValuePair<int, int>[] ranges;
        private ReadOnlyFixedSizeStorage<Tfeatures> storageReader;
        private int currentRange;
        private IEnumerator<Tfeatures> currentRangeEnumerator;
        private Tfeatures currentHit;
        private int currentHitId;
        private int count;
        private int progress;
        private ScoreFunction scorefunction;

        private CandidateResultEnumerator(Tfeatures query, KeyValuePair<int, int>[] ranges, ReadOnlyFixedSizeStorage<Tfeatures> storageReader, IDistanceFunction<Tfeatures> distanceFunction)
        {
            this.ranges = ranges;
            this.storageReader = storageReader;
            currentRange = -1;
            currentRangeEnumerator = null;
            currentHit = default(Tfeatures);
            currentHitId = -1;
            count = 0;
            progress = 0;
            foreach (KeyValuePair<int, int> range in ranges)
            {
                count += range.Value - range.Key + 1;
            }
            scorefunction = delegate()
            {
                return distanceFunction.Distance(query, currentHit);
            };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        private bool MoveNextRange()
        {
            int nextRange = currentRange + 1;
            if (nextRange == ranges.Length)
            {
                return false;
            }
            currentRange = nextRange;
            KeyValuePair<int, int> range = ranges[currentRange];
            currentRangeEnumerator = GetRangeEnumerator(range.Key, range.Value);
            currentHitId = range.Key - 1;
            return true;
        }

        private IEnumerator<Tfeatures> GetRangeEnumerator(int start, int end)
        {
            for (int index = start; index <= end; ++index)
            {
                yield return storageReader[index];
            }
        }

        public bool MoveNext()
        {
            if (currentHitId < 0)
            {
                if (!MoveNextRange())
                {
                    currentHit = default(Tfeatures);
                    return false;
                }
            }
            while (!currentRangeEnumerator.MoveNext())
            {
                if (!MoveNextRange())
                {
                    currentHit = default(Tfeatures);
                    return false;
                }
            }
            currentHit = currentRangeEnumerator.Current;
            ++currentHitId;
            ++progress;
            return true;
        }

        public bool MoveNext(int minHitId)
        {
            while (CurrentPostingId < minHitId)
            {
                if (!MoveNext())
                {
                    return false;
                }
            }
            return true;
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public int Progress
        {
            get
            {
                return progress;
            }
        }

        public int CurrentPostingId
        {
            get
            {
                if (currentHit == null)
                {
                    return -1;
                }
                return currentHitId;
            }
        }

        public int CurrentHitCount
        {
            get
            {
                return 1;
            }
        }

        public IHitEnumerator GetCurrentHitEnumerator()
        {
            return GetSpecializedCurrentHitEnumerator();
        }

        public ScoreFunction ScoreFunction
        {
            get
            {
                return scorefunction;
            }
            set
            {
                scorefunction = value;
            }
        }

        public IHitEnumerator<Tfeatures> GetSpecializedCurrentHitEnumerator()
        {
            return new ArrayHitEnumerator<Tfeatures>(currentHitId, new Tfeatures[] { currentHit });
        }
    }
}
