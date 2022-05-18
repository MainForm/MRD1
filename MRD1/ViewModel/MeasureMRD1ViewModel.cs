using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Collections.ObjectModel;

namespace MRD1.ViewModel
{
    public class MeasureMRD1ViewModel : BaseViewModel
    {
        private int __MeasuringProgress = 0;
        public int MeasuringProgress
        {
            get => __MeasuringProgress;
            set => SetProperty(ref __MeasuringProgress, value);
        }

        private MeasureStatus __MeasuringStatus = MeasureStatus.None;
        public MeasureStatus MeasureStatus
        {
            get => __MeasuringStatus;
            set => SetProperty(ref __MeasuringStatus, value);
        }

        private LedPosition __LedPosition = LedPosition.Top;
        public LedPosition LedPosition
        {
            get => __LedPosition;
            set => SetProperty(ref __LedPosition, value);
        }

        public string Patient_name
        {
            get
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

                return mainWindow?.selectPatient.Name;
            }
        }

        private Measurement __currentMeasurement;

        public Measurement CurrentMeasurement
        {
            get => __currentMeasurement;
            set => SetProperty(ref __currentMeasurement, value);
        }

        private ObservableCollection<RecordData[]> __current_data;

        public ObservableCollection<RecordData[]> CurrentData
        {
            get => __current_data;
            set => SetProperty(ref __current_data, value);
        }
    }
}