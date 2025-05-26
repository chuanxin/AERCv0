using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.CommonCls;

namespace Dry.Models.Service
{
    public class DesignReceiptDBService
    {
        DryEntities DryDB = new DryEntities();
        public List<DataStruct> GetPayeeReceiptData(short unit, int year, int IANumStart, int IANumEnd)
        {
            GetData gd = new GetData();
            List<DataStruct> Data = new List<DataStruct>();
            List<Pay> total = DryDB.Pay.ToList();
            var resdata = (from summview in DryDB.SummaryView
                           join lasvermno in DryDB.LastVerOfMapNo on summview.MapNo equals lasvermno.MapNo
                           join pay in DryDB.Pay on summview.MapNo equals pay.MapNo
                           where summview.ApplyUnit == unit && summview.ApplyYear == year && summview.IANum >= IANumStart && summview.IANum <= IANumEnd && (pay.ItemCode == 2)
                           select new
                           {
                               summview.MapNo,
                               summview.IANum,
                               summview.Name,
                               summview.IdNo,
                               summview.Addr,
                               summview.Designer_Name,
                               pay.PayMoney
                           }).OrderBy(m => m.IANum).ToList();
            //var datas = (from pay in DryDB.Pay
            //             join smv in DryDB.SummaryView on pay.MapNo equals smv.MapNo
            //             where (smv.ApplyYear == year) && (smv.ApplyUnit == unit) && (smv.IANum >= IANumStart) && (smv.IANum <= IANumEnd) && (pay.ItemCode == 2)
            //             select new { smv.ApplyYear, pay.PayMoney, smv.IANum });
            NumberToChinese ntoc = new NumberToChinese();
            foreach (var item in resdata)
            {
                Data.Add(new DataStruct
                {
                    IAName = gd.GetUnitName(unit),
                    IANum = item.IANum,
                    Content = item.Designer_Name,
                    ApplyYear = year,
                    FarmerName = item.Name,
                    FarmerID = item.IdNo,
                    Addr = item.Addr.Replace(" ", ""),
                    //SubsidyFeeCNS = ntoc.GetChineseNumber(total.Any(m => m.MapNo == item.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8) ? total.Where(m => m.MapNo == item.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8).Sum(n => n.PayMoney) : 0) + "元整",
                    //Payfee = total.Any(m => m.MapNo == item.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8) ? total.Where(m => m.MapNo == item.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8).Sum(n => n.PayMoney) : 0
                    SubsidyFeeCNS = ntoc.GetChineseNumber(item.PayMoney) + "元整",
                    Payfee = item.PayMoney
                });
            }
            return Data;
        }

        public class DataStruct
        {
            public int ApplyYear { get; set; }
            public string Content { get; set; }
            public string IAName { get; set; }
            public int IANum { get; set; }
            public string FarmerName { get; set; }
            public string FarmerID { get; set; }
            public string Addr { get; set; }
            public string SubsidyFeeCNS { get; set; }
            public int Payfee { get; set; }

        }
    }
}
