using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.ViewModel
{
    public class Operatoritem
    {
        public string Operator { get; set; }
    }

    public class Symitem
    {
        public string type { get; set; }
        public float borderwidth { get; set; }
        public string bordercolor { get; set; }
        public string fillcolor { get; set; }
        public float filltrans { get; set; }
    }

    public class Labobjitem
    {
        public string fontsize { get; set; }
        public string field { get; set; }
        public string fontcolor { get; set; }

    }

    public class IrrgApiParms
    {
        public string Ia { get; set; }
        public string Mapcode { get; set; }
        public List<object> Locobj { get; set; }
        public List<Symitem> Sym { get; set; }
        public string Labtype { get; set; }
        public List<Labobjitem> Labobj { get; set; }
        public string Coor { get; set; }
        public string Tcode { get; set; }
        public string Wit { get; set; }
    }

    public class Whereitem
    {
        public List<object> wherE { get; set; }
    }

    public class secland
    {
        public string section { get; set; }
        public string land_no { get; set; }
    }
}
