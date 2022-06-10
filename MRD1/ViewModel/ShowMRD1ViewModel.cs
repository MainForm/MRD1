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

        private ObservableCollection<double> __data;

        public ObservableCollection<double> RecordData
        {
            get => __data;
            set => SetProperty(ref __data, value);
        }

        private double? __mrd1 = null;
        public double? MRD1
        {
            get => __mrd1;
            set => SetProperty(ref __mrd1, value);
        }
        public ChartValues<double> MRD1_chartData
        {
            get
            {
                if (RecordData == null)
                    return null;

                return new ChartValues<double>(__data);
            }
        }


        public void addRecordData(double data)
        {
            __data.Add(data);

            NotifyPropertyChanged("MRD1_chartData");
        }
    }
}
