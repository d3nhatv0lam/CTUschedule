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

namespace CTUschedule.ViewModels
{
    public partial class CourseListViewModel : ViewModelBase
    {

        public static CourseListViewModel Instance { get; set; }
        private HTQL_CourseCatalog courseCatalog;

        private string _courseName;

        [ObservableProperty]
        private ObservableCollection<QuickselectInformation> _quickselectinfor;

        public string CourseName
        {
            get => _courseName;
            set
            {
                _courseName = value;
                OnPropertyChanged(nameof(CourseName));
                courseCatalog.QuickSearch(CourseName);
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
                        Quickselectinfor = new ObservableCollection<QuickselectInformation>(quickslect.data.dkmh_tu_dien_hoc_phan_ma_auto_complete);
                    }
                    catch
                    {
                        try
                        {
                            var courseListData = JsonConvert.DeserializeObject<CourseListData>(responseBody.Body);
                        }
                        catch
                        {
                            // lỗi
                        }
                    }
                }
            }
        }

        public void Init()
        {
            CourseName = String.Empty;
            courseCatalog.NavigateToCourseCatalog();
        }
    }
}
