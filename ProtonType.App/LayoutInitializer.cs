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

using System.Linq;
using AvalonDock.Layout;
using tainicom.ProtonType.Framework.Attributes;

namespace tainicom.ProtonType.App
{
    class LayoutInitializer : ILayoutUpdateStrategy
    {
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            //AD wants to add the anchorable into destinationContainer
            //just for test provide a new anchorablepane 
            //if the pane is floating let the manager go ahead
            LayoutAnchorablePane destPane = destinationContainer as LayoutAnchorablePane;
            if (destinationContainer != null &&
                destinationContainer.FindParent<LayoutFloatingWindow>() != null)
                return false;

            if (anchorableToShow.Content != null)
            {
                var itemType = anchorableToShow.Content.GetType();
                var defaultViewAttributes = itemType.GetCustomAttributes(typeof(DefaultViewAttribute), true);
                foreach (DefaultViewAttribute defaultViewAttribute in defaultViewAttributes)
                {
                    string defaultLayoutAnchorablePaneName = defaultViewAttribute.DefaultLayoutAnchorablePaneName;
                    LayoutAnchorablePane layoutAnchorablePane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name.Equals(defaultLayoutAnchorablePaneName));
                    if (layoutAnchorablePane != null)
                    {
                        layoutAnchorablePane.Children.Add(anchorableToShow);
                        return true;
                    }
                }
            }

            var rightPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == "rightPane");

            if (rightPane != null)
            {
                rightPane.Children.Add(anchorableToShow);
                return true;
            }

            return false;

        }


        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
            layout.ActiveContent = anchorableShown;
        }


        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
            layout.ActiveContent = anchorableShown;
        }
    }
}
