using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI.UserControls.Views
{
    /// <summary>
    /// Interaction logic for SpaceView.xaml
    /// </summary>
    public partial class SpaceView : UserControl
    {
        public SpaceView()
        {
            InitializeComponent();
        }

        public void Refresh()
        {
            Clear();
            Render();
        }

        public void Clear()
        {
            MyCanvas.Children.Clear();
        }

        public void Render()
        {
            var rnd = new Random();
            var line = new Line();
            line.Stroke = Brushes.Cyan;
            line.StrokeThickness = 2;
            line.X1 = rnd.Next(640);
            line.Y1 = rnd.Next(480);
            line.X2 = rnd.Next(640);
            line.Y2 = rnd.Next(480);
            MyCanvas.Children.Add(line);
        }
    }
}
