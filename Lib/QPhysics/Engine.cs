using Lib.QMath.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.QPhysics
{
    public class Particles
    {
        public double[,] Charge;
        public double[,] Position;
        public double[,] Momentum;
    }

    public class Space
    {
        public int Size { get; set; }
        public int Axis { get; set; }
        public int Charges { get; set; }
        public double Temper { get; set; }
        public Particles Particles { get; set; }

        public Space(Particles particles = null)
        {
            Particles = particles;
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
                space.Particles = new Particles
                {
                    Charge = new double[space.Size, space.Charges],
                    Position = new double[space.Size, space.Axis],
                    Momentum = new double[space.Size, space.Axis]
                };


                for (var i = 0; i < space.Size; i++)
                {
                    for (var j = 0; j < space.Axis; j++)
                    {
                        space.Particles.Position[i, j] = seed.NextDouble() * 2.0 - 1.0;
                    }
                }

                for (var i = 0; i < space.Size; i++)
                {
                    for (var j = 0; j < space.Charges; j++)
                    {
                        space.Particles.Charge[i, j] = 1.0;
                    }
                }
                return space;
            }
        }
    }

    public static class Engine
    {
        public static void Charge(this Space space, Segment segment, int i, int charge, double value)
        {
            lock (space)
            {
                space.Particles.Charge[segment.Offset + i, charge] = value;
            }
        }

        public static double Read(this Space space, Segment segment, int i, int axes)
        {
            lock (space)
            {
                return space.Particles.Position[segment.Offset + i, axes];
            }
        }

        public static Space Eval(this Space space)
        {
            lock (space)
            {
                var acc = new double[space.Size, space.Axis];

                for (var a = 0; a < space.Size; a++)
                {
                    for (var i = 0; i < space.Axis; i++)
                    {
                        acc[a, i] += (space.Particles.Position[a, i] - space.Particles.Momentum[a, i]) * space.Force(a) * space.Temper;
                    }
                }

                for (var a = 0; a < space.Size; a++)
                {
                    for (var b = 0; b < space.Size; b++)
                    {
                        for (var i = 0; i < space.Axis; i++)
                        {
                            acc[a, i] += (space.Particles.Position[a, i] - space.Particles.Position[b, i]) * space.Force(a, b) * space.Temper;
                        }
                    }
                }

                for (var a = 0; a < space.Size; a++)
                {
                    for (var i = 0; i < space.Axis; i++)
                    {
                        space.Particles.Position[a, i] += acc[a, i];
                        space.Particles.Momentum[a, i] = acc[a, i];
                    }
                }

                return space;
            }
        }

        public static double[,] Distance(this Space space, Segment sa, Segment sb)
        {
            var result = new double[sa.Length, sb.Length];
            for (var a = 0; a < sa.Length; a++)
            {
                for (var b = 0; b < sb.Length; b++)
                {
                    result[a, b] = space.Distance(sa.Offset + a, sb.Offset + b);
                }
            }
            return result;
        }

        public static double Force(this Space space, int a) => space.Charge(a) / space.Distance(a);
        public static double Charge(this Space space, int a) => Mul(space.Charges, i => space.Particles.Charge[a, i] * space.Particles.Charge[a, i]);
        public static double Distance(this Space space, int a) => Sum(space.Axis, i => Math.Pow(space.Particles.Position[a, i] - space.Particles.Momentum[a, i], 2));
        public static double Force(this Space space, int a, int b) => space.Charge(a, b) / space.Distance(a, b);
        public static double Charge(this Space space, int a, int b) => Mul(space.Charges, i => space.Particles.Charge[a, i] * space.Particles.Charge[b, i]);
        public static double Distance(this Space space, int a, int b) => Sum(space.Axis, i => Math.Pow(space.Particles.Position[a, i] - space.Particles.Position[b, i], 2));
        public static double Sum(int count, Func<int, double> f) => Enumerable.Range(0, count).Aggregate(0.0, (acc, i) => acc += f(i));
        public static double Sub(int count, Func<int, double> f) => Enumerable.Range(0, count).Aggregate(0.0, (acc, i) => acc -= f(i));
        public static double Mul(int count, Func<int, double> f) => Enumerable.Range(0, count).Aggregate(1.0, (acc, i) => acc *= f(i));
        public static double Div(int count, Func<int, double> f) => Enumerable.Range(0, count).Aggregate(1.0, (acc, i) => acc /= f(i));
    }
}
