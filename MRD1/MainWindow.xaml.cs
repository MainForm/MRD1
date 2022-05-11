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

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Window = System.Windows.Window;

using MRD1.DeapLearning;

namespace MRD1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoCapture[] cameras = new VideoCapture[2];
        private RITnet __model;

        public UserControl CurrentContent
        {
            get => ViewContentControl.Content as UserControl;
            set => ViewContentControl.Content = value;
        }

        public RITnet Model
        {
            get => __model;
        }

        public MainWindow()
        {
            InitializeComponent();

            for(int i = 0; i < cameras.Length; i++)
                cameras[i] = new VideoCapture();

            var LeftCamera = getCamera(CameraPosition.Left);
            LeftCamera.Open(0);
            LeftCamera.Set(VideoCaptureProperties.FrameHeight, 720);
            LeftCamera.Set(VideoCaptureProperties.FrameWidth, 1280);

            __model = new RITnet("./RITnet.onnx");

            changeContent(new SelectPatient());
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            foreach(VideoCapture cam in cameras)
            {
                if (cam.IsOpened())
                    cam.Release();

                cam.Dispose();
            }
        }

        public VideoCapture getCamera(CameraPosition position)
            => cameras[(int)position];
        
        public void changeContent(UserControl page)
            => ViewContentControl.Content = page;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SelectPaitentListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            CurrentContent = new SelectPatient();
        }

        private void MeasureMRD1ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            changeContent(new MeasureMRD1());
        }

        private void ReplayDataListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            changeContent(new ReplayData());
        }

        private void SettingListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            changeContent(new Setting());
        }
    }
}
