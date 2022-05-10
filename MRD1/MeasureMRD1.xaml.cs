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

using MRD1.ViewModel;

namespace MRD1
{
    /// <summary>
    /// MeasureMRD1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MeasureMRD1 : UserControl
    {
        MeasureMRD1ViewModel ViewModel = null;
        public MeasureMRD1()
        {
            InitializeComponent();

            ViewModel = new MeasureMRD1ViewModel();
            DataContext = ViewModel;


        }
        
    }
}
