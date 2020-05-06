using System;
using System.Linq;

namespace Lib.PSO
{
    public delegate bool     Exit();
    public delegate double   Cost(double[] solution);
    //public delegate double[] Coefficients(int cost, double[] solution);

    public class Swarm
    {
        protected double[]       _mass, _global_min_cost, _global_max_cost;
        protected double[][]     _position, _velocity, _global_min, _global_max;
        protected double[,]      _min_cost, _max_cost;
        protected double[,][]    _min, _max;             
        protected int            _particles, _dimensions;
        protected double         _range_min, _range_max, _charge;
        //protected Coefficients   _coefficients;
        protected Cost[]         _functions;
        protected Exit           _exit;
        protected Random         _rnd;
        public static double Mod(double a, double b = 1.0)
        {
            var c = a % b;
            if ((c < 0 && b > 0) || (c > 0 && b < 0))
            {
                c += b;
            }
            return c;
        }

        public delegate void Handle(int function, double cost, double[] solution);
        public event Handle Minimum;
        public event Handle Maximum;


        public Swarm(int particles, int dimensions, double charge, double rangeMin, double rangeMax, int seed, Exit exit,
            params Cost[] functions)
        {
            _rnd                = new Random(seed);                                    // Random value provider for samples.
            _exit               = exit;                                                // Exit condition
            _range_min          = rangeMin;                                            // Solution space rangeMin boundary
            _range_max          = rangeMax;                                            // Solution space rangeMax boundary
            _charge             = charge;                                              // Affects velocity behaviour
            _particles          = particles;                                           // Number of samples.
            _dimensions         = dimensions;                                          // Level of freedom.
            _functions          = functions;                                           // Cost functions
                                
            _mass               = new double[particles];                               // Particle's mass
            _position           = new double[particles][];                             // Particle's current solution
            _velocity           = new double[particles][];                             // Particle's current velocity
                                
            _min                = new double[particles, functions.Length][];          // Particle's best solution
            _min_cost           = new double[particles, functions.Length];            // Particle's best cost
                                   
            _max                = new double[particles, functions.Length][];          // Particle's worst solution
            _max_cost           = new double[particles, functions.Length];            // Particle's worst cost
                                   
            _global_min         = new double[functions.Length][];                     // Global best solution
            _global_min_cost    = new double[functions.Length];                       // Global best cost

            _global_max         = new double[functions.Length][];                     // Global worst solution
            _global_max_cost    = new double[functions.Length];                       // Global worst cost

            // Initialize Particles
            for (var p = 0; p < particles; p++)
            {
                _position[p] = new double[dimensions];
                _velocity[p] = new double[dimensions];
                for (var d = 0; d < dimensions; d++)
                {
                    _velocity[p][d] = _rnd.NextDouble() - 0.5;
                    _position[p][d] = _rnd.NextDouble() - 0.5;
                }
            }

            for (var f = 0; f < _functions.Length; f++)
            {
                _global_min[f] = new double[dimensions];
                _global_max[f] = new double[dimensions];
                
                var cost = _functions[f](Solution(new double[dimensions]));

                _global_min_cost[f] = cost;
                _global_max_cost[f] = cost;

                for (var p = 0; p < particles; p++)
                {
                    _min[p, f] = new double[dimensions];
                    _max[p, f] = new double[dimensions];
                    _mass[p] = _max_cost[p, f] = _min_cost[p, f] = cost;
                }
            }
        }

        protected double[] Solution(double[] position)
        {
            var scale = _range_max - _range_min;
            var result = (double[])position.Clone();
            
            for (var d = 0; d < _dimensions; d++)
            {
                result[d] *= scale;
                result[d] += _range_min;
            }

            return result;
        }

        private void Charge(int p, double[][] c)
        {
            for (var d = 0; d < _dimensions; d++)
            {
                var v = _velocity[p][d];         // Current Velocity
                for (var f = 0; f < _functions.Length; f++)
                {
                    // Mass
                    var m  = _mass[p];
                    
                    // Random
                    var r = _rnd.NextDouble();

                    // Distances
                    var d1 = _position[p][d] - _min[p, f][d];
                    var d2 = _position[p][d] - _global_min[f][d];   
                    var d3 = _position[p][d] - _max[p, f][d];
                    var d4 = _position[p][d] - _global_max[f][d];

                    // Charging Terms
                    v += _charge * c[f][0] * c[f][1] * _velocity[p][d];                 // Inertia
                    v += _charge * r * c[f][0] * c[f][2] * (1.0 + m / (1.0 + d1 * d1)); // Cognitive motivator
                    v += _charge * r * c[f][0] * c[f][3] * (1.0 + m / (1.0 + d2 * d2)); // Global motivator
                    v -= _charge * r * c[f][0] * c[f][4] * (1.0 + m / (1.0 + d3 * d3)); // Cognitive lesson
                    v -= _charge * r * c[f][0] * c[f][5] * (1.0 + m / (1.0 + d4 * d4)); // Global lesson
                }

                _velocity[p][d] = Math.Tanh(v);

                // Update position
                _position[p][d] = Mod(_position[p][d] + _velocity[p][d]);
            }
        }

        private void UpdateGlobalAndPersonalCases(int p)
        {
            _mass[p] = 0;
            // Update global and personal best and worst cases.
            for (var f = 0; f < _functions.Length; f++)
            {
                var solution = Solution(_position[p]);
                var cost = _functions[f](solution);
                _mass[p] += cost; // += 1.0 / (_mass[p] - cost)^2 # Div by ZERO error

                if (cost < _min_cost[p, f])
                {
                    _min[p, f] = (double[]) _position[p].Clone();
                    _min_cost[p, f] = cost;

                    if (!(cost < _global_min_cost[f])) continue;

                    _global_min[f] = _min[p, f];
                    _global_min_cost[f] = cost;

                    OnMinimum(f, cost, solution);
                }
                else if (cost > _max_cost[p, f])
                {
                    _max[p, f] = (double[]) _position[p].Clone();
                    _max_cost[p, f] = cost;

                    if (!(cost > _global_max_cost[f])) continue;

                    _global_max[f] = _max[p, f];
                    _global_max_cost[f] = cost;

                    OnMaximum(f, cost, solution);
                }
            }
        }

        protected virtual double[] Coefficients(int f)
        {
            return new[]
            {
                1.0, // Temperature 
                1.0, // Inertia
                1.0, // Personal Motivation
                1.0, // Swarm Motivation
                1.0, // Personal Lesson
                1.0  // Swarm Lesson
            };
        }

        public void Search()
        {
            // Iteration
            while (true)
            {
                // Coefficients
                var c = new double[_functions.Length][];
                for (var f = 0; f < _functions.Length; f++)
                {
                    c[f] = Coefficients(f);
                }

                for (var p = 0; p < _particles; p++)
                {
                    Charge(p, c);
                    UpdateGlobalAndPersonalCases(p);
                    if (_exit()) return;
                }
            }
        }

        protected virtual void OnMinimum(int function, double cost, double[] solution)
        {
            Minimum?.Invoke(function, cost, solution);
        }
        protected virtual void OnMaximum(int function, double cost, double[] solution)
        {
            Maximum?.Invoke(function, cost, solution);
        }
    }
}
