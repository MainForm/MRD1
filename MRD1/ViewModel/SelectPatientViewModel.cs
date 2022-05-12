using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

using MySql.Data.MySqlClient;

namespace MRD1.ViewModel
{
    public class SelectPatientViewModel : BaseViewModel
    {
        private MySqlConnection connection;

        private ObservableCollection<Patient> _patients;
        public ObservableCollection<Patient> Patients 
        {
            get => _patients;
            set => SetProperty(ref _patients, value);
        }

        public SelectPatientViewModel(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public void updatePatient()
        {
            string sql = "SELECT * FROM patient_tb";

            using MySqlCommand command = new MySqlCommand(sql, connection);
            using MySqlDataReader table = command.ExecuteReader();

            ObservableCollection<Patient> patients = new ObservableCollection<Patient>();

            while (table.Read())
            {
                patients.Add(new Patient()
                {
                    ID = table.GetInt32("ID"),
                    Name = table.GetString("Name"),
                    Birthday = table.GetDateTime("Birthday"),
                    Gender = table.GetChar("Gender"),
                    Callnumber = table.GetString("Callnumber"),
                });
            }

            Patients = patients;
        }

        public void removePatient(Patient patient)
        {
            Patients.Remove(patient);

            string sql = $"DELETE FROM patient_tb WHERE Patient_id={patient.ID}";
            using MySqlCommand command = new MySqlCommand(sql, connection);

            try
            {
                using MySqlDataReader myReader = command.ExecuteReader();
            }
            catch(Exception ex)
            {
                
            }
        }
    }
}
