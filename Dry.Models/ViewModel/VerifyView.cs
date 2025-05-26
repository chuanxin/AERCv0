using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.ViewModel
{
    public class VerifyView
    {
        #region 基本資料
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
        #endregion

        /// <summary>get and set Verify person List</summary>
        public List<System.Web.Mvc.SelectListItem> personDDL { get; set; }

        /// <summary>get and set Verify status List</summary>
        public List<System.Web.Mvc.SelectListItem> statusDDL { get; set; }


        //public int RecNo { get; set; }
        /// <summary>
        /// The Verify Status
        /// </summary>
        public byte statCode { get; set; }

        /// <summary>
        /// The Verify Deciript
        /// </summary>
        public string Descript { get; set; }

        /// <summary>
        /// The Reduce Money
        /// </summary>
        [Range(0, 1000000, ErrorMessage = "輸入範圍需大於0之整數")]
        public string ReduceM { get; set; }

        /// <summary>
        /// The Pay Money
        /// </summary>
        public string PayM { get; set; }
        /// <summary>
        /// The money of orginal pay
        /// </summary>
        public string OrginalPayM { get; set; }

        /// <summary>
        /// The Verify Date
        /// </summary>
        [Required(ErrorMessage = " 此欄位為必填欄位")]
        public DateTime VDate { get; set; }

        /// <summary>
        /// The verify person
        /// </summary>
        public Guid VID { get; set; }
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
