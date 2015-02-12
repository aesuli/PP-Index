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

namespace Esuli.MiPai.IO
{
    using System;
    using System.IO;
    using Esuli.Base;
    using Esuli.Base.IO;
    using Esuli.MiPai.Index;
    using Esuli.MiPai.Indexing;

    public class PermutationPrefixTreeSerialization<Tfeature>
        where Tfeature : IIntId
    {
        public static readonly int leafNode = 0;
        public static readonly int internalNode = 1;

        private static readonly string indexFileExtension = ".ptr";

        public static void Write(PermutationPrefixTree<Tfeature> tree, string name, string location)
        {
            using (var stream = new FileStream(location + Path.DirectorySeparatorChar + name + "_" + tree.PruneLevel + indexFileExtension, FileMode.Create, FileAccess.Write, FileShare.None, Esuli.Base.Constants.OneMebi))
            {
                WriteTreeHeader(tree.Depth,tree.HitCount,tree.PruneLevel,stream);
                WriteNode(tree.Root, stream);
            }
        }

        public static void WriteTreeHeader(int depth, int hitCount, int pruneLevel,Stream stream)
        {
            VariableByteCoding.Write(depth, stream);
            VariableByteCoding.Write(hitCount, stream);
            VariableByteCoding.Write(pruneLevel, stream);
        }

        private static void WriteNode(PermutationPrefixTreeNode node, Stream stream)
        {
            PermutationPrefixTreeNode[] nodes = node.childs;
            if (nodes == null)
            {
                WriteNodeLabels(node.labels, leafNode, stream);
            }
            else
            {
                WriteNodeLabels(node.labels, internalNode, stream);
                for (int i = 0; i < nodes.Length; ++i)
                {
                    WriteNode(nodes[i], stream);
                }
            }
        }

        public static void WriteNodeLabels(int[] labels,int leafFlag, Stream stream)
        {
            VariableByteCoding.Write(labels.Length, stream);
            VariableByteCoding.Write(labels[0], stream);
            for (int i = 1; i < labels.Length; ++i)
            {
                VariableByteCoding.Write(labels[i] - labels[i - 1], stream);
            }
            VariableByteCoding.Write(leafFlag, stream);
        }

        public static PermutationPrefixTree<Tfeature> Read(string name, string location, int pruneLevel)
        {
            PermutationPrefixGenerator<Tfeature> generator = PermutationPrefixGeneratorSerialization<Tfeature>.Read(name, location);
            var prunedTreeFilename = location + Path.DirectorySeparatorChar + name + "_" + pruneLevel + indexFileExtension;
            if (!File.Exists(prunedTreeFilename))
            {
                using (var stream = new FileStream(location + Path.DirectorySeparatorChar + name + Constants.PermutationPrefixStreamSuffix, FileMode.Open, FileAccess.Read, FileShare.None, Esuli.Base.Constants.OneMebi, FileOptions.SequentialScan))
                {
                    var permutationPrefixEnumerator = new StreamEnumerator<PermutationPrefix>(stream, new PermutationPrefixSerialization());
                    var permutationPrefixTree = new PermutationPrefixTree<Tfeature>(permutationPrefixEnumerator, generator, pruneLevel);
                    PermutationPrefixTreeSerialization<Tfeature>.Write(permutationPrefixTree, name, location);
                }
            }
            using (var stream = new FileStream(prunedTreeFilename, FileMode.Open, FileAccess.Read, FileShare.Read, Esuli.Base.Constants.OneMebi))
            {
                int depth = (int)VariableByteCoding.Read(stream);
                int hitCount = (int)VariableByteCoding.Read(stream);
                int treePruneLevel = (int)VariableByteCoding.Read(stream);
                if (treePruneLevel != pruneLevel)
                {
                    throw new Exception("Prune level (" + treePruneLevel + ") stored in file " + prunedTreeFilename + " does not match the file name.");
                }
                PermutationPrefixTreeNode wordTree = ReadNode(stream);
                return new PermutationPrefixTree<Tfeature>(generator, depth, wordTree, hitCount, pruneLevel);
            }
        }

        private static PermutationPrefixTreeNode ReadNode(Stream stream)
        {
            int length = (int)VariableByteCoding.Read(stream);
            int[] labels = new int[length];
            labels[0] = (int)VariableByteCoding.Read(stream);
            for (int i = 1; i < length; ++i)
            {
                labels[i] = (int)VariableByteCoding.Read(stream) + labels[i - 1];
            }
            int hasChilds = (int)VariableByteCoding.Read(stream);
            PermutationPrefixTreeNode[] nodes = null;
            if (hasChilds != 0)
            {
                nodes = new PermutationPrefixTreeNode[length];
                for (int i = 0; i < length; ++i)
                {
                    nodes[i] = ReadNode(stream);
                }
            }
            return new PermutationPrefixTreeNode(labels, nodes);
        }
    }
}
