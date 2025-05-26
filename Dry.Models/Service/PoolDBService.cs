using Dry.Models.CommonCls;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-2-13
-- Description: 蓄水池的資料庫操作類別
-- =============================================
*/
namespace Dry.Models.Service
{
    public class PoolDBService
    {
        private DryEntities DryDB = new DryEntities();
        private DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
        private GetData gda = new GetData();
        private CommClass comm = new CommClass();

        #region Obtain pool type list from db.
        public List<PoolTypeList> GetPoolTypeList()
        {
            //return DryDB.PoolTypeList.ToList();
            
            return DryDB.PoolTypeList.Where(o => o.PtypeCode >= 3).ToList();
        }
        #endregion

        #region Obtain farmer pool data by P_NO.
        public Pool GetPoolByPNo(int pno)
        {
            return DryDB.Pool.Single(p => p.PNo == pno);
        }
        #endregion

        #region  Obtain farmer pool apply case data by MapNo.
        public PoolApply GetPoolApplyDataByMapNo(int mno)
        {
            return DryDB.PoolApply.Find(mno);
        }
        #endregion

        #region Create Pool Object
        public Pool CreatePoolObj(int MNo, short poolweight, string poolsize, byte pooltypecode, int poolPrice)
        {
            Pool pooldata = new Pool();
            pooldata.MapNo = MNo;
            pooldata.PoolWeight = poolweight;
            pooldata.PoolSize = poolsize;
            pooldata.PtypeCode = pooltypecode;
            pooldata.PoolPrice = poolPrice;
            return pooldata;
        }
        #endregion

        #region DB Event of PoolApply
        public DBCommon.DbEvent CreatePoolApply(PoolApply data)
        {
            if (DryDB.PoolApply.Any(m => m.MapNo == data.MapNo))
            {
                try
                {
                    DryDB.PoolApply.Attach(data);
                    DryDB.Entry(data).State = System.Data.EntityState.Modified;
                    DryDB.SaveChanges();
                    dbstatus.KeyValue = data.MapNo;
                    dbstatus.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    dbstatus.DbMessage = "Create PoolApply Failed : " + e.Message + "<br>";
                }
            }
            else
            {
                try
                {
                    DryDB.PoolApply.Add(data);
                    DryDB.SaveChanges();
                    dbstatus.KeyValue = data.MapNo;
                    dbstatus.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    dbstatus.DbMessage = "Create PoolApply Failed : " + e.Message + "<br>";
                }
            }

            return dbstatus;
        }

        public DBCommon.DbEvent UpdatePoolApply(PoolApply data)
        {
            try
            {
                DryDB.PoolApply.Attach(data);
                DryDB.Entry(data).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.MapNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modified PoolApply Failed  : " + e.Message + "<br>";
            }
            return dbstatus;
        }
        #endregion

