#region License
//   Copyright 2019-2021 Kastellanos Nikolaos
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace tainicom.ProtonType.App.ViewModels
{
    class ObservableEnumerableAggregate<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        List<IEnumerable<T>> _groups = new List<IEnumerable<T>>();
        internal void AddAggregatedCollection(IEnumerable<T> enumerable)
        {
            ((INotifyCollectionChanged)enumerable).CollectionChanged += ObservableEnumerableAggregate_CollectionChanged;
            _groups.Add(enumerable);

            if (enumerable.Count() > 0)
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (IList)enumerable.ToList()));
        }

        void ObservableEnumerableAggregate_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(e);
        }
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler == null) return;
            handler(this, e);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var group in _groups)
            {
                foreach (var item in group)
                {
                    yield return item;
                }
            }

            yield return default(T);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var group in _groups)
            {
                foreach (var item in group)
                {
                    yield return item;
                }
            }

            yield return default(T);
        }

    }
}
