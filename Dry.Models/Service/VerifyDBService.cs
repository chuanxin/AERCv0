using Dry.Models.CommonCls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dry.Models.ViewModel;

namespace Dry.Models.Service
{
    public class VerifyDBService
    {
        private DryEntities DryDB = new DryEntities();
        private CommClass getComm = new CommClass();
        private DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
        private GetData getDataCls = new GetData();

        #region get Create Model Data
        public Dry.Models.ViewModel.VerifyView GetModelData(int _MNo,string Account)
        {

            #region Get Needed Data
            Case CaseDta = getDataCls.GetCaseDataFromMapNo(_MNo);
            Farmer farmer = getDataCls.GetFarmerData(_MNo);
            List<GetData.NewFarm> Farm = getDataCls.GetFarmData(_MNo);
            Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct FSys = getDataCls.GetFarmerSystemData(_MNo);
            //Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct FSys = getDataCls.GetFarmerSystemData(_MNo);
            MappingClass mapingCls = new MappingClass();
            #endregion

            

            #region Assign Model Data
            Dry.Models.ViewModel.VerifyView ModelData = new Dry.Models.ViewModel.VerifyView();

            ModelData.Case = CaseDta.EventNo;
            ModelData.IANum = CaseDta.IANum;
            ModelData.ApplyY = CaseDta.ApplyYear;
            ModelData.Gold = CaseDta.Gold;
            ModelData.BuildArea = Farm.Sum(f => f.BuildArea);
            ModelData.Name = farmer.Name;
            ModelData.Address = farmer.Addr;
            ModelData.Farm = Farm;
            ModelData.EndType = (FSys == null) ? "未申請末端設施" : FSys.FacName;
            ModelData.ApplyUnit = CaseDta.ApplyUnit;
            ModelData.Complete = CaseDta.Complete;
            ModelData.IsModify = CaseDta.Step >= 11 ? true : false;

            #endregion

            ModelData.personDDL = getComm.GetExamineMen(Account);
            ModelData.statusDDL = getComm.GetStatusList();
            ModelData.VDate = DateTime.Now;
            ModelData.ReduceM = "0";

            ModelData.VID = new AERC.Models.CommonCls.GetAdminData().GetAdminByAccount(Account).Admin_Id;
            int eventno = DryDB.VerMapping.Where(o => o.MapNo == _MNo).FirstOrDefault().EventNo;
            if (DryDB.VerifyLog.Any(p => p.MapNo == _MNo))
            {
                FacVerify facVerify = DryDB.FacVerify.Find(DryDB.VerifyLog.Where(p => p.MapNo == _MNo).First().V_RecNo);
                ModelData.VDate = facVerify.VDate;
                ModelData.VID = facVerify.VId;
                ModelData.statCode = facVerify.StatCode;
                ModelData.PayM = facVerify.PayM.ToString();
                ModelData.ReduceM = facVerify.ReduceM.ToString();
                ModelData.Descript = facVerify.VDes;
                var MoneyData = GetMoney(_MNo);
                //ModelData.OrginalPayM = MoneyData.GovPay.PayFarmer.ToString();//"100000";
                ModelData.OrginalPayM = (facVerify.PayM + facVerify.ReduceM).ToString();
            }


            bool changeDesign = DryDB.VerMapping.Any(o => o.EventNo == eventno && o.ChgReason == "Change Design");
            //if(DryDB.VerifyLog.Any(p => p.MapNo == _MNo))
            if (!changeDesign)
            {

                var MoneyData = GetMoney(_MNo);
                ModelData.OrginalPayM = MoneyData.GovPay.PayFarmer.ToString();
                ModelData.PayM = MoneyData.GovPay.PayFarmer.ToString();
                ModelData.ReduceM = "0";

            }
            else
            {

                int omapno = getDataCls.GetOrginMapno(_MNo);
                var MoneyData = GetMoney(_MNo); 
                ModelData.PayM = MoneyData.GovPay.PayFarmer.ToString();//"100000";
                var MoneyData1 = GetMoney(omapno);
                ModelData.OrginalPayM = MoneyData1.GovPay.PayFarmer.ToString();
                ModelData.ReduceM = (MoneyData1.GovPay.PayFarmer - MoneyData.GovPay.PayFarmer).ToString();
                ModelData.statCode = 2;
            }
            //var MoneyData = GetMoney(_MNo);
            //ModelData.OrginalPayM = MoneyData.GovPay.PayFarmer.ToString();//"100000";
            return ModelData;
        }
        #endregion

