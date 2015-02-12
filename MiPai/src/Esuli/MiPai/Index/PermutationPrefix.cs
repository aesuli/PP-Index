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
    using System.Text;

    public class PermutationPrefix : IComparable<PermutationPrefix>
    {
        protected int [] data;

        protected PermutationPrefix()
        {
        }

        public PermutationPrefix(int[] data)
        {
            this.data = data;
        }

        public PermutationPrefix(PermutationPrefix referenceWord, int size)
        {
            int[] wordData = referenceWord.Data;
            data = new int[size];
            for (int i = 0; i < size; ++i)
            {
                data[i] = wordData[i];
            }
        }

        public PermutationPrefix(PermutationPrefix referenceWord) 
            : this(referenceWord,referenceWord.Data.Length)
        {
        }

        public int[] Data
        {
            get
            {
                return data;
            }
        }

        public int CompareTo(PermutationPrefix other)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                int delta = data[i] - other.data[i];
                if (delta != 0)
                {
                    return delta;
                }
            }
            return 0;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; ++i)
            {
                stringBuilder.Append(data[i]);
                stringBuilder.Append(" ");
            }
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Length = stringBuilder.Length - 1;
            }
            return stringBuilder.ToString();
        }
    }
}
