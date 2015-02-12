// Copyright (C) 2013 Andrea Esuli (andrea@esuli.it)
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

namespace PivotSelectionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using System.Xml;
    using Esuli.Base.Comparers;
    using Esuli.Base.IO;
    using Esuli.Base.IO.Storage;
    using Esuli.MiPai.Datatypes.CoPhIR;
    using Esuli.MiPai.Datatypes.MPEG7;
    using Esuli.MiPai.Index;
    using Esuli.MiPai.Indexing;
    using Esuli.MiPai.IO;

    class Program
    {
        private static Stopwatch stopwatch = new Stopwatch();
        private static char[] commaSep = new char[] { ',' };
        private static char[] spaceSep = new char[] { ' ' };

        // This is the code for the PP-Index part of the experiments run for the paper:
        // Giuseppe Amato, Fabrizio Falchi and Andrea Esuli
        // Pivot selection strategies for permutation-based similarity search
        // SISAP 2013, Lecture Notes in Computer Science Volume 8199, 2013, pp 91-102
        // http://link.springer.com/chapter/10.1007%2F978-3-642-41062-8_10

        static void Main(string[] args)
        {
            #region help
            string[] help = new string[] {
                "-c - convert cophir tgz file into binary format",
                "-c <path and prefix of tgz files> <suffix of tgz files> <first file id> <last file id> <path and name of output storage>",
                @"e.g.: -c H:\CoPhIR-tar\sapir_id_ _xml_r.tgz 1 106 H:\CoPhIR-dat\CoPhIR",
                "",
                "-m - map groundtruth flickr ids to storage positions",
                "-m <path to flickr id file> <path to idMap file>",
                @"e.g.: -m H:\GroundTruth\CoPhIR_10000k.csv H:\CoPhIR-dat\CoPhIR.idmap",
                "",
                "-t - compare distance function with groundtruth distances",
                "-t <csv file with groundtruth distances> <file with mapped ids> <path and name of cophir storage>",
                @"e.g.: -t H:\GroundTruth\CoPhIR_10000k.csv H:\CoPhIR-dat\CoPhIR.idmap.GT H:\CoPhIR-dat\CoPhIR",
                "",
                "-s - distance computation speed test",
                "-s <path and name of cophir storage> <number of object to test>",
                @"e.g.: -s H:\CoPhIR-dat\CoPhIR 1000",
                "",
                "-sas - sequential access speed test",
                "-sas <path and name of cophir storage> <number of object to test>",
                @"e.g.: -sas H:\CoPhIR-dat\CoPhIR 1000",
                "",
                "-sar - random access speed test",
                "-sar <path and name of cophir storage> <number of object to test>",
                @"e.g.: -sar H:\CoPhIR-dat\CoPhIR 1000",
                "",
                "-ri - indexing with random selection of pivots",
                "-ri <seed> <number of pivots> <permutation lenght> <path and name of cophir storage> <output director> <number of parallel indexing threads>",
                @"e.g.: -ri 0 1000 6 H:\CoPhIR-dat\CoPhIR E:\Indexes\ 12",
                "",
                "-i - index using user provided ids for pivots",
                "-i <file with flickr ids of pivots> <path and name of cophir storage> <output directory> <permutation lenght> <number of parallel indexing threads> [number of pivots to keep]",
                @"e.g.: -i H:\Medoids\1k\CoPhiR_1M_rnd_fast_1k_med_6.dat.txt H:\CoPhIR-dat\CoPhIR E:\Indexes 6 12",
                "",
                "-q - search on index",
                "-q <path and name of cophir storage> <csv file with groundtruth distances> <mapped flickr ids> <index> <output directory> <number of results per query> <number of min candidates> <number of maxcandidates> <number of query prefix permutations>",
                @"e.g.: -q H:\CoPhIR-dat\CoPhIR H:\GroundTruth\CoPhIR_10000k.csv H:\CoPhIR-dat\CoPhIR.idmap.GT E:\Indexes\CoPhIR_rand_0_1000_6 H:\Results\ 101 1000 10000 0",
                "",
                "-e - evaluate results",
                "-e <csv file with groundtruth distances> <output directory> <number of results to evaluate> <result file>",
                @"e.g.: -e H:\GroundTruth\CoPhIR_10000k.csv H:\Results\ 100 H:\Results\CoPhIR_rand_0 _1000_6_CoPhIR_10000k.csv_101_1000_10000_0.txt",
                "",
                "-x - mix results",
                "-x <number of results> <output filename> <input results filename>+",
                @"e.g.: -x 101 H:\Results\mix.txt H:\Results\src1.txt H:\Results\src2.txt H:\Results\src3.txt",
                "",
                "-ts - prefix tree stats",
                "-ts <index path and name> <prune level>",
                @"e.g.: -ts H:\Indexes\idx 1000",
                "",
                "-ps prefix stats",
                "-ps <prefix filename>",
                @"e.g.: -ps H:\Indexes\idx.ppr",
            };

            if (args.Length == 0)
            {
                Console.Out.Write("Command: (just hit return to exit) ");
                var tokens = Console.In.ReadLine().Split(new char[] { ' ' });
                var list = new List<string>();
                string token = "";
                bool inQuotes = false;
                for (int i = 0; i < tokens.Length; ++i)
                {
                    if (tokens[i].StartsWith("\""))
                    {
                        token = tokens[i].Substring(1);
                        inQuotes = true;
                    }
                    else if (inQuotes)
                    {
                        token += " " + tokens[i];
                        if (token.EndsWith("\""))
                        {
                            list.Add(token.Remove(token.Length - 1));
                            inQuotes = false;
                            token = "";
                        }
                    }
                    else
                    {
                        if (tokens[i].Length > 0)
                        {
                            list.Add(tokens[i]);
                        }
                    }
                }
                args = list.ToArray();
            }

            if (args.Length == 0)
            {
                foreach (string line in help)
                {
                    Console.WriteLine(line);
                }
                return;
            }
            #endregion

            int bufferSize = Esuli.Base.Constants.OneMebi * 10;

            #region convert
            if (args[0] == "-c")
            {
                string prefix = args[1];
                string suffix = args[2];
                int start = int.Parse(args[3]);
                int end = int.Parse(args[4]);
                FileInfo fileInfo = new FileInfo(args[5]);
                string storageName = fileInfo.Name;
                string storageLocation = fileInfo.DirectoryName;
                if (!Directory.Exists(storageLocation))
                {
                    Directory.CreateDirectory(storageLocation);
                }

                stopwatch.Reset();
                stopwatch.Start();

                ISelector selector = new SelectAll();

                int mod = 100000;

                using (var storageWriter = new SequentialWriteOnlyFixedSizeStorage<CoPhIRData, CoPhIRDataObjectSerialization<SelectAll>>(storageName, storageLocation, true))
                using (var idStream = File.CreateText(storageLocation + Path.DirectorySeparatorChar + storageName + ".idmap"))
                {
                    try
                    {
                        for (int i = start; i <= end; ++i)
                        {
                            {


                                string dataFile = prefix + i + suffix;

                                Stream stream = new FileStream(dataFile, FileMode.Open, FileAccess.Read);
                                TextReader textReader = new StreamReader(new GZipStream(stream, CompressionMode.Decompress));
                                string line;
                                int id = -1;
                                int farmId = -1;
                                int serverId = -1;
                                long secret = -1;
                                CoPhIRData prevImage = null;
                                try
                                {
                                    while ((line = textReader.ReadLine()) != null)
                                    {
                                        if (line.ToLower().StartsWith("<photo"))
                                        {
                                            int pos = line.IndexOf("id=\"") + 4;
                                            string portion = line.Substring(pos, line.IndexOf("\"", pos) - pos);
                                            id = int.Parse(portion);

                                            pos = line.IndexOf("farm=\"") + 6;
                                            portion = line.Substring(pos, line.IndexOf("\"", pos) - pos);
                                            farmId = int.Parse(portion);

                                            pos = line.IndexOf("server=\"") + 8;
                                            portion = line.Substring(pos, line.IndexOf("\"", pos) - pos);
                                            serverId = int.Parse(portion);

                                            pos = line.IndexOf("secret=\"") + 8;
                                            portion = line.Substring(pos, line.IndexOf("\"", pos) - pos);
                                            secret = long.Parse(portion, System.Globalization.NumberStyles.HexNumber);
                                        }
                                        if (line.Contains("<Mpeg7>"))
                                        {
                                            StringBuilder stringBuilder = new StringBuilder(line);
                                            while ((line = textReader.ReadLine()) != null)
                                            {
                                                stringBuilder.Append(line);
                                                if (line.Contains("</Mpeg7>"))
                                                {
                                                    break;
                                                }
                                            }

                                            try
                                            {
                                                StringReader stringReader = new StringReader(stringBuilder.ToString());
                                                XmlReader reader = XmlReader.Create(stringReader);
                                                while (reader.Read())
                                                {
                                                    switch (reader.NodeType)
                                                    {
                                                        case (XmlNodeType.Element):
                                                            if (reader.Name == "Image")
                                                            {
                                                                CoPhIRData image = new CoPhIRData(reader, selector);
                                                                if (id == -1 || farmId == -1 || serverId == -1 || secret == -1)
                                                                {
                                                                    throw new Exception("Missing id");
                                                                }
                                                                image.Id = id;
                                                                image.FarmId = farmId;
                                                                image.ServerId = serverId;
                                                                image.Secret = secret;

                                                                id = -1;
                                                                farmId = -1;
                                                                serverId = -1;
                                                                secret = -1;
                                                                if (storageWriter.Count % mod == 0)
                                                                {
                                                                    Console.Write(".");
                                                                }
                                                                idStream.WriteLine(image.Id + " " + storageWriter.Count);
                                                                storageWriter.Write(image);
                                                                prevImage = image;
                                                            }
                                                            break;
                                                    }
                                                }
                                                reader.Close();
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(dataFile + "\n" + id + "\n" + storageWriter.Count + "\n" + stream.Position + "\n" + e.ToString() + "\n" + e.StackTrace);
                                                if (storageWriter.Count % mod == 0)
                                                {
                                                    Console.Write(".");
                                                }
                                                idStream.WriteLine(prevImage.Id + " " + storageWriter.Count);
                                                storageWriter.Write(prevImage);
                                            }

                                        }
                                    }
                                    textReader.Close();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(dataFile + "\n" + id + "\n" + storageWriter.Count + "\n" + stream.Position + "\n" + e.ToString() + "\n" + e.StackTrace);
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        int storageCount = storageWriter.Count;

                        stopwatch.Stop();

                        Console.WriteLine("Data read in " + stopwatch.Elapsed.TotalSeconds + " seconds (" + storageCount + ")");
                    }
                }
            }
            #endregion
            #region map ids
            else if (args[0] == "-m")
            {
                int mod = 100000;
                int counter = 0;
                string groundTruthFilename = args[1];
                string idMapFilename = args[2];

                var commaSep = new char[] { ',' };
                var spaceSep = new char[] { ' ' };

                var idSet = new HashSet<int>();
                using (var groundTruthStream = File.OpenText(groundTruthFilename))
                {
                    string line = null;
                    while ((line = groundTruthStream.ReadLine()) != null)
                    {
                        var values = line.Split(commaSep);
                        int queryId = int.Parse(values[0]);
                        idSet.Add(queryId);
                        for (int i = 1; i < values.Length; i += 2)
                        {
                            int resultId = int.Parse(values[i]);
                            idSet.Add(resultId);
                        }
                    }
                }

                var idMap = new SortedDictionary<int, int>();
                using (var idMapStream = File.OpenText(idMapFilename))
                {
                    string line = null;
                    while ((line = idMapStream.ReadLine()) != null)
                    {
                        var values = line.Split(spaceSep);
                        int flickrId = int.Parse(values[0]);
                        if (idSet.Contains(flickrId))
                        {
                            int storagePosition = int.Parse(values[1]);
                            idMap.Add(flickrId, storagePosition);
                        }
                        if (++counter % mod == 0)
                        {
                            Console.Write(".");
                        }
                    }
                }

                using (var outIdMapStream = File.CreateText(idMapFilename + ".GT"))
                {
                    var pairs = idMap.GetEnumerator();
                    while (pairs.MoveNext())
                    {
                        outIdMapStream.WriteLine("{0} {1}", pairs.Current.Key, pairs.Current.Value);
                    }
                }
            }
            #endregion
            #region test distances
            else if (args[0] == "-t")
            {
                string groundTruthFilename = args[1];
                string idMapFilename = args[2];
                var storageFileInfo = new FileInfo(args[3]);
                string storageName = storageFileInfo.Name;
                string storageLocation = storageFileInfo.DirectoryName;

                var groundTruth = new Dictionary<int, List<KeyValuePair<int, double>>>();
                using (var groundTruthStream = File.OpenText(groundTruthFilename))
                {
                    string line = null;
                    while ((line = groundTruthStream.ReadLine()) != null)
                    {
                        var values = line.Split(commaSep);
                        int queryId = int.Parse(values[0]);
                        var result = new List<KeyValuePair<int, double>>();
                        for (int i = 1; i < values.Length; i += 2)
                        {
                            int resultId = int.Parse(values[i]);
                            result.Add(new KeyValuePair<int, double>(resultId, Double.Parse(values[i + 1])));
                        }
                        groundTruth.Add(queryId, result);
                    }
                }

                var idMap = new SortedDictionary<int, int>();
                using (var idMapStream = File.OpenText(idMapFilename))
                {
                    string line = null;
                    while ((line = idMapStream.ReadLine()) != null)
                    {
                        var values = line.Split(spaceSep);
                        int flickrId = int.Parse(values[0]);
                        int storagePosition = int.Parse(values[1]);
                        idMap.Add(flickrId, storagePosition);
                    }
                }

                double eps = 0.0001;
                var distanceFunction = new CoPhIRFastDataDistanceFunction();
                using (var storage = new ReadOnlyFixedSizeStorage<CoPhIRData>(storageName, storageLocation))
                {
                    var queries = groundTruth.GetEnumerator();
                    while (queries.MoveNext())
                    {
                        int queryFlickrId = queries.Current.Key;
                        var queryObj = new CoPhIRFastData(storage[idMap[queryFlickrId]]);
                        var results = queries.Current.Value;
                        foreach (var result in results)
                        {
                            int resultFlickrId = result.Key;
                            double realDistance = result.Value;
                            var resultObj = new CoPhIRFastData(storage[idMap[resultFlickrId]]);
                            double myDistance = distanceFunction.Distance(queryObj, resultObj);
                            if (Math.Abs(myDistance - realDistance) > eps)
                            {
                                Console.WriteLine("NO {0} {1} {2} {3} {4}", queryFlickrId, resultFlickrId, realDistance, myDistance, realDistance - myDistance);
                            }
                            else
                            {
                                Console.WriteLine("OK {0} {1} {2} {3} {4}", queryFlickrId, resultFlickrId, realDistance, myDistance, realDistance - myDistance);
                            }
                        }
                    }
                }
            }
            #endregion
            #region random pivot selection indexing
            else if (args[0] == "-ri")
            {
                int seed = int.Parse(args[1]);
                int pivotCount = int.Parse(args[2]);
                int prefixLength = int.Parse(args[3]);
                var storageFileInfo = new FileInfo(args[4]);
                string storageName = storageFileInfo.Name;
                string storageLocation = storageFileInfo.DirectoryName;
                string indexLocation = args[5];

                int parallelIndexers = int.Parse(args[6]);

                int pruneLevel = 1000;

                string tempLocation = indexLocation;
                int mergeWays = 16;
                int blockSize = 100 * Esuli.Base.Constants.OneKilo;

                string indexName = storageName + "_rand_" + seed + "_" + pivotCount + "_" + prefixLength;

                stopwatch.Reset();
                stopwatch.Start();

                using (var storage = new ReadOnlyFixedSizeStorage<CoPhIRData>(storageName, storageLocation))
                {

                    var referencePoints = new List<CoPhIRFastData>();

                    var rand = new Random(seed);
                    for (int i = 0; i < pivotCount; ++i)
                    {
                        referencePoints.Add(new CoPhIRFastData(storage[rand.Next(storage.Count)]));
                    }

                    var indexer = new ParallelIndexer<CoPhIRFastData, CoPhIRFastDataObjectSerialization>(parallelIndexers, indexName, indexLocation, tempLocation, new PermutationPrefixGenerator<CoPhIRFastData>(new CoPhIRFastDataDistanceFunction(), referencePoints.ToArray()), prefixLength, mergeWays, parallelIndexers, blockSize, bufferSize);

                    var featureExtractor = new CoPhIRFastDataExtractor();

                    for (int i = 0; i < storage.Count; ++i)
                    {
                        CoPhIRData image = storage[i];

                        indexer.Index<CoPhIRData>(image.Id, image, featureExtractor);

                        if (i % 1000 == 0)
                        {
                            if (i % 10000 == 0)
                            {
                                if (i % 100000 == 0)
                                {
                                    Console.Out.Write(i);
                                }
                                else
                                {
                                    Console.Out.Write("+");
                                }
                            }
                            else
                            {
                                Console.Out.Write(".");
                            }
                        }
                    }

                    indexer.BuildIndex(pruneLevel);

                    stopwatch.Stop();

                    Console.WriteLine("Data indexed in " + stopwatch.Elapsed.TotalSeconds + " seconds (" + indexer.HitCount + ")");
                }
            }
            #endregion
            #region user provided pivot selection indexing
            else if (args[0] == "-i")
            {
                var pivotList = new FileInfo(args[1]);
                var storageFileInfo = new FileInfo(args[2]);
                string storageName = storageFileInfo.Name;
                string storageLocation = storageFileInfo.DirectoryName;
                string indexLocation = args[3];
                int prefixLength = int.Parse(args[4]);
                int parallelIndexers = int.Parse(args[5]);
                int toKeep = int.MaxValue;
                if (args.Length > 6)
                {
                    toKeep = int.Parse(args[6]);
                }
                int pruneLevel = 100;

                string tempLocation = indexLocation;
                int mergeWays = 16;
                int blockSize = 200 * Esuli.Base.Constants.OneKilo;

                stopwatch.Reset();
                stopwatch.Start();

                using (var storage = new ReadOnlyFixedSizeStorage<CoPhIRData>(storageName, storageLocation))
                {
                    if (!File.Exists(pivotList.FullName + "_map"))
                    {
                        var idSet = new HashSet<int>();
                        using (var idFile = File.OpenText(pivotList.FullName))
                        {
                            string line = null;
                            while ((line = idFile.ReadLine()) != null)
                            {
                                idSet.Add(int.Parse(line));
                            }
                        }

                        using (var mapFile = File.CreateText(pivotList.FullName + "_map"))
                        {
                            for (int i = 0; i < storage.Count; ++i)
                            {
                                var data = storage[i];
                                if (idSet.Contains(data.Id))
                                {
                                    mapFile.WriteLine(i);
                                }
                            }
                            Console.WriteLine();
                        }
                    }

                    var referencePoints = new List<CoPhIRFastData>();
                    using (var mapFile = File.OpenText(pivotList.FullName + "_map"))
                    {
                        string line = null;
                        int kept = 0;
                        while ((line = mapFile.ReadLine()) != null && kept < toKeep)
                        {
                            referencePoints.Add(new CoPhIRFastData(storage[int.Parse(line)]));
                            ++kept;
                            if (referencePoints.Count % 10 == 0)
                            {
                                Console.Write("!");
                            }
                        }
                    }

                    Console.Out.WriteLine("Using {0} pivots", referencePoints.Count);

                    string indexName = storageName + "_" + pivotList.Name + "_" + referencePoints.Count + "_" + prefixLength;

                    var featuresExtractor = new CoPhIRFastDataExtractor();

                    var indexer = new ParallelIndexer<CoPhIRFastData, CoPhIRFastDataObjectSerialization>(parallelIndexers, indexName, indexLocation, tempLocation, new PermutationPrefixGenerator<CoPhIRFastData>(new CoPhIRFastDataDistanceFunction(), referencePoints.ToArray()), prefixLength, mergeWays, parallelIndexers, blockSize, bufferSize);

                    for (int i = 0; i < storage.Count; ++i)
                    {
                        CoPhIRData image = storage[i];

                        indexer.Index<CoPhIRData>(image.Id, image, featuresExtractor);

                        if (i % 1000 == 0)
                        {
                            if (i % 10000 == 0)
                            {
                                if (i % 100000 == 0)
                                {
                                    Console.Write(i);
                                }
                                else
                                {
                                    Console.Write("+");
                                }
                            }
                            else
                            {
                                Console.Write(".");
                            }
                        }
                    }

                    indexer.BuildIndex(pruneLevel);

                    stopwatch.Stop();

                    Console.WriteLine("Data indexed in " + stopwatch.Elapsed.TotalSeconds + " seconds (" + indexer.HitCount + ")");
                }
            }
            #endregion
            #region distance speed test
            else if (args[0] == "-s")
            {
                var storageFileInfo = new FileInfo(args[1]);
                string storageName = storageFileInfo.Name;
                string storageLocation = storageFileInfo.DirectoryName;

                var size = int.Parse(args[2]);
                var vect = new CoPhIRFastData[size];

                using (var storage = new ReadOnlyFixedSizeStorage<CoPhIRData>(storageName, storageLocation))
                {
                    for (int i = 0; i < size; ++i)
                    {
                        vect[i] = new CoPhIRFastData(storage[i]);
                    }
                }
                var f = new CoPhIRFastDataDistanceFunction();

                stopwatch.Start();
                for (int i = 0; i < size; ++i)
                {
                    var ob1 = vect[i];
                    for (int j = 0; j < size; ++j)
                    {
                        var ob2 = vect[j];
                        f.Distance(ob1, ob2);
                    }
                }

                stopwatch.Stop();

                Console.WriteLine("Distance computation time for {0} objects {1:E} seconds\n", size, stopwatch.Elapsed.TotalSeconds / size);
            }
            #endregion
            #region sequential access speed test
            else if (args[0] == "-sas")
            {
                var storageFileInfo = new FileInfo(args[1]);
                string storageName = storageFileInfo.Name;
                string storageLocation = storageFileInfo.DirectoryName;

                var size = int.Parse(args[2]);
                var count = int.Parse(args[3]);
                var vect = new CoPhIRFastData[size];

                stopwatch.Reset();
                using (var storage = new ReadOnlyFixedSizeStorage<CoPhIRData>(storageName, storageLocation))
                {
                    for (int i = 0; i < count; ++i)
                    {
                        Console.WriteLine(".");

                        int offset = new Random().Next(storage.Count - size);
                        for (int j = 0; j < size; ++j)
                        {
                            stopwatch.Start();
                            vect[j] = new CoPhIRFastData(storage[offset + j]);
                            stopwatch.Stop();
                        }
                    }
                }

                Console.WriteLine("Sequential access time for {0} objects {1:E} seconds\n", size, stopwatch.Elapsed.TotalSeconds / count);
            }
            #endregion
            #region random access speed test
            else if (args[0] == "-sar")
            {
                var storageFileInfo = new FileInfo(args[1]);
                string storageName = storageFileInfo.Name;
                string storageLocation = storageFileInfo.DirectoryName;

                var size = int.Parse(args[2]);
                var count = int.Parse(args[3]);

                var vect = new CoPhIRFastData[size];

                stopwatch.Reset();
                using (var storage = new ReadOnlyFixedSizeStorage<CoPhIRData>(storageName, storageLocation))
                {
                    for (int i = 0; i < count; ++i)
                    {
                        Console.WriteLine(".");

                        var rand = new Random();
                        for (int j = 0; j < size; ++j)
                        {
                            stopwatch.Start();
                            vect[j] = new CoPhIRFastData(storage[rand.Next(storage.Count)]);
                            stopwatch.Stop();
                        }
                    }
                }

                Console.WriteLine("Random access time for {0} objects {1:E} seconds\n", size, stopwatch.Elapsed.TotalSeconds / count);
            }
            #endregion
            #region search
            else if (args[0] == "-q")
            {
                var storageFileInfo = new FileInfo(args[1]);
                string storageName = storageFileInfo.Name;
                string storageLocation = storageFileInfo.DirectoryName;

                var queryFileInfo = new FileInfo(args[2]);

                var idMapFilename = args[3];

                var indexFileInfo = new FileInfo(args[4]);
                string indexName = indexFileInfo.Name;
                string indexLocation = indexFileInfo.DirectoryName;

                string resultsLocation = args[5];

                int resultsCount = int.Parse(args[6]);

                int minCount = int.Parse(args[7]);
                int maxCount = int.Parse(args[8]);

                int permutations = int.Parse(args[9]);

                int maxPermutations = 10;

                var idMap = new SortedDictionary<int, int>();
                using (var idMapStream = File.OpenText(idMapFilename))
                {
                    string line = null;
                    while ((line = idMapStream.ReadLine()) != null)
                    {
                        var values = line.Split(spaceSep);
                        int flickrId = int.Parse(values[0]);
                        int storagePosition = int.Parse(values[1]);
                        idMap.Add(flickrId, storagePosition);
                    }
                }

                var queries = new List<int>();
                using (var groundTruthStream = File.OpenText(queryFileInfo.FullName))
                {
                    string line = null;
                    while ((line = groundTruthStream.ReadLine()) != null)
                    {
                        var values = line.Split(commaSep);
                        int queryId = int.Parse(values[0]);
                        queries.Add(queryId);
                    }
                }

                ReadOnlyFixedSizeStorage<CoPhIRData> storageReader = new ReadOnlyFixedSizeStorage<CoPhIRData>(storageName, storageLocation);

                using (var index = new PermutationPrefixIndex<CoPhIRFastData>(indexName, indexLocation, minCount))
                {

                    double avgTime = 0.0;
                    double avgCandidates = 0.0;
                    double avgSequences = 0.0;
                    var featuresExtractor = new CoPhIRFastDataExtractor();
                    using (var resultFile = File.CreateText(resultsLocation + Path.DirectorySeparatorChar + indexName + "_" + queryFileInfo.Name + "_" + resultsCount + "_" + minCount + "_" + maxCount + "_" + permutations + ".txt"))
                    {
                        for (int i = 0; i < queries.Count; ++i)
                        {
                            CoPhIRData image = storageReader[idMap[queries[i]]];

                            KeyValuePair<double, KeyValuePair<int, int>>[] results;
                            int localPermutations = permutations;
                            do
                            {
                                stopwatch.Reset();
                                stopwatch.Start();
                                results = index.Search(featuresExtractor.ExtractFeatures(-1, image)[0], resultsCount, minCount, maxCount, localPermutations, PermutationType.ClosestPair, int.MaxValue);
                                stopwatch.Stop();
                                ++localPermutations;
                            }
                            while (results.Length < resultsCount && localPermutations <= maxPermutations);

                            if (results.Length < resultsCount)
                            {
                                Console.WriteLine("Query " + i + " has less results than requested.");
                            }

                            resultFile.Write(image.Id);
                            for (int j = 0; j < results.Length; ++j)
                            {
                                resultFile.Write(",");
                                resultFile.Write(results[j].Value.Value);
                                resultFile.Write(",");
                                resultFile.Write(results[j].Key);
                            }
                            resultFile.WriteLine();
                            Console.WriteLine("Query " + i + " done in " + stopwatch.Elapsed.TotalSeconds + " seconds " + index.LastAccessed + " accessed " + index.LastSequences + " sequences.");
                            avgTime += stopwatch.Elapsed.TotalSeconds;
                            avgCandidates += index.LastAccessed;
                            avgSequences += index.LastSequences;
                        }
                        Console.WriteLine("Average\t" + index.Depth + "\t" + avgTime / queries.Count + "\t" + avgCandidates / queries.Count + "\t" + avgSequences / queries.Count + ".");
                    }
                }
            }
            #endregion
            #region evaluate
            else if (args[0] == "-e")
            {
                string groundTruthFilename = args[1];
                string evaluationDirectory = args[2];
                int k = int.Parse(args[3]);

                var resultsSortedWithDistances = new Dictionary<int, List<KeyValuePair<double, int>>>();
                string resultsFilename = args[4];
                using (var resultsStream = File.OpenText(resultsFilename))
                {
                    Console.Out.WriteLine("Reading results file {0}", resultsFilename);
                    string line = null;
                    while ((line = resultsStream.ReadLine()) != null)
                    {
                        var values = line.Split(commaSep);
                        int queryId = int.Parse(values[0]);
                        var resultSortedWithDistances = new List<KeyValuePair<double, int>>();
                        for (int j = 1; j < values.Length; j += 2)
                        {
                            int resultId = int.Parse(values[j]);
                            if (resultId != queryId)
                            {
                                resultSortedWithDistances.Add(new KeyValuePair<double, int>(Double.Parse(values[j + 1]), resultId));
                            }
                        }
                        resultsSortedWithDistances.Add(queryId, resultSortedWithDistances);
                    }
                }

                var resultsAsSet = new Dictionary<int, HashSet<int>>();
                var resultEnumerator = resultsSortedWithDistances.GetEnumerator();
                while (resultEnumerator.MoveNext())
                {
                    var resultList = resultEnumerator.Current.Value;
                    resultList.Sort(new KeyValuePairKeyComparer<double, int>());
                    if (resultList.Count > k)
                    {
                        resultList.RemoveRange(k, resultList.Count - k);
                    }
                    var resultAsSet = new HashSet<int>();
                    foreach (var pair in resultList)
                    {
                        resultAsSet.Add(pair.Value);
                    }
                    int queryId = resultEnumerator.Current.Key;
                    resultsAsSet.Add(queryId, resultAsSet);
                }

                var groundTruthSortedWithDistances = new Dictionary<int, List<KeyValuePair<int, double>>>();
                var groundTruthAsSet = new Dictionary<int, HashSet<int>>();
                using (var groundTruthStream = File.OpenText(groundTruthFilename))
                {
                    string line = null;
                    while ((line = groundTruthStream.ReadLine()) != null)
                    {
                        var values = line.Split(commaSep);
                        int queryId = int.Parse(values[0]);
                        var resultSortedWithDistances = new List<KeyValuePair<int, double>>();
                        var resultAsSet = new HashSet<int>();
                        int localK = 0;
                        for (int i = 1; i < values.Length; i += 2)
                        {
                            int resultId = int.Parse(values[i]);
                            resultSortedWithDistances.Add(new KeyValuePair<int, double>(resultId, Double.Parse(values[i + 1])));
                            resultAsSet.Add(resultId);
                            ++localK;
                            if (localK == k)
                            {
                                break;
                            }
                        }
                        groundTruthSortedWithDistances.Add(queryId, resultSortedWithDistances);
                        groundTruthAsSet.Add(queryId, resultAsSet);
                    }
                }

                using (var evaluationFile = File.CreateText(evaluationDirectory + Path.DirectorySeparatorChar + "eval_" + new FileInfo(groundTruthFilename).Name + "_" + new FileInfo(args[4]).Name + ".txt"))
                {
                    double recall = 0.0;
                    var recalls = new Dictionary<int, double>();
                    var counter = 0;
                    foreach (int queryId in groundTruthAsSet.Keys)
                    {
                        var gtSet = groundTruthAsSet[queryId];
                        int localCounter = 0;
                        double localRecall = 0.0;
                        foreach (int resultId in resultsAsSet[queryId])
                        {
                            if (gtSet.Contains(resultId))
                            {
                                ++localRecall;
                            }
                            ++localCounter;
                        }
                        recalls[queryId] = localRecall / localCounter;
                        recall += localRecall;
                        counter += localCounter;
                    }
                    recall /= counter;
                    evaluationFile.WriteLine("avg " + recall);
                    foreach (var queryId in recalls.Keys)
                    {
                        evaluationFile.WriteLine(queryId + " " + recalls[queryId]);
                    }
                }
            }
            #endregion
            #region mix results
            else if (args[0] == "-x")
            {
                int k = int.Parse(args[1]);
                string outputFilename = args[2];

                var resultsSortedWithDistances = new Dictionary<int, List<KeyValuePair<double, int>>>();
                for (int i = 3; i < args.Length; ++i)
                {
                    string resultsFilename = args[i];
                    using (var resultsStream = File.OpenText(resultsFilename))
                    {
                        Console.Out.WriteLine("Reading results file {0}", resultsFilename);
                        string line = null;
                        while ((line = resultsStream.ReadLine()) != null)
                        {
                            var values = line.Split(commaSep);
                            int queryId = int.Parse(values[0]);
                            List<KeyValuePair<double, int>> resultSortedWithDistances;
                            if (i == 3)
                            {
                                resultSortedWithDistances = new List<KeyValuePair<double, int>>();
                                resultsSortedWithDistances.Add(queryId, resultSortedWithDistances);
                            }
                            else
                            {
                                resultSortedWithDistances = resultsSortedWithDistances[queryId];
                            }
                            for (int j = 1; j < values.Length; j += 2)
                            {
                                int resultId = int.Parse(values[j]);
                                resultSortedWithDistances.Add(new KeyValuePair<double, int>(Double.Parse(values[j + 1]), resultId));
                            }
                        }
                    }
                }

                using (var output = File.CreateText(outputFilename))
                {
                    var resultEnumerator = resultsSortedWithDistances.GetEnumerator();
                    while (resultEnumerator.MoveNext())
                    {
                        var resultList = resultEnumerator.Current.Value;
                        resultList.Sort(new KeyValuePairKeyComparer<double, int>());
                        for (int i = 0; i < resultList.Count - 1; ++i)
                        {
                            if (resultList[i].Value == resultList[i + 1].Value)
                            {
                                resultList.RemoveAt(i);
                                --i;
                            }
                        }
                        if (resultList.Count > k)
                        {
                            resultList.RemoveRange(k, resultList.Count - k);
                        }
                        int queryId = resultEnumerator.Current.Key;
                        output.Write(queryId);
                        foreach (var pair in resultList)
                        {
                            output.Write("," + pair.Value + "," + pair.Key);
                        }
                        output.WriteLine();
                    }
                }
            }
            #endregion
            #region prefix tree stats
            else if (args[0] == "-ts")
            {
                var idxFileInfo = new FileInfo(args[1]);
                var pruneLevel = int.Parse(args[2]);
                var prefixTree = PermutationPrefixTreeSerialization<CoPhIRFastData>.Read(idxFileInfo.Name, idxFileInfo.DirectoryName, pruneLevel);
                PrintPrefixTree(prefixTree.Root);
            }
            #endregion
            #region prefix stats
            else if (args[0] == "-ps")
            {
                var fileInfo = new FileInfo(args[1]);
                using (var stream = new FileStream(args[1], FileMode.Open, FileAccess.Read, FileShare.None, bufferSize, FileOptions.SequentialScan))
                {
                    var permutationPrefixEnumerator = new StreamEnumerator<PermutationPrefix>(stream, new PermutationPrefixSerialization());
                    if (!permutationPrefixEnumerator.MoveNext())
                    {
                        return;
                    }

                    PermutationPrefix prevPP = permutationPrefixEnumerator.Current;

                    var prefixLength = prevPP.Data.Length;
                    var ppGroupCounts = new int[prefixLength];
                    for (var i = 0; i < prefixLength; ++i)
                    {
                        ppGroupCounts[i] = 1;
                    }

                    var total = 0;
                    while (permutationPrefixEnumerator.MoveNext())
                    {
                        var pp = permutationPrefixEnumerator.Current;
                        for (var i = 0; i < prefixLength; ++i)
                        {
                            if (prevPP.Data[i] != pp.Data[i])
                            {
                                ++ppGroupCounts[i];
                            }
                        }
                        ++total;
                        prevPP = pp;
                    }

                    var averages = new double[prefixLength];
                    var sums = new double[prefixLength];
                    var ppGroupSizes = new int[prefixLength];
                    var mins = new int[prefixLength];
                    var maxs = new int[prefixLength];
                    for (int i = 0; i < prefixLength; ++i)
                    {
                        averages[i] = (double)total / ppGroupCounts[i];
                        sums[i] = 0.0;
                        ppGroupSizes[i] = 1;
                        maxs[i] = 0;
                        mins[i] = int.MaxValue;
                    }

                    permutationPrefixEnumerator.Reset();
                    permutationPrefixEnumerator.MoveNext();
                    prevPP = permutationPrefixEnumerator.Current;

                    while (permutationPrefixEnumerator.MoveNext())
                    {
                        var pp = permutationPrefixEnumerator.Current;
                        for (var i = 0; i < prefixLength; ++i)
                        {
                            if (prevPP.Data[i] == pp.Data[i])
                            {
                                ++ppGroupSizes[i];
                            }
                            else
                            {
                                mins[i] = Math.Min(ppGroupSizes[i], mins[i]);
                                maxs[i] = Math.Max(ppGroupSizes[i], maxs[i]);
                                sums[i] += Math.Pow(ppGroupSizes[i] - averages[i], 2.0);
                                ppGroupSizes[i] = 1;
                            }
                        }
                        prevPP = pp;
                    }

                    for (var i = 0; i < prefixLength; ++i)
                    {
                        mins[i] = Math.Min(ppGroupSizes[i], mins[i]);
                        maxs[i] = Math.Max(ppGroupSizes[i], maxs[i]);
                        sums[i] += Math.Pow(ppGroupSizes[i] - averages[i], 2.0);
                    }

                    for (int i = 0; i < prefixLength; ++i)
                    {
                        var stddev = Math.Sqrt(sums[i] / ppGroupCounts[i]);

                        Console.WriteLine("{0} {1} {2} {3} {4} {5}", i, ppGroupCounts[i], averages[i], mins[i], maxs[i], stddev);
                    }
                }
            }
            #endregion

            Console.Beep();
        }

        private static void PrintPrefixTree(PermutationPrefixTreeNode permutationPrefixTreeNode)
        {
            PrintPrefixTree(permutationPrefixTreeNode, 0);
        }

        private static void PrintPrefixTree(PermutationPrefixTreeNode permutationPrefixTreeNode, int depth)
        {
            if (permutationPrefixTreeNode.childs != null)
            {
                foreach (var child in permutationPrefixTreeNode.childs)
                {
                    PrintPrefixTree(child, depth + 1);
                }
            }
            else
            {
                Console.WriteLine("{0} {1}", permutationPrefixTreeNode.labels[1] - permutationPrefixTreeNode.labels[0] + 1, depth);
            }
        }
    }
}



