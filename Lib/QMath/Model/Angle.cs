using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QMath.Model
{
    public class Angle : Number
    {
        public Angle(double value) : base(value)
        {
        }

        public static implicit operator double(Angle n) => n.Value;
        public static implicit operator Angle(double n) => new Angle ( n );
        public Radian AsRadian => new Radian( Value * Math.PI / 180.0);
    }
}
