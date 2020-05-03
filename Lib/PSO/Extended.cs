using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.PSO
{
    public class Extended : Swarm
    {
        public Extended(int particles, int dimensions, bool invert, double rangeMin, double rangeMax, int seed, Exit exit, params Cost[] functions) : base(particles, dimensions, invert, rangeMin, rangeMax, seed, exit, functions)
        {
        }

        private double _prevCost = 0.0;
        private double _temperature = 1.0;
        protected override double[] Coefficients(int f)
        {
            var coefficients = base.Coefficients(f);
            coefficients[0] =
            coefficients[1] =
            coefficients[2] =
            coefficients[3] =
            1.0 / (1.0 + _global_max_cost[f] * _global_max_cost[f]) * 2.05;

            coefficients[2] *= 2.05;
            coefficients[3] *= 2.05;

            _temperature = _prevCost > _global_min_cost[0]
                    ? 1.0
                    : _temperature * 1.000000000000001 //1.000000000000001
                ;

            coefficients[0] *= _temperature;
            return coefficients;
        }
    }
}
