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

namespace Esuli.MiPai.Datatypes.BIC
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using Esuli.MiPai.Indexing;

    public class BICFeaturesExtractor : IFeaturesExtractor<Bitmap, BICDescriptor>
    {
        private static readonly int imageResizeSide = 128;
        private static readonly int size = imageResizeSide * imageResizeSide * 3;
        public static readonly byte[] DLogMapping = { 0, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 };

        public BICDescriptor[] ExtractFeatures(int id, Bitmap item)
        {
            var data = new byte[size];

            using (var bitmap = new Bitmap(imageResizeSide, imageResizeSide, PixelFormat.Format24bppRgb))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(item, new Rectangle(new Point(), bitmap.Size), 0, 0, item.Width, item.Height, GraphicsUnit.Pixel);

                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, imageResizeSide, imageResizeSide), ImageLockMode.ReadWrite, bitmap.PixelFormat);

                System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, data, 0, size);

                bitmap.UnlockBits(bitmapData);
            }

            for (int i = 0; i < size; ++i)
            {
                data[i] = (byte)(data[i] >> 6);
            }

            var histogram = new int[128];
            int top = -imageResizeSide * 3;
            int left = -3;
            int examined = 0;
            int right = 3;
            int bottom = imageResizeSide * 3;
            var internalCount = 0;
            var borderCount = 0;
            while (true)
            {
                var valueExamined = data[examined] + (data[examined + 1] << 2) + (data[examined + 2] << 4);

                var valueTop = valueExamined;
                if (top >= 0)
                {
                    valueTop = data[top] + (data[top + 1] << 2) + (data[top + 2] << 4);
                }

                var valueRight = valueExamined;
                if (right % (imageResizeSide * 3 )!= 0)
                {
                    valueRight = data[right] + (data[right + 1] << 2) + (data[right + 2] << 4);
                }

                var valueLeft = valueExamined;
                if ((left + (imageResizeSide * 3)) % (imageResizeSide * 3) != (imageResizeSide - 1) * 3)
                {
                    valueLeft = data[left] + (data[left + 1] << 2) + (data[left + 2] << 4);
                }

                var valueBottom = valueExamined;
                if (bottom < size)
                {
                    valueBottom = data[bottom] + (data[bottom + 1] << 2) + (data[bottom + 2] << 4);
                }

                if (valueExamined == valueBottom &&
                    valueExamined == valueTop &&
                    valueExamined == valueRight &&
                    valueExamined == valueLeft)
                {
                    ++histogram[valueExamined];
                    ++internalCount;
                }
                else
                {
                    ++histogram[valueExamined + 64];
                    ++borderCount;
                }

                examined += 3;
                left += 3;
                right += 3;
                top += 3;
                bottom += 3;
                if (right > size)
                {
                    break;
                }
            }

            Debug.Assert(internalCount + borderCount == imageResizeSide * imageResizeSide);

            var bic = new byte[64];
            for (int i = 0; i < 64; ++i)
            {
                bic[i] = (byte)(DLogMapping[(histogram[i] * 255) / internalCount] | (DLogMapping[(histogram[i + 64] * 255) / borderCount] << 4));
            }

            return new[] { new BICDescriptor(id, bic) };
        }
    }
}
