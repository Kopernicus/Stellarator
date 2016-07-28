/**
 * Kopernicus Planetary System Modifier
 * ====================================
 * Created by: BryceSchroeder and Teknoman117 (aka. Nathaniel R. Lewis)
 * Maintained by: Thomas P., NathanKell and KillAshley
 * Additional Content by: Gravitasi, aftokino, KCreator, Padishar, Kragrathea, OvenProofMars, zengei, MrHappyFace
 * -------------------------------------------------------------
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
 * MA 02110-1301  USA
 *
 * This library is intended to be used as a plugin for Kerbal Space Program
 * which is copyright 2011-2015 Squad. Your usage of Kerbal Space Program
 * itself is governed by the terms of its EULA, not the license above.
 *
 * https://kerbalspaceprogram.com
 */

using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using ConfigNodeParser;
using Microsoft.Xna.Framework;
using ProceduralQuadSphere.Unity;
using StellarGenerator;
using XnaGeometry;

namespace Kopernicus
{
    namespace Configuration
    {
        // Simple parser for numerics
        [RequireConfigType(ConfigType.Value)]
        public class NumericParser<T> : IParsable
        {
            public T value;
            public MethodInfo parserMethod;
            public void SetFromString(string s)
            {
                value = (T) parserMethod.Invoke (null, new object[] {s});
            }
            public NumericParser()
            {
                // Get the parse method for this object
                parserMethod = typeof(T).GetMethod ("Parse", new Type[] {typeof(string)});
            }
            public NumericParser(T i) : this()
            {
                value = i;
            }

            // Convert
            public static implicit operator T(NumericParser<T> parser)
            {
                return parser.value;
            }
            public static implicit operator NumericParser<T>(T value)
            {
                return new NumericParser<T>(value);
            }
        }

        // Simple parser for numeric collections
        [RequireConfigType(ConfigType.Value)]
        public class NumericCollectionParser<T> : IParsable
        {
            public List<T> value;
            public MethodInfo parserMethod;
            public void SetFromString (string s)
            {
                // Need a new list
                value = new List<T> ();

                // Get the tokens of this string
                foreach (string e in s.Split(' '))
                {
                    value.Add((T) parserMethod.Invoke (null, new object[] {e}));
                }
            }
            public NumericCollectionParser()
            {
                // Get the parse method for this object
                parserMethod = typeof(T).GetMethod ("Parse", new Type[] {typeof(string)});
            }
            public NumericCollectionParser(T[] i) : this()
            {
                value = new List<T>(i);
            }
            public NumericCollectionParser(List<T> i) : this()
            {
                value = i;
            }

            // Convert
            public static implicit operator T[](NumericCollectionParser<T> parser)
            {
                return parser.value.ToArray();
            }
            public static implicit operator NumericCollectionParser<T>(T[] value)
            {
                return new NumericCollectionParser<T>(value);
            }
        }

        // Simple parser for string arrays
        [RequireConfigType(ConfigType.Value)]
        public class StringCollectionParser : IParsable
        {
            public IList<string> value;
            public void SetFromString (string s)
            {
                // Need a new list
                value = new List<string> (s.Split (',').Select(a => a.Trim()));
            }
            public StringCollectionParser()
            {
                value = new List<string> ();
            }
            public StringCollectionParser(string[] i)
            {
                value = new List<string>(i);
            }
            public StringCollectionParser(IList<string> i)
            {
                value = i;
            }

            // Convert
            public static implicit operator string[](StringCollectionParser parser)
            {
                return parser.value.ToArray();
            }
            public static implicit operator StringCollectionParser(string[] value)
            {
                return new StringCollectionParser(value);
            }
        }

        // Parser for color
        [RequireConfigType(ConfigType.Value)]
        public class ColorParser : IParsable
        {
            public Color value;
            public void SetFromString(string str)
            {
                if (str.StartsWith("RGBA("))
                {
                    str = str.Replace("RGBA(", string.Empty);
                    str = str.Replace(")", string.Empty);
                    str = str.Replace(" ", string.Empty);
                    string[] colorArray = str.Split(',');

                    value = new Color(float.Parse(colorArray[0]) / 255, float.Parse(colorArray[1]) / 255, float.Parse(colorArray[2]) / 255, float.Parse(colorArray[3]) / 255);
                }
                else if (str.StartsWith("RGB("))
                {
                    str = str.Replace("RGB(", string.Empty);
                    str = str.Replace(")", string.Empty);
                    str = str.Replace(" ", string.Empty);
                    string[] colorArray = str.Split(',');

                    value = new Color(float.Parse(colorArray[0]) / 255, float.Parse(colorArray[1]) / 255, float.Parse(colorArray[2]) / 255, 1);
                }
                else if (str.StartsWith("HSBA("))
                {
                    str = str.Replace("HSBA(", string.Empty);
                    str = str.Replace(")", string.Empty);
                    str = str.Replace(" ", string.Empty);
                    string[] colorArray = str.Split(',');

                    // Parse
                    float h = Single.Parse(colorArray[0]) / 255f;
                    float s = Single.Parse(colorArray[1]) / 255f;
                    float b = Single.Parse(colorArray[2]) / 255f;

                    // RGB
                    value = new Color(b, b, b, Single.Parse(colorArray[3]) / 255f);
                    if (s != 0)
                    {
                        float max = b;
                        float dif = b * s;
                        float min = b - dif;
                        h = h * 360f;

                        // Check
                        if (h < 60f)
                        {
                            value.r = max;
                            value.g = h * dif / 60f + min;
                            value.b = min;
                        }
                        else if (h < 120f)
                        {
                            value.r = -(h - 120f) * dif / 60f + min;
                            value.g = max;
                            value.b = min;
                        }
                        else if (h < 180f)
                        {
                            value.r = min;
                            value.g = max;
                            value.b = (h - 120f) * dif / 60f + min;
                        }
                        else if (h < 240f)
                        {
                            value.r = min;
                            value.g = -(h - 240f) * dif / 60f + min;
                            value.b = max;
                        }
                        else if (h < 300f)
                        {
                            value.r = (h - 240f) * dif / 60f + min;
                            value.g = min;
                            value.b = max;
                        }
                        else if (h <= 360f)
                        {
                            value.r = max;
                            value.g = min;
                            value.b = -(h - 360f) * dif / 60 + min;
                        }
                        else
                        {
                            value.r = 0;
                            value.g = 0;
                            value.b = 0;
                        }
                    }
                }
                else
                {
                    value = Parser.ParseColor(str);
                }
            }
            public ColorParser()
            {
                value = Color.white;
            }
            public ColorParser(Color i)
            {
                value = i;
            }

