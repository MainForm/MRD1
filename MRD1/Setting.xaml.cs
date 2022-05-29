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
        Task taskPlay = null;

        public Setting()
        {
            InitializeComponent();

            ViewModel = new SettingViewModel();
            DataContext = ViewModel;

            taskPlay = new Task(threadFunctionPlay, cancelToken.Token);
            taskPlay.Start();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            cancelToken.Cancel();
        }

        void threadFunctionPlay()
        {
            try
            {
                while (true)
                {
                    cancelToken.Token.ThrowIfCancellationRequested();

                    if (ViewModel.leftCamera.IsOpened())
                    {
                        readFrame(ViewModel.leftCamera, out Mat frame);

                        Dispatcher.Invoke(() =>
                        {
                            ViewModel.LeftImage = WriteableBitmapConverter.ToWriteableBitmap(frame);
                        });
                    }

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
            catch(OperationCanceledException ex)
            {
                
            }
        }

        bool readFrame(VideoCapture camera,out Mat frame)
        {
            frame = new Mat();
            if(camera.Read(frame) == false)
                return false;

            return true;
        }
    }
}
