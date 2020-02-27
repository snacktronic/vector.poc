using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using UI.Core;

namespace UI.UserControls.Views
{
    public interface ISpaceViewModel
    {
        ObservableCollection<Line> Shapes { get; }
        void AddLine();
    }
    public class SpaceViewModel : Model, ISpaceViewModel
    {
        private ObservableCollection<Line> _shapes;
        public SpaceViewModel()
        {
            _shapes = new RangeObservableCollection<Line>();            
        }

        public void AddLine()
        {            
            var line = new Line();
            line.Stroke = Brushes.Cyan;
            line.StrokeThickness = 2;
            _shapes.Add(line);            
        }

        public void Clear()
        {
            _shapes.Clear();
        }

        public ObservableCollection<Line> Shapes => _shapes;
    }
}
