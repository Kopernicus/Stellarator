/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Stellarator.JSON
{
    /// <summary>
    /// JsonConverter that dynamically creates objects using a custom Create method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">
        /// contents of JSON object that will be deserialized
        /// </param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);

        public override Boolean CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override Object ReadJson(JsonReader reader,
            Type objectType,
            Object existingValue,
            JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            return target;
        }

        public override void WriteJson(JsonWriter writer, Object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected static Boolean FieldExists(String fieldName, JObject jObject)
        {
            return jObject[fieldName] != null;
        }
    }
}
