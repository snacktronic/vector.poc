using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.PSO
{
    public class Extended : Swarm
    {
        public Extended(int particles, int dimensions, bool invert, double min, double max, int seed, Exit exit, params Cost[] functions) : base(particles, dimensions, invert, min, max, seed, exit, functions)
        {
        }

        private double _prevCost = 0.0;
        private double _temperature = 1.0;
        protected override double[] Coefficients(int f)
        {
            var coefficients = base.Coefficients(f);
            coefficients[0] = 
            coefficients[1] *=
            coefficients[2] =
            coefficients[3] =
            1.0 / (1.0 + _global_minimum_cost[f] * _global_minimum_cost[f]);

            coefficients[2] *= 2.05;
            coefficients[3] *= 2.05;

            _temperature = _prevCost > _global_minimum_cost[0]
                ? 1.0
                : _temperature * 1.0000001;

           

            coefficients[0] *= _temperature;
            Console.WriteLine("Coef: " + coefficients[0]);
            return coefficients;
        }
    }
}
