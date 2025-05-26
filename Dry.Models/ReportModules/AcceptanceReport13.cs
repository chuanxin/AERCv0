using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dry.Models.Service;

namespace Dry.Models.ReportModules
{
    public class AcceptanceReport13
    {
        public byte[] GetReport_AcceptanceReport(short unitcode, int years, int IANumStart, int IANumEnd, string sourcepath)
        {
            
            List<AcceptanceReportDBService.DataStruct> data = new List<AcceptanceReportDBService.DataStruct>();
            data = new AcceptanceReportDBService().GetReportData(unitcode,years,IANumStart,IANumEnd);
            byte[] resdata = GetAcceptanceReport(data, sourcepath);
            return resdata;
        }
        public byte[] GetAcceptanceReport(List<AcceptanceReportDBService.DataStruct> Data, string sourcepath)
        {
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["驗收報告"];

            int count = Data.Count;

            for (int i = 0; i < count; i++)
            {
                int startRow = i * 39 + 1;
                if (i == 0) { startRow = 40; }
                int endRow = startRow + 39 - 1;
                if (count != 1)
                {
                    sheet.Cells[1, 1, 39, 11].Copy(sheet.Cells[startRow, 1, endRow, 11]);
                }

                for (int row = 1; row <= 39; row++)
                {
                    //sheet.Cells["A" + row + ":K" + row].Copy(sheet.Cells["A" + (i * 40 + row) + ":K" + (i * 40 + row)]);
                    sheet.Row(i * 39 + row).Height = sheet.Row(row).Height;

                }
                //sheet.Cells[i * 18, 2, i * 18  + 4, 4].Merge = true;
                sheet = SetAcceptanceReportData(sheet, Data[i], i+1 );//DB

            }

            byte[] file = excel.GetAsByteArray();


            return file;
        }
        public ExcelWorksheet SetAcceptanceReportData(ExcelWorksheet sh, AcceptanceReportDBService.DataStruct Data, int count)
        {
            int number = ((count - 1) * 39);
            sh.Cells[number + 3, 5].Value = Data.FarmerName;

            sh.Cells[number + 4, 5].Value = Data.SectionName;
            sh.Cells[number + 3, 9].Value = Data.IANum;
            sh.Cells[number + 5, 5].Value = Data.Area;
            sh.Cells[number + 6, 5].Value = Data.FacType;
            sh.Cells[number + 7, 5].Value = Data.AcceptanceDate_Year + "年";
            sh.Cells[number + 7, 6].Value = Data.AcceptanceDate_Month + "月";
            sh.Cells[number + 7, 7].Value = Data.AcceptanceDate_Day + "日";
            //sh.Cells[number + 11, 5].Value = Data.Area + "公頃";
            //sh.Cells[number + 12, 5].Value = " 管長(L1)：   " + Data.L1 + "   公尺      管徑(D1)：   " + Data.D1 + "   吋";
            //sh.Cells[number + 13, 5].Value = " 管長(L2)：   " + Data.L2 + "   公尺      管徑(D2)：   " + Data.D2 + "   吋";
            //sh.Cells[number + 15, 5].Value = "   " + Data.SS + "     公尺";
            //sh.Cells[number + 16, 5].Value = "   " + Data.SL + "     公尺";
            //sh.Cells[number + 19, 5].Value = "名稱：調節控制設施   規格：             數量：       全";
            sh.Cells[number + 25, 7].Value = Data.AcceptanceResults;
            return sh;
        }
 /*public class DataStruct
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
    }*/
    }

   
   
}
