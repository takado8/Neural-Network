using MNIST.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
namespace DigitReco
{
    class pictureBits
    {
        public byte label;
        public int[,] arr = new int[28, 28];
        public int[] oneDimArr = new int[784];

        public pictureBits(byte _label, int[,] _arr, int[] _oneDimArr)
        {
            oneDimArr = _oneDimArr;
            label = _label;
            arr = _arr;
        }

        public void printDigit()
        {
            for (int i = 0; i < 28; i++)
            {
                for (int k = 0; k < 28; k++)
                {
                    Console.Write(arr[i, k]);
                }
                Console.WriteLine();
            }
            Console.WriteLine(label);
        }

        public static List<pictureBits> readmnist(string imagespath, string labelsPath)
        { 
            var all = new List<pictureBits>();
            var data = FileReaderMNIST.LoadImagesAndLables(
            labelsPath,
            imagespath);

            foreach (var ee in data)
            {
                int[] oneD_arr = new int[784];
                int[,] arr = new int[28, 28];
                for (int i = 0; i < 28; i++)
                {
                    for (int k = 0; k < 28; k++)
                    {
                        if (ee.Image[i, k] > 0)
                        {
                            arr[i, k] = 0;
                            oneD_arr[28 * k + i] = 0;
                        }
                        else
                        {
                            arr[i, k] = 1;
                            oneD_arr[28 * k + i] = 1;
                        }
                    }
                }
                pictureBits pb = new pictureBits(ee.Label, arr, oneD_arr);
                all.Add(pb);
            }
            return all;
        }

        public static int[] image2dTo1d(int[,] arr)
        {
            int[] oneD_arr = new int[784];
           
            for (int i = 0; i < 28; i++)
            {
                for (int k = 0; k < 28; k++)
                {
                    oneD_arr[28 * k + i] = arr[i, k];
                }
            }
            return oneD_arr; 
        }
 
        public static int[,] ImageTo2dIntArray(string path)
        {
            Bitmap bmp = new Bitmap(path);
            int width = bmp.Width;
            int height = bmp.Height;
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            byte[] bytes = new byte[height * data.Stride];
            try
            {
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
            }
            finally
            {
                bmp.UnlockBits(data);
            }
            byte[,] result = new byte[height, width];
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    int offset = y * data.Stride + x * 3;
                    result[y, x] = (byte)((bytes[offset + 0] + bytes[offset + 1] + bytes[offset + 2]) / 3);
                }
            int[,] arr = new int[28, 28];
            for (int i = 0; i < 28; i++)
            {
                for (int k = 0; k < 28; k++)
                {
                    if (result[i, k] > 0)
                    {
                        arr[i, k] = 0;
                    }
                    else
                    {
                        arr[i, k] = 1;
                    }
                }
            }
            return arr;
        }

        public static Bitmap ConvertToBitmap(BitmapSource bitmapSource)
        {
            var width = bitmapSource.PixelWidth;
            var height = bitmapSource.PixelHeight;
            var stride = width * ((bitmapSource.Format.BitsPerPixel + 7) / 8);
            var memoryBlockPointer = Marshal.AllocHGlobal(height * stride);
            bitmapSource.CopyPixels(new System.Windows.Int32Rect(0, 0, width, height), memoryBlockPointer, height * stride, stride);
            var bitmap = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, memoryBlockPointer);
            return bitmap;
        }

        public static byte[] ImageToByte(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
    }
}
