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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using AvalonDock.Layout;
using tainicom.ProtonType.Framework.Attributes;

namespace tainicom.ProtonType.App
{
    class PanesTemplateSelector: DataTemplateSelector
    {
        public PanesTemplateSelector()
        {
        }

        //Libraries
        public DataTemplate LibraryViewTemplate { get; set; }

        Dictionary<Type, Type> templates = new Dictionary<Type, Type>();

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            LayoutContent itemAsLayoutContent = item as LayoutContent;
            var itemType = item.GetType();

            var defaultViewAttributes = itemType.GetCustomAttributes(typeof(DefaultViewAttribute), true);
            foreach (DefaultViewAttribute defaultViewAttribute in defaultViewAttributes )
            {
                var defaultViewType = defaultViewAttribute.DefaultViewType;
                return CreateDataTemplate(defaultViewType);
            }

            foreach (Type viewModelType in templates.Keys)
            {
                if (viewModelType == itemType)
                {
                    Type viewType = templates[viewModelType];
                    return CreateDataTemplate(viewType);
                }
            }

            return base.SelectTemplate(item, container);
        }

        internal void AddTemplate(Type viewModelType, Type viewType)
        {
            if (templates.ContainsKey(viewModelType)) return;
            templates.Add(viewModelType, viewType);
        }

        public DataTemplate CreateDataTemplate(Type type)        
        {
            DataTemplate template = new DataTemplate();
            template.DataType = type;
            FrameworkElementFactory spFactory = new FrameworkElementFactory(type);
            spFactory.Name = type.Name;
            //set the visual tree of the data template
            template.VisualTree = spFactory;
            return template;
        }

    }
}
