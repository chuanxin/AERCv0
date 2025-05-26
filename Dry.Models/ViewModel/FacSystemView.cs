using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class FacSystemView
    {
        /// <summary>get and set System No</summary>
        [Required(ErrorMessage = " 此欄位為必填欄位")]
        [RegularExpression(@"^[A-Za-z-0-9]{3,10}$", ErrorMessage = "系統代號僅能輸入3 ~ 10個英文字")]
        public string StdSysNo { get; set; }

        /// <summary>get and set System Name</summary>
        [Required(ErrorMessage = " 此欄位為必填欄位")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "系統名字僅能輸入3 ~ 10個文字")]        
        public string StdSysName { get; set; }

        /// <summary>get and set System Description</summary>   
        [StringLength(200, ErrorMessage = "系統描述不得超過200個文字")]        
        public string StdSysDesp { get; set; }

        /// <summary>get and set System End Type DDL List</summary>
        [Range(0, 99, ErrorMessage = "請選擇末端型式")]
        public List<SelectListItem> EndTypeDDL { get; set; }

        /// <summary>get and set System Fac Type DDL List</summary>
        [Range(0, 99, ErrorMessage = "請選擇噴頭種類")]
        public List<SelectListItem> FacTypeDDL { get; set; }

        /// <summary>get and set System Mat List</summary>
        public List<FacSysMAT> SysMat { get; set; }

        
    }
}
