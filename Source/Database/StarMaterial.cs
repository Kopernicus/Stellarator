/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using Kopernicus.Configuration;

namespace Stellarator.Database
{
    /// <summary>
    /// Prefab for StarMaterial
    /// </summary>
    public class StarMaterial
    {
        [ParserTarget("emitColor0")] public ColorParser emitColor0;
        [ParserTarget("emitColor1")] public ColorParser emitColor1;
        [ParserTarget("sunspotPower")] public NumericParser<Double> sunspotPower;
        [ParserTarget("sunspotColor")] public ColorParser sunspotColor;
        [ParserTarget("rimColor")] public ColorParser rimColor;
        [ParserTarget("rimPower")] public NumericParser<Double> rimPower;
        [ParserTarget("rimBlend")] public NumericParser<Double> rimBlend;
    }
}