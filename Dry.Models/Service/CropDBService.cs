using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;
using System.Web.Mvc;

namespace Dry.Models.Service
{
    public class CropDBService
    {
        DBCommon.DbEvent dbStatus = new DBCommon.DbEvent();
        /// <summary>
        /// 搜尋作物資料-依作物名稱搜尋
        /// </summary>
        /// <param name="Query"></param>
        /// <returns></returns>
        public List<CropView> GetCropByQuery(string Query)
        {
            List<AERC.Models.ViewModel.CropView> OriData = new AERC.Models.CommonCls.GetCrop().GetCropList(Query);
            List<CropView> Data = new List<CropView>();
            foreach(var item in OriData)
            {
                Data.Add(new CropView { Crop_Id = item.Crop_Id, Crop = item.Crop, Crop_Type = item.Crop_Type });
            }
            return Data;
        }
        /// <summary>
        /// 修改作物
        /// </summary>
        /// <param name="CropID">作物代碼</param>
        /// <param name="CropName">作物</param>
        /// <param name="CropTypeID">作物類型代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent EditCrop(short CropID, string CropName, short CropTypeID)
        {
            var status = new AERC.Models.Service.CropDBService().EditCrop(CropID, CropName, CropTypeID);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }

        /// <summary>
        /// 刪除作物
        /// </summary>
        /// <param name="CropID">作物代碼</param>
        /// <returns></returns>
        public DBCommon.DbEvent DelCrop(short CropID)
        {
            var status = new AERC.Models.Service.CropDBService().DelCrop(CropID);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }
        /// <summary>
        /// 新增作物
        /// </summary>
        /// <param name="CropName"></param>
        /// <param name="CropTypeID"></param>
        /// <returns></returns>
        public DBCommon.DbEvent AddCrop(string CropName,short CropTypeID)
        {
            var status = new AERC.Models.Service.CropDBService().AddCrop(CropName, CropTypeID);
            dbStatus.DbMessage = status.DbMessage;
            dbStatus.KeyValue = status.KeyValue;
            return dbStatus;
        }

        public List<SelectListItem> GetCropTypeDDL()
        {
            List<SelectListItem> CropType = new List<SelectListItem>();
            var Data = new AERC.Models.CommonCls.GetCrop().GetCropTypeList();
            foreach(var item in Data)
            {
                CropType.Add(new SelectListItem
                {
                    Text = item.Crop_Type1,
                    Value = item.Crop_Type_Id.ToString()
                });
            }
            return CropType;
        }
    }
}
