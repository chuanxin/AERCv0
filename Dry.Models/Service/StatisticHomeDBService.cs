using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models.CommonCls;
using Dry.Models.ViewModel;
using System.Transactions;
using AERC.Models;
using OfficeOpenXml;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;

namespace Dry.Models.Service
{
    public class StatisticHomeDBService
    {
        private DryEntities DryDB = new DryEntities();
        private CommonEntities CommonDB = new CommonEntities();
        //private StatisticHomeView StaView = new StatisticHomeView();

        
        
        public int Reservationsfarmerliu(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.ReservationsofFarmer_lu = (from limit in DryDB.Subsidy
                                                       where limit.IAUnit == unit && limit.ApplyUnit == 17 && limit.SYear == year
                                                       select limit.FcstAmt).FirstOrDefault();
        }

        
        public int Reservationsfarmliu(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.ReservationsofFarm_lu = (from limit in DryDB.Subsidy
                                                    where limit.IAUnit == unit && limit.ApplyUnit == 17 && limit.SYear == year
                                                    select limit.FcstArea).FirstOrDefault();            
        }

        
        public decimal ReservationsMoneyliu(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.ReservationsofMoney_lu = (from limit in DryDB.Subsidy
                                                     where limit.IAUnit == unit && limit.ApplyUnit == 17 && limit.SYear == year
                                                     select limit.Total_M).FirstOrDefault()/1000;
                        
        }
        
        public decimal ReservationsCMoneyliu(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.Ch_ReservationsofMoney_lu = (from limit in DryDB.Subsidy
                                                        where limit.IAUnit == unit && limit.ApplyUnit == 17 && limit.SYear == year
                                                        select limit.Now_M).FirstOrDefault()/1000;
        }

        
        public int CountFarmerliu(int year, int unit)
        {
            /*
            return StaView.NumofFarmer_lu = (from Cases in DryDB.Case 
                                             join Mapping in DryDB.VerMapping on Cases.EventNo equals Mapping.EventNo
                                             join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                              where (Cases.Enable == true) && (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (pools.ApplyUnit == 17)
                                              select Cases).Count();
             */
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.NumofFarmer_lu = (from SummaryView in DryDB.SummaryView
                                             join pools in DryDB.PoolApply on SummaryView.MapNo equals pools.MapNo
                                             where (SummaryView.ApplyUnit == unit) && (SummaryView.ApplyYear == year) && (pools.ApplyUnit == 17) && (SummaryView.Gold == false)
                                             select SummaryView).GroupBy(m => m.MapNo).Count();
        }

        
        public double CountFarmliu(int year, int unit)
        {
            /*
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.VerMapping on Farms.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (pools.ApplyUnit == 17)
                                         select Farms.FarmArea);
             */
           
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.SummaryView on Farms.MapNo equals Mapping.MapNo
                                         
                                         //join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                         //where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (pools.ApplyUnit == 17) && (Mapping.Gold == false)
                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Gold == false)
                                         && (from poos in DryDB.PoolApply where poos.ApplyUnit == 17 select poos.MapNo).Contains(Farms.MapNo)

                                         select Farms.BuildArea);

            foreach (double a in farms)
            {
                StaView.NumofFarm_lu = StaView.NumofFarm_lu + a;
            }
            
