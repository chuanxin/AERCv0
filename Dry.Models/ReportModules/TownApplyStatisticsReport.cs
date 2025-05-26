using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Dry.Models.ViewModel;
using System.Data.Entity.Infrastructure;

namespace Dry.Models.ReportModules
{
    public class TownApplyStatisticsReport
    {
        private DryEntities DryDB = new DryEntities();
        private Dry.Models.CommonCls.GetData gd = new Dry.Models.CommonCls.GetData();
       
        /// <summary>
        /// 產生設施面積及補助統計表不含黃金廊道
        /// </summary>
        /// <param name="ayear">年</param>
        /// <param name="applyunit">單位</param>
        /// <param name="step">目前步驟</param>
        
        /// <returns>byte[]</returns>
        public Byte[] CreateReport(int ayear, int applyunit, int step)
        {
            
            var dataSource = from tb1 in DryDB.CaseDetail
                             join tb2 in DryDB.CasePayDetail on tb1.MapNo equals tb2.mapno into jb1
                             from tb3 in jb1.DefaultIfEmpty()
                             join tb4 in DryDB.PoolApply on tb1.MapNo equals tb4.MapNo into jb2
                             from tb5 in jb2.DefaultIfEmpty()
                             where tb1.ApplyUnit == applyunit && tb1.ApplyYear == ayear && /*tb1.Step >= step*/ tb1.Complete == true && tb1.Gold == false
                             
                             select new
                             {
                                 tb1.landCity,
                                 tb1.landTown,
                                 tb1.CatalogCNS,
                                 tb1.buildarea,
                                 tb3.FarmerFee,
                                 tb3.Total,
                                 tb3.田間管路設施費,
                                 tb3.規劃設計費,
                                 tb3.水源設施費,
                                 tb3.調控設施費,tb3.動力設備費,tb3.蓄水設備費,tb3.設施費總計,tb3.工作費,
                                 PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0,
                                 PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb1.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Sum(m => m.PoolWeight) : 0,
                                 EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0
                             };

         
           
            var datalist = (from tb1 in dataSource
                            
                            group tb1 by new { tb1.landTown, tb1.CatalogCNS } into tb4
                            select new
                            {
                                landTown = tb4.Key.landTown,
                                EndTypeCNS = tb4.Key.CatalogCNS,
                                buildarea = tb4.Sum(m => m.buildarea / 10000),
                                reccount = tb4.Count(),
                                farmerPay = tb4.Sum(m => m.FarmerFee),
                                田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                水源設施費 = tb4.Sum(m => m.水源設施費),
                                調控設施費 = tb4.Sum(m => m.調控設施費),
                                動力設備費 = tb4.Sum(m => m.動力設備費),
                                蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                設施費總計 = tb4.Sum(m => m.設施費總計),
                                工作費 = tb4.Sum(m => m.工作費),
                                PoolCount = tb4.Sum(m => m.PoolCount),
                                PoolWeight = tb4.Sum(m => m.PoolWeight),
                                EngCount = tb4.Sum(m => m.EngCount)
                            }).OrderBy(m => m.landTown).ThenBy(m => m.EndTypeCNS).ToList();
            
            var datalist1 = (from tb1 in dataSource
                             
                             group tb1 by new { tb1.CatalogCNS } into tb4
                             select new
                             {

                                 EndTypeCNS = tb4.Key.CatalogCNS,
                                 buildarea = tb4.Sum(m => m.buildarea / 10000),
                                 reccount = tb4.Count(),
                                 farmerPay = tb4.Sum(m => m.FarmerFee),
                                 田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                 規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                 水源設施費 = tb4.Sum(m => m.水源設施費),
                                 調控設施費 = tb4.Sum(m => m.調控設施費),
                                 動力設備費 = tb4.Sum(m => m.動力設備費),
                                 蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                 設施費總計 = tb4.Sum(m => m.設施費總計),
                                 工作費 = tb4.Sum(m => m.工作費),
                                 PoolCount = tb4.Sum(m => m.PoolCount),
                                 PoolWeight = tb4.Sum(m => m.PoolWeight),
                                 EngCount = tb4.Sum(m => m.EngCount)
                             }).OrderBy(m => m.EndTypeCNS).ToList();

            string title = gd.GetUnitName((short)applyunit) + ayear + "年度管路工程設施各鄉鎮及各類補助金額統計表";


           

            string sourcepath = @"~\ReportSample\TownApplyStatistics.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["統計表"];
            
            int irow = 1;
            sheet.Cells[irow,1].Value = title;
            irow = 3;
            if (datalist.Count > 0)
            {
                foreach (var item in datalist)
                {
                    irow++;


                    sheet.Cells[irow, 1].Value = item.landTown;
                    sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                    sheet.Cells[irow, 3].Value = item.buildarea;
                    sheet.Cells[irow, 4].Value = item.reccount;
                    sheet.Cells[irow, 5].Value = item.farmerPay;
                    sheet.Cells[irow, 6].Value = item.田間管路設施費;
                    sheet.Cells[irow, 7].Value = item.蓄水設備費;
                    sheet.Cells[irow, 8].Value = item.動力設備費;
                    sheet.Cells[irow, 9].Value = item.調控設施費;
                    sheet.Cells[irow, 10].Value = item.規劃設計費;
                    sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                    sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                    sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";//補助費
                    sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";//百分比
                    sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";//總工程費
                    sheet.Cells[irow, 16].Value = item.PoolWeight.ToString() + "/" + item.PoolCount.ToString();
                    sheet.Cells[irow, 17].Value = item.EngCount.ToString();


                }

                
                irow++;
                sheet.Cells[irow, 1].Value = "總計";
                sheet.Cells[irow, 2].Value = "小計";
                sheet.Cells[irow, 3].Formula = "+SUM(C4:C" + (irow - 1) + ")";
                sheet.Cells[irow, 4].Formula = "+SUM(D4:D" + (irow - 1) + ")";
                sheet.Cells[irow, 5].Formula = "+SUM(E4:E" + (irow - 1) + ")";
                sheet.Cells[irow, 6].Formula = "+SUM(F4:F" + (irow - 1) + ")";
                sheet.Cells[irow, 7].Formula = "+SUM(G4:G" + (irow - 1) + ")";
                sheet.Cells[irow, 8].Formula = "+SUM(H4:H" + (irow - 1) + ")";
                sheet.Cells[irow, 9].Formula = "+SUM(I4:I" + (irow - 1) + ")";
                sheet.Cells[irow, 10].Formula = "+SUM(J4:J" + (irow - 1) + ")";
                sheet.Cells[irow, 11].Formula = "+SUM(K4:K" + (irow - 1) + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(L4:L" + (irow - 1) + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";//補助費
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";//百分比
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";//總工程費
                sheet.Cells[irow, 16].Value = dataSource.Sum(o => o.PoolWeight).ToString()+"/"+dataSource.Sum(o => o.PoolCount).ToString();
                sheet.Cells[irow, 17].Value = dataSource.Sum(o => o.EngCount).ToString();

                foreach (var item in datalist1)
                {
                    irow++;
                    sheet.Cells[irow, 1].Value = "合計";
                    sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                    sheet.Cells[irow, 3].Value = item.buildarea;
                    sheet.Cells[irow, 4].Value = item.reccount;
                    sheet.Cells[irow, 5].Value = item.farmerPay;
                    sheet.Cells[irow, 6].Value = item.田間管路設施費;
                    sheet.Cells[irow, 7].Value = item.蓄水設備費;
                    sheet.Cells[irow, 8].Value = item.動力設備費;
                    sheet.Cells[irow, 9].Value = item.調控設施費;
                    sheet.Cells[irow, 10].Value = item.規劃設計費;
                    sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                    sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                    sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";//補助費
                    sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";//百分比
                    sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";//總工程費
                    sheet.Cells[irow, 16].Value = item.PoolWeight.ToString()+ "/"+ item.PoolCount.ToString();
                    sheet.Cells[irow, 17].Value = item.EngCount.ToString();
                }
            }
            




            return excel.GetAsByteArray();

        }

        /// <summary>
        /// 產設施面積及補助統計表(黃金廊道專用)
        /// </summary>
        /// <param name="ayear">年</param>
        /// <param name="applyunit">單位</param>
        /// <param name="step">目前步驟</param>
        /// <returns>byte[]</returns>
        public Byte[] CreateReportGoldOld(int ayear, int applyunit, int step)
        {
            string title = gd.GetUnitName((short)applyunit) + ayear + "年度管路工程設施各地區及各類補助金額統計表";
            //List<casepaydetail> CasePays = new List<casepaydetail>();
            


            List<CasePayDetailView> CasePays = getPays4Gold(ayear, applyunit/*, step*/);
            if (CasePays.Count() == 0) 
            {
                string sourcepath1 = @"~\ReportSample\TownApplyStatistics.xlsx";
                FileStream fs1 = new FileStream(HttpContext.Current.Server.MapPath(sourcepath1), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                ExcelPackage excelA = new ExcelPackage(fs1);
                
                ExcelWorksheet sheetA = excelA.Workbook.Worksheets["統計表"];
                sheetA.Cells[1, 1].Value = title;
                return excelA.GetAsByteArray();

            }
            #region 統計資料
           
            var CaseDetail = (from tb in DryDB.CaseDetail
                              where tb.ApplyUnit == applyunit && tb.ApplyYear == ayear && /*tb.Step >= step*/ tb.Complete == true && tb.Gold == true
                              select new { tb,
                                  PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Count() ?? 0,
                                  PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Sum(m => m.PoolWeight) : 0,
                                  EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb.MapNo).Count() ?? 0
                              }).ToList();

            var datalist = (from tb1 in CaseDetail
                            join tb2 in CasePays on tb1.tb.MapNo equals tb2.mapno into tb3
                            from tb2 in tb3.DefaultIfEmpty()
                            where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == true

                            group new { tb1, tb2 } by new { tb1.tb.landTown, tb1.tb.CatalogCNS } into tb4
                            select new
                            {
                                landTown = tb4.Key.landTown,
                                EndTypeCNS = tb4.Key.CatalogCNS,
                                buildarea = tb4.Sum(m => m.tb1.tb.buildarea / 10000),
                                reccount = tb4.Count(),
                                farmerPay = tb4.Sum(m => m.tb2.FarmerFee),
                                田間管路設施費 = tb4.Sum(m => m.tb2.田間管路設施費),
                                規劃設計費 = tb4.Sum(m => m.tb2.規劃設計費),
                                水源設施費 = tb4.Sum(m => m.tb2.水源設施費),
                                調控設施費 = tb4.Sum(m => m.tb2.調控設施費),
                                動力設備費 = tb4.Sum(m => m.tb2.動力設備費),
                                蓄水設備費 = tb4.Sum(m => m.tb2.蓄水設備費),
                                設施費總計 = tb4.Sum(m => m.tb2.設施費總計),
                                工作費 = tb4.Sum(m => m.tb2.工作費),
                                PoolCount = tb4.Sum(m => m.tb1.PoolCount),
                                PoolWeight = tb4.Sum(m => m.tb1.PoolWeight),
                                EngCount = tb4.Sum(m => m.tb1.EngCount)
                            }).OrderBy(m => m.landTown).ThenBy(m => m.EndTypeCNS).ToList();


            var datalist1 = (from tb1 in CaseDetail
                             join tb2 in CasePays on tb1.tb.MapNo equals tb2.mapno into tb3
                             from tb2 in tb3.DefaultIfEmpty()
                             where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == true
                             group new { tb1, tb2 } by new { tb1.tb.CatalogCNS } into tb4
                             select new
                             {

                                 EndTypeCNS = tb4.Key.CatalogCNS,
                                 buildarea = tb4.Sum(m => m.tb1.tb.buildarea / 10000),
                                 reccount = tb4.Count(),
                                 farmerPay = tb4.Sum(m => m.tb2.FarmerFee),
                                 田間管路設施費 = tb4.Sum(m => m.tb2.田間管路設施費),
                                 規劃設計費 = tb4.Sum(m => m.tb2.規劃設計費),
                                 水源設施費 = tb4.Sum(m => m.tb2.水源設施費),
                                 調控設施費 = tb4.Sum(m => m.tb2.調控設施費),
                                 動力設備費 = tb4.Sum(m => m.tb2.動力設備費),
                                 蓄水設備費 = tb4.Sum(m => m.tb2.蓄水設備費),
                                 設施費總計 = tb4.Sum(m => m.tb2.設施費總計),
                                 工作費 = tb4.Sum(m => m.tb2.工作費),
                                 PoolCount = tb4.Sum(m => m.tb1.PoolCount),
                                 PoolWeight = tb4.Sum(m => m.tb1.PoolWeight),
                                 EngCount = tb4.Sum(m => m.tb1.EngCount)
                             }).OrderBy(m => m.EndTypeCNS).ToList();

            #endregion

            #region 填寫報表




            
            #region 開檔
            string sourcepath = @"~\ReportSample\TownApplyStatistics.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["統計表"];
           
            #endregion
           
            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 3;
            foreach (var item in datalist)
            {
                irow++;
                sheet.Cells[irow, 1].Value = item.landTown;
                sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                sheet.Cells[irow, 3].Value = item.buildarea;
                sheet.Cells[irow, 4].Value = item.reccount;
                sheet.Cells[irow, 5].Value = item.farmerPay;
                sheet.Cells[irow, 6].Value = item.田間管路設施費;
                sheet.Cells[irow, 7].Value = item.蓄水設備費;
                sheet.Cells[irow, 8].Value = item.動力設備費;
                sheet.Cells[irow, 9].Value = item.調控設施費;
                sheet.Cells[irow, 10].Value = item.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";


            }

            
            irow++;
            sheet.Cells[irow, 1].Value = "總計";
            sheet.Cells[irow, 2].Value = "小計";
            sheet.Cells[irow, 3].Formula = "+SUM(C4:C" + (irow - 1) + ")";
            sheet.Cells[irow, 4].Formula = "+SUM(D4:D" + (irow - 1) + ")";
            sheet.Cells[irow, 5].Formula = "+SUM(E4:E" + (irow - 1) + ")";
            sheet.Cells[irow, 6].Formula = "+SUM(F4:F" + (irow - 1) + ")";
            sheet.Cells[irow, 7].Formula = "+SUM(G4:G" + (irow - 1) + ")";
            sheet.Cells[irow, 8].Formula = "+SUM(H4:H" + (irow - 1) + ")";
            sheet.Cells[irow, 9].Formula = "+SUM(I4:I" + (irow - 1) + ")";
            sheet.Cells[irow, 10].Formula = "+SUM(J4:J" + (irow - 1) + ")";
            sheet.Cells[irow, 11].Formula = "+SUM(K4:K" + (irow - 1) + ")";
            sheet.Cells[irow, 12].Formula = "+SUM(L4:L" + (irow - 1) + ")";
            sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
            sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
            sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";


            foreach (var item in datalist1)
            {
                irow++;
                sheet.Cells[irow, 1].Value = "合計";
                sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                sheet.Cells[irow, 3].Value = item.buildarea;
                sheet.Cells[irow, 4].Value = item.reccount;
                sheet.Cells[irow, 5].Value = item.farmerPay;
                sheet.Cells[irow, 6].Value = item.田間管路設施費;
                sheet.Cells[irow, 7].Value = item.蓄水設備費;
                sheet.Cells[irow, 8].Value = item.動力設備費;
                sheet.Cells[irow, 9].Value = item.調控設施費;
                sheet.Cells[irow, 10].Value = item.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
            }
            #endregion
       
            return excel.GetAsByteArray();

        }
        /// <summary>
        /// 產設施面積及補助統計表(含黃金廊道)
        /// </summary>
        /// <param name="ayear"></param>
        /// <param name="applyunit"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public Byte[] CreateReportGold(int ayear, int applyunit, int step)
        {
            string title = gd.GetUnitName((short)applyunit) + ayear + "年度管路工程設施各鄉鎮及各類補助金額統計表";
            
            //System.Diagnostics.Debug.WriteLine($"{DryDB.Database.Connection.ConnectionTimeout} ms");
            #region 統計資料
            
            var coaCaseDetail = (from tb1 in DryDB.CaseDetail
                              join tb2 in DryDB.CasePayDetail on tb1.MapNo equals tb2.mapno into jb1
                              from tb3 in jb1.DefaultIfEmpty()
                              join tb4 in DryDB.PoolApply on tb1.MapNo equals tb4.MapNo into jb2
                              from tb5 in jb2.DefaultIfEmpty()
                              where tb1.ApplyUnit == applyunit && tb1.ApplyYear == ayear && /*tb1.Step >= step*/ tb1.Complete == true && tb1.Gold == false
                              //&& tb5.ApplyUnit != 17 && tb5.ApplyUnit != 16
                              select new
                              {
                                  tb1.landCity,
                                  tb1.landTown,
                                  tb1.CatalogCNS,
                                  tb1.buildarea,
                                  tb3.FarmerFee,
                                  tb3.Total,
                                  tb3.田間管路設施費,
                                  tb3.規劃設計費,
                                  tb3.水源設施費,
                                  tb3.調控設施費,
                                  tb3.動力設備費,
                                  tb3.蓄水設備費,
                                  tb3.設施費總計,
                                  tb3.工作費,
                                  PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0,
                                  PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb1.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Sum(m => m.PoolWeight) : 0,
                                  EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0
                              }).ToList();

            
            #region 重新計算黃金廊道補助金額
            List<CasePayDetailView> CasePays = getPays4Gold(ayear, applyunit/*, step*/);
                     
            
            var goldcaselist = (from tb in DryDB.CaseDetail
                            where tb.ApplyUnit == applyunit && tb.ApplyYear == ayear && tb.Complete == true && tb.Gold == true
                            select tb).ToList();

            var GoldCaseDetail = (from tb in goldcaselist
                              join tb1 in CasePays on tb.MapNo equals tb1.mapno into jb1
                              from tb2 in jb1.DefaultIfEmpty()
                              
                              select new
                              {
                                  tb.landCity,
                                  tb.landTown,
                                  tb.CatalogCNS,
                                  tb.buildarea,
                                  tb2.FarmerFee,
                                  tb2.Total,
                                  tb2.田間管路設施費 ,
                                  tb2.規劃設計費 ,
                                  tb2.水源設施費 ,
                                  tb2.調控設施費,
                                  tb2.動力設備費,
                                  tb2.蓄水設備費,
                                  tb2.設施費總計,
                                  tb2.工作費,
                                  PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Count() ?? 0,
                                  PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Sum(m => m.PoolWeight) : 0,
                                  EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb.MapNo).Count() ?? 0
                              }).ToList();
            #endregion
            var AllCases = (from tb in coaCaseDetail select new {
                tb.landCity,
                tb.landTown,
                tb.CatalogCNS,
                tb.buildarea,
                tb.FarmerFee,
                tb.Total,
                tb.田間管路設施費,
                tb.規劃設計費,
                tb.水源設施費,
                tb.調控設施費,
                tb.動力設備費,
                tb.蓄水設備費,
                tb.設施費總計,
                tb.工作費,
                tb.PoolCount,
                tb.PoolWeight,
                tb.EngCount
            })
                .Concat
                (from tb in GoldCaseDetail select new {
                    tb.landCity,
                    tb.landTown,
                    tb.CatalogCNS,
                    tb.buildarea,
                    tb.FarmerFee,
                    tb.Total,
                    tb.田間管路設施費,
                    tb.規劃設計費,
                    tb.水源設施費,
                    tb.調控設施費,
                    tb.動力設備費,
                    tb.蓄水設備費,
                    tb.設施費總計,
                    tb.工作費,
                    tb.PoolCount,
                    tb.PoolWeight,tb.EngCount
                });

            if (AllCases.Count() == 0) 
            {
                string sourcepath1 = @"~\ReportSample\TownApplyStatistics.xlsx";
                FileStream fs1 = new FileStream(HttpContext.Current.Server.MapPath(sourcepath1), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                ExcelPackage excelA = new ExcelPackage(fs1);
                
                ExcelWorksheet sheetA = excelA.Workbook.Worksheets["統計表"];
                sheetA.Cells[1, 1].Value = title;
                return excelA.GetAsByteArray();

            }

            
            var datalist = (from tb1 in /*CaseDetail*/ AllCases
                                //join tb2 in CasePays on tb1.MapNo equals tb2.mapno into tb3
                                //from tb2 in tb3.DefaultIfEmpty()
                                //where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == gold

                            group tb1 by new { tb1.landTown, tb1.CatalogCNS } into tb4
                            select new
                            {
                                landTown = tb4.Key.landTown,
                                EndTypeCNS = tb4.Key.CatalogCNS,
                                buildarea = tb4.Sum(m => m.buildarea / 10000),
                                reccount = tb4.Count(),
                                farmerPay = tb4.Sum(m => m.FarmerFee),
                                田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                水源設施費 = tb4.Sum(m => m.水源設施費),
                                調控設施費 = tb4.Sum(m => m.調控設施費),
                                動力設備費 = tb4.Sum(m => m.動力設備費),
                                蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                設施費總計 = tb4.Sum(m => m.設施費總計),
                                工作費 = tb4.Sum(m => m.工作費),
                                PoolCount = tb4.Sum(m => m.PoolCount),
                                PoolWeight = tb4.Sum(m => m.PoolWeight),
                                EngCount = tb4.Sum(m => m.EngCount)
                            }).OrderBy(m => m.landTown).ThenBy(m => m.EndTypeCNS).ToList();

            
            var datalist1 = (from tb1 in /*CaseDetail*/ AllCases
                                 //join tb2 in CasePays on tb1.MapNo equals tb2.mapno into tb3
                                 //from tb2 in tb3.DefaultIfEmpty()
                                 //where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == gold
                             group tb1 by new { tb1.CatalogCNS } into tb4
                             select new
                             {

                                 EndTypeCNS = tb4.Key.CatalogCNS,
                                 buildarea = tb4.Sum(m => m.buildarea / 10000),
                                 reccount = tb4.Count(),
                                 farmerPay = tb4.Sum(m => m.FarmerFee),
                                 田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                 規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                 水源設施費 = tb4.Sum(m => m.水源設施費),
                                 調控設施費 = tb4.Sum(m => m.調控設施費),
                                 動力設備費 = tb4.Sum(m => m.動力設備費),
                                 蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                 設施費總計 = tb4.Sum(m => m.設施費總計),
                                 工作費 = tb4.Sum(m => m.工作費),
                                 PoolCount = tb4.Sum(m => m.PoolCount),
                                 PoolWeight = tb4.Sum(m => m.PoolWeight),
                                 EngCount = tb4.Sum(m => m.EngCount)
                             }).OrderBy(m => m.EndTypeCNS).ToList();

            #endregion

            #region 填寫報表




            
            #region 開檔
            string sourcepath = @"~\ReportSample\TownApplyStatistics.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["統計表"];

            #endregion
            
            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 3;
            foreach (var item in datalist)
            {
                irow++;
                sheet.Cells[irow, 1].Value = item.landTown;
                sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                sheet.Cells[irow, 3].Value = item.buildarea;
                sheet.Cells[irow, 4].Value = item.reccount;
                sheet.Cells[irow, 5].Value = item.farmerPay;
                sheet.Cells[irow, 6].Value = item.田間管路設施費;
                sheet.Cells[irow, 7].Value = item.蓄水設備費;
                sheet.Cells[irow, 8].Value = item.動力設備費;
                sheet.Cells[irow, 9].Value = item.調控設施費;
                sheet.Cells[irow, 10].Value = item.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
                sheet.Cells[irow, 16].Value = item.PoolWeight.ToString()+ "/" +item.PoolCount.ToString();
                sheet.Cells[irow, 17].Value = item.EngCount.ToString();

            }

            
            irow++;
            sheet.Cells[irow, 1].Value = "總計";
            sheet.Cells[irow, 2].Value = "小計";
            sheet.Cells[irow, 3].Formula = "+SUM(C4:C" + (irow - 1) + ")";
            sheet.Cells[irow, 4].Formula = "+SUM(D4:D" + (irow - 1) + ")";
            sheet.Cells[irow, 5].Formula = "+SUM(E4:E" + (irow - 1) + ")";
            sheet.Cells[irow, 6].Formula = "+SUM(F4:F" + (irow - 1) + ")";
            sheet.Cells[irow, 7].Formula = "+SUM(G4:G" + (irow - 1) + ")";
            sheet.Cells[irow, 8].Formula = "+SUM(H4:H" + (irow - 1) + ")";
            sheet.Cells[irow, 9].Formula = "+SUM(I4:I" + (irow - 1) + ")";
            sheet.Cells[irow, 10].Formula = "+SUM(J4:J" + (irow - 1) + ")";
            sheet.Cells[irow, 11].Formula = "+SUM(K4:K" + (irow - 1) + ")";
            sheet.Cells[irow, 12].Formula = "+SUM(L4:L" + (irow - 1) + ")";
            sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
            sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
            sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
            sheet.Cells[irow, 16].Value = datalist.Sum(o => o.PoolWeight).ToString()+ "/" +datalist.Sum(o => o.PoolCount).ToString();
            sheet.Cells[irow, 17].Value = datalist.Sum(o => o.EngCount).ToString();

            foreach (var item in datalist1)
            {
                irow++;
                sheet.Cells[irow, 1].Value = "合計";
                sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                sheet.Cells[irow, 3].Value = item.buildarea;
                sheet.Cells[irow, 4].Value = item.reccount;
                sheet.Cells[irow, 5].Value = item.farmerPay;
                sheet.Cells[irow, 6].Value = item.田間管路設施費;
                sheet.Cells[irow, 7].Value = item.蓄水設備費;
                sheet.Cells[irow, 8].Value = item.動力設備費;
                sheet.Cells[irow, 9].Value = item.調控設施費;
                sheet.Cells[irow, 10].Value = item.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
                sheet.Cells[irow, 16].Value = item.PoolWeight.ToString() + "/" + item.PoolCount.ToString();
                sheet.Cells[irow, 17].Value = item.EngCount.ToString();
            }
            #endregion

            return excel.GetAsByteArray();

        }

        /// <summary>
        /// 全國鄉鎮補助統計表
        /// </summary>
        /// <param name="ayear"></param>
        /// <param name="byear"></param>
        /// <param name="type">0為預算1為結案</param>
        /// <returns></returns>
        public Byte[] CreateReportAll(int ayear, int byear /*step, bool gold*/)
        {
            string title = "推廣單位" + ayear + "~" + byear + "年度管路工程設施各鄉鎮及各類補助金額統計表";
            
            #region 重新計算黃金廊道補助金額            
            
            List<CasePayDetailView> CasePays = getPays4GoldAll(ayear, byear, 1);
            #endregion
                        
            #region 統計資料
            //農委會                        
            var coaCaseDetail = (from tb1 in DryDB.CaseDetail
                                 join tb2 in DryDB.CasePayDetail on tb1.MapNo equals tb2.mapno into jb1
                                 from tb3 in jb1.DefaultIfEmpty()
                                 join tb4 in DryDB.PoolApply on tb1.MapNo equals tb4.MapNo into jb2
                                 from tb5 in jb2.DefaultIfEmpty()
                                 where tb1.ApplyYear >= ayear && tb1.ApplyYear <= byear && tb1.Complete == true && tb1.Gold == false                                 
                                 select new
                                 {
                                     tb1.landCity,
                                     tb1.landTown,
                                     tb1.CatalogCNS,
                                     tb1.buildarea,
                                     tb3.FarmerFee,
                                     tb3.Total,
                                     tb3.田間管路設施費,
                                     tb3.規劃設計費,
                                     tb3.水源設施費,
                                     tb3.調控設施費,
                                     tb3.動力設備費,
                                     tb3.蓄水設備費,
                                     tb3.設施費總計,
                                     tb3.工作費,
                                     PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0,
                                     PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb1.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Sum(m => m.PoolWeight) : 0,
                                     EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0
                                 }).ToList();
                                            
            //黃金廊道
           
            var goldcaselist = (from tb in DryDB.CaseDetail
                                where tb.ApplyYear >= ayear && tb.ApplyYear <= byear && tb.Complete == true && tb.Gold == true
                                select tb).ToList();
                                
            var GoldCaseDetail = (from tb in goldcaselist
                                  join tb1 in CasePays on tb.MapNo equals tb1.mapno into jb1
                                  from tb2 in jb1.DefaultIfEmpty()

                                  select new
                                  {
                                      tb.landCity,
                                      tb.landTown,
                                      tb.CatalogCNS,
                                      tb.buildarea,
                                      tb2.FarmerFee,
                                      tb2.Total,
                                      tb2.田間管路設施費,
                                      tb2.規劃設計費,
                                      tb2.水源設施費,
                                      tb2.調控設施費,
                                      tb2.動力設備費,
                                      tb2.蓄水設備費,
                                      tb2.設施費總計,
                                      tb2.工作費,
                                      PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Count() ?? 0,
                                      PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Sum(m => m.PoolWeight) : 0,
                                      EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb.MapNo).Count() ?? 0
                                  }).ToList();
            //統計資料整合
            
            var AllCases = (from tb in coaCaseDetail
                            select new
                            {
                                tb.landCity,
                                tb.landTown,
                                tb.CatalogCNS,
                                tb.buildarea,
                                tb.FarmerFee,
                                tb.Total,
                                tb.田間管路設施費,
                                tb.規劃設計費,
                                tb.水源設施費,
                                tb.調控設施費,
                                tb.動力設備費,
                                tb.蓄水設備費,
                                tb.設施費總計,
                                tb.工作費,
                                tb.PoolCount,
                                tb.PoolWeight,
                                tb.EngCount
                            })
                .Concat
                (from tb in GoldCaseDetail
                 select new
                 {
                     tb.landCity,
                     tb.landTown,
                     tb.CatalogCNS,
                     tb.buildarea,
                     tb.FarmerFee,
                     tb.Total,
                     tb.田間管路設施費,
                     tb.規劃設計費,
                     tb.水源設施費,
                     tb.調控設施費,
                     tb.動力設備費,
                     tb.蓄水設備費,
                     tb.設施費總計,
                     tb.工作費,
                     tb.PoolCount,
                     tb.PoolWeight,
                     tb.EngCount
                 });            
            
            if (AllCases.ToList().Count() == 0) 
            {
                string sourcepath1 = @"~\ReportSample\TownApplyStatistics.xlsx";
                FileStream fs1 = new FileStream(HttpContext.Current.Server.MapPath(sourcepath1), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                ExcelPackage excelA = new ExcelPackage(fs1);
                
                ExcelWorksheet sheetA = excelA.Workbook.Worksheets["統計表"];
                sheetA.Cells[1, 1].Value = title;
                return excelA.GetAsByteArray();

            }

            
            var datalist = (from tb1 in /*CaseDetail*/ AllCases
                                //join tb2 in CasePays on tb1.MapNo equals tb2.mapno into tb3
                                //from tb2 in tb3.DefaultIfEmpty()
                                //where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == gold

                            group tb1 by new {tb1.landCity, tb1.landTown, tb1.CatalogCNS } into tb4
                            select new
                            {
                                landTown = tb4.Key.landCity + tb4.Key.landTown,
                                EndTypeCNS = tb4.Key.CatalogCNS,
                                buildarea = tb4.Sum(m => m.buildarea / 10000),
                                reccount = tb4.Count(),
                                farmerPay = tb4.Sum(m => m.FarmerFee),
                                田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                水源設施費 = tb4.Sum(m => m.水源設施費),
                                調控設施費 = tb4.Sum(m => m.調控設施費),
                                動力設備費 = tb4.Sum(m => m.動力設備費),
                                蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                設施費總計 = tb4.Sum(m => m.設施費總計),
                                工作費 = tb4.Sum(m => m.工作費),
                                PoolCount = tb4.Sum(m => m.PoolCount),
                                PoolWeight = tb4.Sum(m => m.PoolWeight),
                                EngCount = tb4.Sum(m => m.EngCount)
                            }).OrderBy(m => m.landTown).ThenBy(m => m.EndTypeCNS).ToList();

            
            var datalist1 = (from tb1 in /*CaseDetail*/ AllCases
                                 //join tb2 in CasePays on tb1.MapNo equals tb2.mapno into tb3
                                 //from tb2 in tb3.DefaultIfEmpty()
                                 //where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == gold
                             group tb1 by new { tb1.CatalogCNS } into tb4
                             select new
                             {

                                 EndTypeCNS = tb4.Key.CatalogCNS,
                                 buildarea = tb4.Sum(m => m.buildarea / 10000),
                                 reccount = tb4.Count(),
                                 farmerPay = tb4.Sum(m => m.FarmerFee),
                                 田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                 規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                 水源設施費 = tb4.Sum(m => m.水源設施費),
                                 調控設施費 = tb4.Sum(m => m.調控設施費),
                                 動力設備費 = tb4.Sum(m => m.動力設備費),
                                 蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                 設施費總計 = tb4.Sum(m => m.設施費總計),
                                 工作費 = tb4.Sum(m => m.工作費),
                                 PoolCount = tb4.Sum(m => m.PoolCount),
                                 PoolWeight = tb4.Sum(m => m.PoolWeight),
                                 EngCount = tb4.Sum(m => m.EngCount)
                             }).OrderBy(m => m.EndTypeCNS).ToList();

            #endregion

            #region 填寫報表




            
            #region 開檔
            string sourcepath = @"~\ReportSample\TownApplyStatistics.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["統計表"];

            #endregion
            
            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 3;
            foreach (var item in datalist)
            {
                irow++;
                sheet.Cells[irow, 1].Value = item.landTown;
                sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                sheet.Cells[irow, 3].Value = item.buildarea;
                sheet.Cells[irow, 4].Value = item.reccount;
                sheet.Cells[irow, 5].Value = item.farmerPay;
                sheet.Cells[irow, 6].Value = item.田間管路設施費;
                sheet.Cells[irow, 7].Value = item.蓄水設備費;
                sheet.Cells[irow, 8].Value = item.動力設備費;
                sheet.Cells[irow, 9].Value = item.調控設施費;
                sheet.Cells[irow, 10].Value = item.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
                sheet.Cells[irow, 16].Value = item.PoolWeight.ToString() + "/" + item.PoolCount.ToString();
                sheet.Cells[irow, 17].Value = item.EngCount.ToString();

            }

            
            irow++;
            sheet.Cells[irow, 1].Value = "總計";
            sheet.Cells[irow, 2].Value = "小計";
            sheet.Cells[irow, 3].Formula = "+SUM(C4:C" + (irow - 1) + ")";
            sheet.Cells[irow, 4].Formula = "+SUM(D4:D" + (irow - 1) + ")";
            sheet.Cells[irow, 5].Formula = "+SUM(E4:E" + (irow - 1) + ")";
            sheet.Cells[irow, 6].Formula = "+SUM(F4:F" + (irow - 1) + ")";
            sheet.Cells[irow, 7].Formula = "+SUM(G4:G" + (irow - 1) + ")";
            sheet.Cells[irow, 8].Formula = "+SUM(H4:H" + (irow - 1) + ")";
            sheet.Cells[irow, 9].Formula = "+SUM(I4:I" + (irow - 1) + ")";
            sheet.Cells[irow, 10].Formula = "+SUM(J4:J" + (irow - 1) + ")";
            sheet.Cells[irow, 11].Formula = "+SUM(K4:K" + (irow - 1) + ")";
            sheet.Cells[irow, 12].Formula = "+SUM(L4:L" + (irow - 1) + ")";
            sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
            sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
            sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
            sheet.Cells[irow, 16].Value = AllCases.Sum(o => o.PoolWeight).ToString() + "/" + AllCases.Sum(o => o.PoolCount).ToString();
            sheet.Cells[irow, 17].Value = AllCases.Sum(o => o.EngCount).ToString();

            foreach (var item in datalist1)
            {
                irow++;
                sheet.Cells[irow, 1].Value = "合計";
                sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                sheet.Cells[irow, 3].Value = item.buildarea;
                sheet.Cells[irow, 4].Value = item.reccount;
                sheet.Cells[irow, 5].Value = item.farmerPay;
                sheet.Cells[irow, 6].Value = item.田間管路設施費;
                sheet.Cells[irow, 7].Value = item.蓄水設備費;
                sheet.Cells[irow, 8].Value = item.動力設備費;
                sheet.Cells[irow, 9].Value = item.調控設施費;
                sheet.Cells[irow, 10].Value = item.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
                sheet.Cells[irow, 16].Value = item.PoolWeight.ToString() + "/" + item.PoolCount.ToString();
                sheet.Cells[irow, 17].Value = item.EngCount.ToString();
            }
            #endregion

            return excel.GetAsByteArray();

        }


        public Byte[] CreateReportAllBudget(int ayear, int byear /*step, bool gold*/, int type)
        {
            string title = "推廣單位" + ayear + "~" + byear + "年度管路工程設施各鄉鎮及各類補助金額統計表";

            #region 重新計算黃金廊道補助金額

            List<CasePayDetailView> CasePays = getPays4GoldAll(ayear, byear, 0);
            #endregion

            #region 統計資料
                                    
            var coaCaseDetail = (from tb1 in DryDB.CaseDetail
                                 join tb2 in DryDB.CasePayDetail on tb1.MapNo equals tb2.mapno into jb1
                                 from tb3 in jb1.DefaultIfEmpty()
                                 join tb4 in DryDB.PoolApply on tb1.MapNo equals tb4.MapNo into jb2
                                 from tb5 in jb2.DefaultIfEmpty()
                                 where tb1.ApplyYear >= ayear && tb1.ApplyYear <= byear && tb1.Step >=7 && tb1.Gold == false
                                 select new
                                 {
                                     tb1.landCity,
                                     tb1.landTown,
                                     tb1.CatalogCNS,
                                     tb1.buildarea,
                                     tb1.farmarea,
                                     tb3.FarmerFee,
                                     tb3.Total,
                                     tb3.田間管路設施費,
                                     tb3.規劃設計費,
                                     tb3.水源設施費,
                                     tb3.調控設施費,
                                     tb3.動力設備費,
                                     tb3.蓄水設備費,
                                     tb3.設施費總計,
                                     tb3.工作費,
                                     PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0,
                                     PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb1.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Sum(m => m.PoolWeight) : 0,
                                     EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0
                                 }).ToList();

            

            var goldcaselist = (from tb in DryDB.CaseDetail
                                where tb.ApplyYear >= ayear && tb.ApplyYear <= byear && tb.Step >= 7 && tb.Gold == true
                                select tb).ToList();

            var GoldCaseDetail = (from tb in goldcaselist
                                  join tb1 in CasePays on tb.MapNo equals tb1.mapno into jb1
                                  from tb2 in jb1.DefaultIfEmpty()

                                  select new
                                  {
                                      tb.landCity,
                                      tb.landTown,
                                      tb.CatalogCNS,
                                      tb.buildarea,
                                      tb.farmarea,
                                      tb2.FarmerFee,
                                      tb2.Total,
                                      tb2.田間管路設施費,
                                      tb2.規劃設計費,
                                      tb2.水源設施費,
                                      tb2.調控設施費,
                                      tb2.動力設備費,
                                      tb2.蓄水設備費,
                                      tb2.設施費總計,
                                      tb2.工作費,
                                      PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Count() ?? 0,
                                      PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Sum(m => m.PoolWeight) : 0,
                                      EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb.MapNo).Count() ?? 0
                                  }).ToList();
            

            var AllCases = (from tb in coaCaseDetail
                            select new
                            {
                                tb.landCity,
                                tb.landTown,
                                tb.CatalogCNS,
                                tb.buildarea,
                                tb.farmarea,
                                tb.FarmerFee,
                                tb.Total,
                                tb.田間管路設施費,
                                tb.規劃設計費,
                                tb.水源設施費,
                                tb.調控設施費,
                                tb.動力設備費,
                                tb.蓄水設備費,
                                tb.設施費總計,
                                tb.工作費,
                                tb.PoolCount,
                                tb.PoolWeight,
                                tb.EngCount
                            })
                .Concat
                (from tb in GoldCaseDetail
                 select new
                 {
                     tb.landCity,
                     tb.landTown,
                     tb.CatalogCNS,
                     tb.buildarea,
                     tb.farmarea,
                     tb.FarmerFee,
                     tb.Total,
                     tb.田間管路設施費,
                     tb.規劃設計費,
                     tb.水源設施費,
                     tb.調控設施費,
                     tb.動力設備費,
                     tb.蓄水設備費,
                     tb.設施費總計,
                     tb.工作費,
                     tb.PoolCount,
                     tb.PoolWeight,
                     tb.EngCount
                 });

            if (AllCases.Count() == 0) 
            {
                string sourcepath1 = @"~\ReportSample\TownApplyStatisticsA.xlsx";
                FileStream fs1 = new FileStream(HttpContext.Current.Server.MapPath(sourcepath1), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                ExcelPackage excelA = new ExcelPackage(fs1);
                
                ExcelWorksheet sheetA = excelA.Workbook.Worksheets["統計表"];
                sheetA.Cells[1, 1].Value = title;
                return excelA.GetAsByteArray();

            }

            
            var datalist = (from tb1 in /*CaseDetail*/ AllCases
                                //join tb2 in CasePays on tb1.MapNo equals tb2.mapno into tb3
                                //from tb2 in tb3.DefaultIfEmpty()
                                //where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == gold

                            group tb1 by new { tb1.landCity, tb1.landTown, tb1.CatalogCNS } into tb4
                            select new
                            {
                                landCity = tb4.Key.landCity,
                                landTown = /*tb4.Key.landCity + */tb4.Key.landTown,
                                EndTypeCNS = tb4.Key.CatalogCNS,
                                buildarea = tb4.Sum(m => m.buildarea / 10000),
                                farmarea = tb4.Sum(m => m.farmarea / 10000),
                                reccount = tb4.Count(),
                                farmerPay = tb4.Sum(m => m.FarmerFee),
                                田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                水源設施費 = tb4.Sum(m => m.水源設施費),
                                調控設施費 = tb4.Sum(m => m.調控設施費),
                                動力設備費 = tb4.Sum(m => m.動力設備費),
                                蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                設施費總計 = tb4.Sum(m => m.設施費總計),
                                工作費 = tb4.Sum(m => m.工作費),
                                PoolCount = tb4.Sum(m => m.PoolCount),
                                PoolWeight = tb4.Sum(m => m.PoolWeight),
                                EngCount = tb4.Sum(m => m.EngCount)
                            }).OrderBy(m => m.landTown).ThenBy(m => m.EndTypeCNS).ToList();

            
            var datalist1 = (from tb1 in /*CaseDetail*/ AllCases
                                 //join tb2 in CasePays on tb1.MapNo equals tb2.mapno into tb3
                                 //from tb2 in tb3.DefaultIfEmpty()
                                 //where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == gold
                             group tb1 by new { tb1.CatalogCNS } into tb4
                             select new
                             {

                                 EndTypeCNS = tb4.Key.CatalogCNS,
                                 buildarea = tb4.Sum(m => m.buildarea / 10000),
                                 farmarea = tb4.Sum(m => m.farmarea / 10000),
                                 reccount = tb4.Count(),
                                 farmerPay = tb4.Sum(m => m.FarmerFee),
                                 田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                 規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                 水源設施費 = tb4.Sum(m => m.水源設施費),
                                 調控設施費 = tb4.Sum(m => m.調控設施費),
                                 動力設備費 = tb4.Sum(m => m.動力設備費),
                                 蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                 設施費總計 = tb4.Sum(m => m.設施費總計),
                                 工作費 = tb4.Sum(m => m.工作費),
                                 PoolCount = tb4.Sum(m => m.PoolCount),
                                 PoolWeight = tb4.Sum(m => m.PoolWeight),
                                 EngCount = tb4.Sum(m => m.EngCount)
                             }).OrderBy(m => m.EndTypeCNS).ToList();

            #endregion

            #region 填寫報表




            
            #region 開檔
            string sourcepath = @"~\ReportSample\TownApplyStatisticsA.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["統計表"];

            #endregion
            
            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 3;
            foreach (var item in datalist)
            {
                irow++;
                sheet.Cells[irow, 1].Value = item.landCity;
                sheet.Cells[irow, 2].Value = item.landTown;
                sheet.Cells[irow, 3].Value = item.EndTypeCNS;
                sheet.Cells[irow, 4].Value = item.buildarea;
                sheet.Cells[irow, 5].Value = item.reccount;
                sheet.Cells[irow, 6].Value = item.farmerPay;
                sheet.Cells[irow, 7].Value = item.田間管路設施費;
                sheet.Cells[irow, 8].Value = item.蓄水設備費;
                sheet.Cells[irow, 9].Value = item.動力設備費;
                sheet.Cells[irow, 10].Value = item.調控設施費;
                sheet.Cells[irow, 11].Value = item.規劃設計費;
                sheet.Cells[irow, 12].Formula = "+SUM(G" + irow + ":K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "+SUM(F" + irow + ",L" + irow + ")";
                //sheet.Cells[irow, 14].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                //sheet.Cells[irow, 15].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                //sheet.Cells[irow, 16].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
                sheet.Cells[irow, 14].Value = item.PoolWeight;
                sheet.Cells[irow, 15].Value = item.PoolCount;
                sheet.Cells[irow, 16].Value = item.EngCount;
                sheet.Cells[irow, 17].Value = item.farmarea;

            }

            
            irow++;
            sheet.Cells[irow, 1].Value = "總計";
            sheet.Cells[irow, 3].Value = "小計";
            sheet.Cells[irow, 4].Formula = "+SUM(D4:D" + (irow - 1) + ")";
            sheet.Cells[irow, 5].Formula = "+SUM(E4:E" + (irow - 1) + ")";
            sheet.Cells[irow, 6].Formula = "+SUM(F4:F" + (irow - 1) + ")";
            sheet.Cells[irow, 7].Formula = "+SUM(G4:G" + (irow - 1) + ")";
            sheet.Cells[irow, 8].Formula = "+SUM(H4:H" + (irow - 1) + ")";
            sheet.Cells[irow, 9].Formula = "+SUM(I4:I" + (irow - 1) + ")";
            sheet.Cells[irow, 10].Formula = "+SUM(J4:J" + (irow - 1) + ")";
            sheet.Cells[irow, 11].Formula = "+SUM(K4:K" + (irow - 1) + ")";
            sheet.Cells[irow, 12].Formula = "+SUM(L4:L" + (irow - 1) + ")";
            sheet.Cells[irow, 13].Formula = "+SUM(M4:M" + (irow - 1) + ")";            
            //sheet.Cells[irow, 13].Formula = "IF(D" + irow + ">0,G" + irow + "/D" + irow + ")";
            //sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
            //sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
            sheet.Cells[irow, 14].Value = AllCases.Sum(o => o.PoolWeight);
            sheet.Cells[irow, 15].Value = AllCases.Sum(o => o.PoolCount);
            sheet.Cells[irow, 16].Value = AllCases.Sum(o => o.EngCount);
            sheet.Cells[irow, 17].Formula = "+SUM(Q4:Q" + (irow - 1) + ")";

            foreach (var item in datalist1)
            {
                irow++;
                sheet.Cells[irow, 1].Value = "合計";
                sheet.Cells[irow, 3].Value = item.EndTypeCNS;
                sheet.Cells[irow, 4].Value = item.buildarea;
                sheet.Cells[irow, 5].Value = item.reccount;
                sheet.Cells[irow, 6].Value = item.farmerPay;
                sheet.Cells[irow, 7].Value = item.田間管路設施費;
                sheet.Cells[irow, 8].Value = item.蓄水設備費;
                sheet.Cells[irow, 9].Value = item.動力設備費;
                sheet.Cells[irow, 10].Value = item.調控設施費;
                sheet.Cells[irow, 11].Value = item.規劃設計費;
                sheet.Cells[irow, 12].Formula = "+SUM(G" + irow + ":K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "+SUM(F" + irow + ",L" + irow + ")";
                //sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                //sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                //sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
                sheet.Cells[irow, 14].Value = item.PoolWeight; 
                sheet.Cells[irow, 15].Value = item.PoolCount;
                sheet.Cells[irow, 16].Value = item.EngCount;
                sheet.Cells[irow, 17].Value = item.farmarea;
            }
            #endregion

            return excel.GetAsByteArray();

        }

        /// <summary>
        /// 全國縣市補助統計表
        /// </summary>
        /// <param name="ayear"></param>
        /// <param name="step"></param>
        /// <param name="gold"></param>
        /// <returns></returns>
        public Byte[] CreateReportAll4City(int ayear, /*int step, bool gold*/ int byear)
        {
            string title = "推廣單位" + ayear + "~" + byear + "年度管路工程設施各縣市及各類補助金額統計表";

            #region 重新計算黃金廊道補助金額
            List<CasePayDetailView> CasePays = getPays4GoldAll(ayear, byear, 1);


            #endregion
            //if (CasePays.Count() == 0) 
            //{
            //    string sourcepath1 = @"~\ReportSample\TownApplyStatistics.xlsx";
            //    FileStream fs1 = new FileStream(HttpContext.Current.Server.MapPath(sourcepath1), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            //    //載入Excel檔案
            //    ExcelPackage excelA = new ExcelPackage(fs1);
            //    //取得Sheet
            //    ExcelWorksheet sheetA = excelA.Workbook.Worksheets["統計表"];
            //    sheetA.Cells[1, 1].Value = title;
            //    return excelA.GetAsByteArray();

            //}
            #region 統計資料
            
            var coaCaseDetail = (from tb1 in DryDB.CaseDetail
                                 join tb2 in DryDB.CasePayDetail on tb1.MapNo equals tb2.mapno into jb1
                                 from tb3 in jb1.DefaultIfEmpty()
                                 join tb4 in DryDB.PoolApply on tb1.MapNo equals tb4.MapNo into jb2
                                 from tb5 in jb2.DefaultIfEmpty()
                                 where tb1.ApplyYear >= ayear && tb1.ApplyYear <= byear && tb1.Complete == true && tb1.Gold == false
                                 //&& tb5.ApplyUnit != 17 && tb5.ApplyUnit != 16
                                 select new
                                 {
                                     tb1.landCity,
                                     tb1.landTown,
                                     tb1.CatalogCNS,
                                     tb1.buildarea,
                                     tb3.FarmerFee,
                                     tb3.Total,
                                     tb3.田間管路設施費,
                                     tb3.規劃設計費,
                                     tb3.水源設施費,
                                     tb3.調控設施費,
                                     tb3.動力設備費,
                                     tb3.蓄水設備費,
                                     tb3.設施費總計,
                                     tb3.工作費,
                                     PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0,
                                     PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb1.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Sum(m => m.PoolWeight) : 0,
                                     EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0
                                 }).ToList();
            
            var goldcaselist = (from tb in DryDB.CaseDetail
                                where tb.ApplyYear >= ayear && tb.ApplyYear <= byear && tb.Complete == true && tb.Gold == true
                                select tb).ToList();

            var GoldCaseDetail = (from tb in goldcaselist
                                  join tb1 in CasePays on tb.MapNo equals tb1.mapno into jb1
                                  from tb2 in jb1.DefaultIfEmpty()

                                  select new
                                  {
                                      tb.landCity,
                                      tb.landTown,
                                      tb.CatalogCNS,
                                      tb.buildarea,
                                      tb2.FarmerFee,
                                      tb2.Total,
                                      tb2.田間管路設施費,
                                      tb2.規劃設計費,
                                      tb2.水源設施費,
                                      tb2.調控設施費,
                                      tb2.動力設備費,
                                      tb2.蓄水設備費,
                                      tb2.設施費總計,
                                      tb2.工作費,
                                      PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Count() ?? 0,
                                      PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Sum(m => m.PoolWeight) : 0,
                                      EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb.MapNo).Count() ?? 0
                                  }).ToList();

            var AllCases = (from tb in coaCaseDetail
                            select new
                            {
                                tb.landCity,
                                tb.landTown,
                                tb.CatalogCNS,
                                tb.buildarea,
                                tb.FarmerFee,
                                tb.Total,
                                tb.田間管路設施費,
                                tb.規劃設計費,
                                tb.水源設施費,
                                tb.調控設施費,
                                tb.動力設備費,
                                tb.蓄水設備費,
                                tb.設施費總計,
                                tb.工作費,
                                tb.PoolCount,
                                tb.PoolWeight,
                                tb.EngCount
                            })
                .Concat
                (from tb in GoldCaseDetail
                 select new
                 {
                     tb.landCity,
                     tb.landTown,
                     tb.CatalogCNS,
                     tb.buildarea,
                     tb.FarmerFee,
                     tb.Total,
                     tb.田間管路設施費,
                     tb.規劃設計費,
                     tb.水源設施費,
                     tb.調控設施費,
                     tb.動力設備費,
                     tb.蓄水設備費,
                     tb.設施費總計,
                     tb.工作費,
                     tb.PoolCount,
                     tb.PoolWeight,
                     tb.EngCount
                 });
            if (AllCases.Count() == 0) 
            {
                string sourcepath1 = @"~\ReportSample\TownApplyStatistics.xlsx";
                FileStream fs1 = new FileStream(HttpContext.Current.Server.MapPath(sourcepath1), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                
                ExcelPackage excelA = new ExcelPackage(fs1);
                
                ExcelWorksheet sheetA = excelA.Workbook.Worksheets["統計表"];
                sheetA.Cells[1, 1].Value = title;
                return excelA.GetAsByteArray();

            }

            
            var datalist = (from tb1 in /*CaseDetail*/ AllCases
                                //join tb2 in CasePays on tb1.MapNo equals tb2.mapno into tb3
                                //from tb2 in tb3.DefaultIfEmpty()
                                //where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == gold

                            group tb1 by new { tb1.landCity, tb1.CatalogCNS } into tb4
                            select new
                            {
                                landCity = tb4.Key.landCity,
                                EndTypeCNS = tb4.Key.CatalogCNS,
                                buildarea = tb4.Sum(m => m.buildarea / 10000),
                                reccount = tb4.Count(),
                                farmerPay = tb4.Sum(m => m.FarmerFee),
                                田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                水源設施費 = tb4.Sum(m => m.水源設施費),
                                調控設施費 = tb4.Sum(m => m.調控設施費),
                                動力設備費 = tb4.Sum(m => m.動力設備費),
                                蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                設施費總計 = tb4.Sum(m => m.設施費總計),
                                工作費 = tb4.Sum(m => m.工作費),
                                PoolCount = tb4.Sum(m => m.PoolCount),
                                PoolWeight = tb4.Sum(m => m.PoolWeight),
                                EngCount = tb4.Sum(m => m.EngCount)
                            }).OrderBy(m => m.landCity).ThenBy(m => m.EndTypeCNS).ToList();

            
            var datalist1 = (from tb1 in /*CaseDetail*/ AllCases
                                 //join tb2 in CasePays on tb1.MapNo equals tb2.mapno into tb3
                                 //from tb2 in tb3.DefaultIfEmpty()
                                 //where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == gold
                             group tb1 by new { tb1.CatalogCNS } into tb4
                             select new
                             {

                                 EndTypeCNS = tb4.Key.CatalogCNS,
                                 buildarea = tb4.Sum(m => m.buildarea / 10000),
                                 reccount = tb4.Count(),
                                 farmerPay = tb4.Sum(m => m.FarmerFee),
                                 田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                 規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                 水源設施費 = tb4.Sum(m => m.水源設施費),
                                 調控設施費 = tb4.Sum(m => m.調控設施費),
                                 動力設備費 = tb4.Sum(m => m.動力設備費),
                                 蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                 設施費總計 = tb4.Sum(m => m.設施費總計),
                                 工作費 = tb4.Sum(m => m.工作費),
                                 PoolCount = tb4.Sum(m => m.PoolCount),
                                 PoolWeight = tb4.Sum(m => m.PoolWeight),
                                 EngCount = tb4.Sum(m => m.EngCount)
                             }).OrderBy(m => m.EndTypeCNS).ToList();

            #endregion
            
            #region 填寫報表




            
            #region 開檔
            string sourcepath = @"~\ReportSample\TownApplyStatistics.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["統計表"];

            #endregion
            
            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 3;
            foreach (var item in datalist)
            {
                irow++;
                sheet.Cells[irow, 1].Value = item.landCity;
                sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                sheet.Cells[irow, 3].Value = item.buildarea;
                sheet.Cells[irow, 4].Value = item.reccount;
                sheet.Cells[irow, 5].Value = item.farmerPay;
                sheet.Cells[irow, 6].Value = item.田間管路設施費;
                sheet.Cells[irow, 7].Value = item.蓄水設備費;
                sheet.Cells[irow, 8].Value = item.動力設備費;
                sheet.Cells[irow, 9].Value = item.調控設施費;
                sheet.Cells[irow, 10].Value = item.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";


            }

            
            irow++;
            sheet.Cells[irow, 1].Value = "總計";
            sheet.Cells[irow, 2].Value = "小計";
            sheet.Cells[irow, 3].Formula = "+SUM(C4:C" + (irow - 1) + ")";
            sheet.Cells[irow, 4].Formula = "+SUM(D4:D" + (irow - 1) + ")";
            sheet.Cells[irow, 5].Formula = "+SUM(E4:E" + (irow - 1) + ")";
            sheet.Cells[irow, 6].Formula = "+SUM(F4:F" + (irow - 1) + ")";
            sheet.Cells[irow, 7].Formula = "+SUM(G4:G" + (irow - 1) + ")";
            sheet.Cells[irow, 8].Formula = "+SUM(H4:H" + (irow - 1) + ")";
            sheet.Cells[irow, 9].Formula = "+SUM(I4:I" + (irow - 1) + ")";
            sheet.Cells[irow, 10].Formula = "+SUM(J4:J" + (irow - 1) + ")";
            sheet.Cells[irow, 11].Formula = "+SUM(K4:K" + (irow - 1) + ")";
            sheet.Cells[irow, 12].Formula = "+SUM(L4:L" + (irow - 1) + ")";
            sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
            sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
            sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";


            foreach (var item in datalist1)
            {
                irow++;
                sheet.Cells[irow, 1].Value = "合計";
                sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                sheet.Cells[irow, 3].Value = item.buildarea;
                sheet.Cells[irow, 4].Value = item.reccount;
                sheet.Cells[irow, 5].Value = item.farmerPay;
                sheet.Cells[irow, 6].Value = item.田間管路設施費;
                sheet.Cells[irow, 7].Value = item.蓄水設備費;
                sheet.Cells[irow, 8].Value = item.動力設備費;
                sheet.Cells[irow, 9].Value = item.調控設施費;
                sheet.Cells[irow, 10].Value = item.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
            }
            #endregion

            return excel.GetAsByteArray();

        }
        /// <summary>
        /// 全國各單位補助統計表
        /// </summary>
        /// <param name="ayear"></param>
        /// <param name="byear"></param>
        /// <returns></returns>
        public Byte[] CreateReportAll4IA(int ayear, int byear)
        {
            string title = "推廣單位" + ayear + "~" + byear + "年度管路工程設施各單位各類補助金額統計表";

            #region 重新計算黃金廊道補助金額
            List<CasePayDetailView> CasePays = getPays4GoldAll(ayear, byear, 1);


            #endregion
            /*
            if (CasePays.Count() == 0) //沒有pay金額資料
            {
                string sourcepath1 = @"~\ReportSample\TownApplyStatistics.xlsx";
                FileStream fs1 = new FileStream(HttpContext.Current.Server.MapPath(sourcepath1), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                //載入Excel檔案
                ExcelPackage excelA = new ExcelPackage(fs1);
                //取得Sheet
                ExcelWorksheet sheetA = excelA.Workbook.Worksheets["統計表"];
                sheetA.Cells[1, 1].Value = title;
                return excelA.GetAsByteArray();

            }
            */
            #region 統計資料
            
            var coaCaseDetail = (from tb1 in DryDB.CaseDetail
                                 join tb2 in DryDB.CasePayDetail on tb1.MapNo equals tb2.mapno into jb1
                                 from tb3 in jb1.DefaultIfEmpty()
                                 join tb4 in DryDB.PoolApply on tb1.MapNo equals tb4.MapNo into jb2
                                 from tb5 in jb2.DefaultIfEmpty()
                                 where tb1.ApplyYear >= ayear && tb1.ApplyYear <= byear && tb1.Complete == true && tb1.Gold == false
                                 //&& tb5.ApplyUnit != 17 && tb5.ApplyUnit != 16
                                 select new
                                 {
                                     tb1.ApplyUnit,
                                     tb1.landCity,
                                     tb1.landTown,
                                     tb1.CatalogCNS,
                                     tb1.buildarea,
                                     tb3.FarmerFee,
                                     tb3.Total,
                                     tb3.田間管路設施費,
                                     tb3.規劃設計費,
                                     tb3.水源設施費,
                                     tb3.調控設施費,
                                     tb3.動力設備費,
                                     tb3.蓄水設備費,
                                     tb3.設施費總計,
                                     tb3.工作費,
                                     PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0,
                                     PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb1.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb1.MapNo).Sum(m => m.PoolWeight) : 0,
                                     EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb1.MapNo).Count() ?? 0
                                 }).ToList();
            
