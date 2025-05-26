using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Dry.Models.Service
{
    public class FacilityReportDBService
    {
        private DryEntities DryDB = new DryEntities();
        private GetData getDataCls = new GetData();
        private CommClass getComm = new CommClass();
        private DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();

        #region get Create Data
        public FacilityReportView GetModelData(int _MNo)
        {
            #region Get Data
            Case CaseDta = getDataCls.GetCaseDataFromMapNo(_MNo);
            Farmer farmer = getDataCls.GetFarmerData(_MNo);
            List<GetData.NewFarm> Farm = getDataCls.GetFarmData(_MNo);
            Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct FSys = getDataCls.GetFarmerSystemData(_MNo);
            List<Pool> FarmerPool = getDataCls.GetPoolData(_MNo);
            MappingClass mapingCls = new MappingClass();
            #endregion

            FacilityReportView ModelData = new FacilityReportView();
            #region Assign basic Data
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
            ModelData.EDate = DateTime.Now.Date;
            ModelData.ResultList = getDataCls.GetRadioList();
            ModelData.Complete = CaseDta.Complete;
            ModelData.IsModify = CaseDta.Step < 9 ? false : true;
            //ModelData.Result = true;
            #endregion
            FacInspection data = DryDB.FacInspection.Find(_MNo);
            if(data!=null)
            {
                ModelData.EDate = data.E_Date;
                ModelData.Result = data.Result.ToString();
                ModelData.ResultList = getDataCls.GetRadioList(data.Result);
            }
            return ModelData;
        }
        #endregion

        #region get Edit Data
        public FacilityReportView GetEditModelData(int _MNo)
        {
            #region Get Edit Data
            Case CaseDta = getDataCls.GetCaseDataFromMapNo(_MNo);
            Farmer farmer = getDataCls.GetFarmerData(_MNo);
            List<GetData.NewFarm> Farm = getDataCls.GetFarmData(_MNo);
            Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct FSys = getDataCls.GetFarmerSystemData(_MNo);
            List<Pool> FarmerPool = getDataCls.GetPoolData(_MNo);
            MappingClass mapingCls = new MappingClass();
            #endregion

            FacInspection data = DryDB.FacInspection.Single(fi => fi.MapNo == _MNo);
            FacilityReportView ModelData = new FacilityReportView();
            #region Assign basic Data
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
            ModelData.EDate = data.E_Date;
            ModelData.Result = data.Result.ToString();
            ModelData.ResultList = getDataCls.GetRadioList(data.Result);
            #endregion
            return ModelData;
        }
        #endregion

        #region DB Event of FacilityReport

        #region Insert FacilityReport to DB
        public DBCommon.DbEvent InsertFacilityReport(int _MNo, JsData data)
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    DateTime edate = Convert.ToDateTime(data.EDate).Date;
                    bool res = (data.Res.Equals("true")) ? true : false;

                    FacInspection FacRdata = DryDB.FacInspection.Find(_MNo);
                    if (FacRdata != null)
                    {
                        FacRdata.E_Date = edate;
                        FacRdata.Result = res;
                        DryDB.FacInspection.Attach(FacRdata);
                        DryDB.Entry(FacRdata).State = System.Data.EntityState.Modified;
                    }
                    else
                    {
                        FacRdata = new FacInspection();
                        FacRdata.E_Date = edate;
                        FacRdata.Result = res;
                        FacRdata.MapNo = _MNo;
                        DryDB.FacInspection.Add(FacRdata);
                    }
                    

                   
                    DryDB.SaveChanges();
                    dbstatus.KeyValue = FacRdata.MapNo;
                    dbstatus.DbMessage = "Success";
                    if (dbstatus.DbMessage.Equals("Success"))
                    {
                        dbstatus = getComm.UpdateCase(_MNo, 9);
                        if (dbstatus.DbMessage.Equals("Success"))
                        {
                            scope.Complete();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Insert FacilityReport Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion
        
        #region Modify FacilityReport Data to DB
        public DBCommon.DbEvent ModifyFacilityReport(int _MNo, JsData data)
        {
            try
            {            
                DateTime edate = Convert.ToDateTime(data.EDate).Date;
                bool res = (data.Res.Equals("false")) ? false : true;

                FacInspection FacRdata = DryDB.FacInspection.Single(fi => fi.MapNo == _MNo);
                FacRdata.E_Date = edate;
                FacRdata.Result = res;
                DryDB.FacInspection.Attach(FacRdata);
                DryDB.Entry(FacRdata).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbstatus.KeyValue = FacRdata.MapNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modify FacilityReport Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #endregion

        #region Json Data Struct
        public class JsData
        {
            public string EDate { get; set; }
            public string Res { get; set; }
        }
        #endregion
    }
}
