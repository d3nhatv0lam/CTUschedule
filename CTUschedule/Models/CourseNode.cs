using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.Models
{
    public class CourseNode
    {

        public string MaHocPhan { get; }
        public string TenHocPhan { get; }

        public bool IsExpanded { get; set; } = false;

        public ObservableCollection<CourseNode>? SubNodes { get; } = new ObservableCollection<CourseNode>();

        public CourseInformation? Course { get; set; } = new CourseInformation()
        {
            si_so_con_lai = null,
        };

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
        }
    }
}
