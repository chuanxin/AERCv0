using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Dry.Models.CommonCls;
using AERC.Models.CommonCls;


namespace Dry.Models.ReportModules
{
    public class EngineeringReport
    {
        private GetUints getuints = new GetUints();
        public byte[] GetReport_EngineeringReport(short unit, int year, int IANumStart, int IANumEnd,int step)
        {

            List<EngineeringReportView> data = setEngineeringData(unit, year, IANumStart, IANumEnd, step, 0); 
            List<EngineeringReportView> data1 = setEngineeringData(unit, year, IANumStart, IANumEnd, step, 17); 
            List<EngineeringReportView> data2 = setEngineeringData(unit, year, IANumStart, IANumEnd, step, 16); 

            
            string sample_Path = @"~/ReportSample/EngineeringReportSample.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["農水署明細表"];
            if (data.Count > 0)
            {
                
                CreateEngineeringReportData(data, sheet);
            }
            if (data1.Count > 0)
            {
                sheet = excel.Workbook.Worksheets["瑠公明細表"];
                CreateEngineeringReportData(data1, sheet);
            }
            if (data2.Count > 0)
            {
                sheet = excel.Workbook.Worksheets["七星明細表"];
                CreateEngineeringReportData(data2, sheet);
            }
            byte[] resdata = excel.GetAsByteArray();
            return resdata;
        }

