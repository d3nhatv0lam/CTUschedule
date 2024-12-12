using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.ViewModels
{
    public partial class SignInViewModel : ViewModelBase
    {
        
        private string _mSSV = "";
        private string _password = "";

        public string MSSV
        {
            get => _mSSV;
            set
            {
                if (value == _mSSV) return;
                _mSSV = value;
                OnPropertyChanged(nameof(MSSV));
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                if (value == _password) return;
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }


        public SignInViewModel()
        {

        }

        [RelayCommand]
        public void Login()
        {

        }
    }
}
