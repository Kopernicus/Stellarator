/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Collections.Generic;
using ProceduralQuadSphere.Unity;

namespace Stellarator.Database
{
    /// <summary>
    /// Prefab for StarMaterial
    /// </summary>
    public class StarLight
    {
        public Color sunlightColor;
        public Double sunlightIntensity;
        public Double sunlightShadowStrength;
        public Color scaledSunlightColor;
        public Double scaledSunlightIntensity;
        public Color IVASunColor;
        public Double IVASunIntensity;
        public Color ambientLightColor;
        public Color sunLensFlareColor;
        public Boolean givesOffLight;
    }
}