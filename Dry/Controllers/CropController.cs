using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
using Dry.Models.Service;

namespace Dry.Controllers
{
    [Authorize]
    public class CropController : Controller
    {
        //
        // GET: /Crop/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetCropList(bool _search, int? page, int? rows, string sord, string sidx, string searchString)
        {
            int pageSize, pageNum, totalRecords, totalPages;
            pageSize = rows.HasValue ? rows.Value : 10;
            pageNum = page.HasValue ? page.Value : 1;
            if (!_search)
            {
                List<GetData.CropStruct> OriDataList = new GetData().GetCrop();
                List<CropStruct> DataList = new List<CropStruct>();
                foreach (var item in OriDataList)
                {
                    DataList.Add(new CropStruct { Crop_Id = item.cropid, Crop = item.cropname, Crop_Type = item.croptype });
                }
                
                totalRecords = DataList.Count;
                totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
                var jsonData = new
                {
                    total = totalPages,
                    page = pageNum,
                    records = totalRecords,
                    rows = DataList.Skip((pageNum - 1) * pageSize).Take(pageSize)
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<CropView> DataList = new CropDBService().GetCropByQuery(searchString);
                totalRecords = DataList.Count;
                totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
                var jsonData = new
                {
                    total = totalPages,
                    page = pageNum,
                    records = totalRecords,
                    rows = DataList.Skip((pageNum - 1) * pageSize).Take(pageSize)
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EditCrop(string Crop, short? Crop_Type, string oper, short? id)
        {
            DBCommon.DbEvent status = new DBCommon.DbEvent();
            switch (oper)
            {
                case "edit":
                    status = new CropDBService().EditCrop(id.Value, Crop, Crop_Type.Value);
                    break;
                case "del":
                    status = new CropDBService().DelCrop(id.Value);
                    break;
            }
            return Json(status.DbMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddCrop(string Crop, short Crop_Type, string oper, short? id)
        {
            DBCommon.DbEvent status = new CropDBService().AddCrop(Crop, Crop_Type);
            return Json(status.DbMessage, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCropTypeDDL()
        {
            return Json(new CropDBService().GetCropTypeDDL(), JsonRequestBehavior.AllowGet);
        }
    }
    class CropStruct
    {
        /// <summary>
        /// 作物代碼
        /// </summary>
        public short Crop_Id { get; set; }
        /// <summary>
        /// 作物名
        /// </summary>
        public string Crop { get; set; }
        /// <summary>
        /// 作物類別
        /// </summary>
        public string Crop_Type { get; set; }
    }
}
