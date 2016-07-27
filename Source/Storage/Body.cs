/**
 * Stellar Generator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Reflection;

namespace StellarGenerator.Storage
{
    /// <summary>
    /// Class that represents the Kopernicus config structure.
    /// Body Node.
    /// </summary>
    public class Body
    {
        // Values
        public String name;
        public String cacheFile;
        public Boolean barycenter;
        public String cbNameLater;
        public Int32 flightGlobalsIndex;
        public Boolean finalizeOrbit;
        public Boolean randomMainMenuBody;

        // TODO: Nodes
        public Properties Properties { get; set; }

        /// <summary>
        /// Creates a new Body
        /// </summary>
        public Body()
        {
            
        }

        public Body(Body template)
        {
            // Copy object fields
            Utility.CopyObjectFields(template, this);

            // Copy nodes
            foreach (PropertyInfo info in typeof(Body).GetProperties())
                info.SetValue(this, Activator.CreateInstance(info.PropertyType, template), null);
        }
    }
}