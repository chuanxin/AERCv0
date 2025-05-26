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

namespace Dry.Models.ReportModules
{
    //private DryEntities DryDB = new DryEntities();
    
    public class DesignReceiptReport15
    {
        DryEntities DryDB = new DryEntities();
        public byte[] GetReport_DesignReceiptReport(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
            List<DesignReceiptDBService.DataStruct> Data = new List<DesignReceiptDBService.DataStruct>();
            Data = new DesignReceiptDBService().GetPayeeReceiptData(unit, year, IANumStart, IANumEnd);
            byte[] returnData = PayeeReceiptReport(Data, sourcepath);
            return returnData;
        }
        private byte[] PayeeReceiptReport(List<DesignReceiptDBService.DataStruct> dt, string sourcepath)
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

            SetData(wk, wksheet, dt, reportlength);
            MemoryStream files = new MemoryStream();
            wk.Write(files);
            files.Close();
            return files.ToArray();
        }
        private HSSFSheet SetData(HSSFWorkbook wk, HSSFSheet sh, List<DesignReceiptDBService.DataStruct> dt, int reportlength)
        {
            HSSFCellStyle cs = wk.CreateCellStyle() as HSSFCellStyle;
            HSSFFont font1 = wk.CreateFont() as HSSFFont;
            int count = dt.Count;
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                
                sh.GetRow(2 + j).GetCell(7).SetCellValue(dt[i].IANum);
                sh.GetRow(4 + j).GetCell(2).SetCellValue(dt[i].SubsidyFeeCNS);
                //sh.GetRow(6 + j).GetCell(1).SetCellValue(dt[i].Content);
                sh.GetRow(6 + j).GetCell(2).SetCellValue(dt[i].ApplyYear);
                sh.GetRow(6 + j).GetCell(7).SetCellValue(dt[i].FarmerName);
                sh.GetRow(10 + j).GetCell(2).SetCellValue(dt[i].IAName);
                sh.GetRow(13 + j).GetCell(2).SetCellValue(dt[i].Content);
                //sh.GetRow(17 + j).GetCell(2).SetCellValue(dt[i].Addr);
                //sh.GetRow(19 + j).GetCell(3).SetCellValue(dt[i].FarmerID);

                j = j + reportlength;
            }

            return sh;
        }
       
    }
        
    
}
