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

        private MainWindow MainWindow = Application.Current.MainWindow as MainWindow;
        public SelectPatient()
        {
            InitializeComponent();

            MainWindow mainwindow = Application.Current.MainWindow as MainWindow;

            ViewModel = new SelectPatientViewModel(mainwindow.Connection);
            DataContext = ViewModel;

            ViewModel.updatePatient();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void SelectPatientListViewItem(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.DataContext is Patient patient)
            {
                MainWindow.selectPatient = patient;
                MainWindow.MeasureMRD1ListViewItem.IsSelected = true;
            }
        }

        private void RemovePatientListViewItem(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.DataContext is Patient patient)
            {
                ViewModel.removePatient(patient);
            }
        }

        private void AddPatientButton_Clicked(object sender, RoutedEventArgs e)
        {
            if(ViewModel.AddName?.Length <= 0)
            {
                MainWindow.MainSnackbar.MessageQueue.Enqueue("이름을 입력하세요.");
                return;
            }

            if(ViewModel.AddCallnumber?.Length <= 5)
            {
                MainWindow.MainSnackbar.MessageQueue.Enqueue("전화번호를 입력하세요.");
                return;
            }

            if(ViewModel.AddBirth.HasValue == false)
            {
                MainWindow.MainSnackbar.MessageQueue.Enqueue("생일을 선택하세요.");
                return;
            }

            ViewModel.InsertPatient(new Patient
            {
                Name = ViewModel.AddName,
                Birthday = ViewModel.AddBirth.Value,
                Gender = ViewModel.AddGender.ToString().ElementAt(0),
                Callnumber = ViewModel.AddCallnumber,
            });
        }
    }
}
