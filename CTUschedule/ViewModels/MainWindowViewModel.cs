using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace CTUschedule.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        public List<ViewModelBase> _pageViewModels = new List<ViewModelBase>();
        [ObservableProperty]
        public ViewModelBase _currentViewModel;
       


        public MainWindowViewModel()
        {
            PageViewModels.Add(new SignInViewModel());

            CurrentViewModel = PageViewModels.First();
        }
    }
}
