using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Dry.Models.CommonCls;
using AERC.Models.CommonCls;
using Dry.Models.ViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Dry.Models.Service;

namespace Dry.Models.ReportModules
{
    //private DryEntities DryDB = new DryEntities();
    
    public class DesignReceiptReport
    {
        public byte[] GetReport_DesignReceiptReport(short unit, int year, int IANumStart, int IANumEnd/*, string sourcepath*/)
        {
            DryEntities DryDB = new DryEntities();
            List<DesignReceiptView> data = new List<DesignReceiptView>();
            GetData gd = new GetData();
            var datas = from pay in DryDB.Pay
                               join smv in DryDB.SummaryView on pay.MapNo equals smv.MapNo
                               where (smv.ApplyYear == year) && (smv.ApplyUnit == unit) && (smv.IANum >= IANumStart) && (smv.IANum <= IANumEnd) && (pay.ItemCode == 2)
                               select new { smv.ApplyYear , pay.PayMoney,smv.IANum, smv.ApplyUnit };
            foreach (var item in datas.OrderBy(m => m.IANum))
            {
                DesignReceiptView dv = new DesignReceiptView();
                dv.DYears = item.ApplyYear.ToString();
                dv.DesignPrice = item.PayMoney.ToString();
                dv.DNo = item.IANum.ToString();
                dv.ChineseMoney = new NumberToChinese().GetChineseNumber(item.PayMoney);
                dv.IaName = gd.GetUnitName((short)item.ApplyUnit);
                data.Add(dv);
                
            }
            
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in data)
                {
                    document.NewPage();
                    writeReceipt(document, item);
                }
                document.Close();
                return stream.GetBuffer();
            }

        }
        public byte[] GetDesignReceiptReportData(List<DesignReceiptView> Data, string sourcepath)
        {
            #region basic Data
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["設計費收據"];

            int count = Data.Count; 
            for (int i = 0; i < count; i++)
            {
                int number = (i * 24);
                if (number != 0)
                {
                    sheet.Select("A1:K24");
                    sheet.SelectedRange.Copy(sheet.Cells["A" + (number + 1).ToString() + ":K" + (number + 1).ToString()]);
                    //for (int j = 1; j <= 24; j++)
                    //{
                    //    sheet.Cells["A" + j.ToString() + ":K" + j.ToString()].Copy(sheet.Cells["A" + (number + j).ToString() + ":K" + (number + j).ToString()]);
                    //}
                }
            }
            //for (int i = 1; i <= count; i++)
            int itemindex = 1;
            foreach (var item in Data)
            {
                sheet = SetDesignReceiptData(sheet, item, itemindex);//DB
                itemindex += 1;
            }
            byte[] file = excel.GetAsByteArray();

            #endregion

            return file;
        }
        #region 設定加入設計費收據表
        public ExcelWorksheet SetDesignReceiptData(ExcelWorksheet sh, DesignReceiptView Data, int index)
        {
            int number = ((index - 1) * 24);

            sh.Row(number + 1).Height = 37;
            sh.Row(number + 2).Height = 33;
            sh.Row(number + 3).Height = 54;
            //sh.Row(number + 4).Height = 12;
            sh.Row(number + 5).Height = 50;
            sh.Row(number + 6).Height = 50;
            sh.Row(number + 7).Height = 50;
            //sh.Row(number + 8).Height = 12;
            //sh.Row(number + 9).Height = 12;
            sh.Row(number + 10).Height = 37;
            //sh.Row(number + 11).Height = 12;
            sh.Row(number + 12).Height = 24;
            sh.Row(number + 13).Height = 24;
            //sh.Row(number + 14).Height = 12;
            sh.Row(number + 15).Height = 32;
            sh.Row(number + 16).Height = 32;
            sh.Row(number + 17).Height = 32;
            sh.Row(number + 18).Height = 47;
            sh.Row(number + 19).Height = 47;
            sh.Row(number + 20).Height = 47;
            sh.Row(number + 21).Height = 47;
            sh.Row(number + 22).Height = 47;
            //sh.Row(number + 23).Height = 12;
            sh.Row(number + 24).Height = 34;

            string money = Data.DesignPrice;
            string chmoney = new NumberToChinese().GetChineseNumber(int.Parse(money.ToString().Trim()));
            string[] strs = new string[money.Length];
            for (int i = 0; i < money.Length; i++)
            {
                strs[i] = money.Substring(money.Length - 1 - i, 1);
                sh.Cells[number + 3, 9 - i].Value = strs[i].ToString();
            }
            #region
            #endregion
            #region
            sh.Cells[number + 12, 7].Value = Data.DYears;
            sh.Cells[number + 15, 2].Value = Data.ChineseMoney + "元整"; 
            sh.Cells[number + 15, 10].Style.Numberformat.Format = "#,##0";
            sh.Cells[number + 15, 10].Value = int.Parse(Data.DesignPrice) ;
            sh.Cells[number + 18, 2].Value = Data.DNo;
            sh.Cells[number + 24, 4].Value = Data.DYears;  
            #endregion
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion



        public void writeReceipt(Document doc, DesignReceiptView item)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("領  款  收  據", new iTextSharp.text.Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(string.Format("設施編號:{0}", item.DNo), new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 350 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(string.Format("新臺幣:{0}元整", item.ChineseMoney), new iTextSharp.text.Font(baseFT, 14)) { IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            string content = $"此係{item.DYears}年度管路灌溉設施補助申請案規劃設計費，上款如數領訖無訛。";
            doc.Add(new Paragraph(content, new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph("  此致", new iTextSharp.text.Font(baseFT, 16)) { IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(item.IaName, new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_CENTER, IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(string.Format("領款人(簽名或蓋章):"), new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            //doc.Add(new Paragraph("簽名或蓋章:", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            //doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(string.Format("身分證字號:"), new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(string.Format("住址:"), new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(string.Format("連絡電話:"), new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });

            doc.Add(Chunk.NEWLINE);
            doc.Add(Chunk.NEWLINE);
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph("中   華   民   國    年    月    日", new iTextSharp.text.Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER });
        }

    }
}
