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

namespace Dry.Models.ReportModules
{
    public class ClosingAccountReport
    {
        private DryEntities DryDB = new DryEntities();
        private Dry.Models.CommonCls.GetData gd = new Dry.Models.CommonCls.GetData();
        /// <summary>
        /// 產生工程決算表
        /// </summary>
        /// <param name="ayear">年期</param>
        /// <param name="applyunit">會別</param>
        /// <param name="step">案件步驟</param>
        /// <returns>Byte[] EXCEL檔案</returns>
        public byte[] CreateReport(int ayear, int applyunit, int step)
        {
            var datalist = (from tb1 in DryDB.CaseDetail
                           join tb2 in DryDB.CasePayDetail on tb1.MapNo equals tb2.mapno into tb3
                           from tb2 in tb3.DefaultIfEmpty()
                           where tb1.ApplyUnit == applyunit && tb1.ApplyYear == ayear && /*tb1.Step >= step*/ tb1.Complete == true && tb1.Gold == false
                           group new { tb1,tb2 } by new { tb1.landTown, tb1.CatalogCNS } into tb4
                           select new
                           {
                               landTown = tb4.Key.landTown,
                               EndTypeCNS = tb4.Key.CatalogCNS,
                               buildarea = tb4.Sum(m => m.tb1.buildarea / 10000),
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
                           }).OrderBy( m => m.landTown).ThenBy(m => m.EndTypeCNS).ToList();


            var datalist1 = (from tb1 in DryDB.CaseDetail
                             join tb2 in DryDB.CasePayDetail on tb1.MapNo equals tb2.mapno into tb3
                             from tb2 in tb3.DefaultIfEmpty()
                             where tb1.ApplyUnit == applyunit && tb1.ApplyYear == ayear && /*tb1.Step >= step*/ tb1.Complete == true && tb1.Gold == false
                             group new { tb1, tb2 } by new { tb1.CatalogCNS } into tb4
                             select new
                             {

                                 EndTypeCNS = tb4.Key.CatalogCNS,
                                 buildarea = tb4.Sum(m => m.tb1.buildarea / 10000),
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
                             }).OrderBy(m => m.EndTypeCNS).ToList();

            string title = gd.GetUnitName((short)applyunit) + ayear + "年農田水利設施更新改善計畫(全部)";


            
            string sourcepath = @"~\ReportSample\ClosingAccount.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["工程決算書"];
            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 4;
            foreach (var data in datalist)
            {
                irow += 1;
                sheet.Cells[irow, 1].Value = data.landTown;
                sheet.Cells[irow, 2].Value = data.EndTypeCNS;
                sheet.Cells[irow, 3].Value = data.buildarea;
                sheet.Cells[irow, 4].Value = data.reccount;
                sheet.Cells[irow, 5].Value = data.farmerPay;
                sheet.Cells[irow, 6].Value = data.田間管路設施費;
                sheet.Cells[irow, 7].Value = data.蓄水設備費;
                sheet.Cells[irow, 8].Value = data.動力設備費;
                sheet.Cells[irow, 9].Value = data.調控設施費;
                sheet.Cells[irow, 10].Value = data.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";

            }
            foreach (var data in datalist1)
            {
                irow += 1;
                sheet.Cells[irow, 1].Value = "合計";
                sheet.Cells[irow, 2].Value = data.EndTypeCNS;
                sheet.Cells[irow, 3].Value = data.buildarea;
                sheet.Cells[irow, 4].Value = data.reccount;
                sheet.Cells[irow, 5].Value = data.farmerPay;
                sheet.Cells[irow, 6].Value = data.田間管路設施費;
                sheet.Cells[irow, 7].Value = data.蓄水設備費;
                sheet.Cells[irow, 8].Value = data.動力設備費;
                sheet.Cells[irow, 9].Value = data.調控設施費;
                sheet.Cells[irow, 10].Value = data.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
            }

            return excel.GetAsByteArray();

        }
        /// <summary>
        /// 產生含黃金廊道工程決算表
        /// </summary>
        /// <param name="ayear">年期</param>
        /// <param name="applyunit">會別</param>
        /// <param name="step">案件目前步驟</param>
        /// <returns>Byte[] EXCEL檔案</returns>
        public byte[] CreateReportGoldOld(int ayear, int applyunit, int step)
        {
            string title = gd.GetUnitName((short)applyunit) + ayear + "年農田水利設施更新改善計畫(全部)";

            #region 重新計算黃金廊道經費
            List<casepaydetail> CasePays = clacGoldPay(ayear, applyunit);
            #endregion



            if (CasePays.Count() == 0)
            {
                string sourcepath1 = @"~\ReportSample\ClosingAccount.xlsx";
                FileStream fs1 = new FileStream(HttpContext.Current.Server.MapPath(sourcepath1), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                ExcelPackage excelA = new ExcelPackage(fs1);
                ExcelWorksheet sheetA = excelA.Workbook.Worksheets["工程決算書"];
                sheetA.Cells[1, 1].Value = title;
                return excelA.GetAsByteArray();

            }

            var CaseDetail = (from tb in DryDB.CaseDetail
                              where tb.ApplyUnit == applyunit && tb.ApplyYear == ayear && /*tb.Step >= step*/ tb.Complete == true && tb.Gold == true
                              select new { tb }).ToList();
            
            var datalist = (//from tb1 in DryDB.CaseDetail
                            //join tb2 in DryDB.CasePayDetail on tb1.MapNo equals tb2.mapno into tb3
                            from tb1 in CaseDetail
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
                            }).OrderBy(m => m.landTown).ThenBy(m => m.EndTypeCNS).ToList();


            var datalist1 = (//from tb1 in DryDB.CaseDetail
                             //join tb2 in DryDB.CasePayDetail on tb1.MapNo equals tb2.mapno into tb3
                             from tb1 in CaseDetail
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
                             }).OrderBy(m => m.EndTypeCNS).ToList();

            


            
            string sourcepath = @"~\ReportSample\ClosingAccount.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["工程決算書"];
            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 4;
            foreach (var data in datalist)
            {
                irow += 1;
                sheet.Cells[irow, 1].Value = data.landTown;
                sheet.Cells[irow, 2].Value = data.EndTypeCNS;
                sheet.Cells[irow, 3].Value = data.buildarea;
                sheet.Cells[irow, 4].Value = data.reccount;
                sheet.Cells[irow, 5].Value = data.farmerPay;
                sheet.Cells[irow, 6].Value = data.田間管路設施費;
                sheet.Cells[irow, 7].Value = data.蓄水設備費;
                sheet.Cells[irow, 8].Value = data.動力設備費;
                sheet.Cells[irow, 9].Value = data.調控設施費;
                sheet.Cells[irow, 10].Value = data.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";

            }
            foreach (var data in datalist1)
            {
                irow += 1;
                sheet.Cells[irow, 1].Value = "合計";
                sheet.Cells[irow, 2].Value = data.EndTypeCNS;
                sheet.Cells[irow, 3].Value = data.buildarea;
                sheet.Cells[irow, 4].Value = data.reccount;
                sheet.Cells[irow, 5].Value = data.farmerPay;
                sheet.Cells[irow, 6].Value = data.田間管路設施費;
                sheet.Cells[irow, 7].Value = data.蓄水設備費;
                sheet.Cells[irow, 8].Value = data.動力設備費;
                sheet.Cells[irow, 9].Value = data.調控設施費;
                sheet.Cells[irow, 10].Value = data.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
            }

