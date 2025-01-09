using CTUschedule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.Utilities
{
    public static class CourseNodeListFindIndex
    {
        public static int FindIndex(CourseNode CourseList,string nhomHocPhan)
        {
            int index = -1;
            int intNhomHocPhan = Int32.Parse(nhomHocPhan);
            int l = 0, r = CourseList.SubNodes.Count - 1;

            while (l <= r) 
            {
                int mid = l + (r - l) / 2;
                int CourseintNhomHocPhan = Int32.Parse(CourseList.SubNodes[mid].CourseGroup.First().dkmh_nhom_hoc_phan_ma);
                if (CourseintNhomHocPhan >= intNhomHocPhan)
                {
                    index = mid;
                    r = mid - 1;
                }
                else l = mid + 1;
            }

            return index;
        }
        
    }
}
