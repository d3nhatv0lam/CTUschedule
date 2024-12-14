using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CTUschedule.Resources.Dialogs;
using Avalonia.Media.TextFormatting;
using CTUschedule.Utilities;

namespace CTUschedule.ViewModels
{
    public partial class SignInViewModel : ViewModelBase
    {
        private HTQL_Signin _signin;
        private string _mSSV = "";
        private string _password = "";
        private string _capcha = "";
        [ObservableProperty]
        private Bitmap _capchaImage;
        [ObservableProperty]
        private bool _isLogining = false;

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
        
        public string Capcha
        {
            get => _capcha;
            set
            {
                if (value == _capcha) return;
                _capcha = value;
                OnPropertyChanged(nameof(Capcha));
            }
        }


        public SignInViewModel()
        {
            _signin = new HTQL_Signin();
            NavigateToSignin();
        }

        private void NavigateToSignin()
        {
            _signin.NavigateToSignin();
            LoadCapcha();
        }

        private void LoadCapcha()
        {
            try
            {
                CapchaImage = new Bitmap(_signin.CapchaPath);
            }
            catch (Exception ex)
            {

            }
            
        }

        [RelayCommand]
        public async void Login()
        {
            if (CheckerInternetHelper._isHasInternet == false) return;
            IsLogining = true;
            bool Islogin = await Task.Run(() =>  _signin.SignIn(MSSV, Password, Capcha));
 
            if (!Islogin)
            {
                INotificationPopup noti = new NotificationPopupController(NotificationPopupController.Type.Error, "Đăng nhập thất bại", "Kiểm tra lại thông tin đăng nhập");
                noti.ShowNotification();
                Task.Run(() => NavigateToSignin());
            }
            else
            {

            }
            IsLogining = false;
        }
    }
}
