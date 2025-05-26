using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.Service;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Web;
using Dry.Models.CommonCls;

namespace Dry.Models.ReportModules
{
    public class PlantArea
    {
        private DryEntities DryDB = new DryEntities();
        private GetData gd = new GetData();
        /// <summary>
        /// 年度面積統計含黃金廊道
        /// </summary>
        /// <param name="ayear"></param>
        /// <param name="applyunit"></param>
        /// <param name="step"></param>
        /// <param name="gold"></param>
        /// <returns></returns>
        public byte[] CreateReport(int ayear, int applyunit, int step, bool gold)
        {

            

            var sourdata = from a in DryDB.CropAreaDetail
                           join b in DryDB.SummaryView on a.MapNo equals b.MapNo
                           where b.Complete == true && b.ApplyYear == ayear && b.ApplyUnit == applyunit
                           select a;
            if (gold) 
            {
                sourdata = from o in sourdata
                           join b in DryDB.SummaryView on o.MapNo equals b.MapNo 
                           where b.Gold == false
                           select o;
            }


            var datalist = (from a in /*DryDB.CropAreaDetail*/ sourdata
                            join b in DryDB.SummaryView on a.MapNo equals b.MapNo
                            where /*b.Step >= step*/ b.Complete == true /*&& b.Gold == gold */&& b.ApplyYear == ayear && b.ApplyUnit == applyunit
                            group a by new { a.Crop, a.Crop_Type } into c

                            select new { Crop = c.Key.Crop, Crop_Type = c.Key.Crop_Type, Buildarea = c.Sum(a => a.Buildarea / 10000) }).ToList().OrderBy(e => e.Crop_Type);
            var datalist1 = (from a in /*DryDB.CropAreaDetail*/ sourdata
                             join b in DryDB.SummaryView on a.MapNo equals b.MapNo
                             where /*b.Step >= step*/ b.Complete == true /*&& b.Gold == gold */&& b.ApplyYear == ayear && b.ApplyUnit == applyunit
                             group a by new { a.Crop_Type } into c

                             select new { Crop_Type = c.Key.Crop_Type, Buildarea = c.Sum(a => a.Buildarea / 10000) }).ToList().OrderBy(e => e.Crop_Type);



            string title = gd.GetUnitName((short)applyunit) + ayear + "年農作物統計表";
                
            
            

            string sourcepath = @"~\ReportSample\PlantArea.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["農作物統計表"];
            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 2;
            foreach (var item in datalist)
            {
                irow +=1;
                sheet.Cells[irow, 1].Value = item.Crop_Type;
                sheet.Cells[irow, 2].Value = item.Crop;
                sheet.Cells[irow, 3].Value = item.Buildarea;


            }
            irow += 1; 
            foreach (var item in datalist1)
            {
                irow += 1;
                sheet.Cells[irow, 1].Value = item.Crop_Type;
                sheet.Cells[irow, 2].Value = "合計";
                sheet.Cells[irow, 3].Value = item.Buildarea;
            }
            return excel.GetAsByteArray();
        }
    }
}
