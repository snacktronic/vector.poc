using Lib.QMath.Model;
using Lib.QPhysics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QLogic
{
    public class Circuit
    {
        private Space mental;
        public Circuit(int symbols, int inputs, int outputs, int constraints)
        {
            var segments = new List<Segment>()
                .Define(symbols, out var Symbols)
                .Define(inputs, out var Inputs)
                .Define(outputs, out var Ouputs)
                .Define(constraints, out var Constraints);

            mental = new Space(2, 1, segments);
        }
    }
}
