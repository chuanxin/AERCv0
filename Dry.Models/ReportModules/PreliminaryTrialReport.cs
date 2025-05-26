using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dry.Models.ReportModules
{
    public class PreliminaryTrialReport
    {
        DryEntities DryDB = new DryEntities();

        public byte[] getReport(int unit, int year, int IANumStart, int IANumEnd)
        {
            string unitname = new CommonCls.GetData().GetUnitName((short)unit);
            var datas = from a in DryDB.CaseDetail
                        join b in DryDB.CasePayDetail on a.MapNo equals b.mapno
                        where a.ApplyUnit == unit && a.ApplyYear == year && a.IANum >= IANumStart && a.IANum <= IANumEnd && a.Complete == true
                        orderby a.IANum
                        select new
                        {
                            a.IANum,
                            a.Name,
                            a.Addr,
                            a.CatalogCNS,
                            b.田間管路設施費,
                            b.規劃設計費,
                            b.水源設施費,
                            b.調控設施費,
                            b.動力設備費,
                            b.蓄水設備費,
                            b.設施費總計,
                            b.工作費,
                            b.FarmerFee,
                            b.Total
                        };
            //if (datas.Count() == 0)
            //{
            //    return null;
            //}
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
                BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                document.NewPage();
                document.Add(new Paragraph("推廣單位初審意見表", new iTextSharp.text.Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 });
                document.Add(new Paragraph($"一、推廣單位：{unitname}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 10 });
                document.Add(new Paragraph($"二、受理期間(案件數量)：{datas.Count()}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 10 });
                document.Add(new Paragraph($"三、申請案件初審意見：", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 30 });
                PdfPTable table = new PdfPTable(new float[] { 16f, 28f, 24f, 45f, 47f, 27f });
                table.TotalWidth = 488f;
                table.LockedWidth = true;
                table.AddCell(new PdfPCell(new Phrase("序號\n(案號)", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER/*, FixedHeight = 26*/ });
                table.AddCell(new PdfPCell(new Phrase("申請人姓名", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("通訊地址", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("施設形式", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("初審意見", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("備註", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                int rowi = 2;
                int recindex = 1;
                foreach (var item in datas)
                {

                    table.AddCell(new PdfPCell(new Phrase($"{recindex}\n({item.IANum})", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 104 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.Name}", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase($"{item.Addr}", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT });
                    string sysline = "1. 田間管路灌溉系統\n";
                    switch (item.CatalogCNS)
                    {
                        case "穿孔管":
                            sysline += "    ■穿孔管  □噴頭\n    □微噴    □滴灌\n";
                            break;
                        case "噴頭":
                            sysline += "    □穿孔管  ■噴頭\n    □微噴    □滴灌\n";
                            break;
                        case "微噴":
                            sysline += "    □穿孔管  □噴頭\n    ■微噴    □滴灌\n";
                            break;
                        case "滴灌":
                            sysline += "    □穿孔管  □噴頭\n    □微噴    ■滴灌\n";
                            break;
                        default:
                            sysline += "    □穿孔管  □噴頭\n    □微噴   □滴灌\n";
                            break;
                    }
                    sysline += "2. 灌溉調控設施\n";
                    if (item.動力設備費 > 0)
                    {
                        sysline += "    ■動力設備\n";
                    }
                    else sysline += "    □動力設備\n";
                    if (item.蓄水設備費 > 0)
                    {
                        sysline += "    ■調蓄設施\n";
                    }
                    else sysline += "    □調蓄設施\n";
                    if (item.調控設施費 > 0)
                    {
                        sysline += "    ■調節控制設施\n";
                    }
                    else sysline += "    □調節控制設施\n";
                    table.AddCell(new PdfPCell(new Phrase($"{sysline}", new iTextSharp.text.Font(baseFT, 10))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT });
                    string feeline = $"■合格\n  補助金額{((item.Total ?? 0) - (item.FarmerFee ?? 0)).ToString("N0")}元\n\n□不合格\n  原因____________";
                    table.AddCell(new PdfPCell(new Phrase($"{feeline}", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT });
                    table.AddCell(new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT });
                    rowi++;
                    recindex++;
                    if (rowi == 7)
                    {
                        document.Add(table);
                        document.NewPage();
                        table = new PdfPTable(new float[] { 16f, 28f, 24f, 45f, 47f, 27f });
                        table.TotalWidth = 488f;
                        table.LockedWidth = true;
                        rowi = 0;
                    }
                }
                document.Add(table);
                document.Close();
                return stream.GetBuffer();

            }
        }

        public byte[] getReportByIds(int[] ids, short unit)
        {
            string unitname = new CommonCls.GetData().GetUnitName(unit);
            var arrayIds = new List<int>();
            for (int i = 0; i < ids.Count(); i++)
            {
                arrayIds.Add(ids[i]);
                setCasePDateById(ids[i]);
            }

            var datas = from a in DryDB.CaseDetail
                        join b in DryDB.CasePayDetail on a.MapNo equals b.mapno
                        where arrayIds.Contains((int)a.EventNo)
                        orderby a.IANum
                        select new
                        {
                            a.MapNo,
                            a.IANum,
                            a.Name,
                            a.Addr,
                            a.CatalogCNS,
                            b.田間管路設施費,
                            b.規劃設計費,
                            b.水源設施費,
                            b.調控設施費,
                            b.動力設備費,
                            b.蓄水設備費,
                            b.設施費總計,
                            b.工作費,
                            b.FarmerFee,
                            b.Total,
                            a.IdNo
                        };
            //var datab = from a in datas
            //            group a by new { a.IdNo } into m
            //            select new { Idno = m.Key.IdNo, Reccount = m.Count() };
            //var datac = from a in datas
            //            join b in datab on a.IdNo equals b.Idno
            //            select new {
            //                a.MapNo,
            //                a.IANum,
            //                a.Name,
            //                a.Addr,
            //                a.CatalogCNS,
            //                a.田間管路設施費,
            //                a.規劃設計費,
            //                a.水源設施費,
            //                a.調控設施費,
            //                a.動力設備費,
            //                a.蓄水設備費,
            //                a.設施費總計,
            //                a.工作費,
            //                a.FarmerFee,
            //                a.Total,
            //                a.IdNo,
            //                b.Reccount
            //            };
            //var datas = from a in datas0
            //            where arrayIds.Contains((int)a.MapNo)
            //if (datas.Count() == 0)
            //{
            //    return null;
            //}
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
                BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                document.NewPage();
                document.Add(new Paragraph("推廣單位初審意見表", new iTextSharp.text.Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 });
                document.Add(new Paragraph($"一、推廣單位：{unitname}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 10 });
                document.Add(new Paragraph($"二、受理期間(案件數量)：{datas.Count()}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 10 });
                document.Add(new Paragraph($"三、申請案件初審意見：", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 30 });
                PdfPTable table = new PdfPTable(new float[] { 18f, 20f, 30f, 46f, 46f, 27f });
                table.TotalWidth = 488f;
                table.LockedWidth = true;
                table.AddCell(new PdfPCell(new Phrase("序號\n(案號)", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER/*, FixedHeight = 26*/ });
                table.AddCell(new PdfPCell(new Phrase("申請人姓名", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("通訊地址", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("施設形式", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("初審意見", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("備註", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                int rowi = 2;
                int recindex = 1;
                foreach (var item in datas /*datac*/)
                {

                    table.AddCell(new PdfPCell(new Phrase($"{recindex}\n({item.IANum})", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 104 });
                    //too slow to create this line
                    //if (datas.Count(m => m.IdNo == item.IdNo) > 1 /*item.Reccount > 1*/)
                    //{
                    //    table.AddCell(new PdfPCell(new Phrase($"*{item.Name}", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                    //}
                    //else
                    //{
                    //    table.AddCell(new PdfPCell(new Phrase($"{item.Name}", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });
                    //}

                    table.AddCell(new PdfPCell(new Phrase($"{item.Name}", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER });

                    table.AddCell(new PdfPCell(new Phrase($"{item.Addr}", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT });
                    string sysline = "1. 田間管路灌溉系統\n";
                    switch (item.CatalogCNS)
                    {
                        case "穿孔管":
                            sysline += "    ■穿孔管  □噴頭\n    □微噴    □滴灌\n";
                            break;
                        case "噴頭":
                            sysline += "    □穿孔管  ■噴頭\n    □微噴    □滴灌\n";
                            break;
                        case "微噴":
                            sysline += "    □穿孔管  □噴頭\n    ■微噴    □滴灌\n";
                            break;
                        case "滴灌":
                            sysline += "    □穿孔管  □噴頭\n    □微噴    ■滴灌\n";
                            break;
                        default:
                            sysline += "    □穿孔管  □噴頭\n    □微噴   □滴灌\n";
                            break;
                    }
                    sysline += "2. 灌溉調控設施\n";
                    if (item.動力設備費 > 0)
                    {
                        sysline += "    ■動力設備\n";
                    }
                    else sysline += "    □動力設備\n";
                    if (item.蓄水設備費 > 0)
                    {
                        sysline += "    ■調蓄設施\n";
                    }
                    else sysline += "    □調蓄設施\n";
                    if (item.調控設施費 > 0)
                    {
                        sysline += "    ■調節控制設施\n";
                    }
                    else sysline += "    □調節控制設施\n";
                    table.AddCell(new PdfPCell(new Phrase($"{sysline}", new iTextSharp.text.Font(baseFT, 11))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT });
                    string feeline = $"■合格\n  補助金額{((item.Total ?? 0) - (item.FarmerFee ?? 0)).ToString("N0")}元\n\n□不合格\n  原因____________";
                    table.AddCell(new PdfPCell(new Phrase($"{feeline}", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT });
                    table.AddCell(new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_LEFT });
                    rowi++;
                    recindex++;
                    if (rowi == 7)
                    {
                        document.Add(table);
                        document.NewPage();
                        table = new PdfPTable(new float[] { 18f, 20f, 30f, 46f, 46f, 27f });
                        table.TotalWidth = 488f;
                        table.LockedWidth = true;
                        rowi = 0;
                    }
                }
                document.Add(table);
                document.Close();
                return stream.GetBuffer();

            }
        }

        public byte[] getReportXlsByIds(int[] ids, short unit)
        {
            string unitname = new CommonCls.GetData().GetUnitName(unit);
            string title = $"{unitname}書面審查表人名清冊";
            var arrayIds = new List<int>();
            for (int i = 0; i < ids.Count(); i++)
            {
                arrayIds.Add(ids[i]);
            }
            #region GetData

            
            var datas = from a in DryDB.CaseDetail
                        join b in DryDB.CasePayDetail on a.MapNo equals b.mapno
                        where arrayIds.Contains((int)a.EventNo)
                        orderby a.IANum
                        select new
                        {
                            a.MapNo,
                            a.IANum,
                            a.Name,
                            a.Addr,
                            a.CatalogCNS,
                            b.田間管路設施費,
                            b.規劃設計費,
                            b.水源設施費,
                            b.調控設施費,
                            b.動力設備費,
                            b.蓄水設備費,
                            b.設施費總計,
                            b.工作費,
                            b.FarmerFee,
                            b.Total,
                            a.IdNo
                        };
            //var datab = from a in datas
            //            group a by new { a.IdNo } into m
            //            select new { Idno = m.Key.IdNo, Reccount = m.Count() };
            //var datac = from a in datas
            //            join b in datab on a.IdNo equals b.Idno
            //            select new
            //            {
            //                a.MapNo,
            //                a.IANum,
            //                a.Name,
            //                a.Addr,
            //                a.CatalogCNS,
            //                a.田間管路設施費,
            //                a.規劃設計費,
            //                a.水源設施費,
            //                a.調控設施費,
            //                a.動力設備費,
            //                a.蓄水設備費,
            //                a.設施費總計,
            //                a.工作費,
            //                a.FarmerFee,
            //                a.Total,
            //                a.IdNo,
            //                b.Reccount
            //            };
            #endregion
            string sourcepath = @"~\ReportSample\PTReport.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["書面審查清冊"];

            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 3;
            int recno = 1;
            foreach (var item in datas /*datac*/)
            {
                sheet.Cells[irow, 1].Value = recno;
                sheet.Cells[irow, 2].Value = item.IANum;
                
                sheet.Cells[irow, 3].Value = item.Name;
                sheet.Cells[irow, 4].Value = item.Addr;
                recno++;
                irow++;
            }
            return excel.GetAsByteArray();
        }

        private void setCasePDateById(int eventno)
        {
            Case casedata = DryDB.Case.Find(eventno);
            DryDB.Case.Attach(casedata);
            casedata.PDate = DateTime.Now;
            DryDB.SaveChanges();

        }
    }
}
