using Dry.Models;
using Dry.Models.CommonCls;
using Dry.Models.ReportModules;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dry.Controllers
    
{
    [Authorize]
    [SessionCheck]
    public class PreliminaryTrialController : Controller
    {
        //
        // GET: /PreliminaryTrial/
        private DryEntities DB = new DryEntities();
        private GetData getData = new GetData();
        public ActionResult Index()
        {
            ApplyIndexView data = new Models.ViewModel.ApplyIndexView();
            data.ApplyYearDDL = new CommClass().GetApplyYearByUnit(short.Parse(Session["UnitID"].ToString()));
            return View(data);
        }

        public JsonResult GetFarmerDataNew(byte ApplyYear)
        {
            //List<CaseDetail> farmList = new List<CaseDetail>();
            short unitId = getData.GetUnitId((Guid)Session["User"]);
            var farmList = from a in DB.CaseDetail
                           where a.ApplyYear == ApplyYear && a.ApplyUnit == unitId && a.Complete == true
                           select a;
            List < gridtmp > cases = new List<gridtmp>();
            foreach (var item in farmList)
            {
                gridtmp fData = new gridtmp();
                fData.案號 = item.IANum;
                fData.編號 = item.EventNo;
                fData.申請年度 = item.ApplyYear;
                fData.承辦單位 = getData.GetUnitName(item.ApplyUnit);
                fData.黃金廊道 = item.Gold == true ? "是" : "否";
                fData.申請人姓名 = item.Name;/////HaoHsuan add
                fData.末端型式 = item.EndTypeCNS;
                fData.Export = false;
                fData.列印 = "否";
                //fData.s = item.Step;
                fData.末端型式 = item.CatalogCNS;
                fData.施作面積 = item.buildarea ?? 0;
                fData.匯出日期 = item.PDate ==null ? string.Empty : item.PDate.Value.ToShortDateString();
                if (item.Complete)
                    fData.案件狀態 = "<font color=blue>已結案</font>";
                else
                {
                    string step = string.Empty;
                    switch (item.Step)
                    {
                        case 1:
                            step = "完成農戶資料";
                            break;
                        case 2:
                            step = "完成農地資料";
                            break;
                        case 3:
                            step = "完成動力設施資料";
                            break;
                        case 4:
                            step = "完成蓄水設施資料";
                            break;
                        case 5:
                            step = "完成管路系統資料";
                            break;
                        case 6:
                            step = "完成調控設施資料";
                            break;
                        case 7:
                            step = "完成現場勘查";
                            break;
                        case 8:
                            step = "完成預算書製作";
                            break;
                        case 9:
                            step = "完成變更設計";
                            break;
                        case 10:
                            step = "完成竣工報驗";
                            break;
                        case 11:
                            step = "完成案件驗收";
                            break;
                        case 12:
                            step = "完成電子檔上傳";
                            break;
                    }
                    fData.案件狀態 = "<font color=red>" + step + "</font>";
                    if (item.Step >= 7)
                        fData.案件狀態 = "<font color=green>" + step + "</font>";
                }
                cases.Add(fData);
            }

            return Json(cases, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ExportDoc(string parmsString)
        {
            
            
            var o = JsonConvert.DeserializeObject<tmpclass>(parmsString, new JsonSerializerSettings() { Error = (sender, args) => args.ErrorContext.Handled = true });
            short units = (short)Session["UnitID"];
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
            OperationLog += $"MapNos:{parmsString}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            byte[] result = new PreliminaryTrialReport().getReportByIds(o.Ids, units);
            byte[] result1 = new PreliminaryTrialReport().getReportXlsByIds(o.Ids, units);
            string handle = Guid.NewGuid().ToString();
            string handle1 = Guid.NewGuid().ToString();
            TempData[handle] = result;
            TempData[handle1] = result1;


            return new JsonResult() { Data = new { FileGuid = handle, FileGuid1 = handle1 } } ;
        }

        public ActionResult DownFile(string docids)
        {
            if (TempData[docids] != null)
            {
                byte[] file = TempData[docids] as byte[];
                string filename = $"data{DateTime.Now.ToString("MMddmmss")}. - 初審意見表.pdf";
                return File(file, "application/pdf", filename);
            }
            else return new EmptyResult();
        }

        public ActionResult DownFileXls(string docids)
        {
            if (TempData[docids] != null)
            {
                byte[] file = TempData[docids] as byte[];
                string filename = $"data{DateTime.Now.ToString("MMddmmss")}. - 初審意見表.xlsx";
                return File(file, "application/vnd.ms-excel", filename);
            }
            else return new EmptyResult();
        }
        //public ActionResult ExportAgreement(ReportView data)
        //{
        //    short units = (short)Session["UnitID"];
        //    Guid userid = (Guid)Session["User"];
        //    string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} ";
        //    OperationLog += $"Applyyear:{data.ApplyYear} IANumStart:{data.IANumStart} IANumEnd:{data.IANumEnd}";
        //    string ip = Request.UserHostAddress;
        //    string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
        //    byte[] result = new PreliminaryTrialReport().getReport(units, data.ApplyYear, data.IANumStart, data.IANumEnd);
        //    if (result == null)
        //    {
        //        return null;
        //    }
        //    return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 初審意見表.pdf");
        //}
        struct gridtmp
        {
            public int 申請年度 { get; set; }
            public string 承辦單位 { get; set; }
            public string 申請人姓名 { get; set; }
            public string 末端型式 { get; set; }
            public double 施作面積 { get; set; }
            public string 黃金廊道 { get; set; }
            public string 案件狀態 { get; set; }
            public int 編號 { get; set; }
            public int 案號 { get; set; }
            public bool Export { get; set; }
            public string 列印 { get; set; }
            public string 匯出日期 { get; set; }
        }
        public class tmpclass
        {
            public int[] Ids { get; set; }           

        }
    }
}
