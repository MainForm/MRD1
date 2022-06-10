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

using System.IO;
using Newtonsoft.Json;

using DirectShowLib;

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
            var Names = from cameras in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice) select cameras.Name;
            ViewModel.CameraNames = new System.Collections.ObjectModel.ObservableCollection<string>(Names);

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
                                MainWindow.MRD1_Setting.Save(MainWindow.settingFileName);
                            }
                        }

                        Dispatcher.Invoke(() =>
                        {
                            if (frame.Empty())
                                ViewModel.LeftImage = null;
                            else
                                ViewModel.LeftImage = WriteableBitmapConverter.ToWriteableBitmap(frame);
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ViewModel.LeftImage = null;
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

                        if (ViewModel.isRightCamera_getDistance)
                        {
                            if (getDistancePerFixel(frame, out double distance))
                            {
                                ViewModel.RightCameraDistancePerPixel = distance;
                                ViewModel.isRightCamera_getDistance = false;
                                MainWindow.MRD1_Setting.Save(MainWindow.settingFileName);
                            }
                        }
                        Dispatcher.Invoke(() =>
                        {
                            if (frame.Empty())
                                ViewModel.RightImage = null;
                            else
                                ViewModel.RightImage = WriteableBitmapConverter.ToWriteableBitmap(frame);
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ViewModel.RightImage = null;
                        });
                    }

                    Thread.Sleep(16);
                }
            }
            catch (OperationCanceledException ex)
            {

            }
        }

        bool getDistancePerFixel(Mat frame,out double distance, double block_length = 5)
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
                            distance = block_length / distances.Average();
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

        private void RightCameraDistance_clicked(object sender, RoutedEventArgs e)
        {
            ViewModel.isRightCamera_getDistance = true;
        }

        private void LeftCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }

    public class MRD1Setting
    {
        public double? LeftCameraDistancePerPixel { get; set; } = null;
        public double? RightCameraDistancePerPixel { get; set; } = null;
        public int thickness { get; set; } = 3;

        public double MRD1_Threshold { get; set; } = 1.5f;

        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this));
        }

        public static MRD1Setting Create(string path)
        {
            MRD1Setting MRD1_Setting;
            if (File.Exists(path) == false)
            {
                MRD1_Setting = new MRD1Setting()
                {
                    LeftCameraDistancePerPixel = null,
                    RightCameraDistancePerPixel = null,
                    MRD1_Threshold = 1.5,
                    thickness = 3,
                };

                File.WriteAllText(path, JsonConvert.SerializeObject(MRD1_Setting));
            }
            else
            {
                MRD1_Setting = JsonConvert.DeserializeObject<MRD1Setting>(File.ReadAllText(path));
            }

            return MRD1_Setting;
        }
    }
}
