using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.CommonCls;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using System.Net;
using System.Text;
using System.IO;
using OfficeOpenXml;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class SlopelandQueryController : Controller
    {
        private SlopelandQueryService SRdb = new SlopelandQueryService();
        //
        // GET: /Query/
        CommClass comClass = new CommClass();
        public ActionResult Index()
        {
            SlopelandQueryView view = new SlopelandQueryView();
            view.CityDDL = GetCityDDL();
            view.TownDDL = GetTownDDL("");
            view.SectionList = GetSectionList("");
            return View(view);
        }
        /// <summary>
        /// 載入縣市DropDownList
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetCityDDL()
        {
            return comClass.GetCityDDL();
        }
        /// <summary>
        /// 載入鄉鎮市DropDownList
        /// </summary>
        /// <param name="CityCode">縣市代碼</param>
        /// <returns></returns>
        private List<SelectListItem> GetTownDDL(string CityCode)
        {
            return comClass.GetTownDDL(CityCode); ;
        }
        /// <summary>
        /// 取得地段列表
        /// </summary>
        /// <param name="TownCode">鄉鎮代碼</param>
        /// <returns></returns>
        private List<SelectListItem> GetSectionList(string townid)
        {
            List<SelectListItem> section = comClass.GetSectionDDL(townid);
            return section;
        }
        public JsonResult GetTownDDLByAjax(string CityCode)
        {
            List<SelectListItem> TownDDL = GetTownDDL(CityCode);
            return Json(TownDDL, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSecDDLByAjax(string townid)
        {
            List<SelectListItem> SecDDL = GetSectionList(townid);
            return Json(SecDDL, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        public JsonResult QueryJS(string CityCode, string TownId, string Section, string LandNo)
        {
            ResultView results = Query(CityCode, TownId, Section, LandNo);
            return Json(results, JsonRequestBehavior.AllowGet);
            //return results;
        }

        private ResultView Query(string CityCode, string TownId, string Section, string LandNo)
        {
            SlopelandQueryService slopeQuery = new SlopelandQueryService();
            TownId = slopeQuery.GetTownCode(TownId);
            Section = slopeQuery.GetSection(Convert.ToInt32(Section));
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            string result = null;
            HttpWebRequest request = HttpWebRequest.Create("https://cid.swcb.gov.tw/autocomplete.do?method=search") as HttpWebRequest;
            request.Method = "POST";
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded";
            TownId = TownId.PadLeft(2, '0');
            Section = Section.PadLeft(4, '0');
            string param = "country=" + CityCode + "&town=" + CityCode + TownId + "&road=" + CityCode + TownId + Section + "&landNo=" + LandNo;

            byte[] bs = Encoding.ASCII.GetBytes(param);
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            using (WebResponse response = request.GetResponse())
            {
                StreamReader sr = new StreamReader(response.GetResponseStream());
                result = sr.ReadToEnd();
                sr.Close();
            }
            System.Web.Script.Serialization.JavaScriptSerializer objSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            ResultView results = objSerializer.Deserialize<ResultView>(result);
            //return Json(results, JsonRequestBehavior.AllowGet);
            return results;
        }

        public ActionResult Download()
        {
            
            string filepath = Server.MapPath("~/ReportSample/SearchRoadSample.xlsx");
            
            string filename = System.IO.Path.GetFileName(filepath);
          
            Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            
            return File(iStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
        }
        public ActionResult Upload()
        {
            var RoadList = new List<RoadData>();
            if (Request != null)
            {
                
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;

                        for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
                        {
                            var Road = new RoadData();
                            string CityID = SRdb.GetCityCodeByName(workSheet.Cells[rowIterator, 1].Value.ToString());
                            string TwonID = SRdb.GetTownCodeByName(CityID,workSheet.Cells[rowIterator, 2].Value.ToString());
                            string SectionID = SRdb.GetSectionByName(TwonID, workSheet.Cells[rowIterator, 3].Value.ToString());
                            string LandID = workSheet.Cells[rowIterator, 4].Value.ToString();
                            var jsondata = Query(CityID, TwonID, SectionID, LandID);
                            Road.RCity = workSheet.Cells[rowIterator, 1].Value.ToString();
                            Road.RTwon = workSheet.Cells[rowIterator, 2].Value.ToString();
                            Road.RRoad = workSheet.Cells[rowIterator, 3].Value.ToString();
                            Road.RRoadNumber = workSheet.Cells[rowIterator, 4].Value.ToString();
                            Road.CheckType = jsondata.results[0].landClass.ToString();
                            Road.Geological_Conservation = jsondata.results[0].gisBou.ToString();
                            Road.Water_Conservation = jsondata.results[0].inSoilWater.ToString();
                            RoadList.Add(Road);
                        }
                    }
                }
            }
            byte[] filefinal = SRdb.GetRoadReportData(RoadList);
            return File(filefinal, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", DateTime.Now.ToString("yyyyMMdd") + " - 山坡地查定資料.xlsx");
        }
    }

}
