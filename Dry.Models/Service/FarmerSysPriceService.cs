/*
-- =============================================
-- Author: WEI, HaoHsuan
-- Create date: 2014-6-24
-- Description: 計算田間管路系統經費及輔助費用
-- =============================================
*/
using Dry.Models.CommonCls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;


namespace Dry.Models.Service
{
    public class FarmerSysPriceService
    {
        FarmerSysDBService farmersys_db = new FarmerSysDBService();
        Calculate_Funding CalFund = new Calculate_Funding();
        GetData GData = new GetData();
        private DryEntities DryDB = new DryEntities();

        #region 計算田間管路所需經費及輔助費用(一般輔助) return [所需經費, 輔助費, 自備款]
        /// <summary>
        /// 計算田間管路所需經費及輔助費用(一般輔助) return [所需經費, 輔助費, 自備款]
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public int[] GetHelpTotalPrice(FarmerSysView.ParaJsonData Data, int MapNo)
        {
            int area = GData.GetFarmBuildArea(MapNo);
            int[] Price = new int[3] { 0, 0, 0 };
            int CasePrice = CalFund.Remaining_funds(/*3*/5, MapNo, GData.GetCaseDataFromMapNo(MapNo)); 
            
            
            Price[0] = GetPrice(Data, MapNo, area, CasePrice);

            
            Data.TotalPrice = Price[0];

            int hMoney = GetHelpPrice(Data, area, MapNo); 
            Price[1] = hMoney;

            
            Price[1] = Price[1] > CasePrice ? CasePrice : Price[1];
            
            Price[2] = Price[0] - Price[1];
            return Price;
        }
        #endregion

        #region 計算田間管路所需經費及輔助費用(黃金廊道) return [所需經費, 輔助費(農委會), 輔助費(七星), 自備款]
        /// <summary>
        /// 計算田間管路所需經費及輔助費用(黃金廊道) return [所需經費, 輔助費(農委會), 輔助費(七星), 自備款]
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public int[] GetGoldHelpTotalPrice(FarmerSysView.ParaJsonData Data, int MapNo)
        {
            int area = GData.GetFarmBuildArea(MapNo);
            int[] Price = new int[4] { 0, 0, 0, 0 };
            int CasePrice = CalFund.Remaining_funds(/*3*/5, MapNo, GData.GetCaseDataFromMapNo(MapNo)); 
            
            Price[0] = GetPrice(Data, MapNo, area, CasePrice);

            Data.TotalPrice = Price[0];
            
            int[] GoldMoney = new int[2];
            GoldMoney = GetGoldHelpPrice(Data, area, MapNo);
            Price[1] = GoldMoney[0]; 
            Price[2] = GoldMoney[1]; 
            
            Price[1] = Price[1] > CasePrice ? CasePrice : Price[1];
            
            Price[3] = Price[0] - Price[1] - Price[2];
            return Price;
        }
        #endregion

        #region 取得 輔助費用(一般輔助)
        /// <summary>
        /// 取得 輔助費用(一般輔助)
        /// </summary>
        /// <param name="Data">田間管路系統資料</param>
        /// <param name="area">面積</param>
        /// <param name="MapNo">版本編號</param>
        /// <returns></returns>
        public int GetHelpPrice(FarmerSysView.ParaJsonData Data, int area, int MapNo)
        {
            Case cse = GData.GetCaseDataFromMapNo(MapNo);
            
            //if (cse.ApplyUnit == 17)//瑠公
            //{
            //    return CalFund.CalLiuIrrMoney(area, Data.TotalPrice, MapNo, Data.EndTypeDataAry.First().Endtype);
            //}
            //else //農委會
            //{
            //    bool applied = GData.ChkAppliedByMaoNo(MapNo);
            
            //    bool isGold = cse.Gold;
            //    return CalFund.CalIrrMoney(isGold, applied, area, Data.EndTypeDataAry.First().Endtype, Data.EndTypeDataAry.First().Fac, Data.EndTypeDataAry.First().SS, Data.EndTypeDataAry.First().SL, Data.TotalPrice, cse);
            //}
            bool applied = GData.ChkAppliedByMaoNo(MapNo);
            
            bool isGold = cse.Gold;
            return CalFund.CalIrrMoney(isGold, applied, area, Data.EndTypeDataAry.First().Endtype, Data.EndTypeDataAry.First().Fac, Data.EndTypeDataAry.First().SS, Data.EndTypeDataAry.First().SL, Data.TotalPrice, cse);            


            /*
            switch (Data.Unit)//補助來源選擇
            {
                case 0://農委會
                    return CalFund.CalIrrMoney(false, area, Data.Endtype, Data.Fac, Data.SS, Data.SL, Data.TotalPrice);
                case 16://七星
                    return CalFund.CalIrrMoney(false, area, Data.Endtype, Data.Fac, Data.SS, Data.SL, Data.TotalPrice);, //Data.Drop);
                case 17://瑠公
                    //return CalFund.CalIrrMoney(area, Data.TotalPrice);
            }
            return 0;
            */
        }
        #endregion

