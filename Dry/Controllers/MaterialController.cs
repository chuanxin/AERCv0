/*
-- =============================================
-- Author: WEI
-- Create date: 2014-10-23
-- Description: 管路灌溉材料表管理
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.ViewModel;
using Dry.Models.Service;
using Dry.Models.CommonCls;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class MaterialController : Controller
    {
        //
        // GET: /Material/
        MaterialDBService materialDB = new MaterialDBService();
        short UnitID = new short();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetMaterialData(bool _search, int? page, int? rows, string sord, string sidx, string searchString)
        {
            UnitID = (short)Session["UnitID"];
            List<MaterialView> Data =new List<MaterialView>();
            Data = materialDB.GetMaterialData(_search, searchString, sidx, UnitID);
            int pageSize = rows.HasValue ? rows.Value : 10;
            int pageNum = page.HasValue ? page.Value : 1;
            int totalRecords = Data.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = pageNum,
                records = totalRecords,
                rows = Data.Skip((pageNum - 1) * pageSize).Take(pageSize)
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditDelMaterialData(int? ModuleCNS, int? Spec1CNS, int? Spec2CNS, int? Spec3CNS, int? MatTypeCNS, double? SpecLength, string oper, int id, string MName = "", string ItemUnit = "", string Note="")
        {
            DBCommon.DbEvent Status = new DBCommon.DbEvent();
            switch(oper)
            {
                case "edit":
                    MaterialView materialView = new MaterialView();
                    if (ModuleCNS.HasValue)
                    {
                        materialView.POMNo = id;
                        materialView.ModuleNo = ModuleCNS.HasValue ? ModuleCNS.Value : 1;
                        materialView.MName = MName;
                        materialView.MatType = MatTypeCNS.Value;
                        materialView.Spec1 = Spec1CNS.Value;
                        materialView.Spec2 = Spec2CNS.Value;
                        materialView.Spec3 = Spec3CNS.Value;
                        materialView.SpecLength = SpecLength.Value;
                        materialView.ItemUnit = ItemUnit;
                        materialView.Note = Note;
                        Status = materialDB.EditMaterial(materialView);
                    }
                    else
                        Status.DbMessage = "Modified FacSysMAT Failed : Data is null";
                    break;
                case "del":
                    Status = materialDB.DelMaterial(id);
                    break;
            }
            return Json(Status.DbMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddMaterialData(int ModuleCNS, string MName, int Spec1CNS, int Spec2CNS, int Spec3CNS, int MatTypeCNS, double? SpecLength, string ItemUnit, string Note)
        {
            DBCommon.DbEvent Status = new DBCommon.DbEvent();
            MaterialView materialView = new MaterialView();
            materialView.ModuleNo = ModuleCNS;
            materialView.Bunit = (short)Session["UnitID"];
            materialView.MName = MName;
            //materialView.Spec = Spec;
            materialView.Spec1 = Spec1CNS;
            materialView.Spec2 = Spec2CNS;
            materialView.Spec3 = Spec3CNS;
            materialView.MatType = MatTypeCNS;
            materialView.SpecLength = SpecLength ?? 0;
            materialView.ItemUnit = ItemUnit;
            materialView.Note = Note;
            Status = materialDB.AddMaterial(materialView);
            return Json(Status, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 模組
        /// </summary>
        /// <returns></returns>
        public JsonResult GetModuleDDL()
        {
            List<SelectListItem> list = new CommClass().GetModuleList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 規格
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSpecDDL()
        {
            List<SelectListItem> list = new CommClass().GetSpecDDL();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 材質
        /// </summary>
        /// <returns></returns>
        public JsonResult GetMatType()
        {
            List<SelectListItem> list = new CommClass().GetQuality();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMaterialPrice(int POMNo, bool _search, int? page, int? rows, string sord, string sidx, string searchString)
        {
            List<MaterialPriceView> Data = materialDB.GetMaterialPrice(POMNo);
            int pageSize = rows.HasValue ? rows.Value : 10;
            int pageNum = page.HasValue ? page.Value : 1;
            int totalRecords = Data.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

            var jsonData = new
            {
                total = totalPages,
                page = pageNum,
                records = totalRecords,
                rows = Data.Skip((pageNum - 1) * pageSize).Take(pageSize)
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult EditMaterialPrice(short? BYear, float? Price, string oper, int id)
        {
            DBCommon.DbEvent Status = new DBCommon.DbEvent();
            MaterialPriceView materialPriceView;
            switch (oper)
            {
                case "edit":

                    materialPriceView = new MaterialPriceView();
                    if (Price.HasValue)
                    {
                        materialPriceView.No = id;
                        materialPriceView.Price = Price.HasValue ? Price.Value : 0;
                        Status = materialDB.EditMaterialPrice(materialPriceView);
                    }
                    else
                        Status.DbMessage = "Modified PriceOfMat Failed : Data is null";
                    break;
                case "del":
                    Status = materialDB.DelMaterialPrice(id);
                    break;
            }
            return Json(Status.DbMessage, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult AddMaterialPrice(int? POMNo, short? BYear, float? Price, string oper, int? id)
        {
            DBCommon.DbEvent Status = new DBCommon.DbEvent();
            MaterialPriceView materialPriceView;
            materialPriceView = new MaterialPriceView();
            if (POMNo.HasValue && BYear.HasValue && Price.HasValue)
            {
                materialPriceView.BYear = BYear.HasValue ? BYear.Value : (short)(DateTime.Now.Year - 1911);
                materialPriceView.Price = Price.HasValue ? Price.Value : 0;
                materialPriceView.POMNo = POMNo.HasValue ? POMNo.Value : 0;
                Status = materialDB.AddMaterialPrice(materialPriceView);
            }
            else
                Status.DbMessage = "Create PriceOfMat Failed : Data is null";
            return Json(Status.DbMessage, JsonRequestBehavior.AllowGet);
        }
    }
}
