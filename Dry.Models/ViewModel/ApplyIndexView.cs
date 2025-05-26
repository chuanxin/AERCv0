using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Dry.Models.ViewModel
{
    public class ApplyIndexView
    {
        public IEnumerable<SelectListItem> ApplyYearDDL { get; set; }
    }
}
