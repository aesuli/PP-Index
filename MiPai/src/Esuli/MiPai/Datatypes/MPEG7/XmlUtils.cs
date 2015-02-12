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

namespace Esuli.MiPai.Datatypes.MPEG7
{
    using System;

    public class XmlUtils
    {
        public static readonly char [] splitChars = new char [] {' ','\"'};

        public static byte[] ReadByteArray(string value)
        {
            string[] values = value.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            byte[] array = new byte[values.Length];
            for (int i = 0; i < values.Length; ++i)
            {
                array[i] = byte.Parse(values[i]);
            }
            return array;
        }

        public static short[] ReadShortArray(string value)
        {
            string[] values = value.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            short[] array = new short[values.Length];
            for (int i = 0; i < values.Length; ++i)
            {
                array[i] = short.Parse(values[i]);
            }
            return array;
        }

        internal static sbyte[] ReadSByteArray(string value)
        {
            string[] values = value.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            sbyte[] array = new sbyte[values.Length];
            for (int i = 0; i < values.Length; ++i)
            {
                array[i] = sbyte.Parse(values[i]);
            }
            return array;
        }
    }
}
