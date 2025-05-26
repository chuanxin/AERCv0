using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization; 
using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
using Dry.Models.Service;
/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-2-14
-- Description: 工程預算書的資料庫操作類別
-- =============================================
*/
namespace Dry.Models.Service
{
    public class BudgetBookDBService
    {
        private DryEntities DryDB = new DryEntities();
        private Calculate_Funding calFund = new Calculate_Funding();
        private GetData getDataCls = new GetData();
        private CtrlPriceService CPSer = new CtrlPriceService();
        private FarmerSysPriceService fsys = new FarmerSysPriceService();

        #region 產製預算書
        /// <summary>
        /// 產製預算書
        /// </summary>
        /// <param name="_MNo">版本編號</param>
        /// <returns>預算書</returns>
        public BudgetBookView GetBudgetBookData(int _MNo)
        {
            //Case CaseDta = getDataCls.GetCaseDataFromMapNo(_MNo);
            return GetCoABudgetBookData(_MNo);

            
        }
        #endregion

        #region 農委會工程預算書
        /// <summary>
        /// 農委會工程預算書
        /// </summary>
        /// <param name="_MNo">版本編號</param>
        /// <returns>農委會工程預算書</returns>
        public BudgetBookView GetCoABudgetBookData(int _MNo)
        {
            #region Get Data
            Case CaseDta = getDataCls.GetCaseDataFromMapNo(_MNo);
            Farmer farmer = getDataCls.GetFarmerData(_MNo);
            List<Dry.Models.CommonCls.GetData.NewFarm> Farm = getDataCls.GetFarmData(_MNo)./*OrderByDescending(f => f.Section).ThenByOrderBy(f => f.LandNo).*/ToList();
            Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct FSys = getDataCls.GetFarmerSystemData(_MNo);
            PigingConf piping = getDataCls.GetPigingConfData(_MNo);
            List<Pay> pays = getDataCls.GetPay(_MNo);
            TotalFee totalFee = getDataCls.GetTotalFee(_MNo);
            List<FarmerSysM> FarmerSysMat = getDataCls.GetFarmerSysM(_MNo);
            List<Pool> FarmerPool = getDataCls.GetPoolData(_MNo);
            MappingClass mapingCls = new MappingClass();
            List<EndType> endType = getDataCls.GetEndTypeData(_MNo);
            #endregion

            BudgetBookView ModelData = new BudgetBookView();
            
            #region Assign basic Data
            ModelData.Case = CaseDta.EventNo;
            ModelData.IANum = CaseDta.IANum;
            ModelData.ApplyY = CaseDta.ApplyYear;
            ModelData.Gold = CaseDta.Gold;
            ModelData.BuildArea = Farm.Sum(f => f.BuildArea);
            ModelData.Name = farmer.Name;
            ModelData.Block = piping == null ? "" : piping.Cblock;
            ModelData.SS = endType.Count <= 0 ? 0 : endType.FirstOrDefault().SS;
            ModelData.SL = endType.Count <= 0 ? 0 : endType.FirstOrDefault().SL;
            string farmerAddr = "";
            if (farmer.Addr.Split('(').Count() > 1)
                farmerAddr = farmer.Addr.Split('(')[0] + farmer.Addr.Split(')')[1];
            else
                farmerAddr = farmer.Addr;
            ModelData.Address = farmerAddr.Replace(" ", "");
            for (int i = 0; i < Farm.Count(); i++)
            {
                if (Farm[i].full_sectName.Split('(').Count() > 1)
                {
                    Farm[i].full_sectName = Farm[i].full_sectName.Split('(')[0] + Farm[i].full_sectName.Split(')')[1];
                }
            }
            ModelData.Farm = Farm;
            //ModelData.EndType = (FSys == null) ? "未申請末端設施" : FSys.FacName;
            ModelData.EndType = (FSys == null) ? "其它" : FSys.FacName;
            ModelData.ApplyUnit = CaseDta.ApplyUnit;
            if (getDataCls.ChkAppliedByMaoNo(_MNo))
            {
                ModelData.IsApplied = true;
            }
            else
            {
                ModelData.IsApplied = false;
            }
            #endregion

            //int TotalMoney = 0;
            int PipingMoney = 0;
            int PlanningMoney = 0;
            int WorkMoney = 0;

            #region Piping System(田間管路)
            int[] p_money = { 0, 0, 0, 0 };
            int m_money = 0;
            if (piping != null)
            {
                
                p_money = getDataCls.GetPigingMoneyByDB(_MNo); 
                m_money = new FarmerSysPriceService().GetPriceHadNoWorkPrice(_MNo); 

                #region L1 Money
                ModelData.L1_length = piping.L1.ToString();

                
                
                double L1MatPrice = piping.L1Amount == 0 ? 0 : piping.L1Price;
                short L1MatAmount = piping.L1Amount;

                
                int L1TotalPrice = (int)Math.Round(L1MatPrice * L1MatAmount, 0, MidpointRounding.AwayFromZero);

                ModelData.L1 = new BudgetBookstruct(L1MatAmount.ToString(), string.Format("{0:N0}", L1MatPrice), string.Format("{0:N0}", L1TotalPrice));
                //TotalMoney += L1TotalPrice;
                #endregion

                #region L2 Money
                ModelData.L2_length = piping.L2.ToString();

                
                double L2MatPrice = 0;
                short L2MatAmount = 0;
                int L2TotalPrice = 0;
                if (piping.L2 != 0)
                {
                    
                    L2MatPrice = (int)piping.L2Amount == 0 ? 0 : piping.L2Price;
                    L2MatAmount = (short)piping.L2Amount;
                    L2TotalPrice = (int)Math.Round(L2MatPrice * L2MatAmount, 0, MidpointRounding.AwayFromZero);

                    ModelData.L2 = new BudgetBookstruct(L2MatAmount.ToString(), string.Format("{0:N0}", L2MatPrice), string.Format("{0:N0}", L2TotalPrice));
                    //TotalMoney += L2TotalPrice;
                }
                #endregion

                #region Irr System(灌溉系統)
                int irrsystem_price = 0; 
                if (FarmerSysMat != null)
                {

                    irrsystem_price = m_money;
                    ModelData.IrrSystem = new BudgetBookstruct("1"/*((double)ModelData.BuildArea / 10000).ToString()*/,
                                                                "",
                                                                string.Format("{0:N0}", irrsystem_price),
                                                                /*"詳如標準表"*/ "詳如數量表");
                    //TotalMoney += (int)Math.Round(irrsystem_price, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    ModelData.IrrSystem = new BudgetBookstruct("", "", "");
                }
                #endregion

                #region Piping Mat Cost(材料費)
                ModelData.PipingMatMoney = string.Format("{0:N0}", L1TotalPrice + L2TotalPrice + irrsystem_price);
                #endregion

                #region Work cost
                WorkMoney = p_money[3];
                
                byte Endtype = endType.FirstOrDefault().EndTypeCode;
                byte Factype = endType.FirstOrDefault().FacType;
                PigingLimit pigLimit = getDataCls.GetPigingLimit(CaseDta, Endtype, Factype);
                int WorkItemMoney = pigLimit == null ? 0 : pigLimit.WorkingFee;
                string WorkItem = "0";
                if (p_money[0] == 0)
                {
                    ModelData.Work = new BudgetBookstruct("", "", "");
                }
                else
                {
                    //if (Endtype != 2)
                    //{
                    //    WorkItem = WorkItemMoney.ToString();
                    //}
                    //else
                    //{
                    //WorkItem = (Math.Round((WorkMoney / ((double)ModelData.BuildArea / 10000)), 0, MidpointRounding.AwayFromZero)).ToString(); 

                    WorkItem = fsys.GetWorkPriceOrg(_MNo).ToString();


                    //}
                    ModelData.Work = new BudgetBookstruct(((double)ModelData.BuildArea / 10000).ToString(),
                        /*WorkItemMoney.ToString()*/ModelData.BuildArea <= 0 ? "0" : string.Format("{0:N0}", Int32.Parse(WorkItem)),
                                                            string.Format("{0:N0}", WorkMoney));
                    //TotalMoney += WorkMoney;
                }
                
                #endregion
                
                #region Piping Total Cost(田間管路設施費)
                PipingMoney = p_money[0];
                ModelData.PipingTotal = string.Format("{0:N0}", PipingMoney);
                #endregion

                #region Planning Cost
                PlanningMoney = pays.Any(m => m.ItemCode == 2) ? pays.Where(m => m.ItemCode == 2).FirstOrDefault().Total.Value : 0;
                #endregion
            }
            #endregion

            #region 規劃設計費
            ModelData.Planning = new BudgetBookstruct("1", "", string.Format("{0:N0}", PlanningMoney));
            //TotalMoney += PlanningMoney;
            #endregion

            #region 調控設施
            int CntrlMoney = 0; 
            int CtrlFarPay = 0;
            int CtrlItemMoney = 0; 
            

            List<CntrlMat> MatData = getDataCls.GetCntrlMatData(_MNo);
            
            if (MatData != null)
            {
                //List<CtrlTableView> ctblview = new List<CtrlTableView>();
                //foreach (var matitem in MatData)
                //{
                //    ctblview.Add(new CtrlTableView { CntrlCode = matitem.CntrlCode, MatAmt = matitem.MatAmt, MatNo = matitem.MatNo, MatPrice = matitem.MatPrice });
                //}

                //int[] price = CPSer.GetCtrlHelpTotalPrice(ctblview.ToArray(), _MNo);
                CtrlItemMoney = 0;
                //CtrlFarPay = price[2];

                var VArobj = MatData.Select(m => m.CntrlCode).Distinct();
                string meno = string.Empty;
                int amount = 0;
                foreach (var item in VArobj)
                {
                    meno += mapingCls.GetCntrltypeName(item) + "#";
                    amount++;
                }
                if (meno.Length > 0)
                {
                    meno += "(詳如數量表)#";
                    //amount++;
                }
                if (pays.Any(m => m.ItemCode == 4))
                {
                    CntrlMoney = pays.Where(m => m.ItemCode == 4).FirstOrDefault().Total.Value;
                    //CntrlMoney = pays.Where(m => m.ItemCode == 4).FirstOrDefault().PayMoney;
                    CtrlFarPay = pays.Where(m => m.ItemCode == 4).FirstOrDefault().FarmerMoney;
                    //CntrlMoney -= CtrlFarPay;
                }
                ModelData.RegulatedFac = new BudgetBookstruct(amount.ToString(), string.Format("{0:N0}", ""/*CtrlItemMoney*/), string.Format("{0:N0}", CntrlMoney), meno);
                //TotalMoney += CntrlMoney;
            }
            #endregion

            #region 動力設備
            int EngineMoney = 0; 
            //int EngineItemMoney = 0; 
            var englist = getDataCls.GetEngineData(_MNo);
            if (englist != null)
            {
                List<EngineDBService.EngineData> engData = new List<EngineDBService.EngineData>();
                foreach(var item in englist)
                {
                    engData.Add(new EngineDBService.EngineData { EngineCode = item.EngCode, EngPrice = item.EngPrice, RegCode = item.EngRegCode });
                }
                //EngineItemMoney = calFund.GetEngMoneybyEngList(engData.ToArray(), CaseDta).GovPay;

                short[] EngAry = new short[] { 0, 0, 0, 0, 0, 0, 0 };

                foreach (var engitem in englist)
                {
                    EngAry[engitem.EngCode - 1]++;
                }
                string memo = string.Empty;
                for (short i = 0; i < (short)EngAry.Length; i++)
                {
                    if (EngAry[i] > 0)
                    {
                        memo += mapingCls.GetEngName((short)(i + 1)) + " * " + EngAry[i] + "#";
                    }
                }
                if (DryDB.Pay.Any(m => m.MapNo == _MNo && m.ItemCode == 5))
                    EngineMoney = DryDB.Pay.Where(m => m.MapNo == _MNo && m.ItemCode == 5).FirstOrDefault().Total.Value;
                ModelData.Engine = new BudgetBookstruct(englist.Count.ToString(), string.Format("{0:N0}", ""/*EngineItemMoney*/), string.Format("{0:N0}", EngineMoney), memo);

                //TotalMoney += EngineMoney;
            }
            
            #endregion

            #region 蓄水池
            int PoolMoney = 0; 
            //int PoolItemMoney = 0; 
            if (FarmerPool != null)
            {
                ModelData.PoolWei = FarmerPool.Sum(p => p.PoolWeight);

                List<PoolDBService.PoolData> poldata = new List<PoolDBService.PoolData>();
                string memo = string.Empty;
                foreach (var politem in FarmerPool.OrderBy(p => p.PoolWeight))
                {
                    poldata.Add(new PoolDBService.PoolData { poolType = politem.PtypeCode, poolW = politem.PoolWeight, poolPrice = politem.PoolPrice, poolSize = politem.PoolSize });
                    memo += politem.PoolWeight + "噸#";
                }
                //memo = memo.ToString().Substring(0, memo.Length - 1);
                //PoolItemMoney = calFund.GetPoolMoneybyList(poldata.ToArray(), CaseDta, _MNo).GovPay;
                if(pays.Any(m=>m.ItemCode == 6))
                {
                    PoolMoney = pays.Where(m => m.ItemCode == 6).FirstOrDefault().Total.Value;
                }
                //PoolMoney = FarmerPool.Count * PoolItemMoney;
                ModelData.Pool = new BudgetBookstruct(FarmerPool.Count.ToString(), string.Format("{0:N0}", ""/*PoolItemMoney*/), string.Format("{0:N0}", PoolMoney), memo);
                //TotalMoney += PoolMoney;
            }
            
            #endregion

            #region 合計
            
           
            ModelData.Total = string.Format("{0:N0}", totalFee == null ? 0 : totalFee.Total.Value );

            
            int piping_FarPay = p_money[2];

            
            int Ctrl_FarPay = CtrlFarPay;

            
            //int farpay = totalFee == null ? 0 : totalFee.FarmerFee - CtrlFarPay;//20160530 alex modify 扣除調控農戶自付部分- CtrlFarPay

            int farpay = totalFee == null ? 0 : totalFee.FarmerFee ;
            ModelData.FarmerPay = string.Format("{0:N0}", farpay);

            
            int govpay = totalFee == null ? 0 : totalFee.GovSubsidy - PlanningMoney;
            
            if (ModelData.Gold == true && ModelData.ApplyY < 109)
            {
                govpay = totalFee.GovSubsidy + totalFee.GoldSubsidy - PlanningMoney;
            }
            ModelData.GovPay_Pay = string.Format("{0:N0}", govpay);

            
            ModelData.GovPay_Planning = string.Format("{0:N0}", PlanningMoney);

            
            ModelData.GovPay_Sum = string.Format("{0:N0}", totalFee==null?0:totalFee.GovSubsidy);
            #endregion
            //ModelData.Total = string.Format("{0:N0}", totalFee == null ? 0 : totalFee.Total.Value - CtrlFarPay);
            ModelData.Total = string.Format("{0:N0}", totalFee == null ? 0 : totalFee.Total.Value);
            #region 國字總計
            ModelData.ChineseMoney = new NumberToChinese().GetChineseNumber(totalFee == null ? 0 : totalFee.Total.Value);
            #endregion
            #region 填寫各項補助金額
            
            if (DryDB.CasePayDetail.Any(m => m.mapno == _MNo))
            {
                var paydetail = DryDB.CasePayDetail.Where(m => m.mapno == _MNo).FirstOrDefault();
                ModelData.GovPay_Detail = new List<string>();
                if (ModelData.Gold == true && ModelData.ApplyY < 109) 
                {
                    if (paydetail.田間管路設施費 > 0)
                    {
                        
                        //int v = paydetail.田間管路設施費 ?? 0;
                        
                        int v = (int)Math.Round((int)DryDB.Pay.Where(m => m.MapNo == _MNo && m.ItemCode == 1).FirstOrDefault().Total * 0.7, 0, MidpointRounding.AwayFromZero);                        
                        ModelData.GovPay_Detail.Add($"A項補助費:{v.ToString("N0")}");
                        //paymeno.Add($"田間管路設施費:{v.ToString("N0")}");
                    }
                    if (paydetail.調控設施費 > 0)
                    {
                        //int v = paydetail.調控設施費 ?? 0;
                        
                        int v = (int)Math.Round((int)DryDB.Pay.Where(m => m.MapNo == _MNo && m.ItemCode == 4).FirstOrDefault().Total * 0.7, 0, MidpointRounding.AwayFromZero);
                        ModelData.GovPay_Detail.Add($"C項補助費:{v.ToString("N0")}");

                    }
                    if (paydetail.動力設備費 > 0)
                    {
                        int v = paydetail.動力設備費 ?? 0;
                        ModelData.GovPay_Detail.Add($"D項補助費:{v.ToString("N0")}");
                    }
                    if (paydetail.蓄水設備費 > 0)
                    {
                        int v = paydetail.蓄水設備費 ?? 0;
                        ModelData.GovPay_Detail.Add($"E項補助費:{v.ToString("N0")}");
                    }
                }
                else
                {

                    if (paydetail.田間管路設施費 > 0)
                    {
                        int v = paydetail.田間管路設施費 ?? 0;
                        ModelData.GovPay_Detail.Add($"A項補助費:{v.ToString("N0")}");
                        //paymeno.Add($"田間管路設施費:{v.ToString("N0")}");
                    }
                    if (paydetail.調控設施費 > 0)
                    {
                        int v = paydetail.調控設施費 ?? 0;
                        ModelData.GovPay_Detail.Add($"C項補助費:{v.ToString("N0")}");

                    }
                    if (paydetail.動力設備費 > 0)
                    {
                        int v = paydetail.動力設備費 ?? 0;
                        ModelData.GovPay_Detail.Add($"D項補助費:{v.ToString("N0")}");
                    }
                    if (paydetail.蓄水設備費 > 0)
                    {
                        int v = paydetail.蓄水設備費 ?? 0;
                        ModelData.GovPay_Detail.Add($"E項補助費:{v.ToString("N0")}");
                    }
                }
            }
            
            #endregion
            return ModelData;
        }
        #endregion

