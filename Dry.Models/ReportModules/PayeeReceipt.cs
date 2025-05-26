using CusNPOI;
using Dry.Models.CommonCls;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dry.Models.Service;
using OfficeOpenXml;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Dry.Models.ReportModules
{
    public class PayeeReceipt
    {
        DryEntities DryDB = new DryEntities();
        public byte[] GetReport_PayeeReceipt(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
            
            List<PayeeReceiptDBService.DataStruct> Data = new List<PayeeReceiptDBService.DataStruct>();
            Data = new PayeeReceiptDBService().GetPayeeReceiptData(unit, year, IANumStart, IANumEnd);
            byte[] returnData = PayeeReceiptReport(Data, sourcepath);
            //byte[] returnData = PayeeReceiptReportPDF(Data);
            //byte[] returnData = GetDesignReceiptReportData12(Data, sourcepath);
            return returnData;
        }
        public byte[] GetReport_PayeeReceiptPDF(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
            List<PayeeReceiptDBService.DataStruct> Data = new List<PayeeReceiptDBService.DataStruct>();
            Data = new PayeeReceiptDBService().GetPayeeReceiptData(unit, year, IANumStart, IANumEnd);
            
            byte[] returnData = PayeeReceiptReportPDF(Data);
            
            return returnData;
        }
        public byte[] GetReport_Batch_PayeeReceiptPDF(short unit, int year, int IANumStart, int IANumEnd)
        {
            //List<PayeeReceiptDBService.DataStruct> Data = new List<PayeeReceiptDBService.DataStruct>();
            //Data = new PayeeReceiptDBService().GetPayeeReceiptData(unit, year, IANumStart, IANumEnd);
            BudgetBookReport BBR = new BudgetBookReport();
            var Data = DryDB.SummaryView.Where(m => m.ApplyUnit == unit && m.ApplyYear == year && m.IANum >= IANumStart && m.IANum <= IANumEnd)
                .Select(m => new { m.MapNo, m.IANum }).OrderBy(o => o.IANum).DefaultIfEmpty();
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data.OrderBy(o => o.IANum))
                {
                    document.NewPage();
                    PayeeReceiptDBService.DataStruct receiptdata = new PayeeReceiptDBService().GetPayeeReceiptDataByMapno((int)item.MapNo);
                    BBR.writeReceipt(document, receiptdata);
                }
                document.Close();
                return stream.GetBuffer();
            }
            
            //return returnData;
        }

        private byte[] PayeeReceiptReportPDF(List<PayeeReceiptDBService.DataStruct> dt)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in dt)
                {
                    document.NewPage();
                    
                    document.Add(new Paragraph("領  款  收  據", new Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER });
                    document.Add(new Paragraph(string.Format("設施編號:{0}",item.IANum), new Font(baseFT, 14)) {Alignment = Element.ALIGN_LEFT, IndentationLeft = 350 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph(string.Format("新台幣:{0}", item.SubsidyFeeCNS), new Font(baseFT, 14)) { IndentationLeft = 50 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph("此係推廣省水管路灌溉設施補助計畫設施補助款，上款如數領訖無訛。", new Font(baseFT, 14)) {Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
                    document.Add(Chunk.NEWLINE);
                    //document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph("  此致", new Font(baseFT, 16)) { IndentationLeft = 50 });
                    //document.Add(Chunk.NEWLINE);
                    //document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph(item.IAName, new Font(baseFT, 14)) { Alignment = Element.ALIGN_CENTER, IndentationLeft = 50 });
                    document.Add(Chunk.NEWLINE);
                    //document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph(string.Format("領款人:{0}",item.FarmerName), new Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph("簽 章:", new Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph(string.Format("住 址:{0}",item.Addr), new Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph(string.Format("身分證字號:{0}",item.FarmerID), new Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph("中   華   民   國    年    月    日", new Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER });
                 }
                document.Close();
                return stream.GetBuffer();
            }
        }
        private byte[] PayeeReceiptReport(List<PayeeReceiptDBService.DataStruct> dt, string sourcepath)
        {
            CusCopyRow cus = new CusCopyRow();
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook wk = new HSSFWorkbook(fs);
            int count = dt.Count;
            int reportlength = 39;

            HSSFSheet wksheet = wk.GetSheetAt(0) as HSSFSheet;
            
            for (int j = 1; j < count; j++)
            {
                for (int row = 0; row < 30; row++)
                {
                    
                    cus.CopyRow(wksheet, row, reportlength * j + row);
                    

                }
            }

            SetData(wk, wksheet, dt, reportlength);
            MemoryStream files = new MemoryStream();
            wk.Write(files);
            files.Close();
            return files.ToArray();
        }

        public byte[] GetDesignReceiptReportData12(List<PayeeReceiptDBService.DataStruct> Data, string sourcepath)
        {
            #region basic Data
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["收據"];

            int count = Data.Count; 
            for (int i = 0; i < count; i++)
            {
                int number = (i * 45);
                if (number != 0)
                {
                    sheet.Select("A1:I45");
                    sheet.SelectedRange.Copy(sheet.Cells["A" + (number + 1).ToString() + ":I" + (number + 1).ToString()]);
                    //for (int j = 1; j <= 24; j++)
                    //{
                    //    sheet.Cells["A" + j.ToString() + ":K" + j.ToString()].Copy(sheet.Cells["A" + (number + j).ToString() + ":K" + (number + j).ToString()]);
                    //}
                }
            }
            //for (int i = 1; i <= count; i++)
            int itemindex = 0;
            foreach (var item in Data)
            {
                sheet = SetDesignReceiptData12(sheet, item, itemindex);//DB
                itemindex += 1;
            }
            byte[] file = excel.GetAsByteArray();

            #endregion

            return file;
        }
        public ExcelWorksheet SetDesignReceiptData12(ExcelWorksheet sh, PayeeReceiptDBService.DataStruct Data, int index)
        {
            int number = ((index ) * 45);

            sh.Cells[number + 3, 7].Value = Data.IANum;
            sh.Cells[number + 5, 3].Value = Data.SubsidyFeeCNS;
            sh.Cells[number + 7, 2].Value = Data.Content;
            sh.Cells[number + 10, 3].Value = Data.IAName; 
            sh.Cells[number + 14, 3].Value = Data.FarmerName; 
            sh.Cells[number + 18, 3].Value = Data.Addr; 
            sh.Cells[number + 20, 4].Value = Data.FarmerID; 
            
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }

        private HSSFSheet SetData(HSSFWorkbook wk, HSSFSheet sh, List<PayeeReceiptDBService.DataStruct> dt, int reportlength)
        {
            HSSFCellStyle cs = wk.CreateCellStyle() as HSSFCellStyle;
            HSSFFont font1 = wk.CreateFont() as HSSFFont;
            int count = dt.Count;
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                
                sh.GetRow(2 + j).GetCell(7).SetCellValue(dt[i].IANum);
                sh.GetRow(4 + j).GetCell(2).SetCellValue(dt[i].SubsidyFeeCNS);
                sh.GetRow(6 + j).GetCell(1).SetCellValue(dt[i].Content);
                sh.GetRow(9 + j).GetCell(2).SetCellValue(dt[i].IAName);
                sh.GetRow(13 + j).GetCell(2).SetCellValue(dt[i].FarmerName);
                sh.GetRow(17 + j).GetCell(2).SetCellValue(dt[i].Addr);
                sh.GetRow(19 + j).GetCell(3).SetCellValue(dt[i].FarmerID);

                j = j + reportlength;
            }

            return sh;
        }
        /*public class DataStruct
        {
            public int ApplyYear { get; set; }
            public string Content { get; set; }
            public string IAName { get; set; }
            public int IANum { get; set; }
            public string FarmerName { get; set; }
            public string FarmerID { get; set; }
            public string Addr { get; set; }
            public string SubsidyFeeCNS { get; set; }

        }*/
    }
}
