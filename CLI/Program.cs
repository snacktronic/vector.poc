using Lib.QLogic;
using Lib.QMath.Model;
using Lib.QPhysics;
using Lib.Signal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Test02();

            Console.ReadLine();
        }

        private static void Test02()
        {
            var expected = "Hello World!".ToCharArray();
            var symbols = expected.Distinct().ToArray();
            var model = new char[expected.Length];
            var constraints = new double[123];

            //double[] constraints(Circuit c)
            //{
            //    // Discreet constraints
            //    rm.For.Select(m => c.Nearest)
            //    Enumerable.Range(0, model.Length).Select(m => c.NearestIn(range["m"], ))
            //    // Fuzzy constraints
            //    return Enumerable.Range(0, model.Length).Select(i => );
            //    return new double[1];
            //}

            var space = new Space(symbols.Length, model.Length, constraints.Length);
            space.Translate(symbols, model);


            var rs = new Segment { Offset = 0, Length = symbols.Length - 1 };
            var rm = new Segment { Offset = symbols.Length, Length = model.Length - 1 };

            
        }

        private static void Test01()
        {
            Console.WriteLine($"{new FractionalBandWidth(0.5).Thetha.AsAngle}");
        }
    }
}
