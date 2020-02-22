using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QMath.Model
{
    public static class SegmentExtensions
    {
        public static IEnumerable<Segment> Define(this IEnumerable<Segment> segments, int length, out Segment segment)
        {
            var offset = segments.DefaultIfEmpty().Max(s => s.Max);
            segment = new Segment(offset, length);
            return segments.Concat(new[] { segment });
        }
    }
    public class Segment
    {
        public int Offset { get; }
        public int Length { get; }
        public int Max    { get; }
        public Segment(int length) : this(0, length)
        {
        }

        public Segment(int offset, int length)
        {
            Offset = offset;
            Length = length;
            Max = Offset + Length;
        }
        public IEnumerable<T> Select<T>(Func<int, T> f) => Enumerable.Range(Offset, Max).Select(f);
        public IEnumerable<int> Range => Enumerable.Range(Offset, Max);
    }
}
