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
    public class Circuit
    {
        public Space Internals => _internals;
        private Space _internals;
        private readonly Segment _origin;
        private readonly Segment _inputs;
        private readonly Segment _outputs;
        private readonly Segment _symbols;

        public Circuit(int inputs, int outputs, int symbols)
        {
            _internals = new Space (axes:2)
                .Define(1, out _origin)
                .Define((inputs * inputs + inputs)/2,  out _inputs)
                .Define(outputs, out _outputs)
                .Define(symbols, out _symbols)
                .Initialize(new Random());
        }

        public int[] Translate()
        {
            var translation = new int[_outputs.Length];
            var distance = _internals.SquaredDistance(_outputs, _symbols);
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

        public T[] Translate<T>(T[] symbols)
        {
            return Translate().Select(t => symbols[t]).ToArray();
        }

        public void Set(double[] inputs)
        {
            var cross = new List<double>();
            for (var i = 0; i < inputs.Length; i++)
            {
                for (var j = i + 1; j < inputs.Length; j++)
                {
                    cross.Add(inputs[i] + inputs[j] > 0.0 ? 0.0 : -1.0);
                }
            }

            inputs = cross.ToArray();

            for (int i = 0; i < inputs.Length; i++)
            {
                _internals.SetStrength(_inputs, i, inputs[i]);
            }
        }

        public double Execute()
        {
            for (int i = 0; i < _internals.Axes; i++)
            {
                _internals.Fields.Position[_origin.Offset, i] = 0.0;
                _internals.Fields.Direction[_origin.Offset, i] = 0.0;
            }

            _internals.Eval(_inputs);
            return _internals.Temper;
        }
    }
}
