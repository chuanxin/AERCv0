/*
-- =============================================
-- Author:  Wei
-- Create date: 2014-11-12
-- Description: 作物資料的資料庫操作類別
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.Service
{
    public class CropDBService
    {
        DBCommon.DbEvent status = new DBCommon.DbEvent();
        CommonEntities commDB = new CommonEntities();
        /// <summary>
        /// 新增作物(不包含作物系數欄位)
        /// </summary>
        /// <param name="CropName">作物</param>
        /// <param name="CropTypeID">作物類型代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent AddCrop(string CropName,short CropTypeID)
        {
            Crop crop = new Crop();
            crop.Crop1 = CropName;
            crop.Crop_Type_Id = CropTypeID;
            try
            {
                commDB.Crop.Add(crop);
                commDB.SaveChanges();
                status.DbMessage = "Success";
            }
            catch (Exception e)
            {
                status.DbMessage = "Create Crop Failed : " + e.Message;
            }
            return status;
        }
        /// <summary>
        /// 修改作物(不包含作物系數欄位)
        /// </summary>
        /// <param name="CropID">作物代碼</param>
        /// <param name="CropName">作物</param>
        /// <param name="CropTypeID">作物類型代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent EditCrop(short CropID, string CropName, short CropTypeID)
        {
            Crop crop = commDB.Crop.Single(p => p.Crop_Id == CropID);
            crop.Crop1 = CropName;
            crop.Crop_Type_Id = CropTypeID;
            try
            {
                commDB.Crop.Attach(crop);
                commDB.Entry(crop).State = System.Data.EntityState.Modified;
                commDB.SaveChanges();

                status.DbMessage = "Success";
                status.KeyValue = crop.Crop_Id;
            }
            catch (Exception e)
            {
                status.DbMessage = "Modified Crop Failed : " + e.Message;
            }
            return status;
        }
        /// <summary>
        /// 刪除作物
        /// </summary>
        /// <param name="CropID">作物代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent DelCrop(short CropID)
        {
            Crop crop = commDB.Crop.Single(p => p.Crop_Id == CropID);
            try
            {
                commDB.Crop.Remove(crop);
                commDB.SaveChanges();
                status.DbMessage = "Success";
            }
            catch (Exception e)
            {
                status.DbMessage = "Removed Crop Failed : " + e.Message;
            }
            return status;
        }
    }
}
