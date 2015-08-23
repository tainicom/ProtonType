#region License
//   Copyright 2019 Kastellanos Nikolaos
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

using tainicom.ProtonType.Framework.Modules;
using tainicom.ProtonType.Framework.ViewModels;

namespace tainicom.ProtonType.Framework.Commands
{
    public class RemovePaneCmd : CommandBase<ISiteViewModel>
    {
        PaneViewModel _pane;
        bool _result;

        public RemovePaneCmd(ISiteViewModel receiver, PaneViewModel pane): base(receiver)
        {
            _pane = pane;
        }

        public override void Execute()
        {
            _result = ((ISiteViewModelReceiver)Receiver).__RemovePane(_pane);
        }

        public override void Undo()
        {
            if (_result)
            {
                ((ISiteViewModelReceiver)Receiver).__AddPane(_pane);
            }
        }
    }
}
