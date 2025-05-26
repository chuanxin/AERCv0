/*
-- =============================================
-- Author: WEI
-- Create date: 2014-2-10
-- Description: 調控設施的資料庫操作類別
-- =============================================
*/

using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using AERC.Models;

namespace Dry.Models.Service
{
    public class CtrlDBService
    {
        private DryEntities DryDB = new DryEntities();
        private CommonEntities CommonDB = new CommonEntities();
        private GetData getData = new GetData();
        private DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
        private CommClass comm = new CommClass();

        public List<CntrlFac> GetCntrlFacDataList()
        {
            return DryDB.CntrlFac.ToList();
        }
        public List<CntrlList> GetCntrListDataList() 
        {
            //return DryDB.CntrlList.Where(c => c.CntrlCode >= 5).ToList();
            return DryDB.CntrlList.ToList();
        }
        /// <summary>
        /// 取得調控設施物料
        /// </summary>
        /// <param name="mapNo"></param>
        /// <returns></returns>
        public List<CtrlTableView> GetCntrlMatDataList(int mapNo)
        {
            var cntrlMat = DryDB.CntrlMat.ToList();
            var cntrlFac = DryDB.CntrlFac.ToList();
            var cntrlApply = DryDB.CntrlApply.ToList();
            var cntrlList = DryDB.CntrlList.ToList();
            var Common = CommonDB.Unit.ToList();
            
            var Result = (from CtrlMat in cntrlMat
                          join CtrlList in cntrlList on CtrlMat.CntrlCode equals CtrlList.CntrlCode
                          join CtrlFac in cntrlFac on CtrlMat.CFNo equals CtrlFac.CFNo
                          join CtrlApply in cntrlApply on CtrlFac.MapNo equals CtrlApply.MapNo
                          where CtrlApply.MapNo == mapNo
                          select new { CtrlMat.MatNo, CtrlList.CntrlCode, CtrlMat.MatName, CtrlMat.MatAmt, CtrlMat.MatPrice, CtrlMat.MatAmtAply,CtrlMat.MatPriceAply }
                        ).ToList();
            List<CtrlTableView> CtrlTableList = new List<CtrlTableView>();
            
            foreach (var item in Result)
            {
                CtrlTableList.Add(new CtrlTableView() { MatNo = item.MatNo, CntrlCode = item.CntrlCode, MatName = item.MatName, MatAmt = item.MatAmt, MatPrice = item.MatPrice, MatAmtAply=item.MatAmtAply,MatPriceAply = item.MatPriceAply });
            }
            return CtrlTableList;
        }
        
        public List<Unit> GetUnitDataList()
        {
            return CommonDB.Unit.ToList();
        }
        
