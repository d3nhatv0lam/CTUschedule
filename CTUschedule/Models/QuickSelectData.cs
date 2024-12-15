using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTUschedule.Models
{
    public class QuickSelectData
    {
        public int code { get; set; }
        public string msg { get; set; }
        public QuickselectDataList data { get; set; }
    }

    public class QuickselectDataList
    {
        public List<QuickselectInformation> dkmh_tu_dien_hoc_phan_ma_auto_complete { get; set; }
    }

    public class QuickselectInformation
    {
        public string value { get; set; }
        public string label  { get; set; }   
    }
}
