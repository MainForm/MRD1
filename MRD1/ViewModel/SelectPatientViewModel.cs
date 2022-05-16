using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

using MySql.Data.MySqlClient;
using System.Windows.Controls;
using System.Globalization;

namespace MRD1.ViewModel
{
    public class SelectPatientViewModel : BaseViewModel
    {
        private MySqlConnection connection;

        #region Data Binding 
        private ObservableCollection<Patient> _patients;
        public ObservableCollection<Patient> Patients
        {
            get => _patients;
            set => SetProperty(ref _patients, value);
        }

        private string __AddName;
        public string AddName
        {
            get => __AddName;
            set => SetProperty(ref __AddName, value);
        }

        private string __AddCallnumber;

        public string AddCallnumber
        {
            get => __AddCallnumber;
            set => SetProperty(ref __AddCallnumber, value);
        }

        private Gender _gender = 0;

        public Gender AddGender
        {
            get => _gender;
            set => SetProperty(ref _gender, value);
        }

        private DateTime? __AddBirth;
        public DateTime? AddBirth
        {
            get => __AddBirth;
            set => SetProperty(ref __AddBirth, value);
        }

        #endregion

        public SelectPatientViewModel(MySqlConnection connection)
        {
            this.connection = connection;
        }

        #region Database Function

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

            string sql = $"DELETE FROM patient_tb WHERE ID={patient.ID}";
            using MySqlCommand command = new MySqlCommand(sql, connection);

            try
            {
                using MySqlDataReader myReader = command.ExecuteReader();
            }
            catch (Exception ex)
            {

            }
        }

        public void InsertPatient(Patient patient)
        {
            string sql = $"INSERT INTO patient_tb(Name,Birthday,Gender,Callnumber) " +
                            $"Value(\"{patient.Name}\",\"{patient.Birthday.ToString("yyyy-MM-dd")}\"" +
                            $",'{patient.Gender}',\"{patient.Callnumber}\");";
            using MySqlCommand command = new MySqlCommand(sql, connection);

            try
            {
                using MySqlDataReader myReader = command.ExecuteReader();
            }
            catch (Exception ex)
            {

            }

            updatePatient();
        }

        #endregion
    }

    public enum Gender
    {
        Male,Female
    }

    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrEmpty((value ?? "").ToString()) ?
                            new ValidationResult(false, "반드시 채워 주세요") :
                            ValidationResult.ValidResult;
        }
    }
}
