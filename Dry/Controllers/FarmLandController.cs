/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-5-30
-- Description: 農地資料填寫
-- =============================================
*/

using Dry.Models.Service;
using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Net;
using static Dry.Models.CommonCls.LandCoordinate;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class FarmLandController : Controller
    {
        #region Parameter
        private int _MNo;
        private FarmLandDBService farmlanddb = new FarmLandDBService();
        #endregion

        #region Create Farm Controller
        public ActionResult CreateFarm()
        {
            //Session["MapNo"] = 69;
            if (Session["MapNo"] == null)
                return RedirectToAction("Index", "ApplyIndex");
            _MNo = (int)Session["MapNo"];

            GetData gd = new GetData();
            FarmLandView ModelData = new FarmLandView();
            ModelData.IsCgh = gd.chgDesign(_MNo);
            ModelData.LandTypeList = farmlanddb.GetList();
            ModelData.CityDDL = farmlanddb.GetCityDDL();
            ModelData.TownDDL = farmlanddb.GetTownDDL("");
            ModelData.SectionList = farmlanddb.GetSectionList("");
            ModelData.Pcent_Par = "1";
            ModelData.Pcent_Chd = "1";
            ModelData.iaWMS = new MappingClass().GetWmsIaName(Convert.ToInt16(Session["UnitID"].ToString()));
            ModelData.ApplyUnit = gd.GetCaseDataFromMapNo(_MNo).ApplyUnit; 
            //ModelData.IsModify = farmlanddb.CheckFarmData(_MNo);
            ModelData.Step = gd.GetCaseDataFromMapNo(_MNo).Step; 
            ModelData.IsModify = ModelData.Step >= 2 ? true : false; 
            int Bno = new GetData().GetBeforeNowMapNo(_MNo); 
            if (Bno != _MNo && ModelData.Step == 1)
            {
                var msg = new FarmLandDBService().cloneData(Bno,_MNo);
                ModelData.IsModify = false; 
            }
            //return View(ModelData);
            return PartialView(ModelData);
        }
        public JsonResult ListFarm(int id)
        {
            /*int Bno = new GetData().GetBeforeNowMapNo(id); 
            if (Bno != id)
            {
                if (new GetData().GetCaseDataFromMapNo(id).Step == 1) 
                {
                    id = Bno; 
                }
            }*/
            List<Dry.Models.Service.FarmLandDBService.JsData> FarmData = farmlanddb.GetFarmData(id);
            var jsonData = new
            {
                rows = FarmData
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult SingleFarm(string fno)
        {
            var data = farmlanddb.GetSingleFarmData(Guid.Parse(fno));
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        #region Receive Postback LandData
        public JsonResult CreateData()
        {
            string msg = "Success";
            msg = farmlanddb.InsertFarm((int)Session["MapNo"]).DbMessage;
            if (msg == "Success")
            {
                if (Session["UnitID"].ToString() == "7" || Session["UnitID"].ToString() == "13")
                    if (!(bool)Session["Status"])
                        msg = "NotStatus";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateSingleData(FarmLandDBService.JsData Resul)
        {
            if (Resul == null)
            {
                return Content("非法進入");
            }
            else
            {
                string msg = "Success";
                Resul.MapNo = (int)Session["MapNo"];
                msg = farmlanddb.InsertFarmSingle(Resul).DbMessage;
                //if (msg == "Success")
                //{
                //    if (Session["UnitID"].ToString() == "7" || Session["UnitID"].ToString() == "13")
                //        if (!(bool)Session["Status"])
                //            msg = "NotStatus";
                //}
                return Content(msg);
            }
        }
        #endregion
        public JsonResult DelSingleData(string fno)
        {
            string msg = "Fail";
            if (fno != null)
            {
                msg = farmlanddb.DelFarmDataSingle(Guid.Parse(fno)).DbMessage;
            }
            return Json(msg,JsonRequestBehavior.AllowGet);
        }
        #region Edit Farm Controller
        public ActionResult EditFarm()
        {
            //Session["MapNo"] = 20;
            _MNo = (int)Session["MapNo"];
            FarmLandView ModelData = new FarmLandView();
            ModelData.LandTypeList = farmlanddb.GetLandTypeList();
            //ModelData.CropList = farmlanddb.GetCropList();

            ModelData.CityDDL = farmlanddb.GetCityDDL();
            ModelData.TownDDL = farmlanddb.GetTownDDL("");
            ModelData.SectionList = farmlanddb.GetSectionList("");
            //ModelData.FarmData = Json(farmlanddb.GetFarmData(_MNo));
            return View(ModelData);
        }
        #endregion

        #region Receive Postback LandData 編修回傳的js
        public ActionResult ModifyData([System.Web.Http.FromBody] List<FarmLandDBService.JsData> ResultArry)
        {
            _MNo = (int)Session["MapNo"];
            if (ResultArry == null)
                return Content("非法進入");
            else
            {
                string msg = "Success";
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

                List<string> macList = new List<string>();
                foreach (var nic in nics)
                {
                    
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        macList.Add(nic.GetPhysicalAddress().ToString());
                    }
                }
                var macs = from nic in NetworkInterface.GetAllNetworkInterfaces()
                            where nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                            select nic.GetPhysicalAddress().ToString();
                
                string IP = Request.UserHostAddress;
                string MacAddress = macs.First();

                string Hostname = Dns.GetHostEntry(IP).HostName;
                msg = farmlanddb.ModifyFarm(ResultArry, _MNo,IP,Hostname,MacAddress).DbMessage;
                return Content(msg);
            }
        }
        [HttpPost]
        
        public ActionResult ModifyDataSingle(FarmLandDBService.JsData Data)
        {
            _MNo = (int)Session["MapNo"];
                        
            if (Data == null)
                return Content("非法進入");
            else
            {
                string msg = "Success";
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                
                List<string> macList = new List<string>();
                
                foreach (var nic in nics)
                {
                    
                    if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        macList.Add(nic.GetPhysicalAddress().ToString());
                    }
                }
                
                var macs = from nic in NetworkInterface.GetAllNetworkInterfaces()
                           where nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                           select nic.GetPhysicalAddress().ToString();
                
                string IP = Request.UserHostAddress;                
                string MacAddress = macs.First();                
                //string Hostname = Dns.GetHostEntry(IP).HostName;                
                string Hostname = string.Empty;
                try
                {

                    Hostname = Dns.GetHostEntry(IP).HostName;
                    //Hostname = Dns.GetHostEntry("192.168.3.93").HostName;

                }
                catch (Exception )
                {
                //    msg = ex.Message;
                //    return Content(msg);
                //    throw;
                }
                Data.MapNo = _MNo;                
                msg = farmlanddb.ModifyFarm(Data, IP, Hostname, MacAddress).DbMessage;
                return Content(msg);
            }
        }
        #endregion
        /// <summary>
        /// 瑠公限定-匯出「加入會員申請書」
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportForLiuGongMemberPDF(string data)
        {
            Dictionary<string, string> dataStr = new Dictionary<string, string>();
            Dry.Models.ViewModel.LiuGongMemberStruct Data = JsonConvert.DeserializeObject<Dry.Models.ViewModel.LiuGongMemberStruct>(data);

            #region 設施區域資料
            dataStr.Add("Build_Area", Data.Build_Area);
            dataStr.Add("Approved_Area", Data.Approved_Area.ToString());
            int count = 1;
            foreach (var item in Data.SectionData)
            {
                dataStr.Add("Section_" + count.ToString(), item.Section);
                dataStr.Add("SubSection_" + count.ToString(), item.SubSection);
                dataStr.Add("LandType_" + count.ToString(), item.LandType);
                dataStr.Add("LandNO_" + count.ToString(), item.LandNO);
                dataStr.Add("LandArea_" + count.ToString(), item.LandArea);
                count++;
            }
            #endregion
            #region 農戶資料
            var farmerData = new Dry.Models.CommonCls.GetData().GetFarmerByMapNo(Data.MapNo);
            string[] AddrTmp = farmerData.Addr.Split(' ');
            string Addr = string.Empty;
            foreach(string item in AddrTmp)
            {
                Addr += item;
            }
            //string ZipCode = new Dry.Models.CommonCls.ZipCode().Get3Code(AddrTmp[0], AddrTmp[1]); 
            string ZipCode = new AERC.Models.CommonCls.GetTownData().GetZipCode(AddrTmp[0], AddrTmp[1]).ToString(); 
            dataStr.Add("Farmer_ZipCode", ZipCode);
            dataStr.Add("Farmer_Addr", Addr);
            dataStr.Add("Farmer_Name", farmerData.Name);
            dataStr.Add("Farmer_ID", farmerData.IdNo);
            dataStr.Add("TelNo", farmerData.Tel);
            dataStr.Add("PhoneNO", farmerData.Phone);
            
            #endregion
            

            byte[] result = new Dry.Models.CommonCls.ExportReports().ExportPDF(dataStr, Server.MapPath("~/ReportSample/LiugongMember.pdf"));
            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 會員申請書.pdf");
        }

        #region Get Farm Data
        public JsonResult GetFarmData()
        {
            _MNo = (int)Session["MapNo"];
            GetData getData = new GetData();
            bool isChg = getData.chgDesign(_MNo);
            bool isModify = farmlanddb.CheckFarmData(_MNo);
            int BMno;
            if(isModify)
            {
                BMno = _MNo;
            }
            else
            {
                if(isChg)
                {
                    BMno = farmlanddb.getMapNo(_MNo);
                }
                else
                {
                    BMno = _MNo;
                }
            }
            var obj = farmlanddb.GetFarmData(BMno);
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 取得鄉鎮列表
        public JsonResult GetTownDDL(string CityCode)
        {
            List<SelectListItem> TownDDL = farmlanddb.GetTownDDL(CityCode);
            return Json(TownDDL, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 取得地段列表
        public JsonResult GetSecDDL(string townid)
        {
            List<SelectListItem> SecDDL = farmlanddb.GetSectionList(townid);
            return Json(SecDDL, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 取得作物資料
        public JsonResult GetCropData(string query)
        {
            //var list = new CropDs();
            var datasrc = farmlanddb.GetCropData();//list.GetCropList();
            var retn_res = datasrc.Where(c => c.cropname.ToLower().Contains(query.ToLower()) || c.cropid.ToString() == query).OrderBy(c => c.cropname.Length);
            return Json(retn_res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 檢測地號重複申請
        public JsonResult ApplyChk(int sectid, string lno)
        {
            _MNo = (int)Session["MapNo"];
            //var status = farmlanddb.ChkLandNoApply(sectid, lno, _MNo);
            //20230711 alex modify using version 2,重複申請檢查第2版，依據補助項目進行面積統計分類，扣除目前案件
            var status = farmlanddb.ChkLandNoApplyV2(sectid, lno, _MNo);
            status.status = status.status.Replace("\n", "<br/>");
            return Json(status, JsonRequestBehavior.AllowGet);
        }        
        #endregion
        /// <summary>
        /// 取得地號座標位置
        /// </summary>
        /// <param name="SectionID"></param>
        /// <param name="LandCode"></param>
        /// <returns></returns>
        public string GetLandNumber(int SectionID, string LandCode)
        {
            #region old code
            //old code
            //string lanLng = new LandCoordinate().GetLandNoLatlng(SectionID, LandCode);
            //System.Web.Script.Serialization.JavaScriptSerializer objSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //CoordinateView results = objSerializer.Deserialize<CoordinateView>(lanLng);
            //if (results.landPointXY.Count() <= 0)
            //{
            //    results = new CoordinateView();
            //    results.landPointXY = new List<landPointData>();
            //    results.landPointXY.Add(new landPointData { landNo = LandCode, x = "0", y = "0" });
            //}
            #endregion
            
            
            return new LandCoordinate().GetLandNoLatlngByMoiWeb(SectionID, LandCode);
            //return new LandCoordinate().GetLandNoLatlngByMoi(SectionID, LandCode);
            //return Json(lanLng, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetLandNumberV2(int SectionID, string LandCode)
        {
            string LandnoInfo = new LandCoordinate().GetLandNoLatlngByMoiWeb(SectionID, LandCode);

            CoorPoint LandnoObj = JsonConvert.DeserializeObject<CoorPoint>(LandnoInfo);


            //var result = new { X = LandnoObj.X, Y = LandnoObj.Y, IaCNS = "灌區外",
            //    MngCNS = "灌區外", StnCNS = "灌區外", GrpCNS = "灌區外"
            //};
            if (LandnoObj.X  == "0" || LandnoObj.Y == "0")
            {
                var result = new
                {
                    X = LandnoObj.X,
                    Y = LandnoObj.Y,
                    IaCNS = "無資料",
                    MngCNS = "無資料",
                    StnCNS = "無資料",
                    GrpCNS = "無資料"
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var grp = new LandCoordinate().GetGRPgByXYWeb(decimal.Parse(LandnoObj.X) ,decimal.Parse(LandnoObj.Y) , 0);
                if (grp.Ia != null)
                {
                    var result = new
                    {
                        X = LandnoObj.X,
                        Y = LandnoObj.Y,
                        IaCNS = grp.IaCNS,
                        MngCNS = grp.MngCNS,
                        StnCNS = grp.StnCNS,
                        GrpCNS = grp.GrpCNS
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new
                    {
                        X = LandnoObj.X,
                        Y = LandnoObj.Y,
                        IaCNS = "灌區外",
                        MngCNS = "灌區外",
                        StnCNS = "灌區外",
                        GrpCNS = "灌區外"
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                
            }
            
        }

        public ActionResult GetGrpbyXY(decimal x, decimal y, int coortype)
        {
            var grp = new LandCoordinate().GetGRPgByXYWeb(x, y, coortype);
            //var grp = new LandCoordinate().GetGRPgByXY(x, y, coortype);
            if (grp.IaCNS == null)
            {
                grp.IaCNS = "灌區外";
                grp.MngCNS = "灌區外";
                grp.StnCNS = "灌區外";
                grp.GrpCNS = "灌區外";
            }
            return Json(grp, JsonRequestBehavior.AllowGet);
        }
        public ActionResult _Blank()
        {
            return View();
        }
        #region 呼叫GIS圖台

        /// <summary>
        /// 根據地段地號轉換API字串
        /// </summary>
        /// 
        /// <returns></returns>
        public ActionResult getMapData( string o )
        {
            
            List<string> fnos = JsonConvert.DeserializeObject<List<string>>(o);
            List<MapDataService.MapLandData> lnds = new List<MapDataService.MapLandData>();
            foreach (var item in fnos)
            {
                //Console.WriteLine(item.SectCode);
                //Console.WriteLine(item);
                lnds.Add( new MapDataService().getLandData(item));
            }
            List<IrrgApiParms> jobjs = new MapDataService().createMapJsonByLand(lnds);

            return Json(jobjs, JsonRequestBehavior.AllowGet);          
            

        }
        #endregion
        
    }
}
