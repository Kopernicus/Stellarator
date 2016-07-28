/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */


using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ConfigNodeParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProceduralQuadSphere.Unity;
using Color = ProceduralQuadSphere.Unity.Color;

namespace Stellarator
{
    /// <summary>
    /// Tools to help us
    /// </summary>
    public class Utility
    {
        // Credit goes to Kragrathea.
        public static Bitmap BumpToNormalMap(Bitmap source, float strength)
        {
            strength = Mathf.Clamp(strength, 0.0F, 10.0F);
            var result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            for (int by = 0; @by < result.Height; @by++)
            {
                for (var bx = 0; bx < result.Width; bx++)
                {
                    Int32 x = bx == 0 ? result.Width : bx;
                    var xLeft = ((Color)source.GetPixel(x - 1, by)).grayscale * strength;
                    x = bx == result.Width - 1 ? 0 : bx;
                    var xRight = ((Color)source.GetPixel(x + 1, by)).grayscale * strength;
                    Int32 y = by == 0 ? result.Height : by;
                    var yUp = ((Color)source.GetPixel(bx, y - 1)).grayscale * strength;
                    y = by == result.Height - 1 ? 0 : by;
                    var yDown = ((Color)source.GetPixel(bx, y + 1)).grayscale * strength;
                    var xDelta = (xLeft - xRight + 1) * 0.5f;
                    var yDelta = (yUp - yDown + 1) * 0.5f;
                    result.SetPixel(bx, @by, new Color(yDelta, yDelta, yDelta, xDelta));
                }
            }
            return result;
        }

        /// <summary>
        /// Converts a JSON Object to a configNode
        /// </summary>
        public static ConfigNode JSONToNode(String name, JToken obj)
        {
            ConfigNode node = new ConfigNode(name);
            foreach (var jToken in obj)
            {
                var x = (JProperty)jToken;
                if (x.Value.HasValues)
                    node.AddConfigNode(JSONToNode(x.Name, x.Value));
                else
                    node.AddValue(x.Name, x.ToObject<String>());
            }
            return node;
        }


        /// <summary>
        /// Transforms a normal color into the color form Atmosphere from Ground wants
        /// </summary>
        public static String LightColor(String c)
        {
            Int32[] components = c.Replace("RGBA(", "").Replace(")", "").Split(',').Select(s => Int32.Parse(s)).ToArray();
            return "RGBA(" + (255 - components[0]) + "," + (255 - components[1]) + "," + (255 - components[2]) + ",255)";
        }

        /// <summary>
        /// Generates a random color
        /// </summary>
        /// <returns></returns>
        public static String GenerateColor()
        {
            Int32 r = Generator.Random.Next(0, 256);
            Int32 g = Generator.Random.Next(0, 256);
            Int32 b = Generator.Random.Next(0, 256);
            return "RGBA(" + r + "," + g + "," + b + ",255)";
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
    }
}