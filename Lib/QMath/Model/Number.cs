using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QMath.Model
{
    public class Number
    {
        public double Value;
        public static implicit operator double(Number n) => n.Value;
        public static implicit operator Number(double n) => new Number ( n );

        public Number(double value) => Value = value;

        public override string ToString() => $"{Value}";
    }
}
