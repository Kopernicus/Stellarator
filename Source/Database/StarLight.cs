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
    public class StarLight
    {
        [ParserTarget("ambientLightColor")]
        public ColorParser AmbientLightColor { get; set; }

        [ParserTarget("givesOffLight")]
        public NumericParser<bool> GivesOffLight { get; set; }

        [ParserTarget("IVASunColor")]
        public ColorParser IvaSunColor { get; set; }

        [ParserTarget("IVASunIntensity")]
        public NumericParser<double> IvaSunIntensity { get; set; }

        [ParserTarget("scaledSunlightColor")]
        public ColorParser ScaledSunlightColor { get; set; }

        [ParserTarget("scaledSunlightIntensity")]
        public NumericParser<double> ScaledSunlightIntensity { get; set; }

        [ParserTarget("sunLensFlareColor")]
        public ColorParser SunLensFlareColor { get; set; }

        [ParserTarget("sunlightColor")]
        public ColorParser SunlightColor { get; set; }

        [ParserTarget("sunlightIntensity")]
        public NumericParser<double> SunlightIntensity { get; set; }

        [ParserTarget("sunlightShadowStrength")]
        public NumericParser<double> SunlightShadowStrength { get; set; }
    }
}