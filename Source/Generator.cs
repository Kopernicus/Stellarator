using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Accrete;
using ConfigNodeParser;
using Newtonsoft.Json;
using System.Linq;

namespace StellarGenerator
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
        /// This is the core of the whole app. It generates a solar system, based on a seed.
        /// </summary>
        public static void Generate(Int32 seed, String folder)
        {
            // Log
            Console.WriteLine("Generating the solar system...");

            // Create a new Solar System using libaccrete
            SolarSystem system = new SolarSystem(false, true, s => { });
            SolarSystem.Generate(ref system);

            // Reuse the random component
            Random = system.random;

            // Create the root node
            ConfigNode root = new ConfigNode("@Kopernicus:FINAL");

            // TODO: Sun

            // Select Kerbin
            List<Planet> allBodies = new List<Planet>(system.bodies);
            allBodies.AddRange(system.bodies.SelectMany(p => p.bodies_orbiting));
            allBodies = allBodies.Where(p => !p.gas_giant).ToList();
            Kerbin = allBodies[Random.Next(0, allBodies.Count)];

            // Iterate over all bodies in the generated system
            foreach (Planet planet in system.bodies)
                root.AddConfigNode(GenerateBody(planet));

            // Save the config
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/systems/" + folder);
            ConfigNode wrapper = new ConfigNode();
            wrapper.AddConfigNode(root);
            wrapper.Save(Directory.GetCurrentDirectory() + "/systems/" + folder + "/System.cfg");
        }

        /// <summary>
        /// Generates the parameters for a planet
        /// </summary>
        public static ConfigNode GenerateBody(Planet planet, String referenceBody = "Sun")
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
            node.AddValue("randomMainMenuBody", "True");

            // Template
            ConfigNode template = new ConfigNode("Template");
            node.AddConfigNode(template);
            if (planet != Kerbin)
            {
                if (planet.gas_giant)
                    template.AddValue("name", "Jool");
                else
                {
                    template.AddValue("name", GetTemplate());
                    template.AddValue("removeAllPQSMods", "True");
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
            properties.AddValue("radius", "" + planet.radius);
            properties.AddValue("geeASL", "" + planet.surface_grav);
            properties.AddValue("rotationPeriod", "" + planet.day * 60 * 60);
            properties.AddValue("tidallyLocked", "" + planet.resonant_period);
            properties.AddValue("initialRotation", "" + Random.Next(0, 361));
            properties.AddValue("albedo", "" + planet.albedo);
            // TODO: Timewarplimits
            properties.AddValue("useTheInName", "False");

            // Orbit
            ConfigNode orbit = new ConfigNode("Orbit");
            node.AddConfigNode(orbit);
            orbit.AddValue("referenceBody", referenceBody);
            orbit.AddValue("inclination", "" + planet.axial_tilt); // Lol
            orbit.AddValue("eccentricity", "" + planet.e);
            orbit.AddValue("semiMajorAxis", "" + planet.a);
            orbit.AddValue("longitudeOfAscendingNode", "" + Random.Range(0, 181));
            orbit.AddValue("meanAnomalyAtEpochD", "" + Random.Range(0, 181));
            orbit.AddValue("color", GenerateColor());

            // Return
            return node;
        }

        /// <summary>
        /// Deserializes a file from the "data" folder, using JSON.NET
        /// </summary>
        public static T Load<T>(String filename)
        {
            // Get the full path
            String path = Directory.GetCurrentDirectory() + "/data/" + filename;
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        /// <summary>
        /// Returns a random name for the body
        /// </summary>
        public static String GenerateName()
        {
            Dictionary<String, List<String>> names = Load<Dictionary<String, List<String>>>("names.json");
            List<String> prefix = names["prefix"];
            List<String> suffix = names["suffix"];
            return prefix[Random.Next(0, prefix.Count)] + suffix[Random.Next(0, suffix.Count)];
        }

        /// <summary>
        /// Gets a random template for the body. Doesn't include Jool, Kerbin and Sun
        /// </summary>
        public static String GetTemplate()
        {
            return Templates[Random.Next(0, Templates.Length)];
        }

        /// <summary>
        /// Generates a random color
        /// </summary>
        /// <returns></returns>
        public static String GenerateColor()
        {
            Int32 r = Random.Next(0, 256);
            Int32 g = Random.Next(0, 256);
            Int32 b = Random.Next(0, 256);
            return "RGBA(" + r + "," + g + "," + b + ",255)";
        }
    }
}