using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

/*
-- =============================================
-- Author: Hao Hsuan
-- Create date: 2014-2-13
-- Description: 蓄水池的View型別定義
-- =============================================
*/
namespace Dry.Models.ViewModel
{
    public class PoolView
    {
        /// <summary>
        /// 是否為修改
        /// </summary>
        public bool IsModify { get; set; }
        /// <summary>
        /// 目前步驟
        /// </summary>
        public byte Step { get; set; }
        /// <summary>get and set PoolType List</summary>
        public List<SelectListItem> PoolTypeDDL { get; set; }
        /// <summary>get and set Unit List</summary>
        public List<SelectListItem> UnitDDL { get; set; }
        /// <summary>get and set Unit List</summary>
        public List<SelectListItem> WeightDDL { get; set; }
        /// <summary>
        /// 蓄水池價格
        /// </summary>
        public int PoolPrice { get; set; }

        /// <summary>get and set Pool Data List</summary>
        public List<ModifyPoolData> PoolData { get; set; }
        /// <summary>get and set Now Step Can Use Money</summary>
        public int CaseMoney { get; set; }
        /// <summary>
        /// 政府補助款
        /// </summary>
        public int GovM { get; set; }
        /// <summary>
        /// 農戶自備款
        /// </summary>
        public int FarmM { get; set; }
        /// <summary>
        /// 七星補助款
        /// </summary>
        public int GoldM { get; set; }
        
        public int TotalM { get; set; }
        public int Area { get; set; }

        #region Validation
        [Required(ErrorMessage = " 此欄位為必填欄位")]
        [Range(10, 100, ErrorMessage = "容量範圍為10 ~ 100")]
        public string txt_PoolW { get; set; }

        [Required(ErrorMessage = " 此欄位為必填欄位")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "請輸入:周長x高")]
        [RegularExpression(@"^\d{1,4}x\d{1,4}$", ErrorMessage = "請輸入:周長x高")]
        public string txt_PoolSize { get; set; }

        [Range(0, 99, ErrorMessage = "請選擇補助單位")]
        public short ddl_PoolUnit { get; set; }

        [Required(ErrorMessage = " 此欄位為必填欄位")]
        [Range(1, 1000000, ErrorMessage = "輸入範圍為1 ~ 1,000,000")]
        public string FacPrice { get; set; }
        #endregion

        #region Modify Pool Data Struct
        public class ModifyPoolData
        {
            private short _poolType;
            private string _poolTypeName;
            private short _poolW;
            private string _poolSize;
            private int _money;

            /// <summary>ModifyPool Data Struct</summary>
            /// <param name="poolType">Pool Type</param>
            /// <param name="poolTypeName">Pool Type Name</param>
            /// <param name="poolW">Pool Weight</param>
            /// <param name="poolSize">Pool Size </param>
            /// <param name="money">Pool Apply Money</param>
            public ModifyPoolData(short poolType, string poolTypeName, short poolW, string poolSize, int money)
            {
                _poolType = poolType;
                _poolTypeName = poolTypeName;
                _poolW = poolW;
                _poolSize = poolSize;
                _money = money;
            }

            public short PoolType { get { return this._poolType; } }
            public string PoolTypeName { get { return this._poolTypeName; } }
            public short PoolWeight { get { return this._poolW; } }
            public string PoolSize { get { return this._poolSize; } }
            public int Money { get { return this._money; } }
        }
        #endregion
    }
}
