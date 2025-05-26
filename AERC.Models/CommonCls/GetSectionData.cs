/*
-- =============================================
-- Author:  Wei
-- Create date: 2014-11-10
-- Description: 讀取地段資料
-- =============================================
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.CommonCls
{
    public class GetSectionData
    {
        CommonEntities commDB = new CommonEntities();
        /// <summary>
        /// 讀取地段資料
        /// </summary>
        /// <param name="TownId"></param>
        /// <returns></returns>
        public List<Section> GetSectionList(short TownId)
        {
            return commDB.Section.Where(p => p.Town_Id == TownId).ToList();
        }

        /// <summary>
        /// 取得地段代碼
        /// </summary>
        /// <param name="SectionID"></param>
        /// <returns></returns>
        public string GetSectionCode(int SectionID)
        {
            return commDB.Section.Find(SectionID).Section_Code.Value.ToString();
        }
    }
}
