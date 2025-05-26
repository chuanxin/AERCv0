using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;
using System.Linq.Dynamic;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style; 
using Dry.Models.CommonCls;
using Dry.Models.Service;
using System.IO;
using System.Web;


namespace Dry.Models.Service
{
    public class SubsidyReportDBService
    {
        double totalarea = 0;
        int WaterReservoirAtotal = 0;
        int WaterReservoirBtotal = 0;
        private GetData getDataCls = new GetData();

        #region 補助清冊

        public byte[] GetSubsidyReportData(List<SubsidyReportView> Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/SubsidyReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["補助清冊"];

            int count = Data.Count;

            for (int k = 1; k <= count; k++)
            {
                sheet = SetSubsidyReportTable(sheet, Data[k], k, count);
            }
            

            byte[] file = excel.GetAsByteArray();

            #endregion

            return file;
        }
        #region 設定補助清冊
        public ExcelWorksheet SetSubsidyReportTable(ExcelWorksheet sh, SubsidyReportView Data, int count, int groupcount)
        {
            sh.Cells[1, 5].Value = Data.Pyears;

            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;
            int startRowNumber = sh.Dimension.Start.Row;
            int endRowNumber = sh.Dimension.End.Row;

            int DataRowNumber = 0;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";

            }//End for

            #region
            if (endRowNumber % 30 == 0)
            {
                DataRowNumber = endRowNumber + 5;
                for (int j = 1; j <= 4; j++)
                {
                    sh.Cells["A" + j.ToString() + ":K" + j.ToString()].Copy(sh.Cells["A" + (endRowNumber + j).ToString() + ":K" + (endRowNumber + j).ToString()]);
                }
            }
            else
            {
                DataRowNumber = endRowNumber + 1;
            }

            //for (var index = 5; index < count+5 ; index++)
            //{
            //    for (var j = 0; j < endColumn; j++)
            //    {
            
            sh.Cells[DataRowNumber, 1, DataRowNumber + 1, 1].Merge = true;
            sh.Cells[DataRowNumber, 1].Value = Data.PNo;
            sh.Cells[DataRowNumber, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber + 1, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 1].Style.WrapText = true;
            sh.Cells[DataRowNumber, 1].Style.Font.Size = 10;

            
            sh.Cells[DataRowNumber, 2, DataRowNumber + 1, 2].Merge = true;
            sh.Cells[DataRowNumber, 2].Value = Data.PName;
            sh.Cells[DataRowNumber, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber + 1, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 2].Style.Font.Size = 10;

            
            sh.Cells[DataRowNumber, 3, DataRowNumber + 1, 3].Merge = true;
            sh.Cells[DataRowNumber, 3].Value = Data.PLocation;
            sh.Cells[DataRowNumber, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber + 1, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 3].Style.WrapText = true;
            sh.Cells[DataRowNumber, 3].Style.Font.Size = 10;

            
            sh.Cells[DataRowNumber, 4, DataRowNumber + 1, 4].Merge = true;
            sh.Column(4).Style.Numberformat.Format = Data.PArea;
            sh.Cells[DataRowNumber, 4].Value = 0.12;
            double totalarea1 = 0.12;
            sh.Cells[DataRowNumber, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber + 1, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 4].Style.WrapText = true;
            sh.Cells[DataRowNumber, 4].Style.Font.Size = 10;
            //float.Parse(sh.Cells[DataRowNumber, 4].Text);
            
            sh.Cells[DataRowNumber, 5, DataRowNumber + 1, 5].Merge = true;
            sh.Cells[DataRowNumber, 5].Value = Data.PPower;
            sh.Cells[DataRowNumber, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber + 1, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 5].Style.Font.Size = 10;

            
            sh.Cells[DataRowNumber, 6, DataRowNumber + 1, 6].Merge = true;
            sh.Cells[DataRowNumber, 6].Value = Data.PPool;
            sh.Cells[DataRowNumber, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber + 1, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 6].Style.Font.Size = 10;

            
            sh.Cells[DataRowNumber, 7].Value = Data.PIrrigate1;
            sh.Cells[DataRowNumber, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            //sh.Cells[CorrectRow, 7].Style.WrapText = true;
            sh.Cells[DataRowNumber, 7].Style.Font.Size = 10;
            sh.Cells[DataRowNumber + 1, 7].Value = Data.PIrrigate2;
            sh.Cells[DataRowNumber + 1, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            //sh.Cells[DataRowNumber + 1, 7].Style.WrapText = true;
            sh.Cells[DataRowNumber + 1, 7].Style.Font.Size = 10;

            
            sh.Cells[DataRowNumber, 8, DataRowNumber + 1, 8].Merge = true;
            sh.Cells[DataRowNumber, 8].Value = Data.PAddress;
            sh.Cells[DataRowNumber, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber + 1, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 8].Style.WrapText = true;
            sh.Cells[DataRowNumber, 8].Style.Font.Size = 10;

            
            sh.Cells[DataRowNumber, 9, DataRowNumber + 1, 9].Merge = true;
            sh.Cells[DataRowNumber, 9].Value = Data.PPersonID;
            sh.Cells[DataRowNumber, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber + 1, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 9].Style.Font.Size = 10;

            
            sh.Cells[DataRowNumber, 10, DataRowNumber + 1, 10].Merge = true;
            sh.Cells[DataRowNumber, 10].Value = Data.PPhone;
            sh.Cells[DataRowNumber, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber + 1, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 10].Style.Font.Size = 10;

            
            sh.Cells[DataRowNumber, 11, DataRowNumber + 1, 11].Merge = true;
            sh.Cells[DataRowNumber, 11].Value = Data.PMarks;
            sh.Cells[DataRowNumber, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber + 1, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 11].Style.Font.Size = 10;

            totalarea = totalarea + totalarea1;
            //    }
            //    DataRowNumber = DataRowNumber + 2;

            //}
            #endregion

            int startRowNumberfinal = sh.Dimension.Start.Row;
            int endRowNumberfinal = sh.Dimension.End.Row;
            int DataRowNumberfinal = endRowNumberfinal + 1;
            
            if (count == groupcount)
            {
                sh.Cells[DataRowNumberfinal, 1, DataRowNumberfinal, 11].Merge = true;
                sh.Cells[DataRowNumberfinal, 1, DataRowNumberfinal, 11].Value = "總  計:" + Data.PCount + "件   "+ Data.PFramCount +"戶農家    面 積:" + totalarea + "公 頃";
            }

            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion

        #endregion

        #region 管路工程設施輔助明細表

        public byte[] GetEngineeringReportData(List<EngineeringReportView> Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/EngineeringReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["明細表"];

            int count = 48;

            for (int k = 1; k <= count; k++)
            {
                sheet = SetEngineeringReportData(sheet, Data[k], k, count);
            }

            byte[] file = excel.GetAsByteArray();

            #endregion
            return file;
        }
        #region 設定管路工程設施輔助明細表
        public ExcelWorksheet SetEngineeringReportData(ExcelWorksheet sh, EngineeringReportView Data, int count, int groupcount)
        {
            sh.Cells[1, 1].Value = Data.EIA;
            sh.Cells[1, 7].Value = Data.Eyears;

            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;
            int startRowNumber = sh.Dimension.Start.Row;
            int endRowNumber = sh.Dimension.End.Row;
            int DataRowNumber = endRowNumber + 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";

            }//End for
            sh.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            if (endRowNumber % 30 == 0)
            {
                DataRowNumber = endRowNumber + 5;
                for (int j = 1; j <= 4; j++)
                {
                    sh.Cells["A" + j.ToString() + ":R" + j.ToString()].Copy(sh.Cells["A" + (endRowNumber + j).ToString() + ":R" + (endRowNumber + j).ToString()]);
                }
            }
            else
            {
                DataRowNumber = endRowNumber + 1;
            }
            #region
            //for (var index = 1; index < count + 1; index++)//row
            //{
            //    for (var j = 0; j < endColumn; j++)//column
            //    {
            //設施編號
            sh.Cells[DataRowNumber, 1].Value = Data.ENo;
            sh.Cells[DataRowNumber, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 1].Style.WrapText = true;
            sh.Cells[DataRowNumber, 1].Style.Font.Size = 10;

            //姓名
            sh.Cells[DataRowNumber, 2].Value = Data.EName;
            sh.Cells[DataRowNumber, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 2].Style.Font.Size = 10;

            //面積(工頃)
            sh.Column(3).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 3].Value = Data.EArea;
            sh.Cells[DataRowNumber, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 3].Style.WrapText = true;
            sh.Cells[DataRowNumber, 3].Style.Font.Size = 10;

            //地點
            sh.Cells[DataRowNumber, 4].Value = Data.ELocation;
            sh.Cells[DataRowNumber, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 4].Style.WrapText = true;
            sh.Cells[DataRowNumber, 4].Style.Font.Size = 10;
            //灌溉型式
            sh.Cells[DataRowNumber, 5].Value = Data.EIrrigation;
            sh.Cells[DataRowNumber, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 5].Style.Font.Size = 10;

            //農戶配合款
            sh.Column(6).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 6].Value = Data.EFarmer;
            sh.Cells[DataRowNumber, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 6].Style.Font.Size = 10;

            //末端設施
            sh.Column(7).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 7].Value = Data.Eendfacility;
            sh.Cells[DataRowNumber, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            //sh.Cells[CorrectRow, 7].Style.WrapText = true;
            sh.Cells[DataRowNumber, 7].Style.Font.Size = 10;

            //水源設施
            sh.Column(8).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 8].Value = Data.EWaterFacility;
            sh.Cells[DataRowNumber, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 8].Style.WrapText = true;
            sh.Cells[DataRowNumber, 8].Style.Font.Size = 10;

