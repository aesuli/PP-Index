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

namespace Esuli.MiPai.Datatypes.ImageThumbnail
{
    using System.Drawing;
    using Esuli.MiPai.Datatypes;

    public class ImageThumbnailFromFilenameExtractor : IFeaturesExtractor<string, ImageThumbnail>
    {
        private ImageThumbnailExtractor featuresExtractor;

        public ImageThumbnailFromFilenameExtractor()
        {
            featuresExtractor = new ImageThumbnailExtractor();
        }

        public ImageThumbnail[] ExtractFeatures(int id, string item)
        {
            ImageThumbnail[] features = null;
            using (var bitmap = new Bitmap(item))
            {
                features = featuresExtractor.ExtractFeatures(id, bitmap);
            }
            return features;
        }
    }
}