        #region 黃金廊道工程預算書
        /// <summary>
        /// 黃金廊道工程預算書
        /// </summary>
        /// <param name="_MNo">版本編號</param>
        /// <returns>黃金廊道工程預算書</returns>
        public BudgetBookView GetGoldBudgetBookData(int _MNo)
        {
            #region Get Data
            Case CaseDta = getDataCls.GetCaseDataFromMapNo(_MNo);
            Farmer farmer = getDataCls.GetFarmerData(_MNo);
            List<Dry.Models.CommonCls.GetData.NewFarm> Farm = getDataCls.GetFarmData(_MNo);
            Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct FSys = getDataCls.GetFarmerSystemData(_MNo);
            //List<Pay> pay = GetPayData(_MNo);
            PigingConf piping = getDataCls.GetPigingConfData(_MNo); 
            List<EndType> endType = getDataCls.GetEndTypeData(_MNo); 
            List<FarmerSysM> FarmerSysMat = getDataCls.GetFarmerSysM(_MNo);
            List<Pool> FarmerPool = getDataCls.GetPoolData(_MNo);
            List<Pay> pays = getDataCls.GetPay(_MNo);
            TotalFee totalFee = getDataCls.GetTotalFee(_MNo);
            MappingClass mapingCls = new MappingClass();
            #endregion

            BudgetBookView ModelData = new BudgetBookView();

            #region Assign basic Data
            ModelData.Case = CaseDta.EventNo;
            ModelData.IANum = CaseDta.IANum;
            ModelData.ApplyY = CaseDta.ApplyYear;
            ModelData.Gold = CaseDta.Gold;
            ModelData.BuildArea = Farm.Sum(f => f.BuildArea);
            ModelData.Name = farmer.Name;
            ModelData.Address = farmer.Addr;
            ModelData.Farm = Farm;
            ModelData.EndType = (FSys == null) ? "其它" : FSys.FacName;
            ModelData.ApplyUnit = CaseDta.ApplyUnit;
            ModelData.Block = piping == null ? "" : piping.Cblock;
            ModelData.SS = endType.Count <= 0 ? 0 : endType.FirstOrDefault().SS;
            ModelData.SL = endType.Count <= 0 ? 0 : endType.FirstOrDefault().SL;
            if (getDataCls.ChkAppliedByMaoNo(_MNo))
            {
                ModelData.IsApplied = true;
            }
            else
            {
                ModelData.IsApplied = false;
            }

            #endregion

            int TotalMoney = 0;
            int PipingMoney = 0;
            int PlanningMoney = 0;
            int WorkMoney = 0;
            int LiuM = 0;

            #region Piping System(田間管路)

            if (piping != null)
            {
                #region L1 Money
                #region Old Code
                ////Get L1 materiel
                //int L1Mat = piping.L1Mat;
                ////Get L1 materiel amount
                //short L1MatAmount = piping.L1Amount;
                ////Get L1 materiel price
                //double L1MatPrice = getDataCls.GetMatPrice(L1Mat, ModelData.ApplyY, CaseDta.ApplyUnit);
                #endregion


                
                double L1MatPrice = piping.L1Price;
                short L1MatAmount = piping.L1Amount;

                
                int L1TotalPrice = (int)(L1MatPrice * L1MatAmount);

                ModelData.L1 = new BudgetBookstruct(L1MatAmount.ToString(), string.Format("{0:N0}", L1MatPrice), string.Format("{0:N0}", L1TotalPrice));
                TotalMoney += L1TotalPrice;
                ModelData.L1_length = piping.L1.ToString();
                #endregion

                #region L2 Money
                            
                if (piping.L2 != 0)
                {
                    #region Old Code
                    //int L2Mat = (int)piping.L2Mat;
                    ////Get L2 materiel amount
                    //short L2MatAmount = (short)piping.L2Amount;
                    ////Get L2 materiel price
                    //double L2MatPrice = getDataCls.GetMatPrice(L2Mat, ModelData.ApplyY, CaseDta.ApplyUnit);
                    #endregion


                    
                    double L2MatPrice = piping.L2Price; 
                    short L2MatAmount = (short)piping.L2Amount; 

                    
                    int L2TotalPrice = (int)(L2MatPrice * L2MatAmount);
                    ModelData.L2 = new BudgetBookstruct(L2MatAmount.ToString(), string.Format("{0:N0}", L2MatPrice), string.Format("{0:N0}", L2TotalPrice));
                    TotalMoney += L2TotalPrice;
                }
                #endregion
                ModelData.L2_length = piping.L2.ToString();
                #region Irr System(灌溉系統)
                if (FarmerSysMat != null)
                {
                    int irrsystem_price = 0;
                    /*
                    foreach (var item in FarmerSysMat)
                    {
                        double pom_price = getDataCls.GetMatPrice(item.POMNo, ModelData.ApplyY, CaseDta.ApplyUnit);
                        irrsystem_price += (int)(pom_price * item.Amount);
                    }
                     */
                    irrsystem_price = new FarmerSysPriceService().GetPriceHadNoWorkPrice(_MNo); 
                    ModelData.IrrSystem = new BudgetBookstruct((ModelData.BuildArea / 10000).ToString(), "", string.Format("{0:N0}", irrsystem_price), "詳如標準表");
                    TotalMoney += irrsystem_price;
                }
                #endregion

                #region Piping Mat Cost(材料費)
                ModelData.PipingMatMoney = string.Format("{0:N0}", TotalMoney);
                #endregion

                #region Work cost
                int[] p_money = { 0, 0, 0, 0 };
                p_money = getDataCls.GetPigingMoneyByDB(_MNo); 
                
                WorkMoney = p_money[3];
                int WorkItem = fsys.GetWorkPriceOrg(_MNo);
                

                //}
                ModelData.Work = new BudgetBookstruct(((double)ModelData.BuildArea / 10000).ToString(),
                    /*WorkItemMoney.ToString()*/ModelData.BuildArea <= 0 ? "0" : string.Format("{0:N0}",WorkItem),
                                                        string.Format("{0:N0}", WorkMoney));

                //WorkMoney = CalWorkMoney(ModelData.BuildArea, FSys.EndType, FSys.SubEndType, FSys.FacType, endType.First().SS, endType.First().SL);
                //ModelData.Work = new BudgetBookstruct(((double)ModelData.BuildArea / 10000).ToString(), "", string.Format("{0:N0}", WorkMoney));
                TotalMoney += WorkMoney;
                #endregion

                #region Piping Total Cost(田間管路設施費)

                PipingMoney = TotalMoney;
                ModelData.PipingTotal = string.Format("{0:N0}", PipingMoney);
                #endregion

                #region Planning Cost //規劃設計費
                //PlanningMoney = (int)(PipingMoney * 0.02);
                

                #endregion
            }
            #endregion

            #region 規劃設計費
            PlanningMoney = pays.Any(m => m.ItemCode == 2) ? pays.Where(m => m.ItemCode == 2).FirstOrDefault().Total.Value : 0;
            ModelData.Planning = new BudgetBookstruct("1", "", string.Format("{0:N0}", PlanningMoney));
            TotalMoney += PlanningMoney;
            #endregion

            #region 調控設施
            int CntrlMoney = 0;
            int CtrlFarPay = 0;
            List<CntrlMat> MatData = getDataCls.GetCntrlMatData(_MNo);

            if (MatData != null)
            {
                
                List<CtrlTableView> ctblview = new List<CtrlTableView>();
                foreach (var matitem in MatData)
                {
                    ctblview.Add(new CtrlTableView { CntrlCode = matitem.CntrlCode, MatAmt = matitem.MatAmt, MatNo = matitem.MatNo, MatPrice = matitem.MatPrice ,MatAmtAply = matitem.MatAmtAply,MatPriceAply = matitem.MatPriceAply});
                }

                int[] price = CPSer.GetCtrlGoldHelpTotalPrice(ctblview.ToArray(), _MNo);
                CntrlMoney = price[1]+price[2]; 
                CtrlFarPay = price[3];
                                
                

                var VArobj = MatData.Select(m => m.CntrlCode).Distinct();
                string meno = string.Empty;
                int amount = 0;
                foreach (var item in VArobj)
                {
                    meno += mapingCls.GetCntrltypeName(item) + "#";
                    amount++;
                }
                
                //ModelData.RegulatedFac = new BudgetBookstruct(amount.ToString(), "", string.Format("{0:N0}", CntrlMoney), meno);
                ModelData.RegulatedFac = new BudgetBookstruct(amount.ToString(), string.Format("{0:N0}","" /*CtrlItemMoney*/), string.Format("{0:N0}", CntrlMoney), meno);
                TotalMoney += CntrlMoney;
            }
            #endregion

            #region 動力設備
            int EngineMoney = 0;
            var englist = getDataCls.GetEngineData(_MNo);
            if (englist != null)
            {
                List<EngineDBService.EngineData> engData = new List<EngineDBService.EngineData>();
                foreach(var item in englist)
                {
                    engData.Add(new EngineDBService.EngineData { EngineCode = item.EngCode, EngPrice = item.EngPrice, RegCode = item.EngRegCode });
                    
                }
                EngineMoney = calFund.GetEngMoneybyEngList(engData.ToArray(), CaseDta).GovPay;

                short[] EngAry = new short[] { 0, 0, 0, 0, 0, 0, 0 };

                foreach (var engitem in englist)
                {
                    EngAry[engitem.EngCode - 1]++;
                }
                string memo = string.Empty;
                for (short i = 0; i < (short)EngAry.Length; i++)
                {
                    if (EngAry[i] > 0)
                    {
                        memo += mapingCls.GetEngName((short)(i + 1)) + " * " + EngAry[i] + "#";
                    }
                }
                ModelData.Engine = new BudgetBookstruct(englist.Count.ToString(), "", string.Format("{0:N0}", EngineMoney), memo);

                TotalMoney += EngineMoney;
            }
            #endregion

            #region 蓄水池
            int PoolMoney = 0;
            if (FarmerPool != null)
            {
                ModelData.PoolWei = FarmerPool.Sum(p => p.PoolWeight);

                List<PoolDBService.PoolData> poldata = new List<PoolDBService.PoolData>();
                string memo = string.Empty;
                foreach (var politem in FarmerPool.OrderBy(p => p.PoolWeight))
                {
                    poldata.Add(new PoolDBService.PoolData { poolType = politem.PtypeCode, poolW = politem.PoolWeight });
                    memo += politem.PoolWeight + "頓#";
                }
                if (pays.Any(m => m.ItemCode == 6))
                {
                    PoolMoney = pays.Where(m => m.ItemCode == 6).FirstOrDefault().Total.Value;
                   
                }


                //PoolMoney = calFund.GetPoolMoneybyList(poldata.ToArray(), CaseDta, _MNo).GovPay;

                ModelData.Pool = new BudgetBookstruct(FarmerPool.Count.ToString(), "", string.Format("{0:N0}", PoolMoney), memo);
                TotalMoney += PoolMoney;
            }
                       

            #endregion

            #region 合計
            ModelData.Total = string.Format("{0:N0}", TotalMoney);


            //int farpay = piping_FarPay + Ctrl_FarPay;
            int farpay = totalFee.FarmerFee - CtrlFarPay; 
            ModelData.FarmerPay = string.Format("{0:N0}", farpay);

            
            //int govpay = (PipingMoney - piping_FarPay) + (CntrlMoney - Ctrl_FarPay) + EngineMoney + PoolMoney; //pay.Sum(p => p.PayMoney);
           

            if (pays.Any(m => m.ItemCode == 5 || m.ItemCode == 6))
            {
                foreach (var item in pays.Where(m => m.ItemCode == 5 || m.ItemCode == 6))
                {
                    if (item.ApplyUnit == 17)
                    {
                        LiuM += item.Total.Value;
                    }
                }
            }



            int govpay = totalFee.GovSubsidy - PlanningMoney - LiuM; 
            
            ModelData.GovPay_Pay = string.Format("{0:N0}", govpay);
            
            ModelData.LiuPay = string.Format("{0:N0}", LiuM);
            
            int goldpay = totalFee.GoldSubsidy;
            ModelData.GoldPay = string.Format("{0:N0}", goldpay);

            
            ModelData.GovPay_Planning = string.Format("{0:N0}", PlanningMoney);

            
            ModelData.GovPay_Sum = string.Format("{0:N0}", (govpay + goldpay + LiuM+ PlanningMoney));

            #endregion

            #region 國字總計
            ModelData.ChineseMoney = new NumberToChinese().GetChineseNumber(TotalMoney);
            #endregion

            return ModelData;
        }
        #endregion

