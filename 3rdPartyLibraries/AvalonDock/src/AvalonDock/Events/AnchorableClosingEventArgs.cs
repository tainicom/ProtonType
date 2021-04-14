/************************************************************************

   AvalonDock

   Copyright (C) 2019 Kastellanos Nikos

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.ComponentModel;
using AvalonDock.Layout;

namespace AvalonDock
{
    public class AnchorableClosingEventArgs : CancelEventArgs
    {
        public readonly LayoutAnchorable Anchorable;

        public AnchorableClosingEventArgs(LayoutAnchorable anchorable)
        {
            Anchorable = anchorable;
        }

    }
}
