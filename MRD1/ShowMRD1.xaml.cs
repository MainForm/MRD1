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

namespace MRD1
{
    /// <summary>
    /// ShowMRD1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShowMRD1 : UserControl
    {
        public ShowMRD1ViewModel ViewModel;
        public ShowMRD1(ShowMRD1ViewModel ViewModel = null)
        {
            InitializeComponent();

            if (ViewModel == null)
                DataContext = new ShowMRD1ViewModel();
            else
                DataContext = ViewModel;
        }
    }
}
