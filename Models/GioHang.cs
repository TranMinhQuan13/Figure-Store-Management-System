using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnQLFIGURE.Models
{
    public class GioHang
    {
        public int MaFigure { get; set; }
        public string TenFigure { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public decimal ThanhTien { get; set; }
        public string Anh { get; set; }

        private QL_FigureEntities6 db = new QL_FigureEntities6();
        public GioHang()
        {
           
        }
        public GioHang(int mafigure, int soluong = 1)
        {
            MaFigure = mafigure;
            FIGURE fg = db.FIGUREs.FirstOrDefault(s => s.MaFigure == mafigure);
            TenFigure = fg.TenFigure;
            GiaBan = fg.GiaBan ?? (decimal)0;
            SoLuong = soluong;
            ThanhTien = SoLuong * GiaBan;
            Anh = fg.AnhBia;
        }
    }
}