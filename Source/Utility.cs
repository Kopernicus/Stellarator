/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */


using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ConfigNodeParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProceduralQuadSphere.Unity;
using Color = ProceduralQuadSphere.Unity.Color;
using Accrete;

namespace Stellarator
{
    /// <summary>
    /// Tools to help us
    /// </summary>
    public class Utility
    {
        // Credit goes to Kragrathea.
        public static Bitmap BumpToNormalMap(Bitmap source, Single strength)
        {
            strength = Mathf.Clamp(strength, 0.0F, 10.0F);
            var result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            for (Int32 by = 0; @by < result.Height; @by++)
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
        /// Makes a color a bit different
        /// </summary>
        public static Color AlterColor(Color c)
        {
            return new Color((Single) Math.Min(1, c.r * Generator.Random.Range(0.92, 1.02)), (Single) Math.Min(1, c.g * Generator.Random.Range(0.92, 1.02)), (Single) Math.Min(1, c.b * Generator.Random.Range(0.92, 1.02)), c.a);
        }

        /// <summary>
        /// Extracts the average color from a bitmap file
        /// </summary>
        public static Color GetAverageColor(Bitmap bm)
        {
            BitmapData srcData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Int32 stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;
            Int64[] totals = { 0, 0, 0 };

            Int64 width = bm.Width;
            Int64 height = bm.Height;

            unsafe
            {
                Byte* p = (Byte*)(void*)Scan0;
                for (Int32 y = 0; y < height; y++)
                {
                    for (Int32 x = 0; x < width; x++)
                    {
                        for (Int32 color = 0; color < 3; color++)
                        {
                            Int32 idx = y * stride + x * 4 + color;
                            totals[color] += p[idx];
                        }
                    }
                }
            }

            Byte avgB = (Byte)(totals[0] / (width * height));
            Byte avgG = (Byte)(totals[1] / (width * height));
            Byte avgR = (Byte)(totals[2] / (width * height));
            return new Color32(avgR, avgG, avgB, 255);
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
        /// Modifies a color so that it becomes a multiplier color
        /// </summary>
        public static Color ReColor(Color c, Color average)
        {
            // Do some maths..
            Color ret = new Color(c.r / average.r, c.g / average.g, c.b / average.b, 1);
            return ret;
        }


        /// <summary>
        /// Transforms a normal color into the color form Atmosphere from Ground wants
        /// </summary>
        public static Color LightColor(Color c)
        {
            return new Color(1 - c.r, 1 - c.g, 1 - c.b, 1);
        }

        /// <summary>
        /// Generates a random color
        /// </summary>
        /// <returns></returns>
        public static Color GenerateColor()
        {
            Int32 r = Generator.Random.Next(0, 256);
            Int32 g = Generator.Random.Next(0, 256);
            Int32 b = Generator.Random.Next(0, 256);
            return new Color32((Byte)r, (Byte)g, (Byte)b, 255);
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