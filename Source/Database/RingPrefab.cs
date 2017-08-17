/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System.Collections.Generic;
using Kopernicus.Configuration;

namespace Stellarator.Database
{
    /// <summary>
    ///     Prefab for a PQS
    /// </summary>
    /// {
    public class RingPrefab
    {
        [ParserTargetCollection("Rings")]
        public List<Ring> Rings { get; set; }

        public class Ring
        {
            [ParserTarget("angle")]
            public NumericParser<double> Angle { get; set; }

            [ParserTarget("innerRadius")]
            public NumericParser<double> InnerRadius { get; set; }

            [ParserTarget("lockRotation")]
            public NumericParser<bool> LockRotation { get; set; }

            [ParserTarget("outerRadius")]
            public NumericParser<double> OuterRadius { get; set; }
        }
    }
}