        #region 瑠公水利會工程預算書
        /// <summary>
        /// 瑠公水利會工程預算書
        /// </summary>
        /// <param name="_MNo">版本編號</param>
        /// <returns>瑠公水利會預算書</returns>
        public BudgetBookView GetLiuGongBudgetBookData(int _MNo)
        {
            #region Get Data
            Case CaseDta = getDataCls.GetCaseDataFromMapNo(_MNo);
            Farmer farmer = getDataCls.GetFarmerData(_MNo);
            List<Dry.Models.CommonCls.GetData.NewFarm> Farm = getDataCls.GetFarmData(_MNo);
            Dry.Models.ViewModel.FarmerSysView.FarmerSysStruct FSys = getDataCls.GetFarmerSystemData(_MNo);
            PigingConf piping = getDataCls.GetPigingConfData(_MNo);
            List<FarmerSysM> FarmerSysMat = getDataCls.GetFarmerSysM(_MNo);
            List<Pool> FarmerPool = getDataCls.GetPoolData(_MNo);
            MappingClass mapingCls = new MappingClass();
            double area = (double)getDataCls.GetFarmBuildArea(_MNo) / 10000; 
            List<Pay> payList = getDataCls.GetPay(_MNo);
            TotalFee totalFee = getDataCls.GetTotalFee(_MNo);
            int PayMoney = 0; 
            int FarmerMoney = 0; 
            #endregion

            BudgetBookView ModelData = new BudgetBookView();

            #region Assign basic Data
            ModelData.Case = CaseDta.EventNo;
            ModelData.IANum = CaseDta.IANum;
            ModelData.ApplyY = CaseDta.ApplyYear;
            ModelData.Gold = CaseDta.Gold;
            ModelData.BuildArea = Farm == null ? 0 : Farm.Sum(f => f.BuildArea);
            ModelData.Name = farmer.Name;
            ModelData.Address = farmer.Addr;
            ModelData.Farm = Farm;
            ModelData.EndType = (FSys == null) ? "未申請末端設施" : FSys.FacName;
            ModelData.ApplyUnit = CaseDta.ApplyUnit;
            #endregion

            #region Show Data

            #region 田間管路材料費
            
            PayMoney = payList.Where(m => m.ItemCode == 1).Any() == false ? 0 : payList.Where(m => m.ItemCode == 1).First().PayMoney;
            FarmerMoney = payList.Where(m => m.ItemCode == 1).Any() == false ? 0 : payList.Where(m => m.ItemCode == 1).First().FarmerMoney;
            
            int PipingMoney = PayMoney + FarmerMoney; 
            ModelData.Liu_PipingMat = new BudgetBookstruct("1", "", string.Format("{0:N0}", PipingMoney));
            #endregion

            #region 動力設備費用
            var englist = getDataCls.GetEngineData(_MNo);
            
            PayMoney = payList.Where(m => m.ItemCode == 5).Any() == false ? 0 : payList.Where(m => m.ItemCode == 5).First().PayMoney;
            FarmerMoney = payList.Where(m => m.ItemCode == 5).Any() == false ? 0 : payList.Where(m => m.ItemCode == 5).First().FarmerMoney;
            int e_money = PayMoney + FarmerMoney; 
            //int e_money = getDataCls.GetEngineMoney(_MNo);

            string memo = string.Empty;
            if (englist != null)
            {
                short[] EngAry = new short[] { 0, 0, 0, 0, 0, 0, 0 };
                foreach (var engitem in englist)
                {
                    EngAry[engitem.EngCode - 1]++;
                }
                
                for (short i = 0; i < (short)EngAry.Length; i++)
                {
                    if (EngAry[i] > 0)
                    {
                        memo += mapingCls.GetEngName((short)(i + 1)) + " * " + EngAry[i] + "#";
                    }
                }
                ModelData.Liu_Engine = new BudgetBookstruct(englist.Count.ToString(), "", string.Format("{0:N0}", e_money), memo);
            }
            #endregion

            #region 蓄水設備費用
            int PoolCount = 0;
            memo = "";
            if (FarmerPool != null)
            {
                MappingClass mpg = new MappingClass();
                memo = string.Empty;
                foreach (var politem in FarmerPool.OrderBy(p => p.PoolWeight))
                {
                    memo += mpg.GetPooltypeName(politem.PtypeCode) + " * " + politem.PoolWeight + "t#";
                }
                PoolCount = FarmerPool.Count;
            }
            
            PayMoney = payList.Where(m => m.ItemCode == 6).Any() == false ? 0 : payList.Where(m => m.ItemCode == 6).First().PayMoney;
            FarmerMoney = payList.Where(m => m.ItemCode == 6).Any() == false ? 0 : payList.Where(m => m.ItemCode == 6).First().FarmerMoney;
            int pool_money = PayMoney + FarmerMoney; 
            
            ModelData.Liu_Pool = new BudgetBookstruct(PoolCount.ToString(), "", string.Format("{0:N0}", pool_money), memo);
            #endregion

            #region 工作費用
            
            PigingConf pigingConf = getDataCls.GetPigingConfData(_MNo);
            double ha = (double)ModelData.BuildArea / 10000;
            
            int wmoney = pigingConf == null ? 0 : pigingConf.WorkPrice; 
            ModelData.Liu_WorkM = new BudgetBookstruct(ha.ToString(), "", string.Format("{0:N0}", wmoney));
            #endregion

            #region 設施費總計
            int GovPay = totalFee == null ? 0 : totalFee.GovSubsidy; 
            int FarmerPay = totalFee == null ? 0 : totalFee.FarmerFee; 
            
            int Total_Money = GovPay + FarmerPay;
            
            ModelData.Liu_TotalMoney = string.Format("{0:N0}", Total_Money);
            #endregion
            #region 水利會補助款
            
            ModelData.Liu_IAMoney = string.Format("{0:N0}", GovPay);
            #endregion

            #region 農戶配合款
            //int ftmoney = Total_Money - aimoney;
            //ModelData.Liu_FarmerMoney = string.Format("{0:N0}", ftmoney);
            ModelData.Liu_FarmerMoney = string.Format("{0:N0}", FarmerPay);
            #endregion

            #region 小計
            ModelData.Liu_sum = string.Format("{0:N0}", Total_Money);
            #endregion

            #region 國字總計
            ModelData.Liu_ChineseMoney = new NumberToChinese().GetChineseNumber(Total_Money);
            #endregion

            #endregion

            return ModelData;
        }

