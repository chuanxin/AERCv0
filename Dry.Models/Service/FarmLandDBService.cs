using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AERC.Models;
using System.Net;
using System.IO;
using System.Web;

namespace Dry.Models.Service
{
    public class FarmLandDBService
    {
        private DryEntities DryDB = new DryEntities();
        private CommonEntities commEnties = new CommonEntities();
        private DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
        private Dry.Models.CommonCls.CommClass comm = new Dry.Models.CommonCls.CommClass();
        private Dry.Models.CommonCls.GetData getData = new Dry.Models.CommonCls.GetData();

        #region 特定mno是否已有農地資料
        /// <summary>
        /// 特定mno是否已有農地資料
        /// </summary>
        /// <returns>bool</returns>
        public bool ChkFarmData(int mno)
        {
            if (DryDB.Farm.Any(f => f.MapNo == mno))
                return true;
            else
                return false;
        }
        #endregion

        /// <summary>
        /// 取得前一個版本的版本編號
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <returns></returns>
        public int getMapNo(int mno) {
            return getData.GetBeforeNowMapNo(mno);
        }

        #region 載入縣市DropDownList
        /// <summary>
        /// 載入縣市DropDownList
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetCityDDL()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var CityList = commEnties.City.ToList();
            items.Add(new SelectListItem() { Text = "縣市", Value = "-1" });
            foreach (var item in CityList)
            {
                items.Add(new SelectListItem()
                {
                    Text = item.City1,
                    Value = item.City_Code
                });
            }
            return items;
        }
        public List<SelectListItem> GetLiuCityDDL()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var CityList = commEnties.City.ToList();
            items.Add(new SelectListItem() { Text = "縣市", Value = "-1" });
            foreach (var item in CityList)
            {
                if(item.City_Code == "A" || item.City_Code == "F")
                {
                    items.Add(new SelectListItem()
                    {
                        Text = item.City1,
                        Value = item.City_Code
                    });
                }
            }
            return items;
        }
        #endregion

        #region 取得 指定索引的縣市DropDownList
        public List<SelectListItem> GetCityDDLByIdx(char citycode)
        {            
            List<SelectListItem> items = new List<SelectListItem>();
            var CityList = commEnties.City.ToList();
            items.Add(new SelectListItem() { Text = "縣市", Value = "-1" });
            foreach (var ctitem in CityList)
            {

                if (ctitem.City_Code.Equals(citycode))
                {
                    items.Add(new SelectListItem() { Text = ctitem.City1, Value = ctitem.City_Code.ToString(), Selected = true });
                }
                else
                {
                    items.Add(new SelectListItem() { Text = ctitem.City1, Value = ctitem.City_Code.ToString() });
                }
            }
            return items;
        }
        #endregion

        #region 載入鄉鎮市DropDownList
        /// <summary>
        /// 載入鄉鎮市DropDownList
        /// </summary>
        /// <param name="CityCode">縣市代碼</param>
        /// <returns></returns>
        public List<SelectListItem> GetTownDDL(string CityCode)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = "鄉鎮市區", Value = "-1" });
            if (CityCode.Equals(""))
                return items;
            var TownList = commEnties.Town.Where(p => p.City_Code == CityCode.ToUpper()).ToList();
            if (CityCode != "-1")
            {
                foreach (var item in TownList)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = item.Town1,
                        Value = item.Town_Id.ToString()
                    });
                }
            }
            return items;
        }
        #endregion

        #region 取得 指定索引的鄉鎮市DropDownList
        public List<SelectListItem> GetTownDDLByIdx(string CityCode, short townid)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var townList = commEnties.Town.Where(t => t.City_Code == CityCode).ToList();
            items.Add(new SelectListItem() { Text = "鄉鎮市區", Value = "-1" });
            foreach (var twitem in townList)
            {
                if (twitem.Town_Id.Equals(townid.ToString()))
                {
                    items.Add(new SelectListItem() { Text = twitem.Town1, Value = twitem.Town_Id.ToString(), Selected = true });
                }
                else
                {
                    items.Add(new SelectListItem() { Text = twitem.Town1, Value = twitem.Town_Id.ToString() });
                }
            }
            return items;
        }
        #endregion

        #region 取得地段列表
        /// <summary>
        /// 取得地段列表
        /// </summary>
        /// <param name="TownCode">鄉鎮代碼</param>
        /// <returns></returns>
        public List<SelectListItem> GetSectionList(string townid)
        {
            List<SelectListItem> section = new List<SelectListItem>();
            section.Add(new SelectListItem() { Text = "選擇地段", Value = "-1" });
            if (townid.Equals(""))
                return section;
            else
            {
                short id = Convert.ToInt16(townid);
                foreach (var item in commEnties.Section.Where(sec => sec.Town_Id == id).ToList())
                {
                    string ddl_text = item.Section1;
                    if (item.Subsection != null && item.Subsection != "")
                    {
                        ddl_text += "-" + item.Subsection;
                    }
                    section.Add(new SelectListItem() { Text = ddl_text, Value = item.Section_Id.ToString() });
                }
                return section;
            }            
        }
        #endregion

        #region 取得 指定索引的地段DropDownList
        /// <summary>
        /// 指定索引的地段DropDownList
        /// </summary>
        /// <param name="townid">鄉鎮代碼</param>
        /// <param name="sectid">地段代碼</param>
        /// <returns>地段DropDownList</returns>
        public List<SelectListItem> GetSectionByIdx(short townid, string sectid)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var sectList = commEnties.Section.Where(sec => sec.Town_Id == townid).ToList();
            items.Add(new SelectListItem() { Text = "選擇地段", Value = "-1" });
            foreach (var secitem in sectList)
            {
                if (secitem.Section_Id.Equals(sectid))
                {
                    items.Add(new SelectListItem() { Text = secitem.Section1, Value = secitem.Section_Id.ToString(), Selected = true });
                }
                else
                {
                    items.Add(new SelectListItem() { Text = secitem.Section1, Value = secitem.Section_Id.ToString() });
                }
            }
            return items;
        }
        #endregion

        #region Get the Land Type List
        public List<SelectListItem> GetLandTypeList()
        {
            List<SelectListItem> landtype = new List<SelectListItem>();
            foreach (var item in DryDB.LandTypeList.ToList())
            {
                string ddl_text = item.LandTypeCNS;
                landtype.Add(new SelectListItem() { Text = ddl_text, Value = item.LandType.ToString() });
            }
            return landtype;
        }
        #endregion

        #region Get the Crop List
        public List<SelectListItem> GetCropList()
        {
            List<SelectListItem> croplist = new List<SelectListItem>();
            var db_crlist = commEnties.Crop.ToList();
            if (db_crlist.Count > 0)
            {
                foreach (var item in commEnties.Crop.ToList())
                {
                    string ddl_text = item.Crop1;
                    croplist.Add(new SelectListItem() { Text = ddl_text, Value = item.Crop_Id.ToString() });
                }
            }
            else
            {
                croplist.Add(new SelectListItem() { Text = "Test", Value = "1" });
            }
            return croplist;
        }
        #endregion

        #region Get the Crop List By FNo
        public List<CorpData> GetCropListByFNo(Guid FNO)
        {
            List<CorpData> croplist = new List<CorpData>();
            if (DryDB.FarmCrop.Any(FC => FC.FNo == FNO))
            {
                foreach (var item in DryDB.FarmCrop.Where(FC => FC.FNo == FNO).ToList())
                {
                    //var corpname = commEnties.Crop.SingleOrDefault(C => C.Crop_Id == item.CropCode).Crop1;
                    var coptitem = commEnties.Crop.FirstOrDefault(C => C.Crop_Id == item.CropCode);
                    var corpname = "其它";
                    if (coptitem != null) corpname = coptitem.Crop1;
                    
                    croplist.Add(new CorpData() { corpcode = item.CropCode.ToString(), corpname = corpname });
                }
            }
            return croplist;
        }
        #endregion

        #region Get the Land Type Mapping Name
        public string GetLandTypeName(int landtype)
        {
            return DryDB.LandTypeList.Single(C => C.LandType == landtype).LandTypeCNS;
        }
        #endregion

        #region Get SectCode Mapping Name
        public string GetSectCodeName(int sectcode)
        {
            var section = commEnties.Section.Single(Sec => Sec.Section_Id == sectcode);
            string retn_str = section.Section1;
            if (!section.Subsection.Equals(""))
                retn_str += "-" + section.Subsection;
            return retn_str;
        }
        #endregion

        #region Get FarmData List
        public List<JsData> GetFarmData(int mno)
        {
            List<JsData> farmlist = new List<JsData>();
            bool hasFarm = DryDB.Farm.Any(m => m.MapNo == mno);
            int NewMno = mno;
            if (!hasFarm)
            {
                mno = getData.GetBeforeNowMapNo(mno);
                if (mno == NewMno)
                    hasFarm = true;
            }
            List<Farm> Farms = DryDB.Farm.Where(f => f.MapNo == mno).OrderBy(m => m.Section).ToList();
            string msg = "Success";
            if (!hasFarm)
            {
                msg = InsertFarm(Farms, NewMno).DbMessage;
            }
            if (msg == "Success")
            {
                foreach (var item in Farms)
                {
                    var CorpAry = GetCropListByFNo(item.FNo);
                    var LandEquityAry = DryDB.LandEquity.Where(LE => LE.FNo == item.FNo).ToList();
                    List<HolderData> HolderAry = new List<HolderData>();
                    foreach (var landEq in LandEquityAry)
                    {
                        HolderAry.Add(new HolderData()
                        {
                            name = landEq.Name,
                            id = landEq.IdNo,
                            addr = landEq.Addr,
                            area = landEq.EquityArea ?? 0,
                            perC = landEq.Pcent_Chd ?? 0,
                            perP = landEq.Pcent_Par ?? 0
                        }
                        );
                    }

                    short twid = commEnties.Section.Any(s => s.Section_Id == (short)item.Section) ? (short)commEnties.Section.Single(s => s.Section_Id == (short)item.Section).Town_Id : (short)0;
                    if(twid!=0)
                    {
                        Town t = commEnties.Town.Single(twn => twn.Town_Id == twid);
                        string townname = t.Town1;

                        string ctcode = t.City_Code;
                        City ct = commEnties.City.Single(cty => cty.City_Code == t.City_Code);
                        string ctname = ct.City1;
                        string sectionname = GetSectCodeName(item.Section);
                        bool isApplied = item.IsApplied ?? false;
                        farmlist.Add(new JsData
                        {
                            FNo = item.FNo,
                            MapNo = item.MapNo,
                            Area = item.FarmArea,
                            BArea = item.BuildArea,
                            Corp = CorpAry,
                            Holder = HolderAry,
                            LandNo = item.LandNo,
                            Lat = item.Lat,
                            Long = item.Long,
                            OutSide = item.Outside,
                            OutSideStr = item.Outside ? "是" : "否",
                            CTName = ct.City1,
                            CTCode = ctcode,
                            twName = townname,
                            twCode = twid,
                            SectCode = (short)item.Section,
                            SectName = sectionname,
                            FullName = ctname + "-" + townname + "-" + sectionname,
                            TypeCode = item.LandType,
                            TypeName = GetLandTypeName(item.LandType),
                            Pcent_Par = item.Pcent_Par,
                            Pcent_Chd = item.Pcent_Chd,
                            ApplicationStatus = item.ApplicationStatus ?? "",
                            IsApplied = isApplied,
                            IsAppliedStr = isApplied ? "是" : "否",
                            CorpCount = CorpAry.Count(),
                            HolderCount = HolderAry.Count()
                        });
                    }
                    else
                    {
                        farmlist.Add(new JsData());
                    }
                }
            }


            return farmlist;
        }
        public bool CheckFarmData(int MapNo)
        {
            return DryDB.Farm.Any(p => p.MapNo == MapNo);
        }
        #endregion

        public JsData GetSingleFarmData(Guid fid)
        {
            JsData ReturnData = new JsData();
            Farm FarmData = DryDB.Farm.Find(fid);
            if(FarmData!=null)
            {
                var CorpAry = GetCropListByFNo(fid);
                var LandEquityAry = DryDB.LandEquity.Where(LE => LE.FNo == fid).ToList();
                List<HolderData> HolderAry = new List<HolderData>();
                foreach (var landEq in LandEquityAry)
                {
                    HolderAry.Add(new HolderData()
                    {
                        name = landEq.Name,
                        id = landEq.IdNo,
                        addr = landEq.Addr,
                        area = landEq.EquityArea ?? 0,
                        perC = landEq.Pcent_Chd ?? 0,
                        perP = landEq.Pcent_Par ?? 0
                    }
                    );
                }
                short twid = (short)commEnties.Section.Single(s => s.Section_Id == (short)FarmData.Section).Town_Id;
                Town t = commEnties.Town.Single(twn => twn.Town_Id == twid);
                string townname = t.Town1;

                string ctcode = t.City_Code;
                City ct = commEnties.City.Single(cty => cty.City_Code == t.City_Code);
                string ctname = ct.City1;
                string sectionname = GetSectCodeName(FarmData.Section);
                bool isApplied = FarmData.IsApplied ?? false;
                ReturnData.FNo = FarmData.FNo;
                ReturnData.MapNo = FarmData.MapNo;
                ReturnData.Area = FarmData.FarmArea;
                ReturnData.BArea = FarmData.BuildArea;
                ReturnData.Corp = CorpAry;
                ReturnData.Holder = HolderAry;
                ReturnData.LandNo = FarmData.LandNo;
                ReturnData.Lat = FarmData.Lat;
                ReturnData.Long = FarmData.Long;
                ReturnData.OutSide = FarmData.Outside;
                ReturnData.OutSideStr = FarmData.Outside ? "是" : "否";
                ReturnData.CTName = ct.City1;
                ReturnData.CTCode = ctcode;
                ReturnData.twName = townname;
                ReturnData.twCode = twid;
                ReturnData.SectCode = (short)FarmData.Section;
                ReturnData.SectName = sectionname;
                ReturnData.FullName = ctname + "-" + townname + "-" + sectionname;
                ReturnData.TypeCode = FarmData.LandType;
                ReturnData.TypeName = GetLandTypeName(FarmData.LandType);
                ReturnData.Pcent_Par = FarmData.Pcent_Par;
                ReturnData.Pcent_Chd = FarmData.Pcent_Chd;
                ReturnData.ApplicationStatus = FarmData.ApplicationStatus ?? "";
                ReturnData.IsApplied = isApplied;
                ReturnData.IsAppliedStr = isApplied ? "是" : "否";
                ReturnData.CorpCount = CorpAry.Count();
                ReturnData.HolderCount = HolderAry.Count();
            }
            
            return ReturnData;
        }

        #region DB Event

        #region Create Farm
        /// <summary>
        /// Farm Data
        /// </summary>
        /// <param name="Fdata"> Farm Object</param>
        /// <returns>DbEvent Result</returns>
        public DBCommon.DbEvent CreateFarm(Farm Fdata)
        {
            DryDB = new DryEntities();
            //DryDB.Farm.Add(Fdata);
            //DryDB.SaveChanges();
            //dbstatus.KeyValue = Fdata.FNo;
            //dbstatus.DbMessage = "Fail";
            try
            {
                DryDB.Farm.Add(Fdata);
                DryDB.SaveChanges();
                dbstatus.KeyValue = Fdata.FNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Create Farm Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Update Farm
        public DBCommon.DbEvent UpdateFarm(Farm data)
        {
            try
            {
                DryDB.Farm.Attach(data);
                DryDB.Entry(data).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.FNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modified Farm Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Delete Farm
        public DBCommon.DbEvent DeleteFarm(Farm data)
        {
            try
            {
                dbstatus.KeyValue = data.FNo;
                DryDB.Farm.Remove(data);
                DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Farm Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Delete Farm via MapNo
        /// <summary>
        /// Delete Farm via MapNo
        /// </summary>
        /// <param name="mapno">Mapping No</param>
        /// <returns>DB Event Result</returns>
        public DBCommon.DbEvent DelFarmViaMNo(int mapno)
        {
            try
            {
                dbstatus.KeyValue = mapno;
                DryDB.Farm.Where(f => f.MapNo == mapno).ToList().ForEach(farmitem => DryDB.Farm.Remove(farmitem));
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Farm via MNo Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion


        #region Create FarmCrop
        /// <summary>
        /// Create FarmCrop
        /// </summary>
        /// <param name="FCdata">FarmCrop Object</param>
        /// <returns>DbEvent Result</returns>
        public DBCommon.DbEvent CreateFarmCrop(FarmCrop FCdata)
        {
            try
            {
                DryDB.FarmCrop.Add(FCdata);
                DryDB.SaveChanges();
                dbstatus.KeyValue = FCdata.No;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Create FarmCrop Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Delete FarmCrop
        public DBCommon.DbEvent DeleteFarmCrop(FarmCrop data)
        {
            try
            {
                dbstatus.KeyValue = data.No;
                DryDB.FarmCrop.Remove(data);
                DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed FarmCorp Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Delete Farm Crop By FNo
        /// <summary>
        /// Delete Farm Crop via same FNo
        /// </summary>
        /// <param name="fno">Farm No</param>
        /// <returns>DB Event Result</returns>
        public DBCommon.DbEvent DelCropByFNo(Guid fno)
        {
            //var CorpList = DryDB.FarmCrop.Where(f => f.FNo == fno);            
            try
            {
                dbstatus.KeyValue = fno;
                DryDB.FarmCrop.Where(f => f.FNo == fno).ToList().ForEach(cropitem => DryDB.FarmCrop.Remove(cropitem));
                // DryDB.Entry(CorpList).State = System.Data.EntityState.Deleted;               
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Farm Corps Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Create LandEquity
        /// <summary>
        /// Create LandEquity Data
        /// </summary>
        /// <param name="LECdata">LandEquity Object</param>
        /// <returns>DbEvent Result</returns>
        public DBCommon.DbEvent CreateLandEquity(LandEquity LECdata)
        {
            try
            {
                DryDB.LandEquity.Add(LECdata);
                DryDB.SaveChanges();
                dbstatus.KeyValue = LECdata.LNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Create LandEquity Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Delete Land Equity
        public DBCommon.DbEvent DeleteLandEquity(LandEquity data)
        {
            try
            {
                dbstatus.KeyValue = data.LNo;
                DryDB.LandEquity.Remove(data);
                DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed LandEquity Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Delete Land Equity via FNo
        public DBCommon.DbEvent DeleteLandEquityByFNo(Guid fno)
        {
            try
            {
                dbstatus.KeyValue = fno;
                DryDB.LandEquity.Where(f => f.FNo == fno).ToList().ForEach(landeuqitem => DryDB.LandEquity.Remove(landeuqitem));
                // DryDB.Entry(CorpList).State = System.Data.EntityState.Deleted;               
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Land Holder Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #endregion

        #region 取得新FNo
        /// <summary>
        /// 取得新FNo
        /// </summary>
        /// <returns></returns>
        public Guid GetNewFNo()
        {
            return Guid.NewGuid();
        }
        #endregion

        #region Get Crop Data
        public List<Dry.Models.CommonCls.GetData.CropStruct> GetCropData()
        {
            return getData.GetCrop();
        }
        #endregion

        #region check landno apply
        public Dry.Models.CommonCls.GetData.MsgStruct ChkLandNoApply(int sectid,string landno,int mno)
        {
            landno = SpileLandNo(landno);
            return getData.ChkApply(sectid, landno);
            //var t = getData.ChkApplyByItem(sectid, landno, 1, mno);
            //return getData.ChkApply4Case(sectid, landno, mno);
        }
        #endregion
        //public class CropStruct : Dry.Models.CommonCls.GetData.CropStruct { }

        public CommonCls.GetData.MsgStruct ChkLandNoApplyV2(int sectid, string landno, int mno)
        {
            landno = SpileLandNo(landno);
            return getData.ChkApply4Case(sectid, landno, mno);
        }

        #region Insert Farm Data To DB
        /// <summary>
        /// Insert Farm Data To Database
        /// </summary>
        /// <param name="ResultArry">Js data</param>
        /// <param name="mno">版本編號</param>
        /// <returns></returns>
        public DBCommon.DbEvent InsertFarm(/*List<JsData> ResultArry, */int mno)
        {
            string msg = "Success";
            using (TransactionScope scope = new TransactionScope())
            {
                //double area = 0;
                //foreach (var item in ResultArry)
                //{
                //    Farm farmObj = new Farm();
                //    farmObj.FNo = GetNewFNo();
                //    farmObj.MapNo = mno;
                //    farmObj.Section = item.SectCode;
                //    farmObj.LandNo = SpileLandNo(item.LandNo);
                //    farmObj.LandType = item.TypeCode;
                //    farmObj.FarmArea = item.Area;
                //    farmObj.BuildArea = item.BArea;
                //    farmObj.FinalArea = item.BArea;
                //    farmObj.Outside = item.OutSide;
                //    farmObj.IsApplied = item.IsApplied;
                //    farmObj.Long = item.Long;
                //    farmObj.Lat = item.Lat;
                //    farmObj.Pcent_Chd = item.Pcent_Chd;
                //    farmObj.Pcent_Par = item.Pcent_Par;

                //    area += item.BArea;

                //    msg = CreateFarm(farmObj).DbMessage;
                //    if (msg.Equals("Success"))
                //    {
                //        foreach (var corp in item.Corp)
                //        {
                //            FarmCrop farmcorpObj = new FarmCrop();
                //            farmcorpObj.FNo = farmObj.FNo;
                //            farmcorpObj.CropCode = corp.corpcode;
                //            msg = CreateFarmCrop(farmcorpObj).DbMessage;
                //            if (!msg.Equals("Success"))
                //            {
                //                break;
                //            }
                //        }

                //        if (msg.Equals("Success") && item.Holder != null)
                //        {
                //            foreach (var hold in item.Holder)
                //            {
                //                LandEquity LEObj = new LandEquity();
                //                LEObj.FNo = farmObj.FNo;
                //                LEObj.Name = hold.name;
                //                LEObj.Addr = hold.addr;
                //                LEObj.EquityArea = hold.area;
                //                if (hold.id != "")
                //                    LEObj.IdNo = hold.id;
                //                LEObj.Pcent_Chd = hold.perC;
                //                LEObj.Pcent_Par = hold.perP;
                //                LEObj.IsMember = false;

                //                msg = CreateLandEquity(LEObj).DbMessage;
                //                if (!msg.Equals("Success"))
                //                {
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //    if (!msg.Equals("Success"))
                //    {
                //        break;
                //    }
                //}
                //if (msg.Equals("Success"))//若成功則更新目前步驟
                //{
                    msg = comm.UpdateCase(mno, 2).DbMessage;
                //}
                //if (msg.Equals("Success"))
                //{//寫入BudgetBook資料表
                //    msg = SaveBudgetBook(mno, area).DbMessage;
                //}
                if (msg.Equals("Success"))
                {
                    scope.Complete();
                }
            }
            dbstatus.DbMessage = msg;
            return dbstatus;
        }
        public DBCommon.DbEvent InsertFarm(List<Farm> Data,int NewMapNo)
        {
            List<FarmCrop> farmcrops = DryDB.FarmCrop.ToList();
            List<LandEquity> holders = DryDB.LandEquity.ToList();
            string msg = "Success";
            using (TransactionScope scope = new TransactionScope())
            {
                foreach (var item in Data)
                {
                    Farm farm = new Farm();
                    farm.FNo = Guid.NewGuid();
                    farm.MapNo = NewMapNo;
                    farm.Section = item.Section;
                    farm.LandNo = item.LandNo;
                    farm.LandType = item.LandType;
                    farm.Long = item.Long;
                    farm.Lat = item.Lat;
                    farm.FarmArea = item.FarmArea;
                    farm.BuildArea = item.BuildArea;
                    farm.FinalArea = item.FinalArea;
                    farm.IsApplied = item.IsApplied;
                    farm.ApplicationStatus = item.ApplicationStatus;
                    farm.AppliedArea = item.AppliedArea;
                    farm.AreaCanApply = item.AreaCanApply;
                    farm.Outside = item.Outside;
                    farm.Pcent_Chd = item.Pcent_Chd;
                    farm.Pcent_Par = item.Pcent_Par;
                    msg = CreateFarm(farm).DbMessage;
                    if (msg.Equals("Success"))
                    {
                        List<FarmCrop> crops = farmcrops.Where(m => m.FNo == item.FNo).ToList();
                        foreach (var corp in crops)
                        {
                            FarmCrop farmcrop = new FarmCrop();
                            farmcrop.FNo = farm.FNo;
                            farmcrop.CropCode = corp.CropCode;
                            msg = CreateFarmCrop(farmcrop).DbMessage;
                            if (!msg.Equals("Success"))
                            {
                                break;
                            }
                        }
                        List<LandEquity> Holder = holders.Where(m => m.FNo == item.FNo).ToList();
                        if (msg.Equals("Success") && Holder != null)
                        {
                            foreach (var hold in Holder)
                            {
                                LandEquity newhoder = new LandEquity();
                                newhoder.FNo = farm.FNo;
                                newhoder.Name = hold.Name;
                                newhoder.IdNo = hold.IdNo;
                                newhoder.Addr = hold.Addr;
                                newhoder.Pcent_Chd = hold.Pcent_Chd;
                                newhoder.Pcent_Par = hold.Pcent_Par;
                                newhoder.EquityArea = hold.EquityArea;
                                newhoder.IsMember = hold.IsMember;
                                newhoder.Note = hold.Note;
                                msg = CreateLandEquity(newhoder).DbMessage;
                                if (!msg.Equals("Success"))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    if (!msg.Equals("Success"))
                    {
                        break;
                    }
                }
                if (msg.Equals("Success"))
                {
                    scope.Complete();
                }
            }
            dbstatus.DbMessage = msg;
            return dbstatus;
        }

        public DBCommon.DbEvent InsertFarmSingle(JsData Result)
        {
            string msg = "Success";
            using (TransactionScope scope = new TransactionScope())
            {
                double area = 0;
                Farm farmObj = new Farm();
                farmObj.FNo = GetNewFNo();
                farmObj.MapNo = Result.MapNo;
                farmObj.Section = Result.SectCode;
                farmObj.LandNo = SpileLandNo(Result.LandNo);
                farmObj.LandType = Result.TypeCode;
                farmObj.FarmArea = Result.Area;
                farmObj.BuildArea = Result.BArea;
                farmObj.FinalArea = Result.BArea;
                farmObj.Outside = Result.OutSide;
                farmObj.IsApplied = Result.IsApplied;
                farmObj.Long = Result.Long;
                farmObj.Lat = Result.Lat;
                farmObj.Pcent_Chd = Result.Pcent_Chd;
                farmObj.Pcent_Par = Result.Pcent_Par;
                farmObj.ApplicationStatus = Result.ApplicationStatus;

                area += Result.BArea;
                List<CorpData> Cropdata = Result.Cpp;
                msg = CreateFarm(farmObj).DbMessage;
                if (msg.Equals("Success"))
                {
                    foreach (var corp in Cropdata)
                    {
                        FarmCrop farmcorpObj = new FarmCrop();
                        farmcorpObj.FNo = farmObj.FNo;
                        farmcorpObj.CropCode = Convert.ToInt16(corp.corpcode);
                        msg = CreateFarmCrop(farmcorpObj).DbMessage;
                    }

                    if (msg.Equals("Success") && Result.Holder != null)
                    {
                        foreach (var hold in Result.Holder)
                            {
                                LandEquity LEObj = new LandEquity();
                                LEObj.FNo = farmObj.FNo;
                                LEObj.Name = hold.name;
                                LEObj.Addr = hold.addr;
                                LEObj.EquityArea = hold.area;
                                if (hold.id != "")
                                    LEObj.IdNo = hold.id;
                                LEObj.Pcent_Chd = hold.perC;
                                LEObj.Pcent_Par = hold.perP;
                                LEObj.IsMember = false;

                                msg = CreateLandEquity(LEObj).DbMessage;
                            }
                        
                    }
                }
                if (msg.Equals("Success"))
                {
                    scope.Complete();
                }
            }
            dbstatus.DbMessage = msg;
            return dbstatus;
        }
        #endregion

        class LogData
        {
            public Farm FarmLogData { get; set; }
            public List<FarmCrop> FarmCropLogData { get; set; }
            public List<LandEquity> LandEquityLogData { get; set; }
        }

        #region Modify Farm Data To DB
        /// <summary>
        /// Modify Farm Data To Database
        /// </summary>
        /// <param name="ResultArry">Js data</param>
        /// <param name="mno">版本編號</param>
        /// <returns></returns>
        public DBCommon.DbEvent ModifyFarm(List<JsData> ResultArry, int mno,string ip,string hostname,string macaddress)
        {
            string msg = "Success";
            List<LogData> logData = new List<LogData>();
            
            using (TransactionScope scope = new TransactionScope())
            {
                
                msg = DelFarmViaMNo(mno).DbMessage;
                double area=0;
                if(msg.Equals("Success"))
                {
                    foreach (var item in ResultArry)
                    {
                        List<FarmCrop> FarmCropLog = new List<FarmCrop>(); 
                        List<LandEquity> LandEquityLog = new List<LandEquity>(); 


                        Farm farmObj = new Farm();
                        farmObj.FNo = GetNewFNo();
                        farmObj.MapNo = mno;
                        farmObj.Section = item.SectCode;
                        farmObj.LandNo = item.LandNo;
                        farmObj.LandType = item.TypeCode;
                        farmObj.FarmArea = item.Area;
                        farmObj.BuildArea = item.BArea;
                        farmObj.FinalArea = item.BArea;
                        farmObj.Outside = item.OutSide;
                        farmObj.IsApplied = item.IsApplied;
                        farmObj.Long = item.Long;
                        farmObj.Lat = item.Lat;

                        area += item.BArea; 

                        msg = CreateFarm(farmObj).DbMessage;

                        if (msg.Equals("Success"))
                        {
                            foreach (var corp in item.Corp)
                            {
                                FarmCrop farmcorpObj = new FarmCrop();
                                farmcorpObj.FNo = farmObj.FNo;
                                farmcorpObj.CropCode = Convert.ToInt16(corp.corpcode);

                                FarmCropLog.Add(farmcorpObj); 

                                msg = CreateFarmCrop(farmcorpObj).DbMessage;
                                if (!msg.Equals("Success"))
                                {
                                    break;
                                }
                            }
                            if (msg.Equals("Success"))
                            {
                                if (item.Holder != null)
                                {
                                    
                                    var rs = DeleteLandEquityByFNo(farmObj.FNo);
                                    if (rs.Equals("Success"))
                                    {

                                        foreach (var hold in item.Holder)
                                        {
                                            LandEquity LEObj = new LandEquity();
                                            LEObj.FNo = farmObj.FNo;
                                            LEObj.Name = hold.name;
                                            LEObj.Addr = hold.addr;
                                            LEObj.EquityArea = hold.area;
                                            LEObj.IdNo = hold.id;
                                            LEObj.Pcent_Chd = hold.perC;
                                            LEObj.Pcent_Par = hold.perP;
                                            LEObj.IsMember = false;

                                            LandEquityLog.Add(LEObj); 

                                            msg = CreateLandEquity(LEObj).DbMessage;
                                            if (!msg.Equals("Success"))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else { msg = rs.DbMessage; }
                                }
                            }
                        }

                        logData.Add(new LogData { FarmLogData = farmObj, FarmCropLogData = FarmCropLog, LandEquityLogData = LandEquityLog }); 

                        if (!msg.Equals("Success"))
                        {
                            msg = "Modify Farm Has Error!";
                            break;
                        }
                    }
                }
                //if (msg.Equals("Success"))
                //{//寫入BudgetBook資料表
                //    msg = SaveBudgetBook(mno, area).DbMessage;
                //}
                if(msg.Equals("Success"))
                {
                    msg = EditPigingConfWorkPrice(mno).DbMessage;
                    if(msg.Equals("Success"))
                    {
                        msg = new TotalFeeDBService().UpdateTotalFee(mno).DbMessage;
                    }
                }
                if (msg.Equals("Success"))
                {
                    scope.Complete();
                }
            }

            #region 記錄Log專用
            string FarmDataTemp = "";
            string FarmCropDataTemp = "";
            string LandEquityDataTemp = "";
            FarmDataLog DataLog = new FarmDataLog();
            DataLog.MapNo = mno;
            DataLog.DBMessage = msg;
            foreach (var item in logData)
            {
                FarmDataTemp += "FNo:" + item.FarmLogData.FNo + ", MapNo:" + item.FarmLogData.MapNo + ", Section:" + item.FarmLogData.Section + ", LandNo:" +
                    item.FarmLogData.LandNo + ", LandType:" + item.FarmLogData.LandType + ", Long:" + item.FarmLogData.Long + ", Lat:" + item.FarmLogData.Lat + ", FarmArea:" + item.FarmLogData.FarmArea +
                    ", BuildArea:" + item.FarmLogData.BuildArea + ", Outside:" + item.FarmLogData.Outside + ", Pcent_Chd:" + item.FarmLogData.Pcent_Chd + ", Pcent_Par:" + item.FarmLogData.Pcent_Par + " ; ";
                foreach (var item2 in item.FarmCropLogData)
                {
                    FarmCropDataTemp += "No:" + item2.No + ", FNo:" + item2.FNo + ", CropCode:" + item2.CropCode + " ; ";
                }
                foreach (var item3 in item.LandEquityLogData)
                {
                    LandEquityDataTemp += "LNo:" + item3.LNo + ", FNo:" + item3.FNo + ", Name:" + item3.Name + ", IdNo:" + item3.IdNo + ", Addr:" + item3.Addr + ", Pcent_Chd:" + item3.Pcent_Chd +
                        ", Pcent_Par:" + item3.Pcent_Par + ", EquityArea:" + item3.EquityArea + ", IsMember:" + item3.IsMember + ", Note:" + item3.Note;
                }
            }
            FarmDataLog FarmLog = new FarmDataLog();
            FarmLog.MapNo = mno;
            FarmLog.DBMessage = msg;
            FarmLog.Farm_Data = FarmDataTemp;
            FarmLog.FarmCrop_Data = FarmCropDataTemp;
            FarmLog.LandEquity_Data = LandEquityDataTemp;
            FarmLog.UserIP = ip;
            FarmLog.UserHostName = hostname;
            FarmLog.UserMac = macaddress;
            FarmLog.EventTime = DateTime.Now;
            try
            {
                DryDB.FarmDataLog.Add(FarmLog);
                DryDB.SaveChanges();
                //dbstatus.KeyValue = LECdata.LNo;
                //dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                //dbstatus.DbMessage = "Create LandEquity Failed : " + e.Message + "<br>";
            }
            #endregion
            


            dbstatus.DbMessage = msg;
            return dbstatus;

            #region Old Code
            /*
            string msg = "Success";
            using (TransactionScope scope = new TransactionScope())
            {
                int countidx =0;
                //先刪除所有農地資料，以頁面回傳之農地資料為準
                DelFarmViaMNo(mno);

                foreach (var item in ResultArry)
                {
                    if (item.FNo == null)//this farm is new add
                    {
                        msg = InsertFarm(new List<JsData> { ResultArry[countidx] }, mno).DbMessage;
                    }
                    else
                    {
                        Farm farmObj = DryDB.Farm.Single(f => f.FNo == item.FNo);
                        farmObj.Section = item.SectCode;
                        farmObj.LandNo = item.LandNo;
                        farmObj.LandType = item.TypeCode;
                        farmObj.FarmArea = item.Area;
                        farmObj.BuildArea = item.BArea;
                        farmObj.Outside = item.OutSide;
                        farmObj.Long = item.Long;
                        farmObj.Lat = item.Lat;

                        msg = UpdateFarm(farmObj).DbMessage;

                        //remove all corps by this FNo
                        msg = DelCropByFNo(farmObj.FNo).DbMessage;
                        if (msg.Equals("Success") && item.Corp != null)
                        {
                            foreach (var corp in item.Corp)
                            {
                                FarmCrop farmcorpObj = new FarmCrop();
                                farmcorpObj.FNo = farmObj.FNo;
                                farmcorpObj.CropCode = corp.corpcode;
                                msg = CreateFarmCrop(farmcorpObj).DbMessage;
                                if (!msg.Equals("Success"))
                                {
                                    break;
                                }
                            }

                            msg = DeleteLandEquityByFNo(farmObj.FNo).DbMessage;
                            if (msg.Equals("Success") && item.Holder != null)
                            {
                                foreach (var hold in item.Holder)
                                {
                                    LandEquity LEObj = new LandEquity();
                                    LEObj.FNo = farmObj.FNo;
                                    LEObj.Name = hold.name;
                                    LEObj.Addr = hold.addr;
                                    LEObj.EquityArea = hold.area;
                                    LEObj.IdNo = hold.id;
                                    LEObj.Pcent_Chd = hold.perC;
                                    LEObj.Pcent_Par = hold.perP;
                                    LEObj.IsMember = false;

                                    msg = CreateLandEquity(LEObj).DbMessage;
                                    if (!msg.Equals("Success"))
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!msg.Equals("Success"))
                    {
                        break;
                    }
                }
                if (msg.Equals("Success"))
                {
                    scope.Complete();
                }
            }
            */
            #endregion
        }

        //new
        public DBCommon.DbEvent ModifyFarm(JsData FarmData, string ip, string hostname, string macaddress)
        {
            string msg = "Success";
            List<LogData> logData = new List<LogData>();

            using (TransactionScope scope = new TransactionScope())
            {
                List<FarmCrop> FarmCropLog = new List<FarmCrop>(); 
                List<LandEquity> LandEquityLog = new List<LandEquity>(); 

                Farm farmObj = DryDB.Farm.Find(FarmData.FNo);
                //farmObj.FNo = GetNewFNo();
                //farmObj.MapNo = FarmData.MapNo;
                farmObj.Section = FarmData.SectCode;
                farmObj.LandNo = SpileLandNo(FarmData.LandNo);
                farmObj.LandType = FarmData.TypeCode;
                farmObj.FarmArea = FarmData.Area;
                farmObj.BuildArea = FarmData.BArea;
                farmObj.FinalArea = FarmData.BArea;
                farmObj.Outside = FarmData.OutSide;
                farmObj.ApplicationStatus = FarmData.ApplicationStatus;
                farmObj.IsApplied = FarmData.IsApplied;
                farmObj.Long = FarmData.Long;
                farmObj.Lat = FarmData.Lat;
                farmObj.Pcent_Chd = FarmData.Pcent_Chd;
                farmObj.Pcent_Par = FarmData.Pcent_Par;


                msg = UpdateFarm(farmObj).DbMessage;

                if (msg.Equals("Success"))
                {
                    
                    msg = DelCropByFNo(farmObj.FNo).DbMessage;
                    if(msg.Equals("Success"))
                    {
                        //foreach (var corp in FarmData.Corp)
                        foreach (var corp in FarmData.Cpp)
                        {
                            FarmCrop farmcorpObj = new FarmCrop();
                            farmcorpObj.FNo = farmObj.FNo;
                            farmcorpObj.CropCode = Convert.ToInt16(corp.corpcode);

                            FarmCropLog.Add(farmcorpObj); 
                            FarmCrop DelData = DryDB.FarmCrop.Where(m => m.FNo == farmObj.FNo).FirstOrDefault();
                            msg = CreateFarmCrop(farmcorpObj).DbMessage;
                        }
                        
                    }
                    
                    if (msg.Equals("Success"))
                    {
                        if (FarmData.Holder != null)
                        {
                            
                            string rs = DeleteLandEquityByFNo(farmObj.FNo).DbMessage;
                            if (rs.Equals("Success"))
                            {


                                foreach (var hold in FarmData.Holder)
                                {
                                    LandEquity LEObj = new LandEquity();
                                    LEObj.FNo = farmObj.FNo;
                                    LEObj.Name = hold.name;
                                    LEObj.Addr = hold.addr;
                                    LEObj.EquityArea = hold.area;
                                    LEObj.IdNo = hold.id;
                                    LEObj.Pcent_Chd = hold.perC;
                                    LEObj.Pcent_Par = hold.perP;
                                    LEObj.IsMember = false;

                                    LandEquityLog.Add(LEObj); 

                                    msg = CreateLandEquity(LEObj).DbMessage;
                                }
                                
                            }
                            else { msg = rs; }
                        }
                        else
                        {
                            
                            string rs = DeleteLandEquityByFNo(farmObj.FNo).DbMessage;
                            msg = rs;
                        }
                    }
                }

                logData.Add(new LogData { FarmLogData = farmObj, FarmCropLogData = FarmCropLog, LandEquityLogData = LandEquityLog }); 

                if (!msg.Equals("Success"))
                {
                    msg = "Modify Farm Has Error!";
                }
                if (msg.Equals("Success"))
                {
                    msg = EditPigingConfWorkPrice(FarmData.MapNo).DbMessage;
                    if (msg.Equals("Success"))
                    {
                        msg = new TotalFeeDBService().UpdateTotalFee(FarmData.MapNo).DbMessage;
                    }
                }
                if (msg.Equals("Success"))
                {
                    scope.Complete();
                }
            }

            

            #region 記錄Log專用
            string FarmDataTemp = "";
            string FarmCropDataTemp = "";
            string LandEquityDataTemp = "";
            FarmDataLog DataLog = new FarmDataLog();
            DataLog.MapNo = FarmData.MapNo;
            DataLog.DBMessage = msg;
            foreach (var item in logData)
            {
                FarmDataTemp += "FNo:" + item.FarmLogData.FNo + ", MapNo:" + item.FarmLogData.MapNo + ", Section:" + item.FarmLogData.Section + ", LandNo:" +
                    item.FarmLogData.LandNo + ", LandType:" + item.FarmLogData.LandType + ", Long:" + item.FarmLogData.Long + ", Lat:" + item.FarmLogData.Lat + ", FarmArea:" + item.FarmLogData.FarmArea +
                    ", BuildArea:" + item.FarmLogData.BuildArea + ", Outside:" + item.FarmLogData.Outside + ", Pcent_Chd:" + item.FarmLogData.Pcent_Chd + ", Pcent_Par:" + item.FarmLogData.Pcent_Par + " ; ";
                foreach (var item2 in item.FarmCropLogData)
                {
                    FarmCropDataTemp += "No:" + item2.No + ", FNo:" + item2.FNo + ", CropCode:" + item2.CropCode + " ; ";
                }
                foreach (var item3 in item.LandEquityLogData)
                {
                    LandEquityDataTemp += "LNo:" + item3.LNo + ", FNo:" + item3.FNo + ", Name:" + item3.Name + ", IdNo:" + item3.IdNo + ", Addr:" + item3.Addr + ", Pcent_Chd:" + item3.Pcent_Chd +
                        ", Pcent_Par:" + item3.Pcent_Par + ", EquityArea:" + item3.EquityArea + ", IsMember:" + item3.IsMember + ", Note:" + item3.Note;
                }
            }
            FarmDataLog FarmLog = new FarmDataLog();
            FarmLog.MapNo = FarmData.MapNo;
            FarmLog.DBMessage = msg;
            FarmLog.Farm_Data = FarmDataTemp;
            FarmLog.FarmCrop_Data = FarmCropDataTemp;
            FarmLog.LandEquity_Data = LandEquityDataTemp;
            FarmLog.UserIP = ip;
            FarmLog.UserHostName = hostname;
            FarmLog.UserMac = macaddress;
            FarmLog.EventTime = DateTime.Now;
            try
            {
                DryDB.FarmDataLog.Add(FarmLog);
                DryDB.SaveChanges();
                //dbstatus.KeyValue = LECdata.LNo;
                //dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                //dbstatus.DbMessage = "Create LandEquity Failed : " + e.Message + "<br>";
            }
            #endregion



            dbstatus.DbMessage = msg;
            return dbstatus;
        }
        #endregion

        private DBCommon.DbEvent EditPigingConfWorkPrice(int mno)
        {
            int WorkPrice = 0;
            double area = DryDB.Farm.Where(m => m.MapNo == mno).Sum(n => n.BuildArea);
            FarmerSysPriceService sysPrice = new FarmerSysPriceService();
            FarmerSysDBService sysdb = new FarmerSysDBService();
            short ApplyUnit = getData.GetCaseDataFromMapNo(mno).ApplyUnit;
            List<EndType> endtypeData = getData.GetEndTypeData(mno);
            FarmerSysView.ParaJsonData Data = new FarmerSysView.ParaJsonData();
            Data.EndTypeDataAry = new List<FarmerSysView.EndTypeStruct>();
            foreach(var item in endtypeData)
            {
                
                Data.EndTypeDataAry.Add(new FarmerSysView.EndTypeStruct { Endtype = item.EndTypeCode, Fac = item.FacType, SS = item.SS, SL = item.SL, StdpipeHei = item.StdpipeHei.Value });
            }
            
            if (Data.EndTypeDataAry.Count > 0)
            {
                
                //if (ApplyUnit == 17)
                //{
                //    WorkPrice = sysPrice.GetLiuWorkPrice((int)area, mno, Data.EndTypeDataAry.First().Endtype, Data.EndTypeDataAry.First().Fac);
                //}
                //else
                //{
                //    WorkPrice = sysPrice.GetWorkPrice(Data, (int)area, mno);
                //}
                WorkPrice = sysPrice.GetWorkPrice(Data, (int)area, mno);
            }
            else
            {
                WorkPrice = 0;
            }
            //更新田間管路的工作費
            return sysdb.ModifyPigingConfWorkPrice(WorkPrice, mno);
        }

        public DBCommon.DbEvent DelFarmDataSingle(Guid fno)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    DryDB.Farm.Remove(DryDB.Farm.Find(fno));
                    DryDB.SaveChanges();
                    dbstatus.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    dbstatus.DbMessage = "Removed Farm via MNo Failed : " + e.Message + "<br>";
                }
                if(dbstatus.DbMessage=="Success")
                {
                    scope.Complete();
                }
                return dbstatus;
            }
        }

        #region Json Data Struct
        /// <summary>
        /// Json Data Struct
        /// </summary>        
        public class JsData
        {
            /// <summary>
            /// 農地代碼
            /// </summary>
            public Guid FNo { get; set; }
            public int MapNo { get; set; }
            public string LandNo { get; set; }

            public string CTName { get; set; }
            public string CTCode { get; set; }
            public string twName { get; set; }
            public int twCode { get; set; }
            public string SectName { get; set; }
            public short SectCode { get; set; }
            public string FullName { get; set; }

            public string TypeName { get; set; }
            public byte TypeCode { get; set; }
            public double Area { get; set; }
            public double BArea { get; set; }
            public string Lat { get; set; }
            public string Long { get; set; }
            public bool OutSide { get; set; }
            public string OutSideStr { get; set; }
            public bool IsApplied { get; set; }
            public string IsAppliedStr { get; set; }
            public string ApplicationStatus { get; set; }

            public int Pcent_Chd { get; set; }
            public int Pcent_Par { get; set; }

            public int CorpCount { get; set; }
            public int HolderCount { get; set; }
            public List<CorpData> Corp { get; set; }
            public List<HolderData> Holder { get; set; }
            public List<CorpData> Cpp { get; set; }
        }

        /////////////////////////
        #region Crop Data Struct
        public class CorpData
        {
            public string corpcode { get; set; }
            public string corpname { get; set; }
        }
        #endregion

        #region Holder Data Struct
        public class HolderData
        {
            public string name { get; set; }
            public string id { get; set; }
            public string addr { get; set; }
            public int perP { get; set; }
            public int perC { get; set; }
            public double area { get; set; }
        }
        #endregion

        #endregion

        /// <summary>
        /// 取得地段地號座標(old)
        /// </summary>
        /// <param name="SecionID">地段代碼</param>
        /// <returns></returns>
        public string[] GetLandNumberLatLng(int SecionID, string LandCode)
        {
            //string[] latLng = new string[2];
            AERC.Models.CommonCls.GetLandNumber getLandNumber = new AERC.Models.CommonCls.GetLandNumber();
            List<Land_Number> landNumber = getLandNumber.GetLandNumberData(SecionID, LandCode);
            string[] latLng = { "0", "0" };
            if (landNumber.Count > 0)
            {
                latLng[0] = landNumber[0].Longitude.ToString();
                latLng[1] = landNumber[0].Latitude.ToString();
            }
            
            return latLng;
        }
        private string SpileLandNo(string Landno)
        {
            string[] Spiled = Landno.Split('-');

            
            if (Spiled.Length > 1)
            {
                return Spiled[0].PadLeft(4, '0') + "-" + Spiled[1].PadLeft(4, '0');
            }
            else
            {
                return Spiled[0].PadLeft(4, '0') + "-0000";
            }
        }

        /// <summary>
        /// 刪除土地持分資料By FNo
        /// </summary>
        /// <param name="FNo">土地持分資料KEY</param>
        /// <returns></returns>
        public DBCommon.DbEvent DelEquityByFNo(Guid fno)
        {
            try
            {
                var LandList = DryDB.LandEquity.Where(x => x.FNo == fno).ToList();
                if (LandList.Count > 0)
                {
                    foreach (var data in LandList)
                    {
                        DryDB.LandEquity.Remove(data);
                        DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                        DryDB.SaveChanges();
                    }
                }

                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed LandEquity By FNo Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        /// <summary>
        /// 複製土地資料By mapno
        /// </summary>
        /// <param name="oldmapno">前一版mapno</param>
        /// <param name="newmapno">目前mapno</param>
        /// <returns></returns>
        public DBCommon.DbEvent cloneData(int oldmapno, int newmapno)
        {
            

            int reccount = (from farm in DryDB.Farm
                           where farm.MapNo == newmapno 
                                        select farm).ToList().Count;
            if (reccount > 0) {
                dbstatus.DbMessage = "Success";
                return dbstatus;
            } 
            
            
            List<Farm> farmdata = (from farm in DryDB.Farm
                                  where farm.MapNo == oldmapno
                                  select farm).ToList();
            /*
            foreach (Farm farm in farmdata){
                var newfarm = new Farm();

                newfarm.FNo = Guid.NewGuid();
                newfarm.MapNo = newmapno;
                newfarm.Section = farm.Section;
                newfarm.LandNo = farm.LandNo;
                newfarm.LandType = farm.LandType;
                newfarm.Long = farm.Long;
                newfarm.Lat = farm.Lat;
                newfarm.FarmArea = farm.FarmArea;
                newfarm.BuildArea = farm.BuildArea;
                newfarm.FinalArea = farm.FinalArea;
                newfarm.IsApplied = farm.IsApplied;
                newfarm.ApplicationStatus = farm.ApplicationStatus;
                newfarm.AppliedArea = farm.AppliedArea;
                newfarm.Outside = farm.Outside;
                newfarm.Pcent_Chd = farm.Pcent_Chd;
                newfarm.Pcent_Par = farm.Pcent_Par;
                string msg = CreateFarm(newfarm).DbMessage;
                
            }
             */
             string msg = InsertFarm(farmdata, newmapno).DbMessage;
            dbstatus.DbMessage=msg;
            return dbstatus;


        }

        //#region 土地資料服務
        //public MapLandData getLandData(string fno)
        //{
        //    Guid gfno = Guid.Parse(fno);
        //    var objs = (from f in DryDB.Farm
        //              join l in DryDB.LandData on f.Section equals l.Section_Id
        //              where f.FNo == gfno
        //              select new { l.LandCode, f.LandNo }).FirstOrDefault();
        //    MapLandData o = new MapLandData();
        //    if (objs != null)
        //    {
        //        o.LandCode = objs.LandCode;
        //        o.LandNo = toLand_no(objs.LandNo);    
        //    }
        //    return o;
        //}

        //private string toLand_no(string landno)
        //{

        //    if (landno.Contains("-"))
        //    {
        //        string[] o = landno.Split('-');
        //        string o1 = o[0];
        //        string o2 = o[1];
        //        int l1 = int.Parse(o1);
        //        int l2 = int.Parse(o2);
        //        if (l2 > 0)
        //        {
        //            return l1.ToString() + "-" + l2.ToString();
        //        }
        //        else
        //        {
        //            return l1.ToString();
        //        }
                
                
        //    }
        //    else
        //    {
        //        return landno;
        //    }
        //}

        //public class MapLandData
        //{
        //    public string LandCode { get; set; }
        //    public string LandNo { get; set; }
        //}
            

        //#endregion
    }
}
