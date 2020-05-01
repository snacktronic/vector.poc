using System;

namespace Lib.PSO
{
    public delegate bool     Exit();
    public delegate double   Cost(double[] solution);
    //public delegate double[] Coefficients(int cost, double[] solution);

    public class Swarm
    {
        protected double[]       _mass, _global_minimum_cost, _global_maximum_cost;
        protected double[][]     _position, _velocity, _global_minimum, _global_maximum;
        protected double[,]      _minimum_cost, _maximum_cost;
        protected double[,][]    _minimum, _maximum;             
        protected int            _particles, _dimensions;
        protected double         _min, _max, _charge;
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


        public Swarm(int particles, int dimensions, bool invert, double min, double max, int seed, Exit exit,
            params Cost[] functions)
        {
            _rnd                = new Random(seed);                                    // Random value provider for samples.
            _exit               = exit;                                                // Exit condition
            _min                = min;                                                 // Solution space min boundary
            _max                = max;                                                 // Solution space max boundary
            _charge             = invert ? -1.0 : 1.0;                                 // Affects velocity behaviour
            _particles          = particles;                                           // Number of samples.
            _dimensions         = dimensions;                                          // Level of freedom.
            _functions          = functions;                                           // Cost functions
                                
            _mass               = new double[particles];                               // Particle's mass
            _position           = new double[particles][];                             // Particle's current solution
            _velocity           = new double[particles][];                             // Particle's current velocity
                                
            _minimum             = new double[particles, functions.Length][];          // Particle's best solution
            _minimum_cost        = new double[particles, functions.Length];            // Particle's best cost
                                
            _maximum             = new double[particles, functions.Length][];          // Particle's worst solution
            _maximum_cost        = new double[particles, functions.Length];            // Particle's worst cost
                                
            _global_minimum      = new double[functions.Length][];                     // Global best solution
            _global_minimum_cost = new double[functions.Length];                       // Global best cost

            _global_maximum      = new double[functions.Length][];                     // Global worst solution
            _global_maximum_cost = new double[functions.Length];                       // Global worst cost

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
                _global_minimum[f] = new double[dimensions];
                _global_maximum[f] = new double[dimensions];

                var cost = _functions[f](Solution(new double[dimensions]));
                _global_minimum_cost[f] = cost;
                _global_maximum_cost[f] = cost;

                for (var p = 0; p < particles; p++)
                {
                    _minimum[p, f] = new double[dimensions];
                    _maximum[p, f] = new double[dimensions];
                    _mass[p] = _maximum_cost[p, f] = _minimum_cost[p, f] = cost;
                }
            }
        }

        protected double[] Solution(double[] position)
        {
            var scale = _max - _min;
            var result = (double[])position.Clone();
            
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
                c[f] = Coefficients(f);
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
                        
                        // Random
                        var r = _rnd.NextDouble();

                        // Distances
                        var d1 = _position[p][d] - _minimum[p, f][d];       
                        var d2 = _position[p][d] - _global_minimum[f][d];   
                        var d3 = _position[p][d] - _maximum[p, f][d];      
                        var d4 = _position[p][d] - _global_maximum[f][d];  

                        // Charging Terms
                        v += c[f][0] * c[f][1] * _velocity[p][d];                           // Inertia
                        v += _charge * r * c[f][0] * c[f][2] * (1.0 + m / (1.0 + d1 * d1)); // Cognitive motivator
                        v += _charge * r * c[f][0] * c[f][3] * (1.0 + m / (1.0 + d2 * d2)); // Global motivator
                        v -= _charge * r * c[f][0] * c[f][4] * (1.0 + m / (1.0 + d3 * d3)); // Cognitive lesson
                        v -= _charge * r * c[f][0] * c[f][5] * (1.0 + m / (1.0 + d4 * d4)); // Global lesson
                    }

                    _velocity[p][d] = v;
                    // Update position
                    _position[p][d] = Mod(_position[p][d] + v);
                }

                _mass[p] = 0;
                // Update global and personal best and worst cases.
                for (var f = 0; f < _functions.Length; f++)
                {
                    var solution = Solution(_position[p]);
                    var cost = _functions[f](solution);
                    _mass[p] += cost; // += 1.0 / (_mass[p] - cost)^2 # Div by ZERO error

                    if (cost < _minimum_cost[p, f])
                    {
                        _minimum[p, f] = (double[])_position[p].Clone();
                        _minimum_cost[p, f] = cost;

                        if (!(cost < _global_minimum_cost[f])) continue;

                        _global_minimum[f] = _minimum[p, f];
                        _global_minimum_cost[f] = cost;

                        OnMinimum(f, cost, solution);
                    }
                    else if (cost > _maximum_cost[p, f])
                    {
                        _maximum[p, f] = (double[])_position[p].Clone();
                        _maximum_cost[p, f] = cost;

                        if (!(cost > _global_maximum_cost[f])) continue;

                        _global_maximum[f] = _maximum[p, f];
                        _global_maximum_cost[f] = cost;

                        OnMaximum(f, cost, solution);
                    } 
                }
            }
        }
        
        protected virtual double[] Coefficients(int f)
        {
            return new[]
            {
                1.0, // Global Temperature 
                1.0, // Inertia
                1.0, // Personal Motivation
                1.0, // Swarm Motivation
                1.0, // Personal Lesson
                1.0  // Swarm Lesson
            };
        }

        public void Search()
        {
            while (!_exit())
            {
                Charge();
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
