using CTUschedule.Models;
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
        private CourseListViewModel _courseListViewModel { get; }
        public ObservableCollection<CourseNode> CourseNodes { get; set; }

      

        public ScheduleViewModel() 
        {
            Instance = this;
            _courseListViewModel = CourseListViewModel.Instance;
            _courseListViewModel.CourseListUpdate += _courseListViewModel_CourseListUpdate;

            //CourseNodes = new ObservableCollection<CourseNode>
            //{
            //    new CourseNode("CT172","asd", new ObservableCollection<CourseNode>()
            //    {
            //        new CourseNode(new CourseInformation()
            //        {
            //            dkmh_nhom_hoc_phan_ma = "asdasgsdsfgfdh",
            //            si_so_con_lai = 10,
            //        }),
            //         new CourseNode(new CourseInformation()
            //        {
            //            dkmh_nhom_hoc_phan_ma = "heelooo",
            //            si_so_con_lai = 100,
            //        }),
            //    }),
            //};
        }

        private void _courseListViewModel_CourseListUpdate(object? sender, EventArgs e)
        {
            CourseNodes = _courseListViewModel.SelectedCourseList;
        }

        public void Init()
        {

        }


    }
}
