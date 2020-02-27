using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UI.Core
{
    public sealed class Notifier : INotifyPropertyChanged
    {
        private object _owner;
        private HashSet<string> _suppressed;
        public Notifier(object ownder, bool suppressed = false)
        {
            _owner = ownder;
            _suppressed = suppressed ? new HashSet<string>() : default;
        }
        private event PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged -= value;
                _propertyChanged += value;
            }
        
            remove
            {
                _propertyChanged -= value;
            }
        }

        public void NotifyPropertyChanged([CallerMemberName] string property = null)
        {
            NotifyPropertyChanged(default, property);
        }
        public void NotifyPropertyChanged(object sender, [CallerMemberName] string property = null)
        {
            if (_suppressed != null)
                _suppressed.Add(property);
            else
                _propertyChanged?.Invoke(sender ?? _owner ?? this, new PropertyChangedEventArgs(property));
        }

        public bool Suppress()
        {
            if (_suppressed != (default)) return false;
            _suppressed = _suppressed ?? new HashSet<string>();
            return true;
        }

        public bool Reset(object sender = null)
        {
            if (_suppressed == null) return false;
            var properties = _suppressed.ToArray();
            _suppressed = default;
            foreach (var property in properties)
            {
                NotifyPropertyChanged(property);
            }
            return true;
        }
    }
}
