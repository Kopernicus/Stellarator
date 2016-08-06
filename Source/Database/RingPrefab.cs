/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Collections.Generic;
using Kopernicus.Configuration;

namespace Stellarator.Database
{
    /// <summary>
    /// Prefab for a PQS
    /// </summary>{
    public class RingPrefab
    {
        [ParserTargetCollection("Rings")] public List<Ring> rings;
        public class Ring
        {
            [ParserTarget("innerRadius")] public NumericParser<Double> innerRadius;
            [ParserTarget("outerRadius")] public NumericParser<Double> outerRadius;
            [ParserTarget("angle")] public NumericParser<Double> angle;
            [ParserTarget("lockRotation")] public NumericParser<Boolean> lockRotation;
        }
    }
}