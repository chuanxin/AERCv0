using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dry.Models.ReportModules
{
    public class LiugongApplyListReport
    {
        public byte[] GetReport_LiugongApplyList(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
            List<DataStruct> data = new List<DataStruct>();
            DryEntities DryDB = new DryEntities();
            var oridata = (from list in DryDB.SummaryView
                           join cases in DryDB.Case on list.EventNo equals cases.EventNo
                           where list.ApplyUnit == unit && list.ApplyYear == year// && list.IANum >= IANumStart && list.IANum <= IANumEnd
                           select new
                           {
                               list.MapNo,
                               list.ApplyYear,
                               cases.CDate,
                               list.Name,
                               list.IdNo,
                               list.Addr,
                               list.Tel,
                               list.Phone
                           });
            if (year == 0)
            {
                oridata = (from list in DryDB.SummaryView
                           join cases in DryDB.Case on list.EventNo equals cases.EventNo
                           where list.ApplyUnit == unit //&& list.IANum <= IANumStart && list.IANum >= IANumEnd
                           select new
                           {
                               list.MapNo,
                               list.ApplyYear,
                               cases.CDate,
                               list.Name,
                               list.IdNo,
                               list.Addr,
                               list.Tel,
                               list.Phone
                           });
            }
            List<AERC.Models.Town> towns = new AERC.Models.CommonEntities().Town.ToList();
            
            
            List<LiugongReport_Town> farms = DryDB.LiugongReport_Town.ToList();
            List<FarmCrop> farmcrop = DryDB.FarmCrop.ToList();
            List<AERC.Models.Crop> crops = new AERC.Models.CommonCls.GetCrop().GetCropList();

            int count = 1;
            foreach (var item in oridata)
            {
                DataStruct list = new DataStruct();
                list.No += count++;
                list.ApplyYear = item.ApplyYear.ToString();
                list.ApplyDate = (item.CDate.Year - 1911).ToString() + item.CDate.ToString("/MM/dd");
                list.Name = item.Name;
                list.IdNo = item.IdNo;
                list.Addr = item.Addr.Replace(" ", "");
                list.Phone = item.Tel;
                list.CellPhone = item.Phone;
                list.Farm = new List<FarmList>();
                if (farms.Any(m => m.MapNo == item.MapNo))
                {
                    List<LiugongReport_Town> towndata = farms.Where(m => m.MapNo == item.MapNo).ToList();
                    foreach (var farmitem in towndata)
                    {
                        string cropname = "";
                        foreach (var cropitem in farmcrop.Where(m => m.FNo == farmitem.FNo).ToList())
                        {
                            cropname += crops.Where(m => m.Crop_Id == cropitem.CropCode).FirstOrDefault().Crop1 + " ";
                        }
                        FarmList farmlist = new FarmList();
                        farmlist.ApplySection = farmitem.Town;
                        farmlist.Section = farmitem.Section;
                        farmlist.SubSection = farmitem.Subsection;
                        farmlist.LandNo = farmitem.LandNo;
                        farmlist.LandType = farmitem.LandTypeCNS;
                        farmlist.Corp = cropname;
                        farmlist.FarmArea = farmitem.FarmArea;
                        farmlist.ApplyArea = farmitem.BuildArea;
                        list.Farm.Add(farmlist);
                    }

                }
                data.Add(list);
            }
            byte[] resdata = GetApplyListReport(data, sourcepath);
            return resdata;
        }
        private byte[] GetApplyListReport(List<DataStruct> Data, string sourcepath)
        {
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["清冊"];

            int count = Data.Sum(m => m.Farm.Count());
            int totalpage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(count / 18))) + 1;
            int pagecount = 1;
            int RowNo = 3;
            int RowCount = 1;
            sheet.Cells[21, 16].Value = "1/" + totalpage.ToString();
            for (int j = 0; j < totalpage; j++)
            {
                int startRow = j * 21 + 1;
                if (j == 0) { startRow = 22; }
                int endRow = startRow + 21 - 1;
                sheet.Cells[1, 1, 21, 16].Copy(sheet.Cells[startRow, 1, endRow, 16]);
                for (int row = 1; row <= 21; row++)
                {
                    sheet.Row(j * 21 + row).Height = sheet.Row(row).Height;
                }
                sheet.Cells[endRow, 16].Value = (pagecount++).ToString() + "/" + totalpage.ToString();
            }

            for (int i = 0; i < Data.Count; i++)
            {
                
                if (RowCount == 19)
                {
                    RowNo += 3;
                    RowCount = 1;
                }
                sheet.Cells[RowNo, 1].Value = Data[i].No;
                sheet.Cells[RowNo, 2].Value = Data[i].ApplyYear;
                sheet.Cells[RowNo, 3].Value = Data[i].ApplyDate;
                sheet.Cells[RowNo, 4].Value = Data[i].Name;
                sheet.Cells[RowNo, 5].Value = Data[i].IdNo;
                sheet.Cells[RowNo, 6].Value = Data[i].Addr;
                sheet.Cells[RowNo, 7].Value = Data[i].Phone;
                sheet.Cells[RowNo, 8].Value = Data[i].CellPhone;
                foreach (var item in Data[i].Farm)
                {
                    if (RowCount == 19)
                    {
                        RowNo += 3;
                        RowCount = 1;
                    }
                    sheet.Cells[RowNo, 9].Value = item.ApplySection;
                    sheet.Cells[RowNo, 10].Value = item.Section;
                    sheet.Cells[RowNo, 11].Value = item.SubSection;
                    sheet.Cells[RowNo, 12].Value = item.LandNo;
                    sheet.Cells[RowNo, 13].Value = item.LandType;
                    string Crop = "";
                    foreach (var cropitem in item.Corp)
                    {
                        Crop += cropitem + " ";
                    }
                    sheet.Cells[RowNo, 14].Value = Crop;
                    sheet.Cells[RowNo, 15].Value = item.FarmArea;
                    sheet.Cells[RowNo, 16].Value = item.ApplyArea;
                    RowNo++;
                    RowCount++;
                }
                //sheet.Cells[RowNo, 2].Value = data[i].Name;
                if (Data[i].Farm.Count <= 0) { RowNo++; RowCount++; }

            }

            byte[] file = excel.GetAsByteArray();


            return file;
        }
        private ExcelWorksheet SetApplyListReportData(ExcelWorksheet sh, DataStruct Data, int count)
        {
            int number = ((count - 1) * 40);
            //sh.Cells[number + 3, 5].Value = Data.No;

            //sh.Cells[number + 4, 5].Value = Data.ApplyYear;
            //sh.Cells[number + 3, 9].Value = Data.ApplyDate;
            //sh.Cells[number + 5, 5].Value = Data.Name;
            //sh.Cells[number + 6, 5].Value = Data.IdNo;
            //sh.Cells[number + 7, 5].Value = Data.AcceptanceDate_Year + "年";
            //sh.Cells[number + 7, 6].Value = Data.AcceptanceDate_Month + "月";
            //sh.Cells[number + 7, 7].Value = Data.AcceptanceDate_Day + "日";
            //sh.Cells[number + 11, 5].Value = Data.Area + "公頃";
            //sh.Cells[number + 12, 5].Value = " 管長(L1)：   " + Data.L1 + "   公尺      管徑(D1)：   " + Data.D1 + "   吋";
            //sh.Cells[number + 13, 5].Value = " 管長(L2)：   " + Data.L2 + "   公尺      管徑(D2)：   " + Data.D2 + "   吋";
            //sh.Cells[number + 15, 5].Value = "   " + Data.SS + "     公尺";
            //sh.Cells[number + 16, 5].Value = "   " + Data.SL + "     公尺";
            //sh.Cells[number + 19, 5].Value = "名稱：調節控制設施   規格：             數量：       全";
            //sh.Cells[number + 25, 7].Value = Data.AcceptanceResults;
            return sh;
        }
        public class DataStruct
        {
            public string No { get; set; }
            public string ApplyYear { get; set; }
            public string ApplyDate { get; set; }
            public string Name { get; set; }
            public string IdNo { get; set; }
            public string Addr { get; set; }
            public string Phone { get; set; }
            public string CellPhone { get; set; }
            public List<FarmList> Farm { get; set; }

        }
        public class FarmList
        {
            public string ApplySection { get; set; }
            public string Section { get; set; }
            public string SubSection { get; set; }
            public string LandNo { get; set; }
            public string LandType { get; set; }
            public string Corp { get; set; }
            public double FarmArea { get; set; }
            public double ApplyArea { get; set; }
        }
    }
}
