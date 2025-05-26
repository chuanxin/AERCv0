/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-6-30
-- Description: 產製工程預算書
-- =============================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using Dry.Models.ReportModules;
using Dry.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;
using Dry.Models.CommonCls;
using System.Runtime.InteropServices;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class BudgetBookController : Controller
    {
        #region Parameter
        private int _MNo;
        private BudgetBookDBService db = new BudgetBookDBService();
        private SubsidyReportDBService SRdb = new SubsidyReportDBService();
        private FarmLandDBService farmlanddb = new FarmLandDBService();
        private GetData getDataCls = new GetData();
        private BudgetBookReport bbr = new BudgetBookReport();
        #endregion
        #region Excel補助清冊
        public ActionResult ExportSubsidy()
        {
            List<SubsidyReportView> Data = new List<SubsidyReportView>();
            byte[] file = SRdb.GetSubsidyReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 補助清冊.xlsx");
        }
        #endregion

        #region Excel管路工程設施輔助明細表
        public ActionResult ExportEngineering()
        {
            List<EngineeringReportView> Data = new List<EngineeringReportView>();
            byte[] file = SRdb.GetEngineeringReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 管路工程設施輔助明細表.xlsx");
        }
        #endregion

        #region Excel匯出管路工程設施面積及輔助金額統計表
        public ActionResult ExportEngineeringCost()
        {
            EngineeringCostReportView Data = new EngineeringCostReportView();
            byte[] file = SRdb.GetEngineeringCostReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 金額統計表" + ".xlsx");
        }
        #endregion

        #region Excel管路灌溉設施工程決算表
        public ActionResult ExportStatement()
        {
            List<StatementReportView> Data = new List<StatementReportView>();
            byte[] file = SRdb.GetStatementReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 管路灌溉設施工程決算表.xlsx");
        }
        #endregion

        #region Excel成果統計表
        public ActionResult ExportStatistics()
        {
            StatisticsReportView Data = new StatisticsReportView();
            byte[] file = SRdb.GetStatisticsReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 成果統計表.xlsx");
        }
        #endregion

        #region Excel農作物統計表
        public ActionResult ExportFarmStatistics()
        {
            List<FarmStatisticsReportView> Data = new List<FarmStatisticsReportView>();
            byte[] file = SRdb.GetFarmStatisticsReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 農作物統計表.xlsx");
        }
        #endregion

        #region Excel歷年鄉鎮統計表
        public ActionResult ExportTown()
        {
            List<TownReportView> Data = new List<TownReportView>();
            byte[] file = SRdb.GetTownReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 歷年鄉鎮統計表.xlsx");
        }
        #endregion

        #region Excel農地筆數統計表
        public ActionResult ExportFarmCount()
        {
            List<FarmCountReportView> Data = new List<FarmCountReportView>();
            byte[] file = SRdb.GetFarmCountReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 農地筆數統計表.xlsx");
        }
        #endregion

        #region Excel歷年受益戶資料清查一覽表
        public ActionResult ExportBenefit()
        {
            BenefitReportView Data = new BenefitReportView();
            byte[] file = SRdb.GetBenefitReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 受益戶資料清查一覽表.xlsx");
        }
        #endregion

        #region Excel普查卡
        public ActionResult ExportSurvey()
        {
            SurveyReportView Data = new SurveyReportView();
            byte[] file = SRdb.GetSurveyReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 普查卡.xlsx");
        }
        #endregion

        #region Excel地籍卡
        public ActionResult ExportLand()
        {
            LandReportView Data = new LandReportView();
            byte[] file = SRdb.GetLandReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 地籍卡.xlsx");
        }
        #endregion

        #region Excel基本資料卡
        public ActionResult ExportPerson()
        {
            PersonReportView Data = new PersonReportView();
            byte[] file = SRdb.GetPersonReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 基本資料卡.xlsx");
        }
        #endregion

        #region Excel加入會員審核表
        public ActionResult ExportExamine()
        {
            ExamineReportView Data = new ExamineReportView();
            byte[] file = SRdb.GetExamineReportData(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 加入會員審核表.xlsx");
        }
        #endregion

        

        public ActionResult Create()
        {
            //Session["MapNo"] = 68;
            if (Session["MapNo"] == null)
                return RedirectToAction("Index", "ApplyIndex");
            _MNo = (int)Session["MapNo"];
            BudgetBookView ModelData = db.GetBudgetBookData(_MNo);
            ModelData.Step = new GetData().GetCaseDataFromMapNo(_MNo).Step; 
            //return View(ModelData);
            return PartialView(ModelData);
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult NextStep()
        {
            _MNo = (int)Session["MapNo"];
            return Json(new CommClass().UpdateCase(_MNo, 8).DbMessage, JsonRequestBehavior.AllowGet);
        }
        #region Download Excel Fun
        [HttpPost]
        public ActionResult ExportPDF()
        {
            _MNo = (int)Session["MapNo"];            
            BudgetBookView Data = db.GetBudgetBookData(_MNo);
            Int16 unit = Data.ApplyUnit;
            //string FileName = Data.ApplyY.ToString() + "-" + Data.IANum.ToString() + "-工程預算書.xlsx";
            string FileName = Data.ApplyY.ToString() + "-" + Data.IANum.ToString() + "-工程預算書.pdf";
            byte[] file;
            #region 客化(舊版)
            /*
            switch (unit) {
                case 4: 
                    file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                case 6: 
                    file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                case 7: 
                    file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                case 8: 
                    file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));
                    FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.xls";
                    return File(file, "application/vnd.ms-excel", FileName);
                case 9: 
                    file = bbr.ExportExcel(_MNo);
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                case 10:

                    if (Data.Gold == true)
                    {
                        file = (new BudgetBookReportGold10().ExportExcel(_MNo));
                        FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.xlsx";
                        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                    }
                    else
                    {
                        FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.xls";
                        file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));

                        return File(file, "application/vnd.ms-excel", FileName);
                    }
                case 12: 
                    file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                case 13: 
                    file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                case 14: 
                    file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                case 15: 
                    file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                case 17: 
                    file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));
                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                case 19: 
                    file = (new BudgetBookReportCustom().ExportExcelCutom(_MNo));
                    FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.xls";
                    return File(file, "application/vnd.ms-excel", FileName);
                default: 
                    //file = bbr.ExportExcel(_MNo);
                    //return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
                    FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.pdf";
                    file = bbr.ExportPdf(_MNo);
                    return File(file, "application/pdf", FileName);
            }
            */

            #endregion

            
            Guid userid = (Guid)Session["User"];            
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} {_MNo}";            
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);            
            file = bbr.ExportPdf(_MNo);
            return File(file, "application/pdf", FileName);

        }
        
        //public ActionResult ExportExcelPDF()
        //{
        //    _MNo = (int)Session["MapNo"];
        //    BudgetBookView Data = db.GetBudgetBookData(_MNo);
        //    Int16 unit = Data.ApplyUnit;
        //    string FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 工程預算書.pdf";
            

        //}
        public ActionResult ExportVerifyBook()
        {
            _MNo = (int)Session["MapNo"];
            BudgetBookView Data = db.GetBudgetBookData(_MNo);
            Int16 unit = Data.ApplyUnit;
            string FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 驗收報告書.xlsx";

            byte[] file = new VerifyBookReport().CreateExcel(Data);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);

        }

        #region 設定瑠公材料數量表

        //public ExcelWorksheet SetLiuMatTable(ExcelWorksheet sh, List<GetData.SysMat> matdata)
        //{
        //    int row = 39;
        //    foreach (var mat in matdata)
        //    {
        //        sh.Cells[row, 1].Value = mat.matname;
        //        sh.Cells[row, 3].Value = mat.spec;
        //        sh.Cells[row, 4].Value = mat.itemunit;
        //        sh.Cells[row, 5].Value = mat.amount;
        //        sh.Cells[row, 6].Value = mat.matprice;
        //        sh.Cells[row, 7].Value = mat.amount * mat.matprice;
        //        sh.Cells[row, 8].Value = mat.description;
        //        row++;
        //    }

        //    /*
        //    using (var range = sh.Cells[39, 1, 59, 7])//fromRow, fromCol, toRow, toCol 11 8 25 8
        //    {
        //        range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //        range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //        range.Style.WrapText = true;
        //    }*/
        //    return sh;
        //}

        #endregion


        #region 材料數量表
        /*程式碼移到 BudgetBookReport
        public ExcelWorksheet SetCoaMatTable(ExcelWorksheet sh,BudgetBookView data, List<GetData.SysMat> matdata)
        {
            int row = GetStartRow((short)Session["UnitID"], data.Gold);
            sh.Cells[row, 1].Value = "設施型式：" + data.EndType;
            sh.Cells[row + 1, 1].Value = "坵塊型狀：" + (data.Block.Length > 0 ? data.Block.Split('x')[0] + "m ×" + data.Block.Split('x')[1] + "m" : "");
            sh.Cells[row + 2, 1].Value = "噴頭配置間距(SS×SL)：" + data.SS + "×" + data.SL;
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
        */

        /*程式碼移到 BudgetBookReport
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
        public int GetStartRow(short unit,bool Gold)
        {
            switch(unit)
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
        */
        #region Write Liu Cell Value
        /*程式碼移到 BudgetBookReport
        public ExcelWorksheet SetLiuCellValue(ExcelWorksheet sh, int CorrectRow, BudgetBookstruct bbstruct)
        {
            if (bbstruct != null)
            {
                if (CorrectRow == 10)//工作費那欄特殊
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
        */
        #endregion

        #region 設定設施項目表格
        /*程式碼移到 BudgetBookReport
        public ExcelWorksheet SetbudgetBookTable(ExcelWorksheet sh, BudgetBookView Data)
        {
            _MNo = (int)Session["MapNo"];
            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);//取該筆農地資料(for 二次申請check)
            //田間管路設施費
            sh.Cells[11, 8].Value = Data.PipingTotal;
            //材料費
            sh.Cells[12, 8].Value = Data.PipingMatMoney;

            //L1
            sh = SetCellValue(sh, 13, Data.L1);
            //L2
            if (Data.L2 != null && Convert.ToInt16(Data.L2.ItemAmount) > 0)
            {
                sh = SetCellValue(sh, 14, Data.L2);
            }
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
            if (FarmData.Any(m=>m.IsApplied == true) == true)
            {
                sh.Cells[22, 3].Value = ">=(A+C) x 60%";
                sh.Cells[23, 3].Value = "<(A+C) x 40% + (D+E)";
            }
            #endregion
            sh.Cells[23, 8].Value = Data.GovPay_Pay;
            //規劃費
            sh.Cells[24, 8].Value = Data.GovPay_Planning;
            //小計
            sh.Cells[25, 8].Value = Data.GovPay_Sum;

            sh.Cells[26, 3].Value = "新台幣 "+ Data.ChineseMoney + "元整";

            using (var range = sh.Cells[11, 9, 25, 10])//fromRow, fromCol, toRow, toCol 11 8 25 8
            {
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.WrapText = true;
                //range.AutoFitColumns();
            }
            return sh;
        }
        */
        /*程式碼移到 BudgetBookReport
        public Microsoft.Office.Interop.Excel.Worksheet CustomSetbudgetBookTable(Microsoft.Office.Interop.Excel.Worksheet sh, BudgetBookView Data)
        {
            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(_MNo);//取該筆農地資料(for 二次申請check)
            Microsoft.Office.Interop.Excel.Range xlRange;
            //田間管路設施費
            sh.Cells[11, 8] = Data.PipingTotal;
            //材料費
            sh.Cells[12, 8] = Data.PipingMatMoney;

            //L1
            sh = SetNTCellValue(sh, 13, Data.L1);
            sh.Cells[13, 9] = "(L1)" + Data.L1_length + "公尺";
            //L2
            if (Data.L2 != null && Convert.ToInt16(Data.L2.ItemAmount) > 0)
            {
                sh = SetNTCellValue(sh, 14, Data.L2);
            }
            sh.Cells[14, 9] = "(L2)" + Data.L2_length + "公尺";
            //IRR 
            sh = SetNTCellValue(sh, 15, Data.IrrSystem);
            //Work
            sh = SetNTCellValue(sh, 16, Data.Work);

            //Planning
            sh = SetNTCellValue(sh, 17, Data.Planning);
            //調控
            sh = SetNTCellValue(sh, 18, Data.RegulatedFac);
            //動力
            sh = SetNTCellValue(sh, 19, Data.Engine);
            //蓄水
            sh.Cells[20, 1] = " E.蓄水池( " + Data.PoolWei + " )噸";
            sh = SetNTCellValue(sh, 20, Data.Pool);

            //合計
            sh.Cells[21, 8] = Data.Total;

            //農戶自備款
            sh.Cells[22, 8] = Data.FarmerPay;

            //農戶請領款
            #region 二次申請判斷
            if (FarmData.Any(m => m.IsApplied == true) == true)
            {
                sh.Cells[22, 3] = ">=(A+C) x 60%";
                sh.Cells[23, 3] = "<(A+C) x 40% + (D+E)";
            }
            #endregion
            sh.Cells[23, 8] = Data.GovPay_Pay;
            //規劃費
            sh.Cells[24, 8] = Data.GovPay_Planning;
            //小計
            sh.Cells[25, 8] = Data.GovPay_Sum;

            sh.Cells[26, 3] = "新台幣 " + Data.ChineseMoney + "元整";

            xlRange = sh.Range[sh.Cells[11, 9], sh.Cells[25, 10]];// get_Range(StartRow, StartCol, EndRow, EndCol)//fromRow, fromCol, toRow, toCol

            xlRange.WrapText = true;
            xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            xlRange.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            return sh;
        }
         * */
        #endregion
        /*程式碼移到 BudgetBookReport
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
                        sh.Row(CorrectRow).Height = h;
                    }
                    sh.Cells[CorrectRow, 9].Value = memo;
                    sh.Cells[CorrectRow, 9].Style.WrapText = true;
                }
            }
            return sh;
        }
        */
        #endregion
        #region oldcode
        /*
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
                    memo = memo.ToString().Substring(0, memo.Length - 1).Trim().TrimEnd("\r\n".ToCharArray());//截取除最后一位的前面所有字符
                    sh.Cells[CorrectRow, 9] = memo ;
                    xlRange = sh.Range[sh.Cells[CorrectRow, 9], sh.Cells[CorrectRow, 9]];
                    xlRange.WrapText = true;
                }
            }
            return sh;
        }//附註
        */
        #endregion
        #region Excel設定
        ////設定保護Sheet的密碼
        //sheet.Protection.SetPassword("1234");
        ////凍結視窗
        //sheet.View.FreezePanes(2, 13);


        ////設定range
        //using (var range = sh.Cells[2, 2, 3, 7])//fromRow, fromCol, toRow, toCol
        //{
        //    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;                
        //    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //    range.Style.Font.Bold = true;               
        //    range.Style.Font.Color.SetColor(Color.Black);
        //    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        //    range.Merge = true;
        //}            
        #endregion
        #region 匯出規劃委託書
        /// <summary>
        /// 匯出規劃委託書WORD檔
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportProxyWord()
        {
            _MNo = (int)Session["MapNo"];

            Dictionary<string, string> dataStr = db.MergeExportProxyData(_MNo);
            
            byte[] Result = new Dry.Models.CommonCls.ExportReports().ExportWord(dataStr, Server.MapPath("~/ReportSample/ExportProxy.docx"));
            return File(Result, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", DateTime.Now.ToString("yyyyMMdd") + " - 規劃委託書.docx");
        }
        /// <summary>
        /// 匯出規劃委託書PDF檔
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportProxyPDF()
        {
            _MNo = (int)Session["MapNo"];
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} {_MNo}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            Dictionary<string, string> dataStr = db.MergeExportProxyData(_MNo);
            
            //byte[] result = new Dry.Models.CommonCls.ExportReports().ExportPDF(dataStr, Server.MapPath("~/ReportSample/ExportProxy.pdf"));
            byte[] result = new Dry.Models.CommonCls.ExportReports().ProxyPDF(dataStr);
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 規劃委託書.pdf");
        }
        #endregion
        #region 匯出接受補助設置旱作管路灌溉設施切結書
        /// <summary>
        /// 匯出切結書WORD檔
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportAcceptAffidavitWord()
        {
            _MNo = (int)Session["MapNo"];
            Dictionary<string, string> dataStr = db.MergeExportAcceptAffidavitData(_MNo);

            
            GetData gd = new GetData();
            Case casedata = gd.GetCaseDataFromMapNo(_MNo);
            if (casedata.ApplyUnit == 9)
            {
                byte[] Result = new Dry.Models.CommonCls.ExportReports().ExportWord(dataStr, Server.MapPath("~/ReportSample/ExportAcceptAffidavit9.docx"));
                return File(Result, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", DateTime.Now.ToString("yyyyMMdd") + " - 接受補助設置旱作管路灌溉設施切結書.docx");
            }
            else
            {
                byte[] Result = new Dry.Models.CommonCls.ExportReports().ExportWord(dataStr, Server.MapPath("~/ReportSample/ExportAcceptAffidavit.docx"));
                return File(Result, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", DateTime.Now.ToString("yyyyMMdd") + " - 接受補助設置旱作管路灌溉設施切結書.docx");
            }
            
        }
        [HttpPost]
        public ActionResult ExportAcceptAffidavitPDF()
        {
            _MNo = (int)Session["MapNo"];
            //Dictionary<string, string> dataStr = db.MergeExportAcceptAffidavitData(_MNo);
            //string CtrlName = "", CtrlAmount = "";
            //foreach (var item in dataStr)
            //{
            //    if (item.Key == "CtrlItem")
            //    {
            //        CtrlName = item.Value.Replace("<w:br />", "\n");

            //    }
            //    if (item.Key == "CtrlAmount")
            //    {
            //        CtrlAmount = item.Value.Replace("<w:br />", Environment.NewLine);
            //    }
            //}
            //dataStr["CtrlItem"] = CtrlName;
            //dataStr["CtrlAmount"] = CtrlAmount;
            
            //byte[] result = new Dry.Models.CommonCls.ExportReports().ExportPDF(dataStr, Server.MapPath("~/ReportSample/ExportAcceptAffidavit.pdf"));

            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} {_MNo}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);

            byte[] result = new Dry.Models.CommonCls.ExportReports().AcceptAffidavitPDF(_MNo);
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 切結書.pdf");
        }
        #endregion
        #region 匯出配合農時提前施設切結書
        [HttpPost]
        public ActionResult ExportFitAffidavitWord()
        {
            _MNo = (int)Session["MapNo"];
            Dictionary<string, string> dataStr = db.MergeExportAcceptAffidavitData(_MNo);

            
            byte[] Result = new Dry.Models.CommonCls.ExportReports().ExportWord(dataStr, Server.MapPath("~/ReportSample/ExportFitAffidavit.docx"));
            return File(Result, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", DateTime.Now.ToString("yyyyMMdd") + " - 配合農時提前施設切結書.docx");
        }
        [HttpPost]
        public ActionResult ExportFitAffidavitPDF()
        {
            _MNo = (int)Session["MapNo"];
            Dictionary<string, string> dataStr = db.MergeExportAcceptAffidavitData(_MNo);
            string CtrlName = "", CtrlAmount = "";
            foreach (var item in dataStr)
            {
                if (item.Key == "CtrlItem")
                {
                    CtrlName = item.Value.Replace("<w:br />", "\n");

                }
                if (item.Key == "CtrlAmount")
                {
                    CtrlAmount = item.Value.Replace("<w:br />", Environment.NewLine);
                }
            }
            dataStr["CtrlItem"] = CtrlName;
            dataStr["CtrlAmount"] = CtrlAmount;
            
            byte[] result = new Dry.Models.CommonCls.ExportReports().ExportPDF(dataStr, Server.MapPath("~/ReportSample/ExportFitAffidavit.pdf"));
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 配合農時提前施設切結書.pdf");
        }
        #endregion
        #region 結案申報書(竣工報驗書)
        [HttpPost]
        public ActionResult ExportCompleteWord()
        {
            _MNo = (int)Session["MapNo"];
            Dictionary<string, string> dataStr = db.MergeExportCompleteData(_MNo);
            
            byte[] Result = new Dry.Models.CommonCls.ExportReports().ExportWord(dataStr, Server.MapPath("~/ReportSample/ExportComplete.docx"));
            return File(Result, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", DateTime.Now.ToString("yyyyMMdd") + " - 竣工報驗書.docx");
        }
        /// <summary>
        /// 結案申報書(竣工報驗書)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportCompletePDF()
        {
            _MNo = (int)Session["MapNo"];
            //Dictionary<string, string> dataStr = db.MergeExportCompleteData(_MNo);
            
            //byte[] result = new Dry.Models.CommonCls.ExportReports().ExportPDF(dataStr, Server.MapPath("~/ReportSample/ExportComplete.pdf"));

            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} {_MNo}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            
            byte[] result = new Dry.Models.CommonCls.ExportReports().CompleteVerifyPDF(_MNo);
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 結案申報書.pdf");
        }
        #endregion
        #region 調節控制材料數量計算表
        [HttpPost]
        public ActionResult ExportCtlMat()
        {
            int _MNo = (int)Session["MapNo"];
            BudgetBookView Data = db.GetBudgetBookData(_MNo);
            Int16 unit = Data.ApplyUnit;
            GetData gd = new GetData();
            byte[] result;
            if (gd.GetGoldFromMapno(_MNo)) 
            {
                
                //result = bbr.CreateCtlMatGold(_MNo);
                result = bbr.CreateCtlMat(_MNo);
            }
            else
            {
                result = bbr.CreateCtlMat(_MNo);
            }
            
            string FileName = Data.ApplyY.ToString() + " - " + Data.IANum.ToString() + " - 調節控制材料數量計算表.xls";
            return File(result, "application/vnd.ms-excel", FileName);


        }
        #endregion
        #endregion
    }
}