            var goldcaselist = (from tb in DryDB.CaseDetail
                                where tb.ApplyYear >= ayear && tb.ApplyYear <= byear && tb.Complete == true && tb.Gold == true
                                select tb).ToList();

            var GoldCaseDetail = (from tb in goldcaselist
                                  join tb1 in CasePays on tb.MapNo equals tb1.mapno into jb1
                                  from tb2 in jb1.DefaultIfEmpty()

                                  select new
                                  {
                                      tb.ApplyUnit,
                                      tb.landCity,
                                      tb.landTown,
                                      tb.CatalogCNS,
                                      tb.buildarea,
                                      tb2.FarmerFee,
                                      tb2.Total,
                                      tb2.田間管路設施費,
                                      tb2.規劃設計費,
                                      tb2.水源設施費,
                                      tb2.調控設施費,
                                      tb2.動力設備費,
                                      tb2.蓄水設備費,
                                      tb2.設施費總計,
                                      tb2.工作費,
                                      PoolCount = (int?)DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Count() ?? 0,
                                      PoolWeight = DryDB.Pool.Any(m => m.MapNo == tb.MapNo) ? DryDB.Pool.Where(m => m.MapNo == tb.MapNo).Sum(m => m.PoolWeight) : 0,
                                      EngCount = (int?)DryDB.Engine.Where(m => m.MapNo == tb.MapNo).Count() ?? 0
                                  }).ToList();

