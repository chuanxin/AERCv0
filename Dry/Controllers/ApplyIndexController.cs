using AERC.Models.Service;
using AERC.Models.ViewModel;
using Dry.Models;
using Dry.Models.CommonCls;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using EncryptStringV1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class ApplyIndexController : Controller
    {
        //
        // GET: /ApplyIndex/
        private FarmerDBService farmerDB = new FarmerDBService();
        private GetData getData = new GetData();
        private CommClass comcls = new CommClass();
        private DryEntities DB = new DryEntities();
        public ActionResult Index()
        {
            ApplyIndexView data = new Models.ViewModel.ApplyIndexView();
            data.ApplyYearDDL = comcls.GetApplyYearByUnit(short.Parse(Session["UnitID"].ToString()));
            return View(data);
        }
        public ActionResult IndexDel()
        {
            ApplyIndexView data = new Models.ViewModel.ApplyIndexView();
            data.ApplyYearDDL = comcls.GetApplyYearByUnit(short.Parse(Session["UnitID"].ToString()));
            return View(data);
        }

        [HttpPost]
        public ActionResult Redirect()
        {
            Session.Remove("MapNo");
            return RedirectToAction("Index", "ApplyStep");
        }
        public JsonResult GetFarmerData(byte ApplyYear, bool _search, int? page, int? rows, string sord, string sidx, string searchString)
        {
            
            List<Case> farmList = farmerDB.GetCaseData(ApplyYear, _search, searchString, sidx, sord, (byte)getData.GetUnitId((Guid)Session["User"]));
            List<FarmerView.CaseData> cases = new List<FarmerView.CaseData>();
            //FarmerView.CaseData fData = new FarmerView.CaseData();
            //var aa = (from m in DB.CaseDetail
            //          where m.ApplyYear == ApplyYear
            //          select m).ToList();
            //var bb = (from m in farmList
            //          join s in aa
            //          on m.FId equals s.FId into ms
            //          from s in ms.DefaultIfEmpty()
            //          select new { m, CatalogCNS = s.CatalogCNS ?? "其他", buildarea = s.buildarea ?? 0 }).ToList();
            var bb = (from m in farmList
                      join s in DB.CaseDetail on m.EventNo equals s.EventNo
                      
                      /*from s in ms.DefaultIfEmpty()*/
                      select new { m, CatalogCNS = s.CatalogCNS ?? "其他", buildarea = s.buildarea ?? 0 }) /*.ToList()*/;
            if (bb != null)
            {

            

            foreach (var item in bb)
            {
                FarmerView.CaseData fData = new FarmerView.CaseData();
                fData.EventNo = item.m.EventNo;
                fData.FId = item.m.FId;
                fData.ApplyYear = item.m.ApplyYear;
                fData.ApplyUnit = getData.GetUnitName(item.m.ApplyUnit);
                fData.Gold = item.m.Gold == true ? "是" : "否";
                fData.CDate = item.m.CDate.ToString("yyyy/MM/dd");
                if (item.m.UDate != null)
                    fData.UDate = item.m.UDate.Value.ToString("yyyy/MM/dd");
                else
                    fData.UDate = "";
                fData.Name = item.m.Farmer.Name;/////HaoHsuan add
                fData.IANum = item.m.IANum;
                fData.Complete = item.m.Complete;
                fData.Step = item.m.Step;
                fData.CatalogCNS = item.CatalogCNS;
                fData.buildarea = item.buildarea;
                                
                if (item.m.Complete)
                    fData.Status = "<font color=blue>已結案</font>";
                else
                {
                    string step = string.Empty;
                    switch (item.m.Step)
                    {
                        case 1: step = "完成農戶資料";
                            break;
                        case 2: step = "完成農地資料";
                            break;
                        case 3: step = "完成動力設施資料";
                            break;
                        case 4: step = "完成蓄水設施資料";
                            break;
                        case 5: step = "完成管路系統資料";
                            break;
                        case 6: step = "完成調控設施資料";
                            break;
                        case 7: step = "完成現場勘查";
                            break;
                        case 8: step = "完成預算書製作";
                            break;
                        case 9: step = "完成變更設計";
                            break;
                        case 10: step = "完成竣工報驗";
                            break;
                        case 11: step = "完成案件驗收";
                            break;
                        case 12: step = "完成電子檔上傳";
                            break;
                    }
                    fData.Status = "<font color=red>" + step + "</font>";
                    if(fData.Step >= 7)
                        fData.Status = "<font color=green>" + step + "</font>";
                }
                cases.Add(fData);
            }
            }
            int pageSize = rows.HasValue ? rows.Value : 10;
            int pageNum = page.HasValue ? page.Value : 1;
            int totalRecords = farmList.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = pageNum,
                records = totalRecords,
                rows = cases.Skip((pageNum - 1) * pageSize).Take(pageSize)
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFarmerDataNew(int ApplyYear)
        {

            List<CaseDetail> farmList = new List<CaseDetail>();
            short unitId = (short)getData.GetUnitId((Guid)Session["User"]);
            //List<int> unitlist = new List<int>();
            /*
            if (unitId == 0 || unitId == 99 || unitId == 20)
            {
                unitlist = DB.CaseDetail.Select(o => o.EventNo).ToList();
            }
            else
            {
                //unitlist = DB.CaseDetail.Where(o => o.ApplyUnit == unitId).Select(o => o.EventNo).ToList();
            }


            if (ApplyYear == 0)
            {
                
                farmList = (from o in DB.CaseDetail
                            where unitlist.Contains(o.EventNo)
                            select o).ToList();
            }
            else
            {
                
                farmList = (from o in DB.CaseDetail
                            where unitlist.Contains(o.EventNo) && o.ApplyYear == ApplyYear
                            select o).ToList();
            }
            */
            var datas = from a in DB.Case
                        where a.ApplyYear == ApplyYear && a.Enable == false
                        let ver = a.VerMapping.OrderByDescending(m => m.ChgVer).FirstOrDefault()
                        select new {
                            a.IANum,
                            a.EventNo,
                            a.ApplyYear,
                            a.ApplyUnit,
                            a.Gold,
                            a.Farmer.Name,
                            CatalogCNS = ver.PigingConf.EndType.FirstOrDefault().EndTypeList.Catalog ?? "其他",
                            buildarea = ver.Farm.Sum(o => (double?)o.BuildArea / 10000),
                            a.Step,
                            a.Complete,
                            a.Reason
                        };

            if (unitId != 0 && unitId != 99 && unitId != 20)
            {
                datas = from a in datas
                        where a.ApplyUnit == unitId
                        select a;
            }


            List<gridtmp> cases = new List<gridtmp>();


            foreach (var item in /*farmList*/ datas)
            {
                gridtmp fData = new gridtmp();
                fData.案號 = item.IANum;
                fData.編號 = item.EventNo;
                fData.申請年度 = item.ApplyYear;
                fData.承辦單位 = getData.GetUnitName(item.ApplyUnit);
                fData.黃金廊道 = item.Gold == true ? "是" : "否";
                fData.申請人姓名 = item.Name;/////HaoHsuan add
                //fData.末端型式 = item.EndTypeCNS;
                //fData.Complete = item.Complete;
                //fData.s = item.Step;
                fData.末端型式 = item.CatalogCNS;
                fData.施作面積 = (item.buildarea ?? 0).ToString("0.0000");
                fData.原因 = item.Reason ?? string.Empty;
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
        struct gridtmp
        {
            public int 申請年度 { get; set; }
            public string 承辦單位 { get; set; }
            public string 申請人姓名 { get; set; }
            public string 末端型式 { get; set; }
            public string 施作面積 { get; set; }
            public string 黃金廊道 { get; set; }
            public string 案件狀態 { get; set; }
            public int 編號 { get; set; }
            public int 案號 { get; set; }
            public string 原因 { get; set; }
        }
        public JsonResult DelData(int EventNo,string rsn)
        {
            
            Guid userid = (Guid)Session["User"];
            string OperationLog = $"{this.ControllerContext.RouteData.Values["action"].ToString()} {this.ControllerContext.RouteData.Values["controller"].ToString()} {EventNo}";
            string ip = Request.UserHostAddress;
            string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
            return Json(new Dry.Models.Service.DelCaseDataDBService().ModifyCaseStatus(EventNo, rsn).DbMessage, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult Redirect(int eventno)
        {
            Session["MapNo"] = getData.GetNewestMapNo(eventno);
            return Redirect("../ApplyStep/Index");
        }
        
        public ActionResult Report(int eventno)
        {
            string filePath = Server.MapPath("~/App_Data/");
            int mapno = getData.GetNewestMapNo(eventno);
            byte[] file = new LiugongPDFProcess().CreatePDF(filePath, mapno);
            
            return File(file, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 申請書.pdf");
        }

        public ActionResult Gisweb()
        {
            try
            {
                Guid userid = (Guid)Session["User"];
                AdminDBService ads = new AdminDBService();
                UserInfo userinfo = ads.GetIndividualAdmin(userid);
                string OperationLog = $"user:{userinfo.Account} ";
                string ip = Request.UserHostAddress;
                string status = new UserLogDBService().SaveData(userid, OperationLog, ip);
                RijndaelEnhanced encrypttools = new RijndaelEnhanced("xu.6g6jo3", "aerc4521314#alex");
                
                string token = encrypttools.Encrypt(userinfo.Account);
                token = new EscapeString().Escape(token);
                string gisurlstring = System.Configuration.ConfigurationManager.AppSettings["gisurl"];
                return Redirect($"{gisurlstring}/home/webindex?token={token}");
                //return Redirect($"http://localhost:59685/home/webindex?token={token}");
            }
            catch (Exception ex)
            {
                RedirectToAction("login","Account");
                throw;
            }

            //RedirectToAction("login", "Account");
        }
    }
}
