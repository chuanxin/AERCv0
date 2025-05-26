/*
-- =============================================
-- Author:  Wei
-- Create date: 2014-17-5
-- Description: 讀取水利構造物資料
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models.ViewModel;
using AERC.Models;

namespace AERC.Models.CommonCls
{
    public class GetStructure
    {
        CommonEntities comm = new CommonEntities();
        
        /// <summary>
        /// 取得所有構造物資料
        /// </summary>
        /// <returns></returns>
        public List<Structure> GetStructureData()
        {
            return comm.Structure.ToList();
        }

        /// <summary>
        /// 取得特定構造物資料
        /// </summary>
        /// <param name="SId">構造物代碼</param>
        /// <returns></returns>
        public Structure GetStructureData(Guid SId)
        {
            return comm.Structure.Find(SId);
        }

        /// <summary>
        /// 取得特定構造物資料
        /// </summary>
        /// <param name="UnitID">單位代碼</param>
        /// <returns></returns>
        public List<Structure> GetStructureData(short UnitID)
        {
            return comm.Structure.Where(p => p.Unit_Id == UnitID).ToList();
        }

        /// <summary>
        /// 取得所有構造物型態
        /// </summary>
        /// <returns></returns>
        public List<StructureType> GetStuctureType()
        {
            return comm.StructureType.ToList();
        }

        /// <summary>
        /// 取得所有單位
        /// </summary>
        /// <returns></returns>
        public List<Unit> GetUnit()
        {
            return comm.Unit.ToList();
        }
    }
}
