using System;
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

using System.Threading;
using System.Collections.ObjectModel;

namespace MRD1
{
    /// <summary>
    /// SelectPatient.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelectPatient : UserControl
    {
        public SelectPatientViewModel ViewModel;
        public SelectPatient()
        {
            InitializeComponent();

            MainWindow mainwindow = Application.Current.MainWindow as MainWindow;

            ViewModel = new SelectPatientViewModel(mainwindow.Connection);
            DataContext = ViewModel;

            ViewModel.updatePatient();
        }

        private void SelectPatientListViewItem(object sender, RoutedEventArgs e)
        {

        }

        private void RemovePatientListViewItem(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.DataContext is Patient patient)
            {
                ViewModel.removePatient(patient);
            }
        }
    }
}
