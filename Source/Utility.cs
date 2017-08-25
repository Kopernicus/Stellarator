/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */


namespace Stellarator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using Accrete;
    using ConfigNodeParser;
    using DynamicExpresso;
    using ProceduralQuadSphere.Unity;
    using Color = ProceduralQuadSphere.Unity.Color;

    /// <summary>
    ///     Tools to help us
    /// </summary>
    public static class Utility
    {
        #region Eval

        /// <summary>
        ///     Tries to evaluate an expression and returns the expression text when it fails
        /// </summary>
        private static String TryEval(this Interpreter interpreter, String expression)
        {
            try
            {
                return interpreter.Eval<Object>(expression).ToString();
            }
            catch (Exception)
            {
                return expression;
            }
        }

        #endregion

        #region Bitmaps

        // Credit goes to Kragrathea.
        public static UnsafeBitmap BumpToNormalMap(UnsafeBitmap source, Single strength)
        {
            strength = Mathf.Clamp(strength, 0.0F, 10.0F);
            UnsafeBitmap result =
                new UnsafeBitmap(new Bitmap(source.Bitmap.Width, source.Bitmap.Height, PixelFormat.Format32bppArgb));
            source.LockBitmap();
            result.LockBitmap();
            for (Int32 by = 0; by < result.Bitmap.Height; by++)
            {
                for (Int32 bx = 0; bx < result.Bitmap.Width; bx++)
                {
                    Int32 x = bx == 0 ? result.Bitmap.Width : bx;
                    Single xLeft = ((Color) source.GetPixel(x - 1, by)).grayscale * strength;
                    x = bx == (result.Bitmap.Width - 1) ? 0 : bx;
                    Single xRight = ((Color) source.GetPixel(x + 1, by)).grayscale * strength;
                    Int32 y = by == 0 ? result.Bitmap.Height : by;
                    Single yUp = ((Color) source.GetPixel(bx, y - 1)).grayscale * strength;
                    y = by == (result.Bitmap.Height - 1) ? 0 : by;
                    Single yDown = ((Color) source.GetPixel(bx, y + 1)).grayscale * strength;
                    Single xDelta = ((xLeft - xRight) + 1) * 0.5f;
                    Single yDelta = ((yUp - yDown) + 1) * 0.5f;
                    result.SetPixel(bx, by, new Color(yDelta, yDelta, yDelta, xDelta));
                }
            }
            source.UnlockBitmap();
            result.UnlockBitmap();
            return result;
        }

        /// <summary>
        ///     Extracts the average color from a bitmap file
        /// </summary>
        public static Color GetAverageColor(Bitmap bm)
        {
            Byte avgB;
            Byte avgG;
            Byte avgR;
            using (UnsafeBitmap bitmap = new UnsafeBitmap(bm))
            {
                bitmap.LockBitmap();
                Int64[] totals = {0, 0, 0};

                Int64 width = bm.Width;
                Int64 height = bm.Height;

                for (Int32 y = 0; y < height; y++)
                {
                    for (Int32 x = 0; x < width; x++)
                    {
                        System.Drawing.Color c = bitmap.GetPixel(x, y);
                        totals[0] += c.B;
                        totals[1] += c.G;
                        totals[2] += c.R;
                    }
                }

                avgB = (Byte) (totals[0] / (width * height));
                avgG = (Byte) (totals[1] / (width * height));
                avgR = (Byte) (totals[2] / (width * height));
                bitmap.UnlockBitmap();
            }
            return new Color32(avgR, avgG, avgB, 255);
        }

        #endregion

        #region Colors

        /// <summary>
        ///     Generates a random color
        /// </summary>
        /// <returns></returns>
        public static Color GenerateColor()
        {
            Int32 r = Generator.Random.Next(0, 256);
            Int32 g = Generator.Random.Next(0, 256);
            Int32 b = Generator.Random.Next(0, 256);
            return new Color32((Byte) r, (Byte) g, (Byte) b, 255);
        }

        /// <summary>
        ///     Makes a color a bit different
        /// </summary>
        public static Color AlterColor(Color c)
        {
            return new Color((Single) Math.Min(1, c.r * Generator.Random.Range(0.92, 1.02)),
                             (Single) Math.Min(1, c.g * Generator.Random.Range(0.92, 1.02)),
                             (Single) Math.Min(1, c.b * Generator.Random.Range(0.92, 1.02)), c.a);
        }

        /// <summary>
        ///     Modifies a color so that it becomes a multiplier color
        /// </summary>
        public static Color ReColor(Color c, Color average)
        {
            // Do some maths..
            return new Color(c.r / average.r, c.g / average.g, c.b / average.b, 1);
        }

        /// <summary>
        ///     Transforms a normal color Int32o the color form Atmosphere from Ground wants
        /// </summary>
        public static Color LightColor(Color c)
        {
            return new Color(1 - c.r, 1 - c.g, 1 - c.b, 1);
        }

        /// <summary>
        ///     Makes a color darker
        /// </summary>
        [SuppressMessage("ReSharper", "UnusedMember.Global")] //Because it is used in VertexPlanet.cfg >> Reflection <3
        public static Color Dark(Color c)
        {
            if ((c.r > 0.5) || (c.g > 0.5) || (c.b > 0.5))
            {
                c = c * new Color(0.5f, 0.5f, 0.5f);
            }
            return c;
        }

        #endregion

        #region ConfigNode

        /// <summary>
        ///     Deserializes a file from the "data" folder, using ConfigNodes
        /// </summary>
        public static ConfigNode Load(String foldername)
        {
            ConfigNode supernode = new ConfigNode("STELLARATOR");
            return Directory
                .GetFiles(Directory.GetCurrentDirectory() + "/data/" + foldername, "*.cfg", SearchOption.AllDirectories)
                .Select(ConfigNode.Load)
                .Aggregate(supernode, (current, n) => Merge(n.GetNode("STELLARATOR"), current));
        }

        /// <summary>
        ///     Merges two config Nodes
        /// </summary>
        private static ConfigNode Merge(ConfigNode from, ConfigNode to)
        {
            foreach (KeyValuePair<String, String> value in from.values)
            {
                to.AddValue(value.Key, value.Value);
            }
            foreach (ConfigNode node in from.nodes)
            {
                if (to.HasNode(node.name))
                {
                    ConfigNode toNode = to.GetNode(node.name);
                    if (toNode == null)
                    {
                        throw new ArgumentNullException(nameof(toNode));
                    }
                    Merge(node, toNode);
                }
                else
                {
                    to.AddConfigNode(node);
                }
            }
            return to;
        }

        /// <summary>
        ///     Evals the values of a confignode
        /// </summary>
        public static ConfigNode Eval(ConfigNode node, Interpreter interpreter)
        {
            for (Int32 i = 0; i < node.CountValues; i++)
            {
                node.SetValue(node.values[i].Key, interpreter.TryEval(node.values[i].Value));
            }
            for (Int32 i = 0; i < node.CountNodes; i++)
            {
                node.nodes[i] = Eval(node.nodes[i], interpreter);
            }
            return node;
        }

        /// <summary>
        ///     Saves a config Node
        /// </summary>
        public static void Save(ConfigNode node, String parent, String path, String header)
        {
            // Create wrapper nodes
            ConfigNode root = new ConfigNode(parent);
            ConfigNode wrapper = new ConfigNode();
            root.AddConfigNode(node);
            wrapper.AddConfigNode(root);

            // Save
            wrapper.Save(path, header);
        }

        #endregion

        #region Random

        /// <summary>
        ///     Returns a random Booleanean
        /// </summary>
        public static Boolean Boolean(this Random random, Int32 prob = 50)
        {
            return random.Next(0, 100) < prob;
        }

        /// <summary>
        ///     Chooses a random element from an array
        /// </summary>
        public static T Choose<T>(this Random r, T[] array)
        {
            return array[r.Next(0, array.Length)];
        }

        #endregion

        #region Template

        /// <summary>
        ///     Returns a random name for the star
        /// </summary>
        public static String GenerateStarName()
        {
            ConfigNode namesDatabase = Load("starnames");
            // Load Greek Letters
            String[] commonLetters = namesDatabase.GetValues("commonLetters");
            String[] rareLetters = namesDatabase.GetValues("rareLetters");
            // Load Greek Characters
            String[] commonChars = namesDatabase.GetValues("commonChars");
            String[] rareChars = namesDatabase.GetValues("rareChars");
            // Load constellation names
            String[] prefix = namesDatabase.GetValues("starprefix");
            String[] middle = namesDatabase.GetValues("starmiddle");
            String[] suffix = namesDatabase.GetValues("starsuffix");
            // Load multiple systems nomenclature
            String[] multiple = namesDatabase.GetValues("starmultiple");
            Boolean useChars = false;

            // Generate Constellation Name First

            String name = Generator.Random.Choose(prefix);
            name += Generator.Random.Choose(middle);
            // Avoid being anal
            if (name == "An")
            {
                name += "n";
            }
            name += Generator.Random.Choose(suffix);

            // Add Letters or Characters
            if (Generator.Random.Next(0, 100) < 25)
            {
                useChars = true;
            }

            // Choose which letter to use
            Int32 letter = Generator.Random.Next(0, 501);
            if (letter < 350)
            {
                if (useChars)
                {
                    name = Generator.Random.Choose(commonChars) + " " + name;
                }
                else
                {
                    name = Generator.Random.Choose(commonLetters) + " " + name;
                }
            }
            else if (letter < 500)
            {
                if (useChars)
                {
                    name = Generator.Random.Choose(rareChars) + " " + name;
                }
                else
                {
                    name = Generator.Random.Choose(rareLetters) + " " + name;
                }
            }
            else
            {
                if (useChars)
                {
                    name = "κ " + name;
                }
                else
                {
                    name = "Kappa " + name;
                }
            }

            // Add Majoris or Minoris

            if (Generator.Random.Next(0, 100) < 10)
            {
                if (Generator.Random.Next(0, 100) < 50)
                {
                    name += " Minoris";
                }
                else
                {
                    name += " Majoris";
                }
            }

            // Add multiple systems nomenclature

            if (Generator.Random.Next(0, 100) < 5)
            {
                name += " " + Generator.Random.Choose(multiple);
            }

            return name;
        }


        /// <summary>
        ///     Returns a systematic name for the star
        /// </summary>
        public static String SystematicStarName()
        {
            ConfigNode namesDatabase = Load("starnames");
            // Load Acronyms
            String[] acronym = namesDatabase.GetValues("acronym");

            String name = Generator.Random.Choose(acronym);
            name += new String('0', Generator.Random.Next(0, 5));
            name += Generator.Random.Next(100, 9999);
            return name;
        }


        /// <summary>
        ///     Returns a random name for the body
        /// </summary>
        public static String GenerateName()
        {
            ConfigNode namesDatabase = Load("names");
            String[] prefix = namesDatabase.GetValues("prefix");
            String[] middle = namesDatabase.GetValues("middle");
            String[] suffix = namesDatabase.GetValues("suffix");
            Boolean hasMiddle = false;

            String name = Generator.Random.Choose(prefix);
            if (Generator.Random.Next(0, 100) < 50)
            {
                name += Generator.Random.Choose(middle);
                hasMiddle = true;
            }
            if ((Generator.Random.Next(0, 100) < 50) || !hasMiddle)
            {
                name += Generator.Random.Choose(suffix);
            }
            if ((name == "Kerbin") || (name == "Kerbol"))
            {
                name = GenerateName();
            }
            return name;
        }

        /// <summary>
        ///     Gets a random template for the body. Doesn't include Jool, Kerbin and Sun
        /// </summary>
        public static String GetTemplate(Boolean atmosphere, Boolean includeKerbin)
        {
            return atmosphere
                       ? new[] {"Eve", "Duna", "Laythe", "Kerbin"}[Generator.Random.Next(0, includeKerbin ? 4 : 3)]
                       : Templates.StockTemplates[Generator.Random.Next(0, Templates.StockTemplates.Length)];
        }

        #endregion
    }
}