/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using Kopernicus.Configuration;

namespace Stellarator.Database
{
    /// <summary>
    ///     Prefab for a Star
    /// </summary>
    public class StarPrefab
    {
        [ParserTarget("Light")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public StarLight Light { get; set; }

        [ParserTarget("Material")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public StarMaterial Material { get; set; }
    }
}