using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class SubsidyView
    {
        public int ApplyYear { get; set; }
        public int ApplyUnit { get; set; }
        public List<SelectListItem> YearDDL { get; set; }
        public List<SelectListItem> UnitDDL { get; set; }
        public SubsidyLimit SubsidyLimitList { get; set; }
    }
}
