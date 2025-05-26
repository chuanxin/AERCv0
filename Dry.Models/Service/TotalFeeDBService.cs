/*
-- =============================================
-- Author: wei
-- Create date: 2015-6-16
-- Description: 總經費的資料庫操作類別
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.CommonCls;
using Dry.Models.ViewModel;

namespace Dry.Models.Service
{
    public class TotalFeeDBService
    {
        private DryEntities DryDB = new DryEntities();
        private DBCommon.DbEvent dbstatus = new DBCommon.DbEvent();
        private GetData gd = new GetData();
        private Calculate_Funding clf = new Calculate_Funding();
        /// <summary>
        /// 寫入或更新TotalFee資料表
        /// </summary>
        /// <param name="Totaldata"></param>
        /// <param name="step">1:田間管路；2:動力設備；3:蓄水池；4:調控設備</param>
       /// <returns></returns>
        public DBCommon.DbEvent UpdateTotalFee(int mno)
        {
            TotalFee data = DryDB.TotalFee.Find(mno);
            bool gold = gd.GetGoldFromMapno(mno);
            int[] total = { 0, 0, 0 };
            if (gold == true)
            {
                
                //total = CaculateTotalPayGold(mno);
                total = CaculateTotalPay(mno);
            }
            else
            {
                total = CaculateTotalPay(mno);
            }
            
            
            
            if (data == null)
            {
                
                data = new TotalFee();
                data.MapNo = mno;
                data.GovSubsidy = total[0];
                data.FarmerFee = total[1];
                if (gold == true)
                {
                    //data.GoldSubsidy = total[2];
                    data.GoldSubsidy = 0; 
                }
                else { data.GoldSubsidy = 0; }
                
              
                DryDB.TotalFee.Add(data);
            }
            else
            {
                
                data.GovSubsidy = total[0];
                data.FarmerFee = total[1];
                if (gold == true) {
                    
                    //data.GoldSubsidy = total[2]; 
                    data.GoldSubsidy = 0;
                } else 
                {
                    data.GoldSubsidy = 0;
                }
                
               
                
                DryDB.Entry(data).State = System.Data.EntityState.Modified;
            }
            try
            {
                DryDB.SaveChanges();
                dbstatus.DbMessage = "Success";
            }
            catch (Exception e)
            {
                dbstatus.DbMessage = "Modified TotalFee Failed : " + e.Message + "<br>";
            }
            return dbstatus;
        }


        private int[] CaculateTotalPay(int mno)
        {
            int[] result = { 0, 0 };
            
            if (DryDB.Pay.Any(m => m.MapNo == mno))
            {
                result[0] = DryDB.Pay.Where(m => m.MapNo == mno).Sum(m => m.PayMoney);
                result[1] = DryDB.Pay.Where(m => m.MapNo == mno).Sum(m => m.FarmerMoney);
            }
            
            return result;
        }
        /// <summary>
        /// 計算黃金廊道補助金額，利用原本PayMoney、FarmerMoney依據補助項目重新計算PayMoney、FarmerMoney
        /// </summary>
        /// <param name="mno"></param>
        /// <returns></returns>
        private int[] CaculateTotalPayGold(int mno)
        {
            int[] result = { 0, 0 ,0};
            List<Pay> payary = DryDB.Pay.Where(m => m.MapNo == mno).ToList();
            if (payary != null) 
            {
                foreach (var item in payary)
                {
                    int govpay = 0;
                    int goldpay = 0;
                    int farmpay = 0;
                    switch (item.ItemCode)
                    {
                        case 1:
                            govpay= item.PayMoney;
                            goldpay= (int)Math.Round((double)(item.Total * 0.7) - item.PayMoney,0,MidpointRounding.AwayFromZero);
                            farmpay= (int)item.Total - govpay - goldpay;
                            result[0] += govpay;
                            result[2] += goldpay;
                            result[1] += farmpay;

                            break;
                        case 2:
                            result[0] += item.PayMoney;
                            break;
                        case 4:
                            govpay= item.PayMoney;
                            goldpay= (int)Math.Round((double)(item.Total * 0.7) - item.PayMoney,0,MidpointRounding.AwayFromZero);
                            farmpay= (int)item.Total - govpay - goldpay;
                            result[0] += govpay;
                            result[2] += goldpay;
                            result[1] += farmpay;
                            break;
                        case 5:
                            List<Engine> engs = gd.GetEngineData(mno);
                            if (engs != null)
                            {
                                foreach (var eng in engs)
                                {
                                    
                                    result[0] += eng.EngPrice;
                                }
                            }
                            break;
                        case 6:
                            
                            List<Pool> pools = gd.GetPoolData(mno);
                            if (pools != null)
                            {
                                foreach (var pool in pools)
                                {
                                   
                                    result[0] += pool.PoolPrice;
                                }
                            }
                            
                            
                            break;
                        case 8:
                            result[0] += item.PayMoney;
                            break;
                    }
                }
            }
            return result;
            
        }

    }

}
