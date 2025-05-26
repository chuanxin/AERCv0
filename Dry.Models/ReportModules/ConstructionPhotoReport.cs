using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Dry.Models.ReportModules
{
    public class ConstructionPhotoReport
    {
        public byte[] GetReport_AcceptanceReport(short unit, int year, int IANumStart, int IANumEnd, string sourcepath, string imagepath)
        {
            List<DataStruct> data = new List<DataStruct>();
            List<DataStruct> data1 = new List<DataStruct>();
            DryEntities DryDB = new DryEntities();

            List<AERC.Models.Town> towns = new AERC.Models.CommonEntities().Town.ToList();
            var oridata = DryDB.SummaryView.Where(m => m.ApplyUnit == unit && m.ApplyYear == year && m.IANum >= IANumStart && m.IANum <= IANumEnd).OrderBy(m => m.IANum).ToList();
            List<Farm> farms = DryDB.Farm.ToList();
            foreach (var item in oridata)
            {
                DataStruct list = new DataStruct();
                int MapNo = item.MapNo.Value;
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
                                }).OrderByDescending(m => m.Section).ThenBy(m => m.LandNo).ToList();//.OrderByDescending(m => m.Section).ToList();
                if (farmData.Count > 0)
                {
                    list.SectionName = farmData.FirstOrDefault().City +
                        farmData.FirstOrDefault().Town +
                        farmData.FirstOrDefault().Section + "段" +
                        (farmData.FirstOrDefault().Subsection == "" ? "" : "-" + farmData.FirstOrDefault().Subsection + "小段") + farmData.FirstOrDefault().LandNo + "號" +
                        ",等" + farmData.Count + "筆";
                }
                list.IANum = item.IANum.ToString();
                list.FarmerName = item.Name;
                list.MapNo = MapNo;
                for (int i = 0; i < farmData.Count; i++)
                {
                    DataStruct list1 = new DataStruct();
                    list1.IANum = item.IANum.ToString();
                    list1.FarmerName = item.Name;
                    list1.MapNo = MapNo;
                    list1.SectionName = farmData[i].City + farmData[i].Town + farmData[i].Section + "段" + 
                        (farmData[i].Subsection == "" ? "" : "-" + farmData.FirstOrDefault().Subsection + "小段") +
                        farmData[i].LandNo + "號";
                    data1.Add(list1);
                }
                
                data.Add(list);
            }

            byte[] resdata = GetAcceptanceReport(data, sourcepath, imagepath,data1);
            return resdata;
        }
        public byte[] GetAcceptanceReport(List<DataStruct> Data, string sourcepath, string imagepath, List<DataStruct> Data1)
        {
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["前後動蓄引"];
            ExcelWorksheet sheet1 = excel.Workbook.Worksheets["前後動"];
            ExcelWorksheet sheet2 = excel.Workbook.Worksheets["前後蓄"];
            ExcelWorksheet sheet3 = excel.Workbook.Worksheets["前後動蓄"];
            ExcelWorksheet sheet4 = excel.Workbook.Worksheets["前後蓄引"];
            ExcelWorksheet sheet5 = excel.Workbook.Worksheets["前後動引"];
            ExcelWorksheet sheet6 = excel.Workbook.Worksheets["前後引"];
            ExcelWorksheet sheet7 = excel.Workbook.Worksheets["前後"];
            ExcelWorksheet sheet8 = excel.Workbook.Worksheets["管路"];
            
            int count = Data.Count;

            for (int i = 0; i < count; i++)
            {
                int startRow = i * 49 + 1;
                //if (i == 0) { startRow = 49 + 1; }
                int endRow = startRow + 49 - 1;
                sheet.Cells[1, 1, 49, 11].Copy(sheet.Cells[startRow, 1, endRow, 11]);
                sheet1.Cells[1, 1, 49, 11].Copy(sheet1.Cells[startRow, 1, endRow, 11]);
                sheet2.Cells[1, 1, 49, 11].Copy(sheet2.Cells[startRow, 1, endRow, 11]);
                sheet3.Cells[1, 1, 49, 11].Copy(sheet3.Cells[startRow, 1, endRow, 11]);
                sheet4.Cells[1, 1, 49, 11].Copy(sheet4.Cells[startRow, 1, endRow, 11]);
                sheet5.Cells[1, 1, 49, 11].Copy(sheet5.Cells[startRow, 1, endRow, 11]);
                sheet6.Cells[1, 1, 49, 11].Copy(sheet6.Cells[startRow, 1, endRow, 11]);
                sheet7.Cells[1, 1, 49, 11].Copy(sheet7.Cells[startRow, 1, endRow, 11]);
                for (int row = 1; row <= 49; row++)
                {
                    //sheet.Cells["A" + row + ":K" + row].Copy(sheet.Cells["A" + (i * 51 + row) + ":K" + (i * 51 + row)]);
                    sheet.Row(i * 49 + row).Height = sheet.Row(row).Height;
                    sheet1.Row(i * 49 + row).Height = sheet1.Row(row).Height;
                    sheet2.Row(i * 49 + row).Height = sheet2.Row(row).Height;
                    sheet3.Row(i * 49 + row).Height = sheet3.Row(row).Height;
                    sheet4.Row(i * 49 + row).Height = sheet4.Row(row).Height;
                    sheet5.Row(i * 49 + row).Height = sheet5.Row(row).Height;
                    sheet6.Row(i * 49 + row).Height = sheet6.Row(row).Height;
                    sheet7.Row(i * 49 + row).Height = sheet7.Row(row).Height;
                }
                //sheet.Cells[i * 18, 2, i * 18  + 4, 4].Merge = true;

                sheet = SetAcceptanceReportData(sheet, Data[i], i + 1, Data[i].MapNo, imagepath);//DB
                sheet1 = SetAcceptanceReportData(sheet1, Data[i], i + 1, Data[i].MapNo, imagepath);//DB
                sheet2 = SetAcceptanceReportData(sheet2, Data[i], i + 1, Data[i].MapNo, imagepath);//DB
                sheet3 = SetAcceptanceReportData(sheet3, Data[i], i + 1, Data[i].MapNo, imagepath);//DB
                sheet4 = SetAcceptanceReportData(sheet4, Data[i], i + 1, Data[i].MapNo, imagepath);//DB
                sheet5 = SetAcceptanceReportData(sheet5, Data[i], i + 1, Data[i].MapNo, imagepath);//DB
                sheet6 = SetAcceptanceReportData(sheet6, Data[i], i + 1, Data[i].MapNo, imagepath);//DB
                sheet7 = SetAcceptanceReportData(sheet7, Data[i], i + 1, Data[i].MapNo, imagepath);//DB
            }

            for (int i = 0; i < Data1.Count; i++)
            {
                int startRow = i * 49 + 1;
                int endRow = startRow + 49 - 1;
                sheet8.Cells[1, 1, 49, 11].Copy(sheet8.Cells[startRow, 1, endRow, 11]);
                for (int row = 1; row <= 49; row++)
                {
                    sheet8.Row(i * 49 + row).Height = sheet8.Row(row).Height;
                }
                sheet8 = SetAcceptanceReportData(sheet8, Data1[i], i + 1, Data1[i].MapNo, imagepath);
            }
                byte[] file = excel.GetAsByteArray();


            return file;
        }
        public ExcelWorksheet SetAcceptanceReportData(ExcelWorksheet sh, DataStruct Data, int count, int MapNo, string imagepath)
        {
            DryEntities DryDB = new DryEntities();
            int number = ((count - 1) * 49);

            sh.Cells[number + 1, 4].Value = Data.IANum;
            sh.Cells[number + 1, 7].Value = Data.FarmerName;
            sh.Cells[number + 2, 4].Value = Data.SectionName;

            return sh;
        }
        public class DataStruct
        {
            public int MapNo { get; set; }
            public string IANum { get; set; }
            public string FarmerName { get; set; }
            public string SectionName { get; set; }
            public Image Before { get; set; }
            public Image After { get; set; }
        }
    }
    
}
