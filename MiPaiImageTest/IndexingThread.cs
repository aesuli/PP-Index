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

namespace Esuli.MiPai.Image.Test
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using Esuli.Base;
    using Esuli.Base.IO;
    using Esuli.Base.IO.Storage;
    using Esuli.MiPai.Datatypes;
    using Esuli.MiPai.Datatypes.BIC;
    using Esuli.MiPai.Datatypes.ImageThumbnail;
    using Esuli.MiPai.Indexing;

    public enum FeatureType
    {
        Thumbnail,
        BIC,
    }

    public class IndexingThread
    {
        public static readonly double ticksPerSecond = 10000000.0;
        public static readonly int referenceObjectSampleFactor = 100;

        private string indexLocation;
        private string startingPath;
        private StatusStrip statusStrip;
        private int referenceObjectsCount;
        private int permutationPrefixLength;
        private int temporaryIndexMaxHitCount = 100000;
        private int bufferSize = 2 << 24;
        private int parallelIndexingThreads;
        private object featuresExtractor;
        private List<KeyValuePair<int, string>> indexingErrors;
        private FeatureType featureType;
        private int seed;

        public IndexingThread(int seed, FeatureType featureType, int parallelProcesses, string indexLocation, string startingPath, StatusStrip statusStrip, int referenceObjectsCount, int permutationPrefixLength)
        {
            this.seed = seed;
            this.parallelIndexingThreads = parallelProcesses;
            this.indexLocation = indexLocation;
            this.startingPath = startingPath;
            this.statusStrip = statusStrip;
            this.referenceObjectsCount = referenceObjectsCount;
            this.permutationPrefixLength = permutationPrefixLength;
            if (featureType == FeatureType.BIC)
            {
                featuresExtractor = new BICFromFilenameExtractor();
            }
            else if (featureType == FeatureType.Thumbnail)
            {
                featuresExtractor = new ImageThumbnailFromFilenameExtractor();
            }
            indexingErrors = new List<KeyValuePair<int, string>>();
            this.featureType = featureType;
        }

        private void OnIndexingError<Tfeatures>(Exception e, int id, Tfeatures features, object item)
        {
            lock (indexingErrors)
            {
                indexingErrors.Add(new KeyValuePair<int, string>(id, (string)item));
            }
        }

        public void Run()
        {
            var startingDirectory = new DirectoryInfo(startingPath);
            string indexName = startingDirectory.Name + "_" + permutationPrefixLength + "_" + referenceObjectsCount + "_"+ Enum.GetName(typeof(FeatureType),featureType);

            var storage = new SequentialWriteOnlyStorage<string>(indexName + MiPaiImageIndex.filenamesStorageSuffix, indexLocation, true);

            int candidateCount = referenceObjectsCount * referenceObjectSampleFactor;
            string[] candidateReferencePoints = SelectCandidateReferencePoints(startingDirectory, candidateCount);

            Random rand = new Random(seed);

            if (featureType == FeatureType.BIC)
            {
                var referenceObjects = new List<BICDescriptor>(referenceObjectsCount);
                while (referenceObjects.Count < referenceObjectsCount)
                {
                    int i = rand.Next(candidateReferencePoints.Length);
                    try
                    {
                        referenceObjects.Add((featuresExtractor as BICFromFilenameExtractor).ExtractFeatures(0, candidateReferencePoints[i])[0]);
                    }
                    catch
                    {
                    }
                }

                var similarityReader = new PermutationPrefixGenerator<BICDescriptor>(new BICDistanceFunction(), referenceObjects.ToArray());

                BuildIndex<BICDescriptor, BICSerialization>(similarityReader,indexName,storage, startingDirectory);
            }
            else if(featureType ==FeatureType.Thumbnail) {
                var referenceObjects = new List<ImageThumbnail>(referenceObjectsCount);
                while (referenceObjects.Count < referenceObjectsCount)
                {
                    int i = rand.Next(candidateReferencePoints.Length);
                    try
                    {
                        referenceObjects.Add((featuresExtractor as ImageThumbnailFromFilenameExtractor).ExtractFeatures(0, candidateReferencePoints[i])[0]);
                    }
                    catch
                    {
                    }
                }

                var similarityReader = new PermutationPrefixGenerator<ImageThumbnail>(new ImageThumbnailManhattanDistanceFunction(), referenceObjects.ToArray());

                BuildIndex<ImageThumbnail, ImageThumbnailSerialization>(similarityReader, indexName, storage, startingDirectory);
            }

            storage.Close();

            var sb = new StringBuilder();

            foreach (KeyValuePair<int, string> error in indexingErrors)
            {
                sb.AppendLine(error.Key + " " + error.Value);
            }

            MessageBox.Show(sb.ToString(),"Indexing errors",MessageBoxButtons.OK,MessageBoxIcon.Warning,MessageBoxDefaultButton.Button1);
        }

        private void BuildIndex<Tfeatures, TfeatureSerialization>(PermutationPrefixGenerator<Tfeatures> similarityReader, string indexName, SequentialWriteOnlyStorage<string> storage, DirectoryInfo startingDirectory) 
            where Tfeatures:IIntId
            where TfeatureSerialization : class, IFixedSizeObjectSerialization<Tfeatures>, new()
        {
            var indexer = new ParallelIndexer<Tfeatures, TfeatureSerialization>(parallelIndexingThreads, indexName, indexLocation, indexLocation, similarityReader, referenceObjectsCount, 3, 3, temporaryIndexMaxHitCount, bufferSize);

            indexer.IndexingError += OnIndexingError<Tfeatures>;
            long startl = 0;
            long endl = 0;

            startl = DateTime.Now.Ticks;

            IndexDirectory<Tfeatures>(startingDirectory, storage, indexer);

            indexer.BuildIndex(100);

            endl = DateTime.Now.Ticks;

            long ts = endl - startl;

            StatusLabelMessage(storage.Count + " files indexed in "
                + ((double)ts) / ticksPerSecond + " seconds.");
        }

        private void StatusLabelMessage(string message)
        {
            if (statusStrip.InvokeRequired)
            {
                statusStrip.Invoke(new MethodInvoker(() =>
                {
                    statusStrip.Items["toolStripStatusLabel"].Text = message;
                }
                ));
            }
            else
            {
                statusStrip.Items["toolStripStatusLabel"].Text = message;
            }
        }

        private string[] SelectCandidateReferencePoints(DirectoryInfo directoryInfo, int candidateCount)
        {
            int i = 0;
            var candidates = new List<string>(candidateCount);
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                if (i >= candidateCount)
                {
                    break;
                }
                try
                {
                    using (Bitmap image = new Bitmap(fileInfo.FullName))
                    {
                        ++i;
                        candidates.Add(fileInfo.FullName);
                    }
                }
                catch
                {
                }
            }
            foreach (DirectoryInfo dis in directoryInfo.GetDirectories())
            {
                int remaining = candidateCount - candidates.Count;
                if (remaining <= 0)
                {
                    break;
                }
                string[] subDirReferencePoints = SelectCandidateReferencePoints(dis, candidateCount);
                foreach (string subDirReferencePoint in subDirReferencePoints)
                {
                    candidates.Add(subDirReferencePoint);
                }
            }
            return candidates.ToArray();
        }

        private void IndexDirectory<Tfeatures>(DirectoryInfo directoryInfo, ISequentiallyWriteableStorage<string> storage, IOffLineIndexer<Tfeatures> indexer)
        {
            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
            {
                IndexFile(fileInfo, storage, indexer);
            }
            foreach (DirectoryInfo dis in directoryInfo.GetDirectories())
            {
                IndexDirectory(dis, storage, indexer);
            }
        }

        private void IndexFile<Tfeatures>(FileInfo fileInfo, ISequentiallyWriteableStorage<string> storage, IOffLineIndexer<Tfeatures> indexer)
        {
            try
            {
                int id = storage.Write(fileInfo.FullName);
                StatusLabelMessage(id + " " + fileInfo.FullName);
                indexer.Index<string>(id, fileInfo.FullName, featuresExtractor as IFeaturesExtractor<string, Tfeatures>);
            }
            catch
            {
                return;
            }
        }
    }
}