using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.ViewModel;
using Dry.Models.CommonCls;
using Dry.Models.Service;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class ReapplyQueryController : Controller
    {
        //
        // GET: /ReapplyQuery/
        private FarmLandDBService farmlanddb = new FarmLandDBService();
        public ActionResult Index()
        {
            ReapplyQueryView queryData = new ReapplyQueryView();
            GetData gd = new GetData();
            short UnitID = (short)Session["UnitID"];
            queryData.CityDDL = UnitID == 17 ? farmlanddb.GetLiuCityDDL() : farmlanddb.GetCityDDL();
            queryData.TownDDL = farmlanddb.GetTownDDL("");
            queryData.SectionList = farmlanddb.GetSectionList("");
            return View(queryData);
        }
        
        public JsonResult FarmQuery(int FarmSection, string LandNo)
        {
            //var ApplyMsg = new GetData().ChkApply(FarmSection, LandNo);
            
            var ApplyMsg = new GetData().ChkApplyV2(FarmSection, LandNo);
            
            //var ApplyMsg = new GetData().ChkApplyV3(FarmSection, LandNo);
            ApplyMsg.status = ApplyMsg.status.Replace("\n", "<br/>");
            return Json(ApplyMsg, JsonRequestBehavior.AllowGet);
        }

        #region 取得鄉鎮列表
        public JsonResult GetTownDDL(string CityCode)
        {
            List<SelectListItem> TownDDL = farmlanddb.GetTownDDL(CityCode);
            return Json(TownDDL, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 取得地段列表
        public JsonResult GetSecDDL(string townid)
        {
            List<SelectListItem> SecDDL = farmlanddb.GetSectionList(townid);
            return Json(SecDDL, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
