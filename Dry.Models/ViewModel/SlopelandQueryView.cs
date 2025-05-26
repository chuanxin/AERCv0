using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class SlopelandQueryView
    {
        /// <summary>縣市代碼</summary>
        public string CityCode { get; set; }
        /// <summary>鄉鎮市代碼</summary>
        public string TownId { get; set; }
        /// <summary>縣市DropDownList</summary>
        public List<SelectListItem> CityDDL { get; set; }
        /// <summary>鄉鎮市DropDownList</summary>
        public List<SelectListItem> TownDDL { get; set; }
        /// <summary>地段</summary>
        public string Section { get; set; }
        ///<summary>地段DropDownList</summary>
        public List<SelectListItem> SectionList { get; set; }
        /// <summary>地號</summary>
        public string LandNo { get; set; }
    }
    public class ResultView
    {
        public List<Result> results { get; set; }
    }
    public class Result
    {
        public string landItem { get; set; }
        public string landRights { get; set; }
        public string landClass { get; set; }
        public string limit_exceed { get; set; }
        public string inSoilWater { get; set; }
        public string gisBou { get; set; }
        public string status { get; set; }
        public string sys_hint { get; set; }
        public string landNo { get; set; }
    }
    public class RoadData
    {
        public string RCity { get; set; }
        public string RTwon { get; set; }
        public string RRoad { get; set; }
        public string RRoadNumber { get; set; }
        public string CheckType { get; set; }
        public string Geological_Conservation { get; set; }
        public string Water_Conservation { get; set; }
    } 
}
