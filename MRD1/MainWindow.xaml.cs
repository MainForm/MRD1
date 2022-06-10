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

using System.IO;

using MySql.Data.MySqlClient;

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using Window = System.Windows.Window;

using Newtonsoft.Json;

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
        private MySqlConnection __connection;

        public static string settingFileName = "setting.json";

        public CameraSetting[] CameraSettings = new CameraSetting[2]
        {
            new CameraSetting
            {
                index = 1,
            },
            new CameraSetting
            {
                index = 0,
            },
        };

        public UserControl CurrentContent
        {
            get => ViewContentControl.Content as UserControl;
            set {
                ViewContentControl.Content = value;
            }
        }

        public static MRD1Setting MRD1_Setting;

        public RITnet Model
        {
            get => __model;
        }

        public MySqlConnection Connection
        {
            get => __connection;
        }

        public Patient selectPatient { get; set; } = null;
        public int? selectMeasureID { get; set; } = null;

        public MainWindow()
        {
            InitializeComponent();

            MRD1_Setting = MRD1Setting.Create(settingFileName);

            for(int i = 0; i < cameras.Length; i++)
                cameras[i] = new VideoCapture();

            for(int i = 0;i < 2; i++)
            {
                var RightCamera = getCamera((CameraPosition)i);
                RightCamera.Open(CameraSettings[i].index);
                RightCamera.Set(VideoCaptureProperties.FrameHeight, CameraSettings[i].frame_height);
                RightCamera.Set(VideoCaptureProperties.FrameWidth, CameraSettings[i].frame_width);
            }

            __model = new RITnet("./RITnet.onnx");

            __connection = new MySqlConnection("Server=127.0.0.1;port=3306;Database=mrd1_db;Uid=MRD1;Pwd=''");

            try
            {
                __connection.Open();
            }
            catch(MySqlException e)
            {
                MainSnackbar.MessageQueue.Enqueue("데이터 베이스 연결에 실패 했습니다.");
            }

            CurrentContent = new SelectPatient();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            __connection.Close();
            __connection.Dispose();

            foreach(VideoCapture cam in cameras)
            {
                if (cam.IsOpened())
                    cam.Release();

                cam.Dispose();
            }
        }

        public VideoCapture getCamera(CameraPosition position)
            => cameras[(int)position];
        
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
            CurrentContent = new MeasureMRD1();
        }

        private void ReplayDataListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            CurrentContent = new ReplayData();
        }

        private void SettingListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            CurrentContent = new Setting();
        }
    }
}