            var AllCases = (from tb in coaCaseDetail
                            select new
                            {
                                tb.ApplyUnit,
                                tb.landCity,
                                tb.landTown,
                                tb.CatalogCNS,
                                tb.buildarea,
                                tb.FarmerFee,
                                tb.Total,
                                tb.田間管路設施費,
                                tb.規劃設計費,
                                tb.水源設施費,
                                tb.調控設施費,
                                tb.動力設備費,
                                tb.蓄水設備費,
                                tb.設施費總計,
                                tb.工作費,
                                tb.PoolCount,
                                tb.PoolWeight,
                                tb.EngCount
                            })
                .Concat
                (from tb in GoldCaseDetail
                 select new
                 {
                     tb.ApplyUnit,
                     tb.landCity,
                     tb.landTown,
                     tb.CatalogCNS,
                     tb.buildarea,
                     tb.FarmerFee,
                     tb.Total,
                     tb.田間管路設施費,
                     tb.規劃設計費,
                     tb.水源設施費,
                     tb.調控設施費,
                     tb.動力設備費,
                     tb.蓄水設備費,
                     tb.設施費總計,
                     tb.工作費,
                     tb.PoolCount,
                     tb.PoolWeight,
                     tb.EngCount
                 });
            
            var datalist = (from tb1 in /*CaseDetail*/ AllCases
                                //join tb2 in CasePays on tb1.MapNo equals tb2.mapno into tb3
                                //from tb2 in tb3.DefaultIfEmpty()
                                //where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == gold

                            group tb1 by new { /*tb1.landCity*/tb1.ApplyUnit, tb1.CatalogCNS } into tb4
                            select new
                            {
                                landCity = tb4.Key.ApplyUnit,
                                EndTypeCNS = tb4.Key.CatalogCNS,
                                buildarea = tb4.Sum(m => m.buildarea / 10000),
                                reccount = tb4.Count(),
                                farmerPay = tb4.Sum(m => m.FarmerFee),
                                田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                水源設施費 = tb4.Sum(m => m.水源設施費),
                                調控設施費 = tb4.Sum(m => m.調控設施費),
                                動力設備費 = tb4.Sum(m => m.動力設備費),
                                蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                設施費總計 = tb4.Sum(m => m.設施費總計),
                                工作費 = tb4.Sum(m => m.工作費),
                                PoolCount = tb4.Sum(m => m.PoolCount),
                                PoolWeight = tb4.Sum(m => m.PoolWeight),
                                EngCount = tb4.Sum(m => m.EngCount)
                            }).OrderBy(m => m.landCity).ThenBy(m => m.EndTypeCNS).ToList();

            
            var datalist1 = (from tb1 in /*CaseDetail*/ AllCases
                                 //join tb2 in CasePays on tb1.MapNo equals tb2.mapno into tb3
                                 //from tb2 in tb3.DefaultIfEmpty()
                                 //where tb1.tb.ApplyUnit == applyunit && tb1.tb.ApplyYear == ayear && /*tb1.tb.Step >= step*/ tb1.tb.Complete == true && tb1.tb.Gold == gold
                             group tb1 by new { tb1.CatalogCNS } into tb4
                             select new
                             {

                                 EndTypeCNS = tb4.Key.CatalogCNS,
                                 buildarea = tb4.Sum(m => m.buildarea / 10000),
                                 reccount = tb4.Count(),
                                 farmerPay = tb4.Sum(m => m.FarmerFee),
                                 田間管路設施費 = tb4.Sum(m => m.田間管路設施費),
                                 規劃設計費 = tb4.Sum(m => m.規劃設計費),
                                 水源設施費 = tb4.Sum(m => m.水源設施費),
                                 調控設施費 = tb4.Sum(m => m.調控設施費),
                                 動力設備費 = tb4.Sum(m => m.動力設備費),
                                 蓄水設備費 = tb4.Sum(m => m.蓄水設備費),
                                 設施費總計 = tb4.Sum(m => m.設施費總計),
                                 工作費 = tb4.Sum(m => m.工作費),
                                 PoolCount = tb4.Sum(m => m.PoolCount),
                                 PoolWeight = tb4.Sum(m => m.PoolWeight),
                                 EngCount = tb4.Sum(m => m.EngCount)
                             }).OrderBy(m => m.EndTypeCNS).ToList();