        #region get Edit Model Data
        public Dry.Models.ViewModel.VerifyView GetEditModelData(int _MNo, Guid UserID)
        {
            #region Get Needed Data
            Case CaseDta = getDataCls.GetCaseDataFromMapNo(_MNo);
            Farmer farmer = getDataCls.GetFarmerData(_MNo);
            List<GetData.NewFarm> Farm = getDataCls.GetFarmData(_MNo);
            Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct FSys = getDataCls.GetFarmerSystemData(_MNo);
            MappingClass mapingCls = new MappingClass();
            #endregion

            #region Assign Model Data
            FacVerify data = GetFacVerifyData(_MNo);

            Dry.Models.ViewModel.VerifyView ModelData = new Dry.Models.ViewModel.VerifyView();

            #region Basic Data
            ModelData.Case = CaseDta.EventNo;
            ModelData.IANum = CaseDta.IANum;
            ModelData.ApplyY = CaseDta.ApplyYear;
            ModelData.Gold = CaseDta.Gold;
            ModelData.BuildArea = Farm.Sum(f => f.BuildArea);
            ModelData.Name = farmer.Name;
            ModelData.Address = farmer.Addr;
            ModelData.Farm = Farm;
            ModelData.EndType = (FSys == null) ? "未申請末端設施" : FSys.FacName;
            ModelData.ApplyUnit = CaseDta.ApplyUnit;
            #endregion

            ModelData.personDDL = getComm.GetExamineMenById(UserID, data.VId);
            ModelData.statusDDL = getComm.GetStatusList();//data.StatCode.ToString()
            ModelData.statCode = data.StatCode;
            var MoneyData = GetMoney(_MNo);
            ModelData.PayM = MoneyData.GovPay.PayFarmer.ToString();//"100000";  
            ModelData.VDate = data.VDate;
            ModelData.ReduceM = data.ReduceM.ToString();
            ModelData.Descript = data.VDes;
            ModelData.VID = data.VId;

            #endregion

            return ModelData;
        }
        #endregion

        #region Insert to VerLog
        public DBCommon.DbEvent InsertVerLog(int _MNo)
        {
            
            try
            {
                if (!DryDB.VerifyLog.Any(m => m.MapNo == _MNo))
                {
                    VerifyLog vlog = new VerifyLog();
                    vlog.MapNo = _MNo;
                    DryDB.VerifyLog.Add(vlog);
                    DryDB.SaveChanges();
                    dbstatus.KeyValue = vlog.V_RecNo;
                }
                else
                {
                    dbstatus.KeyValue = DryDB.VerifyLog.Where(m => m.MapNo == _MNo).FirstOrDefault().V_RecNo;
                }
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Insert VerLog Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Insert to Verify
        public DBCommon.DbEvent InsertVerify(int _MNo, FacVerify verData)
        {
            try
            {
                if (DryDB.FacVerify.Any(m => m.V_RecNo == verData.V_RecNo))
                {
                    DryDB.FacVerify.Attach(verData);
                    DryDB.Entry(verData).State = System.Data.EntityState.Modified;
                    DryDB.SaveChanges();
                    dbstatus.KeyValue = verData.V_RecNo;
                    dbstatus.DbMessage = "Success";
                }
                else
                {
                    DryDB.FacVerify.Add(verData);
                    DryDB.SaveChanges();
                    dbstatus.KeyValue = verData.V_RecNo;
                    dbstatus.DbMessage = "Success";
                }

            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Insert Verify Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Modify Verify Data
        public DBCommon.DbEvent ModifyVerify(int _MNo, FacVerify verData)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    DryDB.FacVerify.Attach(verData);
                    DryDB.Entry(verData).State = System.Data.EntityState.Modified;
                    DryDB.SaveChanges();
                    dbstatus.KeyValue = verData.V_RecNo;
                    dbstatus.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    dbstatus.DbMessage = "Modify Verify Failed : " + e.Message + "<br>";
                }
                if (dbstatus.DbMessage.Equals("Success"))
                {
                    scope.Complete();
                }
            }
            return dbstatus;
        }
        #endregion

        #region Modify Verify Data to DB
        public DBCommon.DbEvent ModifyToVerifyDB(int _MNo, JsData data)
        {
            FacVerify Ver = GetFacVerifyData(_MNo);
            Ver.StatCode = data.Status;
            //Ver.V_RecNo = GetFacVlogData(_MNo);
            Ver.VDate = Convert.ToDateTime(data.VDate).Date;
            Ver.VDes = data.Des;
            Ver.VId = data.ID;
            Ver.ReduceM = data.RM;
            Ver.PayM = data.PM;
            dbstatus = ModifyVerify(_MNo, Ver);            
            return dbstatus;
        }
        #endregion

        #region Insert Verify Data to DB
        public DBCommon.DbEvent InsertToVerifyDB(int _MNo, JsData data)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                dbstatus = InsertVerLog(_MNo);
                if (dbstatus.DbMessage.Equals("Success"))
                {
                    FacVerify Ver = new FacVerify();
                    Ver.StatCode = data.Status;
                    Ver.V_RecNo = (int)dbstatus.KeyValue;
                    Ver.VDate = Convert.ToDateTime(data.VDate).Date;
                    Ver.VDes = data.Des;
                    Ver.VId = data.ID;
                    Ver.ReduceM = data.RM;
                    Ver.PayM = data.PM;
                    dbstatus = InsertVerify(_MNo, Ver);
                    if (dbstatus.DbMessage.Equals("Success"))
                    {
                        dbstatus = getComm.UpdateCase(_MNo, 11, true);
                        if (dbstatus.DbMessage.Equals("Success"))
                        {
                            scope.Complete();
                        }
                    }
                }
            }
            return dbstatus;
        }
        #endregion

