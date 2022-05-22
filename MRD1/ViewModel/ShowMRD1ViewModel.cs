using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using System.Collections.ObjectModel;

using LiveCharts;
using LiveCharts.Wpf;

namespace MRD1.ViewModel
{
    public class ShowMRD1ViewModel : BaseViewModel
    {
        ImageSource __Image = null;
        public ImageSource Image
        {
            get => __Image;
            set
            {
                SetProperty(ref __Image, value);
            }
        }

        private ObservableCollection<RecordData> __data;

        public ObservableCollection<RecordData> RecordData
        {
            get => __data;
            set => SetProperty(ref __data, value);
        }

        private int? __mrd1 = null;
        public int? MRD1
        {
            get => __mrd1;
            set => SetProperty(ref __mrd1, value);
        }
        public ChartValues<int> MRD1_chartData
        {
            get
            {
                if (RecordData == null)
                    return null;

                return new ChartValues<int>(from data in __data select data.mrd1);
            }
        }


        public void addRecordData(RecordData data)
        {
            __data.Add(data);

            NotifyPropertyChanged("MRD1_chartData");
        }
    }
}
