using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
using Dry.Models.Service;
using System.Data;

/*
-- =============================================
-- Author: Ian
-- Create date: 2015-1-28
-- Description: 設計者資料庫操作類別
-- =============================================
*/

namespace Dry.Models.Service
{
    public class DesignerDBService
    {
        private DryEntities DryDB = new DryEntities();

        /// <summary>
        /// 取得設計者列表
        /// </summary>
        /// <param name="Unit_Id">單位ID</param>
        /// <returns>設計者列表</returns>
        public List<DesignerList> GetDesignerDatas(short Unit_Id)
        {
            return DryDB.DesignerList.Where(x => x.Unit_Id == Unit_Id).ToList();
        }

        /// <summary>
        /// 新增設計者
        /// </summary>
        /// <param name="designer">設計者資料</param>
        /// <returns>新增筆數</returns>
        public int InsertDesigner(DesignerList designer)
        {
            int i = 0;
            try
            {
                DryDB.DesignerList.Add(designer);
                i = DryDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return i;
        }

        /// <summary>
        /// 取得設計者資料
        /// </summary>
        /// <param name="Designer_Id">設計者ID</param>
        /// <returns>設計者資料</returns>
        public DesignerList EditDesigner(Guid Designer_Id)
        {
            return DryDB.DesignerList.Find(Designer_Id);
        }

        /// <summary>
        /// 修改設計者資料
        /// </summary>
        /// <param name="designer">設計者資料</param>
        /// <returns>修改筆數</returns>
        public int UpdateDesigner(DesignerList designer)
        {
            DesignerList designerList = DryDB.DesignerList.Where(x => x.Designer_Id == designer.Designer_Id).FirstOrDefault();
            designerList.Designer_Name = designer.Designer_Name;
            designerList.Phone = designer.Phone;
            return DryDB.SaveChanges();
        }

        /// <summary>
        /// 刪除設計者資料
        /// </summary>
        /// <param name="Designer_Id">設計者ID</param>
        /// <returns>刪除筆數</returns>
        public int DelDesigner(Guid Designer_Id)
        {
            DesignerList designer = DryDB.DesignerList.Find(Designer_Id);
            DryDB.DesignerList.Remove(designer);
            return DryDB.SaveChanges();
        }
    }
}
