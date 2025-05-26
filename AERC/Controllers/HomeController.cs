using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AERC.Controllers
{
    public class HomeController : Controller
    {        
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        #region

        public class SysMat
        {
            public string module { get; set; }
            public string matname { get; set; }
            public string spec { get; set; }
            public string description{ get; set; }

            public SysMat()
            {
                // TODO: Add constructor logic here                
            }

            public List<SysMat> GetMatList()
            {
                List<SysMat> list = new List<SysMat>();
                list.Add(new SysMat() { module = "輸水管", matname = "支管", spec="&1 1/4\" x4m", description = "輸水管之支管" });
                list.Add(new SysMat() { module = "接頭類", matname = "三通管", spec = "&4 x 1 1/4'(外牙)", description = "接頭類之三通管接頭類之三通管" });
                list.Add(new SysMat() { module = "接頭類", matname = "三通管", spec = "&2 x 1 1/4'(外牙)", description = "接頭類之三通管接頭類之三通管" });
                return list;
            }
        }

        public JsonResult GetData(string query)
        {
            var list = new SysMat();
            var fetchTag = list.GetMatList();//.Where(m => m.name.ToLower().StartsWith(query.ToLower()));
            return Json(fetchTag, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
