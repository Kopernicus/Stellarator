using System;
using Accrete;

namespace StellarGenerator
{
    public class Generator
    {
        /// <summary>
        /// The random number generator
        /// </summary>
        public static Random Random { get; set; }

        /// <summary>
        /// This is the core of the whole app. It generates a solar system, based on a seed.
        /// </summary>
        public static void Generate(Int32 seed, String folder)
        {
            // Create a new Solar System using libaccrete
            SolarSystem system = new SolarSystem(false, true, s => { });
            SolarSystem.Generate(ref system);

            // Reuse the random component
            Random = system.random;

            // 
        }
    }
}