/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Accrete;
using ConfigNodeParser;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using Kopernicus.Configuration;
using Newtonsoft.Json.Linq;
using ProceduralQuadSphere;
using ProceduralQuadSphere.Unity;
using XnaGeometry;

namespace Stellarator
{
    public class Generator
    {
        /// <summary>
        /// The random number generator
        /// </summary>
        public static Random Random { get; set; }

        /// <summary>
        /// The Kerbin equivalent for the system
        /// </summary>
        public static Planet Kerbin { get; set; }

        /// <summary>
        /// All Stock bodies that we can use as a template
        /// </summary>
        public static String[] Templates = {"Moho", "Eve", "Gilly", "Mun", "Minmus", "Duna", "Ike", "Dres", "Laythe", "Vall", "Tylo", "Bop", "Pol", "Eeloo"};

        /// <summary>
        /// Files that should get copied
        /// </summary>
        public static Dictionary<String, String> toCopy = new Dictionary<String, String>();

        /// <summary>
        /// This is the core of the whole app. It generates a solar system, based on a seed.
        /// </summary>
        public static void Generate(Int32 seed, String folder)
        {
            // Log
            Console.WriteLine("Generating the solar system...");
            Console.WriteLine();

            // Create a new Solar System using libaccrete
            SolarSystem system = new SolarSystem(false, true, s => { });
            SolarSystem.Generate(ref system, seed, Int32.MaxValue);

            // Reuse the random component
            Random = system.random;

            // All Nodes
            List<ConfigNode> nodes = new List<ConfigNode>();

            // Create the root node
            ConfigNode root = new ConfigNode("@Kopernicus:AFTER[KOPERNICUS]");
            ConfigNode deletor = new ConfigNode("!Body,*");
            root.AddConfigNode(deletor);

            // Save the config
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/systems/" + folder);
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/systems/" + folder + "/Configs");
            ConfigNode wrapper = new ConfigNode();
            wrapper.AddConfigNode(root);
            wrapper.Save(Directory.GetCurrentDirectory() + "/systems/" + folder + "/System.cfg");

            // Sun
            nodes.Add(GenerateSun(system, folder));

            // Select Kerbin
            List<Planet> allBodies = new List<Planet>(system.bodies);
            allBodies = allBodies.Where(p => !p.gas_giant && p.surface_pressure > 0).ToList();
            Kerbin = allBodies[Random.Next(0, allBodies.Count)];

            // Iterate over all bodies in the generated system
            foreach (Planet planet in system.bodies)
            {
                ConfigNode node = GenerateBody(planet, folder);
                nodes.Add(node);
                foreach (Planet moon in planet.bodies_orbiting)
                    nodes.Add(GenerateBody(moon, folder, node.GetValue("name")));
            }

            // Log
            Console.WriteLine("Saving the system");

            // Save the config
            foreach (ConfigNode n in nodes)
            {
                root = new ConfigNode("@Kopernicus:AFTER[KOPERNICUS]");
                root.AddConfigNode(n);
                wrapper = new ConfigNode();
                wrapper.AddConfigNode(root);
                String name = n.HasValue("cbNameLater") ? n.GetValue("cbNameLater") : n.GetValue("name");
                wrapper.Save(Directory.GetCurrentDirectory() + "/systems/" + folder + "/Configs/" + name + ".cfg");            
                
                // Log
                Console.WriteLine($"Saved {name}");
            }

            // Copy files
            foreach (KeyValuePair<String, String> kvP in toCopy)
                File.Copy(kvP.Key, kvP.Value);
        }

