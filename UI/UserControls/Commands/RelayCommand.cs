using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.UserControls.Commands
{
    public class RelayCommand : Command
    {
        private Action<object> _relay;
        public RelayCommand(Action<object> relay)
        {
            _relay = relay;
        }
        internal override void ExecuteImpl(object parameter)
        {
            _relay.Invoke(parameter);
        }
    }
}
