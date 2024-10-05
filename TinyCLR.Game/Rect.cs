// Copyright (c) 2012 Chris Taylor

namespace TinyCLR.Game
{
    /// <summary>
    /// Describes the location, width and height of a rectangle.
    /// </summary>
public struct Rect
    {
        /// <summary>
        /// Constructs a new instance of the Rect structure
        /// </summary>
        /// <param name="x">X-coordinate of the upper left corner of the rectangle</param>
        /// <param name="y">Y-coordinate of the upper left corner of the rectangle</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        public Rect(int x, int y, int width, int height)
          : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// X-coordinate of the upper left corner of the rectangle
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y-coordinate of the upper left corner of the rectangle
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Width of the rectangle
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the rectangle
        /// </summary>
        public int Height { get; set; }

        public static Rect Union(Rect a, Rect b)
        {
            int x1 = System.Math.Min(a.X, b.X);
            int y1 = System.Math.Min(a.Y, b.Y);
            int x2 = System.Math.Max(a.X + a.Width, b.X + b.Width);
            int y2 = System.Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
