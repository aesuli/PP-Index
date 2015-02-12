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

namespace Esuli.Base.Collections
{
    using System.Collections.Generic;

    public class KeyValuePairKeyReverseExternalComparer<Tkey, Tvalue> : IComparer<KeyValuePair<Tkey, Tvalue>>
    {
        private IComparer<Tkey> comparer;

        public KeyValuePairKeyReverseExternalComparer(IComparer<Tkey> comparer)
        {
            this.comparer = comparer;
        }

        public int Compare(KeyValuePair<Tkey, Tvalue> x, KeyValuePair<Tkey, Tvalue> y)
        {
            return comparer.Compare(y.Key, x.Key);
        }
    }
}
