using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
    public class CoverReport
    {
        DryEntities DryDB = new DryEntities();
        public byte[] GetReport_Cover(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
            byte[] returnData;
            GetData gd = new GetData();
            List<DataStruct> Data = new List<DataStruct>();
            List<Farm> farms = DryDB.Farm.ToList();
            
            List<EndTypeList> endTypeList = DryDB.EndTypeList.ToList();
            List<FacTypeList> facTypeList = DryDB.FacTypeList.ToList();
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
                int MapNo = item.MapNo.Value;

                var croplist = (from crops in DryDB.CropAreaDetail
                            where crops.MapNo == item.MapNo
                            select crops.Crop).Distinct().ToList();

                List<EndType> endType = DryDB.EndType.Where(m => m.MapNo == MapNo).ToList();
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
                string SectionName="";
                double Area = 0f;
                string FacEndType = "";
                if (farmData.Count > 0)
                {
                    SectionName+=farmData.FirstOrDefault().Town+farmData.FirstOrDefault().Section+"段";
                    if (farmData.FirstOrDefault().Subsection.Length > 0)
                        SectionName += farmData.FirstOrDefault().Subsection + "小段";
                    SectionName += farmData.FirstOrDefault().LandNo + "地號, 等" + farmData.Count + "筆";
                    Area = farmData.Sum(m => m.BuildArea);
                    
                }
                foreach (var endtypeitem in endType)
                {
                    if (endtypeitem.EndTypeCode == 1)
                    {
                        FacEndType += endTypeList.Find(m => m.EndType == endtypeitem.EndTypeCode).EndTypeCNS + " ";
                    }
                    else
                    {
                        FacEndType += facTypeList.Find(m => m.FacType == endtypeitem.FacType).FTpeCNS + endTypeList.Find(m => m.EndType == endtypeitem.EndTypeCode).EndTypeCNS + " ";
                    }
                }
                if (string.IsNullOrEmpty(FacEndType)) FacEndType = "其它";

                Data.Add(new DataStruct
                {
                    IAName = gd.GetUnitName(unit),
                    IANum = item.IANum,
                    ApplyYear = year,
                    FarmerName = item.Name,
                    Addr = item.Addr.Replace(" ", ""),
                    SectionName = SectionName,
                    Area = Area,
                    FacEndType = FacEndType,
                    Crop = croplist
                });
            }
            //byte[] returnData = Cover(Data, sourcepath);
            if (unit == 15)
            {
                returnData = Cover15(Data, sourcepath);
            }
            else
            {
               returnData = Cover(Data, sourcepath);
            }
            return returnData;
        }
        private byte[] Cover(List<DataStruct> data, string sourcepath)
        {
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["封面"];

            int count = data.Count/*39*/;
            for (int i = 0; i < count; i++)
            {
                for (int row = 1; row <= 19; row++)
                {
                    sheet.Cells["A" + row + ":G" + row].Copy(sheet.Cells["A" + (i * 19 + row) + ":G" + (i * 19 + row)]);
                    sheet.Row(i * 19 + row).Height = sheet.Row(row).Height;
                    //sheet.Row(i * 19 + row).Style.HorizontalAlignment = sheet.Row(row).Style.HorizontalAlignment;
                }
            }
            for (int k = 0; k < count; k++)
            {
                sheet = SetCoverReportData(sheet, data[k], k + 1, count);
            }

            byte[] file = excel.GetAsByteArray();

            return file;
        }

        private byte[] Cover15(List<DataStruct> data, string sourcepath)
        {
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["封面"];

            int count = data.Count/*39*/;
            for (int i = 0; i < count; i++)
            {
                for (int row = 1; row <= 19; row++)
                {
                    sheet.Cells["A" + row + ":G" + row].Copy(sheet.Cells["A" + (i * 19 + row) + ":G" + (i * 19 + row)]);
                    sheet.Row(i * 19 + row).Height = sheet.Row(row).Height;
                    //sheet.Row(i * 19 + row).Style.HorizontalAlignment = sheet.Row(row).Style.HorizontalAlignment;
                }
            }
            for (int k = 0; k < count; k++)
            {
                sheet = SetCoverReportData15(sheet, data[k], k + 1, count);//DB
            }

            byte[] file = excel.GetAsByteArray();

            return file;
        }

        public ExcelWorksheet SetCoverReportData(ExcelWorksheet sh, DataStruct Data, int count, int groupcount)
        {

            
            int number = ((count-1) * 19);
            sh.Cells[number + 2, 5].Value = Data.ApplyYear + "年度";
            sh.Cells[number + 3, 4].Value = Data.IAName;
            sh.Cells[number + 5, 4].Value = Data.IANum;
            sh.Cells[number + 6, 4].Value = Data.FarmerName;
            sh.Cells[number + 7, 4].Value = Data.Addr;
            sh.Cells[number + 8, 4].Value = Data.SectionName;
            sh.Cells[number + 9, 4].Value = Data.Area / 10000 + "公頃";
            sh.Cells[number + 10, 4].Value = Data.FacEndType;


            

            return sh;
        }

        public ExcelWorksheet SetCoverReportData15(ExcelWorksheet sh, DataStruct Data, int count, int groupcount)
        {

            int number = ((count - 1) * 19);
            sh.Cells[number + 2, 5].Value = Data.ApplyYear + "年度";
            sh.Cells[number + 3, 4].Value = Data.IAName;
            sh.Cells[number + 5, 4].Value = Data.IANum;
            sh.Cells[number + 6, 4].Value = Data.FarmerName;
            sh.Cells[number + 7, 4].Value = Data.Addr;
            sh.Cells[number + 8, 4].Value = Data.SectionName;
            sh.Cells[number + 9, 4].Value = Data.Area / 10000 + "公頃";
            sh.Cells[number + 10, 4].Value = Data.FacEndType;
            foreach (string item in Data.Crop)
            {
                sh.Cells[number + 11, 4].Value += item.ToString();
                sh.Cells[number + 11, 4].Value += " ";
            }
            




            return sh;
        }

        public byte[] GetReport_CoverPDF(short unit, int year, int IANumStart, int IANumEnd)
        {
            var datalist = from d in DryDB.SummaryView
                           where d.ApplyYear == year && d.ApplyUnit == unit && d.IANum >= IANumStart && d.IANum <= IANumEnd
                           orderby d.IANum
                           select new { d.MapNo, d.ApplyUnit };
            BudgetBookDBService db = new BudgetBookDBService();
            BudgetBookReport budgetbook = new BudgetBookReport();
            using ( iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                foreach (var item in datalist)
                {
                    BudgetBookView Data = db.GetBudgetBookData((int)item.MapNo);
                    document.NewPage();
                    budgetbook.wrirteCover(document, Data);
                }
                document.Close();
                return stream.GetBuffer();
            }
        }

        public class DataStruct
        {
            public int ApplyYear { get; set; }
            public string IAName { get; set; }
            public int IANum { get; set; }
            public string FarmerName { get; set; }
            public string Addr { get; set; }
            public string SectionName { get; set; }
            public double Area { get; set; }
            public string FacEndType { get; set; }
            public List<string> Crop { get; set; }

        }
    }
}
