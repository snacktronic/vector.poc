using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.PSO
{
    public class TotalCost
    {
        private readonly double[] _average;
        private readonly Cost[] _costs;

        public TotalCost(params Cost[] costs)
        {
            _costs = costs;
            _average = new double[_costs.Length];
        }

        public double Cost(double[] solution)
        {
            var i = 0;
            foreach (var cost in _costs.Select(f => f(solution)))
            {
                _average[i] = (cost + _average[i++]) / 2.0;
            }

            var to = 0;
            var total = _average
                .OrderBy(x => x)
                //.Select(Math.Tanh)
                .Select(x => Math.Pow(2.0 * x, to++))
                .Sum();

            return total;
        }
    }
}
