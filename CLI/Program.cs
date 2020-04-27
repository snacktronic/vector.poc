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
            var cnt = 0;
            
            var swarm = new Lib.PSO.Swarm(particles:10, dimensions:5, invert: false, min:-10.0, max:10.0, seed:1, 
                exit:() => ++cnt > 10000, 
                coefficients:f => new [] {0.01, 1.0, 1.0, 1.0, 1.0, 1.0},
                solution => solution.Sum(x => x*x)  // Sphere
                );

            swarm.Minimum += (function, cost, solution) =>
            {
                Console.WriteLine($"Minimum[{function}]: {cost:C}"); 
            };

            swarm.Maximum += (function, cost, solution) =>
            {
                Console.WriteLine($"Maximum[{function}]: {cost:C}");
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
