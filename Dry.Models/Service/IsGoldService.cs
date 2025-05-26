/*
-- =============================================
-- Author: WEI
-- Create date: 2014-3-5
-- Description: 判斷是否為黃金廊道
-- =============================================
*/

using Dry.Models.CommonCls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.Service
{
    public class IsGoldService
    {
        /// <summary>
        /// 判斷是否為黃金廊道
        /// </summary>
        /// <param name="MapNo">案件代碼</param>
        /// <returns></returns>
        public bool IsGold(int MapNo)
        {
            GetData getDate = new GetData();
            Case dbCase = getDate.GetCaseDataFromMapNo(MapNo);
            return dbCase.Gold;
        }
    }
}
