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

        public UnsafeBitmap(Int32 width, Int32 height)
        {
            Bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        private BitmapData Data { get; set; }
        private Byte* PBase { get; set; } = null;

        // three elements used for MakeGreyUnsafe
        private Int32 Width { get; set; }

        public Bitmap Bitmap { get; }

        public void Dispose()
        {
            Bitmap.Dispose();
        }

        public void LockBitmap()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = Bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((Int32) boundsF.X,
                                       (Int32) boundsF.Y,
                                       (Int32) boundsF.Width,
                                       (Int32) boundsF.Height);

            // Figure out the number of Bytes in a row
            // This is rounded up to be a multiple of 4
            // Bytes, since a scan line in an image must always be a multiple of 4 Bytes
            // in length. 
            Width = (Int32) boundsF.Width * sizeof(PixelData);
            if ((Width % 4) != 0)
            {
                Width = 4 * ((Width / 4) + 1);
            }
            Data = Bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            PBase = (Byte*) Data.Scan0.ToPointer();
        }

        public Color GetPixel(Int32 x, Int32 y)
        {
            PixelData returnValue = *PixelAt(x, y);
            return returnValue.ToColor();
        }

        public void SetPixel(Int32 x, Int32 y, Color colour)
        {
            PixelData* pixel = PixelAt(x, y);
            *pixel = PixelData.FromColor(colour);
        }

        public void UnlockBitmap()
        {
            Bitmap.UnlockBits(Data);
            Data = null;
            PBase = null;
        }

        private PixelData* PixelAt(Int32 x, Int32 y)
        {
            return (PixelData*) (PBase + (y * Width) + (x * sizeof(PixelData)));
        }
    }

    public struct PixelData
    {
        private Byte Blue { get; set; }
        private Byte Green { get; set; }
        private Byte Red { get; set; }

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