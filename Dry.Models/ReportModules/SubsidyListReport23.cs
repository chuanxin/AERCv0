using CusNPOI;
using Dry.Models.CommonCls;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Dry.Models.ReportModules
{
    public class SubsidyListReport23
    {
        DryEntities DryDB = new DryEntities();
        public byte[] GetReport_SubsidyList(short unit, int year, int IANumStart, int IANumEnd, string sourcepath, string Name)
        {
            GetData gd = new GetData();
            List<Farm> farms = DryDB.Farm.ToList();
            
            DataStruct data = new DataStruct();
            
            var DesIdresult = (from summview in DryDB.SummaryView

                          where summview.ApplyUnit == unit && summview.ApplyYear == year && summview.Step >= 7 && summview.IANum >= IANumStart && summview.IANum <= IANumEnd

                          select new 
                          {
                              summview.DesId,
                              summview.Designer_Name
                          }).Distinct().ToList();

            data.DesData = new List<Deslist>();
            foreach (var Item in DesIdresult)
            {
                Deslist list = new Deslist();
                list.DesID = Guid.Parse(Item.DesId.ToString());
                list.DesignerName = Item.Designer_Name;
                data.DesData.Add(list);
            }
            //end
            var resdata = (from summview in DryDB.SummaryView
                           join lasvermno in DryDB.LastVerOfMapNo on summview.MapNo equals lasvermno.MapNo
                           where summview.ApplyUnit == unit && summview.ApplyYear == year && summview.Step >= 7 && summview.IANum >= IANumStart && summview.IANum <= IANumEnd
                           select new
                           {
                               summview.MapNo,
                               summview.IANum,
                               summview.Name,
                               summview.IdNo,
                               summview.Addr,
                               summview.DesId,
                               summview.Designer_Name
                           }).OrderBy(m => m.IANum).ToList();
            data.IAName = gd.GetUnitName(unit);
            data.ApplyYear = year;
            data.ApplyData = new List<ApplyList>();
            List<EndTypeList> endTypeList = DryDB.EndTypeList.ToList();
            List<FacTypeList> facTypeList = DryDB.FacTypeList.ToList();
            foreach (var farmerItem in resdata)
            {
                ApplyList list = new ApplyList();
                int MapNo = farmerItem.MapNo.Value;
                list.IANum = farmerItem.IANum;
                list.FarmerName = farmerItem.Name;
                list.FarmerID = farmerItem.IdNo;
                list.Addr = farmerItem.Addr.Replace(" ", "");
                list.DesignerID = farmerItem.DesId;
                list.DesignerName = farmerItem.Designer_Name;
                var farmData = (from farm in farms
                                join landdata in DryDB.LandData on farm.Section equals landdata.Section_Id
                                where farm.MapNo == farmerItem.MapNo
                                select new
                                {
                                    farm.FNo,
                                    landdata.City,
                                    landdata.Town,
                                    landdata.Section,
                                    landdata.Subsection,
                                    farm.LandNo,
                                    farm.BuildArea
                                }).OrderBy(m => m.Section).ToList();
                if(farmData.Count > 0)
                {
                    list.SectionName = farmData.FirstOrDefault().City +
                        farmData.FirstOrDefault().Town +
                        farmData.FirstOrDefault().Section + "段" +
                        (farmData.FirstOrDefault().Subsection == "" ? "" : "-" + farmData.FirstOrDefault().Subsection + "小段") +
                        ",等" + farmData.Count + "筆";
                    List<EndType> endType = DryDB.EndType.Where(m => m.MapNo == MapNo).ToList();
                    foreach (var item in endType)
                    {
                        if (item.EndTypeCode == 1)
                        {
                            list.FacEndType += endTypeList.Find(m => m.EndType == item.EndTypeCode).EndTypeCNS + " " ;
                        }
                        else
                        {
                            list.FacEndType += facTypeList.Find(m => m.FacType == item.FacType).FTpeCNS + endTypeList.Find(m => m.EndType == item.EndTypeCode).EndTypeCNS + " ";
                        }
                    }
                    list.Area = Math.Round(farmData.Sum(m => m.BuildArea) / 10000, 4, MidpointRounding.AwayFromZero);
                }
                list.TotalPrice = DryDB.TotalFunds.Any(m => m.MapNo == MapNo) ? DryDB.TotalFunds.Where(m => m.MapNo == MapNo).FirstOrDefault().Total.Value : 0;
                list.FarmerPay = DryDB.TotalFunds.Any(m => m.MapNo == MapNo) ? DryDB.TotalFunds.Where(m => m.MapNo == MapNo).FirstOrDefault().FarmerFee : 0;
                list.PigingTotalPrice = DryDB.Pay.Any(m => m.MapNo == MapNo && m.ItemCode == 1) ? DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode == 1).FirstOrDefault().PayMoney : 0;
                list.PoolEngPrice = DryDB.Pay.Any(m => m.MapNo == MapNo && (m.ItemCode == 3 || m.ItemCode == 5 || m.ItemCode == 6)) ? DryDB.Pay.Where(m => m.MapNo == MapNo && (m.ItemCode == 3 || m.ItemCode == 5 || m.ItemCode == 6)).Sum(m=>m.PayMoney) : 0;
                list.PlanningPrice = DryDB.Pay.Any(m => m.MapNo == MapNo && m.ItemCode == 2) ? DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode == 2).FirstOrDefault().PayMoney : 0;
                list.CtrlMatPay = DryDB.Pay.Any(m => m.MapNo == MapNo && m.ItemCode == 4) ? DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode == 4).FirstOrDefault().PayMoney : 0;

                data.ApplyData.Add(list);
            }
            byte[] returnData = SubsidyList(data, sourcepath, Name);
            return returnData;
        }
        private byte[] SubsidyList(DataStruct dt, string sourcepath, string Name)
        {
            CusCopyRow cus = new CusCopyRow();
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook wk = new HSSFWorkbook(fs);
            

            int reportlength = 38;
            int RowsCount = dt.ApplyData.Count;

            int pagecount = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(RowsCount / 16))) + 1;
            if (RowsCount % 16 >= 14 || RowsCount % 16 == 0)
            {
                pagecount = pagecount + 1;
            }

            HSSFSheet wksheet = wk.GetSheetAt(0) as HSSFSheet;
            
            for (int j = 1; j < pagecount; j++)
            {
                for (int row = 0; row < 38; row++)
                {
                    
                    cus.CopyRow(wksheet, row, reportlength * j + row);
                }
            }


            SetData(wk, wksheet, dt, reportlength, Name);
            MemoryStream files = new MemoryStream();
            wk.Write(files);
            files.Close();
            return files.ToArray();
        }
        private HSSFSheet SetData(HSSFWorkbook wk, HSSFSheet sh, DataStruct dt, int reportlength, string Name)
        {
            HSSFCellStyle cs = wk.CreateCellStyle() as HSSFCellStyle;
            HSSFFont font1 = wk.CreateFont() as HSSFFont;
            int count = dt.ApplyData.Count;
            int totalpage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(count / 16))) + 1;
            if (count % 16 >= 14 || count % 16 == 0)
            {
                totalpage = totalpage + 1;
            }
            int pagecount = 1;
            int j = 0;
            int sumCell = 0;
            sh.GetRow(0).GetCell(0).SetCellValue(Name + dt.ApplyYear.ToString() + "年度管路灌溉推廣與發展計畫補助印領清冊");
            sh.GetRow(1).GetCell(0).SetCellValue("頁數: " + pagecount);
            double priceA = 0, priceB = 0;
            for (int i = 0; i < count; i++)
            {
                
                sh.GetRow(4 + j).GetCell(0).SetCellValue(dt.ApplyData[i].IANum);
                sh.GetRow(4 + j).GetCell(1).SetCellValue(dt.ApplyData[i].FarmerName);
                sh.GetRow(5 + j).GetCell(1).SetCellValue(dt.ApplyData[i].FarmerID);
                sh.GetRow(4 + j).GetCell(2).SetCellValue(dt.ApplyData[i].Addr);
                sh.GetRow(4 + j).GetCell(3).SetCellValue(dt.ApplyData[i].SectionName);
                sh.GetRow(4 + j).GetCell(4).CellStyle.WrapText=true;
                sh.GetRow(4 + j).GetCell(4).SetCellValue(dt.ApplyData[i].FacEndType);
                sh.GetRow(4 + j).GetCell(5).SetCellValue(dt.ApplyData[i].Area);
                
                sh.GetRow(4 + j).GetCell(6).SetCellValue(dt.ApplyData[i].FarmerPay);
                sh.GetRow(4 + j).GetCell(7).SetCellValue(dt.ApplyData[i].PigingTotalPrice);
                sh.GetRow(4 + j).GetCell(8).SetCellValue(dt.ApplyData[i].PoolEngPrice);
                sh.GetRow(4 + j).GetCell(9).SetCellValue(dt.ApplyData[i].CtrlMatPay);

                sumCell = 4 + j + 1;
                
                //sh.GetRow(4 + j).GetCell(10).CellFormula = "IF(H" + sumCell + "+J" + sumCell + "=0,\"\",I" + sumCell + "+J" + sumCell + ")";
                sh.GetRow(4 + j).GetCell(10).CellFormula = "SUM(H" + sumCell + ":J" + sumCell + ")";
                sh.GetRow(4 + j).GetCell(11).SetCellValue(dt.ApplyData[i].PlanningPrice);
                sh.GetRow(4 + j).GetCell(12).SetCellValue(dt.ApplyData[i].TotalPrice);
                //sh.GetRow(4 + j).GetCell(13).SetCellValue(dt.ApplyData[i].DesignerName);

                j = j + 2;
                if (i != 0 && (i + 1) % 16 == 0)
                {
                    j = j + 6;
                    pagecount++;
                    sh.GetRow(0 + j).GetCell(0).SetCellValue(dt.IAName + dt.ApplyYear.ToString() + "年度管路灌溉推廣與發展計畫補助印領清冊");
                    sh.GetRow(1 + j).GetCell(0).SetCellValue("頁數: " + pagecount);
                }

                
                
                if (dt.ApplyData[i].DesignerID.ToString() == dt.DesData[0].DesID.ToString())
                {
                    priceA = priceA + dt.ApplyData[i].PlanningPrice;
                }
                else if (dt.ApplyData[i].DesignerID.ToString() == dt.DesData[1].DesID.ToString())
                {
                    priceB = priceB + dt.ApplyData[i].PlanningPrice;
                }
                ///
            }

            sh.ForceFormulaRecalculation = true;
            #region 最後畫合計部分格子

            
            double area = 0;
            int workfee = 0, farmersubsidy = 0, farmfacility = 0, waterpowerfacility = 0, total = 0,ctrlmat = 0,planfee = 0,totalall = 0;
            area = dt.ApplyData.Sum(m => m.Area);
            workfee = dt.ApplyData.Sum(m => m.TotalPrice);
            farmersubsidy = dt.ApplyData.Sum(m => m.FarmerPay);
            farmfacility = dt.ApplyData.Sum(m => m.PigingTotalPrice);
            waterpowerfacility = dt.ApplyData.Sum(m => m.PoolEngPrice);
            ctrlmat = dt.ApplyData.Sum(m => m.CtrlMatPay);
            total = farmfacility + waterpowerfacility + ctrlmat;
            planfee = dt.ApplyData.Sum(m => m.PlanningPrice);
            totalall = total + farmersubsidy + planfee;
            
            var DesIddata = (from m in dt.ApplyData
                             where m.DesignerName != null
                      select new { m.DesignerID, m.DesignerName }).Distinct().ToList();
            

            sh.GetRow(reportlength * totalpage - 8).GetCell(5).SetCellValue(area);
            //sh.GetRow(reportlength * totalpage - 8).GetCell(6).SetCellValue(workfee);
            sh.GetRow(reportlength * totalpage - 8).GetCell(6).SetCellValue(farmersubsidy);
            sh.GetRow(reportlength * totalpage - 8).GetCell(7).SetCellValue(farmfacility);
            sh.GetRow(reportlength * totalpage - 8).GetCell(8).SetCellValue(waterpowerfacility);
            sh.GetRow(reportlength * totalpage - 8).GetCell(9).SetCellValue(ctrlmat);
            sh.GetRow(reportlength * totalpage - 8).GetCell(10).SetCellValue(total);
            sh.GetRow(reportlength * totalpage - 8).GetCell(11).SetCellValue(planfee);
            sh.GetRow(reportlength * totalpage - 8).GetCell(12).SetCellValue(totalall);

            RemoveMergeCells(sh, reportlength * totalpage - 8, 0);
            RemoveMergeCells(sh, reportlength * totalpage - 8, 1);
            RemoveMergeCells(sh, reportlength * totalpage - 8, 2);
            RemoveMergeCells(sh, reportlength * totalpage - 8, 3);
            sh.AddMergedRegion(new CellRangeAddress(reportlength * totalpage - 8, reportlength * totalpage - 7, 0, 3));
            RemoveMergeCells(sh, reportlength * totalpage - 6, 0);
            RemoveMergeCells(sh, reportlength * totalpage - 6, 1);
            RemoveMergeCells(sh, reportlength * totalpage - 6, 2);
            RemoveMergeCells(sh, reportlength * totalpage - 6, 3);
            sh.AddMergedRegion(new CellRangeAddress(reportlength * totalpage - 6, reportlength * totalpage - 5, 0, 3));
            RemoveMergeCells(sh, reportlength * totalpage - 4, 0);
            RemoveMergeCells(sh, reportlength * totalpage - 4, 1);
            RemoveMergeCells(sh, reportlength * totalpage - 4, 2);
            RemoveMergeCells(sh, reportlength * totalpage - 4, 3);
            sh.AddMergedRegion(new CellRangeAddress(reportlength * totalpage - 4, reportlength * totalpage - 3, 0, 3));
            sh.GetRow(reportlength * totalpage - 8).GetCell(0).SetCellValue("合                              計");
            
            switch (DesIddata.Count)
            {
                case 0:
                    sh.GetRow(reportlength * totalpage - 6).GetCell(0).SetCellValue("末端設計費(末端工程費 x 2%) 設計人：");
                    break;
                case 1:
                    sh.GetRow(reportlength * totalpage - 6).GetCell(0).SetCellValue("末端設計費(末端工程費 x 2%) 設計人：" + DesIddata[0].DesignerName);
                    sh.GetRow(reportlength * totalpage - 6).GetCell(11).SetCellValue(priceA);
                    break;
                case 2:
                    sh.GetRow(reportlength * totalpage - 4).GetCell(0).SetCellValue("末端設計費(末端工程費 x 2%) 設計人：" + DesIddata[1].DesignerName);
                    sh.GetRow(reportlength * totalpage - 4).GetCell(11).SetCellValue(priceB);
                    break;
            }

            
            cs.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            cs.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            cs.BorderLeft = BorderStyle.Medium;
            
            font1.FontName = "標楷體";
            font1.FontHeightInPoints = 12;
            sh.GetRow(reportlength * totalpage - 8).GetCell(0).CellStyle = cs;
            sh.GetRow(reportlength * totalpage - 8).GetCell(0).CellStyle.SetFont(font1);
            sh.GetRow(reportlength * totalpage - 6).GetCell(0).CellStyle = cs;
            sh.GetRow(reportlength * totalpage - 6).GetCell(0).CellStyle.SetFont(font1);
            sh.GetRow(reportlength * totalpage - 4).GetCell(0).CellStyle = cs;
            sh.GetRow(reportlength * totalpage - 4).GetCell(0).CellStyle.SetFont(font1);
            #endregion
            return sh;
        }

        private static void RemoveMergeCells(HSSFSheet sheet, int row, int column)
        {
            int sheetMergeCount = sheet.NumMergedRegions;
            int index = 0;
            for (int i = 0; i < sheetMergeCount; i++)
            {
                CellRangeAddress ca = sheet.GetMergedRegion(i); 
                int firstColumn = ca.FirstColumn;
                int lastColumn = ca.LastColumn;
                int firstRow = ca.FirstRow;
                int lastRow = ca.LastRow;
                if (row >= firstRow && row <= lastRow)
                {
                    if (column >= firstColumn && column <= lastColumn)
                    {
                        index = i;
                    }
                }
            }
            sheet.RemoveMergedRegion(index);
        }
        public class DataStruct
        {
            public int ApplyYear { get; set; }
            public string IAName { get; set; }

            public List<ApplyList> ApplyData { get; set; }
            public List<Deslist> DesData { get; set; }
        }
        public class ApplyList
        {
            public int IANum { get; set; }
            public string FarmerName { get; set; }
            public string FarmerID { get; set; }
            public string Addr { get; set; }
            public string Town { get; set; }
            public string SectionName { get; set; }
            public string LandNo { get; set; }
            public double Area { get; set; }
            public string FacEndType { get; set; }
            public int TotalPrice { get; set; }
            public int PlanningPrice { get; set; }
            public int FarmerPay { get; set; }
            public int PigingTotalPrice { get; set; }
            public int PoolEngPrice { get; set; }
            public int CtrlMatPay { get; set; }
            public Guid DesignerID { get; set; }
            public string DesignerName { get; set; }
        }
        public class Deslist
        {
            public Guid DesID { get; set; }
            public string DesignerName { get; set; }
            public int DesignerPrice { get; set; }
        }
    }
}
