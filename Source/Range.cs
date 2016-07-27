/**
 * Stellar Generator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using Accrete;
using Newtonsoft.Json.Linq;

namespace StellarGenerator
{
    /// <summary>
    /// Ranges for doubles
    /// </summary>
    public class Range
    {
        public Double min;
        public Double max;

        public Double Next()
        {
            return Generator.Random.Range(min, max);
        }

        public Range(JObject obj)
        {
            min = obj["min"].ToObject<Double>();
            max = obj["max"].ToObject<Double>();
        }
    }
}