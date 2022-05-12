using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using OpenCvSharp;

namespace MRD1
{
    public class Algorithm
    {


        public static Mat getPupil(Mat input, Mat Orignal)
        {
            var thres = input.Threshold(2, 255, ThresholdTypes.Binary);
            Mat hierarchy = new Mat();

            Cv2.FindContours(thres, out Mat[] contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

            Cv2.DrawContours(Orignal, contours, 0, new Scalar(0, 255, 0));


            //var param = new SimpleBlobDetector.Params();
            //var blobDetector = SimpleBlobDetector.Create();
            //var keypoints = blobDetector.Detect(thres);
            //Cv2.ImShow("thres", thres);
            //Mat output = new Mat();
            //Cv2.DrawKeypoints(Orignal, keypoints, output, new Scalar(0, 255, 0));

            return Orignal;
        }
    }
}