        /// <summary>
        /// Creates the ConfigNode for the SunBody
        /// </summary>
        public static ConfigNode GenerateSun(SolarSystem system, String folder)
        {
            // Create the Body node
            String name = GenerateName();
            ConfigNode node = new ConfigNode("Body");
            node.AddValue("name", "Sun");
            node.AddValue("cbNameLater", name);

            // Template
            ConfigNode template = new ConfigNode("Template");
            node.AddConfigNode(template);
            template.AddValue("name", "Sun");

            // Properties
            ConfigNode properties = new ConfigNode("Properties");
            node.AddConfigNode(properties);
            properties.AddValue("radius", "" + system.stellar_radius_ratio * 261600000);
            properties.AddValue("mass", "" + system.stellar_mass_ratio * 1.75656696858329E+28);
            // TODO: Timewarplimits
            properties.AddValue("useTheInName", "False");

            // Log
            Console.WriteLine($"Generated root body named {name}");

            // Scaled Space
            ConfigNode scaled = new ConfigNode("ScaledVersion");
            node.AddConfigNode(scaled);
            ConfigNode mat = new ConfigNode("Material");
            scaled.AddConfigNode(mat);

            // Load database stuff
            Dictionary<String, Dictionary<String, Dictionary<String, Object>>[]> data = Utility.Load<Dictionary<String, Dictionary<String, Dictionary<String, Object>>[]>>("stars.json");
            Dictionary<String, Dictionary<String, Object>> star = data[system.type.star_class][Random.Next(0, data[system.type.star_class].Length)];
            mat.AddValue("emitColor0", star["material"]["emitColor0"].ToString());
            mat.AddValue("emitColor1", star["material"]["emitColor1"].ToString());
            mat.AddValue("sunspotPower", "" + new Range((JObject) star["material"]["sunspotPower"]).Next());
            mat.AddValue("sunspotColor", star["material"]["sunspotColor"].ToString());
            mat.AddValue("rimColor", star["material"]["rimColor"].ToString());
            mat.AddValue("rimPower", "" + new Range((JObject) star["material"]["rimPower"]).Next());
            mat.AddValue("rimBlend", "" + new Range((JObject) star["material"]["rimBlend"]).Next());

            // Light Node
            ConfigNode light = new ConfigNode("Light");
            scaled.AddConfigNode(light);
            light.AddValue("sunlightColor", star["light"]["sunlightColor"].ToString());
            light.AddValue("sunlightIntensity", "" + new Range((JObject) star["light"]["sunlightIntensity"]).Next());
            light.AddValue("sunlightShadowStrength", "" + new Range((JObject)star["light"]["sunlightShadowStrength"]).Next());
            light.AddValue("scaledSunlightColor", star["light"]["scaledSunlightColor"].ToString()); 
            light.AddValue("scaledSunlightIntensity", "" + new Range((JObject)star["light"]["scaledSunlightIntensity"]).Next());
            light.AddValue("IVASunColor", star["light"]["IVASunColor"].ToString());
            light.AddValue("IVASunIntensity", "" + new Range((JObject)star["light"]["IVASunIntensity"]).Next());
            light.AddValue("ambientLightColor", star["light"]["ambientLightColor"].ToString());
            light.AddValue("sunLensFlareColor", star["light"]["sunLensFlareColor"].ToString()); 
            light.AddValue("givesOffLight", "" + (Boolean)star["light"]["givesOffLight"]);
            light.AddValue("luminosity", "" + system.stellar_luminosity_ratio * 1360);

            // TODO: Coronas       

            // Log
            Console.WriteLine($"Generated scaled space for {name}");
            Console.WriteLine();

            // Return it
            return node;
        }

