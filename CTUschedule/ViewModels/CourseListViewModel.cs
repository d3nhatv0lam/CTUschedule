using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Newtonsoft.Json;
using System.Diagnostics;
using CTUschedule.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Threading;
using CTUschedule.Resources.Dialogs;
using CTUschedule.Utilities;
using System.Threading;
using OpenQA.Selenium.DevTools.V138.Network;


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
                //if (_courseName == value) return;
                _courseName = value;
                OnPropertyChanged(nameof(CourseName));
                if (String.IsNullOrEmpty(_courseName))
                {
                    QuickselectList.Clear();
                    return;
                }
                QuickSearchDelayAndRunOnLastUpdate();
               
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
                if (value == null) return;
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
                GetTenHocPhanVaMaHocPhanAfterSearch();
            }
        }

        [ObservableProperty]
        private ObservableCollection<CourseInformation> _filterCourseList = new ObservableCollection<CourseInformation>();

        public ObservableCollection<CourseNode> CourseNodes = new ObservableCollection<CourseNode>();

        private event EventHandler _courseNodesUpdate;

        public event EventHandler CourseNodesUpdate
        {
            add
            {
                _courseNodesUpdate += value;
            }
            remove
            {
                _courseNodesUpdate -= value;
            }
        }

        public void OnCourseNodesUpdate()
        {
            if (_courseNodesUpdate != null)
            {
                _courseNodesUpdate(this, new EventArgs());
            }
        }

        public CourseListViewModel() 
        {
            Instance = this;
            courseCatalog = new HTQL_CourseCatalog();
            courseCatalog.network.LoadingFinished += Network_LoadingFinished;
        }

        private async void Network_LoadingFinished(object? sender, LoadingFinishedEventArgs e)
        {
            GetResponseBodyCommandResponse? responseBody = null;
            try
            {
                responseBody = await courseCatalog.network.GetResponseBody(
                    new GetResponseBodyCommandSettings { RequestId = e.RequestId });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[Network] GetResponseBody failed: " + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(responseBody?.Body)) return;

            // Chỉ xử lý JSON
            if (IsJson(responseBody.Body))
            {
                if (TryParseQuickSelect(responseBody.Body)) return;
                TryParseCourseList(responseBody.Body);
            }
        }

        private bool IsJson(string body)
        {
            body = body.Trim();
            return (body.StartsWith("{") && body.EndsWith("}")) ||
                   (body.StartsWith("[") && body.EndsWith("]"));
        }


        private async void Network_ResponseReceived(
    object? sender,
    OpenQA.Selenium.DevTools.V138.Network.ResponseReceivedEventArgs e)
        {
            if (e.Response?.MimeType != "application/json") return;

            GetResponseBodyCommandResponse? responseBody = null;

            try
            {
                responseBody = await courseCatalog.network.GetResponseBody(
                    new GetResponseBodyCommandSettings { RequestId = e.RequestId });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Network] Cannot get response body: {ex.Message}");
                return;
            }

            if (string.IsNullOrEmpty(responseBody?.Body))
            {
                Debug.WriteLine("[Network] Empty response body");
                return;
            }

            // Thử parse QuickSelectData trước
            if (TryParseQuickSelect(responseBody.Body)) return;

            // Nếu không parse được QuickSelect thì thử CourseListData
            TryParseCourseList(responseBody.Body);
        }

        private bool TryParseQuickSelect(string jsonBody)
        {
            try
            {
                var quickSelect = JsonConvert.DeserializeObject<QuickSelectData>(jsonBody);
                if (quickSelect?.data?.dkmh_tu_dien_hoc_phan_ma_auto_complete == null)
                    return false;

                QuickselectList = new ObservableCollection<QuickselectInformation>(
                    quickSelect.data.dkmh_tu_dien_hoc_phan_ma_auto_complete);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Network] QuickSelect parse error: {ex.Message}");
                return false;
            }
        }

        private bool TryParseCourseList(string jsonBody)
        {
            try
            {
                var courseListData = JsonConvert.DeserializeObject<CourseListData>(jsonBody);
                if (courseListData?.data?.data == null) return false;

                CourseList = new ObservableCollection<CourseInformation>(
                    courseListData.data.data);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Network] CourseList parse error: {ex.Message}");
                return false;
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


        private void GetTenHocPhanVaMaHocPhanAfterSearch()
        {
            if (CourseList.Count == 0)
            {
                TenHocPhan = MaHocPhan = "";
            }
            else
            {
                TenHocPhan = CourseList.First().dkmh_tu_dien_hoc_phan_ten_vn;
                MaHocPhan = CourseList.First().dkmh_tu_dien_hoc_phan_ma;
            }
        }

        private bool IsHasInternet()
        {
            return CheckerInternetHelper._isHasInternet;
        }

        // khóa đối tượng cho 1 luồng
        private static readonly object _lock = new object();
        CancellationTokenSource _cts;

        // chạy lệnh quicksearch khi không có cập nhập gì về CourseName trong 250ms
        private void QuickSearchDelayAndRunOnLastUpdate()
        {
            // tạo một hàng đợi khi gọi hàm
            lock (_lock)
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                // khi Task.run này xảy ra thì hàm này đã xong, thực hiện hàm đợi, khi đó sẽ hủy token cũ đi -> hủy luôn hàm Task.run này
                // chỉ khi nào hàng đợi hết thì mới thực hiện được task quicksearch
                Task.Run(async () =>
                {
                    await Task.Delay(250,token);
                    try
                    {
                        if (!token.IsCancellationRequested)
                            courseCatalog.QuickSearch(CourseName);
                        
                    }
                    catch (TaskCanceledException)
                    {

                    }
                });
            }
        }

        [RelayCommand]
        public void SearchCourseData()
        {
            if (!IsHasInternet()) return;
            courseCatalog.Search(CourseName);
        }

        public void SearchCourseData(string courseName,List<string> nhomHocphan)
        {
            if (!IsHasInternet()) return;
            if (!courseCatalog.IsDriveUrlCatalogPage())
            {
                courseCatalog.NavigateToCourseCatalog();
            }


            courseCatalog.Search(courseName);

            FilterCourseList = new ObservableCollection<CourseInformation>(CourseList.Where((course)=> nhomHocphan.Contains(course.dkmh_nhom_hoc_phan_ma)));
            // chọn tất cả
            foreach (var course in FilterCourseList)
            {
                course.IsSelected = true;
            }
            SaveCourseNode();
        }


        private void SaveCourseNode()
        {
            ObservableCollection<CourseNode> subNode = new ObservableCollection<CourseNode>();
            //// get All học phần đã chọn vào 1 subnode
            for (int i = 0; i < FilterCourseList.Count; i++)
            {
                // tìm thấy một nhóm HP được chọn
                if (!FilterCourseList[i].IsSelected) continue;
                // 1 node chứa N nhóm giống nhau
                ObservableCollection<CourseInformation> CourseGroup = new ObservableCollection<CourseInformation>();
                for (int j = 0; j < FilterCourseList.Count; j++)
                {
                    // kiểm tra xem các Course khác có cùng nhóm đã được chọn
                    if (FilterCourseList[i].dkmh_nhom_hoc_phan_ma == FilterCourseList[j].dkmh_nhom_hoc_phan_ma)
                    {
                        // them vào nhóm
                        CourseGroup.Add(FilterCourseList[j]);
                        // sau khi add xong rồi thì un check đi
                        FilterCourseList[j].IsSelected = false;
                    }
                }
                // add vào subnode
                subNode.Add(new CourseNode(CourseGroup));
            }
            if (subNode.Count == 0) return;

            // tim các dữ liệu đã lưu xem đã từng lưu chưa?
            bool IsChanged = false;
            for (int i = 0; i < CourseNodes.Count; i++)
            {
                // đã có thì update cái mới đè lên
                if (CourseNodes[i].MaHocPhan == subNode.First().CourseGroup.First().dkmh_tu_dien_hoc_phan_ma)
                {
                    // lưu lại các tùy chọn tkb TRANG SCHEDULE
                    //for(int j = 0; j < subNode.Count; j++)
                    //{
                    //    subNode[j].IsScheduleSelected = CourseNodes[i].SubNodes[j].IsScheduleSelected;
                    //}

                    // Từng subnode con sẽ đem insert vào node cha
                    for (int j = 0; j < subNode.Count; j++)
                    {
                        // lấy nhóm học phần để tìm và chèn
                        string nhomHocPhan = subNode[j].CourseGroup.First().dkmh_nhom_hoc_phan_ma;
                        // tìm vị trí để chèn vào tăng dần cho đẹp
                        int insertIndex = CourseNodeListFindIndex.FindIndex(CourseNodes[i],nhomHocPhan);

                        // nhóm hp lớn nhất
                        if (insertIndex == CourseNodes[i].SubNodes.Count)
                        {
                            CourseNodes[i].SubNodes.Add(subNode[j]);
                        }
                        else
                        // nếu tồn tại nhóm HP này trong danh sách
                        if (CourseNodes[i].SubNodes[insertIndex].CourseGroup.First().dkmh_nhom_hoc_phan_ma == nhomHocPhan)
                        {
                            // gán trạng thái lịch tkb lại cho node mới
                            subNode[j].IsScheduleSelected = CourseNodes[i].SubNodes[insertIndex].IsScheduleSelected;
                            //thay node mới vào
                            CourseNodes[i].SubNodes[insertIndex] = subNode[j];
                        }
                        // chèn vào vị trí phù hợp
                        else CourseNodes[i].SubNodes.Insert(insertIndex, subNode[j]);
                    }

                    //CourseNodes[i] = new CourseNode(MaHocPhan, TenHocPhan, subNode);
                    IsChanged = true;
                    break;
                }
            }
            // chưa lưu thì lưu
            if (!IsChanged) CourseNodes.Add(new CourseNode(MaHocPhan, TenHocPhan, subNode));

            //Update lại UI của trang này
            //FilterCourseList = new ObservableCollection<CourseInformation>(FilterCourseList);
            OnPropertyChanged(nameof(FilterCourseList));
            // gọi trang Edit Update lại
            OnCourseNodesUpdate();
        }

        [RelayCommand]
        public void SaveCourseData()
        {
            SaveCourseNode();

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

            if (!IsHasInternet()) return;
            courseCatalog.NavigateToCourseCatalog();
        }
    }
}
