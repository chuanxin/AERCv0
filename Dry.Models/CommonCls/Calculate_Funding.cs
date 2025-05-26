/*
-- =============================================
-- Author: HaoHsuan
-- Create date: 2014-4-12
-- Description: 計算經費
-- =============================================
*/
//
//                       _oo0oo_
//                      o8888888o
//                      88" . "88
//                      (| -_- |)
//                      0\  =  /0
//                    ___/`---'\___
//                  .' \\|     |// '.
//                 / \\|||  :  |||// \
//                / _||||| -:- |||||- \
//               |   | \\\  -  /// |   |
//               | \_|  ''\---/''  |_/ |
//               \  .-\__  '-'  ___/-. /
//             ___'. .'  /--.--\  `. .'___
//          ."" '<  `.___\_<|>_/___.' >' "".
//         | | :  `- \`.;`\ _ /`;.`/ - ` : | |
//         \  \ `_.   \_ __\ /__ _/   .-` /  /
//     =====`-.____`.___ \_____/___.-`___.-'=====
//                       `=---='
//
//
//     ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
//               佛祖保佑         永無BUG


using Dry.Models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.ViewModel;

namespace Dry.Models.CommonCls
{
    public class Calculate_Funding
    {
        private GetData getDataCls = new GetData();

        #region 取得動力設施總補助金額
        /// <summary>
        /// 取得動力設施總補助金額
        /// </summary>
        /// <param name="engary">動力設施列表</param>
        /// <param name="casedata">案件資料</param>
        /// <returns>總補助金額</returns>
        public MoneyData GetEngMoneybyEngList(EngineDBService.EngineData[] engary, Case casedata)
        {
            int money = 0;
            if (engary != null)
            {
                foreach (var eng in engary)
                {                    
                    money += eng.EngPrice;                  

                }
            }
            int govM = money;
            int farmM = 0;            
            return new MoneyData { GovPay = govM, FarmerPay = farmM };
        }
        #endregion

        #region 取得動力設施總補助金額for 黃金廊道
        /// <summary>
        /// 取得動力設施總補助金額
        /// </summary>
        /// <param name="engary">動力設施列表</param>
        /// <param name="casedata">案件資料</param>
        /// <returns>總補助金額</returns>
        public MoneyData GetEngMoneybyEngListGold(EngineDBService.EngineData[] engary, Case casedata)
        {
            int money = 0;
            int goldm = 0;
            if (engary != null)
            {
                foreach (var eng in engary)
                {
                    money += eng.EngPrice;
                    goldm += eng.EngPrice - getDataCls.GetEngPriceStdFromGold(eng.EngPrice);
                }
            }
            int govM = money - goldm;
            
            int farmM = 0;
            if (casedata.ApplyUnit == 17)
            {
                int MaxSubsidy = getDataCls.GetSubsidyLimit(casedata).EngineMaxSubsidy;
                if (govM > MaxSubsidy)
                {
                    govM = MaxSubsidy;
                    farmM = money - govM;
                }
            }
            return new MoneyData { GovPay = govM, FarmerPay = farmM, GoldPay = goldm };
        }
        #endregion

        #region 取得單一動力設施之補助金額
        /// <summary>
        /// 取得單一動力設施之補助金額(預設價錢)
        /// </summary>
        /// <param name="EngineCode">動力引擎代碼</param>
        /// <param name="casedata">案件資料</param>
        /// <returns>補助金額</returns>
        public int GetengineMoney(byte EngineCode, Case casedata)
        {
            short money = 0;
            if (casedata.ApplyUnit == 17)
            {
                
                #region old code
                //switch (EngineCode)
                //{
                //    //馬達含抽水機
                //    case 1: money = 10000;
                //        break;
                //    //柱塞式泵(馬達+柱塞泵)
                //    case 2: money = 12000;
                //        break;
                //    //汽油引擎
                //    case 3: money = 13000;
                //        break;
                //    //柴油引擎
                //    case 4: money = 30000;
                //        break;
                //}
                #endregion
                money = (short)getDataCls.GetEngPriceByCase(casedata, EngineCode);
            }
            else
            {
                if (casedata.Gold)
                {
                    switch (EngineCode)
                    {
                        
                        case 1: money = 5700;
                            break;
                        
                        case 2: money = 8500;
                            break;
                        
                        case 3: money = 8500;
                            break;
                        
                        case 4: money = 15700;
                            break;
                    }
                }
                else
                {
                    switch (EngineCode)
                    {
                        
                        case 1: money = 4000;
                            break;
                        
                        case 2: money = 6000;
                            break;
                        
                        case 3: money = 6000;
                            break;
                        
                        case 4: money = 11000;
                            break;
                    }
                }
            }
            money = (short)getDataCls.GetEngPriceByCase(casedata, (byte)EngineCode);
            return money;
        }
        #endregion

