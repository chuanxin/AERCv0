using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.ViewModel
{
    public class ExamineView
    {
        public List<FileTypeList> ListFileTypeList { get; set; }
        public List<EFile> EFileList { get; set; }
        public int MapNo { get; set; }
        /// <summary>
        /// 是否為修改
        /// </summary>
        public bool IsModify { get; set; }
        /// <summary>
        /// 目前步驟
        /// </summary>
        public byte Step { get; set; }
        //[Required(ErrorMessage = " 此欄位為必填欄位")]
        //[RegularExpression(@"^([+-]?)[0-9]\d*\.\d*$",ErrorMessage="格式輸入錯誤")]
        //public string EnginePntX { get; set; }
        //[Required(ErrorMessage = " 此欄位為必填欄位")]
        //[RegularExpression(@"^([+-]?)[0-9]\d*\.\d*$", ErrorMessage = "格式輸入錯誤")]
        //public string EnginePntY { get; set; }
        //[Required(ErrorMessage = " 此欄位為必填欄位")]
        //[RegularExpression(@"^([+-]?)[0-9]\d*\.\d*$", ErrorMessage = "格式輸入錯誤")]
        //public string WaterPntX { get; set; }
        //[Required(ErrorMessage = " 此欄位為必填欄位")]
        //[RegularExpression(@"^([+-]?)[0-9]\d*\.\d*$", ErrorMessage = "格式輸入錯誤")]
        //public string WaterPntY { get; set; }
        [Required(ErrorMessage = " 必填")]
        public byte Result { get; set; }
        //[Required(ErrorMessage = " 此欄位為必填欄位")]
        public string Reason { get; set; }
        public Guid Examiner { get; set; }
        [Required(ErrorMessage = " 必填")]
        //[RegularExpression(@"^[0-9]{4}/(((0[13578]|(10|12))/(0[1-9]|[1-2][0-9]|3[0-1]))|(02/(0[1-9]|[1-2][0-9]))|((0[469]|11)/(0[1-9]|[1-2][0-9]|30)))$", ErrorMessage = "格式輸入錯誤")]
        public DateTime EDate { get; set; }
        public string Note { get; set; }

        public int previousMNo { get; set; }
        //[RegularExpression(@"\b((?!-1)\w)+\b", ErrorMessage = "請選擇勘查人員")]
        public Guid AdminID { get; set; }
        /// <summary>
        /// 電子檔伺服器主機IP位置
        /// </summary>
        public string EFilePath { get; set; }
        /// <summary>依會別載入相對應的WMS</summary>
        public string iaWMS { get; set; }
    }
    public class ImgDataView
    {
        public string FilePath { get; set; }
        public byte FileType { get; set; }
        public string Coordinate { get; set; }
    }
    public class ExamineJsonData
    {
        public List<ImgDataView> ImgData { get; set; }
        public Guid Examiner { get; set; }
        public byte Result { get; set; }
        public string Reason { get; set; }
        public string EDate { get; set; }
        public string Note { get; set; }
    }
}