        #endregion

        #region 計算田間管路設施之工作費
        private int CalWorkMoney(double area, int Endtype, int? SubEndType, int Factype, double ss, double sl)
        {
            
            double ha = (double)area / 10000;
            int w10 = 0;
            int Money = 0;
            switch (Endtype)
            {
                case 1:
                    //return (int)Math.Round(((double)area / 10000 * m), 0, MidpointRounding.AwayFromZero);
                    Money = (int)Math.Round(ha * 22000,0,MidpointRounding.AwayFromZero);
                    break;
                case 2:
                    
                    if (SubEndType.Value == 6)
                    {
                        Money = (int)Math.Round(ha * 49000, 0, MidpointRounding.AwayFromZero);
                        break;
                    }
                    w10 = 31000;
                    if (Factype == 2)
                        w10 = 55000;
                    Money = (int)Math.Round(w10 * (10 / Math.Sqrt(ss * sl)), 0, MidpointRounding.AwayFromZero);
                    break;
                case 3:
                    w10 = 62000;
                    if (Factype == 2)
                        w10 = 37000;
                    Money = (int)Math.Round(w10 * (5 / Math.Sqrt(ss * sl)), 0, MidpointRounding.AwayFromZero);
                    break;
                case 4:
                    if (SubEndType.Value == 7)
                        Money = (int)Math.Round(ha * 43000, 0, MidpointRounding.AwayFromZero); 
                    else
                        Money = (int)Math.Round(ha * 22000,0,MidpointRounding.AwayFromZero); 
                    break;
            }
            return Money;

            /*
            int m = 0;
            switch (Data.Endtype)
            {
                case 1://穿孔管                    
                    return (int)Math.Round(((double)area / 10000 * 22000), 0, MidpointRounding.AwayFromZero);
                case 2://噴頭
                    m = 55000;//default : 埋設固定(1)
                    if (Data.Fac == 2)//地表定置
                        m = 31000;
                    return (int)Math.Round((double)m * 10 / Math.Sqrt((double)Data.SS * Data.SL), 0, MidpointRounding.AwayFromZero);
                case 3://微噴
                    m = 62000;//default : 埋設固定(1) & 棚架式(3)
                    if (Data.Fac == 2)//地表定置
                        m = 37000;
                    return (int)Math.Round((double)m * 5 / Math.Sqrt((double)Data.SS * Data.SL), 0, MidpointRounding.AwayFromZero);
                case 7://滴嘴滴灌系統
                    return (int)Math.Round((double)area / 10000 * 43000, 0, MidpointRounding.AwayFromZero);
                case 8://滴水管滴灌系統
                    return (int)Math.Round((double)area / 10000 * 22000, 0, MidpointRounding.AwayFromZero);
            }
            return 0; 
            */
        }
        #endregion

        

