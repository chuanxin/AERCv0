using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.ReportModules;
using Dry.Models.CommonCls;
using AERC.Models.CommonCls;
using Dry.Models.Service;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class StatReportController : Controller
    {
        
        public ActionResult Index(Dry.Models.ViewModel.ReportView data)
        {
            List<SelectListItem> ApplyYearDDL = new  CommClass().GetApplyYearByUnit(short.Parse(Session["UnitID"].ToString()));
            if (data.ApplyYear == 0 /*ApplyYearDDL.Any(m => m.Selected == true)*/)
            {
                data.ApplyYear = Convert.ToInt32(ApplyYearDDL.LastOrDefault().Value);
                //data.ApplyYearStart = data.ApplyYear;
                //data.ApplyYearEnd = data.ApplyYear;
            }
           
            data.ApplyYearDDL = ApplyYearDDL.Where(m => m.Text != "歷年").ToList();
            foreach (var item in ApplyYearDDL)
            {
                if (item.Value == data.ApplyYear.ToString())
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }
            data.UnitsDDL = new GetUints().GetUnitDDL();
            //data.gold = false;
            ViewBag.Unit=int.Parse(Session["UnitID"].ToString());
            data.govtype = 1;
            return View(data);
        }
        /// <summary>
        /// 下載農作物統計表
        /// </summary>
        /// <param name="data">View Data</param>
        /// <returns>EXCEL報表</returns>
        public ActionResult PlantArea(Dry.Models.ViewModel.ReportView data)
        {

            
            int unit = int.Parse(Session["UnitID"].ToString());
            if (unit == 0 || unit == 99 || unit == 20)
            {
                unit = data.ApplyUnit;
            }
            int step = 7;
            byte[] result;
            data.gold = (data.govtype == 2) ? true : false;
            result = new PlantArea().CreateReport(data.ApplyYear, unit, step, data.gold);
            
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 農作物統計表.xlsx");
        }
        /// <summary>
        /// 下載工程決算表
        /// </summary>
        /// <param name="data">View Data</param>
        /// <returns>EXCEL報表</returns>
        public ActionResult ClosingAccount(Dry.Models.ViewModel.ReportView data)
    {
            int unit = int.Parse(Session["UnitID"].ToString());
            if (unit == 0 || unit == 99 || unit == 20)
            {
                unit = data.ApplyUnit;
            }
            int step = 7;
            byte[] result;
            data.gold = (data.govtype == 2) ? true : false;
            if (unit == 9 || unit == 10)
            {
                if (data.govtype == 2)
                {
                    
                    result = new ClosingAccountReport().CreateReport(data.ApplyYear, unit, step);
                }else
                {
                    
                    result = new ClosingAccountReport().CreateReportGold(data.ApplyYear, unit, step);
                }

            }else
            {
                result = new ClosingAccountReport().CreateReport(data.ApplyYear, unit, step);
            }

            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 工程決算書.xlsx"); ;

    }
        /// <summary>
        /// 下載鄉鎮設施面積及補助統計表
        /// </summary>
        /// <param name="data">view data</param>
        /// <returns>EXCEL報表</returns>
        public ActionResult TownApplyStastics(Dry.Models.ViewModel.ReportView data)
        {
            int unit = int.Parse(Session["UnitID"].ToString());
            if (unit == 0 || unit == 99 || unit == 20)
            {
                unit = data.ApplyUnit;
            }
            int step = 7;
            byte[] result;
            data.gold = (data.govtype == 2) ? true : false;
            if (unit == 9 || unit == 10)
            {
                if (data.govtype == 2)
                {
                    
                    result = new TownApplyStatisticsReport().CreateReport(data.ApplyYear, unit, step);
                    return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 設施面積及補助.xlsx");
                }
                else
                {
                    
                    result = new TownApplyStatisticsReport().CreateReportGold(data.ApplyYear, unit, step);
                    return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 設施面積及補助.xlsx");
                }
            }else
            {
                result = new TownApplyStatisticsReport().CreateReport(data.ApplyYear, unit, step);
                return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 設施面積及補助.xlsx");
            }
                        
        }
        /// <summary>
        /// 下載全國鄉鎮設施面積及補助統計表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult TownApplyStasticsAll(Dry.Models.ViewModel.ReportView data)
        {
            
            //int step = 7;
            byte[] result;
            result = new TownApplyStatisticsReport().CreateReportAll(data.ApplyYearStart,data.ApplyYearEnd/*unit, step, data.gold*/);
            
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 設施面積及補助.xlsx");
        }

        public ActionResult TownApplyStasticsAllBudget(Dry.Models.ViewModel.ReportView data)
        {

            //int step = 7;
            byte[] result;
            result = new TownApplyStatisticsReport().CreateReportAllBudget(data.ApplyYearStart, data.ApplyYearEnd, 0);

            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 設施面積及補助.xlsx");
        }
        public ActionResult CityApplyStasticsAll(Dry.Models.ViewModel.ReportView data)
        {
            //int unit = int.Parse(Session["UnitID"].ToString());
            //if (unit == 0 || unit == 99 || unit == 20)
            //{
            //    unit = data.ApplyUnit;
            //}
            //int step = 7;
            byte[] result;
            result = new TownApplyStatisticsReport().CreateReportAll4City(data.ApplyYearStart, data.ApplyYearEnd);

            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 設施面積及補助.xlsx");
        }
        /// <summary>
        /// 歷年成果統計表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult DeliverResult(Dry.Models.ViewModel.ReportView data)
        {
            int unit = int.Parse(Session["UnitID"].ToString());
            if (unit == 0 || unit == 99 || unit == 20)
            {
                unit = data.ApplyUnit;
            }
            byte[] result = new DeliverResultReport().CreateReport(unit, data.ApplyYear,72);
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 成果統計.xlsx"); 
        }
        /// <summary>
        /// 全國歷年成果統計表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult DeliverResultAll(Dry.Models.ViewModel.ReportView data)
        {
            int unit = int.Parse(Session["UnitID"].ToString());
            if (unit == 0 || unit == 99 || unit == 20)
            {
                unit = data.ApplyUnit;
            }            
            byte[] result = new DeliverResultReport().CreateReportAll(unit, /*data.ApplyYearStart*/72, data.ApplyYearEnd);
            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 成果統計.xlsx");
        }
        /// <summary>
        /// 水利會歷年成果統計
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult IaApplyStatisticsReport(Dry.Models.ViewModel.ReportView data)
        {
            byte[] result;
            result = new TownApplyStatisticsReport().CreateReportAll4IA(data.ApplyYearStart, data.ApplyYearEnd);

            return File(result, "application/vnd.ms-excel", DateTime.Now.ToString("yyyyMMdd") + " - 設施面積及補助.xlsx");
        }

    }
}
