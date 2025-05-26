using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.Service
{
    public class AcceptanceReportDBService
    {
        DryEntities DryDB = new DryEntities();
        public List<DataStruct> GetReportData(short unit, int year, int IANumStart, int IANumEnd)
        {
            List<DataStruct> data = new List<DataStruct>();
            //DryEntities DryDB = new DryEntities();

            List<AERC.Models.Town> towns = new AERC.Models.CommonEntities().Town.ToList();
            List<AcceptanceReportData> oridata = DryDB.AcceptanceReportData.Where(m => m.ApplyUnit == unit && m.ApplyYear == year && m.IANum >= IANumStart && m.IANum <= IANumEnd).OrderBy(m => m.IANum).ToList();
            List<EndTypeList> endTypeList = DryDB.EndTypeList.ToList();
            List<FacTypeList> facTypeList = DryDB.FacTypeList.ToList();
            List<Farm> farms = DryDB.Farm.ToList();
            foreach (var item in oridata)
            {
                DataStruct list = new DataStruct();
                int MapNo = item.MapNo.Value;
                list.FarmerName = item.Name;
                var farmData = (from farm in farms
                                join landdata in DryDB.LandData on farm.Section equals landdata.Section_Id
                                where farm.MapNo == item.MapNo
                                select new
                                {
                                    farm.FNo,
                                    landdata.City,
                                    landdata.Town,
                                    landdata.Section,
                                    landdata.Subsection,
                                    farm.LandNo,
                                    farm.BuildArea
                                }).OrderByDescending(m => m.Section).ThenBy(m => m.LandNo).ToList();
                if (farmData.Count > 0)
                {
                    list.SectionName = farmData.FirstOrDefault().City +
                        farmData.FirstOrDefault().Town +
                        farmData.FirstOrDefault().Section + "段" +
                        (farmData.FirstOrDefault().Subsection == "" ? "" : "-" + farmData.FirstOrDefault().Subsection + "小段") + farmData.FirstOrDefault().LandNo + "號" +
                        ",等" + farmData.Count + "筆";
                    List<EndType> endType = DryDB.EndType.Where(m => m.MapNo == MapNo).ToList();
                    foreach (var items in endType)
                    {
                        if (items.EndTypeCode == 1)
                        {
                            list.FacType += endTypeList.Find(m => m.EndType == items.EndTypeCode).EndTypeCNS + " ";
                        }
                        else
                        {
                            list.FacType += facTypeList.Find(m => m.FacType == items.FacType).FTpeCNS + endTypeList.Find(m => m.EndType == items.EndTypeCode).EndTypeCNS + " ";
                        }
                    }
                    if (string.IsNullOrEmpty(list.FacType)) list.FacType = "其它";
                    list.Area = (Math.Round(farmData.Sum(m => m.BuildArea) / 10000, 4, MidpointRounding.AwayFromZero)).ToString() + "公頃";
                }
                if (item.VDate.HasValue)
                {
                    list.AcceptanceDate_Year = (item.VDate.Value.Year - 1911).ToString();
                    list.AcceptanceDate_Month = item.VDate.Value.Month.ToString();
                    list.AcceptanceDate_Day = item.VDate.Value.Day.ToString();
                }
                list.IANum = item.IANum.ToString();
                list.AcceptanceResults = DryDB.Pay.Any(m => m.MapNo == MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8) ? string.Format("{0:N0}", DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8).Sum(n => n.PayMoney)) : "";
                data.Add(list);
                
            }
            return data;
        }

        public DataStruct GetReportDataByMapno(int mapno)
        {
            List<EndTypeList> endTypeList = DryDB.EndTypeList.ToList();
            List<FacTypeList> facTypeList = DryDB.FacTypeList.ToList();
            var oridata = DryDB.AcceptanceReportData.Where(m => m.MapNo == mapno).FirstOrDefault();
            DataStruct list = new DataStruct();
            //int MapNo = item.MapNo.Value;
            list.FarmerName = oridata.Name;
            var farmData = (from farm in DryDB.Farm
                            join landdata in DryDB.LandData on farm.Section equals landdata.Section_Id
                            where farm.MapNo == mapno
                            select new
                            {
                                farm.FNo,
                                landdata.City,
                                landdata.Town,
                                landdata.Section,
                                landdata.Subsection,
                                farm.LandNo,
                                farm.BuildArea
                            }).OrderByDescending(m => m.Section).ThenBy(m => m.LandNo).ToList();
            if (farmData.Count > 0)
            {
                list.SectionName = farmData.FirstOrDefault().City +
                    farmData.FirstOrDefault().Town +
                    farmData.FirstOrDefault().Section + "段" +
                    (farmData.FirstOrDefault().Subsection == "" ? "" : "-" + farmData.FirstOrDefault().Subsection + "小段") + farmData.FirstOrDefault().LandNo + "號" +
                    ",等" + farmData.Count + "筆";
                List<EndType> endType = DryDB.EndType.Where(m => m.MapNo == mapno).ToList();
                foreach (var items in endType)
                {
                    if (items.EndTypeCode == 1)
                    {
                        list.FacType += endTypeList.Find(m => m.EndType == items.EndTypeCode).EndTypeCNS + " ";
                    }
                    else
                    {
                        list.FacType += facTypeList.Find(m => m.FacType == items.FacType).FTpeCNS + endTypeList.Find(m => m.EndType == items.EndTypeCode).EndTypeCNS + " ";
                    }
                }
                if (string.IsNullOrEmpty(list.FacType)) list.FacType = "其它";
                list.Area = (Math.Round(farmData.Sum(m => m.BuildArea) / 10000, 4, MidpointRounding.AwayFromZero)).ToString() + "公頃";
            }
            if (oridata.VDate.HasValue)
            {
                list.AcceptanceDate_Year = (oridata.VDate.Value.Year - 1911).ToString();
                list.AcceptanceDate_Month = oridata.VDate.Value.Month.ToString();
                list.AcceptanceDate_Day = oridata.VDate.Value.Day.ToString();
            }
            list.IANum = oridata.IANum.ToString();
            list.AcceptanceResults = DryDB.Pay.Any(m => m.MapNo == mapno && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8) ? string.Format("{0:N0}", DryDB.Pay.Where(m => m.MapNo == mapno && m.ItemCode != 2 && m.ItemCode != 7 && m.ItemCode != 8).Sum(n => n.PayMoney)) : "";
            return list;
        }
        public class DataStruct
        {
            public string FarmerName { get; set; }
            public string IANum { get; set; }
            public string SectionName { get; set; }
            public string Area { get; set; }
            public string FacType { get; set; }
            public string AcceptanceDate_Year { get; set; }
            public string AcceptanceDate_Month { get; set; }
            public string AcceptanceDate_Day { get; set; }
            public string AcceptanceResults { get; set; }
        }
    }
}
