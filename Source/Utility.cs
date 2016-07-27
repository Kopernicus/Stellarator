/**
 * Stellar Generator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */


using System.Reflection;

namespace StellarGenerator
{
    /// <summary>
    /// Utility functions
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Copy one objects fields to another object via reflection
        /// </summary>
        /// <param name="source">Object to copy fields from</param>
        /// <param name="destination">Object to copy fields to</param>
        public static void CopyObjectFields<T>(T source, T destination)
        {
            // Reflection based copy
            foreach (FieldInfo field in typeof(T).GetFields())
            {
                // Only copy non static fields
                if (!field.IsStatic)
                    field.SetValue(destination, field.GetValue(source));
            }
        }
    }
}