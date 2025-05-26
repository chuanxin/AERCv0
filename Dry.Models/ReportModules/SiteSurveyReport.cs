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

namespace Dry.Models.ReportModules
{
    public class SiteSurveyReport
    {
        DryEntities DryDB = new DryEntities();
        public byte[] GetReport_SiteSurvey(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
            GetData gd = new GetData();
            List<DataStruct> Data = new List<DataStruct>();

            byte[] returnData = SiteSurvey_Report(Data, sourcepath);
            return returnData;
        }
        private byte[] SiteSurvey_Report(List<DataStruct> dt, string sourcepath)
        {
            List<DataStruct> dt0 = new List<DataStruct>();
            List<DataStruct> dt1 = new List<DataStruct>();


            CusCopyRow cus = new CusCopyRow();
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook wk = new HSSFWorkbook(fs);
            int reportlength = 35;
            int HoldCount = dt0.Count;
            int DripCount = dt1.Count;

            HSSFSheet wksheet = wk.GetSheetAt(0) as HSSFSheet;
            for (int j = 1; j < HoldCount; j++)
            {
                for (int row = 0; row < 35; row++)
                {
                    cus.CopyRow(wksheet, row, reportlength * j + row);
                }
            }
            SetData(wk, wksheet, dt0, reportlength, 0);
            wksheet = wk.GetSheetAt(1) as HSSFSheet;
            for (int j = 1; j < DripCount; j++)
            {
                for (int row = 0; row < 35; row++)
                {
                    cus.CopyRow(wksheet, row, reportlength * j + row);
                }
            }

            SetData(wk, wksheet, dt1, reportlength, 1);
            MemoryStream files = new MemoryStream();
            wk.Write(files);
            files.Close();
            return files.ToArray();
        }
        private HSSFSheet SetData(HSSFWorkbook wk, HSSFSheet sh, List<DataStruct> dt, int reportlength, byte SheetNo)
        {
            int count = dt.Count;
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                
                sh.GetRow(0 + j).GetCell(2).SetCellValue(104);
                sh.GetRow(0 + j).GetCell(14).SetCellValue("102" + "\r\n" + "李旭庭");
                sh.GetRow(0 + j).GetCell(14).CellStyle.WrapText = true;
                sh.GetRow(1 + j).GetCell(2).SetCellValue("李旭庭");
                sh.GetRow(1 + j).GetCell(4).SetCellValue("古魯社段0597-0000地號");
                sh.GetRow(1 + j).GetCell(12).SetCellValue("0.22");
                sh.GetRow(2 + j).GetCell(3).SetCellValue("PSA-211穿孔管");
                sh.GetRow(2 + j).GetCell(8).SetCellValue("甘藷");

                j = j + reportlength;
            }

            return sh;
        }

        public class DataStruct
        {
            public int ApplyYear { get; set; }
            public string FarmerName { get; set; }
            public string SectionName { get; set; }
            public double Area { get; set; }
            public int IANum { get; set; }
            public string FacType { get; set; }
            public string Farmtype { get; set; }
        }
    }
}
