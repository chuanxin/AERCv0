/*
-- =============================================
-- Author: WEI, HaoHsuan
-- Create date: 2014-6-18
-- Description: 農戶資料的資料庫操作類別
-- =============================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dry.Models.ViewModel;
using Dry.Models.CommonCls;
using System.Transactions;
using AERC.Models;
using System.Linq.Dynamic;

namespace Dry.Models.Service
{
    public class FarmerDBService
    {
        private DryEntities DryDB = new DryEntities();
        private CommonEntities CommDB = new CommonEntities();
        private DBCommon.DbEvent dbState = new DBCommon.DbEvent();
        private GetData getData = new GetData();

        /// <summary>
        /// 寫入農戶資料
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public DBCommon.DbEvent SaveData(FarmerView Data)
        {
            GetData getData = new GetData();
            Data.ApplyUnit = getData.GetUnitId(Data.CId);
            Farmer FarmerData = getData.GetFarmer(Data.FId);
            List<Case> CaseData = getData.GetCase(Data.FId);
            Data.FId = getData.GetFIdByEvntNo(Data.EvntNo);
            using (TransactionScope scope = new TransactionScope())
            {
                
                dbState = SaveFarmer(Data, FarmerData);
                if (dbState.DbMessage == "Success")
                {
                    
                    dbState = SaveCase(Data, CaseData);
                    if (dbState.DbMessage == "Success")
                    {
                        
                        dbState = SaveVerMapping((int)dbState.KeyValue, Data.CId);
                        if (dbState.DbMessage == "Success")
                        {
                            
                            dbState = SaveDesigner(Data.DesingerId, (int)dbState.KeyValue, Data.ApplyUnit);
                            if (dbState.DbMessage.Equals("Success"))
                            {
                                scope.Complete();
                            }
                        }
                    }
                }
            }
            return dbState;
        }
        /// <summary>
        /// 修改農戶資料
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public DBCommon.DbEvent ModifyData(FarmerView Data,int MapNo)
        {
            Farmer farmer = DryDB.Farmer.Find(DryDB.VerMapping.Find(MapNo).Case.FId);
            farmer.IdNo = Data.FarmerIdNo;
            farmer.Name = Data.FarmerName;
            farmer.CityCode = Data.FarmerCityCode;
            farmer.Addr = Data.FarmerAddr;
            farmer.Phone = Data.FarmerPhone;
            farmer.Tel = Data.FarmerTel;
            farmer.IsMember = Data.IsMember;
            farmer.UDate = DateTime.Now.Date;
            farmer.CId = Data.CId;
            using (TransactionScope scope = new TransactionScope())
            {
                dbState = ModifyFarmer(farmer);
                if (dbState.DbMessage.Equals("Success"))
                {
                    Case caseData = DryDB.Case.Find(DryDB.VerMapping.Find(MapNo).EventNo);
                    caseData.ApplyYear = Data.ApplyYear;
                    caseData.IANum = Data.IANum;
                    caseData.Gold = Data.Gold;
                    caseData.Is12 = Data.Is12;
                    caseData.UDate = DateTime.Now.Date;
                    dbState = ModifyCase(caseData);
                    
                    if (dbState.DbMessage.Equals("Success"))
                    {
                        dbState = SaveDesigner(Data.DesingerId, MapNo, Data.ApplyUnit);
                    }
                    if (dbState.DbMessage.Equals("Success"))
                    {
                        scope.Complete();
                    }
                }
            }
            //return ModifyFarmer(farmer);
            return dbState;
        }

        /// <summary>
        /// 變更設計 (更新Farmer及Case資料表，並創建新的版本編號)
        /// </summary>
        /// <returns></returns>
        public DBCommon.DbEvent chgDBEvent(FarmerView Data)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Farmer famer = getData.GetFarmer(Data.FId);
                famer.IdNo = Data.FarmerIdNo;
                famer.Name = Data.FarmerName;
                famer.CityCode = Data.FarmerCityCode;
                famer.Addr = Data.FarmerAddr;
                famer.Phone = Data.FarmerPhone;
                famer.Tel = Data.FarmerTel;
                famer.IsMember = Data.IsMember;
                famer.CId = Data.CId;
                famer.UDate = DateTime.Now;

                dbState = ModifyFarmer(famer);
                if (dbState.DbMessage.Equals("Success"))
                {
                    Case cse = DryDB.Case.Single(c => c.EventNo == Data.EvntNo);
                    cse.Gold = Data.Gold;
                    cse.UDate = DateTime.Now;
                    cse.Step = 1;
                    cse.IANum = Data.IANum;
                    cse.Complete = false;
                    cse.Enable = true;
                    dbState = ModifyCase(cse);
                    if (dbState.DbMessage.Equals("Success"))
                    {
                        dbState = CreateNewVerMapping(Data.EvntNo, Data.CId);
                        if (dbState.DbMessage.Equals("Success"))
                        {
                            dbState = SaveDesigner(Data.DesingerId, (int)dbState.KeyValue, cse.ApplyUnit);// getData.GetUnitId(Data.CId)
                            if (dbState.DbMessage.Equals("Success"))
                            {
                                scope.Complete();
                            }
                        }
                    }
                }
            }
            return dbState;
        }

        #region 更新Farmer資料表
        /// <summary>
        /// 更新Farmer資料表
        /// </summary>
        /// <returns></returns>
        private DBCommon.DbEvent ModifyFarmer(Farmer data)
        {
            try
            {
                DryDB.Farmer.Attach(data);
                DryDB.Entry(data).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbState.DbMessage = "Success";
                dbState.KeyValue = data.IdNo;
            }
            catch (Exception e)
            {
                dbState.DbMessage = "Modified Farmer Failed : " + e.Message;
            }
            return dbState;
        }
        #endregion

        #region 更新Case資料表
        /// <summary>
        /// 更新Case資料表
        /// </summary>
        /// <returns></returns>
        private DBCommon.DbEvent ModifyCase(Case data)
        {
            try
            {
                DryDB.Case.Attach(data);
                DryDB.Entry(data).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbState.DbMessage = "Success";
                dbState.KeyValue = data.EventNo;
            }
            catch (Exception e)
            {
                dbState.DbMessage = "Modified Farmer Failed : " + e.Message;
            }
            return dbState;
        }
        #endregion

        #region 建立新版本資料
        private DBCommon.DbEvent CreateNewVerMapping(int EventNo, Guid CId)
        {
            VerMapping verMap = new VerMapping();
            try
            {
                short vertimes = (short)DryDB.VerMapping.Where(v => v.EventNo == EventNo).ToList().Count;
                vertimes++;

                verMap.EventNo = EventNo;
                verMap.ChgVer = vertimes;
                verMap.ChgReason = "Change Design";
                verMap.ChgDate = DateTime.Now;
                verMap.ChgId = CId;

                DryDB.VerMapping.Add(verMap);
                DryDB.SaveChanges();
                dbState.DbMessage = "Success";
                dbState.KeyValue = verMap.MapNo;
            }
            catch (Exception e)
            {
                dbState.DbMessage = "Create VerMapping Failed : " + e.Message;
            }
            return dbState;
        }
        #endregion

        #region 寫入Farmer資料表
        /// <summary>
        /// 寫入Farmer資料表
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="farmerData">Farmer資料表是否有寫入過資料。</param>
        /// <returns></returns>
        private DBCommon.DbEvent SaveFarmer(FarmerView Data, Farmer farmerData)
        {
            Farmer farmer = new Farmer();
            
            farmer.IdNo = Data.FarmerIdNo.ToUpper();
            farmer.Name = Data.FarmerName;
            farmer.CityCode = Data.FarmerCityCode;
            farmer.Addr = Data.FarmerAddr;
            farmer.Phone = Data.FarmerPhone;
            farmer.Tel = Data.FarmerTel;
            farmer.IsMember = Data.IsMember;
            farmer.CId = Data.CId;

            if (farmerData == null)
            {
                farmer.FId = Data.FId = Guid.NewGuid();
                farmer.CDate = DateTime.Now;
                farmer.UDate = null;
                DryDB.Farmer.Add(farmer);
            }
            else
            {
                //farmerData.FId = Data.FId;
                farmerData.IdNo = Data.FarmerIdNo.ToUpper();
                farmerData.Name = Data.FarmerName;
                farmerData.CityCode = Data.FarmerCityCode;
                farmerData.Addr = Data.FarmerAddr;
                farmerData.Phone = Data.FarmerPhone;
                farmerData.Tel = Data.FarmerTel;
                farmerData.IsMember = Data.IsMember;
                farmerData.CId = Data.CId;
                farmerData.UDate = DateTime.Now;
                //DryDB = new DryEntities();
                DryDB.Farmer.Attach(farmerData);
                DryDB.Entry(farmerData).State = System.Data.EntityState.Modified;
            }

            try
            {
                DryDB.SaveChanges();
                dbState.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbState.DbMessage = "Modified Farmer Failed : " + e.Message;
            }
            return dbState;
        }
        #endregion

        #region 寫入Case
        /// <summary>
        /// 寫入Case資料表
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="caseData"></param>
        /// <returns></returns>
        private DBCommon.DbEvent SaveCase(FarmerView Data, List<Case> caseData)
        {
            Case icase = new Case();
            icase.FId = Data.FId;
            icase.ApplyYear = Data.ApplyYear;
            icase.ApplyUnit = Data.ApplyUnit;
            icase.Gold = Data.Gold;
            icase.Is12 = Data.Is12;
            icase.Step = 1;
            icase.CId = Data.CId;
            icase.IANum = Data.IANum;
            icase.Complete = false;
            
            icase.Enable = true; 

            if (caseData.Count <= 0)
            {
                icase.CDate = DateTime.Now;
                icase.UDate = null;
                DryDB.Case.Add(icase);
            }
            else
            {
                Case cases = caseData[0];
                //cases.FId = Data.FId;
                cases.ApplyYear = Data.ApplyYear;
                cases.ApplyUnit = Data.ApplyUnit;
                cases.Gold = Data.Gold;
                cases.Is12 = Data.Is12;
                cases.Step = 1;
                cases.CId = Data.CId;
                cases.UDate = DateTime.Now;
                cases.IANum = Data.IANum;
                cases.Complete = false;
                cases.Enable = true;
                
                DryDB = new DryEntities();
                DryDB.Case.Attach(cases);
                DryDB.Entry(cases).State = System.Data.EntityState.Modified;
                dbState.KeyValue = cases.EventNo;
            }
            try
            {
                DryDB.SaveChanges();
                dbState.DbMessage = "Success";
                if (caseData.Count <= 0)
                    dbState.KeyValue = icase.EventNo;
            }
            catch (Exception e)
            {
                dbState.DbMessage = "Modified Case Failed : " + e.Message;
            }
            return dbState;
        }
        #endregion

        #region 寫入版本資料
        /// <summary>
        /// 寫入版本資料
        /// </summary>
        /// <param name="EventNo"></param>
        /// <param name="CId"></param>
        /// <returns></returns>
        private DBCommon.DbEvent SaveVerMapping(int EventNo, Guid CId)
        {
            VerMapping verMap = GetVerMapping(EventNo);
            if (verMap == null)
            {
                verMap = new VerMapping();
                verMap.EventNo = EventNo;
                verMap.ChgVer = 1;
                verMap.ChgDate = DateTime.Now;
                verMap.ChgId = CId;
                verMap.ChgReason = "Create Case";
                DryDB.VerMapping.Add(verMap);
                try
                {
                    DryDB.SaveChanges();
                    dbState.DbMessage = "Success";

                }
                catch (Exception e)
                {
                    dbState.DbMessage = "Modified VerMapping Failed : " + e.Message;
                }
            }
            dbState.KeyValue = verMap.MapNo;
            return dbState;
        }
        #endregion

        #region 寫入設計者資料表
        /// <summary>
        /// 寫入設計者資料表
        /// </summary>        
        /// <returns></returns>
        private DBCommon.DbEvent SaveDesigner(Guid DesignId, int MapNo, short Unit)
        {
            FacDesign Des = new FacDesign();
            if (DryDB.FacDesign.Any(D => D.MapNo == MapNo))
            {
                Des = DryDB.FacDesign.Single(D => D.MapNo == MapNo);
                Des.DesId = DesignId;
                DryDB.FacDesign.Attach(Des);
                DryDB.Entry(Des).State = System.Data.EntityState.Modified;
            }
            else
            {
                Des.DesId = DesignId;
                Des.MapNo = MapNo;
                Des.Unit = Unit;
                DryDB.FacDesign.Add(Des);
            }
            try
            {
                DryDB.SaveChanges();
                dbState.DbMessage = "Success";
                dbState.KeyValue = Des.MapNo;
            }
            catch (Exception e)
            {
                dbState.DbMessage = "Designer DB Event Failed : " + e.Message;
            }
            return dbState;
        }
        #endregion

         #region 取得設計者id
        /// <summary>
        /// 取得設計者id
        /// </summary>        
        /// <returns>設計者id</returns>
        public Guid GetDesigner(int MapNo)
        {
            return getData.GetDesignID(MapNo);
        }
        #endregion


        /// <summary>
        /// 取得申請案件資料
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public List<Case> GetCaseDataO(byte ApplyYear, bool search, string query, string sortIdx,string sord, byte unit)
        {
            List<Case> data = new List<Case>();
            //var list = new List<Case>();
            int iaNum = -1;
            try
            {
                iaNum = Convert.ToInt32(query);
            }
            catch
            {

            }
            if (ApplyYear > 0)
            {
                if (unit > 0 && unit < 99) 
                {
                    if (search)
                    {
                        if (iaNum != -1)
                        {
                            data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.ApplyYear == ApplyYear && c.IANum == iaNum)./*OrderBy(sortIdx + " " + sord).*/ToList();
                            if (sord.Equals("desc"))
                                data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.ApplyYear == ApplyYear && c.IANum == iaNum && c.Enable == true).OrderByDescending(n => n.IANum).ToList();
                            else
                                data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.ApplyYear == ApplyYear && c.IANum == iaNum && c.Enable == true).OrderBy(n => n.IANum).ToList();
                        }
                        else
                        {
                            //var temData = (from cases in DryDB.Case
                            //               join farmer in DryDB.Farmer on cases.FId equals farmer.FId
                            //               where cases.ApplyUnit == unit && cases.ApplyYear == ApplyYear && farmer.Name.Contains(query)
                            //               select cases).OrderBy(sortIdx).ToList();
                            var temData = (from cases in DryDB.Case
                                           join farmer in DryDB.Farmer on cases.FId equals farmer.FId
                                           where cases.ApplyUnit == unit && cases.ApplyYear == ApplyYear && farmer.Name.Contains(query)
                                           select cases).ToList();
                            foreach (var item in temData)
                            {
                                data.Add(new Case
                                {
                                    EventNo = item.EventNo,
                                    FId = item.FId,
                                    ApplyYear = item.ApplyYear,
                                    ApplyUnit = item.ApplyUnit,
                                    Farmer = item.Farmer,
                                    IANum = item.IANum,
                                    Gold = item.Gold,
                                    Step = item.Step,
                                    Complete = item.Complete,
                                    CId = item.CId,
                                    CDate = item.CDate,
                                    UDate = item.UDate
                                });
                            }
                            //List<Farmer> farmerData = DryDB.Farmer.Where(p => p.Name.Contains(query)).ToList();
                            //data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.ApplyYear == ApplyYear).Join(farmerData).OrderBy(sortIdx).ToList();

                            //data = (from cases in DryDB.Case 
                            //        join farmer in DryDB.Farmer on cases.FId equals farmer.FId
                            //        where cases.ApplyUnit == unit && cases.ApplyYear == ApplyYear
                            //        select cases).ToList();
                        }
                    }
                    else
                    {
                        //data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.ApplyYear == ApplyYear && c.Enable == true)/*.OrderBy(sortIdx + " " + sord)*/.ToList();
                        if (sord.Equals("desc"))
                            data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.ApplyYear == ApplyYear && c.Enable == true).OrderByDescending(n => n.IANum).ToList();
                        else
                            data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.ApplyYear == ApplyYear && c.Enable == true).OrderBy(n => n.IANum).ToList();

                    }
                }
                else 
                {
                    if (search)
                    {
                        if (iaNum != -1)
                        {
                            if (sord.Equals("desc"))
                                data = DryDB.Case.Where(c => c.ApplyYear == ApplyYear && c.IANum == iaNum && c.Enable == true).OrderByDescending(n => n.IANum).ToList();
                            else
                                data = DryDB.Case.Where(c => c.ApplyYear == ApplyYear && c.IANum == iaNum && c.Enable == true).OrderBy(n => n.IANum).ToList();
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (sord.Equals("desc"))
                            data = DryDB.Case.Where(c => c.ApplyYear == ApplyYear && c.Enable == true).OrderByDescending(n => n.IANum).ToList();
                        else
                            data = DryDB.Case.Where(c => c.ApplyYear == ApplyYear && c.Enable == true).OrderBy(n => n.IANum).ToList();
                    }
                }
                    
            }
            else
            {
                if (unit > 0 && unit < 99)
                {
                    if(search)
                    {
                        if(iaNum!=-1)
                        {
                            if (sord.Equals("desc"))
                                data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.IANum == iaNum && c.Enable == true).OrderByDescending(n => n.IANum).ToList();
                            else
                                data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.IANum == iaNum && c.Enable == true).OrderBy(n => n.IANum).ToList();
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (sord.Equals("desc"))
                            data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.Enable == true).OrderByDescending(n => n.IANum).ToList();
                        else
                            data = DryDB.Case.Where(c => c.ApplyUnit == unit && c.Enable == true).OrderBy(n => n.IANum).ToList();
                    }
                    
                }
                else
                {
                    if(search)
                    {
                        if (iaNum != -1)
                        {
                            if (sord.Equals("desc"))
                                data = DryDB.Case.Where(c => c.IANum == iaNum && c.Enable == true).OrderByDescending(n => n.IANum).ToList();
                            else
                                data = DryDB.Case.Where(c => c.IANum == iaNum && c.Enable == true).OrderBy(n => n.IANum).ToList();
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (sord.Equals("desc"))
                            data = DryDB.Case.OrderByDescending(n => n.IANum).ToList();
                        else
                            data = DryDB.Case.OrderBy(n => n.IANum).ToList();
                    }
                    
                }
            }
            return data;
        }

        public List<Case> GetCaseData(byte ApplyYear, bool search, string query, string sortIdx, string sord, byte unit)
        {
            List<Case> data = new List<Case>();
            //var list = new List<Case>();
            int iaNum = -1;
            try
            {
                iaNum = Convert.ToInt32(query);
            }
            catch
            {

            }
            switch (unit)
            {
                case 0:
                case 99:
                    
                    if (search)
                    {
                        if (iaNum != -1)
                        {
                            data = DryDB.Case.Where(c => c.IANum == iaNum).ToList();
                        }
                        else
                        {
                            data = (from o in DryDB.Case
                                    join s in DryDB.SummaryView on o.EventNo equals s.EventNo
                                    where s.Name.Contains(query)
                                    select o).ToList();
                        }
                        
                        if (ApplyYear > 0)
                        {
                            data = data.Where(o => o.ApplyYear == ApplyYear && o.Enable == true).ToList();
                        }

                    }
                    break;
                default:
                    if (search)
                    {
                        if (iaNum != -1)
                        {
                            data = DryDB.Case.Where(c => c.IANum == iaNum && c.ApplyUnit == unit).ToList();
                        }
                        else
                        {
                            data = (from o in DryDB.Case
                                    join s in DryDB.SummaryView on o.EventNo equals s.EventNo
                                    where s.Name.Contains(query) && o.ApplyUnit == unit
                                    select o).ToList();
                        }
                        
                        if (ApplyYear > 0)
                        {
                            data = data.Where(o => o.ApplyYear == ApplyYear && o.Enable == true).ToList();
                        }

                    }
                    else
                    {
                        data = DryDB.Case.Where(o => o.ApplyYear == ApplyYear && o.ApplyUnit == unit && o.Enable == true).ToList();
                    }
                    break;
            }
            #region Oldcode
            /*if (search)
            {
                if (iaNum != -1)
                {
                    data = DryDB.Case.Where(c => c.IANum == iaNum).ToList();
                }
                else
                {
                    data = (from o in DryDB.Case
                            join s in DryDB.SummaryView on o.EventNo equals s.EventNo
                            where s.Name.Contains(query)
                            select o).ToList();
                }
            }
            else
            {
                data = DryDB.Case.ToList();
            }
            
            switch (unit)
            {
                case 0:
                case 99:
                    break;
                default:
                    data = data.Where(o => o.ApplyUnit == unit).ToList();
                    break;
            }
            
            switch (unit)
            {
                case 0:
                case 99:
                    if (search)
                    {
                        if (ApplyYear > 0)
                        {
                            data = data.Where(o => o.ApplyYear == ApplyYear).ToList();
                        }

                    }
                    else
                    {
                        data = data.Where(o => o.ApplyYear == 0).ToList();
                    }
                    //data = data.Where(o => o.ApplyYear == ApplyYear).ToList();    
                    break;
                default:
                    if (ApplyYear > 0)
                    {
                        data = data.Where(o => o.ApplyYear == ApplyYear).ToList();
                    }
                    else//歷年
                    {
                        if (!search)
                        {
                            data = data.Where(o => o.ApplyYear == ApplyYear).ToList();
                        }
                    }
                    break;
            }*/
            #endregion

            
            if (sord.Equals("desc"))
            {
                data = data.OrderByDescending(o => o.ApplyYear).ThenByDescending(n => n.IANum).ToList();
            }
            else
            {
                data = data.OrderBy(o => o.ApplyYear).ThenBy(n => n.IANum).ToList();
            }
            return data;

        }

        public FarmerView GetFarmer(int EvntNo, int Mapno)
        {
            FarmerView farmerView = new FarmerView();
            GetData getData = new GetData();
            Case cases = DryDB.Case.Find(EvntNo);
            Guid FId = cases.FId;
            Farmer farmer = getData.GetFarmer(FId);
            farmerView.FId = farmer.FId;
            farmerView.FarmerIdNo = farmer.IdNo;
            farmerView.FarmerName = farmer.Name;
            farmerView.FarmerCityCode = farmer.CityCode;
            farmerView.FarmerAddr = farmer.Addr;
            farmerView.FarmerPhone = farmer.Phone;
            farmerView.FarmerTel = farmer.Tel;
            farmerView.IsMember = farmer.IsMember;
            farmerView.CId = farmer.CId;
            farmerView.Gold = cases.Gold;
            farmerView.DesingerId = getData.GetDesignID(Mapno);
            farmerView.Is12 = cases.Is12;
            return farmerView;
        }
        /// <summary>
        /// 取得版本資料
        /// </summary>
        /// <param name="EvntNo">案件號碼</param>
        /// <returns></returns>
        /// Hao Hsuan Re-write 103/05/22
        private VerMapping GetVerMapping(int EvntNo)
        {
            var v = getData.GetVerMapData(EvntNo);
            return v;
        }

        #region old code
        /* 
        private VerMapping GetVerMapping(int EvntNo)
        {
            return DryDB.VerMapping.Find(EvntNo);
        }*/
        #endregion
    }
}
