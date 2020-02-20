using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Core
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification = false;
        public bool _suppressed = false;
        public bool SuppressNotification {
            get => _suppressNotification;
            set
            {
                _suppressNotification = value;
                if (_suppressed && !value)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                _suppressed = false;
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!SuppressNotification)
                base.OnCollectionChanged(e);
            else
                _suppressed = true;
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)throw new ArgumentNullException("list");
            SuppressNotification = true;
            foreach (T item in list) Add(item);
            SuppressNotification = false;
        }
    }
}
