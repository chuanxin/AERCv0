using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.Service;
using Dry.Models.ViewModel;

namespace Dry.Models.Service
{
    public class ReportService
    {
        DryEntities DryDB = new DryEntities();
        public ReportView.ApplyArea SubsidyAreaOnLiugong()
        {
            ReportView.ApplyArea reportView = new ReportView.ApplyArea();
            List<Case> cases = DryDB.Case.Where(m => m.ApplyUnit == 17).ToList();
            foreach(var item in cases)
            {
                int Mno = item.VerMapping.FirstOrDefault().Farm.Max(m => m.MapNo);
                List<Farm> farms = DryDB.Farm.Where(m => m.MapNo == Mno).ToList();
                var FarmSectionGroup = farms.GroupBy(m => m.Section);

            }

            return reportView;
        }
        /// <summary>
        /// 0:MAX; 1:MIN
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<int> GetIANumFromMaxToMin(short unit,int year)
        {
            List<Case> datalist = DryDB.Case.Where(m => m.ApplyUnit == unit && m.ApplyYear == year && m.Enable == true).ToList();
            List<int> data = new List<int>();
            if (datalist.Count > 0)
            {
                data.Add(datalist.Max(m => m.IANum));
                data.Add(datalist.Min(m => m.IANum));
            }
            else
            {
                data.Add(0);
                data.Add(0);
            }
            return data;
        }
    }
}
