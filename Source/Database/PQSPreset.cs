/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Collections.Generic;

namespace Stellarator.Database
{
    /// <summary>
    /// Prefab for a PQS
    /// </summary>
    public class PQSPreset
    {
        public Int32 minRadius;
        public Int32 maxRadius;
        public Dictionary<String, Dictionary<String, Object>> mods;
    }
}
