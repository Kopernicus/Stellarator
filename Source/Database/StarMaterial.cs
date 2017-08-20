/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using Kopernicus.Configuration;

namespace Stellarator.Database
{
    /// <summary>
    ///     Prefab for StarMaterial
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StarMaterial
    {
        [ParserTarget("emitColor0")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ColorParser EmitColor0 { get; set; }

        [ParserTarget("emitColor1")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ColorParser EmitColor1 { get; set; }

        [ParserTarget("rimBlend")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NumericParser<double> RimBlend { get; set; }

        [ParserTarget("rimColor")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ColorParser RimColor { get; set; }

        [ParserTarget("rimPower")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NumericParser<double> RimPower { get; set; }

        [ParserTarget("sunspotColor")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ColorParser SunspotColor { get; set; }

        [ParserTarget("sunspotPower")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NumericParser<double> SunspotPower { get; set; }
    }
}