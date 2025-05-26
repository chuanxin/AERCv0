using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class SectionView
    {
        public string CityCode { get; set; }
        public short TownId { get; set; }
        public int SectionCode { get; set; }
        // <summary>縣市DropDownList</summary>
        public IEnumerable<SelectListItem> CityDDL { get; set; }
        /// <summary>鄉鎮市DropDownList</summary>
        public IEnumerable<SelectListItem> TownDDL { get; set; }
        public IEnumerable<SelectListItem> SectionDDL { get; set; }
    }
    public class SectionList
    {
        public int Section_Id { get; set; }
        public string Section_Code { get; set; }
        public string Section { get; set; }
        public string Subsection { get; set; }
        public short Town_Id { get; set; }
    }
    public class LandNumberList
    {
        public int Land_Number_Id { get; set; }
        public string Land_Code { get; set; }
        public int Section_Id { get; set; }
        public byte Land_type_Id { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
    }
}
