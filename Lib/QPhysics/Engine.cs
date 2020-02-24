using Lib.QMath.Model;
using System;
using System.Linq;

namespace Lib.QPhysics
{
    public class Fields
    {
        public double[] Strength;
        public double[,] Position;
        public double[,] Direction;
    }

    public class Space
    {
        public int Size { get; set; }
        public int Axis { get; set; }
        // t=1-c^2/C^2
        public double Temper { get; set; }
        public Fields Fields { get; set; }
        public Space(int axis)
        {
            Axis = axis;
        }
    }

    public static class SpaceFactory
    {
        public static Space Define(this Space space, int length, out Segment variable)
        {
            lock (space)
            {
                variable = new Segment(space.Size, length);
                space.Size += length;
                return space;
            }
        }

        public static Space Initialize(this Space space, Random seed)
        {
            lock (space)
            {
                space.Fields = new Fields
                {
                    Strength = new double[space.Size],
                    Position = new double[space.Size, space.Axis],
                    Direction = new double[space.Size, space.Axis]
                    
                };

                for (var i = 0; i < space.Size; i++)
                {
                    for (var j = 0; j < space.Axis; j++)
                    {
                        space.Fields.Position[i, j] = seed.NextDouble() * 2.0 - 1.0;
                    }
                }

                for (var i = 0; i < space.Size; i++)
                {
                    space.Fields.Strength[i] = 1.0;
                }
                return space;
            }
        }
    }

    public static class Engine
    {
        public static void SetStrength(this Space space, Segment segment, int i, double value)
        {
            lock (space)
            {
                space.Fields.Strength[segment.Offset + i] = value;
            }
        }

        public static double GetStrength(this Space space, Segment segment, int i)
        {
            lock (space)
            {
                return space.Fields.Strength[segment.Offset + i];
            }
        }

        public static double GetPosition(this Space space, Segment segment, int i, int axes)
        {
            lock (space)
            {
                return space.Fields.Position[segment.Offset + i, axes];
            }
        }

        public static Space Eval(this Space space, Segment forces)
        {
            lock (space)
            {
                space.Temper = forces.Range.Sum(f=>space.Fields.Strength[f]) / forces.Length;
                var acc = new double[space.Size, space.Axis];

                for (var a = 0; a < space.Size; a++)
                {
                    for (var i = 0; i < space.Axis; i++)
                    {
                        foreach (var f in forces.Range)
                        {
                            acc[a, i] += (space.Fields.Position[a, i] - space.Fields.Direction[a, i]) * space.Strength(f,  a);
                        }
                    }
                }

                for (var a = 0; a < space.Size; a++)
                {
                    for (var b = 0; b < space.Size; b++)
                    {
                        for (var i = 0; i < space.Axis; i++)
                        {
                            foreach (var f in forces.Range)
                            {
                                acc[a, i] += (space.Fields.Position[a, i] - space.Fields.Position[b, i]) * space.Force(f, a, b);
                            }
                        }
                    }
                }

                for (var a = 0; a < space.Size; a++)
                {
                    for (var i = 0; i < space.Axis; i++)
                    {
                        space.Fields.Position[a, i] += acc[a, i];
                    }
                }

                space.Fields.Direction = acc;

                return space;
            }
        }

        public static double[,] SquaredDistance(this Space space, Segment sa, Segment sb)
        {
            var result = new double[sa.Length, sb.Length];
            for (var a = 0; a < sa.Length; a++)
            {
                for (var b = 0; b < sb.Length; b++)
                {
                    result[a, b] = space.SquaredDistance(sa.Offset + a, sb.Offset + b);
                }
            }
            return result;
        }

        public static double Force(this Space space, int f, int a, int b)
        {
            var s = space.Strength(f, a) * space.Strength(f, b);
            var x = space.SquaredDistance(a,b);
            var t = space.Temper;
            var k = 1/(t*x+1);
            return s * (Math.Tanh(Math.PI * x) * (k - t) * (1 - t));
        }
        public static double Strength(this Space space, int f, int a) => 1.0 / (space.SquaredDistance(f, a) + 1.0) * space.Fields.Strength[f];
        public static double SquaredDistance(this Space space, int a, int b) => Sum(space.Axis, i => Math.Pow(space.Fields.Position[a, i] - space.Fields.Position[b, i], 2));
        public static double Sum(int count, Func<int, double> f) => Enumerable.Range(0, count).Aggregate(0.0, (acc, i) => acc += f(i));
    }
}