        #region 新增資料
        /// <summary>
        /// 寫入CntrlApply、CntrlFac、CntrlMat、Pay資料表格
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public DBCommon.DbEvent CreateData(JsonData Data, int MapNo)
        {

            using (TransactionScope scope = new TransactionScope())
            {
                if(Data.CtrlMatAry == null)
                {
                    if(DryDB.CntrlFac.Any(m => m.MapNo == MapNo))
                    {
                        int cfno = DryDB.CntrlFac.Where(m => m.MapNo == MapNo).FirstOrDefault().CFNo;
                        dbstatus = DelCntrlMat(MapNo, cfno);
                    }
                    
                }
                else
                {
                    
                    CntrlApply cntrlApply = new CntrlApply();
                    cntrlApply.MapNo = MapNo;
                    cntrlApply.ApplyUnit = Data.ApplyUnit;
                    cntrlApply.FacMoney = 0;
                    dbstatus = CreateCntrlApply(cntrlApply, MapNo);
                    if (dbstatus.DbMessage == "Success")
                    {
                        
                        dbstatus = CreateCntrlFac(MapNo);
                        if (dbstatus.DbMessage == "Success")
                        {
                            int CFNo = Convert.ToInt32(dbstatus.KeyValue);
                            
                            dbstatus = CreateCntrlMat(Data.CtrlMatAry.ToArray(), MapNo, CFNo);
                        }
                    }
                    
                }
                if (dbstatus.DbMessage.Equals("Success"))
                {
                    dbstatus.DbMessage = comm.UpdateCase(MapNo, 6).DbMessage;
                }
                if (dbstatus.DbMessage == "Success")
                {
                    
                    dbstatus = CreatePay(MapNo, Data.ApplyUnit, Data.CtrlMatAry == null ? null : Data.CtrlMatAry.ToArray());
                    if (dbstatus.DbMessage.Equals("Success"))
                    {
                        dbstatus = new TotalFeeDBService().UpdateTotalFee(MapNo);
                    }
                }
                if (dbstatus.DbMessage == "Success")
                {
                    scope.Complete();
                }
                return dbstatus;
            }

        }
        /// <summary>
        /// 寫入CntrlMat資料表格
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <param name="ApplyUnit"></param>
        /// <param name="FacMoney"></param>
        public DBCommon.DbEvent CreateCntrlMat(CtrlTableView[] Data, int MapNo, int CFNo)
        {

            
            dbstatus = DelCntrlMat(MapNo, CFNo);
            if (dbstatus.DbMessage == "Success")
            {
                
                foreach (CtrlTableView CV in Data)
                {
                    CntrlMat RealData = new CntrlMat();
                    RealData.CFNo = CFNo;
                    RealData.CntrlCode = CV.CntrlCode;
                    RealData.MatName = CV.MatName;
                    RealData.MatAmt = CV.MatAmt;
                    RealData.MatPrice = CV.MatPrice;
                    RealData.MatAmtAply = CV.MatAmtAply;
                    RealData.MatPriceAply = CV.MatPriceAply;
                    
                    DryDB.CntrlMat.Add(RealData);
                }
                try
                {
                    
                    DryDB.SaveChanges();
                    dbstatus.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    dbstatus.DbMessage = "Inserted CntrlMat Failed : " + e.Message;
                }
            }

            return dbstatus;
        }
        /// <summary>
        /// 寫入CntrlFac資料表格
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        private DBCommon.DbEvent CreateCntrlFac(int MapNo)
        {
            CntrlFac Data = new CntrlFac();
            if (!DryDB.CntrlFac.Where(m => m.MapNo == MapNo).Any())
            {
                Data.MapNo = MapNo;
                DryDB.CntrlFac.Add(Data);
                try
                {
                    DryDB.SaveChanges();
                    dbstatus.KeyValue = DryDB.CntrlFac.Single(m => m.MapNo == MapNo).CFNo;
                    dbstatus.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    dbstatus.DbMessage = "CntrlFac Failed：" + e.Message;
                }
            }
            else
            {
                dbstatus.KeyValue = DryDB.CntrlFac.Single(m => m.MapNo == MapNo).CFNo;
                dbstatus.DbMessage = "Success";
            }
            return dbstatus;
        }
        /// <summary>
        /// 寫入CntrlApply資料表格
        /// </summary>
        /// <param name="cntrlApply"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        private DBCommon.DbEvent CreateCntrlApply(CntrlApply cntrlApply, int MapNo)
        {
            
            CntrlApply cntrlApply_temp = getData.GetCntrlApplyData(MapNo);
            try
            {
                if (cntrlApply_temp == null)
                {
                    DryDB.CntrlApply.Add(cntrlApply);
                    
                    new CommClass().UpdateCase(MapNo, 6);
                }
                else
                {
                    DryDB.CntrlApply.Attach(cntrlApply);
                    DryDB.Entry(cntrlApply).State = System.Data.EntityState.Modified;
                }
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modified CntrlApply Failed : " + e.Message;
            }
            return dbstatus;
        }

        /// <summary>
        /// 寫入Pay資料表格
        /// </summary>
        /// <param name="MapNo">版本編號</param>
        /// <param name="ApplyUnit">補助單位代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent CreatePay(int MapNo, short ApplyUnit, CtrlTableView[] Data)
        {
            CtrlPriceService ctrlPrice = new CtrlPriceService();
            int[] Price = new int[3];
            Price = ctrlPrice.GetCtrlHelpTotalPrice(Data, MapNo);
            PayDBService payDB = new PayDBService();
            Pay pay = payDB.GetPay(MapNo, 4);

            
            if (pay == null)
            {
                pay = new Pay();
                pay.MapNo = MapNo;
                pay.ItemCode = 4;
                pay.ApplyUnit = ApplyUnit;
                pay.PayMoney = Price[1];
                pay.FarmerMoney = Price[2];
                dbstatus = payDB.CreatePay(pay);
            }
            else
            {
                pay.MapNo = MapNo;
                pay.ItemCode = 4;
                pay.ApplyUnit = ApplyUnit;
                pay.PayMoney = Price[1];
                pay.FarmerMoney = Price[2];
                dbstatus = payDB.UpdatePay(pay);
            }
            return dbstatus;
        }
        #endregion

        #region 刪除資料
        /// <summary>
        /// 刪除CntrlMat資料
        /// </summary>
        /// <param name="Data"></param>
        public DBCommon.DbEvent DelCntrlMat(int MapNo, int CFNo)
        {
            IQueryable<CntrlMat> ctrlMat = DryDB.CntrlMat.Where(p => p.CFNo == CFNo);
            CtrlView MatView = new CtrlView();
            MatView.MatDataList = ctrlMat.ToList();
            foreach (var item in MatView.MatDataList)
            {
                CntrlMat cntrlMat = DryDB.CntrlMat.Find(item.MatNo);
                DryDB.CntrlMat.Remove(cntrlMat);
            }
            //DryDB.CntrlMat
            try
            {
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed CntrlMat Failed : " + e.Message;
            }
            return dbstatus;
        }
        #endregion

        public int GetFacMoney(int MapNo)
        {
            try
            {
                return DryDB.CntrlApply.Single(c => c.MapNo == MapNo).FacMoney;
            }
            catch
            {
                return 0;
            }
        }
    }
}
