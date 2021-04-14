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
using tainicom.ProtonType.Framework.Commands;

namespace tainicom.ProtonType.App.CommandMgr
{
    class BatchCmd : ICommandQueue
    {
        public bool Idle { get; private set; }

        IList<ICommand> commands = new List<ICommand>();
        bool isReadOnly = false;

        internal BatchCmd()
        {
            Idle = true;
        }

        public void Enqueue(ICommand command)
        {
            if (isReadOnly)
                throw new NotSupportedException("Collection is read-only");

            commands.Add(command);
        }

        private void MakeReadOnly()
        {
            isReadOnly = true;
            commands = ((List<ICommand>)commands).AsReadOnly();
        }
        
        internal ICommand CreateCommand()
        {
            this.MakeReadOnly();
            if (commands.Count == 0) return null;
            if (commands.Count == 1) return commands[0];
            return new CommandGroupCmd(commands);
        }

        /// <summary>
        /// Execute/Undo more than one command in a single step.
        /// Useful for compiling simple commands into more complex one.
        /// </summary>
        class CommandGroupCmd : ICommand
        {
            readonly IList<ICommand> commandGroup;

            public CommandGroupCmd(IList<ICommand> commandGroup)
            {
                this.commandGroup = commandGroup;
            }

            void ICommand.Execute()
            {
                // execute all commands in a row
                for(int i=0;i<commandGroup.Count;i++)
                    commandGroup[i].Execute();
            }

            void ICommand.Undo()
            {
                // undo all commands in reverse order
                for (int i = (commandGroup.Count-1); i >=0; i--)
                    commandGroup[i].Undo();
            }
        }

    }
}
