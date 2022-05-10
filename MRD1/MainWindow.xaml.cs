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

namespace MRD1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            changeContent(new SelectPatient());
        }

        public void changeContent(UserControl page)
            => ViewContentControl.Content = page;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SelectPaitentListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            changeContent(new SelectPatient());
        }

        private void MeasureMRD1ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            changeContent(new MeasureMRD1());
        }

        private void ReplayDataListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            changeContent(new ReplayData());
        }

        private void SettingListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            changeContent(new Setting());
        }
    }
}