        #region 計算調控設施補助費用
        private int CalCntrolMoney(int area, int total_money)
        {
            
            int MaxMoney = 0;
            int Money = Convert.ToInt32(Math.Floor(total_money * 0.49));

            MaxMoney = (int)Math.Ceiling(((double)area / 10000) * 50000);

            if (Money > MaxMoney)
                Money = MaxMoney;
            return Money;
        }
        #endregion

        #region 取得地目名稱
        public string GetLTypeName(byte type)
        {
            return getDataCls.GetLandTypeName(type).LandTypeCNS;
        }
        #endregion

        #region 取得農戶設施系統物料表
        public List<Dry.Models.CommonCls.GetData.SysMat> GetFarMat(int mapno)
        {
            return getDataCls.GetFarSysviaMapNo(mapno);
        }
        #endregion
        private List<FarmerSysView.PriceJsonData> FarmerSysMtoParaJsonData(List<FarmerSysM> data)
        {
            List<FarmerSysView.PriceJsonData> newData = new List<FarmerSysView.PriceJsonData>();
            foreach (var item in data)
            {
                newData.Add(new FarmerSysView.PriceJsonData
                {
                    POMNo = item.POMNo,
                    Amt = item.Amount
                });
            }
            return newData;
        }
        /// <summary>
        /// 取得農戶基本資料
        /// </summary>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public FarmerData GetFarmerData(int MapNo)
        {
            Farmer farmer = DryDB.VerMapping.Find(MapNo).Case.Farmer;
            FarmerData farmerdata = new FarmerData();
            farmerdata.FId = farmer.FId;
            farmerdata.IdNo = farmer.IdNo;
            farmerdata.Name = farmer.Name;
            farmerdata.CityCode = farmer.CityCode;
            if (farmer.Addr.Split('(').Count() > 1 && farmer.Addr.Split(')').Count() > 1)
            {
                farmer.Addr = farmer.Addr.Split('(')[0] + farmer.Addr.Split(')')[1];
            }
            farmerdata.Addr = farmer.Addr;
            farmerdata.Phone = farmer.Phone;
            farmerdata.Tel = farmer.Tel;
            farmerdata.IsMember = farmer.IsMember;
            farmerdata.CId = farmer.CId;
            farmerdata.CDate = farmer.CDate;
            farmerdata.UDate = farmer.UDate;
            farmerdata.ApplyYear = DryDB.VerMapping.Find(MapNo).Case.ApplyYear;
            farmerdata.IANum = DryDB.VerMapping.Find(MapNo).Case.IANum;
            var units = new AERC.Models.CommonCls.GetUints().GetUnitData(DryDB.VerMapping.Find(MapNo).Case.ApplyUnit);
            farmerdata.IAName = units.Unit1;

            return farmerdata;
        }
        /// <summary>
        /// 整理要匯出報表的資料(推廣旱作管路灌溉系統規劃委託書)
        /// </summary>
        /// <param name="MNo"></param>
        /// <returns></returns>
        public Dictionary<string,string> MergeExportProxyData(int MNo)
        {
            FarmerData farmer = GetFarmerData(MNo);

            Dictionary<string, string> dataStr = new Dictionary<string, string>();
            dataStr.Add("ApplyYear", farmer.ApplyYear.ToString());
            dataStr.Add("IA", farmer.IAName);
            dataStr.Add("FarmerName", farmer.Name);
            dataStr.Add("FarmerIDNo", farmer.IdNo);
            dataStr.Add("FarmerAddr", farmer.Addr);
            dataStr.Add("FarmerPhone", $"{farmer.Tel} {farmer.Phone}");
            dataStr.Add("IANum", farmer.IANum.ToString());
            return dataStr;
        }

