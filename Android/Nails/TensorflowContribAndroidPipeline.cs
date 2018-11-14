using Android.App;
using Org.Tensorflow.Contrib.Android;

namespace Nails
{
    public class TensorflowContribAndroidPipeline
        :IPipeline
    {
        readonly TensorFlowInferenceInterface inferenceInterface;

        public TensorflowContribAndroidPipeline(string modelName)
        {
            var assets = Application.Context.Assets;
            inferenceInterface = new TensorFlowInferenceInterface(assets, modelName);
        }

        static readonly int ModelInputSize = 256;
        static readonly string InputName = "input";
        static readonly string OutputName = "output/Sigmoid";

        public float[] RecognizeImage(float[] input)
        {
            var outputNames = new[] { OutputName };
            var outputs = new float[ModelInputSize * ModelInputSize];

            inferenceInterface.Feed(InputName, input, 1, ModelInputSize, ModelInputSize, 1);
            inferenceInterface.Run(outputNames);
            inferenceInterface.Fetch(OutputName, outputs);
            return outputs;
        }
    }
}