            return excel.GetAsByteArray();

        }
        public byte[] CreateReportGold(int ayear, int applyunit, int step)
        {
            string title = gd.GetUnitName((short)applyunit) + ayear + "年農田水利設施更新改善計畫(全部)";

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

            //黃金廊道
            #region 重新計算黃金廊道補助金額
            List<CasePayDetailView> CasePays = new TownApplyStatisticsReport().getPays4Gold(ayear, applyunit/*, step*/);
            
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
            #endregion
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
            
            var datalist = (
                            from tb1 in AllCases
                            group new { tb1} by new { tb1.landTown, tb1.CatalogCNS } into tb4
                            select new
                            {
                                landTown = tb4.Key.landTown,
                                EndTypeCNS = tb4.Key.CatalogCNS,
                                buildarea = tb4.Sum(m => m.tb1.buildarea / 10000),
                                reccount = tb4.Count(),
                                farmerPay = tb4.Sum(m => m.tb1.FarmerFee),
                                田間管路設施費 = tb4.Sum(m => m.tb1.田間管路設施費),
                                規劃設計費 = tb4.Sum(m => m.tb1.規劃設計費),
                                水源設施費 = tb4.Sum(m => m.tb1.水源設施費),
                                調控設施費 = tb4.Sum(m => m.tb1.調控設施費),
                                動力設備費 = tb4.Sum(m => m.tb1.動力設備費),
                                蓄水設備費 = tb4.Sum(m => m.tb1.蓄水設備費),
                                設施費總計 = tb4.Sum(m => m.tb1.設施費總計),
                                工作費 = tb4.Sum(m => m.tb1.工作費),
                            }).OrderBy(m => m.landTown).ThenBy(m => m.EndTypeCNS).ToList();


            var datalist1 = (
                             from tb1 in AllCases
                             group new { tb1} by new { tb1.CatalogCNS } into tb4
                             select new
                             {

                                 EndTypeCNS = tb4.Key.CatalogCNS,
                                 buildarea = tb4.Sum(m => m.tb1.buildarea / 10000),
                                 reccount = tb4.Count(),
                                 farmerPay = tb4.Sum(m => m.tb1.FarmerFee),
                                 田間管路設施費 = tb4.Sum(m => m.tb1.田間管路設施費),
                                 規劃設計費 = tb4.Sum(m => m.tb1.規劃設計費),
                                 水源設施費 = tb4.Sum(m => m.tb1.水源設施費),
                                 調控設施費 = tb4.Sum(m => m.tb1.調控設施費),
                                 動力設備費 = tb4.Sum(m => m.tb1.動力設備費),
                                 蓄水設備費 = tb4.Sum(m => m.tb1.蓄水設備費),
                                 設施費總計 = tb4.Sum(m => m.tb1.設施費總計),
                                 工作費 = tb4.Sum(m => m.tb1.工作費),
                             }).OrderBy(m => m.EndTypeCNS).ToList();
            
            
            string sourcepath = @"~\ReportSample\ClosingAccount.xlsx";
            FileStream fs = new FileStream(HttpContext.Current.Server.MapPath(sourcepath), FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            ExcelPackage excel = new ExcelPackage(fs);
            ExcelWorksheet sheet = excel.Workbook.Worksheets["工程決算書"];
            int irow = 1;
            sheet.Cells[irow, 1].Value = title;
            irow = 4;
            foreach (var data in datalist)
            {
                irow += 1;
                sheet.Cells[irow, 1].Value = data.landTown;
                sheet.Cells[irow, 2].Value = data.EndTypeCNS;
                sheet.Cells[irow, 3].Value = data.buildarea;
                sheet.Cells[irow, 4].Value = data.reccount;
                sheet.Cells[irow, 5].Value = data.farmerPay;
                sheet.Cells[irow, 6].Value = data.田間管路設施費;
                sheet.Cells[irow, 7].Value = data.蓄水設備費;
                sheet.Cells[irow, 8].Value = data.動力設備費;
                sheet.Cells[irow, 9].Value = data.調控設施費;
                sheet.Cells[irow, 10].Value = data.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";

            }
            foreach (var data in datalist1)
            {
                irow += 1;
                sheet.Cells[irow, 1].Value = "合計";
                sheet.Cells[irow, 2].Value = data.EndTypeCNS;
                sheet.Cells[irow, 3].Value = data.buildarea;
                sheet.Cells[irow, 4].Value = data.reccount;
                sheet.Cells[irow, 5].Value = data.farmerPay;
                sheet.Cells[irow, 6].Value = data.田間管路設施費;
                sheet.Cells[irow, 7].Value = data.蓄水設備費;
                sheet.Cells[irow, 8].Value = data.動力設備費;
                sheet.Cells[irow, 9].Value = data.調控設施費;
                sheet.Cells[irow, 10].Value = data.規劃設計費;
                sheet.Cells[irow, 11].Formula = "+SUM(F" + irow + ":J" + irow + ")";
                sheet.Cells[irow, 12].Formula = "+SUM(E" + irow + ",K" + irow + ")";
            }

            return excel.GetAsByteArray();

        }

        private List<casepaydetail> clacGoldPay(int ayear, int applyunit)
        {
            List<casepaydetail> CasePays = new List<casepaydetail>();

            var datalist1 = from tb2 in DryDB.SummaryView
                            where tb2.ApplyUnit == applyunit && tb2.ApplyYear == ayear && tb2.Complete == true && tb2.Gold == true
                            select new { tb2.MapNo };
            #region 重新計算案件金額
            foreach (var data in datalist1)
            {
                var datalist2 = (from tb1 in DryDB.Pay
                                 where tb1.MapNo == data.MapNo
                                 select new { tb1 }
                                     ).ToList();
                if (datalist2.Count > 0)
                {
                    casepaydetail casepay = new casepaydetail();
                    casepay.mapno = (int)data.MapNo;
                    foreach (var data1 in datalist2)
                    {
                        switch (data1.tb1.ItemCode)
                        {
                            case 1:
                                casepay.田間管路設施費 = (int)Math.Round((double)(data1.tb1.Total) * 0.7, 0, MidpointRounding.AwayFromZero);
                                casepay.FarmerFee += (data1.tb1.Total.GetValueOrDefault() - casepay.田間管路設施費);
                                casepay.Total += data1.tb1.Total.GetValueOrDefault();
                                break;
                            case 2:
                                casepay.規劃設計費 = (int)data1.tb1.Total;
                                casepay.Total += data1.tb1.Total.GetValueOrDefault();
                                break;
                            case 3:
                                casepay.水源設施費 = (int)data1.tb1.Total;
                                casepay.Total += data1.tb1.Total.GetValueOrDefault();
                                break;
                            case 4:
                                casepay.調控設施費 = (int)Math.Round((double)(data1.tb1.Total) * 0.7, 0, MidpointRounding.AwayFromZero);
                                casepay.FarmerFee += (data1.tb1.Total.GetValueOrDefault() - casepay.調控設施費);
                                casepay.Total += data1.tb1.Total.GetValueOrDefault();
                                break;
                            case 5:
                                casepay.動力設備費 = (int)data1.tb1.Total;
                                casepay.Total += data1.tb1.Total.GetValueOrDefault();
                                break;
                            case 6:
                                casepay.蓄水設備費 = (int)(data1.tb1.Total);
                                casepay.Total += data1.tb1.Total.GetValueOrDefault();
                                break;
                            case 7:
                                casepay.設施費總計 = (int)Math.Round((double)(data1.tb1.Total) * 0.7, 0, MidpointRounding.AwayFromZero);
                                casepay.FarmerFee += (data1.tb1.Total.GetValueOrDefault() - casepay.設施費總計);
                                casepay.Total += data1.tb1.Total.GetValueOrDefault();
                                break;
                            case 8:
                                casepay.工作費 = (int)Math.Round((double)(data1.tb1.Total) * 0.7, 0, MidpointRounding.AwayFromZero);
                                casepay.FarmerFee += (data1.tb1.Total.GetValueOrDefault() - casepay.工作費);
                                casepay.Total += data1.tb1.Total.GetValueOrDefault();
                                break;


                        }
                    }
                    CasePays.Add(casepay);
                }
            }
            #endregion
            return CasePays;
        }
        private class casepaydetail
        {
            public int mapno { get; set; }
            public int 田間管路設施費 { get; set; }
            public int 規劃設計費 { get; set; }
            public int 水源設施費 { get; set; }
            public int 調控設施費 { get; set; }
            public int 動力設備費 { get; set; }
            public int 蓄水設備費 { get; set; }
            public int 設施費總計 { get; set; }
            public int 工作費 { get; set; }
            public int FarmerFee { get; set; }
            public int Total { get; set; }
        }

    }
}
