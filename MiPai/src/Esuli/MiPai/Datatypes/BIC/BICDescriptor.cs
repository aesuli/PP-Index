﻿// Copyright (C) 2013 Andrea Esuli
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
    using Esuli.Base;

    public class BICDescriptor : IComparable<BICDescriptor>, IIntId
    {
        public static readonly int DataSize = 64;

        private int id;
        private byte [] data;

        public BICDescriptor(int id, byte[] data)
        {
            this.id = id;
            this.data = data;
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public byte[] Data
        {
            get
            {
                return data;
            }
        }

        public int CompareTo(BICDescriptor other)
        {
            return id - other.id;
        }

    }
}
