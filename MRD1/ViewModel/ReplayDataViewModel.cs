using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

using MySql.Data.MySqlClient;

using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media;

using LiveCharts;
using LiveCharts.Wpf;

namespace MRD1.ViewModel
{
    public class ReplayDataViewModel : BaseViewModel
    {
        private MySqlConnection connection;
        private int ID = 0;

        public ReplayDataViewModel(MySqlConnection Connection,int id)
        {
            this.connection = Connection;

            ID = id;
            updateData(ID);
        }

        public void updateData(int Measurement_id)
        {
            string qurey = $"SELECT * from record_data WHERE Measurement_ID = {Measurement_id}";

            using MySqlCommand command = new MySqlCommand(qurey, connection);
            using MySqlDataReader table = command.ExecuteReader();

            __RecordData[0].Clear();
            __RecordData[1].Clear();

            while (table.Read())
            {
                var geometry = table.GetMySqlGeometry("pupil_center");
                int imageSize = table.GetInt32("image_size");
                var pos = (CameraPosition)Enum.Parse(typeof(CameraPosition), table.GetString("Eye_Position"));


                __RecordData[(int)pos].Add(new RecordData()
                {
                    ID = table.GetInt32("ID"),
                    Measurement_ID = table.GetInt32("Measurement_ID"),
                    Eye_Position = pos,
                    index = table.GetInt32("record_index"),
                    pupil_center = new Point((int)geometry.XCoordinate, (int)geometry.YCoordinate),
                    pupil_radius = table.GetInt32("pupil_radius"),
                    image = table.GetMat("image", imageSize),
                    mrd1 = table.GetInt32("MRD1")
                });
            }
        }

        private ObservableCollection<RecordData>[] __RecordData = new ObservableCollection<RecordData>[2]
        {
            new ObservableCollection<RecordData>(),
            new ObservableCollection<RecordData>(),
        };

        public int DataCount
        {
            get => __RecordData[0].Count - 1;
        }

        int __index = 0;
        public int index
        {
            get => __index;
            set {
                SetProperty(ref __index, value);

                //Left Eye value update
                NotifyPropertyChanged("LeftEyeMRD1");
                NotifyPropertyChanged("LeftEyePupilCenterX");
                NotifyPropertyChanged("LeftEyePupilCenterY");
                NotifyPropertyChanged("LeftEyePupilRadius");

                //Right Eye value update
                NotifyPropertyChanged("RightEyeMRD1");
                NotifyPropertyChanged("RightEyePupilCenterX");
                NotifyPropertyChanged("RightEyePupilCenterY");
                NotifyPropertyChanged("RightEyePupilRadius");

                //Image update
                NotifyPropertyChanged("LeftEyeImage");
                NotifyPropertyChanged("RightEyeImage");
            }
        }

        public ObservableCollection<RecordData>[] RecordData
        {
            get => __RecordData;
            set => SetProperty(ref __RecordData, value);
            
        }

        public ImageSource LeftEyeImage
        {
            get
            {
                if(isLeftEyeOverlay == true)
                    return WriteableBitmapConverter.ToWriteableBitmap(RecordData[0][index].drawResult());
                else
                    return WriteableBitmapConverter.ToWriteableBitmap(RecordData[0][index].image);
            }
        }

        bool __isLeftEyeOverlay = true;

        public bool isLeftEyeOverlay
        {
            get => __isLeftEyeOverlay;
            set
            {
                SetProperty(ref __isLeftEyeOverlay, value);
                NotifyPropertyChanged("LeftEyeImage");
            }
        }

        public ImageSource RightEyeImage
        {
            get
            {
                if (isRightEyeOverlay == true)
                    return WriteableBitmapConverter.ToWriteableBitmap(RecordData[1][index].drawResult());
                else
                    return WriteableBitmapConverter.ToWriteableBitmap(RecordData[1][index].image);
            }
        }

        bool __isRightEyeOverlay = true;

        public bool isRightEyeOverlay
        {
            get => __isRightEyeOverlay;
            set
            {
                SetProperty(ref __isRightEyeOverlay, value);
                NotifyPropertyChanged("RightEyeImage");
            }
        }

