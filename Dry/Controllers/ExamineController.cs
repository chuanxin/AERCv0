/*
-- =============================================
-- Author: WEI
-- Create date: 2014-2-10
-- Description: 現場勘查
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Dry.Models.ViewModel;
using Dry.Models.Service;
using Dry.Models;
using Dry.Models.CommonCls;
using System.IO;
using System.Xml;

namespace Dry.Controllers
{
    [System.Web.Mvc.Authorize][SessionCheck]
    public class ExamineController : Controller
    {
        ExamineDBService examDB = new ExamineDBService();
        GetData getData = new GetData();
        CommClass comm = new CommClass();
        ExamineView exam = new ExamineView();
        int MapNo = new int();
        /// <summary>
        /// 本地檔案路徑
        /// </summary>
        static string LocalPath;
        /// <summary>
        /// 檔案相對位罝路徑
        /// </summary>
        static string ServerPath;

        public ExamineController()
        {
            XmlDocument data = new XmlDocument();
            data.Load(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/FilePath.xml"));
            LocalPath = data.SelectSingleNode("Path/LocalPath").Attributes["Value"].Value + data.SelectSingleNode("Path/Folder").Attributes["Value"].Value;
            ServerPath = data.SelectSingleNode("Path/ServerPath").Attributes["Value"].Value + data.SelectSingleNode("Path/Folder").Attributes["Value"].Value;
        }

        public ActionResult Index()
        {
            if (Session["MapNo"] == null)
                return RedirectToAction("Index", "ApplyIndex");
            //Session["MapNo"] = 59;
            MapNo = Convert.ToInt32(Session["MapNo"]);
            Examine ex;
            ViewBag.Admin = comm.GetExamineMen(User.Identity.Name);
            exam.Step = getData.GetCaseDataFromMapNo(MapNo).Step;
            exam.IsModify = exam.Step >= 7 ? true : false; 
            int BMno= exam.IsModify == true ? MapNo : getData.GetBeforeNowMapNo(MapNo);
            
            if (MapNo != BMno)
            {
                if (!Directory.Exists(Path.Combine(LocalPath, MapNo.ToString()))) 
                    if (Directory.Exists(Path.Combine(LocalPath, BMno.ToString())))
                        ExamineDBService.CopyDirectory(Path.Combine(LocalPath, BMno.ToString()), Path.Combine(LocalPath, MapNo.ToString()));
                    else
                        Directory.CreateDirectory(Path.Combine(LocalPath, MapNo.ToString()));
            }
            
            exam.EFileList = examDB.GetEFileList(BMno);
            ex = examDB.GetExamineData(BMno);
            exam.Result = 0;
            exam.ListFileTypeList = examDB.GetFileTypeList();
            exam.iaWMS = new MappingClass().GetWmsIaName(Convert.ToInt16(Session["UnitID"].ToString()));
            if (ex != null)
            {
                exam.AdminID = ex.Examiner;
                exam.Result = ex.Result;
                exam.Reason = ex.Reason;
                exam.EDate = ex.EDate;
                exam.Note = ex.Note;
                exam.EFilePath = ServerPath;
            }
            else
            {
                exam.EDate = DateTime.Now.Date;
            }
            return PartialView(exam);
        }
        //已整合至Index() by wei in 2015/2/25
        //public ActionResult chgExamine()
        //{
        //    //Session["MapNo"] = 22; //測試用，到時註解
        //    MapNo = (int)Session["MapNo"];
        //    int BMno = getData.GetBeforeNowMapNo(MapNo);//get previous mapping version No

        //    ViewBag.Admin = comm.GetExamineMen(User.Identity.Name);
        //    exam.Result = 0;
        //    exam.ListFileTypeList = examDB.GetFileTypeList();
        //    exam.previousMNo = BMno;
        //    exam.EFilePath = ServerPath;

        //   
        //    if (!Directory.Exists(Path.Combine(LocalPath, MapNo.ToString())))
        //        ExamineDBService.CopyDirectory(Path.Combine(LocalPath, BMno.ToString()), Path.Combine(LocalPath, MapNo.ToString()));

        //    //get previous mapping version data
        //    exam.EFileList = examDB.GetEFileList(BMno);
        //    Examine ex = examDB.GetExamineData(BMno);
        //    if (ex != null)
        //    {
        //        exam.AdminID = ex.Examiner;
        //        exam.Result = ex.Result;
        //        exam.Reason = ex.Reason;
        //        exam.EDate = ex.EDate;
        //        exam.Note = ex.Note;
        //    }
        //    else
        //    {
        //        exam.EDate = DateTime.Now.Date;
        //    }
        //    exam.iaWMS = new MappingClass().GetWmsIaName(Convert.ToInt16(Session["UnitID"].ToString()));
        //    return View(exam);
        //}

        public ActionResult CreateData([FromBody] ExamineJsonData ResultArry)
        {
            DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
            MapNo = Convert.ToInt32(Session["MapNo"]);
            if (ModelState.IsValid)
            {
                MapNo = Convert.ToInt32(Session["MapNo"]);
                //Examine examine = new Examine();
                //examine.MapNo = MapNo;
                //examine.EnginePnt = "";
                //examine.WaterPnt = "";
                //examine.Result = ResultArry.Result;
                //examine.Reason = ResultArry.Reason;
                //examine.Examiner = ResultArry.Examiner;
                //examine.EDate = Convert.ToDateTime(ResultArry.EDate);
                //examine.Note = ResultArry.Note;
                dbMsg = examDB.CreateData(ResultArry, MapNo);
            }
            return Content(dbMsg.DbMessage);
        }
        /// <summary>
        /// 略過目前步驟
        /// </summary>
        /// <returns></returns>
        public JsonResult PassData()
        {
            MapNo = Convert.ToInt32(Session["MapNo"]);
            return Json(new CommClass().UpdateCase(MapNo, 7).DbMessage, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult FileUpload(string file, string fileType)
        {
            MapNo = Convert.ToInt32(Session["MapNo"]);
            DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
            var stream = Request.InputStream;
            string name = file.Split('.')[1];
            string fileName = DateTime.Now.ToString("yyyyMMdd-hhmm") + "_" + new Random(Guid.NewGuid().GetHashCode()).Next().ToString() + "." + name;
            string filePath = Path.Combine(LocalPath, MapNo.ToString());
            
            string virtualPath = Path.Combine(ServerPath, MapNo.ToString(), fileName);
            
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            string outPath = Path.Combine(filePath, fileName);
            string Latlng = string.Empty;
            using (FileStream fs = new FileStream(outPath, FileMode.Create))
            {
                stream.CopyTo(fs);
                fs.Close();
                
                ImageExif exif = new ImageExif(outPath);
                Latlng = exif.Latitude.ToString() + "," + exif.Longitude.ToString();
                
                //EFile eFile = new EFile();
                //eFile.MapNo = MapNo;
                //eFile.FileType = Convert.ToByte(fileType);
                //eFile.Filepath = fileName;
                //eFile.Coordinate = Latlng;
                //eFile.UpldTime = DateTime.Now;
                //dbMsg = examDB.CreateImgFile(eFile);
            }
            return Content(Latlng + ";" + virtualPath);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult DeleteImg(string fileName)
        {
            DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
            dbMsg = examDB.DeleteImg(fileName);
            //return Index();
            return Content(dbMsg.DbMessage);
        }

    }
}
