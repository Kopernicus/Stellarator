/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

namespace Stellarator.Database
{
    using System.Collections.Generic;
    using Kopernicus.Configuration;

    /// <summary>
    ///     Prefab for a PQS
    /// </summary>
    /// {
    public class RingPrefab
    {
        [ParserTargetCollection("Rings")] public List<Ring> rings;

        public class Ring
        {
            [ParserTarget("angle")] public NumericParser<double> angle;
            [ParserTarget("innerRadius")] public NumericParser<double> innerRadius;
            [ParserTarget("lockRotation")] public NumericParser<bool> lockRotation;
            [ParserTarget("outerRadius")] public NumericParser<double> outerRadius;
        }
    }
}