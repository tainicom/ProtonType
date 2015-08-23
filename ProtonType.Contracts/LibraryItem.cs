#region License
//   Copyright 2015 Kastellanos Nikolaos
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
#endregion

using System;

namespace tainicom.ProtonType.Contracts
{
    public class LibraryItem : LibraryItemDescription
    {
        public Type Type;
        public Guid GUID;
        public LibraryItem(string name, Type type)
            : base(name)
        {
            this.Type = type;
        }
        public LibraryItem(string name)
            : base(name)
        {
            this.GUID = Guid.NewGuid();
        }

        public override string ToString()
        {
            return string.Format("LibraryItem '{0}', Type={1}, GUID={2}", Name, Type.Name, GUID);
        }
    }
}
