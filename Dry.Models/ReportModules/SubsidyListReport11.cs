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
    public class SubsidyListReport11
    {
        DryEntities DryDB = new DryEntities();
        public byte[] GetReport_SubsidyList(short unit, int year, int IANumStart, int IANumEnd, string sourcepath, string CheeseList, int number
            , string name1,string s1, string e1, string name2, string s2, string e2)
        {
            GetData gd = new GetData();
            List<Farm> farms = DryDB.Farm.ToList();
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
           // List<string> resdata = new List<string>();
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
            //Data
            if (CheeseList.Equals("A"))
            {            
                 resdata = (from summview in DryDB.SummaryView
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
            }
            else if (CheeseList.Equals("B"))
            {
                 resdata = (from summview in DryDB.SummaryView
                               join pools in DryDB.PoolApply on summview.MapNo equals pools.MapNo
                               join pay in DryDB.TotalFunds on summview.MapNo equals pay.MapNo
                               join lasvermno in DryDB.LastVerOfMapNo on summview.MapNo equals lasvermno.MapNo
                               where summview.ApplyUnit == unit && summview.ApplyYear == year && summview.Step >= 7 && (pay.FarmerFee == 0) && (pools.ApplyUnit == 0) && summview.IANum >= IANumStart && summview.IANum <= IANumEnd
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
            }
            else
            {
                 resdata = (from summview in DryDB.SummaryView
                               join pools in DryDB.PoolApply on summview.MapNo equals pools.MapNo
                               join lasvermno in DryDB.LastVerOfMapNo on summview.MapNo equals lasvermno.MapNo
                               where summview.ApplyUnit == unit && summview.ApplyYear == year && summview.Step >= 7 && (pools.ApplyUnit == 17) && summview.IANum >= IANumStart && summview.IANum <= IANumEnd
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
            }
            //dataend




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
                    if (endType.Count == 0)
                    {
                        list.FacEndType = "其它";
                    }else
                    {
                        foreach (var item in endType)
                        {
                            if (item.EndTypeCode == 1)
                            {
                                //list.FacEndType += endTypeList.Find(m => m.EndType == item.EndTypeCode).EndTypeCNS + " " ;
                                list.FacEndType += endTypeList.Find(m => m.EndType == item.EndTypeCode).Catalog + " ";
                            }
                            else
                            {
                                // list.FacEndType += facTypeList.Find(m => m.FacType == item.FacType).FTpeCNS + endTypeList.Find(m => m.EndType == item.EndTypeCode).EndTypeCNS + " ";
                                list.FacEndType += endTypeList.Find(m => m.EndType == item.EndTypeCode).Catalog + " ";
                            }
                        }
                    }
                    
                    list.Area = Math.Round(farmData.Sum(m => m.BuildArea) / 10000, 4, MidpointRounding.AwayFromZero);
                }
                list.TotalPrice = DryDB.TotalFunds.Any(m => m.MapNo == MapNo) ? DryDB.TotalFunds.Where(m => m.MapNo == MapNo).FirstOrDefault().Total.Value : 0;
                list.FarmerPay = DryDB.TotalFunds.Any(m => m.MapNo == MapNo) ? DryDB.TotalFunds.Where(m => m.MapNo == MapNo).FirstOrDefault().FarmerFee : 0;
                list.PigingTotalPrice = DryDB.Pay.Any(m => m.MapNo == MapNo && m.ItemCode == 1) ? DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode == 1).FirstOrDefault().PayMoney : 0;
                list.PoolEngPrice = DryDB.Pay.Any(m => m.MapNo == MapNo && (m.ItemCode == 3 || m.ItemCode == 5 || m.ItemCode == 6)) ? DryDB.Pay.Where(m => m.MapNo == MapNo && (m.ItemCode == 3 || m.ItemCode == 5 || m.ItemCode == 6)).Sum(m=>m.PayMoney) : 0;
                list.PlanningPrice = DryDB.Pay.Any(m => m.MapNo == MapNo && m.ItemCode == 2) ? DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode == 2).FirstOrDefault().PayMoney : 0;
                //list.PlanningPrice = DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode == 2).FirstOrDefault().PayMoney;
                list.CtrlMatPay = DryDB.Pay.Any(m => m.MapNo == MapNo && m.ItemCode == 4) ? DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode == 4).FirstOrDefault().PayMoney : 0;

                data.ApplyData.Add(list);
            }
            byte[] returnData = SubsidyList(data, sourcepath, CheeseList, number,name1,s1,e1,name2,s2,e2);
            return returnData;
        }
        private byte[] SubsidyList(DataStruct dt, string sourcepath, string CheeseList, int number
            ,string name1,string s1, string e1, string name2, string s2, string e2)
        {
            CusCopyRow cus = new CusCopyRow();
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite);
            HSSFWorkbook wk = new HSSFWorkbook(fs);
            

            int reportlength = 26;
            int RowsCount = dt.ApplyData.Count;
            int pagerow = 9;
            int pagecount = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(RowsCount / pagerow))) + 1;

            
            if (RowsCount % pagerow >= 7 )
            {
                pagecount = pagecount + 1;
            }
            if (RowsCount < 9 )
            {
                pagecount = pagecount + 1;
            }
            HSSFSheet wksheet = wk.GetSheetAt(0) as HSSFSheet;
            
            for (int j = 1; j < pagecount; j++)
            {
                for (int row = 0; row < 26; row++)
                {
                    
                    cus.CopyRow(wksheet, row, reportlength * j + row);
                }
            }


            SetData(wk, wksheet, dt, reportlength, CheeseList, number,name1,s1,e1,name2,s2,e2,pagecount);
            MemoryStream files = new MemoryStream();
            wk.Write(files);
            files.Close();
            return files.ToArray();
        }
        private HSSFSheet SetData(HSSFWorkbook wk, HSSFSheet sh, DataStruct dt, int reportlength ,string CheeseList, int number
            ,string name1,string s1, string e1, string name2, string s2, string e2,int pagecounts)
        {
            HSSFCellStyle cs = wk.CreateCellStyle() as HSSFCellStyle;
            HSSFFont font1 = wk.CreateFont() as HSSFFont;
            int count = dt.ApplyData.Count;
            //int totalpage = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(count / 9))) + 1;
            int totalpage = pagecounts;
            
            sh.GetRow(24).GetCell(0).SetCellValue("承辦人：");
            sh.GetRow(24).GetCell(3).SetCellValue("股長：");
            sh.GetRow(24).GetCell(6).SetCellValue("主任：");
            sh.GetRow(25).GetCell(0).SetCellValue("主計室：");
            sh.GetRow(25).GetCell(2).SetCellValue("主計室股長：");
            sh.AddMergedRegion(new CellRangeAddress(25, 25, 4, 5));
            sh.GetRow(25).GetCell(4).SetCellValue("主計室主任：");
            sh.GetRow(25).GetCell(8).SetCellValue("總幹事：");
            sh.GetRow(25).GetCell(10).SetCellValue("會長：");
            //if (count % 9 >=8 || count % 9 == 0)
            //{
            //    totalpage = totalpage + 1;
            //}
            int pagecount = 1;
            int j = 0;
            int sumCell = 0;
            //
            #region 第一頁表頭
		 
	
            if (CheeseList.Equals("A"))
            {
                sh.GetRow(0 + j).GetCell(0).SetCellValue(dt.IAName + dt.ApplyYear.ToString() + "年度第" + number + "梯次(推廣)管路灌溉設施補助金額明細表");
            }
            else if (CheeseList.Equals("B"))
            {
                sh.GetRow(0 + j).GetCell(0).SetCellValue(dt.IAName + dt.ApplyYear.ToString() + "年度第" + number + "梯次(推廣)蓄水池灌溉設施補助金額明細表");
            }
            else
            {
                sh.GetRow(0 + j).GetCell(0).SetCellValue(dt.IAName + dt.ApplyYear.ToString() + "年度第" + number + "梯次(瑠公)蓄水池灌溉設施補助金額明細表");
            }
            sh.GetRow(1).GetCell(0).SetCellValue("頁數: " + pagecount);
            #endregion
            
            j = 4;
            int pageSumSatrt = j+1;
            int pageSumEnd = 0;
            #region 寫表身
            for (int i = 0; i < count; i++)
            {

                
                
                sh.GetRow(j).GetCell(0).SetCellValue(dt.ApplyData[i].IANum);
                sh.GetRow(j).GetCell(1).SetCellValue(dt.ApplyData[i].FarmerName);
                sh.GetRow(j + 1).GetCell(1).SetCellValue(dt.ApplyData[i].FarmerID);
                sh.GetRow(j).GetCell(2).SetCellValue(dt.ApplyData[i].Addr);
                sh.GetRow(j).GetCell(3).SetCellValue(dt.ApplyData[i].SectionName);
                sh.GetRow(j).GetCell(4).CellStyle.WrapText=true;
                sh.GetRow(j).GetCell(4).SetCellValue(dt.ApplyData[i].FacEndType);
                sh.GetRow(j).GetCell(5).SetCellValue(dt.ApplyData[i].Area);
                sh.GetRow(j).GetCell(6).SetCellValue(dt.ApplyData[i].PlanningPrice);
                sh.GetRow(j).GetCell(7).SetCellValue(dt.ApplyData[i].FarmerPay);
                //sh.GetRow(4 + j).GetCell(7).SetCellValue(dt.ApplyData[i].PigingTotalPrice);
                //sh.GetRow(4 + j).GetCell(8).SetCellValue(dt.ApplyData[i].PoolEngPrice);
                //sh.GetRow(4 + j).GetCell(9).SetCellValue(dt.ApplyData[i].CtrlMatPay);
                
                sh.GetRow(j).GetCell(8).SetCellValue(dt.ApplyData[i].PigingTotalPrice);
                sh.GetRow(j).GetCell(9).SetCellValue(dt.ApplyData[i].PoolEngPrice);
                //sh.GetRow(4 + j).GetCell(10).SetCellValue(dt.ApplyData[i].CtrlMatPay);

                sumCell = j + 1;
                //填入公式
                //sh.GetRow(4 + j).GetCell(10).CellFormula = "IF(H" + sumCell + "+J" + sumCell + "=0,\"\",I" + sumCell + "+J" + sumCell + ")";
                sh.GetRow(j).GetCell(10).CellFormula = "SUM(I" + sumCell + ":J" + sumCell + ")";
                //sh.GetRow(4 + j).GetCell(11).SetCellValue(dt.ApplyData[i].PlanningPrice);
                sh.GetRow(j).GetCell(11).SetCellValue(dt.ApplyData[i].TotalPrice);
                //sh.GetRow(4 + j).GetCell(12).SetCellValue(dt.ApplyData[i].DesignerName);
                if (i != 0 && ((i + 1) % 9 == 0)) 
                {
                    j = j + 2;
                    pageSumEnd = j;
                    
                    sh.GetRow(j).GetCell(0).SetCellValue("小計");
                    sh.GetRow(j).GetCell(5).CellFormula = "SUM(F" + (pageSumSatrt).ToString() + ":F" + (pageSumEnd).ToString() + ")";
                    sh.GetRow(j).GetCell(6).CellFormula = "SUM(G" + (pageSumSatrt).ToString() + ":G" + (pageSumEnd).ToString() + ")";
                    sh.GetRow(j).GetCell(7).CellFormula = "SUM(H" + (pageSumSatrt).ToString() + ":H" + (pageSumEnd).ToString() + ")";
                    sh.GetRow(j).GetCell(8).CellFormula = "SUM(I" + (pageSumSatrt).ToString() + ":I" + (pageSumEnd).ToString() + ")";
                    sh.GetRow(j).GetCell(9).CellFormula = "SUM(J" + (pageSumSatrt).ToString() + ":J" + (pageSumEnd).ToString() + ")";
                    sh.GetRow(j).GetCell(10).CellFormula = "SUM(K" + (pageSumSatrt).ToString() + ":K" + (pageSumEnd).ToString() + ")";
                    sh.GetRow(j).GetCell(11).CellFormula = "SUM(L" + (pageSumSatrt).ToString() + ":L" + (pageSumEnd).ToString() + ")";
                    
                    j = j + 4;
                    pagecount++;
                    if (CheeseList.Equals("A"))
                    {
                        sh.GetRow(j).GetCell(0).SetCellValue(dt.IAName + dt.ApplyYear.ToString() + "年度第" + number.ToString() + "梯次(推廣)管路灌溉設施補助金額明細表");
                    }
                    else if (CheeseList.Equals("B"))
                    {
                        sh.GetRow(j).GetCell(0).SetCellValue(dt.IAName + dt.ApplyYear.ToString() + "年度第" + number.ToString() + "梯次(推廣)管路灌溉設施補助金額明細表");
                    }
                    else
                    {
                        sh.GetRow(j).GetCell(0).SetCellValue(dt.IAName + dt.ApplyYear.ToString() + "年度第" + number.ToString() + "梯次(瑠公)管路灌溉設施補助金額明細表");
                    }
                    j = j + 1;
                    sh.GetRow(j).GetCell(0).SetCellValue("頁數: " + pagecount);
                    j = j + 1;
                    pageSumSatrt = j + 1;
                }
                j = j + 2;

                
              
            }
            #endregion
            //
            #region 最後一筆小計
            pageSumEnd = j;
            sh.GetRow(j).GetCell(0).SetCellValue("小計");
            sh.GetRow(j).GetCell(5).CellFormula = "SUM(F" + (pageSumSatrt).ToString() + ":F" + (pageSumEnd).ToString() + ")";
            sh.GetRow(j).GetCell(6).CellFormula = "SUM(G" + (pageSumSatrt).ToString() + ":G" + (pageSumEnd).ToString() + ")";
            sh.GetRow(j).GetCell(7).CellFormula = "SUM(H" + (pageSumSatrt).ToString() + ":H" + (pageSumEnd).ToString() + ")";
            sh.GetRow(j).GetCell(8).CellFormula = "SUM(I" + (pageSumSatrt).ToString() + ":I" + (pageSumEnd).ToString() + ")";
            sh.GetRow(j).GetCell(9).CellFormula = "SUM(J" + (pageSumSatrt).ToString() + ":J" + (pageSumEnd).ToString() + ")";
            sh.GetRow(j).GetCell(10).CellFormula = "SUM(K" + (pageSumSatrt).ToString() + ":K" + (pageSumEnd).ToString() + ")";
            sh.GetRow(j).GetCell(11).CellFormula = "SUM(L" + (pageSumSatrt).ToString() + ":L" + (pageSumEnd).ToString() + ")";

            if (count % 9 >= 7)
            {
                

                j = reportlength * (totalpage - 1) ;
                if (CheeseList.Equals("A"))
                {
                    sh.GetRow(0 + j).GetCell(0).SetCellValue(dt.IAName + dt.ApplyYear.ToString() + "年度第" + number + "梯次(推廣)管路灌溉設施補助金額明細表");
                }
                else if (CheeseList.Equals("B"))
                {
                    sh.GetRow(0 + j).GetCell(0).SetCellValue(dt.IAName + dt.ApplyYear.ToString() + "年度第" + number + "梯次(推廣)蓄水池灌溉設施補助金額明細表");
                }
                else
                {
                    sh.GetRow(0 + j).GetCell(0).SetCellValue(dt.IAName + dt.ApplyYear.ToString() + "年度第" + number + "梯次(瑠公)蓄水池灌溉設施補助金額明細表");
                }
                sh.GetRow(1).GetCell(0).SetCellValue("頁數: " + pagecount);

            }
            
            #endregion
            
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
            
            

            sh.GetRow(reportlength * totalpage - 8).GetCell(5).SetCellValue(area);
            //sh.GetRow(reportlength * totalpage - 8).GetCell(6).SetCellValue(workfee);
            sh.GetRow(reportlength * totalpage - 8).GetCell(6).SetCellValue(planfee);
            sh.GetRow(reportlength * totalpage - 8).GetCell(7).SetCellValue(farmersubsidy);
            sh.GetRow(reportlength * totalpage - 8).GetCell(8).SetCellValue(farmfacility);
            sh.GetRow(reportlength * totalpage - 8).GetCell(9).SetCellValue(waterpowerfacility);
            //sh.GetRow(reportlength * totalpage - 8).GetCell(10).SetCellValue(ctrlmat);
            sh.GetRow(reportlength * totalpage - 8).GetCell(10).SetCellValue(total);
            
            sh.GetRow(reportlength * totalpage - 8).GetCell(11).SetCellValue(totalall);
            
            RemoveMergeCells(sh, reportlength * totalpage - 8, 0);
            RemoveMergeCells(sh, reportlength * totalpage - 8, 1);
            RemoveMergeCells(sh, reportlength * totalpage - 8, 2);
            RemoveMergeCells(sh, reportlength * totalpage - 8, 3);
            sh.AddMergedRegion(new CellRangeAddress(reportlength * totalpage - 8, reportlength * totalpage - 7, 0, 3));
           
            sh.GetRow(reportlength * totalpage - 8).GetCell(0).SetCellValue("合                              計");
            
            #region 設計人統計
            if (name1 != string.Empty && name1 != null)
            {
                if (s1 != string.Empty && e1 != string.Empty && s1 != null && e1 != null)
                {
                    int snum1 = int.Parse(s1);
                    int enum1 = int.Parse(e1);
                    int paydesignFee = dt.ApplyData.Where(m => m.IANum >= snum1 && m.IANum <= enum1).Sum(m => m.PlanningPrice);
                    RemoveMergeCells(sh, reportlength * totalpage - 6, 0);
                    RemoveMergeCells(sh, reportlength * totalpage - 6, 1);
                    RemoveMergeCells(sh, reportlength * totalpage - 6, 2);
                    RemoveMergeCells(sh, reportlength * totalpage - 6, 3);
                    sh.AddMergedRegion(new CellRangeAddress(reportlength * totalpage - 6, reportlength * totalpage - 5, 0, 3));
                    sh.GetRow(reportlength * totalpage - 6).GetCell(0).SetCellValue("末端規劃費(末端工程費 x 2%) 規劃人：" + name1 + 
                        "自設施編號" + snum1.ToString() + "至設施編號" + enum1.ToString() + "號");
                    sh.GetRow(reportlength * totalpage - 6).GetCell(6).CellStyle.WrapText = true;
                    sh.GetRow(reportlength * totalpage - 6).GetCell(6).SetCellValue(paydesignFee);
                }
            }
            if (name2 != string.Empty && name2 != null)
            {
                if (s2 != string.Empty && e2 != string.Empty && s2 != null && e2 != null)
                {
                    int snum1 = int.Parse(s2);
                    int enum1 = int.Parse(e2);
                    int paydesignFee = dt.ApplyData.Where(m => m.IANum >= snum1 && m.IANum <= enum1).Sum(m => m.PlanningPrice);
                    
                    RemoveMergeCells(sh, reportlength * totalpage - 4, 0 );
                    RemoveMergeCells(sh, reportlength * totalpage - 4, 1 );
                    RemoveMergeCells(sh, reportlength * totalpage - 4, 2 );
                    RemoveMergeCells(sh, reportlength * totalpage - 4, 3 );

                    
                    sh.AddMergedRegion(new CellRangeAddress(reportlength * totalpage - 4, reportlength * totalpage - 3, 0, 3));
                    sh.GetRow(reportlength * totalpage - 4).GetCell(0).SetCellValue("末端規劃費(末端工程費 x 2%) 規劃人：" + name2 +
                        "自設施編號" + snum1.ToString() + "至設施編號" + enum1.ToString() + "號");
                    sh.GetRow(reportlength * totalpage - 4).GetCell(0).CellStyle.WrapText = true;
                    sh.GetRow(reportlength * totalpage - 4).GetCell(6).SetCellValue(paydesignFee);
                }
            }

            #endregion
            #region 最後的印核章
            sh.GetRow(reportlength * totalpage - 2).GetCell(0).SetCellValue("承辦人：");
            sh.GetRow(reportlength * totalpage - 2).GetCell(3).SetCellValue("股長：");
            sh.GetRow(reportlength * totalpage - 2).GetCell(6).SetCellValue("主任：");
            sh.GetRow(reportlength * totalpage - 1).GetCell(0).SetCellValue("主計室：");
            sh.GetRow(reportlength * totalpage - 1).GetCell(2).SetCellValue("主計室股長：");
            sh.AddMergedRegion(new CellRangeAddress(reportlength * totalpage - 1, reportlength * totalpage - 1, 4, 5));
            sh.GetRow(reportlength * totalpage - 1).GetCell(4).SetCellValue("主計室主任：");
            sh.GetRow(reportlength * totalpage - 1).GetCell(8).SetCellValue("總幹事：");
            sh.GetRow(reportlength * totalpage - 1).GetCell(10).SetCellValue("會長：");
            #endregion
            

            
            cs.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
            cs.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            cs.BorderLeft = BorderStyle.Medium;
            
            //font1.FontName = "標楷體";
            //font1.FontHeightInPoints = 12;
            //sh.GetRow(reportlength * totalpage - 8).GetCell(0).CellStyle = cs;
            //sh.GetRow(reportlength * totalpage - 8).GetCell(0).CellStyle.SetFont(font1);
            //sh.GetRow(reportlength * totalpage - 6).GetCell(0).CellStyle = cs;
            //sh.GetRow(reportlength * totalpage - 6).GetCell(0).CellStyle.SetFont(font1);
            //sh.GetRow(reportlength * totalpage - 4).GetCell(0).CellStyle = cs;
            //sh.GetRow(reportlength * totalpage - 4).GetCell(0).CellStyle.SetFont(font1);
            #endregion
            #region 修正第一頁表頭
            sh.AddMergedRegion(new CellRangeAddress(0,0,0,11));
            #endregion
            return sh;
        }

        //private static void RemoveMergeCells(int times, int rowIndex, HSSFSheet sheet)
        //{

        //    for (int execTime = 0; execTime < times; execTime++)
        //    {
        //        for (int i = 0; i < sheet.NumMergedRegions; i++)
        //        {
        //            CellRangeAddress cellRangeAddress = sheet.GetMergedRegion(i);
        //            if (cellRangeAddress.FirstRow == rowIndex)
        //            {
        //                //if (cellRangeAddress.FirstColumn <= 2 || cellRangeAddress.LastColumn <=2)
        //                //{
        //                    sheet.RemoveMergedRegion(i);
        //                //}

        //            }
        //        }
        //    }
        //}
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
