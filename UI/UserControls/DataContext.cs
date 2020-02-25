using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using UI.Core;
using UI.UserControls.Commands;
using UI.UserControls.Views;

namespace UI.UserControls
{
    public class DataContext : FrameworkElement, ISpaceViewModel
    {
        private SpaceViewModel _spaceViewModel = new SpaceViewModel();
        public FrameworkElement Owner { get; private set; }
        public RelayCommand ExecuteCommand { get; private set; }

        public ObservableCollection<Line> Shapes => _spaceViewModel.Shapes;

        public DataContext(FrameworkElement owner)
        {
            Owner = owner;
            ExecuteCommand = new RelayCommand(HandleExecuteCommand);
        }

        private void HandleExecuteCommand(object parameters)
        {
            switch (parameters)
            {
                case "AddLine":
                    AddLine();
                    break;
                case "Animate":
                    Animate();
                    break;
                case "Eval":
                    UI.Shared.Eval();
                    Update();
                    break;
                case "Initialize":
                    Initialize();
                    break;
            }
        }

        private void Update()
        {
            var internals = UI.Shared.Circuit.Internals;
            
            var lines = _spaceViewModel.Shapes.ToArray();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var coll = (RangeObservableCollection<Line>)_spaceViewModel.Shapes;
                coll.SuppressNotification = true;
                lock (line)
                {
                    line.X1 = 1000 + 10*internals.Fields.Position[i, 0];
                    line.Y1 = 500  + 10*internals.Fields.Position[i, 1];
                    line.X2 = line.X1 + 10*internals.Fields.Direction[i, 0];
                    line.Y2 = line.Y1 + 10*internals.Fields.Direction[i, 1];
                }
                coll.SuppressNotification = false;
            }
        }

        private void Initialize()
        {
            var internals = UI.Shared.Circuit.Internals;
            for (var i = 0; i < internals.Size; i++)
            {
                _spaceViewModel.AddLine();
            }
            Update();
        }

        private void Animate()
        {
            //Task.Run(() => 
            {
                // Animate
                var rnd = new Random();
                foreach (var item in _spaceViewModel.Shapes.ToArray())
                {
                    if (item is Line i)
                    {

                        //App.Current.Dispatcher.Invoke(() =>
                        //{
                        var coll = (RangeObservableCollection<Line>) _spaceViewModel.Shapes;
                        coll.SuppressNotification = true;
                        lock (i)
                        {
                            i.X1 += rnd.Next(10);
                            i.Y1 += rnd.Next(10);
                            i.X2 += rnd.Next(10);
                            i.Y2 += rnd.Next(10);
                        }

                        coll.SuppressNotification = false;
                        //});
                    }
                }
            }

            //);
        }
        public void AddLine()
        {
            _spaceViewModel.AddLine();
        }
    }
}
    
