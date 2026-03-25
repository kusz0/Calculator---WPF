using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Calculator_Wpf.ViewModels
{
    class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _result = "0";
        public string Result 
        { 
            get => _result;
            set 
            {
                _result = value;

                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