            // Convert
            public static implicit operator Color(ColorParser parser)
            {
                return parser.value;
            }
            public static implicit operator ColorParser(Color value)
            {
                return new ColorParser(value);
            }
        }

        // Parser for enum
        [RequireConfigType(ConfigType.Value)]
        public class EnumParser<T> : IParsable where T : struct, IConvertible
        {
            public T value;
            public void SetFromString(string s)
            {
                value = (T) Enum.Parse(typeof (T), s);
            }
            public EnumParser ()
            {

            }
            public EnumParser (T i)
            {
                value = i;
            }

            // Convert
            public static implicit operator T(EnumParser<T> parser)
            {
                return parser.value;
            }
            public static implicit operator EnumParser<T>(T value)
            {
                return new EnumParser<T>(value);
            }
        }

        // Parser for quaternion
        [RequireConfigType(ConfigType.Value)]
        public class QuaternionParser : IParsable
        {
            public Quaternion value;
            public void SetFromString(string s)
            {
                value = Parser.ParseQuaternion(s);
            }
            public QuaternionParser()
            {

            }
            public QuaternionParser(Quaternion value)
            {
                this.value = value;
            }

            // Convert
            public static implicit operator Quaternion(QuaternionParser parser)
            {
                return parser.value;
            }
            public static implicit operator QuaternionParser(Quaternion value)
            {
                return new QuaternionParser(value);
            }
        }

        // Parser for vec2
        [RequireConfigType(ConfigType.Value)]
        public class Vector2Parser : IParsable
        {
            public Vector2 value;
            public void SetFromString(string s)
            {
                value = Parser.ParseVector2(s);
            }
            public Vector2Parser()
            {

            }
            public Vector2Parser(Vector2 value)
            {
                this.value = value;
            }

            // Convert
            public static implicit operator Vector2(Vector2Parser parser)
            {
                return parser.value;
            }
            public static implicit operator Vector2Parser(Vector2 value)
            {
                return new Vector2Parser(value);
            }
        }

        // Parser for vec3
        [RequireConfigType(ConfigType.Value)]
        public class Vector3Parser : IParsable
        {
            public Vector3 value;
            public void SetFromString(string s)
            {
                value = Parser.ParseVector3(s);
            }
            public Vector3Parser()
            {

            }
            public Vector3Parser(Vector3 value)
            {
                this.value = value;
            }

            // Convert
            public static implicit operator Vector3(Vector3Parser parser)
            {
                return parser.value;
            }
            public static implicit operator Vector3Parser(Vector3 value)
            {
                return new Vector3Parser(value);
            }
        }

        // Parser for vec4
        [RequireConfigType(ConfigType.Value)]
        public class Vector4Parser : IParsable
        {
            public Vector4 value;
            public void SetFromString(string s)
            {
                value = Parser.ParseVector4(s);
            }
            public Vector4Parser()
            {

            }
            public Vector4Parser(Vector4 value)
            {
                this.value = value;
            }

            // Convert
            public static implicit operator Vector4(Vector4Parser parser)
            {
                return parser.value;
            }
            public static implicit operator Vector4Parser(Vector4 value)
            {
                return new Vector4Parser(value);
            }
        }

        // Parser for a float curve
        [RequireConfigType(ConfigType.Node)]
        public class FloatCurveParser : IParserEventSubscriber
        {
            public Curve curve { get; set; }

            // Build the curve from the data found in the node
            void IParserEventSubscriber.Apply(ConfigNode node)
            {
                curve = new Curve();
                curve.Load (node);
            }

            // We don't use this
            void IParserEventSubscriber.PostApply(ConfigNode node) {  }

            // Default constructor
            public FloatCurveParser ()
            {
                this.curve = null;
            }

            // Default constructor
            public FloatCurveParser (Curve curve)
            {
                this.curve = curve;
            }

            // Convert
            public static implicit operator Curve(FloatCurveParser parser)
            {
                return parser.curve;
            }
            public static implicit operator FloatCurveParser(Curve value)
            {
                return new FloatCurveParser(value);
            }
        }
    }
}
