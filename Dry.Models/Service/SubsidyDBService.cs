using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;
using System.Web.Mvc;

namespace Dry.Models.Service
{
    public class SubsidyDBService
    {
        DryEntities DryDB = new DryEntities();
        public List<SelectListItem> GetFromSubsidyApplyYear()
        {
            List<SelectListItem> Data = new List<SelectListItem>();
            var ApplyYear = DryDB.SubsidyLimit.GroupBy(p => p.ApplyYear).Select(g => new { ApplyYear = g.Key, count = g.Count() }).ToList();
            foreach(var item in ApplyYear)
            {
                Data.Add(
                    new SelectListItem
                    {
                        Text = item.ApplyYear.ToString(),
                        Value = item.ApplyYear.ToString()
                    });
            }
            return Data;
        }

        public SubsidyLimit GetSubsidyLimitList(int unit, int year)
        {
            return DryDB.SubsidyLimit.Where(m => m.ApplyUnit == unit && m.ApplyYear == year).FirstOrDefault();
        }
    }
}
