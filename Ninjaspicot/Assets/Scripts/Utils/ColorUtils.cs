using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZepLink.RiceNinja.Utils
{
    public enum CustomColor
    {
        None,
        Red,
        DarkRed,
        Green,
        White,
        Blue,
        Yellow,
        Black,
        NightBlue,
        MidnightBlue
    }

    public static class ColorUtils
    {
        public static Color Red => new Color(0.87f, 0.23f, 0.23f, 1);
        public static Color Green => new Color(0, 1, 0.5f, 1);
        public static Color White => new Color(1, 1, 1, 1);
        public static Color Blue => new Color(0, 0.8f, 1);
        public static Color NightBlue => new Color(0, .1f, .2f);
        public static Color MidnightBlue => new Color(0, 0, .05f);
        public static Color Yellow => new Color(1, .9f, 0);
        public static Color Black => new Color(0, 0, 0);
        public static Color DarkRed => new Color(.55f, 0, 0);

        public static Dictionary<CustomColor, Color> CustomColors = new Dictionary<CustomColor, Color>
        {
            { CustomColor.White, White },
            { CustomColor.Green, Green },
            { CustomColor.Red, Red },
            { CustomColor.Blue, Blue },
            { CustomColor.Yellow, Yellow },
            { CustomColor.Black, Black },
            { CustomColor.NightBlue, NightBlue },
            { CustomColor.MidnightBlue, MidnightBlue },
            { CustomColor.DarkRed, DarkRed }
        };

        public static Color GetColor(CustomColor color, float alpha = 1)
        {
            if (!CustomColors.ContainsKey(color))
                return White;

            var result = CustomColors[color];

            return new Color(result.r, result.g, result.b, alpha);
        }


        // Shape recognition
        public static List<List<Vector2i>> FindShapes(Texture2D texture)
        {
            var width = texture.width;
            var data = CreateBWData(texture, 0.5f);
            var res = new List<List<Vector2i>>();

            for (int start = FindShapeStart(data, 0); start != -1; start = FindShapeStart(data, start + 1))
            {
                var shape = GetShapePoints(data, start, width);
                res.Add(shape);
            }

            return res;
        }

        // converts the pixel data into a pure data array
        static byte[] CreateBWData(Texture2D aTexture, float aBWThreshold)
        {
            var tmp = aTexture.GetPixels();
            var count = tmp.Length;
            var data = new byte[count];

            for (int i = 0; i < count; i++)
            {
                data[i] = (byte)(tmp[i].grayscale > aBWThreshold ? 1 : 0);
            }

            return data;
        }

        // This simply iterates the image row by row from the bottom to find the next "filled" point
        static int FindShapeStart(byte[] aData, int aStartingIndex)
        {
            var count = aData.Length;

            for (int i = aStartingIndex; i < count; i++)
            {
                if (aData[i] > 0)
                    return i;
            }

            return -1;
        }

        // this is just a helper that translates a point into an index and checks if the point is "filled".
        static bool TestPoint(byte[] aData, int aWidth, Vector2i aPoint)
        {
            int index = aPoint.x + aPoint.y * aWidth;
            if (index > 0 && index < aData.Length)
                return aData[index] > 0;
            return false;
        }

        static List<Vector2i> GetShapePoints(byte[] aData, int aIndex, int aWidth)
        {
            var res = new List<Vector2i>();
            var allPoints = new List<Vector2i>();
            var pointCache = new HashSet<int>();
            var dir = new Vector2i(1, 0);
            var start = new Vector2i(aIndex % aWidth, aIndex / aWidth);
            var point = start;
            var deltaDir = new Vector2i();

            res.Add(point);
            allPoints.Add(point);

            var done = false;
            var maxPoints = 40000;

            while (!done && --maxPoints > 0)
            {
                var d = dir.right;

                for (int i = 0; i < 7; i++)
                {
                    var current = point + d;
                    int index = current.x + current.y * aWidth;
                    // If we are back to the start or if we reached a point we already included
                    if (current == start || pointCache.Contains(index))
                    {
                        done = true;
                        break;
                    }
                    if (TestPoint(aData, aWidth, current))
                    {
                        // valid point, so move on to that point and update dir
                        // so we know where we were coming from
                        point = current;
                        dir = d;
                        var newDelta = current - res.Last();
                        if (deltaDir == newDelta)
                        {
                            res.RemoveAt(res.Count - 1);
                        }
                        res.Add(current);
                        allPoints.Add(current);
                        deltaDir = newDelta;
                        pointCache.Add(index);
                        break;
                    }
                    // rotate 45° each iteration
                    d = d.left45;
                }
            }

            if (deltaDir.normalized == (res.First() - res.Last()).normalized)
            {
                res.RemoveAt(res.Count - 1);
            }

            RemoveShape(aData, aWidth, allPoints);

            return res;
        }
        // removes the pixels of a shape from the data array
        static void RemoveShape(byte[] aData, int aWidth, List<Vector2i> aPoints)
        {
            int count = aPoints.Count;
            var one = new Vector2i(1, 1);
            // image resolution constraints.
            var min = new Vector2i(0, 0);
            var max = new Vector2i(aWidth - 1, aData.Length / aWidth - 1);
            for (int i = 0; i < count; i++)
            {
                // construct valid bottom-left and top-right coordinates
                Vector2i s = Vector2i.Clamp(aPoints[i] - one, min, max);
                Vector2i e = Vector2i.Clamp(aPoints[i] + one, min, max);
                // remove the whole 3x3 area around each point
                for (int y = s.y; y <= e.y; y++)
                {
                    for (int x = s.x; x <= e.x; x++)
                        aData[x + y * aWidth] = 0;
                }
            }
        }
    }


    public struct Vector2i
    {
        public int x;
        public int y;
        public Vector2i(int aX, int aY)
        {
            x = aX; y = aY;
        }

        public static Vector2i operator +(Vector2i aV1, Vector2i aV2)
        {
            return new Vector2i(aV1.x + aV2.x, aV1.y + aV2.y);
        }
        public static Vector2i operator -(Vector2i aV1, Vector2i aV2)
        {
            return new Vector2i(aV1.x - aV2.x, aV1.y - aV2.y);
        }
        public static Vector2i operator +(Vector2i aV1, int aScalar)
        {
            return new Vector2i(aV1.x + aScalar, aV1.y + aScalar);
        }
        public static Vector2i operator +(int aScalar, Vector2i aV1)
        {
            return new Vector2i(aV1.x + aScalar, aV1.y + aScalar);
        }
        public static Vector2i operator -(Vector2i aV1, int aScalar)
        {
            return new Vector2i(aV1.x - aScalar, aV1.y - aScalar);
        }
        public static Vector2i operator -(int aScalar, Vector2i aV1)
        {
            return new Vector2i(-aV1.x + aScalar, -aV1.y + aScalar);
        }
        public static Vector2i operator -(Vector2i aV1)
        {
            return new Vector2i(-aV1.x, -aV1.y);
        }
        public static Vector2i operator *(Vector2i aV1, int aScalar)
        {
            return new Vector2i(aV1.x * aScalar, aV1.y * aScalar);
        }
        public static Vector2i operator *(int aScalar, Vector2i aV1)
        {
            return new Vector2i(aV1.x * aScalar, aV1.y * aScalar);
        }
        public static Vector2i operator /(Vector2i aV1, int aScalar)
        {
            return new Vector2i(aV1.x / aScalar, aV1.y / aScalar);
        }
        public static implicit operator Vector2(Vector2i aV1)
        {
            return new Vector2(aV1.x, aV1.y);
        }
        public static explicit operator Vector2i(Vector2 aV1)
        {
            return new Vector2i(Mathf.RoundToInt(aV1.x), Mathf.RoundToInt(aV1.y));
        }
        public static bool operator ==(Vector2i aV1, Vector2i aV2)
        {
            return aV1.x == aV2.x && aV1.y == aV2.y;
        }
        public static bool operator !=(Vector2i aV1, Vector2i aV2)
        {
            return aV1.x != aV2.x || aV1.y != aV2.y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Vector2i)
                return this == (Vector2i)obj;
            return false;
        }
        public override int GetHashCode()
        {
            return (x.GetHashCode() + y).GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }

        // This only reduces the vector uniformly by the largest component
        // So (2,2) --> (1,1) but (2,1) --> (1,0)
        public Vector2i normalized
        {
            get
            {
                int len = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                return this / len;
            }
        }

        public Vector2i right
        {
            get { return new Vector2i(y, -x); }
        }
        public Vector2i left
        {
            get { return new Vector2i(-y, x); }
        }
        public Vector2i right45
        {
            // (this + right).normalized
            get { return new Vector2i(x + y, y - x).normalized; }
        }
        public Vector2i left45
        {
            // (this + left).normalized
            get { return new Vector2i(x - y, y + x).normalized; }
        }
        // performs a component-wise min operation
        public static Vector2i Min(Vector2i aV1, Vector2i aV2)
        {
            return new Vector2i(Mathf.Min(aV1.x, aV2.x), Mathf.Min(aV1.y, aV2.y));
        }
        // performs a component-wise max operation
        public static Vector2i Max(Vector2i aV1, Vector2i aV2)
        {
            return new Vector2i(Mathf.Max(aV1.x, aV2.x), Mathf.Max(aV1.y, aV2.y));
        }
        // Keep the vector inside the box defined by min and max
        public static Vector2i Clamp(Vector2i aVec, Vector2i aMin, Vector2i aMax)
        {
            aVec.x = Mathf.Clamp(aVec.x, aMin.x, aMax.x);
            aVec.y = Mathf.Clamp(aVec.y, aMin.y, aMax.y);
            return aVec;
        }
    }
}
