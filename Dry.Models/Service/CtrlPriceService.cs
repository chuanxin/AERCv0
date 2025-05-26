/*
-- =============================================
-- Author: WEI
-- Create date: 2014-3-5
-- Description: 計算調控設施所需經費及輔助費用
-- =============================================
*/

using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Dry.Models.Service
{
    public class CtrlPriceService
    {
        /// <summary>
        /// 計算調控設施所需經費及輔助費用 return [所需經費, 輔助費, 自備款]
        /// </summary>
        /// <param name="data"></param>
        /// <param name="MapNo"></param>
        /// <returns>[所需經費, 輔助費, 自備款]</returns>
        public int[] GetCtrlHelpTotalPrice(CtrlTableView[] data, int MapNo)
        {
            int[] Price = new int[3] { 0, 0, 0 };
            if(data != null)
            {
                GetData gd = new GetData();
                Calculate_Funding CalculateCls = new Calculate_Funding();
                Price[0] = GetCtrlTotalPrice(data); //所需經費
                
                Price[1] = GetCtrlHelpPrice(data,MapNo);
                
                if (Price[0] > Price[1])
                    Price[2] = Price[0] - Price[1];
            }
            
            return Price;
        }


        /// <summary>
        /// 計算調控設施所需經費及輔助費用 return [所需經費, 輔助費,七星,自備款]
        /// </summary>
        /// <param name="data"></param>
        /// <param name="MapNo"></param>
        /// <returns>[所需經費, 輔助費, 自備款]</returns>
        public int[] GetCtrlGoldHelpTotalPrice(CtrlTableView[] data, int MapNo)
        {
            int[] Price = new int[4] { 0, 0, 0, 0 };
            if (data != null)
            {
                GetData gd = new GetData();
                Calculate_Funding CalculateCls = new Calculate_Funding();
                Price[0] = GetCtrlTotalPrice(data); 
                
                //Price[1] = GetCtrlHelpPrice(Price[0], MapNo, CalculateCls.Remaining_funds(6, MapNo, gd.GetCaseDataFromMapNo(MapNo)));
                Price[1] = GetCtrlHelpPrice(data,MapNo);
                SubsidyLimit limitdata = gd.GetSubsidyLimit(gd.GetCaseDataFromMapNo(MapNo));

                
                if (Price[0] > Price[1])
                {
                    //Price[2] = (int)(double)(Price[0] * 0.7) - Price[1];
                    Price[2] = (int)Math.Round((double)(Price[0] * /*0.7*/ (limitdata.GoldPercent / 100 ) ) - Price[1], 0, MidpointRounding.AwayFromZero);
                }
                    
                    Price[3] = Price[0] - Price[1] - Price[2];
            }

            return Price;
        }




        /// <summary>
        /// 計算調控設施所需經費
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public int GetCtrlTotalPrice(CtrlTableView[] Data)
        {
            float TotalPrice = 0;
            foreach(var item in Data)
            {
                //TotalPrice += (float)item.MatAmt * (float)item.MatPrice;
                
                //TotalPrice += (float)item.MatAmt * (float)item.MatPriceAply;
                
                TotalPrice += (float)item.MatAmtAply * (float)item.MatPriceAply;
            }
            return (int)Math.Round((double)TotalPrice, 0, MidpointRounding.AwayFromZero);
        }
        
        /// <summary>
        /// 計算補助費用        
        /// </summary>
        /// <param name="TotalPrice">調控設施所需經費</param>
        /// <param name="MapNo">版本編號</param>
        /// <param name="TotalHelpPrice">案件可用經費</param>
        /// <returns></returns>
        public int GetCtrlHelpPriceOrg(float TotalPrice, int MapNo, int TotalHelpPrice)
        {
        
            GetData getData = new GetData();
            float Area = (float)getData.GetFarmBuildArea(MapNo) / 10000;
            Case cse = getData.GetCaseDataFromMapNo(MapNo);
            if (cse.ApplyUnit == 17)
            {
                return (int)(Math.Floor((double)TotalPrice * 0.8));
            }
            else
            {
                if (Area < 0.3)
                    return 0;
                if (TotalPrice > Area * 60000)
                {
                    if ((int)Math.Round(Area * 60000, 0, MidpointRounding.AwayFromZero) > TotalHelpPrice)
                        return TotalHelpPrice;
                    return (int)Math.Round(Area * 60000, 0, MidpointRounding.AwayFromZero);
                }
                if ((int)Math.Round(TotalPrice * 0.49, 0, MidpointRounding.AwayFromZero) > TotalHelpPrice)
                    return TotalHelpPrice;

                return (int)Math.Round(TotalPrice * 0.49, 0, MidpointRounding.AwayFromZero);
            }
        }

        /// <summary>
        /// 計算補助金額舊版給108之前
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int GetCtrlHelpPrice2018(CtrlTableView[] Data) 
        {
            float TotalPrice = 0;
            foreach (var item in Data)
            {
                //TotalPrice += (float)item.MatAmt * (float)item.MatPrice;
                
                TotalPrice += (float)item.MatAmt * (float)item.MatPrice;
            }
            return (int)Math.Round((double)TotalPrice, 0, MidpointRounding.AwayFromZero);
        }
        public int GetCtrlHelpPrice(/*float TotalPrice, int MapNo, int TotalHelpPrice*/CtrlTableView[] Data,int MapNo)
        {

            GetData getData = new GetData();            

            float Area = (float)getData.GetFarmBuildArea(MapNo) / 10000; 
            
            Case cse = getData.GetCaseDataFromMapNo(MapNo);
            int TotalHelpPrice_Remaining = new Calculate_Funding().Remaining_funds(6, MapNo, cse);


            SubsidyLimit limitdata = getData.GetSubsidyLimit(cse);
            if (Area < /*0.3*/limitdata.ControlMinimumArea)
                return 0;

            int TotalHelpPrice = (int)Math.Round(Area * limitdata.ControlMaxSubsidy, 0, MidpointRounding.AwayFromZero);
            if (TotalHelpPrice > TotalHelpPrice_Remaining)
            {
                TotalHelpPrice = TotalHelpPrice_Remaining;
            }
            
            float ApplyPrice = 0;
            foreach (var item in Data)
            {
                //ApplyPrice += (float)item.MatAmtAply * (float)item.MatPriceAply;
                
                ApplyPrice += (float)item.MatAmt * (float)item.MatPriceAply;
            }
            int TotalPrice = (int)Math.Round(ApplyPrice * (limitdata.GeneralPercent / 100), 0, MidpointRounding.AwayFromZero);
            
            if (TotalPrice > TotalHelpPrice) 
                {
                    return TotalHelpPrice;
            }else
            {

                return TotalPrice;
            }
                                  

                
            //}
        }
        
        /// <summary>
        /// 計算單補助單價給2018以前資料用
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="itemCount"></param>
        /// <param name="itemPrice"></param>
        /// <param name="MapNo"></param>
        /// <returns></returns>
        public int[] GetItemPrice2018(string itemName, int itemCount, int itemPrice, int MapNo)
        {
            GetData getData = new GetData();
            int[] result = new int[2];
            int calcCount = 0;
            int calcPrice = 0;
            float Area = (float)getData.GetFarmBuildArea(MapNo) / 10000;
            
            if (Area < 0.2)
            {
                result[0] = 0;
                result[1] = 0;
                return result;
            }
            float HelpPrice = 0;



            switch (itemName)
            {
                case "控制箱":
                    
                    HelpPrice = Area * 10000;
                    
                    if (itemCount > 1)
                    {
                        itemCount = 1;
                    }
                    
                    if ((itemCount * itemPrice * 0.49) > HelpPrice)
                    {
                        calcCount = itemCount;
                        calcPrice = (int)HelpPrice / itemCount;
                    }
                    else
                    {
                        calcPrice = itemPrice * 49 / 100;
                        calcCount = itemCount;
                    }
                    break;
                case "電磁閥":
                    
                    HelpPrice = Area * 30000;

                    
                    if ((int)(Area / 0.1) < itemCount)
                    {
                        calcCount = (int)(Area / 0.1);

                    }
                    else
                    {
                        calcCount = itemCount;
                    }
                    
                    if ((calcCount * itemPrice * 0.49) > HelpPrice)
                    {
                        //calcCount = itemCount;
                        calcPrice = (int)HelpPrice / calcCount;
                    }
                    else
                    {
                        calcPrice = itemPrice * 49 / 100;
                        //calcCount = itemCount;
                    }
                    break;
                case "過濾器":
                    
                    HelpPrice = Area * 6000;
                    
                    if (itemCount > 1)
                    {
                        itemCount = 1;
                    }
                    
                    if ((itemCount * itemPrice * 0.49) > HelpPrice)
                    {
                        calcCount = itemCount;
                        calcPrice = (int)HelpPrice / itemCount;
                    }
                    else
                    {
                        calcPrice = itemPrice * 49 / 100;
                        calcCount = itemCount;
                    }
                    break;
                case "流量錶":
                    
                    HelpPrice = Area * 7000;
                    
                    if (itemCount > 1)
                    {
                        itemCount = 1;
                    }
                    
                    if ((itemCount * itemPrice * 0.49) > HelpPrice)
                    {
                        calcCount = itemCount;
                        calcPrice = (int)HelpPrice / itemCount;
                    }
                    else
                    {
                        calcPrice = itemPrice * 49 / 100;
                        calcCount = itemCount;
                    }
                    break;
                case "液肥注入器":
                    
                    HelpPrice = Area * 7000;
                    
                    if (itemCount > 1)
                    {
                        itemCount = 1;
                    }
                    
                    if ((itemCount * itemPrice * 0.49) > HelpPrice)
                    {
                        calcCount = itemCount;
                        calcPrice = (int)HelpPrice / itemCount;
                    }
                    else
                    {
                        calcPrice = itemPrice * 49 / 100;
                        calcCount = itemCount;
                    }
                    break;
                default:
                    calcPrice = itemPrice;
                    calcCount = itemCount;

                    break;

            }
            result[0] = calcPrice;
            result[1] = calcCount;
            return result;
        }

        public int[] GetItemPrice(string itemName, int itemCount, int itemPrice, int MapNo)
        {
            GetData getData = new GetData();
            Case casedata = getData.GetCaseDataFromMapNo(MapNo);
            SubsidyLimit limitdata = getData.GetSubsidyLimit(casedata);

            int[] result = new int[2];
            int calcCount = 0;
            int calcPrice = 0;
            float Area = (float)getData.GetFarmBuildArea(MapNo) / 10000;
            
            if (Area < /*0.2*/ limitdata.ControlMinimumArea)
            {
                result[0] = 0;
                result[1] = 0;
                return result;
            }
            float HelpPrice = Area * limitdata.ControlMaxSubsidy ;

            
            //int remainfound = new Calculate_Funding().Remaining_funds(6, MapNo, casedata);
            //if (HelpPrice > remainfound)
            //{
            //    HelpPrice = remainfound;
            //}

            //switch (itemName)
            //{
            //    case "控制箱":
            //        
            //        //HelpPrice = Area * 10000;
            //        
            //        //if (itemCount > 1)
            //        //{
            //        //    itemCount = 1;
            //        //}
            //        
            //        //if ((itemCount * itemPrice * 0.49) > HelpPrice)
            //        //{
            //        //    calcCount = itemCount;
            //        //    calcPrice = (int)HelpPrice / itemCount;
            //        //}
            //        //else
            //        //{
            //        //    calcPrice = itemPrice * 49 / 100;
            //        //    calcCount = itemCount;
            //        //}

            //        break;
            //    case "電磁閥":
            //        
            //        HelpPrice = Area * 30000;

            //        
            //        if ((int)(Area / 0.1) < itemCount)
            //        {
            //            calcCount = (int)(Area / 0.1);

            //        }
            //        else
            //        {
            //            calcCount = itemCount;
            //        }
            //        //49%
            //        if ((calcCount * itemPrice * 0.49) > HelpPrice)
            //        {
            //            //calcCount = itemCount;
            //            calcPrice = (int)HelpPrice / calcCount;
            //        }
            //        else
            //        {
            //            calcPrice = itemPrice * 49 / 100;
            //            //calcCount = itemCount;
            //        }
            //        break;
            //    case "過濾器":
            //        
            //        HelpPrice = Area * 6000;
            //        
            //        if (itemCount > 1)
            //        {
            //            itemCount = 1;
            //        }
            //        
            //        if ((itemCount * itemPrice * 0.49) > HelpPrice)
            //        {
            //            calcCount = itemCount;
            //            calcPrice = (int)HelpPrice / itemCount;
            //        }
            //        else
            //        {
            //            calcPrice = itemPrice * 49 / 100;
            //            calcCount = itemCount;
            //        }
            //        break;
            //    case "流量錶":
            //        //小屋頂
            //        HelpPrice = Area * 7000;
            //        
            //        if (itemCount > 1)
            //        {
            //            itemCount = 1;
            //        }
            //        
            //        if ((itemCount * itemPrice * 0.49) > HelpPrice)
            //        {
            //            calcCount = itemCount;
            //            calcPrice = (int)HelpPrice / itemCount;
            //        }
            //        else
            //        {
            //            calcPrice = itemPrice * 49 / 100;
            //            calcCount = itemCount;
            //        }
            //        break;
            //    case "液肥注入器":
            //        
            //        HelpPrice = Area * 7000;
            //        
            //        if (itemCount > 1)
            //        {
            //            itemCount = 1;
            //        }
            //        
            //        if ((itemCount * itemPrice * 0.49) > HelpPrice)
            //        {
            //            calcCount = itemCount;
            //            calcPrice = (int)HelpPrice / itemCount;
            //        }
            //        else
            //        {
            //            calcPrice = itemPrice * 49 / 100;
            //            calcCount = itemCount;
            //        }
            //        break;
            //    default:
            //        calcPrice = itemPrice;
            //        calcCount = itemCount;

            //        break;

            //}
            calcPrice = itemPrice;
            calcCount = itemCount;
            result[0] = calcPrice;
            result[1] = calcCount;
            return result;
        }
    }
}
