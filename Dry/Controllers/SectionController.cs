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
    [Authorize]
    public class SectionController : Controller
    {
        //
        // GET: /Section/
        DBCommon.DbEvent dbStatus = new DBCommon.DbEvent();
        public ActionResult Index()
        {
            SectionView sectionView = new SectionView();
            sectionView.CityDDL = new CommClass().GetCityDDL();
            sectionView.TownDDL = new CommClass().GetTownDDL("-1");
            sectionView.SectionDDL = new CommClass().GetSectionDDL("-1");
            return View(sectionView);
        }
        public JsonResult GetTownDDL(string CityCode)
        {
            List<SelectListItem> TownDDL = new CommClass().GetTownDDL(CityCode);
            return Json(TownDDL, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSectionDDL(string TownId)
        {
            List<SelectListItem> SectionDDL = new CommClass().GetSectionDDL(TownId);
            return Json(SectionDDL, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSectionList(short? Town_Id, bool _search, int? page, int? rows, string sord, string sidx, string searchString)
        {
            List<SectionList> Data = new SectionDBService().GetSectionList(Town_Id.Value);
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

        public JsonResult GetLandNumberList(int? Section_Id, bool _search, int? page, int? rows, string sord, string sidx)
        {
            List<LandNumberList> Data = new SectionDBService().GetLandNumberList(Section_Id.Value);
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

        public JsonResult EditSection(int? Section_Code,string Section,string Subsection,string oper,int id)
        {
            switch(oper)
            {
                case "edit":
                    dbStatus = new SectionDBService().EditSection(id, Section_Code.Value, Section, Subsection);
                    break;
                case "del":
                    dbStatus = new SectionDBService().DelSection(id);
                    break;
            }
            return Json(dbStatus.DbMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddSection(int Section_Code, string Section, string Subsection, short Town_Id, string oper, int? id)
        {
            dbStatus = new SectionDBService().AddSection(Section_Code, Section, Subsection, Town_Id);
            return Json(dbStatus.DbMessage, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 編輯、刪除地號
        /// </summary>
        /// <param name="Land_Code"></param>
        /// <param name="Longitude"></param>
        /// <param name="Latitude"></param>
        /// <param name="oper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditLandNumber(string Land_Code, decimal? Longitude, decimal? Latitude, string oper, int? id)
        {
            switch(oper)
            {
                case "edit":
                    dbStatus = new SectionDBService().EditLandNumber(id.Value, Land_Code, Longitude.Value, Latitude.Value);
                    break;
                case "del":
                    dbStatus = new SectionDBService().DelLandNumber(id.Value);
                    break;
            }
            
            return Content(dbStatus.DbMessage);
        }
        /// <summary>
        /// 新增地號
        /// </summary>
        /// <param name="Section_Id"></param>
        /// <param name="Land_Code"></param>
        /// <param name="Longitude"></param>
        /// <param name="Latitude"></param>
        /// <returns></returns>
        public ActionResult AddLandNumber(int Section_Id, string Land_Code, decimal Longitude, decimal Latitude)
        {
            dbStatus = new SectionDBService().AddLandNumber(Section_Id, Land_Code, Longitude, Latitude);
            return Content(dbStatus.DbMessage);
        }
    }
}
