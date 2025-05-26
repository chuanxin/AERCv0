using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models.CommonCls;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Web;
using Dry.Models.ViewModel;
using Dry.Models.Service;

namespace Dry.Models.Service
{
    public class SlopelandQueryService
    {
        /// <summary>
        /// 取得鄉鎮代碼
        /// </summary>
        /// <param name="TownID"></param>
        /// <returns></returns>
        public string GetTownCode(string TownID)
        {
            GetTownData getTownData = new GetTownData();
            string TownCode = getTownData.GetTownCode(TownID);
            return TownCode;
        }
        public string GetTownCodeByName(string CityCode,string TownName)
        {
            GetTownData getTownData = new GetTownData();
            var TownList = getTownData.GetTownList(CityCode.ToUpper());
            string TownCode = "";
            if (CityCode != "-1")
            {
                foreach (var item in TownList)
                {
                    if(item.Town1 == TownName)
                    {
                        TownCode = item.Town_Id.ToString();
                    }
                }
            }
            return TownCode;
        }

        /// <summary>
        /// 地段代碼
        /// </summary>
        /// <param name="SectionID"></param>
        /// <returns></returns>
        public string GetSection(int SectionID)
        {
            GetSectionData getSection = new GetSectionData();
            return getSection.GetSectionCode(SectionID);
        }

        public string GetSectionByName(string TownID, string SectionName)
        {
            string SectionID = "";
            if (TownID != "")
            {
                List<AERC.Models.Section> List = new AERC.Models.CommonCls.GetSectionData().GetSectionList(Convert.ToInt16(TownID));
                foreach (var item in List)
                {
                    if (item.Subsection.Length <= 0)
                    {
                        if (SectionName == item.Section1)
                        {
                            SectionID = item.Section_Id.ToString();
                        }
                    }
                    else
                    {
                        if (SectionName == item.Section1 + " - " + item.Subsection)
                        {
                            SectionID = item.Section_Id.ToString();
                        }
                    }

                }
            }
            return SectionID;
        }
        /// <summary>
        /// 取得城市代碼
        /// </summary>
        /// <param name="TownID"></param>
        /// <returns></returns>
        public string GetCityCodeByName(string CityName)
        {
            GetTownData getTownData = new GetTownData();
            var CityList = getTownData.GetCityList();
            string CityCode = "";
            foreach (var item in CityList)
            {
                if(item.City1.ToString() == CityName.ToString())
                {
                    CityCode = item.City_Code;
                }
            }
            return CityCode;
        }

        public byte[] GetRoadReportData(List<RoadData> Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/SearchRoad.xlsx";
            //開檔
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            //載入Excel檔案
            ExcelPackage excel = new ExcelPackage(fs);
            //取得Sheet
            ExcelWorksheet sheet = excel.Workbook.Worksheets["山坡地查定資料"];
            int count = Data.Count;//資料筆數
            for (int i = 0; i < count; i++)
            {
                sheet = SetRoadReportData(sheet, Data[i]);//DB
            }
            byte[] file = excel.GetAsByteArray();
            #endregion
            return file;
        }
        public ExcelWorksheet SetRoadReportData(ExcelWorksheet sh, RoadData Data)
        {
            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

               
                sh.Column(i).Style.Font.Name = "標楷體";
            }//End for

            int startRow = sh.Dimension.Start.Row;
            int endRow = sh.Dimension.End.Row;
            int DataRowNumber = endRow + 1;

            
            sh.Cells[DataRowNumber, 1].Value = Data.RCity;
            sh.Cells[DataRowNumber, 1].Style.Font.Size = 12;
            
            sh.Cells[DataRowNumber, 2].Value = Data.RTwon;
            sh.Cells[DataRowNumber, 2].Style.Font.Size = 12;
            
            sh.Cells[DataRowNumber, 3].Value = Data.RRoad;
            sh.Cells[DataRowNumber, 3].Style.Font.Size = 12;
            
            sh.Cells[DataRowNumber, 4].Value = Data.RRoadNumber;
            sh.Cells[DataRowNumber, 4].Style.Font.Size = 12;
            
            sh.Cells[DataRowNumber, 5].Value = Data.CheckType;
            sh.Cells[DataRowNumber, 5].Style.Font.Size = 12;
            
            sh.Cells[DataRowNumber, 6].Value = Data.Geological_Conservation;
            sh.Cells[DataRowNumber, 6].Style.Font.Size = 12;
            
            sh.Cells[DataRowNumber, 7].Value = Data.Water_Conservation;
            sh.Cells[DataRowNumber, 7].Style.Font.Size = 12;



            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
    }
}