        /// <summary>
        /// 整理要匯出報表的資料(接受補助設置旱作管路灌溉設施切結書、配合農時提前施設切結書)
        /// </summary>
        /// <param name="MNo"></param>
        /// <returns></returns>
        public Dictionary<string, string> MergeExportAcceptAffidavitData(int MNo)
        {
            GetData getData = new GetData();
            FarmerData farmer = GetFarmerData(MNo);
            Case FarmerCase = getData.GetCaseDataFromMapNo(MNo);
            Dictionary<string, string> dataStr = new Dictionary<string, string>();
            dataStr.Add("ApplyYear", FarmerCase.ApplyYear.ToString());
            dataStr.Add("ApplyUnit", getData.GetUnitName(FarmerCase.ApplyUnit));
            dataStr.Add("FarmerName", farmer.Name);
            dataStr.Add("FarmerID", farmer.IdNo);
            string AddrResult = "";
            foreach (string item in farmer.Addr.Split(' '))
            {
                AddrResult += item;
            }
            dataStr.Add("FarmerAddr", AddrResult);
            string TelAndPhone = "";
            TelAndPhone += farmer.Tel == null ? "" : farmer.Tel + "　";
            TelAndPhone += farmer.Phone == null ? "" : farmer.Phone;
            dataStr.Add("FarmerPhone", TelAndPhone);
            dataStr.Add("Eng", getData.GetEngineCountByPower(MNo).ToString());
            dataStr.Add("Pump", getData.GetEngineCountByPump(MNo).ToString());
            dataStr.Add("Pool", getData.GetPoolTon(MNo).ToString());
            dataStr.Add("DocketNo", FarmerCase.IANum.ToString());
            double area = (double)getData.GetFarmBuildArea(MNo) / 10000;
            EndType endType = new EndType();
            List<EndType> endTypeList = getData.GetEndTypeData(MNo);
            if (endTypeList != null)
                endType = endTypeList.FirstOrDefault();
            switch (endType == null ? 0 : endType.EndTypeCode)
            {
                case 1: 
                    dataStr.Add("Pipe", area.ToString());
                    dataStr.Add("Nozzle", "0");
                    dataStr.Add("Micro", "0");
                    dataStr.Add("Drip", "0");
                    break;
                case 6: 
                case 2: 
                    dataStr.Add("Pipe", "0");
                    dataStr.Add("Nozzle", area.ToString());
                    dataStr.Add("Micro", "0");
                    dataStr.Add("Drip", "0");
                    break;
                case 3: 
                    dataStr.Add("Pipe", "0");
                    dataStr.Add("Nozzle", "0");
                    dataStr.Add("Micro", area.ToString());
                    dataStr.Add("Drip", "0");
                    break;
                case 7: 
                case 8: 
                case 4: 
                    dataStr.Add("Pipe", "0");
                    dataStr.Add("Nozzle", "0");
                    dataStr.Add("Micro", "0");
                    dataStr.Add("Drip", area.ToString());
                    break;
                case 5: 
                    dataStr.Add("Pipe", "0");
                    dataStr.Add("Nozzle", "0");
                    dataStr.Add("Micro", "0");
                    dataStr.Add("Drip", "0");
                    break;
                default: 
                    dataStr.Add("Pipe", "0");
                    dataStr.Add("Nozzle", "0");
                    dataStr.Add("Micro", "0");
                    dataStr.Add("Drip", "0");
                    break;
            }

            string MatName = "", MatAmount = "";
            int countTmp = 0;
            var CtrlData = getData.GetCntrlMatData(MNo);
            if (CtrlData != null)
            {
                foreach (var item in CtrlData)
                {
                    MatName += item.MatName;
                    MatAmount += item.MatAmt;
                    if (countTmp < CtrlData.Count())
                    {
                        MatName += "<w:br />";
                        MatAmount += "<w:br />";
                    }
                    countTmp++;
                }
            }

            dataStr.Add("CtrlItem", MatName);
            dataStr.Add("CtrlAmount", MatAmount);
            return dataStr;
        }
        
