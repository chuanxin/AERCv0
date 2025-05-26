/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-4-30
-- Description: 公版系統建置
-- =============================================
*/

using Dry.Models.Service;
using Dry.Models.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dry.Controllers
{
    [Authorize]
    public class StandardSysController : Controller
    {
        FacSystemDBService facdb = new FacSystemDBService();
        
        public ActionResult CreateFacSystem()
        {
            FacSystemView ModelData = new FacSystemView();
            ModelData.EndTypeDDL = facdb.GetEndTypeList();
            ModelData.FacTypeDDL = facdb.GetFacTypeList();
            ModelData.StdSysName = "";

            return View(ModelData);
        }

        public ActionResult CreateData([System.Web.Http.FromBody] FacSystemDBService.JsData ResultArry)
        {
            if (ResultArry.MatNoAry == null)
                return Content("非法進入");
            string msg = "Success";
            if (ModelState.IsValid)
            {
                if (ResultArry.MatNoAry.Count > 0)
                {
                    msg = facdb.InsertFacSystem(ResultArry, (short)Session["UnitID"]).DbMessage;
                }
                else
                {
                    msg = "請至少輸入一筆物料\n";
                }
            }
            else
            {
                msg = "Model State Is Not Valid !\n";
            }
            return Content(msg);
        }

        #region MAT Struct
        
        public List<Dry.Models.ViewModel.SysMat> GetMatList(short UnitID)
        {
            List<Dry.Models.ViewModel.SysMat> list = new List<Dry.Models.ViewModel.SysMat>();
            list = facdb.GetFacSysMAT(UnitID);
            return list;
        }
        public JsonResult GetData(string query)
        {
            short UnitID = Convert.ToInt16(Session["UnitID"].ToString());
            //var list = new Dry.Models.ViewModel.SysMat();
            //var retn_res = list.GetMatList(UnitID).Where(m => m.matname.ToLower().Contains(query.ToLower()) || m.pomno == query).ToList().OrderBy(m => m.matname);
            var retn_res = GetMatList(UnitID).Where(m => m.matname.ToLower().Contains(query.ToLower()) || m.pomno == query).ToList().OrderBy(m => m.matname);
            return Json(retn_res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataByPomno(int pomno)
        {
            var MatData = facdb.GetMATbyPmno(pomno);
            if (MatData == null)
                return null;
            return Json(MatData, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
