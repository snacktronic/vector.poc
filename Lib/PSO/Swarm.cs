using System;

namespace Lib.PSO
{
    public delegate bool     Exit();
    public delegate double   Cost(double[] solution);
    public delegate double[] Coefficients(int cost);

    public class Swarm
    {
        double[]       _mass, _global_best_cost, _global_worst_cost;
        double[][]     _position, _velocity, _global_best, _global_worst;
        double[,]      _cost, _best_cost, _worst_cost;
        double[,][]    _best, _worst;             
        int            _particles, _dimensions;
        double         _min, _max;
        Coefficients   _coefficients;
        Cost[]         _functions;
        Exit           _exit;
        Random         _rnd;

        public Swarm(int particles, int dimensions, double min, double max, int seed, Exit exit, Coefficients coefficients, params Cost[] functions)
        {
            _rnd               = new Random(seed);                                    // Random value provider for samples.
            _min               = min;
            _max               = max;
            _particles         = particles;                                           // Number of samples.
            _dimensions        = dimensions;                                          // Level of freedom.
            _coefficients      = coefficients;                                        // Scalar for each 6 terms
            _functions         = functions;                                           // Cost functions
                               
            _mass              = new double[particles];                               // Particle's mass
            _position          = new double[particles][];                             // Particle's current solution
            _velocity          = new double[particles][];                             // Particle's current velocity
            _cost              = new double[particles, functions.Length];             // Particle's current cost ??? May not be necessary
                               
            _best              = new double[particles, functions.Length][];           // Particle's best solution
            _best_cost         = new double[particles, functions.Length];             // Particle's best cost
                               
            _worst             = new double[particles, functions.Length][];           // Particle's worst solution
            _worst_cost        = new double[particles, functions.Length];             // Particle's worst cost
                               
            _global_best       = new double[functions.Length][];                      // Global best solution
            _global_best_cost  = new double[functions.Length];                        // Global best cost

            _global_worst      = new double[functions.Length][];                      // Global worst solution
            _global_worst_cost = new double[functions.Length];                        // Global worst cost


            // Initialize Particles
            for (var p = 0; p < particles; p++)
            {
                _position[p] = new double[dimensions];
                _velocity[p] = new double[dimensions];
                for (var d = 0; d < dimensions; d++)
                {
                    _velocity[p][d] = _rnd.NextDouble() - 0.5;
                }
            }

            for (var f = 0; f < _functions.Length; f++)
            {
                var template = new double[dimensions];
                _global_best[f] = (double[]) template.Clone();
                _global_worst[f] = (double[]) template.Clone();

                var cost = _functions[f](Solution(template));
                _global_worst_cost[f] = cost;
                _global_best_cost[f] = cost;

                for (var p = 0; p < particles; p++)
                {
                    _mass[p] = _cost[p, f] = _best_cost[p, f] = _global_best_cost[f];
                }
            }
        }

        private double[] Solution(double[] position)
        {
            var scale = _max - _min;
            var result = new double[_dimensions];
            
            for (var d = 0; d < _dimensions; d++)
            {
                result[d] *= scale;
                result[d] += _min;
            }

            return result;
        }

        public void Charge()
        {
            // Coefficients
            var c = new double[_functions.Length][];
            for (var f = 0; f < _functions.Length; f++)
            {
                c[f] = _coefficients(f);
            }

            for (var p = 0; p < _particles; p++)
            {
                for (var d = 0; d < _dimensions; d++)
                {
                    var v = _velocity[p][d];         // Current Velocity
                    for (var f = 0; f < _functions.Length; f++)
                    {
                        // Mass
                        var m  = _mass[p];
                        // Distances
                        var d1 = _position[p][d] - _best[p, f][d];       
                        var d2 = _position[p][d] - _global_best[f][d];   
                        var d3 = _position[p][d] - _worst[p, f][d];      
                        var d4 = _position[p][d] - _global_worst[f][d];  

                        // Charging Terms
                        v += c[f][0] * c[f][1] * _velocity[p][d];       // Inertia
                        v += c[f][0] * c[f][2] * (m * 1.0 / (d1 * d1)); // Cognitive motivator
                        v += c[f][0] * c[f][3] * (m * 1.0 / (d2 * d2)); // Global motivator
                        v -= c[f][0] * c[f][4] * (m * 1.0 / (d3 * d3)); // Cognitive lesson
                        v -= c[f][0] * c[f][5] * (m * 1.0 / (d4 * d4)); // Global lesson
                    }

                    // Update position
                    _position[p][d] += v;
                    _position[p][d] %= 1.0;
                }

                // Update global and personal best and worst cases.
                for (var f = 0; f < _functions.Length; f++)
                {
                    var solution = Solution(_position[p]);
                    _cost[p,f] = _functions[f](solution);
                    _mass[p] = 0.5 * _mass[p] + _cost[p, f]; // += 1.0 / (_mass[p] - cost)^2 # Div by ZERO error

                    if (_cost[p, f] > _best_cost[p, f])
                    {
                        _best[p, f] = _position[p];
                        _best_cost[p, f] = _cost[p, f];

                        if (!(_cost[p, f] > _global_best_cost[f])) continue;

                        _global_best[f] = _position[p];
                        _global_best_cost[f] = _cost[p, f];
                    }
                    else if (_cost[p, f] < _worst_cost[p, f])
                    {
                        _worst[p, f] = _position[p];
                        _worst_cost[p, f] = _cost[p, f];

                        if (!(_cost[p, f] < _global_worst_cost[f])) continue;

                        _global_worst[f] = _position[p];
                        _global_worst_cost[f] = _cost[p, f];
                    } 
                }
            }
        }
 
    }
}
