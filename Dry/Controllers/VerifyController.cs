/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-5-05
-- Description: 辦理驗收
-- =============================================
*/

using Dry.Models.Service;
using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class VerifyController : Controller
    {
        #region Parameter
        private VerifyDBService db = new VerifyDBService();
        private int _MNo;
        #endregion

        #region Create Page

        #region Create Controller
        public ActionResult CreateVerify()
        {
            //Session["MapNo"] = 20;
            _MNo = (int)Session["MapNo"];
            VerifyView ModelData = new VerifyView();
            ModelData = db.GetModelData(_MNo, User.Identity.Name);

            //ViewBag.MoneyData = db.GetMoney(_MNo);
            //return View(ModelData);
            return PartialView(ModelData);
        }
        #endregion

        #region Receive Postback CreateData
        public ActionResult CreateData([System.Web.Http.FromBody] VerifyDBService.JsData ResultArry)
        {
            if (ResultArry == null)
                return Content("非法進入");
            _MNo = (int)Session["MapNo"];
            string msg = "Success";
            if (ModelState.IsValid)
            {
                msg = db.InsertToVerifyDB(_MNo, ResultArry).DbMessage;
            }
            else
            {
                msg = "驗證未通過 !\n";
            }
            return Content(msg);
        }
        #endregion

        #endregion

        #region Edit Page
        public ActionResult EditVerify()
        {
            //Session["MapNo"] = 20;
            _MNo = (int)Session["MapNo"];
            Guid _UserID = (Guid)Session["User"];
            VerifyView ModelData = new VerifyView();
            ModelData = db.GetEditModelData(_MNo, _UserID);
            return View(ModelData);
        }

        #region Receive Postback EditData
        public ActionResult EditData([System.Web.Http.FromBody] VerifyDBService.JsData ResultArry)
        {
            if (ResultArry == null)
                return Content("非法進入");
            _MNo = (int)Session["MapNo"];
            string msg = "Success";
            if (ModelState.IsValid)
            {
                msg = db.ModifyToVerifyDB(_MNo, ResultArry).DbMessage;
            }
            else
            {
                msg = "驗證未通過 !\n";
            }
            return Content(msg);
        }
        #endregion

        #endregion

        public ActionResult ExportStatementPdf()
        {
            _MNo = (int)Session["MapNo"];
            Dictionary<string, string> dataStr = db.StatementData(_MNo);
            List<Dry.Models.CommonCls.ExportPDFDataContent> ExportData = new List<Models.CommonCls.ExportPDFDataContent>();
            ExportData.Add(new Models.CommonCls.ExportPDFDataContent { KeyData = dataStr, TemplateUrl = Server.MapPath("~/ReportSample/LiugongStatement_Cover.pdf") });
            ExportData.Add(new Models.CommonCls.ExportPDFDataContent { KeyData = dataStr, TemplateUrl = Server.MapPath("~/ReportSample/LiugongStatement_Content.pdf") });
           
            byte[] result = new Dry.Models.CommonCls.ExportReports().ExportPDFList(ExportData);
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 規劃委託書.pdf");
        }
    }
}
