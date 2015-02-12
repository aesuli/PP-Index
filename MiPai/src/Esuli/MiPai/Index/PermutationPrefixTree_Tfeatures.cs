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
    using Esuli.Base.Comparers;
    using Esuli.MiPai.Indexing;

    public class PermutationPrefixTree<Tfeatures>
       : IPermutationPrefixTree<Tfeatures>
    {
        private PermutationPrefixGenerator<Tfeatures> permutationPrefixGenerator;
        private int pruneLevel;
        private int depth;
        private int hitCount;
        private PermutationPrefixTreeNode root;

        public PermutationPrefixTree(IEnumerator<PermutationPrefix> enumerator, PermutationPrefixGenerator<Tfeatures> generator, int pruneLevel)
        {
            this.pruneLevel = pruneLevel;
            int counter = 0;
            permutationPrefixGenerator = generator;
            root = new PermutationPrefixTreeNode(false);

            int[] data;
            int localDepth = 0;
            int leafLevel;
            while (enumerator.MoveNext())
            {
                var prefix = enumerator.Current;
                data = prefix.Data;
                localDepth = data.Length;
                leafLevel = localDepth - 1;

                var node = root;
                for (int i = 0; i < localDepth; ++i)
                {
                    int labelLength = node.labels.Length;
                    bool missingChild = false;
                    if (labelLength == 0)
                    {
                        missingChild = true;
                    }
                    else
                    {
                        if (node.labels[labelLength - 1] == data[i])
                        {
                            node = node.childs[labelLength - 1];
                        }
                        else
                        {
                            missingChild = true;
                        }
                    }

                    if (missingChild)
                    {
                        if (labelLength != 0)
                        {
                            var leftMostNode = node.childs[labelLength - 1];
                            while (true)
                            {
                                if (leftMostNode.childs == null)
                                {
                                    break;
                                }
                                leftMostNode = leftMostNode.childs[0];
                            }
                            var rightMostNode = node.childs[labelLength - 1];
                            while (true)
                            {
                                if (rightMostNode.childs == null)
                                {
                                    break;
                                }
                                rightMostNode = rightMostNode.childs[rightMostNode.childs.Length - 1];
                            }
                            int range = rightMostNode.labels[1] - leftMostNode.labels[0] + 1;
                            if (range < pruneLevel)
                            {
                                var prunedNode = new PermutationPrefixTreeNode(true);
                                prunedNode.labels = new int[] { leftMostNode.labels[0], rightMostNode.labels[1] };
                                node.childs[labelLength - 1] = prunedNode;
                            }
                        }
                        Array.Resize<int>(ref node.labels, labelLength + 1);
                        Array.Resize<PermutationPrefixTreeNode>(ref node.childs, labelLength + 1);
                        node.labels[labelLength] = data[i];
                        var childNode = new PermutationPrefixTreeNode((i == leafLevel));
                        node.childs[labelLength] = childNode;
                        node = childNode;

                    }

                    if (i == leafLevel)
                    {
                        if (missingChild)
                        {
                            node.labels = new int[] { counter, counter };
                        }
                        else
                        {
                            node.labels[1] = counter;
                        }
                    }
                }
                ++counter;
            }

            depth = localDepth;
            hitCount = counter;
        }

        public PermutationPrefixTree(PermutationPrefixGenerator<Tfeatures> permutationPrefixGenerator, int depth, PermutationPrefixTreeNode root, int hitCount, int pruneLevel)
        {
            this.permutationPrefixGenerator = permutationPrefixGenerator;
            this.pruneLevel = pruneLevel;
            this.depth = depth;
            this.hitCount = hitCount;
            this.root = root;
        }

        public PermutationPrefixGenerator<Tfeatures> PermutationPrefixGenerator
        {
            get
            {
                return permutationPrefixGenerator;
            }
        }

        public int PruneLevel
        {
            get
            {
                return pruneLevel;
            }
        }
        
        public int Depth
        {
            get
            {
                return depth;
            }
        }

        public int HitCount
        {
            get
            {
                return hitCount;
            }
        }

        public PermutationPrefixTreeNode Root
        {
            get
            {
                return root;
            }
        }

        static int[][][] topDownPermutationPatterns = new int[][][] { 
            new int[][] { new int[] { 0, 1 } },

            new int[][] { new int[] { 1, 2 } },
            new int[][] { new int[] { 0, 1 },new int[] { 1, 2 } },
            new int[][] { new int[] { 0, 2 } },
            new int[][] { new int[] { 0, 1 },new int[] { 0, 2 } },

            new int[][] { new int[] { 2, 3 } },
            new int[][] { new int[] { 0, 1 }, new int[] { 2, 3 } },
            new int[][] { new int[] { 1, 2 }, new int[] { 2, 3 } },
            new int[][] { new int[] { 0, 1 },new int[] { 1, 2 }, new int[] { 2, 3 } },
            new int[][] { new int[] { 0, 2 }, new int[] { 2, 3 } },
            new int[][] { new int[] { 0, 1 },new int[] { 0, 2 }, new int[] { 2, 3 } },
        };

        public List<KeyValuePair<int, int>> SearchRanges(Tfeatures query, int minCount, int maxCount,int permutationCount, PermutationType permutationType)
        {
            double [] distances;
            PermutationPrefix word = permutationPrefixGenerator.GetPermutationPrefix(0, query, depth, out distances);
            int[] data = word.Data;

            List<KeyValuePair<int, int>> wordRanges = new List<KeyValuePair<int, int>>();

            wordRanges.AddRange(SearchRanges(data, minCount, maxCount));

            if (permutationType == PermutationType.TopDown)
            {
                permutationCount = Math.Min(permutationCount, topDownPermutationPatterns.Length);
                int [] tempData = new int[data.Length];
                for (int i = 0; i < permutationCount; ++i)
                {
                    data.CopyTo(tempData, 0);
                    int[][] permutationList = topDownPermutationPatterns[i];
                    for (int j = 0; j < permutationList.Length; ++j)
                    {
                        try
                        {
                            int first = permutationList[j][0];
                            int second = permutationList[j][1];
                            int temp = tempData[first];
                            tempData[first] = tempData[second];
                            tempData[second] = temp;
                            wordRanges.AddRange(SearchRanges(tempData, minCount, maxCount));
                        }
                        catch { }
                    }
                }
            }
            else if (permutationType == PermutationType.Random)
            {
                Random random = new Random(query.GetHashCode());
                int dataLength = data.Length - 1;
                for (int i = 0; i < permutationCount; ++i)
                {
                    int swapPosition = random.Next(dataLength);
                    int temp = data[swapPosition];
                    data[swapPosition] = data[swapPosition + 1];
                    data[swapPosition + 1] = temp;
                    wordRanges.AddRange(SearchRanges(data, minCount, maxCount));
                    data[swapPosition + 1] = data[swapPosition];
                    data[swapPosition] = temp;
                }
            }
            else if (permutationType == PermutationType.ClosestPair)
            {
                var sortedPairs = new List<KeyValuePair<double, KeyValuePair<int, int>>>();
                for (int i = 0; i < data.Length - 1; ++i)
                {
                    for (int j = i + 1; j < data.Length; ++j)
                    {
                        sortedPairs.Add(new KeyValuePair<double, KeyValuePair<int, int>>(distances[j] - distances[i], new KeyValuePair<int, int>(i, j)));
                    }
                }
                sortedPairs.Sort(new KeyValuePairKeyComparer<double, KeyValuePair<int, int>>());

                var tempData = new int[data.Length];
                for (int i = 0; i < permutationCount; ++i)
                {
                    data.CopyTo(tempData, 0);
                    var pair = sortedPairs[i].Value;
                    int first = pair.Key;
                    int second = pair.Value;
                    int temp = tempData[first];
                    tempData[first] = tempData[second];
                    tempData[second] = temp;
                    wordRanges.AddRange(SearchRanges(tempData, minCount, maxCount));
                }
            }

            if (wordRanges.Count > 1)
            {
                wordRanges.Sort(KeyValuePairKeyComparer<int,int>.CompareDelegate);
                int i = 1;
                while (i<wordRanges.Count)
                {
                    KeyValuePair<int, int> prev = wordRanges[i - 1];
                    KeyValuePair<int, int> curr = wordRanges[i];
                    if (prev.Value < (curr.Key - 1))
                    {
                        ++i;
                    }
                    else
                    {
                        if (curr.Value > prev.Value)
                        {
                            wordRanges[i - 1] = new KeyValuePair<int, int>(prev.Key, curr.Value);
                        }
                        wordRanges.RemoveAt(i);
                    }
                }
            }

            return wordRanges;
        }

        private List<KeyValuePair<int, int>> SearchRanges(int [] data, int minCount, int maxCount) {
            List<KeyValuePair<int, int>> wordRanges = new List<KeyValuePair<int, int>>();
            PermutationPrefixTreeNode node;

            int wordSize = 0;

            for (int i = depth; i > 0; --i)
            {
                node = FindMatch(data, root, i);

                if (node != null)
                {
                    PermutationPrefixTreeNode tempNode = node;
                    while (tempNode.childs != null)
                    {
                        tempNode = tempNode.childs[0];
                    }
                    int minIdPos = tempNode.labels[0];
                    tempNode = node;
                    while (tempNode.childs != null)
                    {
                        tempNode = tempNode.childs[tempNode.childs.Length - 1];
                    }
                    int maxIdPos = tempNode.labels[tempNode.labels.Length - 1];

                    wordSize = maxIdPos - minIdPos + 1;

                    if (wordSize > maxCount && wordRanges.Count > 0)
                    {
                        return wordRanges;
                    }

                    wordRanges.Clear();
                    wordRanges.Add(new KeyValuePair<int, int>(minIdPos, maxIdPos));

                    if (wordSize >= minCount)
                    {
                        return wordRanges;
                    }
                }
            }

            int[] onePoint = new int[1];
            for (int i = 1; i < data.Length; ++i)
            {
                onePoint[0] = data[i];
                node = FindMatch(onePoint, root, 1);

                if (node != null)
                {
                    PermutationPrefixTreeNode tempNode = node;
                    while (tempNode.childs != null)
                    {
                        tempNode = tempNode.childs[0];
                    }
                    int minIdPos = tempNode.labels[0];
                    tempNode = node;
                    while (tempNode.childs != null)
                    {
                        tempNode = tempNode.childs[tempNode.childs.Length - 1];
                    }
                    int maxIdPos = tempNode.labels[tempNode.labels.Length - 1];
                    int thisWordSize = maxIdPos - minIdPos + 1;
                    if (wordSize + thisWordSize > maxCount && wordRanges.Count > 0)
                    {
                        return wordRanges;
                    }
                    wordRanges.Add(new KeyValuePair<int, int>(minIdPos, maxIdPos));
                    wordSize += thisWordSize;
                }

                if (wordSize >= minCount)
                {
                    break;
                }
            }

            wordRanges.Sort(KeyValuePairKeyComparer<int, int>.CompareDelegate);
            return wordRanges;
        }

        private PermutationPrefixTreeNode FindMatch(int[] data, PermutationPrefixTreeNode node, int maxDepth)
        {
            for (int i = 0; i < maxDepth; ++i)
            {
                if (node.childs == null)
                {
                    return null;
                }
                int label = data[i];
                if (node.childs.Length == permutationPrefixGenerator.ReferencePointsFeatures.Length)
                {
                    node = node.childs[label];
                }
                else
                {
                    int pos = Array.BinarySearch<int>(node.labels, label);
                    if (pos < 0)
                    {
                        return null;
                    }
                    else
                    {
                        node = node.childs[pos];
                    }
                }
            }
            return node;
        }
    }
}
