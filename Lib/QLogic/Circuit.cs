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
        private readonly Segment _inputs;
        private readonly Segment _outputs;
        private readonly Segment _symbols;

        public Circuit(int inputs, int outputs, int symbols)
        {
            _mental = new Space (axis:3)
                .Define(inputs,  out _inputs)
                .Define(outputs, out _outputs)
                .Define(symbols, out _symbols)
                .Initialize(new Random());
        }

        public int[] Translate()
        {
            var translation = new int[_outputs.Length];
            var distance = _mental.SquaredDistance(_outputs, _symbols);
            for (var o = 0; o < _outputs.Length; o++)
            {
                var min = double.MaxValue;
                for (var s = 0; s < _symbols.Length; s++)
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
            for (var i = 0; i < _inputs.Length; i++)
            {
                _mental.SetStrength(_inputs, i, inputs[i]);
            }

            _mental.Temper =  1.0 / -(1.0+inputs.Sum( i => i * i)+1);
        }

        public double Execute()
        {
            if (_mental.Temper > 0)
            {
                _mental.Eval(_inputs);
            }
            return 1.0 -_mental.Temper;
        }
    }
}
