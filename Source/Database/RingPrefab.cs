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
    public class RingPrefab
    {
        [ParserTargetCollection("Rings")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<Ring> Rings { get; set; }

        // ReSharper disable once ClassNeverInstantiated.Global
        public class Ring
        {
            [ParserTarget("angle")]
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public NumericParser<double> Angle { get; set; }

            [ParserTarget("innerRadius")]
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public NumericParser<double> InnerRadius { get; set; }

            [ParserTarget("lockRotation")]
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public NumericParser<bool> LockRotation { get; set; }

            [ParserTarget("outerRadius")]
            // ReSharper disable once UnusedAutoPropertyAccessor.Global
            public NumericParser<double> OuterRadius { get; set; }
        }
    }
}