        #region 輔助費用(黃金廊道) return [農委會, 七星]
        /// <summary>
        /// 輔助費用(黃金廊道) return [農委會, 七星]
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public int[] GetGoldHelpPrice(FarmerSysView.ParaJsonData Data, int area, int MapNo)
        {
            int[] Price = new int[2];
            bool applied = GData.ChkAppliedByMaoNo(MapNo);
            Case casedata = GData.GetCaseDataFromMapNo(MapNo);
            
            Price[0] = CalFund.CalIrrMoney(casedata.Gold,applied, area, Data.EndTypeDataAry.First().Endtype, Data.EndTypeDataAry.First().Fac, Data.EndTypeDataAry.First().SS, Data.EndTypeDataAry.First().SL, Data.TotalPrice, casedata); //農委;
            Price[1] = CalFund.SevenCalIrrMoney(Data.TotalPrice, Price[0]); 
            
            //Price[0] = 0; //農委;
            //Price[1] = CalFund.CalIrrMoney(true, applied, area, Data.EndTypeDataAry.First().Endtype, Data.EndTypeDataAry.First().Fac, Data.EndTypeDataAry.First().SS, Data.EndTypeDataAry.First().SL, Data.TotalPrice, casedata); //七星
            return Price;
        }
        #endregion

        #region 所需經費(主管 + 物料 + 工作費(瑠公未加))
        /// <summary>
        /// 所需經費(主管費用 + 系統物料費用 + 工作費(瑠公未加))
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <param name="area"></param>
        /// <param name="CasePrice">案件可用經費</param>
        /// <returns></returns>
        public int GetPrice(FarmerSysView.ParaJsonData Data, int MapNo, int area, int CasePrice)
        {
            Case cse = GData.GetCaseDataFromMapNo(MapNo);
            double ha = (double)area / 10000;

            int Price = 0;
            if (Data.PriceJsonDataAry != null)
            {
                
                Price = GetTotalPrice(Data.PriceJsonDataAry.ToList<FarmerSysView.PriceJsonData>(), cse.ApplyUnit, cse.ApplyYear); 
                if (cse.ApplyUnit == 17)
                {
                }
                else 
                {
                    

                    
                    //int matTotalPrice = (int)(Math.Round(Price * ha, 0, MidpointRounding.AwayFromZero));
                    //Price = matTotalPrice;
                }
            }

            if (Data.MainJsonDataAry != null)
            {
                int mainMatPrice = GetMainPipePrice(Data.MainJsonDataAry.ToList<FarmerSysView.MainJsonData>(), cse.ApplyUnit, cse.ApplyYear);
                Price += mainMatPrice;
            }

            
            if (Price > 0) 
            {
                
                //if (cse.ApplyUnit == 17)//瑠公
                //{
                //    //Price += GetLiuWorkPrice(area);                
                //}
                //else {
                //    Price += GetWorkPrice(Data, area, MapNo);
                //}

                Price += GetWorkPrice(Data, area, MapNo);

                /*
                int getpric = GetWorkPrice(Data, area);
                Price += getpric;
                switch (Data.Unit)
                {
                    case 0: //農委會
                        int getpric = GetWorkPrice(Data, area);
                        Price += getpric;
                        break;
                    case 17: //瑠公
                        Price += GetLiuWorkPrice(area);
                        break;
                }*/
            }
            return Price;
        }
        /// <summary>
        /// 所需經費(系統物料費用)-未加工作費
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="MapNo"></param>
        /// <param name="area"></param>
        /// <param name="CasePrice"></param>
        /// <returns></returns>
        public int GetPriceHadNoWorkPrice(int MapNo)
        {

            int Price = 0;
            
            Price = GetTotalPriceFromDB(MapNo); //所需經費

            return Price;
        }
        #endregion

