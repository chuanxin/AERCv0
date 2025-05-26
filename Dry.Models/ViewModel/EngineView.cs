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
-- Description: 動力設施的View型別定義
-- =============================================
*/
namespace Dry.Models.ViewModel
{
    public class EngineView
    {
        /// <summary>get and set EngineType List</summary>
        public List<SelectListItem> EngineDDL { get; set; }
        /// <summary>get and set Unit List</summary>
        public List<SelectListItem> UnitDDL { get; set; }
        /// <summary>get and set Engine Data List</summary>
        public List<ModifyEngineData> EngData { get; set; }

        /// <summary>get and set Now Step Can Use Money</summary>
        public int CaseMoney { get; set; }

        public int TotalM { get; set; }
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
        public int Area { get; set; }
        /// <summary>
        /// 動力設備價格
        /// </summary>
        public int EngPrice { get; set; }

        #region Validation
        [Required(ErrorMessage = " 必填")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = " 輸入3 ~ 10個長度之字串")]
        public string txt_EngRegcd { get; set; }

        [Required(ErrorMessage = " 必填")]
        [Range(1, 1000000, ErrorMessage = " 輸入範圍為1 ~ 1,000,000")]
        public string FacPrice { get; set; }

        [Range(0, 99, ErrorMessage = "請選擇")]
        public int ddl_EngineUnit { get; set; }
        /// <summary>
        /// 是否為可修改
        /// </summary>
        public bool IsModify { get; set; }
        /// <summary>
        /// 目前步驟
        /// </summary>
        public byte Step { get; set; }
        #endregion

        #region Modify Engine Data Struct
        public class ModifyEngineData
        {
            private short _EngineCode;
            private string _EngineName;
            private string _EngineRegCode;
            private string _EngineMoney;
            private int _ENo;

            /// <summary>ModifyEngineData Struct</summary>
            /// <param name="enginecode">EngineType</param>
            /// <param name="enginename">EngineType Name</param>
            /// <param name="engineregcode">Engine Regcode</param>
            /// <param name="enginemoney">Engine Apply Money</param>
            /// <param name="eno">no</param>
            public ModifyEngineData(short enginecode, string enginename, string engineregcode, string enginemoney, int eno = -1)
            {
                _EngineCode = enginecode;
                _EngineName = enginename;
                _EngineRegCode = engineregcode;
                _EngineMoney = enginemoney;
                _ENo = eno;
            }

            public int ENo
            {
                get { return this._ENo; }
                set { this._ENo = value; }
            }

            public short EngineCode { get { return this._EngineCode; } }
            public string EngineName { get { return this._EngineName; } }
            public string EngineRegCode { get { return this._EngineRegCode; } }
            public string EngineMoney { get { return this._EngineMoney; } }
        }
        #endregion
    }
}
