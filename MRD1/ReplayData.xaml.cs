﻿using System;
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

using System.Diagnostics;

using MRD1.ViewModel;

using OpenCvSharp.WpfExtensions;

namespace MRD1
{
    /// <summary>
    /// ReplayData.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReplayData : UserControl
    {
        ReplayDataViewModel ViewModel;
        
        public ReplayData()
        {
            InitializeComponent();
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow; 


            ViewModel = new ReplayDataViewModel(mainWindow.Connection,mainWindow.selectMeasureID.Value);

            DataContext = ViewModel;
        }

        private void LeftEyeImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pt = e.GetPosition(sender as Image);

            if (e.Delta > 0)
            {
                LeftEyeScaleTransform.ScaleX *= 1.1;
                LeftEyeScaleTransform.ScaleY *= 1.1;
            }
            else
            {
                if (LeftEyeScaleTransform.ScaleX <= 0.5 ||
                    LeftEyeScaleTransform.ScaleY <= 0.5) 
                {
                    LeftEyeScaleTransform.ScaleX = 0.5;
                    LeftEyeScaleTransform.ScaleY = 0.5;
                    return;
                }
                LeftEyeScaleTransform.ScaleX /= 1.1;
                LeftEyeScaleTransform.ScaleY /= 1.1;
            }
        }

        private void RightEyeImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                RightEyeScaleTransform.ScaleX *= 1.1;
                RightEyeScaleTransform.ScaleY *= 1.1;
            }
            else
            {
                if (RightEyeScaleTransform.ScaleX <= 0.5 ||
                    RightEyeScaleTransform.ScaleY <= 0.5)
                {
                    RightEyeScaleTransform.ScaleX = 0.5;
                    RightEyeScaleTransform.ScaleY = 0.5;
                    return;
                }
                RightEyeScaleTransform.ScaleX /= 1.1;
                RightEyeScaleTransform.ScaleY /= 1.1;
            }
        }

        private void previousData_Clicked(object sender, RoutedEventArgs e)
        {
            if(ViewModel.index > 0)
                ViewModel.index -= 1;
        }

        private void nextData_clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.index < ViewModel.DataCount - 1)
                ViewModel.index += 1;
        }

        private void playData_clicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

        }
    }
}
