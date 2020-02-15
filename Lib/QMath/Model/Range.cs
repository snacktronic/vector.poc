using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QMath.Model
{
    public class Range
    {
        public int Offset { get; set; }
        public int Length { get; set; }
        public int Max => Offset + Length;
        public IEnumerable<T> Select<T>(Func<int, T> f) => Enumerable.Range(Offset, Max).Select(f);
    }
}
