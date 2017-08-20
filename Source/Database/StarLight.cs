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
    public class StarLight
    {
        [ParserTarget("ambientLightColor")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ColorParser AmbientLightColor { get; set; }

        [ParserTarget("givesOffLight")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NumericParser<bool> GivesOffLight { get; set; }

        [ParserTarget("IVASunColor")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ColorParser IvaSunColor { get; set; }

        [ParserTarget("IVASunIntensity")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NumericParser<double> IvaSunIntensity { get; set; }

        [ParserTarget("scaledSunlightColor")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ColorParser ScaledSunlightColor { get; set; }

        [ParserTarget("scaledSunlightIntensity")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NumericParser<double> ScaledSunlightIntensity { get; set; }

        [ParserTarget("sunLensFlareColor")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ColorParser SunLensFlareColor { get; set; }

        [ParserTarget("sunlightColor")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public ColorParser SunlightColor { get; set; }

        [ParserTarget("sunlightIntensity")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NumericParser<double> SunlightIntensity { get; set; }

        [ParserTarget("sunlightShadowStrength")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public NumericParser<double> SunlightShadowStrength { get; set; }
    }
}