using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace CTUschedule.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel Instance;

        [ObservableProperty]
        private string _title = "CTUschedule";
        [ObservableProperty]
        private List<ViewModelBase> _pageViewModels = new List<ViewModelBase>();
        [ObservableProperty]
        private ViewModelBase _currentViewModel;
       


        public MainWindowViewModel()
        {
            Instance = this;
            PageViewModels.Add(new SignInViewModel());

            CurrentViewModel = PageViewModels.First();
        }

    }
}
