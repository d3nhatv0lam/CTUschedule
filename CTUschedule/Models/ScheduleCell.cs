using OpenQA.Selenium.BiDi.Modules.Script;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.Models
{
    public class ScheduleCell
    {
        public string MaHocPhan { get; set; }
        public string TenHocPhan { get; set; }

        public string NhomHocPhan { get; set; }
        public string PhongHoc { get; set; }
        public string GiangVien { get; set; }
        public int SoTietHoc  { get; set; }

        public bool IsRedStatus { get; set; } = false;
        public bool IsYellowStatus { get; set; } = false;
        public bool IsGreenStatus { get; set; } = false;

        public ScheduleCell()
        {

        }

        public ScheduleCell(CourseInformation course)
        {
            MaHocPhan = course.dkmh_tu_dien_hoc_phan_ma;
            TenHocPhan = course.dkmh_tu_dien_hoc_phan_ten_vn;
            NhomHocPhan = course.dkmh_nhom_hoc_phan_ma;
            PhongHoc = course.dkmh_tu_dien_phong_hoc_ten;
            GiangVien = course.dkmh_tu_dien_giang_vien_ten_vn;
            SoTietHoc = course.tiet_hoc.Trim('-').Length;
            SetSlotStatus(course);
        }

        private void setStatus(bool Red, bool Yellow, bool Green)
        {
            IsRedStatus = Red;
            IsYellowStatus = Yellow;
            IsGreenStatus = Green;
        }

        private void SetSlotStatus(CourseInformation Course)
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
    public class ScheduleCellTableIndex
    {
        public int ThuDiHoc { get; set; } // cot
        public int TietBatDau { get; set; } // hang
        public int SoTietHoc { get; set; }

        public static ScheduleCellTableIndex GetTableIndex(CourseInformation course)
        {
            ScheduleCellTableIndex newScheduleItem = new ScheduleCellTableIndex();
            if (course.dkmh_thu_trong_tuan_ma != null) 
            newScheduleItem.ThuDiHoc = (int)course.dkmh_thu_trong_tuan_ma;

            string tiethoc = course.tiet_hoc.Trim('-');
            newScheduleItem.TietBatDau = tiethoc[0] - '0';
            newScheduleItem.SoTietHoc = tiethoc.Length;

            return newScheduleItem;
        }
    }
}
