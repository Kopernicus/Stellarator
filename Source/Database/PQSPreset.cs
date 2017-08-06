/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

namespace Stellarator.Database
{
    using ConfigNodeParser;
    using Kopernicus.Configuration;

    /// <summary>
    ///     Prefab for a PQS
    /// </summary>
    public class PQSPreset
    {
        [ParserTarget("maxRadius")] public NumericParser<int> maxRadius;
        [ParserTarget("minRadius")] public NumericParser<int> minRadius;
        [ParserTarget("Mods")] public ConfigNode mods;
    }
}