using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.UserControls.Commands
{
    public abstract class Command : ICommand
    {
        private bool _CanExecute = true;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _CanExecute;
        }

        public void Execute(object parameter)
        {

            try
            {
                _CanExecute = false;
                CanExecuteChanged.Invoke(this, new EventArgs());
                ExecuteImpl(parameter);
            }
            finally
            {
                _CanExecute = true;
                CanExecuteChanged.Invoke(this, new EventArgs());
            }
        }

        internal abstract void ExecuteImpl(object parameter);
    }
}
