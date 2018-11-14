using Android.Graphics;
using System;
using System.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace Nails
{
    public class ImageProcess
    {
        static readonly int ModelInputSize = 256;

        public Bitmap BitmapPostrocess(Bitmap original, float[] mask)
        {

            //Bitmap tmp1 = Bitmap.CreateBitmap(2, 2, Bitmap.Config.Argb8888);
            //tmp1.SetPixel(0, 0, new Color(255, 255, 255));
            //tmp1.SetPixel(1, 0, new Color(0, 0, 0));
            //tmp1.SetPixel(0, 1, new Color(255, 0, 0));
            //tmp1.SetPixel(1, 1, new Color(0, 0, 255));

            //var tmp2 = new int[4];
            //tmp1.GetPixels(tmp2, 0, 2, 0, 0, 2, 2);

            int xTarget = original.Width;
            int yTarget = original.Height;

            int[] resultArray = new int[xTarget * yTarget];
            int[] maskScalledArray = new int[xTarget * yTarget];
            Bitmap result = Bitmap.CreateBitmap(xTarget, yTarget, Bitmap.Config.Argb8888);
            original.GetPixels(resultArray, 0, xTarget, 0, 0, xTarget, yTarget);


            float maskMax = mask.Max();
            mask = Array.ConvertAll(mask, v => 255 * (v / maskMax));
            maskMax = mask.Max();
            var maskBitmap = Bitmap.CreateBitmap(ModelInputSize, ModelInputSize, Bitmap.Config.Argb8888);
            var maskInt = Array.ConvertAll(mask, v => (int)(v - 16777216));
            maskBitmap.SetPixels(maskInt, 0, ModelInputSize, 0, 0, ModelInputSize, ModelInputSize);
            var maskScalledBitmap = Bitmap.CreateScaledBitmap(maskBitmap, xTarget, yTarget, true);
            maskScalledBitmap.GetPixels(maskScalledArray, 0, xTarget, 0, 0, xTarget, yTarget);

            int maskVal;
            for (int i = 0; i < resultArray.Length; i++)
            {
                maskVal = (maskScalledArray[i] & 0xFF);
                resultArray[i] = ((resultArray[i]) & 0xFF) * (1 - maskVal / 128)
                    + ((((resultArray[i] >> 8) & 0xFF) * (1 - maskVal / 128)) << 8)
                    + (((resultArray[i] >> 16) & 0xFF) << 16)
                    - 16777216;
            }

            result.SetPixels(resultArray, 0, xTarget, 0, 0, xTarget, yTarget);
            return result;
        }

        public float[] GetBitmapPixels(Bitmap bitmap)
        {

            var floatValues = new float[ModelInputSize * ModelInputSize];

            using (var scaledBitmap = Bitmap.CreateScaledBitmap(bitmap, ModelInputSize, ModelInputSize, false))
            {
                using (var resizedBitmap = scaledBitmap.Copy(Bitmap.Config.Argb8888, false))
                {
                    var intValues = new int[ModelInputSize * ModelInputSize];
                    resizedBitmap.GetPixels(intValues, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);

                    for (int i = 0; i < intValues.Length; ++i)
                    {
                        var val = intValues[i];

                        //floatValues[i] = ((float)(0.3 * (((val >> 16) & 0xFF) - 123) + 0.59 * (((val >> 8) & 0xFF) - 117) + 0.11 * ((val & 0xFF) - 104)) / 256);
                        floatValues[i] = ((float)(((val >> 16) & 0xFF) + ((val >> 8) & 0xFF) + (val & 0xFF)) / 768);
                    }

                    resizedBitmap.Recycle();
                }
                scaledBitmap.Recycle();
            }
            var m1 = floatValues.Max();
            var m2 = floatValues.Min();
            return floatValues;
        }
    }
}