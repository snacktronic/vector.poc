using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QMath.Model
{
    public class Radian : Number
    {
        public Radian(double value) : base(value)
        {
        }

        public static implicit operator double(Radian n) => n.Value;
        public static implicit operator Radian(double n) => new Radian ( n );

        public Angle AsAngle => new Angle(Value * 180.0 / Math.PI);
    }
}
