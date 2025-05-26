using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.ViewModel
{
    public class StatisticHomeView
    {
        public int ReservationsofFarmer_coa { get; set; }//預定戶數
        public int ReservationsofFarm_coa { get; set; }//預定面積
        public decimal ReservationsofMoney_coa { get; set; }//預定經費
        public decimal Ch_ReservationsofMoney_coa { get; set; }//俢改經費
        public int NumofFarmer_coa { get; set; }//建檔戶數
        public double NumofFarm_coa { get; set; }//建檔面積
        public double ReachOfArea_coa { get; set; } //已達成面積
        public int ProvisionOfFarmer_coa { get; set; }//已編列戶數
        public double ProvisionOfFarm_coa { get; set; }//已編列面積
        public decimal ProvisionOfMoney_coa { get; set; }//已編列經費
        public decimal Unusemoney_coa { get; set; }//未編列經費
        public int ProvisionofFarmerend_coa { get; set; }//結案戶數
        public double ProvisionOfFarmend_coa { get; set; }//已結案面積
        public decimal ProvisionOfMoneyend_coa { get; set; }//已結案經費
        public double PlanExe_coa { get; set; }//計畫執行率
        public decimal Desinge_coa { get; set; }//設計費
        public int ReservationsofFarmer_lu { get; set; }//預定戶數
        public int ReservationsofFarm_lu { get; set; }//預定面積
        public decimal ReservationsofMoney_lu { get; set; }//預定經費
        public decimal Ch_ReservationsofMoney_lu { get; set; }//俢改經費
        public int NumofFarmer_lu { get; set; }//建檔戶數
        public double NumofFarm_lu { get; set; }//建檔面積
        public double ReachOfArea_lu { get; set; } //已達成面積
        public int ProvisionOfFarmer_lu { get; set; }//已編列戶數
        public double ProvisionOfFarm_lu { get; set; }//已編列面積
        public decimal ProvisionOfMoney_lu { get; set; }//已編列經費
        public decimal Unusemoney_lu { get; set; }//未編列經費
        public int ProvisionofFarmerend_lu { get; set; }//結案戶數
        public double ProvisionOfFarmend_lu { get; set; }//已結案面積
        public decimal ProvisionOfMoneyend_lu { get; set; }//已結案經費
        public double PlanExe_lu { get; set; }//計畫執行率
        public decimal Desinge_lu { get; set; }//設計費

        public int ReservationsofFarmer_pis { get; set; }//預定戶數
        public int ReservationsofFarm_pis { get; set; }//預定面積
        public decimal ReservationsofMoney_pis { get; set; }//預定經費
        public decimal Ch_ReservationsofMoney_pis { get; set; }//俢改經費
        public int NumofFarmer_pis { get; set; }//建檔戶數
        public double NumofFarm_pis { get; set; }//建檔面積
        public double ReachOfArea_pis { get; set; } //已達成面積
        public int ProvisionOfFarmer_pis { get; set; }//已編列戶數
        public double ProvisionOfFarm_pis { get; set; }//已編列面積
        public decimal ProvisionOfMoney_pis { get; set; }//已編列經費
        public decimal Unusemoney_pis { get; set; }//未編列經費
        public int ProvisionofFarmerend_pis { get; set; }//結案戶數
        public double ProvisionOfFarmend_pis { get; set; }//已結案面積
        public decimal ProvisionOfMoneyend_pis { get; set; }//已結案經費
        public double PlanExe { get; set; }//計畫執行率
        public decimal Desinge_pis { get; set; }//設計費

        public int ReservationsofFarmer_gold { get; set; }//預定戶數
        public int ReservationsofFarm_gold { get; set; }//預定面積
        public decimal ReservationsofMoney_gold { get; set; }//預定經費
        public decimal Ch_ReservationsofMoney_gold { get; set; }//俢改經費
        public int NumofFarmer_gold { get; set; }//建檔戶數
        public double NumofFarm_gold { get; set; }//建檔面積
        public double ReachOfArea_gold { get; set; } //已達成面積
        public int ProvisionOfFarmer_gold { get; set; }//已編列戶數
        public double ProvisionOfFarm_gold { get; set; }//已編列面積
        public feeStruct ProvisionOfMoney_gold { get; set; }//已編列經費
        public decimal Unusemoney_gold { get; set; }//未編列經費
        public int ProvisionofFarmerend_gold { get; set; }//結案戶數
        public double ProvisionOfFarmend_gold { get; set; }//已結案面積
        public feeStruct ProvisionOfMoneyend_gold { get; set; }//已結案經費
        public double PlanExe_gold { get; set; }//計畫執行率
        public decimal Desinge_gold { get; set; }//設計費
        public int ReservationsofFarmer_cx { get; set; }//預定戶數
        public int ReservationsofFarm_cx { get; set; }//預定面積
        public decimal ReservationsofMoney_cx { get; set; }//預定經費
        public decimal Ch_ReservationsofMoney_cx { get; set; }//俢改經費
        public int NumofFarmer_cx { get; set; }//建檔戶數
        public double NumofFarm_cx { get; set; }//建檔面積
        public double ReachOfArea_cx { get; set; } //已達成面積
        public int ProvisionOfFarmer_cx { get; set; }//已編列戶數
        public double ProvisionOfFarm_cx { get; set; }//已編列面積
        public decimal ProvisionOfMoney_cx { get; set; }//已編列經費
        public decimal Unusemoney_cx { get; set; }//未編列經費
        public int ProvisionofFarmerend_cx { get; set; }//結案戶數
        public double ProvisionOfFarmend_cx { get; set; }//已結案面積
        public decimal ProvisionOfMoneyend_cx { get; set; }//已結案經費
        public double PlanExe_cx { get; set; }//計畫執行率
        public decimal Desinge_cx { get; set; }//設計費
        public string IaName { get; set; }
    }

    public class StatisticHomeView2020
    {
        public int ReservationsofFarmer_coa { get; set; }//預定戶數
        public int ReservationsofFarm_coa { get; set; }//預定面積
        public decimal ReservationsofMoney_coa { get; set; }//預定經費
        public decimal Ch_ReservationsofMoney_coa { get; set; }//俢改經費
        public int NumofFarmer_coa { get; set; }//建檔戶數
        public double NumofFarm_coa { get; set; }//建檔面積
        public double ReachOfArea_coa { get; set; } //已達成面積
        public int ProvisionOfFarmer_coa { get; set; }//已編列戶數
        public double ProvisionOfFarm_coa { get; set; }//已編列面積
        public decimal ProvisionOfMoney_coa { get; set; }//已編列經費
        public decimal Unusemoney_coa { get; set; }//未編列經費
        public int ProvisionofFarmerend_coa { get; set; }//結案戶數
        public double ProvisionOfFarmend_coa { get; set; }//已結案面積
        public decimal ProvisionOfMoneyend_coa { get; set; }//已結案經費
        public double PlanExe_coa { get; set; }//計畫執行率
        public decimal Desinge_coa { get; set; }//設計費
        public int ReservationsofFarmer_lu { get; set; }//預定戶數
        public int ReservationsofFarm_lu { get; set; }//預定面積
        public decimal ReservationsofMoney_lu { get; set; }//預定經費
        public decimal Ch_ReservationsofMoney_lu { get; set; }//俢改經費
        public int NumofFarmer_lu { get; set; }//建檔戶數
        public double NumofFarm_lu { get; set; }//建檔面積
        public double ReachOfArea_lu { get; set; } //已達成面積
        public int ProvisionOfFarmer_lu { get; set; }//已編列戶數
        public double ProvisionOfFarm_lu { get; set; }//已編列面積
        public decimal ProvisionOfMoney_lu { get; set; }//已編列經費
        public decimal Unusemoney_lu { get; set; }//未編列經費
        public int ProvisionofFarmerend_lu { get; set; }//結案戶數
        public double ProvisionOfFarmend_lu { get; set; }//已結案面積
        public decimal ProvisionOfMoneyend_lu { get; set; }//已結案經費
        public double PlanExe_lu { get; set; }//計畫執行率
        public decimal Desinge_lu { get; set; }//設計費

        public int ReservationsofFarmer_pis { get; set; }//預定戶數
        public int ReservationsofFarm_pis { get; set; }//預定面積
        public decimal ReservationsofMoney_pis { get; set; }//預定經費
        public decimal Ch_ReservationsofMoney_pis { get; set; }//俢改經費
        public int NumofFarmer_pis { get; set; }//建檔戶數
        public double NumofFarm_pis { get; set; }//建檔面積
        public double ReachOfArea_pis { get; set; } //已達成面積
        public int ProvisionOfFarmer_pis { get; set; }//已編列戶數
        public double ProvisionOfFarm_pis { get; set; }//已編列面積
        public decimal ProvisionOfMoney_pis { get; set; }//已編列經費
        public decimal Unusemoney_pis { get; set; }//未編列經費
        public int ProvisionofFarmerend_pis { get; set; }//結案戶數
        public double ProvisionOfFarmend_pis { get; set; }//已結案面積
        public decimal ProvisionOfMoneyend_pis { get; set; }//已結案經費
        public double PlanExe { get; set; }//計畫執行率
        public decimal Desinge_pis { get; set; }//設計費

        public int ReservationsofFarmer_gold { get; set; }//預定戶數
        public int ReservationsofFarm_gold { get; set; }//預定面積
        public decimal ReservationsofMoney_gold { get; set; }//預定經費
        public decimal Ch_ReservationsofMoney_gold { get; set; }//俢改經費
        public int NumofFarmer_gold { get; set; }//建檔戶數
        public double NumofFarm_gold { get; set; }//建檔面積
        public double ReachOfArea_gold { get; set; } //已達成面積
        public int ProvisionOfFarmer_gold { get; set; }//已編列戶數
        public double ProvisionOfFarm_gold { get; set; }//已編列面積
        public decimal ProvisionOfMoney_gold { get; set; }//已編列經費
        public decimal Unusemoney_gold { get; set; }//未編列經費
        public int ProvisionofFarmerend_gold { get; set; }//結案戶數
        public double ProvisionOfFarmend_gold { get; set; }//已結案面積
        public decimal ProvisionOfMoneyend_gold { get; set; }//已結案經費
        public double PlanExe_gold { get; set; }//計畫執行率
        public decimal Desinge_gold { get; set; }//設計費
        public int ReservationsofFarmer_cx { get; set; }//預定戶數
        public int ReservationsofFarm_cx { get; set; }//預定面積
        public decimal ReservationsofMoney_cx { get; set; }//預定經費
        public decimal Ch_ReservationsofMoney_cx { get; set; }//俢改經費
        public int NumofFarmer_cx { get; set; }//建檔戶數
        public double NumofFarm_cx { get; set; }//建檔面積
        public double ReachOfArea_cx { get; set; } //已達成面積
        public int ProvisionOfFarmer_cx { get; set; }//已編列戶數
        public double ProvisionOfFarm_cx { get; set; }//已編列面積
        public decimal ProvisionOfMoney_cx { get; set; }//已編列經費
        public decimal Unusemoney_cx { get; set; }//未編列經費
        public int ProvisionofFarmerend_cx { get; set; }//結案戶數
        public double ProvisionOfFarmend_cx { get; set; }//已結案面積
        public decimal ProvisionOfMoneyend_cx { get; set; }//已結案經費
        public double PlanExe_cx { get; set; }//計畫執行率
        public decimal Desinge_cx { get; set; }//設計費
        public string IaName { get; set; }
    }
    public class stasticAnnualBudget
    {
        /// <summary>
        /// 預定戶數
        /// </summary>
        public int ReservationsofFarmer { get; set; }
        /// <summary>
        /// 預定面積
        /// </summary>
        public int ReservationsofFarm { get; set; }
        /// <summary>
        /// 預定經費
        /// </summary>
        public decimal ReservationsofMoney { get; set; }
        /// <summary>
        /// 俢改經費
        /// </summary>
        public decimal ReservationsofMoneyNow { get; set; }
        /// <summary>
        /// 建檔戶數
        /// </summary>
        public int NumofFarmer { get; set; }
        /// <summary>
        /// 建檔面積
        /// </summary>
        public double NumofFarm { get; set; }
        /// <summary>
        /// 已達成面積
        /// </summary>
        public double ReachOfArea { get; set; }
        /// <summary>
        /// 已編列戶數
        /// </summary>
        public int ProvisionOfFarmer { get; set; }
        /// <summary>
        /// 已編列面積
        /// </summary>
        public double ProvisionOfFarm { get; set; }
        /// <summary>
        /// 已編列經費
        /// </summary>
        public decimal ProvisionOfMoney { get; set; }
        /// <summary>
        /// 未編列經費
        /// </summary>
        public decimal Unusemoney { get; set; }
        /// <summary>
        /// 結案戶數
        /// </summary>
        public int ProvisionofFarmerend { get; set; }
        /// <summary>
        /// 已結案面積
        /// </summary>
        public double ProvisionOfFarmend { get; set; }
        /// <summary>
        /// 已結案經費
        /// </summary>
        public int ProvisionOfMoneyend { get; set; }
        /// <summary>
        /// 計畫執行率
        /// </summary>
        public double PlanExe { get; set; }
        /// <summary>
        /// 水利會
        /// </summary>
        public string IaName { get; set; }
        public int DesignFee { get; set; }

    }
    public class stasticAnnualBudgetGold
    {
        /// <summary>
        /// 預定戶數
        /// </summary>
        public int ReservationsofFarmer { get; set; }
        /// <summary>
        /// 預定面積
        /// </summary>
        public int ReservationsofFarm { get; set; }
        /// <summary>
        /// 預定經費
        /// </summary>
        public decimal ReservationsofMoney { get; set; }
        /// <summary>
        /// 俢改經費
        /// </summary>
        public decimal ReservationsofMoneyNow { get; set; }
        /// <summary>
        /// 建檔戶數
        /// </summary>
        public int NumofFarmer { get; set; }
        /// <summary>
        /// 建檔面積
        /// </summary>
        public double NumofFarm { get; set; }
        /// <summary>
        /// 已達成面積
        /// </summary>
        public double ReachOfArea { get; set; }
        /// <summary>
        /// 已編列戶數
        /// </summary>
        public int ProvisionOfFarmer { get; set; }
        /// <summary>
        /// 已編列面積
        /// </summary>
        public double ProvisionOfFarm { get; set; }
        /// <summary>
        /// 已編列經費
        /// </summary>
        public feeStruct ProvisionOfMoney { get; set; }
        /// <summary>
        /// 未編列經費
        /// </summary>
        public int Unusemoney { get; set; }
        /// <summary>
        /// 結案戶數
        /// </summary>
        public int ProvisionofFarmerend { get; set; }
        /// <summary>
        /// 已結案面積
        /// </summary>
        public double ProvisionOfFarmend { get; set; }
        /// <summary>
        /// 已結案經費
        /// </summary>
        public feeStruct ProvisionOfMoneyend { get; set; }
        /// <summary>
        /// 計畫執行率
        /// </summary>
        public double PlanExe { get; set; }
        /// <summary>
        /// 水利會
        /// </summary>
        public string IaName { get; set; }
        public int DesignFee { get; set; }

    }
    public class feeStruct
    {
        private int _govpay;
        private int _goldpay;
        private int _lupay;

        public feeStruct(int igovpay, int igoldpay, int ilupay)
        {
            _govpay = igovpay;
            _goldpay = igoldpay;
            _lupay = ilupay;
        }

        public int govpay { get { return _govpay; } }
        public int goldpay { get { return _goldpay; } }
        public int lupay { get { return _lupay; } }
    }
    public class StatisticCoaView
    {
        /// <summary>
        /// 執行單位
        /// </summary>
        public int Ia { get; set; }
        /// <summary>
        /// 執行單位名稱
        /// </summary>
        public string IaName { get; set; }
        /// <summary>
        /// 建檔戶數
        /// </summary>
        public int Farmer { get; set; }
        /// <summary>
        /// 建檔面積
        /// </summary>
        public decimal FarmArea { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int BudgetFarmer { get; set; }        
        /// <summary>
        /// 已編列預算面積
        /// </summary>
        public decimal BudgetFarmArea { get; set; }
        /// <summary>
        /// 編列預算
        /// </summary>
        public int BudgetMoney { get; set; }
        /// <summary>
        /// 已驗收案件
        /// </summary>
        public int CompleteFarmer { get; set; }
        /// <summary>
        /// 已驗收面積
        /// </summary>
        public decimal CompleteFarmArea { get; set; }
        /// <summary>
        /// 已驗收金額
        /// </summary>
        public int  CompleteBugdetMoney { get; set; }
        /// <summary>
        /// 預定預算
        /// </summary>
        public int RsvBudget { get; set; }
        /// <summary>
        /// 預定面積
        /// </summary>
        public decimal RsvFarmArea { get; set; }
    }
}
