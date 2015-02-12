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
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;

    public class ImageThumbnailExtractor : IFeaturesExtractor<Bitmap, ImageThumbnail>
    {

        public ImageThumbnail[] ExtractFeatures(int id, Bitmap item)
        {
            using (Bitmap bitmap = new Bitmap(ImageThumbnail.ThumbnailSide, ImageThumbnail.ThumbnailSide, PixelFormat.Format24bppRgb))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(item, new Rectangle(new Point(), bitmap.Size), 0, 0, item.Width, item.Height, GraphicsUnit.Pixel);

                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, ImageThumbnail.ThumbnailSide, ImageThumbnail.ThumbnailSide), ImageLockMode.ReadWrite, bitmap.PixelFormat);

                Debug.Assert(bitmapData.Stride * bitmapData.Height == ImageThumbnail.DataLength);

                byte[] data = new byte[ImageThumbnail.DataLength];

                System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, data, 0, ImageThumbnail.DataLength);

                bitmap.UnlockBits(bitmapData);

                return new[] { new ImageThumbnail(id, data) };
            }
        }
    }
}
