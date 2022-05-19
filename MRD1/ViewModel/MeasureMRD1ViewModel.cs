using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;

namespace MRD1.ViewModel
{
    public class MeasureMRD1ViewModel : BaseViewModel
    {
        MySqlConnection connection;
        public MeasureMRD1ViewModel(MySqlConnection Connection)
        {
            connection = Connection;

            updateMeasurement(LedPosition.Top);
            updateMeasurement(LedPosition.Middle);
            updateMeasurement(LedPosition.Bottom);
        }

        public void updateMeasurement(LedPosition position)
        {
            MainWindow mainWindow = Application.Current.MainWindow as MainWindow;

            string sql = $"SELECT * FROM measurement WHERE Patient_ID={mainWindow?.selectPatient.ID} AND LED_Position=\"{position.ToString()}\"";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            using MySqlDataReader table = command.ExecuteReader();

            ObservableCollection<Measurement> measurement = new ObservableCollection<Measurement>();

            while (table.Read())
            {
                measurement.Add(new Measurement()
                {
                    ID = table.GetInt32("ID"),
                    Patient_ID = table.GetInt32("Patient_ID"),
                    Led_Position = (LedPosition)Enum.Parse(typeof(LedPosition),table.GetString("LED_Position")),
                    date = table.GetDateTime("date"),
                }) ;
            }

            switch (position)
            {
                case LedPosition.Top:
                    TopMeasurement = measurement;
                    break;
                case LedPosition.Middle:
                    MiddleMeasurement = measurement;
                    break;
                case LedPosition.Bottom:
                    BottomMeasurement = measurement;
                    break;
            }
        }

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

        private ObservableCollection<Measurement> __TopMeasurement;
        public ObservableCollection<Measurement> TopMeasurement
        {
            get => __TopMeasurement;
            set => SetProperty(ref __TopMeasurement, value);
        }

        private ObservableCollection<Measurement> __MiddleMeasurement;

        public ObservableCollection<Measurement> MiddleMeasurement
        {
            get=> __MiddleMeasurement;
            set => SetProperty(ref __MiddleMeasurement, value);
        }

        private ObservableCollection<Measurement> __BottomMeasurement;

        public ObservableCollection <Measurement> BottomMeasurement
        {
            get => __BottomMeasurement;
            set => SetProperty(ref __BottomMeasurement, value);
        }

        public ObservableCollection<Measurement> getListMeasurement(LedPosition position)
        {
            switch (position)
            {
                case LedPosition.Top:
                    return TopMeasurement;
                case LedPosition.Middle:
                    return MiddleMeasurement;
                case LedPosition.Bottom:
                    return BottomMeasurement;
            }

            return null;
        }

    }
}