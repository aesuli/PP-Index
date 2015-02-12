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
    using Esuli.Base.IO.Storage;
    using Esuli.MiPai.Datatypes.BIC;
    using Esuli.MiPai.Datatypes.ImageThumbnail;
    using Esuli.MiPai.Index;
    using Esuli.MiPai.IO;

    public class MiPaiImageIndex : IDisposable
    {
        public static readonly string filenamesStorageSuffix = "_filenames";

        private ReadOnlyStorage<string> storage;
        private IPermutationPrefixIndex index;
        private object featureExtractor;
        private Type featureType;

        public MiPaiImageIndex(string indexLocation, string indexName)
        {
            storage = new ReadOnlyStorage<string>(indexName + filenamesStorageSuffix, indexLocation);
            featureType = PermutationPrefixGeneratorInfo.GetFeatureType(indexName, indexLocation);
            if (featureType == typeof(BICDescriptor))
            {
                index = new PermutationPrefixIndex<BICDescriptor>(indexName, indexLocation, 10);
                featureExtractor = new BICFeaturesExtractor();
            }
            else if (featureType == typeof(ImageThumbnail))
            {
                index = new PermutationPrefixIndex<ImageThumbnail>(indexName, indexLocation, 10);
                featureExtractor = new ImageThumbnailExtractor();
            }

        }

        public IPermutationPrefixIndex PPIndex
        {
            get
            {
                return index;
            }
        }

        public int LastAccessed
        {
            get
            {
                if (featureType == typeof(BICDescriptor))
                {
                    return (index as PermutationPrefixIndex<BICDescriptor>).LastAccessed;
                }
                else if (featureType == typeof(ImageThumbnail)) {
                    return (index as PermutationPrefixIndex<ImageThumbnail>).LastAccessed;
                }
                return -1;
            }
        }

        public IEnumerator<string> Search(Bitmap image, int resultsCount, int additionalQueries)
        {
            KeyValuePair<double, KeyValuePair<Int32, Int32>>[] results = null;
            if (featureType == typeof(BICDescriptor))
            {
                var features = (featureExtractor as BICFeaturesExtractor).ExtractFeatures(-1, image);
                results = (index as PermutationPrefixIndex<BICDescriptor>).Search(features[0], resultsCount, resultsCount * 10, resultsCount * 100, additionalQueries, PermutationType.TopDown, int.MaxValue);
            }
            else if (featureType == typeof(ImageThumbnail))
            {
                var features = (featureExtractor as ImageThumbnailExtractor).ExtractFeatures(-1, image);
                results = (index as PermutationPrefixIndex<ImageThumbnail>).Search(features[0], resultsCount, resultsCount * 10, resultsCount * 100, additionalQueries, PermutationType.TopDown, int.MaxValue);
            }
            for (int i = 0; i < results.Length; ++i)
            {
                yield return storage[results[i].Value.Value];
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
                storage.Dispose();
                (index as IDisposable).Dispose();
            }
        }
    }
}