            #endregion

            #region 填寫報表




            
            #region 開檔
            string sourcepath = @"~\ReportSample\TownApplyStatistics.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            ExcelPackage excel = new ExcelPackage(fs);
            
            ExcelWorksheet sheet = excel.Workbook.Worksheets["統計表"];

            #endregion
            
            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 3;
            AERC.Models.CommonCls.GetUints qry = new AERC.Models.CommonCls.GetUints();
            foreach (var item in datalist)
            {
                irow++;
                sheet.Cells[irow, 1].Value = qry.GetUnitData(item.landCity).Unit1; ;
                sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                sheet.Cells[irow, 3].Value = item.buildarea;
                sheet.Cells[irow, 4].Value = item.reccount;
                sheet.Cells[irow, 5].Value = item.farmerPay;
                sheet.Cells[irow, 6].Value = item.田間管路設施費;
                sheet.Cells[irow, 7].Value = item.蓄水設備費;
                sheet.Cells[irow, 8].Value = item.動力設備費;
                sheet.Cells[irow, 9].Value = item.調控設施費;
                sheet.Cells[irow, 10].Value = item.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
                sheet.Cells[irow, 16].Value = item.PoolWeight.ToString() + "/" + item.PoolCount.ToString();
                sheet.Cells[irow, 17].Value = item.EngCount.ToString();

            }

            
            irow++;
            sheet.Cells[irow, 1].Value = "總計";
            sheet.Cells[irow, 2].Value = "小計";
            sheet.Cells[irow, 3].Formula = "+SUM(C4:C" + (irow - 1) + ")";
            sheet.Cells[irow, 4].Formula = "+SUM(D4:D" + (irow - 1) + ")";
            sheet.Cells[irow, 5].Formula = "+SUM(E4:E" + (irow - 1) + ")";
            sheet.Cells[irow, 6].Formula = "+SUM(F4:F" + (irow - 1) + ")";
            sheet.Cells[irow, 7].Formula = "+SUM(G4:G" + (irow - 1) + ")";
            sheet.Cells[irow, 8].Formula = "+SUM(H4:H" + (irow - 1) + ")";
            sheet.Cells[irow, 9].Formula = "+SUM(I4:I" + (irow - 1) + ")";
            sheet.Cells[irow, 10].Formula = "+SUM(J4:J" + (irow - 1) + ")";
            sheet.Cells[irow, 11].Formula = "+SUM(K4:K" + (irow - 1) + ")";
            sheet.Cells[irow, 12].Formula = "+SUM(L4:L" + (irow - 1) + ")";
            sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
            sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
            sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
            sheet.Cells[irow, 16].Value = datalist.Sum(o => o.PoolWeight).ToString() + "/" + datalist.Sum(o => o.PoolCount).ToString();
            sheet.Cells[irow, 17].Value = datalist.Sum(o => o.EngCount).ToString();

