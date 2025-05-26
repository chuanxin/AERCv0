using Dry.Models;
using Dry.Models.CommonCls;
using Dry.Models.MaterialModules;
/*
-- =============================================
-- Author: HaoHsuan, WEI
-- Create date: 2014-4-15
-- Description: 計算經費
-- =============================================
*/
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
    public class FarmerSysController : Controller
    {
        
        private FarmerSysDBService farmersys_db = new FarmerSysDBService();
        private CommClass comm = new CommClass();
        private Calculate_Funding CalculateCls = new Calculate_Funding();
        private FarmerSysPriceService priceService = new FarmerSysPriceService();
        private GetData gd = new GetData();
        int MapNo = new int();
        short UnitID = new short();
        
        public ActionResult CreateFarmerSys()
        {
            if (Session["MapNo"] == null)
                return RedirectToAction("Index", "ApplyIndex");
            MapNo = (int)Session["MapNo"];
            UnitID = (short)Session["UnitID"];

            FarmerSysView ModelData = new FarmerSysView();

            ModelData.EndTypeDDL = farmersys_db.GetEndTypeList();
            //if(UnitID != 17)
            //{
                ModelData.EndTypeDDL.RemoveAt(4);
            //}
            ModelData.SecondEndTypeDDL = farmersys_db.GetEndTypeList(); 
            ModelData.FacTypeDDL = farmersys_db.GetFacTypeList();
            ModelData.SecondFacTypeDDL = farmersys_db.GetFacTypeList(); 
            ModelData.WaterSrcDDL = farmersys_db.GetWaterSrcList();
            ModelData.DropDDL = farmersys_db.GetDropList(); 
            ModelData.SecondDropDDL = farmersys_db.GetDropList(); 
            ModelData.SprayDDL = farmersys_db.GetSprayList(); 
            ModelData.SecondSprayDDL = farmersys_db.GetSprayList(); 
            ModelData.UnitDDL = comm.GetUnitList(MapNo);
            ModelData.StdSysDDL = comm.GetStdSysList(UnitID);
            ModelData.GroupDDL = comm.GetGroupDDL();
            ModelData.StdSysByAmountDDL = new List<SelectListItem>();
            ModelData.ddl_FarmerSysUnit = -1;
            ModelData.ddl_StdSys = -1;
            ModelData.pipDDL = farmersys_db.GetpipMatDDL(UnitID);
            ModelData.StdpipeHei = 0;
            ModelData.Step = gd.GetCaseDataFromMapNo(MapNo).Step; 
            //ModelData.IsModify = farmersys_db.CheckFarmSysData(MapNo); 
            ModelData.IsModify = ModelData.Step >= 5 /*3*/ ? true : false; 
            ModelData.ApplyUnit = gd.GetCaseDataFromMapNo(MapNo).ApplyUnit; 
            ModelData.L1QualityDDL = ModelData.L2QualityDDL = ModelData.BranchPipeMaterialDDL = ModelData.StdpipeMaterialDDL = comm.GetQuality(); 
            ModelData.L1SpecDDL = ModelData.L2SpecDDL = ModelData.BranchPipeSpecDDL = ModelData.StdpipeSpecDDL = comm.GetSpecDDL(); 
            ModelData.NozzleSpec = 0;
            ModelData.NozzleType = 0;
            ModelData.NozzleSpecDDL = comm.GetNozzleSpecByMatDDL(UnitID, Convert.ToInt32(ModelData.EndTypeDDL.FirstOrDefault().Value));
            //ModelData.L1Price = farmersys_db.GetL1PriceDDL(26, 1, UnitID, 105);
            if (ModelData.NozzleSpecDDL.Count > 0)
            {
                ModelData.NozzleMaterialDDL = comm.GetMatListByModelAndSpec(UnitID,
                    Convert.ToInt32(ModelData.EndTypeDDL.FirstOrDefault().Value),
                    Convert.ToInt32(ModelData.NozzleSpecDDL.FirstOrDefault().Value));
                ModelData.NozzleSpec = Convert.ToInt32(ModelData.NozzleSpecDDL.FirstOrDefault().Value);
                if(ModelData.NozzleMaterialDDL.Count > 0)
                {
                    ModelData.NozzleType = Convert.ToInt32(ModelData.NozzleMaterialDDL.FirstOrDefault().Value);
                }
            }

            //ModelData.Length = 100;
            ModelData.Length = (int)Math.Sqrt(new GetData().GetFarmBuildArea(MapNo));
            ModelData.width = gd.GetFarmBuildArea(MapNo) / ModelData.Length;
            
            ModelData.CaseMoney = CalculateCls.Remaining_funds(/*3*/5, MapNo, gd.GetCaseDataFromMapNo(MapNo));

            int BMno = ModelData.IsModify == true ? MapNo : gd.GetBeforeNowMapNo(MapNo);
            
            
            PigingConf pigConf = gd.GetPigingConfData(BMno);
            //List<EndType> endType = gd.GetEndTypeData(BMno);
            if (pigConf != null)
            {
                ModelData.L1Len = pigConf.L1;
                ModelData.L1Mat = gd.MatToId(pigConf.L1Mat == null ? null : pigConf.L1Mat);
                ModelData.L1Spec = gd.SpecToId(pigConf.L1Spec == null ? 0f : pigConf.L1Spec.Value);
                ModelData.L1Price = pigConf.L1Price;
                ModelData.L1MatAmt = pigConf.L1Amount;
                if (pigConf.L2 > 0 && pigConf.L2Price > 0 && pigConf.L2Amount > 0)
                {
                    ModelData.L2Len = (double)pigConf.L2;
                    ModelData.L2Mat = gd.MatToId(pigConf.L2Mat == null ? null : pigConf.L2Mat);
                    ModelData.L2Spec = gd.SpecToId(pigConf.L2Spec == null ? 0f : pigConf.L2Spec.Value);
                    ModelData.L2Price = pigConf.L2Price;
                    ModelData.L2MatAmt = (short)pigConf.L2Amount;
                }
                ModelData.BranchPipeMaterial = gd.MatToId(pigConf.EndType.FirstOrDefault().BranchPipeMat == null ? null : pigConf.EndType.FirstOrDefault().BranchPipeMat);
                ModelData.BranchPipeSpec = gd.SpecToId(pigConf.EndType.FirstOrDefault().BranchPipeSpec == null ? 0f : pigConf.EndType.FirstOrDefault().BranchPipeSpec.Value);
                ModelData.SS = pigConf.EndType.First().SS;
                ModelData.SL = pigConf.EndType.First().SL;
                ModelData.Block = pigConf.Cblock;
                ModelData.Length = Convert.ToInt32(pigConf.Cblock.Split('x')[0]);
                ModelData.width = Convert.ToInt32(pigConf.Cblock.Split('x')[1]);
                
                
                foreach (var item in ModelData.FacTypeDDL)
                {
                    if (item.Value == pigConf.EndType.First().FacType.ToString())
                        item.Selected = true;
                }
                foreach (var item in ModelData.EndTypeDDL)
                {
                    if (pigConf.EndType.First().EndTypeCode == 7 || pigConf.EndType.First().EndTypeCode == 8)
                    {
                        if (item.Value == "4")
                            item.Selected = true;
                        foreach (var itemDrop in ModelData.DropDDL)
                        {
                            if (itemDrop.Value == pigConf.EndType.First().EndTypeCode.ToString())
                                itemDrop.Selected = true;
                        }
                    }
                    else if (pigConf.EndType.First().EndTypeCode == 6)
                    {
                        if (item.Value == "2")
                            item.Selected = true;
                        foreach (var itemSpray in ModelData.SprayDDL)
                        {
                            if (itemSpray.Value == pigConf.EndType.First().EndTypeCode.ToString())
                                itemSpray.Selected = true;
                        }
                    }
                    else
                    {
                        if (item.Value == pigConf.EndType.First().EndTypeCode.ToString())
                            item.Selected = true;
                    }
                }
                
                ModelData.DisplayInfo = "none";
                ModelData.DisplayInfoAdd = "block";
                if (pigConf.EndType.Count > 1)
                {
                    ModelData.DisplayInfo = "block";
                    ModelData.DisplayInfoAdd = "none";
                    foreach (var item in ModelData.SecondFacTypeDDL)
                    {
                        if (item.Value == pigConf.EndType.Last().FacType.ToString())
                            item.Selected = true;
                    }
                    foreach (var item in ModelData.SecondEndTypeDDL)
                    {

                        if (pigConf.EndType.Last().EndTypeCode == 7 || pigConf.EndType.Last().EndTypeCode == 8)
                        {
                            if (item.Value == "4")
                                item.Selected = true;
                            foreach (var itemDrop in ModelData.SecondDropDDL)
                            {
                                if (itemDrop.Value == pigConf.EndType.Last().EndTypeCode.ToString())
                                    itemDrop.Selected = true;
                            }
                        }
                        else if (pigConf.EndType.Last().EndTypeCode == 6)
                        {
                            if (item.Value == "2")
                                item.Selected = true;
                            foreach (var itemSpray in ModelData.SecondSprayDDL)
                            {
                                if (itemSpray.Value == pigConf.EndType.Last().EndTypeCode.ToString())
                                    itemSpray.Selected = true;
                            }
                        }
                        else
                        {
                            if (item.Value == pigConf.EndType.Last().EndTypeCode.ToString())
                                item.Selected = true;
                        }
                    }
                    
                    ModelData.SecondSL = pigConf.EndType.Last().SL;
                    ModelData.SecondSS = pigConf.EndType.Last().SS;
                    ModelData.SecondStdpipeHei = pigConf.EndType.Last().StdpipeHei == null ? 0 : pigConf.EndType.Last().StdpipeHei.Value;
                }
                if (ModelData.EndTypeDDL.Any(m => m.Selected == true))
                {
                    if (ModelData.EndTypeDDL.Where(m => m.Selected == true).FirstOrDefault().Value == "2")
                    {
                        foreach (var item in ModelData.SprayDDL)
                        {
                            if (item.Value == pigConf.EndType.FirstOrDefault().EndTypeCode.ToString())
                            {
                                item.Selected = true;
                            }
                        }
                    }
                    else if (ModelData.EndTypeDDL.Where(m => m.Selected == true).FirstOrDefault().Value == "4")
                    {
                        foreach (var item in ModelData.DropDDL)
                        {
                            foreach (var subitem in ModelData.DropDDL)
                            {
                                if (subitem.Value == pigConf.EndType.FirstOrDefault().EndTypeCode.ToString())
                                {
                                    subitem.Selected = true;
                                }
                            }
                        }
                    }
                }
                ModelData.NozzleSpec = gd.SpecToId(pigConf.EndType.FirstOrDefault().SprinklerSpec == null ? 0f : pigConf.EndType.FirstOrDefault().SprinklerSpec.Value);
                if (ModelData.NozzleSpec != 0f)
                {
                    int Endtype = Convert.ToInt32(pigConf.EndType.FirstOrDefault().EndTypeCode);
                    int Mn = 1;
                    if(Endtype == 1)
                    {
                        Mn = 6;
                    }
                    else if (Endtype == 2)
                    {
                        Mn = 5;
                    }else if(Endtype == 3)
                    {
                        Mn = 8;
                    }
                    else if (Endtype == 7)
                    {
                        Mn = 9;
                    }
                    else if(Endtype==8)
                    {
                        Mn = 12;
                    }
                    UnitID = (short)Session["UnitID"];
                    ModelData.NozzleSpecDDL = comm.GetNozzleSpecByMatDDL(UnitID, Mn);

                    ModelData.NozzleMaterialDDL = comm.GetNozzleMatListBySpec(ModelData.NozzleSpec, Endtype, UnitID);
                    foreach (var item in ModelData.NozzleMaterialDDL)
                    {
                        if (item.Text == pigConf.EndType.FirstOrDefault().SprinklerType)
                            ModelData.NozzleType = Convert.ToInt32(item.Value);
                    }
                }

                

                foreach (var item in ModelData.WaterSrcDDL)
                {
                    if (item.Value == pigConf.IrrWCode.ToString())
                        item.Selected = true;
                        
                   
                }

                if (pigConf.EndType.First().StdpipeHei != null)
                {
                    ModelData.StdpipeHei = (float)pigConf.EndType.First().StdpipeHei;
                    ModelData.StdpipeMat = gd.MatToId(pigConf.EndType.FirstOrDefault().StdpipeMat == null ? null : pigConf.EndType.FirstOrDefault().StdpipeMat);
                    ModelData.StdpipeSpec = gd.SpecToId(pigConf.EndType.FirstOrDefault().StdpipeSpec == null ? 0f : pigConf.EndType.FirstOrDefault().StdpipeSpec.Value);
                }
                else
                {
                    ModelData.StdpipeHei = 1;
                }
            }
            else
            {
                
                ModelData.DisplayInfo = "none";
                ModelData.DisplayInfoAdd = "block";
                //////
                ModelData.L2Len = 0;
                ModelData.L2Mat = 1;
                ModelData.L2MatAmt = 0;
            }

            #region 讀入先前版本編號的農戶設施系統物料表
            
            Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct farmerSys = gd.GetFarmerSystemData(BMno);
            if (farmerSys != null)
            {
                ModelData.Mat = farmersys_db.GetFarmerSysMData(farmerSys.FarSysNo);
                
                ModelData.ddl_FarmerSysUnit = farmerSys.ApplyUnit;
                foreach (var items in ModelData.UnitDDL)
                {
                    if (items.Value == ModelData.ddl_FarmerSysUnit.ToString())
                        items.Selected = true;
                }

                
                ModelData.ddl_StdSys = farmerSys.FacNo;
                foreach (var itemp in ModelData.StdSysDDL)
                {
                    if (itemp.Value == farmerSys.FacNo.ToString())
                        itemp.Selected = true;
                }

                foreach (var item in ModelData.Mat)
                {
                    foreach(var subitem in item.List)
                        subitem.TotalPrice = (int)Math.Round(subitem.Amount * subitem.Price, 1);
                }
            }else
            {
                int caseaplyunit = gd.getCaseAppliedUnit(MapNo);
                ModelData.ddl_FarmerSysUnit = caseaplyunit;
                foreach (var items in ModelData.UnitDDL)
                {
                    if (items.Value == caseaplyunit.ToString())
                        items.Selected = true;
                }
            }
            #endregion
            
            
            ModelData.BuildArea = new GetData().GetFarmBuildArea(MapNo);
            ViewBag.Gold = gd.GetGoldFromMapno(MapNo);

            //ModelData.Length = (int)Math.Sqrt(ModelData.BuildArea);
            //ModelData.width = ModelData.BuildArea / ModelData.Length;

            return PartialView(ModelData);
        }

        public JsonResult GetNozzleTypeBySpec(int SpecNo, int EndType)
        {
            UnitID = (short)Session["UnitID"];
            List<SelectListItem> MatDDL = comm.GetNozzleMatListBySpec(SpecNo, EndType, Convert.ToInt16(Session["UnitID"]));
            return Json(MatDDL, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPerforatedPipeByEndType(int EndType)
        {
            UnitID = (short)Session["UnitID"];
            int ModuleNo = 0;
            if (EndType == 1)
            {
                ModuleNo = 6;
            }
            else if (EndType == 2 || EndType == 6)
            {
                ModuleNo = 5;
            }
            else if (EndType == 3)
            {
                ModuleNo = 8;
            }else if(EndType == 7)
            {
                ModuleNo = 9;
            }
            else if (EndType == 8)
            {
                ModuleNo = 12;
            }
            List<SelectListItem> MatDDL = comm.GetNozzleSpecByMatDDL(Convert.ToInt16(Session["UnitID"]), ModuleNo);
            return Json(MatDDL, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GETL1formView(int L1Mats, int L1Spec)
        {
            FarmerSysView L1num = new FarmerSysView();
            int years, units;
            System.DateTime currentTime = System.DateTime.Now;
            units = Convert.ToInt32(Session["UnitID"]);
            years = currentTime.Year - 1911;
            L1num.L1Price = farmersys_db.GetL1PriceDDL(L1Spec, L1Mats, units, years);
            L1num.L1Spec = farmersys_db.GetL1SpecDDL(L1Spec, L1Mats, units, years);
            
            MapNo = (int)Session["MapNo"];
            
            bool slope = gd.GetIs12FromMapNo(MapNo);
            if (slope == true) L1num.L1Price =(int)(L1num.L1Price * 1.2);
            return Json(L1num, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GETL2formView(int L2Mats, int L2Spec)
        {
            FarmerSysView L1num = new FarmerSysView();
            int years, units;
            System.DateTime currentTime = System.DateTime.Now;
            units = Convert.ToInt32(Session["UnitID"]);
            years = currentTime.Year - 1911;
            L1num.L2Price = farmersys_db.GetL1PriceDDL(L2Spec, L2Mats, units, years);
           
            MapNo = (int)Session["MapNo"];
            bool slope = gd.GetIs12FromMapNo(MapNo);
            if (slope == true) L1num.L2Price = (int)(L1num.L2Price * 1.2);
            return Json(L1num, JsonRequestBehavior.AllowGet);
        }

        #region Get Std MAT
        /// <summary>
        /// 公版事件-沒帶數量
        /// </summary>
        /// <param name="stdsys"></param>
        /// <returns></returns>
        public JsonResult GetStdSys(int stdsys)
        {
            UnitID = (short)Session["UnitID"];
            var retn_res = farmersys_db.GetStdMatList(stdsys, UnitID);

            List<SysMat> list = new List<SysMat>();
            MapNo = (int)Session["MapNo"];
            foreach (var item in retn_res)
            {
                short y = gd.GetCaseDataFromMapNo(MapNo).ApplyYear;
                int unt = 1;
                var matPriceData = farmersys_db.GetMatPrice(y, unt, item.POMNo);
                
                bool slope = gd.GetIs12FromMapNo(MapNo);
                double vmatprice = 0;
                if (slope == true) 
                {
                    vmatprice = matPriceData == null ? 0 : (int)(matPriceData.Price * 1.2);
                }else
                {
                    vmatprice = matPriceData == null ? 0 : matPriceData.Price;
                }
                list.Add(new SysMat()
                {
                    module = farmersys_db.GetModuleName(item.ModuleNo),
                    matname = item.MName,
                    pomno = item.POMNo.ToString(),
                    spec1 = item.Spec1,
                    spec2 = item.Spec2,
                    spec3 = item.Spec3,
                    itemunit = item.ItemUnit,
                    description = item.Note == null ? "" : item.Note,
                    matprice = vmatprice
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStdMatDDL(byte EndType)
        {
            short UnitID = (short)Session["UnitID"];
            FarmerSysView ModelData = new FarmerSysView();
            ModelData.StdSysByAmountDDL = comm.GetStdSysList(EndType, UnitID);
            return Json(ModelData.StdSysByAmountDDL, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 公版事件-有帶數量
        /// </summary>
        /// <param name="stdsys"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="height"></param>
        /// <param name="SL"></param>
        /// <param name="SS"></param>
        /// <returns></returns>
        public JsonResult GetStdSysByAmount(int stdsys, int width, int length, float height, int SL, int SS)
        {
            UnitID = (short)Session["UnitID"];
            var retn_res = farmersys_db.GetStdMatList(stdsys, UnitID);

            List<SysMat> list = new List<SysMat>();
            if (stdsys != -1)
            {
                MapNo = (int)Session["MapNo"];
                foreach (var item in retn_res)
                {
                    short y = gd.GetCaseDataFromMapNo(MapNo).ApplyYear; 
                    int unt = 1;
                    var matPriceData = farmersys_db.GetMatPrice(y, unt, item.POMNo);
                    
                    bool slope = gd.GetIs12FromMapNo(MapNo);
                    double vmatprice = 0;
                    if (slope == true)
                    {
                        vmatprice = matPriceData == null ? 0 : (int)(matPriceData.Price * 1.2);
                    }
                    else
                    {
                        vmatprice = matPriceData == null ? 0 : matPriceData.Price;
                    }
                    list.Add(new SysMat()
                    {
                        module = farmersys_db.GetModuleName(item.ModuleNo),
                        matname = item.MName,
                        pomno = item.POMNo.ToString(),
                        spec1 = item.Spec1,
                        spec2 = item.Spec2,
                        spec3 = item.Spec3,
                        itemunit = item.ItemUnit,
                        matamount = MatOfAmount(item.AmountFormula, width, length, height, SL, SS),
                        description = item.Note == null ? "" : item.Note,
                        //matprice = matPriceData == null ? 0 : matPriceData.Price
                        matprice = vmatprice
                    });
                }
            }
            

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 公版-新
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public JsonResult GetStdSysByCondition([System.Web.Http.FromBody] FarmerSysView.StdMaterialStruct Data)
        {
            MapNo = (int)Session["MapNo"];
            UnitID = (short)Session["UnitID"];
            List<StdSysMat> retn_res = new MaterialModule().GetStdMatList(Data, UnitID, MapNo);

            List<SysMat> list = new List<SysMat>();
            
            bool slope = gd.GetIs12FromMapNo(MapNo);
            foreach (var item in retn_res)
            {
                short y = gd.GetCaseDataFromMapNo(MapNo).ApplyYear;
                //int unt = 1;
                //var matPriceData = farmersys_db.GetMatPrice(y, unt, item.POMNo);
                if (slope == true) item.Price = (int)(item.Price * 1.2);

                list.Add(new SysMat()
                {
                    module = farmersys_db.GetModuleName(item.ModuleNo),
                    matname = item.MName,
                    pomno = item.POMNo.ToString(),
                    spec1 = item.Spec1,
                    spec2 = item.Spec2,
                    spec3 = item.Spec3,
                    itemunit = item.ItemUnit,
                    matamount = item.Amount,
                    description = item.Note == null ? "" : item.Note,
                    //matprice = matPriceData == null ? 0 : matPriceData.Price
                    matprice = item.Price
                });
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        
        //動態產生材料功能由js呼叫
        public JsonResult GetStdSysByConditionAddGroup([System.Web.Http.FromBody] FarmerSysView.StdMaterialStruct Data)
        {
            MapNo = (int)Session["MapNo"];
            UnitID = (short)Session["UnitID"];
            List<StdSysMat> retn_res = new MaterialModule().GetStdMatList(Data, UnitID, MapNo);
            List<MatGroup> groups = new DryEntities().MatGroup.ToList();
            List<MatList> list = new List<MatList>();
            
            //bool slope = gd.GetIs12FromMapNo(MapNo); 

            
            foreach (var groupitem in retn_res.GroupBy(m => m.Group).Select(o => new { Grouop = o.Key}))
            {
                MatList matlist = new MatList();
                matlist.GroupNo = groupitem.Grouop;
                matlist.GroupName = groups.Find(m=>m.GroupID == groupitem.Grouop).GroupCNS;
                List<SysMat> mlist = new List<SysMat>();
                int OrderNo = 1;
                foreach (var subitem in retn_res.Where(m => m.Group == groupitem.Grouop))
                {
                    SysMat mat = new SysMat();
                    short y = gd.GetCaseDataFromMapNo(MapNo).ApplyYear;
                    mat.module = farmersys_db.GetModuleName(subitem.ModuleNo);
                    mat.matname = subitem.MName;
                    mat.pomno = subitem.POMNo.ToString();
                    mat.spec1 = subitem.Spec1;
                    mat.spec2 = subitem.Spec2;
                    mat.spec3 = subitem.Spec3;
                    mat.itemunit = subitem.ItemUnit;
                    mat.matamount = subitem.Amount;
                    mat.description = subitem.Note == null ? "" : subitem.Note;
                    //if (slope == true) subitem.Price = (int)(subitem.Price * 1.2);
                    mat.matprice = subitem.Price;
                    mat.mattype = subitem.MatType;
                    mat.order = OrderNo;
                    
                    mat.group = subitem.Group;
                    mlist.Add(mat);
                    matlist.List = mlist;
                    OrderNo++;
                }
                list.Add(matlist);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 計算材料數量
        /// </summary>
        /// <param name="FormulaType"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="height"></param>
        /// <param name="SL"></param>
        /// <param name="SS"></param>
        /// <returns></returns>
        private int MatOfAmount(int FormulaType,int width,int length,float height,int SL,int SS)
        {
            
            if (SL <= 0 && SS <= 0)
                return 0;
            int BN = (int)(length / SL);
            int SN = (int)(width / SS);
            float Fh = (float)0.5;
            switch(FormulaType)
            {
                case 1:
                    return (int)((BN * width) / 100);
                case 2:
                    return BN;
                case 3:
                    return BN * 2;
                case 4:
                    return 1;
                case 5:
                    return 3;
                case 6:
                    return (int)((BN * width) / 4);
                case 7:
                    return (int)(((float)BN * (float)SN * height) / 4);
                case 8:
                    return BN * SN;
                case 9:
                    return BN * SN * 2;
                case 10:
                    return (int)((BN * SN * Fh) / 6);
                default:
                    return 0;
            }
        }
        #endregion

        #region MAT Struct
        public class MatList
        {
            public string GroupName { get; set; }
            public int GroupNo { get; set; }
            public List<SysMat> List { get; set; }
        }
        public class SysMat
        {
            public string module { get; set; }
            public string pomno { get; set; }
            public string matname { get; set; }
            public string mattype { get; set; }
            public string spec1 { get; set; }
            public string spec2 { get; set; }
            public string spec3 { get; set; }
            public string itemunit { get; set; }
            public double matprice { get; set; }
            public int matamount { get; set; } 
            public string description { get; set; }
            public int order { get; set; }
            public int group { get; set; }
            
        }
        public List<Dry.Models.ViewModel.SysMat> GetMatList(short UnitID)
        {

            List<Dry.Models.ViewModel.SysMat> MatList = farmersys_db.GetFacSysMAT(UnitID);
            return MatList;
        }
        public JsonResult GetData(string query)
        {
            short UnitID = (short)Session["UnitID"];
            List<Dry.Models.ViewModel.SysMat> list = new List<Models.ViewModel.SysMat>();
            var retn_res = GetMatList(UnitID).Where(m => m.matname.ToLower().Contains(query.ToLower()) || m.pomno == query).ToList().OrderBy(m => m.matname);
            return Json(retn_res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDataByPomno(int pomno)
        {
            UnitID = (short)Session["UnitID"];
            var MatData = farmersys_db.GetMATbyPmno(pomno, UnitID);
            if (MatData == null)
                return null;
            else
            {
                MapNo = (int)Session["MapNo"];
                Case casedata = gd.GetCaseDataFromMapNo(MapNo);
                short y = casedata.ApplyYear;// Convert.ToInt16(DateTime.Now.Year - 1911 - 1);
                int unt = casedata.ApplyUnit;
                var matPriceData = farmersys_db.GetMatPrice(y, unt, pomno);//
                double price = 0;
               
                bool slope = gd.GetIs12FromMapNo(MapNo);

                if (matPriceData != null) 
                {
                    if (slope == true) 
                    {
                        price = (int)(matPriceData.Price * 1.2);
                    }
                    else 
                    {
                        price = matPriceData.Price;
                    }
                }
                    

                SysMat MatjsData = new SysMat()
                {
                    module = MatData.module,
                    matname = MatData.matname,
                    mattype = MatData.mattype,
                    pomno = MatData.pomno,
                    spec1 = MatData.spec1,
                    spec2 = MatData.spec2,
                    spec3 = MatData.spec3,
                    itemunit = MatData.itemunit,
                    matprice = price,
                    description = MatData.description
                };
                return Json(MatjsData, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 取得範本資料
        public JsonResult GetFsys(string query)
        {
            Case cse = gd.GetCaseDataFromMapNo(MapNo);
            List<GetData.FSysDs> list = farmersys_db.GetFarSysInfo((byte)cse.ApplyUnit, cse.ApplyYear);
            var retn_res = list.Where(m => m.ianum.StartsWith(query) || m.farsys.ToLower() == query.ToLower() || m.farname.ToLower().Contains(query.ToLower())).ToList().OrderBy(m => m.ianum);
            return Json(retn_res, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult GetTotalPrice([System.Web.Http.FromBody] FarmerSysView.ParaJsonData Data)
        {
            int[] Price;
            MapNo = (int)Session["MapNo"];
            if (new IsGoldService().IsGold(MapNo)) 
                //Price = priceService.GetGoldHelpTotalPrice(Data, MapNo);
                
                Price = priceService.GetHelpTotalPrice(Data, MapNo);
            else 
                Price = priceService.GetHelpTotalPrice(Data, MapNo);
            Case casedata = gd.GetCaseDataFromMapNo(MapNo);
            int CaseM = CalculateCls.Remaining_funds(/*3*/5, MapNo, casedata);
            string contentStr = "";
            //contentStr += (CaseM - Price[1]).ToString() + ";";
            for (int i = 0; i < Price.Length; i++)
            {
                contentStr += Price[i].ToString();
                if (i < Price.Length - 1)
                    contentStr += ";";
            }
            return Content(contentStr);
        }

        

        
        public ActionResult SaveData([System.Web.Http.FromBody] FarmerSysView.ParaJsonData Data)
        {
            DBCommon.DbEvent dbMsg = new DBCommon.DbEvent();
            if (ModelState.IsValid)
            {
                MapNo = (int)Session["MapNo"];
                dbMsg = farmersys_db.SaveData(Data, MapNo);
            }
            else
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(k => k.Key, k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                dbMsg.DbMessage = "Fail";
            }
            return Content(dbMsg.DbMessage);
        }
        /// <summary>
        /// 略過目前步驟
        /// </summary>
        /// <param name="step">目前的步驟</param>
        /// <returns></returns>
        public ActionResult PassData()
        {
            MapNo = (int)Session["MapNo"];
            return Json(new CommClass().UpdateCase(MapNo, 5 /*3*/).DbMessage,JsonRequestBehavior.AllowGet);
        }
    }
}
