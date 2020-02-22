using Lib.QMath.Model;
using Lib.QPhysics;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QLogic
{
    public static class CircuitExtensions
    {
        public static T[] Translate<T>(this Circuit circuit, T[] symbols)
        {
            return circuit.Translate().Select(t => symbols[t]).ToArray();
        }
    }
    public class Circuit
    {
        private Space _mental;
        private readonly Segment Inputs;
        private readonly Segment Outputs;
        private readonly Segment Symbols;

        public Circuit(int inputs, int outputs, int symbols)
        {
            _mental = new Space { Axis = 3, Forces = 1 }
                .Define(inputs,  out Inputs)
                .Define(outputs, out Outputs)
                .Define(symbols, out Symbols)
                .Initialize(new Random());
        }

        public int[] Translate()
        {
            var translation = new int[Outputs.Length];
            var distance = _mental.Distance(Outputs, Symbols);
            for (var o = 0; o < Outputs.Length; o++)
            {
                var min = double.MaxValue;
                for (var s = 0; s < Symbols.Length; s++)
                {
                    if (distance[o, s] < min)
                    {
                        translation[o] = s;
                        min = distance[o, s];
                    }
                }   
            }
            return translation;
        }

        public void Set(double[] inputs)
        {
            for (var i = 0; i < Inputs.Length; i++)
            {
                _mental.SetStrength(Inputs, i, inputs[i]);
            }

            _mental.Temper =  1.0 / -(1.0+inputs.Sum( i => i * i)+1);
        }

        public double Execute()
        {
            if (_mental.Temper > 0)
            {
                _mental.Eval();
            }
            return _mental.Temper;
        }
    }
}
