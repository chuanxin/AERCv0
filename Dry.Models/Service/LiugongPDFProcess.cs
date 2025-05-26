/*
-- =============================================
-- Author: wei
-- Create date: 2015-6-25
-- Description: 產出瑠公旱作申請書
-- =============================================
*/
//
//                       _oo0oo_
//                      o8888888o
//                      88" . "88
//                      (| -_- |)
//                      0\  =  /0
//                    ___/`---'\___
//                  .' \\|     |// '.
//                 / \\|||  :  |||// \
//                / _||||| -:- |||||- \
//               |   | \\\  -  /// |   |
//               | \_|  ''\---/''  |_/ |
//               \  .-\__  '-'  ___/-. /
//             ___'. .'  /--.--\  `. .'___
//          ."" '<  `.___\_<|>_/___.' >' "".
//         | | :  `- \`.;`\ _ /`;.`/ - ` : | |
//         \  \ `_.   \_ __\ /__ _/   .-` /  /
//     =====`-.____`.___ \_____/___.-`___.-'=====
//                       `=---='
//
//
//     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
//               佛祖保佑         永無BUG

using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.CommonCls;
using System.Xml;

namespace Dry.Models.Service
{
    public class LiugongPDFProcess
    {
        DryEntities DB = new DryEntities();
        
