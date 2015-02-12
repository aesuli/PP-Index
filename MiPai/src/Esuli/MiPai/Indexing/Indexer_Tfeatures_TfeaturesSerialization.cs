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
    using System.IO;
    using Esuli.Base;
    using Esuli.Base.Comparers;
    using Esuli.Base.IO;
    using Esuli.Base.IO.Sort;
    using Esuli.Base.IO.Storage;
    using Esuli.MiPai.Datatypes;
    using Esuli.MiPai.Index;
    using Esuli.MiPai.IO;

    public class Indexer<Tfeature, TfeatureSerialization>
        : IOffLineIndexer<Tfeature>, IDisposable
        where Tfeature : IIntId
        where TfeatureSerialization : class, IFixedSizeObjectSerialization<Tfeature>, new ()
    {
        private int depth;

        private static readonly IFixedSizeObjectSerialization<Tfeature> featureSerialization = new TfeatureSerialization();
        private static readonly PermutationPrefixSerialization prefixSerialization = new PermutationPrefixSerialization();

        private PermutationPrefixGenerator<Tfeature> permutationPrefixGenerator;
        private Stream prefixAndFeatureStream;

        private string name;
        private string location;
        private string tempLocation;

        private int mergeWays;
        private int sortProcesses;
        private int blockSize;
        private int bufferSize;

        private int hitCount;

        public event IndexingError<Tfeature> IndexingError;

        public Indexer(string name, string location, string tempLocation, PermutationPrefixGenerator<Tfeature> permutationPrefixGenerator, int depth, int mergeWays, int sortProcesses, int blockSize, int bufferSize)
        {
            this.name = name;
            this.location = location;
            this.tempLocation = tempLocation;
            this.mergeWays = mergeWays;
            this.sortProcesses = sortProcesses;
            this.blockSize = blockSize;
            this.bufferSize = bufferSize;
            this.depth = depth;
            hitCount = 0;

            this.permutationPrefixGenerator = permutationPrefixGenerator;
            PermutationPrefixGeneratorSerialization<Tfeature>.Write(permutationPrefixGenerator, name, location, featureSerialization);

            prefixAndFeatureStream = new FileStream(tempLocation + Path.DirectorySeparatorChar + name + Esuli.MiPai.IO.Constants.MixedIndexFileExtension, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                prefixAndFeatureStream.Dispose();
            }
        }

        public void Index<Titem>(int id, Titem item, IFeaturesExtractor<Titem, Tfeature> featuresExtractor)
        {
            var feature = default(Tfeature);
            try
            {
                Tfeature[] features = featuresExtractor.ExtractFeatures(id, item);
                for (int i = 0; i < features.Length; ++i)
                {
                    try
                    {
                        feature = features[i];
                        Index(feature);
                    }
                    catch (Exception e)
                    {
                        IndexingError<Tfeature> indexingError = IndexingError;
                        if (indexingError != null)
                        {
                            indexingError(e, id, feature, null);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                IndexingError<Tfeature> indexingError = IndexingError;
                if (indexingError != null)
                {
                    indexingError(e, id, feature, null);
                }
            }
        }

        public void Index(Tfeature feature)
        {
            try
            {
                var prefix = permutationPrefixGenerator.GetPermutationPrefix(hitCount, feature, depth);
                prefixSerialization.Write(prefix, prefixAndFeatureStream);
                featureSerialization.Write(feature, prefixAndFeatureStream);
                ++hitCount;
            }
            catch (Exception e)
            {
                IndexingError<Tfeature> indexingError = IndexingError;
                if (indexingError != null)
                {
                    indexingError(e, feature.Id, feature, null);
                }
            }
        }

        public void BuildIndex(int pruneLevel)
        {
            prefixAndFeatureStream.Close();

            using (var stream = new FileStream(tempLocation + Path.DirectorySeparatorChar + name + Esuli.MiPai.IO.Constants.MixedIndexFileExtension, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize, FileOptions.SequentialScan))
            {
                var serialization = new PairSerialization<PermutationPrefix, Tfeature>(prefixSerialization, featureSerialization);
                var enumerator = new StreamEnumerator<KeyValuePair<PermutationPrefix, Tfeature>>(stream, serialization);
                var comparer = new KeyValuePairKeyComparer<PermutationPrefix, Tfeature>();
                var sortedEnumerator = ParallelMWayMergeSort.MergeSort<KeyValuePair<PermutationPrefix, Tfeature>>(enumerator, mergeWays, sortProcesses, serialization, comparer, blockSize, bufferSize, tempLocation + Path.DirectorySeparatorChar + name + IO.Constants.MixedIndexFileExtension);

                using (var prefixStream = new FileStream(location + Path.DirectorySeparatorChar + name + Esuli.MiPai.IO.Constants.PermutationPrefixStreamSuffix, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan))
                using (var featureStorage = new SequentialWriteOnlyFixedSizeStorage<Tfeature,TfeatureSerialization>(name,location))
                {
                    while (sortedEnumerator.MoveNext())
                    {
                        var pair = sortedEnumerator.Current;
                        prefixSerialization.Write(pair.Key, prefixStream);
                        featureStorage.Write(pair.Value);
                    }
                }
            }

            File.Delete(tempLocation + Path.DirectorySeparatorChar + name + Esuli.MiPai.IO.Constants.MixedIndexFileExtension);

            using (var stream = new FileStream(location + Path.DirectorySeparatorChar + name + Esuli.MiPai.IO.Constants.PermutationPrefixStreamSuffix, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize, FileOptions.SequentialScan)) 
            {
                var permutationPrefixEnumerator = new StreamEnumerator<PermutationPrefix>(stream,prefixSerialization);
                var permutationPrefixTree = new PermutationPrefixTree<Tfeature>(permutationPrefixEnumerator, permutationPrefixGenerator, pruneLevel);
                PermutationPrefixTreeSerialization<Tfeature>.Write(permutationPrefixTree, name, location);
            }
        }

        public long HitCount
        {
            get
            {
                return hitCount;
            }
        }
    }
}
