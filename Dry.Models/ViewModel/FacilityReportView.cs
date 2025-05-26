using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.ViewModel
{
    public class FacilityReportView
    {
        /// <summary>get and set apply year</summary>
        public byte ApplyY { get; set; }
        /// <summary>get and set apply Unit</summary>
        public Int16 ApplyUnit { get; set; }
        /// <summary>get and set is Gold case</summary>
        public bool Gold { get; set; }
        /// <summary>get and set apply case</summary>
        public int Case { get; set; }
        /// <summary>get and set IANum</summary>
        public int IANum { get; set; }
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
        ///// <summary>get and set radio list</summary>        
        public IEnumerable<System.Web.Mvc.SelectListItem> ResultList { get; set; } 


        /// <summary>get and set finished date</summary>
        [Required(ErrorMessage = " 此欄位為必填欄位")]
        public DateTime EDate { get; set; }
        ///// <summary>get and set finished date</summary>  
        [Required(ErrorMessage = " 此欄位為必填欄位")]
        public string Result { get; set; }

        /// <summary>
        /// 是否已結案
        /// </summary>
        public bool Complete { get; set; }
        /// <summary>
        /// 是否為修改
        /// </summary>
        public bool IsModify { get; set; }
    }
}
