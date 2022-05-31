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

        double __LeftCamera_DistancePerPixel;
        public double LeftCameraDistancePerPixel
        {
            get => __LeftCamera_DistancePerPixel;
            set => SetProperty(ref __LeftCamera_DistancePerPixel, value);
        }

        #endregion

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
    }
}
