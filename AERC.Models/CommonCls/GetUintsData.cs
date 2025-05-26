/*
-- =============================================
-- Author:  Wei
-- Create date: 2015-1-21
-- Description: 讀取單位資料
-- =============================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AERC.Models.CommonCls
{
    public class GetUints
    {
        CommonEntities commDB = new CommonEntities();
        public Unit GetUnitData(short UnitID)
        {
            return commDB.Unit.Find(UnitID);
        }
        public List<SelectListItem> GetUnitDDL()
        {
            List<SelectListItem> Data = new List<SelectListItem>();
            foreach(var item in commDB.Unit.ToList())
            {
                Data.Add(new SelectListItem
                {
                    Text = item.Unit1,
                    Value = item.Unit_Id.ToString()
                });
            }
            return Data;
        }
    }
}
