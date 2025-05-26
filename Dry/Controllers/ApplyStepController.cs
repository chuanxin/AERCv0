using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.CommonCls;
using Dry.Models.Service;
using Dry.Models.ViewModel;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class ApplyStepController : Controller
    {
        //
        // GET: /ApplyStep/
        GetData getData = new GetData();
        int _MapNo = new int();        
        public ActionResult Index()
        {
            ApplyStepView DataView = new ApplyStepView();
            if (Session["MapNo"] != null)
            {
                _MapNo = Convert.ToInt32(Session["MapNo"].ToString());                
                DataView.Step = getData.GetCaseDataFromMapNo(_MapNo).Step;
                bool Status = (bool)Session["Status"];
                if (Session["UnitID"].ToString() == "7" || Session["UnitID"].ToString() == "13")
                    if (!Status)
                        if (DataView.Step >= 2)
                            DataView.Step = 1;
                
                //if (DataView.Step == 8)
                //{
                    if (Session["IsChg"] != null)
                    {
                        if ((bool)Session["IsChg"])
                        {
                            DataView.Step = 0;
                            //Session.Remove("IsChg");
                        }
                    }
                //}
            }
            else
            {
                DataView.Step = 0;
            }            
            DataView.Url = GetUrl(DataView.Step + 1);
            return View(DataView);
        }
        private string GetUrl(int step)
        {
            switch (step)
            {
                case 0:
                case 1:
                    return Url.Action("Index", "Farmer");
                case 2:
                    return Url.Action("CreateFarm", "FarmLand");
                case 5:
                    return Url.Action("CreateFarmerSys", "FarmerSys");
                case 3:
                    return Url.Action("Create", "ApplyEngine");
                case 4:
                    return Url.Action("Create", "ApplyPool");
                case 6:
                    return Url.Action("Index", "Ctrl");
                case 7:
                    return Url.Action("Index", "Examine");
                case 8:
                    return Url.Action("Create", "BudgetBook");
                case 9:
                    return Url.Action("ChangDesign", "ChgDesign");
                case 10:
                    return Url.Action("CompletionReport", "FacilityReport");
                case 11:
                    return Url.Action("CreateVerify", "Verify");
                case 12:
                    return Url.Action("Efile", "Efile");
                default:
                    return Url.Action("Index", "ApplyIndex");
            }
        }
        public JsonResult GetSelectedStepData(string step)
        {
            if (Session["MapNo"] != null/*TempData["MapNo"] != null*/)
            {
                return Json(GetUrl(Convert.ToInt32(step)), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error",JsonRequestBehavior.AllowGet);
            }
        }

    }
}
