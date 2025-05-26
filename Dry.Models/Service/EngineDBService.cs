using Dry.Models.CommonCls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-2-10
-- Description: 動力設施的資料庫操作類別
-- =============================================
*/
namespace Dry.Models.Service
{
    public class EngineDBService
    {
        private DryEntities DryDB = new DryEntities();
        private GetData gda = new GetData();
        private DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
        private CommClass comm = new CommClass();

        #region check the mno have any engine data
        /// <summary>
        /// Check the mno have any engine data
        /// </summary>
        /// <returns>bool</returns>
        public bool ChkEngData(int mno)
        {
            if (DryDB.Engine.Any(e => e.MapNo == mno))
                return true;
            else
                return false;
        }
        #endregion

        #region Obtain engine list from db.
        public List<EngineList> GetEngineList()
        {
            return DryDB.EngineList.ToList();
        }
        #endregion

        #region Obtain farmer engine data by ENo.
        public Engine GetEngineByENo(int eno)
        {
            return DryDB.Engine.Single(e => e.ENo == eno);
        }
        #endregion

        #region Obtain farmer engine apply case data by MapNo.
        public EngApply GetEngApplyDataByMapNo(int mno)
        {
            if (DryDB.EngApply.Any(ea => ea.MapNo == mno))
                return DryDB.EngApply.Single(ea => ea.MapNo == mno);
            else
                return null;
        }
        #endregion

        #region Create Engine Object
        public Engine CreateEngObj(int mno, byte engcode, string engregcode,int price)
        {
            Engine engdata = new Engine();
            Case cases = gda.GetCaseDataFromMapNo(mno);
            
            //engdata.ENo = db.GetNewENo();
            engdata.MapNo = mno;
            engdata.EngCode = engcode;
            engdata.EngRegCode = engregcode;
            engdata.EngPrice = price;
            return engdata;
        }
        #endregion

