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
        public ObservableCollection<CourseNode> CourseNodes { get; set;}

        [ObservableProperty]
        // 9row, 6colum
        private List<ObservableCollection<ScheduleCell>> _schedule = new List<ObservableCollection<ScheduleCell>>();
        private List<ObservableCollection<ScheduleCell>> emptyScheduleTable;


        public ScheduleViewModel() 
        {
            Instance = this;
            _courseListEditViewModel = CourseListEditViewModel.Instance;
            Init();

            _courseListEditViewModel.ScheduleChanged += _courseListEditViewModel_ScheduleChanged;
 
        }

        public void Init()
        {
            emptyScheduleTable = EmptyTableSchedule();
            Schedule = emptyScheduleTable;
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
            CourseNodes = CourseNode.Uncheck_UnExpandCourseNode(_courseListEditViewModel.CourseNodes);
        }


        [RelayCommand]
        public void CheckBoxTask(ObservableCollection<CourseNode> scheduleList)
        {
            if (scheduleList.Count != 0) return;

            //reload all schedule
            foreach (var node in CourseNodes)
            {
                bool IsReallyChoose = false;
                foreach(var child in node.SubNodes)
                {
                   
                    if (child.Course.IsSelected)
                    {
                        if (IsReallyChoose)
                        {
                            Debug.WriteLine("Trung mon roi!");
                            break;
                        }
                        IsReallyChoose = true;
                        ScheduleCellTableIndex scheduleCellTableIndex = ScheduleCellTableIndex.GetTableIndex(child.Course);
                        var tableIndex = Schedule[scheduleCellTableIndex.TietBatDau][scheduleCellTableIndex.ThuDiHoc - 1];
                        tableIndex = new ScheduleCell(child.Course);
                        Schedule[scheduleCellTableIndex.TietBatDau][scheduleCellTableIndex.ThuDiHoc - 1] = tableIndex;
                        Schedule = new List<ObservableCollection<ScheduleCell>>(Schedule);
                    }
                }
                IsReallyChoose = false;
            }
        }

    }
}
