using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;

namespace MRD1.DeapLearning
{
    public class RITnet : IDisposable
    {
        private InferenceSession __session = null;
        public InferenceSession Session
        {
            get => __session;
        }

        public RITnet(string modelPath,bool gpu = true)
        {
            if(gpu == true)
            {
                var gpuOption = SessionOptions.MakeSessionOptionWithCudaProvider();

                __session = new InferenceSession(modelPath, gpuOption);
            }
            else
            {
                __session = new InferenceSession(modelPath);
            }
        }

        public void Dispose()
        {
             Session?.Dispose();
        }

        public Mat PredictEye(Mat input, Size size)
        {
            Mat matInput = new Mat();
            Size outputSize = input.Size();
            input = input.CvtColor(ColorConversionCodes.BGR2GRAY);
            input.ConvertTo(matInput, MatType.CV_32FC1);
            matInput = matInput.Resize(size) / 255.0F;

            //var intput_tensor = MatToTensor(matInput);
            var intput_tensor = matInput.toDenseTensor();
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor<float>("input",intput_tensor)
            };

            matInput.Dispose();

            using (var output = Session.Run(inputs))
            {
                var outTensor = output.ElementAt(0).Value as DenseTensor<byte>;

                return outTensor?.toMat(size).Resize(outputSize);
            }
        }
    }

    public static class TensorConverter
    {
        public static DenseTensor<float> toDenseTensor(this Mat input)
        {
            float[] arr = new float[input.Total()];

            input.GetArray(out arr);

            return new DenseTensor<float>(arr, new[] { 1, 1, input.Height, input.Width });
        }

        public static Mat toMat(this DenseTensor<byte> input, Size size)
        {
            var arr = input.ToArray();

            return new Mat(size.Height, size.Width, MatType.CV_8UC1, arr);
        }
    }
}
