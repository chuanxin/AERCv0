using CusNPOI;
using Dry.Models.CommonCls;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dry.Models.Service;
using Dry.Models.ReportModules;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Dry.Models.ReportModules
{
    public class PayeeReceipt12
    {
        DryEntities DryDB = new DryEntities();
        public byte[] GetReport_PayeeReceipt(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
            
            List<PayeeReceiptDBService.DataStruct> Data = new List<PayeeReceiptDBService.DataStruct>();
            Data = new PayeeReceiptDBService().GetPayeeReceiptData(unit, year, IANumStart, IANumEnd);
            byte[] returnData = PayeeReceiptReport12(Data, sourcepath);
            return returnData;
        }

        public byte[] GetReport_PayeeReceipt12(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {

            List<PayeeReceiptDBService.DataStruct> Data = new List<PayeeReceiptDBService.DataStruct>();
            Data = new PayeeReceiptDBService().GetPayeeReceiptData(unit, year, IANumStart, IANumEnd);
            //byte[] returnData = PayeeReceiptReport(Data, sourcepath);
            byte[] resdata = GetDesignReceiptReportData12(Data, sourcepath);
            //DesignReceiptView dv = new DesignReceiptView();
            //dv.DYears = item.ApplyYear.ToString();
            //dv.DesignPrice = item.PayMoney.ToString();
            //dv.DNo = item.IANum.ToString();
            //dv.ChineseMoney = new NumberToChinese().GetChineseNumber(item.PayMoney);
            //data.Add(dv);

            return resdata;
        }

        public byte[] GetDesignReceiptReportData12(List<PayeeReceiptDBService.DataStruct> Data, string sourcepath)
        {
            #region basic Data
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["設計費收據"];

            int count = Data.Count; 
            for (int i = 0; i < count; i++)
            {
                int number = (i * 24);
                if (number != 0)
                {
                    sheet.Select("A1:K24");
                    sheet.SelectedRange.Copy(sheet.Cells["A" + (number + 1).ToString() + ":K" + (number + 1).ToString()]);
                    //for (int j = 1; j <= 24; j++)
                    //{
                    //    sheet.Cells["A" + j.ToString() + ":K" + j.ToString()].Copy(sheet.Cells["A" + (number + j).ToString() + ":K" + (number + j).ToString()]);
                    //}
                }
            }
            //for (int i = 1; i <= count; i++)
            int itemindex = 1;
            foreach (var item in Data)
            {
                sheet = SetDesignReceiptData12(sheet, item, itemindex);//DB
                itemindex += 1;
            }
            byte[] file = excel.GetAsByteArray();

            #endregion

            return file;
        }
        #region 設定加入設計費收據表
        public ExcelWorksheet SetDesignReceiptData12(ExcelWorksheet sh, PayeeReceiptDBService.DataStruct Data, int index)
        {
            int number = ((index - 1) * 24);

            sh.Row(number + 1).Height = 37;
            sh.Row(number + 2).Height = 33;
            sh.Row(number + 3).Height = 54;
            //sh.Row(number + 4).Height = 12;
            sh.Row(number + 5).Height = 50;
            sh.Row(number + 6).Height = 50;
            sh.Row(number + 7).Height = 50;
            //sh.Row(number + 8).Height = 12;
            //sh.Row(number + 9).Height = 12;
            sh.Row(number + 10).Height = 37;
            //sh.Row(number + 11).Height = 12;
            sh.Row(number + 12).Height = 24;
            sh.Row(number + 13).Height = 24;
            //sh.Row(number + 14).Height = 12;
            sh.Row(number + 15).Height = 32;
            sh.Row(number + 16).Height = 32;
            sh.Row(number + 17).Height = 32;
            sh.Row(number + 18).Height = 47;
            sh.Row(number + 19).Height = 47;
            sh.Row(number + 20).Height = 47;
            sh.Row(number + 21).Height = 47;
            sh.Row(number + 22).Height = 47;
            //sh.Row(number + 23).Height = 12;
            sh.Row(number + 24).Height = 34;

            string money = Data.Payfee.ToString();
            string chmoney = new NumberToChinese().GetChineseNumber(int.Parse(money.ToString().Trim()));
            string[] strs = new string[money.Length];
            for (int i = 0; i < money.Length; i++)
            {
                strs[i] = money.Substring(money.Length - 1 - i, 1);
                sh.Cells[number + 3, 9 - i].Value = strs[i].ToString();
            }
            #region
            #endregion
            #region
            //sh.Cells[number + 12, 7].Value = Data.ApplyYear;//"105";//年度
            sh.Cells[number + 15, 2].Value = chmoney + "元整"; //"壹仟参佰貳拾柒" + "元整";//Data.ChineseMoney
            sh.Cells[number + 15, 10].Style.Numberformat.Format = "#,##0";
            sh.Cells[number + 15, 10].Value = Data.Payfee;//int.Parse(money);
            sh.Cells[number + 18, 2].Value = Data.IANum; //"9002";
            sh.Cells[number + 19, 2].Value = Data.FarmerName; 
            sh.Cells[number + 20, 2].Value = Data.Addr; 
            sh.Cells[number + 21, 3].Value = Data.FarmerID; 
            sh.Cells[number + 24, 4].Value = Data.ApplyYear;  //"105";
            #endregion
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion

       


        private byte[] PayeeReceiptReport12(List<PayeeReceiptDBService.DataStruct> dt, string sourcepath)
        {
            CusCopyRow cus = new CusCopyRow();
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook wk = new HSSFWorkbook(fs);
            int count = dt.Count;
            int reportlength = 39;

            HSSFSheet wksheet = wk.GetSheetAt(0) as HSSFSheet;
            
            for (int j = 1; j < count; j++)
            {
                for (int row = 0; row < 30; row++)
                {
                    
                    cus.CopyRow(wksheet, row, reportlength * j + row);
                }
            }

            SetData12(wk, wksheet, dt, reportlength);
            MemoryStream files = new MemoryStream();
            wk.Write(files);
            files.Close();
            return files.ToArray();
        }
        private HSSFSheet SetData12(HSSFWorkbook wk, HSSFSheet sh, List<PayeeReceiptDBService.DataStruct> dt, int reportlength)
        {
            HSSFCellStyle cs = wk.CreateCellStyle() as HSSFCellStyle;
            HSSFFont font1 = wk.CreateFont() as HSSFFont;
            int count = dt.Count;
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                
                sh.GetRow(2 + j).GetCell(7).SetCellValue(dt[i].IANum);
                sh.GetRow(4 + j).GetCell(2).SetCellValue(dt[i].SubsidyFeeCNS);
                sh.GetRow(6 + j).GetCell(1).SetCellValue(dt[i].Content);
                sh.GetRow(9 + j).GetCell(2).SetCellValue(dt[i].IAName);
                sh.GetRow(13 + j).GetCell(2).SetCellValue(dt[i].FarmerName);
                sh.GetRow(17 + j).GetCell(2).SetCellValue(dt[i].Addr);
                sh.GetRow(19 + j).GetCell(3).SetCellValue(dt[i].FarmerID);

                j = j + reportlength;
            }

            return sh;
        }
        /*public class DataStruct
        {
            public int ApplyYear { get; set; }
            public string Content { get; set; }
            public string IAName { get; set; }
            public int IANum { get; set; }
            public string FarmerName { get; set; }
            public string FarmerID { get; set; }
            public string Addr { get; set; }
            public string SubsidyFeeCNS { get; set; }

        }*/
    }
}
