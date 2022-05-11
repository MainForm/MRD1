using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

namespace MRD1.ViewModel
{
    public class SelectPatientViewModel : BaseViewModel
    {
        private ObservableCollection<Patient> _patients;
        public ObservableCollection<Patient> Patients 
        {
            get => _patients;
            set => SetProperty(ref _patients, value);
        }

        public SelectPatientViewModel()
        {
            Patients = new ObservableCollection<Patient>()
            {
                new Patient()
                {
                    Name = "Test1",
                    Birthday = DateTime.Now,
                    Callnumber = "000-1111-2222",
                    Gender = 'M',
                }
            };

        }
    }
}
