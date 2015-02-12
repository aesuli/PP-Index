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

namespace Esuli.Scheggia.Enumerators
{
    using System;
    using System.Collections.Generic;
    using Esuli.Scheggia.Core;
    
    public class HitListEnumerator 
        : IHitEnumerator
    {
        private List<KeyValuePair<int,KeyValuePair<Type,object>>> hitList;
        private int pos;

        public HitListEnumerator(List<KeyValuePair<int, KeyValuePair<Type, object>>> hitList)
        {
            this.hitList = hitList;
            pos = -1;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        
        public int Count
        {
            get
            {
                return hitList.Count;
            }
        }

        public int Progress
        {
            get
            {
                return pos;
            }
        }

        public int CurrentEnumeratorId
        {
            get
            {
                return hitList[pos].Key;
            }
        }

        public Type HitType
        {
            get
            {
                return hitList[pos].Value.Key;
            }
        }

        public object CurrentHit
        {
            get
            {
                return hitList[pos].Value.Value;
            }
        }

        public bool MoveNext()
        {
            ++pos;
            return pos < hitList.Count;
        }
    }
}