        /// <summary>
        /// 整理要匯出報表的資料(接受補助設置旱作管路灌溉設施切結書、配合農時提前施設切結書)嘉南版
        /// </summary>
        /// <param name="MNo"></param>
        /// <returns></returns>
        public Dictionary<string,string> MergeExportAcceptAffidavitData(int MNo,string year ,string moon, string day)
        {
            GetData getData = new GetData();
            FarmerData farmer = GetFarmerData(MNo);
            Case FarmerCase = getData.GetCaseDataFromMapNo(MNo);
            BudgetBookView GOVPay = GetCoABudgetBookData(MNo);
            int pool, eng;
            
            int GovPayTotle = int.Parse(GOVPay.GovPay_Pay.ToString().Replace(",", ""));           
            int design = int.Parse(GOVPay.Planning.TotalPrice.ToString().Replace(",", ""));
            if (GOVPay.Pool !=  null)
            {
                string a = GOVPay.Pool.TotalPrice.ToString().Replace(",", "");
                pool = int.Parse(a);
            }
            else
            {
                pool = 0;
            }

            if (GOVPay.Engine != null)
            {
                string a = GOVPay.Engine.TotalPrice.ToString().Replace(",", "");
                eng = int.Parse(a);
            }
            else
            {
                eng = 0;
            }

            int PipePay = GovPayTotle /*- design*/ - pool - eng;
            Dictionary<string, string> dataStr = new Dictionary<string, string>();
            dataStr.Add("ApplyYear", FarmerCase.ApplyYear.ToString());
            dataStr.Add("ApplyUnit", getData.GetUnitName(FarmerCase.ApplyUnit));
            dataStr.Add("FarmerName", farmer.Name);
            dataStr.Add("FarmerID", farmer.IdNo);
            dataStr.Add("Year", year);
            dataStr.Add("Moon", moon);
            dataStr.Add("Day", day);
            string AddrResult = "";
            foreach (string item in farmer.Addr.Split(' '))
            {
                AddrResult += item;
            }
            dataStr.Add("FarmerAddr", AddrResult);
            string TelAndPhone = "";
            TelAndPhone += farmer.Tel == null ? "" : farmer.Tel + "　";
            TelAndPhone += farmer.Phone == null ? "" : farmer.Phone;
            dataStr.Add("FarmerPhone", TelAndPhone);
            dataStr.Add("Eng", getData.GetEngineCountByPower(MNo).ToString());
            dataStr.Add("Pump", getData.GetEngineCountByPump(MNo).ToString());
            dataStr.Add("Pool", getData.GetPoolTon(MNo).ToString());
            dataStr.Add("DocketNo", FarmerCase.IANum.ToString());
            double area = (double)getData.GetFarmBuildArea(MNo) / 10000;
            EndType endType = new EndType();
            List<EndType> endTypeList = getData.GetEndTypeData(MNo);
            if (endTypeList != null)
                endType = endTypeList.FirstOrDefault();
            switch (endType == null ? 0 : endType.EndTypeCode)
            {
                case 1: 
                    dataStr.Add("Pipe", area.ToString());
                    dataStr.Add("PipePrice", PipePay.ToString());
                    dataStr.Add("Nozzle", "0");
                    dataStr.Add("Micro", "0");
                    dataStr.Add("Drip", "0");
                    dataStr.Add("NozzlePrice", "0");
                    dataStr.Add("MicroPrice", "0");
                    dataStr.Add("DripPrice", "0");
                    break;
                case 6: 
                case 2: 
                    dataStr.Add("Pipe", "0");
                    dataStr.Add("PipePrice", "0");
                    dataStr.Add("Nozzle", area.ToString());
                    dataStr.Add("NozzlePrice", PipePay.ToString());
                    dataStr.Add("Micro", "0");
                    dataStr.Add("Drip", "0");
                    dataStr.Add("MicroPrice", "0");
                    dataStr.Add("DripPrice", "0");
                    break;
                case 3: 
                    dataStr.Add("Pipe", "0");
                    dataStr.Add("Nozzle", "0");
                    dataStr.Add("PipePrice", "0");
                    dataStr.Add("NozzlePrice", "0");
                    dataStr.Add("Micro", area.ToString());
                    dataStr.Add("MicroPrice", PipePay.ToString());
                    dataStr.Add("Drip", "0");
                    dataStr.Add("DripPrice", "0");
                    break;
                case 7: 
                case 8: 
                case 4: 
                    dataStr.Add("Pipe", "0");
                    dataStr.Add("Nozzle", "0");
                    dataStr.Add("Micro", "0");
                    dataStr.Add("PipePrice", "0");
                    dataStr.Add("NozzlePrice", "0");
                    dataStr.Add("MicroPrice", "0");
                    dataStr.Add("Drip", area.ToString());
                    dataStr.Add("DripPrice", PipePay.ToString());
                    break;
                case 5: 
                    dataStr.Add("Pipe", "0");
                    dataStr.Add("Nozzle", "0");
                    dataStr.Add("Micro", "0");
                    dataStr.Add("Drip", "0");
                    break;
                default: 
                    dataStr.Add("Pipe", "0");
                    dataStr.Add("Nozzle", "0");
                    dataStr.Add("Micro", "0");
                    dataStr.Add("Drip", "0");
                    dataStr.Add("PipePrice", "0");
                    dataStr.Add("NozzlePrice", "0");
                    dataStr.Add("MicroPrice", "0");
                    dataStr.Add("DripPrice", "0");
                    break;
            }

            string MatName = "", MatAmount = "";
            int countTmp = 0;
            var CtrlData = getData.GetCntrlMatData(MNo);
            if (CtrlData != null)
            {
                foreach (var item in CtrlData)
                {
                    MatName += item.MatName;
                    MatAmount += item.MatAmt;
                    if (countTmp < CtrlData.Count())
                    {
                        MatName += "<w:br />";
                        MatAmount += "<w:br />";
                    }
                    countTmp++;
                }
            }

            dataStr.Add("CtrlItem", MatName);
            dataStr.Add("CtrlAmount", MatAmount);
            dataStr.Add("EngMoney", eng.ToString());
            dataStr.Add("PoolMoney", pool.ToString());
            return dataStr;
        }
        /// <summary>
        /// 整理要匯出報表的資料(竣工報驗書)
        /// </summary>
        /// <param name="MNo"></param>
        /// <returns></returns>
        public Dictionary<string,string > MergeExportCompleteData(int MNo)
        {
            GetData getData = new GetData();
            FarmerData farmer = GetFarmerData(MNo);
            Case FarmerCase = getData.GetCaseDataFromMapNo(MNo);
            List<Farm> farms = getData.GetFarmDataByMNo(MNo);
            var TownData = new AERC.Models.CommonEntities().Town.ToList();
            var SectionData = new AERC.Models.CommonEntities().Section.ToList();
            string townName = "", sectionName = "", subsectionName = "", landNo = "", cityName = "";
            double landArea = 0;
            if(farms.Count>0)
            {
                var farmData = from farm in farms
                               join section in SectionData on farm.Section equals section.Section_Id
                               join town in TownData on section.Town_Id equals town.Town_Id
                               select new
                               {
                                   town.City.City1,
                                   town.Town1,
                                   section.Section1,
                                   section.Subsection,
                                   farm.LandNo,
                                   farm.BuildArea
                               };

                if (/*farmData != null*/ farmData.Count() > 0)
                {
                    sectionName += farmData.FirstOrDefault().Section1 == null ? "" : farmData.FirstOrDefault().Section1;
                    landNo += farmData.FirstOrDefault().LandNo == null ? "" : farmData.FirstOrDefault().LandNo;
                    townName += farmData.FirstOrDefault().Town1 == null ? "" : farmData.FirstOrDefault().Town1;
                    subsectionName += farmData.FirstOrDefault().Subsection;
                    cityName += farmData.FirstOrDefault().City1 == null ? "" : farmData.FirstOrDefault().City1;
                    foreach (var item in farmData)
                    {
                        landArea += item.BuildArea;
                    }
                }
            }
            
            
            Dictionary<string, string> dataStr = new Dictionary<string, string>();
            dataStr.Add("ApplyYear", FarmerCase.ApplyYear.ToString());
            dataStr.Add("Town", townName);
            dataStr.Add("Section", sectionName);
            dataStr.Add("SubSection", subsectionName == "" ? "　　　" : subsectionName);
            dataStr.Add("LandNo", landNo);
            dataStr.Add("LandAmount", farms.Count().ToString());
            dataStr.Add("LandArea", ((double)landArea / 10000).ToString());
            dataStr.Add("ApplyUnit", getData.GetUnitName(FarmerCase.ApplyUnit));
            dataStr.Add("DocketNo", FarmerCase.IANum.ToString());
            dataStr.Add("FarmerName", farmer.Name);
            dataStr.Add("City", cityName);
            string AddrResult = "";
            foreach (string item in farmer.Addr.Split(' '))
            {
                AddrResult += item;
            }
            dataStr.Add("FarmerAddr", AddrResult);
            dataStr.Add("FarmerTel", farmer.Tel + "　" + farmer.Phone);
            return dataStr;
        }
    }
}
