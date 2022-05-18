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

        CancellationTokenSource cancelToken = new CancellationTokenSource();

        Task threadPlay;

        public MeasureMRD1()
        {
            InitializeComponent();

            DataContext = ViewModel;

            LeftEyeContentControl.Content = new ShowMRD1(ShowMRD1ViewModels[0]);
            RightEyeContentControl.Content = new ShowMRD1(ShowMRD1ViewModels[1]);

            threadPlay = new Task(threadFunctionPlay, cancelToken.Token);
            threadPlay.Start();

        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            cancelToken.Cancel();
        }

        private void threadFunctionPlay()
        {
            try
            {
                while (true)
                {
                    cancelToken.Token.ThrowIfCancellationRequested();

                    Mat[] frames = new Mat[2]
                    {
                        new Mat(),
                        new Mat(),
                    };
                    Mat[] predicts = new Mat[2]
                    {
                        new Mat(),
                        new Mat(),
                    };

                    for (int i = 0; i < frames.Length; i++)
                    {
                        var camera = MainWindow.getCamera((CameraPosition)i);
                        if (camera.IsOpened() == true)
                        {
                            camera.Read(frames[i]);
                            predicts[i] = MainWindow.Model.PredictEye(frames[i], new OpenCvSharp.Size(640, 400));
                        }
                    }

                    var postProcess = Task.Factory.StartNew(() =>
                    {
                        cancelToken.Token.ThrowIfCancellationRequested();
                        RecordData[] records = new RecordData[2]
                        {
                            new RecordData()
                            {
                                Measurement_ID = ViewModel.CurrentMeasurement?.ID.Value,
                                Eye_Position = CameraPosition.Left,
                                index = ViewModel.MeasuringProgress,
                                image = frames[0],
                            },
                            new RecordData()
                            {
                                Measurement_ID = ViewModel.CurrentMeasurement?.ID.Value,
                                Eye_Position = CameraPosition.Right,
                                index = ViewModel.MeasuringProgress,
                                image = frames[1],
                            },
                        };

                        for (int i = 0; i < predicts.Length; i++)
                        {
                            var Pupils = Algorithm.getPupil(predicts[i]);

                            if (Pupils == null)
                            {
                                records[i] = null;
                                break;
                            }

                            var center = Pupils?.Item1.ToPoint();

                            records[i].pupil_radius = (int)Pupils?.Item2;
                            records[i].pupil_center = center.Value;

                            int? mrd1 = Algorithm.getMRD1(predicts[i], records[i].pupil_center);

                            if (mrd1 == null)
                            {
                                records[i] = null;
                                break;
                            }

                            records[i].mrd1 = mrd1.Value;
                        }

                        
                        if (ViewModel.MeasureStatus == MeasureStatus.Start)
                        {
                            if (records[0] != null && records[1] != null)
                            {
                                ViewModel.MeasuringProgress++;

                                records[0].InsertDB(MainWindow.Connection);
                                records[1].InsertDB(MainWindow.Connection);

                                if (ViewModel.MeasuringProgress == 50)
                                {
                                    ViewModel.MeasuringProgress = 0;
                                    ViewModel.MeasureStatus = MeasureStatus.None;
                                }
                            }
                        }

                        try
                        {
                            for (int i = 0; i < frames.Length; i++)
                            {
                                Mat result = records[i]?.drawResult() ?? frames[i];
                                Dispatcher.Invoke(() =>
                                {
                                    ShowMRD1ViewModels[i].Image = WriteableBitmapConverter.ToWriteableBitmap(result);
                                });
                            }
                        }
                        catch (TaskCanceledException) { }
                    }, cancelToken.Token);

                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private void StartMeasuringButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            MainWindow.MainSnackbar.MessageQueue.Enqueue($"now Position : {ViewModel.LedPosition.ToString()}");

            switch (ViewModel.MeasureStatus)
            {
                case MeasureStatus.None:
                    /*
                     * 아두이노 해당 LED 반짝반짝
                     */
                    ViewModel.MeasureStatus = MeasureStatus.Ready;
                    break;
                case MeasureStatus.Ready:
                    /*
                     * 아두이노 해당 LED 점등
                     */

                    ViewModel.CurrentMeasurement = new Measurement
                    {
                        Led_Position = ViewModel.LedPosition,
                        Patient_ID = MainWindow.selectPatient.ID,
                        date = DateTime.Now,
                    };

                    ViewModel.CurrentMeasurement.InsertDB(MainWindow.Connection);

                    ViewModel.MeasuringProgress = 0;

                    ViewModel.MeasureStatus = MeasureStatus.Start;
                    break;
                case MeasureStatus.Start:

                    ViewModel.MeasureStatus = MeasureStatus.None;
                    break;
            }
        }
    }
}
