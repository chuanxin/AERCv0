using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
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
    public class FarmLands
    {
        DryEntities DryDB = new DryEntities();
        public byte[] getListByUnitYear(short unit, int year, int IANumStart, int IANumEnd)
        {
            string unitName = new GetData().GetUnitName(unit);

            var datas = from f in DryDB.FarmData
                        join s in DryDB.LandData on f.Section equals s.Section_Id
                        join v in DryDB.CaseDetail on f.MapNo equals v.MapNo
                        where v.ApplyYear == year && v.ApplyUnit == unit && v.IANum >= IANumStart && v.IANum <= IANumEnd
                        select new { s.City, s.Town, s.Section, s.Subsection, f.LandNo, f.BuildArea, f.Outside, v.CatalogCNS, v.IANum, v.ApplyYear};
            List<FarmLandReport> reportdatas = new List<FarmLandReport>();

            foreach (var data in datas)
            {
                FarmLandReport item = new FarmLandReport();
                item.UnitName = unitName;
                item.City = data.City;
                item.Town = data.Town;
                item.Section = string.IsNullOrEmpty(data.Subsection) ? data.Section : data.Section + "-" + data.Subsection;
                item.LandNo = data.LandNo;
                item.BuildArea = (decimal)data.BuildArea;
                item.Outside = data.Outside == true ? "灌區外" : "灌區內";
                item.CatalogCNS = data.CatalogCNS;
                item.IaNum = data.IANum;
                item.ApplyYear = data.ApplyYear;
                reportdatas.Add(item);
            }
            string sourcepath = @"~\ReportSample\FarmLands.xlsx";
            //FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            using (FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                ExcelPackage excel = new ExcelPackage(fs);
                
                ExcelWorksheet sheet = excel.Workbook.Worksheets["清冊"];
                int irow = 2;
                foreach (var item in reportdatas.OrderBy(m => m.IaNum))
                {
                    sheet.Cells[irow, 1].Value = item.UnitName;
                    sheet.Cells[irow, 2].Value = item.ApplyYear;
                    sheet.Cells[irow, 3].Value = item.IaNum;
                    sheet.Cells[irow, 4].Value = item.City;
                    sheet.Cells[irow, 5].Value = item.Town;
                    sheet.Cells[irow, 6].Value = item.Section;
                    sheet.Cells[irow, 7].Value = item.LandNo;
                    sheet.Cells[irow, 8].Value = item.BuildArea;
                    sheet.Cells[irow, 9].Value = item.CatalogCNS;
                    sheet.Cells[irow, 10].Value = item.Outside;
                    irow++;
                }

                return excel.GetAsByteArray();
            }
            
            
        }
    }
}
