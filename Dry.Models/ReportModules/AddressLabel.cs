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
    public class AddressLabel
    {
        public byte[] GetReport_AddressLabel(short unit, int year, int IANumStart, int IANumEnd, string sourcepath)
        {
            List<AddressReportView> data = new List<AddressReportView>();
            DryEntities DryDB=new DryEntities();
            
            List<AERC.Models.Town> towns = new AERC.Models.CommonEntities().Town.ToList();
            var oridata = DryDB.SummaryView.Where(m => m.IANum >= IANumStart && m.IANum <= IANumEnd && m.ApplyUnit == unit && m.ApplyYear == year).OrderBy(m => m.IANum).ToList();
            foreach(var item in oridata)
            {
                data.Add(new AddressReportView
                {
                    AAddress = item.Addr.Replace(" ", ""),
                    ACaseNumber = item.IANum.ToString(),
                    AName = item.Name,
                    ZpiCode = towns.Any(m => m.Town1 == item.Addr.Split(' ')[1]) ? towns.Where(m => m.Town1 == item.Addr.Split(' ')[1]).FirstOrDefault().Zip_Code.Value.ToString() : ""
                });
            }
            byte[] resdata = GetAddressReportData(data, sourcepath);
            return resdata;
        }
        public byte[] GetAddressReportData(List<AddressReportView> Data, string sourcepath)
        {
            #region basic Data

            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["地址"];

            int count = Data.Count;
            int modcount = Convert.ToInt32(Math.Ceiling((double)count / 16));
            for (int i = 1; i < modcount; i++)
            {
                for (int row = 1; row <= 32; row++)
                {
                    sheet.Cells["A" + row + ":J" + row].Copy(sheet.Cells["A" + (i * 32 + row) + ":J" + (i * 32 + row)]);
                    sheet.Row(i * 32 + row).Height = sheet.Row(row).Height;
                }
            }

            for (int i = 0; i < count; i++)
            {
                sheet = SetAddressReportData(sheet, Data[i], i + 1);//DB
            }

            byte[] file = excel.GetAsByteArray();

            #endregion

            return file;
        }
        #region 設定住址標籤
        public ExcelWorksheet SetAddressReportData(ExcelWorksheet sh, AddressReportView Data, int index)
        {
            #region
            if (index % 2 == 0)//偶數
            {
                int number = ((index - 2) * 2);
                sh.Cells[number + 1, 6].Value = Data.ZpiCode + "　" + Data.AAddress;//地址
                sh.Cells[number + 2, 6].Value = Data.ACaseNumber + "  " + Data.AName;//案號 + 姓名
            }
            else//奇數
            {
                int number = ((index - 1) * 2);
                sh.Cells[number + 1, 1].Value = Data.ZpiCode + "　" + Data.AAddress; ;//地址
                sh.Cells[number + 2, 1].Value = Data.ACaseNumber + "  " + Data.AName;//案號 + 姓名
            }
            #endregion
            //sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion
    }
}