        static BaseFont bfChinese = BaseFont.CreateFont(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        static BaseFont bfNum = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        //static BaseFont bfChinese = BaseFont.CreateFont(@"C:\\WINDOWS\Fonts\kaiu.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        Font font0 = new Font(bfChinese, 22f, Font.NORMAL);
        Font font1 = new Font(bfChinese, 11.5f, Font.NORMAL);
        Font font2 = new Font(bfChinese, 16f, Font.NORMAL);
        Font fontNum = new Font(bfNum, 11.6f, Font.NORMAL);

        public byte[] CreatePDF(string filePath, int mapNo)
        {
            
            var reports = DB.LiugongReport.Where(p => p.MapNo == mapNo).ToList();
            var engine = DB.LiugongReport_Engine.Where(p => p.MapNo == mapNo).ToList();
            var piging = DB.LiugongReport_Piging.Where(p => p.MapNo == mapNo).ToList();
            var pool = DB.LiugongReport_Pool.Where(p => p.MapNo == mapNo).ToList();


            Document Doc = new Document();
            Doc.SetPageSize(PageSize.A4);
            
            MemoryStream Memory = new MemoryStream();
            PdfWriter PdfWriter = PdfWriter.GetInstance(Doc, Memory);

            
            Doc.Open();
            cover(Doc, reports.FirstOrDefault(), piging.FirstOrDefault());
            Doc.NewPage();
            for (int i = 0; i < Math.Ceiling(reports.Count() / 6d); i++) 
            {
                page1(Doc, reports, engine, pool,piging, i);
                Doc.NewPage();
            }
            page2(Doc, reports.FirstOrDefault(), piging.ToList(), engine.ToList(), pool.ToList());
            Doc.NewPage();
            page3(Doc, reports.FirstOrDefault(), piging);
            Doc.NewPage();
            page4(Doc, reports.FirstOrDefault(), piging);

            Doc.Close();
            byte[] content = Memory.ToArray();
            return content;
            //..
            //var fileName = Guid.NewGuid().ToString() + ".pdf";
            //var fileStream = new FileStream(filePath + fileName, FileMode.CreateNew, FileAccess.ReadWrite);
            //// Write out PDF from memory stream.
            //fileStream.Write(content, 0, (int)content.Length);
            //fileStream.Close();
            //Memory.Close();

            //return filePath + fileName;
        }

        public void cover(Document Doc, LiugongReport report,LiugongReport_Piging report_piging)
        {
            Doc.Add(new Paragraph("\n")); 
            string titlestring = string.Format("旱作灌溉推廣與發展計畫\n{0}年度旱作管路灌溉設施預算書", report.ApplyYear);
            Paragraph title = new Paragraph(new Phrase(titlestring, font0))
            {
                Alignment = Element.ALIGN_CENTER
            };
            Doc.Add(title);
            Doc.Add(new Paragraph("\n")); 
            string c1string = "申請案號：{0}" + Environment.NewLine;
            c1string = c1string + "農戶姓名：{1}" + Environment.NewLine;
            c1string = c1string + "住址：{2}" + Environment.NewLine;
            c1string = c1string + "地段：{3}{4}" + Environment.NewLine;
            c1string = c1string + "地號：{5}{6}" + Environment.NewLine;
            c1string = c1string + "設施面積：{7}公頃" + Environment.NewLine;
            //c1string = c1string + "設施形式：□穿孔管系統 □噴頭系統 □微噴系統 □滴灌系統" + Environment.NewLine;
            //c1string = c1string + "          □軟管澆灌" + Environment.NewLine;
            c1string = c1string + GetEndType(report_piging.EndTypeCode == null ? (byte)0 : report_piging.EndTypeCode.Value);
            c1string = c1string + "◆本戶為再次申請，其補助經費最高上限為70%(第一次申請年度：{8}年)" + Environment.NewLine;

            c1string = string.Format(c1string, report.IANum, report.Name, report.Addr, report.Section, report.Subsection, report.LandTypeCNS, report.LandNo, report.BuildArea/10000d, "");
            Doc.Add(new Phrase(c1string, font2));

            for (int i = 0; i < 10; i++)
            {
                Doc.Add(new Paragraph("\n")); 
            }

            
            PdfPTable t1 = new PdfPTable(4)
            {
                TotalWidth = 400f, 
                LockedWidth = true, 
                HorizontalAlignment = Element.ALIGN_CENTER 
            };
            t1.AddCell(new PdfPCell(new Phrase("主辦人員", font2)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, FixedHeight = 40f, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t1.AddCell(new PdfPCell(new Phrase("管理組長", font2)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t1.AddCell(new PdfPCell(new Phrase("祕書", font2)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t1.AddCell(new PdfPCell(new Phrase("總幹事", font2)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t1, 4, 40f);
            Doc.Add(t1);
            PdfPTable t2 = new PdfPTable(3)
            {
                TotalWidth = 400f, 
                LockedWidth = true, 
                HorizontalAlignment = Element.ALIGN_CENTER 
            };
            
            t2.AddCell(new PdfPCell(new Phrase("設計", font2)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, FixedHeight = 40f, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase("主計室", font2)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase("會長", font2)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 3, 40f);
            Doc.Add(t2);
        }

        public void page1(Document Doc, List<LiugongReport> reports, List<LiugongReport_Engine> engine, List<LiugongReport_Pool> pool,List<LiugongReport_Piging> piging, int indexPage)
        {
            var report = reports.FirstOrDefault();
                       
            Paragraph title = new Paragraph(new Phrase("台北市瑠公農田管理處", font0))
            {
                Alignment = Element.ALIGN_CENTER
            };
            Doc.Add(title);
            Doc.Add(new Paragraph("\n")); 
            
            PdfPTable Table1 = new PdfPTable(2)
            {
                TotalWidth = 150f, 
                LockedWidth = true, 
                HorizontalAlignment = Element.ALIGN_RIGHT 
            };
            
            Table1.AddCell(new PdfPCell(new Phrase("收件日期", font1)));
            Table1.AddCell(new PdfPCell(new Phrase(report.ApplyYear.ToString() + "年"+"　月　日", font1)));
            Table1.AddCell(new PdfPCell(new Phrase("申請案號", font1)));
            Table1.AddCell(new PdfPCell(new Phrase(report.IANum.ToString(), font1)));
            Doc.Add(Table1);
            Doc.Add(new Paragraph("\n"));
            Doc.Add(new Paragraph(new Phrase("推廣旱作管路灌溉計畫設施補助申請及勘查核定表", font2)) { Alignment = Element.ALIGN_CENTER });

            string c2string = "    申請人願依照台北市瑠公農田管理處核定「推廣旱作管路灌溉執行作業要點」訂定之內容，填具下列資料，並依規定檢核資料向  貴單位申請並供排定優先順序，如無法列入補助對象時，絕無異議。";
            Doc.Add(new Paragraph(new Phrase(c2string, font1)));

            string c3string = "    此    致\n    申請人： {0} 簽章：       身分證字號： {1} 聯絡電話： {2}\n    地址： {3}";
            c3string = string.Format(c3string, report.Name, report.IdNo, report.Phone, report.Addr);
            Doc.Add(new Paragraph(new Phrase(c3string, font1)));
            Doc.Add(new Paragraph("\n"));

            
            PdfPTable Table2 = new PdfPTable(2)
            {
                TotalWidth = 525f, 
                LockedWidth = true, 
            };
            Table2.AddCell(new PdfPCell(new Phrase("施設地區：" + report.Addr.Substring(0, 7), font1)));
            string c4string = "面積合計： {0}筆  {1} Ha 施設面積： {2} Ha";
            c4string = string.Format(c4string, reports.Count(), (reports.Sum(p => p.FarmArea)/10000d).ToString(), (reports.Sum(p => p.BuildArea)/10000d).ToString());
            Table2.AddCell(new PdfPCell(new Phrase(c4string, font1)));
            Doc.Add(Table2);


            
            PdfPTable Table3 = new PdfPTable(7)
            {
                TotalWidth = 525f, 
                LockedWidth = true, 
            };
            string[] t3title = new string[] { "地段", "地目", "地號", "該筆面積", "施設面積", "作物別" };
            List<string[]> t3List = new List<string[]>();
            List<bool> t4List = new List<bool>();
            for (int i = indexPage * 6; i < (indexPage * 6) + 6; i++)
            {
                if (i == reports.Count())
                {
                    break;
                }
                else
                {
                    string[] t3data = new string[] { reports[i].Section + "\n" + reports[i].Subsection, reports[i].LandTypeCNS, reports[i].LandNo, (reports[i].FarmArea/10000d).ToString(), (reports[i].BuildArea/10000d).ToString(), "" };
                    t3List.Add(t3data);
                    t4List.Add(reports[i].Outside);
                }
            }

            for (int i = 0; i < 48; i++)
            {
                string t3cellstring = string.Empty;
                if (i == 0 || i == 7 || i == 14 || i == 21 || i == 28 || i == 35)
                {
                    t3cellstring = t3title[i / 7].ToString();
                }
                else if (i == 1 || i == 8 || i == 15 || i == 22 || i == 29 || i == 36)
                {
                    if (t3List.Count() > 0)
                    {
                        t3cellstring = t3List[0][i / 7].ToString();
                    }
                }
                else if (i == 2 || i == 9 || i == 16 || i == 23 || i == 30 || i == 37)
                {
                    if (t3List.Count() > 1)
                    {
                        t3cellstring = t3List[1][i / 7].ToString();
                    }
                }
                else if (i == 3 || i == 10 || i == 17 || i == 24 || i == 31 || i == 38)
                {
                    if (t3List.Count() > 2)
                    {
                        t3cellstring = t3List[2][i / 7].ToString();
                    }
                }
                else if (i == 4 || i == 11 || i == 18 || i == 25 || i == 32 || i == 39)
                {
                    if (t3List.Count() > 3)
                    {
                        t3cellstring = t3List[3][i / 7].ToString();
                    }
                }
                else if (i == 5 || i == 12 || i == 19 || i == 26 || i == 33 || i == 40)
                {
                    if (t3List.Count() > 4)
                    {
                        t3cellstring = t3List[4][i / 7].ToString();
                    }
                }
                else if (i == 6 || i == 13 || i == 20 || i == 27 || i == 34 || i == 41)
                {
                    if (t3List.Count() > 5)
                    {
                        t3cellstring = t3List[5][i / 7].ToString();
                    }
                }

                PdfPCell t3cell = new PdfPCell(new Phrase(t3cellstring, font1))
                {
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE
                };
                Table3.AddCell(t3cell);
            }
            Doc.Add(Table3);

            
            PdfPTable Table4 = new PdfPTable(new float[] { 75f, 45f, 30f, 45f, 30f, 45f, 30f, 45f, 30f, 45f, 30f, 45f, 30f })
            {
                TotalWidth = 525f, 
                LockedWidth = true, 
            };
            PdfPCell t4Cell1 = new PdfPCell(new Phrase("施設區域\n(請打V)", font1))
            {
                Rowspan = 2,
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthTop = 1f,
                BorderWidthBottom = 1f,
                BorderWidthLeft = 1f,
                BorderWidthRight = 0f
            };
            Table4.AddCell(t4Cell1);
            for (int i = 0; i < 6; i++) 
            {
                PdfPCell t4Cell2 = new PdfPCell(new Phrase("會員區", font1))
                {
                    BorderWidthTop = 1f,
                    BorderWidthBottom = 0f,
                    BorderWidthRight = 0f,
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER
                };
                string t4string = string.Empty;
                if (t4List.Count() > i && t4List[i])
                {
                    t4string = "V";
                }
                PdfPCell t4Cell3 = new PdfPCell(new Phrase(t4string, font1))
                {
                    BorderWidthTop = 1f,
                    BorderWidthBottom = 0f,
                    BorderWidthRight = (i == 5) ? 1f : 0f,
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER
                };
                Table4.AddCell(t4Cell2);
                Table4.AddCell(t4Cell3);
            }
            for (int i = 0; i < 6; i++) 
            {
                PdfPCell t4Cell4 = new PdfPCell(new Phrase("事業區", font1))
                {
                    BorderWidthBottom = 1f,
                    BorderWidthRight = 0f,
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER
                };
                string t4string = string.Empty;
                if (t4List.Count() > i && !t4List[i])
                {
                    t4string = "V";
                }
                PdfPCell t4Cell5 = new PdfPCell(new Phrase(t4string, font1))
                {
                    BorderWidthBottom = 1f,
                    BorderWidthRight = (i == 5) ? 1f : 0f,
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER
                };
                Table4.AddCell(t4Cell4);
                Table4.AddCell(t4Cell5);
            }
            Doc.Add(Table4);

            
            PdfPTable Table5 = new PdfPTable(new float[] { 75f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f })
            {
                TotalWidth = 525f, 
                LockedWidth = true, 
            };
            PdfPCell t5cellTitle = new PdfPCell(new Phrase("申\n請\n補\n助\n項\n目", font1))
            {
                HorizontalAlignment = PdfPCell.ALIGN_CENTER
            };
            Table5.AddCell(t5cellTitle);

            string[] t5items = new string[] { "馬\n達\n+\n抽\n水\n機", "馬\n達\n+\n柱\n塞\n泵", "汽\n油\n引\n擎", "柴\n油\n引\n擎", "蓄\n水\n槽", "引\n水\n管\n路", "微\n噴\n系\n統", "噴\n灌\n系\n統", "滴\n灌\n系\n統", "穿\n孔\n管\n系\n統", "軟\n管\n系\n統", "", "", "", "" };
            for (int i = 0; i < 15; i++)
            {
                PdfPCell t5Cells = new PdfPCell(new Phrase(t5items[i], font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER };
                Table5.AddCell(t5Cells);
            }
            Doc.Add(Table5);

            
            PdfPTable Table6 = new PdfPTable(new float[] { 75f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f })
            {
                TotalWidth = 525f, 
                LockedWidth = true, 
            };
            PdfPCell t6cellTitle = new PdfPCell(new Phrase("請打V", font1))
            {
                HorizontalAlignment = PdfPCell.ALIGN_CENTER
            };
            Table6.AddCell(t6cellTitle);


            for (int i = 0; i < 15; i++)
            {
                string code = t5items[i].Replace("\n", "");
                int counts = engine.Where(p => p.EngCNS == code).Count();
                string t6string = counts > 0 ? "V" : "";
                switch (i)
                {
                    case 4:
                        t6string = pool.Count() > 0 ? "V" : "";
                        break;
                    case 6:
                        t6string = piging.Where(p => p.EndTypeCode == 3).Count() > 0 ? "V" : "";
                        break;
                    case 7:
                        t6string = piging.Where(p => p.EndTypeCode == 2 || p.EndTypeCode == 6).Count() > 0 ? "V" : "";
                        break;
                    case 8:
                        t6string = piging.Where(p => p.EndTypeCode == 4 || p.EndTypeCode == 7 || p.EndTypeCode == 8).Count() > 0 ? "V" : "";
                        break;
                    case 9:
                        t6string = piging.Where(p => p.EndTypeCode == 1).Count() > 0 ? "V" : "";
                        break;
                    case 10:
                        t6string = piging.Where(p => p.EndTypeCode == 5).Count() > 0 ? "V" : "";
                        break;
                }
                PdfPCell t6cells = new PdfPCell(new Phrase(t6string, font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER };
                Table6.AddCell(t6cells);
            }
            Doc.Add(Table6);

            
            PdfPTable Table7 = new PdfPTable(new float[] { 75f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f, 30f })
            {
                TotalWidth = 525f, 
                LockedWidth = true, 
            };
            PdfPCell t7Cell1 = new PdfPCell(new Phrase("核定情形", font1))
            {
                HorizontalAlignment = PdfPCell.ALIGN_CENTER
            };
            Table7.AddCell(t7Cell1);
            for (int i = 0; i < 15; i++)
            {
                string code = t5items[i].Replace("\n", "");
                int counts = engine.Where(p => p.EngCNS == code).Count();
                string t7string = counts > 0 ? "V" : "";

                switch(i)
                {
                    case 4:
                        t7string = pool.Count() > 0 ? "V" : "";
                        break;
                    case 6:
                        t7string = piging.Where(p => p.EndTypeCode == 3).Count() > 0 ? "V" : "";
                        break;
                    case 7:
                        t7string = piging.Where(p => p.EndTypeCode == 2 || p.EndTypeCode == 6).Count() > 0 ? "V" : "";
                        break;
                    case 8:
                        t7string = piging.Where(p => p.EndTypeCode == 4 || p.EndTypeCode == 7 || p.EndTypeCode == 8).Count() > 0 ? "V" : "";
                        break;
                    case 9:
                        t7string = piging.Where(p => p.EndTypeCode == 1).Count() > 0 ? "V" : "";
                        break;
                    case 10:
                        t7string = piging.Where(p => p.EndTypeCode == 5).Count() > 0 ? "V" : "";
                        break;
                }
                PdfPCell t7cells = new PdfPCell(new Phrase(t7string, font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER };
                Table7.AddCell(t7cells);
            }
            Doc.Add(Table7);

            
            PdfPTable Table8 = new PdfPTable(new float[] { 75f, 450f })
            {
                TotalWidth = 525f, 
                LockedWidth = true, 
            };
            string t8cell1string = "勘\n查\n結\n果\n說\n明";
            PdfPCell t8Cell1 = new PdfPCell(new Phrase(t8cell1string, font1))
            {
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE
            };
            Table8.AddCell(t8Cell1);

            //string t8string = "1.灌溉水源：□灌排渠道 □山溪溝 □埤(池)塘 □地下水(含淺水井) □其它：自來水" + Environment.NewLine;
            string t8string = GetWaterSource(piging == null ? (byte)0 : piging.First().IrrWCode.Value) + Environment.NewLine;
            t8string = t8string + "2.動力設備馬力數__HP、" + engine.Count() + "台" + Environment.NewLine;
            t8string = t8string + "3.蓄水槽： " + (pool.Count() == 0 ? "" : pool.First().PoolWeight.ToString()) + " 噸 " + pool.Count() + " 座    " + GetPool(pool.Count() == 0 ? (byte)0 : pool.First().PtypeCode) + Environment.NewLine;
            t8string = t8string + "4.田間配置：(1).輸水管路(L1)長 " + piging.FirstOrDefault().L1 + " 公尺、管徑  吋，(L2)：長 " + piging.FirstOrDefault().L2 + " 公尺、管徑  吋" + Environment.NewLine;
            t8string = t8string + "            (2).間行距：支管行距(SL) " + piging.FirstOrDefault().SL + " 公尺，噴頭間距(SS) " + piging.FirstOrDefault().SS + " 公尺。" + Environment.NewLine;
            t8string = t8string + "            (3).施設方式：" + GetFacType(piging.FirstOrDefault().FacType.Value) + Environment.NewLine;
            t8string = t8string + "            (4).支管選擇：□全長不變徑 □ 1/3管長變徑" + Environment.NewLine;
            t8string = t8string + "5.會勘意見：" + Environment.NewLine + report.Reason;
            PdfPCell t8Cell2 = new PdfPCell(new Phrase(t8string, font1));
            Table8.AddCell(t8Cell2);
            Doc.Add(Table8);
            DateTime Edate = report.EDate; //勘察日期
            string c5string = "    備註：1.申請資格：非都市土地或都市土地農業區、保護區內，依法供農作使用之土地。" + Environment.NewLine;
            c5string = c5string + "          2.將申請施設之土地，逐筆地段、地目、地號、面積及作物別填入空欄內。" + Environment.NewLine;
            c5string = c5string + "          3.檢附證件：申請人身份證正反面影本、土地登記謄本及地籍圖謄本(承租私有地者，" + Environment.NewLine;
            c5string = c5string + "            承租人應檢附土地施設同意書，承租國有地者，應檢附租賃契約影本)。" + Environment.NewLine;
            c5string = c5string + "    勘查人員：              申請人簽名或蓋章：        勘查日期 " + (Edate.Year - 1911).ToString() + " 年 " + Edate.Month + " 月 " + Edate.Day + " 日" + Environment.NewLine;
            Doc.Add(new Paragraph(new Phrase(c5string, font1)));
        }

        public void page2(Document Doc, LiugongReport report, List<LiugongReport_Piging> piging, List<LiugongReport_Engine> engine, List<LiugongReport_Pool> pool)
        {
            int BlankCount = 208; 
                          
            Paragraph title = new Paragraph(new Phrase("台北市瑠公農田管理處\n旱作管路灌溉設施預算書", font0))
            {
                Alignment = Element.ALIGN_CENTER
            };
            Doc.Add(title);
            Doc.Add(new Paragraph("\n")); 

            
            Paragraph sub_title = new Paragraph(new Phrase("共 2 頁 第 1 頁", font1))
            {
                Alignment = Element.ALIGN_RIGHT
            };
            Doc.Add(sub_title);

            
            PdfPTable t1 = new PdfPTable(2)
            {
                TotalWidth = 525f,
                LockedWidth = true
            };
            string t1string = "施設地區： {0}{1}";
            t1string = string.Format(t1string, report.LandTypeCNS, report.LandNo);
            t1.AddCell(new PdfPCell(new Phrase(t1string, font1)) { Border = PdfPCell.NO_BORDER });
            t1.AddCell(new PdfPCell(new Phrase((DateTime.Now.Year-1911).ToString() + "年"+DateTime.Now.Month+"月"+DateTime.Now.Day+"日", font1)) { Border = PdfPCell.NO_BORDER, HorizontalAlignment = PdfPCell.ALIGN_RIGHT });
            Doc.Add(t1);

            
            PdfPTable t2 = new PdfPTable(new float[] { 60f, 90f, 80f, 30f, 60f, 60f, 60f, 85f })
            {
                TotalWidth = 525f,
                LockedWidth = true
            };
            string[] t2title = new string[] { "設施項目", "說明", "單位", "數量", "單價", "總價", "附註" };
            for (int i = 0; i < t2title.Length; i++)
            {
                t2.AddCell(new PdfPCell(new Phrase(t2title[i], font1)) { Colspan = (i == 0) ? 2 : 1, HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            }
            
            t2.AddCell(new PdfPCell(new Phrase("A.田間管路灌溉設施費", font1)) { Colspan = 2 });
            blank(t2, 6);
            t2.AddCell(new PdfPCell(new Phrase("  (1)田間管路材料費", font1)) { Colspan = 2 });
            blank(t2, 1);
            t2.AddCell(new PdfPCell(new Phrase("全", font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
            t2.AddCell(new PdfPCell(new Phrase(piging.Count > 0 ? "1" : "", fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
            blank(t2, 1);
            t2.AddCell(new PdfPCell(new Phrase(piging.Count > 0 ? String.Format("{0:N0}", piging.Sum(m => m.Total)) : "", fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT });
            blank(t2, 1);

            
            var engList = engine.GroupBy(m => m.EngCNS)
                            .Select(g => new
                            {
                                cns = g.First().EngCNS,
                                count = g.Count()
                            });
            string engName = "";
            int engCount = 0;
            int index = 0;
            foreach (var item in engList)
            {
                engName += item.cns + "*" + item.count.ToString();
                if (index < engList.Count())
                    engName += "\n";
                if (index > 1) 
                    BlankCount--;
                index++;
                engCount += item.count;
            }
            t2.AddCell(new PdfPCell(new Phrase("  (2)動力設備", font1)) { Colspan = 2, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase(engName, font1)));
            t2.AddCell(new PdfPCell(new Phrase("組", font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase(engCount.ToString(), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 1);
            t2.AddCell(new PdfPCell(new Phrase(engine.Count <= 0 ? "" : String.Format("{0:N0}", engine.Sum(m => m.EngPrice)), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase(engine.Count <= 0 ? "" : "含組裝及零件工料", fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });

            
            string poolName = "";
            int poolCount = 0;
            index = 0;
            var poolList = pool.GroupBy(m => m.PtypeCNS)
                .Select(g => new
                {
                    cns = g.First().PtypeCNS,
                    count = g.Count()
                });
            foreach (var item in poolList)
            {
                poolName += item.cns + "*" + item.count.ToString();
                if (index < poolList.Count())
                    poolName += "\n";
                if (index > 1) 
                    BlankCount--;
                index++;
                poolCount += item.count;
            }
            t2.AddCell(new PdfPCell(new Phrase("  (3)調蓄設施", font1)) { Colspan = 2, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase(poolName, font1)) { VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase("座", font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase(poolCount <= 0 ? "" : poolCount.ToString(), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 1);
            t2.AddCell(new PdfPCell(new Phrase(poolCount <= 0 ? "" : poolCount.ToString(), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase(poolCount <= 0 ? "" : "含組裝及零件工料", font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });

            t2.AddCell(new PdfPCell(new Phrase("B.工作費", font1)) { Colspan = 2 });
            blank(t2, 1);
            t2.AddCell(new PdfPCell(new Phrase("ha", fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
            t2.AddCell(new PdfPCell(new Phrase(piging.Count <= 0 ? "" : ((double)report.BuildArea / 10000).ToString(), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
            t2.AddCell(new PdfPCell(new Phrase(piging.Count <= 0 ? "" : "48,000", fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT });
            t2.AddCell(new PdfPCell(new Phrase(piging.Count <= 0 ? "" : String.Format("{0:N0}", piging.FirstOrDefault().WorkPrice), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT });
            blank(t2, 1);

            t2.AddCell(new PdfPCell(new Phrase("C.包商管理費(A+B)×    %", font1)) { Colspan = 2 });
            blank(t2, 1);
            t2.AddCell(new PdfPCell(new Phrase("全", font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase("－", font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase("－", font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase("－", font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase("▇自行施工\n□委外施工", font1)) { HorizontalAlignment = PdfPCell.ALIGN_LEFT });

            t2.AddCell(new PdfPCell(new Phrase("D.設施費總計(A+B+C)", font1)) { Colspan = 2 });
            blank(t2, 4);
            t2.AddCell(new PdfPCell(new Phrase(String.Format("{0:N0}", report.Total), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 1);

            t2.AddCell(new PdfPCell(new Phrase("E.設施費\n分擔", font1)) { Rowspan = 3, HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase("農戶配合款", font1)));
            t2.AddCell(new PdfPCell(new Phrase("≧(A+B)×30%", font1)));
            blank(t2, 3);
            t2.AddCell(new PdfPCell(new Phrase(String.Format("{0:N0}", report == null ? 0 : report.FarmerFee), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 1);

            t2.AddCell(new PdfPCell(new Phrase("管理處補助款", font1)));
            t2.AddCell(new PdfPCell(new Phrase("(A+B)×70%+C", font1)));
            blank(t2, 3);
            t2.AddCell(new PdfPCell(new Phrase(String.Format("{0:N0}", report.GovSubsidy), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 1);

            t2.AddCell(new PdfPCell(new Phrase("小計", font1)));
            blank(t2, 4);
            t2.AddCell(new PdfPCell(new Phrase(String.Format("{0:N0}", report == null ? 0 : report.Total), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 1);

            
            blank(t2, BlankCount);

            t2.AddCell(new PdfPCell(new Phrase("本設施預算總計", font1)) { Colspan = 2, HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            t2.AddCell(new PdfPCell(new Phrase(String.Format("新台幣 {0}元整", new NumberToChinese().GetChineseNumber(report == null ? 0 : report.Total.Value)), font1)) { Colspan = 6, HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            Doc.Add(t2);
        }

        public void page3(Document Doc, LiugongReport report, List<LiugongReport_Piging> piging)
        {
                     
            Paragraph title = new Paragraph(new Phrase("台北市瑠公農田管理處\n工程預算書", font0))
            {
                Alignment = Element.ALIGN_CENTER
            };
            Doc.Add(title);
            Doc.Add(new Paragraph("\n")); 

            
            Paragraph sub_title = new Paragraph(new Phrase("共 2 頁 第 2 頁", font1))
            {
                Alignment = Element.ALIGN_RIGHT
            };
            Doc.Add(sub_title);

            
            PdfPTable t1 = new PdfPTable(2)
            {
                TotalWidth = 525f,
                LockedWidth = true
            };
            t1.AddCell(new PdfPCell(new Phrase("工程名稱： 旱作管路灌溉設施工程", font1)) { Border = PdfPCell.NO_BORDER });
            t1.AddCell(new PdfPCell(new Phrase((DateTime.Now.Year - 1911).ToString() + "年" + DateTime.Now.Month.ToString() + "月" + DateTime.Now.Day.ToString() + "日", font1)) { Border = PdfPCell.NO_BORDER, HorizontalAlignment = PdfPCell.ALIGN_RIGHT });
            Doc.Add(t1);
            
            PdfPTable t2 = new PdfPTable(new float[] { 150f, 80f, 30f, 60f, 60f, 60f, 85f })
            {
                TotalWidth = 525f,
                LockedWidth = true
            };
            string[] t2title = new string[] { "工程項目", "說明", "單位", "數量", "單價", "總價", "附註" };
            for (int i = 0; i < t2title.Length; i++)
            {
                t2.AddCell(new PdfPCell(new Phrase(t2title[i], font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            }
            string t2string = "一、田間管路設施費(地號：{0}{1})";
            t2string = string.Format(t2string, report.LandTypeCNS, report.LandNo);
            t2.AddCell(new PdfPCell(new Phrase(t2string, font1)) { Colspan = 7 });
            
            for (int i = 0; i < piging.Count(); i++)
            {
                t2.AddCell(new PdfPCell(new Phrase(piging[i].MAT.ToString(), font1)));
                blank(t2, 1);
                t2.AddCell(new PdfPCell(new Phrase(String.Format("{0:N0}", piging[i].ItemUnit), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
                t2.AddCell(new PdfPCell(new Phrase(String.Format("{0:N0}", piging[i].Amount), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
                t2.AddCell(new PdfPCell(new Phrase(String.Format("{0:N0}", piging[i].SysPrice), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
                t2.AddCell(new PdfPCell(new Phrase(String.Format("{0:N0}", piging[i].Total), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
                t2.AddCell(new PdfPCell(new Phrase(piging[i].Note, font1)) { HorizontalAlignment = PdfPCell.CCITT_BLACKIS1, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            }
            
            t2.AddCell(new PdfPCell(new Phrase("小計", font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 4);
            t2.AddCell(new PdfPCell(new Phrase(String.Format("{0:N0}", piging.Sum(p => p.Total)), font1)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 1);
            
            int blankAmount = (35 - piging.Count()) * 7;
            blank(t2, blankAmount);
            
            t2.AddCell(new PdfPCell(new Phrase("本工程預算總計", font1)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 4);
            t2.AddCell(new PdfPCell(new Phrase(String.Format("{0:N0}", piging.Sum(p => p.Total)), fontNum)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT, VerticalAlignment = PdfPCell.ALIGN_MIDDLE });
            blank(t2, 1);
            Doc.Add(t2);
        }

        public void page4(Document Doc, LiugongReport report,List<LiugongReport_Piging>piging)
        {
            string c1string = @"申請編號：{0}    設施地號：{1}" + Environment.NewLine;
            c1string += "設施形式：{2}微噴系統  {3}噴灌系統  {4}軟管澆灌  {5}滴灌系統" + Environment.NewLine;
            c1string += "田區主管：長度 {6}公尺，管徑 {7}吋" + Environment.NewLine;
            c1string += "噴頭間距：行距 {8}公尺，間距 {9}公尺" + Environment.NewLine;

            string[] args = new string[10];
            args[0] = report.IANum.ToString();
            args[1] = report.LandTypeCNS + report.LandNo;
            args[2] = "□";
            args[3] = "□";
            args[4] = "□";
            args[5] = "■";
            args[6] = piging.FirstOrDefault().L1.ToString();
            args[7] = "";
            args[8] = piging.FirstOrDefault().SL.ToString();
            args[9] = piging.FirstOrDefault().SS.ToString();

            c1string = string.Format(c1string, args);
            Paragraph c1 = new Paragraph(new Phrase(c1string, font2));
            Doc.Add(c1);
            Doc.Add(new Paragraph("\n")); 

            
            string ServerPath;
            string FolderPath;
            string LocalPath;
            XmlDocument data = new XmlDocument();
            data.Load(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/FilePath.xml"));
            LocalPath = data.SelectSingleNode("Path/LocalPath").Attributes["Value"].Value;
            ServerPath = data.SelectSingleNode("Path/ServerPath").Attributes["Value"].Value;
            FolderPath = data.SelectSingleNode("Path/Folder").Attributes["Value"].Value;
            string filePath = LocalPath + @"\" + FolderPath + @"\" + report.MapNo.ToString() + @"\" + report.FileType + @"\" + report.Filepath;
            if(File.Exists(filePath))
            {
                Image image = Image.GetInstance(filePath);
                Doc.Add(image);
            }
            
        }

        #region common function
        public PdfPTable blank(PdfPTable t, int length)
        {
            for (int i = 0; i < length; i++)
            {
                t.AddCell(new PdfPCell(new Phrase(" ")));
            }
            return t;
        }

        public PdfPTable blank(PdfPTable t, int length, float height)
        {
            for (int i = 0; i < length; i++)
            {
                t.AddCell(new PdfPCell(new Phrase(" "))
                {
                    FixedHeight = height
                });
            }
            return t;
        }
        private string GetEndType(byte EndTypeCode)
        {
            switch(EndTypeCode)
            {
                case 1:
                    return "設施形式：▇穿孔管系統 □噴頭系統 □微噴系統 □滴灌系統" + Environment.NewLine +
                           "          □軟管澆灌" + Environment.NewLine;
                case 2:
                    return "設施形式：□穿孔管系統 ▇噴頭系統 □微噴系統 □滴灌系統" + Environment.NewLine +
                           "          □軟管澆灌" + Environment.NewLine;
                case 3:
                    return "設施形式：□穿孔管系統 □噴頭系統 ▇微噴系統 □滴灌系統" + Environment.NewLine +
                           "          □軟管澆灌" + Environment.NewLine;
                case 4:
                    return "設施形式：□穿孔管系統 □噴頭系統 □微噴系統 ▇滴灌系統" + Environment.NewLine +
                           "          □軟管澆灌" + Environment.NewLine;
                case 5:
                    return "設施形式：□穿孔管系統 □噴頭系統 □微噴系統 ▇滴灌系統" + Environment.NewLine +
                           "          □軟管澆灌" + Environment.NewLine;
                default:
                    return "設施形式：□穿孔管系統 □噴頭系統 □微噴系統 □滴灌系統" + Environment.NewLine +
                           "          □軟管澆灌" + Environment.NewLine;
            }
        }
        private string GetWaterSource(byte IrrWCode)
        {
            switch(IrrWCode)
            {
                case 1:
                    return "1.灌溉水源：▇灌排渠道 □山溪溝 □埤(池)塘 □地下水(含淺水井) □其它：";
                case 2:
                    return "1.灌溉水源：□灌排渠道 ▇山溪溝 □埤(池)塘 □地下水(含淺水井) □其它：";
                case 3:
                    return "1.灌溉水源：□灌排渠道 □山溪溝 ▇埤(池)塘 □地下水(含淺水井) □其它：";
                case 4:
                    return "1.灌溉水源：□灌排渠道 □山溪溝 □埤(池)塘 ▇地下水(含淺水井) □其它：";
                case 5:
                    return "1.灌溉水源：□灌排渠道 □山溪溝 □埤(池)塘 □地下水(含淺水井) ▇其它：";
                default:
                    return "1.灌溉水源：□灌排渠道 □山溪溝 □埤(池)塘 □地下水(含淺水井) □其它：";
            }
        }
        private string GetPool(byte PtypeCode)
        {
            switch(PtypeCode)
            {
                case 1:
                    return "材質：▇RC □磚造 □塑膠類 □鋁合金 □不鏽鋼 □塑鋼";
                case 2:
                    return "材質：□RC ▇磚造 □塑膠類 □鋁合金 □不鏽鋼 □塑鋼";
                case 3:
                    return "材質：□RC □磚造 ▇塑膠類 □鋁合金 □不鏽鋼 □塑鋼";
                case 4:
                    return "材質：□RC □磚造 □塑膠類 ▇鋁合金 □不鏽鋼 □塑鋼";
                case 5:
                    return "材質：□RC □磚造 □塑膠類 □鋁合金 ▇不鏽鋼 □塑鋼";
                case 6:
                    return "材質：□RC □磚造 □塑膠類 □鋁合金 □不鏽鋼 ▇塑鋼";
                default:
                    return "材質：□RC □磚造 □塑膠類 □鋁合金 □不鏽鋼 □塑鋼";
            }
        }
        private string GetFacType(byte FacType)
        {
            switch(FacType)
            {
                case 1:
                    return "▇埋設固定式 □地表定置式 □附掛棚架式 □其它：";
                case 2:
                    return "□埋設固定式 ▇地表定置式 □附掛棚架式 □其它：";
                case 3:
                    return "□埋設固定式 □地表定置式 ▇附掛棚架式 □其它：";
                default:
                    return "□埋設固定式 □地表定置式 □附掛棚架式 □其它：";
            }
        }
        #endregion
    }
}
