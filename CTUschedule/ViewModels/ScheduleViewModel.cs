using Avalonia.Controls;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CTUschedule.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.ViewModels
{
    public partial class ScheduleViewModel : ViewModelBase
    {
        public static ScheduleViewModel Instance { get; set; }
        private CourseListEditViewModel _courseListEditViewModel { get; }
        public ObservableCollection<CourseNode> CourseNodes { get; set;} = new ObservableCollection<CourseNode>();

        [ObservableProperty]
        // 9row, 6colum
        private List<ObservableCollection<ScheduleCell>> _schedule = new List<ObservableCollection<ScheduleCell>>();

        private List<ScheduleCell> ScheduleItemPool = new List<ScheduleCell>();
        private List<ScheduleCell> SelectedScheduleItem;

        public ScheduleViewModel() 
        {
            Instance = this;
            _courseListEditViewModel = CourseListEditViewModel.Instance;
            Init();

            _courseListEditViewModel.ScheduleChanged += _courseListEditViewModel_ScheduleChanged;
 
        }

        public void Init()
        {

            CourseNodes = CourseNode.UnExpandAllCourseNode(CourseNodes);
            Schedule = EmptyTableSchedule();
        }

        // get empty table
        // thời khóa biểu mặc dịnh có 9 row 6 col
        private List<ObservableCollection<ScheduleCell>> EmptyTableSchedule(int rows = 9, int columns = 6)
        {
            List<ObservableCollection<ScheduleCell>> newTable = new List<ObservableCollection<ScheduleCell>>();
            for (int i = 0; i <= rows; i++)
            {
                ObservableCollection<ScheduleCell> row = new ObservableCollection<ScheduleCell>();
                for (int j = 0; j <= columns; j++)
                {
                    row.Add(new ScheduleCell());
                }
                newTable.Add(row);
            }
            return newTable;
        }

        private void _courseListEditViewModel_ScheduleChanged(object? sender, EventArgs e)
        {
            CourseNodes = _courseListEditViewModel.CourseNodes;
            ScheduleItemPool = CreateScheduleItemPool();
            RenderSchedule();
        }

        private List<ScheduleCell> CreateScheduleItemPool()
        {
            List<ScheduleCell> newPool = new List<ScheduleCell>();
            foreach (var node in CourseNodes)
            {
                foreach (var child in node.SubNodes)
                {
                    foreach (var course in child.CourseGroup)
                    {
                        ScheduleCell newScheduleItem = new ScheduleCell(course);
                        newPool.Add(newScheduleItem);
                    }
                }
            }
            return newPool;
        }

        private void RenderSchedule()
        {
            foreach (var node in CourseNodes)
            {
                //if (node.MaHocPhan == maHocPhan)
                {
                    foreach (var child in node.SubNodes)
                    {
                        if (child.IsScheduleSelected)
                        {
                            AddCourseToSchedule(child.CourseGroup);
                        }
                    }
                }
            }
        }

        private void AddCourseToSchedule(ObservableCollection<CourseInformation> courseGroup)
        {
            foreach (var course in courseGroup) 
            { 
                foreach(var itempool in ScheduleItemPool)
                {
                    // check luôn mã học phần vì trong pool có nhiều nhóm mà khác học phần
                    if (course.dkmh_tu_dien_hoc_phan_ma == itempool.MaHocPhan && course.dkmh_nhom_hoc_phan_ma == itempool.NhomHocPhan)
                    {
                        itempool.IsShowCell = true;
                        Schedule[itempool.RowIndex][itempool.ColumnIndex] = itempool;
                    }
                }
            }
            Schedule = Schedule;
        }

        [RelayCommand]
        public void CheckBoxTask(TreeViewItem treeViewItem)
        {
            // Check u check in Child
            var parent = treeViewItem.Parent as TreeViewItem;
            if (parent == null) return;

            // get MaHocPhan of Child
            var grid = parent.GetVisualDescendants().OfType<Grid>().FirstOrDefault();
            if (grid == null) return;
            var maHocPhan = grid.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault().Text;

            Schedule = EmptyTableSchedule();
            RenderSchedule();

        }

        //[RelayCommand]
        //public void CheckBoxTask(ObservableCollection<CourseNode> scheduleList)
        //{
        //    if (scheduleList.Count != 0) return;

        //    //reload all schedule
        //    foreach (var node in CourseNodes)
        //    {
        //        bool IsReallyChoose = false;
        //        foreach (var child in node.SubNodes)
        //        {

        //            if (child.Course.IsSelected)
        //            {
        //                if (IsReallyChoose)
        //                {
        //                    Debug.WriteLine("Trung mon roi!");
        //                    break;
        //                }
        //                IsReallyChoose = true;

        //            }
        //        }
        //        IsReallyChoose = false;
        //    }
        //}

    }
}
