/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using ConfigNodeParser;
using Kopernicus.Configuration;

namespace Stellarator.Database
{
    /// <summary>
    /// Prefab for a PQS
    /// </summary>
    public class PQSPreset
    {
        [ParserTarget("minRadius")] public NumericParser<Int32> minRadius;
        [ParserTarget("maxRadius")] public NumericParser<Int32> maxRadius;
        [ParserTarget("Mods")] public ConfigNode mods;
    }
}