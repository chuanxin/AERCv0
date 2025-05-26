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
    public class TownManageController : Controller
    {
        //
        // GET: /TownManage/
        TownManageDBService townDB = new TownManageDBService();
        public ActionResult Index()
        {
            TownManageView townView = new TownManageView();
            CommClass commClass = new CommClass();
            townView.CityDDL = commClass.GetCityDDL();
            return View(townView);
        }
        
        public JsonResult GetTownList(string CityCode, bool _search, int? page, int? rows, string sord, string sidx, string searchString)
        {
            List<TownDataView> DataList = townDB.GetTownData(CityCode);
            int pageSize = rows.HasValue ? rows.Value : 10;
            int pageNum = page.HasValue ? page.Value : 1;
            int totalRecords = DataList.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = pageNum,
                records = totalRecords,
                rows = DataList.Skip((pageNum - 1) * pageSize).Take(pageSize)
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOfficeDDL(string CityCode)
        {
            List<SelectListItem> DataList = new CommClass().GetLandOfficeDDL(CityCode);
            return Json(DataList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditTown(short? Zip_Code, string Town, short? Land_Office, string oper, short id)
        {
            DBCommon.DbEvent status = new DBCommon.DbEvent();
            TownManageDBService twnDBService = new TownManageDBService();
            switch(oper)
            {
                case "edit":
                    status = twnDBService.EditTown(id, Zip_Code.Value, Town, Land_Office.Value);
                    break;
                case "del":
                    status = twnDBService.DelTown(id);
                    break;
            }
            
            return Json(status.DbMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddTown(string City_Code, short Zip_Code, string Town_Code, string Town, short Land_Office, string oper, short? id)
        {
            DBCommon.DbEvent status = new DBCommon.DbEvent();
            status = new TownManageDBService().AddTown(City_Code, Zip_Code, Town_Code, Town, Land_Office);
            return Json(status.DbMessage, JsonRequestBehavior.AllowGet);
        }
    }
}