        #region 設定填表資料
        public List<EngineeringReportView> setEngineeringData(short unit, int year, int IANumStart, int IANumEnd,int step, int coa){

            List<EngineeringReportView> data = new List<EngineeringReportView>(); //主表
            //List<EngineeringReportView> fdata1 = new List<EngineeringReportView>(); //副表
            
            DryEntities DryDB = new DryEntities();
            
            
            List<AERC.Models.Town> towns = new AERC.Models.CommonEntities().Town.ToList();
            /*List<SummaryView> oridata = DryDB.SummaryView.Where(m => m.ApplyUnit == unit && m.ApplyYear == year && m.IANum >= IANumStart 
                && m.IANum <= IANumEnd && m.Step >= step && m.Gold == false).OrderBy( m => m.IANum).ToList();
            */
            List<SummaryView> oridata = new List<SummaryView>();
            switch (coa)
            {
                case 0:
                    
                    oridata = (from p in DryDB.SummaryView
                    join s in DryDB.CasePayUnit on p.MapNo equals s.MapNo
                    where p.ApplyUnit == unit && p.ApplyYear == year && p.IANum >= IANumStart && p.IANum <= IANumEnd && p.Step >= step && p.Gold == false
                    && s.ApplyUnit == 0 && s.GovPayUnit == 1
                               select p).OrderBy(p => p.IANum).ToList();

                    break;

                case 16:
                    
                    oridata = (from p in DryDB.SummaryView
                               join s in DryDB.CasePayUnit on p.MapNo equals s.MapNo
                               where p.ApplyUnit == unit && p.ApplyYear == year && p.IANum >= IANumStart && p.IANum <= IANumEnd && p.Step >= step 
                               && p.Gold == false && s.ApplyUnit == 16 && s.GovPayUnit == 1
                               select p).OrderBy(p => p.IANum).ToList();
                    break;
                case 17:
                    
                    oridata = (from p in DryDB.SummaryView
                               join s in DryDB.CasePayUnit on p.MapNo equals s.MapNo
                               where p.ApplyUnit == unit && p.ApplyYear == year && p.IANum >= IANumStart && p.IANum <= IANumEnd && p.Step >= step 
                               && p.Gold == false && s.ApplyUnit == 17 && s.GovPayUnit == 1
                               select p).OrderBy(p => p.IANum).ToList();
                    break;
                default:
                    break;
            }
            
            if (step == 11)
            {
                oridata = oridata.Where(o => o.Complete == true).ToList();
            }
            
            GetData gd = new GetData();
            
            
            EngineeringReportView subitem = new EngineeringReportView();
                       
            foreach (var item in oridata)
            {
                EngineeringReportView listdata = new EngineeringReportView();
                string aa = getuints.GetUnitData(item.ApplyUnit).Unit1;
                
                listdata.EIA =  aa;
                listdata.Eyears = item.ApplyYear;
                listdata.EName = item.Name;
                listdata.ENo = item.IANum.ToString();
                listdata.Designer_Name = item.Designer_Name;
                                
                
                try
                {
                    int sectionId = DryDB.FarmData.FirstOrDefault(m => m.MapNo == item.MapNo).Section;
                    listdata.ELocation = DryDB.LandData.FirstOrDefault(m => m.Section_Id == sectionId).Town;
                }
                catch 
                {
                    listdata.ELocation = "空白";
                }
                
                    
                listdata.EArea = (float)gd.GetFarmBuildArea((int)item.MapNo) / 10000;
                List<EndType> endtype = gd.GetEndTypeData((int)item.MapNo);
                if (endtype.Count > 0)
                {
                    //var endTypeCode = endtype.First().EndTypeCode;
                    //listdata.EIrrigation = DryDB.EndTypeList.Where(m => m.EndType == endTypeCode).First().EndTypeCNS;

                    foreach (var itemtype in endtype)
                    {
                        switch (itemtype.EndTypeCode)
                        {
                            case 1:
                                listdata.EIrrigation = "穿孔管";
                                subitem.EFacilityAreaA += listdata.EArea;
                                subitem.EFacilityA += 1;
                              
                                break;
                            case 2:
                                listdata.EIrrigation = "噴頭";
                                subitem.EFacilityAreaB += listdata.EArea;
                                subitem.EFacilityB += 1;
                                break;
                            case 3:
                                listdata.EIrrigation = "微噴";
                                subitem.EFacilityAreaD += listdata.EArea;
                                subitem.EFacilityD += 1;
                                break;
                            case 4:
                                listdata.EIrrigation = "滴灌";
                                subitem.EFacilityAreaC += listdata.EArea;
                                subitem.EFacilityC += 1;
                                break;
                            case 5:
                                listdata.EIrrigation = "軟管";
                                break;
                            case 6:
                                listdata.EIrrigation = "噴頭";
                                subitem.EFacilityAreaB += listdata.EArea;
                                subitem.EFacilityB += 1;
                                break;
                            case 7:
                                listdata.EIrrigation = "滴灌";
                                subitem.EFacilityAreaC += listdata.EArea;
                                subitem.EFacilityC += 1;
                                break;
                            case 8:
                                listdata.EIrrigation = "滴灌";
                                subitem.EFacilityAreaC += listdata.EArea;
                                subitem.EFacilityC += 1;
                                break;
                        }

                    }
                }
                else 
                {
                    listdata.EIrrigation = "其它";
                    subitem.EFacilityAreaE += listdata.EArea;
                    subitem.EFacilityE += 1;
                   
                }
                

                List<Pay> pay = DryDB.Pay.Where(m => m.MapNo == item.MapNo).ToList();
                int efarmer = 0; 
                foreach (var itema in pay) 
                {
                    switch (itema.ItemCode)
                    {
                        case 1:
                            listdata.Eendfacility = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityendfacilityA += itema.PayMoney;
                                    subitem.EFacilityFarmerA += itema.FarmerMoney;
                                    //subitem.EFacilityA += 1;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityendfacilityB += itema.PayMoney;
                                    subitem.EFacilityFarmerB += itema.FarmerMoney;
                                    //subitem.EFacilityB += 1;
                                    break;
                                case "微噴":
                                    subitem.EFacilityendfacilityD += itema.PayMoney;
                                    subitem.EFacilityFarmerD += itema.FarmerMoney;
                                    //subitem.EFacilityD += 1;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityendfacilityC += itema.PayMoney;
                                    subitem.EFacilityFarmerC += itema.FarmerMoney;
                                    //subitem.EFacilityC += 1;
                                    break;
                                case "其它":
                                    subitem.EFacilityendfacilityE += itema.PayMoney;
                                    subitem.EFacilityFarmerE += itema.FarmerMoney;
                                    //subitem.EFacilityE += 1;
                                    break;
                            }
                            break;
                        case 2:
                            listdata.EDesignCharges = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityDesignA += itema.PayMoney;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityDesignB += itema.PayMoney;
                                    break;
                                case "微噴":
                                    subitem.EFacilityDesignD += itema.PayMoney;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityDesignC += itema.PayMoney;
                                    break;
                                case "其它":
                                    subitem.EFacilityDesignE += itema.PayMoney;
                                    break;
                            }
                            break;
                        case 3:
                            listdata.EWaterFacility = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityWaterFacilityA += itema.PayMoney;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityWaterFacilityB += itema.PayMoney;
                                    break;
                                case "微噴":
                                    subitem.EFacilityWaterFacilityD += itema.PayMoney;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityWaterFacilityC += itema.PayMoney;
                                    break;
                                case "其它":
                                    subitem.EFacilityWaterFacilityE += itema.PayMoney;
                                    break;
                            }
                            break;
                        case 4:
                            listdata.ERegulation = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityRegulationA += itema.PayMoney;
                                    subitem.EFacilityFarmerA += itema.FarmerMoney;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityRegulationB += itema.PayMoney;
                                    subitem.EFacilityFarmerB += itema.FarmerMoney;
                                    break;
                                case "微噴":
                                    subitem.EFacilityRegulationD += itema.PayMoney;
                                    subitem.EFacilityFarmerD += itema.FarmerMoney;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityRegulationC += itema.PayMoney;
                                    subitem.EFacilityFarmerC += itema.FarmerMoney;
                                    break;
                                case "其它":
                                    subitem.EFacilityRegulationE += itema.PayMoney;
                                    subitem.EFacilityFarmerE += itema.FarmerMoney;
                                    break;
                            }
                            break;
                        case 5:
                            listdata.EPowerEquipment = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityPowerA += itema.PayMoney;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityPowerB += itema.PayMoney;
                                    break;
                                case "微噴":
                                    subitem.EFacilityPowerD += itema.PayMoney;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityPowerC += itema.PayMoney;
                                    break;
                                case "其它":
                                    subitem.EFacilityPowerE += itema.PayMoney;
                                    break;
                            }
                            break;
                        case 6:
                            listdata.EWaterReservoir = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityWaterReservoirA += itema.PayMoney;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityWaterReservoirB += itema.PayMoney;
                                    break;
                                case "微噴":
                                    subitem.EFacilityWaterReservoirD += itema.PayMoney;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityWaterReservoirC += itema.PayMoney;
                                    break;
                                case "其它":
                                    subitem.EFacilityWaterReservoirE += itema.PayMoney;
                                    break;
                            }
                            break;
                        case 7:
                            
                            break;
                        case 8:
                           
                            break;
                    }
                }
                listdata.EFarmer = efarmer;

                data.Add(listdata);
                
            }
            
            
            if(subitem.EFacilityA != 0)
            {
                data.Last().EFacilityA = subitem.EFacilityA;
                data.Last().EFacilityAreaA = subitem.EFacilityAreaA;
                data.Last().EFacilityDesignA = subitem.EFacilityDesignA;
                data.Last().EFacilityendfacilityA = subitem.EFacilityendfacilityA;
                data.Last().EFacilityFarmerA = subitem.EFacilityFarmerA;
                data.Last().EFacilityPowerA = subitem.EFacilityPowerA;
                data.Last().EFacilityRegulationA = subitem.EFacilityRegulationA;
                data.Last().EFacilityWaterFacilityA = subitem.EFacilityWaterFacilityA;
                data.Last().EFacilityWaterReservoirA = subitem.EFacilityWaterReservoirA;
            }

            if (subitem.EFacilityB != 0)
            {
                data.Last().EFacilityB = subitem.EFacilityB;
                data.Last().EFacilityAreaB = subitem.EFacilityAreaB;
                data.Last().EFacilityDesignB = subitem.EFacilityDesignB;
                data.Last().EFacilityendfacilityB = subitem.EFacilityendfacilityB;
                data.Last().EFacilityFarmerB = subitem.EFacilityFarmerB;
                data.Last().EFacilityPowerB = subitem.EFacilityPowerB;
                data.Last().EFacilityRegulationB = subitem.EFacilityRegulationB;
                data.Last().EFacilityWaterFacilityB = subitem.EFacilityWaterFacilityB;
                data.Last().EFacilityWaterReservoirB = subitem.EFacilityWaterReservoirB;
            }

            if (subitem.EFacilityC != 0)
            {
                data.Last().EFacilityC = subitem.EFacilityC;
                data.Last().EFacilityAreaC = subitem.EFacilityAreaC;
                data.Last().EFacilityDesignC = subitem.EFacilityDesignC;
                data.Last().EFacilityendfacilityC = subitem.EFacilityendfacilityC;
                data.Last().EFacilityFarmerC = subitem.EFacilityFarmerC;
                data.Last().EFacilityPowerC = subitem.EFacilityPowerC;
                data.Last().EFacilityRegulationC = subitem.EFacilityRegulationC;
                data.Last().EFacilityWaterFacilityC = subitem.EFacilityWaterFacilityC;
                data.Last().EFacilityWaterReservoirC = subitem.EFacilityWaterReservoirC;
            }

            


            if (subitem.EFacilityD != 0)
            {
                data.Last().EFacilityD = subitem.EFacilityD;
                data.Last().EFacilityAreaD = subitem.EFacilityAreaD;
                data.Last().EFacilityDesignD = subitem.EFacilityDesignD;
                data.Last().EFacilityendfacilityD = subitem.EFacilityendfacilityD;
                data.Last().EFacilityFarmerD = subitem.EFacilityFarmerD;
                data.Last().EFacilityPowerD = subitem.EFacilityPowerD;
                data.Last().EFacilityRegulationD = subitem.EFacilityRegulationD;
                data.Last().EFacilityWaterFacilityD = subitem.EFacilityWaterFacilityD;
                data.Last().EFacilityWaterReservoirD = subitem.EFacilityWaterReservoirD;
            }
            

            if (subitem.EFacilityE != 0)
            {
                data.Last().EFacilityE = subitem.EFacilityE;
                data.Last().EFacilityAreaE = subitem.EFacilityAreaE;
                data.Last().EFacilityDesignE = subitem.EFacilityDesignE;
                data.Last().EFacilityendfacilityE = subitem.EFacilityendfacilityE;
                data.Last().EFacilityFarmerE = subitem.EFacilityFarmerE;
                data.Last().EFacilityPowerE = subitem.EFacilityPowerE;
                data.Last().EFacilityRegulationE = subitem.EFacilityRegulationE;
                data.Last().EFacilityWaterFacilityE = subitem.EFacilityWaterFacilityE;
                data.Last().EFacilityWaterReservoirE = subitem.EFacilityWaterReservoirE;
            }

            

            return data;

        }
        #endregion


        #region 產生報表明細表
        public void CreateEngineeringReportData(List<EngineeringReportView> Data,ExcelWorksheet sheet)
        {
            #region basic Data
                   

            //int count = 48;//資料筆數
            int count = Data.Count;//包含最後一筆統計
            /*
            for (int k = 1; k <= count; k++)
            {
                sheet = SetEngineeringReportData(sheet, Data[k], k, count);//DB
            }
            */
            int k = 1;
            foreach (var data in Data) 
            {
                sheet = SetEngineeringReportData(sheet, data, k, count);//DB
                k++;
            }
            //填寫最後設施統計資料
            //sheet = SetEngineeringReportData(sheet, Data1.First(), k, count);

            //byte[] file = excel.GetAsByteArray();

            #endregion
            //return file;
        }
        #endregion

        #region 填寫管路工程設施輔助明細表
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

                
                sh.Column(i).Style.Font.Name = "標楷體";

            }
            sh.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            if (endRowNumber % 30 == 0)
            {
                DataRowNumber = endRowNumber + 5;
                for (int j = 1; j <= 4; j++)
                {
                    sh.Cells["A" + j.ToString() + ":S" + j.ToString()].Copy(sh.Cells["A" + (endRowNumber + j).ToString() + ":S" + (endRowNumber + j).ToString()]);
                }
            }
            else
            {
                DataRowNumber = endRowNumber + 1;
            }
            #region 填寫表身
            //for (var index = 1; index < count + 1; index++)//row
            //{
            //    for (var j = 0; j < endColumn; j++)//column
            //    {
            
            sh.Cells[DataRowNumber, 1].Value = Data.ENo;
            sh.Cells[DataRowNumber, 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 1].Style.WrapText = true;
            sh.Cells[DataRowNumber, 1].Style.Font.Size = 8;

            
            sh.Cells[DataRowNumber, 2].Value = Data.EName;
            sh.Cells[DataRowNumber, 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 2].Style.Font.Size = 10;

            
            sh.Column(3).Style.Numberformat.Format = "0.0000";
            sh.Cells[DataRowNumber, 3].Value = Data.EArea;
            sh.Cells[DataRowNumber, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 3].Style.WrapText = true;
            sh.Cells[DataRowNumber, 3].Style.Font.Size = 10;

            
            sh.Cells[DataRowNumber, 4].Value = Data.ELocation;
            sh.Cells[DataRowNumber, 4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 4].Style.WrapText = true;
            sh.Cells[DataRowNumber, 4].Style.Font.Size = 10;
            
            sh.Cells[DataRowNumber, 5].Value = Data.EIrrigation;
            sh.Cells[DataRowNumber, 5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 5].Style.Font.Size = 10;

            
            sh.Column(6).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 6].Value = Data.EFarmer;
            sh.Cells[DataRowNumber, 6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 6].Style.Font.Size = 10;

            
            sh.Column(7).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 7].Value = Data.Eendfacility;
            sh.Cells[DataRowNumber, 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            //sh.Cells[CorrectRow, 7].Style.WrapText = true;
            sh.Cells[DataRowNumber, 7].Style.Font.Size = 10;

            
            sh.Column(8).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 8].Value = Data.EWaterFacility;
            sh.Cells[DataRowNumber, 8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 8].Style.WrapText = true;
            sh.Cells[DataRowNumber, 8].Style.Font.Size = 10;

            
            sh.Column(9).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 9].Value = Data.ERegulation;
            sh.Cells[DataRowNumber, 9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 9].Style.Font.Size = 10;

            
            sh.Column(10).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 10].Value = Data.EWaterReservoir;
            sh.Cells[DataRowNumber, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 10].Style.Font.Size = 10;

            
            sh.Column(11).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 11].Value = Data.EPowerEquipment;
            sh.Cells[DataRowNumber, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 11].Style.Font.Size = 10;

            
            sh.Column(12).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            string Gt = "G" + DataRowNumber;
            string Kt = "K" + DataRowNumber;
            sh.Cells[DataRowNumber, 12].Formula = "+SUM(" + Gt + ":" + Kt + ")";
            sh.Cells[DataRowNumber, 12].Style.Font.Size = 10;

            
            sh.Column(13).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 13].Value = Data.EDesignCharges;
            sh.Cells[DataRowNumber, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 13].Style.Font.Size = 10;

            
            sh.Column(14).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            string Lt = "L" + DataRowNumber;
            string Mt = "M" + DataRowNumber;
            sh.Cells[DataRowNumber, 14].Formula = "+SUM(" + Lt + ":" + Mt + ")";
            sh.Cells[DataRowNumber, 14].Style.Font.Size = 10;

            
            sh.Column(15).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 15].Style.Font.Size = 10;
            string Ft = "F" + DataRowNumber;
            string Nt = "N" + DataRowNumber;
            sh.Cells[DataRowNumber, 15].Formula = "+SUM(" + Ft + "," + Nt + ")";

            
            sh.Column(16).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 16].Style.Font.Size = 10;
            string Ct = "C" + DataRowNumber;
            Lt = "G" + DataRowNumber;
            sh.Cells[DataRowNumber, 16].Formula = "IF(" + Ct +">0,+ROUND((" + Lt + "/" + Ct + "),0),0)";

            
            sh.Column(17).Style.Numberformat.Format = "0.00";
            sh.Cells[DataRowNumber, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 17].Style.Font.Size = 8;
            string Pt = "R" + DataRowNumber;
            string Ot = "P" + DataRowNumber;
            
            //sh.Cells[DataRowNumber, 17].Formula = "IF(" + Pt + ">0,+ROUND((" + Ot + "/" + Pt + ")*100, 2),0)";


            
            sh.Column(18).Style.Numberformat.Format = "#,##0";
            sh.Cells[DataRowNumber, 18].Value = 274359;
            sh.Cells[DataRowNumber, 18].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 18].Style.Font.Size = 10;
            Ot = "O" + DataRowNumber;
            Ct = "C" + DataRowNumber;
            sh.Cells[DataRowNumber, 18].Formula = "IF(" + Ct + ">0,+ROUND((" + Ot + "/" + Ct + "),2),0)";
            //    }
            //    DataRowNumber++;
            //}
            
            sh.Cells[DataRowNumber, 19].Value = Data.Designer_Name;
            sh.Cells[DataRowNumber, 19].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
            sh.Cells[DataRowNumber, 19].Style.WrapText = true;
            sh.Cells[DataRowNumber, 19].Style.Font.Size = 10;
            #endregion
           

            int startRowNumberfinal = sh.Dimension.Start.Row;
            int endRowNumberfinal = sh.Dimension.End.Row;
            int DataRowNumberfinal = endRowNumberfinal + 1;

            #region 合計-總計
            if (count == groupcount)
            {
                int k = endRowNumberfinal / 30;
                int m = endRowNumberfinal - (k * 30);
                int mod = 0;
                if (m == 0)
                {
                    mod = m;
                }
                else
                    mod = 30 - m;
                int checknumber = m + 6 + 1;
                if (checknumber > 30 || m == 0)
                {
                    DataRowNumberfinal = endRowNumberfinal + 4 + mod;
                    for (int j = 3; j <= 4; j++)
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
                
                sh.Column(3).Style.Numberformat.Format = "0.0000";
                sh.Cells[DataRowNumberfinal, 3].Value = Data.EFacilityAreaA;
                sh.Cells[DataRowNumberfinal + 1, 3].Value = Data.EFacilityAreaB;
                sh.Cells[DataRowNumberfinal + 2, 3].Value = Data.EFacilityAreaC;
                sh.Cells[DataRowNumberfinal + 3, 3].Value = Data.EFacilityAreaD;
                sh.Cells[DataRowNumberfinal + 4, 3].Value = Data.EFacilityAreaE;
                
                sh.Cells[DataRowNumberfinal, 5].Value = "穿孔管";
                sh.Cells[DataRowNumberfinal + 1, 5].Value = "噴頭";
                sh.Cells[DataRowNumberfinal + 2, 5].Value = "滴灌";
                sh.Cells[DataRowNumberfinal + 3, 5].Value = "微噴";
                //sh.Cells[DataRowNumberfinal + 4, 5].Value = "軟管澆灌";
                sh.Cells[DataRowNumberfinal + 4, 5].Value = "其它";
                
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
                    //int number = 25 + i;
                    int number = DataRowNumberfinal + i;
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
                    //int number = 25 + i;
                    int number = DataRowNumberfinal + i;
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
                    //int number = 25 + i;
                    int number = DataRowNumberfinal + i;
                    Ft = "F" + number;
                    Nt = "N" + number;
                    number = number + i;
                    sh.Cells[DataRowNumberfinal + i, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); //儲存格框線
                    sh.Cells[DataRowNumberfinal + i, 15].Style.Font.Size = 9;
                    sh.Cells[DataRowNumberfinal + i, 15].Formula = "+SUM(" + Ft + "," + Nt + ")";
                }
                
                sh.Column(16).Style.Numberformat.Format = "#,##0";
                Ct = "C" + DataRowNumberfinal;
                Lt = "G" + DataRowNumberfinal;
                sh.Cells[DataRowNumberfinal, 16].Formula = "IF(" + Ct + ">0,+ROUND((" + Lt + "/" + Ct + "),0),0)";
                Ct = "C" + (DataRowNumberfinal + 1);
                Lt = "G" + (DataRowNumberfinal + 1);
                sh.Cells[DataRowNumberfinal + 1, 16].Formula = "IF(" + Ct + ">0,+ROUND((" + Lt + "/" + Ct + "),0),0)";
                Ct = "C" + (DataRowNumberfinal + 2);
                Lt = "G" + (DataRowNumberfinal + 2);
                sh.Cells[DataRowNumberfinal + 2, 16].Formula = "IF(" + Ct + ">0,+ROUND((" + Lt + "/" + Ct + "),0),0)";
                Ct = "C" + (DataRowNumberfinal + 3);
                Lt = "G" + (DataRowNumberfinal + 3);
                sh.Cells[DataRowNumberfinal + 3, 16].Formula = "IF(" + Ct + ">0,+ROUND((" + Lt + "/" + Ct + "),0),0)";
                Ct = "C" + (DataRowNumberfinal + 4);
                Lt = "G" + (DataRowNumberfinal + 4);
                sh.Cells[DataRowNumberfinal + 4, 16].Formula = "IF(" + Ct + ">0,+ROUND((" + Lt + "/" + Ct + "),0),0)";
                
                sh.Column(17).Style.Numberformat.Format = "0.00";
                Pt = "R" + DataRowNumberfinal;
                Ot = "P" + DataRowNumberfinal;
                //sh.Cells[DataRowNumberfinal, 17].Formula = "IF(" + Pt +">0,+ROUND((" + Ot + "/" + Pt + ")*100,2),0)";
                Pt = "R" + (DataRowNumberfinal + 1);
                Ot = "P" + (DataRowNumberfinal + 1);
                //sh.Cells[DataRowNumberfinal + 1, 17].Formula = "IF(" + Pt + ">0,+ROUND((" + Ot + "/" + Pt + ")*100,2),0)";
                Pt = "R" + (DataRowNumberfinal + 2);
                Ot = "P" + (DataRowNumberfinal + 2);
                //sh.Cells[DataRowNumberfinal + 2, 17].Formula = "IF(" + Pt + ">0,+ROUND((" + Ot + "/" + Pt + ")*100,2),0)";
                Pt = "R" + (DataRowNumberfinal + 3);
                Ot = "P" + (DataRowNumberfinal + 3);
                //sh.Cells[DataRowNumberfinal + 3, 17].Formula = "IF(" + Pt + ">0,+ROUND((" + Ot + "/" + Pt + ")*100,2),0)";
                Pt = "R" + (DataRowNumberfinal + 4);
                Ot = "P" + (DataRowNumberfinal + 4);
                //sh.Cells[DataRowNumberfinal + 4, 17].Formula = "IF(" + Pt + ">0,+ROUND((" + Ot + "/" + Pt + ")*100,2),0)";
                
                sh.Column(18).Style.Numberformat.Format = "#,##0";
                Ot = "O" + DataRowNumberfinal;
                Ct = "C" + DataRowNumberfinal;
                sh.Cells[DataRowNumberfinal, 18].Formula ="IF("+ Ct + ">0,+ROUND((" + Ot + "/" + Ct + "),2),0)";
                Ot = "O" + (DataRowNumberfinal + 1);
                Ct = "C" + (DataRowNumberfinal + 1);
                sh.Cells[DataRowNumberfinal + 1, 18].Formula = "IF(" + Ct + ">0,+ROUND((" + Ot + "/" + Ct + "),2),0)";
                Ot = "O" + (DataRowNumberfinal + 2);
                Ct = "C" + (DataRowNumberfinal + 2);
                sh.Cells[DataRowNumberfinal + 2, 18].Formula = "IF(" + Ct + ">0,+ROUND((" + Ot + "/" + Ct + "),2),0)";
                Ot = "O" + (DataRowNumberfinal + 3);
                Ct = "C" + (DataRowNumberfinal + 3);
                sh.Cells[DataRowNumberfinal + 3, 18].Formula = "IF(" + Ct + ">0,+ROUND((" + Ot + "/" + Ct + "),2),0)";
                Ot = "O" + (DataRowNumberfinal + 4);
                Ct = "C" + (DataRowNumberfinal + 4);
                sh.Cells[DataRowNumberfinal + 4, 18].Formula = "IF(" + Ct + ">0,+ROUND((" + Ot + "/" + Ct + "),2),0)";
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

           // sh.HeaderFooter.EvenFooter.RightAlignedText = sh.HeaderFooter.OddFooter.RightAlignedText = string.Format("第 {0} 頁/共 {1} 頁", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
            return sh;
        }
        #endregion

        #region 產生黃金廊道補助明細資料表
        public byte[] GetReport_EngineeringReportGold(short unit, int year, int IANumStart, int IANumEnd,int step)
        {

            List<EngineeringReportView> data = setEngineeringData(unit, year, IANumStart, IANumEnd,step, 0); 
            List<EngineeringReportView> data2 = setEngineeringData(unit, year, IANumStart, IANumEnd, step, 17); 
            List<EngineeringReportView> data3 = setEngineeringData(unit, year, IANumStart, IANumEnd, step, 16); 
            
            string sample_Path = @"~/ReportSample/EngineeringReportSampleGold2019.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sample_Path), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["農水署明細表"];
            ExcelWorksheet sheet2 = excel.Workbook.Worksheets["瑠公明細表"] ;
            ExcelWorksheet sheet3 = excel.Workbook.Worksheets["七星明細表"] ;
            if (data.Count >0 )
            {
                CreateEngineeringReportData(data, sheet); 
            }


            if (data2.Count > 0)
            {
                CreateEngineeringReportData(data2, sheet2);
            }
            if (data3.Count > 0)
            {
                CreateEngineeringReportData(data3, sheet3);
            }

            
            //List<EngineeringReportViewGold> data1 = setEngineeringDataGold(unit, year, IANumStart, IANumEnd,step);
            List<EngineeringReportView> data4 = setEngineeringDataGold2019(unit, year, IANumStart, IANumEnd,step,0);
            List<EngineeringReportView> data5 = setEngineeringDataGold2019(unit, year, IANumStart, IANumEnd,step,16);
            List<EngineeringReportView> data6 = setEngineeringDataGold2019(unit, year, IANumStart, IANumEnd,step,17);

            ExcelWorksheet sheet4 = excel.Workbook.Worksheets["黃金廊道-農水署"];
            ExcelWorksheet sheet5 = excel.Workbook.Worksheets["黃金廊道-七星"];
            ExcelWorksheet sheet6 = excel.Workbook.Worksheets["黃金廊道-瑠公"];
            if (data4.Count > 0)
            {
                CreateEngineeringReportData(data4, sheet4);    
            }
            if (data5.Count > 0)
            {
                CreateEngineeringReportData(data5, sheet5);
            }
            if (data6.Count > 0)
            {
                CreateEngineeringReportData(data6, sheet6);
            }

            byte[] resdata = excel.GetAsByteArray();
            return resdata;
        }
        #endregion

        #region 設定黃金廊道資料
        public List<EngineeringReportViewGold> setEngineeringDataGold (short unit, int year, int IANumStart, int IANumEnd,int step)
        {
            List<EngineeringReportViewGold> data = new List<EngineeringReportViewGold>(); 

            DryEntities DryDB = new DryEntities();


            List<AERC.Models.Town> towns = new AERC.Models.CommonEntities().Town.ToList();
            //List<SummaryView> oridata = DryDB.SummaryView.Where(m => m.ApplyUnit == unit && m.ApplyYear == year && m.IANum >= IANumStart && m.IANum <= IANumEnd).OrderBy(m => m.IANum).ToList();
            var oridata = (from SummaryView in DryDB.SummaryView
                           
                           where (SummaryView.ApplyUnit == unit) && (SummaryView.ApplyYear == year) && (SummaryView.IANum >= IANumStart) && (SummaryView.IANum <= IANumEnd) &&(SummaryView.Gold == true) &&(SummaryView.Step >= step)
                           
                           select SummaryView).OrderBy(m => m.IANum).ToList();

            GetData gd = new GetData();


            EngineeringReportViewGold subitem = new EngineeringReportViewGold();

            foreach (var item in oridata)
            {
                EngineeringReportViewGold listdata = new EngineeringReportViewGold();
                string aa = getuints.GetUnitData(item.ApplyUnit).Unit1;

                listdata.EIA = aa;
                listdata.Eyears = item.ApplyYear;
                listdata.EName = item.Name;
                listdata.ENo = item.IANum.ToString();
                listdata.Designer_Name = item.Designer_Name;
                
                try
                {
                    int sectionId = DryDB.FarmData.FirstOrDefault(m => m.MapNo == item.MapNo).Section;
                    listdata.ELocation = DryDB.LandData.FirstOrDefault(m => m.Section_Id == sectionId).Town;
                }
                catch
                {
                    listdata.ELocation = "空白";
                }


                listdata.EArea = (float)gd.GetFarmBuildArea((int)item.MapNo) / 10000;
                
                
                List<EndType> endtype = gd.GetEndTypeData((int)item.MapNo);
                if (endtype.Count > 0)
                {
                    //var endTypeCode = endtype.First().EndTypeCode;
                    //listdata.EIrrigation = DryDB.EndTypeList.Where(m => m.EndType == endTypeCode).First().EndTypeCNS;

                    foreach (var itemtype in endtype)
                    {
                        switch (itemtype.EndTypeCode)
                        {
                            case 1:
                                listdata.EIrrigation = "穿孔管";


                                break;
                            case 2:
                                listdata.EIrrigation = "噴頭";

                                break;
                            case 3:
                                listdata.EIrrigation = "微噴";

                                break;
                            case 4:
                                listdata.EIrrigation = "滴灌";

                                break;
                            case 5:
                                listdata.EIrrigation = "軟管";
                                break;
                            case 6:
                                listdata.EIrrigation = "噴頭";

                                break;
                            case 7:
                                listdata.EIrrigation = "滴灌";

                                break;
                            case 8:
                                listdata.EIrrigation = "滴灌";

                                break;
                        }

                    }
                }
                else
                {
                    listdata.EIrrigation = "其它";


                }


                List<Pay> pay = DryDB.Pay.Where(m => m.MapNo == item.MapNo).ToList();
                //int efarmer = 0; 
                foreach (var itema in pay)
                {
                    switch (itema.ItemCode)
                    {
                        case 1:
                            if (item.ApplyYear > 108)
                            {
                                //listdata.EndGoldPay = (int)Math.Round((double)(itema.Total * 0.7), 0, MidpointRounding.AwayFromZero) - itema.PayMoney;
                                listdata.EndCoaPay = itema.PayMoney;
                            }else
                            {
                                listdata.EndGoldPay = (int)Math.Round((double)(itema.Total * 0.7), 0, MidpointRounding.AwayFromZero) - itema.PayMoney;
                                listdata.EndCoaPay = itema.PayMoney;
                            }
                            
                            break;
                        case 2:
                            listdata.PlanPay = itema.PayMoney;
                            break;
                        case 3:

                            break;
                        case 4:
                            if (item.ApplyYear > 108)
                            {
                                //listdata.CtrlMatGoldPay = (int)Math.Round((double)(itema.Total * 0.7), 0, MidpointRounding.AwayFromZero) - itema.PayMoney;
                                listdata.CtrlMatCoaPay = itema.PayMoney;
                            }else
                            {
                                listdata.CtrlMatGoldPay = (int)Math.Round((double)(itema.Total * 0.7), 0, MidpointRounding.AwayFromZero) - itema.PayMoney;
                                listdata.CtrlMatCoaPay = itema.PayMoney;
                            }
                            
                            break;
                        case 5:
                            if (itema.ApplyUnit == 17)
                            {
                                listdata.EngLuPay = itema.PayMoney;
                            }
                            else
                            {
                                listdata.EngCoaPay = itema.PayMoney;
                            }
                            break;
                        case 6:
                            if (itema.ApplyUnit == 17)
                            {
                                listdata.PoolLuPay = itema.PayMoney;
                            }
                            else
                            {
                                listdata.PoolCoaPay = itema.PayMoney;
                            }
                            break;
                        case 7:

                            break;
                        case 8:

                            break;
                    }
                }

                
                /*if ((listdata.EndGoldPay >0) || (listdata.CtrlMatGoldPay >0))*/ data.Add(listdata);


            }
            return data;
        }
        #endregion

        public List<EngineeringReportView> setEngineeringDataGold2019(short unit, int year, int IANumStart, int IANumEnd, int step, int coa)
        {

            List<EngineeringReportView> data = new List<EngineeringReportView>();
            
            //List<EngineeringReportView> fdata1 = new List<EngineeringReportView>(); 
            DryEntities DryDB = new DryEntities();
            List<AERC.Models.Town> towns = new AERC.Models.CommonEntities().Town.ToList();
            /*List<SummaryView> oridata = DryDB.SummaryView.Where(m => m.ApplyUnit == unit && m.ApplyYear == year && m.IANum >= IANumStart 
                && m.IANum <= IANumEnd && m.Step >= step && m.Gold == false).OrderBy( m => m.IANum).ToList();
            */
            List<SummaryView> oridata = new List<SummaryView>();
            switch (coa)
            {
                case 0:
                   
                    oridata = (from s in DryDB.SummaryView
                               join p in DryDB.CasePayUnit on s.MapNo equals p.MapNo
                               where s.ApplyUnit == unit && s.ApplyYear == year && s.IANum >= IANumStart && s.IANum <= IANumEnd && s.Step >= step && s.Gold == true
                               && p.ApplyUnit == 0 && p.GovPayUnit == 1
                               select s).OrderBy(p => p.IANum).ToList();

                    break;

                case 16:
                    
                    oridata = (from p in DryDB.SummaryView
                               join s in DryDB.CasePayUnit on p.MapNo equals s.MapNo
                               where p.ApplyUnit == unit && p.ApplyYear == year && p.IANum >= IANumStart && p.IANum <= IANumEnd && p.Step >= step
                               && p.Gold == true && s.ApplyUnit == 16 && s.GovPayUnit == 1
                               select p).OrderBy(p => p.IANum).ToList();
                    break;
                case 17:
                    
                    oridata = (from p in DryDB.SummaryView
                               join s in DryDB.CasePayUnit on p.MapNo equals s.MapNo
                               where p.ApplyUnit == unit && p.ApplyYear == year && p.IANum >= IANumStart && p.IANum <= IANumEnd && p.Step >= step
                               && p.Gold == true && s.ApplyUnit == 17 && s.GovPayUnit == 1
                               select p).OrderBy(p => p.IANum).ToList();
                    break;
                default:
                    break;
            }
            
            if (step == 11)
            {
                oridata = oridata.Where(o => o.Complete == true).ToList();
            }
           


            GetData gd = new GetData();


            EngineeringReportView subitem = new EngineeringReportView();

            foreach (var item in oridata)
            {
                EngineeringReportView listdata = new EngineeringReportView();
                string aa = getuints.GetUnitData(item.ApplyUnit).Unit1;

                listdata.EIA = aa;
                listdata.Eyears = item.ApplyYear;
                listdata.EName = item.Name;
                listdata.ENo = item.IANum.ToString();
                listdata.Designer_Name = item.Designer_Name;

                
                try
                {
                    int sectionId = DryDB.FarmData.FirstOrDefault(m => m.MapNo == item.MapNo).Section;
                    listdata.ELocation = DryDB.LandData.FirstOrDefault(m => m.Section_Id == sectionId).Town;
                }
                catch
                {
                    listdata.ELocation = "空白";
                }


                listdata.EArea = (float)gd.GetFarmBuildArea((int)item.MapNo) / 10000;
                
                
                List<EndType> endtype = gd.GetEndTypeData((int)item.MapNo);
                if (endtype.Count > 0)
                {
                    //var endTypeCode = endtype.First().EndTypeCode;
                    //listdata.EIrrigation = DryDB.EndTypeList.Where(m => m.EndType == endTypeCode).First().EndTypeCNS;

                    foreach (var itemtype in endtype)
                    {
                        switch (itemtype.EndTypeCode)
                        {
                            case 1:
                                listdata.EIrrigation = "穿孔管";
                                subitem.EFacilityAreaA += listdata.EArea;
                                subitem.EFacilityA += 1;

                                break;
                            case 2:
                                listdata.EIrrigation = "噴頭";
                                subitem.EFacilityAreaB += listdata.EArea;
                                subitem.EFacilityB += 1;
                                break;
                            case 3:
                                listdata.EIrrigation = "微噴";
                                subitem.EFacilityAreaD += listdata.EArea;
                                subitem.EFacilityD += 1;
                                break;
                            case 4:
                                listdata.EIrrigation = "滴灌";
                                subitem.EFacilityAreaC += listdata.EArea;
                                subitem.EFacilityC += 1;
                                break;
                            case 5:
                                listdata.EIrrigation = "軟管";
                                break;
                            case 6:
                                listdata.EIrrigation = "噴頭";
                                subitem.EFacilityAreaB += listdata.EArea;
                                subitem.EFacilityB += 1;
                                break;
                            case 7:
                                listdata.EIrrigation = "滴灌";
                                subitem.EFacilityAreaC += listdata.EArea;
                                subitem.EFacilityC += 1;
                                break;
                            case 8:
                                listdata.EIrrigation = "滴灌";
                                subitem.EFacilityAreaC += listdata.EArea;
                                subitem.EFacilityC += 1;
                                break;
                        }

                    }
                }
                else
                {
                    listdata.EIrrigation = "其它";
                    subitem.EFacilityAreaE += listdata.EArea;
                    subitem.EFacilityE += 1;

                }


                List<Pay> pay = DryDB.Pay.Where(m => m.MapNo == item.MapNo).ToList();
                int efarmer = 0; 
                foreach (var itema in pay)
                {
                    switch (itema.ItemCode)
                    {
                        case 1:
                            listdata.Eendfacility = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityendfacilityA += itema.PayMoney;
                                    subitem.EFacilityFarmerA += itema.FarmerMoney;
                                    //subitem.EFacilityA += 1;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityendfacilityB += itema.PayMoney;
                                    subitem.EFacilityFarmerB += itema.FarmerMoney;
                                    //subitem.EFacilityB += 1;
                                    break;
                                case "微噴":
                                    subitem.EFacilityendfacilityD += itema.PayMoney;
                                    subitem.EFacilityFarmerD += itema.FarmerMoney;
                                    //subitem.EFacilityD += 1;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityendfacilityC += itema.PayMoney;
                                    subitem.EFacilityFarmerC += itema.FarmerMoney;
                                    //subitem.EFacilityC += 1;
                                    break;
                                case "其它":
                                    subitem.EFacilityendfacilityE += itema.PayMoney;
                                    subitem.EFacilityFarmerE += itema.FarmerMoney;
                                    //subitem.EFacilityE += 1;
                                    break;
                            }
                            break;
                        case 2:
                            listdata.EDesignCharges = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityDesignA += itema.PayMoney;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityDesignB += itema.PayMoney;
                                    break;
                                case "微噴":
                                    subitem.EFacilityDesignD += itema.PayMoney;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityDesignC += itema.PayMoney;
                                    break;
                                case "其它":
                                    subitem.EFacilityDesignE += itema.PayMoney;
                                    break;
                            }
                            break;
                        case 3:
                            listdata.EWaterFacility = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityWaterFacilityA += itema.PayMoney;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityWaterFacilityB += itema.PayMoney;
                                    break;
                                case "微噴":
                                    subitem.EFacilityWaterFacilityD += itema.PayMoney;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityWaterFacilityC += itema.PayMoney;
                                    break;
                                case "其它":
                                    subitem.EFacilityWaterFacilityE += itema.PayMoney;
                                    break;
                            }
                            break;
                        case 4:
                            listdata.ERegulation = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityRegulationA += itema.PayMoney;
                                    subitem.EFacilityFarmerA += itema.FarmerMoney;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityRegulationB += itema.PayMoney;
                                    subitem.EFacilityFarmerB += itema.FarmerMoney;
                                    break;
                                case "微噴":
                                    subitem.EFacilityRegulationD += itema.PayMoney;
                                    subitem.EFacilityFarmerD += itema.FarmerMoney;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityRegulationC += itema.PayMoney;
                                    subitem.EFacilityFarmerC += itema.FarmerMoney;
                                    break;
                                case "其它":
                                    subitem.EFacilityRegulationE += itema.PayMoney;
                                    subitem.EFacilityFarmerC += itema.FarmerMoney;
                                    break;
                            }
                            break;
                        case 5:
                            listdata.EPowerEquipment = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityPowerA += itema.PayMoney;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityPowerB += itema.PayMoney;
                                    break;
                                case "微噴":
                                    subitem.EFacilityPowerD += itema.PayMoney;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityPowerC += itema.PayMoney;
                                    break;
                                case "其它":
                                    subitem.EFacilityPowerE += itema.PayMoney;
                                    break;
                            }
                            break;
                        case 6:
                            listdata.EWaterReservoir = itema.PayMoney;
                            efarmer += itema.FarmerMoney;
                            switch (listdata.EIrrigation)
                            {
                                case "穿孔管":
                                    subitem.EFacilityWaterReservoirA += itema.PayMoney;
                                    break;
                                case "噴頭":
                                    subitem.EFacilityWaterReservoirB += itema.PayMoney;
                                    break;
                                case "微噴":
                                    subitem.EFacilityWaterReservoirD += itema.PayMoney;
                                    break;
                                case "滴灌":
                                    subitem.EFacilityWaterReservoirC += itema.PayMoney;
                                    break;
                                case "其它":
                                    subitem.EFacilityWaterReservoirE += itema.PayMoney;
                                    break;
                            }
                            break;
                        case 7:

                            break;
                        case 8:

                            break;
                    }
                }
                listdata.EFarmer = efarmer;

                data.Add(listdata);

            }

            
            if (subitem.EFacilityA != 0)
            {
                data.Last().EFacilityA = subitem.EFacilityA;
                data.Last().EFacilityAreaA = subitem.EFacilityAreaA;
                data.Last().EFacilityDesignA = subitem.EFacilityDesignA;
                data.Last().EFacilityendfacilityA = subitem.EFacilityendfacilityA;
                data.Last().EFacilityFarmerA = subitem.EFacilityFarmerA;
                data.Last().EFacilityPowerA = subitem.EFacilityPowerA;
                data.Last().EFacilityRegulationA = subitem.EFacilityRegulationA;
                data.Last().EFacilityWaterFacilityA = subitem.EFacilityWaterFacilityA;
                data.Last().EFacilityWaterReservoirA = subitem.EFacilityWaterReservoirA;
            }

            if (subitem.EFacilityB != 0)
            {
                data.Last().EFacilityB = subitem.EFacilityB;
                data.Last().EFacilityAreaB = subitem.EFacilityAreaB;
                data.Last().EFacilityDesignB = subitem.EFacilityDesignB;
                data.Last().EFacilityendfacilityB = subitem.EFacilityendfacilityB;
                data.Last().EFacilityFarmerB = subitem.EFacilityFarmerB;
                data.Last().EFacilityPowerB = subitem.EFacilityPowerB;
                data.Last().EFacilityRegulationB = subitem.EFacilityRegulationB;
                data.Last().EFacilityWaterFacilityB = subitem.EFacilityWaterFacilityB;
                data.Last().EFacilityWaterReservoirB = subitem.EFacilityWaterReservoirB;
            }

            if (subitem.EFacilityC != 0)
            {
                data.Last().EFacilityC = subitem.EFacilityC;
                data.Last().EFacilityAreaC = subitem.EFacilityAreaC;
                data.Last().EFacilityDesignC = subitem.EFacilityDesignC;
                data.Last().EFacilityendfacilityC = subitem.EFacilityendfacilityC;
                data.Last().EFacilityFarmerC = subitem.EFacilityFarmerC;
                data.Last().EFacilityPowerC = subitem.EFacilityPowerC;
                data.Last().EFacilityRegulationC = subitem.EFacilityRegulationC;
                data.Last().EFacilityWaterFacilityC = subitem.EFacilityWaterFacilityC;
                data.Last().EFacilityWaterReservoirC = subitem.EFacilityWaterReservoirC;
            }




            if (subitem.EFacilityD != 0)
            {
                data.Last().EFacilityD = subitem.EFacilityD;
                data.Last().EFacilityAreaD = subitem.EFacilityAreaD;
                data.Last().EFacilityDesignD = subitem.EFacilityDesignD;
                data.Last().EFacilityendfacilityD = subitem.EFacilityendfacilityD;
                data.Last().EFacilityFarmerD = subitem.EFacilityFarmerD;
                data.Last().EFacilityPowerD = subitem.EFacilityPowerD;
                data.Last().EFacilityRegulationD = subitem.EFacilityRegulationD;
                data.Last().EFacilityWaterFacilityD = subitem.EFacilityWaterFacilityD;
                data.Last().EFacilityWaterReservoirD = subitem.EFacilityWaterReservoirD;
            }


            if (subitem.EFacilityE != 0)
            {
                data.Last().EFacilityE = subitem.EFacilityE;
                data.Last().EFacilityAreaE = subitem.EFacilityAreaE;
                data.Last().EFacilityDesignE = subitem.EFacilityDesignE;
                data.Last().EFacilityendfacilityE = subitem.EFacilityendfacilityE;
                data.Last().EFacilityFarmerE = subitem.EFacilityFarmerE;
                data.Last().EFacilityPowerE = subitem.EFacilityPowerE;
                data.Last().EFacilityRegulationE = subitem.EFacilityRegulationE;
                data.Last().EFacilityWaterFacilityE = subitem.EFacilityWaterFacilityE;
                data.Last().EFacilityWaterReservoirE = subitem.EFacilityWaterReservoirE;
            }



            return data;

        }


        #region 填寫黃金廊道補助明細表
        public void CreateEngineeringReportDataGold(List<EngineeringReportViewGold> Data,ExcelWorksheet sh)
        {
                        
            #region 填寫excel
            sh.Cells[1, 1].Value = Data.First().EIA;
            sh.Cells[1, 6].Value = Data.First().Eyears;
            int irow = 5;

            foreach (var item in Data){
                

                sh.Cells[irow,1].Value = item.ENo;
                sh.Cells[irow,1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow,2].Value = item.EName;
                sh.Cells[irow,2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow,3].Value = item.EArea;
                sh.Cells[irow,3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow,4].Value = item.ELocation;
                sh.Cells[irow,4].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow,5].Value = item.EIrrigation;
                sh.Cells[irow,5].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow,6].Value = item.EndGoldPay;
                sh.Cells[irow,6].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow,7].Value = item.CtrlMatGoldPay;
                sh.Cells[irow,7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow,8].Formula ="+SUM( F" + irow + ":G" + irow + ")";
                sh.Cells[irow,8].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow,9].Value = item.EndCoaPay;
                sh.Cells[irow,9].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow, 10].Value = item.CtrlMatCoaPay;
                sh.Cells[irow, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow, 11].Value = item.EngCoaPay;
                sh.Cells[irow, 11].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow, 12].Value = item.PoolCoaPay;
                sh.Cells[irow, 12].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow, 13].Value = item.PlanPay;
                sh.Cells[irow, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 

                sh.Cells[irow, 14].Formula = "+SUM( I" + irow + ":M" + irow + ")";
                sh.Cells[irow, 14].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow, 15].Value = item.EngLuPay;
                sh.Cells[irow, 15].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow, 16].Value = item.PoolLuPay;
                sh.Cells[irow, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow, 17].Formula = "+SUM( O" + irow + ":P" + irow + ")";
                sh.Cells[irow, 17].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                sh.Cells[irow, 18].Value = item.Designer_Name;
                sh.Cells[irow, 18].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin); 
                irow++;
            }
            
            sh.Column(1).Style.Font.Size = 8;
            sh.Cells[1, 1].Style.Font.Size = 16;
            sh.Column(2).Style.Font.Size = 10;
            //sh.Column(3).Style.Numberformat.Format = "#,##0"; 
            sh.Column(3).Style.Numberformat.Format = "0.0000";
            sh.Column(6).Style.Numberformat.Format = "#,##0";
            sh.Column(7).Style.Numberformat.Format = "#,##0";
            sh.Column(8).Style.Numberformat.Format = "#,##0";

            for (int i = 1; i <= 8; i++)
            {
                sh.Column(i).Style.Font.Name = "標楷體";
            }
            

            #endregion
            
        }

        #endregion


        #region 設定管路補助金額明細表格
        public class EngineeringReportView
        {/// 管路工程設施輔助
            public string EIA { get; set; }
            /// <summary>
            ///水利會
            /// </summary>
            public int Eyears { get; set; }
            /// <summary>
            ///年度
            /// </summary>
            public string ENo { get; set; }
            /// <summary>
            /// 設施編號
            /// </summary>
            public string EName { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public float EArea { get; set; }
            /// <summary>
            /// 面積(公頃)
            /// </summary>
            public string ELocation { get; set; }
            /// <summary>
            /// 地點
            /// </summary>
            public string EIrrigation { get; set; }
            /// <summary>
            /// 灌溉型式
            /// </summary>
            public int EFarmer { get; set; }
            /// <summary>
            /// 農戶配合款
            /// </summary>
            public int Eendfacility { get; set; }
            /// <summary>
            /// 末端設施
            /// </summary>
            public int EWaterFacility { get; set; }
            /// <summary>
            /// 水源設施
            /// </summary>
            public int ERegulation { get; set; }
            /// <summary>
            /// 調控設施
            /// </summary>
            public int EWaterReservoir { get; set; }
            /// <summary>
            /// 蓄水池
            /// </summary>
            public int EPowerEquipment { get; set; }
            /// <summary>
            /// 動力設備
            /// </summary>
            public int EDesignCharges { get; set; }
            /// <summary>
            /// 設計費
            /// </summary>
            //public string ESubsidies { get; set; }
            ///// <summary>
            ///// 補助費
            ///// </summary>
            //public string EPercentage { get; set; }
            ///// <summary>
            ///// 百分比
            ///// </summary> 
            //public string ETotalCost { get; set; }
            ///// <summary>
            ///// 總工程費
            ///// </summary> 
            public int EFacilityA { get; set; }
            /// <summary>
            /// 穿孔管設施數量
            /// </summary> 
            public int EFacilityB { get; set; }
            /// <summary>
            /// 噴頭設施數量
            /// </summary> 
            public int EFacilityC { get; set; }
            /// <summary>
            /// 滴灌設施數量
            /// </summary> 
            public int EFacilityD { get; set; }
            /// <summary>
            /// 微噴設施數量
            /// </summary> 
            public int EFacilityE { get; set; }
            /// <summary>
            /// 軟管澆灌設施數量
            /// </summary> 
            public float EFacilityAreaA { get; set; }
            /// <summary>
            /// 穿孔管設施面積
            /// </summary> 
            public float EFacilityAreaB { get; set; }
            /// <summary>
            /// 噴頭設施面積
            /// </summary> 
            public float EFacilityAreaC { get; set; }
            /// <summary>
            /// 滴灌設施面積
            /// </summary> 
            public float EFacilityAreaD { get; set; }
            /// <summary>
            /// 微噴設施面積
            /// </summary> 
            public float EFacilityAreaE { get; set; }
            /// <summary>
            /// 軟管澆灌設施面積
            /// </summary> 
            public int EFacilityFarmerA { get; set; }
            /// <summary>
            /// 穿孔管設施農戶配合款
            /// </summary> 
            public int EFacilityFarmerB { get; set; }
            /// <summary>
            /// 噴頭設施農戶配合款
            /// </summary> 
            public int EFacilityFarmerC { get; set; }
            /// <summary>
            /// 滴灌設施農戶配合款
            /// </summary> 
            public int EFacilityFarmerD { get; set; }
            /// <summary>
            /// 微噴設施農戶配合款
            /// </summary> 
            public int EFacilityFarmerE { get; set; }
            /// <summary>
            /// 軟管澆灌設施農戶配合款
            /// </summary> 
            public int EFacilityendfacilityA { get; set; }
            /// <summary>
            /// 穿孔管末端設施款
            /// </summary> 
            public int EFacilityendfacilityB { get; set; }
            /// <summary>
            /// 噴頭末端設施款
            /// </summary> 
            public int EFacilityendfacilityC { get; set; }
            /// <summary>
            /// 滴灌末端設施款
            /// </summary> 
            public int EFacilityendfacilityD { get; set; }
            /// <summary>
            /// 微噴末端設施款
            /// </summary> 
            public int EFacilityendfacilityE { get; set; }
            /// <summary>
            /// 軟管澆灌末端設施款
            /// </summary> 
            public int EFacilityWaterFacilityA { get; set; }
            /// <summary>
            /// 穿孔管設施水源設施款
            /// </summary> 
            public int EFacilityWaterFacilityB { get; set; }
            /// <summary>
            /// 噴頭設施水源設施款
            /// </summary> 
            public int EFacilityWaterFacilityC { get; set; }
            /// <summary>
            /// 滴灌設施水源設施款
            /// </summary> 
            public int EFacilityWaterFacilityD { get; set; }
            /// <summary>
            /// 微噴設施水源設施款
            /// </summary> 
            public int EFacilityWaterFacilityE { get; set; }
            /// <summary>
            /// 軟管澆灌設施水源設施款
            /// </summary> 
            public int EFacilityRegulationA { get; set; }
            /// <summary>
            /// 穿孔管設施調控設施款
            /// </summary> 
            public int EFacilityRegulationB { get; set; }
            /// <summary>
            /// 噴頭設施調控設施款
            /// </summary> 
            public int EFacilityRegulationC { get; set; }
            /// <summary>
            /// 滴灌設施調控設施款
            /// </summary> 
            public int EFacilityRegulationD { get; set; }
            /// <summary>
            /// 微噴設施調控設施款
            /// </summary> 
            public int EFacilityRegulationE { get; set; }
            /// <summary>
            /// 軟管澆灌設施調控設施款
            /// </summary> 
            public int EFacilityWaterReservoirA { get; set; }
            /// <summary>
            /// 穿孔管設施蓄水池款
            /// </summary> 
            public int EFacilityWaterReservoirB { get; set; }
            /// <summary>
            /// 噴頭設施蓄水池款
            /// </summary> 
            public int EFacilityWaterReservoirC { get; set; }
            /// <summary>
            /// 滴灌設施蓄水池款
            /// </summary> 
            public int EFacilityWaterReservoirD { get; set; }
            /// <summary>
            /// 微噴設施蓄水池款
            /// </summary> 
            public int EFacilityWaterReservoirE { get; set; }
            /// <summary>
            /// 軟管澆灌設施蓄水池款
            /// </summary> 
            public int EFacilityPowerA { get; set; }
            /// <summary>
            /// 穿孔管設施動力設備款
            /// </summary> 
            public int EFacilityPowerB { get; set; }
            /// <summary>
            /// 噴頭設施動力設備款
            /// </summary> 
            public int EFacilityPowerC { get; set; }
            /// <summary>
            /// 滴灌設施動力設備款
            /// </summary> 
            public int EFacilityPowerD { get; set; }
            /// <summary>
            /// 微噴設施動力設備款
            /// </summary> 
            public int EFacilityPowerE { get; set; }
            /// <summary>
            /// 穿孔管設施設計費
            /// </summary> 
            public int EFacilityDesignA { get; set; }
            /// <summary>
            /// 噴頭設施設計費
            /// </summary> 
            public int EFacilityDesignB { get; set; }
            /// <summary>
            /// 微噴設施設計費
            /// </summary> 
            public int EFacilityDesignC { get; set; }
            /// <summary>
            /// 滴灌設施設計費
            /// </summary> 
            public int EFacilityDesignD { get; set; }
            /// <summary>
            /// 微噴設施設計費
            /// </summary> 
            public int EFacilityDesignE { get; set; }
            /// <summary>
            /// 設計者
            /// </summary>
            public string Designer_Name { get; set; }
        }
        #endregion

        #region 設定黃金廊道管路補助明細表
        public class EngineeringReportViewGold
        {
            public string EIA { get; set; }
            /// <summary>
            ///水利會
            /// </summary>
            public int Eyears { get; set; }
            /// <summary>
            ///年度
            /// </summary>
            public string ENo { get; set; }
            /// <summary>
            /// 設施編號
            /// </summary>
            public string EName { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public float EArea { get; set; }
            /// <summary>
            /// 面積(公頃)
            /// </summary>
            public string ELocation { get; set; }
            /// <summary>
            /// 地點
            /// </summary>
            public string EIrrigation { get; set; }
            /// <summary>
            /// 灌溉型式
            /// </summary>
            public int EndGoldPay { get; set; }
            /// <summary>
            /// 末端設施七星補助款
            /// </summary>
            public int CtrlMatGoldPay { get; set; }
            /// <summary>
            /// 調控設施七星補助款
            /// </summary>
            public int PoolCoaPay { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int PoolLuPay { get; set; }
            public int EngCoaPay { get; set; }
            public int EngLuPay { get; set; }
            public int EndCoaPay { get; set; }
            public int EndLuPay { get; set; }
            public int PlanPay { get; set; }
            public int CtrlMatCoaPay { get; set; }
            /// <summary>
            /// 設計者
            /// </summary>
            public string Designer_Name { get; set; }
           
        }
        #endregion

        
        
    }

}
