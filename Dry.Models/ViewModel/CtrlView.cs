/*
-- =============================================
-- Author: WEI
-- Create date: 2014-2-10
-- Description: 調控設施的View型別定義
-- =============================================
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models;

namespace Dry.Models.ViewModel
{
    public class CtrlView
    {
        //顯示資料陣列
        public List<Farmer> DataList { get; set; }
        public List<CntrlFac> FacDataList { get; set; }
        public List<CntrlList> ListDataList { get; set; }
        public List<CtrlTableView> MatData { get; set; }
        public List<Unit> UnitDataList { get; set; }
        public List<CntrlMat> MatDataList { get; set; }
        /// <summary>
        /// 是否為修改
        /// </summary>
        public bool IsModify { get; set; }
        /// <summary>
        /// 目前步驟
        /// </summary>
        public byte Step { get; set; }
        #region CntrlMat資料表的欄位定義
        public int MatNo { get; set; }
        public int CFNo { get; set; }
        [Range(0, 99, ErrorMessage = "請選擇類型")]
        public int CntrlCode { get; set; }
        [Required(ErrorMessage = " 此欄位為必填欄位")]
        [StringLength(30,ErrorMessage="名稱過長")]
        public string MatName { get; set; }
        [Required(ErrorMessage = " 此欄位為必填欄位")]
        [RegularExpression("[0-9]*", ErrorMessage = "請輸入數字")]
        [Range(0, 32767, ErrorMessage = "請輸入有效數量")]
        public short MatAmt { get; set; }
        [Required(ErrorMessage = " 此欄位為必填欄位")]
        [RegularExpression("[0-9]*", ErrorMessage = "請輸入數字")]
        [Range(0, 2147483647, ErrorMessage = "請輸入有效金額")]
        public float MatPrice { get; set; }
        #endregion

        #region CntrlApply資料表的欄位定義
        [Required(ErrorMessage = " 此欄位為必填欄位")]
        [RegularExpression("[0-9]*", ErrorMessage = "請輸入數字")]
        [Range(0, 2147483647, ErrorMessage = "請輸入有效金額")]
        public int FacMoney { get; set; }
        [Range(0, 99, ErrorMessage = "請選擇補助單位")]
        public short ApplyUnit { get; set; }
        #endregion

        #region Pay資料表的欄位定義
        public int PayMoney { get; set; }
        #endregion

        public int CtrlTotalPrice { get; set; } //估計所需經費
        public int CtrlHelpPrice { get; set; } //可輔助費用
        public int CtrlSelfPrice { get; set; } //自備款
        public int CtrlGoldPrice { get; set; } //黃金廊道補助金額
        public string UnitName { get; set; } //單位名稱
        public string GoldMessage { get; set; } //黃金廊道訊息
        public int CaseMoney { get; set; }
    }
}
