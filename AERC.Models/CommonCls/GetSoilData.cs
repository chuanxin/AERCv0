/*
-- =============================================
-- Author:  Wei
-- Create date: 2014-7-14
-- Description: 讀取土壤資料
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.CommonCls
{
    public class GetSoilData
    {
        CommonEntities comm = new CommonEntities();

        /// <summary>
        /// 取得所有土壤資料
        /// </summary>
        /// <returns></returns>
        public List<Soil> GetSoil()
        {
            return comm.Soil.ToList();
        }
    }
}
