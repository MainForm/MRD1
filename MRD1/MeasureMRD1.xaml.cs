using MRD1.ViewModel;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Collections.ObjectModel;

namespace MRD1
{
    /// <summary>
    /// MeasureMRD1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MeasureMRD1 : UserControl
    {
        object mysql_Lock = new object();

        public MeasureMRD1ViewModel ViewModel;
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

            ViewModel = new MeasureMRD1ViewModel(MainWindow.Connection);
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
                                for(int i = 0; i < records.Length; i++)
                                {
                                    ShowMRD1ViewModels[i].addRecordData(records[i]);
                                }

                                ViewModel.MeasuringProgress++;

                                lock (mysql_Lock)
                                {
                                    records[0].InsertDB(MainWindow.Connection);
                                    records[1].InsertDB(MainWindow.Connection);
                                }

                                if (ViewModel.MeasuringProgress == 50)
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        MainWindow.MainSnackbar.MessageQueue.Enqueue($"검사 완료");
                                    });
                                    ViewModel.MeasuringProgress = 0;
                                    ViewModel.MeasureStatus = MeasureStatus.None;
                                }
                            }
                        }

                        try
                        {
                            for (int i = 0; i < frames.Length; i++)
                            {
                                ShowMRD1ViewModels[i].MRD1 = records[i]?.mrd1;
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

                    for(int i = 0; i < ShowMRD1ViewModels.Length; i++)
                    {
                        ShowMRD1ViewModels[i].RecordData = new ObservableCollection<RecordData>();
                    }

                    ViewModel.CurrentMeasurement.InsertDB(MainWindow.Connection);
                    ViewModel.getListMeasurement(ViewModel.LedPosition).Add(ViewModel.CurrentMeasurement);

                    ViewModel.MeasuringProgress = 0;

                    ViewModel.MeasureStatus = MeasureStatus.Start;
                    break;
                case MeasureStatus.Start:

                    break;
            }
        }
    }
}
