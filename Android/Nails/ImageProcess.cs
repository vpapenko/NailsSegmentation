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

        /// <summary>
        /// Get result image using original image and mask
        /// </summary>
        /// <param name="original"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public Bitmap BitmapPostrocess(Bitmap original, float[] mask)
        {
            int xTarget = original.Width;
            int yTarget = original.Height;

            int[] resultArray = new int[xTarget * yTarget];
            int[] maskScalledArray = new int[xTarget * yTarget];

            //load original image to array.
            //Color of pixel store in one int value in BGRA format (first 8 bits is blue, next 8 bits is green...)
            original.GetPixels(resultArray, 0, xTarget, 0, 0, xTarget, yTarget);

            //Mask normalization
            // (v <= 0.5 ? 0 : 255) normalize mask to 255 for nail pixel and 0 for non nail pixel
            // ... - 16777216 : add 255 to alfa chanel
            // Result: mask data stored in blue chanel + 255 alfa
            var maskInt = Array.ConvertAll(mask, v => (int)((v <= 0.5 ? 0 : 255) - 16777216));

            //Scale mask to result image size
            var maskBitmap = Bitmap.CreateBitmap(ModelInputSize, ModelInputSize, Bitmap.Config.Argb8888);
            maskBitmap.SetPixels(maskInt, 0, ModelInputSize, 0, 0, ModelInputSize, ModelInputSize);
            var maskScalledBitmap = Bitmap.CreateScaledBitmap(maskBitmap, xTarget, yTarget, true);
            maskScalledBitmap.GetPixels(maskScalledArray, 0, xTarget, 0, 0, xTarget, yTarget);
            maskBitmap.Recycle();
            maskScalledBitmap.Recycle();

            //Combine original image and mask
            int maskVal;
            float r_original, g_original, b_original;
            float shift_val = (float)0.25;
            float shift;
            for (int i = 0; i < resultArray.Length; i++)
            {
                //Cut everything exept blue chanel. Result is 
                maskVal = (maskScalledArray[i] & 0xFF);

                b_original = (resultArray[i]) & 0xFF;
                g_original = (resultArray[i] >> 8) & 0xFF;
                r_original = (resultArray[i] >> 16) & 0xFF;

                shift = maskVal <= 128 ? 1 : shift_val;

                resultArray[i] = (int)(b_original * shift)
                    + ((int)(g_original * shift) << 8)
                    + ((int)(r_original) << 16)
                    - 16777216;
            }

            //Load result to bitmap.
            Bitmap result = Bitmap.CreateBitmap(xTarget, yTarget, Bitmap.Config.Argb8888);
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

                        floatValues[i] = ((float)(((val >> 16) & 0xFF) + ((val >> 8) & 0xFF) + (val & 0xFF)) / 768);
                    }
                    resizedBitmap.Recycle();
                }
                scaledBitmap.Recycle();
            }

            return floatValues;
        }
    }
}