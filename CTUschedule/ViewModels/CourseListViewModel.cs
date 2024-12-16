using OpenQA.Selenium.BiDi.Modules.Network;
using OpenQA.Selenium.DevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using OpenQA.Selenium.DevTools.V85.Network;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Diagnostics;
using CTUschedule.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Threading;
using CTUschedule.Resources.Dialogs;

namespace CTUschedule.ViewModels
{
    public partial class CourseListViewModel : ViewModelBase
    {
        public static CourseListViewModel Instance { get; set; }
        private HTQL_CourseCatalog courseCatalog;

        private string _courseName;
        public string CourseName
        {
            get => _courseName;
            set
            {
                if (_courseName == value) return;

                _courseName = value;
                OnPropertyChanged(nameof(CourseName));
                if (String.IsNullOrEmpty(_courseName))
                {
                    QuickselectList.Clear();
                    return;
                }
                courseCatalog.QuickSearch(CourseName);
            }
        }

        [ObservableProperty]
        private string _tenHocPhan = "";
        [ObservableProperty]
        private string _maHocPhan = "";

        private bool _isHideOutOfSlot = false;

        public bool IsHideOutOfSlot
        {
            get => _isHideOutOfSlot;
            set
            {
                if (_isHideOutOfSlot == value) return;
                _isHideOutOfSlot = value;
                OnPropertyChanged(nameof(IsHideOutOfSlot));
                ReloadFilterDatagid();
            }
        }

        // Môn đã được chọn nhanh
        [ObservableProperty]
        private ObservableCollection<QuickselectInformation> _quickselectList = new ObservableCollection<QuickselectInformation>();

        private QuickselectInformation _quickSelecteditem;
        public QuickselectInformation QuickSelecteditem
        {
            get => _quickSelecteditem;
            set
            {
                //_quickSelecteditem = value;
                CourseName = value.value;
            }
        }
        

        private ObservableCollection<CourseInformation> _courseList = new ObservableCollection<CourseInformation>();

        public ObservableCollection<CourseInformation> CourseList
        {
            get => _courseList;
            set
            {
                if (_courseList == value) return;
                _courseList= value;
                ReloadFilterDatagid();
            }
        }

        [ObservableProperty]
        private ObservableCollection<CourseInformation> _filterCourseList = new ObservableCollection<CourseInformation>();

        public ObservableCollection<CourseNode> SelectedCourseList = new ObservableCollection<CourseNode>();

        private event EventHandler _courseListUpdate;

        public event EventHandler CourseListUpdate
        {
            add
            {
                _courseListUpdate += value;
            }
            remove
            {
                _courseListUpdate -= value;
            }
        }

        public void OnCourseListUpdate()
        {
            if (_courseListUpdate != null)
            {
                _courseListUpdate(this, new EventArgs());
            }
        }

        public CourseListViewModel() 
        {
            Instance = this;
            courseCatalog = new HTQL_CourseCatalog();
            courseCatalog.network.ResponseReceived += Network_ResponseReceived;
        }

        private async void Network_ResponseReceived(object? sender, OpenQA.Selenium.DevTools.V85.Network.ResponseReceivedEventArgs e)
        {
            if (e.Response.MimeType == "application/json")
            {
                //responseBody;
                GetResponseBodyCommandResponse responseBody = null;
                // check quickSelect
                try
                {
                     responseBody = await courseCatalog.network.GetResponseBody(new GetResponseBodyCommandSettings { RequestId = e.RequestId });
                }
                catch (Exception ex)
                {
                    // can't get respond
                    Debug.WriteLine(ex.Message);
                    // lỗi
                }
                finally
                {
                    //try get quickselect
                    try
                    {
                        var quickslect =  JsonConvert.DeserializeObject<QuickSelectData>(responseBody.Body);
                        QuickselectList = new ObservableCollection<QuickselectInformation>(quickslect.data.dkmh_tu_dien_hoc_phan_ma_auto_complete);
                    }   
                    catch
                    {
                        try
                        {
                            var courseListData = JsonConvert.DeserializeObject<CourseListData>(responseBody.Body);
                            CourseList = new ObservableCollection<CourseInformation>(courseListData.data.data);
                        }
                        catch
                        {
                            // lỗi
                        }
                    }
                }
            }
        }

        private void ReloadFilterDatagid()
        {
            if (IsHideOutOfSlot == true)
            {
                FilterCourseList = new ObservableCollection<CourseInformation>(CourseList.Where(course => course.si_so_con_lai != 0));
            }
            else FilterCourseList = CourseList;
        }

        [RelayCommand]
        public void SearchCourseData()
        {
            courseCatalog.Search(CourseName);
            if (CourseList.Count == 0)
            {
                TenHocPhan = MaHocPhan = "";
            }
            else
            {
                TenHocPhan = CourseList.First().dkmh_tu_dien_hoc_phan_ten_vn;
                MaHocPhan = CourseName;
            }
        }

        [RelayCommand]
        public void SaveCourseData()
        {
            
            ObservableCollection<CourseNode> subNode = new ObservableCollection<CourseNode>();
            //// get All học phần đã chọn vào 1 subnode
            for (int i = 0; i < FilterCourseList.Count; i++)
            {
                if (!FilterCourseList[i].IsSelected) continue;

                for (int j = 0; j < FilterCourseList.Count; j++)
                {
                    if (FilterCourseList[i].dkmh_nhom_hoc_phan_ma == FilterCourseList[j].dkmh_nhom_hoc_phan_ma)
                    {
                        subNode.Add(new CourseNode(FilterCourseList[j]));
                        FilterCourseList[j].IsSelected = false;
                    }
                    else continue;
                }
            }
            if (subNode.Count == 0) return;

            // tim các dữ liệu đã lưu xem đã từng lưu chưa?
            bool IsChanged = false;
            for (int i = 0; i < SelectedCourseList.Count; i++)
            {
                // đã có thì update cái mới đè lên
                if (SelectedCourseList[i].MaHocPhan == subNode.First().Course.dkmh_tu_dien_hoc_phan_ma)
                {

                    SelectedCourseList[i] = new CourseNode(MaHocPhan, TenHocPhan, subNode);
                    IsChanged = true; break;
                }
            }
            // chưa lưu thì lưu
            if (!IsChanged) SelectedCourseList.Add(new CourseNode(MaHocPhan, TenHocPhan, subNode));

            // gọi trang lịch tkb update
            OnCourseListUpdate();

            INotificationPopup SuccessPopup = new NotificationPopupController(NotificationPopupController.Type.Succes, "Thêm Nhóm thành Công!", "Bạn đã có thể thêm vào thời khóa biểu");
            SuccessPopup.ShowNotification();
        }

        public void Init()
        {
            CourseName = MaHocPhan = TenHocPhan = String.Empty;
            Dispatcher.UIThread.Invoke(() =>
            {
                CourseList.Clear();
                FilterCourseList.Clear();
            });
            courseCatalog.NavigateToCourseCatalog();
        }
    }
}
