/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ConfigNodeParser;
using ProceduralQuadSphere.Unity;
using Color = ProceduralQuadSphere.Unity.Color;
using Accrete;
using DynamicExpresso;

namespace Stellarator
{
    /// <summary>
    /// Tools to help us
    /// </summary>
    public static class Utility
    {
        // Credit goes to Kragrathea.
        public static Bitmap BumpToNormalMap(Bitmap source, Single strength)
        {
            strength = Mathf.Clamp(strength, 0.0F, 10.0F);
            Bitmap result = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            for (Int32 by = 0; by < result.Height; by++)
            {
                for (Int32 bx = 0; bx < result.Width; bx++)
                {
                    Int32 x = bx == 0 ? result.Width : bx;
                    Single xLeft = ((Color)source.GetPixel(x - 1, by)).grayscale * strength;
                    x = bx == result.Width - 1 ? 0 : bx;
                    Single xRight = ((Color)source.GetPixel(x + 1, by)).grayscale * strength;
                    Int32 y = by == 0 ? result.Height : by;
                    Single yUp = ((Color)source.GetPixel(bx, y - 1)).grayscale * strength;
                    y = by == result.Height - 1 ? 0 : by;
                    Single yDown = ((Color)source.GetPixel(bx, y + 1)).grayscale * strength;
                    Single xDelta = (xLeft - xRight + 1) * 0.5f;
                    Single yDelta = (yUp - yDown + 1) * 0.5f;
                    result.SetPixel(bx, by, new Color(yDelta, yDelta, yDelta, xDelta));
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
        /// Applies the given colors to the bitmap using unsafe code
        /// </summary>
        public static void ApplyData(Bitmap bmp, Color[] colors)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                    bmp.SetPixel(x, y, colors[y * bmp.Width + x]);
            }
        }

        /// <summary>
        /// Modifies a color so that it becomes a multiplier color
        /// </summary>
        public static Color ReColor(Color c, Color average)
        {
            // Do some maths..
            return new Color(c.r / average.r, c.g / average.g, c.b / average.b, 1);
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
        /// Deserializes a file from the "data" folder, using ConfigNodes
        /// </summary>
        public static ConfigNode Load(String foldername)
        {
            ConfigNode supernode = new ConfigNode("STELLARATOR");
            foreach (String file in Directory.GetFiles(Directory.GetCurrentDirectory() + "/data/" + foldername, "*.cfg", SearchOption.AllDirectories))
            {
                ConfigNode n = ConfigNode.Load(file);
                supernode = Merge(n.GetNode("STELLARATOR"), supernode);
            }
            return supernode; 
        }

        /// <summary>
        /// Merges two config Nodes
        /// </summary>
        public static ConfigNode Merge(ConfigNode from, ConfigNode to)
        {
            foreach (var value in from.values)
                to.AddValue(value.Key, value.Value);
            foreach (ConfigNode node in from.nodes)
            {
                if (to.HasNode(node.name))
                {
                    ConfigNode to_ = to.GetNode(node.name);
                    to_ = Merge(node, to_);
                }
                else
                    to.AddConfigNode(node);
            }
            return to;
        }

        /// <summary>
        /// Returns a random boolean
        /// </summary>
        public static Boolean Boolean(this Random random, Int32 prob = 50)
        {
            return random.Next(0, 100) < prob;
        }
        
        /// <summary>
        /// Returns a random name for the body
        /// </summary>
        public static String GenerateName()
        {
            ConfigNode namesDatabase = Load("names");
            String[] prefix = namesDatabase.GetValues("prefix");
            String[] middle = namesDatabase.GetValues("middle");
            String[] suffix = namesDatabase.GetValues("suffix");
            Boolean hasMiddle = false;

            String name = prefix[Generator.Random.Next(0, prefix.Length)];
            if (Generator.Random.Next(0, 100) < 50)
            {
                name += middle[Generator.Random.Next(0, middle.Length)];
                hasMiddle = true;
            }
            if (Generator.Random.Next(0, 100) < 50 || !hasMiddle)
                name += suffix[Generator.Random.Next(0, suffix.Length)];
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
                return new[] { "Eve", "Duna", "Laythe", "Kerbin" }[Generator.Random.Next(0, includeKerbin ? 4 : 3)];
            return Generator.Templates[Generator.Random.Next(0, Generator.Templates.Length)];
        }

        /// <summary>
        /// Evals the values of a confignode
        /// </summary>
        public static ConfigNode Eval(ConfigNode node, Interpreter interpreter)
        {
            for (Int32 i = 0; i < node.CountValues; i++)
                node.SetValue(node.values[i].Key, interpreter.TryEval(node.values[i].Value));
            for (Int32 i = 0; i < node.CountNodes; i++)
                node.nodes[i] = Eval(node.nodes[i], interpreter);
            return node;
        }

        /// <summary>
        /// Tries to evaluate an expression and returns the expression text when it fails
        /// </summary>
        public static String TryEval(this Interpreter interpreter, string expression)
        {
            try
            {
                return interpreter.Eval<Object>(expression).ToString();
            }
            catch (Exception e)
            {
                return expression;
            }
        }
    }
}