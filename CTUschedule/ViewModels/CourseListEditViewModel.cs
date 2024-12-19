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
using System.Text.Json;
using System.Threading.Tasks;
//using Newtonsoft.Json;

namespace CTUschedule.ViewModels
{
    public partial class CourseListEditViewModel : ViewModelBase
    {
        public static CourseListEditViewModel Instance { get; set; }
        private MainHomeViewModel _mainHomeViewModel { get; }
        private CourseListViewModel _courseListViewModel { get; }


        private ObservableCollection<CourseNode> _courseNodes = new ObservableCollection<CourseNode>();

        public ObservableCollection<CourseNode> CourseNodes
        {
            get => _courseNodes;
            set
            {
                _courseNodes = value;
                OnPropertyChanged(nameof(CourseNodes));
                OnScheduleChanged(); 
            }
        }

        [ObservableProperty]
        private bool _isOpenDialog = false;

        private event EventHandler _scheduleChanged;

        public event EventHandler ScheduleChanged
        {
            add
            {
                _scheduleChanged += value;
            }
            remove
            {
                _scheduleChanged -= value;
            }
        }

        private void OnScheduleChanged()
        {
            if (_scheduleChanged != null)
            {
                _scheduleChanged(this, new EventArgs());
            }
        }


        public CourseListEditViewModel()
        {
            Instance = this;
            _mainHomeViewModel = MainHomeViewModel.Instance;
            _courseListViewModel = CourseListViewModel.Instance;
            _courseListViewModel.CourseListUpdate += _courseListViewModel_CourseListUpdate;

            CourseNodes = new ObservableCollection<CourseNode>
            {
                new CourseNode("CT172","asd", new ObservableCollection<CourseNode>()
                {
                    new CourseNode(new CourseInformation()
                    {
                        dkmh_tu_dien_hoc_phan_ma ="CT172",
                        dkmh_nhom_hoc_phan_ma = "01",
                        dkmh_tu_dien_lop_hoc_phan_si_so = 40,
                        si_so_con_lai = 10,
                        dkmh_thu_trong_tuan_ma = 5,
                        tiet_hoc="---------",
                        dkmh_tu_dien_phong_hoc_ten ="C1002",
                        dkmh_tu_dien_giang_vien_ten_vn = "GV1",
                    }),
                     new CourseNode(new CourseInformation()
                    {
                         dkmh_tu_dien_hoc_phan_ma ="CT172",
                        dkmh_nhom_hoc_phan_ma = "02",
                        dkmh_tu_dien_lop_hoc_phan_si_so = 200,
                        si_so_con_lai = 100,
                        dkmh_thu_trong_tuan_ma = 7,
                         tiet_hoc="---------",
                        dkmh_tu_dien_phong_hoc_ten ="D1002",
                        dkmh_tu_dien_giang_vien_ten_vn = "GV1",
                    }),
                }),
                new CourseNode("CT17x","Test2", new ObservableCollection<CourseNode>()
                {
                    new CourseNode(new CourseInformation()
                    {
                        dkmh_tu_dien_hoc_phan_ma ="CT17x",
                        dkmh_nhom_hoc_phan_ma = "01",
                        si_so_con_lai = 10,
                        dkmh_tu_dien_lop_hoc_phan_si_so = 40,
                        dkmh_thu_trong_tuan_ma = 3,
                        tiet_hoc="---------",
                        dkmh_tu_dien_phong_hoc_ten ="C1003",
                        dkmh_tu_dien_giang_vien_ten_vn = "Test3",
                    }),
                     new CourseNode(new CourseInformation()
                    {
                         dkmh_tu_dien_hoc_phan_ma ="CT17x",
                        dkmh_nhom_hoc_phan_ma = "02",
                        dkmh_tu_dien_lop_hoc_phan_si_so = 40,
                        si_so_con_lai = 0,
                        dkmh_thu_trong_tuan_ma = 2,
                        tiet_hoc="123-----",
                        dkmh_tu_dien_phong_hoc_ten ="DB/201",
                        dkmh_tu_dien_giang_vien_ten_vn = "Test4",
                    }),
                      new CourseNode(new CourseInformation()
                    {
                        dkmh_tu_dien_hoc_phan_ma ="CT17x",
                        dkmh_nhom_hoc_phan_ma = "03",
                        dkmh_tu_dien_lop_hoc_phan_si_so = 40,
                        si_so_con_lai = 1,
                        dkmh_thu_trong_tuan_ma = 2,
                        tiet_hoc="----678-",
                        dkmh_tu_dien_phong_hoc_ten ="DB/201",
                        dkmh_tu_dien_giang_vien_ten_vn = "Test4",
                    }),
                }),
            };
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
                // phải xóa 1 nhóm học phần
                string nhomHocPhan = node.MaHocPhan;
                foreach(var child in node.SubNodes)
                {
                    // bị select thì bỏ ra
                    if (child.Course.IsSelected)
                    {
                        // lưu nhóm học phần bị xóa lại để xóa hết các nhóm chung
                        nhomHocPhan = child.Course.dkmh_nhom_hoc_phan_ma;
                        continue;
                    };
                    //
                    if (child.Course.dkmh_nhom_hoc_phan_ma == nhomHocPhan) continue;
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
        public async void ReloadData()
        {
            _mainHomeViewModel.IsChangingView = true;
            await Task.Run(() =>
            {
                ObservableCollection<CourseNode> newData = new ObservableCollection<CourseNode>();

                foreach (var node in CourseNodes)
                {
                    // get MaHocPhan
                    string MaHocPhan = node.MaHocPhan;
                    // get group hocphan
                    List<string> nhomHocPhan = new List<string>();
                    foreach (var listitem in node.SubNodes)
                    {
                        if (nhomHocPhan.Count == 0) nhomHocPhan.Add(listitem.Course.dkmh_nhom_hoc_phan_ma);
                        else
                        if (listitem.Course.dkmh_nhom_hoc_phan_ma != nhomHocPhan.Last()) nhomHocPhan.Add(listitem.Course.dkmh_nhom_hoc_phan_ma);
                    }
                    // search
                    _courseListViewModel.SearchCourseData(MaHocPhan, nhomHocPhan);
                }
            });
            _mainHomeViewModel.IsChangingView = false;
            INotificationPopup Reloadedpopup = new NotificationPopupController(NotificationPopupController.Type.Succes, "Cập nhật thành công!", "Đã Cập nhật các nhóm học phần");
            Reloadedpopup.ShowNotification();
        }

        [RelayCommand]
        public async void SaveData()
        {
            SaveFileDialog saveDialog = new SaveFileDialog()
            {
                DefaultExtension= "json",
                ShowOverwritePrompt = true,
                InitialFileName = "Nhóm học phần_Backup",
                Title = "Choose Save Location",
                Filters = new List<FileDialogFilter> { 
                    new FileDialogFilter { Name = "JSON Files", Extensions = new List<string> { "json" }}, 
                    //new FileDialogFilter { Name = "All Files", Extensions = new List<string> { "*" }}
                },
            };

            
            var SaveLocationResult = await saveDialog.ShowAsync(new Window()
            {
                Topmost = true,
            });

            if (String.IsNullOrEmpty(SaveLocationResult)) return;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // Tạo JSON đẹp hơn
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            string json = JsonSerializer.Serialize(CourseNodes, options);
            System.IO.File.WriteAllText(SaveLocationResult, json);

            INotificationPopup SavedPopup = new NotificationPopupController(NotificationPopupController.Type.Succes, "Lưu thành công!", "Dữ liệu đã được lưu!");
            SavedPopup.ShowNotification();
        }

        [RelayCommand]
        public async void LoadData()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                AllowMultiple = false,
                Title = "Choose Dile!",
                Filters = new List<FileDialogFilter> {
                    new FileDialogFilter { Name = "JSON Files", Extensions = new List<string> { "json" }},
                },
            };

            var LoadLocationResult = await dialog.ShowAsync(new Window()
            {
                Topmost = true,
            });
            if (String.IsNullOrEmpty(LoadLocationResult.First())) return;

            try
            {
                string json = System.IO.File.ReadAllText(LoadLocationResult.First());
                var Loadednodes = JsonSerializer.Deserialize<List<CourseNode>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                CourseNodes = new ObservableCollection<CourseNode>(Loadednodes);
                INotificationPopup LoadedPopup = new NotificationPopupController(NotificationPopupController.Type.Succes, "Load thành công!", "Dữ liệu đã được load!");
                LoadedPopup.ShowNotification();
            }
            catch (Exception ex)
            {
                INotificationPopup LoadFail = new NotificationPopupController(NotificationPopupController.Type.Error, "Load Thất bại!", "Xin hãy kiểm tra lại thông tin");
                LoadFail.ShowNotification();
            }
        }

        [RelayCommand]
        public void OpenDialog(string typeOpen)
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
