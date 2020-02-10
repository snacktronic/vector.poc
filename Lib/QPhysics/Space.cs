using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QPhysics
{
    class Space
    {
        static readonly char[] exptected = "Hello World!".ToCharArray();
        static readonly char[] symbols = exptected.Distinct().ToArray();
        static readonly int model = exptected.Length, cause = 1;
        static readonly int size = 100, axis = 3, charges = 1;

        double[,] _inputs = new double[size, charges];
        double[,] _outputs = new double[size, axis];

        public double this[int i, int j] 
        {
            get => _outputs[i,j];
            set => _inputs[i,j] = value;
         }

        public void eval()
        {
            double[,] acc = new double[size, axis];

            var w = weather();
            for (int a = 0; a < size; a++)
            {
                for (int b = 0; b < size; b++)
                {
                    var c = Sum(charges, i => _inputs[a, i] * _inputs[b, i]);
                    var d = Sum(axis, i => Math.Pow(_outputs[a, i] - _outputs[b, i],2));
                    var f = c / d * w;
                    for (int i = 0; i < axis; i++)
                    {
                        acc[a, i] += (_outputs[a, i] - _outputs[b, i]) * f;
                    }
                }
            }
        }

        private double Sum(int count, Func<int, double> f) => Enumerable.Range(0, count).Select(i => f(i)).Sum();

        private double weather()
        {
            // ideal = 0
            // actual = 123
            // diff = 0 - 123
            throw new NotImplementedException();
        }
    }
}