            foreach (var item in datalist1)
            {
                irow++;
                sheet.Cells[irow, 1].Value = "合計";
                sheet.Cells[irow, 2].Value = item.EndTypeCNS;
                sheet.Cells[irow, 3].Value = item.buildarea;
                sheet.Cells[irow, 4].Value = item.reccount;
                sheet.Cells[irow, 5].Value = item.farmerPay;
                sheet.Cells[irow, 6].Value = item.田間管路設施費;
                sheet.Cells[irow, 7].Value = item.蓄水設備費;
                sheet.Cells[irow, 8].Value = item.動力設備費;
                sheet.Cells[irow, 9].Value = item.調控設施費;
                sheet.Cells[irow, 10].Value = item.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
                sheet.Cells[irow, 13].Formula = "IF(C" + irow + ">0,F" + irow + "/C" + irow + ")";
                sheet.Cells[irow, 14].Formula = "IF(O" + irow + ">0,M" + irow + "/O" + irow + ",)";
                sheet.Cells[irow, 15].Formula = "IF(C" + irow + ">0,SUM(E" + irow + ",F" + irow + ")/C" + irow + ")";
                sheet.Cells[irow, 16].Value = item.PoolWeight.ToString() + "/" + item.PoolCount.ToString();
                sheet.Cells[irow, 17].Value = item.EngCount.ToString();
            }
            #endregion