        #region 取得調蓄設施總補助金額
        /// <summary>
        /// 取得調蓄設施總補助金額
        /// </summary>
        /// <param name="PoolAry">調蓄設施列表</param>
        /// <param name="casedata">案件資料</param>
        /// <returns>總補助金額</returns>
        public MoneyData GetPoolMoneybyList(PoolDBService.PoolData[] PoolAry, Case casedata, int MapNo)
        {
            double ha = (double)getDataCls.GetFarmBuildArea(MapNo) / 10000;
            
            int money = PoolAry == null ? 0 : PoolAry.Sum(m => m.poolPrice);
            
            int govM = money; 
            int farmM = 0; 
            
            SubsidyLimit Subsidy = getDataCls.GetSubsidyLimit(casedata);
           

            if (ha < 0.1) 
                govM = 0;

            
            int goldM = 0;
            
            return new MoneyData { GovPay = govM, FarmerPay = farmM, GoldPay = goldM };
        }
        #endregion

        /// <summary>
        /// 取得黃金廊道補助之前的蓄水池金額
        /// </summary>
        /// <param name="money">蓄水池金額</param>
        /// <returns>補助金額</returns>
        public int GetpoolMoneyNoGold(int money){
            int orgmoney = 0;
            switch (money)
            {
                case 17000:
                    orgmoney = 12000;
                    break;
                case 24000:
                    orgmoney = 17000;
                    break;
                case 30000:
                    orgmoney = 21000;
                    break;
                case 35000:
                    orgmoney = 25000;
                    break;
                case 42000:
                    orgmoney = 30000;
                    break;
                case 51000:
                    orgmoney = 36000;
                    break;
                case 64000:
                    orgmoney = 45000;
                    break;
                case 78000:
                    orgmoney = 55000;
                    break;
                case 85000:
                    orgmoney = 60000;
                    break;
                default :
                    orgmoney = money;
                    break;
            }
            return orgmoney;
        }


        #region 取得單一蓄水池之補助金額
        /// <summary>
        /// 取得單一蓄水池之補助金額
        /// </summary>
        /// <param name="PoolCode">蓄水池類型</param>
        /// <param name="weight">蓄水池容量</param>
        /// <param name="casedata">案件資料</param>
        /// <returns>補助金額</returns>
        public int GetpoolMoney(byte PoolCode, int weight, Case casedata, int MapNo)
        {
            int limitid = getDataCls.GetSubsidyLimit(casedata).LimitID;
            int money = 0;
            
                double ha = (double)getDataCls.GetFarmBuildArea(MapNo) / 10000;
                //蓄水池噸數補助基準
                if (ha < 0.3) { weight = weight < 50/*30*/ ? weight : 50/*30*/; }
                //else if (ha >= 0.3 && ha < 0.7) { weight = weight < 50 ? weight : 50; }
                //else if (ha >= 0.7 && ha < 1) { weight = weight < 70 ? weight : 70; }
                //else if (ha >= 1 && ha < 1.5) { weight = weight < 90 ? weight : 90; }
                //else if (ha >= 1.5) { weight = weight < 100 ? weight : 100; }
                else weight = weight < 100 ? weight : 100;
            if (casedata.Gold)
                {
                    money = GetPoolPriceByWeightGold(weight);                    
                }
                else
                {
                    money = GetPoolPriceByWeight(weight, casedata.ApplyUnit);                 
                }
            //}
            return money;
        }
        #endregion

