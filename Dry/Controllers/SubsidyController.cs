using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models;
using Dry.Models.Service;
using Dry.Models.ViewModel;


namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class SubsidyController : Controller
    {
        //
        // GET: /Subsidy/
        public ActionResult Index()
        {
            SubsidyView DataView = new SubsidyView();
            SubsidyDBService SubsidyService = new SubsidyDBService();
            DataView.ApplyUnit = Convert.ToInt32(Session["UnitID"]);
            DataView.YearDDL = SubsidyService.GetFromSubsidyApplyYear();
            DataView.ApplyYear = Convert.ToInt32(DataView.YearDDL.Last().Value);
            DataView.UnitDDL = new AERC.Models.CommonCls.GetUints().GetUnitDDL();
            DataView.SubsidyLimitList = SubsidyService.GetSubsidyLimitList(DataView.ApplyUnit, DataView.ApplyYear);
            return View(DataView);
        }
    }
}
