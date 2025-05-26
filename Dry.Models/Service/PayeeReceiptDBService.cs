using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.CommonCls;

namespace Dry.Models.Service
{
    public class PayeeReceiptDBService
    {
        DryEntities DryDB = new DryEntities();
        public List<DataStruct> GetPayeeReceiptData(short unit, int year, int IANumStart, int IANumEnd)
        {
            GetData gd = new GetData();
            List<DataStruct> Data = new List<DataStruct>();
            List<Pay> total = DryDB.Pay.ToList();
            var resdata = (from summview in DryDB.SummaryView
                           join lasvermno in DryDB.LastVerOfMapNo on summview.MapNo equals lasvermno.MapNo
                           where summview.ApplyUnit == unit && summview.ApplyYear == year && summview.IANum >= IANumStart && summview.IANum <= IANumEnd
                           select new
                           {
                               summview.MapNo,
                               summview.IANum,
                               summview.Name,
                               summview.IdNo,
                               summview.Addr
                           }).OrderBy(m => m.IANum).ToList();
            NumberToChinese ntoc = new NumberToChinese();
            foreach (var item in resdata)
            {
                Data.Add(new DataStruct
                {
                    IAName = gd.GetUnitName(unit),
                    IANum = item.IANum,
                    Content = (unit == 17 ? "此係旱作灌溉推廣計畫設施補助款，上款如數領訖無訛。" : "此係推廣省水管路灌溉設施補助計畫設施補助款，上款如數領訖無訛。"),
                    ApplyYear = year,
                    FarmerName = item.Name,
                    FarmerID = item.IdNo,
                    Addr = item.Addr.Replace(" ", ""),
                    SubsidyFeeCNS = ntoc.GetChineseNumber(total.Any(m => m.MapNo == item.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8) ? total.Where(m => m.MapNo == item.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8).Sum(n => n.PayMoney) : 0) + "元整" ,
                    Payfee = total.Any(m => m.MapNo == item.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8) ? total.Where(m => m.MapNo == item.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8).Sum(n => n.PayMoney) : 0
                });
            }
            return Data;
        }
        public DataStruct GetPayeeReceiptDataByMapno(int mapno)
        {
            GetData gd = new GetData();
            DataStruct Data = new DataStruct();
            List<Pay> total = DryDB.Pay.ToList();
            var resdata = (from summview in DryDB.SummaryView
                           join lasvermno in DryDB.LastVerOfMapNo on summview.MapNo equals lasvermno.MapNo
                           where summview.MapNo == mapno
                           select new
                           {
                               summview.MapNo,
                               summview.IANum,
                               summview.Name,
                               summview.IdNo,
                               summview.Addr,
                               summview.ApplyUnit,
                               summview.ApplyYear,
                               summview.Tel,
                               summview.Phone
                           }).OrderBy(m => m.IANum).FirstOrDefault();
            NumberToChinese ntoc = new NumberToChinese();
            if (resdata != null)
            {
                Data.IAName = gd.GetUnitName(resdata.ApplyUnit);
                Data.IANum = resdata.IANum;
                Data.Content = "此係推廣管路灌溉設施補助款，上款如數領訖無訛。";
                Data.ApplyYear = resdata.ApplyYear;
                Data.FarmerName = resdata.Name;
                Data.FarmerID = resdata.IdNo;
                Data.Addr = resdata.Addr.Replace(" ", "");
                Data.SubsidyFeeCNS = ntoc.GetChineseNumber(total.Any(m => m.MapNo == resdata.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8) ? total.Where(m => m.MapNo == resdata.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8).Sum(n => n.PayMoney) : 0) + "元整";
                Data.Payfee = total.Any(m => m.MapNo == resdata.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8) ? total.Where(m => m.MapNo == resdata.MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8).Sum(n => n.PayMoney) : 0;
                string phoneline = string.Empty;
                phoneline = resdata.Tel ?? string.Empty;
                if (! string.IsNullOrEmpty(resdata.Phone))
                {
                    if (string.IsNullOrEmpty(phoneline))
                    {
                        phoneline = resdata.Phone;
                    }else phoneline += $",{resdata.Phone}";
                }
                Data.Tel = phoneline;
                //Data.Tel = $"電話:{resdata.Tel ?? "".PadLeft(20)}行動:{resdata.Phone ?? "".PadLeft(20)}";
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
            public string Tel { get; set; }
        }
    }

}
