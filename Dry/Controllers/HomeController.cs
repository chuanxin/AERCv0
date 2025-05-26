/*
-- =============================================
-- Author: WEI
-- Create date: 2014-2-10
-- Description: 旱作灌溉系統首頁的Controller
-- =============================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models;
using Dry.Models.CommonCls;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using Dry.Models.ReportModules;

namespace Dry.Controllers
{
    [Authorize][HandleError][SessionCheck]
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        readonly StatisticHomeDBService SHDB = new StatisticHomeDBService();
        private StatisticHomeView2020 SHV = new StatisticHomeView2020();
        public ActionResult Index()
        {
            int years, units;
            System.DateTime currentTime = System.DateTime.Now;
            units = Convert.ToInt32(Session["UnitID"]);
            years = currentTime.Year - 1911;
            List<int> coalist = new List<int> { 0, 99, 100, 20 };
            if (coalist.Contains(units)  )
            {
               return RedirectToAction("IndexCoa");
            }

            //years = 107;
            List<SelectListItem> ApplyYearDDL = new CommClass().GetApplyYearByUnit(short.Parse(Session["UnitID"].ToString()));
            ApplyYearDDL.RemoveAt(0);
            
            ViewBag.ApplyYearDDL = ApplyYearDDL;
            ViewBag.QryYear = years;
            stasticAnnualBudget coaBuget = SHDB.getStatfromUnit(years, units, 0);
            stasticAnnualBudget luBuget = SHDB.getStatfromUnit(years, units, 17);
            stasticAnnualBudget cxBuget = SHDB.getStatfromUnit(years, units, 16);
            stasticAnnualBudget goldBuget = SHDB.getStatGoldfromUnit2020(years, units,0);
            //stasticAnnualBudgetGold goldBuget = SHDB.getStatGoldfromUnit(years, units);
            
            
            SHV.ReservationsofFarmer_pis = coaBuget.ReservationsofFarmer + luBuget.ReservationsofFarmer + cxBuget.ReservationsofFarmer;
            
            SHV.ReservationsofFarm_pis = coaBuget.ReservationsofFarm + luBuget.ReservationsofFarm + cxBuget.ReservationsofFarm ;
            
            SHV.ReservationsofMoney_pis = (coaBuget.ReservationsofMoney + luBuget.ReservationsofMoney + cxBuget.ReservationsofMoney) / 1000;
            
            SHV.Ch_ReservationsofMoney_pis = (coaBuget.ReservationsofMoneyNow + luBuget.ReservationsofMoneyNow + cxBuget.ReservationsofMoneyNow) / 1000;
            
            SHV.NumofFarmer_pis = coaBuget.NumofFarmer + luBuget.NumofFarmer + cxBuget.NumofFarmer;
            //SHV.NumofFarm_pis = SHDB.CountFarm(years, units);
            SHV.NumofFarm_pis = coaBuget.NumofFarm + luBuget.NumofFarm + cxBuget.NumofFarm;
            //if (SHV.ReservationsofFarm_pis > 0)
            //    SHV.ReachOfArea_pis = Math.Round((SHV.ProvisionOfFarm_pis / SHV.ReservationsofFarm_pis) * 100.0, 2);
            //else
            //    SHV.ReachOfArea_pis = 0.0;
            
            SHV.ProvisionOfFarmer_pis = coaBuget.ProvisionOfFarmer + luBuget.ProvisionOfFarmer + cxBuget.ProvisionOfFarmer;
            
            SHV.ProvisionOfFarm_pis = coaBuget.ProvisionOfFarm + luBuget.ProvisionOfFarm + cxBuget.ProvisionOfFarm;
            
            SHV.ProvisionOfMoney_pis = coaBuget.ProvisionOfMoney + luBuget.ProvisionOfMoney + cxBuget.ProvisionOfMoney;
            
            SHV.Unusemoney_pis = coaBuget.Unusemoney + luBuget.Unusemoney + cxBuget.Unusemoney;
            
            SHV.ProvisionofFarmerend_pis = coaBuget.ProvisionofFarmerend + luBuget.ProvisionofFarmerend + cxBuget.ProvisionofFarmerend;
            
            SHV.ProvisionOfFarmend_pis = coaBuget.ProvisionOfFarmend + luBuget.ProvisionOfFarmend + cxBuget.ProvisionOfFarmend;
            
            SHV.ProvisionOfMoneyend_pis = coaBuget.ProvisionOfMoneyend + luBuget.ProvisionOfMoneyend + cxBuget.ProvisionOfMoneyend;

            //if (SHV.Ch_ReservationsofMoney_pis != 0)
            //    SHV.PlanExe = Math.Round(((double)SHV.ProvisionOfMoneyend_pis / (double)(SHV.Ch_ReservationsofMoney_pis * 1000)) * 100.0, 2);
            //else
            //    SHV.PlanExe = 0.0;
            if (SHV.ProvisionOfMoney_pis > 0)
            {
                SHV.PlanExe = Math.Round( (double)(SHV.ProvisionOfMoneyend_pis / SHV.ProvisionOfMoney_pis) * 100 ,2);
            }
            if (SHV.ReservationsofFarm_pis > 0)
                SHV.ReachOfArea_pis = Math.Round((SHV.ProvisionOfFarm_pis / SHV.ReservationsofFarm_pis) * 100.0, 2);
            else
                SHV.ReachOfArea_pis = 0.0;

            SHV.Desinge_pis = coaBuget.DesignFee + luBuget.DesignFee + cxBuget.DesignFee;

            
            
            SHV.ReservationsofFarmer_lu = luBuget.ReservationsofFarmer;
            
            SHV.ReservationsofFarm_lu = luBuget.ReservationsofFarm;
            
            SHV.ReservationsofMoney_lu = luBuget.ReservationsofMoney / 1000;
            
            SHV.Ch_ReservationsofMoney_lu = luBuget.ReservationsofMoneyNow / 1000;
            
            SHV.NumofFarmer_lu = luBuget.NumofFarmer;
            
            SHV.NumofFarm_lu = luBuget.NumofFarm;
            if (SHV.ReservationsofFarm_lu != 0)
                SHV.ReachOfArea_lu = Math.Round((SHV.NumofFarm_lu / SHV.ReservationsofFarm_lu) * 100.0, 2);
            else
                SHV.ReachOfArea_lu = 0.0;
            
            SHV.ProvisionOfFarmer_lu = luBuget.ProvisionOfFarmer;
            
            SHV.ProvisionOfFarm_lu = luBuget.ProvisionOfFarm;
            
            SHV.ProvisionOfMoney_lu = luBuget.ProvisionOfMoney;
            
            SHV.Unusemoney_lu = luBuget.Unusemoney;
            
            SHV.ProvisionofFarmerend_lu = luBuget.ProvisionofFarmerend;
            
            SHV.ProvisionOfFarmend_lu = luBuget.ProvisionOfFarmend;
            
            SHV.ProvisionOfMoneyend_lu = luBuget.ProvisionOfMoneyend;
            //if (SHV.Ch_ReservationsofMoney_lu != 0)
            //    SHV.PlanExe_lu = Math.Round(((double)SHV.ProvisionOfMoneyend_lu / (double)(SHV.Ch_ReservationsofMoney_lu * 1000)) * 100.0, 2);
            //else
            //    SHV.PlanExe_lu = 0.0;
            SHV.PlanExe_lu = luBuget.PlanExe;
            //20200731 alex modify 
            //SHV.Desinge_lu = SHDB.design_money_liu(years, units);
            SHV.Desinge_lu = luBuget.DesignFee;


            

            SHV.ReservationsofFarmer_coa = coaBuget.ReservationsofFarmer;
            
            SHV.ReservationsofFarm_coa = coaBuget.ReservationsofFarm;
            
            SHV.ReservationsofMoney_coa = coaBuget.ReservationsofMoney / 1000;
            
            SHV.Ch_ReservationsofMoney_coa = coaBuget.ReservationsofMoneyNow / 1000;
            
            SHV.NumofFarmer_coa = coaBuget.NumofFarmer;
            
            SHV.NumofFarm_coa = coaBuget.NumofFarm;
            //if (SHV.ReservationsofFarm_coa != 0)
            //    SHV.ReachOfArea_coa = Math.Round((SHV.ProvisionOfFarm_coa / SHV.ReservationsofFarm_coa) * 100.0, 2);
            //else
            //    SHV.ReachOfArea_coa = 0.0;
            
            SHV.ProvisionOfFarmer_coa = coaBuget.ProvisionOfFarmer;
            
            SHV.ProvisionOfFarm_coa = coaBuget.ProvisionOfFarm;
            
            SHV.ProvisionOfMoney_coa = coaBuget.ProvisionOfMoney;
            
            SHV.Unusemoney_coa = coaBuget.Unusemoney;
            
            SHV.ProvisionofFarmerend_coa = coaBuget.ProvisionofFarmerend;
            
            SHV.ProvisionOfFarmend_coa = coaBuget.ProvisionOfFarmend;
            
            SHV.ProvisionOfMoneyend_coa = coaBuget.ProvisionOfMoneyend;
            if (SHV.ReservationsofFarm_coa != 0)
                SHV.ReachOfArea_coa = Math.Round((SHV.ProvisionOfFarm_coa / SHV.ReservationsofFarm_coa) * 100.0, 2);
            else
                SHV.ReachOfArea_coa = 0.0;
            //if (SHV.Ch_ReservationsofMoney_coa != 0)
            //    SHV.PlanExe_coa = Math.Round(((double)SHV.ProvisionOfMoneyend_coa / (double)(SHV.Ch_ReservationsofMoney_coa * 1000)) * 100.0, 2);
            //else
            //    SHV.PlanExe_coa = 0.0;
            SHV.PlanExe_coa = coaBuget.PlanExe;
            SHV.Desinge_coa = coaBuget.DesignFee;

            
            SHV.ReservationsofFarmer_cx = cxBuget.ReservationsofFarmer;

            SHV.ReservationsofFarm_cx = cxBuget.ReservationsofFarm;

            SHV.ReservationsofMoney_cx = cxBuget.ReservationsofMoney / 1000;

            SHV.Ch_ReservationsofMoney_cx = cxBuget.ReservationsofMoneyNow / 1000;

            SHV.NumofFarmer_cx = cxBuget.NumofFarmer;

            SHV.NumofFarm_cx = cxBuget.NumofFarm;
            if (SHV.ReservationsofFarm_cx != 0)
                SHV.ReachOfArea_cx = Math.Round((SHV.NumofFarm_cx / SHV.ReservationsofFarm_cx) * 100.0, 2);
            else
                SHV.ReachOfArea_cx = 0.0;
            SHV.ProvisionOfFarmer_cx = cxBuget.ProvisionOfFarmer;

            SHV.ProvisionOfFarm_cx = cxBuget.ProvisionOfFarm;

            SHV.ProvisionOfMoney_cx = cxBuget.ProvisionOfMoney;

            SHV.Unusemoney_cx = cxBuget.Unusemoney;

            SHV.ProvisionofFarmerend_cx = cxBuget.ProvisionofFarmerend;

            SHV.ProvisionOfFarmend_cx = cxBuget.ProvisionOfFarmend;

            SHV.ProvisionOfMoneyend_cx = cxBuget.ProvisionOfMoneyend;
            //if (SHV.Ch_ReservationsofMoney_cx != 0)
            //    SHV.PlanExe_cx = Math.Round(((double)SHV.ProvisionOfMoneyend_cx / (double)(SHV.Ch_ReservationsofMoney_cx * 1000)) * 100.0, 2);
            //else
            //    SHV.PlanExe_cx = 0.0;
            SHV.PlanExe_cx = cxBuget.PlanExe;
            SHV.Desinge_cx = cxBuget.DesignFee;


            
            SHV.NumofFarmer_gold = goldBuget.NumofFarmer;
            SHV.NumofFarm_gold = goldBuget.NumofFarm;
            SHV.ProvisionOfFarmer_gold = goldBuget.ProvisionOfFarmer;
            SHV.ProvisionOfFarm_gold = goldBuget.ProvisionOfFarm;
            SHV.ProvisionOfMoney_gold = goldBuget.ProvisionOfMoney;
            SHV.ProvisionofFarmerend_gold = goldBuget.ProvisionofFarmerend;
            SHV.ProvisionOfFarmend_gold = goldBuget.ProvisionOfFarmend;
            SHV.ProvisionOfMoneyend_gold = goldBuget.ProvisionOfMoneyend;
            SHV.PlanExe_gold = goldBuget.PlanExe;
            SHV.Desinge_gold = goldBuget.DesignFee;

            return View(SHV);


        }

        #region staticsallUnit
        public ActionResult AllUnitData(int ApplyYear)
        {

            //int iyear = DateTime.Now.Year - 1911;
            
            List<StatisticHomeView2020> datalist = SHDB.getAllStatisticsData(ApplyYear);


            byte[] f = SHDB.exportStTable(datalist);
            string outputFile = DateTime.Now.ToString("yyyyMMddhhmmss") + "經費統計表.xlsx";
            return File(f, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",outputFile);

        }
        #endregion
       public ActionResult IndexCoa()
        {
            int units = Convert.ToInt32(Session["UnitID"]);
            List<int> coalist = new List<int> { 0, 99, 100, 20 };
            if (!coalist.Contains(units))
            {
                return RedirectToAction("Index");
            }
            
            int years = DateTime.Now.Year - 1911;
            //int years = 107;
            List<StatisticCoaView> ialist = SHDB.StatCoa(years);
            List<SelectListItem> ApplyYearDDL = new CommClass().GetApplyYearByUnit(short.Parse(Session["UnitID"].ToString()));
            ApplyYearDDL.RemoveAt(0);
            ViewBag.ApplyYearDDL = ApplyYearDDL;
            ViewBag.QryYear = years;
            return View(ialist);
        }
        public ActionResult DownloadIaFees(int ApplyYear)
        {
            byte[] f = SHDB.exportIaFees(ApplyYear);
            string outputFile = DateTime.Now.ToString("yyyyMMddhhmmss") + "經費統計表.xlsx";
            return File(f, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", outputFile);
        }

        public ActionResult DownLoadIaGoldFees(int ApplyYear)
        {
            byte[] f = SHDB.exportIaGoldFees(ApplyYear);
            string outputFile = DateTime.Now.ToString("yyyyMMddhhmmss") + "廊道經費統計表.xlsx";
            return File(f, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", outputFile);
        }

        public ActionResult VIPPage()
        {
            return View();
        }
        public ActionResult DownLoadTownFees(int ApplyYear)
        {
            byte[] result;
            
            result = new TownApplyStatisticsReport().CreateReportAllBudget(ApplyYear, ApplyYear, 0);

            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 鄉鎮統計表.xlsx");
        }

        public ActionResult DownLoadIndiTown()
        {
            byte[] result = SHDB.exportIndiagenTown((DateTime.Now.Year-1911));

            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 原民鄉補助明細表.pdf");
        }

        public ActionResult DownLoadIndiIa()
        {
            byte[] result = SHDB.exportIndiagenIa((DateTime.Now.Year - 1911));

            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 管理處辦理原民鄉補助明細表.pdf");
        }

        public ActionResult DownLoadIaKPI()
        {
            byte[] result = SHDB.exportIaKPI((DateTime.Now.Year - 1911));

            return File(result, "application/pdf", DateTime.Now.ToString("yyyyMMdd") + " - 管理處辦理情形.pdf");
        }

        public ActionResult CourseVideos()
        {
            return View();
        }
    }
}
    