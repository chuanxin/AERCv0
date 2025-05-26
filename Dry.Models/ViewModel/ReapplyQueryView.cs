using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class ReapplyQueryView
    {
        public string FarmCityCode { get; set; }
        public short FarmTownId { get; set; }
        public int FarmSection { get; set; }
        public string LandNo { get; set; }

        public List<SelectListItem> CityDDL { get; set; }
        public List<SelectListItem> TownDDL { get; set; }
        public List<SelectListItem> SectionList { get; set; }

    }
}
