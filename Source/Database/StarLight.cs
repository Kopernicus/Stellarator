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
        [ParserTarget("ambientLightColor")] public ColorParser ambientLightColor;
        [ParserTarget("givesOffLight")] public NumericParser<bool> givesOffLight;
        [ParserTarget("IVASunColor")] public ColorParser IVASunColor;
        [ParserTarget("IVASunIntensity")] public NumericParser<double> IVASunIntensity;
        [ParserTarget("scaledSunlightColor")] public ColorParser scaledSunlightColor;
        [ParserTarget("scaledSunlightIntensity")] public NumericParser<double> scaledSunlightIntensity;
        [ParserTarget("sunLensFlareColor")] public ColorParser sunLensFlareColor;
        [ParserTarget("sunlightColor")] public ColorParser sunlightColor;
        [ParserTarget("sunlightIntensity")] public NumericParser<double> sunlightIntensity;
        [ParserTarget("sunlightShadowStrength")] public NumericParser<double> sunlightShadowStrength;
    }
}