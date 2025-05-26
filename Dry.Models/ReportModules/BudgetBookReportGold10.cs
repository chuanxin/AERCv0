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

namespace Dry.Models.ReportModules
{
    public class BudgetBookReportGold10
    {
        #region Parameter
        private int _MNo;
        private BudgetBookDBService db = new BudgetBookDBService();
        private SubsidyReportDBService SRdb = new SubsidyReportDBService();
        private FarmLandDBService farmlanddb = new FarmLandDBService();
        private GetData getDataCls = new GetData();
        #endregion
        
        public byte[] ExportExcel(int mapno)
        {
            _MNo = mapno;
            BudgetBookView Data = db.GetBudgetBookData(_MNo);
            Int16 unit = Data.ApplyUnit;
            

            string sample_Path = @"~/ReportSample/BudgetBook_Sample.xlsx";
            string FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.xlsx";
         
            sample_Path = @"~/ReportSample/Gold_BudgetBook_Sample_10.xlsx";
 
            

            FileStream fs = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);
          
            
            ExcelPackage excel = new ExcelPackage(fs);
            List<string> sheetAry = new List<string>();
            _MNo = mapno;
            sheetAry.AddRange(new string[] { "黃金廊道", "材料數量表" });
            ExcelWorksheet sheet = excel.Workbook.Worksheets[sheetAry[0]];
            sheet = BasicTable(sheet, Data,0);
            sheet = SetbudgetBookTableGold(sheet, Data, _MNo,0);
            sheet = SetCoaMatTable(sheet, Data, db.GetFarMat(_MNo), unit);
            sheet = BasicTable(sheet, Data, 77);
            sheet = SetbudgetBookTableGold(sheet, Data, _MNo, 77);
            sheet = PayReceipt(sheet, Data, _MNo);
               
            fs.Close();

            byte[] file = excel.GetAsByteArray();
            //return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
            return file;
        }

        

        #region 基本資料表格
        public ExcelWorksheet BasicTable(ExcelWorksheet sh, BudgetBookView Data, int OffSetRow)
        {
            int StartCol = 3;
            int EndCol = 4;

            int StartRow = 4 + OffSetRow;
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

            using (var range = sh.Cells[StartRow, StartCol, EndRow, EndCol])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //range.AutoFitColumns();
                range.Style.WrapText = true;
            }

