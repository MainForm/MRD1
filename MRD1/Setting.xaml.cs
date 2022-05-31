using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MRD1.ViewModel;

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Size = OpenCvSharp.Size;

namespace MRD1
{
    /// <summary>
    /// Setting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Setting : UserControl
    {
        SettingViewModel ViewModel;
        MainWindow MainWindow = Application.Current.MainWindow as MainWindow;

        CancellationTokenSource cancelToken = new CancellationTokenSource();
        Task taskLeftCameraPlay = null;
        Task taskRightCameraPlay = null;

        Size chessboard_size = new Size(5, 3);

        public Setting()
        {
            InitializeComponent();

            ViewModel = new SettingViewModel();
            DataContext = ViewModel;

            taskLeftCameraPlay = new Task(threadFunctionLeftCameraPlay, cancelToken.Token);
            taskLeftCameraPlay.Start();

            taskRightCameraPlay = new Task(threadFunctionRightCameraPlay, cancelToken.Token);
            taskRightCameraPlay.Start();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            cancelToken.Cancel();
        }

        void threadFunctionLeftCameraPlay()
        {
            try
            {
                while (true)
                {
                    cancelToken.Token.ThrowIfCancellationRequested();

                    if (ViewModel.leftCamera.IsOpened())
                    {
                        readFrame(ViewModel.leftCamera, out Mat frame);

                        if (ViewModel.isLeftCamera_getDistance)
                        {
                            if(getDistancePerFixel(frame,out double distance))
                            {
                                ViewModel.LeftCameraDistancePerPixel = distance;
                                ViewModel.isLeftCamera_getDistance = false;
                            }
                        }

                        Dispatcher.Invoke(() =>
                        {
                            ViewModel.LeftImage = WriteableBitmapConverter.ToWriteableBitmap(frame);
                        });
                    }

                    Thread.Sleep(16);
                }
            }
            catch(OperationCanceledException ex)
            {
                
            }
        }

        void threadFunctionRightCameraPlay()
        {
            try
            {
                while (true)
                {
                    cancelToken.Token.ThrowIfCancellationRequested();

                    if (ViewModel.rightCamera.IsOpened())
                    {
                        readFrame(ViewModel.rightCamera, out Mat frame);

                        Dispatcher.Invoke(() =>
                        {
                            ViewModel.RightImage = WriteableBitmapConverter.ToWriteableBitmap(frame);
                        });
                    }

                    Thread.Sleep(16);
                }
            }
            catch (OperationCanceledException ex)
            {

            }
        }

        bool getDistancePerFixel(Mat frame,out double distance)
        {
            using Mat thres = frame
                .CvtColor(ColorConversionCodes.BGR2GRAY)
                .Threshold(150, 255, ThresholdTypes.Binary);

            distance = 0;
            double st = double.MaxValue;

            if (Cv2.FindChessboardCorners(thres, chessboard_size, out Point2f[] coners))
            {
                for (int iy = 0; iy < chessboard_size.Height - 1; iy++)
                {
                    for (int ix = 0; ix < chessboard_size.Width - 1; ix++)
                    {
                        double left = PointExtention.getDistance(
                                            coners[(iy * chessboard_size.Width) + ix],
                                            coners[((iy + 1) * chessboard_size.Width) + ix]
                                        );
                        double right = PointExtention.getDistance(
                                            coners[(iy * chessboard_size.Width) + ix + 1],
                                            coners[((iy + 1) * chessboard_size.Width) + ix + 1]
                                        );
                        double top = PointExtention.getDistance(
                                            coners[(iy * chessboard_size.Width) + ix],
                                            coners[(iy * chessboard_size.Width) + ix + 1]
                                        );
                        double bottom = PointExtention.getDistance(
                                            coners[((iy + 1) * chessboard_size.Width) + ix],
                                            coners[((iy + 1) * chessboard_size.Width) + ix + 1]
                                        );

                        var distances = new List<double>
                        {
                            left, right, top, bottom
                        };
                        double current_st = distances.standardDeviation();
                        if (current_st < st)
                        {
                            distance = 0.5 / distances.Average();
                            st = current_st;
                        }
                    }
                }
                                
            }
            else
            {
                return false;
            }

            return true;
        }

        bool readFrame(VideoCapture camera,out Mat frame)
        {
            frame = new Mat();
            if(camera.Read(frame) == false)
                return false;

            return true;
        }

        private void LeftCameraDistance_clicked(object sender, RoutedEventArgs e)
        {
            ViewModel.isLeftCamera_getDistance = true;
        }
    }

    public class MRD1Setting
    {
        public double LeftCameraDistancePerPixel { get; set; }
        public double RightCameraDistancePerPixel { get; set; }
        public int thickness { get; set; }

        public double MRD1_Threshold { get; set; }
    }
}
