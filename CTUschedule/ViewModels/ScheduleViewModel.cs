using CommunityToolkit.Mvvm.ComponentModel;
using CTUschedule.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.ViewModels
{
    public partial class ScheduleViewModel : ViewModelBase
    {
        public static ScheduleViewModel Instance { get; set; }
        private CourseListEditViewModel _courseListEditViewModel { get; }
        public ObservableCollection<CourseNode> CourseNodes { get; set;}

        [ObservableProperty]
        // 9row, 6colum
        private List<ObservableCollection<int>> _schedule = new List<ObservableCollection<int>>();



        public ScheduleViewModel() 
        {
            Instance = this;
            _courseListEditViewModel = CourseListEditViewModel.Instance;
            _courseListEditViewModel.ScheduleChanged += _courseListEditViewModel_ScheduleChanged; ; 
        }

        private void _courseListEditViewModel_ScheduleChanged(object? sender, EventArgs e)
        {
            CourseNodes = CourseNode.Uncheck_UnExpandCourseNode(_courseListEditViewModel.CourseNodes);
        }


        public void Init()
        {

        }


    }
}
