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
    using System.IO;
    using Esuli.Base.IO;
    using Esuli.MiPai.DistanceFunctions;
    using Esuli.MiPai.Indexing;

    public class PermutationPrefixGeneratorSerialization<Tfeature>
    {
        public static void Write(PermutationPrefixGenerator<Tfeature> permutationPrefixGenerator, string name, string location, IFixedSizeObjectSerialization<Tfeature> featureSerialization)
        {
            using (var similarityStream = new FileStream(location + Path.DirectorySeparatorChar + name + Constants.PermutationPrefixGeneratorFileExtension, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                TypeSerialization.Write(typeof(Tfeature), similarityStream);
                TypeSerialization.Write(permutationPrefixGenerator.DistanceFunction.GetType(), similarityStream);
                TypeSerialization.Write(featureSerialization.GetType(), similarityStream);
                Tfeature[] referencePoints = permutationPrefixGenerator.ReferencePointsFeatures;
                VariableByteCoding.Write(referencePoints.Length, similarityStream);
                for (int i = 0; i < referencePoints.Length; ++i)
                {
                    featureSerialization.Write(referencePoints[i], similarityStream);
                }
            }
        }

        public static PermutationPrefixGenerator<Tfeature> Read(string name, string location)
        {
            using (Stream similarityStream = new FileStream(location + Path.DirectorySeparatorChar + name + Constants.PermutationPrefixGeneratorFileExtension, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                TypeSerialization.Skip(similarityStream);
                IDistanceFunction<Tfeature> distanceFunction = TypeSerialization.CreateInstance<IDistanceFunction<Tfeature>>(similarityStream);
                IFixedSizeObjectSerialization<Tfeature> featureSerialization = TypeSerialization.CreateInstance<IFixedSizeObjectSerialization<Tfeature>>(similarityStream);
                long referencePointsCount = VariableByteCoding.Read(similarityStream);
                Tfeature[] referencePoints = new Tfeature[referencePointsCount];
                for (int i = 0; i < referencePointsCount; ++i)
                {
                    referencePoints[i] = featureSerialization.Read(similarityStream);
                }
                return new PermutationPrefixGenerator<Tfeature>(distanceFunction, referencePoints);
            }
        }
    }
}
