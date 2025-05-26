using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using Dry.Models.CommonCls;


namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class DesignerChangeController : Controller
    {
        //
        // GET: /DesignerChage/
        private CommClass comcls = new CommClass();
        private ReportService reportdb = new ReportService();
        private GetData getData = new GetData();
        private DBCommon.DbEvent dbState = new DBCommon.DbEvent();
        private DesingerChangeDBService DC = new DesingerChangeDBService();
        public ActionResult Index(ReportView data)
        {
            data.DesginerDDL = comcls.GetDesinerList((short)Session["UnitID"], getData.GetAdminName((Guid)Session["User"]));
            List<SelectListItem> ApplyYearDDL = comcls.GetApplyYearByUnit(short.Parse(Session["UnitID"].ToString()));
            data.ApplyYear = Convert.ToInt32(ApplyYearDDL.Where(m => m.Selected == true).FirstOrDefault().Value);
            data.ApplyYearDDL = ApplyYearDDL;
            List<int> Ianumlist = reportdb.GetIANumFromMaxToMin(short.Parse(Session["UnitID"].ToString()), data.ApplyYear);
            data.IANumStart = Ianumlist[1];
            data.IANumEnd = Ianumlist[0];
            return View(data);
        }

        [HttpPost]
        public ActionResult Exe_DesignerChange(ReportView data)
        {
            short Unit = (short)Session["UnitID"];
            dbState = DC.SaveDesigner(data.IANumStart, data.IANumEnd, Unit, data.ApplyYear, data.DesginerId);

            return RedirectToAction("Index", "DesignerChange");
        }

    }
}
