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

namespace Esuli.MiPai.Datatypes.CoPhIR
{
    using System;
    using System.Text;
    using System.Xml;
    using Esuli.Base;
    using Esuli.MiPai.Datatypes.MPEG7;

    [Serializable]
    public class CoPhIRData
      : IIntId, IComparable<CoPhIRData>
    {
        public enum FlickrSize
        {
            Square,
            Thumbnail,
            Small,
            Medium,
            Big,
            Original,
        }

        private int id;
        private int farmId;
        private int serverId;
        private long secret;

        private ColorLayout colorLayout;
        private ColorStructure colorStructure;
        private EdgeHistogram edgeHistogram;
        private HomogeneousTexture homogeneousTexture;
        private ScalableColor scalableColor;

        public CoPhIRData(XmlReader reader, ISelector selector)
        {
            colorLayout = null;
            colorStructure = null;
            edgeHistogram = null;
            homogeneousTexture = null;
            scalableColor = null;

            string idString = reader["id"];
            if (idString != null)
            {
                id = int.Parse(idString);
            }
            else
            {
                id = -1;
            }

            bool stop = false;
            while (!stop && reader.Read())
            {
                switch (reader.NodeType)
                {
                    case (XmlNodeType.Element):
                        if (reader.Name == "VisualDescriptor")
                        {
                            string type = reader["xsi:type"];
                            if (type == null)
                            {
                                type = reader["type"];
                            }
                            switch (type)
                            {
                                case ("ColorLayoutType"):
                                    if (selector.UseColorLayout)
                                    {
                                        colorLayout = new ColorLayout(reader);
                                    }
                                    break;
                                case ("ColorStructureType"):
                                    if (selector.UseColorStructure)
                                    {
                                        colorStructure = new ColorStructure(reader);
                                    }
                                    break;
                                case ("EdgeHistogramType"):
                                    if (selector.UseEdgeHistogram)
                                    {
                                        edgeHistogram = new EdgeHistogram(reader);
                                    }
                                    break;
                                case ("HomogeneousTextureType"):
                                    if (selector.UseHomogeneousTexture)
                                    {
                                        homogeneousTexture = new HomogeneousTexture(reader);
                                    }
                                    break;
                                case ("ScalableColorType"):
                                    if (selector.UseScalableColor)
                                    {
                                        scalableColor = new ScalableColor(reader);
                                    }
                                    break;
                            }
                        }
                        break;
                    case (XmlNodeType.EndElement):
                        if (reader.Name == "Image")
                        {
                            stop = true;
                        }
                        break;
                }
            }
        }

        public CoPhIRData(int id, int farmId, int serverId, long secret, ColorLayout colorLayout, ColorStructure colorStructure, EdgeHistogram edgeHistogram, HomogeneousTexture homogeneousTexture, ScalableColor scalableColor)
        {
            this.id = id;
            this.farmId = farmId;
            this.serverId = serverId;
            this.secret = secret;

            this.colorLayout = colorLayout;
            this.colorStructure = colorStructure;
            this.edgeHistogram = edgeHistogram;
            this.homogeneousTexture = homogeneousTexture;
            this.scalableColor = scalableColor;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder("Image");
            str.Append("\n id: ");
            str.Append(id);
            str.Append(" farmId: ");
            str.Append(farmId);
            str.Append(" serverId: ");
            str.Append(serverId);
            str.Append(" secret: ");
            str.Append(secret);
            str.Append(" \n");
            str.Append(colorLayout);
            str.Append(colorStructure);
            str.Append(edgeHistogram);
            str.Append(homogeneousTexture);
            str.Append(scalableColor);
            return str.ToString();
        }

        public String ToURL(FlickrSize size)
        {
            StringBuilder builder = new StringBuilder("http://farm", 80);
            builder.Append(farmId);
            builder.Append(".static.flickr.com/");
            builder.Append(serverId);
            builder.Append("/");
            builder.Append(id);
            builder.Append("_");
            builder.Append(secret.ToString("x10"));
            switch (size)
            {
                case (FlickrSize.Square):
                    builder.Append("_s");
                    break;
                case (FlickrSize.Thumbnail):
                    builder.Append("_t");
                    break;
                case (FlickrSize.Small):
                    builder.Append("_m");
                    break;
                case (FlickrSize.Big):
                    builder.Append("_b");
                    break;
                case (FlickrSize.Original):
                    builder.Append("_o");
                    break;
            }
            builder.Append(".jpg");

            return builder.ToString();
        }

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public int FarmId
        {
            get
            {
                return farmId;
            }
            set
            {
                farmId = value;
            }
        }

        public int ServerId
        {
            get
            {
                return serverId;
            }
            set
            {
                serverId = value;
            }
        }

        public long Secret
        {
            get
            {
                return secret;
            }
            set
            {
                secret = value;
            }
        }

        public ColorLayout ColorLayout
        {
            get
            {
                return colorLayout;
            }
        }

        public ColorStructure ColorStructure
        {
            get
            {
                return colorStructure;
            }
        }

        public EdgeHistogram EdgeHistogram
        {
            get
            {
                return edgeHistogram;
            }
        }

        public HomogeneousTexture HomogeneousTexture
        {
            get
            {
                return homogeneousTexture;
            }
        }

        public ScalableColor ScalableColor
        {
            get
            {
                return scalableColor;
            }
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public int CompareTo(CoPhIRData other)
        {
            return id - other.id;
        }
    }
}
