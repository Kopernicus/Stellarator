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
        [ParserTarget("emitColor0")]
        public ColorParser EmitColor0 { get; set; }

        [ParserTarget("emitColor1")]
        public ColorParser EmitColor1 { get; set; }

        [ParserTarget("rimBlend")]
        public NumericParser<double> RimBlend { get; set; }

        [ParserTarget("rimColor")]
        public ColorParser RimColor { get; set; }

        [ParserTarget("rimPower")]
        public NumericParser<double> RimPower { get; set; }

        [ParserTarget("sunspotColor")]
        public ColorParser SunspotColor { get; set; }

        [ParserTarget("sunspotPower")]
        public NumericParser<double> SunspotPower { get; set; }
    }
}