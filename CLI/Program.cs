using Lib.QLogic;
using Lib.QMath.Model;
using Lib.QPhysics;
using Lib.Signal.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Lib.PSO;
using Lib.QMath;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            //Test001();
            TestPSO();
            Console.WriteLine("EOP!");
            Console.ReadLine();
        }

        private static void TestPSO()
        {
            var i = 0;
            var exit = false;

            var sphere = new Lib.PSO.Extended(particles:1, dimensions:10, charge: 1.0, rangeMin:-100.0, rangeMax:100.0, seed:1, 
                exit:() => exit || ++i < 0,// || ++i > 100000,
                functions: new []{
                new Cost(s => s.Skip(0).Take(3).Sum(x => x * x)),
                new Cost(s => s.Skip(3).Take(3).Sum(x => x * x)),
                new Cost(s => s.Skip(6).Take(3).Sum(x => x * x)),
                new Cost(s => s.Skip(9).Take(3).Sum(x => x * x))
                }
                );

            sphere.Minimum += (function, cost, solution) =>
            {
                var j = i;
                Console.WriteLine($"#{j} Min[{function}]: {cost:C} [{string.Join(",",solution)}]"); 
            };

            sphere.Maximum += (function, cost, solution) =>
            {
                var j = i;
                Console.WriteLine($"#{j} Max[{function}]: {cost:C} [{string.Join(",", solution)}]");
            };

            sphere.Search();
        }

        private static void Test001()
        {
            var expected = "Hello World!".ToCharArray();
            var symbols = expected.Distinct().ToArray();
            var outputs = new char[expected.Length];
            var inputs = new double[expected.Length];
            var circuit = new Circuit(inputs.Length, outputs.Length, symbols.Length);

            do
            {
                outputs = circuit.Translate(symbols);
                Console.WriteLine(new string(outputs));
                inputs = constraint(expected, outputs);
                circuit.Set(inputs);
            } while (circuit.Execute() > 0);

            Console.WriteLine(new string(outputs));
        }

        private static double[] constraint(char[] expected, char[] outputs)
        {
            return Enumerable.Range(0, expected.Length).Select(i => expected[i] == outputs[i] ? 0.0 : 1.0).ToArray();
        }

        private static void Test01()
        {
            Console.WriteLine($"{new FractionalBandWidth(0.5).Thetha.AsAngle}");
        }
    }
}
