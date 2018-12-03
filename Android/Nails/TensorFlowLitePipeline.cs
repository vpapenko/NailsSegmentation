using Android.App;
using System.IO;
using Xamarin.TensorFlow.Lite;

namespace Nails
{
    public class TensorFlowLitePipeline
        :IPipeline
    {
        public TensorFlowLitePipeline()
        {
            var assets = Application.Context.Assets;

            Java.Nio.ByteBuffer b;
            using (BufferedStream sr = new BufferedStream(assets.Open("model.tflite")))
            {
                byte[] a = new byte[sr.Length];
                sr.Read(a, 0, (int)sr.Length);
                b = Java.Nio.ByteBuffer.Wrap(a);
            }
            Xamarin.TensorFlow.Lite.Interpreter interpreter = new Interpreter(b);
            float[] i = new float[256 * 256];
            float[] o = new float[256 * 256];
            interpreter.Run(i, o);
        }

        public float[] RecognizeImage(float[] input)
        {
            return null;
        }
    }
}
