using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using Dry.Models.CommonCls;
using Dry.Models.ReportModules;
using System.Xml;
using System.Web.Security;

namespace Dry.Controllers
{//
    [Authorize][SessionCheck]
    public class ReportController : Controller
    {
        //
        // GET: /Report/
        private CommClass comcls = new CommClass();
        private ReportService reportdb = new ReportService();
        private BudgetBookDBService db = new BudgetBookDBService();
        /// <summary>
        /// 本地檔案路徑
        /// </summary>
        static string LocalPath;
        static string FolderPath;
        public ReportController()
        {
            XmlDocument data = new XmlDocument();
            data.Load(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/FilePath.xml"));
            LocalPath = data.SelectSingleNode("Path/LocalPath").Attributes["Value"].Value;
            FolderPath = data.SelectSingleNode("Path/Folder").Attributes["Value"].Value;
        }
        public ActionResult Index(ReportView data)
        {
            data.CheeseList = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "推廣", Value = "A" },
                new SelectListItem() { Text = "推廣蓄水池", Value = "B" },
                new SelectListItem() { Text = "瑠公蓄水池", Value = "C" }
            };


            //ReportView data = new ReportView();
            List<SelectListItem> ApplyYearDDL = comcls.GetApplyYearByUnit(short.Parse(Session["UnitID"].ToString()));
            ApplyYearDDL = ApplyYearDDL.Where(m => m.Text != "歷年").ToList();
            //List<SelectListItem> CheeseList = new List<SelectListItem>();
            //CheeseList.Add(new SelectListItem() { Text = "推廣", Value = "A" });
            //CheeseList.Add(new SelectListItem() { Text = "推廣蓄水池", Value = "B" });
            //CheeseList.Add(new SelectListItem() { Text = "瑠公", Value = "C" });
            //data.CheeseList = CheeseList;
            if (data.ApplyYear == 0)
            {
                data.ApplyYear = Convert.ToInt32(ApplyYearDDL.LastOrDefault().Value);
            }

