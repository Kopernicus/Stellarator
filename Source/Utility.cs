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
        private static string TryEval(this Interpreter interpreter, string expression)
        {
            try
            {
                return interpreter.Eval<object>(expression).ToString();
            }
            catch (Exception)
            {
                return expression;
            }
        }

        #endregion

        #region Bitmaps

        // Credit goes to Kragrathea.
        public static UnsafeBitmap BumpToNormalMap(UnsafeBitmap source, float strength)
        {
            strength = Mathf.Clamp(strength, 0.0F, 10.0F);
            UnsafeBitmap result =
                new UnsafeBitmap(new Bitmap(source.Bitmap.Width, source.Bitmap.Height, PixelFormat.Format32bppArgb));
            source.LockBitmap();
            result.LockBitmap();
            for (int by = 0; by < result.Bitmap.Height; by++)
            {
                for (int bx = 0; bx < result.Bitmap.Width; bx++)
                {
                    int x = bx == 0 ? result.Bitmap.Width : bx;
                    float xLeft = ((Color) source.GetPixel(x - 1, by)).grayscale * strength;
                    x = bx == (result.Bitmap.Width - 1) ? 0 : bx;
                    float xRight = ((Color) source.GetPixel(x + 1, by)).grayscale * strength;
                    int y = by == 0 ? result.Bitmap.Height : by;
                    float yUp = ((Color) source.GetPixel(bx, y - 1)).grayscale * strength;
                    y = by == (result.Bitmap.Height - 1) ? 0 : by;
                    float yDown = ((Color) source.GetPixel(bx, y + 1)).grayscale * strength;
                    float xDelta = ((xLeft - xRight) + 1) * 0.5f;
                    float yDelta = ((yUp - yDown) + 1) * 0.5f;
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
            byte avgB;
            byte avgG;
            byte avgR;
            using (UnsafeBitmap bitmap = new UnsafeBitmap(bm))
            {
                bitmap.LockBitmap();
                long[] totals = {0, 0, 0};

                long width = bm.Width;
                long height = bm.Height;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        System.Drawing.Color c = bitmap.GetPixel(x, y);
                        totals[0] += c.B;
                        totals[1] += c.G;
                        totals[2] += c.R;
                    }
                }

                avgB = (byte) (totals[0] / (width * height));
                avgG = (byte) (totals[1] / (width * height));
                avgR = (byte) (totals[2] / (width * height));
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
            int r = Generator.Random.Next(0, 256);
            int g = Generator.Random.Next(0, 256);
            int b = Generator.Random.Next(0, 256);
            return new Color32((byte) r, (byte) g, (byte) b, 255);
        }

        /// <summary>
        ///     Makes a color a bit different
        /// </summary>
        public static Color AlterColor(Color c)
        {
            return new Color((float) Math.Min(1, c.r * Generator.Random.Range(0.92, 1.02)),
                             (float) Math.Min(1, c.g * Generator.Random.Range(0.92, 1.02)),
                             (float) Math.Min(1, c.b * Generator.Random.Range(0.92, 1.02)), c.a);
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
        ///     Transforms a normal color into the color form Atmosphere from Ground wants
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
        public static ConfigNode Load(string foldername)
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
            foreach (KeyValuePair<string, string> value in from.values)
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
            for (int i = 0; i < node.CountValues; i++)
            {
                node.SetValue(node.values[i].Key, interpreter.TryEval(node.values[i].Value));
            }
            for (int i = 0; i < node.CountNodes; i++)
            {
                node.nodes[i] = Eval(node.nodes[i], interpreter);
            }
            return node;
        }

        /// <summary>
        ///     Saves a config Node
        /// </summary>
        public static void Save(ConfigNode node, string parent, string path, string header)
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
        ///     Returns a random boolean
        /// </summary>
        public static bool Boolean(this Random random, int prob = 50)
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
        public static string GenerateStarName()
        {
            ConfigNode namesDatabase = Load("starnames");
            // Load Greek Letters
            string[] commonLetters = namesDatabase.GetValues("commonLetters");
            string[] rareLetters = namesDatabase.GetValues("rareLetters");
            // Load Greek Characters
            string[] commonChars = namesDatabase.GetValues("commonChars");
            string[] rareChars = namesDatabase.GetValues("rareChars");
            // Load constellation names
            string[] prefix = namesDatabase.GetValues("starprefix");
            string[] middle = namesDatabase.GetValues("starmiddle");
            string[] suffix = namesDatabase.GetValues("starsuffix");
            // Load multiple systems nomenclature
            string[] multiple = namesDatabase.GetValues("starmultiple");
            bool useChars = false;

            // Generate Constellation Name First

            string name = Generator.Random.Choose(prefix);
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
            int letter = Generator.Random.Next(0, 501);
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
        public static string SystematicStarName()
        {
            ConfigNode namesDatabase = Load("starnames");
            // Load Acronyms
            string[] acronym = namesDatabase.GetValues("acronym");

            string name = Generator.Random.Choose(acronym);
            name += new string('0', Generator.Random.Next(0, 5));
            name += Generator.Random.Next(100, 9999);
            return name;
        }


        /// <summary>
        ///     Returns a random name for the body
        /// </summary>
        public static string GenerateName()
        {
            ConfigNode namesDatabase = Load("names");
            string[] prefix = namesDatabase.GetValues("prefix");
            string[] middle = namesDatabase.GetValues("middle");
            string[] suffix = namesDatabase.GetValues("suffix");
            bool hasMiddle = false;

            string name = Generator.Random.Choose(prefix);
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
        public static string GetTemplate(bool atmosphere, bool includeKerbin)
        {
            return atmosphere
                       ? new[] {"Eve", "Duna", "Laythe", "Kerbin"}[Generator.Random.Next(0, includeKerbin ? 4 : 3)]
                       : Templates.StockTemplates[Generator.Random.Next(0, Templates.StockTemplates.Length)];
        }

        #endregion
    }
}