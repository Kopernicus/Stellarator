/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */


namespace Stellarator
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
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
            var result =
                new UnsafeBitmap(new Bitmap(source.Bitmap.Width, source.Bitmap.Height, PixelFormat.Format32bppArgb));
            source.LockBitmap();
            result.LockBitmap();
            for (var by = 0; by < result.Bitmap.Height; by++)
            {
                for (var bx = 0; bx < result.Bitmap.Width; bx++)
                {
                    var x = bx == 0 ? result.Bitmap.Width : bx;
                    var xLeft = ((Color) source.GetPixel(x - 1, by)).grayscale * strength;
                    x = bx == (result.Bitmap.Width - 1) ? 0 : bx;
                    var xRight = ((Color) source.GetPixel(x + 1, by)).grayscale * strength;
                    var y = by == 0 ? result.Bitmap.Height : by;
                    var yUp = ((Color) source.GetPixel(bx, y - 1)).grayscale * strength;
                    y = by == (result.Bitmap.Height - 1) ? 0 : by;
                    var yDown = ((Color) source.GetPixel(bx, y + 1)).grayscale * strength;
                    var xDelta = ((xLeft - xRight) + 1) * 0.5f;
                    var yDelta = ((yUp - yDown) + 1) * 0.5f;
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
            using (var bitmap = new UnsafeBitmap(bm))
            {
                bitmap.LockBitmap();
                long[] totals = {0, 0, 0};

                long width = bm.Width;
                long height = bm.Height;

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var c = bitmap.GetPixel(x, y);
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
            var r = Generator.Random.Next(0, 256);
            var g = Generator.Random.Next(0, 256);
            var b = Generator.Random.Next(0, 256);
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
            var supernode = new ConfigNode("STELLARATOR");
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory() + "/data/" + foldername, "*.cfg",
                                                    SearchOption.AllDirectories))
            {
                var n = ConfigNode.Load(file);
                supernode = Merge(n.GetNode("STELLARATOR"), supernode);
            }
            return supernode;
        }

        /// <summary>
        ///     Merges two config Nodes
        /// </summary>
        private static ConfigNode Merge(ConfigNode from, ConfigNode to)
        {
            foreach (var value in from.values)
            {
                to.AddValue(value.Key, value.Value);
            }
            foreach (var node in from.nodes)
            {
                if (to.HasNode(node.name))
                {
                    var toNode = to.GetNode(node.name);
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
            for (var i = 0; i < node.CountValues; i++)
            {
                node.SetValue(node.values[i].Key, interpreter.TryEval(node.values[i].Value));
            }
            for (var i = 0; i < node.CountNodes; i++)
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
            var root = new ConfigNode(parent);
            var wrapper = new ConfigNode();
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
            var namesDatabase = Load("starnames");
            // Load Greek Letters
            var commonLetters = namesDatabase.GetValues("commonLetters");
            var rareLetters = namesDatabase.GetValues("rareLetters");
            // Load Greek Characters
            var commonChars = namesDatabase.GetValues("commonChars");
            var rareChars = namesDatabase.GetValues("rareChars");
            // Load constellation names
            var prefix = namesDatabase.GetValues("starprefix");
            var middle = namesDatabase.GetValues("starmiddle");
            var suffix = namesDatabase.GetValues("starsuffix");
            // Load multiple systems nomenclature
            var multiple = namesDatabase.GetValues("starmultiple");
            var useChars = false;

            // Generate Constellation Name First

            var name = Generator.Random.Choose(prefix);
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
            var letter = Generator.Random.Next(0, 501);
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
            var namesDatabase = Load("starnames");
            // Load Acronyms
            var acronym = namesDatabase.GetValues("acronym");

            var name = Generator.Random.Choose(acronym);
            name += new string('0', Generator.Random.Next(0, 5));
            name += Generator.Random.Next(100, 9999);
            return name;
        }


        /// <summary>
        ///     Returns a random name for the body
        /// </summary>
        public static string GenerateName()
        {
            var namesDatabase = Load("names");
            var prefix = namesDatabase.GetValues("prefix");
            var middle = namesDatabase.GetValues("middle");
            var suffix = namesDatabase.GetValues("suffix");
            var hasMiddle = false;

            var name = Generator.Random.Choose(prefix);
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