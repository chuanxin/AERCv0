using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.Service
{
    public class LandNumberDBService
    {
        DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
        CommonEntities commDB = new CommonEntities();
        /// <summary>
        /// 編輯地號
        /// </summary>
        /// <param name="LandNumberID">地號代碼</param>
        /// <param name="Land_Code">地號</param>
        /// <param name="Longitude">經度</param>
        /// <param name="Latitude">緯度</param>
        /// <returns></returns>
        public DBCommon.DbEvent EditLandNumber(int LandNumberID, string Land_Code, decimal Longitude, decimal Latitude)
        {
            Land_Number LandNumber = commDB.Land_Number.Single(p => p.Land_Number_Id == LandNumberID);
            LandNumber.Land_Code = Land_Code;
            LandNumber.Longitude = Longitude;
            LandNumber.Latitude = Latitude;
            try
            {
                commDB.Land_Number.Attach(LandNumber);
                commDB.Entry(LandNumber).State = System.Data.EntityState.Modified;
                commDB.SaveChanges();

                dbMsg.DbMessage = "Success";
                dbMsg.KeyValue = LandNumber.Land_Number_Id;
            }
            catch (Exception e)
            {
                dbMsg.DbMessage = "Modified Land_Number Failed : " + e.Message;
            }
            return dbMsg;
        }
        /// <summary>
        /// 刪除地號
        /// </summary>
        /// <param name="LandNumberID">地號代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent DelLandNumber(int LandNumberID)
        {
            Land_Number LandNumber = commDB.Land_Number.Single(p => p.Land_Number_Id == LandNumberID);
            try
            {
                commDB.Land_Number.Remove(LandNumber);
                commDB.SaveChanges();
                dbMsg.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbMsg.DbMessage = "Removed Land_Number Failed : " + e.Message;
            }
            return dbMsg;
        }
        /// <summary>
        /// 新增地號
        /// </summary>
        /// <param name="Land_Code">地號</param>
        /// <param name="Section_Id">地段代號</param>
        /// <param name="Longitude">經度</param>
        /// <param name="Latitude">緯度</param>
        /// <returns></returns>
        public DBCommon.DbEvent AddLandNumber(string Land_Code, int Section_Id, decimal Longitude, decimal Latitude)
        {
            Land_Number LandNumber = new Land_Number();
            LandNumber.Land_Code = Land_Code;
            LandNumber.Section_Id = Section_Id;
            LandNumber.Longitude = Longitude;
            LandNumber.Latitude = Latitude;
            try
            {
                commDB.Land_Number.Add(LandNumber);
                commDB.SaveChanges();
                dbMsg.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbMsg.DbMessage = "Create Land_Number Failed : " + e.Message;
            }
            return dbMsg;
        }
    }
}
