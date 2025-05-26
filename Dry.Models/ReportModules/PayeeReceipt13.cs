using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.Service;
using CusNPOI;
using System.IO;
using NPOI.HSSF.UserModel;
using System.Web;



namespace Dry.Models.ReportModules
{
    public class PayeeReceipt13
    {
        DryEntities DryDB = new DryEntities();
        public byte[] GetReport_PayeeReceipt(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
           
            List<PayeeReceiptDBService.DataStruct> Data = new List<PayeeReceiptDBService.DataStruct>();
            Data = new PayeeReceiptDBService().GetPayeeReceiptData(unit, year, IANumStart, IANumEnd);
            byte[] returnData = PayeeReceiptReport(Data, sourcepath);
            return returnData;
        }
        private byte[] PayeeReceiptReport(List<PayeeReceiptDBService.DataStruct> dt, string sourcepath)
        {
            CusCopyRow cus = new CusCopyRow();
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook wk = new HSSFWorkbook(fs);
            int count = dt.Count;
            int reportlength = 27;

            HSSFSheet wksheet = wk.GetSheetAt(0) as HSSFSheet;
            
            for (int j = 1; j < count; j++)
            {
                for (int row = 0; row < 26; row++)
                {
                    
                    cus.CopyRow(wksheet, row, reportlength * j + row);
                }
            }

            SetData(wk, wksheet, dt, reportlength);
            MemoryStream files = new MemoryStream();
            wk.Write(files);
            files.Close();
            return files.ToArray();
        }
        private HSSFSheet SetData(HSSFWorkbook wk, HSSFSheet sh, List<PayeeReceiptDBService.DataStruct> dt, int reportlength)
        {
            HSSFCellStyle cs = wk.CreateCellStyle() as HSSFCellStyle;
            HSSFFont font1 = wk.CreateFont() as HSSFFont;
            int count = dt.Count;
            int j = 0;
            for (int i = 0; i < count; i++)
            {
               
                sh.GetRow(6 + j).GetCell(8).SetCellValue(dt[i].IANum);
                sh.GetRow(7 + j).GetCell(2).SetCellValue(dt[i].SubsidyFeeCNS);
                //sh.GetRow(6 + j).GetCell(1).SetCellValue(dt[i].Content);
                sh.GetRow(13 + j).GetCell(2).SetCellValue("臺灣" + dt[i].IAName);
                sh.GetRow(16 + j).GetCell(2).SetCellValue(dt[i].FarmerName);
                sh.GetRow(16 + j).GetCell(16).SetCellValue(dt[i].FarmerName);
                sh.GetRow(20 + j).GetCell(2).SetCellValue(dt[i].Addr);
                sh.GetRow(22 + j).GetCell(3).SetCellValue(dt[i].FarmerID);

                string paystring = dt[i].Payfee.ToString();
                //paystring = "100523";
                int numcolindex = 0;
                for (int k = paystring.Length ; k > 0; k--)
                {
                    sh.GetRow(13 + j).GetCell(19 - numcolindex).SetCellValue(paystring.Substring( k - 1 , 1));
                    numcolindex++;
                }

                j = j + reportlength;
            }

            return sh;
        }
    }
}
