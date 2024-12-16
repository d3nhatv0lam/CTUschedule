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

        public ObservableCollection<CourseNode>? SubNodes { get; }

        public CourseInformation? Course { get; }

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
