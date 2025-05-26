/*
-- =============================================
-- Author:  Wei
-- Create date: 2014-10-5
-- Description: 讀取地段、地號資料
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.CommonCls
{
    public class GetLandNumber
    {
        CommonEntities comm = new CommonEntities();
        /// <summary>
        /// 取得地號資料
        /// </summary>
        /// <param name="SectionID">地段代碼</param>
        /// /// <param name="LandCode">地號</param>
        /// <returns></returns>
        public List<Land_Number> GetLandNumberData(int SectionID, string LandCode)
        {
            string[] LandCodeSplit = LandCode.Split('-');
            if (LandCodeSplit.Count() == 2)
            {
                int param1 = Convert.ToInt32(LandCodeSplit[0]);
                int param2 = Convert.ToInt32(LandCodeSplit[1]);
                LandCode = param1.ToString() + "-" + param2.ToString();
                if (param2 == 0)
                {
                    LandCode = param1.ToString();
                }
            }
            else
            {
                LandCode = Convert.ToInt32(LandCodeSplit[0]).ToString();
            }
            return comm.Land_Number.Where(p => p.Section_Id == SectionID && p.Land_Code == LandCode).ToList();
        }
        /// <summary>
        /// 取得地號資料集
        /// </summary>
        /// <param name="SectionID">地段代碼</param>
        /// <returns></returns>
        public List<Land_Number> GetLandNumberList(int SectionID)
        {
            return comm.Land_Number.Where(p => p.Section_Id == SectionID).ToList();
        }
    }
}
