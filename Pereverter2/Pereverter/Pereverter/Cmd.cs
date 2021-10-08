using System;
using System.Windows.Input;

namespace Pereverter
{
    public class Cmd : ICommand
    {
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => Executed?.Invoke();
        public event Action Executed;
        public event EventHandler CanExecuteChanged;
    }
}