        /// <summary>
        /// 利用蓄水池噸數計算補助金額
        /// <param name="weight">池塘噸數
        /// <returns>蓄水池補助金額</returns>
        public int GetPoolPriceByWeight2018(int weight, int unit)
        {
            
            if (weight < 10) 
            {
                if (weight == 9)
                    return 10800;
                else if (weight == 8)
                    return 9600;
                else if (weight == 7)
                    return 8400;
                else if (weight == 6)
                    return 7200;
                else if (weight == 5)
                    return 6000;
                else
                    return 0;             
            }

            int[] poolprice = { 12000, 17000, 21000, 25000, 30000, 36000, 45000, 55000, 60000, 70000  };
            if (unit == 19)
            {
                poolprice[0] = 23000;
            }
            
                
            

            if (weight % 10 == 0)
            {
                int pindex = (weight /10) -1;
                return poolprice[pindex];

            }
            else
            {
                int pindex1 = (int)(weight / 10) - 1;
                int pindex2 = pindex1 + 1;

                int calcpool;
                double pdelta = (weight % 10) / 10.0;
                double deltapool = pdelta * ( poolprice[pindex2] - poolprice[pindex1]);
                calcpool = poolprice[pindex1] + (int)deltapool;
                return calcpool;

            }

        }

        public int GetPoolPriceByWeight(int weight, int unit)
        {
            return 6000 + weight * 800;
        }
        /// <summary>
        /// 利用蓄水池噸數計算補助金額(黃金廊道)
        /// <param name="weight">池塘噸數
        /// <returns>蓄水池補助金額</returns>
        public int GetPoolPriceByWeightGold(int weight)
        {
            if (weight < 10)
            {
                if (weight == 9)
                    return 15300;
                else if (weight == 8)
                    return 13600;
                else if (weight == 7)
                    return 11900;
                else if (weight == 6)
                    return 10200;
                else if (weight == 5)
                    return 8500;
                else
                    return 0;
            }
            int[] poolprice = { 17000, 24000, 30000, 36000, 43000, 51000, 64000, 78000, 85000, 100000 };
            //計算區間

            if (weight % 10 == 0)
            {
                int pindex = (weight / 10) - 1;
                return poolprice[pindex];

            }
            else
            {
                int pindex1 = (int)(weight / 10) - 1;
                int pindex2 = pindex1 + 1;

                int calcpool;
                double pdelta = (weight % 10) / 10.0;
                double deltapool = pdelta * (poolprice[pindex2] - poolprice[pindex1]);
                calcpool = poolprice[pindex1] + (int)deltapool;
                return calcpool;

            }

        }

        /// <summary>
        /// 利用蓄水池噸數計算補助金額(黃金廊道)
        /// <param name="weight">池塘噸數
        /// <returns>蓄水池補助金額</returns>
        public int GetPoolPriceByWeightYear2018(int weight,int years)
        {
            if (weight < 10 && years == 15)
            {
                if (weight == 9)
                    return 4900;
                else if (weight == 8)
                    return 4300;
                else if (weight == 7)
                    return 3800;
                else if (weight == 6)
                    return 3200;
                else if (weight == 5)
                    return 2700;
                else
                    return 0;
            }
            else if (weight < 10 && years == 20)
            {
                if (weight == 9)
                    return 7300;
                else if (weight == 8)
                    return 6500;
                else if (weight == 7)
                    return 5700;
                else if (weight == 6)
                    return 4900;
                else if (weight == 5)
                    return 4100;
                else
                    return 0;
            }

            int[] poolprice15 = { 5400, 7400, 9400, 11400, 13600, 15600, 19400, 24400, 26400, 30400 };
            int[] poolprice20 = { 8100, 11100, 14100, 17100, 20400, 23400, 29100, 36600, 39600, 45600 };
            int[] poolprice;
            if (years == 15) poolprice = poolprice15;
            else poolprice = poolprice20;

            //計算區間

            if (weight % 10 == 0)
            {
                int pindex = (weight / 10) - 1;
                return poolprice[pindex];

            }
            else
            {
                int pindex1 = (int)(weight / 10) - 1;
                int pindex2 = pindex1 + 1;

                int calcpool;
                double pdelta = (weight % 10) / 10.0;
                double deltapool = pdelta * (poolprice[pindex2] - poolprice[pindex1]);
                calcpool = poolprice[pindex1] + (int)deltapool;
                return calcpool;

            }

        }

