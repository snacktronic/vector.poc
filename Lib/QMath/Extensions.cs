using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QMath
{
    public static class Extensions
    {
        public static double[,] Dot(this double[,] a, double[,] b, Func<double, double, double> product = null) => a.Cross<double, double>(b, (z, x, y) => z + (product?.Invoke(x,y) ?? x *y));
        public static Tout[,] Cross<Tin, Tout>(this Tin[,] a, Tin[,] b, Func<Tout, Tin, Tin, Tout> product)
        {
            // Row and Column Lengths
            var arl = a.GetLength(0);
            var acl = a.GetLength(1);
            var brl = b.GetLength(0);
            var bcl = b.GetLength(1);

            if (acl != brl) throw new Exception("Shape error!");

            var prd = new Tout[arl,bcl];
            for (var r = 0; r < arl; r++)
            for (var c = 0; c < bcl; c++)
            for (var k = 0; k < acl; k++)
            prd[c, r] = product(prd[c, r], a[c, k], b[k, r]);
           
            return prd;
        }

        public static string Printable<TValue>(this TValue[,] a, Func<TValue, string> format = null)
        {
            var axl = a.GetLength(0);
            var ayl = a.GetLength(1);


            var sb = new StringBuilder();
            sb.Append("[");
            for (var ax = 0; ax < axl; ax++)
            {
                sb.Append(ax == 0 ? "[" : " [");
                for (var ay = 0; ay < ayl; ay++) sb.Append((ay > 0 ? ", " : "") + (format != null ? format(a[ax, ay]) : $"{a[ax, ay]}"));
                sb.Append("]"+ (ax != axl-1 ? Environment.NewLine : string.Empty));
            }
            sb.Append("]");
            return sb.ToString();
        }


    }
}