        /// <summary>
        /// Generates the parameters for a planet
        /// </summary>
        public static ConfigNode GenerateBody(Planet planet, String folder, String referenceBody = "Sun")
        {
            String name = GenerateName();
            ConfigNode node = new ConfigNode("Body");

            // Name
            if (planet != Kerbin)
                node.AddValue("name", name);
            else
            {
                node.AddValue("name", "Kerbin");
                node.AddValue("cbNameLater", name);
            }

            // Random Main Menu Body
            node.AddValue("randomMainMenuBody", "" + (referenceBody == "Sun"));

            // Template
            ConfigNode template = new ConfigNode("Template");
            node.AddConfigNode(template);
            if (planet != Kerbin)
            {
                if (planet.gas_giant)
                    template.AddValue("name", "Jool");
                else
                {
                    template.AddValue("name", GetTemplate(planet.surface_pressure > 0.00001, false));
                    template.AddValue("removeAllPQSMods", "True");
                    if (planet.surface_pressure <= 0.00001) 
                        template.AddValue("removeAtmosphere", "True");
                    template.AddValue("removeOcean", "True");
                    // TODO: Handle atmospheres and ocean
                }
            }
            else
            {
                template.AddValue("name", "Kerbin");
                template.AddValue("removePQSMods", "VertexSimplexHeightAbsolute, VertexHeightNoiseVertHeightCurve2, VertexRidgedAltitudeCurve, LandControl, AerialPerspectiveMaterial, VertexHeightMap, QuadEnhanceCoast, MapDecalTangent, MapDecal, FlattenArea[IslandAirfield], PQSCity[KSC2], PQSCity[IslandAirfield]");
                template.AddValue("removeAtmosphere", "True");
                template.AddValue("removeOcean", "True");
            }

            // Properties
            ConfigNode properties = new ConfigNode("Properties");
            node.AddConfigNode(properties);
            properties.AddValue("radius", "" + planet.radius * 100);
            properties.AddValue("geeASL", "" + planet.surface_grav);
            properties.AddValue("rotationPeriod", "" + planet.day * 60 * 60);
            properties.AddValue("tidallyLocked", "" + planet.resonant_period);
            properties.AddValue("initialRotation", "" + Random.Next(0, 361));
            properties.AddValue("albedo", "" + planet.albedo);
            // TODO: Timewarplimits
            properties.AddValue("useTheInName", "False");            
            
            // Log
            Console.WriteLine($"Generated a planet named {name}. GasGiant: {planet.gas_giant}. Template: {template.GetValue("name")}");

            // Color
            Color32 planetColor = Utility.GenerateColor();

            // Orbit
            ConfigNode orbit = new ConfigNode("Orbit");
            node.AddConfigNode(orbit);
            orbit.AddValue("referenceBody", referenceBody);
            orbit.AddValue("inclination", "" + planet.axial_tilt); // Lol
            orbit.AddValue("eccentricity", "" + planet.e);
            orbit.AddValue("semiMajorAxis", "" + planet.a * 100 * Constants.KM_PER_AU);
            orbit.AddValue("longitudeOfAscendingNode", "" + Random.Range(0, 181));
            orbit.AddValue("meanAnomalyAtEpochD", "" + Random.Range(0, 181));
            if (planet.gas_giant)
                orbit.AddValue("color", Parser.WriteColor(Utility.AlterColor(planetColor)));

            // Log
            Console.WriteLine($"Generated orbit around {referenceBody} for {name}");

            // Scaled Space
            ConfigNode scaled = new ConfigNode("ScaledVersion");
            node.AddConfigNode(scaled);
            ConfigNode mat = new ConfigNode("Material");
            scaled.AddConfigNode(mat);
            mat.AddValue("texture", folder + "/PluginData/" + name + "_Texture.png");
            mat.AddValue("normals", folder + "/PluginData/" + name + "_Normals.png");
            if (planet.gas_giant)
                mat.AddValue("color", Parser.WriteColor(Utility.AlterColor(planetColor)));           
            
            // Log
            Console.WriteLine($"Generated scaled space for {name}");

            // Atmosphere
            if (planet.surface_pressure > 0.00001)
            {
                ConfigNode atmosphere = new ConfigNode("Atmosphere");
                node.AddConfigNode(atmosphere);
                atmosphere.AddValue("enabled", "True");
                atmosphere.AddValue("oxygen", "" + Random.Boolean(10));
                atmosphere.AddValue("atmosphereDepth", "" + planet.radius * 100 * Random.Range(0.1, 0.16));
                atmosphere.AddValue("atmosphereMolarMass", "" + planet.molecule_weight / 1000);
                atmosphere.AddValue("staticPressureASL", "" + planet.surface_pressure / 1000 * 101.324996948242);
                atmosphere.AddValue("temperatureSeaLevel", "" + planet.surface_temp);
                if (planet.gas_giant)
                {
                    atmosphere.AddValue("ambientColor", Parser.WriteColor(Utility.AlterColor(planetColor)));
                    atmosphere.AddValue("lightColor", Parser.WriteColor(Utility.AlterColor(Utility.LightColor(planetColor))));
                }
                GenerateAtmosphereCurves(ref atmosphere, planet.gas_giant ? "Jool" : GetTemplate(true, true));            
                
                // Log
                Console.WriteLine($"Generated atmosphere for {name}");
            }

            // Rings :D
            if (planet.gas_giant && Random.Next(0, 100) < 5)
            {
                ConfigNode rings = new ConfigNode("Rings");
                node.AddConfigNode(rings);
                List<Dictionary<String, Object>[]> definitions = Utility.Load<List<Dictionary<String, Object>[]>>("rings.json");
                Dictionary<String, Object>[] def = definitions[Random.Next(0, definitions.Count)];
                foreach (var r in def)
                {
                    ConfigNode ring = new ConfigNode("Ring");
                    rings.AddConfigNode(ring);
                    ring.AddValue("innerRadius", "" + planet.radius * 100 * new Range((JObject) r["innerRadius"]).Next());
                    ring.AddValue("outerRadius", "" + planet.radius * 100 * new Range((JObject) r["outerRadius"]).Next());
                    ring.AddValue("angle", "" + new Range((JObject) r["angle"]).Next());
                    ring.AddValue("color", Parser.WriteColor(Utility.AlterColor(planetColor)));
                    ring.AddValue("lockRotation", "" + (Boolean) r["lockRotation"]);
                    ring.AddValue("unlit", "False");
                }

                // Log
                Console.WriteLine($"Generated rings around {name}");
            }

            // PQS
            if (!planet.gas_giant)
            {
                Color32 average = default(Color32);
                GeneratePQS(ref node, name, folder, planet, ref average);

                // Apply colors
                orbit.AddValue("color", Parser.WriteColor(Utility.AlterColor(average)));
                if (node.HasNode("Atmosphere"))
                {
                    ConfigNode atmosphere = node.GetNode("Atmosphere");
                    atmosphere.AddValue("ambientColor", Parser.WriteColor(Utility.AlterColor(average)));
                    atmosphere.AddValue("lightColor", Parser.WriteColor(Utility.AlterColor(Utility.LightColor(average))));
                }
            }

            // Log
            Console.WriteLine($"Generation of body {name} finished!");
            Console.WriteLine();

            // Return
            return node;
        }

