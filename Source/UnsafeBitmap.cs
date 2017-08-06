/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

namespace Stellarator
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public unsafe class UnsafeBitmap : IDisposable
    {
        public UnsafeBitmap(Image bitmap)
        {
            Bitmap = new Bitmap(bitmap);
        }

        public UnsafeBitmap(int width, int height)
        {
            Bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        private BitmapData Data { get; set; }
        private byte* PBase { get; set; } = null;

        // three elements used for MakeGreyUnsafe
        private int Width { get; set; }

        public Bitmap Bitmap { get; }

        public void Dispose()
        {
            Bitmap.Dispose();
        }

        public void LockBitmap()
        {
            var unit = GraphicsUnit.Pixel;
            var boundsF = Bitmap.GetBounds(ref unit);
            var bounds = new Rectangle((int) boundsF.X,
                                       (int) boundsF.Y,
                                       (int) boundsF.Width,
                                       (int) boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            Width = (int) boundsF.Width * sizeof(PixelData);
            if ((Width % 4) != 0)
            {
                Width = 4 * ((Width / 4) + 1);
            }
            Data = Bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            PBase = (byte*) Data.Scan0.ToPointer();
        }

        public Color GetPixel(int x, int y)
        {
            var returnValue = *PixelAt(x, y);
            return returnValue.ToColor();
        }

        public void SetPixel(int x, int y, Color colour)
        {
            var pixel = PixelAt(x, y);
            *pixel = PixelData.FromColor(colour);
        }

        public void UnlockBitmap()
        {
            Bitmap.UnlockBits(Data);
            Data = null;
            PBase = null;
        }

        private PixelData* PixelAt(int x, int y)
        {
            return (PixelData*) (PBase + (y * Width) + (x * sizeof(PixelData)));
        }
    }

    public struct PixelData
    {
        private byte Blue { get; set; }
        private byte Green { get; set; }
        private byte Red { get; set; }

        public Color ToColor()
        {
            return Color.FromArgb(Red, Green, Blue);
        }

        public static PixelData FromColor(Color c)
        {
            return new PixelData {Blue = c.B, Red = c.R, Green = c.G};
        }
    }
}