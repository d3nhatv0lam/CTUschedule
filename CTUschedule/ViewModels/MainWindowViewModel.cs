using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CTUschedule.Resources.Dialogs;
using CTUschedule.Utilities;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace CTUschedule.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel Instance;
        public CheckerInternetHelper internetHelper;

        [ObservableProperty]
        private string _title = "CTUschedule";
        [ObservableProperty]
        private List<ViewModelBase> _pageViewModels = new List<ViewModelBase>();
        [ObservableProperty]
        private ViewModelBase _currentViewModel;

        

        public MainWindowViewModel()
        {
            Instance = this;
            CheckerInternetAssign();

            //PageViewModels.Add(new SignInViewModel());
            PageViewModels.Add(new MainHomeViewModel());

            CurrentViewModel = PageViewModels.First();

            // xảy ra khi check ra web đã bị logout
            CTU_HTQLWebDriver.Instance.WebLoggedOut += Instance_WebLoggedOut;
        }

        private void Instance_WebLoggedOut(object? sender, System.EventArgs e)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                INotificationPopup lostDataWaring = new NotificationPopupController(NotificationPopupController.Type.Warning, "Hết phiên đăng nhập!", "Login và lưu TKB còn dỡ của bạn.");
                lostDataWaring.ShowNotification();
                GoToSignInView();
            });
        }


        public void GoToSignInView()
        {
            CurrentViewModel = PageViewModels[0];
            (PageViewModels[0] as SignInViewModel).Init();
        }

        public void GoToMainHomeView()
        {
            CurrentViewModel = PageViewModels[1];
            (PageViewModels[1] as MainHomeViewModel).Init();
        }

        // init and UpdateUI for first check
        private void CheckerInternetAssign()
        {
            internetHelper = new CheckerInternetHelper();
            internetHelper.InternetChanged += InternetHelper_InternetChanged;
            internetHelper.OnInternetChanged();
        }

        private void InternetHelper_InternetChanged(object? sender, System.EventArgs e)
        {
            // no internet
            if (internetHelper.IsHasInternet == false)
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    INotificationPopup NoInternetpopup = new NotificationPopupController(NotificationPopupController.Type.Warning, "Không có internet", "Một vài tính năng sẽ không hoạt động!");
                    NoInternetpopup.ShowNotification();
                });
                Title = "CTUschedule - NoNetwork";

            }
            // has internet
            else
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    INotificationPopup HasInternetpopup = new NotificationPopupController(NotificationPopupController.Type.Succes, "Internet có sẵn", "Trãi nghiệm thui~");
                    HasInternetpopup.ShowNotification();
                });
                Title = "CTUschedule";
            }
        }
    }
}
