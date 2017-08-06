/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

namespace Stellarator.Database
{
    using Kopernicus.Configuration;

    /// <summary>
    ///     Prefab for a Star
    /// </summary>
    public class StarPrefab
    {
        [ParserTarget("Light")] public StarLight light;
        [ParserTarget("Material")] public StarMaterial material;
    }
}