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
    /// Properties Node.
    /// </summary>
    public class Properties
    {
        // Values
        public String description;
        public Double radius;
        public Double geeASL;
        public Double mass;
        public Double gravParameter;
        public Boolean rotates;
        public Double rotationPeriod;
        public Boolean tidallyLocked;
        public Double initialRotation;
        public Single inverseRotThresholdAltitude;
        public Double albedo;
        public Double emissivity;
        public Double coreTemperatureOffset;
        public Boolean isHomeWorld;
        public Single[] timewarpAltitudeLimits;
        public Double sphereOfInfluence;
        public Double hillSphere;
        public Boolean solarRotationPeriod;
        public Double navballSwitchRadiusMult;
        public Double navballSwitchRadiusMultLow;
        public Boolean useTheInName;
        public Boolean selectable;
        public Boolean hiddenRnD;

        // TODO: Nodes

        /// <summary>
        /// Creates a new Body
        /// </summary>
        public Properties()
        {

        }

        public Properties(Properties template)
        {
            // Copy object fields
            Utility.CopyObjectFields(template, this);

            // Copy nodes
            foreach (PropertyInfo info in typeof(Body).GetProperties())
                info.SetValue(this, Activator.CreateInstance(info.PropertyType, template), null);
        }
    }
}