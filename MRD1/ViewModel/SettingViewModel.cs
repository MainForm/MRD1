using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

using OpenCvSharp;

namespace MRD1.ViewModel
{
    public class SettingViewModel : BaseViewModel
    {
        public VideoCapture leftCamera;
        public VideoCapture rightCamera;

        public SettingViewModel()
        {
            MainWindow MainWindow = Application.Current.MainWindow as MainWindow;

            leftCamera = MainWindow.getCamera(CameraPosition.Left);
            rightCamera = MainWindow.getCamera(CameraPosition.Right);
        }

        ImageSource __leftImage;

        public ImageSource LeftImage
        {
            get => __leftImage;
            set => SetProperty(ref __leftImage, value);
        } 

        ImageSource __rightImage;
        public ImageSource RightImage
        {
            get => __rightImage;
            set => SetProperty(ref __rightImage, value);
        }

        #region leftCamera setting
        double __leftCameraBrightness;
        public double leftCameraBrightness
        {
            get
            {
                return __leftCameraBrightness;
            }
            set
            {
                leftCamera.Brightness = value;
                __leftCameraBrightness = value;
                NotifyPropertyChanged("leftCameraBrightness");
            }
        }

        bool __isLeftCamera_getDistance;
        
        public bool isLeftCamera_getDistance
        {
            get => __isLeftCamera_getDistance;
            set => SetProperty(ref __isLeftCamera_getDistance, value);
        }

        public double? LeftCameraDistancePerPixel
        {
            get => MainWindow.MRD1_Setting.LeftCameraDistancePerPixel;
            set
            {
                MainWindow.MRD1_Setting.LeftCameraDistancePerPixel = value;
                NotifyPropertyChanged("LeftCameraDistancePerPixel");
            }
        }

        #endregion


        #region rightCameraSetting
        double __rightCameraBrightness;
        public double rightCameraBrightness
        {
            get
            {
                return __rightCameraBrightness;
            }
            set
            {
                rightCamera.Brightness = value;
                __rightCameraBrightness = value;
                NotifyPropertyChanged("rightCameraBrightness");
            }
        }

        bool __isRightCamera_getDistance;
        public bool isRightCamera_getDistance
        {
            get => __isRightCamera_getDistance;
            set => SetProperty(ref __isRightCamera_getDistance, value);
        }

        public double? RightCameraDistancePerPixel
        {
            get => MainWindow.MRD1_Setting.RightCameraDistancePerPixel;
            set
            {
                MainWindow.MRD1_Setting.RightCameraDistancePerPixel = value;
                NotifyPropertyChanged("RightCameraDistancePerPixel");
            }
        }

        #endregion
    }
}
