using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

using System.Threading;

namespace MRD1
{
    /// <summary>
    /// MeasureMRD1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MeasureMRD1 : UserControl
    {
        public MeasureMRD1ViewModel ViewModel = new MeasureMRD1ViewModel();
        MainWindow MainWindow = Application.Current.MainWindow as MainWindow;
        ShowMRD1ViewModel[] ShowMRD1ViewModels = new ShowMRD1ViewModel[2]
        {
            new ShowMRD1ViewModel(),
            new ShowMRD1ViewModel()
        };

        Thread threadPlay;

        public MeasureMRD1()
        {
            InitializeComponent();

            DataContext = ViewModel;

            LeftEyeContentControl.Content = new ShowMRD1(ShowMRD1ViewModels[0]);
            RightEyeContentControl.Content = new ShowMRD1(ShowMRD1ViewModels[1]);

            threadPlay = new Thread(threadFunctionPlay);
            threadPlay.Start();
            
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            threadPlay?.Interrupt();
        }

        private void threadFunctionPlay()
        {
            try
            {
                while (true)
                {
                    Mat[] frames = new Mat[2];

                    for (int i = 0; i < frames.Length; i++)
                    {
                        var camera = MainWindow.getCamera((CameraPosition)i);
                        if (camera.IsOpened() == true)
                        {
                            frames[i] = new Mat();
                            camera.Read(frames[i]);
                        }
                    }

                    for (int i = 0; i < frames.Length; i++)
                    {
                        if (frames[i] != null)
                        {
                            var output = MainWindow.Model.PredictEye(frames[i], new OpenCvSharp.Size(640, 400));

                            output = Algorithm.getPupil(output,frames[i]);

                            Dispatcher.Invoke(() =>
                            {
                                ShowMRD1ViewModels[i].Image = WriteableBitmapConverter.ToWriteableBitmap(output);
                            });
                        }
                    }
                }
            }
            catch (ThreadInterruptedException)
            {

            }
            catch (TaskCanceledException)
            {

            }
            finally
            {

            }
        }
    }
}
