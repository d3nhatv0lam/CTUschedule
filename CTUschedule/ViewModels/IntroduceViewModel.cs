using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.ViewModels
{
    public partial class IntroduceViewModel : ViewModelBase
    {

        private Dictionary<string, string> ContactLink = new Dictionary<string, string>()
        {
            { "Github", "https://github.com/d3nhatv0lam" },
            { "Facebook", "https://www.facebook.com/profile.php?id=100088452777261"},
            { "Youtube", "https://www.youtube.com/@ucduong9984" },
        };

        [ObservableProperty]
        private bool _isOpenQRCode = false;

        public IntroduceViewModel()
        {
           
        }

        [RelayCommand]
        public void OpenGithubPage() => System.Diagnostics.Process.Start( new System.Diagnostics.ProcessStartInfo { 
            FileName = ContactLink["Github"],
            UseShellExecute = true,
        });
        [RelayCommand]
        public void OpenFacebookPage() => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = ContactLink["Facebook"],
            UseShellExecute = true,
        });
        [RelayCommand]
        public void OpenYoutubePage() => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = ContactLink["Youtube"],
            UseShellExecute = true,
        });

        [RelayCommand]
        public void OpenQRCode() => IsOpenQRCode = true;


    }
}
