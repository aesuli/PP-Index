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
    using System.Threading.Tasks;
    using Esuli.Base;
    using Esuli.Base.Comparers;
    using Esuli.Base.IO;
    using Esuli.Base.IO.Sort;
    using Esuli.Base.IO.Storage;
    using Esuli.MiPai.Datatypes;
    using Esuli.MiPai.Index;
    using Esuli.MiPai.IO;

    public class ParallelIndexer<Tfeature, TfeatureSerialization>
        : IOffLineIndexer<Tfeature>
        where Tfeature : IIntId
        where TfeatureSerialization : class, IFixedSizeObjectSerialization<Tfeature>, new()
    {
        private static readonly IFixedSizeObjectSerialization<Tfeature> featureSerialization = new TfeatureSerialization();
        private static readonly PermutationPrefixSerialization prefixSerialization = new PermutationPrefixSerialization();

        private int depth;

        private PermutationPrefixGenerator<Tfeature> permutationPrefixGenerator;
        private Stream [] prefixAndFeatureStreams;

        private string name;
        private string location;
        private string tempLocation;

        private int mergeWayCount;
        private int sortProcesses;
        private int blockSize;
        private int bufferSize;

        private int hitCount;

        private Task<int>[] indexingTasks;

        public event IndexingError<Tfeature> IndexingError;

        public ParallelIndexer(int parallelIndexerCount, string name, string location, string tempLocation, PermutationPrefixGenerator<Tfeature> permutationPrefixGenerator, int depth, int mergeWayCount, int sortProcesses, int blockSize, int bufferSize)
        {
            this.name = name;
            this.location = location;
            this.tempLocation = tempLocation;
            this.mergeWayCount = mergeWayCount;
            this.sortProcesses = sortProcesses;
            this.blockSize = blockSize;
            this.bufferSize = bufferSize;
            this.depth = depth;
            hitCount = 0;

            this.permutationPrefixGenerator = permutationPrefixGenerator;
            PermutationPrefixGeneratorSerialization<Tfeature>.Write(permutationPrefixGenerator, name, location, featureSerialization);

            prefixAndFeatureStreams = new Stream[parallelIndexerCount];
            indexingTasks = new Task<int>[parallelIndexerCount];
            for (int i = 0; i < parallelIndexerCount; ++i)
            {
                int j = i;
                indexingTasks[i] = Task<int>.Factory.StartNew(() =>
                {
                    prefixAndFeatureStreams[j] = new FileStream(tempLocation + Path.DirectorySeparatorChar + name + GetParallelStreamName(j) + Esuli.MiPai.IO.Constants.MixedIndexFileExtension, 
                        FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan); 
                    return j;
                });
            }
            Task.WaitAll(indexingTasks);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                for (int i = 0; i < prefixAndFeatureStreams.Length; ++i)
                {
                    prefixAndFeatureStreams[i].Dispose();
                }
            }
        }

        private string GetParallelStreamName(int i)
        {
            string indexName = name + "_parallel-" + i;
            return indexName;
        }

        public void Index<Titem>(int id, Titem item, IFeaturesExtractor<Titem,Tfeature> featuresExtractor)
        {
            var position = Task.WaitAny(indexingTasks);
            indexingTasks[position] = Task<int>.Factory.StartNew(() =>
            {
                Tfeature feature = default(Tfeature);
                try
                {
                    Tfeature [] features = featuresExtractor.ExtractFeatures(id, item);
                    for (int i = 0; i < features.Length; ++i)
                    {
                        try
                        {
                            feature = features[i];
                            var prefix = permutationPrefixGenerator.GetPermutationPrefix(hitCount, feature, depth);
                            prefixSerialization.Write(prefix, prefixAndFeatureStreams[position]);
                            featureSerialization.Write(feature, prefixAndFeatureStreams[position]);
                            ++hitCount;
                        }
                        catch (Exception e)
                        {
                            IndexingError<Tfeature> indexingError = IndexingError;
                            if (indexingError != null)
                            {
                                indexingError(e, id, feature, item);
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    IndexingError<Tfeature> indexingError = IndexingError;
                    if (indexingError != null)
                    {
                        indexingError(e, id, feature, item);
                    }
                }
                return position;
            });
        }

        public void Index(Tfeature feature)
        {
            var position = Task.WaitAny(indexingTasks);
            indexingTasks[position] = Task<int>.Factory.StartNew(() =>
            {
                try
                {
                    var prefix = permutationPrefixGenerator.GetPermutationPrefix(hitCount, feature, depth);
                    prefixSerialization.Write(prefix, prefixAndFeatureStreams[position]);
                    featureSerialization.Write(feature, prefixAndFeatureStreams[position]);
                    ++hitCount;
                }
                catch(Exception e)
                {
                    IndexingError<Tfeature> indexingError = IndexingError;
                    if (indexingError != null)
                    {
                        indexingError(e, feature.Id, feature, null);
                    }
                }

                return position;
            });
        }

        public void BuildIndex(int pruneLevel)
        {
            Task.WaitAll(indexingTasks);
            var fileInfoList = new List<FileInfo>();
            for (int i = 0; i < prefixAndFeatureStreams.Length; ++i)
            {
                prefixAndFeatureStreams[i].Close();
                prefixAndFeatureStreams[i].Dispose();
                prefixAndFeatureStreams[i] = null;
                fileInfoList.Add(new FileInfo(tempLocation + Path.DirectorySeparatorChar + name + GetParallelStreamName(i) + Esuli.MiPai.IO.Constants.MixedIndexFileExtension));
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            var serialization = new PairSerialization<PermutationPrefix, Tfeature>(prefixSerialization, featureSerialization);
            var enumerator = new StreamEnumeratorEnumerator<KeyValuePair<PermutationPrefix, Tfeature>>(new FileInfoStreamEnumerator(fileInfoList.ToArray(), true, bufferSize, FileOptions.SequentialScan), serialization);
            var comparer = new KeyValuePairKeyComparer<PermutationPrefix, Tfeature>();
            var sortedEnumerator = ParallelMWayMergeSort.MergeSort<KeyValuePair<PermutationPrefix, Tfeature>>(enumerator, mergeWayCount, sortProcesses, serialization, comparer, blockSize, bufferSize, tempLocation + Path.DirectorySeparatorChar + name + IO.Constants.MixedIndexFileExtension);

            using (var prefixStream = new FileStream(location + Path.DirectorySeparatorChar + name + Esuli.MiPai.IO.Constants.PermutationPrefixStreamSuffix, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan))
            using (var featureStorage = new SequentialWriteOnlyFixedSizeStorage<Tfeature, TfeatureSerialization>(name, location,true))
            {
                while (sortedEnumerator.MoveNext())
                {
                    var pair = sortedEnumerator.Current;
                    prefixSerialization.Write(pair.Key, prefixStream);
                    featureStorage.Write(pair.Value);
                }
                hitCount = featureStorage.Count;
            }


            using (var stream = new FileStream(location + Path.DirectorySeparatorChar + name + Esuli.MiPai.IO.Constants.PermutationPrefixStreamSuffix, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize, FileOptions.SequentialScan))
            {
                var permutationPrefixEnumerator = new StreamEnumerator<PermutationPrefix>(stream, prefixSerialization);
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
