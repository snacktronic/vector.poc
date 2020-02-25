using Lib.QLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI
{
    public class Shared
    {
        public static Circuit Circuit;
        public static char[] Expected = "Hello World!".ToCharArray();
        public static char[] Symbols = Expected.Distinct().ToArray();
        public static char[] Outputs = new char[Expected.Length];
        public static double[] Inputs = new double[Expected.Length];
        static Shared()
        {
            Circuit = new Circuit(Inputs.Length, Outputs.Length, Symbols.Length);
        }

        private static double min = double.MaxValue;
        public static double Eval()
        {
            Outputs = Circuit.Translate(Symbols);
            Inputs = constraint(Expected, Outputs);
            Circuit.Set(Inputs);
            Circuit.Execute();
            return min; 
        }

        private static double[] constraint(char[] expected, char[] outputs)
        {
            return Enumerable.Range(0, expected.Length).Select(i => expected[i] == outputs[i] ? 0.0 : 1.0).ToArray();
        }
    }
}
