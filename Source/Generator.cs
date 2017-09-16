/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

namespace Stellarator
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Accrete;
    using ConfigNodeParser;
    using Database;
    using DynamicExpresso;
    using Kopernicus.Configuration;
    using ProceduralQuadSphere;
    using ProceduralQuadSphere.Unity;
    using XnaGeometry;
    using Color = ProceduralQuadSphere.Unity.Color;

    public class Generator
    {
//TODO: Refactor Generator.cs >> Thomas told me (MartinX3)
        /// <summary>
        ///     The random number generator
        /// </summary>
        public static Random Random { get; private set; }

        /// <summary>
        ///     The seed the system is using
        /// </summary>
        private static Int32 Seed { get; set; }

        /// <summary>
        ///     The Kerbin equivalent for the system
        /// </summary>
        private static Planet Kerbin { get; set; }

        /// <summary>
        ///     This is the core of the whole app. It generates a solar system, based on a seed.
        /// </summary>
        public static void Generate(String seed, String folder, Boolean systematicNames)
        {
            // Log
            Console.WriteLine("Generating the solar system...");
            Console.WriteLine();

            // Create a new Solar System using libaccrete
            SolarSystem system = new SolarSystem(false, true, s => { });
            SolarSystem.Generate(ref system, seed.GetHashCode(), Int32.MaxValue);
            Seed = seed.GetHashCode();

            // Reuse the random component
            Random = system.random;

            // All Nodes
            List<ConfigNode> nodes = new List<ConfigNode>();

            // Save the main config
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/systems/" + folder);
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/systems/" + folder + "/Configs");
            Utility.Save(new ConfigNode("!Body,*"), "@Kopernicus:AFTER[KOPERNICUS]",
                         Directory.GetCurrentDirectory() + "/systems/" + folder + "/System.cfg",
                         String.Format(Templates.Header, seed));

            // Sun
            nodes.Add(GenerateSun(system, systematicNames));

            // Select Kerbin
            List<Planet> allBodies = system.Bodies.Where(p => !p.gas_giant && (p.surface_pressure > 0)).ToList();
            Kerbin = allBodies[Random.Next(0, allBodies.Count)];

            // Define Roman Numerals and letters
            const String moons = "abcdefghijklmnopqrstuvwxyz";
            String[] rN =
            {
                "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X",
                "XI", "XII", "XIII", "XIV", "XV", "XVI", "XVII", "XVIII", "XIX", "XX",
                "XXI", "XXII", "XXIII", "XXIV", "XXV", "XXVI", "XXVII", "XXVIII", "XXIX", "XXX",
                "XXXI", "XXXII", "XXXIII", "XXXIV", "XXXV", "XXXVI", "XXXVII", "XXXVIII", "XXXIX", "XL",
                "XLI", "XLII", "XLIII", "XLIV", "XLV", "XLVI", "XLVII", "XLVIII", "XLIX", "L"
            };

            // Iterate over all bodies in the generated system
            for (Int32 i = 0; i < system.Bodies.Length; i++)
            {
                ConfigNode node = GenerateBody(system[i], folder,
                                               systematicName: systematicNames
                                                                   ? nodes[0].GetValue("cbNameLater") + " " + rN[i]
                                                                   : null);
                nodes.Add(node);
                for (Int32 j = 0; j < system[i].BodiesOrbiting.Length; j++)
                {
                    String name = node.HasValue("cbNameLater") ? node.GetValue("cbNameLater") : node.GetValue("name");
                    nodes.Add(GenerateBody(system[i][j], folder, node.GetValue("name"),
                                           systematicNames ? name + moons[j] : null));
                }
            }

            // Log
            Console.WriteLine("Saving the system");

            // Save the config
            foreach (ConfigNode node in nodes)
            {
                String name = node.HasValue("cbNameLater") ? node.GetValue("cbNameLater") : node.GetValue("name");
                Utility.Save(node, "@Kopernicus:AFTER[KOPERNICUS]",
                             Directory.GetCurrentDirectory() + "/systems/" + folder + "/Configs/" + name + ".cfg",
                             String.Format(Templates.Header, seed));

                // Log
                Console.WriteLine($"Saved {name}");
            }
        }

        /// <summary>
        ///     Creates the ConfigNode for the SunBody
        /// </summary>
        private static ConfigNode GenerateSun(SolarSystem system, Boolean systematicNames)
        {
            // Create the Body node

            String name = Random.Next(0, 100) < (systematicNames ? 50 : 1)
                              ? Utility.SystematicStarName()
                              : Utility.GenerateStarName();

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
            properties.AddValue("radius", "" + (system.stellar_radius_ratio * 261600000));
            properties.AddValue("mass", "" + (system.stellar_mass_ratio * 1.75656696858329E+28));
            properties.AddValue("timewarpAltitudeLimits",
                                String.Join(" ",
                                            Templates.SunTimewarplimits.Select(i => (Int32) (i * system
                                                                                                 .stellar_radius_ratio
                                                                                            ))));
            //properties.AddValue("useTheInName", "False");

            // Log
            Console.WriteLine($"Generated root body named {name}");

            // Scaled Space
            ConfigNode scaled = new ConfigNode("ScaledVersion");
            node.AddConfigNode(scaled);

            // Load database stuff
            ConfigNode starDatabase = Utility.Load("stars");
            ConfigNode data = starDatabase.GetNode(system.type.star_class);
            data = Random.Choose(data.nodes);
            StarPrefab star = Parser.CreateObjectFromConfigNode<StarPrefab>(data);

            // Materials
            ConfigNode mat = new ConfigNode("Material");
            scaled.AddConfigNode(mat);
            mat.AddValue("emitColor0", Parser.WriteColor(star.Material.EmitColor0));
            mat.AddValue("emitColor1", Parser.WriteColor(star.Material.EmitColor1));
            mat.AddValue("sunspotPower", "" + star.Material.SunspotPower);
            mat.AddValue("sunspotColor", Parser.WriteColor(star.Material.SunspotColor));
            mat.AddValue("rimColor", Parser.WriteColor(star.Material.RimColor));
            mat.AddValue("rimPower", "" + star.Material.RimPower);
            mat.AddValue("rimBlend", "" + star.Material.RimBlend);

            // Light Node
            ConfigNode light = new ConfigNode("Light");
            scaled.AddConfigNode(light);
            light.AddValue("sunlightColor", Parser.WriteColor(star.Light.SunlightColor));
            light.AddValue("sunlightIntensity", "" + star.Light.SunlightIntensity);
            light.AddValue("sunlightShadowStrength", "" + star.Light.SunlightShadowStrength);
            light.AddValue("scaledSunlightColor", Parser.WriteColor(star.Light.ScaledSunlightColor));
            light.AddValue("scaledSunlightIntensity", "" + star.Light.ScaledSunlightIntensity);
            light.AddValue("IVASunColor", Parser.WriteColor(star.Light.IvaSunColor));
            light.AddValue("IVASunIntensity", "" + star.Light.IvaSunIntensity);
            light.AddValue("ambientLightColor", Parser.WriteColor(star.Light.AmbientLightColor));
            light.AddValue("sunLensFlareColor", Parser.WriteColor(star.Light.SunLensFlareColor));
            light.AddValue("givesOffLight", "" + star.Light.GivesOffLight);
            light.AddValue("luminosity", "" + (system.stellar_luminosity_ratio * 1360));

            // TODO: Coronas       

            // Log
            Console.WriteLine($"Generated scaled space for {name}");
            Console.WriteLine();

            // Return it
            return node;
        }

        /// <summary>
        ///     Generates the parameters for a planet
        /// </summary>
        private static ConfigNode GenerateBody(Planet planet, String folder, String referenceBody = "Sun",
                                               String systematicName = null)
        {
            String name = systematicName ?? Utility.GenerateName();
            ConfigNode node = new ConfigNode("Body");

            // Name
            if (planet != Kerbin)
            {
                node.AddValue("name", name);
            }
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
                {
                    template.AddValue("name", "Jool");
                }
                else
                {
                    template.AddValue("name", Utility.GetTemplate(planet.surface_pressure > 0.00001, false));
                    template.AddValue("removeAllPQSMods", "True");
                    if (planet.surface_pressure <= 0.00001)
                    {
                        template.AddValue("removeAtmosphere", "True");
                    }
                    template.AddValue("removeOcean", "True");
                    // TODO: Handle atmospheres and ocean
                }
            }
            else
            {
                template.AddValue("name", "Kerbin");
                template.AddValue("removePQSMods",
                                  "PQSLandControl, QuadEnhanceCoast, VertexHeightMap, VertexHeightNoiseVertHeightCurve2, VertexRidgedAltitudeCurve, VertexSimplexHeightAbsolute");
                template.AddValue("removeAtmosphere", "True");
                template.AddValue("removeOcean", "True");
            }

            // Properties
            ConfigNode properties = new ConfigNode("Properties");
            node.AddConfigNode(properties);
            properties.AddValue("radius", "" + (planet.radius * 100));
            properties.AddValue("geeASL", "" + planet.surface_grav);
            properties.AddValue("rotationPeriod", "" + (planet.day * 60 * 60));
            properties.AddValue("tidallyLocked", "" + planet.resonant_period);
            properties.AddValue("initialRotation", "" + Random.Next(0, 361));
            properties.AddValue("albedo", "" + planet.albedo);
            properties.AddValue("timewarpAltitudeLimits",
                                String.Join(" ",
                                            Templates.KerbinTimewarplimits.Select(i =>
                                                                                      (Int32) (i * ((planet.radius *
                                                                                                     100) / 600000)))));
            properties.AddValue("useTheInName", "False");

            // Log
            Console.WriteLine($"Generated a planet named {name}. GasGiant: {planet.gas_giant}. Template: {template.GetValue("name")}");

            // Color
            Color planetColor = Utility.GenerateColor();

            // Orbit
            ConfigNode orbit = new ConfigNode("Orbit");
            node.AddConfigNode(orbit);
            orbit.AddValue("referenceBody", referenceBody);
            orbit.AddValue("eccentricity", "" + planet.e);
            orbit.AddValue("semiMajorAxis", "" + (planet.a * 100 * Constants.KM_PER_AU));
            orbit.AddValue("longitudeOfAscendingNode", "" + Random.Range(0, 181));
            orbit.AddValue("meanAnomalyAtEpochD", "" + Random.Range(0, 181));
            if (planet.gas_giant)
            {
                orbit.AddValue("color", Parser.WriteColor(Utility.AlterColor(planetColor)));
            }
            // Inclination
            orbit.AddValue("inclination", "" + Random.Range(-3, 5));

            // Log
            Console.WriteLine($"Generated orbit around {referenceBody} for {name}");

            // Scaled Space
            ConfigNode scaled = new ConfigNode("ScaledVersion");
            node.AddConfigNode(scaled);

            // Material
            ConfigNode mat = new ConfigNode("Material");
            scaled.AddConfigNode(mat);
            if (!planet.gas_giant)
            {
                mat.AddValue("texture", folder + "/PluginData/" + name + "_Texture.png");
                mat.AddValue("normals", folder + "/PluginData/" + name + "_Normals.png");
            }
            if (planet.gas_giant)
            {
                // Texture
                String[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "/data/textures/");
                String texture = Random.Choose(files);
                String textureName = Path.GetFileNameWithoutExtension(texture) + ".png";
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/");
                File.Copy(texture,
                          Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/" + textureName, true);
                mat.AddValue("texture", folder + "/PluginData/" + textureName);

                // Load the Image
                Color average;
                using (Bitmap image = new Bitmap(Directory.GetCurrentDirectory() + "/systems/" + folder +
                                                 "/PluginData/" +
                                                 textureName))
                {
                    average = Utility.GetAverageColor(image);
                }

                // Scale
                mat.AddValue("mainTexScale",
                             (Random.Next(0, 2) == 1 ? 1 : -1) + "," + (Random.Next(0, 2) == 1 ? 1 : -1));

                mat.AddValue("color", Parser.WriteColor(Utility.ReColor(Utility.AlterColor(planetColor), average)));
                ConfigNode gradient = new ConfigNode("Gradient");
                mat.AddConfigNode(gradient);
                gradient.AddValue("0.0", Parser.WriteColor(Utility.AlterColor(planetColor)));
                gradient.AddValue("0.6", "0.0549,0.0784,0.141,1");
                gradient.AddValue("1.0", "0.0196,0.0196,0.0196,1");
            }

            // Log
            Console.WriteLine($"Generated scaled space for {name}");

            // Atmosphere
            if (planet.surface_pressure > 0.00001)
            {
                ConfigNode atmosphere = new ConfigNode("Atmosphere");
                node.AddConfigNode(atmosphere);
                atmosphere.AddValue("enabled", "True");
                atmosphere.AddValue("oxygen", "" + Random.Boolean(10));
                atmosphere.AddValue("atmosphereDepth", "" + (planet.radius * 100 * Random.Range(0.1, 0.16)));
                atmosphere.AddValue("atmosphereMolarMass", "" + (planet.molecule_weight / 1000));
                atmosphere.AddValue("staticPressureASL", "" + ((planet.surface_pressure / 1000) * 101.324996948242));
                atmosphere.AddValue("temperatureSeaLevel", "" + planet.surface_temp);
                if (planet.gas_giant)
                {
                    atmosphere.AddValue("ambientColor", Parser.WriteColor(Utility.AlterColor(planetColor)));
                    atmosphere.AddValue("lightColor",
                                        Parser.WriteColor(Utility.AlterColor(Utility.LightColor(planetColor))));
                }
                GenerateAtmosphereCurves(ref atmosphere, planet.gas_giant ? "Jool" : Utility.GetTemplate(true, true));

                // Log
                Console.WriteLine($"Generated atmosphere for {name}");

                // Ocean
                if (!planet.gas_giant)
                {
                    if (planet.surface_temp < planet.boil_point && (Random.Next(0, 100) < 35))
                    {
                        ConfigNode ocean = new ConfigNode("Ocean");
                        node.AddConfigNode(ocean);
                        ocean.AddValue("density", "" + planet.hydrosphere * (Random.NextDouble() * 2));
                        ocean.AddValue("minLevel", "1");
                        ocean.AddValue("maxLevel", "6");
                        ocean.AddValue("minDetailDistance", "8");
                        ocean.AddValue("maxQuadLengthsPerFrame", "0.03");

                        Color waterColor = Utility.GenerateColor();
                        ocean.AddValue("oceanColor", Parser.WriteColor(waterColor));

                        ConfigNode oceanmat = new ConfigNode("Material");
                        ocean.AddConfigNode(oceanmat);

                        oceanmat.AddValue("colorFromSpace", Parser.WriteColor(waterColor));
                        oceanmat.AddValue("color", Parser.WriteColor(waterColor));

                        ConfigNode oceanfallbackmat = new ConfigNode("FallbackMaterial");
                        ocean.AddConfigNode(oceanfallbackmat);

                        oceanfallbackmat.AddValue("colorFromSpace", Parser.WriteColor(waterColor));
                        oceanfallbackmat.AddValue("color", Parser.WriteColor(waterColor));

                        Console.WriteLine($"Generated ocean for {name}");
                    }
                }
            }

            // Rings :D
            if (planet.gas_giant && (Random.Next(0, 100) < 5))
            {
                ConfigNode rings = new ConfigNode("Rings");
                node.AddConfigNode(rings);
                ConfigNode ringDatatbase = Utility.Load("rings");
                ConfigNode data = Random.Choose(ringDatatbase.nodes);
                RingPrefab def = Parser.CreateObjectFromConfigNode<RingPrefab>(data);
                foreach (RingPrefab.Ring r in def.Rings)
                {
                    ConfigNode ring = new ConfigNode("Ring");
                    rings.AddConfigNode(ring);
                    ring.AddValue("innerRadius", "" + (planet.radius * 0.1 * r.InnerRadius));
                    ring.AddValue("outerRadius", "" + (planet.radius * 0.1 * r.OuterRadius));
                    ring.AddValue("angle", "" + r.Angle);
                    ring.AddValue("color", Parser.WriteColor(Utility.AlterColor(planetColor)));
                    ring.AddValue("lockRotation", "" + r.LockRotation);
                    ring.AddValue("unlit", "False");
                }

                // Log
                Console.WriteLine($"Generated rings around {name}");
            }

            // PQS
            if (!planet.gas_giant)
            {
                Color average;
                GeneratePQS(ref node, name, folder, planet, out average);

                // Apply colors
                orbit.AddValue("color", Parser.WriteColor(Utility.AlterColor(average)));
                if (node.HasNode("Atmosphere"))
                {
                    ConfigNode atmosphere = node.GetNode("Atmosphere");
                    atmosphere.AddValue("ambientColor", Parser.WriteColor(Utility.AlterColor(average)));
                    atmosphere.AddValue("lightColor",
                                        Parser.WriteColor(Utility.AlterColor(Utility.LightColor(average))));
                }
                ConfigNode gradient = new ConfigNode("Gradient");
                mat.AddConfigNode(gradient);
                gradient.AddValue("0.0", Parser.WriteColor(Utility.AlterColor(average)));
                gradient.AddValue("0.6", "0.0549,0.0784,0.141,1");
                gradient.AddValue("1.0", "0.0196,0.0196,0.0196,1");
            }

            // Log
            Console.WriteLine($"Generation of body {name} finished!");
            Console.WriteLine();

            // Return
            return node;
        }

        /// <summary>
        ///     Adds static atmosphere curves
        /// </summary>
        /// <param name="atmosphere"></param>
        /// <param name="template"></param>
        private static void GenerateAtmosphereCurves(ref ConfigNode atmosphere, String template)
        {
            ConfigNode pressure =
                ConfigNode.Load(Directory.GetCurrentDirectory() + "/data/curves/" + template + "Pressure.cfg");
            pressure.name = "pressureCurve";
            atmosphere.AddConfigNode(pressure);
            atmosphere.AddValue("pressureCurveIsNormalized", "True");
            ConfigNode temperature =
                ConfigNode.Load(Directory.GetCurrentDirectory() + "/data/curves/" + template + "Temperature.cfg");
            temperature.name = "temperatureCurve";
            atmosphere.AddConfigNode(temperature);
            atmosphere.AddValue("temperatureCurveIsNormalized", "True");
        }

        /// <summary>
        ///     Generates a PQS Setup + the Scaled Space Maps needed
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        private static void GeneratePQS(ref ConfigNode node, String name, String folder, Planet planet,
                                        out Color average)
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
            ConfigNode pqsDatabase = Utility.Load("pqs");
            List<PQSPreset> data = pqsDatabase.nodes.Select(n => Parser.CreateObjectFromConfigNode<PQSPreset>(n))
                                              .ToList();
            data = data.Where(d => ((planet.radius * 100) > d.MinRadius) && ((planet.radius * 100) < d.MaxRadius))
                       .ToList();
            PQSPreset setup = data[Random.Next(0, data.Count)];

            // Setup the interpreter
            Interpreter interpreter = new Interpreter()
                .SetVariable("planet", planet, typeof(Planet))
                .SetVariable("pqsVersion", setup, typeof(PQSPreset))
                .SetVariable("Random", Random, typeof(Random))
                .SetVariable("Seed", Seed, typeof(Int32))
                .SetVariable("Color", Utility.GenerateColor(), typeof(Color))
                .Reference(typeof(Parser))
                .Reference(typeof(Generator))
                .Reference(typeof(Utility))
                .Reference(typeof(Utils));

            // Transfer the mod nodes and evaluate expressions
            foreach (ConfigNode modNode in setup.Mods.nodes)
            {
                mods.AddConfigNode(Utility.Eval(modNode, interpreter));
            }

            // Create a new PQSObject
            PQS pqsVersion = new PQS(planet.radius * 100);
            List<PQSMod> patchedmods = new List<PQSMod>();

            // Log
            Console.WriteLine($"Created PQS Object for {name}");

            // Get all loaded types
            List<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToList();

            // Load mods from Config
            foreach (ConfigNode mod in mods.nodes)
            {
                // get the mod type
                if (types.Count(t => t.Name == mod.name) == 0)
                {
                    continue;
                }
                Type loaderType = types.FirstOrDefault(t => t.Name == mod.name);
                String testName = mod.name != "LandControl" ? "PQSMod_" + mod.name : "PQSLandControl";
                Type modType = types.FirstOrDefault(t => t.Name == testName);
                if ((loaderType == null) || (modType == null))
                {
                    continue;
                }

                // Do any PQS mods already exist on this PQS matching this mod?
                IEnumerable<PQSMod> existingmods = pqsVersion.mods.Where(m => m.GetType() == modType);

                // Create the loader
                Object loader = Activator.CreateInstance(loaderType);

                // Reflection, because C# being silly... :/
                MethodInfo createNew = loaderType.GetMethod("Create", new[] {typeof(PQS)});
                MethodInfo create = loaderType.GetMethod("Create", new[] {modType});

                IList<PQSMod> pqsmods = existingmods as IList<PQSMod> ?? existingmods.ToList();
                if (pqsmods.Any())
                {
                    // Attempt to find a PQS mod we can edit that we have not edited before
                    PQSMod existingMod = pqsmods.FirstOrDefault(m => !patchedmods.Contains(m) &&
                                                                     (!mod.HasValue("name") ||
                                                                      (mod.HasValue("index")
                                                                           ? (pqsmods.ToList().IndexOf(m) ==
                                                                              Int32.Parse(mod.GetValue("index"))) &&
                                                                             (m.name == mod.GetValue("name"))
                                                                           : m.name == mod.GetValue("name"))));
                    if (existingMod != null)
                    {
                        create.Invoke(loader, new Object[] {existingMod});
                        Parser.LoadObjectFromConfigurationNode(loader, mod);
                        patchedmods.Add(existingMod);
                    }
                    else
                    {
                        createNew.Invoke(loader, new Object[] {pqsVersion});
                        Parser.LoadObjectFromConfigurationNode(loader, mod);
                    }
                }
                else
                {
                    createNew.Invoke(loader, new Object[] {pqsVersion});
                    Parser.LoadObjectFromConfigurationNode(loader, mod);
                }

                // Log
                Console.WriteLine($"Created a new instance of {loaderType.Name} on {name}");
            }

            // Size
            Int32 width = pqsVersion.radius >= 600000 ? 4096 : pqsVersion.radius <= 100000 ? 1024 : 2048;

            // Export ScaledSpace Maps
            using (UnsafeBitmap diffuse = new UnsafeBitmap(width, width / 2))
            {
                using (UnsafeBitmap height = new UnsafeBitmap(width, width / 2))
                {
                    Console.WriteLine("Exporting Scaled Space maps from the PQS. This could take a while...");

                    // Iterate over the PQS
                    pqsVersion.SetupSphere();
                    diffuse.LockBitmap();
                    height.LockBitmap();
                    for (Int32 i = 0; i < width; i++)
                    {
                        for (Int32 j = 0; j < (width / 2); j++)
                        {
                            // Create a VertexBuildData
                            VertexBuildData builddata = new VertexBuildData
                                                        {
                                                            directionFromCenter =
                                                                Quaternion.CreateFromAngleAxis((360d / width) * i,
                                                                                               Vector3.Up) *
                                                                Quaternion
                                                                    .CreateFromAngleAxis(90d - ((180d / (width / 2.0)) * j),
                                                                                         Vector3.Right) *
                                                                Vector3.Forward,
                                                            vertHeight = pqsVersion.radius
                                                        };

                            // Build the maps
                            pqsVersion.OnVertexBuildHeight(builddata);
                            pqsVersion.OnVertexBuild(builddata);
                            builddata.vertColor.a = 1f;
                            Single h = Mathf.Clamp01((Single) ((builddata.vertHeight - pqsVersion.radius) *
                                                               (1d / pqsVersion.radiusMax)));
                            diffuse.SetPixel(i, j, builddata.vertColor);
                            height.SetPixel(i, j, new Color(h, h, h));
                        }
                    }
                    diffuse.UnlockBitmap();
                    height.UnlockBitmap();

                    // Save the textures
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/");
                    using (UnsafeBitmap normals = Utility.BumpToNormalMap(height, 9))
                    {
                        // TODO: Implement something to make strength dynamic
                        diffuse.Bitmap
                               .Save(Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/" + name + "_Texture.png",
                                     ImageFormat.Png);
                        height.Bitmap
                              .Save(Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/" + name + "_Height.png",
                                    ImageFormat.Png); // In case you need it :)
                        normals.Bitmap
                               .Save(Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/" + name + "_Normals.png",
                                     ImageFormat.Png);
                    }
                }

                // Log
                Console.WriteLine($"Saved maps to {Directory.GetCurrentDirectory() + "/systems/" + folder + "/PluginData/"}");

                // Finish
                node.AddConfigNode(pqs);

                // Colors
                average = Utility.GetAverageColor(diffuse.Bitmap);
            }
        }
    }
}