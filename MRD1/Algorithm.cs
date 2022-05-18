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
        public static (Point2d,double)? getPupil(Mat input)
        {
            var thres = input.Threshold(2, 255, ThresholdTypes.Binary);
            Mat hierarchy = new Mat();

            Cv2.FindContours(thres, out Mat[] contours, hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

            if (contours.Length >= 1)
            {
                contours[0].GetArray(out Point[] contour);
                int argmin = Array.IndexOf(contour, contour.MinBy(pt => pt.X)) + 10;
                int argmax = Array.IndexOf(contour, contour.MaxBy(pt => pt.X)) - 10;

                // Trace.WriteLine($"argmin : {argmin}, argmax : {argmax}");

                if (argmin + 20 < argmax)
                {
                    contour = contour.SubArray(argmin, argmax);
                        
                    return fit_LMS_Circle(contour);

                    //Orignal.Ellipse(Cv2.FitEllipse(contour), new Scalar(255, 255, 0));
                    //Orignal.Ellipse(Cv2.FitEllipseAMS(contour), new Scalar(0, 0, 255));
                    //Orignal.Ellipse(Cv2.FitEllipseDirect(contour), new Scalar(0, 255, 0));
                    // Cv2.DrawContours(Orignal,new Mat[] { Mat.FromArray(contour) }, 0, new Scalar(0, 255, 0));
                }
            }
            return null;
        }

        static (Point2d, double) fit_LMS_Circle(Point[] contour)
        {
            Mat xs = Mat.FromArray(from pt in contour select pt.X);
            Mat ys = Mat.FromArray(from pt in contour select pt.Y);

            xs.ConvertTo(xs, MatType.CV_64FC1);
            ys.ConvertTo(ys, MatType.CV_64FC1);

            Mat J = new Mat();
            Cv2.HConcat(new Mat[] { -2 * xs, -2 * ys, Mat.Ones(new Size(1, contour.Length), MatType.CV_64FC1) }, J);

            Mat Y = -(xs.Mul(xs)) - (ys.Mul(ys));

            Mat X = (J.T() * J).Inv() * J.T() * Y;

            double cx = X.At<double>(0, 0);
            double cy = X.At<double>(1, 0);
            double c = X.At<double>(2, 0);

            double r = Math.Sqrt((cx * cx) + (cy * cy) - c);

            return (new Point2d(cx, cy), r);
        }

        public static int? getMRD1(Mat predict, Point pupil_center)
        {
            int direction = (predict.At<byte>(pupil_center.Y,pupil_center.X) == 0) ? 1 : -1;
            int iy = 0;

            for(iy = pupil_center.Y; ; iy += direction)
            {
                if (iy < 0 || iy >= predict.Height)
                    return null;
                

                if(direction == 1)
                {
                    if (predict.At<byte>(iy, pupil_center.X) != 0)
                        break;
                }
                else
                {
                    if (predict.At<byte>(iy, pupil_center.X) == 0)
                        break;
                }
            }

            return pupil_center.Y - iy;
        }
    }

    public static class ArrayExtention
    {
        public static T[] SubArray<T>(this T[] src,int begin,int end)
        {
            int length = end - begin + 1;

            T[] dest = new T[length];

            Array.Copy(src,begin,dest,0,length);

            return dest;
        }
    }
}
