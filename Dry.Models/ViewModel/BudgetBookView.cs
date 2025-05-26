using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
-- =============================================
-- Author: Hao Hsuan
-- Create date: 2014-2-18
-- Description: Define Budget Book View
-- =============================================
*/
namespace Dry.Models.ViewModel
{
    public class BudgetBookView
    {
        /// <summary>get and set apply year</summary>
        public byte ApplyY { get; set; }
        /// <summary>get and set apply Unit</summary>
        public Int16 ApplyUnit { get; set; }
        /// <summary>get and set is Gold case</summary>
        public bool Gold { get; set; }
        /// <summary>get and set apply case</summary>
        public int Case { get; set; }
        /// <summary>get and set IANum </summary>
        public int IANum { get; set; }
        /// <summary>
        /// 目前步驟
        /// </summary>
        public byte Step { get; set; }

        public string Block { get; set; }
        public double SS { get; set; }
        public double SL { get; set; }
        /// <summary>get and set farmer name</summary>
        public string Name { get; set; }
        /// <summary>get and set farmer address</summary>
        public string Address { get; set; }
        /// <summary>get and set facility section</summary>
        public List<Dry.Models.CommonCls.GetData.NewFarm> Farm { get; set; }
        /// <summary>get and set build area</summary>
        public double BuildArea { get; set; }
        /// <summary>get and set endtype,like 穿孔管系統, 噴灌系統, 微噴系統</summary>
        public string EndType { get; set; }
        /// <summary>get and set piping total money</summary>
        public string PipingTotal { get; set; }
        /// <summary>get and set piping materials money</summary>
        public string PipingMatMoney { get; set; }
       
        /// <summary>get and set L1 money</summary>
        public BudgetBookstruct L1 { get; set; }
        /// <summary>get and set L2 money</summary>
        public BudgetBookstruct L2 { get; set; }
        /// <summary>get and set irrigation System money</summary>
        
        public BudgetBookstruct IrrSystem { get; set; }
        /// <summary>get and set work money</summary>
        public BudgetBookstruct Work { get; set; }

        /// <summary>get and set planning money</summary>
        public BudgetBookstruct Planning { get; set; }

        /// <summary>get and set 調控設施 money</summary>
        public BudgetBookstruct RegulatedFac { get; set; }
        /// <summary>get and set engine money</summary>
        public BudgetBookstruct Engine { get; set; }
        /// <summary>get and set pool money</summary>
        public BudgetBookstruct Pool { get; set; }
        /// <summary>get and set pool total Weight</summary>
        public int PoolWei { get; set; }

        /// <summary>get and set total money</summary>
        public string Total { get; set; }
        /// <summary>get and set farmer pay money</summary>
        public string FarmerPay { get; set; }
        /// <summary>get and set government pay(農戶請領款)</summary>
        public string GovPay_Pay { get; set; }
        /// <summary>
        /// 農戶各項請領款細目
        /// </summary>
        public List<string> GovPay_Detail { get; set; }
        /// <summary>get and set government pay for planning(規劃設計費)</summary>
        public string GovPay_Planning { get; set; }
        /// <summary>get and set sum of government pay(小計)</summary>
        public string GovPay_Sum { get; set; }
        /// <summary>get and set Total Number to Chinese</summary>
        public string ChineseMoney { get; set; }
        public string L1_length { get; set; }
        public string L2_length { get; set; }
        ///<summary>get and set government pay for gold(七星補助金額)</summary>
        public string GoldPay { get; set; }
        ///<summary>get and set government pay for gold(瑠公補助金額)</summary>
               
        public string LiuPay { get; set; }
        ///<summary>get and set 是否重複申請</summary>
        public bool IsApplied { get; set; }
        #region 瑠公專用
        /// <summary>get and set piping mat money</summary>
        public BudgetBookstruct Liu_PipingMat { get; set; }
        /// <summary>get and set engine money</summary>
        public BudgetBookstruct Liu_Engine { get; set; }
        /// <summary>get and set pool money</summary>
        public BudgetBookstruct Liu_Pool { get; set; }
        /// <summary>get and set work money</summary>
        public BudgetBookstruct Liu_WorkM { get; set; }
        /// <summary>get and set 包商管理費用</summary>
        public BudgetBookstruct Liu_MngM { get; set; }
        /// <summary>get and set Total費用</summary>
        public string Liu_TotalMoney { get; set; }
        /// <summary>get and set 農戶配合款</summary>
        public string Liu_FarmerMoney { get; set; }
        /// <summary>get and set 水利會補助款</summary>
        public string Liu_IAMoney { get; set; }
        /// <summary>get and set 小計</summary>
        public string Liu_sum { get; set; }
        /// <summary>get and set Total Number to Chinese</summary>
        public string Liu_ChineseMoney { get; set; }
        #endregion

    }

    #region Budget Book Struct
    public class BudgetBookstruct
    {
        private string _ItemAmount;
        private string _ItemPrice;
        private string _TotalPrice;
        private string _Memo;
        /// <summary>New Budget Bookstruct</summary>
        /// <param name="amount">數量</param>
        /// <param name="itemprice">單價</param>
        /// <param name="tprice">總價</param>
        /// <param name="memo">附註</param>
        public BudgetBookstruct(string amount, string itemprice, string tprice, string memo = "")
        {
            _ItemAmount = amount;
            _ItemPrice = itemprice;
            _TotalPrice = tprice;
            _Memo = memo;
        }

        public string ItemAmount { get { return _ItemAmount; }}
        public string ItemPrice { get { return _ItemPrice; } }
        public string TotalPrice { get { return _TotalPrice; } }
        public string Memo { get { return _Memo; } }
    }
    public class FarmerData : Farmer
    {
        /// <summary>
        /// 申請年度
        /// </summary>
        public byte ApplyYear { get; set; }
        /// <summary>
        /// 水利會名稱
        /// </summary>
        public string IAName { get; set; }
        /// <summary>
        /// 案號
        /// </summary>
        public int IANum { get; set; }
    }
    #endregion
}