        /// <summary>
        /// Returns a random name for the body
        /// </summary>
        public static String GenerateName()
        {
            Dictionary<String, List<String>> names = Utility.Load<Dictionary<String, List<String>>>("names.json");
            List<String> prefix = names["prefix"];
            List<String> middle = names["middle"];
            List<String> suffix = names["suffix"];
            Boolean hasMiddle = false;

            String name = prefix[Random.Next(0, prefix.Count)];
            if (Random.Next(0, 100) < 50)
            {
                name += middle[Random.Next(0, middle.Count)];
                hasMiddle = true;
            }
            if (Random.Next(0, 100) < 50 || !hasMiddle)
                name += suffix[Random.Next(0, suffix.Count)];
            if (name == "Kerbin" || name == "Kerbol")
                name = GenerateName();
            return name;
        }

        /// <summary>
        /// Gets a random template for the body. Doesn't include Jool, Kerbin and Sun
        /// </summary>
        public static String GetTemplate(Boolean atmosphere, Boolean includeKerbin)
        {
            if (atmosphere)
                return new[] {"Eve", "Duna", "Laythe", "Kerbin"}[Random.Next(0, includeKerbin ? 4 : 3)];
            return Templates[Random.Next(0, Templates.Length)];
        }

        /// <summary>
        /// Adds static atmosphere curves
        /// </summary>
        /// <param name="atmosphere"></param>
        public static void GenerateAtmosphereCurves(ref ConfigNode atmosphere, String template)
        {
            ConfigNode pressure = ConfigNode.Load(Directory.GetCurrentDirectory() + "/data/curves/" + template + "Pressure.cfg");
            pressure.name = "pressureCurve";
            atmosphere.AddConfigNode(pressure);
            atmosphere.AddValue("pressureCurveIsNormalized", "True");
            ConfigNode temperature = ConfigNode.Load(Directory.GetCurrentDirectory() + "/data/curves/" + template + "Temperature.cfg");
            temperature.name = "temperatureCurve";
            atmosphere.AddConfigNode(temperature);
            atmosphere.AddValue("temperatureCurveIsNormalized", "True");
        }

