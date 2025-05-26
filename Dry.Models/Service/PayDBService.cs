using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.Service
{
    public class PayDBService
    {
        private DryEntities DryDB = new DryEntities();
        private DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();

        #region Get Pay Object By mno and itemcode
        /// <summary>
        /// Get Pay Object By mno and itemcode
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <param name="ItemCode">項目，1:田間管路設施費, 2:規劃設計費, 3:水源設施費, 4:調控設施費, 5:動力設備費, 6:蓄水設施費</param>
        /// <returns>Pay Object</returns>
        public Pay GetPay(int mno, byte ItemCode)
        {
            if (DryDB.Pay.Where(p => p.MapNo == mno && p.ItemCode == ItemCode).Any())
            {
                return DryDB.Pay.Single(p => p.MapNo == mno && p.ItemCode == ItemCode);
            }
            else
            {
                return null;
            }
        }
        #endregion
        
        #region DB Event of Pay
        public DBCommon.DbEvent CreatePay(Pay data)
        {
            if (DryDB.Pay.Where(m => m.MapNo == data.MapNo && m.ItemCode == data.ItemCode).Any())
            {
                Pay pay = DryDB.Pay.Where(m => m.MapNo == data.MapNo && m.ItemCode == data.ItemCode).First();
                pay.MapNo = data.MapNo;
                pay.ItemCode = data.ItemCode;
                pay.ApplyUnit = data.ApplyUnit;
                pay.PayMoney = data.PayMoney;
                pay.FarmerMoney = data.FarmerMoney;
                DryDB.Pay.Attach(pay);
                DryDB.Entry(pay).State = System.Data.EntityState.Modified;
                dbstatus.KeyValue = pay.PayNo;
            }
            else
            {
                Pay pays = new Pay();
                pays.MapNo = data.MapNo;
                pays.ItemCode = data.ItemCode;
                pays.ApplyUnit = data.ApplyUnit;
                pays.PayMoney = data.PayMoney;
                pays.FarmerMoney = data.FarmerMoney;
                pays.Total = data.Total;
                DryDB.Pay.Add(pays);
                dbstatus.KeyValue = pays.PayNo;
            }
            try
            {
                DryDB.SaveChanges();

                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Create Pay Failed : " + e.Message + "<br>";
            }

            return dbstatus;
        }


        public DBCommon.DbEvent UpdatePay(Pay data)
        {
            try
            {
                Pay newData = DryDB.Pay.Where(m => m.MapNo == data.MapNo && m.ItemCode == data.ItemCode).First();
                newData.PayMoney = data.PayMoney;
                newData.FarmerMoney = data.FarmerMoney;
                DryDB.Pay.Attach(newData);
                DryDB.Entry(newData).State = System.Data.EntityState.Modified;
                DryDB.SaveChanges();
                dbstatus.KeyValue = data.PayNo;
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modified Pay Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        public DBCommon.DbEvent DeletePay(Pay data)
        {
            try
            {
                Pay pays = DryDB.Pay.Find(data.PayNo);
                dbstatus.KeyValue = pays.PayNo;
                DryDB.Pay.Remove(pays);
                DryDB.Entry(pays).State = System.Data.EntityState.Deleted;
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Pay Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }

        #endregion
                
        #region Create Pay Object
        /// <summary>
        /// 建立Pay Object
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <param name="ItemCode">項目，1:田間管路設施費, 2:規劃設計費, 3:水源設施費, 4:調控設施費, 5:動力設備費, 6:蓄水設施費</param>
        /// <param name="Unit">補助單位</param>
        /// <param name="ApplyMoney">農戶補助款</param>
        /// <param name="FormerMoney">農戶自備款</param>
        /// <returns>Pay Object</returns>       
        public Pay CreatePayObj(int mno, byte ItemCode, short Unit, int ApplyMoney, int FormerMoney)
        {
            Pay paydata = new Pay();
            paydata.MapNo = mno;
            paydata.ItemCode = ItemCode;
            paydata.ApplyUnit = Unit;
            paydata.PayMoney = ApplyMoney;
            paydata.FarmerMoney = FormerMoney;
            return paydata;
        }
        #endregion
   
    }
}
