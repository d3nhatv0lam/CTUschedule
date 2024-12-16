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
                OnPropertyChanged(nameof(CourseList));
            }
        }


        private List<CourseInformation> SelectedCourseList = new List<CourseInformation>();

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
            // get All học phần đã chọn
            for (int i = 0; i < CourseList.Count; i++)
            {
                if (!CourseList[i].IsSelected) continue;
                
                for (int j = 0; j < CourseList.Count; j++)
                {
                    if (CourseList[i].dkmh_nhom_hoc_phan_ma == CourseList[j].dkmh_nhom_hoc_phan_ma)
                        SelectedCourseList.Add(CourseList[j]);
                    else continue;
                }
            }
            //SelectedCourseList = CourseList.Where(Course => Course.IsSelected == true).ToList();
            Debug.WriteLine(SelectedCourseList.Count);
        }

        public void Init()
        {
            CourseName = String.Empty;
            courseCatalog.NavigateToCourseCatalog();
        }
    }
}
