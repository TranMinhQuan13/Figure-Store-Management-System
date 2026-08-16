using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnQLFIGURE.Models
{
    public class BannerQC:Banner
    {
        public bool IsBanner { get; set; } = false;           // Có dùng làm banner không?
        public int ThuTuBanner { get; set; } = 0;             // Thứ tự hiển thị (1,2,3...)
        public string TieuDeBanner { get; set; } = string.Empty;  // Tiêu đề overlay (nếu muốn)
        public string MoTaBanner { get; set; } = string.Empty;    // Mô tả ngắn
        public string LinkBanner { get; set; } = string.Empty;
    }
}