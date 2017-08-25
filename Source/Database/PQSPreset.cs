/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using ConfigNodeParser;
using Kopernicus.Configuration;

namespace Stellarator.Database
{
    /// <summary>
    ///     Prefab for a PQS
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class PQSPreset
    {
        [ParserTarget("maxRadius")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NumericParser<int> MaxRadius { get; set; }

        [ParserTarget("minRadius")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NumericParser<int> MinRadius { get; set; }

        [ParserTarget("Mods")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ConfigNode Mods { get; set; }
    }
}