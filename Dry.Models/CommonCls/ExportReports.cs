using DocumentFormat.OpenXml.Packaging;
using Dry.Models.ReportModules;
using Dry.Models.Service;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.CommonCls
{
    public class ExportReports
    {
        /// <summary>
        /// 匯出Word檔(套用樣版)
        /// </summary>
        /// <param name="Data">資料內容</param>
        /// <param name="TemplateUrl">樣版路徑</param>
        /// <returns></returns>
        public byte[] ExportWord(Dictionary<string, string> Data, string TemplateUrl)
        {
            string xmlString = null;
            
            byte[] byteArray = System.IO.File.ReadAllBytes(TemplateUrl);
            byte[] resultArray;
            using (MemoryStream mem = new MemoryStream())
            {
                mem.Write(byteArray, 0, (int)byteArray.Length);
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(mem, true))
                {
                    using (StreamReader sr = new StreamReader(wordDoc.MainDocumentPart.GetStream()))
                    {
                        xmlString = sr.ReadToEnd();
                    }
                    foreach (string key in Data.Keys)
                    {
                        xmlString = xmlString.Replace("$" + key + "$", Data[key]);
                    }
                    using (StreamWriter sw = new StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create)))
                    {
                        sw.Write(xmlString);
                    }
                    resultArray = mem.ToArray();
                }
            }
            return resultArray;
        }
        /// <summary>
        /// 匯出PDF檔(套用樣版)
        /// </summary>
        /// <param name="Data">資料內容</param>
        /// <param name="TemplateUrl">樣版路徑</param>
        /// <returns></returns>
        public byte[] ExportPDF(Dictionary<string,string> Data,string TemplateUrl)
        {
            var reader = new MemoryStream();
            PdfReader pdfreader = new PdfReader(TemplateUrl);
            PdfStamper pdfStamper = new PdfStamper(pdfreader, reader);

            AcroFields pdfFormFields = pdfStamper.AcroFields;
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            foreach (var key in Data)
            {
                pdfFormFields.SetFieldProperty(key.Key, "textfont", baseFT, null);
                bool tt = pdfFormFields.SetField(key.Key, key.Value);
            }

            pdfStamper.FormFlattening = true;            
            pdfStamper.Close();
            pdfreader.Close();

            byte[] result = reader.ToArray();
            reader.Close();
            return result;
        }
        /// <summary>
        /// 規劃委託書
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public byte[] ProxyPDF(Dictionary<string, string> Data)
        {
            
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                document.NewPage();
                writeProxy(document, Data);
                document.Close();
                return stream.GetBuffer();
            }

        }

        public static void writeProxy(Document doc, Dictionary<string, string> Data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("推廣管路灌溉設施規劃委託書", new iTextSharp.text.Font(baseFT, 22)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_LEFT });
            string line = $"本人申請農業部農田水利署{Data["ApplyYear"]}年度推廣管路灌溉設施補助，擬委託____________________君（管理處）有關人員代辦系統規劃佈置，恐口說無憑，特立此委託書。";
            doc.Add(new Paragraph(line, new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 60, IndentationLeft = 20 });
            //doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_LEFT});
            doc.Add(new Paragraph($"申請案號：{Data["IANum"]}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT,IndentationLeft = 20 });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT});
            doc.Add(new Paragraph($"委託人(簽名或蓋章)：{Data["FarmerName"]}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"身分證字號：{Data["FarmerIDNo"]}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"通訊地址：{Data["FarmerAddr"]}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"聯絡電話：{Data["FarmerPhone"]}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 30)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"受託人(簽名或蓋章)：", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"身分證字號：", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"通訊地址：", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"聯絡電話：", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("*研習字號或證照編號：", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"中  華  民  國        年        月        日", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER});


        }
        /// <summary>
        /// 切結書
        /// </summary>
        /// <param name="mapno"></param>
        /// <returns></returns>
        public byte[] AcceptAffidavitPDF(int mapno)
        {
            //Dictionary<string, string> dataStr = new Service.BudgetBookDBService().MergeExportAcceptAffidavitData(mapno);
            ViewModel.BudgetBookView dataStr = new Service.BudgetBookDBService().GetBudgetBookData(mapno);
            
            //var ctrlobjs = new GetData().GetCntrlMatData(mapno);
            string ctrlLine = string.Empty;            
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                document.NewPage();
                writeAcceptAffidavit(document, dataStr, mapno);
                document.Close();
                return stream.GetBuffer();
            }
        }
        /// <summary>
        /// 切結書批次輸出
        /// </summary>
        /// <param name="ApplyYear"></param>
        /// <param name="ApplyUnit"></param>
        /// <param name="IANumStart"></param>
        /// <param name="IANumEnd"></param>
        /// <returns></returns>
        public byte[] AcceptAffidavitPDFS(int ApplyYear, int ApplyUnit, int IANumStart, int IANumEnd)
        {
            DryEntities drydb = new DryEntities();
            var datalist = from db in drydb.SummaryView
                           where db.ApplyYear == ApplyYear && db.ApplyUnit == ApplyUnit && db.IANum >= IANumStart && db.IANum <= IANumEnd
                           orderby db.IANum
                           select new { db.MapNo };
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in datalist)
                {
                    ViewModel.BudgetBookView dataStr = new Service.BudgetBookDBService().GetBudgetBookData((int)item.MapNo);
                    document.NewPage();
                    writeAcceptAffidavit(document, dataStr, (int)item.MapNo);
                }
                document.Close();
                return stream.GetBuffer();
            }

        }
        /// <summary>
        /// 切結書舊版V0
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        /// <param name="mapno"></param>
        public static void writeAcceptAffidavitV0(Document doc, ViewModel.BudgetBookView data, int mapno)
        {
            string IaName = new CommonCls.GetData().GetUnitName(data.ApplyUnit);
            DryEntities drydb = new DryEntities();
            var fid = drydb.SummaryView.Where(m => m.MapNo == mapno).DefaultIfEmpty().Select(m => new { m.FId }).FirstOrDefault();
            var farmer = drydb.Farmer.Find(fid.FId);
            string phone = $"{farmer.Tel} {farmer.Phone}";
            string idno = $"{farmer.IdNo}";
            
            string motor = "0";
            string gasoline = "0";
            string disel = "0";
            string pistone = "0";
            
            if (data.Engine != null)
            {
                var objs = data.Engine.Memo.Split('#');
                foreach (var item in objs)
                {

                    if (item.Split('*')[0].Contains("馬達"))
                    {
                        motor = item.Split('*')[1];
                    }
                    if (item.Split('*')[0].Contains("汽油"))
                    {
                        gasoline = item.Split('*')[1];
                    }
                    if (item.Split('*')[0].Contains("柴油"))
                    {
                        disel = item.Split('*')[1];
                    }
                    if (item.Split('*')[0].Contains("柱塞"))
                    {
                        pistone = item.Split('*')[1];
                    }
                }
            }
            
            var ctrlobjs = new GetData().GetCntrlMatData(mapno);
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            
            doc.Add(new Paragraph("申請推廣管路灌溉設施補助切結書", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 12)));
            string line = $"具切結書人於{data.Farm.FirstOrDefault().full_sectName}{data.Farm.FirstOrDefault().LandNo}地號等{data.Farm.Count}筆申請農業部{data.ApplyY}年度推廣管路灌溉設施補助，除遵守貴單位有關規定辦理，願具切結書如下：";
            doc.Add(new Paragraph(line, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT,FirstLineIndent = 20, SpacingAfter = 10 });
            doc.Add(new Paragraph("一、設施內容", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 10 });
            
            PdfPTable table = new PdfPTable(new float[] { 13f, 15f, 15f, 15f, 15f, 13f, 20f, 20f, 20f, 17f });
            table.TotalWidth = 488f;
            table.LockedWidth = true;
            int colheight = 18;
            table.AddCell(new PdfPCell(new Phrase("設施\n項目", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_TOP,HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            table.AddCell(new PdfPCell(new Phrase("動力設備", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight,Colspan = 4});
            Phrase pool = new Phrase();
            pool.Add(new Chunk("調蓄\n設施\n", new iTextSharp.text.Font(baseFT, 12)));
            pool.Add(new Chunk("(蓄水槽)", new iTextSharp.text.Font(baseFT, 6)));
            //table.AddCell(new PdfPCell(new Phrase("調蓄\n設施", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3});
            table.AddCell(new PdfPCell(pool) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3});
            table.AddCell(new PdfPCell(new Phrase("穿孔管\n系統", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            table.AddCell(new PdfPCell(new Phrase("噴頭\n系統", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            table.AddCell(new PdfPCell(new Phrase("微噴\n系統", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            table.AddCell(new PdfPCell(new Phrase("滴灌\n系統", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            table.AddCell(new PdfPCell(new Phrase("馬達", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight * 2, Rowspan = 2 });
            table.AddCell(new PdfPCell(new Phrase("汽油\n引擎", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 2 });
            table.AddCell(new PdfPCell(new Phrase("柴油\n引擎", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 2 });
            table.AddCell(new PdfPCell(new Phrase("柱塞\n式泵", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 2 });
            table.AddCell(new PdfPCell(new Phrase("數量", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight});
            table.AddCell(new PdfPCell(new Phrase($"{motor}台", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight});
            table.AddCell(new PdfPCell(new Phrase($"{gasoline}台", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight});
            table.AddCell(new PdfPCell(new Phrase($"{disel}台", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight});
            table.AddCell(new PdfPCell(new Phrase($"{pistone}台", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight});
            table.AddCell(new PdfPCell(new Phrase($"{data.PoolWei.ToString()}噸", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight});
            int hasize = 10;
            if (data.EndType.Contains("穿孔管"))
            {
                table.AddCell(new PdfPCell(new Phrase($"{(data.BuildArea / 10000).ToString("N4")}公頃", new iTextSharp.text.Font(baseFT, hasize))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            }else
            {
                table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            }
            if (data.EndType.Contains("噴頭"))
            {
                table.AddCell(new PdfPCell(new Phrase($"{(data.BuildArea / 10000).ToString("N4")}公頃", new iTextSharp.text.Font(baseFT, hasize))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            }
            if (data.EndType.Contains("微噴"))
            {
                table.AddCell(new PdfPCell(new Phrase($"{(data.BuildArea / 10000).ToString("N4")}公頃", new iTextSharp.text.Font(baseFT, hasize))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            }
            if (data.EndType.Contains("滴灌"))
            {
                table.AddCell(new PdfPCell(new Phrase($"{(data.BuildArea/10000).ToString("N4")}公頃", new iTextSharp.text.Font(baseFT, hasize))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            }
            //table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight});
            //table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight});
            //table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight});
            table.CompleteRow();
            doc.Add(table);
            
            PdfPTable table1 = new PdfPTable(new float[] { 18f, 29f, 29f, 29f, 29f, 29f});
            table1.TotalWidth = 488f;
            table1.LockedWidth = true;
            table1.AddCell(new PdfPCell(new Phrase($"調節控制設施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Colspan = 6});
            
            table1.AddCell(new PdfPCell(new Phrase($"項目", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight});
            int rec = 1;
            if (ctrlobjs != null)
            {
                
                foreach (var item in ctrlobjs)
                {
                    table1.AddCell(new PdfPCell(new Phrase($"{item.MatName}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight });
                    rec++;
                }
                
                
                for (int i = 0; i < 5 - (ctrlobjs.Count % 5); i++)
                {
                    table1.AddCell(new PdfPCell(new Phrase($"", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight });
                }
                
                table1.AddCell(new PdfPCell(new Phrase($"數量", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight });
                rec = 1;
                foreach (var item in ctrlobjs)
                {
                    table1.AddCell(new PdfPCell(new Phrase($"{item.MatAmt}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight });
                    rec++;
                }
                for (int i = 0; i < 5 - (ctrlobjs.Count % 5); i++)
                {
                    table1.AddCell(new PdfPCell(new Phrase($"", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight });
                }

            }
            else
            {
                
                for (int i = 0; i < 5; i++)
                {
                    table1.AddCell(new PdfPCell(new Phrase($"", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight });
                }
                table1.AddCell(new PdfPCell(new Phrase($"數量", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight });
                
                for (int i = 0; i < 5; i++)
                {
                    table1.AddCell(new PdfPCell(new Phrase($"", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight });
                }

            }
            table1.CompleteRow();
            doc.Add(table1);
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });

            
            doc.Add(new Paragraph("二、補助金額依補助基準計算，並依排定優先順序辦理，如無法列入補助對象時，絕無異議。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24});
            doc.Add(new Paragraph("三、本設施願於____年____月____日前完成並報驗。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24});
            doc.Add(new Paragraph("四、具切結書人如有下列情形之一者，願放棄經費補助，並放棄追訴權：", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24});
            doc.Add(new Paragraph("1.經驗收不合格，且未依指定改善日期辦理完成。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 12});
            doc.Add(new Paragraph("2.經發現向水保局、農糧署或其他機關申請補助者（符合再次申請補助之條件者，不在此限）。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 12});
            doc.Add(new Paragraph("五、所檢附任何相關證明文件、施設前後照片以及單據憑證等資料，若有不實、造假、欺騙、違反要點規定等情事，願自負一切法律責任，並繳回全部補助款，不再接受相關補助。",
                new iTextSharp.text.Font(baseFT, 12,1)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24});
            doc.Add(new Paragraph("六、本設施完成後，自當善加養護，並將經營運轉情形成果資料提供貴單位，供作研究發展計畫之參考。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24});
            doc.Add(new Paragraph("七、若為配合農時而需提前施設，願遵照規定程序辦理，所申請補助金額俟計畫核定後再撥款，倘因計畫變更或調整經費支用項目無法補助時，一切設施費用願意自行承擔，絕無異議。",
                new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24,SpacingAfter = 30 });
            doc.Add(new Paragraph("此致", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 48,SpacingAfter = 10 });
            doc.Add(new Paragraph($"{IaName}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_CENTER,SpacingAfter = 10 });
            doc.Add(new Paragraph($"具切結書人：{data.Name}                 簽名或蓋章", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 48, SpacingAfter = 10 });
            doc.Add(new Paragraph($"身分證字號：{idno}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 48, SpacingAfter = 10 });
            doc.Add(new Paragraph($"通訊地址：{data.Address}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 48, SpacingAfter = 10 });
            doc.Add(new Paragraph($"聯絡電話：{phone}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 48, SpacingAfter = 10 });
            doc.Add(new Paragraph("中華民國      年      月      日", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_CENTER });
            
        }
        /// <summary>
        /// 切結書
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        /// <param name="mapno"></param>
        public static void writeAcceptAffidavit(Document doc, ViewModel.BudgetBookView data, int mapno)
        {
            string IaName = new CommonCls.GetData().GetUnitName(data.ApplyUnit);
            DryEntities drydb = new DryEntities();
            var fid = drydb.SummaryView.Where(m => m.MapNo == mapno).DefaultIfEmpty().Select(m => new { m.FId }).FirstOrDefault();
            var farmer = drydb.Farmer.Find(fid.FId);
            string phone = $"{farmer.Tel} {farmer.Phone}";
            string idno = $"{farmer.IdNo}";
            
            string motor = "0";
            string gasoline = "0";
            string disel = "0";
            string pistone = "0";
            if (data.Engine != null)
            {
                var objs = data.Engine.Memo.Split('#');
                foreach (var item in objs)
                {

                    if (item.Split('*')[0].Contains("馬達"))
                    {
                        motor = item.Split('*')[1];
                    }
                    if (item.Split('*')[0].Contains("汽油"))
                    {
                        gasoline = item.Split('*')[1];
                    }
                    if (item.Split('*')[0].Contains("柴油"))
                    {
                        disel = item.Split('*')[1];
                    }
                    if (item.Split('*')[0].Contains("柱塞"))
                    {
                        pistone = item.Split('*')[1];
                    }
                }
            }
            
            var ctrlobjs = new GetData().GetCntrlMatData(mapno);

            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            
            doc.Add(new Paragraph("推廣管路灌溉設施補助切結書", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 12)));
            string line = $"具切結書人(同申請人)於{data.Farm.FirstOrDefault().full_sectName}{data.Farm.FirstOrDefault().LandNo}地號合計{data.Farm.Count}筆(詳如申請書所列土地)，申請農業部農田水利署{data.ApplyY}年度推廣管路灌溉設施補助，除遵守貴單位有關規定辦理，同意具切結書如下：";
            doc.Add(new Paragraph(line, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 10 });
            doc.Add(new Paragraph("一、補助金額依補助基準計算，並依排定優先順序辦理，如無法列入補助對象時，絕無異議。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });                        
            doc.Add(new Paragraph("二、本設施同意於____年____月____日前完成結案申報。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("三、具切結書人如有下列情形之一者，同意放棄經費補助，並放棄追訴權：", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("1.經系統測試不合格，且未依指定改善日期辦理完成。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 12 });
            doc.Add(new Paragraph("2.經發現曾接受農業部農村發展及水土保持署補助調蓄設施者（符合再次申請補助之條件者，不在此限）。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 12 });
            doc.Add(new Paragraph("四、所灌溉土地非以休閒農場或露營區之方式經營者。", new iTextSharp.text.Font(baseFT, 12, Font.BOLD | Font.UNDERLINE))
            { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("五、所檢附任何相關證明文件、施設前後照片以及單據憑證等資料，若有偽造、變造、隱匿或虛偽等情事，同意自負一切法律責任，並繳回全部補助款。",
                new iTextSharp.text.Font(baseFT, 12, Font.BOLD | Font.UNDERLINE))
            { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("六、本設施完成後，同意將設施運轉成果資料提供貴單位，供作研究發展計畫之參考。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("七、若為配合農時而需提前施設，同意遵照規定程序辦理，所申請補助金額俟計畫核定後再撥款，倘因計畫變更或調整經費支用項目無法補助時，一切設施費用同意自行承擔，絕無異議。",
                new iTextSharp.text.Font(baseFT, 12, Font.BOLD | Font.UNDERLINE))
            { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24, SpacingAfter = 40 });
            doc.Add(new Paragraph("此致", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 24, SpacingAfter = 40 });
            doc.Add(new Paragraph($"{IaName}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 40 });
            doc.Add(new Paragraph($"具切結書人(簽名或蓋章)：{data.Name}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 24, SpacingAfter = 10 });
            doc.Add(new Paragraph($"身分證字號：{idno}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 24, SpacingAfter = 10 });
            doc.Add(new Paragraph($"通訊地址：{data.Address}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 24, SpacingAfter = 10 });
            doc.Add(new Paragraph($"聯絡電話：{phone}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 24, SpacingAfter = 120 });
            doc.Add(new Paragraph("中華民國      年      月      日", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_JUSTIFIED_ALL });

        }
        public static void writeAcceptAffidavitV1(Document doc, ViewModel.BudgetBookView data, int mapno)
        {
            string IaName = new CommonCls.GetData().GetUnitName(data.ApplyUnit);
            DryEntities drydb = new DryEntities();
            var fid = drydb.SummaryView.Where(m => m.MapNo == mapno).DefaultIfEmpty().Select(m => new { m.FId }).FirstOrDefault();
            var farmer = drydb.Farmer.Find(fid.FId);
            string phone = $"{farmer.Tel} {farmer.Phone}";
            string idno = $"{farmer.IdNo}";
            
            string motor = "0";
            string gasoline = "0";
            string disel = "0";
            string pistone = "0";
            if (data.Engine != null)
            {
                var objs = data.Engine.Memo.Split('#');
                foreach (var item in objs)
                {

                    if (item.Split('*')[0].Contains("馬達"))
                    {
                        motor = item.Split('*')[1];
                    }
                    if (item.Split('*')[0].Contains("汽油"))
                    {
                        gasoline = item.Split('*')[1];
                    }
                    if (item.Split('*')[0].Contains("柴油"))
                    {
                        disel = item.Split('*')[1];
                    }
                    if (item.Split('*')[0].Contains("柱塞"))
                    {
                        pistone = item.Split('*')[1];
                    }
                }
            }
            
            var ctrlobjs = new GetData().GetCntrlMatData(mapno);

            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            doc.Add(new Paragraph("申請推廣管路灌溉設施補助切結書", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 12)));
            string line = $"具切結書人於{data.Farm.FirstOrDefault().full_sectName}{data.Farm.FirstOrDefault().LandNo}地號合計{data.Farm.Count}筆，申請農業部農田水利署{data.ApplyY}年度推廣管路灌溉設施補助，除遵守貴單位有關規定辦理，願具切結書如下：";
            doc.Add(new Paragraph(line, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 10 });
            doc.Add(new Paragraph("一、設施內容", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 10 });
            
            PdfPTable table = new PdfPTable(new float[] { 13f, 15f, 15f, 15f, 15f, 20f, 20f, 20f, 20f, 20f });
            table.TotalWidth = 488f;
            table.LockedWidth = true;
            int colheight = 18;
            table.AddCell(new PdfPCell(new Phrase("設施\n項目", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            table.AddCell(new PdfPCell(new Phrase("動力設備", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Colspan = 4 });
            Phrase pool = new Phrase();
            pool.Add(new Chunk("  \n調蓄\n設施\n", new iTextSharp.text.Font(baseFT, 12)));
            pool.Add(new Chunk("(蓄水槽)", new iTextSharp.text.Font(baseFT, 8)));
            //table.AddCell(new PdfPCell(new Phrase("調蓄\n設施", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3});
            table.AddCell(new PdfPCell(pool) { VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            table.AddCell(new PdfPCell(new Phrase("穿孔管\n系統", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            table.AddCell(new PdfPCell(new Phrase("噴頭\n系統", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            table.AddCell(new PdfPCell(new Phrase("微噴\n系統", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            table.AddCell(new PdfPCell(new Phrase("滴灌\n系統", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 3 });
            Phrase motorString = new Phrase();
            motorString.Add(new Chunk("馬達\n", new iTextSharp.text.Font(baseFT, 12)));
            motorString.Add(new Chunk("(含抽水機)", new iTextSharp.text.Font(baseFT, 7)));
            table.AddCell(new PdfPCell(new Phrase(motorString)) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight * 2, Rowspan = 2 });
            table.AddCell(new PdfPCell(new Phrase("汽油\n引擎", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 2 });
            table.AddCell(new PdfPCell(new Phrase("柴油\n引擎", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 2 });
            table.AddCell(new PdfPCell(new Phrase("柱塞\n式泵", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight, Rowspan = 2 });
            table.AddCell(new PdfPCell(new Phrase("數量", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight * 2.5f });
            table.AddCell(new PdfPCell(new Phrase($"台", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase($"台", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase($"台", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase($"台", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase($"材質\n噸\n座", new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            //int hasize = 10;
            table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });
            table.AddCell(new PdfPCell(new Phrase($"公頃", new iTextSharp.text.Font(baseFT, 12))) { VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = colheight });

            table.CompleteRow();
            doc.Add(table);
            
            PdfPTable table1 = new PdfPTable(new float[] { 18f, 29f, 29f, 29f, 29f, 29f });
            table1.TotalWidth = 488f;
            table1.LockedWidth = true;
            table1.AddCell(new PdfPCell(new Phrase($"調節控制設施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight * 1.2f, Colspan = 6 });
            
            
            table1.AddCell(new PdfPCell(new Phrase($"項目", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight * 1.2f });
            for (int i = 0; i < 5; i++)
            {
                table1.AddCell(new PdfPCell(new Phrase($"", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight * 1.2f });
            }
            table1.AddCell(new PdfPCell(new Phrase($"數量", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight * 1.2f });
            
            for (int i = 0; i < 5; i++)
            {
                table1.AddCell(new PdfPCell(new Phrase($"", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = colheight * 1.2f });
            }

            table1.CompleteRow();
            doc.Add(table1);
            doc.Add(new Paragraph(Environment.NewLine, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });

            
            doc.Add(new Paragraph("二、補助金額依補助基準計算，並依排定優先順序辦理，如無法列入補助對象時，絕無異議。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("三、本設施願於____年____月____日前完成結案申報。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("四、具切結書人如有下列情形之一者，願放棄經費補助，並放棄追訴權：", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("1.經系統測試不合格，且未依指定改善日期辦理完成。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 12 });
            doc.Add(new Paragraph("2.經發現曾接受農業部農村發展及水土保持署補助調蓄設施者（符合再次申請補助之條件者，不在此限）。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 12 });
            doc.Add(new Paragraph("五、所灌溉土地非以休閒農場或露營區之方式經營者。", new iTextSharp.text.Font(baseFT, 12, Font.BOLD | Font.UNDERLINE))
            { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("六、所檢附任何相關證明文件、施設前後照片以及單據憑證等資料，若有偽造、變造、隱匿或虛偽等情事，願自負一切法律責任，並繳回全部補助款，不再接受相關補助。",
                new iTextSharp.text.Font(baseFT, 12, Font.BOLD | Font.UNDERLINE))
            { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("七、本設施完成後，願將設施運轉成果資料提供貴單位，供作研究發展計畫之參考。", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24 });
            doc.Add(new Paragraph("八、若為配合農時而需提前施設，願遵照規定程序辦理，所申請補助金額俟計畫核定後再撥款，倘因計畫變更或調整經費支用項目無法補助時，一切設施費用願意自行承擔，絕無異議。",
                new iTextSharp.text.Font(baseFT, 12, Font.BOLD | Font.UNDERLINE))
            { Alignment = Element.ALIGN_LEFT, FirstLineIndent = -24, IndentationLeft = 24, SpacingAfter = 30 });
            doc.Add(new Paragraph("此致", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 24, SpacingAfter = 10 });
            doc.Add(new Paragraph($"{IaName}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 10 });
            doc.Add(new Paragraph($"具切結書人：{data.Name}                 簽名或蓋章", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 48, SpacingAfter = 10 });
            doc.Add(new Paragraph($"身分證字號：{idno}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 48, SpacingAfter = 10 });
            doc.Add(new Paragraph($"通訊地址：{data.Address}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 48, SpacingAfter = 10 });
            doc.Add(new Paragraph($"聯絡電話：{phone}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 48, SpacingAfter = 20 });
            doc.Add(new Paragraph("中華民國      年      月      日", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_CENTER });

        }

        /// <summary>
        /// 竣工報驗書
        /// </summary>
        /// <param name="mapno"></param>
        /// <returns></returns>
        public byte[] CompleteVerifyPDF(int mapno)
        {
            Dictionary<string, string> dataStr = new Service.BudgetBookDBService().MergeExportCompleteData(mapno);
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                document.NewPage();
                writeCompleteVerify(document, dataStr, mapno);
                document.Close();
                return stream.GetBuffer();
            }
        }
        /// <summary>
        /// 竣工報驗書內容第一版
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        public static void writeCompleteVerifyV1(Document doc, Dictionary<string, string> data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("竣工報驗書", new iTextSharp.text.Font(baseFT, 22)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 40 });
            string line = $"茲申請農業部{data["ApplyYear"]}年度推廣管路灌溉設施補助，設置地點：{data["City"]}（縣市）" +
                $"{data["Town"]}（鄉鎮市區）{data["Section"].PadRight(6)}段{data["SubSection"].PadRight(6)}小段{data["LandNo"].PadRight(9)}地號" +
                $"合計{data["LandAmount"]}筆，合計面積{data["LandArea"]}公頃，已於民國_____年_____月_____日全部竣工，請貴單位派員驗收。";
            doc.Add(new Paragraph(line, new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 38, IndentationRight = 38,SpacingAfter = 50 });
            doc.Add(new Paragraph("此致", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 64, SpacingAfter = 20 });
            doc.Add(new Paragraph($"{data["ApplyUnit"]}", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 40 });
            doc.Add(new Paragraph($"申請案號：{data["DocketNo"]}", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 64, SpacingAfter = 16 });
            doc.Add(new Paragraph($"申請人：{data["FarmerName"].PadRight(16)}（簽名或蓋章）", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 64, SpacingAfter = 16 });
            doc.Add(new Paragraph($"通訊地址：{data["FarmerAddr"]}", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 64, SpacingAfter = 16 });
            doc.Add(new Paragraph($"聯絡電話：{data["FarmerTel"]}", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 64, SpacingAfter = 200 });
            doc.Add(new Paragraph($"中華民國            年          月          日", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
            
        }
        /// <summary>
        /// 竣工報驗書內容第二版
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        public static void writeCompleteVerify(Document doc, Dictionary<string, string> data, int mapno)
        {
            GetData getDataCls = new GetData();
            DryEntities drydb = new DryEntities();
            var FSys = getDataCls.GetFarmerSystemData(mapno);
            var endType = getDataCls.GetEndTypeData(mapno);
            var englist = getDataCls.GetEngineData(mapno);
            //var FarmerPool = getDataCls.GetPoolData(mapno);
            var FarmerPool = from a in drydb.Pool
                             join b in drydb.PoolTypeList on a.PtypeCode equals b.PtypeCode
                             where a.MapNo == mapno
                             group a by new { b.PtypeCNS, a.PoolWeight } into g
                             select new { g.Key.PtypeCNS, g.Key.PoolWeight, counts = g.Count() };

            var MatData = getDataCls.GetCntrlMatData(mapno);
            double buildarea = double.Parse(data["LandArea"]) * 10000;
            string seccns = $"{data["Section"]}段";
            if (! string.IsNullOrWhiteSpace( data["SubSection"]))
            {
                seccns += $"{data["SubSection"]}小段";
            }

            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("結案申報書", new iTextSharp.text.Font(baseFT, 22)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 40 });
            string line = $"茲申請農業部農田水利署{data["ApplyYear"]}年度推廣管路灌溉設施補助，設置地點：" + data["City"].PadRight(4) +
                $"{data["Town"]}{seccns}{data["LandNo"].PadRight(9)}地號" +
                $"等{data["LandAmount"]}筆(詳如土地清冊)，合計面積{buildarea}m²，已於民國_____年_____月_____日全部完成，請貴單位辦理結案審查。";
            doc.Add(new Paragraph(line, new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 38, IndentationRight = 38, SpacingAfter = 32 });
            #region 表格
            PdfPTable table = new PdfPTable(new float[] { 25f, 95f, 45f });
            table.TotalWidth = 468f;
            table.LockedWidth = true;
            table.AddCell(new PdfPCell(new Phrase("申請項目", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table.AddCell(new PdfPCell(new Phrase("內容", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            var cell1 = new PdfPCell(new Phrase("結案自主審查\n(申請人自行填寫)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER };
            cell1.BorderWidth = 2;                        
            table.AddCell(cell1 );
            table.AddCell(new PdfPCell(new Phrase("1.灌溉系統", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 48 });
            

            string line1 = "□穿孔管系統  □噴頭系統\n□微噴系統    □滴灌系統\n■未申請";
            if (FSys != null)
            {
                if ( FSys.EndType == 1)
                {
                    line1 = "■穿孔管系統  □噴頭系統\n□微噴系統    □滴灌系統\n□未申請";
                }
                else if (FSys.EndType == 2 || FSys.EndType == 6)
                {
                    line1 = "□穿孔管系統  ■噴頭系統\n□微噴系統    □滴灌系統\n□未申請";
                }
                else if (FSys.EndType == 3 )
                {
                    line1 = "□穿孔管系統  □噴頭系統\n■微噴系統    □滴灌系統\n□未申請";
                }
                else if (FSys.EndType == 4 || FSys.EndType == 7 || FSys.EndType == 8)
                {
                    line1 = "□穿孔管系統  □噴頭系統\n□微噴系統    ■滴灌系統\n□未申請";
                }
            }
            table.AddCell(new PdfPCell(new Phrase(line1, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
            var cell2 = new PdfPCell(new Phrase("□相符      □不符", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE };
            cell2.BorderWidth = 2;
            table.AddCell(cell2);
            table.AddCell(new PdfPCell(new Phrase("2.動力設施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 48 });
            line1 = "□馬達__台   □汽油引擎__台 □柴油引擎___台\n□柱塞式泵浦___台\n■未申請";
            if (englist != null)
            {
                if (englist.Any(m => m.EngCode == 1))
                {
                    line1 = $"■馬達{englist.Count(m => m.EngCode == 1)}台   ";
                }
                else line1 = "□馬達__台   ";
                if (englist.Any(m => m.EngCode == 2))
                {
                    line1 += $"■柱塞式泵浦{englist.Count(m => m.EngCode == 2)}台\n";
                }
                else line1 += "□柱塞式泵浦___台\n";
                if (englist.Any(m => m.EngCode == 3))
                {
                    line1 += $"■汽油引擎{englist.Count(m => m.EngCode == 3)}台 ";
                }
                else line1 += "□汽油引擎__台 ";
                if (englist.Any(m => m.EngCode == 4))
                {
                    line1 += $"■柴油引擎{englist.Count(m => m.EngCode == 4)}台\n";
                }
                else line1 += "□柴油引擎___台\n";                
                
                line1 += "□未申請";
            }

            table.AddCell(new PdfPCell(new Phrase(line1, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(cell2);
            table.AddCell(new PdfPCell(new Phrase("3.調蓄設施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 48 });
            //line1 = "□RC____座____噸  □鋁合金____座____噸\n□不銹鋼____座____噸  □塑膠____噸____座\n■未申請";
            line1 = string.Empty;
            if (FarmerPool != null)
            {
                if (FarmerPool.Count() == 0)
                {
                    line1 = "■未申請";
                }
               
                int j = 1;
                foreach (var item in FarmerPool)
                {
                    if (j >= 3)
                    {
                        line1 += $"\n■{item.PtypeCNS}{item.PoolWeight}噸{item.counts}座  ";
                        j = 1;
                    }else
                    {
                        line1 += $"■{item.PtypeCNS}{item.PoolWeight}噸{item.counts}座  ";
                        j++;
                    }
                    
                }
            }else
            {
                line1 = "■未申請";
            }
            table.AddCell(new PdfPCell(new Phrase(line1, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(cell2);
            table.AddCell(new PdfPCell(new Phrase("4.調控設施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 48 });
            //line1 = "□自動化控制   □微氣象調節   □液肥注入\n□過濾器   □其他調節控制設施\n■未申請";
            line1 = string.Empty;
            if (MatData != null)
            {
                if (MatData.Count() == 0)
                {
                    line1 = "■未申請";
                }
                int j = 1;
                foreach (var item in MatData)
                {
                    if (j >= 3)
                    {
                        line1 += $"\n■{item.MatName}   ";
                        j = 1;
                    }else
                    {
                        line1 += $"■{item.MatName}   ";
                        j++;
                    }
                }
            }else
            {
                line1 += "■未申請";
            }

            table.AddCell(new PdfPCell(new Phrase(line1, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(cell2);
            doc.Add(table);

            #endregion

            doc.Add(new Paragraph("此致", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 64, SpacingAfter = 20 });
            doc.Add(new Paragraph($"{data["ApplyUnit"]}", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 });
            doc.Add(new Paragraph($"申請案號：{data["DocketNo"]}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 32, SpacingAfter = 16 });
            doc.Add(new Paragraph($"申請人（簽名或蓋章）：{data["FarmerName"]}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 32, SpacingAfter = 16 });
            doc.Add(new Paragraph($"通訊地址：{data["FarmerAddr"]}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 32, SpacingAfter = 16 });
            doc.Add(new Paragraph($"聯絡電話：{data["FarmerTel"]}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 32, SpacingAfter = 65 });
            doc.Add(new Paragraph($"中華民國            年          月          日", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });

        }
        public static void writeCompleteVerifyV0(Document doc, Dictionary<string, string> data, int mapno)
        {
            GetData getDataCls = new GetData();
            DryEntities drydb = new DryEntities();
            var FSys = getDataCls.GetFarmerSystemData(mapno);
            var endType = getDataCls.GetEndTypeData(mapno);
            var englist = getDataCls.GetEngineData(mapno);
            //var FarmerPool = getDataCls.GetPoolData(mapno);
            var FarmerPool = from a in drydb.Pool
                             join b in drydb.PoolTypeList on a.PtypeCode equals b.PtypeCode
                             where a.MapNo == mapno
                             group a by new { b.PtypeCNS, a.PoolWeight } into g
                             select new { g.Key.PtypeCNS, g.Key.PoolWeight, counts = g.Count() };

            var MatData = getDataCls.GetCntrlMatData(mapno);

            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("結案申報書", new iTextSharp.text.Font(baseFT, 22)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 40 });
            string line = $"茲申請農業部農田水利署{data["ApplyYear"]}年度推廣管路灌溉設施補助，設置地點：" + data["City"].PadRight(4) +
                $"{data["Town"]}{data["Section"].PadRight(6).PadLeft(10)}段{data["SubSection"].PadRight(6).PadLeft(10)}小段{data["LandNo"].PadRight(9)}地號" +
                $"等{data["LandAmount"]}筆，合計面積{data["LandArea"]}公頃，已於民國_____年_____月_____日全部完成，請貴單位辦理結案審查。";
            doc.Add(new Paragraph(line, new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 38, IndentationRight = 38, SpacingAfter = 32 });
            #region 表格
            PdfPTable table = new PdfPTable(new float[] { 25f, 95f, 45f });
            table.TotalWidth = 468f;
            table.LockedWidth = true;
            table.AddCell(new PdfPCell(new Phrase("申請項目", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table.AddCell(new PdfPCell(new Phrase("內容", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            var cell1 = new PdfPCell(new Phrase("結案自主審查\n(申請人自行填寫)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER };
            cell1.BorderWidth = 2;
            table.AddCell(cell1);
            table.AddCell(new PdfPCell(new Phrase("1.灌溉系統", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 48 });


            string line1 = "□穿孔管系統  □噴頭系統\n□微噴系統    □滴灌系統\n■未申請";
            if (FSys != null)
            {
                if (FSys.EndType == 1)
                {
                    line1 = "■穿孔管系統  □噴頭系統\n□微噴系統    □滴灌系統\n□未申請";
                }
                else if (FSys.EndType == 2 || FSys.EndType == 6)
                {
                    line1 = "□穿孔管系統  ■噴頭系統\n□微噴系統    □滴灌系統\n□未申請";
                }
                else if (FSys.EndType == 3)
                {
                    line1 = "□穿孔管系統  □噴頭系統\n■微噴系統    □滴灌系統\n□未申請";
                }
                else if (FSys.EndType == 4 || FSys.EndType == 7 || FSys.EndType == 8)
                {
                    line1 = "□穿孔管系統  □噴頭系統\n□微噴系統    ■滴灌系統\n□未申請";
                }
            }
            table.AddCell(new PdfPCell(new Phrase(line1, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
            var cell2 = new PdfPCell(new Phrase("□相符      □不符", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE };
            cell2.BorderWidth = 2;
            table.AddCell(cell2);
            table.AddCell(new PdfPCell(new Phrase("2.動力設施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 48 });
            line1 = "□馬達__台   □汽油引擎__台 □柴油引擎___台\n□柱塞式泵浦___台\n■未申請";
            if (englist != null)
            {
                if (englist.Any(m => m.EngCode == 1))
                {
                    line1 = $"■馬達{englist.Count(m => m.EngCode == 1)}台   ";
                }
                else line1 = "□馬達__台   ";
                if (englist.Any(m => m.EngCode == 2))
                {
                    line1 += $"■柱塞式泵浦{englist.Count(m => m.EngCode == 2)}台\n";
                }
                else line1 += "□柱塞式泵浦___台\n";
                if (englist.Any(m => m.EngCode == 3))
                {
                    line1 += $"■汽油引擎{englist.Count(m => m.EngCode == 3)}台 ";
                }
                else line1 += "□汽油引擎__台 ";
                if (englist.Any(m => m.EngCode == 4))
                {
                    line1 += $"■柴油引擎{englist.Count(m => m.EngCode == 4)}台\n";
                }
                else line1 += "□柴油引擎___台\n";

                line1 += "□未申請";
            }

            table.AddCell(new PdfPCell(new Phrase(line1, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(cell2);
            table.AddCell(new PdfPCell(new Phrase("3.調蓄設施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 48 });
            //line1 = "□RC____座____噸  □鋁合金____座____噸\n□不銹鋼____座____噸  □塑膠____噸____座\n■未申請";
            line1 = string.Empty;
            if (FarmerPool != null)
            {
                if (FarmerPool.Count() == 0)
                {
                    line1 = "■未申請";
                }

                int j = 1;
                foreach (var item in FarmerPool)
                {
                    if (j >= 3)
                    {
                        line1 += $"\n■{item.PtypeCNS}{item.PoolWeight}噸{item.counts}座  ";
                        j = 1;
                    }
                    else
                    {
                        line1 += $"■{item.PtypeCNS}{item.PoolWeight}噸{item.counts}座  ";
                        j++;
                    }

                }
            }
            else
            {
                line1 = "■未申請";
            }
            table.AddCell(new PdfPCell(new Phrase(line1, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(cell2);
            table.AddCell(new PdfPCell(new Phrase("4.調控設施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 48 });
            //line1 = "□自動化控制   □微氣象調節   □液肥注入\n□過濾器   □其他調節控制設施\n■未申請";
            line1 = string.Empty;
            if (MatData != null)
            {
                if (MatData.Count() == 0)
                {
                    line1 = "■未申請";
                }
                int j = 1;
                foreach (var item in MatData)
                {
                    if (j >= 3)
                    {
                        line1 += $"\n■{item.MatName}   ";
                        j = 1;
                    }
                    else
                    {
                        line1 += $"■{item.MatName}   ";
                        j++;
                    }
                }
            }
            else
            {
                line1 += "■未申請";
            }

            table.AddCell(new PdfPCell(new Phrase(line1, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(cell2);
            doc.Add(table);

            #endregion

            doc.Add(new Paragraph("此致", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 64, SpacingAfter = 20 });
            doc.Add(new Paragraph($"{data["ApplyUnit"]}", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 });
            doc.Add(new Paragraph($"申請案號：{data["DocketNo"]}", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 32, SpacingAfter = 16 });
            doc.Add(new Paragraph($"申請人：{data["FarmerName"].PadRight(4)}（簽名或蓋章）", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 64, SpacingAfter = 16 });
            doc.Add(new Paragraph($"通訊地址：{data["FarmerAddr"]}", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 32, SpacingAfter = 16 });
            doc.Add(new Paragraph($"聯絡電話：{data["FarmerTel"]}", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 32, SpacingAfter = 65 });
            doc.Add(new Paragraph($"中華民國            年          月          日", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });

        }
        /// <summary>
        /// 竣工報驗書批次輸出
        /// </summary>
        /// <returns></returns>
        public byte[] CompleteVerifyPDFS(int ApplyYear, int ApplyUnit, int IANumStart, int IANumEnd )
        {
            DryEntities drydb = new DryEntities();
            var ss = new Service.BudgetBookDBService();


            var datalist = from db in drydb.SummaryView
                           where db.ApplyYear == ApplyYear && db.ApplyUnit == ApplyUnit && db.IANum >= IANumStart && db.IANum <= IANumEnd
                           orderby db.IANum
                           select new { db.MapNo };
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in datalist)
                {
                    Dictionary<string, string> dataStr = ss.MergeExportCompleteData((int)item.MapNo);
                    ViewModel.BudgetBookView data = ss.GetBudgetBookData((int)item.MapNo);
                    document.NewPage();
                    writeCompleteVerify(document, dataStr, (int)item.MapNo);
                }
                document.Close();
                return stream.GetBuffer();
            }


        }
        /// <summary>
        /// 列印切結書、收據、結案申報書
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] export3report(ViewModel.ReportView data,short unitid)
        {
            DryEntities drydb = new DryEntities();
            BudgetBookReport BR = new BudgetBookReport();
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            //BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                var datalist = from db in drydb.SummaryView
                               where db.ApplyYear == data.ApplyYear && db.ApplyUnit == unitid && db.IANum >= data.IANumStart && db.IANum <= data.IANumEnd
                               orderby db.IANum
                               select new { db.MapNo };
                foreach (var item in datalist)
                {
                    
                    ViewModel.BudgetBookView dataStr = new Service.BudgetBookDBService().GetBudgetBookData((int)item.MapNo);
                    document.NewPage();
                    writeAcceptAffidavit(document, dataStr, (int)item.MapNo);

                    
                    document.NewPage();
                    Service.PayeeReceiptDBService.DataStruct receiptdata = new PayeeReceiptDBService().GetPayeeReceiptDataByMapno((int)item.MapNo);
                    BR.writeReceipt(document, receiptdata);
                    
                    Dictionary<string, string> dataStr1 = new Service.BudgetBookDBService().MergeExportCompleteData((int)item.MapNo);
                    document.NewPage();
                    writeCompleteVerify(document, dataStr1, (int)item.MapNo);

                }
                document.Close();
                return stream.GetBuffer();
            }
        }

        public byte[] ExportPDFList(List<ExportPDFDataContent> Data)
        {
            List<PdfReader> readers = new List<PdfReader>(); 
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            
            using(Document document = new Document())
            {
                foreach (var item in Data)
                {
                    MemoryStream stream = new MemoryStream();
                    PdfReader pdfreader = new PdfReader(item.TemplateUrl);
                    Guid FileName = Guid.NewGuid();

                    PdfStamper pdfStamper = new PdfStamper(pdfreader, stream);
                    AcroFields pdfFormFields = pdfStamper.AcroFields;
                    foreach (var key in item.KeyData)
                    {
                        //pdfFormFields.SetFieldProperty(key.Key, "textfont", baseFT, null);
                        bool tt = pdfFormFields.SetField(key.Key, key.Value);
                    }
                    //pdfreader.RemoveUnusedObjects();
                    pdfStamper.FormFlattening = true;

                    pdfStamper.Close();
                    pdfreader.Close();
                    
                    readers.Add(new PdfReader(stream.GetBuffer()));
                }
               
                byte[] data = mergePDFFiles(readers);
                return data;
            }
        }

        public byte[] ExportPDFListA(List<ExportPDFDataContent> Data)
        {
            List<PdfReader> readers = new List<PdfReader>(); 
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            using (Document document = new Document())
            {
                foreach (var item in Data)
                {
                    MemoryStream stream = new MemoryStream();
                    PdfReader pdfreader = new PdfReader(item.TemplateUrl);
                    Guid FileName = Guid.NewGuid();

                    PdfStamper pdfStamper = new PdfStamper(pdfreader, stream, PdfWriter.VERSION_1_6);

                    AcroFields pdfFormFields = pdfStamper.AcroFields;
                    foreach (var key in item.KeyData)
                    {
                        pdfFormFields.SetFieldProperty(key.Key, "textfont", baseFT, null);
                        bool tt = pdfFormFields.SetField(key.Key, key.Value);
                    }
                    pdfStamper.FormFlattening = true;
                    pdfreader.RemoveUnusedObjects();

                    int pageNum = pdfreader.NumberOfPages;
                    for (int i = 1; i < pageNum; i++)
                    {
                        pdfreader.SetPageContent(i, pdfreader.GetPageContent(i));
                    }
                    pdfStamper.SetFullCompression();
                    pdfStamper.Close();
                    pdfreader.Close();



                    
                    readers.Add(new PdfReader(stream.GetBuffer()));
                }
                
                byte[] data = mergePDFFiles(readers);
                return data;
            }
        }

        public byte[] ExportPDFListAL(List<ExportPDFDataContent> Data)
        {
            List<PdfReader> readers = new List<PdfReader>(); 
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            using (Document document = new Document())
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data)
                {
                    document.NewPage();
                    document.Add(new Paragraph("設施編號：" + item.KeyData["DocketNo"] + "農戶姓名：" + item.KeyData["FarmerName"], new Font(baseFT, 10)) { IndentationLeft = 32, SpacingAfter = 14, Alignment = Element.ALIGN_RIGHT });

                    Paragraph pHead = new Paragraph("接受補助設置省水管路灌溉設施切結書", new Font(baseFT, 20));
                    pHead.Alignment = Element.ALIGN_CENTER;
                    document.Add(pHead);
                    document.Add(new Paragraph("具切結書人申請行政院農業委員會" + item.KeyData["ApplyYear"] + "年度「推廣省水管路灌溉」計畫設施補助，除遵守貴單位有關規定辦理，願具切結書如下：", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("一、設施內容", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    //document.Add(Chunk.NEWLINE);
                    PdfPTable table = new PdfPTable(new float[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 });
                    table.TotalWidth = 550f;
                    table.LockedWidth = true;
                    PdfPCell cell_1 = new PdfPCell(new Phrase("設施\n項目", new Font(baseFT, 14)));
                    cell_1.Rowspan = 2;
                    cell_1.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_1);
                    PdfPCell cell_2 = new PdfPCell(new Phrase("抽送水設施", new Font(baseFT, 12)));
                    cell_2.Colspan = 2;
                    cell_2.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_2);
                    PdfPCell cell_3 = new PdfPCell(new Phrase("蓄水槽", new Font(baseFT, 12)));
                    cell_3.Rowspan = 2;
                    cell_3.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_3);
                    PdfPCell cell_4 = new PdfPCell(new Phrase("穿孔管\n灌溉", new Font(baseFT, 12)));
                    cell_4.Rowspan = 2;
                    cell_4.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_4);
                    PdfPCell cell_5 = new PdfPCell(new Phrase("噴頭\n灌溉", new Font(baseFT, 12)));
                    cell_5.Rowspan = 2;
                    cell_5.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_5);
                    PdfPCell cell_6 = new PdfPCell(new Phrase("微噴\n灌溉", new Font(baseFT, 12)));
                    cell_6.Rowspan = 2;
                    cell_6.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_6);
                    PdfPCell cell_7 = new PdfPCell(new Phrase("滴水\n灌溉", new Font(baseFT, 12)));
                    cell_7.Rowspan = 2;
                    cell_7.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_7);
                    PdfPCell cell_8 = new PdfPCell(new Phrase("調節控制設施", new Font(baseFT, 12)));
                    cell_8.Colspan = 2;
                    cell_8.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_8);
                    PdfPCell cell_9 = new PdfPCell(new Phrase("動力", new Font(baseFT, 12)));
                    cell_9.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_9);
                    PdfPCell cell_10 = new PdfPCell(new Phrase("抽水機", new Font(baseFT, 12)));
                    cell_10.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_10);
                    PdfPCell cell_11 = new PdfPCell(new Phrase("項目", new Font(baseFT, 12)));
                    cell_11.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_11);
                    PdfPCell cell_12 = new PdfPCell(new Phrase("數量", new Font(baseFT, 12)));
                    cell_12.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_12);
                    table.AddCell(cell_12);
                    PdfPCell cell_13 = new PdfPCell(new Phrase(item.KeyData["Eng"] + "台", new Font(baseFT, 12)));
                    cell_13.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_13);
                    PdfPCell cell_14 = new PdfPCell(new Phrase(item.KeyData["Pump"] + "台", new Font(baseFT, 12)));
                    cell_14.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_14);
                    PdfPCell cell_15 = new PdfPCell(new Phrase(item.KeyData["Pool"] + "噸", new Font(baseFT, 12)));
                    cell_15.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_15);
                    PdfPCell cell_16 = new PdfPCell(new Phrase(item.KeyData["Pipe"] + "公頃", new Font(baseFT, 12)));
                    cell_16.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_16);
                    PdfPCell cell_17 = new PdfPCell(new Phrase(item.KeyData["Nozzle"] + "公頃", new Font(baseFT, 12)));
                    cell_17.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_17);
                    PdfPCell cell_18 = new PdfPCell(new Phrase(item.KeyData["Micro"] + "公頃", new Font(baseFT, 12)));
                    cell_18.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_18);
                    PdfPCell cell_19 = new PdfPCell(new Phrase(item.KeyData["Drip"] + "公頃", new Font(baseFT, 12)));
                    cell_19.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_19);
                    PdfPCell cell_20 = new PdfPCell(new Phrase(item.KeyData["CtrlItem"], new Font(baseFT, 12)));
                    cell_20.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_20);
                    PdfPCell cell_21 = new PdfPCell(new Phrase(item.KeyData["CtrlAmount"], new Font(baseFT, 12)));
                    cell_21.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_21);

                    document.Add(table);
                    document.Add(new Paragraph("    1.「動力」註明柴油、汽油或電動的類別及馬力。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    2.「抽水機」註明柱塞式或一般型式及出水口徑。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("二、補助金額以驗收時的認定，並依計畫補助標準計算。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("三、本設施願於____年____月____日前完成並報驗。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("四、具切結書人如有下列情形之一者，願放棄經費補助，並放棄追訴權：", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    1.未依規定之期限完成並報驗者。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    2.經驗收不合格，且未依指定改善日期辦理完成。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    3.經發現向水保局、農糧署或其他機關申請補助者（符合第二次申請補助之            條件者，不在此限）。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("五、本設施完成後，自當善加養護，並將經營運轉情形成果資料提供貴單位，供作研      究發展計畫之參考", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("此致", new Font(baseFT, 14)) { IndentationLeft = 32, SpacingAfter = 14 });
                    document.Add(new Paragraph("臺灣" + item.KeyData["ApplyUnit"], new Font(baseFT, 16)) { IndentationLeft = 64, SpacingAfter = 14 });
                    document.Add(Chunk.NEWLINE);
                    //document.Add(new Paragraph("申請案號：" + item.KeyData["DocketNo"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("切結書人：" /*+ item.KeyData["FarmerName"]*/ + "            (簽名或蓋章)", new Font(baseFT, 14)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("國民身分證統一編號：" + item.KeyData["FarmerID"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("住址：" + item.KeyData["FarmerAddr"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("聯絡電話：" + item.KeyData["FarmerPhone"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph("中  華  民  國       年      月      日", new Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
                }
                
                //byte[] data = mergePDFFiles(readers);
                //return data;
                document.Close();


                byte[] data = stream.GetBuffer();
                return data;





            }
        }


        public byte[] ExportPDFListAL22(List<ExportPDFDataContent> Data)
        {
            List<PdfReader> readers = new List<PdfReader>(); 
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            using (Document document = new Document())
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data)
                {
                    document.NewPage();
                    document.Add(new Paragraph("設施編號：" + item.KeyData["DocketNo"] + "農戶姓名：" + item.KeyData["FarmerName"], new Font(baseFT, 10)) { IndentationLeft = 32, SpacingAfter = 14, Alignment = Element.ALIGN_RIGHT });

                    Paragraph pHead = new Paragraph("接受補助設置省水管路灌溉設施切結書", new Font(baseFT, 20));
                    pHead.Alignment = Element.ALIGN_CENTER;
                    document.Add(pHead);
                    document.Add(new Paragraph("具切結書人申請行政院農業委員會" + item.KeyData["ApplyYear"] + "年度「推廣省水管路灌溉」計畫設施補助，除遵守貴單位有關規定辦理，願具切結書如下：", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("一、設施內容", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    //document.Add(Chunk.NEWLINE);
                    PdfPTable table = new PdfPTable(new float[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 });
                    table.TotalWidth = 550f;
                    table.LockedWidth = true;
                    PdfPCell cell_1 = new PdfPCell(new Phrase("設施\n項目", new Font(baseFT, 14)));
                    cell_1.Rowspan = 2;
                    cell_1.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_1);
                    PdfPCell cell_2 = new PdfPCell(new Phrase("抽送水設施", new Font(baseFT, 12)));
                    cell_2.Colspan = 2;
                    cell_2.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_2);
                    PdfPCell cell_3 = new PdfPCell(new Phrase("蓄水槽", new Font(baseFT, 12)));
                    cell_3.Rowspan = 2;
                    cell_3.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_3);
                    PdfPCell cell_4 = new PdfPCell(new Phrase("穿孔管\n灌溉", new Font(baseFT, 12)));
                    cell_4.Rowspan = 2;
                    cell_4.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_4);
                    PdfPCell cell_5 = new PdfPCell(new Phrase("噴頭\n灌溉", new Font(baseFT, 12)));
                    cell_5.Rowspan = 2;
                    cell_5.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_5);
                    PdfPCell cell_6 = new PdfPCell(new Phrase("微噴\n灌溉", new Font(baseFT, 12)));
                    cell_6.Rowspan = 2;
                    cell_6.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_6);
                    PdfPCell cell_7 = new PdfPCell(new Phrase("滴水\n灌溉", new Font(baseFT, 12)));
                    cell_7.Rowspan = 2;
                    cell_7.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_7);
                    PdfPCell cell_8 = new PdfPCell(new Phrase("調節控制設施", new Font(baseFT, 12)));
                    cell_8.Colspan = 2;
                    cell_8.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_8);
                    PdfPCell cell_9 = new PdfPCell(new Phrase("動力", new Font(baseFT, 12)));
                    cell_9.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_9);
                    PdfPCell cell_10 = new PdfPCell(new Phrase("抽水機", new Font(baseFT, 12)));
                    cell_10.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_10);
                    PdfPCell cell_11 = new PdfPCell(new Phrase("項目", new Font(baseFT, 12)));
                    cell_11.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_11);
                    PdfPCell cell_12 = new PdfPCell(new Phrase("數量", new Font(baseFT, 12)));
                    cell_12.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_12);
                    table.AddCell(cell_12);
                    PdfPCell cell_13 = new PdfPCell(new Phrase(item.KeyData["Eng"] + "台", new Font(baseFT, 12)));
                    cell_13.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_13);
                    PdfPCell cell_14 = new PdfPCell(new Phrase(item.KeyData["Pump"] + "台", new Font(baseFT, 12)));
                    cell_14.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_14);
                    PdfPCell cell_15 = new PdfPCell(new Phrase(item.KeyData["Pool"] + "噸", new Font(baseFT, 12)));
                    cell_15.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_15);
                    PdfPCell cell_16 = new PdfPCell(new Phrase(item.KeyData["Pipe"] + "公頃", new Font(baseFT, 12)));
                    cell_16.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_16);
                    PdfPCell cell_17 = new PdfPCell(new Phrase(item.KeyData["Nozzle"] + "公頃", new Font(baseFT, 12)));
                    cell_17.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_17);
                    PdfPCell cell_18 = new PdfPCell(new Phrase(item.KeyData["Micro"] + "公頃", new Font(baseFT, 12)));
                    cell_18.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_18);
                    PdfPCell cell_19 = new PdfPCell(new Phrase(item.KeyData["Drip"] + "公頃", new Font(baseFT, 12)));
                    cell_19.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_19);
                    PdfPCell cell_20 = new PdfPCell(new Phrase(item.KeyData["CtrlItem"], new Font(baseFT, 12)));
                    cell_20.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_20);
                    PdfPCell cell_21 = new PdfPCell(new Phrase(item.KeyData["CtrlAmount"], new Font(baseFT, 12)));
                    cell_21.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_21);

                    document.Add(table);
                    document.Add(new Paragraph("    1.「動力」註明柴油、汽油或電動的類別及馬力。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    2.「抽水機」註明柱塞式或一般型式及出水口徑。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("二、補助金額以驗收時的認定，並依計畫補助標準計算。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("三、本設施願於____年____月____日前完成並報驗。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("四、具切結書人如有下列情形之一者，願放棄經費補助，並放棄追訴權：", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    1.未依規定之期限完成並報驗者。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    2.經驗收不合格，且未依指定改善日期辦理完成。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    3.經發現向水保局、農糧署或其他機關申請補助者（符合第二次申請補助之            條件者，不在此限）。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("五、本設施完成後，自當善加養護，並將經營運轉情形成果資料提供貴單位，供作研      究發展計畫之參考", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("此致", new Font(baseFT, 14)) { IndentationLeft = 32, SpacingAfter = 14 });
                    document.Add(new Paragraph( item.KeyData["ApplyUnit"], new Font(baseFT, 16)) { IndentationLeft = 64, SpacingAfter = 14 });
                    document.Add(Chunk.NEWLINE);
                    //document.Add(new Paragraph("申請案號：" + item.KeyData["DocketNo"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("切結書人：" /*+ item.KeyData["FarmerName"]*/ + "            (簽名或蓋章)", new Font(baseFT, 14)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("國民身分證統一編號：" + item.KeyData["FarmerID"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("通信地址：" + item.KeyData["FarmerAddr"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("聯絡電話：" + item.KeyData["FarmerPhone"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph("中  華  民  國       年      月      日", new Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
                }
                
                //byte[] data = mergePDFFiles(readers);
                //return data;
                document.Close();


                byte[] data = stream.GetBuffer();
                return data;





            }
        }

        public byte[] ExportPDFListAL13(List<ExportPDFDataContent> Data)
        {
            List<PdfReader> readers = new List<PdfReader>(); 
            //中文字型
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            using (Document document = new Document())
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data)
                {
                    document.NewPage();
                    document.Add(new Paragraph("設施編號：" + item.KeyData["DocketNo"] + "農戶姓名：" + item.KeyData["FarmerName"], new Font(baseFT, 10)) { IndentationLeft = 32, SpacingAfter = 14, Alignment = Element.ALIGN_RIGHT });

                    Paragraph pHead = new Paragraph("接受補助設置省水管路灌溉設施切結書", new Font(baseFT, 20));
                    pHead.Alignment = Element.ALIGN_CENTER;
                    document.Add(pHead);
                    document.Add(new Paragraph("具切結書人申請行政院農業委員會" + item.KeyData["ApplyYear"] + "年度「推廣省水管路灌溉」計畫設施補助，除遵守貴單位有關規定辦理，願具切結書如下：", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("一、設施內容", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    //document.Add(Chunk.NEWLINE);
                    PdfPTable table = new PdfPTable(new float[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 });
                    table.TotalWidth = 550f;
                    table.LockedWidth = true;
                    PdfPCell cell_1 = new PdfPCell(new Phrase("設施\n項目", new Font(baseFT, 14)));
                    cell_1.Rowspan = 2;
                    cell_1.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_1);
                    PdfPCell cell_2 = new PdfPCell(new Phrase("抽送水設施", new Font(baseFT, 12)));
                    cell_2.Colspan = 2;
                    cell_2.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_2);
                    PdfPCell cell_3 = new PdfPCell(new Phrase("蓄水槽", new Font(baseFT, 12)));
                    cell_3.Rowspan = 2;
                    cell_3.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_3);
                    PdfPCell cell_4 = new PdfPCell(new Phrase("穿孔管\n灌溉", new Font(baseFT, 12)));
                    cell_4.Rowspan = 2;
                    cell_4.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_4);
                    PdfPCell cell_5 = new PdfPCell(new Phrase("噴頭\n灌溉", new Font(baseFT, 12)));
                    cell_5.Rowspan = 2;
                    cell_5.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_5);
                    PdfPCell cell_6 = new PdfPCell(new Phrase("微噴\n灌溉", new Font(baseFT, 12)));
                    cell_6.Rowspan = 2;
                    cell_6.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_6);
                    PdfPCell cell_7 = new PdfPCell(new Phrase("滴水\n灌溉", new Font(baseFT, 12)));
                    cell_7.Rowspan = 2;
                    cell_7.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_7);
                    PdfPCell cell_8 = new PdfPCell(new Phrase("調節控制設施", new Font(baseFT, 12)));
                    cell_8.Colspan = 2;
                    cell_8.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_8);
                    PdfPCell cell_9 = new PdfPCell(new Phrase("動力", new Font(baseFT, 12)));
                    cell_9.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_9);
                    PdfPCell cell_10 = new PdfPCell(new Phrase("抽水機", new Font(baseFT, 12)));
                    cell_10.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_10);
                    PdfPCell cell_11 = new PdfPCell(new Phrase("項目", new Font(baseFT, 12)));
                    cell_11.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_11);
                    PdfPCell cell_12 = new PdfPCell(new Phrase("數量", new Font(baseFT, 12)));
                    cell_12.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_12);
                    table.AddCell(cell_12);
                    PdfPCell cell_13 = new PdfPCell(new Phrase(item.KeyData["Eng"] + "台", new Font(baseFT, 12)));
                    cell_13.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_13);
                    PdfPCell cell_14 = new PdfPCell(new Phrase(item.KeyData["Pump"] + "台", new Font(baseFT, 12)));
                    cell_14.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_14);
                    PdfPCell cell_15 = new PdfPCell(new Phrase(item.KeyData["Pool"] + "噸", new Font(baseFT, 12)));
                    cell_15.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_15);
                    PdfPCell cell_16 = new PdfPCell(new Phrase(item.KeyData["Pipe"] + "公頃", new Font(baseFT, 12)));
                    cell_16.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_16);
                    PdfPCell cell_17 = new PdfPCell(new Phrase(item.KeyData["Nozzle"] + "公頃", new Font(baseFT, 12)));
                    cell_17.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_17);
                    PdfPCell cell_18 = new PdfPCell(new Phrase(item.KeyData["Micro"] + "公頃", new Font(baseFT, 12)));
                    cell_18.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_18);
                    PdfPCell cell_19 = new PdfPCell(new Phrase(item.KeyData["Drip"] + "公頃", new Font(baseFT, 12)));
                    cell_19.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_19);
                    PdfPCell cell_20 = new PdfPCell(new Phrase(item.KeyData["CtrlItem"], new Font(baseFT, 12)));
                    cell_20.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_20);
                    PdfPCell cell_21 = new PdfPCell(new Phrase(item.KeyData["CtrlAmount"], new Font(baseFT, 12)));
                    cell_21.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_21);

                    document.Add(table);
                    document.Add(new Paragraph("    1.「動力」註明柴油、汽油或電動的類別及馬力。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    2.「抽水機」註明柱塞式或一般型式及出水口徑。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("二、補助金額以驗收時的認定，並依計畫補助標準計算。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("三、本設施願於____年____月____日前完成並報驗。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("四、具切結書人如有下列情形之一者，願放棄經費補助，並放棄追訴權：", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    1.未依規定之期限完成並報驗者。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    2.經驗收不合格，且未依指定改善日期辦理完成。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    3.經發現向水保局、農糧署或其他機關申請補助者（符合第二次申請補助之            條件者，不在此限）。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("五、本設施完成後，自當善加養護，並將經營運轉情形成果資料提供貴單位，供作研      究發展計畫之參考", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("此致", new Font(baseFT, 14)) { IndentationLeft = 32, SpacingAfter = 14 });
                    document.Add(new Paragraph("臺灣" + item.KeyData["ApplyUnit"], new Font(baseFT, 16)) { IndentationLeft = 64, SpacingAfter = 14 });
                    document.Add(Chunk.NEWLINE);
                    //document.Add(new Paragraph("申請案號：" + item.KeyData["DocketNo"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("切結書人：" + item.KeyData["FarmerName"] + "            (簽名或蓋章)", new Font(baseFT, 14)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("國民身分證統一編號：" + item.KeyData["FarmerID"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("住址：" + item.KeyData["FarmerAddr"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("連絡電話：" + item.KeyData["FarmerPhone"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph("中  華  民  國       年      月      日", new Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
                }
                
                //byte[] data = mergePDFFiles(readers);
                //return data;
                document.Close();


                byte[] data = stream.GetBuffer();
                return data;





            }
        }

        public byte[] ExportPDFListAL11(List<ExportPDFDataContent> Data)
        {



            List<PdfReader> readers = new List<PdfReader>(); 
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            using (Document document = new Document())
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data)
                {
                    document.NewPage();
                    document.Add(new Paragraph("設施編號：" + item.KeyData["DocketNo"] , new Font(baseFT, 10)) { IndentationLeft = 400,  Alignment = Element.ALIGN_LEFT });
                    document.Add(new Paragraph("農戶姓名：" + item.KeyData["FarmerName"], new Font(baseFT, 10)) { IndentationLeft = 400, SpacingAfter = 14, Alignment = Element.ALIGN_LEFT });
                    Paragraph pHead = new Paragraph("接受補助設置省水管路灌溉設施切結書", new Font(baseFT, 20));
                    pHead.Alignment = Element.ALIGN_CENTER;
                    document.Add(pHead);
                    document.Add(new Paragraph("具切結書人申請行政院農業委員會「推廣省水管路灌溉」計畫設施補助，除遵守貴單位有關規定辦理，願具切結書如下：", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("一、施設內容", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    //document.Add(Chunk.NEWLINE);
                    PdfPTable table = new PdfPTable(new float[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 });
                    table.TotalWidth = 550f;
                    table.LockedWidth = true;
                    PdfPCell cell_1 = new PdfPCell(new Phrase("設施\n項目", new Font(baseFT, 14)));
                    cell_1.Rowspan = 2;
                    cell_1.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_1);
                    PdfPCell cell_2 = new PdfPCell(new Phrase("抽送水設施", new Font(baseFT, 12)));
                    cell_2.Colspan = 2;
                    cell_2.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_2);
                    PdfPCell cell_3 = new PdfPCell(new Phrase("蓄水槽", new Font(baseFT, 12)));
                    cell_3.Rowspan = 2;
                    cell_3.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_3);
                    PdfPCell cell_4 = new PdfPCell(new Phrase("穿孔管\n灌溉", new Font(baseFT, 12)));
                    cell_4.Rowspan = 2;
                    cell_4.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_4);
                    PdfPCell cell_5 = new PdfPCell(new Phrase("噴頭\n灌溉", new Font(baseFT, 12)));
                    cell_5.Rowspan = 2;
                    cell_5.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_5);
                    PdfPCell cell_6 = new PdfPCell(new Phrase("微噴\n灌溉", new Font(baseFT, 12)));
                    cell_6.Rowspan = 2;
                    cell_6.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_6);
                    PdfPCell cell_7 = new PdfPCell(new Phrase("滴水\n灌溉", new Font(baseFT, 12)));
                    cell_7.Rowspan = 2;
                    cell_7.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_7);
                    PdfPCell cell_8 = new PdfPCell(new Phrase("調節控制設施", new Font(baseFT, 12)));
                    cell_8.Colspan = 2;
                    cell_8.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_8);
                    PdfPCell cell_9 = new PdfPCell(new Phrase("動力", new Font(baseFT, 12)));
                    cell_9.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_9);
                    PdfPCell cell_10 = new PdfPCell(new Phrase("抽水機", new Font(baseFT, 12)));
                    cell_10.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_10);
                    PdfPCell cell_11 = new PdfPCell(new Phrase("項目", new Font(baseFT, 12)));
                    cell_11.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_11);
                    PdfPCell cell_12 = new PdfPCell(new Phrase("數量", new Font(baseFT, 12)));
                    cell_12.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_12);
                    table.AddCell(cell_12);
                    PdfPCell cell_13 = new PdfPCell(new Phrase(item.KeyData["Eng"] + "台", new Font(baseFT, 12)));
                    cell_13.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_13);
                    PdfPCell cell_14 = new PdfPCell(new Phrase(item.KeyData["Pump"] + "台", new Font(baseFT, 12)));
                    cell_14.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_14);
                    PdfPCell cell_15 = new PdfPCell(new Phrase(item.KeyData["Pool"] + "噸", new Font(baseFT, 12)));
                    cell_15.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_15);
                    PdfPCell cell_16 = new PdfPCell(new Phrase(item.KeyData["Pipe"] + "公頃", new Font(baseFT, 12)));
                    cell_16.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_16);
                    PdfPCell cell_17 = new PdfPCell(new Phrase(item.KeyData["Nozzle"] + "公頃", new Font(baseFT, 12)));
                    cell_17.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_17);
                    PdfPCell cell_18 = new PdfPCell(new Phrase(item.KeyData["Micro"] + "公頃", new Font(baseFT, 12)));
                    cell_18.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_18);
                    PdfPCell cell_19 = new PdfPCell(new Phrase(item.KeyData["Drip"] + "公頃", new Font(baseFT, 12)));
                    cell_19.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_19);
                    PdfPCell cell_20 = new PdfPCell(new Phrase(item.KeyData["CtrlItem"], new Font(baseFT, 12)));
                    cell_20.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_20);
                    PdfPCell cell_21 = new PdfPCell(new Phrase(item.KeyData["CtrlAmount"], new Font(baseFT, 12)));
                    cell_21.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_21);
                    
                    PdfPCell cell_22 = new PdfPCell(new Phrase(/*"補助\n金額"*/" \n ", new Font(baseFT, 14)));
                    cell_22.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_22);
                    PdfPCell cell_23 = new PdfPCell(new Phrase(/*item.KeyData["EngMoney"] + "元"*/"", new Font(baseFT, 12)));
                    cell_23.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell_23.Colspan = 2;
                    table.AddCell(cell_23);
                    PdfPCell cell_24 = new PdfPCell(new Phrase(/*item.KeyData["PoolMoney"] + "元"*/"", new Font(baseFT, 12)));
                    cell_24.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_24);
                    PdfPCell cell_25 = new PdfPCell(new Phrase(/*item.KeyData["PipePrice"] + "元"*/"", new Font(baseFT, 12)));
                    cell_25.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_25);
                    PdfPCell cell_26 = new PdfPCell(new Phrase(/*item.KeyData["NozzlePrice"] + "元"*/"", new Font(baseFT, 12)));
                    cell_26.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_26);
                    PdfPCell cell_27 = new PdfPCell(new Phrase(/*item.KeyData["MicroPrice"] + "元"*/"", new Font(baseFT, 12)));
                    cell_27.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_27);
                    PdfPCell cell_28 = new PdfPCell(new Phrase(/*item.KeyData["DripPrice"] + "元"*/"", new Font(baseFT, 12)));
                    cell_28.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell_28);
                    table.AddCell(cell_21);
                    table.AddCell(cell_21);
                    
                    document.Add(table);
                    document.Add(new Paragraph("二、補助金額以驗收時的認定，並依計畫補助標準計算。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("三、本設施願於 "+item.KeyData["Year"]+" 年 "+item.KeyData["Moon"]+" 月 "+item.KeyData["Day"]+" 日前完成並報驗。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("四、具切結書人如有下列情形之一者，願放棄經費補助，並放棄追訴權：", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    1.未依規定之期限完成並報驗者。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    2.經驗收不合格，且未依指定改善日期辦理完成。", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("    3.經發現向水保局、農糧署或其他機關申請補助者（符合第二次申請補助之 條件者，不在此限）。", new Font(baseFT, 16)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("五、本設施完成後，自當善加養護，並將經營運轉情形成果資料提供貴單", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("位，供作研究發展計畫之參考", new Font(baseFT, 14)) { SpacingAfter = 4 });
                    document.Add(new Paragraph("此致", new Font(baseFT, 14)) { IndentationLeft = 32, SpacingAfter = 14 });
                    document.Add(new Paragraph(item.KeyData["ApplyUnit"], new Font(baseFT, 16)) { IndentationLeft = 64, SpacingAfter = 14 });
                    document.Add(Chunk.NEWLINE);
                    //document.Add(new Paragraph("申請案號：" + item.KeyData["DocketNo"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("切結書人：" /*+ item.KeyData["FarmerName"]*/ + "            (簽名或蓋章)", new Font(baseFT, 14)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("國民身分證統一編號：" + item.KeyData["FarmerID"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("住址：" + item.KeyData["FarmerAddr"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("連絡電話：" + item.KeyData["FarmerPhone"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph("中  華  民  國  "+item.KeyData["ApplyYear"] +  "  年      月      日", new Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
                }
                
                //byte[] data = mergePDFFiles(readers);
                //return data;
                document.Close();


                byte[] data = stream.GetBuffer();
                return data;
            }
        }


        public byte[] ExportPDFListB(List<ExportPDFDataContent> Data)
        {
            List<PdfReader> readers = new List<PdfReader>(); 
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
           
            using (Document document = new Document( PageSize.A4,72,72,72,72 ))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data)
                {
                                     
                    document.NewPage();


                    Paragraph pHead = new Paragraph("竣工報驗書", new Font(baseFT, 22));
                    pHead.Alignment = Element.ALIGN_CENTER;
                    document.Add(pHead);
                    document.Add(Chunk.NEWLINE);


                    StringBuilder line = new StringBuilder();

                    line.Append("茲申請農業部" + item.KeyData["ApplyYear"] + "年度推廣省水管路灌溉計畫設施補助，");
                    line.Append("設置地點：" + item.KeyData["Town"] + "（鄉鎮市），" + item.KeyData["Section"] + "段");
                    line.Append(item.KeyData["SubSection"] + "小段" + item.KeyData["LandNo"] + "地號合計" + item.KeyData["LandAmount"] + "筆，");
                    line.Append("合計面積" + item.KeyData["LandArea"] + "公頃，已於民國____年____月____日全部竣工，請貴單位派員驗收。");
                    document.Add(new Paragraph(line.ToString(), new Font(baseFT, 16)) {SpacingAfter = 4 });

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    document.Add(new Paragraph("此致", new Font(baseFT, 16)) { IndentationLeft = 32, SpacingAfter = 14 });

                    document.Add(new Paragraph("臺灣" + item.KeyData["ApplyUnit"], new Font(baseFT, 18)) { IndentationLeft = 64, SpacingAfter = 14 });

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);



                    document.Add(new Paragraph("申請案號：" + item.KeyData["DocketNo"], new Font(baseFT, 16)) { IndentationLeft = 24});
                    document.Add(new Paragraph("申請人：" /*+ item.KeyData["FarmerName"]*/ + "            (簽名或蓋章)", new Font(baseFT, 14)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("住址：" + item.KeyData["FarmerAddr"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("連絡電話：" + item.KeyData["FarmerTel"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    document.Add(new Paragraph("中  華  民  國       年      月      日", new Font(baseFT, 16)) {Alignment = Element.ALIGN_CENTER});
                    


                    
                }
                
                document.Close();
                

                byte[] data = stream.GetBuffer();
                return data;
            }
        }

        public byte[] ExportPDFList22(List<ExportPDFDataContent> Data)
        {
            List<PdfReader> readers = new List<PdfReader>(); 
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            using (Document document = new Document(PageSize.A4, 72, 72, 72, 72))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data)
                {

                    document.NewPage();


                    Paragraph pHead = new Paragraph("竣工報驗書", new Font(baseFT, 22));
                    pHead.Alignment = Element.ALIGN_CENTER;
                    document.Add(pHead);
                    document.Add(Chunk.NEWLINE);


                    StringBuilder line = new StringBuilder();

                    line.Append("茲申請農業部" + item.KeyData["ApplyYear"] + "年度推廣省水管路灌溉計畫設施補助，");
                    line.Append("設置地點：" + item.KeyData["Town"] + "（鄉鎮市），" + item.KeyData["Section"] + "段");
                    line.Append(item.KeyData["SubSection"] + "小段" + item.KeyData["LandNo"] + "地號合計" + item.KeyData["LandAmount"] + "筆，");
                    line.Append("合計面積" + item.KeyData["LandArea"] + "公頃，已於民國____年____月____日全部竣工，請貴單位派員驗收。");
                    document.Add(new Paragraph(line.ToString(), new Font(baseFT, 16)) { SpacingAfter = 4 });

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    document.Add(new Paragraph("此致", new Font(baseFT, 16)) { IndentationLeft = 32, SpacingAfter = 14 });

                    document.Add(new Paragraph(item.KeyData["ApplyUnit"], new Font(baseFT, 18)) { IndentationLeft = 64, SpacingAfter = 14 });

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);



                    document.Add(new Paragraph("申請案號：" + item.KeyData["DocketNo"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("申請人：" /*+ item.KeyData["FarmerName"]*/ + "            (簽名或蓋章)", new Font(baseFT, 14)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("住址：" + item.KeyData["FarmerAddr"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("連絡電話：" + item.KeyData["FarmerTel"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    document.Add(new Paragraph("中  華  民  國       年      月      日", new Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });




                }
                
                document.Close();


                byte[] data = stream.GetBuffer();
                return data;
            }
        }

        public byte[] ExportPDFList11(List<ExportPDFDataContent> Data)
        {
            List<PdfReader> readers = new List<PdfReader>(); 
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            using (Document document = new Document(PageSize.A4, 72, 72, 72, 72))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data)
                {

                    document.NewPage();

                    document.Add(new Paragraph("設施編號：" + item.KeyData["DocketNo"] , new Font(baseFT, 12)) { IndentationLeft = 350,  Alignment = Element.ALIGN_LEFT });
                    document.Add(new Paragraph("農戶姓名：" + item.KeyData["FarmerName"], new Font(baseFT, 12)) { IndentationLeft = 350, SpacingAfter = 14, Alignment = Element.ALIGN_LEFT });
                    Paragraph pHead = new Paragraph("竣工報驗書", new Font(baseFT, 22));
                    pHead.Alignment = Element.ALIGN_CENTER;
                    document.Add(pHead);
                    document.Add(Chunk.NEWLINE);


                    StringBuilder line = new StringBuilder();

                    line.Append("茲申請農業部" + item.KeyData["ApplyYear"] + "年度推廣省水管路灌溉計畫設施補助，");
                    line.Append("設置地點：" + item.KeyData["Town"] + "（鄉鎮市），" + item.KeyData["Section"] + "段");
                    line.Append(item.KeyData["SubSection"] + "小段" + item.KeyData["LandNo"] + "地號合計" + item.KeyData["LandAmount"] + "筆，");
                    line.Append("合計面積" + item.KeyData["LandArea"] + "公頃，已於民國____年____月____日全部竣工，請貴單位派員驗收。");
                    document.Add(new Paragraph(line.ToString(), new Font(baseFT, 16)) { SpacingAfter = 4 });

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    document.Add(new Paragraph("此致", new Font(baseFT, 16)) { IndentationLeft = 32, SpacingAfter = 14 });

                    document.Add(new Paragraph(item.KeyData["ApplyUnit"], new Font(baseFT, 18)) { IndentationLeft = 64, SpacingAfter = 14 });

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);



                    document.Add(new Paragraph("申請案號：" + item.KeyData["DocketNo"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("申請人：" /*+ item.KeyData["FarmerName"]*/ + "            (簽名或蓋章)", new Font(baseFT, 14)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("住址：" + item.KeyData["FarmerAddr"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("連絡電話：" + item.KeyData["FarmerTel"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    //document.Add(Chunk.NEWLINE);
                    //document.Add(Chunk.NEWLINE);

                    document.Add(new Paragraph("中  華  民  國  "+ item.KeyData["ApplyYear"] + " 年      月      日", new Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });




                }
                
                document.Close();


                byte[] data = stream.GetBuffer();
                return data;
            }
        }

        public byte[] ExportPDFList12(List<ExportPDFDataContent> Data)
        {
            List<PdfReader> readers = new List<PdfReader>(); 
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            using (Document document = new Document(PageSize.A4, 72, 72, 72, 72))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data)
                {

                    document.NewPage();

                    document.Add(new Paragraph("設施編號：" + item.KeyData["DocketNo"] + "農戶姓名：" + item.KeyData["FarmerName"], new Font(baseFT, 12)) { IndentationLeft = 32, SpacingAfter = 14, Alignment = Element.ALIGN_RIGHT });
                    Paragraph pHead = new Paragraph("竣工報驗書", new Font(baseFT, 22));
                    pHead.Alignment = Element.ALIGN_CENTER;
                    document.Add(pHead);
                    document.Add(Chunk.NEWLINE);


                    StringBuilder line = new StringBuilder();

                    line.Append("茲申請農業部" + item.KeyData["ApplyYear"] + "年度推廣省水管路灌溉計畫設施補助，");
                    line.Append("設置地點：" + item.KeyData["Town"] + "（鄉鎮市），" + item.KeyData["Section"] + "段");
                    line.Append(item.KeyData["SubSection"] + "小段" + item.KeyData["LandNo"] + "地號合計" + item.KeyData["LandAmount"] + "筆，");
                    line.Append("合計面積" + item.KeyData["LandArea"] + "公頃，已於民國____年____月____日全部竣工，請貴單位派員驗收。");
                    document.Add(new Paragraph(line.ToString(), new Font(baseFT, 16)) { SpacingAfter = 4 });

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    document.Add(new Paragraph("此致", new Font(baseFT, 16)) { IndentationLeft = 32, SpacingAfter = 14 });

                    document.Add(new Paragraph("臺灣" + item.KeyData["ApplyUnit"], new Font(baseFT, 18)) { IndentationLeft = 64, SpacingAfter = 14 });

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);



                    document.Add(new Paragraph("申請案號：" + item.KeyData["DocketNo"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("申請人：" /*+ item.KeyData["FarmerName"]*/ + "            (簽名或蓋章)", new Font(baseFT, 14)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("住址：" + item.KeyData["FarmerAddr"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(new Paragraph("連絡電話：" + item.KeyData["FarmerTel"], new Font(baseFT, 16)) { IndentationLeft = 24 });
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);
                    document.Add(Chunk.NEWLINE);

                    document.Add(new Paragraph("中  華  民  國       年      月      日", new Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });




                }
                
                document.Close();


                byte[] data = stream.GetBuffer();
                return data;
            }
        }

        public byte[] ExportPDFList13(List<ExportPDFDataContent> Data)
        {
          
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

            using (Document document = new Document(PageSize.A4, 72, 72, 10, 72))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in Data)
                {

                    document.NewPage();
                    document.Add(new Paragraph(item.KeyData["DocketNo"], new Font(baseFT, 24)) { Alignment = Element.ALIGN_RIGHT });
                    document.Add(new Paragraph("臺灣" + item.KeyData["ApplyUnit"], new Font(baseFT, 24)) { Alignment = Element.ALIGN_CENTER });
                    Paragraph pHead = new Paragraph("竣工報驗書", new Font(baseFT, 22)) { IndentationLeft = 154};
                    pHead.Add(new Chunk("          "));
                    //pHead.Add(new Phrase(item.KeyData["DocketNo"], new Font(baseFT, 24)));
                    //pHead.Alignment = Element.ALIGN_CENTER;
                    document.Add(pHead);
                    document.Add(Chunk.NEWLINE);


                    StringBuilder line = new StringBuilder();

                    line.Append("茲申請農業部" + item.KeyData["ApplyYear"] + "年度推廣省水管路灌溉計畫設施補助，");
                    line.Append("設置地點：" + item.KeyData["Town"] + "（鄉鎮市），" + item.KeyData["Section"] + "段");
                    line.Append(item.KeyData["SubSection"] + "小段" + item.KeyData["LandNo"] + "地號合計" + item.KeyData["LandAmount"] + "筆，");
                    line.Append("合計面積" + item.KeyData["LandArea"] + "公頃，已於民國____年____月____日全部竣工，請貴單位派員驗收。");
                    document.Add(new Paragraph(line.ToString(), new Font(baseFT, 16)) { SpacingAfter = 4 });

                    //document.Add(Chunk.NEWLINE);
                    //document.Add(Chunk.NEWLINE);

                    document.Add(new Paragraph("此致", new Font(baseFT, 16)) { IndentationLeft = 32, SpacingAfter = 14 });

                    document.Add(new Paragraph("臺灣" + item.KeyData["ApplyUnit"], new Font(baseFT, 18)) { IndentationLeft = 64, SpacingAfter = 14 });

                    //document.Add(Chunk.NEWLINE);
                   

                    document.Add(new Paragraph("申請案號：" + item.KeyData["DocketNo"], new Font(baseFT, 16)) { IndentationLeft = 24, Leading = 30 });
                    document.Add(new Paragraph("申請人：" + item.KeyData["FarmerName"] + "                 (簽名或蓋章)", new Font(baseFT, 16)) { IndentationLeft = 24, Leading = 30 });
                    document.Add(new Paragraph("住址：" + item.KeyData["FarmerAddr"], new Font(baseFT, 16)) { IndentationLeft = 24, Leading = 30 });
                    document.Add(new Paragraph("連絡電話：" + item.KeyData["FarmerTel"], new Font(baseFT, 16)) { IndentationLeft = 24, Leading = 30 });
                    document.Add(Chunk.NEWLINE);
                    //document.Add(Chunk.NEWLINE);
                   
                    PdfPTable table = new PdfPTable(new float[] { 1, 5 });
                    table.AddCell(new PdfPCell(new Phrase("主 辦 人", new Font(baseFT, 14))) {FixedHeight = 36, VerticalAlignment = Element.ALIGN_MIDDLE });
                    table.AddCell(new PdfPCell(new Phrase("", new Font(baseFT, 14))) );
                    table.AddCell(new PdfPCell(new Phrase("股    長", new Font(baseFT, 14))) { FixedHeight = 36, VerticalAlignment = Element.ALIGN_MIDDLE });
                    table.AddCell(new PdfPCell(new Phrase("", new Font(baseFT, 14))) );
                    table.AddCell(new PdfPCell(new Phrase("組    長", new Font(baseFT, 14))) { FixedHeight = 36, VerticalAlignment = Element.ALIGN_MIDDLE });
                    table.AddCell(new PdfPCell(new Phrase("", new Font(baseFT, 14))) );
                    table.AddCell(new PdfPCell(new Phrase("主 計 室", new Font(baseFT, 14))) { FixedHeight = 36, VerticalAlignment = Element.ALIGN_MIDDLE });
                    table.AddCell(new PdfPCell(new Phrase("", new Font(baseFT, 14))));
                    document.Add(table);
                    document.Add(Chunk.NEWLINE);
                    document.Add(new Paragraph("中  華  民  國       年      月      日", new Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });




                }
                
                document.Close();


                byte[] data = stream.GetBuffer();
                return data;
            }
        }
        /// <summary>
        /// 合併PDF
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] mergePDFFiles(List<PdfReader> data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (Document document = new Document(iTextSharp.text.PageSize.A4))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    writer.CompressionLevel = PdfStream.BEST_COMPRESSION;
                    writer.SetFullCompression();
                    
                    PdfReader reader;
                    document.Open();
                    var cb = writer.DirectContent;
                    PdfImportedPage newPage;
                    for (var i = 0; i < data.Count; i++)
                    {
                        reader = data[i];

                        var iPageNum = reader.NumberOfPages;
                        PdfPTable table = new PdfPTable(iPageNum);
                        for (var j = 1; j <= iPageNum; j++)
                        {
                            
                            document.NewPage();
                            newPage = writer.GetImportedPage(reader, j);
                            cb.AddTemplate(newPage, 0, 0);
                            
                            
                        }
                        
                    }
                    
                    document.Close();
                   
                }
                return stream.ToArray();
            }
        }

    }
    
    public class ExportPDFDataContent
    {
        /// <summary>
        /// 欄位內容資料
        /// </summary>
        public Dictionary<string, string> KeyData { get; set; }
        /// <summary>
        /// 樣版檔案路徑
        /// </summary>
        public string TemplateUrl { get; set; }
    }
}