        /// <summary>
        /// Generates a PQS Setup + the Scaled Space Maps needed
        /// </summary>
        /// <returns></returns>
        public static void GeneratePQS(ref ConfigNode node, String name, String folder, Planet planet, ref Color32 average)
        {                
            // Log
            Console.WriteLine("Preparing to load PQS data");

            // Create the node
            ConfigNode pqs = new ConfigNode("PQS");

            // TODO: Material Settings?

            // Create a node for the mods
            ConfigNode mods = new ConfigNode("Mods");
            pqs.AddConfigNode(mods);

            // Load the PQSDatabase and select a setup
            List<Dictionary<String, Dictionary<String, Object>>> data = Utility.Load<List<Dictionary<String, Dictionary<String, Object>>>>("pqs.json");
            Dictionary<String, Dictionary<String, Object>> setup = data[Random.Next(0, data.Count)];

            // Convert the JSON objects to a config node
            foreach (var kvP in setup)
            {
                // Create the Mod node
                ConfigNode mod = new ConfigNode(kvP.Key);
                mods.AddConfigNode(mod);

                // Process the values
                foreach (var moddata in kvP.Value)
                {
                    JToken tmp;
                    if (!(moddata.Value is JObject))
                        mod.AddValue(moddata.Key, moddata.Value.ToString());
                    else if (((JObject) moddata.Value).TryGetValue("min", out tmp) && ((JObject) moddata.Value).TryGetValue("max", out tmp))
                        mod.AddValue(moddata.Key, "" + new Range((JObject) moddata.Value).Next());
                    else
                        mod.AddConfigNode(Utility.JSONToNode(moddata.Key, (JObject) moddata.Value));
                }
            }

            // Create a new PQSObject
            PQS pqsVersion = new PQS(planet.radius * 100);
            List<PQSMod> patchedMods = new List<PQSMod>(); 
            
            // Log
            Console.WriteLine($"Created PQS Object for {name}");

            // Get all loaded types
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToList();

            // Load mods from Config
            foreach (ConfigNode mod in mods.nodes)
            {
                // get the mod type
                if (types.Count(t => t.Name == mod.name) == 0)
                    continue;
                Type loaderType = types.FirstOrDefault(t => t.Name == mod.name);
                String testName = mod.name != "LandControl" ? "PQSMod_" + mod.name : "PQSLandControl";
                Type modType = types.FirstOrDefault(t => t.Name == testName);
                if (loaderType == null || modType == null)
                    continue;

                // Do any PQS Mods already exist on this PQS matching this mod?
                IEnumerable<PQSMod> existingMods = pqsVersion.mods.Where(m => m.GetType() == modType);

                // Create the loader
                Object loader = Activator.CreateInstance(loaderType);

                // Reflection, because C# being silly... :/
                MethodInfo createNew = loaderType.GetMethod("Create", new Type[] { typeof(PQS) });
                MethodInfo create = loaderType.GetMethod("Create", new Type[] { modType });

                if (existingMods.Any())
                {
                    // Attempt to find a PQS mod we can edit that we have not edited before
                    PQSMod existingMod = existingMods.FirstOrDefault(m => !patchedMods.Contains(m) && (!mod.HasValue("name") || (mod.HasValue("index") ? existingMods.ToList().IndexOf(m) == Int32.Parse(mod.GetValue("index")) && m.name == mod.GetValue("name") : m.name == mod.GetValue("name"))));
                    if (existingMod != null)
                    {
                        create.Invoke(loader, new[] { existingMod });
                        ConfigParser.LoadObjectFromConfigurationNode(loader, mod);
                        patchedMods.Add(existingMod);
                    }
                    else
                    {
                        createNew.Invoke(loader, new [] { pqsVersion });
                        ConfigParser.LoadObjectFromConfigurationNode(loader, mod);
                    }
                }
                else
                {
                    createNew.Invoke(loader, new[] { pqsVersion });
                    ConfigParser.LoadObjectFromConfigurationNode(loader, mod);
                }

                // Log
                Console.WriteLine($"Created a new instance of {loaderType.Name} on {name}");
            }            
            
            // Size
            Int32 width = pqsVersion.radius >= 600000 ? 4096 : pqsVersion.radius <= 100000 ? 1024 : 2048;

            // Export ScaledSpace Maps
            Bitmap diffuse = new Bitmap(width, width / 2);
            Bitmap height  = new Bitmap(width, width / 2);                
            
            // Log
            Console.WriteLine($"Exporting Scaled Space maps from the PQS. This could take a while...");

            // Iterate over the PQS
            for (Int32 i = 0; i < width; i++)
            {
                for (Int32 j = 0; j < width / 2; j++)
                {
                    // Create a VertexBuildData
                    VertexBuildData builddata = new VertexBuildData
                    {
                        directionFromCenter = Quaternion.CreateFromAxisAngle(Vector3.Up, 360d / 2048 * i) * Quaternion.CreateFromAxisAngle(Vector3.Right, 90d - 180d / (width / 2) * j) * Vector3.Forward,
                        vertHeight = pqsVersion.radius
                    };

                    // Build the maps
                    foreach (PQSMod m in pqsVersion.mods)
                        m.OnVertexBuildHeight(builddata);
                    foreach (PQSMod m in pqsVersion.mods) // Memory? Who cares
                        m.OnVertexBuild(builddata);

                    Double h = (builddata.vertHeight - pqsVersion.radius);
                    if (h < 0)
                        h = 0;
                    ProceduralQuadSphere.Unity.Color c = builddata.vertColor;
                    c.a = 1f;
                    diffuse.SetPixel(i, j, c);
                    height.SetPixel(i, j, new ProceduralQuadSphere.Unity.Color((Single) h, (Single) h, (Single) h, 1f));
                }
            }

            // Save the textures
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/");
            diffuse.Save(Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/" + name + "_Texture.png", ImageFormat.Png);
            height.Save(Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/" + name + "_Height.png", ImageFormat.Png); // In case you need it :)
            Bitmap normals = Utility.BumpToNormalMap(height, 7); // TODO: Implement something to make strength dynamic
            normals.Save(Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/" + name + "_Normals.png", ImageFormat.Png);

            // Log
            Console.WriteLine($"Saved maps to {Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/"}");

            // Finish
            node.AddConfigNode(pqs);

            // Colors
            average = Utility.GetAverageColor(diffuse);
        }
    }
}