        #region 取得 物料經費
        /// <summary>
        /// 物料經費(灌溉系統)
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public int GetTotalPrice(List<FarmerSysView.PriceJsonData> Data, short Unit, short y)
        {
            
            double totalPrice = 0;
            foreach (var item in Data)
            {
                //int unt = Unit;
                //var matPriceData = farmersys_db.GetMatPrice(y, unt, item.POMNo);
                //totalPrice += (matPriceData == null ? 0 : matPriceData.Price) * item.Amt;
                totalPrice += item.TotalPrice;
            }
            return (int)Math.Round(totalPrice, 0, MidpointRounding.AwayFromZero);
        }
        public int GetTotalPriceFromDB(int MapNo)
        {
            int Total = 0;
            DryEntities DryDB = new DryEntities();
            List<FarmerSys> farSys = DryDB.FarmerSys.Where(m => m.MapNo == MapNo).ToList();
            foreach(var item in farSys)
            {
                var farm = DryDB.FarmerSysM.Where(m => m.FarSysNo == item.FarSysNo);
                //Total += (int)Math.Ceiling(farm.Count() == 0 ? 0 : farm.Sum(m => m.TotalPrice).Value);
                Total += (int)Math.Round(farm.Count() == 0 ? 0 : farm.Sum(m => m.TotalPrice).Value, 0, MidpointRounding.AwayFromZero);
            }

            //Total = DryDB.Pay.Where(m => m.MapNo == MapNo && m.ItemCode == 1).Sum(m => m.Total).Value;
            return Total;
        }
        public int GetMainPipePriceFromDB(int MapNo, int L1orL2)
        {
            DryEntities DryDB = new DryEntities();
            int MPrice = 0;
            PigingConf pig = DryDB.PigingConf.Find(MapNo);
            if (pig != null)
            {
                if (L1orL2 == 1)
                {
                    MPrice = (int)Math.Round(pig.L1Amount * pig.L1Price, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    MPrice = (int)Math.Round(pig.L2Amount * pig.L2Price, 0, MidpointRounding.AwayFromZero);
                }
            }

            return MPrice;
        }
        #endregion

        #region 取得 主管費用(農委會&瑠公)
        /// <summary>
        /// 主管費用(農委會&瑠公)
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Unit">輔助單位代碼</param>
        /// <returns></returns>
        public int GetMainPipePrice(List<FarmerSysView.MainJsonData> Data, short Unit, short y)
        {
            int totalPrice = 0;
            int unt = Unit;
            #region old code
            //foreach (var item in Data)
            //{
            //    if (item.Amount != null)
            //    {
            //        var matPriceData = farmersys_db.GetMatPrice(y, unt, item.Mat);
            //        totalPrice += ((int)Math.Round(matPriceData == null ? 1 : matPriceData.Price, 0, MidpointRounding.AwayFromZero) * item.Amount);
            //    }
            //}
            #endregion
            
            
            foreach (var item in Data)
            {
                if (item.Amount > 0)
                {
                    totalPrice += (int)Math.Round(item.LPrice * item.Amount, 0, MidpointRounding.AwayFromZero);
                }
            }
            return totalPrice;
        }
        #endregion

        #region 農委會工作費
        /// <summary>
        /// 工作費(農委會)
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="area"></param>
        /// <param name="mno"></param>
        /// <returns></returns>
        public int GetWorkPrice(FarmerSysView.ParaJsonData Data, int area, int mno)
        {
            int m = 0;
            if (/*Data.EndTypeDataAry.First().SS > 0 &&*/ Data.EndTypeDataAry.First().SL > 0)
            {
                
                Case caseData = GData.GetCaseDataFromMapNo(mno);
                SubsidyLimit subLimit = GData.GetSubsidyLimit(caseData);
                PigingLimit pigLimit = GData.GetPigingLimit(caseData, Data.EndTypeDataAry.FirstOrDefault().Endtype, Data.EndTypeDataAry.FirstOrDefault().Fac);
                m = pigLimit == null ? 0 : pigLimit.WorkingFee;
                double workPrice = 0;
                switch (Data.EndTypeDataAry.First().Endtype)
                {
                    case 1:

                        return (int)Math.Round(((double)area / 10000 * m), 0, MidpointRounding.AwayFromZero);
                    case 2:
                        //m = 55000;
                        //if (Data.EndTypeDataAry.First().Fac == 2)
                        //m = 31000;
                         workPrice = Math.Round((double)m * 10 / Math.Sqrt((double)Data.EndTypeDataAry.First().SS * Data.EndTypeDataAry.First().SL),0,MidpointRounding.AwayFromZero);
                        return (int)Math.Round( workPrice * ((double)area / 10000), 0, MidpointRounding.AwayFromZero);
                    case 3:
                        //m = 62000;
                        //if (Data.EndTypeDataAry.First().Fac == 2)
                        //m = 37000;
                        workPrice = Math.Round((double)m * 5 / Math.Sqrt((double)Data.EndTypeDataAry.First().SS * Data.EndTypeDataAry.First().SL), 0, MidpointRounding.AwayFromZero);
                        return (int)Math.Round( workPrice * ((double)area / 10000), 0, MidpointRounding.AwayFromZero);
                    case 6: 
                        return (int)Math.Round((double)area / 10000 * m, 0, MidpointRounding.AwayFromZero);
                    case 7:
                        return (int)Math.Round((double)area / 10000 * m, 0, MidpointRounding.AwayFromZero);
                    case 8:
                        return (int)Math.Round((double)area / 10000 * m, 0, MidpointRounding.AwayFromZero);
                }
            }
            return 0;
        }

        #region 取得工作費原始值(不乘上面積)
        public int GetWorkPriceOrg(int mno)
        {
            int m = 0;
            Case caseData = GData.GetCaseDataFromMapNo(mno);
            SubsidyLimit subLimit = GData.GetSubsidyLimit(caseData);
            
            List<EndType> endType = DryDB.EndType.Where(p => p.MapNo == mno).ToList();
            if (endType.Count == 0)
            {
                return 0;
            }
            else
            {
                //if (endType.First().SS > 0 && endType.First().SL > 0)
                //{

                    PigingLimit pigLimit = GData.GetPigingLimit(caseData, endType.First().EndTypeCode, endType.First().FacType);
                    m = pigLimit == null ? 0 : pigLimit.WorkingFee;
                    switch (endType.First().EndTypeCode)
                    {
                        case 1: 
                            return m;
                        case 2:
                            if (endType.First().SS > 0 && endType.First().SL > 0)
                            {
                                return (int)Math.Round((double)m * 10 / Math.Sqrt((double)endType.First().SS * endType.First().SL), 0, MidpointRounding.AwayFromZero);
                            }
                            else { return 0; }
                        case 3:
                            if (endType.First().SS > 0 && endType.First().SL > 0)
                            {
                                return (int)Math.Round((double)m * 5 / Math.Sqrt((double)endType.First().SS * endType.First().SL), 0, MidpointRounding.AwayFromZero);
                            }
                            else { return 0; }
                        case 6: 
                            return m;
                        case 7:
                            return m;
                        case 8:
                            return m;
                        default:
                            return 0;
                    }
                //}
                //else
                //{
                //    return 0;
                //}
            }
           
        }
        #endregion 取得工作費原始值

        #endregion
        /// <summary>
        /// 農委會-規劃設計費
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public int GetPlanningPrice(int TotalPrice)
        {
            return (int)Math.Round(TotalPrice * 0.02, 0, MidpointRounding.AwayFromZero);
        }
        public int GetPlanningPrice(int TotalPrice, int unitid,int applyyear)
        {
            SubsidyLimit subsidyLimit = DryDB.SubsidyLimit.Where(s => s.ApplyYear == applyyear && s.ApplyUnit == unitid).FirstOrDefault();
            
            return (int)Math.Round(TotalPrice * /*0.02*/ subsidyLimit.PlanningFee / 100 , 0, MidpointRounding.AwayFromZero);
        }


        #region 瑠公水利會工作費
        /// <summary>
        /// 工作費(瑠公)
        /// </summary>
        /// <param name="area"></param>
        /// <param name="mno"></param>
        /// <param name="EndType"></param>
        /// <param name="FacType"></param>
        /// <returns></returns>
        public int GetLiuWorkPrice(int area,int mno,byte EndType,byte FacType)
        {
            
            Case caseData = GData.GetCaseDataFromMapNo(mno);
            SubsidyLimit subLimit = GData.GetSubsidyLimit(caseData);
            PigingLimit pigLimit = GData.GetPigingLimit(caseData, EndType, FacType);
            return (int)Math.Round(((double)area / 10000) * pigLimit.WorkingFee, 0, MidpointRounding.AwayFromZero); 
        }
        #endregion
    }
}
