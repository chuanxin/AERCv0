/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-5-27
-- Description: 蓄水設施
-- =============================================
*/

using Dry.Models;
using Dry.Models.CommonCls;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class ApplyPoolController : Controller
    {
        #region Parameter
        private int _MNo;
        private PoolDBService pooldb = new PoolDBService();
        private CommClass comm = new CommClass();
        private MappingClass maping = new MappingClass();
        private GetData gda = new GetData();

        private Calculate_Funding CalculateCls = new Calculate_Funding();

        #endregion

        #region Create

        #region Create Page Controller
        public ActionResult Create()
        {
            //Session["MapNo"] = 68;
            if (Session["MapNo"] == null)
                return RedirectToAction("Index", "ApplyIndex");
            _MNo = (int)Session["MapNo"];
            PoolView Model = new PoolView();
            Case cse = gda.GetCaseDataFromMapNo(_MNo);
            int area = gda.GetFarmBuildArea(_MNo);//must get current build area.
            int CaseMoney = CalculateCls.Remaining_funds(/*5*/4, _MNo, cse);
            Model.Step = gda.GetCaseDataFromMapNo(_MNo).Step;
            Model.IsModify = Model.Step >= 4/*5*/ ? true : false;
            Model.WeightDDL = comm.GetPoolWeiDDL(cse.ApplyUnit);
            Model.PoolTypeDDL = comm.GetPoolTyeList(cse.ApplyUnit);
            Model.Area = area;
            Model.CaseMoney = CaseMoney;
            
            //Model.PoolPrice = CalculateCls.GetpoolMoney((byte)Convert.ToInt32(Model.PoolTypeDDL.First().Value), Convert.ToInt32(Model.WeightDDL.First().Value), cse, _MNo);
            Model.PoolPrice = gda.GetPoolPriceByCase(cse, (byte)Convert.ToInt32(Model.PoolTypeDDL.First().Value), Convert.ToInt32(Model.WeightDDL.First().Value));

            int BMno;
            if (gda.chgDesign(_MNo))
            {
                //return Redirect("../ApplyPool/chgPool");
                BMno = Model.IsModify == true ? _MNo : gda.GetBeforeNowMapNo(_MNo);
            }
            else
            {
                BMno = _MNo;
            }
            //get previous mapping version data.
            PoolApply applydata = gda.GetPoolApplyData(BMno);

            List<Pool> data = gda.GetPoolData(BMno);
            int FacPrice = applydata == null ? 0 : applydata.FacMoney;

            List<PoolView.ModifyPoolData> NEWpoollist = new List<PoolView.ModifyPoolData>();
            //int OrgPoolPrice = 0;
            int nowMoney = 0;
            if (data != null)
            {
                foreach (var pool in data)
                {
                    
                    int price = pool.PoolPrice;

                    
                    int payMoney = ((nowMoney + price) > CaseMoney) ? ((CaseMoney - nowMoney) < 0) ? 0 : CaseMoney - nowMoney : price;
                    /*                    
                    Case cseitem = new Case();
                    cseitem.Gold = false;
                    cseitem.ApplyUnit = cse.ApplyUnit;
                    OrgPoolPrice += pooldb.CaculatePoolPrice(cseitem, BMno,pool.PtypeCode, pool.PoolWeight);

                    NEWpoollist.Add(new PoolView.ModifyPoolData(pool.PtypeCode, maping.GetPooltypeName(pool.PtypeCode), pool.PoolWeight, pool.PoolSize, price));
                    nowMoney += (OrgPoolPrice - pool.PoolPrice);
                     */
                    NEWpoollist.Add(new PoolView.ModifyPoolData(pool.PtypeCode, maping.GetPooltypeName(pool.PtypeCode), pool.PoolWeight, pool.PoolSize, price));
                    nowMoney += payMoney;
                }
                Model.TotalM = data.Sum(m => m.PoolPrice);
                if (Model.TotalM > Model.CaseMoney)
                {
                    Model.GovM = Model.CaseMoney;
                    Model.FarmM = Model.TotalM - Model.CaseMoney;
                }
                else
                {
                    Model.GovM = Model.TotalM;
                    Model.FarmM = 0;
                }
            }
            int caseaplyunit = gda.getCaseAppliedUnit(_MNo);
            Model.UnitDDL = comm.GetUnitListByIdx(applydata == null ? /*(short)Session["UnitID"]*/ (short)caseaplyunit : applydata.ApplyUnit, BMno);
            Model.ddl_PoolUnit = applydata == null ? /*(short)Session["UnitID"]*/ (short)caseaplyunit : applydata.ApplyUnit;


            Model.PoolData = NEWpoollist;
            Model.FacPrice = FacPrice.ToString();
            ViewBag.Gold = cse.Gold;
            /*
            if(cse.Gold == true)
            {
                Model.GovM = OrgPoolPrice;
                Model.GoldM = Model.TotalM - OrgPoolPrice;
                Model.FarmM = 0;
            }
             */ 
            //Model.TotalM = nowMoney;
            
            return PartialView(Model);
        }
        #endregion

        #region Receive Postback CreateData
        public ActionResult CreateData([System.Web.Http.FromBody] PoolDBService.JsData ResultArry)
        {
            //if (ResultArry.PoolAry == null)
            //    return Content("非法進入");
            string msg = "Success";
            if (ModelState.IsValid)
            {
                if (ResultArry.ddl_Unit == -1)
                {
                    return Content("補助單位沒有填寫!\n");
                }
                //if (ResultArry.PoolAry.Count > 0)
                //{
                _MNo = (int)Session["MapNo"];
                    msg = pooldb.InsertPool(ResultArry, _MNo).DbMessage;
                //}
                //else
                //{
                //    msg = "請至少輸入一筆蓄水設施\n";
                //}
            }
            else
            {
                msg = "驗證未通過 !\n";
            }
            return Content(msg);
        }
        #endregion
        /// <summary>
        /// 略過此步驟
        /// </summary>
        /// <returns></returns>
        public ActionResult PassData()
        {
            _MNo = (int)Session["MapNo"];
            return Json(new CommClass().UpdateCase(_MNo, 4 /*5*/).DbMessage, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Edit

        #region Edit Page Controller
        public ActionResult Edit()
        {
            //Session["MapNo"] = 20;
            _MNo = (int)Session["MapNo"];

            //Get Data Frome DB
            PoolApply applydata = gda.GetPoolApplyData(_MNo);
            List<Pool> data = gda.GetPoolData(_MNo);

            int FacPrice = applydata.FacMoney;
            int area = gda.GetFarmBuildArea(_MNo);

            Case casedata = gda.GetCaseDataFromMapNo(_MNo);

            List<PoolView.ModifyPoolData> NEWpoollist = new List<PoolView.ModifyPoolData>();
            int CaseMoney = CalculateCls.Remaining_funds(/*5*/4, _MNo, casedata);
            int nowMoney = 0;
            foreach (var pool in data)
            {
                
                int price = CalculateCls.GetpoolMoney(pool.PtypeCode, pool.PoolWeight, casedata, _MNo);

                
                int payMoney = ((nowMoney + price) > CaseMoney) ? ((CaseMoney - nowMoney) < 0) ? 0 : CaseMoney - nowMoney : price;

                NEWpoollist.Add(new PoolView.ModifyPoolData(pool.PtypeCode, maping.GetPooltypeName(pool.PtypeCode), pool.PoolWeight, pool.PoolSize, payMoney));
                nowMoney += payMoney;
            }
            PoolView Model = new PoolView();
            Model.PoolTypeDDL = comm.GetPoolTyeList();
            Model.UnitDDL = comm.GetUnitListByIdx(applydata.ApplyUnit, _MNo);
            Model.ddl_PoolUnit = applydata.ApplyUnit;

            Model.PoolData = NEWpoollist;
            Model.FacPrice = FacPrice.ToString();

            Model.Area = area;
            Model.CaseMoney = CaseMoney;
            Model.TotalM = nowMoney;

            return View(Model);
        }
        #endregion

        #region Receive Postback ModifyData
        public ActionResult ModifyData([System.Web.Http.FromBody] PoolDBService.JsData ResultArry)
        {
            //if (ResultArry.PoolAry == null)
            //    return Content("非法進入");
            string msg = "Success";
            if (ModelState.IsValid)
            {
                if (ResultArry.ddl_Unit == -1)
                {
                    return Content("補助單位沒有填寫!\n");
                }
                _MNo = (int)Session["MapNo"];
                msg = pooldb.ModifyPool(ResultArry, _MNo).DbMessage;
                //if (ResultArry.PoolAry.Count > 0)
                //{
                //    msg = pooldb.ModifyPool(ResultArry, _MNo).DbMessage;
                //}
                //else
                //{
                //    msg = "請至少輸入一筆蓄水設施\n";
                //}
            }
            else
            {
                msg = "驗證未通過 !\n";
            }
            return Content(msg);
        }
        #endregion

        #endregion


        #region 計算調蓄設施補助經費結果，回傳給JS
        public JsonResult CalPoolMoney([System.Web.Http.FromBody] PoolDBService.PoolData[] PoolAry)
        {
            //if (PoolAry == null)
            //    return Content("非法進入");
            _MNo = (int)Session["MapNo"];
            Case casedata = gda.GetCaseDataFromMapNo(_MNo);
            Calculate_Funding.MoneyData totalMoney = CalculateCls.GetPoolMoneybyList(PoolAry, casedata, _MNo);
            PoolMoneyData PoolMoney = new PoolMoneyData();
            PoolMoney.GovPay = totalMoney.GovPay;
            PoolMoney.FarmerPay = totalMoney.FarmerPay;
            PoolMoney.PoolPrice = PoolAry == null ? 0 : PoolAry.Last().poolPrice;
            
            int MaxMoney = CalculateCls.Remaining_funds(/*5*/4, _MNo, casedata);
            int PoolUseM = PoolAry == null ? 0 : PoolAry.Sum(p => p.poolPrice);
            PoolMoney.FundLeft = MaxMoney - PoolUseM /*PoolMoney.GovPay*/;
            //
            /*
            if (casedata.Gold)
            {
                
                PoolMoney.GoldPay = totalMoney.GoldPay;
            }
            */
            return Json(PoolMoney, JsonRequestBehavior.AllowGet);
        }
        public class PoolMoneyData : Calculate_Funding.MoneyData
        {
            public int PoolPrice { get; set; }
        }
        #endregion
        public JsonResult CaculatePoolPrice(byte PtypeCode, int PoolTon)
        {
            _MNo = (int)Session["MapNo"];
            Case casedata = gda.GetCaseDataFromMapNo(_MNo);
            return Json(pooldb.CaculatePoolPrice(casedata, _MNo, PtypeCode, PoolTon).ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CaculatePoolPriceYear(int PoolTon,int years)
        {
            return Json(pooldb.CaculatePoolPriceByYears(PoolTon, years).ToString(),JsonRequestBehavior.AllowGet);
        }
    }
}