        public int GetPoolPriceByWeightYear(int weight, int years)
        {
            return 6000 + weight * 800;
        }
        #region 取得案件目前可用補助金額
        /// <summary>
        /// 取得案件目前可用補助金額
        /// 套用總則及水源調控設備等補助基準
        /// </summary>
        /// <param name="Step">目前步驟，田間管路:3, 動力設施:4, 蓄水設施:5, 調控設施:6</param>
        /// <param name="MapNo">版本編號</param>
        /// <returns>案件可用補助金額</returns>
        public int Remaining_funds(int Step, int MapNo, Case casedata)
        {
            DryEntities DryDB = new DryEntities();
            int area = getDataCls.GetFarmBuildArea(MapNo);
            double ha = (double)area / 10000;
            int Subsidylimit = getDataCls.GetSubsidyLimit(casedata).TotalLimit;
            int PersionlimitYear = getDataCls.GetSubsidyLimit(casedata).PersionLimit;
            int Poolmaxsubsidy = getDataCls.GetSubsidyLimit(casedata).PoolMaxSubsidy;
            string idno = casedata.Farmer.IdNo;
            int TotalMoney = 0;
            int lastMoney = 0;            

            if (IsRCPool(MapNo))
            {
                PersionlimitYear = Poolmaxsubsidy;
            }

            TotalMoney = PersionlimitYear;
            
            lastMoney = IsRepeat(casedata, MapNo);
            if (TotalMoney+lastMoney > /*300000*/ PersionlimitYear)
            {
                TotalMoney = /*300000*/PersionlimitYear - lastMoney;
            }            
            List<SummaryView> summary = DryDB.SummaryView.Where(m => m.IdNo == idno).ToList();
            int money = 0;
            int usedM = 0;
            switch (Step)
            {
                case 5:
                    usedM = 0;
                    usedM += getDataCls.GetEngineMoney(MapNo);   
                    usedM += getDataCls.GetPoolMoney(MapNo);                         
                    usedM += getDataCls.GetCntrlMoney(MapNo)[1];
                    money = TotalMoney - usedM;
                    break;
                case 3:
                    usedM = 0;                    
                    usedM += getDataCls.GetPigingMoneyByDB(MapNo)[1];
                    usedM += getDataCls.GetPoolMoney(MapNo);
                    usedM +=getDataCls.GetCntrlMoney(MapNo)[1];
                    money = TotalMoney - usedM;
                    break;
                case 4:
                    usedM = 0;
                    usedM += getDataCls.GetPigingMoneyByDB(MapNo)[1];
                    usedM += getDataCls.GetEngineMoney(MapNo);
                    usedM += getDataCls.GetCntrlMoney(MapNo)[1];
                    money = TotalMoney - usedM;
                    break;
                case 6:
                    usedM = 0;                    
                    usedM += getDataCls.GetPigingMoneyByDB(MapNo)[1];
                    usedM += getDataCls.GetEngineMoney(MapNo);   
                    usedM += getDataCls.GetPoolMoney(MapNo);    

                    money = TotalMoney - usedM;
                    break;
            }
            return money;
        }
        #endregion

        #region 計算調控設施補助費用
        /// <summary>
        /// 計算調控設施補助費用
        /// </summary>
        /// <param name="mno">版本編號</param>
        /// <returns></returns>
        public int Ctrl_GovPay(int mno)
        {
            int area = getDataCls.GetFarmBuildArea(mno);
            Case casedata = getDataCls.GetCaseDataFromMapNo(mno);
            int controlmaxsubsidy = getDataCls.GetSubsidyLimit(casedata).ControlMaxSubsidy;
            double controlminarea = getDataCls.GetSubsidyLimit(casedata).ControlMinimumArea;
            double ha = (double)area / 10000; 
            var CtrlMatList = getDataCls.GetCntrlMatData(mno);
            int GovPayM = 0;

            if (ha >= /*0.2*/ controlminarea && CtrlMatList != null)
            {                
                foreach (var ctrl_item in CtrlMatList)
                {
                    GovPayM += (int)(Math.Round(ctrl_item.MatPrice * ctrl_item.MatAmt, 0, MidpointRounding.AwayFromZero));
                }
                
            }
            int MaxMoney = (int)Math.Round(ha * /*60000*/controlmaxsubsidy, 0, MidpointRounding.AwayFromZero);

            if (GovPayM > MaxMoney){
                GovPayM = MaxMoney;
            }
            return GovPayM;
        }
        #endregion

