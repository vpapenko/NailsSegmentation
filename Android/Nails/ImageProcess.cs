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
            // (v / maskMax) : normalize value to 0<value<1
            // 255 * (...) : scale to 255
            // ... - 16777216 : add 255 to alfa chanel
            // Result: mask data stored in red chanel + 255 alfa
            float maskMax = mask.Max();
            var maskInt = Array.ConvertAll(mask, v => (int)(255 * (v / maskMax) - 16777216));

            //Scale mask to result image size
            var maskBitmap = Bitmap.CreateBitmap(ModelInputSize, ModelInputSize, Bitmap.Config.Argb8888);
            maskBitmap.SetPixels(maskInt, 0, ModelInputSize, 0, 0, ModelInputSize, ModelInputSize);
            var maskScalledBitmap = Bitmap.CreateScaledBitmap(maskBitmap, xTarget, yTarget, true);
            maskScalledBitmap.GetPixels(maskScalledArray, 0, xTarget, 0, 0, xTarget, yTarget);
            maskBitmap.Recycle();
            maskScalledBitmap.Recycle();

            //Combine original image and mask
            int maskVal;
            for (int i = 0; i < resultArray.Length; i++)
            {
                //Cut everything exept red chanel
                maskVal = (maskScalledArray[i] & 0xFF);

                resultArray[i] = ((resultArray[i]) & 0xFF) * (1 - maskVal / 128)
                    + ((((resultArray[i] >> 8) & 0xFF) * (1 - maskVal / 128)) << 8)
                    + (((resultArray[i] >> 16) & 0xFF) << 16)
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