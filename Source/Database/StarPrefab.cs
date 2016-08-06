/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Collections.Generic;
using Kopernicus.Configuration;

namespace Stellarator.Database
{
    /// <summary>
    /// Prefab for a Star
    /// </summary>
    public class StarPrefab
    {
        [ParserTarget("Material")] public StarMaterial material;
        [ParserTarget("Light")] public StarLight light;
    }
}