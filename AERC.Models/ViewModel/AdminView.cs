/*
-- =============================================
-- Author: Ian
-- Create date: 2014-11-18
-- Description: 使用者View型別定義
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AERC.Models.ViewModel
{

    public class AdminView
    {
        [Required(ErrorMessage = " 必填")]
        public string Name { get; set; }
        [Required(ErrorMessage = " 必填")]
        public string Account { get; set; }
        [Required(ErrorMessage = " 必填")]
        public string Password { get; set; }
        [Required(ErrorMessage = " 必填")]
        public short Title_Id { get; set; }
        public IEnumerable<SelectListItem> TitleDDL { get; set; }
        [Required(ErrorMessage = " 必填")]
        public short Organization_Id { get; set; }
        public IEnumerable<SelectListItem> OrganDDL { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public bool Status { get; set; }
        public int UnitId { get; set; }
        public string Admin_Id { get; set; }
        public bool Enabled { get; set; }
    }
    public class AdminViewData : AdminView
    {
        public string Title { get; set; }
        public string Organization { get; set; }
    }
}
