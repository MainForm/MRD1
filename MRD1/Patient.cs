using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MRD1
{
    public class Patient : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T member, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(member, value))
            {
                return false;
            }

            member = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        string __name;
        DateTime __birthday;
        string __callnumber;
        char __gender;

        public int ID { get; init; }
        public string Name
        {
            get => __name;
            set => SetProperty(ref __name, value);
        }

        public DateTime Birthday
        {
            get => __birthday;
            set => SetProperty(ref __birthday, value);
        }

        public decimal Age
        {
            get
            {
                return DateTime.Now.Year - Birthday.Year;
            }
        }

        public string Callnumber
        {
            get => __callnumber;
            set => SetProperty(ref __callnumber, value);
        }

        public char Gender
        {
            get => __gender;
            set => SetProperty(ref __gender, value);
        }
    }
}
