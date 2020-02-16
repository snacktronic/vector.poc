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
        readonly int _size, _axis, _charges;
        //readonly Segment _symbols, _axis, _constraints;
        readonly double[,] _charge, _position;

        // axis: modelde kullamayı düşündüğünüz boyuttan bir fazla kullanılırsa sıkışma durumunda parçacıklar için yeni
        public Space(int axis, int charges, IEnumerable<Segment> segments)
        {
            _size = segments.Last().Max;

            //_symbols      = new Segment { Offset = 0            , Length = symbols };
            //_axis         = new Segment { Offset = _symbols.Max , Length = axis };
            //_constraints  = new Segment { Offset = _axis.Max    , Length = constraints };
            _charge = new double[_size, charges];
            _position = new double[_size, axis];
        }

        public void Charge(Segment segment, int i, int charge, double value)
        {
            if (i < 0 || i >= segment.Length) throw new IndexOutOfRangeException(nameof(i));
            if (charge < 0 || i >= _charges) throw new IndexOutOfRangeException(nameof(charge));
            _charge[segment.Offset + i, charge] = value;
        }

        public double Read(Segment segment, int i, int axes)
        {
            if (i < 0 || i >= segment.Length) throw new IndexOutOfRangeException(nameof(i));
            if (axes < 0 || i >= _axis) throw new IndexOutOfRangeException(nameof(axes));
            return _position[segment.Offset + i, axes];
        }

        public void eval(double temper)
        {
            double[,] acc = new double[_size, _axis];

            for (int a = 0; a < _size; a++)
            {
                for (int b = 0; b < _size; b++)
                {
                    for (int i = 0; i < _axis; i++)
                    {
                        acc[a, i] += (_position[a, i] - _position[b, i]) * Force(a, b) * temper;
                    }
                }
            }

            for (int a = 0; a < _size; a++)
            {
                for (int i = 0; i < _axis; i++)
                {
                    _position[a, i] += acc[a, i];
                }
            }

        }

        public double[,] Distance(Segment sa, Segment sb)
        {
            var result = new double[sb.Length, sa.Length];
            for (int a = 0; a < sa.Length; a++)
            {
                for (int b = 0; b < sb.Length; b++)
                {
                    result[a, b] = Distance(a, b);
                }
            }
            return result;
        }
        public double Force(int a, int b) => Charge(a, b) / Distance(a, b);
        public double Charge(int a, int b) => Mul(_charges, i => _charge[a, i] * _charge[b, i]);
        public double Distance(int a, int b) => Sum(_axis, i => Math.Pow(_position[a, i] - _position[b, i], 2));
        public double Sum(int count, Func<int, double> f) => Enumerable.Range(0, count).Aggregate(0.0, (acc, i) => acc += f(i));
        public double Mul(int count, Func<int, double> f) => Enumerable.Range(0, count).Aggregate(1.0, (acc, i) => acc *= f(i));
        public double Sub(int count, Func<int, double> f) => Enumerable.Range(0, count).Aggregate(1.0, (acc, i) => acc -= f(i));
        public double Div(int count, Func<int, double> f) => Enumerable.Range(0, count).Aggregate(1.0, (acc, i) => acc /= f(i));

        //private double weather()
        //{
        //    // ideal = 0
        //    // actual = 123
        //    // diff = 0 - 123

        //    throw new NotImplementedException();
        //}
    }
}
