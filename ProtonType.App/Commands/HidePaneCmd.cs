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

using AvalonDock.Layout;
using tainicom.ProtonType.Framework.Commands;
using tainicom.ProtonType.Framework.Modules;

namespace tainicom.ProtonType.App.Commands
{
    public class HidePaneCmd : CommandBase<ISiteViewModel>
    {
        private LayoutAnchorable _anchorable;
        //PaneViewModel _pane;
        
        public HidePaneCmd(ISiteViewModel receiver, LayoutAnchorable achorable): base(receiver)
        {
            this._anchorable = achorable;
        }

        //public HidePaneCmd(ISiteViewModel receiver, PaneViewModel pane): base(receiver)
        //{
        //    _pane = pane;
        //}

        public override void Execute()
        {
            _anchorable.Hide();
            //((ISiteViewModelReceiver)Receiver).____HidePane(_pane);
        }

        public override void Undo()
        {
            _anchorable.Show();
            //((ISiteViewModelReceiver)Receiver).__ShowPane(_pane);
        }
    }
}
