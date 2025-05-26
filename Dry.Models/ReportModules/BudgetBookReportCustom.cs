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
    public class BudgetBookReportCustom //客製化工程預算書
    {
        #region Parameter
        private int _MNo;
        private BudgetBookDBService db = new BudgetBookDBService();
        private SubsidyReportDBService SRdb = new SubsidyReportDBService();
        private FarmLandDBService farmlanddb = new FarmLandDBService();
        private GetData getDataCls = new GetData();
        #endregion
        public byte[] ExportExcelCutom(int mapno)
        {
            _MNo = mapno;
            BudgetBookView Data = db.GetBudgetBookData(_MNo);
            Int16 unit = Data.ApplyUnit;

            string sample_Path = "";
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
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_8_VB.xls";
                    break;
                case 9:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_9.xlsx";
                    break;
                case 10:
                    sample_Path = @"~/ReportSample/BudgetBook_Sample_10_VB.xls";
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
                case 19:
                sample_Path = @"~/ReportSample/BudgetBook_Sample_19.xls";
                break;
            }
            //開檔
            FileStream fs = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);

            #region 客制預算書(Microsoft)

            #region 澎湖
            if (unit == 19)
            {
                
                Microsoft.Office.Interop.Excel._Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlBook;
                Microsoft.Office.Interop.Excel.Worksheet xlSheet1;
                Microsoft.Office.Interop.Excel.Worksheet xlSheet2;
                Microsoft.Office.Interop.Excel.Worksheet xlSheet4;
                Microsoft.Office.Interop.Excel.Range xlRange;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                xlBook = xlApp.Workbooks.Open(System.Web.HttpContext.Current.Server.MapPath(sample_Path), 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["工程預算書"];
                xlSheet2 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["2.管路灌溉系統材料數量表"];
                //xlSheet4 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["9.補助款收據"];
                xlSheet1.Activate();

                #region 基本資料
                int StartCol = 3;
                int EndCol = 4;

                int StartRow = 4;
                int EndRow = StartRow;

                xlSheet1.Cells[EndRow, EndCol] = Data.Name;

                xlSheet1.Cells[EndRow, EndCol + 3] = Data.IANum;
                //sh.Cells[EndRow, EndCol + 3].AutoFitColumns();
                xlRange = xlSheet1.Range[xlSheet1.Cells[EndRow, EndCol + 3], xlSheet1.Cells[EndRow, EndCol + 3]];
                //xlRange.Columns.AutoFit();

                EndRow++;
                xlSheet1.Cells[EndRow, EndCol] = Data.Address;

                EndRow++;
                string[] check = Data.Farm[0].full_sectName.ToString().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                string sectname = "";
                foreach (string date in check)
                {
                    sectname += date;
                }
                string sectionMsg = Data.Farm.Count <= 0 ? "" : sectname + "" + Data.Farm[0].LandNo + " 地號,等 " + Data.Farm.Count + "筆土地。";


                xlSheet1.Cells[EndRow, EndCol] = sectionMsg;

                EndRow++;
                xlSheet1.Cells[EndRow, EndCol] = ((double)Data.BuildArea / 10000) + "  公頃";

                EndRow++;
                if (Data.EndType == "")
                {
                    xlSheet1.Cells[EndRow, EndCol] = "其它";
                }
                else
                    xlSheet1.Cells[EndRow, EndCol] = Data.EndType;

                xlRange = xlSheet1.Range[xlSheet1.Cells[StartRow, StartCol], xlSheet1.Cells[EndRow, EndCol]];

                xlRange.WrapText = true;
                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                xlRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                #endregion

                #region 設施項目表格

                
                xlSheet1.Cells[11, 8] = Data.PipingTotal;
                
                xlSheet1.Cells[12, 8] = Data.PipingMatMoney;

                
                xlSheet1 = SetNTCellValue(xlSheet1, 13, Data.L1);
                xlSheet1.Cells[13, 9] = "(L1)" + Data.L1_length + "公尺";
                
                if (Data.L2 != null && Convert.ToInt16(Data.L2.ItemAmount) > 0)
                {
                    xlSheet1 = SetNTCellValue(xlSheet1, 14, Data.L2);
                }
                xlSheet1.Cells[14, 9] = "(L2)" + Data.L2_length + "公尺";
                
                xlSheet1 = SetNTCellValue(xlSheet1, 15, Data.IrrSystem);
                
                xlSheet1 = SetNTCellValue(xlSheet1, 16, Data.Work);

                
                xlSheet1 = SetNTCellValue(xlSheet1, 17, Data.Planning);
                
                xlSheet1 = SetNTCellValue(xlSheet1, 18, Data.RegulatedFac);
                xlSheet1.Cells[18, 7] = "";
                
                xlSheet1 = SetNTCellValue(xlSheet1, 19, Data.Engine);
                
                xlSheet1.Cells[20, 1] = " E.蓄水池( " + Data.PoolWei + " )噸";
                xlSheet1 = SetNTCellValue(xlSheet1, 20, Data.Pool);

                
                xlSheet1.Cells[21, 8] = Data.Total;

                
                xlSheet1.Cells[22, 8] = Data.FarmerPay;

                
                #region 二次申請判斷
                if (FarmData.Any(m => m.IsApplied == true) == true)
                {
                    xlSheet1.Cells[22, 3] = ">=(A+C) x 60%";
                    xlSheet1.Cells[23, 3] = "<(A+C) x 40% + (D+E)";
                }
                #endregion
                xlSheet1.Cells[23, 8] = Data.GovPay_Pay;
                
                xlSheet1.Cells[24, 8] = Data.GovPay_Planning;
                
                xlSheet1.Cells[25, 8] = Data.GovPay_Sum;

                xlSheet1.Cells[26, 3] = "新台幣 " + Data.ChineseMoney + "元整";

                xlRange = xlSheet1.Range[xlSheet1.Cells[11, 9], xlSheet1.Cells[25, 10]];// get_Range(StartRow, StartCol, EndRow, EndCol)//fromRow, fromCol, toRow, toCol

                xlRange.WrapText = true;
                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                xlRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                #endregion

                #region 材料數量表

                int row = 2;
                xlSheet2.Cells[row, 1] = "設施型式：" + Data.EndType;
                xlSheet2.Cells[row + 1, 1] = "坵塊型狀：" + (Data.Block.Length > 0 ? Data.Block.Split('x')[0] + "m ×" + Data.Block.Split('x')[1] + "m" : "");
                xlSheet2.Cells[row + 2, 1] = "噴頭配置間距(SS×SL)：" + Data.SS + "×" + Data.SL;
                row += 4;
                int ItemIndex = 1;
                if (db.GetFarMat(_MNo) != null)
                {
                    if (db.GetFarMat(_MNo).Any(m => m.moduleno == 0))
                    {
                        foreach (var mat in db.GetFarMat(_MNo))
                        {
                            xlSheet2.Cells[row, 2] = mat.matname;
                            xlSheet2.Cells[row, 4] = mat.spec;
                            xlSheet2.Cells[row, 6] = mat.itemunit;
                            xlSheet2.Cells[row, 7] = mat.matprice;
                            xlSheet2.Cells[row, 8] = mat.amount;
                            //xlRange = xlSheet1.get_Range(xlSheet1.Cells[row, 9]);
                            //xlRange.Formula = "G" + row + "*" + "H" + row;
                            xlSheet2.Cells[row, 9] = "=" + "G" + row + "*" + "H" + row;
                            //xlSheet1.Cells[row, 9].Formula = "G" + row + "*" + "H" + row;

                            row++;
                        }
                        xlSheet2.Cells[row + 1, 1] = "總　價";
                        xlSheet2.Cells[row + 1, 9] = Data.IrrSystem.TotalPrice;
                        //xlSheet1.Cells[row + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    else
                    {
                        foreach (var mat in db.GetFarMat(_MNo).GroupBy(m => m.groupno).Select(o => new { Grouop = o.Key }))
                        {

                            xlSheet2.Cells[row, 1] = ItemIndex + ". " + db.GetFarMat(_MNo).Where(m => m.groupno == mat.Grouop).FirstOrDefault().group;
                            xlRange = xlSheet2.get_Range(xlSheet2.Cells[row, 2], xlSheet2.Cells[row, 3]);
                            xlRange.Merge(true);
                            //xlSheet1.Cells[row, 2, row, 3].Merge = false;
                            xlRange = xlSheet2.get_Range(xlSheet2.Cells[row, 4], xlSheet2.Cells[row, 5]);
                            xlRange.Merge(true);
                            //xlSheet1.Cells[row, 4, row, 5].Merge = false;
                            xlRange = xlSheet2.get_Range(xlSheet2.Cells[row, 1], xlSheet2.Cells[row, 9]);
                            xlRange.Merge(true);
                            //xlSheet1.Cells[row, 1, row, 9].Merge = true;
                            row++;
                            foreach (var item in db.GetFarMat(_MNo).Where(m => m.groupno == mat.Grouop))
                            {
                                xlSheet2.Cells[row, 1] = ItemIndex + "-" + item.order;
                                xlSheet2.Cells[row, 2] = item.matname;
                                xlSheet2.Cells[row, 4] = item.spec;
                                xlSheet2.Cells[row, 6] = item.itemunit;
                                xlSheet2.Cells[row, 7] = item.matprice;
                                xlSheet2.Cells[row, 8] = item.amount;
                                //xlRange = xlSheet1.get_Range(xlSheet1.Cells[row, 9], misValue);
                                //xlRange.Formula = "G" + row + "*" + "H" + row;
                                xlSheet2.Cells[row, 9] = "=" + "G" + row + "*" + "H" + row;
                                //xlSheet1.Cells[row, 9].Formula = "G" + row + "*" + "H" + row;
                                row++;
                            }
                            ItemIndex++;
                        }
                        xlSheet2.Cells[row + 1, 1] = "總　價";
                        xlSheet2.Cells[row + 1, 9] = Data.IrrSystem.TotalPrice;
                        //xlSheet1.Cells[row + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                }

                #endregion

                #region 補助款收據
                //xlSheet4 = PayReceipt(xlSheet4, Data, _MNo);
                #endregion

                //xlSheet1.Cells[9, 4] = DateTime.Now.ToString();
                xlBook.Saved = true;
                string export_name = "";
                export_name = Guid.NewGuid().ToString();
                xlBook.SaveCopyAs(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"));
                xlBook.Close();
                xlApp.Quit();
                xlSheet1 = null;
                xlSheet2 = null;
                //xlSheet4 = null;
                xlApp = null;

                FileStream fstream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"), FileMode.Open);
                byte[] data = new Byte[fstream.Length];
                ////Obtain the file into the array of bytes from streams.
                fstream.Read(data, 0, data.Length);
                fstream.Close();
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls")))
                {
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"));
                }

                //return File(data, "application/vnd.ms-excel", FileName);
                return data;
            }
            #endregion

            #region 南投
            if (unit == 8)
            {
                
                Microsoft.Office.Interop.Excel._Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlBook;
                Microsoft.Office.Interop.Excel.Worksheet xlSheet1;
                Microsoft.Office.Interop.Excel.Worksheet xlSheet2;
                Microsoft.Office.Interop.Excel.Worksheet xlSheet4;
                Microsoft.Office.Interop.Excel.Range xlRange;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                xlBook = xlApp.Workbooks.Open(System.Web.HttpContext.Current.Server.MapPath(sample_Path), 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["工程預算書"];
                xlSheet2 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["2.管路灌溉系統材料數量表"];
                xlSheet4 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["9.補助款收據"];
                xlSheet1.Activate();

                #region 基本資料
                int StartCol = 3;
                int EndCol = 4;

                int StartRow = 4;
                int EndRow = StartRow;

                xlSheet1.Cells[EndRow, EndCol] = Data.Name;

                xlSheet1.Cells[EndRow, EndCol + 3] = Data.IANum;
                //sh.Cells[EndRow, EndCol + 3].AutoFitColumns();
                xlRange = xlSheet1.Range[xlSheet1.Cells[EndRow, EndCol + 3], xlSheet1.Cells[EndRow, EndCol + 3]];
                //xlRange.Columns.AutoFit();

                EndRow++;
                xlSheet1.Cells[EndRow, EndCol] = Data.Address;

                EndRow++;
                string[] check = Data.Farm[0].full_sectName.ToString().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                string sectname = "";
                foreach (string date in check)
                {
                    sectname += date;
                }
                string sectionMsg = Data.Farm.Count <= 0 ? "" : sectname + "" + Data.Farm[0].LandNo + " 地號,等 " + Data.Farm.Count + "筆土地。";


                xlSheet1.Cells[EndRow, EndCol] = sectionMsg;

                EndRow++;
                xlSheet1.Cells[EndRow, EndCol] = ((double)Data.BuildArea / 10000) + "  公頃";

                EndRow++;
                if (Data.EndType == "")
                {
                    xlSheet1.Cells[EndRow, EndCol] = "其它";
                }
                else
                    xlSheet1.Cells[EndRow, EndCol] = Data.EndType;

                xlRange = xlSheet1.Range[xlSheet1.Cells[StartRow, StartCol], xlSheet1.Cells[EndRow, EndCol]];

                xlRange.WrapText = true;
                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                xlRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                #endregion

                #region 設施項目表格

                
                xlSheet1.Cells[11, 8] = Data.PipingTotal;
                
                xlSheet1.Cells[12, 8] = Data.PipingMatMoney;

                
                xlSheet1 = SetNTCellValue(xlSheet1, 13, Data.L1);
                xlSheet1.Cells[13, 9] = "(L1)" + Data.L1_length + "公尺";
                
                if (Data.L2 != null && Convert.ToInt16(Data.L2.ItemAmount) > 0)
                {
                    xlSheet1 = SetNTCellValue(xlSheet1, 14, Data.L2);
                }
                xlSheet1.Cells[14, 9] = "(L2)" + Data.L2_length + "公尺";
                
                xlSheet1 = SetNTCellValue(xlSheet1, 15, Data.IrrSystem);
                
                xlSheet1 = SetNTCellValue(xlSheet1, 16, Data.Work);

                
                xlSheet1 = SetNTCellValue(xlSheet1, 17, Data.Planning);
                
                xlSheet1 = SetNTCellValue(xlSheet1, 18, Data.RegulatedFac);
                xlSheet1.Cells[18, 7] = "";
                
                xlSheet1 = SetNTCellValue(xlSheet1, 19, Data.Engine);
                
                xlSheet1.Cells[20, 1] = " E.蓄水池( " + Data.PoolWei + " )噸";
                xlSheet1 = SetNTCellValue(xlSheet1, 20, Data.Pool);

                
                xlSheet1.Cells[21, 8] = Data.Total;

                
                xlSheet1.Cells[22, 8] = Data.FarmerPay;

                
                #region 二次申請判斷
                if (FarmData.Any(m => m.IsApplied == true) == true)
                {
                    xlSheet1.Cells[22, 3] = ">=(A+C) x 60%";
                    xlSheet1.Cells[23, 3] = "<(A+C) x 40% + (D+E)";
                }
                #endregion
                xlSheet1.Cells[23, 8] = Data.GovPay_Pay;
                
                xlSheet1.Cells[24, 8] = Data.GovPay_Planning;
                
                xlSheet1.Cells[25, 8] = Data.GovPay_Sum;

                xlSheet1.Cells[26, 3] = "新台幣 " + Data.ChineseMoney + "元整";

                xlRange = xlSheet1.Range[xlSheet1.Cells[11, 9], xlSheet1.Cells[25, 10]];// get_Range(StartRow, StartCol, EndRow, EndCol)//fromRow, fromCol, toRow, toCol

                xlRange.WrapText = true;
                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                xlRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                #endregion

                #region 材料數量表

                int row = 2;
                xlSheet2.Cells[row, 1] = "設施型式：" + Data.EndType;
                xlSheet2.Cells[row + 1, 1] = "坵塊型狀：" + (Data.Block.Length > 0 ? Data.Block.Split('x')[0] + "m ×" + Data.Block.Split('x')[1] + "m" : "");
                xlSheet2.Cells[row + 2, 1] = "噴頭配置間距(SS×SL)：" + Data.SS + "×" + Data.SL;
                row += 4;
                int ItemIndex = 1;
                if (db.GetFarMat(_MNo) != null)
                {
                    if (db.GetFarMat(_MNo).Any(m => m.moduleno == 0))
                    {
                        foreach (var mat in db.GetFarMat(_MNo))
                        {
                            xlSheet2.Cells[row, 2] = mat.matname;
                            xlSheet2.Cells[row, 4] = mat.spec;
                            xlSheet2.Cells[row, 6] = mat.itemunit;
                            xlSheet2.Cells[row, 7] = mat.matprice;
                            xlSheet2.Cells[row, 8] = mat.amount;
                            //xlRange = xlSheet1.get_Range(xlSheet1.Cells[row, 9]);
                            //xlRange.Formula = "G" + row + "*" + "H" + row;
                            xlSheet2.Cells[row, 9] = "=" + "G" + row + "*" + "H" + row;
                            //xlSheet1.Cells[row, 9].Formula = "G" + row + "*" + "H" + row;

                            row++;
                        }
                        xlSheet2.Cells[row + 1, 1] = "總　價";
                        xlSheet2.Cells[row + 1, 9] = Data.IrrSystem.TotalPrice;
                        //xlSheet1.Cells[row + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    else
                    {
                        foreach (var mat in db.GetFarMat(_MNo).GroupBy(m => m.groupno).Select(o => new { Grouop = o.Key }))
                        {

                            xlSheet2.Cells[row, 1] = ItemIndex + ". " + db.GetFarMat(_MNo).Where(m => m.groupno == mat.Grouop).FirstOrDefault().group;
                            xlRange = xlSheet2.get_Range(xlSheet2.Cells[row, 2], xlSheet2.Cells[row, 3]);
                            xlRange.Merge(true);
                            //xlSheet1.Cells[row, 2, row, 3].Merge = false;
                            xlRange = xlSheet2.get_Range(xlSheet2.Cells[row, 4], xlSheet2.Cells[row, 5]);
                            xlRange.Merge(true);
                            //xlSheet1.Cells[row, 4, row, 5].Merge = false;
                            xlRange = xlSheet2.get_Range(xlSheet2.Cells[row, 1], xlSheet2.Cells[row, 9]);
                            xlRange.Merge(true);
                            //xlSheet1.Cells[row, 1, row, 9].Merge = true;
                            row++;
                            foreach (var item in db.GetFarMat(_MNo).Where(m => m.groupno == mat.Grouop))
                            {
                                xlSheet2.Cells[row, 1] = ItemIndex + "-" + item.order;
                                xlSheet2.Cells[row, 2] = item.matname;
                                xlSheet2.Cells[row, 4] = item.spec;
                                xlSheet2.Cells[row, 6] = item.itemunit;
                                xlSheet2.Cells[row, 7] = item.matprice;
                                xlSheet2.Cells[row, 8] = item.amount;
                                //xlRange = xlSheet1.get_Range(xlSheet1.Cells[row, 9], misValue);
                                //xlRange.Formula = "G" + row + "*" + "H" + row;
                                xlSheet2.Cells[row, 9] = "=" + "G" + row + "*" + "H" + row;
                                //xlSheet1.Cells[row, 9].Formula = "G" + row + "*" + "H" + row;
                                row++;
                            }
                            ItemIndex++;
                        }
                        xlSheet2.Cells[row + 1, 1] = "總　價";
                        xlSheet2.Cells[row + 1, 9] = Data.IrrSystem.TotalPrice;
                        //xlSheet1.Cells[row + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                }

                #endregion

                #region 補助款收據
                xlSheet4 = PayReceipt(xlSheet4, Data, _MNo);
                #endregion

                //xlSheet1.Cells[9, 4] = DateTime.Now.ToString();
                xlBook.Saved = true;
                string export_name = "";
                export_name = Guid.NewGuid().ToString();
                xlBook.SaveCopyAs(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"));
                xlBook.Close();
                xlApp.Quit();
                xlSheet1 = null;
                xlSheet2 = null;
                xlSheet4 = null;
                xlApp = null;

                FileStream fstream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"), FileMode.Open);
                byte[] data = new Byte[fstream.Length];
                ////Obtain the file into the array of bytes from streams.
                fstream.Read(data, 0, data.Length);
                fstream.Close();
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls")))
                {
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"));
                }

                //return File(data, "application/vnd.ms-excel", FileName);
                return data;
            }
            #endregion
            #region 雲林會
            if (unit == 10 & (Data.Gold != true))
            {
                
                Microsoft.Office.Interop.Excel._Application xlApp;
                Microsoft.Office.Interop.Excel.Workbook xlBook;
                Microsoft.Office.Interop.Excel.Worksheet xlSheet1;
                Microsoft.Office.Interop.Excel.Worksheet xlSheet2;
                Microsoft.Office.Interop.Excel.Worksheet xlSheet3;
                Microsoft.Office.Interop.Excel.Worksheet xlSheet4;

                Microsoft.Office.Interop.Excel.Range xlRange;
                object misValue = System.Reflection.Missing.Value;

                xlApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                xlBook = xlApp.Workbooks.Open(System.Web.HttpContext.Current.Server.MapPath(sample_Path), 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlSheet1 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["工程預算書"];
                xlSheet2 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["工程預算書(無承判)"];
                xlSheet3 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["2.管路灌溉系統材料數量表"];
                xlSheet4 = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets["9.補助款收據"];

                xlSheet1.Activate();
                xlSheet3.Activate();

                #region 基本資料
                xlSheet1 = CustomBasicTable(xlSheet1, Data);
                xlSheet2 = CustomBasicTable(xlSheet2, Data);
                #endregion

                #region 設施項目表格
                xlSheet1 = CustomSetbudgetBookTable(xlSheet1, Data);
                xlSheet2 = CustomSetbudgetBookTable(xlSheet2, Data);
                #endregion

                #region 材料數量表
                xlSheet3 = CustomSetCoaMatTable(xlSheet3, Data, db.GetFarMat(_MNo));
                #endregion

                #region 補助款收據
                xlSheet4 = PayReceipt(xlSheet4,Data, _MNo);
                #endregion

                //xlSheet1.Cells[9, 4] = DateTime.Now.ToString();
                xlBook.Saved = true;
                string export_name = "";
                export_name = Guid.NewGuid().ToString();
                xlBook.SaveCopyAs(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"));
                xlBook.Close();
                xlApp.Quit();
                xlSheet1 = null;
                xlSheet2 = null;
                xlSheet3 = null;
                xlSheet4 = null;
                xlApp = null;

                FileStream fstream = new FileStream(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"), FileMode.Open);
                byte[] data = new Byte[fstream.Length];
                ////Obtain the file into the array of bytes from streams.
                fstream.Read(data, 0, data.Length);
                fstream.Close();
                if (System.IO.File.Exists(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls")))
                {
                    System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath(@"~/ReportSample/" + export_name + ".xls"));
                }
                //return File(data, "application/vnd.ms-excel", FileName);
                return data;
            }
            #endregion
            #endregion

            
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
            

            fs.Close();
            byte[] file = excel.GetAsByteArray();
            return file;
        }
        #region 瑠公
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


            using (var range = sh.Cells[6, 6, 15, 7])//fromRow, fromCol, toRow, toCol 11 8 25 8
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
        #region 設定瑠公 Cell Value
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
        #endregion

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

            using (var range = sh.Cells[StartRow, StartCol, EndRow, EndCol])
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                //range.AutoFitColumns();
                range.Style.WrapText = true;
            }

            return sh;
        }
        public Microsoft.Office.Interop.Excel.Worksheet CustomBasicTable(Microsoft.Office.Interop.Excel.Worksheet sh, BudgetBookView Data)
        {
            Microsoft.Office.Interop.Excel.Range xlRange;
            int StartCol = 3;
            int EndCol = 4;

            int StartRow = 4;
            int EndRow = StartRow;

            sh.Cells[EndRow, EndCol] = Data.Name;

            sh.Cells[EndRow, EndCol + 3] = Data.IANum;
            //sh.Cells[EndRow, EndCol + 3].AutoFitColumns();
            xlRange = sh.Range[sh.Cells[EndRow, EndCol + 3], sh.Cells[EndRow, EndCol + 3]];
            //xlRange.Columns.AutoFit();

            EndRow++;
            sh.Cells[EndRow, EndCol] = Data.Address;

            EndRow++;
            string[] check = Data.Farm[0].full_sectName.ToString().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            string sectname = "";
            foreach (string date in check)
            {
                sectname += date;
            }
            string sectionMsg = Data.Farm.Count <= 0 ? "" : sectname + "" + Data.Farm[0].LandNo + " 地號,等 " + Data.Farm.Count + "筆土地。";


            sh.Cells[EndRow, EndCol] = sectionMsg;

            EndRow++;
            sh.Cells[EndRow, EndCol] = ((double)Data.BuildArea / 10000) + "  公頃";

            EndRow++;
            if (Data.EndType == "")
            {
                sh.Cells[EndRow, EndCol] = "其它";
            }
            else
                sh.Cells[EndRow, EndCol] = Data.EndType;

            xlRange = sh.Range[sh.Cells[StartRow, StartCol], sh.Cells[EndRow, EndCol]];

            xlRange.WrapText = true;
            xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            xlRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

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
        public Microsoft.Office.Interop.Excel.Worksheet CustomSetCoaMatTable(Microsoft.Office.Interop.Excel.Worksheet sh, BudgetBookView Data, List<GetData.SysMat> matdata)
        {
            Microsoft.Office.Interop.Excel.Range xlRange;
            int row = 2;
            sh.Cells[row, 1] = "設施型式：" + Data.EndType;
            sh.Cells[row + 1, 1] = "坵塊型狀：" + (Data.Block.Length > 0 ? Data.Block.Split('x')[0] + "m ×" + Data.Block.Split('x')[1] + "m" : "");
            sh.Cells[row + 2, 1] = "噴頭配置間距(SS×SL)：" + Data.SS + "×" + Data.SL;
            row += 4;
            int ItemIndex = 1;
            if (matdata != null)
            {
                if (matdata.Any(m => m.moduleno == 0))
                {
                    foreach (var mat in matdata)
                    {
                        sh.Cells[row, 2] = mat.matname;
                        sh.Cells[row, 4] = mat.spec;
                        sh.Cells[row, 6] = mat.itemunit;
                        sh.Cells[row, 7] = mat.matprice;
                        sh.Cells[row, 8] = mat.amount;
                        //xlRange = xlSheet1.get_Range(xlSheet1.Cells[row, 9]);
                        //xlRange.Formula = "G" + row + "*" + "H" + row;
                        sh.Cells[row, 9] = "=" + "G" + row + "*" + "H" + row;
                        //xlSheet1.Cells[row, 9].Formula = "G" + row + "*" + "H" + row;

                        row++;
                    }
                    sh.Cells[row + 1, 1] = "總　價";
                    sh.Cells[row + 1, 9] = Data.IrrSystem.TotalPrice;
                    //xlSheet1.Cells[row + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }
                else
                {
                    foreach (var mat in matdata.GroupBy(m => m.groupno).Select(o => new { Grouop = o.Key }))
                    {

                        sh.Cells[row, 1] = ItemIndex + ". " + matdata.Where(m => m.groupno == mat.Grouop).FirstOrDefault().group;
                        xlRange = sh.get_Range(sh.Cells[row, 2], sh.Cells[row, 3]);
                        xlRange.Merge(true);
                        //xlSheet1.Cells[row, 2, row, 3].Merge = false;
                        xlRange = sh.get_Range(sh.Cells[row, 4], sh.Cells[row, 5]);
                        xlRange.Merge(true);
                        //xlSheet1.Cells[row, 4, row, 5].Merge = false;
                        xlRange = sh.get_Range(sh.Cells[row, 1], sh.Cells[row, 9]);
                        xlRange.Merge(true);
                        //xlSheet1.Cells[row, 1, row, 9].Merge = true;
                        row++;
                        foreach (var item in matdata.Where(m => m.groupno == mat.Grouop))
                        {
                            sh.Cells[row, 1] = ItemIndex + "-" + item.order;
                            sh.Cells[row, 2] = item.matname;
                            sh.Cells[row, 4] = item.spec;
                            sh.Cells[row, 6] = item.itemunit;
                            sh.Cells[row, 7] = item.matprice;
                            sh.Cells[row, 8] = item.amount;
                            //xlRange = xlSheet1.get_Range(xlSheet1.Cells[row, 9], misValue);
                            //xlRange.Formula = "G" + row + "*" + "H" + row;
                            sh.Cells[row, 9] = "=" + "G" + row + "*" + "H" + row;
                            //xlSheet1.Cells[row, 9].Formula = "G" + row + "*" + "H" + row;
                            row++;
                        }
                        ItemIndex++;
                    }
                    sh.Cells[row + 1, 1] = "總　價";
                    sh.Cells[row + 1, 9] = Data.IrrSystem.TotalPrice;
                    //xlSheet1.Cells[row + 1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
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
                    return 38;
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
            
            _MNo = mapno;
            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);//取該筆農地資料(for 二次申請check)
          
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
            sh.Cells[18, 7].Value = "";
            
            sh = SetCellValue(sh, 19, Data.Engine);
            
            sh.Cells[20, 1].Value = " E.蓄水池( " + Data.PoolWei + " )噸";
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
        public Microsoft.Office.Interop.Excel.Worksheet CustomSetbudgetBookTable(Microsoft.Office.Interop.Excel.Worksheet sh, BudgetBookView Data)
        {
            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);
            Microsoft.Office.Interop.Excel.Range xlRange;
            
            sh.Cells[11, 8] = Data.PipingTotal;
            
            sh.Cells[12, 8] = Data.PipingMatMoney;

            
            sh = SetNTCellValue(sh, 13, Data.L1);
            sh.Cells[13, 9] = "(L1)" + Data.L1_length + "公尺";
            
            if (Data.L2 != null && Convert.ToInt16(Data.L2.ItemAmount) > 0)
            {
                sh = SetNTCellValue(sh, 14, Data.L2);
            }
            sh.Cells[14, 9] = "(L2)" + Data.L2_length + "公尺";
            
            sh = SetNTCellValue(sh, 15, Data.IrrSystem);
            
            sh = SetNTCellValue(sh, 16, Data.Work);

            
            sh = SetNTCellValue(sh, 17, Data.Planning);
            
            sh = SetNTCellValue(sh, 18, Data.RegulatedFac);
            sh.Cells[18, 7] = "";
            
            sh = SetNTCellValue(sh, 19, Data.Engine);
            
            sh.Cells[20, 1] = " E.蓄水池( " + Data.PoolWei + " )噸";
            sh = SetNTCellValue(sh, 20, Data.Pool);

            
            sh.Cells[21, 8] = Data.Total;

            
            sh.Cells[22, 8] = Data.FarmerPay;

            
            #region 二次申請判斷
            if (FarmData.Any(m => m.IsApplied == true) == true)
            {
                sh.Cells[22, 3] = ">=(A+C) x 60%";
                sh.Cells[23, 3] = "<(A+C) x 40% + (D+E)";
            }
            #endregion
            sh.Cells[23, 8] = Data.GovPay_Pay;
            
            sh.Cells[24, 8] = Data.GovPay_Planning;
            
            sh.Cells[25, 8] = Data.GovPay_Sum;

            sh.Cells[26, 3] = "新台幣 " + Data.ChineseMoney + "元整";

            xlRange = sh.Range[sh.Cells[11, 9], sh.Cells[25, 10]];// get_Range(StartRow, StartCol, EndRow, EndCol)//fromRow, fromCol, toRow, toCol

            xlRange.WrapText = true;
            xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            xlRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            return sh;
        }
        #endregion

        #region 領款收據
        public Microsoft.Office.Interop.Excel.Worksheet PayReceipt(Microsoft.Office.Interop.Excel.Worksheet sh, BudgetBookView Data, int MNo)
        {
            FarmerData farmer = db.GetFarmerData(MNo);

            Microsoft.Office.Interop.Excel.Range xlRange;

            sh.Cells[3, 9] = Data.IANum;

            sh.Cells[5, 3] = Data.ChineseMoney;

            sh.Cells[13, 3] = Data.Name;

            sh.Cells[17, 3] = Data.Address;

            sh.Cells[19, 4] = farmer.IdNo;
            return sh;
        }
        #endregion

        #region 設定Cell Value
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
                        sh.Row(CorrectRow).Height = h;
                    }
                    sh.Cells[CorrectRow, 9].Value = memo;
                    sh.Cells[CorrectRow, 9].Style.WrapText = true;
                }
            }
            return sh;
        }
        public Microsoft.Office.Interop.Excel.Worksheet SetNTCellValue(Microsoft.Office.Interop.Excel.Worksheet sh, int CorrectRow, BudgetBookstruct bbstruct)
        {
            Microsoft.Office.Interop.Excel.Range xlRange;
            if (bbstruct != null)
            {
                sh.Cells[CorrectRow, 6] = bbstruct.ItemAmount;
                sh.Cells[CorrectRow, 7] = bbstruct.ItemPrice;
                sh.Cells[CorrectRow, 8] = bbstruct.TotalPrice;

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
                        xlRange = sh.Range[sh.Cells[CorrectRow, 9], sh.Cells[CorrectRow, 9]];
                        xlRange.RowHeight = h;
                        //xlRange.ColumnWidth = 16;
                    }
                    memo = memo.ToString().Substring(0, memo.Length - 1).Trim().TrimEnd("\r\n".ToCharArray());
                    sh.Cells[CorrectRow, 9] = memo;
                    xlRange = sh.Range[sh.Cells[CorrectRow, 9], sh.Cells[CorrectRow, 9]];
                    xlRange.WrapText = true;
                }
            }
            return sh;
        }
        #endregion

    }
}
