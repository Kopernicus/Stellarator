/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using ConfigNodeParser;
using Microsoft.Xna.Framework;
using ProceduralQuadSphere.Unity;
using XnaGeometry;

namespace Stellarator
{
    public static class Parser
    {
        public static String WriteColor(Color32 c)
        {
            return "RGBA(" + c.r + "," + c.g + "," + c.b + "," + c.a + ")";
        }

        public static Color ParseColor(String vectorString)
        {
            String[] strArrays = vectorString.Split(new[] {',', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            if (strArrays.Length < 3 || strArrays.Length > 4)
                return Color.white;
            if (strArrays.Length == 3)
                return new Color(Single.Parse(strArrays[0]), Single.Parse(strArrays[1]), Single.Parse(strArrays[2]));
            return new Color(Single.Parse(strArrays[0]), Single.Parse(strArrays[1]), Single.Parse(strArrays[2]), Single.Parse(strArrays[3]));
        }

        public static Color32 ParseColor32(String vectorString)
        {
            String[] strArrays = vectorString.Split(new Char[] {',', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            if (strArrays.Length < 3 || strArrays.Length > 4)
                return Color.white;
            if (strArrays.Length == 3)
                return new Color32(Byte.Parse(strArrays[0]), Byte.Parse(strArrays[1]), Byte.Parse(strArrays[2]), 255);
            return new Color32(Byte.Parse(strArrays[0]), Byte.Parse(strArrays[1]), Byte.Parse(strArrays[2]), Byte.Parse(strArrays[3]));
        }

        public static Quaternion ParseQuaternion(String quaternionString)
        {
            String[] strArrays = quaternionString.Split(new[] {',', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            if (strArrays.Length != 4)
                return Quaternion.Identity;
            return new Quaternion(Double.Parse(strArrays[0]), Double.Parse(strArrays[1]), Double.Parse(strArrays[2]), Double.Parse(strArrays[3]));
        }

        public static Vector2 ParseVector2(String vectorString)
        {
            String[] strArrays = vectorString.Split(new Char[] {',', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            if (strArrays.Length != 2)
                return Vector2.Zero;
            return new Vector2(Double.Parse(strArrays[0]), Double.Parse(strArrays[1]));
        }

        public static Vector3 ParseVector3(String vectorString)
        {
            String[] strArrays = vectorString.Split(new[] {',', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            if (strArrays.Length != 3)
                return Vector3.Zero;
            return new Vector3(Double.Parse(strArrays[0]), Double.Parse(strArrays[1]), Double.Parse(strArrays[2]));
        }

        public static Vector4 ParseVector4(String vectorString)
        {
            String[] strArrays = vectorString.Split(new[] {',', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            if (strArrays.Length != 4)
                return Vector4.Zero;
            return new Vector4(Double.Parse(strArrays[0]), Double.Parse(strArrays[1]), Double.Parse(strArrays[2]), Double.Parse(strArrays[3]));
        }

        public static void Load(this Curve curve, ConfigNode node)
        {
            string[] values = node.GetValues("key");
            foreach (String t in values)
            {
                string[] strArrays = t.Split(new [] { ' ', ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                curve.Keys.Add(strArrays.Length != 4 ? new CurveKey(Single.Parse(strArrays[0]), Single.Parse(strArrays[1])) : new CurveKey(Single.Parse(strArrays[0]), Single.Parse(strArrays[1]), Single.Parse(strArrays[2]), Single.Parse(strArrays[3])));
            }
        }
    }
}


