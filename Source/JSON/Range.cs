/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using Accrete;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Stellarator.JSON
{
    /// <summary>
    /// Ranges for doubles
    /// </summary>
    public class Range
    {
        public Double min;
        public Double max;

        /// <summary>
        /// Outputs a random variable within the bounds of the range
        /// </summary>
        public static implicit operator Double(Range r)
        {
            return Generator.Random.Range(r.min, r.max);
        }

        /// <summary>
        /// Resonsible for JSON loading
        /// </summary>
        public class Converter : JsonCreationConverter<Double>
        {
            protected override Double Create(Type objectType, JObject jObject)
            {
                if (FieldExists("min", jObject) && FieldExists("max", jObject))
                {
                    return new Range
                    {
                        min = jObject["min"].ToObject<Double>(),
                        max = jObject["max"].ToObject<Double>()
                    };
                }
                return jObject.ToObject<Double>();
            }
        }
    }
}