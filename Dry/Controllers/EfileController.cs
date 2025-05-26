/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-6-05
-- Description: 電子檔上傳
-- =============================================
*/

using Dry.Models.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Dry.Models.ViewModel;
using System.Xml;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class EfileController : Controller
    {
        int MapNo;
        /// <summary>
        /// 本地檔案路徑
        /// </summary>
        static string LocalPath;
        /// <summary>
        /// 檔案相對位罝路徑
        /// </summary>
        static string ServerPath;
        static string FolderPath;
        public EfileController()
        {
            XmlDocument data = new XmlDocument();
            data.Load(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/FilePath.xml"));
            LocalPath = data.SelectSingleNode("Path/LocalPath").Attributes["Value"].Value;
            ServerPath = data.SelectSingleNode("Path/ServerPath").Attributes["Value"].Value;
            FolderPath = data.SelectSingleNode("Path/Folder").Attributes["Value"].Value;
        }
        public ActionResult Efile()
        {
            if (Session["MapNo"] == null)
                return RedirectToAction("Index", "ApplyIndex");
            MapNo = Convert.ToInt32(Session["MapNo"]);
            EfileListView efileData = new EfileListView();
            efileData.EfileList = new EfileDBService().GetEfileList(MapNo);
            efileData.ServerPath = Path.Combine(ServerPath, FolderPath);

            //return View(efileData);
            return PartialView(efileData);
        }
        /// <summary>
        /// 電子檔上傳資料總覽(僅限電子檔類別)
        /// </summary>
        /// <param name="_search">用來判別是電子檔類別，還是一般檔案類別</param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sord"></param>
        /// <param name="sidx"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public JsonResult eFileList(bool _search, int? page, int? rows, string sord, string sidx, string searchString)
        {
            EfileListView efile = new EfileListView();
            MapNo = Convert.ToInt32(Session["MapNo"]);
            efile.EfileList = new EfileDBService().GetEfileListByeFile(MapNo, false);
            int pageSize = rows.HasValue ? rows.Value : 10;
            int pageNum = page.HasValue ? page.Value : 1;
            int totalRecords = efile.EfileList.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = pageNum,
                records = totalRecords,
                rows = efile.EfileList.Skip((pageNum - 1) * pageSize).Take(pageSize)
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 檔案上傳(身份證分正、反面)
        /// </summary>
        /// <param name="file">上傳的檔案</param>
        /// <param name="fileType1">檔案類型</param>
        /// <param name="fileType2">圖片正、反面(1：正面；2：反面)</param>
        /// <returns>圖片路徑</returns>
        [HttpPost]
        public ActionResult ImgUpload(string file, string fileType1, string fileType2)
        {
            MapNo = Convert.ToInt32(Session["MapNo"]);
            string name = file.Split('.')[1];
            string fileName = DateTime.Now.ToString("yyyyMMdd-hhmm") + "_" + new Random(Guid.NewGuid().GetHashCode()).Next().ToString() + "_" + fileType2 + "." + name;
            string filePath = Path.Combine(MapNo.ToString(), fileType1);

            string OutPath = Upload(file, LocalPath, Path.Combine(FolderPath, filePath), fileName);

            //轉換圖片成相對路徑
            string virtualPath = SwapUrl(OutPath);

            EfileView eFIleData = new EfileView();

            eFIleData.MapNo = MapNo;
            eFIleData.FileType = Convert.ToByte(fileType1);
            eFIleData.Filepath = fileName;

            DBCommon.DbEvent dbMsg = new EfileDBService().CreateImg(eFIleData);

            return Content(virtualPath);
        }
        /// <summary>
        /// 刪除照片(身份證正、反面)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult ImgDel(string fileName)
        {
            DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
            dbMsg = new EfileDBService().DeleteImg(fileName);
            return Content(dbMsg.DbMessage);
        }
        /// <summary>
        /// 檔案上傳(一般檔案)
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="fileType">檔案類型</param>
        /// <param name="fileDes">檔案描述</param>
        /// <returns></returns>
        public ActionResult FileUpload(string file, string fileType, string fileDes)
        {
            MapNo = Convert.ToInt32(Session["MapNo"]);
            string name = file.Split('.')[1];
            string fileName = DateTime.Now.ToString("yyyyMMdd-hhmm") + "_" + new Random(Guid.NewGuid().GetHashCode()).Next().ToString() + "." + name;
            string fileFolder = Path.Combine(FolderPath, MapNo.ToString(), fileType);

            string OutPath = Upload(file, LocalPath, fileFolder, fileName);
            
            string virtualPath = SwapUrl(OutPath);
            EfileView eFIleData = new EfileView();

            eFIleData.MapNo = MapNo;
            eFIleData.FileType = Convert.ToByte(fileType);
            eFIleData.Filepath = fileName;
            //eFIleData.UpldTime = DateTime.Now;
            eFIleData.FileDes = fileDes;

            DBCommon.DbEvent dbMsg = new EfileDBService().CreateImg(eFIleData);
            List<FileView> fileView = new List<FileView>();
            fileView.Add(new FileView { FilePath = virtualPath, FileDes = fileDes, FileID = dbMsg.KeyValue.ToString() });
            return Json(fileView, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FileList(bool _search, int? page, int? rows, string sord, string sidx, string searchString)
        {
            MapNo = Convert.ToInt32(Session["MapNo"]);
            EfileListView efile = new EfileListView();
            efile.EfileList = new EfileDBService().GetEfileListByeFile(MapNo,true);
            int pageSize = rows.HasValue ? rows.Value : 10;
            int pageNum = page.HasValue ? page.Value : 1;
            int totalRecords = efile.EfileList.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = pageNum,
                records = totalRecords,
                rows = efile.EfileList.Skip((pageNum - 1) * pageSize).Take(pageSize)
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FileDel(int No)
        {
            DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
            dbMsg = new EfileDBService().DeleteFile(No);
            return Content(dbMsg.DbMessage);
        }

        public ActionResult eFileDel(string oper, int id)
        {
            DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
            dbMsg = new EfileDBService().DeleteFile(id);
            return Content(dbMsg.DbMessage);
        }

        public ActionResult FileDownload(int No)
        {
            string path = Path.Combine(Path.Combine(LocalPath, FolderPath), new EfileDBService().GetFilePath(No));
            return File(System.IO.File.ReadAllBytes(path), "application/octet-stream", HttpUtility.UrlEncode(System.IO.Path.GetFileName(path)));
        }

        class FileView
        {
            public string FilePath { get; set; }
            public string FileDes { get; set; }
            public string FileID { get; set; }
        }

        /// <summary>
        /// 上傳檔案
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="path">路徑</param>
        /// <param name="fileName">檔名</param>
        /// <returns></returns>
        private string Upload(string file, string path,string folder, string fileName)
        {
            var stream = Request.InputStream;
            
            if (!Directory.Exists(Path.Combine(path, folder)))
                Directory.CreateDirectory(Path.Combine(path, folder));
            string outPath = Path.Combine(path + folder, fileName);
            using (FileStream fs = new FileStream(outPath, FileMode.Create))
            {
                stream.CopyTo(fs);
                fs.Close();
            }
            return Path.Combine(folder, fileName);
        }
        /// <summary>
        /// 絕對路徑 轉 相對路徑
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string SwapUrl(string url)
        {
            string virtualPath = Path.Combine(ServerPath, url);
            return virtualPath.Replace(@"\", @"/");
        }
    }
}
