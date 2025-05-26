using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class FarmLandView
    {
        #region ModelData
        /// <summary>
        /// 是否為重變更設計
        /// </summary>
        public bool IsCgh { get; set; }
        /// <summary>
        /// 是否為可修改
        /// </summary>
        public bool IsModify { get; set; }
        /// <summary>
        /// 目前步驟
        /// </summary>
        public byte Step { get; set; }
        ///<summary>縣市DropDownList</summary>
        public List<SelectListItem> CityDDL { get; set; }
        ///<summary>鄉鎮市DropDownList</summary>
        public List<SelectListItem> TownDDL { get; set; }
        ///<summary>地段DropDownList</summary>
        public List<SelectListItem> SectionList { get; set; }
        public List<SelectListItem> LandTypeList { get; set; }
        /// <summary>依會別載入相對應的WMS</summary>
        public string iaWMS { get; set; }
        //public List<SelectListItem> CropList { get; set; }
        public List<Dry.Models.Service.FarmLandDBService.JsData> FarmData { get; set; }
        /// <summary>
        /// 承辦單位
        /// </summary>
        public short ApplyUnit { get; set; }
        #endregion
        
        /// <summary>縣市代碼</summary>
        [RegularExpression(@"\b((?!-1)\w)+\b", ErrorMessage = "請選擇縣市")]
        public string FarmCityCode { get; set; }

        /// <summary>鄉鎮市代碼</summary>
        [RegularExpression(@"\b((?!-1)\w)+\b", ErrorMessage = "請選擇鄉鎮")]
        public short FarmTownId { get; set; }

        #region 農地資訊
        /// <summary>get and set FarmerSystem SS</summary>
        [Required(ErrorMessage = " 必填")]
        [RegularExpression(@"\b((?!-1)\w)+\b", ErrorMessage = "請選擇地段")]
        public int FarmSection { get; set; }

        /// <summary>get and set FarmerSystem SL</summary>
        [Required(ErrorMessage = " 必填")]
        [Range(0, byte.MaxValue, ErrorMessage = "超出範圍")]
        public byte LandType { get; set; }

        [Required(ErrorMessage = " 必填")]
        //[RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "地號範例:0000-0000")]
        public string LandNo { get; set; }

        [Required(ErrorMessage = " 必填")]
        [Range(1, int.MaxValue, ErrorMessage = "超出範圍")]
        public string LandArea { get; set; }

        [Required(ErrorMessage = " 必填")]
        [Range(1, int.MaxValue, ErrorMessage = "超出範圍")]
        public string BuildArea { get; set; }

        [StringLength(15, MinimumLength = 2, ErrorMessage = "緯度僅能輸入3 ~ 15個文字")]
        public string Lat { get; set; }

        [StringLength(15, MinimumLength = 2, ErrorMessage = "經度僅能輸入3 ~ 15個文字")]
        public string Long { get; set; }

        [Required(ErrorMessage = " 必填")]
        public bool Outside { get; set; }

        [DataType(DataType.MultilineText)]
        public string ApplicationStatus { get; set; }

        /// <summary>
        /// 是否重覆申請
        /// </summary>
        public bool IsApplied { get; set; }

        [Required(ErrorMessage = " 必填")]
        [Range(1, int.MaxValue, ErrorMessage = "分母比例超出範圍")]
        public string Pcent_Par { get; set; }

        [Required(ErrorMessage = " 必填")]
        [Range(1, int.MaxValue, ErrorMessage = "分子比例超出範圍")]
        public string Pcent_Chd { get; set; }

        #endregion

        #region 農地作物資訊
        [Required(ErrorMessage = " 必填")]
        [Range(0, byte.MaxValue, ErrorMessage = "選擇作物超出範圍")]
        public byte Crop { get; set; }
        #endregion

        #region 農地持分人資訊

        [Required(ErrorMessage = " 必填")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "姓名僅能輸入2 ~ 20個文字")]
        public string HolderName { get; set; }

        [Required(ErrorMessage = " 必填")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "地址需超過2個字以上")]
        public string Addr { get; set; }

        [Required(ErrorMessage = " 必填")]
        [Range(1, short.MaxValue, ErrorMessage = "分母比例超出範圍")]
        public string Par { get; set; }

        [Required(ErrorMessage = " 必填")]
        [Range(1, short.MaxValue, ErrorMessage = "分子比例超出範圍")]
        public string child { get; set; }

        [Required(ErrorMessage = " 必填")]
        [Range(1, int.MaxValue, ErrorMessage = "持有面積超出範圍")]
        public string hasArea { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "身份證字號僅能輸入10個字")]
        public string IDNum { get; set; }

        #endregion
                
    }
    
    /// <summary>
    /// 瑠公-會員申請書報表
    /// </summary>
    public class LiuGongMemberStruct
    {
        /// <summary>
        /// 施設地區
        /// </summary>
        public string Build_Area { get; set; }
        /// <summary>
        /// 核定面積合計
        /// </summary>
        public float Approved_Area { get; set; }
        /// <summary>
        /// 案件編號
        /// </summary>
        public int MapNo { get; set; }

        public List<SectionDataStruct> SectionData { get; set; }
        
    }
    public class SectionDataStruct
    {
        /// <summary>
        /// 地段
        /// </summary>
        public string Section { get; set; }
        /// <summary>
        /// 小段
        /// </summary>
        public string SubSection { get; set; }
        /// <summary>
        /// 地目
        /// </summary>
        public string LandType { get; set; }
        /// <summary>
        /// 地號
        /// </summary>
        public string LandNO { get; set; }
        /// <summary>
        /// 該筆面積
        /// </summary>
        public string LandArea { get; set; }
    }

    public class FarmLandReport
    {
        public string UnitName { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Section { get; set; }
        public string LandNo { get; set; }
        public decimal BuildArea { get; set; }
        public string CatalogCNS { get; set; }
        public string Outside { get; set; }
        public int IaNum { get; set; }
        public int ApplyYear { get; set; }
    }
}
