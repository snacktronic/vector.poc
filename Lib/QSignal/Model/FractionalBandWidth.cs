using Lib.QMath.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Signal.Model
{
    public class FractionalBandWidth : Number
    {
        public FractionalBandWidth(double value) : base(value)
        {
        }

        public static implicit operator FractionalBandWidth(double d) => new FractionalBandWidth ( d );
        public Radian Thetha => Math.PI / 2.0 * (1 - Value / 2.0);
    }

}