            return excel.GetAsByteArray();

        }

        /// <summary>
        /// 重新計算黃金廊道補助金額
        /// </summary>
        /// <param name="ayear"></param>
        /// <param name="applyunit"></param>
        /// <param name="step"></param>
        /// <param name="gold"></param>
        /// <returns></returns>
        public List<CasePayDetailView> getPays4Gold(int ayear, int applyunit/*, int step*/)
        {
            #region 重新計算黃金廊道補助金額

            //List<casepaydetail> CasePays = new List<casepaydetail>();
            List<CasePayDetailView> CasePays = new List<CasePayDetailView>();
            var datalist2 = (from tb2 in DryDB.SummaryView
                             where tb2.ApplyUnit == applyunit && tb2.ApplyYear == ayear && /*tb2.Step >= step*/ tb2.Complete == true && tb2.Gold == true
                             select new { tb2.MapNo }
                                 ).ToList();
            foreach (var data in datalist2)
            {
                //var datalist3 = from tb1 in DryDB.Pay
                //                where tb1.MapNo == data.MapNo
                //                select new { tb1 };
                var datalist3 = DryDB.Pay.Where(m => m.MapNo == data.MapNo);
                if (datalist3.Count() > 0 )
                {
                    CasePayDetailView casepay = new CasePayDetailView();
                    casepay.mapno = (int)data.MapNo;
                    casepay.FarmerFee = 0;
                    casepay.Total = 0;
                    
                    if (ayear>108)
                    {
                        CasePays.Add(CalcGoldCasePayDetail2020(casepay, datalist3.ToList()));
                    }
                    else
                    {
                        CasePays.Add(CalcGoldCasePayDetail(casepay,datalist3.ToList()));
                    }
                    //CasePays.Add(casepay);
                    
                }
            }
            #endregion
            return CasePays;
        }
        /// <summary>
        /// 計算2019前黃金廊道金額
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        private CasePayDetailView CalcGoldCasePayDetail(CasePayDetailView casepay, List<Pay> datas)
        {
            //CasePayDetailView casepay = new CasePayDetailView();
            foreach (var data in datas)
            {
                switch (data.ItemCode)
                {
                    case 1:
                        casepay.田間管路設施費 = (int)Math.Round((double)(data.Total) * 0.7, 0, MidpointRounding.AwayFromZero);
                        casepay.FarmerFee += (data.Total.GetValueOrDefault() - casepay.田間管路設施費);
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 2:
                        casepay.規劃設計費 = (int)data.Total;
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 3:
                        casepay.水源設施費 = (int)data.Total;
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 4:
                        casepay.調控設施費 = (int)Math.Round((double)(data.Total) * 0.7, 0, MidpointRounding.AwayFromZero);
                        casepay.FarmerFee += (data.Total.GetValueOrDefault() - casepay.調控設施費);
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 5:
                        casepay.動力設備費 = (int)data.Total;
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 6:
                        casepay.蓄水設備費 = (int)(data.Total);
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 7:
                        casepay.設施費總計 = (int)Math.Round((double)(data.Total) * 0.7, 0, MidpointRounding.AwayFromZero);
                        casepay.FarmerFee += (data.Total.GetValueOrDefault() - casepay.設施費總計);
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 8:
                        casepay.工作費 = (int)Math.Round((double)(data.Total) * 0.7, 0, MidpointRounding.AwayFromZero);
                        casepay.FarmerFee += (data.Total.GetValueOrDefault() - casepay.工作費);
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                }
            }
            return casepay;
        }
        /// <summary>
        /// 計算2020後黃金廊道金額
        /// </summary>
        /// <param name="casepay"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        private CasePayDetailView CalcGoldCasePayDetail2020(CasePayDetailView casepay, List<Pay> datas)
        {
            foreach (var data in datas)
            {
                switch (data.ItemCode)
                {
                    case 1:
                        casepay.田間管路設施費 = (data.PayMoney);
                        casepay.FarmerFee += (data.FarmerMoney);
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 2:
                        casepay.規劃設計費 = (int)data.Total;
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 3:
                        casepay.水源設施費 = (int)data.Total;
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 4:
                        casepay.調控設施費 = (data.PayMoney);
                        casepay.FarmerFee += (data.FarmerMoney);
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 5:
                        casepay.動力設備費 = (int)data.Total;
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 6:
                        casepay.蓄水設備費 = (int)(data.Total);
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 7:
                        casepay.設施費總計 = (data.PayMoney);
                        casepay.FarmerFee += (data.FarmerMoney);
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                    case 8:
                        casepay.工作費 = (data.PayMoney);
                        casepay.FarmerFee += (data.FarmerMoney);
                        casepay.Total += data.Total.GetValueOrDefault();
                        break;
                }
            }
            return casepay;
        }
        /// <summary>
        /// 依據黃金廊道資料年期重新計算各項補助金額
        /// </summary>
        /// <param name="yearstart"></param>
        /// <param name="yearend"></param>
        /// <returns></returns>
        public List<CasePayDetailView> getPays4GoldAll(int yearstart, int yearend, int type)
        {
            #region 重新計算黃金廊道補助金額

            List<CasePayDetailView> CasePays = new List<CasePayDetailView>();

            var datalist2 = (from tb2 in DryDB.SummaryView
                             where tb2.ApplyYear >= yearstart &&  tb2.ApplyYear <= yearend && tb2.Step >= 7 && tb2.Gold == true
                             select new { tb2.MapNo,tb2.ApplyYear, tb2.Complete }
                                 )/*.ToList()*/;
            if (type == 1)
            {
                datalist2 = datalist2.Where(m => m.Complete == true);
            }

            foreach (var data in datalist2)
            {
                //var datalist3 = (from tb1 in DryDB.Pay
                //                 where tb1.MapNo == data.MapNo
                //                 select new { tb1 }
                //                     ).ToList();
                var datalist3 = DryDB.Pay.Where(m => m.MapNo == data.MapNo);

                if (datalist3.Count() > 0)
                {
                    CasePayDetailView casepay = new CasePayDetailView();
                    casepay.mapno = (int)data.MapNo;
                    casepay.FarmerFee = 0;
                    casepay.Total = 0;
                    
                    if (data.ApplyYear > 108)
                    {
                        CasePays.Add(CalcGoldCasePayDetail2020(casepay, datalist3.ToList()));
                    }
                    else
                    {
                        CasePays.Add(CalcGoldCasePayDetail(casepay, datalist3.ToList()));
                    }

                    //CasePays.Add(casepay);

                }
            }
            #endregion
            return CasePays;
        }

        /// <summary>
        /// 單筆案件補助金額資料格式
        /// </summary>
        //    private class casepaydetail
        //    {
        //        public int mapno { get; set; }
        //        public int? 田間管路設施費 { get; set; }
        //        public int? 規劃設計費 { get; set; }
        //        public int? 水源設施費 { get; set; }
        //        public int? 調控設施費 { get; set; }
        //        public int? 動力設備費 { get; set; }
        //        public int? 蓄水設備費 { get; set; }
        //        public int? 設施費總計 { get; set; }
        //        public int? 工作費 { get; set; }
        //        public int? FarmerFee { get; set; }
        //        public int? Total { get; set; }
        //    }
    }

    
}
