using Avalonia.Media;
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

        public static List<Brush> Brushes = new List<Brush>
        {
            new SolidColorBrush(Color.Parse("#ffd1dc")), // hong phan
            new SolidColorBrush(Color.Parse("#add8e6")), // xanh duong nhat
            new SolidColorBrush(Color.Parse("#FFFacd")), // vang nhat chanh
            new SolidColorBrush(Color.Parse("#e29b9a")),
            new SolidColorBrush(Color.Parse("#f5f5dc")),
            new SolidColorBrush(Color.Parse("#7d66ba")),
            new SolidColorBrush(Color.Parse("#d5fad6")), // xanh la bac ha
            new SolidColorBrush(Color.Parse("#B2f2e9")),
            new SolidColorBrush(Color.Parse("#84aae5")),
            new SolidColorBrush(Color.Parse("#8ad485")),
            new SolidColorBrush(Color.Parse("#007ba7")), // xanh da troi
            new SolidColorBrush(Color.Parse("#d1fff4")), // pastel cyan
        };

        public string MaHocPhan { get; set; }
        public string TenHocPhan { get; set; }
        public string NhomHocPhan { get; set; }
        public string PhongHoc { get; set; }
        public string GiangVien { get; set; }
        public Brush CellBursh { get; set; }

        public bool IsShowCell { get; set; } = false;

        public int SoTietHoc { get; set; } = 1; // row span default is 1
        public int TietBatDau { get; set; } // hang
        public int ThuDiHoc { get; set; } // cot


        public int RowIndex
        {
            get
            {
                if (TietBatDau >= 6) return TietBatDau + 1;
                return TietBatDau;
            }
        }

        public int ColumnIndex
        {
            get => ThuDiHoc - 1;
        }

        // [TietBatDau,RowRange]
        public int TietKetThuc
        {
            get => TietBatDau + SoTietHoc - 1;
        }
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

            GetScheduleIndex(course);
            SetSlotStatus(course);
        }

        public ScheduleCell(CourseInformation course,Brush color)
        {
            MaHocPhan = course.dkmh_tu_dien_hoc_phan_ma;
            TenHocPhan = course.dkmh_tu_dien_hoc_phan_ten_vn;
            NhomHocPhan = course.dkmh_nhom_hoc_phan_ma;
            PhongHoc = course.dkmh_tu_dien_phong_hoc_ten;
            GiangVien = course.dkmh_tu_dien_giang_vien_ten_vn;
            CellBursh = color;
            GetScheduleIndex(course);
            SetSlotStatus(course);
        }

        private void GetScheduleIndex(CourseInformation course)
        {
            if (course.dkmh_thu_trong_tuan_ma != null)
            ThuDiHoc = (int)course.dkmh_thu_trong_tuan_ma;

            string tiethoc = course.tiet_hoc.Trim('-');
            TietBatDau = tiethoc[0] - '0';
            SoTietHoc = tiethoc.Length;
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
}
