using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.ViewModel
{
    /// <summary>
    /// 地段地號座標位置web service views
    /// </summary>
    public class CoordinateView
    {
        //public string landPointXY { get; set; }
        public List<landPointData> landPointXY { get; set; }
    }

    public class landPointData
    {
        /// <summary>
        /// 地號
        /// </summary>
        //public string landNo { get; set; }
        /// <summary>
        /// x軸
        /// </summary>
        public string X { get; set; }
        /// <summary>
        /// y軸
        /// </summary>
        public string Y { get; set; }
    }
}
