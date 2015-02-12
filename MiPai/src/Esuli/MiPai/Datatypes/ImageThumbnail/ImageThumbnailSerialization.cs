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
    using System;
    using System.IO;
    using Esuli.Base.IO;

    public class ImageThumbnailSerialization : IFixedSizeObjectSerialization<ImageThumbnail>
    {
        public ImageThumbnailSerialization()
        {
        }

        public void Write(ImageThumbnail referencePoint, Stream stream)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            bw.Write(referencePoint.Id);
            bw.Write(referencePoint.Data);
        }

        public ImageThumbnail Read(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            int id = br.ReadInt32();
            byte[] data = br.ReadBytes(ImageThumbnail.DataLength);
            return new ImageThumbnail(id, data);
        }

        public ImageThumbnail Read(byte[] buffer, ref long position)
        {
            int id = BitConverter.ToInt32(buffer, (int)position);
            position += sizeof(Int32);
            byte[] data = new byte[ImageThumbnail.DataLength];
            Array.Copy(buffer, position, data, 0, ImageThumbnail.DataLength);
            return new ImageThumbnail(id, data);
        }

        public int ObjectSize
        {
            get
            {
                return ImageThumbnail.DataLength + sizeof(Int32);
            }
        }
    }
}
