using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QMath
{
    public static class Extensions
    {
        public static TValue[,] Cross<TValue>(this TValue[,] a, TValue[,] b, Func<TValue, TValue, TValue, TValue> product)
        {
            var axl = a.GetLength(0);
            var ayl = a.GetLength(1);
            var bxl = b.GetLength(0);
            var byl = b.GetLength(1);

            if (axl != byl) throw new Exception("Shape error!");

            var prd = new TValue[ayl, bxl];
            for (var ax = 0; ax < axl; ax++)
            for (var ay = 0; ay < ayl; ay++)
            for (var by = 0; by < byl; by++)
            for (var bx = 0; bx < bxl; bx++)
                
            prd[ay, bx] = product(prd[ay, bx], a[ax, ay], b[bx, by]);

            return prd;
        }

        public static string Printable<TValue>(this TValue[,] a)
        {
            var axl = a.GetLength(0);
            var ayl = a.GetLength(1);

            var sb = new StringBuilder();
            sb.Append("[");
            for (var ax = 0; ax < axl; ax++)
            {
                sb.Append("[");
                for (var ay = 0; ay < ayl; ay++) sb.Append((ay > 0 ? ", " : "") + a[ax, ay]);
                sb.Append("]"+Environment.NewLine);
            }
            sb.Append("]");
            return sb.ToString();
        }

    }
}
