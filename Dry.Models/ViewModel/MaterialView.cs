using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class MaterialView
    {
        /// <summary>
        /// 系統物料代碼
        /// </summary>
        public int POMNo { get; set; }
        /// <summary>
        /// 模組代號
        /// </summary>
        public int ModuleNo { get; set; }
        /// <summary>
        /// 模組名稱
        /// </summary>
        public string ModuleCNS { get; set; }
        /// <summary>
        /// 物料名稱
        /// </summary>
        public string MName { get; set; }
        /// <summary>
        /// 規格
        /// </summary>
        public string Spec { get; set; }
        /// <summary>
        /// 規格1(三通管或四通管會用到)
        /// </summary>
        public int Spec1 { get; set; }
        public string Spec1CNS { get; set; }
        /// <summary>
        /// 規格2
        /// </summary>
        public int Spec2 { get; set; }
        public string Spec2CNS { get; set; }
        /// <summary>
        /// 規格3
        /// </summary>
        public int Spec3 { get; set; }
        /// <summary>
        /// 材質
        /// </summary>
        public int MatType { get; set; }
        public string MatTypeCNS { get; set; }
        /// <summary>
        /// 長度
        /// </summary>
        public double SpecLength { get; set; }
        public string Spec3CNS { get; set; }
        /// <summary>
        /// 品項單位
        /// </summary>
        public string ItemUnit { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 單位代碼
        /// </summary>
        public short Bunit { get; set; }
    }
    public class MaterialPriceView
    {
        public List<SelectListItem> YearDDL { get; set; }
        public int No { get; set; }
        public int POMNo { get; set; }
        public short BYear { get; set; }
        public double Price { get; set; }
    }
}