        #region 計算補助單位可用金額
        /// <summary>
        /// 計算補助單位可用金額
        /// </summary>
        /// <param name="Unit">補助單位: 農委會 0, 七星 16, 瑠公 17</param>
        /// <param name="Year">年度</param>
        /// <returns></returns>
        public int Unit_funds(short Unit, byte Year)
        {
            Subsidy sub = getDataCls.GetApplyUnitData(Unit, Year);
            int money = sub.Now_M;

            var CaseList = getDataCls.GetCase();
            List<int> MNo = new List<int>();
            foreach (var item in CaseList)
            {
                
                if (item.VerMapping.Count != 0)
                {
                    MNo.Add(item.VerMapping.Select(v => v.MapNo).Max());
                }
            }

            int WaitspendMoney = 0;
            foreach (var mno in MNo)
            {
                WaitspendMoney += getDataCls.GetUnitMoney(mno, Unit).Sum(p => p.PayMoney);
            }
            return money - WaitspendMoney;
        }
        #endregion

        #region 計算田間管路補助費用
        /// <summary>
        /// 計算田間管路補助費用(農委會 or 農委會的黃金廊道部份)
        /// </summary>
        /// <param name="isGold">是否為黃金廊道</param>
        /// <param name="area">面積</param>
        /// <param name="Endtype">末端型式</param>
        /// <param name="fac">系統型式</param>
        /// <param name="ss">支管行距(SS)</param>
        /// <param name="sl">噴頭間距(SL)</param>
        /// <param name="total_money">設施總經費</param>
        /// <returns></returns>
        public int CalIrrMoney2018(bool isGold, bool isApplied, int area, byte Endtype, byte fac, double ss, double sl, int total_money,Case casedata)
        {
            
            int MaxMoney = 0;
            SubsidyLimit subsidylimit = getDataCls.GetSubsidyLimit(casedata);
            int Money = Convert.ToInt32(Math.Round(total_money * /*0.49*/subsidylimit.GeneralPercent / 100d , 0, MidpointRounding.AwayFromZero));
            if(isApplied && !isGold)
                Money = Convert.ToInt32(Math.Round(total_money * /*0.4*/subsidylimit.AppliedPercent, 0, MidpointRounding.AwayFromZero));
            int m = 0;
            switch (Endtype)
                {
                    case 1:
                    PigingLimit limitdata = getDataCls.GetPigingLimit(casedata, Endtype, fac);

                    MaxMoney = isGold == true ? (int)Math.Round(((double)area / 10000) * /*40000*/ limitdata.FacilityFee, 0, MidpointRounding.AwayFromZero) :
                                                    (int)Math.Round(((double)area / 10000) * /*40000*/ limitdata.FacilityFee, 0, MidpointRounding.AwayFromZero);
                        if (Money > MaxMoney)
                            Money = MaxMoney;
                        break;
                    case 2:
                        if (ss > 0 && sl > 0)
                        {

                        PigingLimit limitdata2 = getDataCls.GetPigingLimit(casedata, Endtype, fac);
                        MaxMoney = (int)Math.Round((double)limitdata2.FacilityFee * (10 / Math.Sqrt(ss * sl)) * ((double)area / 10000), 0, MidpointRounding.AwayFromZero);
                        if (Money > MaxMoney)
                            Money = MaxMoney;

                    }
                        break;
                    case 3:
                        if (ss > 0 && sl > 0)
                        { 
                        PigingLimit limitdata3 = getDataCls.GetPigingLimit(casedata, Endtype, fac);
                        MaxMoney = (int)Math.Round((double)limitdata3.FacilityFee * (5 / Math.Sqrt(ss * sl)) * ((double)area / 10000), 0, MidpointRounding.AwayFromZero);
                            if (Money > MaxMoney)
                                Money = MaxMoney;
                        }
                        break;
                    case 6: 
                    PigingLimit limitdata6 = getDataCls.GetPigingLimit(casedata, Endtype, fac);
                    MaxMoney = (int)Math.Round(((double)area / 10000) * limitdata6.FacilityFee /*80000*/, 0, MidpointRounding.AwayFromZero);
                        if (Money > MaxMoney)
                            Money = MaxMoney;
                        break;
                    case 7:
                    PigingLimit limitdata7 = getDataCls.GetPigingLimit(casedata, Endtype, fac);
                    MaxMoney = isGold == true ? (int)Math.Round((double)area / 10000 * /*153000*/limitdata7.FacilityFee, 0, MidpointRounding.AwayFromZero):
                                                    (int)Math.Round((double)area / 10000 * /*153000*/limitdata7.FacilityFee, 0, MidpointRounding.AwayFromZero);
                        if (Money > MaxMoney)
                            Money = MaxMoney;
                        break;
                    case 8:
                    PigingLimit limitdata8 = getDataCls.GetPigingLimit(casedata, Endtype, fac);
                    MaxMoney = isGold == true ? (int)Math.Round((double)area / 10000 * /*106000*/limitdata8.FacilityFee, 0, MidpointRounding.AwayFromZero):
                                                    (int)Math.Round((double)area / 10000 * /*106000*/limitdata8.FacilityFee, 0, MidpointRounding.AwayFromZero);
                        if (Money > MaxMoney)
                            Money = MaxMoney;
                        break;
                }
            //}
            return Money;
        }

