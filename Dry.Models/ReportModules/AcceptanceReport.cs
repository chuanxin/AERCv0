using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dry.Models.Service;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Dry.Models.ReportModules
{
    public class AcceptanceReport
    {
        /// <summary>
        /// 驗收報告書批次產製
        /// </summary>
        /// <param name="unitcode"></param>
        /// <param name="years"></param>
        /// <param name="IANumStart"></param>
        /// <param name="IANumEnd"></param>
        /// <param name="sourcepath"></param>
        /// <returns></returns>
        public byte[] GetReport_AcceptanceReport(short unitcode, int years, int IANumStart, int IANumEnd/*, string sourcepath*/)
        {
            List<AcceptanceReportDBService.DataStruct> data = new List<AcceptanceReportDBService.DataStruct>();
            data = new AcceptanceReportDBService().GetReportData(unitcode,years,IANumStart,IANumEnd);            
            byte[] resdata = GetAcceptanceReportPDF(data);
            return resdata;
        }
        public byte[] GetAcceptanceReport(List<AcceptanceReportDBService.DataStruct> Data, string sourcepath)
        {
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["驗收報告"];

            int count = Data.Count;

            for (int i = 0; i < count; i++)
            {
                int startRow = i * 40 + 1;
                if (i == 0) { startRow = 41; }
                int endRow = startRow + 40 - 1;
                if (count != 1)
                {
                    sheet.Cells[1, 1, 40, 11].Copy(sheet.Cells[startRow, 1, endRow, 11]);
                }
                    
                for (int row = 1; row <= 40; row++)
                {
                    //sheet.Cells["A" + row + ":K" + row].Copy(sheet.Cells["A" + (i * 40 + row) + ":K" + (i * 40 + row)]);
                    sheet.Row(i * 40 + row).Height = sheet.Row(row).Height;

                }
                //sheet.Cells[i * 18, 2, i * 18  + 4, 4].Merge = true;
                sheet = SetAcceptanceReportData(sheet, Data[i], i+1 );//DB

            }

            byte[] file = excel.GetAsByteArray();


            return file;
        }

        public byte[] GetAcceptanceReportPDF(List<AcceptanceReportDBService.DataStruct> Data)
        {
            byte[] file;
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data)
                {
                    document.NewPage();
                    writeAcceptReportBody(document, item);
                }
                document.Close();
                file = stream.GetBuffer();
            }
            return file;
        }
        /// <summary>
        /// 驗收報告書
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="Data"></param>
        public static void writeAcceptReportBodyV0(Document doc, AcceptanceReportDBService.DataStruct Data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("推廣管路灌溉設施補助之系統驗收報告書", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"一、申請人：{Data.FarmerName}                       申請案號：{Data.IANum}",
                new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"二、設施地點：{Data.SectionName}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"三、申請面積：{Data.Area}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"四、設施型式：{Data.FacType}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("五、驗收日期：     年     月     日", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("六、驗收情形：", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_CENTER });
            PdfPTable table = new PdfPTable(new float[] { 35, 118 });
            table.TotalWidth = 450f;
            table.LockedWidth = true;
            int colheight = 18;
            //row1
            table.AddCell(new PdfPCell(new Phrase("驗收項目", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight});
            table.AddCell(new PdfPCell(new Phrase("驗收情形", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER});
            //row2
            table.AddCell(new PdfPCell(new Phrase("1.核定補助面積", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase("____________m2(平方公尺)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
            //row3
            table.AddCell(new PdfPCell(new Phrase("2.田區輸水主管", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT,VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = (colheight * 2) });
            table.AddCell(new PdfPCell(new Phrase("主管1：管長_____________公尺、管徑_____________吋\n主管2：管長_____________公尺、管徑_____________吋",
                new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED });
            //row4
            table.AddCell(new PdfPCell(new Phrase("3.灌溉器類型", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase("□相符, □不符", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
            //row5
            table.AddCell(new PdfPCell(new Phrase("4.噴頭間距(SS)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase("_____________公尺", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
            //row6
            table.AddCell(new PdfPCell(new Phrase("5.支管間距(SL)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase("_____________公尺", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
            //row7
            table.AddCell(new PdfPCell(new Phrase("6.運轉狀況", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase("□良好, □不良", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
            //row8
            table.AddCell(new PdfPCell(new Phrase("7.其他補助設施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT,VerticalAlignment = Element.ALIGN_MIDDLE,Rowspan=3 });
            table.AddCell(new PdfPCell(new Phrase("名稱：調節控制設施(參考材料數量表)    □相符, □不符", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase("名稱：調蓄設施      材質：____________數量：_____座______噸", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase("名稱：動力設備      型式：____________數量：_____台______HP", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });

            doc.Add(table);
            doc.Add(new Paragraph("七、驗收結果：", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            PdfPTable table1 = new PdfPTable(new float[] { 19, 134 });
            table1.TotalWidth = 450f;
            table1.LockedWidth = true;
            //row1
            table1.AddCell(new PdfPCell(new Phrase("第一次\n驗收", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Rowspan = 3,FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("1.□驗收合格，依核定補助款發放          元", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("2.□驗收合格，依核定補助款減列金額，發放          元(請說明原因)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("3.□驗收不合格，限期改善再行驗收(請註明  年  月  日完成改善)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            //row2
            table1.AddCell(new PdfPCell(new Phrase("改善\n再驗", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Rowspan = 3, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("1.□驗收合格，依核定補助款發放          元", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("2.□驗收合格，依核定補助款減列金額，發放          元(請說明原因)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("3.□驗收不合格，取消補助資格", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("備註", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan=2, FixedHeight = 100 });
            doc.Add(table1);

            PdfPTable table2 = new PdfPTable(new float[] { 23f, 34f, 23f, 34f, 23f, 34f });
            table2.TotalWidth = 517f;
            table2.LockedWidth = true;
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 9)) { Alignment = Element.ALIGN_LEFT });
                        
            //R1
            table2.AddCell(new PdfPCell(new Phrase("監驗", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("股長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE});
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("主任工程師", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //R2
            table2.AddCell(new PdfPCell(new Phrase("驗收", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("組長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("總幹事", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //R3
            table2.AddCell(new PdfPCell(new Phrase("主辦", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("主計單位", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("處長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });

            doc.Add(table2);
        }
        /// <summary>
        /// 功能測試或現地勘查(驗收報告書新版)
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="Data"></param>
        public static void writeAcceptReportBody(Document doc, AcceptanceReportDBService.DataStruct Data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("推廣管路灌溉設施補助功能測試報告書", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"一、申請人：{Data.FarmerName}                       申請案號：{Data.IANum}",
                new iTextSharp.text.Font(baseFT, 12))
            { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"二、設施地點：{Data.SectionName}(詳如土地清冊)。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"三、申請面積：{Data.Area}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"四、設施型式：{Data.FacType}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("五、測試日期：     年     月     日", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("六、功能測試：", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_CENTER });
            PdfPTable table = new PdfPTable(new float[] { 35, 118 });
            table.TotalWidth = 480f;
            table.LockedWidth = true;
            int colheight = 18;
            //row0 title
            table.AddCell(new PdfPCell(new Phrase("項目", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase("檢 查 情 形", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //row1
            table.AddCell(new PdfPCell(new Phrase("1.設施型式", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase("與設計圖說    □相符,   □不符", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
            //row2
            table.AddCell(new PdfPCell(new Phrase("2.設施規格", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE ,FixedHeight = colheight * 8});
            string line1 = string.Empty;
            line1 = "(1)田間主管:\n" +
                    "   主管管徑L1____________吋、L2____________吋\n" +
                    "   支管行距(SL)__________m，噴頭間距(SS)__________m\n" +
                    "   豎管高(H)_____________m\n\n" +
                    "(2)調蓄設施: □鋁合金___座___噸;□不鏽鋼___座___噸;\n"+
                    "             □塑膠___座___噸\n\n" +
                    "(3)動力設備: □馬達___台;□汽油引擎___台;□柴油引擎___台;\n" +
                    "             □柱塞式泵浦___台\n\n" +
                    "(4)調節控制設施: 與規劃型式  □相符,  □不符\n";
            table.AddCell(new PdfPCell(new Phrase(line1, new iTextSharp.text.Font(baseFT, 12))){ HorizontalAlignment = Element.ALIGN_LEFT });
            //row4
            table.AddCell(new PdfPCell(new Phrase("3.功能測試", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase("經現場運轉功能正常    □相符, □不符", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
                      

            doc.Add(table);
            doc.Add(new Paragraph("七、辦理結果：", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            PdfPTable table1 = new PdfPTable(new float[] { 19, 134 });
            table1.TotalWidth = 480f;
            table1.LockedWidth = true;
            //row1
            table1.AddCell(new PdfPCell(new Phrase("\n測試", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_TOP, Rowspan = 3, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("1.□合格，依核定補助款發放__________元", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("2.□合格，依核定補助款減列金額，發放__________元(請說明原因)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("3.□不合格，限期改善再行驗收(請註明  年  月  日完成改善)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            //row2
            table1.AddCell(new PdfPCell(new Phrase("\n複查", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_TOP, Rowspan = 3, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("1.□合格，依核定補助款發放__________元", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("2.□合格，依核定補助款減列金額，發放__________元(請說明原因)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("3.□不合格，取消補助資格", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = colheight });
            table1.AddCell(new PdfPCell(new Phrase("備註:", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2, FixedHeight = 100 });
            doc.Add(table1);

            PdfPTable table2 = new PdfPTable(new float[] { 23f, 34f, 23f, 34f, 23f, 34f });
            table2.TotalWidth = 517f;
            table2.LockedWidth = true;
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 9)) { Alignment = Element.ALIGN_LEFT });

            //R1
            table2.AddCell(new PdfPCell(new Phrase("測試人員", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("股長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("主任工程師", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //R2
            table2.AddCell(new PdfPCell(new Phrase("會辦", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("組長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("副處長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //table2.AddCell(new PdfPCell(new Phrase("副首長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //R3
            table2.AddCell(new PdfPCell(new Phrase("主辦", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("主計單位", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table2.AddCell(new PdfPCell(new Phrase("處長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //table2.AddCell(new PdfPCell(new Phrase("首長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table2.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });

            doc.Add(table2);
        }

        public ExcelWorksheet SetAcceptanceReportData(ExcelWorksheet sh, AcceptanceReportDBService.DataStruct Data, int count)
        {
            int number = ((count - 1) * 40);
            sh.Cells[number + 3, 5].Value = Data.FarmerName;

            sh.Cells[number + 4, 5].Value = Data.SectionName;
            sh.Cells[number + 3, 9].Value = Data.IANum;
            sh.Cells[number + 5, 5].Value = Data.Area;
            sh.Cells[number + 6, 5].Value = Data.FacType;
            sh.Cells[number + 7, 5].Value = Data.AcceptanceDate_Year + "年";
            sh.Cells[number + 7, 6].Value = Data.AcceptanceDate_Month + "月";
            sh.Cells[number + 7, 7].Value = Data.AcceptanceDate_Day + "日";
            //sh.Cells[number + 11, 5].Value = Data.Area + "公頃";
            //sh.Cells[number + 12, 5].Value = " 管長(L1)：   " + Data.L1 + "   公尺      管徑(D1)：   " + Data.D1 + "   吋";
            //sh.Cells[number + 13, 5].Value = " 管長(L2)：   " + Data.L2 + "   公尺      管徑(D2)：   " + Data.D2 + "   吋";
            //sh.Cells[number + 15, 5].Value = "   " + Data.SS + "     公尺";
            //sh.Cells[number + 16, 5].Value = "   " + Data.SL + "     公尺";
            //sh.Cells[number + 19, 5].Value = "名稱：調節控制設施   規格：             數量：       全";
            sh.Cells[number + 25, 7].Value = Data.AcceptanceResults;
            return sh;
        }
 
    }
    
}