        #region Get Money
        public GetData.MoneyStruct GetMoney(int _MNo)
        {
            return getDataCls.GetAllApplyMoney(_MNo);
        }
        #endregion

        #region Get the Newest Verlog RecNo
        /// <summary>
        /// Get the Newest Verlog RecNo
        /// </summary>
        /// <param name="_MNo">版本編號</param>
        /// <returns>Vlog pk </returns>
        public int GetFacVlogData(int _MNo)
        {
            if (DryDB.VerifyLog.Any(vlg => vlg.MapNo == _MNo))
            {
                int RecNo = DryDB.VerifyLog.Where(vlg => vlg.MapNo == _MNo).Max(vlg => vlg.V_RecNo);
                return RecNo;
            }
            return -1;
        }
        #endregion

        #region get the Newest Case Verify Data
        /// <summary>
        /// get the Newest Case Verify Data
        /// </summary>
        /// <param name="_MNo">版本編號</param>
        /// <returns>驗收資料</returns>
        public FacVerify GetFacVerifyData(int _MNo)
        {
            if (DryDB.VerifyLog.Any(vlg => vlg.MapNo == _MNo))
            {
                int RecNo = DryDB.VerifyLog.Where(vlg => vlg.MapNo == _MNo).Max(vlg => vlg.V_RecNo);
                //FacVerify fvdata = DryDB.FacVerify.Single(fvy => fvy.V_RecNo == RecNo);
                return DryDB.FacVerify.Single(fvy => fvy.V_RecNo == RecNo);
            }
            return null;
        }
        #endregion
        /// <summary>
        /// 整理要匯出報表的資料(工程決算書) PS. 瑠公專用
        /// </summary>
        /// <param name="_MNo"></param>
        /// <returns></returns>
        public Dictionary<string, string> StatementData(int _MNo)
        {
            BudgetBookView Data = new BudgetBookDBService().GetLiuGongBudgetBookData(_MNo);
            Case CaseDta = getDataCls.GetCaseDataFromMapNo(_MNo);
            Farmer farmer = getDataCls.GetFarmerData(_MNo);
            List<GetData.NewFarm> farm = getDataCls.GetFarmData(_MNo);
            List<LandTypeList> landType = DryDB.LandTypeList.ToList();
            string SectionStr = ""; 
            string LandNoStr = ""; 
            string SectionTemp = "";
            double buildArea = 0;
            foreach(var item in farm)
            {
                
                if (item.full_sectName != SectionTemp)
                {
                    SectionStr += item.sectName + "、";
                }
                SectionTemp = item.full_sectName;

                
                LandNoStr += landType.Find(c=>c.LandType == item.LandType).LandTypeCNS + " " + item.LandNo + "、";
                buildArea += item.BuildArea;
            }
            
            SectionStr = SectionStr.Remove(SectionStr.LastIndexOf('、'));
            LandNoStr = LandNoStr.Remove(LandNoStr.LastIndexOf('、'));

            Dictionary<string, string> dataStr = new Dictionary<string, string>();
            dataStr.Add("ApplyYear", Data.ApplyY.ToString()); 
            dataStr.Add("IaNum", Data.IANum.ToString()); 
            dataStr.Add("FarmerName", farmer.Name); 
            dataStr.Add("FarmerAddr", farmer.Addr.Replace(" ","")); 
            dataStr.Add("Section", SectionStr); 
            dataStr.Add("LandNo", LandNoStr);
            dataStr.Add("BuildArea", ((float)buildArea / 10000).ToString()); 
            dataStr.Add("BuildType_1", "V"); 
            dataStr.Add("BuildType_2", "V"); 
            dataStr.Add("BuildType_3", "V"); 
            dataStr.Add("BuildType_4", "V"); 
            dataStr.Add("BuildType_5", "V"); 
            return dataStr;
        }

        #region Json Data Struct
        public class JsData
        {
            public byte Status { get; set; }
            public string VDate { get; set; }
            public string Des { get; set; }
            public Guid ID { get; set; }
            public int RM { get; set; }
            public int PM { get; set; }
        }
        #endregion
    }
}