        public int CalIrrMoney(bool isGold, bool isApplied, int area, byte Endtype, byte fac, double ss, double sl, int total_money, Case casedata)
        {
            
            int MaxMoney = 0;
            SubsidyLimit subsidylimit = getDataCls.GetSubsidyLimit(casedata);
            int Money = Convert.ToInt32(Math.Round(total_money * /*0.49*/subsidylimit.GeneralPercent / 100d, 0, MidpointRounding.AwayFromZero));
            if (isApplied && !isGold)
                Money = Convert.ToInt32(Math.Round(total_money * /*0.4*/subsidylimit.AppliedPercent / 100d, 0, MidpointRounding.AwayFromZero));
            PigingLimit limitdata = getDataCls.GetPigingLimit(casedata, Endtype, fac);            
            
            if (isGold)
            {
                MaxMoney = (int)Math.Round(((double)area / 10000) * /*40000*/ limitdata.FacilityFee * (double)subsidylimit.GoldPercent / 100d, 0, MidpointRounding.AwayFromZero);
            }else
            {
                MaxMoney = (int)Math.Round(((double)area / 10000) * /*40000*/ limitdata.FacilityFee, 0, MidpointRounding.AwayFromZero);
            }
            if (Money > MaxMoney)
                Money = MaxMoney;
            return Money;
        }

        #region 計算田間管路補助費用(瑠公)
        /// <summary>
        /// 計算田間管路補助費用(瑠公)
        /// </summary>
        /// <param name="area">面積</param>
        /// <param name="total_money">設施總經費</param>
        /// <returns></returns>
        /// 
        public int CalLiuIrrMoney(int area, int total_money, int mno, byte EndType, byte FacType = 0)
        {
            
            double ha = (double)area / 10000;
            Case casedata = getDataCls.GetCaseDataFromMapNo(mno);
            PigingLimit pigingData = getDataCls.GetPigingLimit(casedata, EndType, FacType);
            int MaxMoney = Convert.ToInt32(Math.Round((double)total_money * pigingData.SubsidyReference / 100, 0, MidpointRounding.AwayFromZero));
            int Money = total_money;

            if (Money > MaxMoney)
                Money = MaxMoney;
            return Money;
        }
        #endregion

        #region 計算田間管路補助費用(七星)
        public int SevenCalIrrMoney(int totalPrice, int coa)
        {
            return (int)Math.Round(totalPrice * 0.7 - coa, 0, MidpointRounding.AwayFromZero);
        }
        #endregion

        #endregion

