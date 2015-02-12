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

namespace Esuli.MiPai.Indexing
{
    using System;
    using System.Collections.Generic;
    using Esuli.Base.Collections;
    using Esuli.Base.Comparers;
    using Esuli.MiPai.DistanceFunctions;
    using Esuli.MiPai.Index;

    public class PermutationPrefixGenerator<Tfeatures>
    {
        private Tfeatures[] referencePointsFeatures;
        private IDistanceFunction<Tfeatures> distanceFunction;
        private KeyValuePairKeyComparer<double, int> comparer;

        public PermutationPrefixGenerator(IDistanceFunction<Tfeatures> distanceFunction, Tfeatures[] referencePointsFeatures)
        {
            this.referencePointsFeatures = referencePointsFeatures;
            this.distanceFunction = distanceFunction;
            comparer = new KeyValuePairKeyComparer<double, int>();
        }

        public PermutationPrefix GetPermutationPrefix(int id, Tfeatures features, int pointsCount)
        {
            List<KeyValuePair<double, int>> topPoints = ComputeClosestReferencePoints(id, features, pointsCount);
            int[] word = new int[topPoints.Count];
            for (int i = 0; i < topPoints.Count; ++i)
            {
                word[i] = topPoints[i].Value;
            }

            return new PermutationPrefix(word);
        }

        public PermutationPrefix GetPermutationPrefix(int id, Tfeatures features, int pointsCount, out double[] distances)
        {
            List<KeyValuePair<double, int>> topPoints = ComputeClosestReferencePoints(id, features, pointsCount);
            int[] word = new int[topPoints.Count];
            distances = new double[topPoints.Count];
            for (int i = 0; i < topPoints.Count; ++i)
            {
                word[i] = topPoints[i].Value;
                distances[i] = topPoints[i].Key;
            }

            return new PermutationPrefix(word);
        }

        private List<KeyValuePair<double, int>> ComputeClosestReferencePoints(int id, Tfeatures features, int pointsCount)
        {
            pointsCount = Math.Min(pointsCount, referencePointsFeatures.Length);
            List<KeyValuePair<double, int>> topPoints = new List<KeyValuePair<double, int>>(pointsCount);

            for (int i = 0; i < referencePointsFeatures.Length; ++i)
            {
                double distance = distanceFunction.Distance(features, referencePointsFeatures[i]);
                if (topPoints.Count < pointsCount)
                {
                    topPoints.Add(new KeyValuePair<double, int>(distance, i));
                    if (topPoints.Count == pointsCount)
                    {
                        HeapUtils<KeyValuePair<double, int>>.Heapify(topPoints, comparer);
                    }
                }
                else if (distance < topPoints[0].Key)
                {
                    HeapUtils<KeyValuePair<double, int>>.ReplaceItem(new KeyValuePair<double, int>(distance, i), 0,
                         topPoints, comparer);
                }
            }
            if (topPoints.Count < pointsCount)
            {
                HeapUtils<KeyValuePair<double, int>>.Heapify(topPoints, comparer);
            }
            HeapUtils<KeyValuePair<double, int>>.HeapSort(topPoints, comparer, true);

            return topPoints;
        }
        
        public Tfeatures[] ReferencePointsFeatures
        {
            get
            {
                return referencePointsFeatures;
            }
        }

        public IDistanceFunction<Tfeatures> DistanceFunction
        {
            get
            {
                return distanceFunction;
            }
        }
    }
}
