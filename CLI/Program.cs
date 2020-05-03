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
            
            var swarm = new Lib.PSO.Extended(particles:1, dimensions:10, invert: false, rangeMin:-100.0, rangeMax:100.0, seed:1, 
                exit:() => exit || ++i < 0,// || ++i > 100000,
                new[]
                    {
                        //new Cost(s=> Math.Abs(s.Sum(x => x) - Math.PI)),
                        //new Cost(s=> s.Sum(x => 2*x * 6*x)),
                        new Cost(s => s.Sum(x => x * x)), // Sphere
                        //new Cost(s => Enumerable.Range(0, 10).Sum(x => (s[x] - (1+x*2)) * (s[x] - (1+x*2)))) // Ordered
                        //new Cost(s => s.Max(x=> x*x )),
                        //new Cost(s => s.Min(x => x*x)),
                        //new Cost(s => s.Average(x => x*x))
                    }
                  //  .Concat(Enumerable.Range(0,10).Select(j => new Cost(s => s[jx*x] * s[j])))
                    .ToArray()
                );

            swarm.Minimum += (function, cost, solution) =>
            {
                if (function != 0) return;
                exit = cost <= 0.0;
                var j = i;
                Console.WriteLine($"#{j} Minimum[{function}]: {cost:C} [{string.Join(",",solution)}]"); 
            };

            swarm.Maximum += (function, cost, solution) =>
            {
                if (function != 0) return;
                var j = i;
                Console.WriteLine($"#{j} Maximum[{function}]: {cost:C} [{string.Join(",", solution)}]");
            };

            swarm.Search();
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
