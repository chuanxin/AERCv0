using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class FarmerView
    {
        #region 屬性
        /// <summary>案件代碼</summary>
        public int EvntNo { get; set; }

        /// <summary>農戶代碼</summary>
        public Guid FId { get; set; }

        /// <summary>身分證字號</summary>
        [Required(ErrorMessage = " 必填")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "請輸入有效身份證號")]
        //[RegularExpression(@"/^[a-zA-Z]\[1-2]\[0-9]{8}$/", ErrorMessage = "請輸入有效身份證號")]
        public string FarmerIdNo { get; set; }

        /// <summary>姓名</summary>
        [Required(ErrorMessage = " 必填")]
        public string FarmerName { get; set; }

        /// <summary>縣市代碼</summary>
        [RegularExpression(@"\b((?!-1)\w)+\b", ErrorMessage = "請選擇縣市")]
        public string FarmerCityCode { get; set; }

        /// <summary>鄉鎮市代碼</summary>
        [RegularExpression(@"\b((?!-1)\w)+\b", ErrorMessage = "請選擇鄉鎮")]
        public short FarmerTownId { get; set; }

        /// <summary>地址</summary>
        [Required(ErrorMessage = " 必填")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "字元數需介於需3 ~100間")]
        public string FarmerAddr { get; set; }

        /// <summary>手機</summary>
        //[Required(ErrorMessage = " 此欄位為必填欄位")]
        [RegularExpression("[0-9]*", ErrorMessage = "請輸入數字")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "請輸入有效號碼")]
        public string FarmerPhone { get; set; }

        /// <summary>家中聯絡電話</summary>
        //[Required(ErrorMessage = " 此欄位為必填欄位")]
        [RegularExpression("[0-9]*", ErrorMessage = "請輸入數字")]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "請輸入有效號碼")]
        public string FarmerTel { get; set; }

        /// <summary>瑠公會員</summary>
        public bool IsMember { get; set; }

        /// <summary>建立者代號</summary>
        public Guid CId { get; set; }

        /// <summary>設計者代號</summary>
        public Guid DesingerId { get; set; }

        /// <summary>建立日期</summary>
        public DateTime CDate { get; set; }

        /// <summary>更新日期</summary>
        public DateTime UDate { get; set; }


        //Case資料表
        /// <summary>是否為黃金廊道方案</summary>
        public bool Gold { get; set; }

        //Case資料表
        /// <summary>是否為山坡地離島</summary>
        public bool Is12 { get; set; }


        /// <summary>承辦單位</summary>
        public short ApplyUnit { get; set; }

        /// <summary>承辦年度</summary>
        [Required(ErrorMessage = "必填")]
        [Range(60, Byte.MaxValue, ErrorMessage = "年度超出範圍")]
        public byte ApplyYear { get; set; }

        /// <summary>水利會案號</summary>
        [Required(ErrorMessage = "必填")]
        [Range(0, int.MaxValue, ErrorMessage = "案號超出範圍")]
        public int IANum { get; set; }

        public List<CaseData> CaseList { get; set; }

        public CaseData caseDta { get; set; }
        /// <summary>
        /// 是否為修改
        /// </summary>
        public bool IsModify { get; set; }
        /// <summary>
        /// 是否為變更設計
        /// </summary>
        public bool IsChg { get; set; }

        /// <summary>
        /// 目前步驟
        /// </summary>
        public byte Step { get; set; }
        #endregion        

        #region DropDownList
        /// <summary>縣市DropDownList</summary>
        public List<SelectListItem> CityDDL { get; set; }
        /// <summary>鄉鎮市DropDownList</summary>
        public List<SelectListItem> TownDDL { get; set; }
        /// <summary>設計者DropDownList</summary>
        public List<SelectListItem> DesginerDDL { get; set; }

        #endregion

        public class CaseData
        {
            public int EventNo { get; set; }
            public Guid FId { get; set; }
            /// <summary>
            /// 申請年度
            /// </summary>
            public byte ApplyYear { get; set; }
            /// <summary>
            /// 承辦單位
            /// </summary>
            public string ApplyUnit { get; set; }
            /// <summary>
            /// 黃金廊道
            /// </summary>
            public string Gold { get; set; }
            /// <summary>
            /// 建立日期
            /// </summary>
            public string CDate { get; set; }
            /// <summary>
            /// 更新日期
            /// </summary>
            public string UDate { get; set; }
            /// <summary>
            /// 申請人姓名
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 水利會案號
            /// </summary>
            public int IANum { get; set; }
            /// <summary>
            /// 目前步驟
            /// </summary>
            public byte Step { get; set; }
            /// <summary>
            /// 是否已結案
            /// </summary>
            public bool Complete { get; set; }
            /// <summary>
            /// 案件狀態
            /// </summary>
            public string Status { get; set; }
            /// <summary>
            /// 離島山坡地
            /// </summary>
            public string Is12 { get; set; }
            /// <summary>
            /// 末端形式
            /// </summary>
            public string CatalogCNS { get; set; }
            /// <summary>
            /// 施作面積
            /// </summary>
            public double? buildarea { get; set; }
        }        
    }
}
