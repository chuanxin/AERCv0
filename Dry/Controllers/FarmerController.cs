/*
-- =============================================
-- Author: WEI, HaoHsuan
-- Create date: 2014-6-20
-- Description: 農戶資料填寫
-- =============================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models;
using Dry.Models.ViewModel;
using Dry.Models.Service;
using Dry.Models.CommonCls;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class FarmerController : Controller
    {
        private FarmerDBService farmerDB = new FarmerDBService();
        private GetData getData = new GetData();
        private CommClass comcls = new CommClass();
        private bool IsChg = false;
        public ActionResult Index()
        {

            IsChg = Session["IsChg"] == null ? false : (bool)Session["IsChg"];
            if (Session["IsChg"] != null)
            {
                //Session.Remove("IsChg");
            }
            FarmerView FarmerData = new FarmerView();
            FarmerData.CityDDL = comcls.GetCityDDL();
            FarmerData.TownDDL = comcls.GetTownDDL("-1");
            FarmerData.DesginerDDL = comcls.GetDesinerList((short)Session["UnitID"], getData.GetAdminName((Guid)Session["User"]));
            FarmerData.DesingerId = (Guid)Session["User"];
            FarmerData.IsMember = Session["UnitID"].ToString() == "17" ? true : false; 
            //FarmerData.Gold = Session["UnitID"].ToString() == "9" || Session["UnitID"].ToString() == "10" || Session["UnitID"].ToString() == "99" ? true : false; 
            
            FarmerData.ApplyYear = (byte)(DateTime.Now.Year - 1911); 
            FarmerData.ApplyUnit = (short)Session["UnitID"]; 
            FarmerData.IANum = getData.GetNewIANum(getData.GetUnitId((Guid)Session["User"]), FarmerData.ApplyYear); 
            ViewBag.checkIanum = true;
            
            if (Session["MapNo"] != null /*TempData["MapNo"] != null*/  )
            {
                int MNo = (int)Session["MapNo"];
                
                Case casedata = getData.GetCaseDataFromMapNo(MNo);
                Farmer farmerdata = getData.GetFarmerData(MNo);
                if (casedata != null && farmerdata != null)
                {
                    FarmerData.FarmerName = farmerdata.Name;
                    FarmerData.FarmerPhone = farmerdata.Phone;
                    FarmerData.FarmerTel = farmerdata.Tel;
                    FarmerData.FarmerIdNo = farmerdata.IdNo;
                    FarmerData.FarmerAddr = farmerdata.Addr.Trim().Split(' ')[2];
                    FarmerData.FarmerCityCode = farmerdata.CityCode;
                    FarmerData.TownDDL = comcls.GetTownDDL(FarmerData.FarmerCityCode);
                    FarmerData.FarmerTownId = getData.GetTownIdbyName(farmerdata.CityCode, farmerdata.Addr.Split(' ')[1]);
                    FarmerData.FId = casedata.FId;
                    FarmerData.EvntNo = casedata.EventNo;
                    FarmerData.ApplyYear = casedata.ApplyYear; 
                    FarmerData.IANum = casedata.IANum; 
                    FarmerData.ApplyUnit = casedata.ApplyUnit; 
                    FarmerData.Step = casedata.Step; 
                    FarmerData.CId = farmerdata.CId;
                    FarmerData.DesingerId = getData.GetDesignID(MNo);
                    FarmerData.Gold = casedata.Gold;
                    FarmerView.CaseData fData = new FarmerView.CaseData();
                    fData.EventNo = casedata.EventNo;
                    fData.ApplyYear = casedata.ApplyYear;
                    fData.ApplyUnit = getData.GetUnitName(casedata.ApplyUnit);
                    fData.Gold = casedata.Gold == true ? "是" : "否";
                    fData.CDate = casedata.CDate.ToString("yyyy/MM/dd");
                    if (casedata.UDate != null)
                        fData.UDate = casedata.UDate.Value.ToString("yyyy/MM/dd");
                    else
                        fData.UDate = "";
                    FarmerData.caseDta = fData;
                    FarmerData.IsModify = IsChg == true ? false : true; 
                    FarmerData.IsChg = IsChg; 
                    FarmerData.Is12 = casedata.Is12;
                    fData.Is12 = casedata.Is12 == true ? "是" : "否";
                    ViewBag.checkIanum = false;
                }
            }
            return PartialView(FarmerData);
            //return View(FarmerData);
        }

        public JsonResult chkIaNum(int applyunit, int applyyear, int ianum)
        {
            bool result = getData.IanumIsExist(applyyear, applyunit, ianum);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult chgFarmer()
        //{
        //    //Session["MapNo"] = 68;
        //    //Guid NowUser = Guid.Parse("1d5c3aee-4814-47d1-b818-11a21dc991dd");
        //    //Session["User"] = NowUser;

        //    int MNo = (int)Session["MapNo"];
        //    Case casedata = getData.GetCaseDataFromMapNo(MNo);
        //    Farmer farmerdata = getData.GetFarmerData(MNo);

        //    FarmerView FarmerData = new FarmerView();
        //    FarmerData.CityDDL = comcls.GetCityDDL();
        //    FarmerData.TownDDL = comcls.GetTownDDL(farmerdata.CityCode);
        //    FarmerData.DesginerDDL = comcls.GetDesinerList((short)Session["UnitID"], getData.GetAdminName((Guid)Session["User"]));//getData.GetExamineAdmin(NowUser);
        //    FarmerData.DesingerId = farmerDB.GetDesigner(MNo);
        //    FarmerData.ApplyYear = casedata.ApplyYear;
        //    FarmerData.Gold = Session["UnitID"].ToString() == "9" || Session["UnitID"].ToString() == "10" || Session["UnitID"].ToString() == "99" ? true : false; 
        //    FarmerData.IsMember = farmerdata.IsMember;
        //    FarmerData.Name = farmerdata.Name;
        //    FarmerData.Phone = farmerdata.Phone;
        //    FarmerData.Tel = farmerdata.Tel;
        //    FarmerData.IdNo = farmerdata.IdNo;
        //    FarmerData.Addr = farmerdata.Addr.Split(' ')[2];
        //    FarmerData.CityCode = farmerdata.CityCode;
        //    FarmerData.TownId = getData.GetTownIdbyName(farmerdata.CityCode, farmerdata.Addr.Split(' ')[1]);
        //    FarmerData.FId = casedata.FId;
        //    FarmerData.EvntNo = casedata.EventNo;
        //    FarmerData.IANum = casedata.IANum;//

        //    FarmerView.CaseData fData = new FarmerView.CaseData();
        //    fData.EventNo = casedata.EventNo;
        //    fData.ApplyYear = casedata.ApplyYear;
        //    fData.ApplyUnit = getData.GetUnitName(casedata.ApplyUnit);
        //    fData.Gold = casedata.Gold == true ? "是" : "否";
        //    fData.CDate = casedata.CDate.ToString("yyyy/MM/dd");
        //    if (casedata.UDate != null)
        //        fData.UDate = casedata.UDate.Value.ToString("yyyy/MM/dd");
        //    else
        //        fData.UDate = "";
        //    FarmerData.caseDta = fData;

        //    return View(FarmerData);
        //}

        

        public JsonResult GetData(int EvntNo)
        {
            int MNo = (int)Session["MapNo"];
            //Farmer farmer = farmerDB.GetFarmer(EvntNo);
            FarmerView FarmerData = farmerDB.GetFarmer(EvntNo, MNo);
            //FarmerData.Name = farmer.Name;
            //FarmerData.IdNo = farmer.IdNo;
            //FarmerData.Addr = farmer.Addr;
            //FarmerData.Tel = farmer.Tel;
            //FarmerData.Phone = farmer.Phone;
            //FarmerData.IsMember = farmer.IsMember;
            //FarmerData.FId = farmer.FId;
            //FarmerData.Gold=farmer
            return Json(FarmerData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTownDDL(string CityCode)
        {
            List<SelectListItem> TownDDL = comcls.GetTownDDL(CityCode);
            return Json(TownDDL, JsonRequestBehavior.AllowGet);
        }

        #region 取得特定水利會及年度之最新案件編號
        /// <summary>
        /// 取得特定水利會最新案件編號
        /// </summary>
        /// <param name="Year">年度</param>
        /// <returns></returns>
        public JsonResult GetNewIANum(int Year)
        {
            Int16 Unit = 1;
            return Json(getData.GetNewIANum(Unit, Year), JsonRequestBehavior.AllowGet);
        }
        #endregion
        /// <summary>
        /// 儲存資料
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public ActionResult SaveData([System.Web.Http.FromBody] FarmerView Data)
        {
            IsChg = Session["IsChg"] == null ? false : (bool)Session["IsChg"];
            DBCommon.DbEvent dbState = new DBCommon.DbEvent();
            Data.CId = (Guid)Session["User"];//Guid.Parse("3bc85515-5b63-493b-8572-c2377aa31f86");
            if (IsChg)
            {
                Data.EvntNo = getData.GetCase(Data.FId).First().EventNo;
                dbState = farmerDB.chgDBEvent(Data);
                Session.Remove("IsChg");
                
            }
            else
            {
                dbState = farmerDB.SaveData(Data);
            }
            
            if (dbState.DbMessage.Equals("Success"))
            {
                Session["MapNo"] = dbState.KeyValue;                
            }
                
            
            //Session.Remove("IsChg");
            return Content(dbState.DbMessage);
        }

        public ActionResult ModifyData([System.Web.Http.FromBody] FarmerView Data)
        {
            int MNo = (int)Session["MapNo"];            
            Data.CId = (Guid)Session["User"];
            DBCommon.DbEvent dbState = new DBCommon.DbEvent();
            dbState = farmerDB.ModifyData(Data, MNo);
            return Content(dbState.DbMessage);
        }
       
        #region 變更設計送出資料
        //public ActionResult ChgSubmitData([System.Web.Http.FromBody] FarmerView Data)
        //{
        //    DBCommon.DbEvent dbState = new DBCommon.DbEvent();
        //    Data.CId = (Guid)Session["User"];//Guid.Parse("3bc85515-5b63-493b-8572-c2377aa31f86");
        //    dbState = farmerDB.chgDBEvent(Data);
        //    if (dbState.DbMessage.Equals("Success"))
        //        Session["MapNo"] = dbState.KeyValue;

        //    return Content(dbState.DbMessage);
        //}
        #endregion
    }
}
