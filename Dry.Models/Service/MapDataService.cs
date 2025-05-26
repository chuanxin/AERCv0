using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dry.Models;
using Dry.Models.ViewModel;


namespace Dry.Models.Service
{
    public class MapDataService
    {
        private DryEntities DryDB = new DryEntities();
        #region 土地資料服務
        public MapLandData getLandData(string fno)
        {
            Guid gfno = Guid.Parse(fno);
            var objs = (from f in DryDB.Farm
                        join l in DryDB.LandData on f.Section equals l.Section_Id
                        where f.FNo == gfno
                        select new { l.LandCode, f.LandNo }).FirstOrDefault();
            MapLandData o = new MapLandData();
            if (objs != null)
            {
                o.LandCode = objs.LandCode;
                o.LandNo = toLand_no(objs.LandNo);
            }
            return o;
        }

       

        #endregion

        #region 依據地段地號產生API JSON
        public List<IrrgApiParms> createMapJsonByLand(List<MapLandData> lands)
        {
            List<IrrgApiParms> objs = new List<IrrgApiParms>();

            if (lands.Count == 0)
            {
                return objs;
            }

            IrrgApiParms o = new IrrgApiParms();
            o.Ia = "08";
            o.Tcode = "nzCzG94AYNaH42cRxAWdsFl0yyS86khA";
            o.Mapcode = "02";
            o.Labtype = "normal";
            o.Coor = "0";
            o.Wit = "";
            //add section landno
            Whereitem wo = new Whereitem();
            wo.wherE = new List<object>();
            foreach (var item in lands)
            {
                wo.wherE.Add(new secland { section = "'" + item.LandCode + "'", land_no = "'" + item.LandNo + "'" });    
            }
            Operatoritem opr = new Operatoritem();
            opr.Operator = "0";

            o.Locobj = new List<object>();
            o.Locobj.Add(wo);
            o.Locobj.Add(opr);


            List<Symitem> so = new List<Symitem>();
            so.Add(new Symitem { type = "02", bordercolor = "#EEEE00", borderwidth = 1.5f, fillcolor = "#FF00FF", filltrans = 0.5f });
            o.Sym = so;

            List<Labobjitem> lab = new List<Labobjitem>();
            lab.Add(new Labobjitem { fontsize = "16px", fontcolor = "#5500DD", field = "landtext" });
            o.Labobj = lab;

            objs.Add(o);

            return objs;

        }
        #endregion


        private string toLand_no(string landno)
        {

            if (landno.Contains("-"))
            {
                string[] o = landno.Split('-');
                string o1 = o[0];
                string o2 = o[1];
                int l1 = int.Parse(o1);
                int l2 = int.Parse(o2);
                if (l2 > 0)
                {
                    return l1.ToString() + "-" + l2.ToString();
                }
                else
                {
                    return l1.ToString();
                }


            }
            else
            {
                return landno;
            }
        }

        public class MapLandData
        {
            public string LandCode { get; set; }
            public string LandNo { get; set; }
        }

    }
}
