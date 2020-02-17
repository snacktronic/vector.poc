using Lib.QLogic;
using Lib.QMath.Model;
using Lib.QPhysics;
using Lib.Signal.Model;
using System;
using System.Collections.Generic;
using System.Data;
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
            Console.WriteLine("EOP!");
            Console.ReadLine();
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
