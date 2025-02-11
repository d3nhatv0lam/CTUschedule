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
        private ObservableCollection<ObservableCollection<ScheduleCell>> _schedule;

        private ObservableCollection<ScheduleCell> ScheduleItemPool = new ObservableCollection<ScheduleCell>();
        private ObservableCollection<ScheduleCell> SelectedScheduleItem = new ObservableCollection<ScheduleCell>();

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
        private ObservableCollection<ObservableCollection<ScheduleCell>> EmptyTableSchedule(int rows = 10, int columns = 6)
        {
            ObservableCollection<ObservableCollection<ScheduleCell>> newTable = new ObservableCollection<ObservableCollection<ScheduleCell>>();
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

        // đặt tất cả về thành ô trống => quản lý vùng nhớ hiệu quả
        private void CleanScheduleTable(ObservableCollection<ObservableCollection<ScheduleCell>> table)
        {
            for (int i = 0; i < table.Count; i++)
                for (int j = 0; j < table[i].Count; j++)
                    table[i][j] = new ScheduleCell();
        }

        // xảy ra khi Node trang EditView có biến động
        private void _courseListEditViewModel_ScheduleChanged(object? sender, EventArgs e)
        {
            CourseNodes = _courseListEditViewModel.CourseNodes;
            OnPropertyChanged(nameof(CourseNodes));

            CleanScheduleTable(Schedule);
            // xóa trước để quản lý vùng nhớ
            ScheduleItemPool = null;
            // sau đó gán lại
            ScheduleItemPool = CreateScheduleItemPool();
            RenderSchedule();
        }

        // tạo một pool các item từ các Course
        private ObservableCollection<ScheduleCell> CreateScheduleItemPool()
        {
            ObservableCollection<ScheduleCell> newPool = new ObservableCollection<ScheduleCell>();
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
                // tìm item trong itempool đê gắn lên TKB
                foreach (var itempool in ScheduleItemPool)
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
                foreach (var child in node.SubNodes)
                {
                    // Selected này lấy ở FE
                    if (child.IsScheduleSelected)
                    {
                        AddCourseToSchedule(child.CourseGroup);
                    }
                }
            }
            OnPropertyChanged(nameof(Schedule));
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
                                    INotificationPopup TrungMonPopup = new NotificationPopupController(NotificationPopupController.Type.ScheduleHasItem, "Trùng Môn", $"Đã có học phần {maHocPhan}!");
                                    TrungMonPopup.ShowNotification();
                                    checkbox.IsChecked = false;
                                    OnPropertyChanged(nameof(CourseNodes));
                                    //CourseNodes = CourseNodes;
                                    return;
                                }
                                else
                                isHasChecked = true;
                            }
                        }


                  

                        List<ScheduleCell> cellWillShowed = new List<ScheduleCell>();
                        // lấy ra các Cell sắp được thêm từ pool
                        foreach (var itempool in ScheduleItemPool)
                        {
                            // điều kiện cần để xác định cell
                            if (itempool.MaHocPhan == maHocPhan && itempool.NhomHocPhan == nhomhocphan)
                            {
                                cellWillShowed.Add(itempool);
                            }
                        }
                        
                        // check trùng lịch của từng môn
                        foreach (var appendCell in cellWillShowed)
                        {
                            foreach(var enableItemInPool in SelectedScheduleItem)
                            {
                                // nếu trùng thứ đi học thì kiểm tra thêm
                                if (appendCell.ThuDiHoc == enableItemInPool.ThuDiHoc)
                                {
                                    
                                    // trường hợp TKB này ở trên hơn so với tkb đã thêm vào lịch và tiết kết thúc của môn nằm trên vẫn nằm trên tiết bắt đầu của môn dưới
                                    // append cell nằm  trên
                                    if ( appendCell.TietBatDau < enableItemInPool.TietBatDau && appendCell.TietKetThuc < enableItemInPool.TietBatDau)
                                           continue;
                                    // append cell nằm dưới                                 // tiết bắt đầu môn nằm dưới phải lớn hơn tiết kết thúc môn nằm trên
                                    if (appendCell.TietBatDau > enableItemInPool.TietBatDau && appendCell.TietBatDau > enableItemInPool.TietKetThuc) 
                                        continue;

                                    
                                    // => trung lich
                                    INotificationPopup TrungLichPopup = new NotificationPopupController(NotificationPopupController.Type.ScheduleConflit, "Trùng Lịch", $"Trùng lịch với {enableItemInPool.MaHocPhan} - Thứ {enableItemInPool.ThuDiHoc}");
                                    TrungLichPopup.ShowNotification();
                                    checkbox.IsChecked = false;
                                    OnPropertyChanged(nameof(CourseNodes));
                                    cellWillShowed = null;
                                    return;
                                    
                                }
                                
                            }
                        }
                        cellWillShowed = null;
                    }
                }

            // check thành công, có thể thêm vào lịch
            CleanScheduleTable(Schedule);
            RenderSchedule();
        }

    }
}
