/**
 * Stellar Generator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;

namespace StellarGenerator
{
    /// <summary>
    /// Extensions for the random class
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns a random boolean
        /// </summary>
        public static Boolean Boolean(this Random random, Int32 prob = 50)
        {
            return random.Next(0, 100) < prob;
        }
    }
}