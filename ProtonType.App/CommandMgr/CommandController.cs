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
using System.Media;

namespace tainicom.ProtonType.App.CommandMgr
{
    public class CommandController : ICommandController
    {
        private Stack<ICommand> _commandStack;
        private Stack<ICommand> _undoStack;

        public bool Idle { get; private set; }


        public CommandController()
        {
            Idle = true;
            _commandStack = new Stack<ICommand>();
            _undoStack = new Stack<ICommand>();
        }

        public void EnqueueAndExecute(ICommand command)
        {
            _Enqueue(command);
            ExecuteCommand();
        }

        private void _Enqueue(ICommand command)
        {
            if (!Idle) throw new OperationCanceledException("Re-etry is not supported. Check 'Idle' property.");

            _commandStack.Clear();
            _commandStack.Push(command);
        }

        internal void ExecuteCommand()
        {
            if (_commandStack.Count == 0)
            {
                SystemSounds.Beep.Play();
                return;
            }

            Idle = false;
            try
            {
                ICommand command = _commandStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
            finally { Idle = true; }
        }

        internal void UndoCommand()
        {
            if (_undoStack.Count == 0)
            {
                SystemSounds.Beep.Play();
                return;
            }

            Idle = false;
            try
            {
                ICommand command = _undoStack.Pop();
                command.Undo();
                _commandStack.Push(command);
            }
            finally { Idle = true; }
        }

        internal void ClearHistory()
        {
            _undoStack.Clear();
            _commandStack.Clear();
        }

        internal int RedoCount { get { return _commandStack.Count; } }
        internal int UndoCount { get { return _undoStack.Count; } }


        public ICommandQueue CreateCommandQueue()
        {
            return new BatchCmd();
        }

        public void EnqueueAndExecute(ICommandQueue batch)
        {
            if (!Idle) throw new OperationCanceledException("Re-etry is not supported. Check 'Idle' property.");

            BatchCmd batchCmd = (BatchCmd)batch;
            var command = batchCmd.CreateCommand();
            if (command == null) return;
            EnqueueAndExecute(command);
        }

    }
}