        #region DB Event of EngineApply
        public DBCommon.DbEvent CreateEngineApply(EngApply data)
        {
            try
            {
                DryDB.EngApply.Add(data);
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.MapNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Create EngineApply Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent UpdateEngineApply(EngApply data)
        {
            try
            {
                DryDB.EngApply.Attach(data);
                DryDB.Entry(data).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.MapNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modified EngineApply Failed  : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region DB Event of Engine
        public DBCommon.DbEvent CreateEngine(Engine data)
        {
            try
            {
                DryDB.Engine.Add(data);
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.ENo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Create Engine Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent UpdateEngine(Engine data)
        {
            try
            {
                DryDB.Engine.Attach(data);
                DryDB.Entry(data).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.ENo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modified Engine Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent DeleteEngine(Engine data)
        {
            try
            {
                dbstatus.KeyValue = data.ENo;
                DryDB.Engine.Remove(data);
                DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Engine Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent DelEngineByMNo(int mno)
        {
            try
            {
                var engList = DryDB.Engine.Where(x => x.MapNo == mno).ToList();
                foreach (var data in engList)
                {
                    DryDB.Engine.Remove(data);
                    DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                    DryDB.SaveChanges();
                }
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Engine By MNo Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region Insert Engine Data To DB
        public DBCommon.DbEvent InsertEngine(JsData ResultArry, int mno)
        {
            Calculate_Funding CalculateCls = new Calculate_Funding();
            PayDBService paydb = new PayDBService();

            string msg = "Success";
            using (TransactionScope scope = new TransactionScope())
            {
                EngApply engaply = new EngApply();
                engaply.MapNo = mno;
                engaply.ApplyUnit = ResultArry.ddl_Unit;
                engaply.FacMoney = ResultArry.FacPrice;
                if (ResultArry.EngAry == null)
                {
                    msg = DelEngineByMNo(mno).DbMessage;
                    if (DryDB.Pay.Any(m => m.MapNo == mno && m.ItemCode == 5))
                    {
                        Pay pays = DryDB.Pay.Where(m => m.MapNo == mno && m.ItemCode == 5).FirstOrDefault();
                        msg = paydb.DeletePay(pays).DbMessage;
                    }
                    if (msg.Equals("Success"))
                    {//更新TotelFee
                        dbstatus = new TotalFeeDBService().UpdateTotalFee(mno);
                    }
                    if (msg.Equals("Success"))//若儲存成功則更新目前步驟
                    {
                        msg = comm.UpdateCase(mno,3 /*4*/).DbMessage;
                    }
                    dbstatus.DbMessage = msg;
                    if (msg == "Success")
                    {
                        scope.Complete();

                    }
                    return dbstatus;
                }
                string rs = CreateEngineApply(engaply).DbMessage;
                if (!rs.Equals("Success"))
                {
                    msg = rs;
                }
                else
                {
                    List<Engine> engList = new List<Engine>();
                    List<short> engary = new List<short>();
                    foreach (var item in ResultArry.EngAry)
                    {
                        Engine eng = CreateEngObj(mno, item.EngineCode, /*item.RegCode*/"0000", item.EngPrice);
                        rs = CreateEngine(eng).DbMessage;
                        if (!rs.Equals("Success"))
                        {
                            msg = rs;
                            break;
                        }
                        else
                        {
                            engList.Add(eng);
                            engary.Add(item.EngineCode);//
                        }
                    }
                    Case casedata = gda.GetCaseDataFromMapNo(mno);
                    List<EngineData> engineList = new List<EngineData>();
                    
                    if (msg.Equals("Success"))//若儲存成功則將經費寫入至pay資料庫
                    {

                        foreach (var item in ResultArry.EngAry)
                        {
                            engineList.Add(new EngineData { EngineCode = item.EngineCode, RegCode = item.RegCode, EngPrice = item.EngPrice });
                        }
                        var mData = CalculateCls.GetEngMoneybyEngList(engineList.ToArray(), casedata);
                        Pay data = paydb.CreatePayObj(mno, 5, engaply.ApplyUnit, mData.GovPay, mData.FarmerPay);
                        msg = paydb.CreatePay(data).DbMessage;
                        if (msg.Equals("Success"))//若儲存成功則更新目前步驟
                        {
                            msg = comm.UpdateCase(mno,3 /*4*/).DbMessage;
                        }
                    }
                    if (msg.Equals("Success"))
                    {//寫入TotalFee
                        dbstatus = new TotalFeeDBService().UpdateTotalFee(mno);
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

        #region Modify Engine Data From DB
        public DBCommon.DbEvent ModifyEngine(JsData ResultArry, int mno)
        {
            Calculate_Funding CalculateCls = new Calculate_Funding();
            PayDBService paydb = new PayDBService();

            string msg = "Success";
            using (TransactionScope scope = new TransactionScope())
            {
                EngApply engaply = GetEngApplyDataByMapNo(mno);
                if(ResultArry.EngAry == null)
                {
                    msg = DelEngineByMNo(mno).DbMessage;
                    if (DryDB.Pay.Any(m => m.MapNo == mno && m.ItemCode == 5))
                    {
                        Pay pays = DryDB.Pay.Where(m => m.MapNo == mno && m.ItemCode == 5).FirstOrDefault();
                        msg = paydb.DeletePay(pays).DbMessage;
                    }
                    if (msg.Equals("Success"))
                    {//更新TotelFee
                        dbstatus = new TotalFeeDBService().UpdateTotalFee(mno);
                    }
                    dbstatus.DbMessage = msg;
                    if (msg == "Success")
                    {
                        scope.Complete();
                        
                    }
                    return dbstatus;
                }
                if (engaply == null)
                {
                    if (ResultArry.EngAry == null)
                    {
                        if (DryDB.Pay.Any(m => m.MapNo == mno && m.ItemCode == 5))
                        {
                            Pay pays = DryDB.Pay.Where(m => m.MapNo == mno && m.ItemCode == 5).FirstOrDefault();
                            msg = paydb.DeletePay(pays).DbMessage;
                        }
                        if (msg != "Success")
                        {
                            dbstatus.DbMessage = msg;
                            return dbstatus;
                        }
                    }

                    dbstatus = InsertEngine(ResultArry, mno);
                    if (msg.Equals("Success"))
                    {
                        scope.Complete();
                        return dbstatus;
                    }
                }
                    
                engaply.ApplyUnit = ResultArry.ddl_Unit;
                engaply.FacMoney = ResultArry.FacPrice;

                string rs = UpdateEngineApply(engaply).DbMessage;
                if (!rs.Equals("Success"))
                {
                    msg = rs;
                }
                else
                {
                    //first delete all engine, and create the new.
                    rs = DelEngineByMNo(mno).DbMessage;
                    if (!rs.Equals("Success"))
                    {
                        msg += rs;
                    }
                    else
                    {
                        List<Engine> engList = new List<Engine>();
                        List<short> engary = new List<short>();
                        foreach (var item in ResultArry.EngAry)
                        {
                            Engine eng = CreateEngObj(mno, item.EngineCode, /*item.RegCode*/"0000", item.EngPrice);
                            rs = CreateEngine(eng).DbMessage;
                            if (!rs.Equals("Success"))
                            {
                                msg += rs;
                                break;
                            }
                            else
                            {
                                engList.Add(eng);
                                engary.Add(item.EngineCode);//
                            }
                        }
                        #region old code
                        Case casedata = gda.GetCaseDataFromMapNo(mno);
                        List<EngineData> engineList = new List<EngineData>();
                        foreach (var item in ResultArry.EngAry)
                        {
                            engineList.Add(new EngineData { EngineCode = item.EngineCode, RegCode = item.RegCode, EngPrice = item.EngPrice });
                        }
                        var mData = CalculateCls.GetEngMoneybyEngList(engineList.ToArray(), casedata);
                        if (msg.Equals("Success"))//若成功則對經費進行更新
                        {
                            //var mData = CalculateCls.Calculate_Engine(engList, ResultArry.ddl_Unit);
                            Pay data = paydb.GetPay(mno, 5);
                            if(data==null)
                            {
                                data = new Pay();
                                data.ApplyUnit = ResultArry.ddl_Unit;
                                data.PayMoney = mData.GovPay;
                                data.FarmerMoney = mData.FarmerPay;
                                data.ItemCode = 5;
                                data.MapNo = mno;
                                msg = paydb.CreatePay(data).DbMessage;
                            }
                            else
                            {
                                data.ApplyUnit = ResultArry.ddl_Unit;
                                data.PayMoney = mData.GovPay;
                                data.FarmerMoney = mData.FarmerPay;
                                msg = paydb.UpdatePay(data).DbMessage;
                            }
                        }
                        if(msg.Equals("Success"))
                        {//更新TotelFee
                            dbstatus = new TotalFeeDBService().UpdateTotalFee(mno);
                        }
                        #endregion
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

        #region Json Data Struct
        public class JsData
        {
            public List<EngineData> EngAry { get; set; }
            public int FacPrice { get; set; }
            public short ddl_Unit { get; set; }
        }

        #region Engine New Add Data Struct
        public class EngineData
        {
            public byte EngineCode { get; set; }
            public string RegCode { get; set; }
            public int EngPrice { get; set; }
        }
        #endregion

        #endregion
    }
}
