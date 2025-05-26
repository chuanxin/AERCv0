/*
-- =============================================
-- Author:  Wei
-- Create date: 2014-11-11
-- Description: 讀取作物資料
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models.ViewModel;
using System.Linq.Dynamic;

namespace AERC.Models.CommonCls
{
    public class GetCrop
    {
        CommonEntities commDB = new CommonEntities();
        /// <summary>
        /// 讀取作物資料
        /// </summary>
        /// <returns></returns>
        public List<CropView> GetCropList(string Query)
        {
            List<CropView> DataList = new List<CropView>();
            var crop = commDB.Crop.ToList();
            var croptype = commDB.Crop_Type.ToList();
            var data = (
                from Crops in crop
                join CropType in croptype on Crops.Crop_Type_Id equals CropType.Crop_Type_Id
                where Crops.Crop1.Contains(Query)
                select new
                {
                    Crops.Crop_Id,
                    Crops.Crop1,
                    Crops.Crop_Modulus,
                    CropType.Crop_Type1
                }
            );
            foreach(var item in data)
            {
                DataList.Add(new CropView
                {
                    Crop_Id = item.Crop_Id,
                    Crop = item.Crop1,
                    Crop_Modulus = item.Crop_Modulus.Value,
                    Crop_Type = item.Crop_Type1
                });
            }
            return DataList;
        }
        public List<Crop> GetCropList()
        {
            return commDB.Crop.ToList();
        }
        /// <summary>
        /// 讀取作物類型
        /// </summary>
        /// <returns></returns>
        public List<Crop_Type> GetCropTypeList()
        {
            return commDB.Crop_Type.ToList();
        }
    }
}