            //調控設施
            sh.Column(9).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 9].Value = Data.ERegulation;
            sh.Cells[DataRowNumber, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 9].Style.Font.Size = 10;

            //蓄水池
            sh.Column(10).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 10].Value = Data.EWaterReservoir;
            sh.Cells[DataRowNumber, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 10].Style.Font.Size = 10;

            //動力設備
            sh.Column(11).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 11].Value = Data.EPowerEquipment;
            sh.Cells[DataRowNumber, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 11].Style.Font.Size = 10;

            //小計
            sh.Column(12).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            string Gt = "G" + DataRowNumber;
            string Kt = "K" + DataRowNumber;
            sh.Cells[DataRowNumber, 12].Formula = "+SUM(" + Gt + ":" + Kt + ")";
            sh.Cells[DataRowNumber, 12].Style.Font.Size = 10;

            //設計費
            sh.Column(13).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 13].Value = Data.EDesignCharges;
            sh.Cells[DataRowNumber, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 13].Style.Font.Size = 10;

            //總計
            sh.Column(14).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            string Lt = "L" + DataRowNumber;
            string Mt = "M" + DataRowNumber;
            sh.Cells[DataRowNumber, 14].Formula = "+SUM(" + Lt + ":" + Mt + ")";
            sh.Cells[DataRowNumber, 14].Style.Font.Size = 10;

            //工程費合計
            sh.Column(15).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 15].Style.Font.Size = 10;
            string Ft = "F" + DataRowNumber;
            string Nt = "N" + DataRowNumber;
            sh.Cells[DataRowNumber, 15].Formula = "+SUM(" + Ft + "," + Nt + ")";

            //補助費
            sh.Column(16).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 16].Style.Font.Size = 10;
            string Ct = "C" + DataRowNumber;
            Lt = "L" + DataRowNumber;
            sh.Cells[DataRowNumber, 16].Formula = "+ROUND((" + Lt + "/" + Ct + "),0)";

            //百分比
            sh.Column(17).Style.Numberformat.Format = "0.00";
            sh.Cells[DataRowNumber, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 17].Style.Font.Size = 10;
            string Pt = "P" + DataRowNumber;
            string Ot = "O" + DataRowNumber;
            sh.Cells[DataRowNumber, 17].Formula = "+ROUND((" + Ot + "/" + Pt + ")*100,2)";

            //總工程費
            sh.Column(18).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 18].Value = 274359;
            sh.Cells[DataRowNumber, 18].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 18].Style.Font.Size = 10;
            Ot = "O" + DataRowNumber;
            Ct = "C" + DataRowNumber;
            sh.Cells[DataRowNumber, 18].Formula = "+ROUND((" + Ot + "/" + Ct + "),2)";
            //    }
            //    DataRowNumber++;
            //}
            #endregion

            int startRowNumberfinal = sh.Dimension.Start.Row;
            int endRowNumberfinal = sh.Dimension.End.Row;
            int DataRowNumberfinal = endRowNumberfinal + 1;

            #region 合計-總計
            if (count == groupcount)
            {
                int k = endRowNumberfinal / 30;
                int m = endRowNumberfinal - (k * 30);
                int mod = 30 - m;
                int checknumber = m + 6 + 1;
                if (checknumber > 30 || m == 0)
                {
                    DataRowNumberfinal = endRowNumberfinal + 4 + mod;
                    for (int j = 1; j <= 4; j++)
                    {
                        sh.Cells["A" + j.ToString() + ":R" + j.ToString()].Copy(sh.Cells["A" + (endRowNumber + mod + j).ToString() + ":R" + (endRowNumber + mod + j).ToString()]);
                    }
                }
                else
                {
                    DataRowNumberfinal = endRowNumberfinal + 1;
                }
                //int a = 10;
                for (int i = 0; i < 6; i++)
                {
                    sh.Row(DataRowNumberfinal + i).Style.Font.Size = 9;
                    for (int j = 1; j < 19; j++)
                    {
                        sh.Cells[DataRowNumberfinal + i, j].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    }
                }
                sh.Cells[DataRowNumberfinal, 1, DataRowNumberfinal + 4, 1].Merge = true;
                sh.Cells[DataRowNumberfinal, 1, DataRowNumberfinal + 4, 1].Value = "合計";
                sh.Cells[DataRowNumberfinal, 2].Value = Data.EFacilityA + "件設施";
                sh.Cells[DataRowNumberfinal + 1, 2].Value = Data.EFacilityB + "件設施";
                sh.Cells[DataRowNumberfinal + 2, 2].Value = Data.EFacilityC + "件設施";
                sh.Cells[DataRowNumberfinal + 3, 2].Value = Data.EFacilityD + "件設施";
                sh.Cells[DataRowNumberfinal + 4, 2].Value = Data.EFacilityE + "件設施";
                
                sh.Column(3).Style.Numberformat.Format = "0.00";
                sh.Cells[DataRowNumberfinal, 3].Value = Data.EFacilityAreaA;
                sh.Cells[DataRowNumberfinal + 1, 3].Value = Data.EFacilityAreaB;
                sh.Cells[DataRowNumberfinal + 2, 3].Value = Data.EFacilityAreaC;
                sh.Cells[DataRowNumberfinal + 3, 3].Value = Data.EFacilityAreaD;
                sh.Cells[DataRowNumberfinal + 4, 3].Value = Data.EFacilityAreaE;
                
                sh.Cells[DataRowNumberfinal, 5].Value = "穿孔管";
                sh.Cells[DataRowNumberfinal + 1, 5].Value = "噴頭";
                sh.Cells[DataRowNumberfinal + 2, 5].Value = "滴灌";
                sh.Cells[DataRowNumberfinal + 3, 5].Value = "微噴";
                sh.Cells[DataRowNumberfinal + 4, 5].Value = "軟管澆灌";
                
                sh.Column(6).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowNumberfinal, 6].Value = Data.EFacilityFarmerA;
                sh.Cells[DataRowNumberfinal + 1, 6].Value = Data.EFacilityFarmerB;
                sh.Cells[DataRowNumberfinal + 2, 6].Value = Data.EFacilityFarmerC;
                sh.Cells[DataRowNumberfinal + 3, 6].Value = Data.EFacilityFarmerD;
                sh.Cells[DataRowNumberfinal + 4, 6].Value = Data.EFacilityFarmerE;
                
                sh.Column(7).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowNumberfinal, 7].Value = Data.EFacilityendfacilityA;
                sh.Cells[DataRowNumberfinal + 1, 7].Value = Data.EFacilityendfacilityB;
                sh.Cells[DataRowNumberfinal + 2, 7].Value = Data.EFacilityendfacilityC;
                sh.Cells[DataRowNumberfinal + 3, 7].Value = Data.EFacilityendfacilityD;
                sh.Cells[DataRowNumberfinal + 4, 7].Value = Data.EFacilityendfacilityE;
                
                sh.Column(8).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowNumberfinal, 8].Value = Data.EFacilityWaterFacilityA;
                sh.Cells[DataRowNumberfinal + 1, 8].Value = Data.EFacilityWaterFacilityB;
                sh.Cells[DataRowNumberfinal + 2, 8].Value = Data.EFacilityWaterFacilityC;
                sh.Cells[DataRowNumberfinal + 3, 8].Value = Data.EFacilityWaterFacilityD;
                sh.Cells[DataRowNumberfinal + 4, 8].Value = Data.EFacilityWaterFacilityE;
                
                sh.Column(9).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowNumberfinal, 9].Value = Data.EFacilityRegulationA;
                sh.Cells[DataRowNumberfinal + 1, 9].Value = Data.EFacilityRegulationB;
                sh.Cells[DataRowNumberfinal + 2, 9].Value = Data.EFacilityRegulationC;
                sh.Cells[DataRowNumberfinal + 3, 9].Value = Data.EFacilityRegulationD;
                sh.Cells[DataRowNumberfinal + 4, 9].Value = Data.EFacilityRegulationE;
                
                sh.Column(10).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowNumberfinal, 10].Value = Data.EFacilityWaterReservoirA;
                sh.Cells[DataRowNumberfinal + 1, 10].Value = Data.EFacilityWaterReservoirB;
                sh.Cells[DataRowNumberfinal + 2, 10].Value = Data.EFacilityWaterReservoirC;
                sh.Cells[DataRowNumberfinal + 3, 10].Value = Data.EFacilityWaterReservoirD;
                sh.Cells[DataRowNumberfinal + 4, 10].Value = Data.EFacilityWaterReservoirE;
                
                sh.Column(11).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowNumberfinal, 11].Value = Data.EFacilityPowerA;
                sh.Cells[DataRowNumberfinal + 1, 11].Value = Data.EFacilityPowerB;
                sh.Cells[DataRowNumberfinal + 2, 11].Value = Data.EFacilityPowerC;
                sh.Cells[DataRowNumberfinal + 3, 11].Value = Data.EFacilityPowerD;
                sh.Cells[DataRowNumberfinal + 4, 11].Value = Data.EFacilityPowerE;
                
                sh.Column(12).Style.Numberformat.Format = "#,##0";
                for (int i = 0; i < 5; i++)
                {
                    int number = 25 + i;
                    Gt = "G" + number;
                    Kt = "K" + number;
                    number = number + i;
                    sh.Cells[DataRowNumberfinal + i, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    sh.Cells[DataRowNumberfinal + i, 12].Style.Font.Size = 9;
                    sh.Cells[DataRowNumberfinal + i, 12].Formula = "+SUM(" + Gt + ":" + Kt + ")";
                }
                
                sh.Column(13).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowNumberfinal, 13].Value = Data.EFacilityDesignA;
                sh.Cells[DataRowNumberfinal + 1, 13].Value = Data.EFacilityDesignB;
                sh.Cells[DataRowNumberfinal + 2, 13].Value = Data.EFacilityDesignC;
                sh.Cells[DataRowNumberfinal + 3, 13].Value = Data.EFacilityDesignD;
                sh.Cells[DataRowNumberfinal + 4, 13].Value = Data.EFacilityDesignE;
                
                sh.Column(14).Style.Numberformat.Format = "#,##0";
                for (int i = 0; i < 5; i++)
                {
                    int number = 25 + i;
                    Lt = "L" + number;
                    Mt = "M" + number;
                    number = number + i;
                    sh.Cells[DataRowNumberfinal + i, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    sh.Cells[DataRowNumberfinal + i, 14].Style.Font.Size = 9;
                    sh.Cells[DataRowNumberfinal + i, 14].Formula = "+SUM(" + Lt + ":" + Mt + ")";
                }
                
                sh.Column(15).Style.Numberformat.Format = "#,##0";
                for (int i = 0; i < 5; i++)
                {
                    int number = 25 + i;
                    Ft = "F" + number;
                    Nt = "N" + number;
                    number = number + i;
                    sh.Cells[DataRowNumberfinal + i, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    sh.Cells[DataRowNumberfinal + i, 15].Style.Font.Size = 9;
                    sh.Cells[DataRowNumberfinal + i, 15].Formula = "+SUM(" + Ft + ":" + Nt + ")";
                }
                
                sh.Column(16).Style.Numberformat.Format = "#,##0";
                Ct = "C" + DataRowNumberfinal;
                Lt = "L" + DataRowNumberfinal;
                sh.Cells[DataRowNumberfinal, 16].Formula = "+ROUND((" + Lt + "/" + Ct + "),0)";
                Ct = "C" + (DataRowNumberfinal + 1);
                Lt = "L" + (DataRowNumberfinal + 1);
                sh.Cells[DataRowNumberfinal + 1, 16].Formula = "+ROUND((" + Lt + "/" + Ct + "),0)";
                Ct = "C" + (DataRowNumberfinal + 2);
                Lt = "L" + (DataRowNumberfinal + 2);
                sh.Cells[DataRowNumberfinal + 2, 16].Formula = "+ROUND((" + Lt + "/" + Ct + "),0)";
                Ct = "C" + (DataRowNumberfinal + 3);
                Lt = "L" + (DataRowNumberfinal + 3);
                sh.Cells[DataRowNumberfinal + 3, 16].Formula = "+ROUND((" + Lt + "/" + Ct + "),0)";
                Ct = "C" + (DataRowNumberfinal + 4);
                Lt = "L" + (DataRowNumberfinal + 4);
                sh.Cells[DataRowNumberfinal + 4, 16].Formula = "+ROUND((" + Lt + "/" + Ct + "),0)";
                
                sh.Column(17).Style.Numberformat.Format = "0.00";
                Pt = "P" + DataRowNumberfinal;
                Ot = "O" + DataRowNumberfinal;
                sh.Cells[DataRowNumberfinal, 17].Formula = "+ROUND((" + Ot + "/" + Pt + ")*100,2)";
                Pt = "P" + (DataRowNumberfinal + 1);
                Ot = "O" + (DataRowNumberfinal + 1);
                sh.Cells[DataRowNumberfinal + 1, 17].Formula = "+ROUND((" + Ot + "/" + Pt + ")*100,2)";
                Pt = "P" + (DataRowNumberfinal + 2);
                Ot = "O" + (DataRowNumberfinal + 2);
                sh.Cells[DataRowNumberfinal + 2, 17].Formula = "+ROUND((" + Ot + "/" + Pt + ")*100,2)";
                Pt = "P" + (DataRowNumberfinal + 3);
                Ot = "O" + (DataRowNumberfinal + 3);
                sh.Cells[DataRowNumberfinal + 3, 17].Formula = "+ROUND((" + Ot + "/" + Pt + ")*100,2)";
                Pt = "P" + (DataRowNumberfinal + 4);
                Ot = "O" + (DataRowNumberfinal + 4);
                sh.Cells[DataRowNumberfinal + 4, 17].Formula = "+ROUND((" + Ot + "/" + Pt + ")*100,2)";
                
                sh.Column(18).Style.Numberformat.Format = "#,##0";
                Ot = "O" + DataRowNumberfinal;
                Ct = "C" + DataRowNumberfinal;
                sh.Cells[DataRowNumberfinal, 18].Formula = "+ROUND((" + Ot + "/" + Ct + "),2)";
                Ot = "O" + (DataRowNumberfinal + 1);
                Ct = "C" + (DataRowNumberfinal + 1);
                sh.Cells[DataRowNumberfinal + 1, 18].Formula = "+ROUND((" + Ot + "/" + Ct + "),2)";
                Ot = "O" + (DataRowNumberfinal + 2);
                Ct = "C" + (DataRowNumberfinal + 2);
                sh.Cells[DataRowNumberfinal + 2, 18].Formula = "+ROUND((" + Ot + "/" + Ct + "),2)";
                Ot = "O" + (DataRowNumberfinal + 3);
                Ct = "C" + (DataRowNumberfinal + 3);
                sh.Cells[DataRowNumberfinal + 3, 18].Formula = "+ROUND((" + Ot + "/" + Ct + "),2)";
                Ot = "O" + (DataRowNumberfinal + 4);
                Ct = "C" + (DataRowNumberfinal + 4);
                sh.Cells[DataRowNumberfinal + 4, 18].Formula = "+ROUND((" + Ot + "/" + Ct + "),2)";
            #endregion

                #region 總計
                sh.Cells[DataRowNumberfinal + 5, 1, DataRowNumberfinal + 5, 2].Merge = true;
                sh.Cells[DataRowNumberfinal + 5, 1, DataRowNumberfinal + 5, 2].Value = "總計";
                sh.Cells[DataRowNumberfinal + 5, 1, DataRowNumberfinal + 5, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                
                int s = DataRowNumberfinal;
                int e = DataRowNumberfinal + 4;
                string number1 = "C" + s;
                string number2 = "C" + e;
                sh.Cells[DataRowNumberfinal + 5, 3].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                
                number1 = "F" + s;
                number2 = "F" + e;
                sh.Cells[DataRowNumberfinal + 5, 6].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                
                number1 = "G" + s;
                number2 = "G" + e;
                sh.Cells[DataRowNumberfinal + 5, 7].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                
                number1 = "H" + s;
                number2 = "H" + e;
                sh.Cells[DataRowNumberfinal + 5, 8].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                
                number1 = "I" + s;
                number2 = "I" + e;
                sh.Cells[DataRowNumberfinal + 5, 9].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                
                number1 = "J" + s;
                number2 = "J" + e;
                sh.Cells[DataRowNumberfinal + 5, 10].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                
                number1 = "K" + s;
                number2 = "K" + e;
                sh.Cells[DataRowNumberfinal + 5, 11].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                
                number1 = "L" + s;
                number2 = "L" + e;
                sh.Cells[DataRowNumberfinal + 5, 12].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                
                number1 = "M" + s;
                number2 = "M" + e;
                sh.Cells[DataRowNumberfinal + 5, 13].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                
                number1 = "N" + s;
                number2 = "N" + e;
                sh.Cells[DataRowNumberfinal + 5, 14].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                
                number1 = "O" + s;
                number2 = "O" + e;
                sh.Cells[DataRowNumberfinal + 5, 15].Formula = "+SUM(" + number1 + ":" + number2 + ")";
                #endregion
            }

            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion

        #endregion

        #region 管路工程設施面積及輔助金額統計表
        public byte[] GetEngineeringCostReportData(EngineeringCostReportView Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/EngineeringCostReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["金額統計表"];

            int group = Data.Rcountgroup;
            //int countpage = Convert.ToInt32(Math.Floor((double)count/39));

            for (int k = 1; k <= group; k++)
            {
                int countgroup = Data.Rdatacount;
                for (int i = 1; i <= countgroup; i++)
                {
                    sheet = SetEngineeringCostReportData(sheet, Data, i, countgroup, k);
                }
            }
            byte[] file = excel.GetAsByteArray();
            #endregion
            return file;
        }
        #region 管路工程設施面積及輔助金額統計表
        public ExcelWorksheet SetEngineeringCostReportData(ExcelWorksheet sh, EngineeringCostReportView Data, int count, int countgroup, int group)
        {
            sh.Cells[1, 1].Value = Data.RIA;
            
            sh.Cells[1, 4].Value = Data.Ryears;

            sh.Cells[1, 14].Value = "推廣";

            sh.Row(1).Style.Font.Size = 14;

            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;
            int startRowNumber = sh.Dimension.Start.Row;
            int endRowNumber = sh.Dimension.End.Row;
            int DataRowNumber = endRowNumber + 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";

            }//End for
            sh.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sh.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            #region
            //for (int k = 1; k < group + 1; k++)
            //{
            //int count = 5;
            //for (var index = 1; index < count + 1; index++)//row
            //{
            startRowNumber = sh.Dimension.Start.Row;//起始列編號，從1算起//1
            endRowNumber = sh.Dimension.End.Row;//結束列編號，從1算起//4
            DataRowNumber = endRowNumber + 1;

            int k = endRowNumber / 28;//商數
            int m = endRowNumber - (k * 28);
            int mod = 28 - m;
            int checknumber = m + countgroup + 1;
            if (count == 1 && checknumber > 28)//群組分段開始
            {
                DataRowNumber = endRowNumber + 3 + mod + 1;
                for (int j = 1; j <= 3; j++)
                {
                    sh.Cells["A" + j.ToString() + ":N" + j.ToString()].Copy(sh.Cells["A" + (endRowNumber + mod + j).ToString() + ":N" + (endRowNumber + mod + j).ToString()]);
                }
            }
            else
            {
                DataRowNumber = endRowNumber + 1;
            }

            //地點
            sh.Cells[DataRowNumber, 1].Value = Data.RLocation;
            sh.Cells[DataRowNumber, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 1].Style.WrapText = true;
            sh.Cells[DataRowNumber, 1].Style.Font.Size = 10;
            //灌溉型式
            sh.Cells[DataRowNumber, 2].Value = Data.RIrrigation;
            sh.Cells[DataRowNumber, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 2].Style.Font.Size = 10;
            //面積(工頃)
            sh.Column(3).Style.Numberformat.Format = "0.00";
            sh.Cells[DataRowNumber, 3].Value = Data.RArea.ToString();
            sh.Cells[DataRowNumber, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 3].Style.WrapText = true;
            sh.Cells[DataRowNumber, 3].Style.Font.Size = 10;
            //戶數
            sh.Cells[DataRowNumber, 4].Value = Data.RUnit;
            sh.Cells[DataRowNumber, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 4].Style.Font.Size = 10;
            //農戶配合款
            sh.Column(5).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 5].Value = Data.RFarmer;
            sh.Cells[DataRowNumber, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 5].Style.Font.Size = 10;
            //末端補助款
            sh.Column(6).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 6].Value = Data.Rendfacility;
            sh.Cells[DataRowNumber, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            //sh.Cells[CorrectRow, 7].Style.WrapText = true;
            sh.Cells[DataRowNumber, 6].Style.Font.Size = 10;
            //蓄水池
            sh.Column(7).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 7].Value = Data.RWaterReservoir;
            sh.Cells[DataRowNumber, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 7].Style.Font.Size = 10;
            //動力設備
            sh.Column(8).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 8].Value = Data.RPowerEquipment;
            sh.Cells[DataRowNumber, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 8].Style.Font.Size = 10;
            //設計費
            sh.Column(9).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 9].Value = Data.RDesignCharges;
            sh.Cells[DataRowNumber, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 9].Style.Font.Size = 10;
            //總計
            sh.Column(10).Style.Numberformat.Format = "#,##0";
            //sh.Cells[DataRowNumber, 11].Value = "100";
            sh.Cells[DataRowNumber, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 10].Style.Font.Size = 10;
            string F = "F" + DataRowNumber;
            string I = "I" + DataRowNumber;
            sh.Cells[DataRowNumber, 10].Formula = "+SUM(" + F + ":" + I + ")";
            //工程費合計
            sh.Column(11).Style.Numberformat.Format = "#,##0";
            //sh.Cells[DataRowNumber, 11].Value = "100";
            sh.Cells[DataRowNumber, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 11].Style.Font.Size = 10;
            string E = "E" + DataRowNumber;
            string J = "J" + DataRowNumber;
            sh.Cells[DataRowNumber, 11].Formula = "+SUM(" + E + ":" + J + ")";
            //補助費
            sh.Column(12).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 12].Style.Font.Size = 10;
            string Ct = "C" + DataRowNumber;
            string Kt = "K" + DataRowNumber;
            sh.Cells[DataRowNumber, 12].Formula = "+ROUND((" + Kt + "/" + Ct + "),0)";
            //百分比
            sh.Column(13).Style.Numberformat.Format = "0.00";
            sh.Cells[DataRowNumber, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 13].Style.Font.Size = 10;
            Kt = "K" + DataRowNumber;
            string Lt = "L" + DataRowNumber;
            sh.Cells[DataRowNumber, 13].Formula = "+ROUND((" + Kt + "/" + Lt + ")*100,2)";
            //總工程費
            sh.Column(14).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
            sh.Cells[DataRowNumber, 14].Style.Font.Size = 10;
            Kt = "K" + DataRowNumber;
            Ct = "C" + DataRowNumber;
            sh.Cells[DataRowNumber, 14].Formula = "+ROUND((" + Kt + "/" + Ct + "),2)";

            //DataRowNumber++;
            //}
            int startRowNumberfinal = sh.Dimension.Start.Row;//起始列編號，從1算起//
            int endRowNumberfinal = sh.Dimension.End.Row;//結束列編號，從1算起//
            int DataRowNumberfinal = endRowNumberfinal + 1;

            if (countgroup == count)//群組分段小計
            {
                #region 小計
                //地點
                sh.Cells[DataRowNumberfinal, 1].Value = Data.RLocation;
                sh.Cells[DataRowNumberfinal, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 1].Style.WrapText = true;
                sh.Cells[DataRowNumberfinal, 1].Style.Font.Size = 10;
                //灌溉型式
                sh.Cells[DataRowNumberfinal, 2].Value = "小計";
                sh.Cells[DataRowNumberfinal, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 2].Style.Font.Size = 10;
                //面積(工頃)
                sh.Column(3).Style.Numberformat.Format = "0.00";
                int number1 = DataRowNumberfinal - count;
                int number2 = DataRowNumberfinal - 1;
                string C1 = "C" + number1;
                string C2 = "C" + number2;
                sh.Cells[DataRowNumberfinal, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 3].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 3].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                //戶數
                C1 = "D" + number1;
                C2 = "D" + number2;
                sh.Cells[DataRowNumberfinal, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 4].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 4].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                //農戶配合款
                sh.Column(5).Style.Numberformat.Format = "#,##0";
                C1 = "E" + number1;
                C2 = "E" + number2;
                sh.Cells[DataRowNumberfinal, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 5].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 5].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                //末端補助款
                sh.Column(6).Style.Numberformat.Format = "#,##0";
                C1 = "F" + number1;
                C2 = "F" + number2;
                sh.Cells[DataRowNumberfinal, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 6].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 6].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                //蓄水池
                sh.Column(7).Style.Numberformat.Format = "#,##0";
                C1 = "G" + number1;
                C2 = "G" + number2;
                sh.Cells[DataRowNumberfinal, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 7].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 7].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                //動力設備
                sh.Column(8).Style.Numberformat.Format = "#,##0";
                C1 = "H" + number1;
                C2 = "H" + number2;
                sh.Cells[DataRowNumberfinal, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 8].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 8].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                //設計費
                sh.Column(9).Style.Numberformat.Format = "#,##0";
                C1 = "I" + number1;
                C2 = "I" + number2;
                sh.Cells[DataRowNumberfinal, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 9].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 9].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                //總計
                sh.Column(10).Style.Numberformat.Format = "#,##0";
                C1 = "J" + number1;
                C2 = "J" + number2;
                sh.Cells[DataRowNumberfinal, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 10].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 10].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                //工程費合計
                sh.Column(11).Style.Numberformat.Format = "#,##0";
                C1 = "K" + number1;
                C2 = "K" + number2;
                sh.Cells[DataRowNumberfinal, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 11].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 11].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                //補助費
                sh.Column(12).Style.Numberformat.Format = "#,##0";
                C1 = "L" + number1;
                C2 = "L" + number2;
                sh.Cells[DataRowNumberfinal, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 12].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 12].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                //百分比
                sh.Column(13).Style.Numberformat.Format = "0.00";
                sh.Cells[DataRowNumberfinal, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 13].Style.Font.Size = 10;
                Kt = "K" + DataRowNumberfinal;
                Lt = "L" + DataRowNumberfinal;
                sh.Cells[DataRowNumberfinal, 13].Formula = "+ROUND((" + Kt + "/" + Lt + ")*100,2)";
                //總工程費
                sh.Column(14).Style.Numberformat.Format = "#,##0";
                C1 = "N" + number1;
                C2 = "N" + number2;
                sh.Cells[DataRowNumberfinal, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowNumberfinal, 14].Style.Font.Size = 10;
                sh.Cells[DataRowNumberfinal, 14].Formula = "+SUM(" + C1 + ":" + C2 + ")";
                #endregion

                endRowNumberfinal = sh.Dimension.End.Row;//結束列編號，從1算起//
                DataRowNumberfinal = endRowNumberfinal + 1;
                if (endRowNumberfinal % 28 == 0)
                {
                    for (int j = 1; j <= 3; j++)
                    {
                        sh.Cells["A" + j.ToString() + ":N" + j.ToString()].Copy(sh.Cells["A" + (endRowNumber + mod + j).ToString() + ":N" + (endRowNumber + mod + j).ToString()]);
                    }
                    DataRowNumberfinal = DataRowNumberfinal + 3;
                }
            }
            //}
            #endregion
            int startRowfinal = sh.Dimension.Start.Row;
            int endRowfinal = sh.Dimension.End.Row;
            int DataRowfinal = endRowfinal + 1;

            int finalgroup = Data.Rcountgroup;

            if (finalgroup == group && countgroup == count)
            {
                k = endRowfinal / 28;
                m = endRowfinal - (k * 28);
                mod = 28 - m;
                checknumber = m + 3 + 1;
                if (checknumber > 28 || m == 0)
                {
                    DataRowfinal = endRowfinal + 3 + mod + 1;
                    for (int j = 1; j <= 3; j++)
                    {
                        sh.Cells["A" + j.ToString() + ":N" + j.ToString()].Copy(sh.Cells["A" + (endRowfinal + mod + j).ToString() + ":N" + (endRowfinal + mod + j).ToString()]);
                    }
                }
                else
                {
                    DataRowfinal = endRowfinal + 1;
                }

                #region 合計
                int CaseNumber = Convert.ToInt32(Data.RCaseNumber);
                for (int j = 0; j < CaseNumber; j++)
                {
                    //地點
                    sh.Cells[DataRowfinal + j, 1].Value = Data.RLocationTotal;
                    sh.Cells[DataRowfinal + j, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 1].Style.WrapText = true;
                    sh.Cells[DataRowfinal + j, 1].Style.Font.Size = 10;
                    //灌溉型式
                    sh.Cells[DataRowfinal + j, 2].Value = Data.RIrrigationTotal;
                    sh.Cells[DataRowfinal + j, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 2].Style.Font.Size = 10;
                    //面積(工頃)
                    sh.Column(3).Style.Numberformat.Format = Data.RAreaTotal.ToString();
                    sh.Cells[DataRowfinal + j, 3].Value = 5.73;
                    sh.Cells[DataRowfinal + j, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 3].Style.Font.Size = 10;
                    //戶數
                    sh.Cells[DataRowfinal + j, 4].Value = Data.RUnitTotal;
                    sh.Cells[DataRowfinal + j, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 4].Style.Font.Size = 10;
                    //農戶配合款
                    sh.Column(5).Style.Numberformat.Format = "#,##0";
                    sh.Cells[DataRowfinal + j, 5].Value = Data.RFarmerTotal;
                    sh.Cells[DataRowfinal + j, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 5].Style.Font.Size = 10;
                    //末端補助款
                    sh.Column(6).Style.Numberformat.Format = "#,##0";
                    sh.Cells[DataRowfinal + j, 6].Value = Data.RendfacilityTotal;
                    sh.Cells[DataRowfinal + j, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 6].Style.Font.Size = 10;
                    //蓄水池
                    sh.Column(7).Style.Numberformat.Format = "#,##0";
                    sh.Cells[DataRowfinal + j, 7].Value = Data.RWaterReservoirTotal;
                    sh.Cells[DataRowfinal + j, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 7].Style.Font.Size = 10;
                    //動力設備
                    sh.Column(8).Style.Numberformat.Format = "#,##0";
                    sh.Cells[DataRowfinal + j, 8].Value = Data.RPowerEquipmentTotal;
                    sh.Cells[DataRowfinal + j, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 8].Style.Font.Size = 10;
                    //設計費
                    sh.Column(9).Style.Numberformat.Format = "#,##0";
                    sh.Cells[DataRowfinal + j, 9].Value = Data.RDesignChargesTotal;
                    sh.Cells[DataRowfinal + j, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 9].Style.Font.Size = 10;
                    //總計
                    sh.Column(10).Style.Numberformat.Format = "#,##0";
                    int number = DataRowfinal + j;
                    F = "F" + number;
                    I = "I" + number;
                    sh.Cells[DataRowfinal + j, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 10].Style.Font.Size = 10;
                    sh.Cells[DataRowfinal + j, 10].Formula = "+SUM(" + F + ":" + I + ")";
                    //工程費合計
                    sh.Column(11).Style.Numberformat.Format = "#,##0";
                    number = DataRowfinal + j;
                    E = "E" + number;
                    J = "J" + number;
                    sh.Cells[DataRowfinal + j, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 11].Style.Font.Size = 10;
                    sh.Cells[DataRowfinal + j, 11].Formula = "+SUM(" + E + ":" + J + ")";
                    //補助費
                    sh.Column(12).Style.Numberformat.Format = "#,##0";
                    sh.Cells[DataRowfinal + j, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 12].Style.Font.Size = 10;
                    number = DataRowfinal + j;
                    Ct = "C" + number;
                    Kt = "K" + number;
                    sh.Cells[DataRowfinal + j, 12].Formula = "+ROUND((" + Kt + "/" + Ct + "),0)";
                    //百分比
                    sh.Column(13).Style.Numberformat.Format = "0.00";
                    sh.Cells[DataRowfinal + j, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 13].Style.Font.Size = 10;
                    number = DataRowfinal + j;
                    Kt = "K" + number;
                    Lt = "L" + number;
                    sh.Cells[DataRowfinal + j, 13].Formula = "+ROUND((" + Kt + "/" + Lt + ")*100,2)";
                    //總工程費
                    sh.Column(14).Style.Numberformat.Format = "#,##0";
                    sh.Cells[DataRowfinal + j, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowfinal + j, 14].Style.Font.Size = 10;
                    number = DataRowfinal + j;
                    Kt = "K" + number;
                    Ct = "C" + number;
                    sh.Cells[DataRowfinal + j, 14].Formula = "+ROUND((" + Kt + "/" + Ct + "),2)";
                }

                #endregion

                #region 總計
                sh.Cells[DataRowfinal + 4, 1].Value = "合計";
                sh.Cells[DataRowfinal + 4, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //灌溉型式
                sh.Cells[DataRowfinal + 4, 2].Value = "總計";
                sh.Cells[DataRowfinal + 4, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //面積公頃
                int s = DataRowfinal;
                int e = DataRowfinal + 3;
                string number3 = "C" + s;
                string number4 = "C" + e;
                sh.Cells[DataRowfinal + 4, 3].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //戶數
                number3 = "D" + s;
                number4 = "D" + e;
                sh.Cells[DataRowfinal + 4, 4].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 4, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //農戶配合款
                number3 = "E" + s;
                number4 = "E" + e;
                sh.Cells[DataRowfinal + 4, 5].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 4, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //末端補助款
                number3 = "F" + s;
                number4 = "F" + e;
                sh.Cells[DataRowfinal + 4, 6].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 4, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //蓄水池
                number3 = "G" + s;
                number4 = "G" + e;
                sh.Cells[DataRowfinal + 4, 7].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 4, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //動力設備
                number3 = "H" + s;
                number4 = "H" + e;
                sh.Cells[DataRowfinal + 4, 8].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 4, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //設計費
                number3 = "I" + s;
                number4 = "I" + e;
                sh.Cells[DataRowfinal + 4, 9].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 4, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //總計
                number3 = "J" + s;
                number4 = "J" + e;
                sh.Cells[DataRowfinal + 4, 10].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 4, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //工程費合計
                number3 = "K" + s;
                number4 = "K" + e;
                sh.Cells[DataRowfinal + 4, 11].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 4, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                //補助費
                sh.Column(12).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowfinal + 4, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowfinal + 4, 12].Style.Font.Size = 10;
                //Ct = "C" + (DataRowfinal + 4);
                //Kt = "K" + (DataRowfinal + 4);
                number3 = "L" + s;
                number4 = "L" + e;
                sh.Cells[DataRowfinal + 4, 12].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                //百分比
                sh.Column(13).Style.Numberformat.Format = "0.00";
                sh.Cells[DataRowfinal + 4, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowfinal + 4, 13].Style.Font.Size = 10;
                Kt = "K" + (DataRowfinal + 4);
                Lt = "L" + (DataRowfinal + 4);
                sh.Cells[DataRowfinal + 4, 13].Formula = "+ROUND((" + Kt + "/" + Lt + ")*100,2)";
                //總工程費
                sh.Column(14).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowfinal + 4, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                sh.Cells[DataRowfinal + 4, 14].Style.Font.Size = 10;
                Kt = "K" + (DataRowfinal + 4);
                Ct = "C" + (DataRowfinal + 4);
                sh.Cells[DataRowfinal + 4, 14].Formula = "+ROUND((" + Kt + "/" + Ct + "),2)";
                #endregion

                startRowfinal = sh.Dimension.Start.Row;
                endRowfinal = sh.Dimension.End.Row;
                DataRowfinal = endRowfinal + 2;

                int numbers = Convert.ToInt32(Data.RNumberTotal);
                int cost = Convert.ToInt32(Data.RCostTotal);

                sh.Cells[DataRowfinal, 1, DataRowfinal, 11].Merge = true;
                sh.Cells[DataRowfinal, 1, DataRowfinal, 11].Value = "未設施、不合格及放棄設施" + numbers + "件，其設計費" + cost + "元";
            }
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }

        #endregion

        #endregion

        #region 管路灌溉設施工程決算表
        public byte[] GetStatementReportData(List<StatementReportView> Data)
        {
            #region basic Data

            string sample_Path = @"~/ReportSample/StatementReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["決算表"];

            int count = Data.Count;

            for (int i = 1; i <= count; i++)
            {
                int number = ((i - 1) * 21);
                for (int j = 1; j <= 21; j++)
                {
                    sheet.Cells["A" + j.ToString() + ":J" + j.ToString()].Copy(sheet.Cells["A" + (number + j).ToString() + ":J" + (number + j).ToString()]);
                }
            }

            for (int i = 1; i <= count; i++)
            {
                sheet = SetStatementReportData(sheet, Data[i], i);
            }

            byte[] file = excel.GetAsByteArray();

            #endregion

            return file;
        }

        #region 設定管路灌溉設施工程決算表
        public ExcelWorksheet SetStatementReportData(ExcelWorksheet sh, StatementReportView Data, int index)
        {
            int number = ((index - 1) * 21);
            #region
            //sh.Cells[number + 1, 10].Value = "第" + index + "頁";
            sh.Cells[number + 1, 10].Style.Font.Size = 14;
            sh.Row(number + 1).Height = 25;
            sh.Cells[number + 2, 2].Value = Data.Syears;
            sh.Row(number + 2).Height = 25;
            sh.Row(number + 3).Height = 20;
            sh.Cells[number + 4, 4].Value = Data.SNo;
            sh.Row(number + 4).Height = 50;
            sh.Cells[number + 5, 4].Value = Data.SName;
            sh.Row(number + 5).Height = 50;
            sh.Cells[number + 6, 3].Value = Data.SAddress;
            sh.Row(number + 6).Height = 50;
            sh.Cells[number + 7, 3].Value = Data.SLocation;
            sh.Row(number + 7).Height = 50;
            sh.Cells[number + 8, 3].Value = Data.SNumber;
            sh.Row(number + 8).Height = 50;
            sh.Cells[number + 9, 4].Value = Data.SArea;
            sh.Row(number + 9).Height = 45;
            sh.Row(number + 10).Height = 45;
            sh.Row(number + 11).Height = 20;
            sh.Row(number + 12).Height = 20;
            sh.Row(number + 13).Height = 20;
            sh.Row(number + 14).Height = 45;
            sh.Row(number + 15).Height = 45;
            sh.Row(number + 16).Height = 45;
            sh.Row(number + 17).Height = 45;
            sh.Row(number + 18).Height = 20;
            sh.Row(number + 19).Height = 20;
            sh.Row(number + 20).Height = 20;
            sh.Row(number + 21).Height = 20;
            //sh.Row(number + 22).Height = 20;
            #endregion
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion
        #endregion

        #region 成果統計表
        public byte[] GetStatisticsReportData(StatisticsReportView Data)
        {
            #region basic Data

            string sample_Path = @"~/ReportSample/StatisticsReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["成果統計表"];

            int group = Data.Scountgroup;
            //int countpage = Convert.ToInt32(Math.Floor((double)count/39));

            for (int k = 1; k <= group; k++)
            {
                int countgroup = Data.Sdatacount;
                for (int i = 1; i <= countgroup; i++)
                {
                    sheet = SetStatisticsReportData(sheet, Data, i, countgroup, k);
                }
            }

            byte[] file = excel.GetAsByteArray();

            #endregion
            return file;
        }
        #region 設定成果統計表
        public ExcelWorksheet SetStatisticsReportData(ExcelWorksheet sh, StatisticsReportView Data, int count, int countgroup, int group)
        {
            sh.Cells[1, 1].Value = Data.SIA;

            //sh.Cells[2, 1].Value = "頁數:" ;
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;
            //int startRowNumber = sh.Dimension.Start.Row;
            //int endRowNumber = sh.Dimension.End.Row;
            //int DataRowNumber = endRowNumber + 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";
            }//End for
            #region
            int startRow = sh.Dimension.Start.Row;
            int endRow = sh.Dimension.End.Row;
            int DataRowNumber = endRow + 1;

            int k = endRow / 44;
            int m = endRow - (k * 44);
            int mod = 44 - m;
            int checknumber = m + countgroup + 1;
            if (count == 1 && checknumber > 44)
            {
                DataRowNumber = endRow + 4 + mod + 1;
                for (int j = 1; j <= 4; j++)
                {
                    sh.Cells["A" + j.ToString() + ":M" + j.ToString()].Copy(sh.Cells["A" + (endRow + mod + j).ToString() + ":M" + (endRow + mod + j).ToString()]);
                }
            }
            else
            {
                DataRowNumber = endRow + 1;
            }

            //sh.Row(DataRowNumber).Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 1].Value = Data.Syears;
            sh.Cells[DataRowNumber, 1].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 2].Value = Data.SExtend;
            sh.Cells[DataRowNumber, 2].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 3].Value = Data.SIrrigation;
            sh.Cells[DataRowNumber, 3].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 4].Value = Data.SCount;
            sh.Cells[DataRowNumber, 4].Style.Font.Size = 9;
            
            sh.Column(5).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 5].Value = Data.SArea;
            sh.Cells[DataRowNumber, 5].Style.Font.Size = 9;
            
            sh.Column(6).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 6].Value = Data.SMoney;
            sh.Cells[DataRowNumber, 6].Style.Font.Size = 9;
            
            sh.Column(7).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 7].Value = Data.SFacility;
            sh.Cells[DataRowNumber, 7].Style.Font.Size = 9;
            
            string Ft = "F" + DataRowNumber;
            string Ht = "G" + DataRowNumber;
            sh.Cells[DataRowNumber, 8].Formula = "+SUM(" + Ft + ":" + Ht + ")";
            sh.Cells[DataRowNumber, 8].Style.Font.Size = 9;
           
            sh.Column(9).Style.Numberformat.Format = "0.00";
            sh.Cells[DataRowNumber, 9].Style.Font.Size = 9;
            string Gp = "G" + DataRowNumber;
            string Hp = "H" + DataRowNumber;
            sh.Cells[DataRowNumber, 9].Formula = "+ROUND((" + Gp + "/" + Hp + ")*100,2)";
            
            sh.Column(10).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 10].Style.Font.Size = 9;
            string Ep = "E" + DataRowNumber;
            sh.Cells[DataRowNumber, 10].Formula = "+ROUND((" + Gp + "/" + Ep + "),0)";
            
            sh.Column(11).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 11].Style.Font.Size = 9;
            sh.Cells[DataRowNumber, 11].Formula = "+ROUND((" + Hp + "/" + Ep + "),2)";
            
            int WaterReservoirA = Convert.ToInt32(Data.SWaterReservoirT);
            int WaterReservoirB = Convert.ToInt32(Data.SWaterReservoirS);
            sh.Cells[DataRowNumber, 12].Value = WaterReservoirA + "/" + WaterReservoirB;
            sh.Cells[DataRowNumber, 12].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 13].Value = Data.SHydro;
            sh.Cells[DataRowNumber, 13].Style.Font.Size = 9;

            WaterReservoirAtotal = WaterReservoirAtotal + WaterReservoirA;
            WaterReservoirBtotal = WaterReservoirBtotal + WaterReservoirB;

            int startRowfinal = sh.Dimension.Start.Row;
            int endRowfinal = sh.Dimension.End.Row;
            int DataRowfinal = endRowfinal + 1;

            if (countgroup == count)
            {
                startRowfinal = sh.Dimension.Start.Row;
                endRowfinal = sh.Dimension.End.Row;
                DataRowfinal = endRowfinal + 1;

                #region 小計

                
                int numberyear = DataRowfinal - count;
                sh.Cells[DataRowfinal, 1].Value = sh.Cells[numberyear,1].Value;
                sh.Cells[DataRowfinal, 1].Style.Font.Size = 9;
                
                sh.Cells[DataRowfinal, 2].Value = "小計";
                sh.Cells[DataRowfinal, 2].Style.Font.Size = 9;
                
                sh.Cells[DataRowfinal, 4].Style.Font.Size = 9;
                int number1 = DataRowfinal - count;
                int number2 = DataRowfinal - 1;
                string D1 = "D" + number1;
                string D2 = "D" + number2;
                sh.Cells[DataRowfinal, 4].Formula = "+SUM(" + D1 + ":" + D2 + ")";
                
                sh.Column(5).Style.Numberformat.Format = "0.0000";
                sh.Cells[DataRowfinal, 5].Style.Font.Size = 9;
                string E1 = "E" + number1;
                string E2 = "E" + number2;
                sh.Cells[DataRowfinal, 5].Formula = "+SUM(" + E1 + ":" + E2 + ")";
                
                sh.Column(6).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowfinal, 6].Style.Font.Size = 9;
                string F1 = "F" + number1;
                string F2 = "F" + number2;
                sh.Cells[DataRowfinal, 6].Formula = "+SUM(" + F1 + ":" + F2 + ")";
                
                sh.Column(7).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowfinal, 7].Style.Font.Size = 9;
                string G1 = "G" + number1;
                string G2 = "G" + number2;
                sh.Cells[DataRowfinal, 7].Formula = "+SUM(" + G1 + ":" + G2 + ")";
                
                sh.Column(8).Style.Numberformat.Format = "#,##0";
                Ft = "F" + DataRowfinal;
                Ht = "G" + DataRowfinal;
                sh.Cells[DataRowfinal, 8].Formula = "+SUM(" + Ft + ":" + Ht + ")";
                sh.Cells[DataRowfinal, 8].Style.Font.Size = 9;
                
                sh.Column(9).Style.Numberformat.Format = "0.00";
                sh.Cells[DataRowfinal, 9].Style.Font.Size = 9;
                Gp = "G" + DataRowfinal;
                Hp = "H" + DataRowfinal;
                sh.Cells[DataRowfinal, 9].Formula = "+ROUND((" + Gp + "/" + Hp + ")*100,2)";
                
                sh.Column(10).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowfinal, 10].Style.Font.Size = 9;
                Ep = "E" + DataRowfinal;
                sh.Cells[DataRowfinal, 10].Formula = "+ROUND((" + Gp + "/" + Ep + "),0)";
                
                sh.Column(11).Style.Numberformat.Format = "#,##0";
                sh.Cells[DataRowfinal, 11].Style.Font.Size = 9;
                Hp = "H" + DataRowfinal;
                sh.Cells[DataRowfinal, 11].Formula = "+ROUND((" + Hp + "/" + Ep + "),0)";
                
                sh.Cells[DataRowfinal, 12].Value = WaterReservoirAtotal + "/" + WaterReservoirBtotal;
                sh.Cells[DataRowfinal, 12].Style.Font.Size = 9;
                
                string M1 = "M" + number1;
                string M2 = "M" + number2;
                sh.Cells[DataRowfinal, 13].Formula = "+SUM(" + M1 + ":" + M2 + ")";
                //sh.Cells[DataRowfinal, 13].Value = 3;
                sh.Cells[DataRowfinal, 13].Style.Font.Size = 9;
                #endregion

                endRowfinal = sh.Dimension.End.Row;
                DataRowfinal = endRowfinal + 1;
                if (endRowfinal % 44 == 0)
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        sh.Cells["A" + j.ToString() + ":M" + j.ToString()].Copy(sh.Cells["A" + (DataRowfinal + j - 1).ToString() + ":M" + (DataRowfinal + j - 1).ToString()]);
                    }
                    DataRowfinal = DataRowfinal + 4;
                }
            }
            #endregion

            int finalgroup = Data.Scountgroup;

            if (finalgroup == group && countgroup == count)
            {
                startRowfinal = sh.Dimension.Start.Row;
                endRowfinal = sh.Dimension.End.Row;
                DataRowfinal = endRowfinal + 1;


                #region 總計
                sh.Cells[DataRowfinal, 1, DataRowfinal + 5, 2].Merge = true;
                sh.Cells[DataRowfinal, 1, DataRowfinal + 5, 2].Value = "總計";
                sh.Cells[DataRowfinal, 1, DataRowfinal + 5, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                for (int i = 0; i <= 5; i++)
                {
                    sh.Row(DataRowfinal + i).Style.Font.Size = 8;
                }
                
                sh.Cells[DataRowfinal, 3].Value = "穿孔管";
                sh.Cells[DataRowfinal, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 1, 3].Value = "噴管";
                sh.Cells[DataRowfinal + 1, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 2, 3].Value = "微噴";
                sh.Cells[DataRowfinal + 2, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 3, 3].Value = "其他";
                sh.Cells[DataRowfinal + 3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 4, 3].Value = "軟管澆灌";
                sh.Cells[DataRowfinal + 4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 5, 3].Value = "合計";
                sh.Cells[DataRowfinal + 5, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                
                sh.Cells[DataRowfinal, 4].Value = Data.SCountTotalA;
                sh.Cells[DataRowfinal, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 1, 4].Value = Data.SCountTotalB;
                sh.Cells[DataRowfinal + 1, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 2, 4].Value = Data.SCountTotalC;
                sh.Cells[DataRowfinal + 2, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 3, 4].Value = Data.SCountTotalD;
                sh.Cells[DataRowfinal + 3, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 4, 4].Value = Data.SCountTotalE;
                sh.Cells[DataRowfinal + 4, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                string number3 = "D" + DataRowfinal;
                string number4 = "D" + (DataRowfinal + 4).ToString();
                sh.Cells[DataRowfinal + 5, 4].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 5, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                
                sh.Cells[DataRowfinal, 5].Value = Data.SAreaTotalA;
                sh.Cells[DataRowfinal, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 1, 5].Value = Data.SAreaTotalB;
                sh.Cells[DataRowfinal + 1, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 2, 5].Value = Data.SAreaTotalC;
                sh.Cells[DataRowfinal + 2, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 3, 5].Value = Data.SAreaTotalD;
                sh.Cells[DataRowfinal + 3, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 4, 5].Value = Data.SAreaTotalE;
                sh.Cells[DataRowfinal + 4, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                number3 = "E" + DataRowfinal;
                number4 = "E" + (DataRowfinal + 4).ToString();
                sh.Cells[DataRowfinal + 5, 5].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 5, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                
                sh.Cells[DataRowfinal, 6].Value = Data.SMoneyTotalA;
                sh.Cells[DataRowfinal, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 1, 6].Value = Data.SMoneyTotalB;
                sh.Cells[DataRowfinal + 1, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 2, 6].Value = Data.SMoneyTotalC;
                sh.Cells[DataRowfinal + 2, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 3, 6].Value = Data.SMoneyTotalD;
                sh.Cells[DataRowfinal + 3, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 4, 6].Value = Data.SMoneyTotalE;
                sh.Cells[DataRowfinal + 4, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                number3 = "F" + DataRowfinal;
                number4 = "F" + (DataRowfinal + 4).ToString();
                sh.Cells[DataRowfinal + 5, 6].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 5, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                
                sh.Cells[DataRowfinal, 7].Value = Data.SFacilityTotalA;
                sh.Cells[DataRowfinal, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 1, 7].Value = Data.SFacilityTotalB;
                sh.Cells[DataRowfinal + 1, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 2, 7].Value = Data.SFacilityTotalC;
                sh.Cells[DataRowfinal + 2, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 3, 7].Value = Data.SFacilityTotalD;
                sh.Cells[DataRowfinal + 3, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 4, 7].Value = Data.SFacilityTotalE;
                sh.Cells[DataRowfinal + 4, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                number3 = "G" + DataRowfinal;
                number4 = "G" + (DataRowfinal + 4).ToString();
                sh.Cells[DataRowfinal + 5, 7].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 5, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                
                for (int i = 0; i <= 5; i++)
                {
                    number3 = "F" + (DataRowfinal + i).ToString();
                    number4 = "G" + (DataRowfinal + i).ToString();
                    sh.Cells[DataRowfinal + i, 8].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                    sh.Cells[DataRowfinal + i, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                }
                
                for (int i = 0; i <= 5; i++)
                {
                    number3 = "G" + (DataRowfinal + i).ToString();
                    number4 = "H" + (DataRowfinal + i).ToString();
                    sh.Cells[DataRowfinal + i, 9].Formula = "+ROUND((" + number3 + "/" + number4 + ")*100,2)";
                    sh.Cells[DataRowfinal + i, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                }
                
                for (int i = 0; i <= 5; i++)
                {
                    number3 = "G" + (DataRowfinal + i).ToString();
                    number4 = "E" + (DataRowfinal + i).ToString();
                    sh.Cells[DataRowfinal + i, 10].Formula = "+ROUND((" + number3 + "/" + number4 + "),0)";
                    sh.Cells[DataRowfinal + i, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                }
                
                for (int i = 0; i <= 5; i++)
                {
                    number3 = "H" + (DataRowfinal + i).ToString();
                    number4 = "E" + (DataRowfinal + i).ToString();
                    sh.Cells[DataRowfinal + i, 11].Formula = "+ROUND((" + number3 + "/" + number4 + "),2)";
                    sh.Cells[DataRowfinal + i, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                }
                
                int a1 = Convert.ToInt32(Data.SWaterReservoirTTotalA);
                int b1 = Convert.ToInt32(Data.SWaterReservoirTTotalB);
                int c1 = Convert.ToInt32(Data.SWaterReservoirTTotalC);
                int d1 = Convert.ToInt32(Data.SWaterReservoirTTotalD);
                int e1 = Convert.ToInt32(Data.SWaterReservoirSTotalE);
                int a2 = Convert.ToInt32(Data.SWaterReservoirSTotalA);
                int b2 = Convert.ToInt32(Data.SWaterReservoirSTotalB);
                int c2 = Convert.ToInt32(Data.SWaterReservoirSTotalC);
                int d2 = Convert.ToInt32(Data.SWaterReservoirSTotalD);
                int e2 = Convert.ToInt32(Data.SWaterReservoirSTotalE);
                sh.Cells[DataRowfinal, 12].Value = a1 + "/" + a2;
                sh.Cells[DataRowfinal, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 1, 12].Value = b1 + "/" + b2;
                sh.Cells[DataRowfinal + 1, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 2, 12].Value = c1 + "/" + c2;
                sh.Cells[DataRowfinal + 2, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 3, 12].Value = d1 + "/" + d2;
                sh.Cells[DataRowfinal + 3, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 4, 12].Value = e1 + "/" + e2;
                sh.Cells[DataRowfinal + 4, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                number3 = (a1 + b1 + c1 + d1 + e1).ToString();
                number4 = (a2 + b2 + c2 + d2 + e2).ToString();
                sh.Cells[DataRowfinal + 5, 12].Value = number3 + "/" + number4;
                sh.Cells[DataRowfinal + 5, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                
                sh.Cells[DataRowfinal, 13].Value = Data.SHydroTotalA;
                sh.Cells[DataRowfinal, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 1, 13].Value = Data.SHydroTotalB;
                sh.Cells[DataRowfinal + 1, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 2, 13].Value = Data.SHydroTotalC;
                sh.Cells[DataRowfinal + 2, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 3, 13].Value = Data.SHydroTotalD;
                sh.Cells[DataRowfinal + 3, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[DataRowfinal + 4, 13].Value = Data.SHydroTotalE;
                sh.Cells[DataRowfinal + 4, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                number3 = "M" + DataRowfinal;
                number4 = "M" + (DataRowfinal + 4).ToString();
                sh.Cells[DataRowfinal + 5, 13].Formula = "+SUM(" + number3 + ":" + number4 + ")";
                sh.Cells[DataRowfinal + 5, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                #endregion
            }
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion
        #endregion

        #region 農作物統計表
        public byte[] GetFarmStatisticsReportData(List<FarmStatisticsReportView> Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/FarmStatisticsReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["農作物"];

            int count = Data.Count;

            for (int k = 1; k <= count; k++)
            {
                sheet = SetFarmStatisticsReportData(sheet, Data[k], k);
            }
            byte[] file = excel.GetAsByteArray();
            #endregion
            return file;
        }
        #region 設定農作物統計表
        public ExcelWorksheet SetFarmStatisticsReportData(ExcelWorksheet sh, FarmStatisticsReportView Data, int count)
        {
            
            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;
            //int startRowNumber = sh.Dimension.Start.Row;
            //int endRowNumber = sh.Dimension.End.Row;
            //int DataRowNumber = endRowNumber + 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";
            }//End for

            sh.Cells[1, 1].Value = Data.Fyears;
            sh.Cells[1, 2].Value = Data.FIA;
            //if(count == 1)
            //{
            //    sh.Cells[1, 6].Value = "第" + pagecount + "頁";
            //}

            #region
            int startRow = sh.Dimension.Start.Row;//起始列編號，從1算起//
            int endRow = sh.Dimension.End.Row;//結束列編號，從1算起//
            int DataRowNumber = endRow + 1;

            //int k = endRow / 45;//商數
            //int m = endRow - (k * 45);
            //int mod = 45 - m;
            //int checknumber = m + count + 1;
            if (endRow % 45 == 0)//分頁複製
            {
                DataRowNumber = endRow + 3;
                for (int j = 1; j <= 2; j++)
                {
                    sh.Cells["A" + j.ToString() + ":F" + j.ToString()].Copy(sh.Cells["A" + (endRow + j).ToString() + ":F" + (endRow + j).ToString()]);
                }
            }
            else
            {
                DataRowNumber = endRow + 1;
            }

            //sh.Row(DataRowNumber).Style.Font.Size = 9;
            //鄉鎮
            sh.Cells[DataRowNumber, 1].Value = Data.FTwon;
            sh.Cells[DataRowNumber, 1].Style.Font.Size = 12;
            //段別
            sh.Cells[DataRowNumber, 2].Value = Data.FRoad;
            sh.Cells[DataRowNumber, 2].Style.Font.Size = 12;
            //作物名稱
            sh.Cells[DataRowNumber, 3].Value = Data.FCrop;
            sh.Cells[DataRowNumber, 3].Style.Font.Size = 12;
            //灌溉型式
            sh.Cells[DataRowNumber, 4].Value = Data.FIrrigation;
            sh.Cells[DataRowNumber, 4].Style.Font.Size = 12;
            //戶數
            sh.Cells[DataRowNumber, 5].Value = Data.FUnit;
            sh.Cells[DataRowNumber, 5].Style.Font.Size = 12;
            //設施面積(公頃)
            sh.Column(5).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 6].Value = Data.FArea;
            sh.Cells[DataRowNumber, 6].Style.Font.Size = 12;

            #endregion
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion
        #endregion

        #region 歷年鄉鎮統計表
        public byte[] GetTownReportData(List<TownReportView> Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/TownStatisticsReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["鄉鎮"];

            int count = Data.Count;

            for (int k = 1; k <= count; k++)
            {
                sheet = SetTownReportData(sheet, Data[k], k);
            }
            byte[] file = excel.GetAsByteArray();
            #endregion
            return file;
        }
        #region 設定鄉鎮統計表
        public ExcelWorksheet SetTownReportData(ExcelWorksheet sh, TownReportView Data, int count)
        {
            
            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;
            //int startRowNumber = sh.Dimension.Start.Row;
            //int endRowNumber = sh.Dimension.End.Row;
            //int DataRowNumber = endRowNumber + 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";
            }//End for

            sh.Cells[1, 1].Value = Data.TIA;
            //if(count == 1)
            //{
            //    sh.Cells[1, 6].Value = "第" + pagecount + "頁";
            //}

            #region
            int startRow = sh.Dimension.Start.Row;
            int endRow = sh.Dimension.End.Row;
            int DataRowNumber = 0; ;

            //int k = endRow / 45;
            //int m = endRow - (k * 45);
            //int mod = 45 - m;
            //int checknumber = m + count + 1;
            if (endRow % 29 == 0)
            {
                DataRowNumber = endRow + 4;
                sh.Cells["A1:M1"].Copy(sh.Cells["A" + (endRow + 1).ToString() + ":M" + (endRow + 1).ToString()]);
                sh.Cells["A2:A3"].Copy(sh.Cells["A" + (endRow + 2).ToString() + ":A" + (endRow + 3).ToString()]);
                sh.Cells[endRow + 2, 1, endRow + 3, 1].Merge = true;
                sh.Cells["B2:M2"].Copy(sh.Cells["B" + (endRow + 2).ToString() + ":M" + (endRow + 2).ToString()]);
                sh.Cells["B3:M3"].Copy(sh.Cells["B" + (endRow + 3).ToString() + ":M" + (endRow + 3).ToString()]);
            }
            else
            {
                DataRowNumber = endRow + 1;
            }
            //sh.Row(DataRowNumber).Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 1].Value = Data.TYears;
            sh.Cells[DataRowNumber, 1].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            
            sh.Cells[DataRowNumber, 2].Value = Data.TUnitA;
            sh.Cells[DataRowNumber, 2].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            
            sh.Column(3).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 3].Value = Data.TAreaA;
            sh.Cells[DataRowNumber, 3].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            
            sh.Cells[DataRowNumber, 4].Value = Data.TUnitB;
            sh.Cells[DataRowNumber, 4].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            
            sh.Column(5).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 5].Value = Data.TAreaB;
            sh.Cells[DataRowNumber, 5].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            
            sh.Cells[DataRowNumber, 6].Value = Data.TUnitC;
            sh.Cells[DataRowNumber, 6].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            
            sh.Column(7).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 7].Value = Data.TAreaC;
            sh.Cells[DataRowNumber, 7].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            
            sh.Cells[DataRowNumber, 8].Value = Data.TUnitD;
            sh.Cells[DataRowNumber, 8].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            
            sh.Column(9).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 9].Value = Data.TAreaD;
            sh.Cells[DataRowNumber, 9].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            
            sh.Cells[DataRowNumber, 10].Value = Data.TUnitE;
            sh.Cells[DataRowNumber, 10].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            
            sh.Column(3).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 11].Value = Data.TAreaE;
            sh.Cells[DataRowNumber, 11].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            
            sh.Column(10).Style.Numberformat.Format = "0.00";
            sh.Cells[DataRowNumber, 12].Style.Font.Size = 12;
            string Bt = "B" + DataRowNumber;
            string Dt = "D" + DataRowNumber;
            string Ft = "F" + DataRowNumber;
            string Ht = "H" + DataRowNumber;
            string Jt = "J" + DataRowNumber;
            sh.Cells[DataRowNumber, 12].Formula = "+SUM(" + Bt + "," + Dt + "," + Ft + "," + Ht + "," + Jt + ")";
            sh.Cells[DataRowNumber, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            string Ct = "C" + DataRowNumber;
            string Et = "E" + DataRowNumber;
            string Gt = "G" + DataRowNumber;
            string It = "I" + DataRowNumber;
            string Kt = "K" + DataRowNumber;
            sh.Cells[DataRowNumber, 13].Formula = "+SUM(" + Ct + "," + Et + "," + Gt + "," + It + "," + Kt + ")";
            sh.Cells[DataRowNumber, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            #endregion
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion
        #endregion

        #region 農地筆數統計表
        public byte[] GetFarmCountReportData(List<FarmCountReportView> Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/FarmCountReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["農地筆數"];

            int count = Data.Count;

            for (int k = 1; k <= count; k++)
            {
                sheet = SetFarmCountReportData(sheet, Data[k], k);
            }
            byte[] file = excel.GetAsByteArray();
            #endregion
            return file;
        }
        #region 設定農地筆數統計表
        public ExcelWorksheet SetFarmCountReportData(ExcelWorksheet sh, FarmCountReportView Data, int count)
        {
            
            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;
            //int startRowNumber = sh.Dimension.Start.Row;
            //int endRowNumber = sh.Dimension.End.Row;
            //int DataRowNumber = endRowNumber + 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";
            }//End for

            //sh.Cells[1, 1].Value = "瑠公水利會";
            //if(count == 1)
            //{
            //    sh.Cells[1, 6].Value = "第" + pagecount + "頁";
            //}

            #region
            int startRow = sh.Dimension.Start.Row;
            int endRow = sh.Dimension.End.Row;
            int DataRowNumber = 0; ;

            if (endRow % 44 == 0)
            {
                DataRowNumber = endRow + 3;
                for (int j = 1; j <= 2; j++)
                {
                    sh.Cells["A" + j.ToString() + ":C" + j.ToString()].Copy(sh.Cells["A" + (endRow + j).ToString() + ":C" + (endRow + j).ToString()]);
                }
            }
            else
            {
                DataRowNumber = endRow + 1;
            }
            //sh.Row(DataRowNumber).Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 1].Value = Data.Fyears;
            sh.Cells[DataRowNumber, 1].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            
            sh.Cells[DataRowNumber, 2].Value = Data.FCount;
            sh.Cells[DataRowNumber, 2].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            
            sh.Column(3).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 3].Value = Data.FArea;
            sh.Cells[DataRowNumber, 3].Style.Font.Size = 12;
            sh.Cells[DataRowNumber, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 

            #endregion
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion
        #endregion

        #region 歷年受益戶資料清查一覽表
        public byte[] GetBenefitReportData(BenefitReportView Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/BenefitReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["受益戶"];
            int group = Data.Bcountgroup;
            //int countpage = Convert.ToInt32(Math.Floor((double)count/39));
            for (int k = 1; k <= group; k++)
            {
                int countgroup = Data.Bdatacount;
                for (int i = 1; i <= countgroup; i++)
                {
                    sheet = SetBenefitReportData(sheet, Data, i, countgroup);
                }
            }
            byte[] file = excel.GetAsByteArray();
            #endregion
            return file;
        }
        #region 設定歷年受益戶資料清查一覽表
        public ExcelWorksheet SetBenefitReportData(ExcelWorksheet sh, BenefitReportView Data, int count, int countgroup)
        {
            sh.Cells[1, 1].Value = Data.BIA + "歷年";
            sh.Cells[1, 5].Value = "(" + Data.BYearS + "年~" + Data.BYearE + "年)";

            //sh.Cells[2, 1].Value = "頁數:" ;
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;
            //int startRowNumber = sh.Dimension.Start.Row;
            //int endRowNumber = sh.Dimension.End.Row;
            //int DataRowNumber = endRowNumber + 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";
            }//End for
            #region
            int startRow = sh.Dimension.Start.Row;
            int endRow = sh.Dimension.End.Row;
            int DataRowNumber = 0;

            int k = endRow / 44;
            int m = endRow - (k * 44);
            int mod = 44 - m;
            int checknumber = m + countgroup + 1;
            if (count == 1 && checknumber > 44)
            {
                DataRowNumber = endRow + 4 + mod + 1;
                //for (int j = 1; j <= 4; j++)
                //{
                //    sh.Cells["A" + j.ToString() + ":S" + j.ToString()].Copy(sh.Cells["A" + (endRow +mod + j).ToString() + ":S" + (endRow +mod + j).ToString()]);
                //    sh.Row(endRow + mod + j).Height = 27;
                //}
                sh.Cells["A" + 1.ToString() + ":S" + 1.ToString()].Copy(sh.Cells["A" + (endRow + mod + 1).ToString() + ":S" + (endRow + mod + 1).ToString()]);
                
                sh.Cells["A" + 2.ToString() + ":S" + 2.ToString()].Copy(sh.Cells["A" + (endRow + mod + 2).ToString() + ":S" + (endRow + mod + 2).ToString()]);
                
                sh.Cells["A" + 3.ToString() + ":S" + 3.ToString()].Copy(sh.Cells["A" + (endRow + mod + 3).ToString() + ":S" + (endRow + mod + 3).ToString()]);

                sh.Cells["A" + 4.ToString() + ":S" + 4.ToString()].Copy(sh.Cells["A" + (endRow + mod + 4).ToString() + ":S" + (endRow + mod + 4).ToString()]);
                //sh.Cells[endRow + mod + 2, 1, endRow + mod + 4, 1].Merge = true;
                sh.Row(endRow + mod + 1).Height = 27;
                sh.Row(endRow + mod + 2).Height = 27;
                sh.Row(endRow + mod + 3).Height = 27;
                sh.Row(endRow + mod + 4).Height = 27;
            }
            else
            {
                DataRowNumber = endRow + 1;
            }

            //sh.Row(DataRowNumber).Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 1].Value = Data.BApplicationYear;
            sh.Cells[DataRowNumber, 1].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 2].Value = Data.BNo;
            sh.Cells[DataRowNumber, 2].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 3].Value = Data.BName;
            sh.Cells[DataRowNumber, 3].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 4].Value = Data.BRoad;
            sh.Cells[DataRowNumber, 4].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 5].Value = Data.BRoadNumber;
            sh.Cells[DataRowNumber, 5].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 6].Value = Data.BPerson;
            sh.Cells[DataRowNumber, 6].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 7].Value = Data.BProportion;
            sh.Cells[DataRowNumber, 7].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 8].Value = Data.BLandArea;
            sh.Cells[DataRowNumber, 8].Style.Font.Size = 9;
            
            sh.Column(9).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 9].Value = Data.BIAArea;
            sh.Cells[DataRowNumber, 9].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 10].Value = Data.BReason;
            sh.Cells[DataRowNumber, 10].Style.Font.Size = 9;
            
            sh.Column(11).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 11].Value = Data.BPersonArea;
            sh.Cells[DataRowNumber, 11].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 12].Value = Data.BRange;
            sh.Cells[DataRowNumber, 12].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 13].Value = Data.BRegistration;
            sh.Cells[DataRowNumber, 13].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 14].Value = Data.BProvidedY;
            sh.Cells[DataRowNumber, 14].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 15].Value = Data.BProvidedN;
            sh.Cells[DataRowNumber, 15].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 16].Value = Data.BProvidedContent;
            sh.Cells[DataRowNumber, 16].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 17].Value = Data.BDecision;
            sh.Cells[DataRowNumber, 17].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 18].Value = Data.BX;
            sh.Cells[DataRowNumber, 18].Style.Font.Size = 9;
            
            sh.Cells[DataRowNumber, 19].Value = Data.BY;
            sh.Cells[DataRowNumber, 19].Style.Font.Size = 9;

            #endregion
            return sh;
        }
        #endregion
        #endregion

        #region 普查卡
        public byte[] GetSurveyReportData(SurveyReportView Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/SurveyReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["普查卡"];
            
            sheet.Cells[1, 4].Value = "(電動式,柴油,汽油)";
            
            sheet.Cells[1, 9].Value = "(鋁合金,RC結構,磚造,不銹鋼,塑膠類)";
            
            sheet.Cells[1, 19].Value = "(微噴,噴灌,穿孔管,滴灌,軟管澆灌)  ";
            int group = 5;
            if (group > 1)
            {
                for (int i = 2; i <= group; i++)
                {
                    int number = ((i - 1) * 26);
                    #region ROW高度
                    sheet.Row(number + 2).Height = 10;
                    sheet.Row(number + 3).Height = 35;
                    sheet.Row(number + 4).Height = 25;
                    sheet.Row(number + 5).Height = 25;
                    sheet.Row(number + 6).Height = 25;
                    sheet.Row(number + 7).Height = 25;
                    sheet.Row(number + 8).Height = 25;
                    sheet.Row(number + 9).Height = 25;
                    sheet.Row(number + 10).Height = 25;
                    sheet.Row(number + 11).Height = 20;
                    sheet.Row(number + 12).Height = 20;
                    sheet.Row(number + 13).Height = 20;
                    sheet.Row(number + 14).Height = 20;
                    sheet.Row(number + 15).Height = 20;
                    sheet.Row(number + 16).Height = 20;
                    sheet.Row(number + 17).Height = 20;
                    sheet.Row(number + 18).Height = 20;
                    sheet.Row(number + 19).Height = 20;
                    sheet.Row(number + 20).Height = 20;
                    sheet.Row(number + 21).Height = 20;
                    sheet.Row(number + 22).Height = 20;
                    sheet.Row(number + 23).Height = 20;
                    sheet.Row(number + 24).Height = 20;
                    sheet.Row(number + 25).Height = 25;
                    #endregion
                    for (int j = 1; j <= 26; j++)
                    {
                        if (j == 6 || j == 7)
                        {
                            //pass row[6],row[7]
                        }
                        else
                            sheet.Cells["A" + j.ToString() + ":Y" + j.ToString()].Copy(sheet.Cells["A" + (number + j).ToString() + ":Y" + (number + j).ToString()]);
                    }
                    #region 表單調整1(工作站別ROW)
                    
                    sheet.Select("A" + (number + 6).ToString() + ":B" + (number + 6 + 1).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["C" + (number + 6).ToString() + ":C" + (number + 6 + 1).ToString()].Merge = true;
                    sheet.Cells["C" + (number + 6).ToString() + ":C" + (number + 6 + 1).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["D" + (number + 6).ToString() + ":H" + (number + 6).ToString()].Merge = true;
                    sheet.Cells["D" + (number + 6 + 1).ToString() + ":H" + (number + 6 + 1).ToString()].Merge = true;
                    sheet.Select("D" + (number + 6).ToString() + ":H" + (number + 6 + 1).ToString());
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["I" + (number + 6).ToString() + ":O" + (number + 6).ToString()].Merge = true;
                    sheet.Cells["I" + (number + 6 + 1).ToString() + ":O" + (number + 6 + 1).ToString()].Merge = true;
                    sheet.Select("I" + (number + 6).ToString() + ":O" + (number + 6 + 1).ToString());
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Select("P" + (number + 6).ToString() + ":R" + (number + 6 + 1).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["S" + (number + 6).ToString() + ":Y" + (number + 6).ToString()].Merge = true;
                    sheet.Cells["S" + (number + 6 + 1).ToString() + ":Y" + (number + 6 + 1).ToString()].Merge = true;
                    sheet.Select("S" + (number + 6).ToString() + ":Y" + (number + 6 + 1).ToString());
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    #endregion

                    #region 表單調整2(工作站別ROW刻文字)
                    
                    sheet.Cells[(number + 7), 4].Style.Font.Size = 10;
                    sheet.Cells[(number + 7), 4].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    sheet.Cells[(number + 7), 4].Value = "(電動式,柴油,汽油)";
                    
                    sheet.Cells[(number + 7), 9].Style.Font.Size = 10;
                    sheet.Cells[(number + 7), 9].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    sheet.Cells[(number + 7), 9].Value = "(鋁合金,RC結構,磚造,不銹鋼,塑膠類)";
                    
                    sheet.Cells[(number + 6), 16].Value = "□使用中";
                    sheet.Cells[(number + 6), 16].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    
                    sheet.Cells[(number + 7), 19].Style.Font.Size = 10;
                    sheet.Cells[(number + 7), 19].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                    sheet.Cells[(number + 7), 19].Value = "(微噴,噴灌,穿孔管,滴灌,軟管澆灌)  ";
                    #endregion

                    #region 表單調整3(地籍卡ROW)
                    
                    sheet.Cells["A" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["A" + (number + 9).ToString()].Merge = false;
                    sheet.Cells["A" + (number + 10).ToString()].Merge = false;
                    sheet.Cells["A" + (number + 8).ToString() + ":A" + (number + 10).ToString()].Merge = true;
                    sheet.Cells["A" + (number + 8).ToString() + ":A" + (number + 10).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Select("B" + (number + 8).ToString() + ":C" + (number + 8).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("B" + (number + 9).ToString() + ":C" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("B" + (number + 10).ToString() + ":C" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("B" + (number + 8).ToString() + ":C" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["D" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["D" + (number + 9).ToString()].Merge = false;
                    sheet.Cells["D" + (number + 10).ToString()].Merge = false;
                    sheet.Cells["D" + (number + 8).ToString() + ":D" + (number + 10).ToString()].Merge = true;
                    sheet.Cells["D" + (number + 10).ToString() + ":D" + (number + 10).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["E" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["E" + (number + 9).ToString()].Merge = false;
                    sheet.Cells["E" + (number + 10).ToString()].Merge = false;
                    sheet.Cells["E" + (number + 8).ToString() + ":E" + (number + 10).ToString()].Merge = true;
                    sheet.Cells["E" + (number + 8).ToString() + ":E" + (number + 10).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Select("F" + (number + 8).ToString() + ":G" + (number + 8).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("F" + (number + 9).ToString() + ":G" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("F" + (number + 10).ToString() + ":G" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("F" + (number + 8).ToString() + ":G" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Select("K" + (number + 9).ToString() + ":L" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("K" + (number + 10).ToString() + ":L" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("K" + (number + 9).ToString() + ":L" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["M" + (number + 9).ToString()].Merge = false;
                    sheet.Cells["M" + (number + 10).ToString()].Merge = false;
                    sheet.Cells["M" + (number + 9).ToString() + ":M" + (number + 10).ToString()].Merge = true;
                    sheet.Cells["M" + (number + 9).ToString() + ":M" + (number + 10).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Select("R" + (number + 9).ToString() + ":T" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("R" + (number + 10).ToString() + ":T" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("R" + (number + 9).ToString() + ":T" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["U" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["U" + (number + 9).ToString()].Merge = false;
                    sheet.Cells["U" + (number + 10).ToString()].Merge = false;
                    sheet.Cells["U" + (number + 8).ToString() + ":U" + (number + 10).ToString()].Merge = true;
                    sheet.Cells["U" + (number + 8).ToString() + ":U" + (number + 10).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Select("V" + (number + 8).ToString() + ":W" + (number + 8).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("V" + (number + 9).ToString() + ":W" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("V" + (number + 10).ToString() + ":W" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("V" + (number + 8).ToString() + ":W" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Select("X" + (number + 8).ToString() + ":Y" + (number + 8).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("X" + (number + 9).ToString() + ":Y" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("X" + (number + 10).ToString() + ":Y" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = false;
                    sheet.Select("X" + (number + 8).ToString() + ":Y" + (number + 10).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    #endregion

                    #region 地籍卡資料TABLE調整
                    for (int k = 0; k < 7; k++)
                    {
                        
                        sheet.Cells["A" + (number + 11 + k * 2).ToString()].Merge = false;
                        sheet.Cells["A" + (number + 12 + k * 2).ToString()].Merge = false;
                        sheet.Cells["A" + (number + 11 + k * 2).ToString() + ":A" + (number + 12 + k * 2).ToString()].Merge = true;
                        sheet.Cells["A" + (number + 11 + k * 2).ToString() + ":A" + (number + 12 + k * 2).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Select("B" + (number + 11 + k * 2).ToString() + ":C" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("B" + (number + 12 + k * 2).ToString() + ":C" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("B" + (number + 11 + k * 2).ToString() + ":C" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["D" + (number + 11 + k * 2).ToString()].Merge = false;
                        sheet.Cells["D" + (number + 12 + k * 2).ToString()].Merge = false;
                        sheet.Cells["D" + (number + 11 + k * 2).ToString() + ":D" + (number + 12 + k * 2).ToString()].Merge = true;
                        sheet.Cells["D" + (number + 11 + k * 2).ToString() + ":D" + (number + 12 + k * 2).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["E" + (number + 11 + k * 2).ToString()].Merge = false;
                        sheet.Cells["E" + (number + 12 + k * 2).ToString()].Merge = false;
                        sheet.Cells["E" + (number + 11 + k * 2).ToString() + ":E" + (number + 12 + k * 2).ToString()].Merge = true;
                        sheet.Cells["E" + (number + 11 + k * 2).ToString() + ":E" + (number + 12 + k * 2).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Select("F" + (number + 11 + k * 2).ToString() + ":G" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("F" + (number + 12 + k * 2).ToString() + ":G" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("F" + (number + 11 + k * 2).ToString() + ":G" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Select("H" + (number + 11 + k * 2).ToString() + ":J" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("H" + (number + 12 + k * 2).ToString() + ":J" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("H" + (number + 11 + k * 2).ToString() + ":J" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Select("K" + (number + 11 + k * 2).ToString() + ":L" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("K" + (number + 12 + k * 2).ToString() + ":L" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("K" + (number + 11 + k * 2).ToString() + ":L" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Select("N" + (number + 11 + k * 2).ToString() + ":Q" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("N" + (number + 12 + k * 2).ToString() + ":Q" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("N" + (number + 11 + k * 2).ToString() + ":Q" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Select("R" + (number + 11 + k * 2).ToString() + ":T" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("R" + (number + 12 + k * 2).ToString() + ":T" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("R" + (number + 11 + k * 2).ToString() + ":T" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["U" + (number + 11 + k * 2).ToString()].Merge = false;
                        sheet.Cells["U" + (number + 12 + k * 2).ToString()].Merge = false;
                        sheet.Cells["U" + (number + 11 + k * 2).ToString() + ":U" + (number + 12 + k * 2).ToString()].Merge = true;
                        sheet.Cells["U" + (number + 11 + k * 2).ToString() + ":U" + (number + 12 + k * 2).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Select("V" + (number + 11 + k * 2).ToString() + ":W" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("V" + (number + 12 + k * 2).ToString() + ":W" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("V" + (number + 11 + k * 2).ToString() + ":W" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Select("X" + (number + 11 + k * 2).ToString() + ":Y" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("X" + (number + 12 + k * 2).ToString() + ":Y" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = false;
                        sheet.Select("X" + (number + 11 + k * 2).ToString() + ":Y" + (number + 12 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    }
                    #endregion
                }
            }
            //int countpage = Convert.ToInt32(Math.Floor((double)count/39));
            for (int k = 1; k <= group; k++)
            {
                int countgroup = 3;
                sheet = SetSurveyReportData(sheet, Data, countgroup,k);
            }
            byte[] file = excel.GetAsByteArray();
            #endregion
            return file;
        }
        #region 設定普查卡
        public ExcelWorksheet SetSurveyReportData(ExcelWorksheet sh, SurveyReportView Data, int countgroup, int count)
        {
            sh.Cells[1, 1].Value = "瑠公管理處";
            sh.Cells[1, 13].Value = "旱作灌溉地區設施戶" + "普查卡";

            //sh.Cells[2, 1].Value = "頁數:" ;
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;
            //int startRowNumber = sh.Dimension.Start.Row;
            //int endRowNumber = sh.Dimension.End.Row;
            //int DataRowNumber = endRowNumber + 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";
            }//End for
            #region 基本資料
            #region copy
            //int startRow = sh.Dimension.Start.Row;//起始列編號，從1算起//
            //int endRow = sh.Dimension.End.Row;//結束列編號，從1算起//
            //int DataRowNumber = 0;
            //int group = 20;//資料筆數
            //if (count != group)//分頁複製
            //{
            //    DataRowNumber = endRow + 2;
            //    for (int j = 1; j <= 26; j++)
            //    {
            //        sh.Cells["A" + j.ToString() + ":Y" + j.ToString()].Copy(sh.Cells["A" + (endRow + j).ToString() + ":Y" + (endRow + j).ToString()]);
            //    }
            //}
            //else
            //{
            //    DataRowNumber = endRow + 1;
            //}
            #endregion
            int number = ((count - 1) * 26);
            sh.Cells[number + 1, 1].Value = "瑠公管理處";
            
            sh.Cells[number + 4, 1].Value = 1;
            sh.Cells[number + 4, 1].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 2].Value = "許正雄";
            sh.Cells[number + 4, 2].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 4].Value = "F100447670";
            sh.Cells[number + 4, 4].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 7].Value = 2;
            sh.Cells[number + 4, 7].Style.Font.Size = 9;
            
            sh.Column(9).Style.Numberformat.Format = "0.0000";
            sh.Cells[number + 4, 10].Value = 130;
            sh.Cells[number + 4, 10].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 12].Value = 102;
            sh.Cells[number + 4, 12].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 15].Value = 18000;
            sh.Cells[number + 4, 15].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 17].Value = 26000;
            sh.Cells[number + 4, 17].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 20].Value = 44000;
            sh.Cells[number + 4, 20].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 23].Value = 0;
            sh.Cells[number + 4, 23].Style.Font.Size = 9;
            
            //sh.Cells[number + 4, 25].Value = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages); ;
            sh.Cells[number + 4, 25].Style.Font.Size = 9;
            #endregion
            #region 工作站別ROW
            
            sh.Cells[number + 6, 1].Value = "工作站";
            sh.Cells[number + 6, 1].Style.Font.Size = 9;
            sh.Cells[number + 6, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            
            sh.Cells[number + 6, 3].Value = "小組";
            sh.Cells[number + 6, 3].Style.Font.Size = 9;
            sh.Cells[number + 6, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            
            sh.Cells[number + 6, 4].Value = "___" + "馬力  □使用中";
            sh.Cells[number + 6, 4].Style.Font.Size = 9;
            sh.Cells[number + 6, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            
            sh.Cells[number + 6, 9].Value = 2 + "噸" + 1 + "座" + " □ 使用中";
            sh.Cells[number + 6, 9].Style.Font.Size = 9;
            sh.Cells[number + 6, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            
            sh.Cells[number + 6, 19].Value = "地表定置式微噴" + " □ 使用中";
            sh.Cells[number + 6, 19].Style.Font.Size = 9;
            sh.Cells[number + 6, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
            #endregion
            #region 地籍卡TABLE
            for (int i = 0; i < countgroup; i++)
            {
                
                sh.Cells[number + 11 + i * 2, 1].Value = "地1";
                sh.Cells[number + 11 + i * 2, 1].Style.Font.Size = 9;
                
                sh.Cells[number + 11 + i * 2, 2].Value = "深坑區土庫段賴仲坑小段";
                sh.Cells[number + 11 + i * 2, 2].Style.Font.Size = 9;
                
                sh.Cells[number + 11 + i * 2, 4].Value = "144-6";
                sh.Cells[number + 11 + i * 2, 4].Style.Font.Size = 9;
                
                sh.Column(number + 11 + i * 2).Style.Numberformat.Format = "0.0000";
                sh.Cells[number + 11 + i * 2, 5].Value = 0.128;
                sh.Cells[number + 11 + i * 2, 5].Style.Font.Size = 9;
                
                sh.Column(number + 11 + i * 2).Style.Numberformat.Format = "0.0000";
                sh.Cells[number + 11 + i * 2, 6].Value = 0.084;
                sh.Cells[number + 11 + i * 2, 6].Style.Font.Size = 9;
                
                sh.Cells[number + 11 + i * 2, 8].Value = "許正雄";
                sh.Cells[number + 11 + i * 2, 8].Style.Font.Size = 9;
                
                sh.Cells[number + 11 + i * 2, 11].Value = "F1004476710";
                sh.Cells[number + 11 + i * 2, 11].Style.Font.Size = 9;
                
                sh.Cells[number + 11 + i * 2, 13].Value = 3;
                sh.Cells[number + 11 + i * 2, 13].Style.Font.Size = 9;
                sh.Cells[number + 12 + i * 2, 13].Value = 1;
                sh.Cells[number + 12 + i * 2, 13].Style.Font.Size = 9;
                
                sh.Cells[number + 11 + i * 2, 14].Value = "地政地址";
                sh.Cells[number + 11 + i * 2, 14].Style.Font.Size = 9;
                
                sh.Cells[number + 11 + i * 2, 18].Value = "3567846";
                sh.Cells[number + 11 + i * 2, 18].Style.Font.Size = 9;
                
                sh.Cells[number + 11 + i * 2, 21].Value = "無";
                sh.Cells[number + 11 + i * 2, 21].Style.Font.Size = 9;
                
                sh.Cells[number + 11 + i * 2, 22].Value = "無";
                sh.Cells[number + 11 + i * 2, 22].Style.Font.Size = 9;
                
                sh.Cells[number + 11 + i * 2, 24].Value = "無";
                sh.Cells[number + 11 + i * 2, 24].Style.Font.Size = 9;
            }
            #endregion

            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

            return sh;
        }
        #endregion
        #endregion

        #region 地籍卡
        public byte[] GetLandReportData(LandReportView Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/LandReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["地籍卡"];

            int group = 5;
            if (group > 1)
            {
                for (int i = 2; i <= group; i++)
                {
                    int number = ((i - 1) * 27);
                    #region ROW高度
                    sheet.Row(number + 2).Height = 10;
                    sheet.Row(number + 3).Height = 35;
                    sheet.Row(number + 4).Height = 25;
                    sheet.Row(number + 5).Height = 25;
                    sheet.Row(number + 6).Height = 25;
                    sheet.Row(number + 7).Height = 30;
                    sheet.Row(number + 8).Height = 20;
                    sheet.Row(number + 9).Height = 20;
                    sheet.Row(number + 10).Height = 20;
                    sheet.Row(number + 11).Height = 20;
                    sheet.Row(number + 12).Height = 20;
                    sheet.Row(number + 13).Height = 20;
                    sheet.Row(number + 14).Height = 20;
                    sheet.Row(number + 15).Height = 20;
                    sheet.Row(number + 16).Height = 20;
                    sheet.Row(number + 17).Height = 20;
                    sheet.Row(number + 18).Height = 20;
                    sheet.Row(number + 19).Height = 20;
                    sheet.Row(number + 20).Height = 20;
                    sheet.Row(number + 21).Height = 20;
                    sheet.Row(number + 22).Height = 20;
                    sheet.Row(number + 23).Height = 20;
                    sheet.Row(number + 24).Height = 20;
                    sheet.Row(number + 25).Height = 20;
                    sheet.Row(number + 26).Height = 25;
                    #endregion
                    for (int j = 1; j <= 27; j++)
                    {
                        sheet.Cells["A" + j.ToString() + ":U" + j.ToString()].Copy(sheet.Cells["A" + (number + j).ToString() + ":U" + (number + j).ToString()]);
                    }

                    #region 表單調整(持分人ROW)
                    
                    sheet.Cells["A" + (number + 8).ToString() + ":B" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["A" + (number + 9).ToString() + ":B" + (number + 9).ToString()].Merge = false;
                    sheet.Cells["A" + (number + 8).ToString() + ":B" + (number + 9).ToString()].Merge = true;
                    sheet.Cells["A" + (number + 8).ToString() + ":B" + (number + 9).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["C" + (number + 8).ToString() + ":E" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["C" + (number + 9).ToString() + ":E" + (number + 9).ToString()].Merge = false;
                    sheet.Select("C" + (number + 8).ToString() + ":E" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    
                    sheet.Cells["F" + (number + 8).ToString() + ":G" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["F" + (number + 9).ToString() + ":G" + (number + 9).ToString()].Merge = false;
                    sheet.Select("F" + (number + 8).ToString() + ":G" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["H" + (number + 8).ToString() + ":I" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["H" + (number + 9).ToString() + ":I" + (number + 9).ToString()].Merge = false;
                    sheet.Select("H" + (number + 8).ToString() + ":I" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["J" + (number + 8).ToString() + ":L" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["J" + (number + 9).ToString() + ":L" + (number + 9).ToString()].Merge = false;
                    sheet.Select("J" + (number + 8).ToString() + ":L" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["R" + (number + 7).ToString() + ":T" + (number + 7).ToString()].Merge = false;
                    sheet.Cells["R" + (number + 8).ToString() + ":T" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["R" + (number + 9).ToString() + ":T" + (number + 9).ToString()].Merge = false;
                    sheet.Select("R" + (number + 7).ToString() + ":T" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["U" + (number + 7).ToString()].Merge = false;
                    sheet.Cells["U" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["U" + (number + 9).ToString()].Merge = false;
                    sheet.Cells["U" + (number + 7).ToString() + ":U" + (number + 9).ToString()].Merge = true;
                    sheet.Cells["U" + (number + 7).ToString() + ":U" + (number + 9).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    #endregion

                    #region 持分人資料TABLE調整
                    for (int k = 0; k < 8; k++)
                    {
                        
                        sheet.Cells["A" + (number + 10 + k * 2).ToString() + ":B" + (number + 10 + k * 2).ToString()].Merge = false;
                        sheet.Cells["A" + (number + 11 + k * 2).ToString() + ":B" + (number + 11 + k * 2).ToString()].Merge = false;
                        sheet.Cells["A" + (number + 10 + k * 2).ToString() + ":B" + (number + 11 + k * 2).ToString()].Merge = true;
                        sheet.Cells["A" + (number + 10 + k * 2).ToString() + ":B" + (number + 11 + k * 2).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["C" + (number + 10 + k * 2).ToString() + ":E" + (number + 10 + k * 2).ToString()].Merge = false;
                        sheet.Cells["C" + (number + 11 + k * 2).ToString() + ":E" + (number + 11 + k * 2).ToString()].Merge = false;
                        sheet.Select("C" + (number + 10 + k * 2).ToString() + ":E" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["F" + (number + 10 + k * 2).ToString() + ":G" + (number + 10 + k * 2).ToString()].Merge = false;
                        sheet.Cells["F" + (number + 11 + k * 2).ToString() + ":G" + (number + 11 + k * 2).ToString()].Merge = false;
                        sheet.Select("F" + (number + 10 + k * 2).ToString() + ":G" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["J" + (number + 10 + k * 2).ToString() + ":L" + (number + 10 + k * 2).ToString()].Merge = false;
                        sheet.Cells["J" + (number + 11 + k * 2).ToString() + ":L" + (number + 11 + k * 2).ToString()].Merge = false;
                        sheet.Select("J" + (number + 10 + k * 2).ToString() + ":L" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        
                        sheet.Cells["R" + (number + 10 + k * 2).ToString() + ":T" + (number + 10 + k * 2).ToString()].Merge = false;
                        sheet.Cells["R" + (number + 11 + k * 2).ToString() + ":T" + (number + 11 + k * 2).ToString()].Merge = false;
                        sheet.Select("R" + (number + 10 + k * 2).ToString() + ":T" + (number + 11 + k * 2).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    }
                    #endregion
                }
            }
            //int countpage = Convert.ToInt32(Math.Floor((double)count/39));
            for (int k = 1; k <= group; k++)
            {
                int countgroup = 3;
                sheet = SetLandReportData(sheet, Data, countgroup, k);
            }
            byte[] file = excel.GetAsByteArray();
            #endregion
            return file;
        }
        #region 設定地籍卡
        public ExcelWorksheet SetLandReportData(ExcelWorksheet sh, LandReportView Data, int countgroup, int count)
        {
            sh.Cells[1, 1].Value = "瑠公管理處";
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;
            //int startRowNumber = sh.Dimension.Start.Row;
            //int endRowNumber = sh.Dimension.End.Row;
            //int DataRowNumber = endRowNumber + 1;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";
            }//End for
            #region 基本資料
            int number = ((count - 1) * 27);
            sh.Cells[number + 1, 1].Value = "瑠公管理處";
            
            sh.Cells[number + 4, 1].Value = "工作站別";
            sh.Cells[number + 4, 1].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 2].Value = "小組別";
            sh.Cells[number + 4, 2].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 4].Value = "深坑區";
            sh.Cells[number + 4, 4].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 7].Value = "深坑區土庫段賴仲坑小段";
            sh.Cells[number + 4, 7].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 11].Value = 144-6;
            sh.Cells[number + 4, 11].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 12].Style.Numberformat.Format = "0.0000";
            sh.Cells[number + 4, 12].Value = 128;
            sh.Cells[number + 4, 12].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 14].Style.Numberformat.Format = "0.0000";
            sh.Cells[number + 4, 14].Value = 84;
            sh.Cells[number + 4, 14].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 16].Value = "地目等則";
            sh.Cells[number + 4, 16].Style.Font.Size = 9;
            
            //sh.Cells[number + 4, 25].Value = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages); ;
            sh.Cells[number + 4, 20].Style.Font.Size = 9;
            #endregion
            #region 申請案號ROW
            
            sh.Cells[number + 6, 1].Value = 1;
            sh.Cells[number + 6, 1].Style.Font.Size = 9;
            //sh.Cells[number + 6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; 
            
            sh.Cells[number + 6, 3].Value = 102;
            sh.Cells[number + 6, 3].Style.Font.Size = 9;
            //sh.Cells[number + 6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 5].Value = "___" + "馬力  □使用中";
            sh.Cells[number + 6, 5].Style.Font.Size = 9;
            //sh.Cells[number + 6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 9].Value = 2 + "噸" + 1 + "座" + " □ 使用中";
            sh.Cells[number + 6, 9].Style.Font.Size = 9;
            //sh.Cells[number + 6, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 12].Value = "□ 使用中";
            sh.Cells[number + 6, 12].Style.Font.Size = 9;
            //sh.Cells[number + 6, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 15].Value = "軟管澆管" + " □ 使用中";
            sh.Cells[number + 6, 15].Style.Font.Size = 9;
            //sh.Cells[number + 6, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 17].Value = "使用分區";
            sh.Cells[number + 6, 17].Style.Font.Size = 9;
            //sh.Cells[number + 6, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 19].Value = "使用地類別";
            sh.Cells[number + 6, 19].Style.Font.Size = 9;
            //sh.Cells[number + 6, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 7, 1].Value = "所有權人(共"+ 3 +"人)";
            sh.Cells[number + 7, 1].Style.Font.Size = 9;
            //sh.Cells[number + 6, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; 
            #endregion
            #region 持分人TABLE
            for (int i = 0; i < countgroup; i++)
            {
                
                sh.Cells[number + 10 + i * 2, 1].Value = "地1";
                sh.Cells[number + 10 + i * 2, 1].Style.Font.Size = 9;
                
                sh.Cells[number + 10 + i * 2, 3].Value = 232.3;
                sh.Cells[number + 10 + i * 2, 3].Style.Font.Size = 9;
                sh.Cells[number + 10 + i * 2, 3].Style.Numberformat.Format = "0.0000";
                
                sh.Cells[number + 10 + i * 2, 6].Value = "自耕";
                sh.Cells[number + 10 + i * 2, 6].Style.Font.Size = 9;
                
                sh.Cells[number + 10 + i * 2, 8].Value = 1;
                sh.Cells[number + 10 + i * 2, 8].Style.Font.Size = 9;
                sh.Cells[number + 10 + i * 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sh.Cells[number + 11 + i * 2, 8].Value = 1;
                sh.Cells[number + 11 + i * 2, 8].Style.Font.Size = 9;
                sh.Cells[number + 11 + i * 2, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                
                sh.Cells[number + 10 + i * 2, 10].Value = "F100361586";
                sh.Cells[number + 10 + i * 2, 10].Style.Font.Size = 9;
                
                sh.Cells[number + 10 + i * 2, 13].Value = "周明通";
                sh.Cells[number + 10 + i * 2, 13].Style.Font.Size = 9;
                sh.Cells[number + 11 + i * 2, 13].Value = "周明通";
                sh.Cells[number + 11 + i * 2, 13].Style.Font.Size = 9;
                
                sh.Cells[number + 10 + i * 2, 14].Value = "新北市新店區";
                sh.Cells[number + 10 + i * 2, 14].Style.Font.Size = 9;
                sh.Cells[number + 11 + i * 2, 14].Value = "新北市新店區";
                sh.Cells[number + 11 + i * 2, 14].Style.Font.Size = 9;
                sh.Cells[number + 10 + i * 2, 15].Value = "新北市新店區塗潭里8鄰新潭路2段62巷11弄8號";
                sh.Cells[number + 10 + i * 2, 15].Style.Font.Size = 9;
                sh.Cells[number + 11 + i * 2, 15].Value = "新北市新店區塗潭里8鄰新潭路2段62巷11弄8號";
                sh.Cells[number + 11 + i * 2, 15].Style.Font.Size = 9;
                
                sh.Cells[number + 10 + i * 2, 18].Value = "無";
                sh.Cells[number + 10 + i * 2, 18].Style.Font.Size = 9;
                
                sh.Cells[number + 10 + i * 2, 21].Value = "買賣";
                sh.Cells[number + 10 + i * 2, 21].Style.Font.Size = 9;
                sh.Cells[number + 11 + i * 2, 21].Value = "67 110";
                sh.Cells[number + 11 + i * 2, 21].Style.Font.Size = 9;
            }
            #endregion

            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

            return sh;
        }
        #endregion
        #endregion

        #region 基本資料卡
        public byte[] GetPersonReportData(PersonReportView Data)
        {
            #region basic Data
            string sample_Path = @"~/ReportSample/PersonReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["基本資料卡"];

            int group = 5;
            if (group > 1)
            {
                for (int i = 2; i <= group; i++)
                {
                    int number = ((i - 1) * 27);
                    #region ROW高度
                    sheet.Row(number + 2).Height = 10;
                    sheet.Row(number + 3).Height = 35;
                    sheet.Row(number + 4).Height = 25;
                    sheet.Row(number + 5).Height = 25;
                    sheet.Row(number + 6).Height = 25;
                    sheet.Row(number + 7).Height = 20;
                    sheet.Row(number + 8).Height = 20;
                    sheet.Row(number + 9).Height = 25;
                    sheet.Row(number + 10).Height = 20;
                    sheet.Row(number + 11).Height = 20;
                    sheet.Row(number + 12).Height = 20;
                    sheet.Row(number + 13).Height = 20;
                    sheet.Row(number + 14).Height = 20;
                    sheet.Row(number + 15).Height = 20;
                    sheet.Row(number + 16).Height = 20;
                    sheet.Row(number + 17).Height = 20;
                    sheet.Row(number + 18).Height = 20;
                    sheet.Row(number + 19).Height = 20;
                    sheet.Row(number + 20).Height = 20;
                    sheet.Row(number + 21).Height = 20;
                    sheet.Row(number + 22).Height = 20;
                    sheet.Row(number + 23).Height = 20;
                    sheet.Row(number + 24).Height = 20;
                    sheet.Row(number + 25).Height = 20;
                    sheet.Row(number + 26).Height = 20;
                    #endregion
                    for (int j = 1; j <= 27; j++)
                    {
                        sheet.Cells["A" + j.ToString() + ":V" + j.ToString()].Copy(sheet.Cells["A" + (number + j).ToString() + ":V" + (number + j).ToString()]);
                    }

                    #region 表單調整(地段ROW)
                    
                    sheet.Cells["A" + (number + 7).ToString() + ":B" + (number + 7).ToString()].Merge = false;
                    sheet.Cells["A" + (number + 8).ToString() + ":B" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["A" + (number + 9).ToString() + ":B" + (number + 9).ToString()].Merge = false;
                    sheet.Cells["A" + (number + 7).ToString() + ":B" + (number + 9).ToString()].Merge = true;
                    sheet.Cells["A" + (number + 7).ToString() + ":B" + (number + 9).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["C" + (number + 7).ToString()].Merge = false;
                    sheet.Cells["C" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["C" + (number + 9).ToString()].Merge = false;
                    sheet.Select("C" + (number + 7).ToString() + ":C" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["D" + (number + 8).ToString() + ":E" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["D" + (number + 9).ToString() + ":E" + (number + 9).ToString()].Merge = false;
                    sheet.Select("D" + (number + 8).ToString() + ":E" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["F" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["F" + (number + 9).ToString()].Merge = false;
                    sheet.Select("F" + (number + 8).ToString() + ":F" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["G" + (number + 7).ToString() + ":H" + (number + 7).ToString()].Merge = false;
                    sheet.Cells["G" + (number + 8).ToString() + ":H" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["G" + (number + 9).ToString() + ":H" + (number + 9).ToString()].Merge = false;
                    sheet.Select("G" + (number + 7).ToString() + ":H" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["I" + (number + 7).ToString()].Merge = false;
                    sheet.Cells["I" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["I" + (number + 9).ToString()].Merge = false;
                    sheet.Select("I" + (number + 7).ToString() + ":I" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["J" + (number + 7).ToString() + ":N" + (number + 7).ToString()].Merge = false;
                    sheet.Cells["J" + (number + 8).ToString() + ":N" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["J" + (number + 9).ToString() + ":N" + (number + 9).ToString()].Merge = false;
                    sheet.Cells["J" + (number + 7).ToString() + ":N" + (number + 9).ToString()].Merge = true;
                    sheet.Cells["J" + (number + 7).ToString() + ":N" + (number + 9).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["T" + (number + 8).ToString() + ":U" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["T" + (number + 9).ToString() + ":U" + (number + 9).ToString()].Merge = false;
                    sheet.Cells["T" + (number + 8).ToString() + ":U" + (number + 9).ToString()].Merge = true;
                    sheet.Cells["T" + (number + 8).ToString() + ":U" + (number + 9).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    
                    sheet.Cells["V" + (number + 8).ToString()].Merge = false;
                    sheet.Cells["V" + (number + 9).ToString()].Merge = false;
                    sheet.Select("V" + (number + 8).ToString() + ":V" + (number + 9).ToString());
                    sheet.SelectedRange.Merge = true;
                    sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    #endregion

                    #region 持分人資料TABLE調整
                    for (int k = 0; k < 5; k++)
                    {
                        
                        sheet.Cells["A" + (number + 10 + k * 3).ToString() + ":B" + (number + 10 + k * 3).ToString()].Merge = false;
                        sheet.Cells["A" + (number + 11 + k * 3).ToString() + ":B" + (number + 11 + k * 3).ToString()].Merge = false;
                        sheet.Cells["A" + (number + 12 + k * 3).ToString() + ":B" + (number + 12 + k * 3).ToString()].Merge = false;
                        sheet.Cells["A" + (number + 10 + k * 3).ToString() + ":B" + (number + 12 + k * 3).ToString()].Merge = true;
                        sheet.Cells["A" + (number + 10 + k * 3).ToString() + ":B" + (number + 12 + k * 3).ToString()].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["C" + (number + 10 + k * 3).ToString()].Merge = false;
                        sheet.Cells["C" + (number + 11 + k * 3).ToString()].Merge = false;
                        sheet.Cells["C" + (number + 12 + k * 3).ToString()].Merge = false;
                        sheet.Select("C" + (number + 10 + k * 3).ToString() + ":C" + (number + 12 + k * 3).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["D" + (number + 10 + k * 3).ToString() + ":E" + (number + 10 + k * 3).ToString()].Merge = false;
                        sheet.Cells["D" + (number + 11 + k * 3).ToString() + ":E" + (number + 11 + k * 3).ToString()].Merge = false;
                        sheet.Cells["D" + (number + 12 + k * 3).ToString() + ":E" + (number + 12 + k * 3).ToString()].Merge = false;
                        sheet.Select("D" + (number + 10 + k * 3).ToString() + ":E" + (number + 12 + k * 3).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["F" + (number + 10 + k * 3).ToString()].Merge = false;
                        sheet.Cells["F" + (number + 11 + k * 3).ToString()].Merge = false;
                        sheet.Cells["F" + (number + 12 + k * 3).ToString()].Merge = false;
                        sheet.Select("F" + (number + 10 + k * 3).ToString() + ":F" + (number + 12 + k * 3).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["G" + (number + 10 + k * 3).ToString() + ":H" + (number + 10 + k * 3).ToString()].Merge = false;
                        sheet.Cells["G" + (number + 11 + k * 3).ToString() + ":H" + (number + 11 + k * 3).ToString()].Merge = false;
                        sheet.Cells["G" + (number + 12 + k * 3).ToString() + ":H" + (number + 12 + k * 3).ToString()].Merge = false;
                        sheet.Select("G" + (number + 10 + k * 3).ToString() + ":H" + (number + 12 + k * 3).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["I" + (number + 10 + k * 3).ToString()].Merge = false;
                        sheet.Cells["I" + (number + 11 + k * 3).ToString()].Merge = false;
                        sheet.Cells["I" + (number + 12 + k * 3).ToString()].Merge = false;
                        sheet.Select("I" + (number + 10 + k * 3).ToString() + ":I" + (number + 12 + k * 3).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["O" + (number + 10 + k * 3).ToString() + ":P" + (number + 10 + k * 3).ToString()].Merge = false;
                        sheet.Cells["O" + (number + 11 + k * 3).ToString() + ":P" + (number + 11 + k * 3).ToString()].Merge = false;
                        sheet.Cells["O" + (number + 12 + k * 3).ToString() + ":P" + (number + 12 + k * 3).ToString()].Merge = false;
                        sheet.Select("O" + (number + 10 + k * 3).ToString() + ":P" + (number + 12 + k * 3).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["Q" + (number + 10 + k * 3).ToString() + ":S" + (number + 10 + k * 3).ToString()].Merge = false;
                        sheet.Cells["Q" + (number + 11 + k * 3).ToString() + ":S" + (number + 11 + k * 3).ToString()].Merge = false;
                        sheet.Cells["Q" + (number + 12 + k * 3).ToString() + ":S" + (number + 12 + k * 3).ToString()].Merge = false;
                        sheet.Select("Q" + (number + 10 + k * 3).ToString() + ":S" + (number + 12 + k * 3).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["T" + (number + 10 + k * 3).ToString() + ":U" + (number + 10 + k * 3).ToString()].Merge = false;
                        sheet.Cells["T" + (number + 11 + k * 3).ToString() + ":U" + (number + 11 + k * 3).ToString()].Merge = false;
                        sheet.Cells["T" + (number + 12 + k * 3).ToString() + ":U" + (number + 12 + k * 3).ToString()].Merge = false;
                        sheet.Select("T" + (number + 10 + k * 3).ToString() + ":U" + (number + 12 + k * 3).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                        
                        sheet.Cells["V" + (number + 10 + k * 3).ToString()].Merge = false;
                        sheet.Cells["V" + (number + 11 + k * 3).ToString()].Merge = false;
                        sheet.Cells["V" + (number + 12 + k * 3).ToString()].Merge = false;
                        sheet.Select("V" + (number + 10 + k * 3).ToString() + ":V" + (number + 12 + k * 3).ToString());
                        sheet.SelectedRange.Merge = true;
                        sheet.SelectedRange.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                    }
                    #endregion
                }
            }
            //int countpage = Convert.ToInt32(Math.Floor((double)count/39));
            for (int k = 1; k <= group; k++)
            {
                int countgroup = 3;
                sheet = SetPersonReportData(sheet, Data, countgroup, k);
            }
            byte[] file = excel.GetAsByteArray();
            #endregion
            return file;
        }
        #region 設定基本資料卡
        public ExcelWorksheet SetPersonReportData(ExcelWorksheet sh, PersonReportView Data, int countgroup, int count)
        {
            sh.Cells[1, 1].Value = "瑠公管理處";
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

            int startColumn = sh.Dimension.Start.Column;
            int endColumn = sh.Dimension.End.Column;

            for (int i = startColumn; i <= endColumn; i++)
            {
                sh.Column(i).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                //sheet1.Column(i).Width = 80;
                //sheet1.Column(i).AutoFit();
                //sh.Column(i).Style.WrapText = true;
                sh.Column(i).Style.Font.Name = "標楷體";
            }//End for
            #region 基本資料
            int number = ((count - 1) * 27);
            sh.Cells[number + 1, 1].Value = "瑠公管理處";
            
            sh.Cells[number + 4, 1].Value = 1;
            sh.Cells[number + 4, 1].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 2].Value = "許正雄";
            sh.Cells[number + 4, 2].Style.Font.Size = 9;
            
            string idbooly = "Y";
            if(idbooly == "Y")
            {
                sh.Cells[number + 4, 4].Value = "■有□無";
            }
            else
                sh.Cells[number + 4, 4].Value = "□有■無";
            sh.Cells[number + 4, 4].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 5].Value = 2;
            sh.Cells[number + 4, 5].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 9].Style.Numberformat.Format = "0.0000";
            sh.Cells[number + 4, 9].Value = 130;
            sh.Cells[number + 4, 9].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 12].Style.Numberformat.Format = "#,##0";
            sh.Cells[number + 4, 12].Value = 18000;
            sh.Cells[number + 4, 12].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 13].Style.Numberformat.Format = "#,##0";
            sh.Cells[number + 4, 13].Value = 26000;
            sh.Cells[number + 4, 13].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 16].Style.Numberformat.Format = "#,##0";
            sh.Cells[number + 4, 16].Value = 44000;
            sh.Cells[number + 4, 16].Style.Font.Size = 9;
            
            sh.Cells[number + 4, 18].Value = 1;
            sh.Cells[number + 4, 18].Style.Font.Size = 9;
            
            //sh.Cells[number + 4, 25].Value = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages); ;
            sh.Cells[number + 4, 21].Style.Font.Size = 9;
            #endregion
            #region 地段ROW
            
            sh.Cells[number + 6, 1].Value = 102;
            sh.Cells[number + 6, 1].Style.Font.Size = 9;
            //sh.Cells[number + 6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 2].Value = "新北市深坑區北深路1段229巷1號";
            sh.Cells[number + 6, 2].Style.Font.Size = 9;
            //sh.Cells[number + 6, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 8].Value = "___" + "馬力  □使用中";
            sh.Cells[number + 6, 8].Style.Font.Size = 9;
            //sh.Cells[number + 6, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 11].Value = 2 + "噸" + 1 + "座" + " □ 使用中";
            sh.Cells[number + 6, 11].Style.Font.Size = 9;
            //sh.Cells[number + 6, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 14].Value = "□ 使用中";
            sh.Cells[number + 6, 14].Style.Font.Size = 9;
            //sh.Cells[number + 6, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            
            sh.Cells[number + 6, 19].Value = "軟管澆管" + " □ 使用中";
            sh.Cells[number + 6, 19].Style.Font.Size = 9;
            //sh.Cells[number + 6, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            #endregion
            #region 持分人TABLE
            for (int i = 0; i < countgroup; i++)
            {
                
                sh.Cells[number + 10 + i * 3, 1].Value = "(1)深坑區土庫段賴仲坑小段";
                sh.Cells[number + 10 + i * 3, 1].Style.Font.Size = 9;
                
                sh.Cells[number + 10 + i * 3, 3].Value = 144-6;
                sh.Cells[number + 10 + i * 3, 3].Style.Font.Size = 9;
                
                sh.Cells[number + 10 + i * 3, 4].Value = 0.128;
                sh.Cells[number + 10 + i * 3, 4].Style.Font.Size = 9;
                sh.Cells[number + 10 + i * 3, 4].Style.Numberformat.Format = "0.0000";
                
                sh.Cells[number + 10 + i * 3, 6].Value = 0.084;
                sh.Cells[number + 10 + i * 3, 6].Style.Font.Size = 9;
                sh.Cells[number + 10 + i * 3, 6].Style.Numberformat.Format = "0.0000";
                
                string landbook = "Y";
                if (landbook == "Y")
                {
                    sh.Cells[number + 10 + i * 3, 7].Value = "■有□無";
                }
                else
                    sh.Cells[number + 10 + i * 3, 7].Value = "□有■無";
                sh.Cells[number + 10 + i * 3, 7].Style.Font.Size = 9;
                
                string landregister = "Y";
                if (landregister == "Y")
                {
                    sh.Cells[number + 10 + i * 3, 9].Value = "■有□無";
                }
                else
                    sh.Cells[number + 10 + i * 3, 9].Value = "□有■無";
                sh.Cells[number + 10 + i * 3, 9].Style.Font.Size = 9;
                
                string landrelation = "1";//1-5
                switch (landrelation.ToString())
                {
                    case "1":
                        sh.Cells[number + 10 + i * 3, 10].Value = "■部分持分   □全部持分";
                        sh.Cells[number + 11 + i * 3, 10].Value = "□部分代耕   □全部代耕";
                        sh.Cells[number + 12 + i * 3, 10].Value = "□國有地租用";
                        break;
                    case "2":
                        sh.Cells[number + 10 + i * 3, 10].Value = "□部分持分   ■全部持分";
                        sh.Cells[number + 11 + i * 3, 10].Value = "□部分代耕   □全部代耕";
                        sh.Cells[number + 12 + i * 3, 10].Value = "□國有地租用";
                        break;
                    case "3":
                        sh.Cells[number + 10 + i * 3, 10].Value = "□部分持分   □全部持分";
                        sh.Cells[number + 11 + i * 3, 10].Value = "■部分代耕   □全部代耕";
                        sh.Cells[number + 12 + i * 3, 10].Value = "□國有地租用";
                        break;
                    case "4":
                        sh.Cells[number + 10 + i * 3, 10].Value = "□部分持分   □全部持分";
                        sh.Cells[number + 11 + i * 3, 10].Value = "□部分代耕   ■全部代耕";
                        sh.Cells[number + 12 + i * 3, 10].Value = "□國有地租用";
                        break;
                    case "5":
                        sh.Cells[number + 10 + i * 3, 10].Value = "□部分持分   □全部持分";
                        sh.Cells[number + 11 + i * 3, 10].Value = "□部分代耕   □全部代耕";
                        sh.Cells[number + 12 + i * 3, 10].Value = "■國有地租用";
                        break;
                }
                
                string Certificate = "Y";
                if (Certificate == "Y")
                {
                    sh.Cells[number + 10 + i * 3, 15].Value = "■有□無";
                    sh.Cells[number + 10 + i * 3, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                else
                    sh.Cells[number + 10 + i * 3, 15].Value = "□有■無";
                sh.Cells[number + 10 + i * 3, 15].Style.Font.Size = 9;
                
                string IDcopy = "Y";
                if (IDcopy == "Y")
                {
                    sh.Cells[number + 10 + i * 3, 17].Value = "■有□無";
                }
                else
                    sh.Cells[number + 10 + i * 3, 17].Value = "□有■無";
                sh.Cells[number + 10 + i * 3, 17].Style.Font.Size = 9;
                
                string ChiefCertificate = "Y";
                if (ChiefCertificate == "Y")
                {
                    sh.Cells[number + 10 + i * 3, 20].Value = "■有□無";
                }
                else
                    sh.Cells[number + 10 + i * 3, 20].Value = "□有■無";
                sh.Cells[number + 10 + i * 3, 20].Style.Font.Size = 9;
                
                string LandAgreement = "Y";
                if (LandAgreement == "Y")
                {
                    sh.Cells[number + 10 + i * 3, 22].Value = "■有□無";
                }
                else
                    sh.Cells[number + 10 + i * 3, 22].Value = "□有■無";
                sh.Cells[number + 10 + i * 3, 22].Style.Font.Size = 9;
            }
            #endregion
            #region 小計
            sh.Cells[number + 25, 3].Value = 3 + "筆";
            sh.Cells[number + 26, 3].Value = "備註欄";
            #endregion

            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

            return sh;
        }
        #endregion
        #endregion

        #region 加入會員審核表
        public byte[] GetExamineReportData(ExamineReportView Data)
        {
            #region basic Data

            string sample_Path = @"~/ReportSample/ExamineReportSample.xlsx";
            
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["審核表"];

            int count = 5;
            for (int i = 0; i < count; i++)
            {
                int number = (i * 26);
                if (number != 0)
                {
                    sheet.Select("A1:L26");
                    sheet.SelectedRange.Copy(sheet.Cells["A" + (number + 1).ToString() + ":J" + (number + 1).ToString()]);
                }
            }

            for (int i = 1; i <= count; i++)
            {
                sheet = SetExamineReportData(sheet, Data, i);
            }

            byte[] file = excel.GetAsByteArray();

            #endregion

            return file;
        }

        #region 設定加入會員審核表
        public ExcelWorksheet SetExamineReportData(ExcelWorksheet sh, ExamineReportView Data, int index)
        {
            int number = ((index - 1) * 26);
            #region
            sh.Row(number + 1).Height = 30;
            sh.Row(number + 2).Height = 35;
            sh.Row(number + 3).Height = 20;
            sh.Cells[number + 4, 3].Value = "XXX";
            sh.Row(number + 4).Height = 35;
            sh.Row(number + 5).Height = 35;
            sh.Row(number + 6).Height = 35;
            sh.Cells[number + 7, 4].Value = "93" + "年";
            sh.Row(number + 7).Height = 35;
            sh.Row(number + 8).Height = 35;//
            sh.Row(number + 9).Height = 35;//
            sh.Row(number + 10).Height = 35;//
            sh.Row(number + 11).Height = 35;//
            sh.Row(number + 12).Height = 35;//
            sh.Row(number + 13).Height = 35;//
            sh.Row(number + 14).Height = 35;//
            sh.Row(number + 15).Height = 35;//
            sh.Row(number + 16).Height = 35;//
            sh.Row(number + 17).Height = 35;//
            sh.Row(number + 18).Height = 35;//
            sh.Row(number + 19).Height = 35;//
            #endregion
            sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion
        #endregion

        


    }
}