            return sh;
        }
        #endregion
        #region 材料數量表
        public ExcelWorksheet SetCoaMatTable(ExcelWorksheet sh, BudgetBookView data, List<GetData.SysMat> matdata, short unitid)
        {
            //int row = GetStartRow((short)Session["UnitID"], data.Gold);
            //int row = GetStartRow(unitid, data.Gold);
            int row = 43;
            sh.Cells[row, 1].Value = "設施型式：" + data.EndType;
            sh.Cells[row + 1, 1].Value = "坵塊型狀：" + (data.Block.Length > 0 ? data.Block.Split('x')[0] + "m ×" + data.Block.Split('x')[1] + "m" : "");
            sh.Cells[row + 2, 1].Value = "噴頭配置間距(SL×SS)：" + data.SL + "×" + data.SS;
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
                    if (Gold) { return 45; }
                    return 46;
                case 10:
                    if (Gold) { return 42; }
                    return 41;
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
        
        #region 設定設施項目表格
        public ExcelWorksheet SetbudgetBookTable(ExcelWorksheet sh, BudgetBookView Data, int mapno)
        {
            //_MNo = (int)Session["MapNo"];
            _MNo = mapno;
            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);//取該筆農地資料(for 二次申請check)
            //田間管路設施費
            sh.Cells[11, 8].Value = Data.PipingTotal;
            //材料費
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

            //IRR 
            sh = SetCellValue(sh, 15, Data.IrrSystem);
            //Work
            sh = SetCellValue(sh, 16, Data.Work);

            //Planning
            sh = SetCellValue(sh, 17, Data.Planning);
            //調控
            sh = SetCellValue(sh, 18, Data.RegulatedFac);
            //動力
            sh = SetCellValue(sh, 19, Data.Engine);
            //蓄水
            sh.Cells[20, 1].Value = " E.蓄水池( " + Data.PoolWei + " )噸";
            sh = SetCellValue(sh, 20, Data.Pool);

            //合計
            sh.Cells[21, 8].Value = Data.Total;

            //農戶自備款
            sh.Cells[22, 8].Value = Data.FarmerPay;

            //農戶請領款
            #region 二次申請判斷
            if (FarmData.Any(m => m.IsApplied == true) == true)
            {
                sh.Cells[22, 3].Value = ">=(A) x 60%";
                sh.Cells[23, 3].Value = "<(A) x 40% + (C+D+E)";
            }
            #endregion
            sh.Cells[23, 8].Value = Data.GovPay_Pay;
            //規劃費
            sh.Cells[24, 8].Value = Data.GovPay_Planning;
            //小計
            sh.Cells[25, 8].Value = Data.GovPay_Sum;

            sh.Cells[26, 3].Value = "新台幣 " + Data.ChineseMoney + "元整";

            using (var range = sh.Cells[11, 9, 25, 10])//fromRow, fromCol, toRow, toCol 11 8 25 8
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.WrapText = true;
                //range.AutoFitColumns();
            }
            return sh;
        }

        #endregion
        public ExcelWorksheet SetbudgetBookTableGold(ExcelWorksheet sh, BudgetBookView Data, int mapno,int OffSetRow)
        {
            
            _MNo = mapno;
            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);
            
            int startRow = 11 + OffSetRow;
            sh.Cells[startRow, 8].Value = Data.PipingTotal;
            startRow++;
            sh.Cells[startRow, 8].Value = Data.PipingMatMoney;

            

            startRow++;
            sh = SetCellValue(sh, startRow, Data.L1);
            sh.Cells[startRow, 9].Value = "(L1)" + Data.L1_length + "公尺";
            startRow++;
            if (Data.L2 != null && Convert.ToInt16(Data.L2.ItemAmount) > 0)
            {
               
                sh = SetCellValue(sh, startRow, Data.L2);
            }
            sh.Cells[startRow, 9].Value = "(L2)" + Data.L2_length + "公尺";

            startRow++;
            sh = SetCellValue(sh, startRow, Data.IrrSystem);
            startRow++;
            sh = SetCellValue(sh, startRow, Data.Work);

            startRow++;
            sh = SetCellValue(sh, startRow, Data.Planning);
            startRow++;
            sh = SetCellValue(sh, startRow, Data.RegulatedFac);
            sh.Cells[startRow, 7].Value = "";
            startRow++;
            sh = SetCellValue(sh, startRow, Data.Engine);
            startRow++;
            sh.Cells[startRow, 1].Value = " E.蓄水池( " + Data.PoolWei + " )噸";
            sh = SetCellValue(sh, startRow, Data.Pool);
            startRow++;
            sh.Cells[startRow, 8].Value = Data.Total;

            startRow++;
            sh.Cells[startRow, 8].Value = Data.FarmerPay;

            #region 二次申請判斷
            if (FarmData.Any(m => m.IsApplied == true) == true)
            {
                sh.Cells[startRow, 3].Value = ">=(A) x 40%";
                //sh.Cells[startRow, 3].Value = "<(A) x 40% + (C+D+E)";
            }
            #endregion

            startRow++;
            sh.Cells[startRow, 8].Value = Data.GovPay_Pay;
            startRow++;
            sh.Cells[startRow, 8].Value = Data.LiuPay;
            startRow++;
            sh.Cells[startRow, 8].Value = Data.GoldPay;

            startRow++;
            sh.Cells[startRow, 8].Value = Data.GovPay_Planning;
            startRow++;
            sh.Cells[startRow, 8].Value = Data.GovPay_Sum;
            startRow++;
            sh.Cells[startRow, 8].Value = Data.Total;
            startRow++;
            sh.Cells[startRow, 3].Value = "新台幣 " + Data.ChineseMoney + "元整";

            using (var range = sh.Cells[11+OffSetRow, 9, 29+OffSetRow, 9])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.WrapText = true;
                //range.AutoFitColumns();
            }
            return sh;
        }

        #region 領款收據
        public ExcelWorksheet PayReceipt(ExcelWorksheet sh, BudgetBookView Data, int MNo)
        {
            
            FarmerData farmer = db.GetFarmerData(MNo);

            sh.Cells[120, 9].Value = Data.IANum;

            string chmoney = new NumberToChinese().GetChineseNumber(int.Parse(Data.GovPay_Pay.Replace(",", ""))) + "元整";
            if (int.Parse(Data.GovPay_Pay.Replace(",", "")) > 0)
                sh.Cells[122, 3].Value = chmoney;

            sh.Cells[130, 3].Value = Data.Name;

            sh.Cells[134, 3].Value = Data.Address;

            sh.Cells[136, 4].Value = farmer.IdNo;

            
            chmoney = new NumberToChinese().GetChineseNumber(int.Parse(Data.LiuPay.Replace(",", ""))) + "元整";
            sh.Cells[156, 9].Value = Data.IANum;
            if (int.Parse(Data.LiuPay.Replace(",", "")) > 0)
                sh.Cells[158, 3].Value = chmoney;

            sh.Cells[166, 3].Value = Data.Name;

            sh.Cells[170, 3].Value = Data.Address;

            sh.Cells[172, 4].Value = farmer.IdNo;

            
            chmoney = new NumberToChinese().GetChineseNumber(int.Parse(Data.GoldPay.Replace(",", ""))) + "元整";
            sh.Cells[191, 9].Value = Data.IANum;
            if (int.Parse(Data.GoldPay.Replace(",", "")) > 0)
                sh.Cells[193, 3].Value = chmoney;

            sh.Cells[201, 3].Value = Data.Name;

            sh.Cells[205, 3].Value = Data.Address;

            sh.Cells[207, 4].Value = farmer.IdNo;

            
            sh.Cells[227, 9].Value = Data.IANum;

            sh.Cells[237, 3].Value = Data.Name;

            sh.Cells[241, 3].Value = Data.Address;

            sh.Cells[243, 4].Value = farmer.IdNo;

            return sh;
        }
        #endregion

      

        #region Write Cell Value
        public ExcelWorksheet SetCellValue(ExcelWorksheet sh, int CorrectRow, BudgetBookstruct bbstruct)
        {
            if (bbstruct != null)
            {
                sh.Cells[CorrectRow, 6].Value = bbstruct.ItemAmount;
                sh.Cells[CorrectRow, 7].Value = bbstruct.ItemPrice;
                sh.Cells[CorrectRow, 8].Value = bbstruct.TotalPrice;

                if (!bbstruct.Memo.Equals(""))
                {
                    string memo = bbstruct.Memo;
                    //if ((CorrectRow >= 18 && CorrectRow <= 20) && bbstruct.Memo != "")
                    if (bbstruct.Memo != "" && bbstruct.Memo.Contains("#"))
                    {
                        memo = string.Empty;
                        int h = 0;
                        foreach (var engcnt in bbstruct.Memo.Split('#'))
                        {
                            memo += engcnt + "\r\n";
                            h += 11;
                        }
                        //sh.Column(9).Width = 16;
                        sh.Row(CorrectRow).Height = h;
                    }
                    sh.Cells[CorrectRow, 9].Value = memo;
                    sh.Cells[CorrectRow, 9].Style.WrapText = true;
                }
            }
            return sh;
        }
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
    }
}
