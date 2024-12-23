using Avalonia.Controls;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CTUschedule.Models;
using CTUschedule.Resources.Dialogs;
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
        private List<ScheduleCell> SelectedScheduleItem = new List<ScheduleCell>();

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
        // thời khóa biểu mặc dịnh có 10 row 6 col
        private List<ObservableCollection<ScheduleCell>> EmptyTableSchedule(int rows = 10, int columns = 6)
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
            Schedule = EmptyTableSchedule();
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
                        SelectedScheduleItem.Add(itempool);
                    }
                }
            }
        }

        private void RenderSchedule()
        {
            SelectedScheduleItem.Clear();
            foreach (var node in CourseNodes)
            {
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
            Schedule = Schedule;
        }

        [RelayCommand]
        public void ReLoadData()
        {
            _courseListEditViewModel.ReloadData();
        }


        [RelayCommand]
        // thêm lịch vào TKB
        public void CheckBoxTask(TreeViewItem treeViewItem)
        {
            // Check u check in Child
            var parent = treeViewItem.Parent as TreeViewItem;
            if (parent == null) return;

            // get MaHocPhan of Child
            var grid = parent.GetVisualDescendants().OfType<Grid>().FirstOrDefault();
            if (grid == null) return;
            var maHocPhan = grid.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault().Text;

            var childgrid = treeViewItem.GetVisualDescendants().OfType<Grid>().FirstOrDefault();
            if (childgrid == null) return;
            // lấy checkbox chứa nhóm đã chọn
            var checkbox = childgrid.GetVisualDescendants().OfType<CheckBox>().FirstOrDefault();
            // lấy nhóm học phần đã chọn
            var nhomhocphan = childgrid.GetVisualDescendants().OfType<TextBlock>().ElementAt(2).Text;

            //đã check mới cần kiểm tra
            if (checkbox.IsChecked == true)
                // check Can Render?
                foreach (var node in CourseNodes)
                {
                    // check OnlyOne Of MaHocPhan Checked
                    if (maHocPhan == node.MaHocPhan)
                    {
                        bool isHasChecked = false;
                        foreach(var child in node.SubNodes)
                        {
                            if (child.IsScheduleSelected)
                            {
                                if (isHasChecked)
                                {
                                    //Error, trung môn
                                    INotificationPopup TrungMonPopup = new NotificationPopupController(NotificationPopupController.Type.Error, "Trùng Môn", $"Đã có học phần {maHocPhan}!");
                                    TrungMonPopup.ShowNotification();
                                    checkbox.IsChecked = false;
                                    CourseNodes = CourseNodes;
                                    return;
                                }
                                else
                                isHasChecked = true;
                            }
                        }
                        // Sum : check trùng lịch
                        foreach(var child in node.SubNodes)
                        {
                            // tìm nhóm học phần đã nhấn check
                            if (child.CourseGroup.First().dkmh_nhom_hoc_phan_ma == nhomhocphan)
                            {
                                
                                foreach(var course in child.CourseGroup)
                                {
                                    // lấy ra từng item trong pool đó
                                    ScheduleCell appendItem = null;
                                    foreach (var itempool in ScheduleItemPool)
                                    {
                                        // item trong pool định danh từ courseNode dùng mã học phần và nhóm học phần
                                        if (course.dkmh_tu_dien_hoc_phan_ma == itempool.MaHocPhan && course.dkmh_nhom_hoc_phan_ma == itempool.NhomHocPhan)
                                        {
                                            appendItem = itempool;
                                            break;
                                        }
                                    }
                                    if (appendItem == null) continue;
                                    // kiểm tra coi có trùng lịch với những học phần đã add không
                                    foreach(var selectedItemInPool in SelectedScheduleItem)
                                    {
                                            // check coi trùng thứ đi học không
                                        if (appendItem.ThuDiHoc == selectedItemInPool.ThuDiHoc )
                                        {
                                            // nếu trùng thứ đi học thì kiểm tra thêm
                                            // tiet bat dau cua mon 2 < tiet ket thuc của mon 1         // tiet ket thuc cua mon 2 > tiet bat dau cua mon 1
                                            if (appendItem.TietBatDau < selectedItemInPool.TietKetThuc || appendItem.TietKetThuc > selectedItemInPool.TietBatDau)
                                            {
                                                // => trung lich
                                                INotificationPopup TrungLichpopup = new NotificationPopupController(NotificationPopupController.Type.Error, "Trùng Lịch", $"Trùng lịch với học phần {selectedItemInPool.MaHocPhan}!");
                                                TrungLichpopup.ShowNotification();
                                                checkbox.IsChecked = false;
                                                CourseNodes = CourseNodes;
                                                return;
                                            }     
                                        }  
                                    }
                                }
                            }
                        }
                    }
                }

            // check thành công, có thể thêm vào lịch
            Schedule = EmptyTableSchedule();
            RenderSchedule();
        }

        [RelayCommand]
        public void Test()
        {
            Debug.WriteLine("hello");
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
