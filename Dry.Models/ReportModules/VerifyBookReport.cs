using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Dry.Models.Service;

namespace Dry.Models.ReportModules
{
    public class VerifyBookReport
    {
        private CommonCls.GetData getDataCls = new CommonCls.GetData();

        public byte[] CreateExcel(BudgetBookView data)
        {
            
            

            string sample_Path;
            switch (data.ApplyUnit)
            {
                case 9:
                    sample_Path = @"~/ReportSample/VerifyBook_9.xlsx";
                    break;
                default:
                    sample_Path = @"~/ReportSample/VerifyBook_10.xlsx";
                    break;
            }
            
            string FileName = data.ApplyY.ToString() + " - " + data.IANum.ToString() + " - 驗收報告書.xlsx";
                       
            
            

            FileStream fs = new FileStream(System.Web.HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);

            ExcelWorksheet sheet = excel.Workbook.Worksheets["驗收報告書"];
            int irow = 3;
            sheet.Cells[irow, 5].Value = data.Name;
            sheet.Cells[irow, 9].Value = data.IANum;
            
            
            irow += 1;         
            sheet.Cells[irow, 5].Value = data.Farm.FirstOrDefault().full_sectName + "地號：" +  data.Farm.FirstOrDefault().LandNo + "等" + data.Farm.Count + "筆";
            irow += 1;
            sheet.Cells[irow, 5].Value = (data.BuildArea /10000).ToString() + "公頃";
            irow += 1;
            sheet.Cells[irow, 5].Value = data.EndType;
            //irow += 1;
            //sheet.Cells[irow, 5].Value = Data.VDate.ToString("yyyy/MM/dd");
            //irow += 1;
            //sheet.Cells[irow, 5].Value = Data.BuildArea;

            irow = 29;
            sheet.Cells[irow, 9].Value = data.GovPay_Pay ;
            irow += 1;
            sheet.Cells[irow, 9].Value = data.GoldPay;
            irow += 1;
            sheet.Cells[irow, 9].Value = data.LiuPay;

            fs.Close();
            byte[] file = excel.GetAsByteArray();
            return file;

        }
    }
}
