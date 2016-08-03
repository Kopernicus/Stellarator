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
    public class StarMaterial
    {
        public Color emitColor0;
        public Color emitColor1;
        public Double sunspotPower;
        public Color sunspotColor;
        public Color rimColor;
        public Double rimPower;
        public Double rimBlend;
    }
}