            return (StaView.NumofFarm_lu / 10000);
        }


       
        public int ProvisionFarmerliu(int year, int unit)
        {
            /*
            return StaView.ProvisionOfFarmer_lu = (from Cases in DryDB.Case
                                                    join Mapping in DryDB.VerMapping on Cases.EventNo equals Mapping.EventNo
                                                    join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                                    where (Cases.Enable == true) && (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (Cases.Step >= 7) && (pools.ApplyUnit == 17)
                                                    select Cases).Count();
             */
            
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.ProvisionOfFarmer_lu = (from Cases in DryDB.SummaryView

                                                   join pools in DryDB.PoolApply on Cases.MapNo equals pools.MapNo
                                                   where (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (Cases.Step >= 7) && (pools.ApplyUnit == 17) && (Cases.Gold == false)
                                                   select Cases).GroupBy(m => m.MapNo).Count();
        }
        
        public double ProvisionFarmliu(int year, int unit)
        {
            /*IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.VerMapping on Farms.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (cases.Step >= 7) && (pools.ApplyUnit == 17)
                                         select Farms.FarmArea);
             */
            
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.SummaryView on Farms.MapNo equals Mapping.MapNo
                                         
                                         //join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Step >= 7) && (Mapping.Gold == false)
                                         && (from poos in DryDB.PoolApply where poos.ApplyUnit == 17 select poos.MapNo).Contains(Farms.MapNo)
                                         select Farms.BuildArea);
 

            foreach (double a in farms)
            {
                StaView.ProvisionOfFarm_lu = StaView.ProvisionOfFarm_lu + a;
            }
            
            return (StaView.ProvisionOfFarm_lu / 10000);
        }
        
        public decimal ProvisionMoneyliu(int year, int unit)
        {
            /*IEnumerable<int> Moneyend = (from Pays in DryDB.Pay
                                         join Mapping in DryDB.VerMapping on Pays.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (cases.Step >= 7) && (pools.ApplyUnit == 17)
                                         select Pays.PayMoney);
             */
            
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<int> Moneyend = (from Pays in DryDB.Pay
                                         join Mapping in DryDB.SummaryView on Pays.MapNo equals Mapping.MapNo

                                         //join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Step >= 7)  && (Mapping.Gold == false)
                                         && (from poos in DryDB.PoolApply where poos.ApplyUnit == 17 select poos.MapNo).Contains(Pays.MapNo)
                                         select Pays.PayMoney);

            foreach (int a in Moneyend)
            {
                StaView.ProvisionOfMoney_lu = StaView.ProvisionOfMoney_lu + a;
            }

            return StaView.ProvisionOfMoney_lu ;

        }
        
        public int ProvisionFarmerEndliu(int year, int unit)
        {
            /*return StaView.ProvisionofFarmerend_lu = (from Cases in DryDB.Case
                                                       join Mapping in DryDB.VerMapping on Cases.EventNo equals Mapping.EventNo
                                                       join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                                       where (Cases.Enable == true) && (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (Cases.Step == 11) && (pools.ApplyUnit == 17)
                                                       select Cases).Count();
             */
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.ProvisionofFarmerend_lu = (from Cases in DryDB.SummaryView
                                                      
                                                      join pools in DryDB.PoolApply on Cases.MapNo equals pools.MapNo
                                                      where (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (Cases.Step == 11) && (pools.ApplyUnit == 17) && (Cases.Gold == false)
                                                      select Cases).GroupBy(m => m.MapNo).Count();
        }
        //已結案面積統計
        public double ProvisionFarmEndliu(int year, int unit)
        {
            /*IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.VerMapping on Farms.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (cases.Step == 11) && (pools.ApplyUnit == 17)
                                         select Farms.FarmArea);
             */
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.SummaryView on Farms.MapNo equals Mapping.MapNo
                                         
                                         //join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Complete == true) && (Mapping.Gold == false)
                                         && (from poos in DryDB.PoolApply where poos.ApplyUnit == 17 select poos.MapNo).Contains(Farms.MapNo)
                                         select Farms.BuildArea);
            foreach (double a in farms)
            {
                StaView.ProvisionOfFarmend_lu = StaView.ProvisionOfFarmend_lu + a;
            }
            
            return (StaView.ProvisionOfFarmend_lu / 10000);
        }
        //已結案經費統計
        public decimal ProvisionMoneyEndliu(int year, int unit)
        {
            /*IEnumerable<int> Moneyend = (from Pays in DryDB.Pay
                                         join Mapping in DryDB.VerMapping on Pays.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (cases.Step == 11) && (pools.ApplyUnit == 17)
                                         select Pays.PayMoney);
             */
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<int> Moneyend = (from Pays in DryDB.Pay
                                         join Mapping in DryDB.SummaryView on Pays.MapNo equals Mapping.MapNo
                                         //join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         //join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Complete == true) && (Mapping.Gold == false)
                                         && (from poos in DryDB.PoolApply where poos.ApplyUnit == 17 select poos.MapNo).Contains(Pays.MapNo)
                                         select Pays.PayMoney);

            foreach (int a in Moneyend)
            {
                StaView.ProvisionOfMoneyend_lu = StaView.ProvisionOfMoneyend_lu + a;
            }

            return StaView.ProvisionOfMoneyend_lu;

        }

        
        public decimal design_money_liu (int year ,int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<int> MoenyDesinge = (from DesignPay in DryDB.Pay
                                             join Mapping in DryDB.SummaryView on DesignPay.MapNo equals Mapping.MapNo
                                             where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Step == 11)
                                             && (Mapping.Gold == false) && (DesignPay.ItemCode == 2)
                                     && (from poos in DryDB.PoolApply where poos.ApplyUnit == 17 select poos.MapNo).Contains(DesignPay.MapNo)
                                             select DesignPay.PayMoney);
            foreach (int a in MoenyDesinge)
            {
                if (a > 0 /*!= null*/)
                {
                    StaView.Desinge_lu = StaView.Desinge_lu + a;
                }
                
            }
            return StaView.Desinge_lu;                                         
        }

        
        public int Reservationsfarmer(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<int> farmers = (from limit in DryDB.Subsidy
                                             where limit.IAUnit == unit && limit.SYear == year
                                             select limit.FcstAmt);
            foreach (int a in farmers)
            {
                StaView.ReservationsofFarmer_pis = StaView.ReservationsofFarmer_pis + a ;
            }
            return StaView.ReservationsofFarmer_pis;
        }
        
        public int Reservationsfarm(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<int> farms = (from limit in DryDB.Subsidy
                                                     where limit.IAUnit == unit && limit.SYear == year
                                                     select limit.FcstArea);
            foreach (int a in farms)
            {
                StaView.ReservationsofFarm_pis = StaView.ReservationsofFarm_pis + a;
            }          
            
            return StaView.ReservationsofFarm_pis;
        }
        
        public decimal ReservationsMoney(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<int> moenys = (from limit in DryDB.Subsidy
                                       where limit.IAUnit == unit && limit.SYear == year
                                       select limit.Total_M);
            foreach(int a in moenys)
            {
                StaView.ReservationsofMoney_pis = StaView.ReservationsofMoney_pis + a;
            }
             return StaView.ReservationsofMoney_pis/1000;

        }
        
        public decimal ReservationsCMoney(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView(); 
            IEnumerable<int> moenys = (from limit in DryDB.Subsidy
                                        where limit.IAUnit == unit && limit.SYear == year
                                        select limit.Now_M);
            foreach (int a in moenys)
            {
                StaView.Ch_ReservationsofMoney_pis = StaView.Ch_ReservationsofMoney_pis + a;
            }
             return StaView.Ch_ReservationsofMoney_pis/1000;
        }
        
        public int CountFarmer(int year, int unit)
        {

            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.NumofFarmer_pis = (from Cases in DryDB.Case
                                       where (Cases.Enable == true) && (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (Cases.Gold == false)
                                       select Cases).Count();
        }
        
        public double CountFarm(int year, int unit)
        {
            /*
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.VerMapping on Farms.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) 
                                         select Farms.FarmArea);
            
            */
            
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.SummaryView on Farms.MapNo equals Mapping.MapNo
                                         
                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Gold == false)
                                         select Farms.BuildArea).ToList();

            
            foreach (double a in farms)
            {
                StaView.NumofFarm_pis = StaView.NumofFarm_pis + a;
            }
            
            return (StaView.NumofFarm_pis / 10000);
        }

        
        public int ProvisionFarmer(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.ProvisionOfFarmer_pis = (from Cases in DryDB.Case
                                             where (Cases.Enable == true) && (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (Cases.Step >= 7) && (Cases.Gold == false)
                                             select Cases).Count();
        }
        
        public double ProvisionFarm(int year, int unit)
        {
            
            /*
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.VerMapping on Farms.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (cases.Step >= 7)
                                         select Farms.FarmArea);
             
             */
            
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.SummaryView on Farms.MapNo equals Mapping.MapNo
                                         
                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Step >= 7) && (Mapping.Gold == false)
                                         select Farms.BuildArea);
            


            foreach (double a in farms)
            {
                StaView.ProvisionOfFarm_pis = StaView.ProvisionOfFarm_pis + a;
            }
            //ProFarm.ProvisionOfFarm = farms.Sum();這個方法不行，不會加總
            return (StaView.ProvisionOfFarm_pis / 10000);
        }
        
        public decimal ProvisionMoney(int year, int unit)
        {
            /*IEnumerable<int> Moneyend = (from Pays in DryDB.Pay
                                         join Mapping in DryDB.VerMapping on Pays.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (cases.Step >= 7)
                                         select Pays.PayMoney);
             */
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<int> Moneyend = (from Pays in DryDB.Pay
                                         join Mapping in DryDB.SummaryView on Pays.MapNo equals Mapping.MapNo
                                         //join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Step >= 7) && (Mapping.Gold == false)
                                         select Pays.PayMoney);

            foreach (int a in Moneyend)
            {
                StaView.ProvisionOfMoney_pis = StaView.ProvisionOfMoney_pis + a;
            }

            return StaView.ProvisionOfMoney_pis;

        }

        
        public int ProvisionFarmerEnd(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.ProvisionofFarmerend_pis = (from Cases in DryDB.Case
                                      where (Cases.Enable == true) && (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (Cases.Complete == true) && (Cases.Gold == false)
                                      select Cases).Count();
        }
        
        public double ProvisionFarmEnd(int year, int unit)
        {
            /*
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.VerMapping on Farms.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (cases.Step == 11)
                                         select Farms.FarmArea);
             */
            
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.SummaryView on Farms.MapNo equals Mapping.MapNo
                                         
                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Complete == true) && (Mapping.Gold == false)
                                         select Farms.BuildArea);
 
            foreach (double a in farms)
            {
                StaView.ProvisionOfFarmend_pis = StaView.ProvisionOfFarmend_pis + a;
            }
            
            return (StaView.ProvisionOfFarmend_pis / 10000);
        }
        
        public decimal ProvisionMoneyEnd(int year, int unit)
        {
            /*IEnumerable<int> Moneyend = (from Pays in DryDB.Pay
                                         join Mapping in DryDB.VerMapping on Pays.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (cases.Step == 11)
                                         select Pays.PayMoney);
            */
            
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<int> Moneyend = (from Pays in DryDB.Pay
                                         join Mapping in DryDB.SummaryView on Pays.MapNo equals Mapping.MapNo
                                         
                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Complete == true) && (Mapping.Gold == false)
                                         select Pays.PayMoney);
            foreach (int a in Moneyend)
            {
                StaView.ProvisionOfMoneyend_pis = StaView.ProvisionOfMoneyend_pis + a;
            }

            return StaView.ProvisionOfMoneyend_pis;

        }

        
        public decimal Provisiondesign_money(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<int> MoenyDesinge = (from DesignPay in DryDB.Pay
                                join Mapping in DryDB.SummaryView on DesignPay.MapNo equals Mapping.MapNo
                                where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Step == 11)
                                && (Mapping.Gold == false) && (DesignPay.ItemCode == 2)
                                //&& (from poos in DryDB.PoolApply where poos.ApplyUnit == 17 select poos.MapNo)
                                select DesignPay.PayMoney);

            foreach (int a in MoenyDesinge)
            {
                if(a != null)
                {
                    StaView.Desinge_pis = StaView.Desinge_pis + a;
                }                
            }
            return StaView.Desinge_pis;
        }
        
        public int CountFarmerGold(int year, int unit)
        {
            /*
            return StaView.NumofFarmer_lu = (from Cases in DryDB.Case 
                                             join Mapping in DryDB.VerMapping on Cases.EventNo equals Mapping.EventNo
                                             join pools in DryDB.PoolApply on Mapping.MapNo equals pools.MapNo
                                              where (Cases.Enable == true) && (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (pools.ApplyUnit == 17)
                                              select Cases).Count();
             */
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.NumofFarmer_gold = (from SummaryView in DryDB.SummaryView
                                             where (SummaryView.ApplyUnit == unit) && (SummaryView.ApplyYear == year) && (SummaryView.Gold == true)
                                             select SummaryView).Count();
        }
        
        public double CountFarmgold(int year, int unit)
        {
            /*
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.VerMapping on Farms.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) 
                                         select Farms.FarmArea);
            
            */
            
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.SummaryView on Farms.MapNo equals Mapping.MapNo

                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Gold == true)
                                         select Farms.BuildArea).ToList();


            foreach (double a in farms)
            {
                StaView.NumofFarm_gold = StaView.NumofFarm_gold + a;
            }
            
            return (StaView.NumofFarm_gold / 10000);
        }
        
        public int ProvisionFarmergold(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.ProvisionOfFarmer_gold = (from Cases in DryDB.Case
                                                    where (Cases.Enable == true) && (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (Cases.Step >= 7) && (Cases.Gold == true)
                                                    select Cases).Count();
        }
        
        public double ProvisionFarmgold(int year, int unit)
        {

            /*
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.VerMapping on Farms.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (cases.Step >= 7)
                                         select Farms.FarmArea);
             
             */
            
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.SummaryView on Farms.MapNo equals Mapping.MapNo

                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Step >= 7) && (Mapping.Gold == true)
                                         select Farms.BuildArea);



            StatisticHomeView StaView = new StatisticHomeView();
            foreach (double a in farms)
            {
                StaView.ProvisionOfFarm_gold = StaView.ProvisionOfFarm_gold + a;
            }
            
            return (StaView.ProvisionOfFarm_gold / 10000);
        }       
        
        public double ProvisionFarmEndgold(int year, int unit)
        {
            /*
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.VerMapping on Farms.MapNo equals Mapping.MapNo
                                         join cases in DryDB.Case on Mapping.EventNo equals cases.EventNo
                                         where (cases.Enable == true) && (cases.ApplyUnit == unit) && (cases.ApplyYear == year) && (cases.Step == 11)
                                         select Farms.FarmArea);
             */
            
            StatisticHomeView StaView = new StatisticHomeView();
            IEnumerable<double> farms = (from Farms in DryDB.Farm
                                         join Mapping in DryDB.SummaryView on Farms.MapNo equals Mapping.MapNo

                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Complete == true) && (Mapping.Gold == true)
                                         select Farms.BuildArea);

            foreach (double a in farms)
            {
                StaView.ProvisionOfFarmend_gold = StaView.ProvisionOfFarmend_gold + a;
            }
            
            return (StaView.ProvisionOfFarmend_gold / 10000);
        }
        
        public int ProvisionFarmerEndgold(int year, int unit)
        {
            StatisticHomeView StaView = new StatisticHomeView();
            return StaView.ProvisionofFarmerend_gold = (from Cases in DryDB.Case
                                                        where (Cases.Enable == true) && (Cases.ApplyUnit == unit) && (Cases.ApplyYear == year) && (Cases.Complete == true) && (Cases.Gold == true)
                                                        select Cases).Count();
        }

        
        public feeStruct ProvisionMoneygold(int year, int unit)
        {

            var Moneyend = (from Pays in DryDB.Pay
                            join Mapping in DryDB.SummaryView on Pays.MapNo equals Mapping.MapNo

                            where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Step >= 7) && (Mapping.Gold == true)
                            select new { Pays.PayMoney, Pays.ItemCode, Pays.ApplyUnit, Pays.FarmerMoney, Pays.Total });

            int govpay = 0;
            int goldpay = 0;
            int lupay = 0;
            foreach (var item in Moneyend)
            {
                //StaView.ProvisionOfMoney_pis = StaView.ProvisionOfMoney_pis + a;

                switch (item.ItemCode)
                {
                    case 1:
                        govpay += item.PayMoney;
                        goldpay += (int)Math.Round((double)(item.Total * 0.7), 0, MidpointRounding.AwayFromZero) - item.PayMoney;
                        break;
                    case 2:
                        govpay += (int)item.Total;
                        //goldpay += (int)(item.Total * 0.7) - item.PayMoney; 
                        break;
                    case 3:
                        govpay += item.PayMoney;
                        goldpay += (int)Math.Round((double)(item.Total * 0.7), 0, MidpointRounding.AwayFromZero) - item.PayMoney;
                        break;
                    case 4:
                        govpay += item.PayMoney;
                        goldpay += (int)Math.Round((double)(item.Total * 0.7), 0, MidpointRounding.AwayFromZero) - item.PayMoney;
                        break;
                    case 5:
                        if (item.ApplyUnit == 17)
                        {
                            lupay += (int)item.Total;
                        }
                        else
                        {
                            govpay += (int)item.Total;
                        }
                        break;
                    case 6:
                        if (item.ApplyUnit == 17)
                        {
                            lupay += (int)item.Total;
                        }
                        else
                        {
                            govpay += (int)item.Total;
                        }
                        break;
                    case 7:
                    case 8:
                        govpay += item.PayMoney;
                        goldpay += (int)Math.Round((double)(item.Total * 0.7), 0, MidpointRounding.AwayFromZero) - item.PayMoney;
                        break;

                }
            }
            StatisticHomeView StaView = new StatisticHomeView();
            StaView.ProvisionOfMoney_gold = new feeStruct(govpay, goldpay, lupay);
            //StaView.ProvisionOfMoney_gold.goldpay = goldpay;
            //StaView.ProvisionOfMoney_gold.lupay = lupay;

            return StaView.ProvisionOfMoney_gold;

        }
        
        public feeStruct ProvisionMoneyEndgold(int year, int unit)
        {
            
            
            var Moneyend = (from Pays in DryDB.Pay
                                         join Mapping in DryDB.SummaryView on Pays.MapNo equals Mapping.MapNo

                                         where (Mapping.ApplyUnit == unit) && (Mapping.ApplyYear == year) && (Mapping.Step == 11) && (Mapping.Gold == true)
                                         select new { Pays.PayMoney, Pays.ItemCode, Pays.ApplyUnit, Pays.FarmerMoney, Pays.Total });
            int govpay = 0;
            int goldpay = 0;
            int lupay = 0;
            foreach (var item in Moneyend)
            {
                //StaView.ProvisionOfMoney_pis = StaView.ProvisionOfMoney_pis + a;
                
                switch (item.ItemCode)
                {
                    case 1:
                        govpay += item.PayMoney;
                        goldpay += (int)Math.Round((double)(item.Total * 0.7), 0, MidpointRounding.AwayFromZero) - item.PayMoney; 
                        break;
                    case 2:
                        govpay += (int)item.Total;
                        //goldpay += (int)(item.Total * 0.7) - item.PayMoney; 
                        break;
                    case 3:
                        govpay += item.PayMoney;
                        goldpay += (int)Math.Round((double)(item.Total * 0.7), 0, MidpointRounding.AwayFromZero) - item.PayMoney; 
                        break;
                    case 4:
                        govpay += item.PayMoney;
                        goldpay += (int)Math.Round((double)(item.Total * 0.7), 0, MidpointRounding.AwayFromZero) - item.PayMoney; 
                        break;
                    case 5:
                        if (item.ApplyUnit == 17) {
                            lupay += (int)item.Total;
                        } else {
                            govpay += (int)item.Total;
                        }
                        break;
                    case 6:
                        if (item.ApplyUnit == 17)
                        {
                            lupay += (int)item.Total;
                        }
                        else
                        {
                            govpay += (int)item.Total;
                        }
                        break;
                    case 7:
                    case 8:
                        govpay += item.PayMoney;
                        goldpay += (int)Math.Round((double)(item.Total * 0.7), 0, MidpointRounding.AwayFromZero) - item.PayMoney; 
                        break;
                        
                }
            }
            StatisticHomeView StaView = new StatisticHomeView();
            StaView.ProvisionOfMoneyend_gold = new feeStruct(govpay, goldpay, lupay);
            //StaView.ProvisionOfMoneyend_gold.goldpay = goldpay;
            //StaView.ProvisionOfMoneyend_gold.lupay = lupay;
            return StaView.ProvisionOfMoneyend_gold;
        }

        public List<StatisticHomeView2020> getAllStatisticsData(int iyear)
        {
            List<int> units = DryDB.Subsidy.Where(o => o.SYear == iyear ).Select(o => (int)o.IAUnit).Distinct().ToList();
            //int[] units = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 21, 22, 23 };
            int years = iyear;
            //List<StatisticHomeView> datalist = new List<StatisticHomeView>();
            List<StatisticHomeView2020> datalist = new List<StatisticHomeView2020>();
            //var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (int item in units)
            {
                
                stasticAnnualBudget coaBuget = getStatfromUnit(years, item, 0);
                
                
                stasticAnnualBudget luBuget = getStatfromUnit(years, item, 17);
                
                stasticAnnualBudget cxBuget = getStatfromUnit(years, item, 16);
                
                stasticAnnualBudget goldBuget = getStatGoldfromUnit2020(years, item,0);
                
                StatisticHomeView2020 o = new StatisticHomeView2020();
                o.ReservationsofFarmer_pis = coaBuget.ReservationsofFarmer + luBuget.ReservationsofFarmer + cxBuget.ReservationsofFarmer; 
                o.ReservationsofFarm_pis = coaBuget.ReservationsofFarm + luBuget.ReservationsofFarm + cxBuget.ReservationsofFarm;
                o.ReservationsofMoney_pis = (coaBuget.ReservationsofMoney + luBuget.ReservationsofMoney + cxBuget.ReservationsofMoney) / 1000; 
                o.Ch_ReservationsofMoney_pis = (coaBuget.ReservationsofMoneyNow + luBuget.ReservationsofMoneyNow + cxBuget.ReservationsofMoneyNow) / 1000;
                o.NumofFarmer_pis = coaBuget.NumofFarmer + luBuget.NumofFarmer + cxBuget.NumofFarmer;
                o.NumofFarm_pis = coaBuget.NumofFarm + luBuget.NumofFarm + cxBuget.NumofFarm;
                if (o.ReservationsofFarm_pis != 0)
                    o.ReachOfArea_pis = Math.Round((o.NumofFarm_pis / o.ReservationsofFarm_pis) * 100.0, 2);
                else
                    o.ReachOfArea_pis = 0.0;
                o.ProvisionOfFarmer_pis = coaBuget.ProvisionOfFarmer + luBuget.ProvisionOfFarmer + cxBuget.ProvisionOfFarmer;
                o.ProvisionOfFarm_pis = coaBuget.ProvisionOfFarm + luBuget.ProvisionOfFarm + cxBuget.ProvisionOfFarm;
                o.ProvisionOfMoney_pis = coaBuget.ProvisionOfMoney + luBuget.ProvisionOfMoney + cxBuget.ProvisionOfMoney;
                o.Unusemoney_pis = coaBuget.Unusemoney + luBuget.Unusemoney + cxBuget.Unusemoney;
                o.ProvisionofFarmerend_pis = coaBuget.ProvisionofFarmerend + luBuget.ProvisionofFarmerend + cxBuget.ProvisionofFarmerend;
                o.ProvisionOfFarmend_pis = coaBuget.ProvisionOfFarmend + luBuget.ProvisionOfFarmend + cxBuget.ProvisionOfFarmend;
                o.ProvisionOfMoneyend_pis = coaBuget.ProvisionOfMoneyend + luBuget.ProvisionOfMoneyend + cxBuget.ProvisionOfMoneyend;

                if (o.Ch_ReservationsofMoney_pis != 0)
                    o.PlanExe = Math.Round(((double)o.ProvisionOfMoneyend_pis / (double)(o.Ch_ReservationsofMoney_pis * 1000)) * 100.0, 2);
                else
                    o.PlanExe = 0.0;
                
                o.ReservationsofFarmer_cx = cxBuget.ReservationsofFarmer;
                o.ReservationsofFarm_cx = cxBuget.ReservationsofFarm;
                o.ReservationsofMoney_cx = cxBuget.ReservationsofMoney / 1000;
                o.Ch_ReservationsofMoney_cx = cxBuget.ReservationsofMoneyNow / 1000;
                o.NumofFarmer_cx = cxBuget.NumofFarmer;
                o.NumofFarm_cx = cxBuget.NumofFarm;
                if (o.ReservationsofFarm_cx != 0)
                    o.ReachOfArea_cx = Math.Round((o.NumofFarm_cx / o.ReservationsofFarm_cx) * 100.0, 2);
                else
                    o.ReachOfArea_cx = 0.0;
                o.ProvisionOfFarmer_cx = cxBuget.ProvisionOfFarmer;
                o.ProvisionOfFarm_cx = cxBuget.ProvisionOfFarm;
                o.ProvisionOfMoney_cx = cxBuget.ProvisionOfMoney;
                o.Unusemoney_cx = cxBuget.Unusemoney;
                o.ProvisionofFarmerend_cx = cxBuget.ProvisionofFarmerend;
                o.ProvisionOfFarmend_cx = cxBuget.ProvisionOfFarmend;
                o.ProvisionOfMoneyend_cx = cxBuget.ProvisionOfMoneyend;
                if (o.Ch_ReservationsofMoney_cx != 0)
                    o.PlanExe_cx = Math.Round(((double)o.ProvisionOfMoneyend_cx / (double)(o.Ch_ReservationsofMoney_cx * 1000)) * 100.0, 2);
                else
                    o.PlanExe_cx = 0.0;

                
                o.ReservationsofFarmer_lu = luBuget.ReservationsofFarmer;
                o.ReservationsofFarm_lu = luBuget.ReservationsofFarm;
                o.ReservationsofMoney_lu = luBuget.ReservationsofMoney / 1000;
                o.Ch_ReservationsofMoney_lu = luBuget.ReservationsofMoneyNow / 1000;
                o.NumofFarmer_lu = luBuget.NumofFarmer;
                o.NumofFarm_lu = luBuget.NumofFarm;
                if (o.ReservationsofFarm_lu != 0)
                    o.ReachOfArea_lu = Math.Round((o.NumofFarm_lu / o.ReservationsofFarm_lu) * 100.0, 2);
                else
                    o.ReachOfArea_lu = 0.0;
                o.ProvisionOfFarmer_lu = luBuget.ProvisionOfFarmer;
                o.ProvisionOfFarm_lu = luBuget.ProvisionOfFarm;
                o.ProvisionOfMoney_lu = luBuget.ProvisionOfMoney;
                o.Unusemoney_lu = luBuget.Unusemoney;
                o.ProvisionofFarmerend_lu = luBuget.ProvisionofFarmerend;
                o.ProvisionOfFarmend_lu = luBuget.ProvisionOfFarmend;
                o.ProvisionOfMoneyend_lu = luBuget.ProvisionOfMoneyend;
                if (o.Ch_ReservationsofMoney_lu != 0)
                    o.PlanExe_lu = Math.Round(((double)o.ProvisionOfMoneyend_lu / (double)(o.Ch_ReservationsofMoney_lu * 1000)) * 100.0, 2);
                else
                    o.PlanExe_lu = 0.0;

 
                o.ReservationsofFarmer_coa = coaBuget.ReservationsofFarmer;
                o.ReservationsofFarm_coa = coaBuget.ReservationsofFarm;
                o.ReservationsofMoney_coa = coaBuget.ReservationsofMoney / 1000;
                o.Ch_ReservationsofMoney_coa = coaBuget.ReservationsofMoneyNow / 1000;
                o.NumofFarmer_coa = coaBuget.NumofFarmer;
                o.NumofFarm_coa = coaBuget.NumofFarm;
                if (o.ReservationsofFarm_coa != 0)
                    o.ReachOfArea_coa = Math.Round((o.NumofFarm_coa / o.ReservationsofFarm_coa) * 100.0, 2);
                else
                    o.ReachOfArea_coa = 0.0;
                o.ProvisionOfFarmer_coa = coaBuget.ProvisionOfFarmer;
                o.ProvisionOfFarm_coa = coaBuget.ProvisionOfFarm;
                o.ProvisionOfMoney_coa = coaBuget.ProvisionOfMoney;
                o.Unusemoney_coa = coaBuget.Unusemoney;
                o.ProvisionofFarmerend_coa = coaBuget.ProvisionofFarmerend;
                o.ProvisionOfFarmend_coa = coaBuget.ProvisionOfFarmend;
                o.ProvisionOfMoneyend_coa = coaBuget.ProvisionOfMoneyend;
                if (o.Ch_ReservationsofMoney_coa != 0)
                    o.PlanExe_coa = Math.Round(((double)o.ProvisionOfMoneyend_coa / (double)(o.Ch_ReservationsofMoney_coa * 1000)) * 100.0, 2);
                else
                    o.PlanExe_coa = 0.0;
                
                o.NumofFarmer_gold = goldBuget.NumofFarmer;
                o.NumofFarm_gold = goldBuget.NumofFarm;
                o.ProvisionOfFarmer_gold = goldBuget.ProvisionOfFarmer;
                o.ProvisionOfFarm_gold = goldBuget.ProvisionOfFarm;
                o.ProvisionOfMoney_gold = goldBuget.ProvisionOfMoney;
                o.ProvisionofFarmerend_gold = goldBuget.ProvisionofFarmerend;
                o.ProvisionOfFarmend_gold = goldBuget.ProvisionOfFarmend;
                o.ProvisionOfMoneyend_gold = goldBuget.ProvisionOfMoneyend;

                datalist.Add(o);
                o.IaName =  new GetData().GetUnitName((short)item);

            }
            return datalist;
        }        

        public stasticAnnualBudget getStatfromUnitOld(int applyYear, int unitid, int subsidyId)
        {
            stasticAnnualBudget result = new stasticAnnualBudget();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            
            var subsidyrow = DryDB.Subsidy.Where(m => m.SYear == applyYear && m.IAUnit == unitid && m.ApplyUnit == subsidyId)/*.DefaultIfEmpty()*/.FirstOrDefault();
            if (subsidyrow != null)
            {
                result.ReservationsofFarmer = subsidyrow.FcstAmt;
                result.ReservationsofFarm = subsidyrow.FcstArea;
                result.ReservationsofMoney = subsidyrow.Total_M;
                result.ReservationsofMoneyNow = subsidyrow.Now_M;
            }
            watch.Stop();
            System.Diagnostics.Debug.WriteLine($"unitid {unitid} 預定:{watch.ElapsedMilliseconds} ms");
            
            
            switch (subsidyId)
            {
                case 0:
                    #region 建檔數
                    watch.Restart();
                    var caserowsAll = from a in DryDB.CaseDetail
                                      join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                                      where a.ApplyYear == applyYear && a.ApplyUnit == unitid && a.Gold == false && b.GovPayUnit == 1 && b.ApplyUnit == 0
                                      select new { a.buildarea,a.Step,a.MapNo,a.Complete };

                    if (caserowsAll.Count() > 0)
                    {
                        result.NumofFarmer = caserowsAll.Count();
                        //result.NumofFarm = caserowsAll.Sum(m => m.a.buildarea ?? 0) / 10000;
                        result.NumofFarm = caserowsAll.Sum(m => m.buildarea ?? 0) / 10000;
                    }
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine($"unitid {unitid} 建檔:{watch.ElapsedMilliseconds} ms");
                    #endregion
                    #region 已編列
                    watch.Restart();
                    //var caserows = from o in caserowsAll where o.a.Step >= 7 select o;
                    var caserows = from o in caserowsAll where o.Step >= 7 select o;
                    if (caserows.Count() > 0)
                    {
                        result.ProvisionOfFarmer = caserows.Count();
                        result.ProvisionOfFarm = caserows.Sum(m => m.buildarea ?? 0) / 10000;
                        var caserowsPay = from o in DryDB.TotalFee                                              
                                          join a in caserows on o.MapNo equals a.MapNo
                                          select new { o.GovSubsidy, o.GoldSubsidy };
                        if (caserowsPay.Count() > 0)
                        {
                            result.ProvisionOfMoney = caserowsPay.Sum(m => m.GovSubsidy + m.GoldSubsidy);
                        }
                        #region 設計費                        
                        var casefeerows = from a in caserows
                                          join p in DryDB.Pay on a.MapNo equals p.MapNo
                                          where a.Step == 11 select new {p.ItemCode,p.PayMoney };
                        if (casefeerows.Count() > 0)
                        {
                            result.DesignFee = casefeerows.Where(m => m.ItemCode == 2).Select(m => m.PayMoney).DefaultIfEmpty(0).Sum();
                        }
                        #endregion                        
                    }
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine($"unitid {unitid} 已編列:{watch.ElapsedMilliseconds} ms");
                    #endregion

                    #region 已驗收
                    watch.Restart();
                    var caserowsComplete = from o in caserows
                                           where o.Complete == true
                                           select o;

                    if (caserowsComplete.Count() > 0)
                    {
                        result.ProvisionofFarmerend = caserowsComplete.Count();                        
                        result.ProvisionOfFarmend = caserowsComplete.Sum(m => m.buildarea ?? 0) / 10000;
                        var caserowsCompletePay = from o in DryDB.TotalFee
                                                  join a in caserowsComplete on o.MapNo equals a.MapNo
                                                  select new { o.GovSubsidy,o.GoldSubsidy } ;
                        if (caserowsCompletePay.Count() > 0)
                        {
                            result.ProvisionOfMoneyend = caserowsCompletePay.Sum(m => m.GovSubsidy + m.GoldSubsidy);
                        }

                    }
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine($"unitid {unitid} 已驗收:{watch.ElapsedMilliseconds} ms");
                    #endregion

                    break;
                case 16:
                    
                    
                    #region 建檔數
                    var caserows16All = from a in DryDB.CaseDetail
                                        join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                                        where a.ApplyYear == applyYear && a.ApplyUnit == unitid && a.Gold == false && b.GovPayUnit == 1 && b.ApplyUnit == 16
                                        select new { a.buildarea, a.Step, a.MapNo, a.Complete };
                    if (caserows16All.Count() > 0)
                    {
                        result.NumofFarmer = caserows16All.Count();
                        result.NumofFarm = caserows16All.Sum(m => m.buildarea ?? 0) / 10000;
                    }
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine($"unitid {unitid} 建檔:{watch.ElapsedMilliseconds} ms");

                    #endregion
                    #region 已編列
                    watch.Restart();
                    var caserows16 = from o in caserows16All where o.Step >= 7 select o;
                    if (caserows16.Count() > 0)
                    {
                        result.ProvisionOfFarmer = caserows16.Count();
                        result.ProvisionOfFarm = caserows16.Sum(m => m.buildarea ?? 0) / 10000;
                        var caserows16Pay = from o in DryDB.TotalFee
                                            join a in caserows16 on o.MapNo equals a.MapNo
                                            select new { o.GovSubsidy, o.GoldSubsidy};
                        if (caserows16Pay.Count() > 0)
                        {
                            result.ProvisionOfMoney = caserows16Pay.Sum(m => m.GovSubsidy + m.GoldSubsidy);
                        }
                        #region 設計費
                        var casefeerows16 = from a in caserows16
                                            join p in DryDB.Pay on a.MapNo equals p.MapNo
                                            where a.Step == 11
                                            select new { p.ItemCode, p.PayMoney };
                        
                        if (casefeerows16.Count() > 0)
                        {
                            result.DesignFee = casefeerows16.Where(m => m.ItemCode == 2).Select(m => m.PayMoney).DefaultIfEmpty(0).Sum();
                                                        
                        }
                        #endregion
                        

                    }
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine($"unitid {unitid} 已編列:{watch.ElapsedMilliseconds} ms");
                    #endregion

                    #region 已驗收
                    watch.Restart();
                    var caserows16Complete = from o in caserows16
                                             where o.Complete == true
                                             select o;
                    if (caserows16Complete.Count() > 0)
                    {
                        result.ProvisionofFarmerend = caserows16Complete.Count();
                        result.ProvisionOfFarmend = caserows16Complete.Sum(m => m.buildarea ?? 0) / 10000;
                        var caserows16CompletePay = from o in DryDB.TotalFee
                                                    join a in caserows16Complete on o.MapNo equals a.MapNo
                                                    select new { o.GovSubsidy,o.GoldSubsidy };
                        result.ProvisionOfMoneyend = caserows16CompletePay.Sum(m => m.GovSubsidy + m.GoldSubsidy);
                    }
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine($"unitid {unitid} 已驗收:{watch.ElapsedMilliseconds} ms");
                    #endregion


                    break;
                case 17:
                    //20200123 alex 修改採用casepayunit view 第一筆
                    
                    #region 建檔數
                    var caserows17All = from a in DryDB.CaseDetail
                                        join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo

                                        where a.ApplyYear == applyYear && a.ApplyUnit == unitid && a.Gold == false && b.GovPayUnit == 1 && b.ApplyUnit == 17
                                        select new { a.buildarea, a.Step, a.MapNo, a.Complete };
                    
                    if (caserows17All.Count() > 0)
                    {
                        result.NumofFarmer = caserows17All.Count();
                        result.NumofFarm = caserows17All.Sum(m => m.buildarea ?? 0) / 10000;
                    }
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine($"unitid {unitid} 建檔:{watch.ElapsedMilliseconds} ms");
                    #endregion
                    #region 已編列
                    watch.Restart();
                    var caserows17 = from o in caserows17All where o.Step >= 7 select o;
                    if (caserows17.Count() > 0)
                    {
                        result.ProvisionOfFarmer = caserows17.Count();
                        result.ProvisionOfFarm = caserows17.Sum(m => m.buildarea ?? 0) / 10000;
                        var caserows17Pay = from o in DryDB.TotalFee
                                            join a in caserows17 on o.MapNo equals a.MapNo
                                            select o;
                        if (caserows17Pay.Count() > 0)
                        {
                            result.ProvisionOfMoney = caserows17Pay.Sum(m => m.GovSubsidy + m.GoldSubsidy);
                        }
                        #region 設計費
                        var casefeerows17 = from a in caserows17
                                            join p in DryDB.Pay on a.MapNo equals p.MapNo
                                            where a.Step == 11
                                            select new { p.ItemCode, p.PayMoney };
                        if (casefeerows17.Count() > 0)
                        {
                            result.DesignFee = casefeerows17.Where(m => m.ItemCode == 2).Select(m => m.PayMoney).DefaultIfEmpty(0).Sum() ;
                        }
                        #endregion
                        
                    }
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine($"unitid {unitid} 已編列:{watch.ElapsedMilliseconds} ms");
                    #endregion

                    #region 已驗收
                    watch.Restart();
                    var caserows17Complete = from o in caserows17
                                             where o.Complete == true
                                             select o;
                    if (caserows17Complete.Count() > 0)
                    {

                        result.ProvisionofFarmerend = caserows17Complete.Count();
                        result.ProvisionOfFarmend = caserows17Complete.Sum(m => m.buildarea ?? 0) / 10000;
                        var caserows17CompletePay = from o in DryDB.TotalFee
                                                    join a in caserows17Complete on o.MapNo equals a.MapNo
                                                    select new { o.GovSubsidy, o.GoldSubsidy };
                        result.ProvisionOfMoneyend = caserows17CompletePay.Sum(m => m.GovSubsidy + m.GoldSubsidy);
                    }
                    watch.Stop();
                    System.Diagnostics.Debug.WriteLine($"unitid {unitid} 已驗收:{watch.ElapsedMilliseconds} ms");
                    #endregion


                    break;
                default:
                    break;
            }
            
            result.Unusemoney = result.ReservationsofMoneyNow - result.ProvisionOfMoney;
            
            if (result.ReservationsofMoneyNow > 0)
            {
                result.PlanExe = Math.Round(((double)result.ProvisionOfMoneyend / (double)result.ReservationsofMoneyNow) * 100d, 2);
            }
            
            if (result.ReservationsofFarm > 0)
            {
                result.ReachOfArea = Math.Round(((double)result.NumofFarm / (double)result.ReservationsofFarm) * 100d, 2);
            }

            return result;
        }
        /// <summary>
        /// 執行單位經費、件數、面積統計含黃金廊道
        /// </summary>
        /// <param name="applyYear">年期</param>
        /// <param name="unitid">執行單位</param>
        /// <param name="subsidyId">經費來源單位</param>
        /// <returns></returns>
        public stasticAnnualBudget getStatfromUnit(int applyYear, int unitid, int subsidyId)
        {
            
            stasticAnnualBudget result = new stasticAnnualBudget();
            
            var subsidyrow = DryDB.Subsidy.Where(m => m.SYear == applyYear && m.IAUnit == unitid && m.ApplyUnit == subsidyId)/*.DefaultIfEmpty()*/.FirstOrDefault();
            if (subsidyrow != null)
            {
                result.ReservationsofFarmer = subsidyrow.FcstAmt;
                result.ReservationsofFarm = subsidyrow.FcstArea;
                result.ReservationsofMoney = subsidyrow.Total_M;
                result.ReservationsofMoneyNow = subsidyrow.Now_M;
            }
            else
            {
                result.ReservationsofFarmer = 0;
                result.ReservationsofFarm = 0;
                result.ReservationsofMoney = 0;
                result.ReservationsofMoneyNow = 0;
            }
            

            #region 建檔數
            
            var caserowsAll = from a in DryDB.CaseDetail
                              join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                              join c in DryDB.TotalFee on a.MapNo equals c.MapNo into d
                              from e in d.DefaultIfEmpty()
                              where a.ApplyYear == applyYear && a.ApplyUnit == unitid && /*a.Gold == false &&*/ b.GovPayUnit == 1 && b.ApplyUnit == subsidyId
                              select new { a.buildarea, a.Step, a.MapNo, a.Complete, e.GovSubsidy, e.GoldSubsidy };
            if (caserowsAll.Count() > 0)
            {
                result.NumofFarmer = caserowsAll.Count();
                result.NumofFarm = (double)caserowsAll.Sum(m => m.buildarea) / 10000;
            }else
            {
                result.NumofFarmer = 0;
                result.NumofFarm = 0;
            }
            


            #endregion
            #region 已編列
            
            var caserowsBuget = from a in caserowsAll
                                where a.Step >=7
                                select new { a.buildarea, a.Step, a.MapNo, a.Complete, a.GovSubsidy, a.GoldSubsidy };
            if (caserowsBuget.Count() > 0)
            {
                result.ProvisionOfFarmer = caserowsBuget.Count();
                result.ProvisionOfFarm = (double)caserowsBuget.Sum(m => m.buildarea) / 10000;
                result.ProvisionOfMoney = caserowsBuget.Sum(m => m.GovSubsidy + m.GoldSubsidy);
                var casefeerows = from a in caserowsBuget
                                  join p in DryDB.Pay on a.MapNo equals p.MapNo
                                  where a.Complete == true && p.ItemCode == 2
                                  select new { p.PayMoney };
                if (casefeerows.Count() > 0)
                {
                    result.DesignFee = casefeerows.Sum(m => m.PayMoney);
                }
            }else
            {
                result.ProvisionOfFarmer = 0;
                result.ProvisionOfFarm = 0;
                result.ProvisionOfMoney = 0;
                result.DesignFee = 0;
            }
            
            
            #endregion

            #region 已驗收
            
            var caserowsComplete = from a in caserowsAll
                                   where a.Complete == true
                                   select new { a.buildarea, a.Step, a.MapNo, a.Complete, a.GovSubsidy, a.GoldSubsidy };
            if (caserowsComplete.Count() > 0)
            {
                result.ProvisionofFarmerend = caserowsComplete.Count();
                result.ProvisionOfFarmend = (double)caserowsComplete.Sum(m => m.buildarea) / 10000;
                result.ProvisionOfMoneyend = caserowsComplete.Sum(m => m.GovSubsidy + m.GoldSubsidy);
            }else
            {
                result.ProvisionofFarmerend = 0;
                result.ProvisionOfFarmend = 0;
                result.ProvisionOfMoneyend = 0;
            }

            
            
            #endregion
            
            
            result.Unusemoney = result.ReservationsofMoneyNow - result.ProvisionOfMoney;
            
            if (result.ProvisionOfMoney > 0)
            {
                //result.PlanExe = Math.Round(((double)result.ProvisionOfMoneyend / (double)result.ReservationsofMoneyNow) * 100d, 2);
                result.PlanExe = Math.Round(((double)result.ProvisionOfMoneyend / (double)result.ReservationsofMoneyNow) * 100d, 2);
            }
            
            if (result.ReservationsofFarm > 0)
            {
                result.ReachOfArea = Math.Round(((double)result.ProvisionOfFarm / (double)result.ReservationsofFarm) * 100d, 2);
            }

            return result;
        }

        public stasticAnnualBudget getStatfromUnitNew(int applyYear, int unitid, int subsidyId)
        {
            
            stasticAnnualBudget result = new stasticAnnualBudget();
            
            #region 預定推廣數值
            var subsidyrow = DryDB.Subsidy.Where(m => m.SYear == applyYear && m.IAUnit == unitid && m.ApplyUnit == subsidyId).FirstOrDefault();
            if (subsidyrow != null)
            {
                result.ReservationsofFarmer = subsidyrow.FcstAmt;
                result.ReservationsofFarm = subsidyrow.FcstArea;
                result.ReservationsofMoney = subsidyrow.Total_M;
                result.ReservationsofMoneyNow = subsidyrow.Now_M;
            }
            else
            {
                result.ReservationsofFarmer = 0;
                result.ReservationsofFarm = 0;
                result.ReservationsofMoney = 0;
                result.ReservationsofMoneyNow = 0;
            }
            #endregion

            


            #region 建檔數
            
            var caserowsAll = from a in DryDB.CaseDetail
                              join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                              //join c in DryDB.TotalFee on a.MapNo equals c.MapNo into d
                              //from e in d.DefaultIfEmpty()
                              where a.ApplyYear == applyYear && a.ApplyUnit == unitid && a.Gold == false && b.GovPayUnit == 1 && b.ApplyUnit == subsidyId
                              select new { a.buildarea };
            if (caserowsAll.Count() > 0)
            {
                result.NumofFarmer = caserowsAll.Count();
                result.NumofFarm = (double)caserowsAll.Sum(m => m.buildarea) / 10000;
            }
            else
            {
                result.NumofFarmer = 0;
                result.NumofFarm = 0;
            }


           

            #endregion
            #region 已編列
            
            var caserowsBuget = from a in DryDB.CaseDetail
                                join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                                join c in DryDB.TotalFee on a.MapNo equals c.MapNo
                                where a.ApplyYear == applyYear && a.ApplyUnit == unitid && a.Gold == false && a.Step >= 7 && b.GovPayUnit == 1 && b.ApplyUnit == subsidyId
                                group new {a.buildarea,c.GovSubsidy,c.GoldSubsidy} by a.ApplyUnit into gp
                                select new
                                {
                                    buildarea = gp.Sum(m=>m.buildarea) / 10000,
                                    GovPay = gp.Sum(m=>m.GovSubsidy+m.GoldSubsidy),
                                    Rec = gp.Count()
                                };
            if (caserowsBuget.FirstOrDefault() != null)
            {
                result.ProvisionOfFarmer = caserowsBuget.FirstOrDefault().Rec;
                result.ProvisionOfFarm = (double)caserowsBuget.FirstOrDefault().buildarea;
                result.ProvisionOfMoney = caserowsBuget.FirstOrDefault().GovPay;
                var casefeerows = from a in DryDB.SummaryView
                                  join p in DryDB.Pay on a.MapNo equals p.MapNo
                                  where a.Complete == true && p.ItemCode == 2
                                  select new { p.PayMoney };
                if (casefeerows.Count() > 0)
                {
                    result.DesignFee = casefeerows.Sum(m => m.PayMoney);
                }
            }
            else
            {
                result.ProvisionOfFarmer = 0;
                result.ProvisionOfFarm = 0;
                result.ProvisionOfMoney = 0;
                result.DesignFee = 0;
            }

            #endregion

            #region 已驗收
          
            var caserowsComplete = from a in DryDB.CaseDetail
                                   join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                                   join c in DryDB.TotalFee on a.MapNo equals c.MapNo
                                   where a.ApplyYear == applyYear && a.ApplyUnit == unitid && a.Gold == false && a.Complete == true && b.GovPayUnit == 1 && b.ApplyUnit == subsidyId
                                   group new { a.buildarea, c.GovSubsidy, c.GoldSubsidy } by a.ApplyUnit into gp
                                   select new
                                   {
                                       buildarea = gp.Sum(m => m.buildarea) / 10000,
                                       GovPay = gp.Sum(m => m.GovSubsidy + m.GoldSubsidy),
                                       Rec = gp.Count()
                                   };
            if (caserowsComplete.FirstOrDefault() != null)
            {
                result.ProvisionofFarmerend = caserowsComplete.FirstOrDefault().Rec;
                result.ProvisionOfFarmend = (double)caserowsComplete.FirstOrDefault().buildarea;
                result.ProvisionOfMoneyend = caserowsComplete.FirstOrDefault().GovPay;
            }
            else
            {
                result.ProvisionofFarmerend = 0;
                result.ProvisionOfFarmend = 0;
                result.ProvisionOfMoneyend = 0;
            }


            #endregion

        
            result.Unusemoney = result.ReservationsofMoneyNow - result.ProvisionOfMoney;
            
            if (result.ProvisionOfMoney > 0)
            {
                //result.PlanExe = Math.Round(((double)result.ProvisionOfMoneyend / (double)result.ReservationsofMoneyNow) * 100d, 2);
                result.PlanExe = Math.Round(((double)result.ProvisionOfMoneyend / (double)result.ProvisionOfMoney) * 100d, 2);
            }
            
            if (result.ReservationsofFarm > 0)
            {
                result.ReachOfArea = Math.Round(((double)result.NumofFarm / (double)result.ReservationsofFarm) * 100d, 2);
            }

            return result;
        }
        public stasticAnnualBudget getStatGoldfromUnit2020Old(int applyYear, int unitid, int subsidyId)
        {
            
            stasticAnnualBudget result = new stasticAnnualBudget();
            
            var subsidyrow = DryDB.Subsidy.Where(m => m.SYear == applyYear && m.IAUnit == unitid && m.ApplyUnit == subsidyId)/*.DefaultIfEmpty()*/.FirstOrDefault();
            if (subsidyrow != null)
            {
                result.ReservationsofFarmer = subsidyrow.FcstAmt;
                result.ReservationsofFarm = subsidyrow.FcstArea;
                result.ReservationsofMoney = subsidyrow.Total_M;
                result.ReservationsofMoneyNow = subsidyrow.Now_M;
            }
            

            #region 建檔數
            

            var caserowsAll = from a in DryDB.CaseDetail
                              where a.ApplyYear == applyYear && a.ApplyUnit == unitid && a.Gold == true
                              select new { a.buildarea, a.Step, a.MapNo, a.Complete };
            if (caserowsAll.Count() > 0)
            {
                result.NumofFarmer = caserowsAll.Count();
                result.NumofFarm = caserowsAll.Sum(m => m.buildarea ?? 0) / 10000;
            }
            
            
            #endregion
            #region 已編列
            

            var caserows = from o in caserowsAll where o.Step >= 7 select new { o.buildarea,o.Step,o.MapNo,o.Complete} ;
            if (caserows.Count() > 0)
            {
                result.ProvisionOfFarmer = caserows.Count();
                result.ProvisionOfFarm = caserows.Sum(m => m.buildarea ?? 0) / 10000;
                var caserowsPay = from o in DryDB.TotalFee
                                  join a in caserows on o.MapNo equals a.MapNo
                                  select new { o.GovSubsidy, o.GoldSubsidy } ;

                if (caserowsPay.Count() > 0)
                {
                    result.ProvisionOfMoney = caserowsPay.Sum(m => m.GovSubsidy + m.GoldSubsidy);
                }
                #region 設計費
                var casefeerows = from a in caserows
                                  join p in DryDB.Pay on a.MapNo equals p.MapNo
                                  where a.Step == 11
                                  select new { p.ItemCode, p.PayMoney };
                if (casefeerows.Count() > 0)
                {
                    result.DesignFee = casefeerows.Where(m => m.ItemCode == 2).Select(m => m.PayMoney).DefaultIfEmpty(0).Sum();
                }
                #endregion
            }
            
            #endregion

            #region 已驗收
            

            var caserowsComplete = from o in caserows
                                   where o.Complete == true
                                   select new { o.buildarea, o.Step, o.MapNo, o.Complete };

            if (caserowsComplete.Count() > 0)
            {
                result.ProvisionofFarmerend = caserowsComplete.Count();
                result.ProvisionOfFarmend = caserowsComplete.Sum(m => m.buildarea ?? 0) / 10000;
                var caserowsCompletePay = from o in DryDB.TotalFee
                                          join a in caserowsComplete on o.MapNo equals a.MapNo
                                          select new { o.GovSubsidy, o.GoldSubsidy } ;
                if (caserowsCompletePay.Count() > 0)
                {
                    result.ProvisionOfMoneyend = caserowsCompletePay.Sum(m => m.GovSubsidy + m.GoldSubsidy);
                }
            }
            
            #endregion
            return result;
        }

        public stasticAnnualBudget getStatGoldfromUnit2020(int applyYear, int unitid, int subsidyId)
        {
            
            stasticAnnualBudget result = new stasticAnnualBudget();
            
            var subsidyrow = DryDB.Subsidy.Where(m => m.SYear == applyYear && m.IAUnit == unitid && m.ApplyUnit == subsidyId)/*.DefaultIfEmpty()*/.FirstOrDefault();
            if (subsidyrow != null)
            {
                result.ReservationsofFarmer = subsidyrow.FcstAmt;
                result.ReservationsofFarm = subsidyrow.FcstArea;
                result.ReservationsofMoney = subsidyrow.Total_M;
                result.ReservationsofMoneyNow = subsidyrow.Now_M;
            }
            else
            {
                result.ReservationsofFarmer = 0;
                result.ReservationsofFarm = 0;
                result.ReservationsofMoney = 0;
                result.ReservationsofMoneyNow = 0;
            }
            


            #region 建檔數
            
            var caserowsAll = from a in DryDB.CaseDetail
                              //join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                              join c in DryDB.TotalFee on a.MapNo equals c.MapNo into d
                              from e in d.DefaultIfEmpty()
                              where a.ApplyYear == applyYear && a.ApplyUnit == unitid && a.Gold == true
                              select new { a.buildarea, a.Step, a.MapNo, a.Complete, e.GovSubsidy, e.GoldSubsidy };

            if (caserowsAll.Count() > 0)
            {
                result.NumofFarmer = caserowsAll.Count();
                result.NumofFarm = (double)caserowsAll.Sum(m => m.buildarea) / 10000;
            }
            else
            {
                result.NumofFarmer = 0;
                result.NumofFarm = 0;
            }

            

            #endregion
            #region 已編列
            
            var caserowsBuget = from a in caserowsAll
                                where a.Step >= 7
                                select new { a.buildarea, a.Step, a.MapNo, a.Complete, a.GovSubsidy, a.GoldSubsidy };
            if (caserowsBuget.Count() > 0)
            {
                result.ProvisionOfFarmer = caserowsBuget.Count();
                result.ProvisionOfFarm = (double)caserowsBuget.Sum(m => m.buildarea) / 10000;
                result.ProvisionOfMoney = caserowsBuget.Sum(m => m.GovSubsidy + m.GoldSubsidy);
                var casefeerows = from a in caserowsBuget
                                  join p in DryDB.Pay on a.MapNo equals p.MapNo
                                  where a.Complete == true && p.ItemCode == 2
                                  select new { p.PayMoney };
                if (casefeerows.Count() > 0)
                {
                    result.DesignFee = casefeerows.Sum(m => m.PayMoney);
                }
            }
            else
            {
                result.ProvisionOfFarmer = 0;
                result.ProvisionOfFarm = 0;
                result.ProvisionOfMoney = 0;
                result.DesignFee = 0;
            }
            
            #endregion

            #region 已驗收
            
            var caserowsComplete = from a in caserowsAll
                                   where a.Complete == true
                                   select new { a.buildarea, a.Step, a.MapNo, a.Complete, a.GovSubsidy, a.GoldSubsidy };
            if (caserowsComplete.Count() > 0)
            {
                result.ProvisionofFarmerend = caserowsComplete.Count();
                result.ProvisionOfFarmend = (double)caserowsComplete.Sum(m => m.buildarea) / 10000;
                result.ProvisionOfMoneyend = caserowsComplete.Sum(m => m.GovSubsidy + m.GoldSubsidy);
            }
            else
            {
                result.ProvisionofFarmerend = 0;
                result.ProvisionOfFarmend = 0;
                result.ProvisionOfMoneyend = 0;
            }
            
            #endregion

            
            result.Unusemoney = result.ReservationsofMoneyNow - result.ProvisionOfMoney;
            
            if (result.ProvisionOfMoney > 0)
            {
                result.PlanExe = Math.Round(((double)result.ProvisionOfMoneyend / (double)result.ProvisionOfMoney) * 100d, 2);
            }
            else
            {
                result.PlanExe = 0;
            }
            
            if (result.ReservationsofFarm > 0)
            {
                result.ReachOfArea = Math.Round(((double)result.NumofFarm / (double)result.ReservationsofFarm) * 100d, 2);
            }

            return result;
        }
        public stasticAnnualBudgetGold getStatGoldfromUnit(int applyYear, int unitid)
        {
            stasticAnnualBudgetGold result = new stasticAnnualBudgetGold();
            var caserowsAll = from a in DryDB.CaseDetail
                              join d in DryDB.TotalFee on a.MapNo equals d.MapNo into o
                              from d in o.DefaultIfEmpty()
                              where /*a.Step >= 7 &&*/ a.ApplyYear == applyYear && a.ApplyUnit == unitid && a.Gold == true
                              
                              select new { a, d };
            if (caserowsAll.Count() > 0)
            {
                result.NumofFarmer = caserowsAll.Count();
                result.NumofFarm = caserowsAll.Sum(m => m.a.buildarea ?? 0) / 10000;
            }
            var caserows = from o in caserowsAll where o.a.Step >= 7 select o;
            if (caserows.Count() > 0)
            {
                result.ProvisionOfFarmer = caserows.Count();
                result.ProvisionOfFarm = caserows.Sum(m => m.a.buildarea ?? 0) / 10000;
                result.ProvisionOfMoney = ProvisionMoneygold(applyYear,unitid);



                var casefeerows = from a in caserows where a.a.Step == 11 select a.a.MapNo;
                if (casefeerows.Count() > 0)
                {
                    if (DryDB.Pay.Any(m => casefeerows.Contains(m.MapNo)))
                    {
                        result.DesignFee = DryDB.Pay.Where(m => casefeerows.Contains(m.MapNo)).Sum(m => m.PayMoney);
                    }
                    
                }

            }

            var caserowsComplete = from o in caserows
                                   where o.a.Complete == true
                                   select o;
            if (caserowsComplete.Count() > 0)
            {
                result.ProvisionofFarmerend = caserowsComplete.Count();
                result.ProvisionOfFarmend = caserowsComplete.Sum(m => m.a.buildarea ?? 0) / 10000;
                //result.ProvisionOfMoneyend = caserowsComplete.Sum(m => m.d.GovSubsidy + m.d.GoldSubsidy);
                result.ProvisionOfMoneyend = ProvisionMoneyEndgold(applyYear, unitid);
            }
            return result;
        }
        /// <summary>
        /// 統計執行單位推廣面積與金額包含黃金廊道
        /// </summary>
        /// <param name="applyyear"></param>
        /// <returns></returns>
        public List<StatisticCoaView> StatCoa(int applyyear)
        {
            List<StatisticCoaView> result = new List<StatisticCoaView>();
            #region 統計設定執行單位
            var ialist = from a in DryDB.SubsidyLimit
                         where a.ApplyYear == applyyear
                         select new { a.ApplyUnit };
            foreach (var item in ialist.OrderBy(m=> m.ApplyUnit))
            {
                StatisticCoaView newdata = new StatisticCoaView();
                newdata.Ia = item.ApplyUnit;
                newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                newdata.RsvFarmArea = DryDB.Subsidy.Where(m => m.IAUnit == item.ApplyUnit && m.SYear == applyyear).Select(m => m.FcstArea).DefaultIfEmpty(0).Sum();
                newdata.RsvBudget = DryDB.Subsidy.Where(m => m.IAUnit == item.ApplyUnit && m.SYear == applyyear).Select(m => m.Now_M).DefaultIfEmpty(0).Sum();
                result.Add(newdata);
            }
            #endregion
            #region 已編列面積、案件
            var casearea = from a in DryDB.CaseDetail
                             where a.ApplyYear == applyyear && a.Step >= 7
                             group a by a.ApplyUnit into gp
                             select new
                             {
                                 ApplyUnit = gp.Key,
                                 Buildarea = gp.Sum(m => m.buildarea) / 10000,
                                 Rec = gp.Count()
                             };
            foreach (var item in casearea)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.BudgetFarmArea = (decimal?)item.Buildarea ?? 0;
                    newdata.BudgetFarmer = item.Rec;
                    result.Add(newdata);
                }else
                {
                    data.BudgetFarmArea = (decimal?)item.Buildarea ?? 0;
                    data.BudgetFarmer = item.Rec;
                }
            }
            #endregion

            #region 已編列預算
            var casebudget = from a in DryDB.SummaryView
                             join b in DryDB.TotalFee on a.MapNo equals b.MapNo
                             where a.ApplyYear == applyyear && a.Step >= 7
                             group b by a.ApplyUnit into gp
                             select new
                             {
                                 ApplyUnit = gp.Key,
                                 PayMoney = gp.Sum(m => m.GovSubsidy + m.GoldSubsidy),
                             };
            foreach (var item in casebudget)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.BudgetMoney = item.PayMoney;
                    result.Add(newdata);
                }
                else
                {
                    data.BudgetMoney = item.PayMoney;
                }
            }
            #endregion

            #region 已驗收面積、案件
            var caseareaComplete = from a in DryDB.CaseDetail
                           where a.ApplyYear == applyyear && a.Complete == true
                           group a by a.ApplyUnit into gp
                           select new
                           {
                               ApplyUnit = gp.Key,
                               Buildarea = gp.Sum(m => m.buildarea) / 10000,
                               Rec = gp.Count()
                           };
            foreach (var item in caseareaComplete)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.CompleteFarmArea = (decimal)item.Buildarea;
                    newdata.CompleteFarmer = item.Rec;
                    result.Add(newdata);
                }
                else
                {
                    data.CompleteFarmArea = (decimal)item.Buildarea;
                    data.CompleteFarmer = item.Rec;
                }
            }
            #endregion
            #region 已驗收金額
            var casebudgetComplete = from a in DryDB.SummaryView
                             join b in DryDB.TotalFee on a.MapNo equals b.MapNo
                             where a.ApplyYear == applyyear && a.Complete == true
                             group b by a.ApplyUnit into gp
                             select new
                             {
                                 ApplyUnit = gp.Key,
                                 PayMoney = gp.Sum(m => m.GovSubsidy + m.GoldSubsidy),
                             };
            foreach (var item in casebudgetComplete)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.CompleteBugdetMoney = item.PayMoney;
                    result.Add(newdata);
                }
                else
                {
                    data.CompleteBugdetMoney = item.PayMoney;
                }
            }
            #endregion

            return result;
        }
        /// <summary>
        /// 依據預算來源統計執行單位推廣面積與金額含黃金廊道
        /// </summary>
        /// <param name="applyyear"></param>
        /// <param name="feeUnit">預算來源</param>
        /// <returns></returns>
        public List<StatisticCoaView> StatIaByFee(int applyyear,int feeUnit)
        {
            List<StatisticCoaView> result = new List<StatisticCoaView>();
            #region 統計設定執行單位、預定目標
            var ialist = from a in DryDB.SubsidyLimit
                         where a.ApplyYear == applyyear
                         select new { a.ApplyUnit };
            foreach (var item in ialist.OrderBy(m => m.ApplyUnit))
            {
                StatisticCoaView newdata = new StatisticCoaView();
                newdata.Ia = item.ApplyUnit;
                newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                newdata.RsvFarmArea = DryDB.Subsidy.Where(m => m.IAUnit == item.ApplyUnit && m.SYear == applyyear && m.ApplyUnit == feeUnit)
                    .Select(m => m.FcstArea).DefaultIfEmpty(0).Sum();
                newdata.RsvBudget = DryDB.Subsidy.Where(m => m.IAUnit == item.ApplyUnit && m.SYear == applyyear && m.ApplyUnit == feeUnit)
                    .Select(m => m.Now_M).DefaultIfEmpty(0).Sum();
                result.Add(newdata);
            }
            #endregion
            
            #region 已編列面積、案件
            var casearea = from a in DryDB.CaseDetail
                           join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                           where a.ApplyYear == applyyear && a.Step >= 7 && b.GovPayUnit == 1 && b.ApplyUnit == feeUnit
                           group a by a.ApplyUnit into gp
                           select new
                           {
                               ApplyUnit = gp.Key,
                               Buildarea = gp.Sum(m => m.buildarea) / 10000,
                               Rec = gp.Count()
                           };
            //int i = 0;
            foreach (var item in casearea)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.BudgetFarmArea = (decimal)item.Buildarea;
                    newdata.BudgetFarmer = item.Rec;
                    result.Add(newdata);
                }
                else
                {
                    data.BudgetFarmArea = (decimal)item.Buildarea;
                    data.BudgetFarmer = item.Rec;
                }
                //i++;
            }
            #endregion

            #region 已編列預算
            var casebudget = from a in DryDB.SummaryView
                             join b in DryDB.TotalFee on a.MapNo equals b.MapNo
                             join c in DryDB.CasePayUnit on a.MapNo equals c.MapNo
                             where a.ApplyYear == applyyear && a.Step >= 7 && c.GovPayUnit == 1 && c.ApplyUnit == feeUnit
                             group b by a.ApplyUnit into gp
                             select new
                             {
                                 ApplyUnit = gp.Key,
                                 PayMoney = gp.Sum(m => m.GovSubsidy + m.GoldSubsidy),
                             };
            foreach (var item in casebudget)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.BudgetMoney = item.PayMoney;
                    result.Add(newdata);
                }
                else
                {
                    data.BudgetMoney = item.PayMoney;
                }
            }
            #endregion

            #region 已驗收面積、案件
            var caseareaComplete = from a in DryDB.CaseDetail
                                   join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                                   where a.ApplyYear == applyyear && a.Complete == true && b.GovPayUnit == 1 && b.ApplyUnit == feeUnit
                                   group a by a.ApplyUnit into gp
                                   select new
                                   {
                                       ApplyUnit = gp.Key,
                                       Buildarea = gp.Sum(m => m.buildarea) / 10000,
                                       Rec = gp.Count()
                                   };
            foreach (var item in caseareaComplete)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.CompleteFarmArea = (decimal)item.Buildarea;
                    newdata.CompleteFarmer = item.Rec;
                    result.Add(newdata);
                }
                else
                {
                    data.CompleteFarmArea = (decimal)item.Buildarea;
                    data.CompleteFarmer = item.Rec;
                }
            }
            #endregion
            #region 已驗收金額
            var casebudgetComplete = from a in DryDB.SummaryView
                                     join b in DryDB.TotalFee on a.MapNo equals b.MapNo
                                     join c in DryDB.CasePayUnit on a.MapNo equals c.MapNo
                                     where a.ApplyYear == applyyear && a.Complete == true && c.GovPayUnit == 1 && c.ApplyUnit == feeUnit
                                     group b by a.ApplyUnit into gp
                                     select new
                                     {
                                         ApplyUnit = gp.Key,
                                         PayMoney = gp.Sum(m => m.GovSubsidy + m.GoldSubsidy),
                                     };
            foreach (var item in casebudgetComplete)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.CompleteBugdetMoney = item.PayMoney;
                    result.Add(newdata);
                }
                else
                {
                    data.CompleteBugdetMoney = item.PayMoney;
                }
            }
            #endregion

            return result;
        }
        /// <summary>
        /// 統計各執行單位黃金廊道數據
        /// </summary>
        /// <param name="applyyear"></param>
        /// <param name="feeUnit"></param>
        /// <returns></returns>
        public List<StatisticCoaView> StatIaGoldByFee(int applyyear, int feeUnit)
        {
            List<StatisticCoaView> result = new List<StatisticCoaView>();
            #region 統計設定執行單位、預定目標
            var ialist = from a in DryDB.SubsidyLimit
                         where a.ApplyYear == applyyear
                         select new { a.ApplyUnit };
            foreach (var item in ialist.OrderBy(m => m.ApplyUnit))
            {
                StatisticCoaView newdata = new StatisticCoaView();
                newdata.Ia = item.ApplyUnit;
                newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                newdata.RsvFarmArea = DryDB.Subsidy.Where(m => m.IAUnit == item.ApplyUnit && m.SYear == applyyear && m.ApplyUnit == feeUnit)
                    .Select(m => m.FcstArea).DefaultIfEmpty(0).Sum();
                newdata.RsvBudget = DryDB.Subsidy.Where(m => m.IAUnit == item.ApplyUnit && m.SYear == applyyear && m.ApplyUnit == feeUnit)
                    .Select(m => m.Now_M).DefaultIfEmpty(0).Sum();
                result.Add(newdata);
            }
            #endregion

            #region 已編列面積、案件
            var casearea = from a in DryDB.CaseDetail
                           join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                           where a.ApplyYear == applyyear && a.Step >= 7 && a.Gold == true && b.GovPayUnit == 1 && b.ApplyUnit == feeUnit
                           group a by a.ApplyUnit into gp
                           select new
                           {
                               ApplyUnit = gp.Key,
                               Buildarea = gp.Sum(m => m.buildarea) / 10000,
                               Rec = gp.Count()
                           };
            //int i = 0;
            foreach (var item in casearea)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.BudgetFarmArea = (decimal)item.Buildarea;
                    newdata.BudgetFarmer = item.Rec;
                    result.Add(newdata);
                }
                else
                {
                    data.BudgetFarmArea = (decimal)item.Buildarea;
                    data.BudgetFarmer = item.Rec;
                }
                //i++;
            }
            #endregion

            #region 已編列預算
            var casebudget = from a in DryDB.SummaryView
                             join b in DryDB.TotalFee on a.MapNo equals b.MapNo
                             join c in DryDB.CasePayUnit on a.MapNo equals c.MapNo
                             where a.ApplyYear == applyyear && a.Step >= 7 && a.Gold == true && c.GovPayUnit == 1 && c.ApplyUnit == feeUnit
                             group b by a.ApplyUnit into gp
                             select new
                             {
                                 ApplyUnit = gp.Key,
                                 PayMoney = gp.Sum(m => m.GovSubsidy + m.GoldSubsidy),
                             };
            foreach (var item in casebudget)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.BudgetMoney = item.PayMoney;
                    result.Add(newdata);
                }
                else
                {
                    data.BudgetMoney = item.PayMoney;
                }
            }
            #endregion

            #region 已驗收面積、案件
            var caseareaComplete = from a in DryDB.CaseDetail
                                   join b in DryDB.CasePayUnit on a.MapNo equals b.MapNo
                                   where a.ApplyYear == applyyear && a.Complete == true && a.Gold == true && b.GovPayUnit == 1 && b.ApplyUnit == feeUnit
                                   group a by a.ApplyUnit into gp
                                   select new
                                   {
                                       ApplyUnit = gp.Key,
                                       Buildarea = gp.Sum(m => m.buildarea) / 10000,
                                       Rec = gp.Count()
                                   };
            foreach (var item in caseareaComplete)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.CompleteFarmArea = (decimal)item.Buildarea;
                    newdata.CompleteFarmer = item.Rec;
                    result.Add(newdata);
                }
                else
                {
                    data.CompleteFarmArea = (decimal)item.Buildarea;
                    data.CompleteFarmer = item.Rec;
                }
            }
            #endregion
            #region 已驗收金額
            var casebudgetComplete = from a in DryDB.SummaryView
                                     join b in DryDB.TotalFee on a.MapNo equals b.MapNo
                                     join c in DryDB.CasePayUnit on a.MapNo equals c.MapNo
                                     where a.ApplyYear == applyyear && a.Complete == true && a.Gold == true && c.GovPayUnit == 1 && c.ApplyUnit == feeUnit
                                     group b by a.ApplyUnit into gp
                                     select new
                                     {
                                         ApplyUnit = gp.Key,
                                         PayMoney = gp.Sum(m => m.GovSubsidy + m.GoldSubsidy),
                                     };
            foreach (var item in casebudgetComplete)
            {
                var data = result.Where(m => m.Ia == item.ApplyUnit).FirstOrDefault();
                if (data == null)
                {
                    StatisticCoaView newdata = new StatisticCoaView();
                    newdata.Ia = item.ApplyUnit;
                    newdata.IaName = new GetData().GetUnitName(item.ApplyUnit);
                    newdata.CompleteBugdetMoney = item.PayMoney;
                    result.Add(newdata);
                }
                else
                {
                    data.CompleteBugdetMoney = item.PayMoney;
                }
            }
            #endregion

            return result;
        }
        public byte[] exportStTable(List<StatisticHomeView2020> datalist)
        {
            ExcelPackage excel = new ExcelPackage();
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add("農水署統計報表");

            string[] FieldName = new string[] {"單位","預定推廣戶數","預定推廣面積(a)","預定推廣金額(千元)","調整推廣金額(千元)(b)",
            "建檔戶數(c)","建檔面積(d)","面積達成率%(d/a)","已編預算戶數","已編預算面積(e)","已編列補助款(f)","尚未編列補助款(g)=(b)-(f)",
            "已驗收戶數","已驗收面積","已驗收金額(h)","計畫執行率%(i)=(h)/(b)","單位面積補助款(j)=(f)/(e)","預估可再推廣面積(g)/(j)",
            "已驗收百分比(k)/(e)"
            };

            #region 農委會
            
           
            int rowindex = 1;
            for (int i = 0; i < FieldName.Length; i++)
            {
                sheet.Cells[rowindex, 1 + i].Value = FieldName[i];
                //sheet.Cells[rowindex, 1 + i].AutoFitColumns();
            }
            rowindex++;

            foreach (var item in datalist)
            {
                sheet.Cells[rowindex, 1].Value = item.IaName;
                sheet.Cells[rowindex, 2].Value = item.ReservationsofFarmer_coa;
                sheet.Cells[rowindex, 3].Value = item.ReservationsofFarm_coa;
                sheet.Cells[rowindex, 4].Value = item.ReservationsofMoney_coa;
                sheet.Cells[rowindex, 5].Value = item.Ch_ReservationsofMoney_coa;
                sheet.Cells[rowindex, 6].Value = item.NumofFarmer_coa;
                sheet.Cells[rowindex, 7].Value = item.NumofFarm_coa;
                sheet.Cells[rowindex, 8].Value =  item.ReachOfArea_coa;
                sheet.Cells[rowindex, 9].Value =  item.ProvisionOfFarmer_coa;
                sheet.Cells[rowindex, 10].Value = item.ProvisionOfFarm_coa;
                sheet.Cells[rowindex, 11].Value = item.ProvisionOfMoney_coa;
                sheet.Cells[rowindex, 12].Value =  item.Unusemoney_coa;
                sheet.Cells[rowindex, 13].Value = item.ProvisionofFarmerend_coa;
                sheet.Cells[rowindex, 14].Value = item.ProvisionOfFarmend_coa;
                sheet.Cells[rowindex, 15].Value = item.ProvisionOfMoneyend_coa;
                sheet.Cells[rowindex, 16].Value = item.PlanExe_coa;
                if (item.ProvisionOfFarm_coa > 0)
                {
                    sheet.Cells[rowindex, 17].Style.Numberformat.Format = "0.00";
                    sheet.Cells[rowindex, 17].Formula = "K" + rowindex + "/" + "J" + rowindex ;
                    sheet.Cells[rowindex, 18].Style.Numberformat.Format = "0.00";
                    sheet.Cells[rowindex, 18].Formula = "L" + rowindex + "/" + "Q" + rowindex;
                    sheet.Cells[rowindex, 19].Style.Numberformat.Format = "0.00%";
                    sheet.Cells[rowindex, 19].Formula = "N" + rowindex + "/" + "J" + rowindex;

                }
                else {
                    sheet.Cells[rowindex, 17].Value = 0;
                    sheet.Cells[rowindex, 18].Value = 0;
                    sheet.Cells[rowindex, 19].Value = 0;
                }
                
                rowindex++;


            }
            sheet.Cells[1, 1, rowindex, FieldName.Length + 1].AutoFitColumns();
            
            

            #endregion

            #region 瑠公
                    
            
            ExcelWorksheet sheet1 = excel.Workbook.Worksheets.Add("瑠公會統計報表");
            rowindex = 1;
            for (int i = 0; i < FieldName.Length; i++)
            {
                sheet1.Cells[rowindex, 1 + i].Value = FieldName[i];
                //sheet1.Cells[rowindex, 1 + i].AutoFitColumns();
            }
            rowindex++;
            foreach (var item in datalist)
            {
                sheet1.Cells[rowindex, 1].Value = item.IaName;
                sheet1.Cells[rowindex, 2].Value = item.ReservationsofFarmer_lu;
                sheet1.Cells[rowindex, 3].Value = item.ReservationsofFarm_lu;
                sheet1.Cells[rowindex, 4].Value = item.ReservationsofMoney_lu;
                sheet1.Cells[rowindex, 5].Value = item.Ch_ReservationsofMoney_lu;
                sheet1.Cells[rowindex, 6].Value = item.NumofFarmer_lu;
                sheet1.Cells[rowindex, 7].Value = item.NumofFarm_lu;
                sheet1.Cells[rowindex, 8].Value =  item.ReachOfArea_lu;
                sheet1.Cells[rowindex, 9].Value =  item.ProvisionOfFarmer_lu;
                sheet1.Cells[rowindex, 10].Value = item.ProvisionOfFarm_lu ;
                sheet1.Cells[rowindex, 11].Value = item.ProvisionOfMoney_lu;
                sheet1.Cells[rowindex, 12].Value =  item.Unusemoney_lu;
                sheet1.Cells[rowindex, 13].Value = item.ProvisionofFarmerend_lu;
                sheet1.Cells[rowindex, 14].Value = item.ProvisionOfFarmend_lu;
                sheet1.Cells[rowindex, 15].Value = item.ProvisionOfMoneyend_lu;
                sheet1.Cells[rowindex, 16].Value = item.PlanExe_lu;

                if (item.ProvisionOfFarm_lu > 0)
                {
                    sheet1.Cells[rowindex, 17].Style.Numberformat.Format = "0.00";
                    sheet1.Cells[rowindex, 17].Formula = "K" + rowindex + "/" + "J" + rowindex;
                    sheet1.Cells[rowindex, 18].Style.Numberformat.Format = "0.00";
                    sheet1.Cells[rowindex, 18].Formula = "L" + rowindex + "/" + "Q" + rowindex;
                    sheet1.Cells[rowindex, 19].Style.Numberformat.Format = "0.00%";
                    sheet1.Cells[rowindex, 19].Formula = "N" + rowindex + "/" + "J" + rowindex;

                }
                else
                {
                    sheet1.Cells[rowindex, 17].Value = 0;
                    sheet1.Cells[rowindex, 18].Value = 0;
                    sheet1.Cells[rowindex, 19].Value = 0;
                }

                rowindex++;
            }
            sheet1.Cells[1, 1, rowindex, FieldName.Length + 1].AutoFitColumns();

            #endregion
            #region 七星


            ExcelWorksheet sheet2 = excel.Workbook.Worksheets.Add("七星會統計報表");
            rowindex = 1;
            for (int i = 0; i < FieldName.Length; i++)
            {
                sheet2.Cells[rowindex, 1 + i].Value = FieldName[i];
                //sheet1.Cells[rowindex, 1 + i].AutoFitColumns();
            }
            rowindex++;
            foreach (var item in datalist)
            {
                sheet2.Cells[rowindex, 1].Value = item.IaName;
                sheet2.Cells[rowindex, 2].Value = item.ReservationsofFarmer_cx;
                sheet2.Cells[rowindex, 3].Value = item.ReservationsofFarm_cx;
                sheet2.Cells[rowindex, 4].Value = item.ReservationsofMoney_cx;
                sheet2.Cells[rowindex, 5].Value = item.Ch_ReservationsofMoney_cx;
                sheet2.Cells[rowindex, 6].Value = item.NumofFarmer_cx;
                sheet2.Cells[rowindex, 7].Value = item.NumofFarm_cx;
                sheet2.Cells[rowindex, 8].Value = item.ReachOfArea_cx;
                sheet2.Cells[rowindex, 9].Value = item.ProvisionOfFarmer_cx;
                sheet2.Cells[rowindex, 10].Value = item.ProvisionOfFarm_cx;
                sheet2.Cells[rowindex, 11].Value = item.ProvisionOfMoney_cx;
                sheet2.Cells[rowindex, 12].Value = item.Unusemoney_cx;
                sheet2.Cells[rowindex, 13].Value = item.ProvisionofFarmerend_cx;
                sheet2.Cells[rowindex, 14].Value = item.ProvisionOfFarmend_cx;
                sheet2.Cells[rowindex, 15].Value = item.ProvisionOfMoneyend_cx;
                sheet2.Cells[rowindex, 16].Value = item.PlanExe_cx;

                if (item.ProvisionOfFarm_cx > 0)
                {
                    sheet2.Cells[rowindex, 17].Style.Numberformat.Format = "0.00";
                    sheet2.Cells[rowindex, 17].Formula = "K" + rowindex + "/" + "J" + rowindex;
                    sheet2.Cells[rowindex, 18].Style.Numberformat.Format = "0.00";
                    sheet2.Cells[rowindex, 18].Formula = "L" + rowindex + "/" + "Q" + rowindex;
                    sheet2.Cells[rowindex, 19].Style.Numberformat.Format = "0.00%";
                    sheet2.Cells[rowindex, 19].Formula = "N" + rowindex + "/" + "J" + rowindex;

                }
                else
                {
                    sheet2.Cells[rowindex, 17].Value = 0;
                    sheet2.Cells[rowindex, 18].Value = 0;
                    sheet2.Cells[rowindex, 19].Value = 0;
                }

                rowindex++;
            }
            sheet2.Cells[1, 1, rowindex, FieldName.Length + 1].AutoFitColumns();

            #endregion

            #region 黃金廊道
            ExcelWorksheet sheet4 = excel.Workbook.Worksheets.Add("黃金廊道統計報表");
            rowindex = 1;
            
            string[] FieldName1 = new string[] {"單位","預定推廣戶數","預定推廣面積(a)","預定推廣金額(千元)","調整推廣金額(千元)(b)",
            "建檔戶數(c)","建檔面積(d)","面積達成率%(d/a)","已編預算戶數","已編預算面積(e)","已編列補助款(f)","尚未編列補助款(g)=(b)-(f)",
            "已驗收戶數","已驗收面積","已驗收金額(h)","計畫執行率%(i)=(h)/(b)","單位面積補助款(j)=(f)/(e)","預估可再推廣面積(g)/(j)",
            "已驗收百分比(k)/(e)"
            };
            for (int i = 0; i < FieldName1.Length; i++)
            {
                sheet4.Cells[rowindex, 1 + i].Value = FieldName1[i];
                //sheet1.Cells[rowindex, 1 + i].AutoFitColumns();
            }
            rowindex++;
            #region 舊黃金廊道表格
            //foreach (var item in datalist)
            //{
            //    sheet4.Cells[rowindex, 1].Value = item.IaName;
            //    sheet4.Cells[rowindex, 2].Value = item.NumofFarmer_gold;
            //    sheet4.Cells[rowindex, 3].Value = item.NumofFarm_gold;
            //    sheet4.Cells[rowindex, 4].Value = item.ProvisionOfFarmer_gold;
            //    sheet4.Cells[rowindex, 5].Value = item.ProvisionOfFarm_gold;
            //    if (item.ProvisionOfMoney_gold != null)
            //    {
            //        sheet4.Cells[rowindex, 6].Value = item.ProvisionOfMoney_gold.govpay;
            //        sheet4.Cells[rowindex, 7].Value = item.ProvisionOfMoney_gold.goldpay;
            //        sheet4.Cells[rowindex, 8].Value = item.ProvisionOfMoney_gold.lupay;
            //    }
            //    sheet4.Cells[rowindex, 9].Value = item.ProvisionofFarmerend_gold;
            //    sheet4.Cells[rowindex, 10].Value = item.ProvisionOfFarmend_gold;
            //    if (item.ProvisionOfMoneyend_gold != null)
            //    {
            //        sheet4.Cells[rowindex, 11].Value = item.ProvisionOfMoneyend_gold.govpay;
            //        sheet4.Cells[rowindex, 12].Value = item.ProvisionOfMoneyend_gold.goldpay;
            //        sheet4.Cells[rowindex, 13].Value = item.ProvisionOfMoneyend_gold.lupay;
            //    }

            //    rowindex++;
            //}
            #endregion
            #region 新黃金廊道表格
            foreach (var item in datalist)
            {
                sheet4.Cells[rowindex, 1].Value = item.IaName;
                sheet4.Cells[rowindex, 2].Value = item.ReservationsofFarmer_gold;
                sheet4.Cells[rowindex, 3].Value = item.ReservationsofFarm_gold;
                sheet4.Cells[rowindex, 4].Value = item.ReservationsofMoney_gold;
                sheet4.Cells[rowindex, 5].Value = item.Ch_ReservationsofMoney_gold;
                sheet4.Cells[rowindex, 6].Value = item.NumofFarmer_gold;
                sheet4.Cells[rowindex, 7].Value = item.NumofFarm_gold;
                sheet4.Cells[rowindex, 8].Value = item.ReachOfArea_gold;
                sheet4.Cells[rowindex, 9].Value = item.ProvisionOfFarmer_gold;
                sheet4.Cells[rowindex, 10].Value = item.ProvisionOfFarm_gold;
                sheet4.Cells[rowindex, 11].Value = item.ProvisionOfMoney_gold;
                sheet4.Cells[rowindex, 12].Value = item.Unusemoney_gold;
                sheet4.Cells[rowindex, 13].Value = item.ProvisionofFarmerend_gold;
                sheet4.Cells[rowindex, 14].Value = item.ProvisionOfFarmend_gold;
                sheet4.Cells[rowindex, 15].Value = item.ProvisionOfMoneyend_gold;
                sheet4.Cells[rowindex, 16].Value = item.PlanExe_gold;

                if (item.ProvisionOfFarm_cx > 0)
                {
                    sheet4.Cells[rowindex, 17].Style.Numberformat.Format = "0.00";
                    sheet4.Cells[rowindex, 17].Formula = "K" + rowindex + "/" + "J" + rowindex;
                    sheet4.Cells[rowindex, 18].Style.Numberformat.Format = "0.00";
                    sheet4.Cells[rowindex, 18].Formula = "L" + rowindex + "/" + "Q" + rowindex;
                    sheet4.Cells[rowindex, 19].Style.Numberformat.Format = "0.00%";
                    sheet4.Cells[rowindex, 19].Formula = "N" + rowindex + "/" + "J" + rowindex;

                }
                else
                {
                    sheet4.Cells[rowindex, 17].Value = 0;
                    sheet4.Cells[rowindex, 18].Value = 0;
                    sheet4.Cells[rowindex, 19].Value = 0;
                }

                rowindex++;
            }
            sheet4.Cells[1, 1, rowindex, FieldName.Length + 1].AutoFitColumns();
            #endregion

            #endregion

            byte[] file = excel.GetAsByteArray();
            return file;
        }

        public byte[] exportIaFees(int ayear)
        {
            #region 統計資料
            List<StatisticCoaView> coadatas = StatIaByFee(ayear, 0);
            List<StatisticCoaView> cxdatas = StatIaByFee(ayear, 16);
            List<StatisticCoaView> ludatas = StatIaByFee(ayear, 17);
            var alldatas = coadatas.Concat(cxdatas).Concat(ludatas);
            var alldataST = from a in alldatas
                            group a by new { a.Ia, a.IaName } into gp
                            select new
                            {
                                Ia = gp.Key.Ia,
                                IaName = gp.Key.IaName,
                                RsvBudget = gp.Sum(m => m.RsvBudget),
                                RsvFarmArea = gp.Sum(m => m.RsvFarmArea),
                                BudgetFarmer = gp.Sum(m => m.BudgetFarmer),
                                BudgetFarmarea = gp.Sum(m => m.BudgetFarmArea),
                                BudgetMoney = gp.Sum(m => m.BudgetMoney),
                                CompleteFarmer = gp.Sum(m => m.CompleteFarmer),
                                CompleteFareArea = gp.Sum(m => m.CompleteFarmArea),
                                CompleteBudgetMoney = gp.Sum(m => m.CompleteBugdetMoney)
                            };
            List<StatisticCoaView> TotalDatas = new List<StatisticCoaView>();
            foreach (var item in alldataST)
            {
                var data = new StatisticCoaView();
                data.Ia = item.Ia;
                data.IaName = item.IaName;
                data.RsvFarmArea = item.RsvFarmArea;
                data.RsvBudget = item.RsvBudget;
                data.BudgetFarmer = item.BudgetFarmer;
                data.BudgetFarmArea = item.BudgetFarmarea;
                data.BudgetMoney = item.BudgetMoney;
                data.CompleteFarmer = item.CompleteFarmer;
                data.CompleteFarmArea = item.CompleteFareArea;
                data.CompleteBugdetMoney = item.CompleteBudgetMoney;
                TotalDatas.Add(data);
            }
            #endregion

            ExcelPackage excel = new ExcelPackage();

            #region 農委會
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add("農水署統計報表");
            writeIaFees(sheet, coadatas);
            #endregion

            #region 七星
            ExcelWorksheet sheet16 = excel.Workbook.Worksheets.Add("七星統計報表");
            writeIaFees(sheet16, cxdatas);
            #endregion
            #region 瑠公
            ExcelWorksheet sheet17 = excel.Workbook.Worksheets.Add("瑠公統計報表");
            writeIaFees(sheet17, ludatas);
            #endregion
            #region 合計
            ExcelWorksheet sheet00 = excel.Workbook.Worksheets.Add("合計統計報表");
            writeIaFees(sheet00, TotalDatas);
            #endregion
            byte[] file = excel.GetAsByteArray();
            return file;
        }
        public byte[] exportIaGoldFees(int ayear)
        {
            #region 統計資料            
            List<StatisticCoaView> coadatas = StatIaGoldByFee(ayear, 0);
            List<StatisticCoaView> cxdatas = StatIaGoldByFee(ayear, 16);
            List<StatisticCoaView> ludatas = StatIaGoldByFee(ayear, 17);

            var alldatas = coadatas.Concat(cxdatas).Concat(ludatas);
            var alldataST = from a in alldatas
                            group a by new { a.Ia, a.IaName } into gp
                            select new
                            {
                                Ia = gp.Key.Ia,
                                IaName = gp.Key.IaName,
                                RsvBudget = gp.Sum(m => m.RsvBudget),
                                RsvFarmArea = gp.Sum(m => m.RsvFarmArea),
                                BudgetFarmer = gp.Sum(m => m.BudgetFarmer),
                                BudgetFarmarea = gp.Sum(m => m.BudgetFarmArea),
                                BudgetMoney = gp.Sum(m => m.BudgetMoney),
                                CompleteFarmer = gp.Sum(m => m.CompleteFarmer),
                                CompleteFareArea = gp.Sum(m => m.CompleteFarmArea),
                                CompleteBudgetMoney = gp.Sum(m => m.CompleteBugdetMoney)
                            };
            List<StatisticCoaView> TotalDatas = new List<StatisticCoaView>();
            foreach (var item in alldataST)
            {
                var data = new StatisticCoaView();
                data.Ia = item.Ia;
                data.IaName = item.IaName;
                data.RsvFarmArea = item.RsvFarmArea;
                data.RsvBudget = item.RsvBudget;
                data.BudgetFarmer = item.BudgetFarmer;
                data.BudgetFarmArea = item.BudgetFarmarea;
                data.BudgetMoney = item.BudgetMoney;
                data.CompleteFarmer = item.CompleteFarmer;
                data.CompleteFarmArea = item.CompleteFareArea;
                data.CompleteBugdetMoney = item.CompleteBudgetMoney;
                TotalDatas.Add(data);
            }
            #endregion

            ExcelPackage excel = new ExcelPackage();

            #region 農委會
            ExcelWorksheet sheet = excel.Workbook.Worksheets.Add("農水署統計報表");
            writeIaFees(sheet, coadatas);
            #endregion

            #region 七星
            ExcelWorksheet sheet16 = excel.Workbook.Worksheets.Add("七星統計報表");
            writeIaFees(sheet16, cxdatas);
            #endregion
            #region 瑠公
            ExcelWorksheet sheet17 = excel.Workbook.Worksheets.Add("瑠公統計報表");
            writeIaFees(sheet17, ludatas);
            #endregion
            #region 合計
            ExcelWorksheet sheet00 = excel.Workbook.Worksheets.Add("合計統計報表");
            writeIaFees(sheet00, TotalDatas);
            #endregion
            byte[] file = excel.GetAsByteArray();
            return file;
        }


        public byte[] exportIndiagenTown(int applyyear)
        {
            //var qry = from a in DryDB.CaseDetail
            //          join b in DryDB.TotalFee on a.MapNo equals b.MapNo
            //          where a.ApplyYear == applyyear && a.Step >=7
            //          group new { a, b } by new { a.landCity, a.landTown } into c                      
            //          select new { LandCity = c.Key.landCity, LandTown = c.Key.landTown, 
            //              Rec = c.Count(), Area = c.Sum(o => o.a.buildarea / 10000d), Fee = c.Sum(o=> o.b.GovSubsidy+o.b.GoldSubsidy) };
            //var qryEnd = from a in qry
            //             join b in DryDB.Indigenous on new { a.LandCity, a.LandTown } equals new { LandCity = b.City, LandTown = b.Town }
            //             orderby b.CityCode + a.LandTown
            //             select a;
            var qryEnd = from a in DryDB.CaseDetail
                         join b in DryDB.TotalFee on a.MapNo equals b.MapNo
                         where a.ApplyYear == applyyear && a.Step >= 7 && a.Indigenous == 1
                         group new { a, b } by new { a.landCity, a.landTown } into c
                         orderby c.Key.landCity+c.Key.landTown
                         select new
                         {
                             LandCity = c.Key.landCity,
                             LandTown = c.Key.landTown,
                             Rec = c.Count(),
                             Area = c.Sum(o => o.a.buildarea) / 10000d,
                             Fee = c.Sum(o => o.b.GovSubsidy + o.b.GoldSubsidy)
                         };
                              
            

            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                document.NewPage();
                string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
                BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                document.Add(new Paragraph($"{applyyear}年原民鄉鎮補助統計一覽表", new iTextSharp.text.Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER, SpacingAfter=18 });

                PdfPTable table = new PdfPTable(new float[] { 15f, 15f, 15f, 20f,20F });
                table.TotalWidth = 495f;
                table.WidthPercentage = 100f;
                
                table.AddCell(new PdfPCell(new Phrase("縣市", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("鄉鎮", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("補助案件數", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("補助面積(公頃)", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("補助金額(元)", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                
                int totalRec = 0;
                double totalArea = 0d;
                long totalFee = 0;
                foreach (var item in qryEnd)
                {
                    table.AddCell(new PdfPCell(new Phrase($"{item.LandCity}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.LandTown}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.Rec}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{string.Format("{0:N2}",item.Area)}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.Fee.ToString("###,###")}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    totalRec += item.Rec;
                    totalArea += item.Area ?? 0d;
                    totalFee += item.Fee ;

                }
                
                table.AddCell(new PdfPCell(new Phrase(string.Empty, new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("合計", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase($"{totalRec}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase($"{string.Format("{0:N2}", totalArea)}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase($"{totalFee.ToString("###,###")}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });


                document.Add(table);
                string today = $"統計日期： {DateTime.Now.Year - 1911} 年 {DateTime.Now.Month} 月 {DateTime.Now.Day} 日";
                document.Add(new Paragraph(today, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingBefore = 24 });
                document.Close();
                return stream.GetBuffer();
            }
        }

        public byte[] exportIndiagenIa(int applyyear)
        {
            //var qry = from a in DryDB.CaseDetail
            //          join b in DryDB.TotalFee on a.MapNo equals b.MapNo
            //          where a.ApplyYear == applyyear && a.Step >=7
            //          group new { a, b } by new { a.landCity, a.landTown } into c                      
            //          select new { LandCity = c.Key.landCity, LandTown = c.Key.landTown, 
            //              Rec = c.Count(), Area = c.Sum(o => o.a.buildarea / 10000d), Fee = c.Sum(o=> o.b.GovSubsidy+o.b.GoldSubsidy) };
            //var qryEnd = from a in qry
            //             join b in DryDB.Indigenous on new { a.LandCity, a.LandTown } equals new { LandCity = b.City, LandTown = b.Town }
            //             orderby b.CityCode + a.LandTown
            //             select a;
            var qryEnd = from a in DryDB.CaseDetail
                         join b in DryDB.TotalFee on a.MapNo equals b.MapNo
                         where a.ApplyYear == applyyear && a.Step >= 7 && a.Indigenous == 1
                         group new { a, b } by a.ApplyUnit into c
                         orderby c.Key
                         select new
                         {
                             Ia = c.Key,
                             Rec = c.Count(),
                             Area = c.Sum(o => o.a.buildarea) / 10000d,
                             Fee = c.Sum(o => o.b.GovSubsidy + o.b.GoldSubsidy)
                         };



            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                document.NewPage();
                string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
                BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                document.Add(new Paragraph($"{applyyear}年各管理處補助原民鄉統計一覽表", new iTextSharp.text.Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 18 });

                PdfPTable table = new PdfPTable(new float[] { 30f, 15f, 20f, 20F });
                table.TotalWidth = 495f;
                table.WidthPercentage = 100f;
                
                
                

                table.AddCell(new PdfPCell(new Phrase("管理處", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("補助案件數", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("補助面積(公頃)", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("補助金額(元)", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                
                int totalRec = 0;
                double totalArea = 0d;
                long totalFee = 0;
                foreach (var item in qryEnd)
                {
                    string iacns = CommonDB.Unit.Where(o => o.Unit_Id == item.Ia).FirstOrDefault().Unit1;
                    
                    table.AddCell(new PdfPCell(new Phrase($"{iacns.Replace("農業部","")  }", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.Rec}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{string.Format("{0:N2}", item.Area)}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.Fee.ToString("N0")}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    totalRec += item.Rec;
                    totalArea += item.Area ?? 0d;
                    totalFee += item.Fee;

                }
                
                
                table.AddCell(new PdfPCell(new Phrase("合計", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase($"{totalRec}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase($"{string.Format("{0:N2}", totalArea)}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase($"{totalFee.ToString("N0")}", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });


                document.Add(table);
                string today = $"統計日期： {DateTime.Now.Year - 1911} 年 {DateTime.Now.Month} 月 {DateTime.Now.Day} 日";
                document.Add(new Paragraph(today, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingBefore = 24 });
                document.Close();
                return stream.GetBuffer();
            }
        }

        public byte[] exportIaKPI(int applyyear)
        {
            var qry = from a in DryDB.CaseDetail
                      join b in DryDB.TotalFee on a.MapNo equals b.MapNo into c
                      from d in c.DefaultIfEmpty()
                      where a.ApplyYear == applyyear && a.ApplyUnit != 99
                      group new { a, d } by a.ApplyUnit into e
                      orderby e.Key
                      select new { e.Key, Rec = e.Count(), Area = e.Sum(o => o.a.buildarea ?? 0) / 10000d, Fee = e.Sum(o => ((int?)o.d.GovSubsidy ?? 0 + (int?)o.d.GoldSubsidy ?? 0))  };

            using (Document document = new Document(PageSize.A4))
            {
                MemoryStream stream = new MemoryStream();
                PdfWriter pdfw = PdfWriter.GetInstance(document, stream);
                document.Open();
                document.NewPage();
                string fontPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\Fonts\kaiu.ttf";
                BaseFont baseFT = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                document.Add(new Paragraph($"{applyyear}年各管理處執行進度表", new iTextSharp.text.Font(baseFT, 18)) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 18 });

                PdfPTable table = new PdfPTable(new float[] { 15f, 15f, 20f, 20f, 20f, 20f });
                table.TotalWidth = 495f;
                table.WidthPercentage = 100f;
                

                table.AddCell(new PdfPCell(new Phrase("管理處", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("核定辦理\n金額(元)", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("辦理補助\n案件數", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("辦理補助\n面積(公頃)", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("辦理補助\n金額(元)", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                table.AddCell(new PdfPCell(new Phrase("補助款\n使用達成率%", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });

                

                foreach (var item in qry)
                {
                    string iacns = CommonDB.Unit.Where(o => o.Unit_Id == item.Key).FirstOrDefault().Unit1;
                    int iaSubsidy = DryDB.Subsidy.Where(o => o.IAUnit == item.Key && o.SYear == applyyear).Sum(o =>(int?)o.Now_M ?? 0);
                    table.AddCell(new PdfPCell(new Phrase($"{iacns.Replace("農業部", "")  }", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{iaSubsidy.ToString("N0") }", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.Rec.ToString("N0") }", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.Area.ToString("N2") }", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    table.AddCell(new PdfPCell(new Phrase($"{item.Fee.ToString("N0") }", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                    if (iaSubsidy > 0)
                    {
                        double kpi = ((double)item.Fee / (double)iaSubsidy ) * 100d;
                        if (kpi >= 100)
                        {
                            table.AddCell(new PdfPCell(new Phrase($" {kpi.ToString("N2")} %", new Font(baseFT, 12, Font.NORMAL, BaseColor.RED))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                        }
                        else if (kpi > 50 && kpi < 100)
                        {
                            table.AddCell(new PdfPCell(new Phrase($" {kpi.ToString("N2")} %", new Font(baseFT, 12, Font.NORMAL, BaseColor.BLUE))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                        }
                        else
                        {
                            table.AddCell(new PdfPCell(new Phrase($" {kpi.ToString("N2")} %", new Font(baseFT, 12, Font.NORMAL, BaseColor.GREEN))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });
                        }

                        

                    }else table.AddCell(new PdfPCell(new Phrase($"0.00 %", new Font(baseFT, 12))) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, PaddingBottom = 6 });

                }
                document.Add(table);
                string today = $"統計日期： {DateTime.Now.Year - 1911} 年 {DateTime.Now.Month} 月 {DateTime.Now.Day} 日";
                document.Add(new Paragraph(today, new iTextSharp.text.Font(baseFT, 12)) { Alignment = Element.ALIGN_LEFT, SpacingBefore = 24 });
                document.Close();
                return stream.GetBuffer();

            }
        }

            /// <summary>
            /// 填寫全省單位各預算來源執行進度統計資料報表
            /// </summary>
            /// <param name="sheet"></param>
            /// <param name="datas"></param>
            private void writeIaFees(ExcelWorksheet sheet, List<StatisticCoaView> datas)
        {
            string[] FieldName = new string[] {"執行單位","預定執行面積(a)","預定執行預算(b)","已編預算戶數(c)","已編預算面積(d)","已編列補助款(e)",
            "未編列補助款(f)","已驗收戶數(g)","已驗收面積(h)","已驗收金額(i)","計畫執行率%(j)=(d)/(a)"
            };
            int rowindex = 1;
            for (int i = 0; i < FieldName.Length; i++)
            {
                sheet.Cells[rowindex, 1 + i].Value = FieldName[i];

            }
            rowindex++;
            foreach (var item in datas)
            {
                sheet.Cells[rowindex, 1].Value = item.IaName;
                sheet.Cells[rowindex, 2].Value = item.RsvFarmArea;
                sheet.Cells[rowindex, 3].Value = item.RsvBudget;
                sheet.Cells[rowindex, 4].Value = item.BudgetFarmer;
                sheet.Cells[rowindex, 5].Value = item.BudgetFarmArea;
                sheet.Cells[rowindex, 6].Value = item.BudgetMoney;
                sheet.Cells[rowindex, 7].Value = item.RsvBudget - item.BudgetMoney;
                sheet.Cells[rowindex, 8].Value = item.CompleteFarmer;
                sheet.Cells[rowindex, 9].Value = item.CompleteFarmArea;
                sheet.Cells[rowindex, 10].Value = item.CompleteBugdetMoney;

                if (item.BudgetMoney > 0)
                {
                    sheet.Cells[rowindex, 11].Style.Numberformat.Format = "0.00%";
                    sheet.Cells[rowindex, 11].Formula = "E" + rowindex + "/" + "B" + rowindex;
                    //sheet.Cells[rowindex, 18].Style.Numberformat.Format = "0.00";
                    //sheet.Cells[rowindex, 18].Formula = "L" + rowindex + "/" + "Q" + rowindex;
                    //sheet.Cells[rowindex, 19].Style.Numberformat.Format = "0.00%";
                    //sheet.Cells[rowindex, 19].Formula = "N" + rowindex + "/" + "J" + rowindex;

                }
                else
                {
                    sheet.Cells[rowindex, 10].Value = 0;
                    //sheet.Cells[rowindex, 18].Value = 0;
                    //sheet.Cells[rowindex, 19].Value = 0;
                }

                rowindex++;
            }
            sheet.Cells[1, 1, rowindex, FieldName.Length + 1].AutoFitColumns();
        }
    }
}
