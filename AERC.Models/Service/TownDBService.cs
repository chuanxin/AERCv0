using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models.ViewModel;

namespace AERC.Models.Service
{
    public class TownDBService
    {
        CommonEntities commDB = new CommonEntities();
        DBCommon.DbEvent dbStatus = new DBCommon.DbEvent();
        /// <summary>
        /// 編輯鄉鎮
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public DBCommon.DbEvent EditTown(TownVIew Data)
        {
            Town town = commDB.Town.Single(p => p.Town_Id == Data.Town_Id);
            town.Town1 = Data.Town;
            town.Zip_Code = Data.Zip_Code;
            town.Land_Office_Id = Data.Land_Office_Id;
            try
            {
                commDB.Town.Attach(town);
                commDB.Entry(town).State = System.Data.EntityState.Modified;
                commDB.SaveChanges();

                dbStatus.DbMessage = "Success";
                dbStatus.KeyValue = town.Town_Id;
            }
            catch (Exception e)
            {
                dbStatus.DbMessage = "Modified Town Failed : " + e.Message;
            }
            return dbStatus;
        }
        /// <summary>
        /// 新增鄉鎮
        /// </summary>
        /// <param name="CityCode">縣市官碼</param>
        /// <param name="Zip_Code">郵遞區號</param>
        /// <param name="Town_Code">鄉鎮代號</param>
        /// <param name="Town">鄉鎮名稱</param>
        /// <param name="Land_Office">地政事務所名稱</param>
        /// <returns></returns>
        public DBCommon.DbEvent AddTown(string CityCode,short Zip_Code, string Town_Code, string Town, short Land_Office)
        {
            Town town = new Town();
            town.Zip_Code = Zip_Code;
            town.Town_Code = Town_Code;
            town.Town1 = Town;
            town.City_Code = CityCode;
            town.Land_Office_Id = Land_Office;
            try
            {
                commDB.Town.Add(town);
                commDB.SaveChanges();
                dbStatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbStatus.DbMessage = "Create Town Failed : " + e.Message;
            }
            return dbStatus;
        }
        /// <summary>
        /// 刪除鄉鎮
        /// </summary>
        /// <param name="id">鄉鎮代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent DelTown(short id)
        {
            Town town = commDB.Town.Single(p => p.Town_Id == id);
            try
            {
                commDB.Town.Remove(town);
                commDB.SaveChanges();
                dbStatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbStatus.DbMessage = "Removed Town Failed : " + e.Message;
            }
            return dbStatus;
        }
    }
}
