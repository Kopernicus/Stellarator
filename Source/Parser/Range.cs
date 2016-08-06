/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using Accrete;
using DynamicExpresso;
using Kopernicus.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Stellarator
{
    /// <summary>
    /// Ranges for doubles
    /// </summary>
    public class Range
    {
        [ParserTarget("min")]
        public Double min;
        [ParserTarget("max")]
        public Double max;

        /// <summary>
        /// Outputs a random variable within the bounds of the range
        /// </summary>
        public static implicit operator Double(Range r)
        {
            return Generator.Random.Range(r.min, r.max);
        }
    }
}