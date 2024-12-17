using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CTUschedule.Models;
using CTUschedule.Resources.Dialogs;
using DialogHostAvalonia;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.ViewModels
{
    public partial class CourseListEditViewModel : ViewModelBase
    {
        public static CourseListEditViewModel Instance { get; set; }
        private CourseListViewModel _courseListViewModel { get; }

        [ObservableProperty]
        private ObservableCollection<CourseNode> _courseNodes = new ObservableCollection<CourseNode>();

        [ObservableProperty]
        private bool _isOpenDialog = false;

  

        public CourseListEditViewModel()
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
            //            dkmh_tu_dien_hoc_phan_ma ="CT172",
            //            dkmh_nhom_hoc_phan_ma = "01",
            //            si_so_con_lai = 10,
            //            dkmh_tu_dien_giang_vien_ten_vn = "Test1",
            //        }),
            //         new CourseNode(new CourseInformation()
            //        {
            //             dkmh_tu_dien_hoc_phan_ma ="CT172",
            //            dkmh_nhom_hoc_phan_ma = "heelooo",
            //            si_so_con_lai = 100,
            //            dkmh_tu_dien_giang_vien_ten_vn = "Test2",
            //        }),
            //    }),
            //    new CourseNode("CT17x","Test2", new ObservableCollection<CourseNode>()
            //    {
            //        new CourseNode(new CourseInformation()
            //        {
            //            dkmh_tu_dien_hoc_phan_ma ="CT17x",
            //            dkmh_nhom_hoc_phan_ma = "01",
            //            si_so_con_lai = 10,
            //            dkmh_tu_dien_giang_vien_ten_vn = "Test3",
            //        }),
            //         new CourseNode(new CourseInformation()
            //        {
            //             dkmh_tu_dien_hoc_phan_ma ="CT17x",
            //            dkmh_nhom_hoc_phan_ma = "02",
            //            si_so_con_lai = 100,
            //            dkmh_tu_dien_giang_vien_ten_vn = "Test4",
            //        }),
            //    }),
            //};
        }

        public void Init()
        {

        }
        [RelayCommand]
        public void Selected_UnSelectItem(ObservableCollection<CourseNode>? course)
        {
            // check Node has Full Value
            if (course.Count == 0)
            {
                foreach (var node in CourseNodes)
                {
                    if (node.Course.IsSelected)
                    {
                        var check = node.SubNodes.Any((courseNode) => courseNode.Course.IsSelected == false);
                        // has a element select is false => parent select = false;
                        if (check == true) node.Course.IsSelected = false;
                    }
                    else
                    {
                        var check = node.SubNodes.All((courseNode) => courseNode.Course.IsSelected == true);
                        if (check == true) node.Course.IsSelected = true;
                    }
                }
                CourseNodes = new ObservableCollection<CourseNode>(CourseNodes);
                return;
            }

            // set all Node is T/F
            foreach (var node in CourseNodes)
            {
                if (node.MaHocPhan == course[0].Course.dkmh_tu_dien_hoc_phan_ma)
                {
                    foreach (var updateNode in node.SubNodes)
                    {
                        updateNode.Course.IsSelected = node.Course.IsSelected;
                    }
                }
            }
            CourseNodes = new ObservableCollection<CourseNode>(CourseNodes);
            //OnPropertyChanged(nameof(CourseNodes));
        }

        public void DeleteSelectedCourse()
        {
            ObservableCollection<CourseNode> deletedCourseNodes = new ObservableCollection<CourseNode>();
            foreach (var node in CourseNodes)
            {
                // cả nhánh đều được chọn => bỏ hết
                if (node.Course.IsSelected) continue;
                // tìm các con của node đó xem coi có node nào bị select không
                ObservableCollection<CourseNode> subnodes = new ObservableCollection<CourseNode>();
                foreach(var child in node.SubNodes)
                {
                    // bị select thì bỏ ra
                    if (child.Course.IsSelected) continue;
                    // không thì thêm vào
                    subnodes.Add(child);
                }
                // gắn các node đã tìm kiếm lại
                deletedCourseNodes.Add(new CourseNode(node.MaHocPhan, node.TenHocPhan, subnodes));
            }
            CourseNodes = deletedCourseNodes;
            //CourseNodes = new ObservableCollection<CourseNode>(deletedCourseNodes);
        }

      

        [RelayCommand]
        public void OpenDialog()
        {
           IsOpenDialog = true;
        }

        [RelayCommand]
        public void CloseDialog(bool IsDeleteData) 
        {
            IsOpenDialog = false;
            if (IsDeleteData)
            {
                DeleteSelectedCourse();
                INotificationPopup deletedPopup = new NotificationPopupController(NotificationPopupController.Type.Succes, "Xóa thành công!", "Đã xóa các nhóm được chọn");
                deletedPopup.ShowNotification();
            }
        }

        private void _courseListViewModel_CourseListUpdate(object? sender, EventArgs e)
        {
            CourseNodes = _courseListViewModel.SelectedCourseList;
        }

    }
}
