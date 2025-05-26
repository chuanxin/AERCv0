/*
-- =============================================
-- Author: WEI,HaoHsuan
-- Create date: 2014-6-24
-- Description: 田間管路的資料庫操作類別
-- =============================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Dry.Models.ViewModel;
using System.Transactions;
using Dry.Models.CommonCls;

namespace Dry.Models.Service
{
    public class FarmerSysDBService
    {
        private DryEntities DryDB = new DryEntities();
        private DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
        private GetData getData = new GetData();
        private CommClass comm = new CommClass();

        #region 取得 主管的價格

        public double GetL1PriceDDL (int L1spec, int L1MatType, int unit,int year)
        {
            FarmerSysView L1Price = new FarmerSysView();

            IEnumerable<double> Lprice = (from FacSysMATs in DryDB.FacSysMAT
                               join PriceOfMats in DryDB.PriceOfMat on FacSysMATs.POMNo equals PriceOfMats.POMNo
                               where (FacSysMATs.SpecNo1 == L1spec) && (FacSysMATs.Bunit == unit) && (PriceOfMats.BYear == year) && (FacSysMATs.MatType == L1MatType) && (FacSysMATs.ModuleNo == 1)
                               select PriceOfMats.Price);
            L1Price.L1Price = Lprice.FirstOrDefault();

            return L1Price.L1Price;
        }

        #endregion
        #region 取得主管單位長度
        public int GetL1SpecDDL(int L1spec, int L1MatType, int unit, int year)
        {
            FarmerSysView L1Price = new FarmerSysView();

            var Lprice = (from FacSysMATs in DryDB.FacSysMAT
                              //join PriceOfMats in DryDB.PriceOfMat on FacSysMATs.POMNo equals PriceOfMats.POMNo
                          where (FacSysMATs.SpecNo1 == L1spec)
                          && (FacSysMATs.Bunit == unit) /*&& (PriceOfMats.BYear == year)*/ && (FacSysMATs.MatType == L1MatType) && (FacSysMATs.ModuleNo == 1)
                          select new { FacSysMATs.SpecLength }).FirstOrDefault();
            if (Lprice == null)
            {
                return 0;
            }
            else
            {
                L1Price.L1Spec = (int)Lprice.SpecLength;

                return L1Price.L1Spec;
            }
            
        }
        #endregion

        #region 取得 末端型式 DropDownList
        public List<SelectListItem> GetEndTypeList()
        {
            List<SelectListItem> item = new List<SelectListItem>();

            var EndList = DryDB.EndTypeList.Where(m=>m.EndType >=1 && m.EndType <= 5).ToList();

            foreach (var end in EndList)
            {
                item.Add(new SelectListItem()
                {
                    Text = end.EndTypeCNS,
                    Value = end.EndType.ToString()
                });
            }
            return item;
        }
        #endregion

        #region 取得 施工方式(地表/理設/棚架) DropDownList
        public List<SelectListItem> GetFacTypeList()
        {
            List<SelectListItem> item = new List<SelectListItem>();

            var FacTypeList = DryDB.FacTypeList.ToList();

            foreach (var factype in FacTypeList)
            {
                item.Add(new SelectListItem()
                {
                    Text = factype.FTpeCNS,
                    Value = factype.FacType.ToString()
                });
            }
            return item;
        }
        #endregion

        #region 取得 主管物料 DropDownList
        public List<SelectListItem> GetpipMatDDL(short Bunit)
        {
            List<SelectListItem> item = new List<SelectListItem>();

            var pipMatList = DryDB.FacSysMAT.Where(fsm => fsm.MName == "PVC塑膠管" && fsm.Bunit == Bunit);

            foreach (var matitem in pipMatList)
            {
                item.Add(new SelectListItem()
                {
                    Text = matitem.MName + "  " + matitem.Spec,
                    Value = matitem.POMNo.ToString()
                });
            }
            return item;
        }
        #endregion

        #region 取得 灌溉水源 DropDownList
        public List<SelectListItem> GetWaterSrcList()
        {
            List<SelectListItem> item = new List<SelectListItem>();

            var WaterSrcList = DryDB.WaterSrcList.ToList();

            foreach (var factype in WaterSrcList)
            {
                item.Add(new SelectListItem()
                {
                    Text = factype.WsCNS,
                    Value = factype.WsCode.ToString()
                });
            }

            //item = Enumerable.Repeat(2).ToString();
            return item;
        }
        #endregion

        #region 取得 滴灌系統 DropDownList
        /// <summary>
        /// 滴灌系統類別
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetDropList()
        {
            List<SelectListItem> item = new List<SelectListItem>();

            //var WaterSrcList = DryDB.WaterSrcList.ToList();
            item.Add(new SelectListItem() { Text = "滴嘴滴灌系統", Value = "7" });
            item.Add(new SelectListItem() { Text = "滴水管滴灌系統", Value = "8" });
            return item;
        }
        #endregion

        #region 取得 噴頭式系統 DropDownList
        /// <summary>
        /// 噴頭式系統類別
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetSprayList()
        {
            List<SelectListItem> item = new List<SelectListItem>();

            //var WaterSrcList = DryDB.WaterSrcList.ToList();
            item.Add(new SelectListItem() { Text = "一般", Value = "2" });
            item.Add(new SelectListItem() { Text = "高壓大型噴頭系統", Value = "6" });
            return item;
        }
        #endregion

        #region 取得 農戶設施系統物料表
        /// <summary>
        /// 取得農戶設施系統物料表
        /// </summary>
        /// <param name="FarSysNo"></param>
        /// <returns></returns>
        public List<FarmerSysView.MatList> GetFarmerSysMData(string FarSysNo)
        {
            //var farmersys = DryDB.FarmerSys.Where(p => p.FarSysNo == FarSysNo).ToList();
            var farmerSysM = DryDB.FarmerSysM.Where(p => p.FarSysNo == FarSysNo).ToList();
            //var facSysMat = DryDB.FacSysMAT.ToList();
            var matModule = DryDB.Mat_Module.ToList();
            //var Result = (from FarmerSysm in farmerSysM
            //              join Farmersys in farmersys on FarmerSysm.FarSysNo equals Farmersys.FarSysNo
            //              join FacSysmat in facSysMat on FarmerSysm.POMNo equals FacSysmat.POMNo
            //              join Matmodule in matModule on FacSysmat.ModuleNo equals Matmodule.ModuleNo
            //              select new
            //              {
            //                  Farmersys.ApplyUnit,
            //                  Farmersys.FacNo,
            //                  FarmerSysm.No,
            //                  FarmerSysm.FarSysNo,
            //                  FarmerSysm.POMNo,
            //                  FacSysmat.MName,
            //                  Matmodule.ModuleCNS,
            //                  FacSysmat.SpecNo1,
            //                  FacSysmat.SpecNo2,
            //                  FacSysmat.SpecNo3,
            //                  FacSysmat.ItemUnit,
            //                  FacSysmat.Note,
            //                  FarmerSysm.Amount
            //              });            
            List<MatGroup> groups = new DryEntities().MatGroup.ToList();
            List<FarmerSysView.MatList> list = new List<FarmerSysView.MatList>();
            
            int mno = DryDB.FarmerSys.Single(fs => fs.FarSysNo == FarSysNo).MapNo;
            Case cse = getData.GetCaseDataFromMapNo(mno);
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            short ApplyUnit = cse.ApplyUnit;
            int main_index = 1;
            int sub_index = 1;
            foreach (var listitem in farmerSysM.GroupBy(m => m.MatGroupID).Select(o => new { Grouop = o.Key }))
            {
                FarmerSysView.MatList matlist = new FarmerSysView.MatList();
                matlist.GroupName = main_index + ". " + groups.Find(m => m.GroupID == (listitem.Grouop == null ? 1 : listitem.Grouop.Value)).GroupCNS;
                matlist.GroupNo = listitem.Grouop == null ? 1 : listitem.Grouop.Value;
                List<FarmerSysView.FarmerSysMat> farmerSys = new List<FarmerSysView.FarmerSysMat>();
                matlist.List = new List<FarmerSysView.FarmerSysMat>();
                sub_index = 1;
                foreach (var item in farmerSysM.Where(m=>m.MatGroupID == listitem.Grouop))
                {
                    //var matPriceData = GetMatPrice((short)(DateTime.Now.Year - 1912), 1, item.POMNo);
                    //float matPrice = (float)getData.GetMatPrice(item.POMNo, cse.ApplyYear, cse.ApplyUnit);
                    matlist.List.Add(new FarmerSysView.FarmerSysMat()
                    {
                        ApplyUnit = ApplyUnit,
                        FacNo = 0,
                        No = item.No,
                        FarSysNo = item.FarSysNo,
                        POMNo = item.POMNo,
                        MatGroup = item.MatGroupID == null ? 1 : item.MatGroupID.Value,
                        MatOrder = item.MatOrder == null ? sub_index : item.MatOrder.Value,
                        //MatOrderCNS = (item.MatGroupID != null ? item.MatGroupID.Value.ToString() : "1") + "-" + (item.MatOrder != null ? item.MatOrder.Value.ToString() : index.ToString()),
                        MatOrderCNS = main_index + "-" + sub_index,
                        MName = item.MName,
                        ModuleCNS = item.ModuleCNS,
                        
                        Spec1 = item.SpecName1 == null ? "" : item.SpecName1,
                        Spec2 = item.SpecName2 == null ? "" : item.SpecName2,
                        Spec3 = item.SpecName3 == null ? "" : item.SpecName3,
                        ItemUnit = item.ItemUnit,
                        Note = item.Note,
                        Price = (float)item.SysPrice,//(float)matPriceData.Price,
                        Amount = item.Amount,
                        TotalPrice = item.TotalPrice == null ? 0 : item.TotalPrice.Value
                    });
                    sub_index++;
                }
                list.Add(matlist);
                main_index++;
            }

            return list;
        }
    
        #endregion

        #region 取得 材料表
        public List<SysMat> GetFacSysMAT(short UnitID)
        {
            List<SysMat> result = new List<SysMat>();
            var data = (from sysMat in DryDB.FacSysMAT
                        join module in DryDB.Mat_Module on sysMat.ModuleNo equals module.ModuleNo
                        where sysMat.Bunit == UnitID
                        select new
                        {
                            module.ModuleCNS,
                            sysMat.POMNo,
                            sysMat.MName,
                            sysMat.MatType,
                            sysMat.SpecNo1,
                            sysMat.SpecNo2,
                            sysMat.SpecNo3,
                            sysMat.ItemUnit,
                            sysMat.Note,

                        });
            List<MAT_Spec> spec = DryDB.MAT_Spec.ToList();
            List<MAT_Types> types = DryDB.MAT_Types.ToList();
            foreach(var item in data)
            {
                result.Add(new SysMat
                {
                    module = item.ModuleCNS,
                    pomno = item.POMNo.ToString(),
                    matname = item.MName,
                    mattype = types.Find(m=>m.TypeNo == item.MatType).TypeName,
                    spec1 = item.SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == item.SpecNo1.Value).SpecName,
                    spec2 = item.SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == item.SpecNo2.Value).SpecName,
                    spec3 = item.SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == item.SpecNo3.Value).SpecName,
                    itemunit = item.ItemUnit,
                    description = item.Note
                });
            }
            return result;
        }
        #endregion

        #region 取得 依特定字串材料表
        public IEnumerable GetFacSysMATByTerm(string term)
        {
            int length = 10;

            var Mat = DryDB.FacSysMAT.Where(m => m.POMNo.ToString().StartsWith(term))
                 .OrderByDescending(m => m.POMNo).Take(length)
                 .Select(m => new { Value = m.POMNo, Name = m.MName + "-" + m.Spec }).AsEnumerable();
            return Mat;
        }
        #endregion

        #region 取得 材料依物料代碼
        /// <summary>
        /// 取得 材料依物料代碼
        /// </summary>
        /// <param name="pomno">系統物料代碼</param>
        /// <param name="UnitID">水利會代碼</param>
        /// <returns></returns>
        public Dry.Models.ViewModel.SysMat GetMATbyPmno(int pomno, short UnitID)
        {
            if (DryDB.FacSysMAT.Any(m => m.POMNo == pomno))
            {
                FacSysMAT MatData = DryDB.FacSysMAT.Single(m => m.POMNo == pomno);
                List<Dry.Models.MAT_Spec> spec = DryDB.MAT_Spec.ToList();
                List<MAT_Types> mattype = DryDB.MAT_Types.ToList();
                Dry.Models.ViewModel.SysMat MatjsData = new Dry.Models.ViewModel.SysMat()
                {
                    module = GetModuleName(MatData.ModuleNo),
                    matname = MatData.MName,
                    pomno = MatData.POMNo.ToString(),
                    mattype = MatData.MatType == null ? "" : mattype.Find(m => m.TypeNo == MatData.MatType).TypeName,
                    spec1 = MatData.SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == MatData.SpecNo1.Value).SpecName,
                    spec2 = MatData.SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == MatData.SpecNo2.Value).SpecName,
                    spec3 = MatData.SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == MatData.SpecNo3.Value).SpecName,
                    itemunit = MatData.ItemUnit,
                    description = MatData.Note == null ? "" : MatData.Note
                };
                return MatjsData;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 取得 材料價格
        public PriceOfMat GetMatPrice(short year, int unit, int pomno)
        {
            if (DryDB.PriceOfMat.Any(pom => pom.POMNo == pomno && pom.BYear == year))
            {
                List<PriceOfMat> PriceOfMatList = DryDB.PriceOfMat.Where(pom => pom.POMNo == pomno && pom.BYear == year).ToList();
                return PriceOfMatList[0];
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 取得 物料模組的對應名稱
        public string GetModuleName(int moduleNo)
        {
            return DryDB.Mat_Module.Single(m => m.ModuleNo == moduleNo).ModuleCNS;
        }
        #endregion

        

        #region 取得 公版系統物料
        /// <summary>
        /// 舊版
        /// </summary>
        /// <param name="std"></param>
        /// <param name="Bunit"></param>
        /// <returns></returns>
        public List<StdSysMat> GetStdMatList(int std, short Bunit)
        {
            if (DryDB.StdFacSysMAT.Any(spom => spom.FacNo == std))
            {
                
                var matlist = from StdFacSysMat in DryDB.StdFacSysMAT
                              join FacSysMat in DryDB.FacSysMAT
                              on StdFacSysMat.POMNo equals FacSysMat.POMNo
                              where StdFacSysMat.FacNo == std && FacSysMat.Bunit == Bunit
                              select new
                              {
                                  FacSysMat.POMNo,
                                  FacSysMat.ModuleNo,
                                  FacSysMat.Bunit,
                                  FacSysMat.MName,
                                  FacSysMat.Spec,
                                  FacSysMat.SpecNo1,
                                  FacSysMat.SpecNo2,
                                  FacSysMat.SpecNo3,
                                  FacSysMat.ItemUnit,
                                  FacSysMat.Note,
                                  StdFacSysMat.AmountFormula
                              };
                List<Dry.Models.MAT_Spec> spec = DryDB.MAT_Spec.ToList();
                List<StdSysMat> matAry = new List<StdSysMat>();
                foreach(var item in matlist)
                {
                    matAry.Add(new StdSysMat
                    {
                        POMNo = item.POMNo,
                        ModuleNo = item.ModuleNo,
                        Bunit = item.Bunit,
                        MName = item.MName,
                        Spec1 = item.SpecNo1 == null ? "" : spec.Find(m => m.SpecNo == item.SpecNo1.Value).SpecName,
                        Spec2 = item.SpecNo2 == null ? "" : spec.Find(m => m.SpecNo == item.SpecNo2.Value).SpecName,
                        Spec3 = item.SpecNo3 == null ? "" : spec.Find(m => m.SpecNo == item.SpecNo3.Value).SpecName,
                        ItemUnit = item.ItemUnit,
                        Note = item.Note,
                        AmountFormula = item.AmountFormula == null ? 0 : item.AmountFormula.Value
                    });
                }
                #region OldCode
                //var matlist = DryDB.StdFacSysMAT.Where(spom => spom.FacNo == std).ToList();
                //foreach(var mat in matlist)
                //{
                //    matAry.Add(DryDB.FacSysMAT.Single(fmat => fmat.POMNo == mat.POMNo));
                //}
                #endregion
                
                return matAry;
            }
            else
            {//滴灌系統
                return null;
            }
        }

        /// <summary>
        /// 新版
        /// </summary>
        /// <returns></returns>
        public List<StdSysMat> GetStdMatListNew(FarmerSysView.StdMaterialStruct Data, short Unit)
        {
            MaterialModules.BranchPipeline MaterialModule = new MaterialModules.BranchPipeline();
            List<StdSysMat> MatList = new List<StdSysMat>();
            List<FacSysMAT> FacSysMat = DryDB.FacSysMAT.Where(m => m.Bunit == Unit).ToList();

            MatList.Add(MaterialModule.GetBranchPipeline(FacSysMat, Data)); 

            return MatList;
        }
        #endregion

        #region 取得 農戶之範本系統物料
        public List<Dry.Models.CommonCls.GetData.FSysDs> GetFarSysInfo(byte ApplyUnit, short ApplyYear = 0)
        {
            if (ApplyYear == 0)
            {
                ApplyYear = (short)(DateTime.Now.Year - 1911);
            }
            return getData.GetFarSys(ApplyUnit, ApplyYear);
        }
        #endregion

        //資料庫事件處理

        #region 將田間管路系統資料寫入資料庫
        /// <summary>
        /// 將田間管路系統資料寫入資料庫
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public DBCommon.DbEvent SaveData(FarmerSysView.ParaJsonData Data, int MapNo)
        {
            FarmerSysPriceService farmerPrice = new FarmerSysPriceService();
            using (TransactionScope scope = new TransactionScope())
            {
                //bool isModify = CheckFarmSysData(MapNo);
                bool isModify = getData.GetCaseDataFromMapNo(MapNo).Step >= 5/*3*/ ? true : false;
                int[] helpMoney = new FarmerSysPriceService().GetHelpTotalPrice(Data, MapNo);
                //Data.TotalPrice = new FarmerSysPriceService().GetTotalPriceFromDB(MapNo);
                Data.TotalPrice = helpMoney[0];
                
                dbstatus = SavePigingConf(Data, MapNo);
                if (dbstatus.DbMessage == "Success")
                {
                    
                    dbstatus = SaveEndType(Data, MapNo);
                    
                    if (dbstatus.DbMessage == "Success")
                    {
                        
                        string FarSysNo = SaveFarmerSys(Data, MapNo);
                        if (dbstatus.DbMessage == "Success")
                        {
                            dbstatus = SaveFarmerSysM(Data, FarSysNo, MapNo);
                            if (dbstatus.DbMessage.Equals("Success"))
                            {
                                Case casedata = getData.GetCaseDataFromMapNo(MapNo);
                                PayDBService paydb = new PayDBService();
                                
                                dbstatus.DbMessage = DelPay(MapNo).DbMessage;
                                
                                //Pay data = paydb.CreatePayObj(MapNo, 1, casedata.ApplyUnit, helpMoney[1], helpMoney[2]);
                                
                                Pay data = paydb.CreatePayObj(MapNo, 1, Data.Unit, helpMoney[1], helpMoney[2]);
                                
                                //Pay planningfee = paydb.CreatePayObj(MapNo, 2, /*casedata.ApplyUnit*/Data.Unit, farmerPrice.GetPlanningPrice(Data.TotalPrice), 0);
                                
                                Pay planningfee = paydb.CreatePayObj(MapNo, 2, /*casedata.ApplyUnit*/Data.Unit, farmerPrice.GetPlanningPrice(Data.TotalPrice,casedata.ApplyUnit,casedata.ApplyYear), 0);
                                if (DryDB.Pay.Any(m => m.MapNo == MapNo && m.ItemCode == 1))
                                {
                                    
                                    if(dbstatus.DbMessage=="Success")
                                    {
                                        dbstatus = paydb.UpdatePay(data);
                                    }
                                }
                                else
                                {
                                    if (dbstatus.DbMessage == "Success")
                                    {
                                        dbstatus = paydb.CreatePay(data);
                                    }
                                }
                                if (DryDB.Pay.Any(m => m.MapNo == MapNo && m.ItemCode == 2))
                                {
                                    if (dbstatus.DbMessage == "Success")
                                    {
                                        dbstatus = paydb.UpdatePay(planningfee);
                                    }
                                }
                                else
                                {
                                    if (dbstatus.DbMessage == "Success")
                                    {
                                        dbstatus = paydb.CreatePay(planningfee);
                                    }
                                }

                                if (dbstatus.DbMessage == "Success")
                                {
                                    //dbstatus = new TotalFeeDBService().UpdateTotalFeeWithStep(MapNo,1);
                                    dbstatus = new TotalFeeDBService().UpdateTotalFee(MapNo);
                                }
                            }
                            if (dbstatus.DbMessage.Equals("Success") && !isModify)
                            {
                                dbstatus.DbMessage = comm.UpdateCase(MapNo, 5 /*3*/).DbMessage;
                            }
                            if (dbstatus.DbMessage == "Success")
                                scope.Complete();
                        }
                    }
                }
            }
            return dbstatus;
        }
        #endregion

        #region 寫入PigingConf資料表
        /// <summary>
        /// 寫入PigingConf資料表
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        private DBCommon.DbEvent SavePigingConf(FarmerSysView.ParaJsonData Data, int MapNo)
        {
            PigingConf pigConf = new PigingConf();
            FarmerSysPriceService farmerPrice = new FarmerSysPriceService();
            int area = getData.GetFarmBuildArea(MapNo);
            pigConf.MapNo = MapNo;
            pigConf.L1 = Data.MainJsonDataAry[0].Length; //主管1長度
            pigConf.L1Mat = Data.MainJsonDataAry[0].Mat; //主管1物料
            pigConf.L1Spec = getData.IdToSpec((int)Data.MainJsonDataAry[0].Spec);
            pigConf.L1Price = Data.MainJsonDataAry[0].LPrice; //主管1單價
            pigConf.L1Amount = Data.MainJsonDataAry[0].Amount; //主管1物料數量
            //表示有設置主管2資料
            if (Data.MainJsonDataAry.Count >= 2)
            {
                if (Data.MainJsonDataAry[1].Length != 0 && Data.MainJsonDataAry[1].Amount != 0)
                {
                    pigConf.L2 = Data.MainJsonDataAry[1].Length; //主管2長度
                    pigConf.L2Mat = Data.MainJsonDataAry[1].Mat; //主管2材料
                    pigConf.L2Spec = getData.IdToSpec((int)Data.MainJsonDataAry[1].Spec);
                    pigConf.L2Price = Data.MainJsonDataAry[1].LPrice; //主管2單價
                    pigConf.L2Amount = Data.MainJsonDataAry[1].Amount; //主管2物料數量
                }
            }
            //
            pigConf.Cblock = Data.Block; //坵塊型狀

            //已移至EndType資料表 in 2015/4/16
            #region old code
            //pigConf.SL = Data.SL; //支管行距
            //pigConf.SS = Data.SS; //噴頭間距
            //pigConf.FacType = Data.Fac; //施設型式代碼
            //pigConf.EndType = Data.Endtype; //末端型式代碼
            //pigConf.StdpipeHei = Data.StdpipeHei;//豎管高度
            #endregion

            
            if (getData.GetCaseDataFromMapNo(MapNo).ApplyUnit == 17)
            {//瑠公
                //工作費
                //pigConf.WorkPrice = farmerPrice.GetLiuWorkPrice(area, MapNo, Data.EndTypeDataAry.First().Endtype, Data.EndTypeDataAry.First().Fac);
                //20240123 alex modify with the regualar rule
                pigConf.WorkPrice = farmerPrice.GetWorkPrice(Data, area, MapNo);
            }
            else
            {//農委會
                //工作費
                pigConf.WorkPrice = farmerPrice.GetWorkPrice(Data, area, MapNo);
            }
            DryDB = new DryEntities();
            pigConf.IrrWCode = Data.IrrWCode; //灌漑水源代碼
            if (getData.GetPigingConfData(MapNo) != null)
            {//更新資料
                DryDB = new DryEntities();
                DryDB.PigingConf.Attach(pigConf);
                DryDB.Entry(pigConf).State = System.Data.EntityState.Modified;
            }
            else
            {//新增資料
                DryDB.PigingConf.Add(pigConf);
            }
            try
            {
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Inserted PigingConf Failed : " + e.Message;
            }
            return dbstatus;
        }
        #endregion
        /// <summary>
        /// 修改田間管路系統的工作費
        /// </summary>
        /// <param name="WordPrice"></param>
        /// <param name="mno"></param>
        /// <returns></returns>
        public DBCommon.DbEvent ModifyPigingConfWorkPrice(int WordPrice , int mno)
        {
            //PigingConf data = getData.GetPigingConfData(mno);
            PigingConf data = DryDB.PigingConf.Find(mno);
            if (data != null)
            {
                try
                {
                    data.WorkPrice = WordPrice;
                    DryDB.PigingConf.Attach(data);
                    DryDB.Entry(data).State = System.Data.EntityState.Modified;
                    DryDB.SaveChanges();
                    dbstatus.DbMessage = "Success";
                }
                catch (Exception e)
                {
                    dbstatus.DbMessage = "Modified PigingConf Failed : " + e.Message + "<br>";
                }
            }
            else
            {
                dbstatus.DbMessage = "Success";
            }
            return dbstatus;
        }

        /// <summary>
        /// 寫入或更新EndType資料表
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        private DBCommon.DbEvent SaveEndType(FarmerSysView.ParaJsonData Data, int MapNo)
        {
            string DelMsg = DelEndType(MapNo).DbMessage; 
            if (DelMsg == "Success")
            {
                foreach (var item in Data.EndTypeDataAry)
                {
                    EndType endType = new EndType();
                    endType.BranchPipeMat = item.BranchPipeMaterial;
                    endType.BranchPipeSpec = getData.IdToSpec((int)item.BranchPipeSpec);
                    endType.StdpipeMat = item.StdpipeMat;
                    endType.StdpipeSpec = getData.IdToSpec((int)item.StdpipeSpec);
                    endType.SprinklerType = item.NozzleType;
                    endType.SprinklerSpec = getData.IdToSpec((int)item.NozzleSpec);
                    endType.MapNo = MapNo;
                    endType.SL = item.SL; 
                    endType.SS = item.SS; 
                    endType.FacType = item.Fac; 
                    endType.EndTypeCode = item.Endtype; 
                    //endType.IrrWCode = item.IrrWCode; 
                    endType.StdpipeHei = item.StdpipeHei;
                    DryDB.EndType.Add(endType);
                    try
                    {
                        DryDB.SaveChanges();
                        dbstatus.DbMessage = "Success";
                    }
                    catch (Exception e)
                    {
                        dbstatus.DbMessage = "Modified EndType Failed : " + e.Message + "<br>";
                        break;
                    }
                }
            }
            else
            {
                dbstatus.DbMessage = DelMsg;
            }
            
            return dbstatus;
        }

        #region 寫入或更新FarmerSys資料表
        /// <summary>
        /// 寫入或更新FarmerSys資料表
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        private string SaveFarmerSys(FarmerSysView.ParaJsonData Data, int MapNo)
        {
            string FarSysNo = getData.GetFarmerSysNo(MapNo);
            FarmerSys farmerSys = new FarmerSys();
            farmerSys.MapNo = MapNo;
            farmerSys.FacNo = 8;
            farmerSys.FacMoney = 0;
            farmerSys.ApplyUnit = Data.Unit;
            
            if (FarSysNo == null)
            {
                int farmerSysListCount = DryDB.FarmerSys.ToList().Count();
                FarSysNo = "F" + (DateTime.Now.Year - 1911).ToString() + (farmerSysListCount + 1).ToString().PadLeft(6, '0');
                while (DryDB.FarmerSys.Any(p => p.FarSysNo == FarSysNo))
                {
                    farmerSysListCount += 1;
                    FarSysNo = "F" + (DateTime.Now.Year - 1911).ToString() + (farmerSysListCount + 1).ToString().PadLeft(6, '0');
                }
                farmerSys.FarSysNo = FarSysNo;
                DryDB = new DryEntities();
                DryDB.FarmerSys.Add(farmerSys);
            }
            else
            {
                DryDB = new DryEntities();
                farmerSys.FarSysNo = FarSysNo;
                DryDB.FarmerSys.Attach(farmerSys);
                DryDB.Entry(farmerSys).State = System.Data.EntityState.Modified;
            }
            try
            {
                //DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch(Exception e)
            {
                dbstatus.DbMessage = "Modified FarmerSys Failed : " + e.Message + "<br>";
            }
            return FarSysNo;
        }
        #endregion

        #region 寫入農戶設施系統物料
        private DBCommon.DbEvent SaveFarmerSysM(FarmerSysView.ParaJsonData Data, string FarSysNo,int mno)
        {
            
            dbstatus = DelFarmerSysM(FarSysNo);
            if (dbstatus.DbMessage == "Success")
            {
                short ApplyYear = DryDB.VerMapping.Find(mno).Case.ApplyYear;
                List<MAT_Spec> specData = DryDB.MAT_Spec.ToList();
                List<Mat_Module> moduleData = DryDB.Mat_Module.ToList();
                List<MatGroup> groups = DryDB.MatGroup.ToList();
                if (Data.PriceJsonDataAry != null)
                {
                    foreach (var item in Data.PriceJsonDataAry)
                    {
                        FarmerSysM farmerSysM = new FarmerSysM();
                        farmerSysM.FarSysNo = FarSysNo;
                        farmerSysM.POMNo = item.POMNo;
                        farmerSysM.Amount = item.Amt;
                        FacSysMAT facSysMat = DryDB.FacSysMAT.Find(item.POMNo);
                        farmerSysM.ModuleNo = facSysMat.ModuleNo;
                        farmerSysM.ModuleCNS = moduleData.Find(m => m.ModuleNo == facSysMat.ModuleNo).ModuleCNS;
                        farmerSysM.MName = facSysMat.MName;
                        farmerSysM.MatGroupID = item.Group;
                        farmerSysM.MatGroupCNS = groups.Find(m => m.GroupID == item.Group).GroupCNS;
                        farmerSysM.MatOrder = item.Order;
                        farmerSysM.Spec = facSysMat.Spec;
                        farmerSysM.Spec1 = facSysMat.SpecNo1 == null ? 0 : specData.Find(m => m.SpecNo == facSysMat.SpecNo1.Value).Spec;
                        farmerSysM.Spec2 = facSysMat.SpecNo2 == null ? 0 : specData.Find(m => m.SpecNo == facSysMat.SpecNo2.Value).Spec;
                        farmerSysM.Spec3 = facSysMat.SpecNo3 == null ? 0 : specData.Find(m => m.SpecNo == facSysMat.SpecNo3.Value).Spec;
                        farmerSysM.SpecName1 = facSysMat.SpecNo1 == null ? "" : specData.Find(m => m.SpecNo == facSysMat.SpecNo1.Value).SpecName;
                        farmerSysM.SpecName2 = facSysMat.SpecNo2 == null ? "" : specData.Find(m => m.SpecNo == facSysMat.SpecNo2.Value).SpecName;
                        farmerSysM.SpecName3 = facSysMat.SpecNo3 == null ? "" : specData.Find(m => m.SpecNo == facSysMat.SpecNo3.Value).SpecName;
                        farmerSysM.ItemUnit = facSysMat.ItemUnit;
                        farmerSysM.SysPrice = item.Price;
                        farmerSysM.Note = facSysMat.Note;
                        farmerSysM.TotalPrice = item.TotalPrice;
                        DryDB.FarmerSysM.Add(farmerSysM);
                        try
                        {
                            DryDB.SaveChanges();
                            dbstatus.DbMessage = "Success";
                        }
                        catch (Exception e)
                        {
                            dbstatus.DbMessage = "Inserted FarmerSysM Failed : " + e.Message;
                            break;
                        }
                    }
                }
            }
            return dbstatus;
        }
        #endregion

        #region 刪除FarmerSysM已存在的資料
        /// <summary>
        /// 刪除FarmerSysM已存在的資料
        /// </summary>
        /// <param name="FarSysNo"></param>
        /// <returns></returns>
        private DBCommon.DbEvent DelFarmerSysM(string FarSysNo)
        {
            IQueryable<FarmerSysM> iFarmerSysM = DryDB.FarmerSysM.Where(p => p.FarSysNo == FarSysNo);
            foreach(var item in iFarmerSysM)
            {
                DryDB.FarmerSysM.Remove(item);
            }
            try
            {
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch(Exception e)
            {
                dbstatus.DbMessage = "Removed FarmerSysM Failed : " + e.Message;
            }
            return dbstatus;
        }
        #endregion
        /// <summary>
        /// 刪除EndType已存在的資料
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        private DBCommon.DbEvent DelEndType(int MapNo)
        {
            IQueryable<EndType> Data = DryDB.EndType.Where(p => p.MapNo == MapNo);
            foreach (var item in Data)
            {
                DryDB.EndType.Remove(item);
            }
            try
            {
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed EndType Failed : " + e.Message;
            }
            return dbstatus;
        }

        public bool CheckFarmSysData(int MapNo)
        {
            return DryDB.FarmerSys.Any(p => p.MapNo == MapNo);
        }

        private DBCommon.DbEvent DelPay(int MapNo)
        {
            IQueryable<Pay> Data = DryDB.Pay.Where(p => p.MapNo == MapNo && (p.ItemCode == 1 || p.ItemCode == 2));
            foreach (var item in Data)
            {
                DryDB.Pay.Remove(item);
            }
            try
            {
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Removed Pay Failed : " + e.Message;
            }
            return dbstatus;
        }
    }
}
