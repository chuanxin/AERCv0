/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-6-8
-- Description: 峻工報驗
-- =============================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.Service;
using Dry.Models.ViewModel;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class FacilityReportController : Controller
    {
        #region Parameter
        private int _MNo;
        private FacilityReportDBService db = new FacilityReportDBService();
        #endregion

        public ActionResult CompletionReport()
        {
            //Session["MapNo"] = 69;
            _MNo = Convert.ToInt32(Session["MapNo"]);
            FacilityReportView ModelData = db.GetModelData(_MNo);
            //return View(ModelData);
            return PartialView(ModelData);
        }

        #region Receive Postback CreateData
        public ActionResult CreateData([System.Web.Http.FromBody] FacilityReportDBService.JsData ResultArry)
        {
            if (ResultArry == null)
                return Content("非法進入");
            _MNo = Convert.ToInt32(Session["MapNo"]);
            string msg = "Success";
            if (ModelState.IsValid)
            {
                msg = db.InsertFacilityReport(_MNo, ResultArry).DbMessage;
            }
            else
            {
                msg = "驗證未通過 !\n";
            }
            return Content(msg);
        }
        #endregion

        public ActionResult Edit_CompletionReport()
        {
            //Session["MapNo"] = 20;
            _MNo = Convert.ToInt32(Session["MapNo"]);
            FacilityReportView ModelData = db.GetEditModelData(_MNo);
            return View(ModelData);
        }

        #region Receive Postback CreateData
        public ActionResult EditData([System.Web.Http.FromBody] FacilityReportDBService.JsData ResultArry)
        {
            if (ResultArry == null)
                return Content("非法進入");
            _MNo = Convert.ToInt32(Session["MapNo"]);
            string msg = "Success";
            if (ModelState.IsValid)
            {
                msg = db.ModifyFacilityReport(_MNo, ResultArry).DbMessage;
            }
            else
            {
                msg = "驗證未通過 !\n";
            }
            return Content(msg);
        }
        #endregion

    }
}
