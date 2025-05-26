using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class ReportView
    {
        /// <summary>
        /// 年度DropDownList
        /// </summary>
        public List<SelectListItem> ApplyYearDDL { get; set; }
        /// <summary>
        /// 年度
        /// </summary>
        public int ApplyYear { get; set; }
        /// <summary>
        /// 案件編號(開始)
        /// </summary>
        public int IANumStart { get; set; }
        /// <summary>
        /// 案件編號(結束)
        /// </summary>
        public int IANumEnd { get; set; }

        public string FilePath { get; set; }

        public string CheeseVal { get; set; }

        public int number { get; set; }

        public List<SelectListItem> CheeseList { get; set; }

        public class ApplyArea
        {
            List<CountyCount> ApplyAreaSum { get; set; }
        }        
        public int step { get; set; }
        public int govtype { get; set; }
        public bool gold { get; set; }
        public string moon { get; set; }
        public string day { get; set; }
        public System.Guid DesginerId { get; set; }
        public List<SelectListItem> DesginerDDL { get; set; }
        public List<SelectListItem> UnitsDDL { get; set; }
        public int ApplyUnit { get; set; }
        public int ApplyYearEnd { get; set; }
        public int ApplyYearStart { get; set; }
    }
    public class CountyCount
    {
        /// <summary>
        /// 年度
        /// </summary>
        public int ApplyYear { get; set; }
        public List<ApplyCounty> CountyData { get; set; }
    }
    public class ApplyCounty
    {
        /// <summary>
        /// 縣市
        /// </summary>
        public string County { get; set; }
        public ApplyCountData CountData { get; set; }
    }
    public class ApplyCountData
    {
        /// <summary>
        /// 申請戶數
        /// </summary>
        public int ApplyCount { get; set; }
        /// <summary>
        /// 面積(Ha)
        /// </summary>
        public double AreaHa { get; set; }
    }
    public class CasePayDetailView
    {
        public int mapno { get; set; }
        public int? 田間管路設施費 { get; set; }
        public int? 規劃設計費 { get; set; }
        public int? 水源設施費 { get; set; }
        public int? 調控設施費 { get; set; }
        public int? 動力設備費 { get; set; }
        public int? 蓄水設備費 { get; set; }
        public int? 設施費總計 { get; set; }
        public int? 工作費 { get; set; }
        public int? FarmerFee { get; set; }
        public int? Total { get; set; }
    }
}
