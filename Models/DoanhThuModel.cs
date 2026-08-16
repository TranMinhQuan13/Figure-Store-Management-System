using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnQLFIGURE.Models
{
    public class DoanhThuModel
    {
        public int Thang { get; set; }
        public decimal TongDoanhThu { get; set; }
        public string ThangText => "Tháng " + Thang;
    }
}