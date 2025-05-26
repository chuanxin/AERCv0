/*
-- =============================================
-- Author: WEI,HaoHsuan
-- Create date: 2014-2-10
-- Description: 調控設施的Controller
-- =============================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dry.Models.Service;
using Dry.Models.ViewModel;
using Dry.Models;
using Dry.Models.CommonCls;
using System.Web.Http;
using Newtonsoft.Json;

namespace Dry.Controllers
{
    [System.Web.Mvc.Authorize][SessionCheck]
    public class CtrlController : Controller
    {
        private CtrlDBService DBService = new CtrlDBService();
        private CommClass comm = new CommClass();
        private GetData getData = new GetData();

        CtrlView ctrlView = new CtrlView();
        private Calculate_Funding CalculateCls = new Calculate_Funding();
        DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
        int MapNo = new int();
        public ActionResult Index()
        {
            //Session["MapNo"] = 68;
            if (Session["MapNo"] == null)
                return RedirectToAction("Index", "ApplyIndex");
            MapNo = Convert.ToInt32(Session["MapNo"]);
            int BMno; 
            ctrlView.Step = getData.GetCaseDataFromMapNo(MapNo).Step;
            ctrlView.IsModify = ctrlView.Step >= 6 ? true : false;
            ctrlView.CaseMoney = CalculateCls.Remaining_funds(6, MapNo, getData.GetCaseDataFromMapNo(MapNo));
            if (getData.chgDesign(MapNo))
            {
                //return Redirect("../Ctrl/chgCtrl");
                
                BMno = ctrlView.IsModify == true ? MapNo : getData.GetBeforeNowMapNo(MapNo);
                
            }
            else
            {
                BMno = MapNo;
            }
            DataList(BMno, ctrlView.CaseMoney,MapNo);
            ViewBag.Gold = getData.GetGoldFromMapno(BMno);
            return PartialView(ctrlView);            
        }
        
        //public ActionResult chgCtrl()
        //{
        //    //Session["MapNo"] = 68; 
        //    MapNo = (int)Session["MapNo"];
        //    int BMno = getData.GetBeforeNowMapNo(MapNo);//get previous mapping version No
        //    ctrlView.CaseMoney = CalculateCls.Remaining_funds(6, MapNo, getData.GetCaseDataFromMapNo(MapNo));
        //    /*
        //    ///
        //    
        //    ctrlView.ListDataList = DBService.GetCntrListDataList();            
        //    
        //    ctrlView.MatData = DBService.GetCntrlMatDataList(BMno);
        //    CtrlPriceService ctrlPrice = new CtrlPriceService();
        //    int[] Price = ctrlPrice.GetCtrlHelpTotalPrice(ctrlView.MatData.ToArray(), BMno);
        //    ctrlView.PayMoney = Price[1]; 
        //    ctrlView.CtrlTotalPrice = Price[0]; 
        //    ctrlView.CtrlSelfPrice = Price[2]; 
        //    ctrlView.FacMoney = DBService.GetFacMoney(BMno);      
        //    ViewBag.List = comm.GetCntrlList();
        //    //取得輔助單位代碼
        //    ctrlView.ApplyUnit = getData.GetApplyUnitId(BMno);
        //    //補助單位DropDownList
        //    List<SelectListItem> list = comm.GetUnitList();
        //    foreach (var item in list)
        //    {
        //        if (item.Value == ctrlView.ApplyUnit.ToString())
        //            item.Selected = true;
        //    }
        //    ViewBag.UnitList = list;

        //    if (comm.IsGold(MapNo))
        //        ctrlView.GoldMessage = "黃金廊道輔助專案";
        //    else
        //        ctrlView.GoldMessage = "";
        //    ctrlView.ListDataList = DBService.GetCntrListDataList();
        //    */
        //    //get previous mapping version ctrl mat list.
        //    DataList(BMno, ctrlView.CaseMoney);
        //    return View(ctrlView);
        //}

        public ActionResult CreateData([FromBody] JsonData ResultArry)
        {
            MapNo = Convert.ToInt32(Session["MapNo"]);
            if (ModelState.IsValid)
                dbMsg = DBService.CreateData(ResultArry, MapNo);
            else {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                 .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                dbMsg.DbMessage = "Fail";
            }
                
            return Content(dbMsg.DbMessage);
        }
        /// <summary>
        /// 略過此步驟
        /// </summary>
        /// <returns></returns>
        public ActionResult PassData()
        {
            MapNo = Convert.ToInt32(Session["MapNo"]);
            return Json(new CommClass().UpdateCase(MapNo, 6).DbMessage, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TotalPrice([FromBody] CtrlTableView[] Data)
        {
            //int[] TotalPrice = new int[3];
            int[] TotalPrice = new int[4];
            CtrlPriceService CtrlPrice = new CtrlPriceService();
            MapNo = Convert.ToInt32(Session["MapNo"]);
            
            //if (getData.GetGoldFromMapno(MapNo) == true)
            //{
                
            //    TotalPrice = CtrlPrice.GetCtrlGoldHelpTotalPrice(Data, MapNo);//
            //    return Content(TotalPrice[0].ToString() + ";" + TotalPrice[1].ToString() + ";" + TotalPrice[2].ToString() + ";" + TotalPrice[3].ToString());
            //}
            //else
            //{
                TotalPrice = CtrlPrice.GetCtrlHelpTotalPrice(Data, MapNo);//
                return Content(TotalPrice[0].ToString() + ";" + TotalPrice[1].ToString() + ";" + TotalPrice[2].ToString());
            //}
            
        }

        public ActionResult DataList(int MapNo, int CaseMoney,int MapNoV0)
        {
            
            ctrlView.ListDataList = DBService.GetCntrListDataList();
            
            //ctrlView.UnitName = getData.GetUnitName(getData.GetApplyUnitId(MapNo));

           
            ctrlView.MatData = DBService.GetCntrlMatDataList(MapNo);
            CtrlPriceService ctrlPrice = new CtrlPriceService();
            
            //if (getData.GetGoldFromMapno(MapNo) == true)
            //{
            //    int[] Price = ctrlPrice.GetCtrlGoldHelpTotalPrice(ctrlView.MatData.ToArray(), MapNo);
            //    ctrlView.PayMoney = Price[1]; 
            //    ctrlView.CtrlTotalPrice = Price[0]; 
            //    ctrlView.CtrlSelfPrice = Price[3]; 
            //    ctrlView.CtrlGoldPrice = Price[2];
            //    ctrlView.FacMoney = DBService.GetFacMoney(MapNo);
            //} else
            //{
            //int[] Price = ctrlPrice.GetCtrlHelpTotalPrice(ctrlView.MatData.ToArray(), MapNo);
            //ctrlView.PayMoney = Price[1];
            //ctrlView.CtrlTotalPrice = Price[0]; 
            //ctrlView.CtrlSelfPrice = Price[2]; 
            //ctrlView.FacMoney = DBService.GetFacMoney(MapNo);
            //}
            int[] Price;
            if (MapNo != MapNoV0)
            {
                Price = ctrlPrice.GetCtrlHelpTotalPrice(ctrlView.MatData.ToArray(), MapNoV0);
            }else  Price = ctrlPrice.GetCtrlHelpTotalPrice(ctrlView.MatData.ToArray(), MapNo);
            

            ctrlView.CtrlTotalPrice = Price[0]; 
            
            ctrlView.PayMoney = Price[1]; 
            ctrlView.CtrlSelfPrice = Price[2]; 
            ctrlView.FacMoney = DBService.GetFacMoney(MapNo);
            EditTable(MapNo);
            return View();
            //return PartialView();
        }

        public ActionResult EditTable(int MapNo)
        {
            //ctrlView.MatData = new List<CtrlTableView>();
           
            ViewBag.List = comm.GetCntrlList();
            
            ctrlView.ApplyUnit = getData.GetApplyUnitId(MapNo);
            if (ctrlView.ApplyUnit == -1)
            {
                ctrlView.ApplyUnit = (short)getData.getCaseAppliedUnit(MapNo);
            }

            
            List<SelectListItem> list = comm.GetUnitList(MapNo);
            foreach (var item in list)
            {
                if (item.Value == ctrlView.ApplyUnit.ToString())
                    item.Selected = true;
            }
            ViewBag.UnitList = list;

            if (comm.IsGold(MapNo))
                ctrlView.GoldMessage = "黃金廊道輔助專案";
            else
                ctrlView.GoldMessage = "";
            
            ctrlView.ListDataList = DBService.GetCntrListDataList();

           
            //if (ctrlView.ListDataList.Count == 0)
            //{
            //    ctrlView.ApplyUnit = (short)getData.getCaseAppliedUnit(MapNo);
            //}

            
            return View();
            //return PartialView();
        }

        public ActionResult ItemPrices(string itemName, int itemCount, int itemPrice)
        {
            CtrlPriceService PriceService = new CtrlPriceService();

            MapNo = Convert.ToInt32(Session["MapNo"]);
            int[] result = PriceService.GetItemPrice(itemName, itemCount, itemPrice, MapNo);
            //return Content(result[0].ToString() + ";" + result[1].ToString());
            return Json(result[0].ToString() + ";" + result[1].ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}
