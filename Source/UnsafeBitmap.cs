/**
 * Stellarator - Creates procedural systems for Kopernicus
 * Copyright (c) 2016 Thomas P.
 * Licensed under the Terms of the MIT License
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Stellarator
{
    public unsafe class UnsafeBitmap
    {
        readonly Bitmap bitmap;

        // three elements used for MakeGreyUnsafe
        private Int32 width;

        private BitmapData bitmapData;
        private Byte* pBase = null;

        public UnsafeBitmap(Image bitmap)
        {
            this.bitmap = new Bitmap(bitmap);
        }

        public UnsafeBitmap(Int32 width, Int32 height)
        {
            this.bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        public void Dispose()
        {
            bitmap.Dispose();
        }

        public Bitmap Bitmap
        {
            get { return bitmap; }
        }

        private Point PixelSize
        {
            get
            {
                GraphicsUnit unit = GraphicsUnit.Pixel;
                RectangleF bounds = bitmap.GetBounds(ref unit);

                return new Point((Int32) bounds.Width, (Int32) bounds.Height);
            }
        }

        public void LockBitmap()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((Int32) boundsF.X,
                                             (Int32) boundsF.Y,
                                             (Int32) boundsF.Width,
                                             (Int32) boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            width = (Int32) boundsF.Width * sizeof(PixelData);
            if (width % 4 != 0)
                width = 4 * (width / 4 + 1);
            bitmapData = bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            pBase = (Byte*) bitmapData.Scan0.ToPointer();
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
            bitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
        }

        public PixelData* PixelAt(Int32 x, Int32 y)
        {
            return (PixelData*) (pBase + y * width + x * sizeof(PixelData));
        }
    }

    public struct PixelData
    {
        public Byte blue;
        public Byte green;
        public Byte red;

        public Color ToColor()
        {
            return Color.FromArgb(red, green, blue);
        }

        public static PixelData FromColor(Color c)
        {
            return new PixelData {blue = c.B, red = c.R, green = c.G};
        }
    }
}