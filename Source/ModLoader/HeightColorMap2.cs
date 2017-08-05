/**
 * Kopernicus Planetary System Modifier
 * ====================================
 * Created by: BryceSchroeder and Teknoman117 (aka. Nathaniel R. Lewis)
 * Maintained by: Thomas P., NathanKell and KillAshley
 * Additional Content by: Gravitasi, aftokino, KCreator, Padishar, Kragrathea, OvenProofMars, zengei, MrHappyFace
 * ------------------------------------------------------------- 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301  USA
 * 
 * This library is intended to be used as a plugin for Kerbal Space Program
 * which is copyright 2011-2015 Squad. Your usage of Kerbal Space Program
 * itself is governed by the terms of its EULA, not the license above.
 * 
 * https://kerbalspaceprogram.com
 */

using System.Collections.Generic;
using System.Linq;
using ConfigNodeParser;
using ProceduralQuadSphere;
using ProceduralQuadSphere.Unity;

namespace Kopernicus
{
    namespace Configuration
    {
        namespace ModLoader
        {
            [RequireConfigType(ConfigType.Node)]
            public class HeightColorMap2 : ModLoader<PQSMod_HeightColorMap2>, IParserEventSubscriber
            {
                // Land class loader 
                public class LandClassLoader2
                {
                    // Land class object
                    public PQSMod_HeightColorMap2.LandClass landClass2;

                    // Name of the class
                    [ParserTarget("name")]
                    public string name 
                    {
                        get { return landClass2.name; }
                        set { landClass2.name = value; }
                    }

                    // Color of the class
                    [ParserTarget("color")]
                    public ColorParser color
                    {
                        get { return landClass2.color; }
                        set { landClass2.color = value; }
                    }

                    // Fractional altitude start
                    // altitude = (vertexHeight - vertexMinHeightOfPQS) / vertexHeightDeltaOfPQS
                    [ParserTarget("altitudeStart")]
                    public NumericParser<double> altitudeStart
                    {
                        get { return landClass2.altStart; }
                        set { landClass2.altStart = value; }
                    }

                    // Fractional altitude end
                    [ParserTarget("altitudeEnd")]
                    public NumericParser<double> altitudeEnd
                    {
                        get { return landClass2.altEnd; }
                        set { landClass2.altEnd = value; }
                    }

                    // Should we blend into the next class
                    [ParserTarget("lerpToNext")]
                    public NumericParser<bool> lerpToNext
                    {
                        get { return landClass2.lerpToNext; }
                        set { landClass2.lerpToNext = value; }
                    }

                    public LandClassLoader2 ()
                    {
                        // Initialize the land class
                        landClass2 = new PQSMod_HeightColorMap2.LandClass("class", 0.0, 0.0, Color.white);
                    }

                    public LandClassLoader2(PQSMod_HeightColorMap2.LandClass c)
                    {
                        landClass2 = c;
                    }
                }

                // The deformity of the simplex terrain
                [ParserTarget("blend")]
                public NumericParser<float> blend
                {
                    get { return mod.blend; }
                    set { mod.blend = value; }
                }

                // The deformity of the simplex terrain
                [ParserTarget("maxHeight")]
                public NumericParser<double> maxHeight
                {
                    get { return mod.maxHeight; }
                    set { mod.maxHeight = value; }
                }

                // The deformity of the simplex terrain
                [ParserTarget("minHeight")]
                public NumericParser<double> minHeight
                {
                    get { return mod.minHeight; }
                    set { mod.minHeight = value; }
                }

                // The land classes
                [ParserTargetCollection("LandClasses")]
                public List<LandClassLoader2> landClasses = new List<LandClassLoader2> ();

                void IParserEventSubscriber.Apply(ConfigNode node) { }

                // Select the land class objects and push into the mod
                void IParserEventSubscriber.PostApply(ConfigNode node)
                {
                    PQSMod_HeightColorMap2.LandClass[] landClassesArray = landClasses.Select(loader => loader.landClass2).ToArray();
                    if (landClassesArray.Length != 0)
                        mod.landClasses = landClassesArray;
                }
            }
        }
    }
}

