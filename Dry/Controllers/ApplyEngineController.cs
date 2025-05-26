/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-5-25
-- Description: 動力設備
-- =============================================
*/
using Dry.Models;
using Dry.Models.CommonCls;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class ApplyEngineController : Controller
    {
        #region Parameter
        private int _MNo;
        private EngineDBService engdb = new EngineDBService();
        private CommClass comm = new CommClass();
        private GetData gda = new GetData();
        private MappingClass maping = new MappingClass();
        private Calculate_Funding CalculateCls = new Calculate_Funding();

        #endregion

        #region Create

        #region Create Page Controller

        public ActionResult Create()
        {
            EngineView ModelData = new EngineView();
            //Session["MapNo"] = 59;
            if (Session["MapNo"] == null)
                return RedirectToAction("Index", "ApplyIndex");
            _MNo = (int)Session["MapNo"];
            ModelData.Step = gda.GetCaseDataFromMapNo(_MNo).Step;
            ModelData.IsModify = ModelData.Step >= /*4*/3 ? true : false;
            ModelData.EngineDDL = comm.GetEngList();
            ModelData.EngPrice = gda.GetEngPriceByMapNo(_MNo, 1); //預設動力設備價格
            ViewBag.Gold = gda.GetGoldFromMapno(_MNo);
            int BMno;
            if (gda.chgDesign(_MNo))
            {//若為變更設計
                BMno = ModelData.IsModify == true ? _MNo : gda.GetBeforeNowMapNo(_MNo);//get previous mapping version No, if the mode in modify, then  get current version mappnog NO.
            }
            else
            {
                BMno = _MNo;
            }
            //get previous mapping version engine data.
            List<Engine> data = gda.GetEngineData(BMno);
            EngApply applydata = gda.GetEngApplyData(BMno);

            //Get that case data and can use money of current version.
            Case casedata = gda.GetCaseDataFromMapNo(_MNo);
            int CaseM = CalculateCls.Remaining_funds(/*4*/3, _MNo, casedata);//get current case available funding.

            List<EngineView.ModifyEngineData> NEWenginelist = new List<EngineView.ModifyEngineData>();
            //int GovM = 0;
            int TotalM = 0;
            //int GoldM = 0;
            if (data != null)
            {
                foreach (var eng in data)
                {
                    //動力設備補助價格改為Engine資料表裡查詢 in 2015/5/13
                    #region old code
                    //int money = CalculateCls.GetengineMoney(eng.EngCode, casedata);
                    #endregion
                    int money = eng.EngPrice;

                    //int applym = money;//實際補助金額
                    //如果動力補助費用 + GovM > 案件可用經費
                    //if (money + GovM > CaseM)
                    //    applym = CaseM - GovM;
                    //GovM += applym;
                    //string msg = string.Format("{0}", applym);
                    //if ((money - applym) > 0)
                    //    msg = string.Format("{0}", money - applym);
                    TotalM += money;
                    string msg = string.Format("{0}", money);
                    NEWenginelist.Add(new EngineView.ModifyEngineData(eng.EngCode, maping.GetEngName(eng.EngCode), eng.EngRegCode, msg, eng.ENo));
                    //七星補助款
                    /*
                    if (casedata.Gold == true)
                    {
                        GoldM += eng.EngPrice - gda.GetEngPriceStdFromGold(eng.EngPrice);

                    }
                    */ 
                }
            }

            //EngineView ModelData = new EngineView();

            ModelData.EngData = NEWenginelist;
            int caseaplyunit = gda.getCaseAppliedUnit(_MNo);
            ModelData.UnitDDL = applydata == null ? comm.GetUnitListByIdx(/*-1*/(short)caseaplyunit, _MNo) : comm.GetUnitListByIdx(applydata.ApplyUnit, _MNo);
            ModelData.ddl_EngineUnit = applydata == null ? /*-1*/ caseaplyunit : applydata.ApplyUnit;
            ModelData.FacPrice = applydata == null ? "0" : applydata.FacMoney.ToString();
            ModelData.CaseMoney = CaseM;
            ModelData.TotalM = TotalM;
            if(TotalM>CaseM)
            {
                /*取消黃金廊道七星補助               
                if (casedata.Gold == true)
                {
                    ModelData.GovM = CaseM ;//黃金廊道
                    ModelData.GoldM =  GoldM;
                    ModelData.FarmM = TotalM - CaseM - GoldM;
                }
                else
                {
                    ModelData.GovM = CaseM;
                    ModelData.FarmM = TotalM - CaseM;
                }
                 */ 
                ModelData.GovM = CaseM;
                ModelData.FarmM = TotalM - CaseM;
            }
            else
            {
                /*取消黃金廊道七星補助
                if (casedata.Gold == true)
                {
                    ModelData.GovM = TotalM - GoldM;//黃金廊道
                    ModelData.GoldM = GoldM;
                    ModelData.FarmM = 0;
                }
                else
                {
                    ModelData.GovM = TotalM;
                    ModelData.FarmM = 0;
                }
                */
                ModelData.GovM = TotalM;
                ModelData.FarmM = 0;

            }

            if (ModelData.IsModify)
            {
                //return Redirect("../ApplyEngine/chgEng");
                
            }
            else
            {
                //ModelData.EngineDDL = comm.GetEngList();
                //ModelData.UnitDDL = comm.GetUnitList(_MNo);
                //ModelData.ddl_EngineUnit = -1;
                //ModelData.CaseMoney = CalculateCls.Remaining_funds(4, _MNo, gda.GetCaseDataFromMapNo(_MNo));
                //return View(ModelData);
                
            }
            return PartialView(ModelData);
        }

        #endregion

        #region Receive Postback CreateData
        public ActionResult CreateData([System.Web.Http.FromBody] EngineDBService.JsData ResultArry)
        {
            //if (ResultArry.EngAry == null)
            //    return Content("非法進入");

            string msg = "Success";
            if (ModelState.IsValid)
            {
                if (ResultArry.ddl_Unit == -1)
                {
                    return Content("補助單位沒有填寫!\n");
                }
                _MNo = (int)Session["MapNo"];
                //if (ResultArry.EngAry.Count > 0)
                //{
                    msg = engdb.InsertEngine(ResultArry, _MNo).DbMessage;
                //}
                //else
                //{
                //    msg = "請至少輸入一筆動力設備\n";
                //}
            }
            else
            {
                msg = "Model State Is Not Valid !\n";
            }
            return Content(msg);
        }
        #endregion
        //略過此步驟
        public ActionResult PassData()
        {
            _MNo = (int)Session["MapNo"];
            return Json(new CommClass().UpdateCase(_MNo,3 /*4*/).DbMessage, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Edit

        #region Edit Page Controller
        //public ActionResult Edit()
        //{
        //    //Session["MapNo"] = 20;
        //    _MNo = (int)Session["MapNo"];

        //    //Get Data From DB
        //    List<Engine> data = gda.GetEngineData(_MNo);
        //    EngApply applydata = gda.GetEngApplyData(_MNo);

        //    List<EngineView.ModifyEngineData> NEWenginelist = new List<EngineView.ModifyEngineData>();
        //    Case casedata = gda.GetCaseDataFromMapNo(_MNo);
        //    int CaseM = CalculateCls.Remaining_funds(4, _MNo, casedata);//案件可用經費
        //    int GovM = 0;
        //    foreach (var eng in data)
        //    {
        //        int money = CalculateCls.GetengineMoney(eng.EngCode, casedata);
        //        int applym = money;//實際補助金額
        //        //如果動力補助費用 + GovM > 案件可用經費
        //        if (money + GovM > CaseM)
        //            applym = CaseM - GovM;
        //        GovM += applym;
        //        string msg = string.Format("{0}", applym);
        //        if ((money - applym) > 0)
        //            msg = string.Format("{0}", money - applym);
        //        NEWenginelist.Add(new EngineView.ModifyEngineData(eng.EngCode, maping.GetEngName(eng.EngCode), eng.EngRegCode, msg, eng.ENo));
        //    }
        //    EngineView ModelData = new EngineView();

        //    ModelData.EngData = NEWenginelist;
        //    ModelData.EngineDDL = comm.GetEngList();
        //    ModelData.UnitDDL = comm.GetUnitListByIdx(applydata.ApplyUnit, _MNo);
        //    ModelData.ddl_EngineUnit = applydata.ApplyUnit;
        //    ModelData.FacPrice = applydata.FacMoney.ToString();
        //    ModelData.CaseMoney = CaseM;
        //    ModelData.TotalM = GovM;

        //    return View(ModelData);
        //}
        #endregion

        #region Receive Postback ModifyData
        public ActionResult ModifyData([System.Web.Http.FromBody] EngineDBService.JsData ResultArry)
        {
            //if (ResultArry.EngAry == null)
            //    return Content("非法進入");
            string msg = "Success";

            if (ModelState.IsValid)
            {
                if (ResultArry.ddl_Unit == -1)
                {
                    return Content("補助單位沒有填寫!\n");
                }
                _MNo = (int)Session["MapNo"];
                //if (ResultArry.EngAry.Count > 0)
                //{
                    msg = engdb.ModifyEngine(ResultArry, _MNo).DbMessage;
                //}
                //else
                //{
                //    msg = "請至少輸入一筆動力設備\n";
                //}
            }
            else
            {
                msg = "Model State Is Not Valid !\n";
            }
            return Content(msg);
        }
        #endregion

        #endregion

        #region Chang Desgin Page Controller
        //已整合至Create() by wei in 2015/2/25
        //public ActionResult chgEng()
        //{
        //    //Session["MapNo"] = 68;//測試用，到時註解
        //    _MNo = (int)Session["MapNo"];
        //    int BMno = gda.GetBeforeNowMapNo(_MNo);//get previous mapping version No

        //    //get previous mapping version engine data.
        //    List<Engine> data = gda.GetEngineData(BMno);
        //    EngApply applydata = gda.GetEngApplyData(BMno);

        //    //Get that case data and can use money of current version.
        //    Case casedata = gda.GetCaseDataFromMapNo(_MNo);
        //    int CaseM = CalculateCls.Remaining_funds(4, _MNo, casedata);//get current case available funding.

        //    List<EngineView.ModifyEngineData> NEWenginelist = new List<EngineView.ModifyEngineData>();
        //    int GovM = 0;
        //    if(data != null)
        //    {
        //        foreach (var eng in data)
        //        {
        //            int money = CalculateCls.GetengineMoney(eng.EngCode, casedata);
        //            int applym = money;//實際補助金額
        //            //如果動力補助費用 + GovM > 案件可用經費
        //            if (money + GovM > CaseM)
        //                applym = CaseM - GovM;
        //            GovM += applym;
        //            string msg = string.Format("{0}", applym);
        //            if ((money - applym) > 0)
        //                msg = string.Format("{0}", money - applym);
        //            NEWenginelist.Add(new EngineView.ModifyEngineData(eng.EngCode, maping.GetEngName(eng.EngCode), eng.EngRegCode, msg, eng.ENo));
        //        }
        //    }
            
        //    EngineView ModelData = new EngineView();

        //    ModelData.EngData = NEWenginelist;
        //    ModelData.EngineDDL = comm.GetEngList();
        //    ModelData.UnitDDL = applydata == null ? comm.GetUnitListByIdx(-1, _MNo) : comm.GetUnitListByIdx(applydata.ApplyUnit, _MNo);
        //    ModelData.ddl_EngineUnit = applydata == null ? -1 : applydata.ApplyUnit;
        //    ModelData.FacPrice = applydata == null ? "0" : applydata.FacMoney.ToString();
        //    ModelData.CaseMoney = CaseM;
        //    ModelData.TotalM = GovM;

        //    return View(ModelData);
        //}
        #endregion

        #region 計算動力設施補助經費結果，回傳給JS
        public JsonResult CalEngineMoney([System.Web.Http.FromBody] EngineDBService.EngineData[] EngAry)
        {
            //if (EngAry == null)
            //    return Content("非法進入");
            
            EngMoney ReturnMoney = new EngMoney();
            _MNo = (int)Session["MapNo"];
            Case casedata = gda.GetCaseDataFromMapNo(_MNo);
            Calculate_Funding.MoneyData totalMoney = new Calculate_Funding.MoneyData();
            if (casedata.Gold == true) {
                //取消黃金廊道七星補助 
                //totalMoney = CalculateCls.GetEngMoneybyEngListGold(EngAry, casedata);
                totalMoney = CalculateCls.GetEngMoneybyEngList(EngAry, casedata);
            }
            else
            {
                totalMoney = CalculateCls.GetEngMoneybyEngList(EngAry, casedata);
            }
            int MaxMoney = CalculateCls.Remaining_funds(/*4*/3, _MNo, casedata);
            ReturnMoney.FundLeft = MaxMoney - totalMoney.GovPay;//計算剩下的錢 edit in 2018/4/3
            ReturnMoney.GovPay = totalMoney.GovPay;
            ReturnMoney.FarmerPay = totalMoney.FarmerPay;
            ReturnMoney.GoldPay = totalMoney.GoldPay;
            ReturnMoney.EngPrice = EngAry == null ? 0 : EngAry.Last().EngPrice;
            return Json(ReturnMoney, JsonRequestBehavior.AllowGet);
        }
        public class EngMoney : Calculate_Funding.MoneyData
        {
            public int EngPrice { get; set; }
        }
        #endregion

        public JsonResult GetEngPrice(byte EngCode)
        {
            _MNo = (int)Session["MapNo"];
            return Json(gda.GetEngPriceByMapNo(_MNo, EngCode), JsonRequestBehavior.AllowGet);
        }
    }
}