            foreach (var item in ApplyYearDDL)
            {
                if (item.Value == data.ApplyYear.ToString())
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            data.ApplyYearDDL = ApplyYearDDL;
            List<int> Ianumlist = reportdb.GetIANumFromMaxToMin(short.Parse(Session["UnitID"].ToString()), data.ApplyYear);
            data.IANumStart = Ianumlist[1];
            data.IANumEnd = Ianumlist[0];
            data.number = 1;
            //data.ApplyUnit  = int.Parse(Session["UnitID"].ToString());
            return View(data);

        }
        public ActionResult SubsidyArea()
        {
            ReportView.ApplyArea data = new ReportService().SubsidyAreaOnLiugong();
            return View();
        }
        /// <summary>
        /// 外出拍攝照片攜帶表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PhotoGraphCarry(ReportView data)
        {
            PhotographCarryReport PC = new PhotographCarryReport();
            string sourcepath = @"~\ReportSample\PhotographCarryReport.xls";
            byte[] result = PC.GetReport_PhotographCarry((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);

            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 施工照片拍攝攜帶表.xls");
        }
        /// <summary>
        /// 印領清冊
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubsidyList(ReportView data)
        {
            var units = Session["UnitID"];
            int unit = Int32.Parse(units.ToString());
            string val = Request.Form["MyList"]; 
            //string num = Request.Form["List"];
            //int number = Int32.Parse(num);
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);

            SubsidyListReport SL = new SubsidyListReport();
            
            string sourcepath = @"~\ReportSample\SubsidyListReport.xls";
            switch (unit)
            {

                case 11:
                    sourcepath = @"~\ReportSample\SubsidyListReport11.xls";
                break;
                case 23:
                    sourcepath = @"~\ReportSample\SubsidyListReport23.xls";
                break;
                

            }
            byte[] result = new byte[0];
            switch (unit)
            {
                case 11:
                    SubsidyListReport11 SL11 = new SubsidyListReport11();
                    // string a = data.CheeseList.ToString();
                    string Dname1 = Request.Form["DesignName1"];
                    string Dname2 = Request.Form["DesignName2"];
                    string s1 = Request.Form["Design1Start"];
                    string e1 = Request.Form["Design1End"];
                    string s2 = Request.Form["Design2Start"];
                    string e2 = Request.Form["Design2End"];

                    result = SL11.GetReport_SubsidyList((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath, val, data.number
                        ,Dname1,s1,e1,Dname2,s2,e2);
                    break;
                case 23:
                    AERC.Models.ViewModel.UserInfo userInfo = new AERC.Models.ViewModel.UserInfo();
                    AERC.Models.Service.AdminDBService adminDB = new AERC.Models.Service.AdminDBService();
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value);
                    userInfo = adminDB.GetIndividualAdmin(Guid.Parse(ticket.UserData));
                    
                    SubsidyListReport23 SL23 = new SubsidyListReport23();
                    // string a = data.CheeseList.ToString();
                    result = SL23.GetReport_SubsidyList((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath, userInfo.Name);
                    break;
                default:
                    result = SL.GetReport_SubsidyList((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
                    break;
            }
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 印領清冊.xls");
        }
        /// <summary>
        /// 旱作管路灌溉系統設施設計表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SystemFacilityDesignReport(ReportView data)
        {
            var units = Session["UnitID"];
            int unit = Int32.Parse(units.ToString());
            SystemFacilityDesignReport SFD = new SystemFacilityDesignReport();
            
            string sourcepath = @"~\ReportSample\SystemFacilityDesignReport.xls";
            switch (unit)
            {

                case 13:
                    sourcepath = @"~\ReportSample\SystemFacilityDesignReport13.xls";
                    break;
            }
            byte[] result = SFD.GetReport_SystemFacilityDesign((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 旱作管路灌溉系統設施設計表.xls");
        }
        /// <summary>
        /// 領款收據舊版2018
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]        
        public ActionResult PayeeReceiptOld(ReportView data)//
        {
            var units = Session["UnitID"];
            int unit = Int32.Parse(units.ToString());
            PayeeReceipt PR = new PayeeReceipt();
            string sourcepath = @"~\ReportSample\PayeeReceiptReport.xls";
            
            switch (unit)
            {
                case 8:
                    sourcepath = @"~\ReportSample\PayeeReceiptReport8.xls";
                break;
                case 12:
                    sourcepath = @"~\ReportSample\PayeeReceiptReport12.xlsx";
                break;
                case 11:
                    sourcepath = @"~\ReportSample\PayeeReceiptReport11.xls";
                break;
                case 13:
                    sourcepath = @"~\ReportSample\PayeeReceiptReport13.xls";
                break;
                case 15:
                    sourcepath = @"~\ReportSample\PayeeReceiptReport15.xls";
                break;
            }
            byte[] result;
            switch (unit)
            {
                case 6:
                    result = PR.GetReport_PayeeReceiptPDF((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
                    return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 領款收據.pdf");
                case 14:
                    result = new PayeeReceipt14().GetReport_PayeeReceipt((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
                    break;
                case 12:
                    result = new PayeeReceipt12().GetReport_PayeeReceipt12((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
                    break;
                case 13:
                    result = new PayeeReceipt13().GetReport_PayeeReceipt((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
                    break;
                case 11:
                    result = new PayeeReceipt11().GetReport_PayeeReceipt((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
                break;
                default:
                    result = PR.GetReport_PayeeReceipt((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
                    //return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 領款收據.pdf");
                break;

            }
            //byte[] result = PR.GetReport_PayeeReceipt((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 領款收據.xls");
        }
        /// <summary>
        /// 領款收據
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PayeeReceipt(ReportView data)
        {
            var units = Session["UnitID"];
            int unit = Int32.Parse(units.ToString());
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            PayeeReceipt PR = new PayeeReceipt();
            byte[] result;            
            result = PR.GetReport_Batch_PayeeReceiptPDF((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd);
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 領款收據.pdf");
                
        }
        /// <summary>
        /// 封面
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CoverReport(ReportView data)//
        {
            var units = Session["UnitID"];
            int unit = Int32.Parse(units.ToString());
            CoverReport CR = new CoverReport();
            //string sourcepath = @"~\ReportSample\Cover.xlsx";
            //switch (unit)
            //{
            //    case 15:
            //        sourcepath = @"~\ReportSample\Cover15.xlsx";
            //        break;
            //}      
            //byte[] result = CR.GetReport_Cover((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            byte[] result = CR.GetReport_CoverPDF((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd);
            //return File(result, "application/ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 封面.xlsx");
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 封面.pdf");
        }
        /// <summary>
        /// 驗收報告書
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AcceptanceReport(ReportView data)
        {
            var units = Session["UnitID"];
            int unit = Int32.Parse(units.ToString());
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            AcceptanceReport AR = new AcceptanceReport();
            #region oldcode
            /*
            
            string sourcepath = @"~\ReportSample\AcceptanceReport.xlsx";
            switch (unit)
            {
                case 4:
                    sourcepath = @"~\ReportSample\AcceptanceReport4.xlsx";
                    break;
                case 7:
                    sourcepath = @"~\ReportSample\AcceptanceReport7.xlsx";
                    break;
                case 10:
                    sourcepath = @"~\ReportSample\AcceptanceReport10.xlsx";
                    break;
                case 11:
                    sourcepath = @"~\ReportSample\AcceptanceReport11.xlsx";
                    break;
                case 12:
                    sourcepath = @"~\ReportSample\AcceptanceReport12.xlsx";
                    break;
                case 13:
                    sourcepath = @"~\ReportSample\AcceptanceReport13.xlsx";
                    break;
                case 14:
                    sourcepath = @"~\ReportSample\AcceptanceReport14.xlsx";
                    break;
                case 15:
                    sourcepath = @"~\ReportSample\AcceptanceReport15.xlsx";
                break;
                case 19:
                    sourcepath = @"~\ReportSample\AcceptanceReport19.xlsx";
                break;
                case 22:
                    sourcepath = @"~\ReportSample\AcceptanceReport_22.xlsx";
                break;
                case 23:
                    sourcepath = @"~\ReportSample\AcceptanceReport23.xlsx";
                break;
            }
            //byte[] result;
            
            switch (unit)
            {
                case 13:
                    result = new AcceptanceReport13().GetReport_AcceptanceReport((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
                    break;
                case 14:
                    result = new AcceptanceReport14().GetReport_AcceptanceReport((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
                break;
                default:
                    result = AR.GetReport_AcceptanceReport((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
                    break;
            }
            */
            #endregion
            byte[] result = AR.GetReport_AcceptanceReport((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd/*, sourcepath*/);
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 功能測試現地勘查報告書.pdf");
        }
        /// <summary>
        /// 管路補助金額明細表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EngineeringReport(ReportView data)
        {
            var units = Session["UnitID"];
            string val = Request.Form["MyList"]; 
           
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            EngineeringReport ER = new EngineeringReport();
            //string sourcepath = @"~\ReportSample\AcceptanceReport.xlsx";
            if (((short)units == 9) || ((short)units == 10))
            {
                byte[] result = ER.GetReport_EngineeringReportGold((short)units, data.ApplyYear, data.IANumStart, data.IANumEnd,data.step);
                
                return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 管路補助金額明細表.xlsx");
            }
            
            //else if ((short)units == 11)
            //{
            //    EngineeringReport11 ER11 = new EngineeringReport11();
            //    byte[] result = ER11.GetReport_EngineeringReport((short)units, data.ApplyYear, data.IANumStart, data.IANumEnd, data.step, val, data.number);
            //    return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 管路補助金額明細表.xlsx");            
            //}
            
            else
            {
                byte[] result = ER.GetReport_EngineeringReport((short)units, data.ApplyYear, data.IANumStart, data.IANumEnd,data.step);
                return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 管路補助金額明細表.xlsx");
            }          

        }
        

        
        /// <summary>
        /// 工程預算書
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BudgetBookReport(ReportView data)
        {
            var units = Session["UnitID"];
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            //byte[] result = new BudgetBookReport().ExportExcelBatch((short)units, data.ApplyYear, data.IANumStart, data.IANumEnd);
            byte[] result = new BudgetBookReport().BudgetBookBatchPDF((short)units, data.ApplyYear, data.IANumStart, data.IANumEnd);
            //return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 工程預算書.xlsx");
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 工程預算書.pdf");
        }
        
       /// <summary>
       /// 切結書
       /// </summary>
       /// <param name="data"></param>
       /// <returns></returns>
          
        [HttpPost]
        public ActionResult ExportAcceptAffidavitPDF(ReportView data)//切結書
        {
            short units = (short)Session["UnitID"];
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            #region oldCode
            //byte[] result;
            //Dry.Models.DryEntities dry = new Dry.Models.DryEntities();
            //var datalist = from db in dry.SummaryView
            //               where db.ApplyYear == data.ApplyYear && db.ApplyUnit == units && db.IANum >= data.IANumStart && db.IANum <= data.IANumEnd
            //               orderby db.IANum
            //               select db;

            //List<Dry.Models.CommonCls.ExportPDFDataContent> ExportData = new List<Models.CommonCls.ExportPDFDataContent>();

            //foreach (var item in datalist)
            //{
            //    Dictionary<string, string> dataStr = db.MergeExportAcceptAffidavitData((int)item.MapNo, data.ApplyYear.ToString(), data.moon, data.day);
            //    string CtrlName = "", CtrlAmount = "";
            //    foreach (var subitem in dataStr)
            //    {
            //        if (subitem.Key == "CtrlItem")
            //        {
            //            CtrlName = subitem.Value.Replace("<w:br />", "\n");

            //        }
            //        if (subitem.Key == "CtrlAmount")
            //        {
            //            CtrlAmount = subitem.Value.Replace("<w:br />", Environment.NewLine);
            //        }
            //    }
            //    dataStr["CtrlItem"] = CtrlName;
            //    dataStr["CtrlAmount"] = CtrlAmount;
            //  
            //    //byte[] result = new Dry.Models.CommonCls.ExportReports().ExportPDF(dataStr, Server.MapPath("~/ReportSample/ExportAcceptAffidavit.pdf"));    
            //    ExportData.Add(new Models.CommonCls.ExportPDFDataContent { KeyData = dataStr, TemplateUrl = Server.MapPath("~/ReportSample/ExportAcceptAffidavit.pdf") });
            //}
            //if (units == 11)
            //{
            //    result = new Dry.Models.CommonCls.ExportReports().ExportPDFListAL11(ExportData);
            //}
            //else if (units == 13)
            //{
            //    result = new Dry.Models.CommonCls.ExportReports().ExportPDFListAL13(ExportData);
            //}
            //else if (units == 22)
            //{
            //    result = new Dry.Models.CommonCls.ExportReports().ExportPDFListAL22(ExportData);
            //}
            //else
            //{
            //    result = new Dry.Models.CommonCls.ExportReports().ExportPDFListAL(ExportData);
            //}           

            //return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 切結書.pdf");
            #endregion
            Byte[] result = new Dry.Models.CommonCls.ExportReports().AcceptAffidavitPDFS(data.ApplyYear, units, data.IANumStart, data.IANumEnd);
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 切結書.pdf");
        }
        

        
        /// <summary>
        /// 竣工報驗書(結案申報書)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportCompletePDF(ReportView data)
        {
            short units = (short)Session["UnitID"];
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            
            #region OldCode 
            //Dry.Models.DryEntities dry = new Dry.Models.DryEntities();
            //var datalist = from db in dry.SummaryView
            //               where db.ApplyYear == data.ApplyYear && db.ApplyUnit == units && db.IANum >= data.IANumStart && db.IANum <= data.IANumEnd
            //               orderby db.IANum
            //               select db;
            //List<Dry.Models.CommonCls.ExportPDFDataContent> ExportData = new List<Models.CommonCls.ExportPDFDataContent>();
            //string pdfPath = "~/ReportSample/ExportComplete.pdf";
            //if (units == 13)
            //{
            //    pdfPath = "~/ReportSample/ExportComplete13.pdf";
            //}
            //foreach (var item in datalist)
            //{
            //    Dictionary<string, string> dataStr = db.MergeExportCompleteData((int)item.MapNo);
            //    ExportData.Add(new Models.CommonCls.ExportPDFDataContent { KeyData = dataStr, TemplateUrl = Server.MapPath(pdfPath/*"~/ReportSample/ExportComplete.pdf"*/) });

            //}
            ////byte[] result = new Dry.Models.CommonCls.ExportReports().ExportPDFListA(ExportData);
            //byte[] result;
            //if (units == 13)
            //{
            //    result = new Dry.Models.CommonCls.ExportReports().ExportPDFList13(ExportData);
            //}
            //else if (units == 11)
            //{
            //    result = new Dry.Models.CommonCls.ExportReports().ExportPDFList11(ExportData);
            //}
            //else if (units == 12)
            //{
            //    result = new Dry.Models.CommonCls.ExportReports().ExportPDFList12(ExportData);
            //}
            //else if (units == 22)
            //{
            //    result = new Dry.Models.CommonCls.ExportReports().ExportPDFList22(ExportData);
            //}
            //else
            //{
            //    result = new Dry.Models.CommonCls.ExportReports().ExportPDFListB(ExportData);
            //}

            //return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 竣工報驗書.pdf");
            #endregion
            byte[] result = new Dry.Models.CommonCls.ExportReports().CompleteVerifyPDFS(data.ApplyYear, units, data.IANumStart, data.IANumEnd);
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 結案申報書.pdf");
        }
        [HttpPost]
        public ActionResult Export3inOne(ReportView data)
        {
            short units = (short)Session["UnitID"];
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            byte[] result = new ExportReports().export3report(data, units);
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 切結收據結書.pdf");
        }
        
        /// <summary>
        /// Excel住址標籤
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportAddress(ReportView data)
        {
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            AddressLabel AL = new AddressLabel();
            string sourcepath = @"~\ReportSample\AddressReportSample1.xlsx";
            byte[] result = AL.GetReport_AddressLabel((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 住址標籤.xlsx");
        }        
        /// <summary>
        /// 施工前後照片V0
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ConstructionPhotoV0(ReportView data)
        {
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            
            ConstructionPhotoReport CR = new ConstructionPhotoReport();
            string sourcepath = @"~\ReportSample\ConstructionPhotoReport.xlsx";
            byte[] result = CR.GetReport_AcceptanceReport((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath, LocalPath + FolderPath);
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 施工前後照片.xlsx");
        }
        /// <summary>
        /// 施工前後照片
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult ConstructionPhoto(ReportView data)
        {
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            
            
            byte[] result = new BudgetBookReport().PictureBatchPDF((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd );
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 施工前後照片.pdf");
        }
        /// <summary>
        /// 瑠公申請清冊
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Liugong_ApplyList(ReportView data)
        {

            LiugongApplyListReport LA = new LiugongApplyListReport();
            string sourcepath = @"~\ReportSample\LiugongApplyList.xlsx";
            byte[] result = LA.GetReport_LiugongApplyList((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 申請清冊.xlsx");
        }

        #region Excel管路工程設施輔助明細表
        //public ActionResult ExportEngineering(ReportView data)//管路工程設施輔助明細表
        //{
        //    EngineeringReport ER = new EngineeringReport();
        //    string sourcepath = @"~\ReportSample\EngineeringReportSample.xlsx";
        //    byte[] result = ER.GetReport_EngineeringReport((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
        //    return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 管路工程設施輔助明細表.xlsx");
        //}
        #endregion

        
        /// <summary>
        /// 設計費收據
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult DesignReceiptReport(ReportView data)//設計費收據
        {
            //byte[] result;
            short units = (short)Session["UnitID"];
            //string sourcepath;
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            DesignReceiptReport DR = new DesignReceiptReport();
            //DesignReceiptReport15 DR15 = new DesignReceiptReport15();
            //sourcepath = @"~\ReportSample\DesignReceipt.xlsx";
            //if (units == 15)
            //{
            //    sourcepath = @"~\ReportSample\DesignReceipt15.xls";
            //}
            //if (units == 15)
            //{
            //     result = DR15.GetReport_DesignReceiptReport((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
            //}
            //else
            //{
            //     result = DR.GetReport_DesignReceiptReport((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
            //}
            //if (units == 15)
            //{
            //    return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 設計費收據.xls");
            //}
            //else
            //{
            //    return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 設計費收據.xlsx");
            //}
            byte[] result = DR.GetReport_DesignReceiptReport((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd);
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 設計費收據.pdf");

        }       

        
        /// <summary>
        /// 現場勘查紀錄表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult SiteSurveyReport(ReportView data)
        {
            SiteSurveyReport SSR = new SiteSurveyReport();
            string sourcepath = @"~\ReportSample\SiteSurvey_Sample.xls";
            byte[] result = SSR.GetReport_SiteSurvey((short)Session["UnitID"], data.ApplyYear, data.IANumStart, data.IANumEnd, sourcepath);
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 現場勘查紀錄表.xls");
        }
        
        
        /// <summary>
        /// 土地清冊
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult ExportFarmLands(ReportView data)
        {
            short units = (short)Session["UnitID"];
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            byte[] result = new FarmLands().getListByUnitYear(units, data.ApplyYear, data.IANumStart, data.IANumEnd);
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + "土地清冊.xlsx");
        }

        public ActionResult ExportCheckList(ReportView data)
        {
            short units = (short)Session["UnitID"];
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            byte[] result = new BudgetBookReport().CompleteCheckListBatchPDF(units, data.ApplyYear, data.IANumStart, data.IANumEnd);
            if (result == null)
            {
                return null;
            }
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 書面審查表.pdf");
        }
    }
}