        #region DB Event of Pool
        public DBCommon.DbEvent CreatePool(Pool data)
        {
            try
            {
                DryDB.Pool.Add(data);
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.PNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Create Pool Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent UpdatePool(Pool data)
        {
            try
            {
                DryDB.Pool.Attach(data);
                DryDB.Entry(data).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.PNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modified Pool Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent DeletePool(Pool data)
        {
            try
            {
                dbstatus.KeyValue = data.PNo;
                DryDB.Pool.Remove(data);
                DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Pool Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent DelPoolByMNo(int mno)
        {
            try
            {
                var poolList = DryDB.Pool.Where(x => x.MapNo == mno).ToList();
                if (poolList.Count > 0)
                {
                    foreach (var data in poolList)
                    {
                        DryDB.Pool.Remove(data);
                        DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                        DryDB.SaveChanges();
                    }
                }
                
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Pool By MNo Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent DelPoolData(int MNO)
        {
            try
            {
                var applypool = DryDB.PoolApply.Find(MNO);
                if (applypool != null)
                {
                    var poolList = DryDB.Pool.Where(m => m.MapNo == MNO).ToList();
                    foreach (var data in poolList)
                    {
                        DryDB.Pool.Remove(data);
                        DryDB.Entry(data).State = System.Data.EntityState.Deleted;
                        DryDB.SaveChanges();
                    }
                    DryDB.PoolApply.Remove(applypool);
                    DryDB.Entry(applypool).State = System.Data.EntityState.Deleted;
                    DryDB.SaveChanges();
                }
                dbstatus.DbMessage = "Success";
            }
            catch(Exception e)
            {
                dbstatus.DbMessage = "Removed Pool By MNo Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        #endregion

        #region Insert Pool Data To DB
        public DBCommon.DbEvent InsertPool(JsData ResultArry, int mno)
        {
            Calculate_Funding CalculateCls = new Calculate_Funding();
            PayDBService paydb = new PayDBService();
            PoolApply poolaply = new PoolApply();
            string msg = "Success";
            
            using (TransactionScope scope = new TransactionScope())
            {
                if(ResultArry.PoolAry != null)
                {
                    
                    poolaply.MapNo = mno;
                    poolaply.ApplyUnit = ResultArry.ddl_Unit;
                    poolaply.FacMoney = ResultArry.FacPrice;

                    string rs = CreatePoolApply(poolaply).DbMessage;
                    if (!rs.Equals("Success"))
                    {
                        msg = rs;
                    }
                    else
                    {
                        List<Pool> poolList = new List<Pool>();
                        List<PoolData> PoolAry = new List<PoolData>();
                        foreach (var item in ResultArry.PoolAry)
                        {
                            if (item.poolSize == null)
                                item.poolSize = "";
                            Pool pool = CreatePoolObj(mno, item.poolW, item.poolSize, item.poolType, item.poolPrice);
                            rs = CreatePool(pool).DbMessage;
                            if (!rs.Equals("Success"))
                            {
                                msg = rs;
                                break;
                            }
                            else
                            {
                                poolList.Add(pool);

                                PoolData pdata = new PoolData();
                                pdata.poolType = item.poolType;
                                pdata.poolW = item.poolW;
                                PoolAry.Add(pdata);
                            }
                        }


                    }
                }
                
                if (msg.Equals("Success"))
                {
                    //msg = comm.UpdateCase(mno, 5).DbMessage;
                    msg = comm.UpdateCase(mno, 4).DbMessage;
                }

                if (msg.Equals("Success"))
                {
                    Case casedata = gda.GetCaseDataFromMapNo(mno);
                    List<PoolData> pooldataList = new List<PoolData>();
                    foreach (var item in ResultArry.PoolAry)
                    {
                        pooldataList.Add(new PoolData { poolType = item.poolType, poolW = item.poolW, poolSize = item.poolSize, poolPrice = item.poolPrice });
                    }
                    var mData = CalculateCls.GetPoolMoneybyList(pooldataList.ToArray(), casedata, mno);
                    Pay data = paydb.CreatePayObj(mno, 6, poolaply.ApplyUnit, mData.GovPay, mData.FarmerPay);
                    msg = paydb.CreatePay(data).DbMessage;
                }
                if (msg.Equals("Success"))
                {
                    dbstatus = new TotalFeeDBService().UpdateTotalFee(mno);
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

        #region Modify Pool Data From DB
        public DBCommon.DbEvent ModifyPool(JsData ResultArry, int mno)
        {
            Calculate_Funding CalculateCls = new Calculate_Funding();
            PayDBService paydb = new PayDBService();

            PoolApply poolaply = GetPoolApplyDataByMapNo(mno);
            
            //
            string msg = "Success";
            using (TransactionScope scope = new TransactionScope())
            {
                string rs = "";
                if (ResultArry.PoolAry.Count > 0)
                {
                    if (poolaply != null)
                    {
                        poolaply.ApplyUnit = ResultArry.ddl_Unit;
                        poolaply.FacMoney = ResultArry.FacPrice;
                        rs = UpdatePoolApply(poolaply).DbMessage;
                    }
                    else
                    {
                        poolaply = new PoolApply();
                        poolaply.MapNo = mno;
                        poolaply.ApplyUnit = ResultArry.ddl_Unit;
                        poolaply.FacMoney = ResultArry.FacPrice;
                        rs = CreatePoolApply(poolaply).DbMessage;
                    }
                }
                else
                {
                    if (poolaply != null)
                    {
                        rs = DelPoolData(mno).DbMessage;
                    }
                    else
                    {
                        rs = "Success";
                    }
                }
                
                if (!rs.Equals("Success"))
                {
                    msg = rs;
                }
                else
                {
                    rs = DelPoolByMNo(mno).DbMessage;
                    if (rs.Equals("Success"))
                    {
                        List<Pool> poolList = new List<Pool>();
                        List<PoolData> PoolAry = new List<PoolData>();
                        foreach (var item in ResultArry.PoolAry)
                        {
                            Pool pool = CreatePoolObj(mno, item.poolW, item.poolSize, item.poolType, item.poolPrice);
                            rs = CreatePool(pool).DbMessage;
                            if (!rs.Equals("Success"))
                            {
                                msg = rs;
                                break;
                            }
                            else
                            {
                                poolList.Add(pool);
                                PoolData pdata = new PoolData();
                                pdata.poolType = item.poolType;
                                pdata.poolW = item.poolW;
                                PoolAry.Add(pdata);
                            }
                        }
                        if (msg.Equals("Success"))
                        {
                            Case casedata = gda.GetCaseDataFromMapNo(mno);
                            List<PoolData> pooldataList = new List<PoolData>();
                            foreach (var item in ResultArry.PoolAry)
                            {
                                pooldataList.Add(new PoolData { poolType = item.poolType, poolW = item.poolW, poolSize = item.poolSize, poolPrice = item.poolPrice });
                            }
                            var mData = CalculateCls.GetPoolMoneybyList(pooldataList.ToArray(), casedata, mno);
                            Pay data = paydb.GetPay(mno, 6);
                            if (data == null)
                            {
                                data = paydb.CreatePayObj(mno, 6, ResultArry.ddl_Unit, mData.GovPay + mData.GoldPay, mData.FarmerPay);
                                msg = paydb.CreatePay(data).DbMessage;
                            }
                            else
                            {
                                data.ApplyUnit = ResultArry.ddl_Unit;
                                data.PayMoney = mData.GovPay + mData.GoldPay;
                                data.FarmerMoney = mData.FarmerPay ;
                                msg = paydb.UpdatePay(data).DbMessage;
                            }

                        }
                        if (msg.Equals("Success"))
                        {
                            Case casedata = gda.GetCaseDataFromMapNo(mno);
                            if (casedata.Gold == true) 
                            {
                                dbstatus = new TotalFeeDBService().UpdateTotalFee(mno);
                            }
                            else
                            {
                                dbstatus = new TotalFeeDBService().UpdateTotalFee(mno);
                            }
                            
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
        /// <summary>
        /// 取得蓄水池單價
        /// </summary>
        /// <param name="caseData"></param>
        /// <param name="PtypeCode"></param>
        /// <param name="PoolTon"></param>
        /// <returns></returns>
        public int CaculatePoolPrice(Case caseData,int MNO,byte PtypeCode,int PoolTon)
        {
            //Calculate_Funding cacu = new Calculate_Funding();
            //return cacu.GetpoolMoney(PtypeCode, PoolTon, caseData, MNO);
            GetData gd = new GetData();
            return gd.GetPoolPriceByCase(caseData, PtypeCode, PoolTon);
        }

        /// <summary>
        /// 取得15年或20年蓄水池單價
        /// </summary>
        /// <param name="caseData"></param>
        /// <param name="PtypeCode"></param>
        /// <param name="PoolTon"></param>
        /// <returns></returns>
        public int CaculatePoolPriceByYears(int PoolTon,int years)
        {
            Calculate_Funding cacu = new Calculate_Funding();
            return cacu.GetPoolPriceByWeightYear(PoolTon, years);
        }

        #region Json Data Struct
        public class JsData
        {
            public List<PoolData> PoolAry { get; set; }
            public int FacPrice { get; set; }
            public short ddl_Unit { get; set; }
        }

        #region Engine New Add Data Struct
        public class PoolData
        {
            public byte poolType { get; set; }
            public short poolW { get; set; }
            public string poolSize { get; set; }
            public int poolPrice { get; set; }
        }
        #endregion

        #endregion
    }
}
