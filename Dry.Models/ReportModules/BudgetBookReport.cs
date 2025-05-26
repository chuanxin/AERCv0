using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using Dry.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using Dry.Models.CommonCls;
using System.Runtime.InteropServices;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace Dry.Models.ReportModules
{
    public class BudgetBookReport
    {
        #region Parameter
        private int _MNo;
        private BudgetBookDBService db = new BudgetBookDBService();
        private SubsidyReportDBService SRdb = new SubsidyReportDBService();
        private FarmLandDBService farmlanddb = new FarmLandDBService();
        private GetData getDataCls = new GetData();
        private DryEntities DryDb = new DryEntities();
        #endregion

        public byte[] ExportExcel(int mapno)
        {
            _MNo = mapno;
            BudgetBookView Data = db.GetBudgetBookData(_MNo);
            Int16 unit = Data.ApplyUnit;
            

            string sample_Path = @"~/ReportSample/BudgetBook_Sample.xlsx";
            string FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.xlsx";
            switch (unit)
            {
                case 4:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_4.xlsx";
                    break;
                case 5:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_5.xlsx";
                    break;
                case 6:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_6.xlsx";
                    break;
                case 7:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_7.xlsx";
                    break;
                case 8:
                    //sample_Path = @"~/ReportSample/BudgetBook_Sample_8.xlsx";
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_8_VB.xls";
                    //sample_Path = @"~/ReportSample/sample.xls";
                    FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.xls";
                    break;
                case 9:
                    sample_Path = Data.Gold == true ? @"~/ReportSample/Gold_BudgetBook_Sample_9.xlsx" : @"~/ReportSample/BudgetBook_Sample_9.xlsx";
                    break;
                case 10:
                    sample_Path = Data.Gold == true ? @"~/ReportSample/Gold_BudgetBook_Sample_10.xlsx" : @"~/ReportSample/BudgetBook_Sample_10_VB.xls";
                    if (Data.Gold == true)
                    {
                        FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.xlsx";
                    }
                    else
                        FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.xls";

                    break;
                case 11:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_11.xlsx";
                    break;
                
                case 12:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_12.xlsx";
                    break;
                case 13:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_13.xlsx";
                    break;
                case 14:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_14.xlsx";
                    break;
                case 15:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_15.xlsx";
                    break;
                case 17:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_17.xlsx";
                    break;
            }
            

            FileStream fs = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);

            ExcelPackage excel = new ExcelPackage(fs);
            List<string> sheetAry = new List<string>();
            _MNo = mapno;
            if (unit == 17)
            {
                ExcelWorksheet sheet = excel.Workbook.Worksheets["工程預算書"];
                sheet = SetLiubudgetBookTable(sheet, Data);
                if (db.GetFarMat(_MNo) != null)
                {
                    sheet = SetLiuMatTable(sheet, db.GetFarMat(_MNo));
                }
            }
            else 
            {
                if (Data.Gold)
                {
                    sheetAry.AddRange(new string[] { "黃金廊道", "材料數量表" });
                    ExcelWorksheet sheet = excel.Workbook.Worksheets[sheetAry[0]];
                    sheet = BasicTable(sheet, Data);
                    sheet = SetbudgetBookTableGold(sheet, Data, _MNo);
                    sheet = SetCoaMatTable(sheet, Data, db.GetFarMat(_MNo), unit);

                }
                else
                {
                    sheetAry.AddRange(new string[] { "工程預算書", "材料數量表" });
                    foreach (string sheetname in sheetAry)
                    {
                        
                        ExcelWorksheet sheet = excel.Workbook.Worksheets[sheetname];
                        sheet = BasicTable(sheet, Data);
                        sheet = SetbudgetBookTable(sheet, Data, _MNo);
                        sheet = SetCoaMatTable(sheet, Data, db.GetFarMat(_MNo), unit);
                        break;
                    }
                }
            }
            fs.Close();

            byte[] file = excel.GetAsByteArray();
            //return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
            return file;
        }
        #region ExportExcelBatch
        public byte[] ExportExcelBatch(int unitId,int year,int startId,int endId)
        {
           
            //string FileName = year.ToString() + " - " + new DateTime().ToShortDateString() + " - 工程預算書.xlsx";
            string sample_Path = @"~/ReportSample/BudgetBook_Sample.xlsx"; ; 
            switch (unitId)
            {
                case 4:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_4.xlsx";
                    break;
                case 5:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_5.xlsx";
                    break;
                case 6:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_6.xlsx";
                    break;
                case 7:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_7.xlsx";
                    break;
                case 8:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_8.xlsx";
                    
                    break;
                case 9:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_9.xlsx";
                    break;
                case 10:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_10.xlsx";
                    break;
                case 11:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_11.xlsx";
                    break;
                case 12:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_12.xlsx";
                    break;
                case 13:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_13.xlsx";
                    break;
                case 14:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_14.xlsx";
                    break;
                case 15:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_15.xlsx";
                    break;
                case 17:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_17.xlsx";
                    break;
                //case 19://澎湖
                //    sample_Path = @"~/ReportSample/BudgetBook_Sample_19.xls";
                //    break;
                case 22:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_22.xlsx";
                    break;
                case 23:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_23.xlsx";
                    break;
            }
            DryEntities dry = new DryEntities();
            var datalist = from db in dry.SummaryView
                           where db.ApplyYear == year && db.ApplyUnit == unitId && db.IANum >= startId && db.IANum <= endId
                           orderby db.IANum
                           select db;


            
            FileStream fs = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheetTemplate = excel.Workbook.Worksheets["工程預算書"];

            bool delsheetTemplate = false;
            foreach (var item in datalist)
            {
                BudgetBookView Data = db.GetBudgetBookData((int)item.MapNo);
                List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData((int)item.MapNo);
                ExcelWorksheet sheet = excel.Workbook.Worksheets.Add(item.IANum.ToString(), sheetTemplate);

                sheet = BasicTable(sheet, Data);
                sheet = SetbudgetBookTable(sheet, Data, (int)item.MapNo);
                sheet = SetCoaMatTable(sheet, Data, db.GetFarMat((int)item.MapNo), (short)unitId);
                delsheetTemplate = true;
            }
            if (delsheetTemplate) excel.Workbook.Worksheets.Delete(sheetTemplate);
            fs.Close();

            return excel.GetAsByteArray();

        }
        #endregion
        /// <summary>
        /// 預算書批次列印
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="year"></param>
        /// <param name="startId"></param>
        /// <param name="endId"></param>
        /// <returns></returns>
        public byte[] BudgetBookBatchPDF(int unitId, int year, int startId, int endId)
        {
            var datalist = from db in DryDb.SummaryView
                           where db.ApplyYear == year && db.ApplyUnit == unitId && db.IANum >= startId && db.IANum <= endId
                           orderby db.IANum
                           select new { db.MapNo,db.ApplyUnit };
            
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in datalist)
                {
                    BudgetBookView Data = db.GetBudgetBookData((int)item.MapNo);
                    document.NewPage();
                    wrirteCover(document, Data);
                    document.NewPage();
                    writeBaseInfo(document, Data);
                    writeBugetTable(document, Data);
                    writePayTable(document, Data);
                    if (item.ApplyUnit == 23)
                    {
                        writeSignupEmpty(document);
                    }else writeSignup(document);
                    document.NewPage();
                    writeLandsInfo(document, Data);
                    
                    if (Data.Engine != null || Data.Pool != null)
                    {
                        document.NewPage();
                        writeEngPool(document, Data, (int)item.MapNo);
                    }


                    document.NewPage();
                    List<GetData.SysMat> sysmat = db.GetFarMat((int)item.MapNo);
                    if (sysmat != null)
                    {
                        writeMaterialTable(document, Data, sysmat);
                    }
                    document.NewPage();
                    List<CntrlMat> cntrlmat = getDataCls.GetCntrlMatData((int)item.MapNo);
                    if (cntrlmat != null)
                    {
                        writeCtrlTable(document, Data, cntrlmat);
                    }
                    
                }

                document.Close();
                return stream.GetBuffer();
            }
        }

        /// <summary>
        /// 書面審查表批次列印
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="year"></param>
        /// <param name="startId"></param>
        /// <param name="endId"></param>
        /// <returns></returns>
        public byte[] CompleteCheckListBatchPDF(int unitId, int year, int startId, int endId)
        {
            var datalist = from db in DryDb.SummaryView
                           where db.ApplyYear == year && db.ApplyUnit == unitId && db.IANum >= startId && db.IANum <= endId
                           orderby db.IANum
                           select new { db.MapNo, db.IANum };
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in datalist)
                {
                    BudgetBookView Data = db.GetBudgetBookData((int)item.MapNo);
                    document.NewPage();
                    writeCompleteCheckList(document, item.IANum);
                }

                document.Close();
                return stream.GetBuffer();

            }
        }

        public byte[] PictureBatchPDF(int unitId, int year, int startId, int endId)
        {
            var datalist = from db in DryDb.SummaryView
                           where db.ApplyYear == year && db.ApplyUnit == unitId && db.IANum >= startId && db.IANum <= endId
                           orderby db.IANum
                           select new { db.MapNo, db.IANum };
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in datalist)
                {
                    BudgetBookView Data = db.GetBudgetBookData((int)item.MapNo);
                    document.NewPage();
                    writePictures(document, Data);
                }

                document.Close();
                return stream.GetBuffer();

            }
        }
        #region 基本資料表格

        #region 基本資料表格
        public ExcelWorksheet BasicTable(ExcelWorksheet sh, BudgetBookView Data)
        {
            int StartCol = 3;
            int EndCol = 4;

            int StartRow = 4;
            int EndRow = StartRow;

            sh.Cells[EndRow, EndCol].Value = Data.Name;

            sh.Cells[EndRow, EndCol + 3].Value = Data.IANum;
            //sh.Cells[EndRow, EndCol + 3].AutoFitColumns();

            EndRow++;
            sh.Cells[EndRow, EndCol].Value = Data.Address;

            EndRow++;
            string sectionMsg = Data.Farm.Count <= 0 ? "" : Data.Farm[0].full_sectName.ToString() + ", 地號: " + Data.Farm[0].LandNo + " ,等 " + Data.Farm.Count + "筆土地。";
            sh.Cells[EndRow, EndCol].Value = sectionMsg;

            EndRow++;
            sh.Cells[EndRow, EndCol].Value = ((double)Data.BuildArea / 10000) + "  公頃";

            EndRow++;
            if (Data.EndType == "")
            {
                sh.Cells[EndRow, EndCol].Value = "其它";
            }
            else
                sh.Cells[EndRow, EndCol].Value = Data.EndType;

            using (var range = sh.Cells[StartRow, StartCol, EndRow, EndCol])//fromRow, fromCol, toRow, toCol
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //range.AutoFitColumns();
                range.Style.WrapText = true;
            }

            return sh;
        }

        #endregion

        #region 設定瑠公設施項目表格
        public ExcelWorksheet SetLiubudgetBookTable(ExcelWorksheet sh, BudgetBookView Data)
        {
            string landnostr = string.Empty;
            foreach (var item in Data.Farm)
            {
                landnostr += db.GetLTypeName(item.LandType) + item.LandNo + "  ";
            }

            sh.Cells[4, 1].Value = "施設地號： " + landnostr;

            DateTime today = DateTime.Now;
            sh.Cells[4, 8].Value = string.Format("{0}年{1}月{2}日", today.Year - 1911, today.Month, today.Day);

            sh.Cells[7, 7].Value = Convert.ToInt32(Data.Liu_PipingMat.TotalPrice.Replace(",", ""));
            sh = SetLiuCellValue(sh, 8, Data.Liu_Engine);
            sh = SetLiuCellValue(sh, 9, Data.Liu_Pool);
            sh = SetLiuCellValue(sh, 10, Data.Liu_WorkM);

            sh.Cells[13, 7].Value = Convert.ToInt32(Data.Liu_FarmerMoney.Replace(",", ""));
            sh.Cells[14, 7].Value = Convert.ToInt32(Data.Liu_IAMoney.Replace(",", ""));
            sh.Cells[30, 3].Value = "新 台 幣  " + Data.Liu_ChineseMoney + "元整";

            sh.Cells[36, 8].Value = sh.Cells[4, 8].Value;
            sh.Cells[38, 1].Value = "一、田間管路設施費(地號：" + landnostr + ")";


            using (var range = sh.Cells[6, 6, 15, 7])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.WrapText = true;
                //range.AutoFitColumns();
            }
            return sh;
        }
        #endregion

        #region 設定瑠公材料數量表
        public ExcelWorksheet SetLiuMatTable(ExcelWorksheet sh, List<GetData.SysMat> matdata)
        {
            int row = 39;
            foreach (var mat in matdata)
            {
                sh.Cells[row, 1].Value = mat.matname;
                sh.Cells[row, 3].Value = mat.spec;
                sh.Cells[row, 4].Value = mat.itemunit;
                sh.Cells[row, 5].Value = mat.amount;
                sh.Cells[row, 6].Value = mat.matprice;
                sh.Cells[row, 7].Value = mat.amount * mat.matprice;
                sh.Cells[row, 8].Value = mat.description;
                row++;
            }
            /*
            using (var range = sh.Cells[39, 1, 59, 7])//fromRow, fromCol, toRow, toCol 11 8 25 8
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.WrapText = true;
            }*/
            return sh;
        }
        #endregion

        #region 材料數量表
        public ExcelWorksheet SetCoaMatTable(ExcelWorksheet sh, BudgetBookView data, List<GetData.SysMat> matdata, short unitid)
        {
            //int row = GetStartRow((short)Session["UnitID"], data.Gold);
            int row = GetStartRow(unitid, data.Gold);
            sh.Cells[row, 1].Value = "設施型式：" + data.EndType;
            sh.Cells[row + 1, 1].Value = "坵塊型狀：" + (data.Block.Length > 0 ? data.Block.Split('x')[0] + "m ×" + data.Block.Split('x')[1] + "m" : "");
            sh.Cells[row + 2, 1].Value = "噴頭配置間距(SL×SS)：" + data.SL + "×" + data.SS;
            sh.Cells[row + 2, 9].Value = data.IANum;
            row += 4;
            int ItemIndex = 1;
            if (matdata != null)
            {
                if (matdata.Any(m => m.moduleno == 0))
                {
                    foreach (var mat in matdata)
                    {
                        sh.Cells[row, 2].Value = mat.matname;
                        sh.Cells[row, 4].Value = mat.spec;
                        sh.Cells[row, 6].Value = mat.itemunit;
                        sh.Cells[row, 7].Value = mat.matprice;
                        sh.Cells[row, 8].Value = mat.amount;
                        sh.Cells[row, 9].Formula = "G" + row + "*" + "H" + row;

                        row++;
                    }
                    sh.Cells[row + 1, 1].Value = "總　價";
                    sh.Cells[row + 1, 9].Value = data.IrrSystem.TotalPrice;
                    sh.Cells[row + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }
                else
                {
                    foreach (var mat in matdata.GroupBy(m => m.groupno).Select(o => new { Grouop = o.Key }))
                    {

                        sh.Cells[row, 1].Value = ItemIndex + ". " + matdata.Where(m => m.groupno == mat.Grouop).FirstOrDefault().group;
                        sh.Cells[row, 2, row, 3].Merge = false;
                        sh.Cells[row, 4, row, 5].Merge = false;
                        sh.Cells[row, 1, row, 9].Merge = true;
                        row++;
                        foreach (var item in matdata.Where(m => m.groupno == mat.Grouop))
                        {
                            sh.Cells[row, 1].Value = ItemIndex + "-" + item.order;
                            sh.Cells[row, 2].Value = item.matname;
                            sh.Cells[row, 4].Value = item.spec;
                            sh.Cells[row, 6].Value = item.itemunit;
                            sh.Cells[row, 7].Value = item.matprice;
                            sh.Cells[row, 8].Value = item.amount;
                            sh.Cells[row, 9].Formula = "G" + row + "*" + "H" + row;
                            row++;
                        }
                        ItemIndex++;
                    }
                    sh.Cells[row + 1, 1].Value = "總　價";
                    sh.Cells[row + 1, 9].Value = data.IrrSystem.TotalPrice;
                    sh.Cells[row + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }
            }


            return sh;
        }

        #endregion
        public int GetStartRow(short unit, bool Gold)
        {
            switch (unit)
            {
                case 4:
                    return 38;
                case 6:
                    return 39;
                case 7:
                    return 37;
                case 8:
                    return 40;
                case 9:
                    if (Gold) { return 46; }
                    return 45;
                case 10:
                    if (Gold) { return 42; }
                    return 41;
                case 11:
                    return 37;
                case 12:
                    return 37;
                case 13:
                    return 39;
                case 14:
                    return 39;
                case 15:
                    return 41;
                default:
                    return 38;
            }
        }

        #region Write Liu Cell Value
        public ExcelWorksheet SetLiuCellValue(ExcelWorksheet sh, int CorrectRow, BudgetBookstruct bbstruct)
        {
            if (bbstruct != null)
            {
                if (CorrectRow == 10)
                {
                    sh.Cells[CorrectRow, 5].Value = bbstruct.ItemAmount;
                    //sh.Cells[CorrectRow, 6].Value = bbstruct.ItemPrice;
                    sh.Cells[CorrectRow, 7].Value = Convert.ToInt32(bbstruct.TotalPrice.Replace(",", ""));
                }
                else
                {
                    sh.Cells[CorrectRow, 5].Value = bbstruct.ItemAmount;
                    sh.Cells[CorrectRow, 6].Value = bbstruct.ItemPrice.Replace(",", "");
                    sh.Cells[CorrectRow, 7].Value = Convert.ToInt32(bbstruct.TotalPrice.Replace(",", ""));
                }

                if (!bbstruct.Memo.Equals(""))
                {
                    string memo = bbstruct.Memo;
                    if ((CorrectRow >= 8 && CorrectRow <= 9) && bbstruct.Memo != "")
                    {
                        memo = string.Empty;
                        int h = 0;
                        foreach (var engcnt in bbstruct.Memo.Split('#'))
                        {
                            memo += engcnt + "\r\n";
                            h += 11;
                        }
                        sh.Column(3).Width = 16;
                        sh.Row(CorrectRow).Height = h;
                    }
                    sh.Cells[CorrectRow, 3].Value = memo;
                    sh.Cells[CorrectRow, 3].Style.WrapText = true;
                }
            }
            return sh;
        }
        #endregion

        #region 設定設施項目表格
        public ExcelWorksheet SetbudgetBookTable(ExcelWorksheet sh, BudgetBookView Data, int mapno)
        {
            _MNo = mapno;
            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);
            sh.Cells[11, 8].Value = Data.PipingTotal;
            sh.Cells[12, 8].Value = Data.PipingMatMoney;

            ////L1
            //sh = SetCellValue(sh, 13, Data.L1);
            ////L2
            //if (Data.L2 != null && Convert.ToInt16(Data.L2.ItemAmount) > 0)
            //{
            //    sh = SetCellValue(sh, 14, Data.L2);
            //}

            //L1
            sh = SetCellValue(sh, 13, Data.L1);
            sh.Cells[13, 9].Value = "(L1)" + Data.L1_length + "公尺";
            //L2
            if (Data.L2 != null && Convert.ToInt16(Data.L2.ItemAmount) > 0)
            {
                sh = SetCellValue(sh, 14, Data.L2);
            }
            sh.Cells[14, 9].Value = "(L2)" + Data.L2_length + "公尺";

            sh = SetCellValue(sh, 15, Data.IrrSystem);
            sh = SetCellValue(sh, 16, Data.Work);
            sh = SetCellValue(sh, 17, Data.Planning);
            sh = SetCellValue(sh, 18, Data.RegulatedFac);
            sh = SetCellValue(sh, 19, Data.Engine);
            sh.Cells[20, 1].Value = " E.調蓄設施( " + Data.PoolWei + " )噸";
            sh = SetCellValue(sh, 20, Data.Pool);            
            sh.Cells[21, 8].Value = Data.Total;
            sh.Cells[22, 8].Value = Data.FarmerPay;

            
            #region 二次申請判斷
            if (FarmData.Any(m => m.IsApplied == true) == true)
            {
                sh.Cells[22, 3].Value = ">=(A) x 60%";
                sh.Cells[23, 3].Value = "<(A) x 40% + (C+D+E)";
            }
            #endregion
            sh.Cells[23, 8].Value = Data.GovPay_Pay;
            sh.Cells[24, 8].Value = Data.GovPay_Planning;
            sh.Cells[25, 8].Value = Data.GovPay_Sum;

            sh.Cells[26, 3].Value = "新台幣 " + Data.ChineseMoney + "元整";

            using (var range = sh.Cells[11, 9, 25, 10])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.WrapText = true;
                //range.AutoFitColumns();
            }
            return sh;
        }


        public ExcelWorksheet SetbudgetBookTableGold(ExcelWorksheet sh, BudgetBookView Data, int mapno)
        {
            _MNo = mapno;
            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);
            sh.Cells[11, 8].Value = Data.PipingTotal;
            sh.Cells[12, 8].Value = Data.PipingMatMoney;

            sh = SetCellValue(sh, 13, Data.L1);
            sh.Cells[13, 9].Value = "(L1)" + Data.L1_length + "公尺";
            if (Data.L2 != null && Convert.ToInt16(Data.L2.ItemAmount) > 0)
            {
                sh = SetCellValue(sh, 14, Data.L2);
            }
            sh.Cells[14, 9].Value = "(L2)" + Data.L2_length + "公尺";

            sh = SetCellValue(sh, 15, Data.IrrSystem);
            sh = SetCellValue(sh, 16, Data.Work);

            sh = SetCellValue(sh, 17, Data.Planning);
            sh = SetCellValue(sh, 18, Data.RegulatedFac);
            sh = SetCellValue(sh, 19, Data.Engine);
            sh.Cells[20, 1].Value = " E.調蓄設施( " + Data.PoolWei + " )噸";
            sh = SetCellValue(sh, 20, Data.Pool);
            sh.Cells[21, 8].Value = Data.Total;

            sh.Cells[22, 8].Value = Data.FarmerPay;

            
            #region 二次申請判斷
            if (FarmData.Any(m => m.IsApplied == true) == true)
            {
                sh.Cells[22, 3].Value = ">=(A) x 60%";
                sh.Cells[22, 3].Value = "<(A) x 40% + (C+D+E)";
            }
            #endregion
            sh.Cells[23, 8].Value = Data.GovPay_Pay;
            sh.Cells[24, 8].Value = Data.LiuPay;
            sh.Cells[25, 8].Value = Data.GoldPay;

            sh.Cells[26, 8].Value = int.Parse(Data.GovPay_Sum.Replace(",", "")) - int.Parse(Data.GovPay_Planning.Replace(",", ""));

            sh.Cells[27, 8].Value = Data.GovPay_Planning;
            
            sh.Cells[28, 8].Value = int.Parse(Data.Total.Replace(",", "")) - int.Parse(Data.FarmerPay.Replace(",",""));
            sh.Cells[29, 3].Value = "新台幣 " + Data.ChineseMoney + "元整";

            using (var range = sh.Cells[11, 9, 29, 9])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.WrapText = true;
                //range.AutoFitColumns();
            }
            return sh;
        }

        #endregion

        #region Write Cell Value
        public ExcelWorksheet SetCellValue(ExcelWorksheet sh, int CorrectRow, BudgetBookstruct bbstruct)//附註
        {
            if (bbstruct != null)
            {
                sh.Cells[CorrectRow, 6].Value = bbstruct.ItemAmount;
                sh.Cells[CorrectRow, 7].Value = bbstruct.ItemPrice;
                sh.Cells[CorrectRow, 8].Value = bbstruct.TotalPrice;

                if (!bbstruct.Memo.Equals(""))
                {
                    string memo = bbstruct.Memo;
                    if ((CorrectRow >= 18 && CorrectRow <= 20) && bbstruct.Memo != "")
                    {
                        memo = string.Empty;
                        int h = 0;
                        foreach (var engcnt in bbstruct.Memo.Split('#'))
                        {
                            memo += engcnt + "\r\n";
                            h += 11;
                        }
                        //sh.Column(9).Width = 16;
                        //sh.Row(CorrectRow).Height = h;
                    }
                    sh.Cells[CorrectRow, 9].Value = memo;
                    sh.Cells[CorrectRow, 9].Style.WrapText = true;
                }
            }
            return sh;
        }
        #endregion

        #endregion
        #region 調節控制材料數量計算表
        public byte[] CreateCtlMat(int mapno)
        {
            string sample_Path = @"~/ReportSample/CtlMat_Sample.xls";

            Microsoft.Office.Interop.Excel._Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlBook;
            Microsoft.Office.Interop.Excel.Worksheet xlSheet1;
            
            Microsoft.Office.Interop.Excel.Range xlRange;
            object misValue = System.Reflection.Missing.Value;
            xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
            xlBook = xlApp.Workbooks.Open(System.Web.HttpContext.Current.Server.MapPath(sample_Path), 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["調節控制材料數量計算表"];
            xlSheet1.Activate();


            BudgetBookView Data = db.GetBudgetBookData(mapno);
            xlSheet1.Cells[10, 3] = Data.IANum;
            xlSheet1.Cells[10, 6] = Data.Name;
            xlSheet1.Cells[10, 8] = Data.BuildArea / 10000;


            
            List<CntrlMat> items = getDataCls.GetCntrlMatData(mapno);
            if (items != null) {
                foreach (var item in items)
                {
                    switch (item.CntrlCode)
                    {
                        case 1:

                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        case 5:
                            xlSheet1.Cells[12, 5] = item.MatAmtAply;
                            xlSheet1.Cells[12, 6] = item.MatPriceAply;
                            xlSheet1.Cells[12, 7] = item.MatAmt;
                            xlSheet1.Cells[12, 8] = item.MatPrice;
                            break;
                        case 6:
                            xlSheet1.Cells[13, 5] = item.MatAmtAply;
                            xlSheet1.Cells[13, 6] = item.MatPriceAply;
                            xlSheet1.Cells[13, 7] = item.MatAmt;
                            xlSheet1.Cells[13, 8] = item.MatPrice;
                            break;
                        case 7:
                            xlSheet1.Cells[14, 5] = item.MatAmtAply;
                            xlSheet1.Cells[14, 6] = item.MatPriceAply;
                            xlSheet1.Cells[14, 7] = item.MatAmt;
                            xlSheet1.Cells[14, 8] = item.MatPrice;
                            break;
                        case 8:
                            xlSheet1.Cells[15, 5] = item.MatAmtAply;
                            xlSheet1.Cells[15, 6] = item.MatPriceAply;
                            xlSheet1.Cells[15, 7] = item.MatAmt;
                            xlSheet1.Cells[15, 8] = item.MatPrice;
                            break;
                        case 9:
                            xlSheet1.Cells[16, 5] = item.MatAmtAply;
                            xlSheet1.Cells[16, 6] = item.MatPriceAply;
                            xlSheet1.Cells[16, 7] = item.MatAmt;
                            xlSheet1.Cells[16, 8] = item.MatPrice;
                            break;
                    }
                }
 
            }



            xlBook.Saved = true;
            string export_name = "";
            export_name = Guid.NewGuid().ToString();
            xlBook.SaveCopyAs(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"));
            xlBook.Close();
            xlApp.Quit();
            xlSheet1 = null;
            
            xlApp = null;

            FileStream fstream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"), FileMode.Open);
            byte[] data = new Byte[fstream.Length];
            
            fstream.Read(data, 0, data.Length);
            fstream.Close();
            if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls")))
            {
                System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"));
            }
            
            return data;



        }
        #endregion

        #region 調節控制材料數量計算表(黃金廊道)
        public byte[] CreateCtlMatGold(int mapno)
        {
            string sample_Path = @"~/ReportSample/CtlMat_Gold_Sample.xls";

            Microsoft.Office.Interop.Excel._Application xlApp;
            Microsoft.Office.Interop.Excel.Workbook xlBook;
            Microsoft.Office.Interop.Excel.Worksheet xlSheet1;

            Microsoft.Office.Interop.Excel.Range xlRange;
            object misValue = System.Reflection.Missing.Value;
            xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
            xlBook = xlApp.Workbooks.Open(System.Web.HttpContext.Current.Server.MapPath(sample_Path), 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["調節控制材料數量計算表"];
            xlSheet1.Activate();


            
            BudgetBookView Data = db.GetBudgetBookData(mapno);
            xlSheet1.Cells[10, 3] = Data.IANum;
            xlSheet1.Cells[10, 6] = Data.Name;
            xlSheet1.Cells[10, 8] = Data.BuildArea / 10000;


            

            List<CntrlMat> items = getDataCls.GetCntrlMatData(mapno);
            if (items != null)
            {
                foreach (var item in items)
                {
                    switch (item.CntrlCode)
                    {
                        case 1:

                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        case 5:
                            xlSheet1.Cells[12, 5] = item.MatAmtAply;
                            xlSheet1.Cells[12, 6] = item.MatPriceAply;
                            xlSheet1.Cells[12, 7] = item.MatAmt;
                            xlSheet1.Cells[12, 8] = item.MatPrice;
                            break;
                        case 6:
                            xlSheet1.Cells[13, 5] = item.MatAmtAply;
                            xlSheet1.Cells[13, 6] = item.MatPriceAply;
                            xlSheet1.Cells[13, 7] = item.MatAmt;
                            xlSheet1.Cells[13, 8] = item.MatPrice;
                            break;
                        case 7:
                            xlSheet1.Cells[14, 5] = item.MatAmtAply;
                            xlSheet1.Cells[14, 6] = item.MatPriceAply;
                            xlSheet1.Cells[14, 7] = item.MatAmt;
                            xlSheet1.Cells[14, 8] = item.MatPrice;
                            break;
                        case 8:
                            xlSheet1.Cells[15, 5] = item.MatAmtAply;
                            xlSheet1.Cells[15, 6] = item.MatPriceAply;
                            xlSheet1.Cells[15, 7] = item.MatAmt;
                            xlSheet1.Cells[15, 8] = item.MatPrice;
                            break;
                        case 9:
                            xlSheet1.Cells[16, 5] = item.MatAmtAply;
                            xlSheet1.Cells[16, 6] = item.MatPriceAply;
                            xlSheet1.Cells[16, 7] = item.MatAmt;
                            xlSheet1.Cells[16, 8] = item.MatPrice;
                            break;
                    }
                }

            }
            
            xlSheet1.Cells[18, 9] = Data.RegulatedFac.TotalPrice;


            xlBook.Saved = true;
            string export_name = "";
            export_name = Guid.NewGuid().ToString();
            xlBook.SaveCopyAs(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"));
            xlBook.Close();
            xlApp.Quit();
            xlSheet1 = null;

            xlApp = null;

            FileStream fstream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"), FileMode.Open);
            byte[] data = new Byte[fstream.Length];

            fstream.Read(data, 0, data.Length);
            fstream.Close();
            if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls")))
            {
                System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"));
            }

            return data;



        }
        #endregion

        public byte[] ExportPdf(int mapno)
        {
            _MNo = mapno;
            BudgetBookView Data = db.GetBudgetBookData(_MNo);
            Int16 unit = Data.ApplyUnit;
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            //BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                document.NewPage();
                wrirteCover(document, Data);
                document.NewPage();
                writeBaseInfo(document, Data);
                writeBugetTable(document, Data);
                writePayTable(document, Data);
                if (unit ==23)
                {
                    writeSignupEmpty(document);
                }else  writeSignup(document);
                document.NewPage();
                writeLandsInfo(document, Data);
                if (Data.Engine != null || Data.Pool != null)
                {
                    document.NewPage();
                    writeEngPool(document, Data, _MNo);
                }

                document.NewPage();
                List<GetData.SysMat> sysmat = db.GetFarMat(_MNo);
                if (sysmat != null)
                {
                    writeMaterialTable(document, Data, sysmat/*, unit*/);
                }
                document.NewPage();
                List<CntrlMat> cntrlmat = getDataCls.GetCntrlMatData(_MNo);
                if (cntrlmat != null)
                {
                    writeCtrlTable(document, Data, cntrlmat);
                }
                document.NewPage();
                writeSysDesignPage(document, Data);

                document.NewPage();
                writePictures(document, Data);

                document.NewPage();
                PayeeReceiptDBService.DataStruct receiptdata = new PayeeReceiptDBService().GetPayeeReceiptDataByMapno(_MNo);
                writeReceipt(document, receiptdata);
                AcceptanceReportDBService.DataStruct acceptdata = new AcceptanceReportDBService().GetReportDataByMapno(_MNo);
                document.NewPage();
                AcceptanceReport.writeAcceptReportBody(document,acceptdata);
                document.NewPage();
                writeCompleteCheckList(document, /*_MNo*/ Data.IANum);
                document.Close();


                return stream.GetBuffer();
            }
        }

       
        private void writeLandsInfo(Document doc, BudgetBookView data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("設施土地清冊", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 });
            doc.Add(new Paragraph($"申請案號:{data.IANum}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT});
            doc.Add(new Paragraph($"申 請 人:{data.Name}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT});
            doc.Add(new Paragraph($"共{data.Farm.Count}筆土地資料,詳列如下", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 20 });
            PdfPTable table = new PdfPTable(new float[] { 30f, 10f, 15f, 15f });
            table.TotalWidth = 517f;
            table.LockedWidth = true;
            table.AddCell(new PdfPCell(new Phrase("地段", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            table.AddCell(new PdfPCell(new Phrase("地號", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            table.AddCell(new PdfPCell(new Phrase("土地面積(m²)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            table.AddCell(new PdfPCell(new Phrase("施設面積(m²)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            for (int i = 0; i < data.Farm.Count; i++)
            {
                table.AddCell(new PdfPCell(new Phrase($"{data.Farm[i].full_sectName}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase($"{data.Farm[i].LandNo}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase($"{(data.Farm[i].FarmArea).ToString("N0")}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase($"{(data.Farm[i].BuildArea).ToString("N0")}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            }
            table.AddCell(new PdfPCell(new Phrase($"合計", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            table.AddCell(new PdfPCell(new Phrase($"", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            table.AddCell(new PdfPCell(new Phrase($"{data.Farm.Sum( a => a.FarmArea).ToString("N0")}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            table.AddCell(new PdfPCell(new Phrase($"{data.Farm.Sum( a => a.BuildArea).ToString("N0")}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            doc.Add(table);            

            doc.Add(new Paragraph("--------------------以下空白--------------------", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_CENTER });

        }
        private void writeEngPool(Document doc, BudgetBookView data, int mapno)
        {
            //throw new NotImplementedException();
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("動力設施與調蓄設施數量表", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"申請案號:{data.IANum}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"設施型式:{data.EndType}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            string blockDim = data.Block.Length > 0 ? data.Block.Split('x')[0] + "m ×" + data.Block.Split('x')[1] + "m" : "";
            doc.Add(new Paragraph($"坵塊形狀:{blockDim}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            if (data.EndType == "穿孔管系統")
            {
                doc.Add(new Paragraph($"噴頭配置間距(SL):{data.SL}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            }
            else
            {
                doc.Add(new Paragraph($"噴頭配置間距(SSxSL):{data.SS} x {data.SL}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            }
            
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });

            MappingClass maping = new MappingClass();
            if (data.Engine != null)
            {
                doc.Add(new Paragraph("動力設施數量表", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 12 });
                
                List<Engine> engs = new GetData().GetEngineData(mapno);
                
                PdfPTable table = new PdfPTable(new float[] { 2f, 1f, 1f });
                table.TotalWidth = 517f;
                table.LockedWidth = true;
                table.AddCell(new PdfPCell(new Phrase("動力設備", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase("數量", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase("金額", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                int engTotal = 0;
                foreach (var item in engs)
                {
                    table.AddCell(new PdfPCell(new Phrase($"{maping.GetEngName(item.EngCode)}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase($"1", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase($"{item.EngPrice.ToString("N0")}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    engTotal += item.EngPrice;
                }
                table.AddCell(new PdfPCell(new Phrase("小計", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase($"{engTotal.ToString("N0")}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT});
                doc.Add(table);
                doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            }
            if (data.Pool != null)
            {
                doc.Add(new Paragraph("調蓄設施數量表", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 12 });
                List<Pool> pools = new GetData().GetPoolData(mapno);
                PdfPTable table = new PdfPTable(new float[] { 1f, 1f, 1f, 1f });
                table.TotalWidth = 517f;
                table.LockedWidth = true;
                table.AddCell(new PdfPCell(new Phrase("材質", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase("噸數", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase("數量", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase("金額", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                int poolTotal = 0;
                foreach (var item in pools)
                {
                    table.AddCell(new PdfPCell(new Phrase($"{maping.GetPooltypeName(item.PtypeCode)}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.PoolWeight}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase($"1", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.PoolPrice.ToString("N0")}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = 20 });
                    poolTotal += item.PoolPrice;
                }
                table.AddCell(new PdfPCell(new Phrase("小計", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase(" ", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase($"{poolTotal.ToString("N0")}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                doc.Add(table);
            }
            doc.Add(new Paragraph("--------------------以下空白--------------------", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_CENTER });
        }

        /// <summary>
        /// 預算書基本資料
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        private void writeBaseInfo(Document doc,BudgetBookView data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("推廣管路灌溉設施計畫預算書", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_CENTER });
            PdfPTable table = new PdfPTable(new float[] { 23, 33, 23, 23 });
            table.TotalWidth = 372f;
            table.LockedWidth = true;
            //row1
            //c1
            table.AddCell(new PdfPCell(new Phrase("農戶姓名", new iTextSharp.text.Font(baseFT, 14))) {HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL, FixedHeight = 20 });
            //c2
            table.AddCell(new PdfPCell(new Phrase(data.Name, new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c3
            table.AddCell(new PdfPCell(new Phrase("申請案號", new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL});
            //c4
            table.AddCell(new PdfPCell(new Phrase(data.IANum.ToString(), new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_CENTER});
            //row2
            //c1
            table.AddCell(new PdfPCell(new Phrase("住址", new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            //c2-4
            table.AddCell(new PdfPCell(new Phrase(data.Address, new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan=3 });
            //row3
            //c1
            string sectionMsg = data.Farm.Count <= 0 ? "" : data.Farm[0].full_sectName.ToString() + ", 地號: " + data.Farm[0].LandNo + " ,等 " + data.Farm.Count + "筆土地。";
            table.AddCell(new PdfPCell(new Phrase("設施地段", new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            //c2-4
            table.AddCell(new PdfPCell(new Phrase(sectionMsg, new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_LEFT, Colspan = 3 });
            //row4
            //c1
            table.AddCell(new PdfPCell(new Phrase("設施面積", new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL, FixedHeight = 20 });
            //c2-4
            table.AddCell(new PdfPCell(new Phrase($"{(data.BuildArea/10000).ToString("0.0000")} 公頃", new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 3 });
            //row5
            //c1
            table.AddCell(new PdfPCell(new Phrase("設施型式", new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL, FixedHeight = 20 });
            //c2-4
            table.AddCell(new PdfPCell(new Phrase(data.EndType, new iTextSharp.text.Font(baseFT, 14))) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 3 });
            doc.Add(table);

        }
        /// <summary>
        /// 預算書田間管路各項補助項目
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        private void writeBugetTable(Document doc, BudgetBookView data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            PdfPTable table = new PdfPTable(new float[] { 41, 41, 14, 22, 22, 22, 22 });
            table.TotalWidth = 517f;
            table.LockedWidth = true;
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 10)) { Alignment = Element.ALIGN_CENTER });
            //row1
            //c1
            table.AddCell(new PdfPCell(new Phrase("施設項目", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL, FixedHeight = 20 });
            //c2
            table.AddCell(new PdfPCell(new Phrase("說明", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL });
            //c3
            table.AddCell(new PdfPCell(new Phrase("單位", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL });
            //c4
            table.AddCell(new PdfPCell(new Phrase("數量", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL });
            //c5
            table.AddCell(new PdfPCell(new Phrase("單價", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL });
            //c6
            table.AddCell(new PdfPCell(new Phrase("總價", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL });
            //c7
            table.AddCell(new PdfPCell(new Phrase("附註", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL });
                        
            //row2
            //c1
            table.AddCell(new PdfPCell(new Phrase("A.田間管路設施費", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
            //c2
            if (data.ApplyY <= 108)
            {
                table.AddCell(new PdfPCell(new Phrase("(1)+(2)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase("(1)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }
            
            //c3
            table.AddCell(new PdfPCell(new Phrase("全", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c4
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c5
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c6
            table.AddCell(new PdfPCell(new Phrase(data.PipingTotal, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
            //c7
            if (data.Gold)
            {
                table.AddCell(new PdfPCell(new Phrase("廊道", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }
            
            //row3
            //c1
            table.AddCell(new PdfPCell(new Phrase("  (1)材料費", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
            //c2
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c3
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c4
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c5
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c6
            table.AddCell(new PdfPCell(new Phrase(data.PipingMatMoney, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
            //c7
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //row4
            //c1
            table.AddCell(new PdfPCell(new Phrase("    田間主管1(L1)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
            //c2
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c3
            table.AddCell(new PdfPCell(new Phrase("支", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            if (data.L1 == null)
            {
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c4
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c6
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c7
            } else
            {
                //c4
                table.AddCell(new PdfPCell(new Phrase(data.L1.ItemAmount ?? string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c5
                table.AddCell(new PdfPCell(new Phrase(data.L1.ItemPrice ?? string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c6
                table.AddCell(new PdfPCell(new Phrase(data.L1.TotalPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                //c7
                table.AddCell(new PdfPCell(new Phrase($"(L1){ data.L1_length ?? string.Empty }公尺", new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_LEFT });
            }

            //row5
            //c1
            table.AddCell(new PdfPCell(new Phrase("    田間主管2(L2)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
            //c2
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c3
            table.AddCell(new PdfPCell(new Phrase("支", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c4-c7
            if (data.L2 == null)
            {
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c4
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c6
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c7
                
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase(data.L2.ItemAmount, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c4
                table.AddCell(new PdfPCell(new Phrase(data.L2.ItemPrice ?? string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c5
                table.AddCell(new PdfPCell(new Phrase(data.L2.TotalPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase($"(L2){ data.L2_length }公尺", new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_LEFT });
            }

            //row6
            //c1
            table.AddCell(new PdfPCell(new Phrase("    灌溉系統", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
            //c2
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c3
            table.AddCell(new PdfPCell(new Phrase("式", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            if (data.IrrSystem == null)
            {
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c4
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c6
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });//c7
            } else if (data.IrrSystem.TotalPrice == "0")
            {
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c4
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c6
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });//c7
                
            }
            else
            {
                //c4
                table.AddCell(new PdfPCell(new Phrase("1", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c6
                table.AddCell(new PdfPCell(new Phrase(data.IrrSystem.TotalPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                //c7
                table.AddCell(new PdfPCell(new Phrase("詳如數量表", new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }

            //row7 109年起沒工作費不標示內容
            if (data.ApplyY <= 108)
            {
                //c1
                table.AddCell(new PdfPCell(new Phrase("  (2)工作費", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                //c2
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c3
                table.AddCell(new PdfPCell(new Phrase("公頃", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                if (data.Work == null)
                {
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c4
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c5
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c6
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });//c7
                }
                else
                {
                    //c4
                    table.AddCell(new PdfPCell(new Phrase((data.BuildArea / 10000d).ToString("0.0000"), new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    //c5
                    table.AddCell(new PdfPCell(new Phrase(data.Work.ItemPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                    //c6
                    table.AddCell(new PdfPCell(new Phrase(data.Work.TotalPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    //c7
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });
                }
            }
            
            //row8
            //c1
            table.AddCell(new PdfPCell(new Phrase("B.規劃設計費", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
            //c2
            table.AddCell(new PdfPCell(new Phrase("A. x 2.0%", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c3
            table.AddCell(new PdfPCell(new Phrase("式", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            if (data.Planning == null)
            {
                //c4
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c6
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c7
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });
            } else if (data.Planning.TotalPrice == "0")
            {
                //c4
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c6
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c7
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }
            else
            {
                //c4
                table.AddCell(new PdfPCell(new Phrase("1", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c6
                table.AddCell(new PdfPCell(new Phrase(data.Planning.TotalPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                //c7
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }

            //row9
            //c1
            table.AddCell(new PdfPCell(new Phrase("C.調控設施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
            //c2
            table.AddCell(new PdfPCell(new Phrase("依計畫補助標準", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c3
            table.AddCell(new PdfPCell(new Phrase("式", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            if (data.RegulatedFac == null)
            {
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c4
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c6
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });//c7

            }
            
            else
            {
                //c4
                table.AddCell(new PdfPCell(new Phrase("1", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c6
                table.AddCell(new PdfPCell(new Phrase(data.RegulatedFac.TotalPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                //c7
                table.AddCell(new PdfPCell(new Phrase("詳如數量表", new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }
            //row10動力設備
            
            if (data.Engine == null)
            {
                //c1
                table.AddCell(new PdfPCell(new Phrase($"D.動力設備(0台)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                //c2
                table.AddCell(new PdfPCell(new Phrase("依計畫補助標準", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c3
                table.AddCell(new PdfPCell(new Phrase("式", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c4
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c6
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c7
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }
            else if (data.Engine.TotalPrice == "0") {
                //c1
                table.AddCell(new PdfPCell(new Phrase($"D.動力設備(0台)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                //c2
                table.AddCell(new PdfPCell(new Phrase("依計畫補助標準", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c3
                table.AddCell(new PdfPCell(new Phrase("式", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c4
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c6
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c7
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }
            else
            {               
                //c1
                table.AddCell(new PdfPCell(new Phrase($"D.動力設備({data.Engine.ItemAmount}台)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                //c2
                table.AddCell(new PdfPCell(new Phrase("依計畫補助標準", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c3
                table.AddCell(new PdfPCell(new Phrase("式", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c4
                table.AddCell(new PdfPCell(new Phrase("1", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c5
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                //c6
                table.AddCell(new PdfPCell(new Phrase(data.Engine.TotalPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });//c6
                //c7
                table.AddCell(new PdfPCell(new Phrase($"詳如數量表", new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });//c7
            } 
                
                
            
         
            //row11
            //c1
            if (data.Pool == null)
            {
                table.AddCell(new PdfPCell(new Phrase($"E.調蓄設施(0噸)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
            }else table.AddCell(new PdfPCell(new Phrase($"E.調蓄設施({data.PoolWei}噸)", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
            //c2
            table.AddCell(new PdfPCell(new Phrase("依計畫補助標準", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c3
            //table.AddCell(new PdfPCell(new Phrase("座", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("式", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
           
            table.AddCell(new PdfPCell(new Phrase("1", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c4
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c5
            
            if (data.Pool == null)
            {
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c6
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });//c7
            }
            else
            {
                table.AddCell(new PdfPCell(new Phrase(data.Pool.TotalPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });//c6
                table.AddCell(new PdfPCell(new Phrase($"詳如數量表", new iTextSharp.text.Font(baseFT, 10))) { HorizontalAlignment = Element.ALIGN_CENTER });//c7
            }


            //row12
            //c1
            table.AddCell(new PdfPCell(new Phrase($"合    計", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
            //c2
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c3
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c4
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c5
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c6
            table.AddCell(new PdfPCell(new Phrase(data.Total, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
            //c7
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });

            doc.Add(table);
        }
        /// <summary>
        /// 預算書補助金額款項
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        private void writePayTable(Document doc, BudgetBookView data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            PdfPTable table = new PdfPTable(new float[] { 18,23, 41, 14, 22, 22, 22, 22 });
            table.TotalWidth = 517f;
            table.LockedWidth = true;
            //row1
            //c12
            table.AddCell(new PdfPCell(new Phrase("農戶配合款", new iTextSharp.text.Font(baseFT, 12))) { FixedHeight = 20 , HorizontalAlignment = Element.ALIGN_LEFT, Colspan=2});
            //c3
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c4
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c5
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c6
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c7
            table.AddCell(new PdfPCell(new Phrase(data.FarmerPay, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
            //c8
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //row234
            table.AddCell(new PdfPCell(new Phrase("政府\n補助款", new iTextSharp.text.Font(baseFT, 12))) { FixedHeight = 20, HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE,Rowspan = 3 });
            //row2
            //c2
            table.AddCell(new PdfPCell(new Phrase("農戶請領款", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 20});
            //c3
            string govpaydetail = string.Empty;
            if (data.GovPay_Detail != null)
            {
                foreach (var item in data.GovPay_Detail)
                {
                    govpaydetail += $"{item}\n";
                }
            }            
            table.AddCell(new PdfPCell(new Phrase(govpaydetail, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
            //c4
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c5
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c6
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c7
            table.AddCell(new PdfPCell(new Phrase(data.GovPay_Pay, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
            //c8
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //row3
            table.AddCell(new PdfPCell(new Phrase("規劃設計費", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            //c3
            table.AddCell(new PdfPCell(new Phrase("B", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c4
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c5
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c6
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c7
            table.AddCell(new PdfPCell(new Phrase(data.GovPay_Planning, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
            //c8
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //row4
            table.AddCell(new PdfPCell(new Phrase("小　　　計", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            //c3
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c4
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c5
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c6
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c7
            table.AddCell(new PdfPCell(new Phrase(data.GovPay_Sum, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
            //c8
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //row5
            table.AddCell(new PdfPCell(new Phrase("本設施預算總計", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_JUSTIFIED_ALL, Colspan = 2, FixedHeight = 20 });
            table.AddCell(new PdfPCell(new Phrase($"新台幣 {data.ChineseMoney}元整", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, Colspan = 6 });
            doc.Add(table);
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_CENTER });
            

        }
        /// <summary>
        /// 公文核章項目
        /// </summary>
        /// <param name="doc"></param>
        private void writeSignup(Document doc)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            PdfPTable table = new PdfPTable(new float[] { 23f, 34f, 23f, 34f, 23f, 34f  });
            table.TotalWidth = 517f;
            table.LockedWidth = true;
            //R1
            table.AddCell(new PdfPCell(new Phrase("設計", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER,VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("股長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("主任工程師", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //R2
            table.AddCell(new PdfPCell(new Phrase("審查", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("組長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("副處長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            //table.AddCell(new PdfPCell(new Phrase("副首長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //R3
            table.AddCell(new PdfPCell(new Phrase("主辦", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("主計單位", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("處長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            //table.AddCell(new PdfPCell(new Phrase("首長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            doc.Add(table);
        }
        /// <summary>
        /// 空白核章
        /// </summary>
        /// <param name="doc"></param>
        private void writeSignupEmpty(Document doc)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            PdfPTable table = new PdfPTable(new float[] { 23f, 34f, 23f, 34f, 23f, 34f });
            table.TotalWidth = 517f;
            table.LockedWidth = true;

            for (int i = 0; i < 3; i++)
            {
                //R1
                table.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            }
            
            //R2
            //table.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            //table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //table.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            //table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //table.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            //table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //R3
            //table.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 40 });
            //table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //table.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            //table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //table.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE/*, FixedHeight = 30*/ });
            //table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            doc.Add(table);
        }

        private void writeSignupOld(Document doc)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            PdfPTable table = new PdfPTable(new float[] { 10, 10, 10, 10 });
            table.TotalWidth = 517f;
            table.LockedWidth = true;
            //c11
            table.AddCell(new PdfPCell(new Phrase("設計", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c12
            table.AddCell(new PdfPCell(new Phrase("審查", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c13
            table.AddCell(new PdfPCell(new Phrase("主辦", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c14
            table.AddCell(new PdfPCell(new Phrase("股長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 60f });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 60f });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 60f });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 60f });
            //c15
            table.AddCell(new PdfPCell(new Phrase("組長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c16
            table.AddCell(new PdfPCell(new Phrase("主計單位", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c17
            table.AddCell(new PdfPCell(new Phrase("總幹事", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c18
            table.AddCell(new PdfPCell(new Phrase("會長", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //c21            
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 60f });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 60f });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 60f });
            table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 60f });

            doc.Add(table);
        }
        /// <summary>
        /// 預算書田間材料數量表
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        /// <param name="matdata"></param>
        /// <param name="unitid"></param>
        private void writeMaterialTable(Document doc, BudgetBookView data, List<GetData.SysMat> matdata/*, short unitid*/)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            PdfPTable table = new PdfPTable(new float[] { 9, 23, 16, 10, 9, 10, 15 });
            table.TotalWidth = 517f;
            table.LockedWidth = true;
            doc.Add(new Paragraph("管路灌溉系統材料數量表", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"申請案號:{data.IANum}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"設施型式:{data.EndType}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            string blockDim = data.Block.Length > 0 ? data.Block.Split('x')[0] + "m ×" + data.Block.Split('x')[1] + "m" : "";
            doc.Add(new Paragraph($"坵塊形狀:{blockDim}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            if (data.EndType == "穿孔管系統")
            {
                doc.Add(new Paragraph($"噴頭配置間距(SL):{data.SL}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            }
            else
            {
                doc.Add(new Paragraph($"噴頭配置間距(SSxSL):{data.SS} x {data.SL}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            }
            
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            //doc.Add(new Paragraph(string.Empty, new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_LEFT });

            //row1
            table.AddCell(new PdfPCell(new Phrase("項目", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER,FixedHeight = 20 });
            table.AddCell(new PdfPCell(new Phrase("材料名稱", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("規格", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("單位", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("單價", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("數量", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("總價", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            int ItemIndex = 1;
            if (matdata != null)
            {
                if (matdata.Any(m => m.moduleno == 0))
                {
                    foreach (var mat in matdata)
                    {
                        table.AddCell(new PdfPCell(new Phrase(mat.matname,new iTextSharp.text.Font(baseFT,12))) {HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                        table.AddCell(new PdfPCell(new Phrase(mat.spec,new iTextSharp.text.Font(baseFT,12))) {HorizontalAlignment = Element.ALIGN_LEFT });
                        table.AddCell(new PdfPCell(new Phrase(mat.itemunit, new iTextSharp.text.Font(baseFT,12))) {HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(mat.matprice.ToString("N1"), new iTextSharp.text.Font(baseFT,12))) {HorizontalAlignment = Element.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(mat.amount.ToString(), new iTextSharp.text.Font(baseFT,12))) {HorizontalAlignment = Element.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase((mat.matprice * mat.amount).ToString("N0"), new iTextSharp.text.Font(baseFT,12))) {HorizontalAlignment = Element.ALIGN_RIGHT });                        

                    }
                    table.AddCell(new PdfPCell(new Phrase("總價", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(data.IrrSystem.TotalPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = 20 });

                }
                else
                {
                    foreach (var mat in matdata.GroupBy(m => m.groupno).Select(o => new { Grouop = o.Key }))
                    {
                        table.AddCell(new PdfPCell(new Phrase($"{ItemIndex}.{matdata.Where(m => m.groupno == mat.Grouop).FirstOrDefault().group}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20, Colspan=7 });
                        int itemorder = 1;
                        foreach (var item in matdata.Where(m => m.groupno == mat.Grouop))
                        {
                            table.AddCell(new PdfPCell(new Phrase($"{ItemIndex}-{/*item.order*/itemorder}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20});
                            table.AddCell(new PdfPCell(new Phrase(item.matname, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT});
                            table.AddCell(new PdfPCell(new Phrase(item.spec, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT});
                            table.AddCell(new PdfPCell(new Phrase(item.itemunit, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER});
                            table.AddCell(new PdfPCell(new Phrase(item.matprice.ToString("N1"), new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT});
                            table.AddCell(new PdfPCell(new Phrase(item.amount.ToString(), new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                            table.AddCell(new PdfPCell(new Phrase((item.matprice * item.amount).ToString("N0"), new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
                            itemorder++;
                            
                        }
                        ItemIndex++;
                    }
                    table.AddCell(new PdfPCell(new Phrase("總價", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT});
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT});
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT});
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT});
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT});
                    table.AddCell(new PdfPCell(new Phrase(data.IrrSystem.TotalPrice, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT});
                }
            }
            doc.Add(table);
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("--------------------以下空白--------------------", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_CENTER });

        }
        /// <summary>
        /// 預算書調控材料數量表
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        /// <param name="cntrldata"></param>
        private void writeCtrlTable(Document doc,BudgetBookView data, List<CntrlMat> cntrldata)
        {
            var ctrlTypes = DryDb.CntrlList.ToDictionary(m => m.CntrlCode, m => m.CntrlCNS);
            
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            PdfPTable table = new PdfPTable(new float[] { 9, 23, 16, 10, 9, 10, 15 });
            table.TotalWidth = 517f;
            table.LockedWidth = true;
            doc.Add(new Paragraph("調控設施材料數量表", new iTextSharp.text.Font(baseFT, 16)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"申請案號:{data.IANum}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph($"設施型式:{data.EndType}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            string blockDim = data.Block.Length > 0 ? data.Block.Split('x')[0] + "m ×" + data.Block.Split('x')[1] + "m" : "";
            doc.Add(new Paragraph($"坵塊形狀:{blockDim}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            if (data.EndType == "穿孔管系統")
            {
                doc.Add(new Paragraph($"噴頭配置間距(SL):{data.SL}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            }
            else
            {
                doc.Add(new Paragraph($"噴頭配置間距(SSxSL):{data.SS} x {data.SL}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            }
            
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            //row1
            table.AddCell(new PdfPCell(new Phrase("項目", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
            table.AddCell(new PdfPCell(new Phrase("材料名稱", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("規格", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("單位", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("單價", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("數量", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            table.AddCell(new PdfPCell(new Phrase("總價", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER });
            //row2
            int cntrlcode = 0;
            if (cntrldata.Count > 0 )
            {
                
                int itemIndex = 1;
                int totalprice = 0;
                int typeIndex = 0;
                foreach (var item in cntrldata)
                {
                    
                    if (cntrlcode != item.CntrlCode)
                    {
                        typeIndex++;
                        table.AddCell(new PdfPCell(new Phrase($"{typeIndex}. {ctrlTypes[item.CntrlCode]}", new iTextSharp.text.Font(baseFT, 12))) {HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20, Colspan=7 });
                        cntrlcode = item.CntrlCode;
                        itemIndex = 1;
                    }
                    table.AddCell(new PdfPCell(new Phrase($"{typeIndex}-{itemIndex}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(item.MatName, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(item.MatPriceAply.ToString("N0"), new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase(item.MatAmtAply.ToString(), new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, FixedHeight = 20 });
                    table.AddCell(new PdfPCell(new Phrase((item.MatPriceAply * item.MatAmtAply).ToString("N0"), new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT, FixedHeight = 20 });
                    totalprice += (int)item.MatPriceAply * item.MatAmtAply;

                    itemIndex++;
                }
                table.AddCell(new PdfPCell(new Phrase("總價", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 20 });
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT });
                table.AddCell(new PdfPCell(new Phrase(totalprice.ToString("N0"), new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_RIGHT });
            }
            
           

            doc.Add(table);
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("--------------------以下空白--------------------", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_CENTER });
        }
        /// <summary>
        /// 預算書封面
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        public void wrirteCover(Document doc, BudgetBookView data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("推廣管路灌溉設施補助計畫", new iTextSharp.text.Font(baseFT, 26)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"{data.ApplyY.ToString()}年度", new iTextSharp.text.Font(baseFT, 26)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            string ianame = getDataCls.GetUnitName(data.ApplyUnit);
            doc.Add(new Paragraph($"{ianame}", new iTextSharp.text.Font(baseFT, 26)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"申請案號:{data.IANum}", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"申 請 人:{data.Name}", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"通訊住址:{data.Address}", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            string sectionMsg = data.Farm.Count <= 0 ? "" : data.Farm[0].full_sectName.ToString() + ",地號:" + data.Farm[0].LandNo + ",等" + data.Farm.Count + "筆土地。";
            doc.Add(new Paragraph($"設施地點:{sectionMsg}", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 110,FirstLineIndent = -90 });
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"申請面積:{(data.BuildArea/10000).ToString("0.0000")}公頃", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 20 });
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph($"設施型式:{data.EndType}", new iTextSharp.text.Font(baseFT, 20)) { Alignment = Element.ALIGN_LEFT,IndentationLeft = 20 });


        }
        /// <summary>
        /// 預算書收據
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="item"></param>
        public void writeReceipt(Document doc, PayeeReceiptDBService.DataStruct item)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("領  款  收  據", new iTextSharp.text.Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER });
            //doc.Add(new Paragraph(string.Format("設施編號:{0}", item.IANum), new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 350 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(string.Format("新臺幣:{0}", item.SubsidyFeeCNS), new iTextSharp.text.Font(baseFT, 14)) { IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(item.Content, new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph("  此致", new iTextSharp.text.Font(baseFT, 16)) { IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(item.IAName, new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 100 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph($"申請案號:{item.IANum}", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(string.Format("領款人(簽名或蓋章):{0}", item.FarmerName), new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            //doc.Add(new Paragraph("簽名或蓋章:", new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            //doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(string.Format("身分證字號:{0}", item.FarmerID), new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(string.Format("通訊地址:{0}", item.Addr), new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph(string.Format("聯絡電話:{0}", item.Tel), new iTextSharp.text.Font(baseFT, 14)) { Alignment = Element.ALIGN_LEFT, IndentationLeft = 50 });

            doc.Add(Chunk.NEWLINE);
            doc.Add(Chunk.NEWLINE);
            doc.Add(Chunk.NEWLINE);
            doc.Add(new Paragraph("中   華   民   國    年    月    日", new iTextSharp.text.Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER });
        }
        /// <summary>
        /// 系統設施設計圖
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        public void writeSysDesignPage(Document doc, BudgetBookView data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("推廣管路灌溉設施計畫系統設施設計圖", new iTextSharp.text.Font(baseFT, 22, 1)) { Alignment = Element.ALIGN_CENTER,SpacingAfter = 12 });

            #region Old
            string line = $"申請人：{data.Name}".PadRight(20) + $"施設型式：{data.EndType}".PadRight(26) + $"申請案號：{data.IANum}".PadRight(20);
            //doc.Add(new Paragraph(line, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            //doc.Add(new Paragraph($"地  段：{data.Farm.First().full_sectName}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            string landno = string.Empty;
            foreach (var item in data.Farm)
            {
                landno += item.LandNo + " ";
            }
            //doc.Add(new Paragraph($"地  號：{landno}", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT });
            //if (data.EndType.Contains("穿孔管"))
            //{
            //    doc.Add(new Paragraph($"SL(行距)：{data.SL} 公尺,L1(田間主管)：{data.L1_length} 公尺", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 12 });
            //}
            //else if (data.EndType.Contains("其他"))
            //{
            //    doc.Add(new Paragraph($"SL(行距)：{data.SL} 公尺,SS(株距)：{data.SS} 公尺,L1(田間主管)：{data.L1_length} 公尺", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 12 });
            //}
            //else
            //{
            //    doc.Add(new Paragraph($"SL(行距)：{data.SL} 公尺,SS(株距)：{data.SS} 公尺,L1(田間主管)：{data.L1_length} 公尺", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 12 });
            //}
            #endregion

            PdfPTable table1 = new PdfPTable(new float[] { 26,30,26 });
            table1.TotalWidth = 517f;
            table1.LockedWidth = true;
            table1.AddCell(new PdfPCell(new Phrase($"申請人：{ data.Name }", new iTextSharp.text.Font(baseFT, 12))){ HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE ,FixedHeight = 16 , BorderWidth = 0});
            table1.AddCell(new PdfPCell(new Phrase($"施設型式：{data.EndType}", new iTextSharp.text.Font(baseFT, 12))){ HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE ,FixedHeight = 16, BorderWidth = 0 });
            table1.AddCell(new PdfPCell(new Phrase($"申請案號：{data.IANum}", new iTextSharp.text.Font(baseFT, 12))){ HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE ,FixedHeight = 16, BorderWidth = 0 });
            //table1.AddCell(new PdfPCell(new Phrase($"地  段：{data.Farm.FirstOrDefault().full_sectName}", new iTextSharp.text.Font(baseFT, 12))){ HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE , FixedHeight = 16, Colspan = 3, BorderWidth = 0 });
            table1.AddCell(new PdfPCell(new Phrase($"施設縣市、鄉鎮、地段、地號及面積詳如土地清冊，合計面積{data.BuildArea.ToString("N0")} m²", new iTextSharp.text.Font(baseFT, 12))){ HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE , FixedHeight = 16, Colspan = 3, BorderWidth = 0 });
            //table1.AddCell(new PdfPCell(new Phrase($"地  號：{landno}", new iTextSharp.text.Font(baseFT, 12))){ HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE , FixedHeight = 16, Colspan = 3, BorderWidth = 0 });
            table1.AddCell(new PdfPCell(new Phrase($"行距(SL)：{data.SL} m", new iTextSharp.text.Font(baseFT, 12))){ HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE , FixedHeight = 16, BorderWidth = 0 });
            if (data.EndType == "穿孔管系統")
            {
                table1.AddCell(new PdfPCell(new Phrase($"{string.Empty}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 16, BorderWidth = 0 });
            }
            else
            {
                table1.AddCell(new PdfPCell(new Phrase($"間距(SS)：{data.SS} m", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 16, BorderWidth = 0 });
            }
            
            table1.AddCell(new PdfPCell(new Phrase($"長度(L1)：{data.L1_length} m", new iTextSharp.text.Font(baseFT, 12))){ HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE , FixedHeight = 16, BorderWidth = 0 });
            doc.Add(table1);
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 8)){ Alignment = Element.ALIGN_LEFT });
            PdfPTable table = new PdfPTable(new float[] { 1 });
            table.TotalWidth = 517f;
            table.LockedWidth = true;
            table.AddCell(new PdfPCell(new Phrase("地籍圖：".PadRight(55) + "比例尺：".PadRight(10), new iTextSharp.text.Font(baseFT, 12)))
            { VerticalAlignment = Element.ALIGN_TOP, HorizontalAlignment = Element.ALIGN_LEFT, FixedHeight = 630});
            doc.Add(table);

        }
        /// <summary>
        /// 申請案件書面審查表
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="ianum"></param>
        public static void writeCompleteCheckList(Document doc, int ianum)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            doc.Add(new Paragraph("申請案件書面審查表", new iTextSharp.text.Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 });
            PdfPTable table = new PdfPTable(new float[] { 1f, 1f });
            table.TotalWidth = 396f;
            table.LockedWidth = true;
            table.AddCell(new PdfPCell(new Phrase("申請案號", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table.AddCell(new PdfPCell(new Phrase($"{ianum}", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            doc.Add(table);
            doc.Add(new Paragraph(" ", new iTextSharp.text.Font(baseFT, 10)));

            PdfPTable table1 = new PdfPTable(new float[] { 121f, 15f, 15f, 34f });
            table1.TotalWidth = 547f;
            table1.LockedWidth = true;
            //row0
            table1.AddCell(new PdfPCell(new Phrase("應附文件", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("合格", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("不合格", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("備註", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //row1
            table1.AddCell(new PdfPCell(new Phrase("1.推廣管路灌溉設施補助申請表", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //row2
            table1.AddCell(new PdfPCell(new Phrase("2.國民身分證正反面影本", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //row3
            table1.AddCell(new PdfPCell(new Phrase("3.地籍圖謄本及三個月內之土地登記謄本", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //row4
            //table1.AddCell(new PdfPCell(new Phrase("4.申請RC調蓄設施者，需農業用地作農業設施容許使用", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("4.施設需農業用地作農業設施者容許使用文件", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //row5
            table1.AddCell(new PdfPCell(new Phrase("5.土地所有權人同意施設證明書或國公有土地租賃契約影本", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //row6
            table1.AddCell(new PdfPCell(new Phrase("6.推廣管路灌溉設施補助切結書", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //row7
            table1.AddCell(new PdfPCell(new Phrase("7.工程預算書", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //row8
            table1.AddCell(new PdfPCell(new Phrase("8.施設完成後之結案申報書", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            //row9
            table1.AddCell(new PdfPCell(new Phrase("9.施設前、後照片", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table1.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            doc.Add(table1);

            PdfPTable table2 = new PdfPTable(new float[] { 11f, 110f, 15f, 15f, 34f });
            table2.TotalWidth = 547f;
            table2.LockedWidth = true;
            //row10
            table2.AddCell(new PdfPCell(new Phrase("10.\n設施\n性能\n規格\n之證\n明文\n件", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT, Rowspan = 3 });
            table2.AddCell(new PdfPCell(new Phrase("載明廠牌、品名及型號之統一發票或收據", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            
            //row11
            table2.AddCell(new PdfPCell(new Phrase("出廠證明書", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            
            //row12
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            
            //row13
            table2.AddCell(new PdfPCell(new Phrase("11.領據、相關單據", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE, Colspan = 2, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            //row14
            table2.AddCell(new PdfPCell(new Phrase("12.\n其\n他\n指\n定\n文\n件", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT, Rowspan = 4 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            //row15
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            //row16
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            //row17
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            //row18
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT, FixedHeight = 30 });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            table2.AddCell(new PdfPCell(new Phrase("", new iTextSharp.text.Font(baseFT, 13))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_LEFT });
            doc.Add(table2);
            doc.Add(new Paragraph("", new iTextSharp.text.Font(baseFT, 13)) { Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("  審查結果：□合格        □不合格         審查人：", new iTextSharp.text.Font(baseFT, 13)) { Alignment = Element.ALIGN_LEFT });

        }
        /// <summary>
        /// 施設前後照片
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="data"></param>
        public static void writePictures(Document doc, BudgetBookView data)
        {
            string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
            BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            string line = ($"申請案號:{data.IANum}").PadRight(40 - data.IANum.ToString().Length) + $"申請人姓名:{data.Name}";
            doc.Add(new Paragraph(line, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingAfter = 20 });
            PdfPTable table = new PdfPTable(new float[] { 12f, 123f });
            table.TotalWidth = 500f;
            table.LockedWidth = true;
            table.AddCell(new PdfPCell(new Phrase("施\n工\n前", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 130 });
            table.AddCell(new PdfPCell(new Phrase("施工前照片", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE});
            table.AddCell(new PdfPCell(new Phrase("施\n工\n後", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 130 });
            table.AddCell(new PdfPCell(new Phrase("施工後照片及系統施噴、滴灌溉情形", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell(new Phrase("動\n力\n設\n備", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 130 });
            table.AddCell(new PdfPCell(new Phrase("動力設備照片", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell(new Phrase("調\n蓄\n設\n施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 130 });
            table.AddCell(new PdfPCell(new Phrase("調蓄設施照片", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            table.AddCell(new PdfPCell(new Phrase("調\n節\n控\n制\n設\n施", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, FixedHeight = 130 });
            table.AddCell(new PdfPCell(new Phrase("調節控制設施照片", new iTextSharp.text.Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
            doc.Add(table);
            doc.Add(new Paragraph("備註：本表之照片可由印表機直接列印出或以沖洗之照片粘貼方式均可，其張數自行調整", new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_CENTER, IndentationLeft = 10, IndentationRight = 10 });

        }

    }
    

}
