/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Collections.Generic;
using Kopernicus.Configuration;
using ProceduralQuadSphere.Unity;

namespace Stellarator.Database
{
    /// <summary>
    /// Prefab for StarMaterial
    /// </summary>
    public class StarLight
    {
        [ParserTarget("sunlightColor")] public ColorParser sunlightColor;
        [ParserTarget("sunlightIntensity")] public NumericParser<Double> sunlightIntensity;
        [ParserTarget("sunlightShadowStrength")] public NumericParser<Double> sunlightShadowStrength;
        [ParserTarget("scaledSunlightColor")] public ColorParser scaledSunlightColor;
        [ParserTarget("scaledSunlightIntensity")] public NumericParser<Double> scaledSunlightIntensity;
        [ParserTarget("IVASunColor")] public ColorParser IVASunColor;
        [ParserTarget("IVASunIntensity")] public NumericParser<Double> IVASunIntensity;
        [ParserTarget("ambientLightColor")] public ColorParser ambientLightColor;
        [ParserTarget("sunLensFlareColor")] public ColorParser sunLensFlareColor;
        [ParserTarget("givesOffLight")] public NumericParser<Boolean> givesOffLight;
    }
}