        #region 計算瑠公總經費
        /// <summary>
        /// 計算瑠公總經費
        /// </summary>
        /// <param name="_MNo"></param>
        /// <returns>[政府補助款, 農戶自備款]</returns>
        public int[] GetLiuGongBudgetBookData(int _MNo)
        {
            #region Get Data
            Case CaseDta = getDataCls.GetCaseDataFromMapNo(_MNo);
            double area = (double)getDataCls.GetFarmBuildArea(_MNo) / 10000;
            List<Pay> payList = getDataCls.GetPay(_MNo);
            int PayMoney = 0; 
            int FarmerMoney = 0; 
            #endregion

            #region Show Data

            #region 田間管路材料費
            PayMoney = payList.Where(m => m.ItemCode == 1).Any() == false ? 0 : payList.Where(m => m.ItemCode == 1).First().PayMoney;
            FarmerMoney = payList.Where(m => m.ItemCode == 1).Any() == false ? 0 : payList.Where(m => m.ItemCode == 1).First().FarmerMoney;

            int PipingMoney = PayMoney + FarmerMoney; 
            #endregion

            #region 動力設備費用
            var englist = getDataCls.GetEngineData(_MNo);
            
            PayMoney = payList.Where(m => m.ItemCode == 5).Any() == false ? 0 : payList.Where(m => m.ItemCode == 5).First().PayMoney;
            FarmerMoney = payList.Where(m => m.ItemCode == 5).Any() == false ? 0 : payList.Where(m => m.ItemCode == 5).First().FarmerMoney;
            int e_money = PayMoney + FarmerMoney; 
            #endregion

            #region 蓄水設備費用
            
            PayMoney = payList.Where(m => m.ItemCode == 6).Any() == false ? 0 : payList.Where(m => m.ItemCode == 6).First().PayMoney;
            FarmerMoney = payList.Where(m => m.ItemCode == 6).Any() == false ? 0 : payList.Where(m => m.ItemCode == 6).First().FarmerMoney;
            int pool_money = PayMoney + FarmerMoney; 
            #endregion

            #region 工作費用
            
            PigingConf pigingConf = getDataCls.GetPigingConfData(_MNo);
            int wmoney = pigingConf == null ? 0 : pigingConf.WorkPrice; 
            #endregion

            #region 設施費總計
            int Total_Money = PipingMoney + e_money + pool_money + wmoney;
            #endregion
            #region 水利會補助款
            
            SubsidyLimit subsidy = getDataCls.GetSubsidyLimit(CaseDta);
            int aimoney = (int)Math.Round(Total_Money * subsidy.GeneralPercent / 100, 0, MidpointRounding.AwayFromZero);
            int iaMaxMoney = (int)Math.Round(area * subsidy.TotalLimit * subsidy.GeneralPercent / 100, 0, MidpointRounding.AwayFromZero);
            if (aimoney > iaMaxMoney)
                aimoney = iaMaxMoney;
            #endregion

            #region 農戶配合款
            int ftmoney = Total_Money - aimoney;
            #endregion

            #endregion

            return new int[] { aimoney, ftmoney };
        }
        #endregion
        /// <summary>
        /// 農戶年度已申請案件使用金額
        /// </summary>
        /// <param name="casedata"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public int IsRepeat(Case casedata, int MapNo)
        {
            DryEntities DryDB = new DryEntities();            
            string idno = casedata.Farmer.IdNo;
            
            var summary = (from m in DryDB.SummaryView
                           where m.IdNo == idno && m.ApplyYear == casedata.ApplyYear
                           select new { m.MapNo }).ToList();
            int TotalMoney = 0;
            foreach (var item in summary)
            {
                if(item.MapNo != MapNo)
                {
                    TotalMoney += DryDB.Pay.Any(m => m.MapNo == item.MapNo && m.ItemCode != 2) ? DryDB.Pay.Where(m => m.MapNo == item.MapNo && m.ItemCode != 2).Sum(n => n.PayMoney) : 0;
                }
                
            }
            return TotalMoney;           
        }
        public bool IsRCPool(int MapNo)
        {
            DryEntities drydb = new DryEntities();
            
            var rec = from a in drydb.VerMapping
                      where a.MapNo == MapNo
                      select new { IdNo = a.Case.Farmer.IdNo, ApplyYear  = a.Case.ApplyYear };
            string idno = string.Empty;
            int applyyear = 0;
            if (rec.Count() > 0)
            {
                idno = rec.FirstOrDefault().IdNo;
                applyyear = rec.FirstOrDefault().ApplyYear;
                var cases = drydb.SummaryView.Where(m => m.IdNo == idno && m.ApplyYear == applyyear).Select(m => new { m.MapNo });
                foreach (var item in cases)
                {
                    if (drydb.Pool.Any(m => m.MapNo == item.MapNo && m.PtypeCode == 1))
                    {
                        return true;
                    }
                }                
            }
            return false;
        }

        #region MoneyData Struct
        public class MoneyData
        {
            public int GovPay { get; set; }
            public int FarmerPay { get; set; }
            public int GoldPay { get; set; }
            public int FundLeft { get; set; }
        }
        #endregion

    }
}
