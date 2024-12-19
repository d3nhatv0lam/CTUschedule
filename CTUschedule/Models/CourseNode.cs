using Newtonsoft.Json;
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
        public bool IsExpanded { get; set; } = false;

        public bool IsScheduleSelected { get; set; } = false;
        public bool IsRedStatus { get; set; } = false;
        public bool IsYellowStatus { get; set; } = false;
        public bool IsGreenStatus { get; set; } = false;

        public ObservableCollection<CourseNode>? SubNodes { get; set; } = new ObservableCollection<CourseNode>();

        public CourseInformation? Course { get; set; } = new CourseInformation()
        {
            si_so_con_lai = null,
        };

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

        public CourseNode(CourseInformation? course)
        {
            Course = course;
            SetSlotStatus();
        }

        public static ObservableCollection<CourseNode> Uncheck_UnExpandCourseNode(ObservableCollection<CourseNode> oldCourseNodes)
        {
            ObservableCollection<CourseNode> newCourseNodes = DeepCopy(oldCourseNodes);
            foreach (var node in newCourseNodes)
            {
                node.IsExpanded = false;
                node.Course.IsSelected = false;
                if (node.SubNodes == null) continue;
                foreach (var childnode in node.SubNodes)
                {
                    childnode.Course.IsSelected = false;
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

        private void SetSlotStatus()
        {
            if (Course == null || Course.si_so_con_lai == null) return;
            if (Course.si_so_con_lai == 0) setStatus(false, false, false);
            else
            // (0%,10%]
            if (Course.si_so_con_lai <= 0.1 * Course.dkmh_tu_dien_lop_hoc_phan_si_so)
                setStatus(true, false, false);
            else
            // (10%,40%)
            if (Course.si_so_con_lai > 0.1 * Course.dkmh_tu_dien_lop_hoc_phan_si_so && Course.si_so_con_lai < 0.4 * Course.dkmh_tu_dien_lop_hoc_phan_si_so)
                setStatus(false, true, false);
            // [40%,100%]
            else setStatus(false, false, true);
        }
    }
}
