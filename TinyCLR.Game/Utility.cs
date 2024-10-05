// Copyright (c) 2012 Chris Taylor

using System;
using System.Drawing;

namespace TinyCLR.Game
{
    public static class Utility
    {
        private static readonly Random _prng = new Random();

        public static Rect[] TileExtractor(int startX, int startY, int spriteWidth, int spriteHeight, int spriteCountX, int spriteCountY, int borderWidth)
        {
            Rect[] rects = new Rect[spriteCountX * spriteCountY];

            int i = 0;
            for (int y = 0; y < spriteCountY; y++)
            {
                for (int x = 0; x < spriteCountX; x++)
                {
                    rects[i].X = startX + (x * spriteWidth) + borderWidth + (borderWidth * x);
                    rects[i].Y = startY + (y * spriteHeight) + borderWidth + (borderWidth * y);
                    rects[i].Width = spriteWidth;
                    rects[i].Height = spriteHeight;
                    i++;
                }
            }

            return rects;
        }

        public static int GetRandomNumber(int maxValue)
        {
            return _prng.Next(maxValue);
        }

        public static void ClearBitmap(Graphics bmp, Brush color)
        {
            bmp.FillRectangle(color, 0, 0, bmp.Width, bmp.Height);
        }

        public static void ClearBitmap(Graphics bmp, int x, int y, int width, int height, Brush color)
        {
            bmp.FillRectangle(color, x, y, width, height);
        }
    }
}