        public ChartValues<int> LeftEyeMRD1ChartValue
        {
            get
            {
                if (RecordData == null)
                    return null;

                if (RecordData[(int)CameraPosition.Left] == null)
                    return null;

                return new ChartValues<int>(from data in RecordData[(int)CameraPosition.Left] select data.mrd1);
            }
        }

        public ChartValues<int> RightEyeMRD1ChartValue
        {
            get
            {
                if (RecordData == null)
                    return null;

                if (RecordData[(int)CameraPosition.Right] == null)
                    return null;

                return new ChartValues<int>(from data in RecordData[(int)CameraPosition.Right] select data.mrd1);
            }
        }

        #region LeftEyeValue binding

        public int LeftEyeMRD1
        {
            get => RecordData[0][index].mrd1;

            set
            {
                RecordData[0][index].mrd1 = value;
                NotifyPropertyChanged("LeftEyeMRD1");
                NotifyPropertyChanged("LeftEyeImage");
            }
        }

        public int LeftEyePupilCenterX
        {
            get => RecordData[0][index].pupil_center.X;
            set
            {
                var ptCenter = RecordData[0][index].pupil_center;
                ptCenter.X = value;
                RecordData[0][index].pupil_center = ptCenter;
                NotifyPropertyChanged("LeftEyeImage");
                NotifyPropertyChanged("LeftEyePupilCenterX");
            }
        }

        public int LeftEyePupilCenterY
        {
            get => RecordData[0][index].pupil_center.Y;
            set
            {
                var ptCenter = RecordData[0][index].pupil_center;
                ptCenter.Y = value;
                RecordData[0][index].pupil_center = ptCenter;
                NotifyPropertyChanged("LeftEyeImage");
                NotifyPropertyChanged("LeftEyePupilCenterY");
            }
        }

        public int LeftEyePupilRadius
        {
            get => RecordData[0][index].pupil_radius;
            set
            {
                var ptCenter = RecordData[0][index].pupil_radius = value;
                NotifyPropertyChanged("LeftEyeImage");
                NotifyPropertyChanged("LeftEyePupilRadius");
            }
        }

        #endregion

        #region RightEyeValue binding

        public int RightEyeMRD1
        {
            get => RecordData[1][index].mrd1;

            set
            {
                RecordData[1][index].mrd1 = value;
                NotifyPropertyChanged("RightEyeMRD1");
                NotifyPropertyChanged("RightEyeImage");
            }
        }

        public int RightEyePupilCenterX
        {
            get => RecordData[1][index].pupil_center.X;
            set
            {
                var ptCenter = RecordData[1][index].pupil_center;
                ptCenter.X = value;
                RecordData[1][index].pupil_center = ptCenter;
                NotifyPropertyChanged("RightEyeImage");
                NotifyPropertyChanged("RightEyePupilCenterX");
            }
        }

        public int RightEyePupilCenterY
        {
            get => RecordData[1][index].pupil_center.Y;
            set
            {
                var ptCenter = RecordData[1][index].pupil_center;
                ptCenter.Y = value;
                RecordData[1][index].pupil_center = ptCenter;
                NotifyPropertyChanged("RightEyeImage");
                NotifyPropertyChanged("RightEyePupilCenterY");
            }
        }

        public int RightEyePupilRadius
        {
            get => RecordData[1][index].pupil_radius;
            set
            {
                var ptCenter = RecordData[1][index].pupil_radius = value;
                NotifyPropertyChanged("RightEyeImage");
                NotifyPropertyChanged("RightEyePupilRadius");
            }
        }

        #endregion
    }

    public static class MysqlTableExpander
    {
        public static Mat GetMat(this MySqlDataReader reader, string column_name,int size)
        {
            byte[] image_data = new byte[size];
            reader.GetBytes(reader.GetOrdinal(column_name), 0, image_data, 0, size);

            return Cv2.ImDecode(image_data,ImreadModes.Color);
        }
    }
}
