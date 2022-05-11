using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
    }
}
