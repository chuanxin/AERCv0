/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-6-24
-- Description: 變更設計
-- =============================================
*/

using Dry.Models.CommonCls;
using Dry.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class ChgDesignController : Controller
    {
        private int _MNo;
        public ActionResult ChangDesign()
        {
            //Session["MapNo"] = 60;
            if (Session["MapNo"] == null)
                return RedirectToAction("Index", "ApplyIndex");
            _MNo = (int)Session["MapNo"];
            bool CaseComplete = new GetData().GetCaseDataFromMapNo(_MNo).Complete;
            if (CaseComplete)
            {
                //return RedirectToAction("CompletionReport", "FacilityReport");
                ViewBag.CaseComplete = "1";
            }
            else ViewBag.CaseComplete = "0";
            //return View();
            return PartialView();
        }
        [HttpPost]
        public ActionResult Changed()
        {
            Session["IsChg"] = true;
            return RedirectToAction("Index", "ApplyStep");
        }
        [HttpPost]
        public ActionResult Continue()
        {
            _MNo = (int)Session["MapNo"];
            string UpdateStep = string.Empty;
            if (new GetData().GetCaseDataFromMapNo(_MNo).Complete)
            {
                UpdateStep = new Dry.Models.CommonCls.CommClass().UpdateCase(_MNo, 10, true).DbMessage;
            }
            else UpdateStep = new Dry.Models.CommonCls.CommClass().UpdateCase(_MNo, 10).DbMessage;
            return RedirectToAction("Index", "ApplyStep");
        }
    }
}
