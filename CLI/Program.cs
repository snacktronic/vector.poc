using Lib.Signal.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine($"{new FractionalBandWidth(0.5).Thetha.AsAngle}");

            Console.ReadLine();
        }
    }
}
