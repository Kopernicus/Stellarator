/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

namespace Stellarator.Database
{
    using Kopernicus.Configuration;

    /// <summary>
    ///     Prefab for StarMaterial
    /// </summary>
    public class StarMaterial
    {
        [ParserTarget("emitColor0")] public ColorParser emitColor0;
        [ParserTarget("emitColor1")] public ColorParser emitColor1;
        [ParserTarget("rimBlend")] public NumericParser<double> rimBlend;
        [ParserTarget("rimColor")] public ColorParser rimColor;
        [ParserTarget("rimPower")] public NumericParser<double> rimPower;
        [ParserTarget("sunspotColor")] public ColorParser sunspotColor;
        [ParserTarget("sunspotPower")] public NumericParser<double> sunspotPower;
    }
}