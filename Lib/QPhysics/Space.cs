using Lib.QMath.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QPhysics
{
    public class Space
    {
        readonly int _size;
        readonly Range _symbols, _axis, _charges;
        readonly double[,] _charge, _position;

        // axis: modelde kullamayı düşündüğünüz boyuttan bir fazla kullanılırsa sıkışma durumunda parçacıklar için yeni
        public Space(int symbols, int model, int constraints, int axis = 3, int charges = 1)
        {
            _size = symbols + model + constraints;
            _symbols = new Range { Offset = 0            , Length = symbols };
            _axis    = new Range { Offset = _symbols.Max , Length = axis };
            _charges = new Range { Offset = _axis.Max    , Length = charges };
            _charge = new double[_size, charges];
            _position = new double[_size, axis];
        }
  
        public void Charge(int i, int charge, double value) => _charge[i, charge] = value;
        public double Read(int i, int axes) => _position[i, axes];

        public void eval()
        {
            double[,] acc = new double[_size, _axis.Length];

            var w = weather();
            for (int a = 0; a < _size; a++)
            {
                for (int b = 0; b < _size; b++)
                {
                    var c = Charge(a, b);
                    var d = Distance(a, b);
                    var f = c / d * w;
                    for (int i = 0; i < _axis.Length; i++)
                    {
                        acc[a, i] += (_position[a, i] - _position[b, i]) * f;
                    }
                }
            }

            for (int a = 0; a < _size; a++)
            {
                for (int i = 0; i < _axis.Length; i++)
                {
                    _position[a, i] += acc[a, i];
                }
            }

        }

        public void Translate(char[] symbols, char[] model)
        {
            for (int i = 0; i < model.Length; i++)
            {

            }
        }

        public double Charge(int a, int b) => Sum(_charges.Length, i => _charge[a, i] * _charge[b, i]);

        public double Distance(int a, int b) => Sum(_axis.Length, i => Math.Pow(_position[a, i] - _position[b, i], 2));

        public double Sum(int count, Func<int, double> f) => Enumerable.Range(0, count).Select(i => f(i)).Sum();

        private double weather()
        {
            // ideal = 0
            // actual = 123
            // diff = 0 - 123
            throw new NotImplementedException();
        }
    }
}
