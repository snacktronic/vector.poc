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
        public static char[] Expected = "ABC".ToCharArray();
        public static char[] Symbols = Expected.Distinct().ToArray();
        public static char[] Output = new char[Expected.Length];
        public static char[] LastOutput = new char[Expected.Length];
        public static double[] Inputs;
        static Shared()
        {
            Inputs = constraint(Expected, Output);
            Circuit = new Circuit(Inputs.Length, Output.Length, Symbols.Length);
        }

        private static double min = double.MaxValue;
        public static double Eval()
        {
            Output = Circuit.Translate(Symbols);
            Inputs = constraint(Expected, Output);
            Circuit.Set(Inputs);
            Circuit.Execute();
            LastOutput = Output;
            return min; 
        }

        
        private static double[] constraint<T>(T[] expected, T[] output)
        {
            var constraits = new List<IEnumerable<double>>();
            constraits
                .Add(Enumerable.Range(0, expected.Length)
                .Select(i => expected[i].Equals(output[i]) ? 0.0 : 1.0));
            //constraits
            //    .Add(Enumerable.Range(0, expected.Length)
            //    .Select(i => LastOutput[i].Equals(expected[i]) || !LastOutput[i].Equals(output[i]) ? 0.0 : 1.0));
            constraits
                .Add(new []
                {
                    output.Distinct().Count() == expected.Distinct().Count() ? 0.0 : 1.0 ,
                });
            return constraits.SelectMany(c => c).ToArray();
        }
    }
}
