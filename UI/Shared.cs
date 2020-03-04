using Lib.QLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace UI
{
    public class Shared
    {
        public static Circuit Circuit;
        public static char[] Expected = "Hello".ToCharArray();
        public static char[] Symbols = Expected.Distinct().ToArray();
        public static char[] Outputs = new char[Expected.Length];
        public static double[] Inputs;
        static Shared()
        {
            Inputs = constraint(Expected, Outputs);
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

        private static double[] constraint<T>(T[] expected, T[] outputs)
        {
            var C = Enumerable.Range(0, expected.Length).Select(i => expected[i].Equals(outputs[i])  ? 0.0 : 1.0).ToArray();

            var S = new List<double>();
            for (int i = 0; i < C.Length; i++)
            {
                for (int j = i+1; j < C.Length; j++)
                {
                    S.Add(C[i] + C[j] == 0.0 ? 0.0 : C[i] > 0 ? 1.0 : 1.0);
                }
            }

            return S.Concat(S.Select(s => s == 0.0 ? 0.0 : -1.0)).ToArray();
        }
    }
}
