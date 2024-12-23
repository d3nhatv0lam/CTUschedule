using Newtonsoft.Json;
using System.Text.Json.Serialization;
using OpenQA.Selenium.DevTools.V129.DOM;
using OpenQA.Selenium.DevTools.V129.Memory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.Models
{
    public class CourseNode
    {

    

    public string MaHocPhan { get; set; }
        public string TenHocPhan { get; set; }
        public bool IsSelected { get; set; } = false;
        public bool IsExpanded { get; set; } = false;

        public bool IsScheduleSelected { get; set; } = false;
        public bool IsRedStatus { get; set; } = false;
        public bool IsYellowStatus { get; set; } = false;
        public bool IsGreenStatus { get; set; } = false;

        public ObservableCollection<CourseNode>? SubNodes { get; set; } = new ObservableCollection<CourseNode>();

        [System.Text.Json.Serialization.JsonIgnore]
        public CourseInformation? representativeNode
        {
            get
            {
                if (CourseGroup.Count == 0) return null;
                return (CourseInformation)CourseGroup[0];
            }
            set { }
        }

        public ObservableCollection<CourseInformation> CourseGroup { get; set; } = new ObservableCollection<CourseInformation>();


        public CourseNode() { }
        
         public CourseNode(string maHocPhan, string tenHocPhan)
        {
            MaHocPhan = maHocPhan;
            TenHocPhan = tenHocPhan;
        }

        public CourseNode(string maHocPhan, string tenHocPhan, ObservableCollection<CourseNode> subNodes)
        {
            MaHocPhan = maHocPhan;
            TenHocPhan = tenHocPhan;
            SubNodes = subNodes;
        }

        public CourseNode(ObservableCollection<CourseInformation> course)
        {
            CourseGroup = course;
            SetSlotStatus(CourseGroup.First());
        }

        private void SetSlotStatus(CourseInformation course)
        {
            if (course == null || course.si_so_con_lai == null) return;
            if (course.si_so_con_lai == 0) setStatus(false, false, false);
            else
            // (0%,10%]
            if (course.si_so_con_lai <= 0.1 * course.dkmh_tu_dien_lop_hoc_phan_si_so)
                setStatus(true, false, false);
            else
            // (10%,40%)
            if (course.si_so_con_lai > 0.1 * course.dkmh_tu_dien_lop_hoc_phan_si_so && course.si_so_con_lai < 0.4 * course.dkmh_tu_dien_lop_hoc_phan_si_so)
                setStatus(false, true, false);
            // [40%,100%]
            else setStatus(false, false, true);
        }

        public static ObservableCollection<CourseNode> UnExpandAllCourseNode(ObservableCollection<CourseNode> courseNodes)
        {
            foreach (var node in courseNodes)
            {
                node.IsExpanded = false;
            }
            return courseNodes;
        }

        public static ObservableCollection<CourseNode> Uncheck_UnExpandCourseNode(ObservableCollection<CourseNode> oldCourseNodes)
        {
            ObservableCollection<CourseNode> newCourseNodes = DeepCopy(oldCourseNodes);
            foreach (var node in newCourseNodes)
            {
                // parent node
                node.IsExpanded = false;
                node.IsSelected = false;

                if (node.SubNodes == null) continue;
                foreach (var childnode in node.SubNodes)
                {
                    foreach (CourseInformation course in childnode.CourseGroup)
                    {
                        course.IsSelected = false;
                    }
                }
            }
            return newCourseNodes;
        }

        private static ObservableCollection<CourseNode> DeepCopy(ObservableCollection<CourseNode> list)
        {
            try
            {
                var json = JsonConvert.SerializeObject(list);
                return JsonConvert.DeserializeObject<ObservableCollection<CourseNode>>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        private void setStatus(bool Red, bool Yellow, bool Green)
        {
            IsRedStatus = Red;
            IsYellowStatus = Yellow;
            IsGreenStatus = Green;
        }

       
    }
}
