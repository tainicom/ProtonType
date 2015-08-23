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

using ProtonType.Framework.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ProtonType.Framework
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle { get; set; }
        public Style DocumentStyle { get; set; }

        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (item is ToolViewModel)
                return ToolStyle;

            if (item is DocumentViewModel)
                return DocumentStyle;

            return base.SelectStyle(item, container);
        }
    }
}
