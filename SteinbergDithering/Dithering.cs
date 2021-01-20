using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteinbergDithering
{
    class Dithering
    {
        private static Bitmap image; //main image
        private static Bitmap bmp; //target image

        private static float errR; //error of Red
        private static float errG; //error of Green
        private static float errB; //error of Blue

        public static Bitmap Make(Bitmap original, int factor, bool isBlackWhite)
        {
            SetImage(original);
            if (isBlackWhite)
            {
                image = PaintItBlack(image);
            }
            return Dither(image, factor);
        }

        private static void SetImage(Bitmap img)
        {
            if (img != null)
            {
                //if not null
                image = img;
                bmp = (Bitmap)image.Clone();
            }
            else
            {
                //if null throw exception.
                throw new NullReferenceException();
            }
        }

        private static Bitmap GetImage()
        {
            return bmp;
        }

        private static Bitmap Dither(Bitmap ditherImage, int factor)
        {
            for (int y = 0; y < ditherImage.Height - 1; y++)
            {
                for (int x = 1; x < ditherImage.Width - 1; x++)
                {
                    Color pix = ditherImage.GetPixel(x, y);
                    float oldR = pix.R;
                    float oldG = pix.G;
                    float oldB = pix.B;
                    //
                    byte newR = (byte)(Math.Round(factor * oldR / 255) * (255 / factor));
                    byte newG = (byte)(Math.Round(factor * oldG / 255) * (255 / factor));
                    byte newB = (byte)(Math.Round(factor * oldB / 255) * (255 / factor));
                    //
                    ditherImage.SetPixel(x, y, Color.FromArgb(newR, newG, newB));
                    //
                    errR = oldR - newR;
                    errG = oldG - newG;
                    errB = oldB - newB;
                    //
                    SetPx(ditherImage, x + 1, y, 7);
                    //
                    SetPx(ditherImage, x - 1, y + 1, 3);
                    //
                    SetPx(ditherImage, x, y + 1, 5);
                    //
                    SetPx(ditherImage, x + 1, y + 1, 1);
                }
            }
            return ditherImage;
        }

        private static void SetPx(Bitmap bmap, int x, int y, int factor)
        {
            //Color c = kitten.GetPixel(x + 1, y);
            Color c = bmap.GetPixel(x, y);
            float r = c.R;
            float g = c.G;
            float b = c.B;

            r = r + (float)(errR * (factor / 16.0));
            g = g + (float)(errG * (factor / 16.0));
            b = b + (float)(errB * (factor / 16.0));
            if (r < 0)
            {
                r = 0;
            }
            else if (r > 255)
            {
                r = 255;
            }
            if (g < 0)
            {
                g = 0;
            }
            else if (g > 255)
            {
                g = 255;
            }
            if (b < 0)
            {
                b = 0;
            }
            else if (b > 255)
            {
                b = 255;
            }
            c = Color.FromArgb((byte)r, (byte)g, (byte)b);
            bmap.SetPixel(x, y, c);
        }

        private static Bitmap PaintItBlack(Bitmap bitmap)
        {
            Bitmap black = (Bitmap)bitmap.Clone();
            for (int i = 0; i < black.Width; i++)
            {
                for (int x = 0; x < black.Height; x++)
                {
                    Color oc = black.GetPixel(i, x);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    black.SetPixel(i, x, nc);
                }
            }
            return black;
        }

        private static byte[,] PaintItBlack(byte[,,] imgArr)
        {
            // R' = G' = B'  = 0.299R + 0.587G + 0.114B
            if (imgArr != null)
            {
                byte[,] gray = new byte[imgArr.GetLength(0), imgArr.GetLength(1)];
                for (int i = 0; i < imgArr.GetLength(0); i++)
                {
                    for (int j = 0; j < imgArr.GetLength(1); j++)
                    {
                        byte grayScale = CheckSize(0.299 * imgArr[i, j, 0] + 0.587 * imgArr[i, j, 1] + 0.114 * imgArr[i, j, 2]);
                        gray[i, j] = grayScale;
                    }
                }
                return gray;
            }
            return null;
        }

        private static byte CheckSize(double input)
        {
            return CheckSize((int)Math.Round(input));
        }

        private static byte CheckSize(int input)
        {
            if (input < 0)
            {
                return 0;
            }
            else if (input > 255)
            {
                return 255;
            }
            else
            {
                return (byte)input;
            }
        }

        public static byte[,,] GetImageArray(Bitmap bmp)
        {
            if (bmp != null)
            {
                int _bmpHeight = bmp.Height;
                int _bmpWidth = bmp.Width;
                if (_bmpWidth > 10 && _bmpHeight > 10)
                {
                    byte[,,] arr = new byte[_bmpHeight, _bmpWidth, 3];
                    for (int i = 0; i < _bmpHeight; i++)
                    {
                        for (int j = 0; j < _bmpWidth; j++)
                        {
                            // For every pixel
                            arr[i, j, 0] = bmp.GetPixel(i, j).R;     // Red
                            arr[i, j, 1] = bmp.GetPixel(i, j).G;    // Green
                            arr[i, j, 2] = bmp.GetPixel(i, j).B;    // Blue
                        }
                    }
                    return arr;
                }
            }
            return null;
        }
    }
}
