using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.ViewModel
{
    public class StructureView
    {
        public class StructData
        {
            /// <summary>
            /// 構造物代碼
            /// </summary>
            public Guid SId { get; set; }
            /// <summary>
            /// 構造物名稱
            /// </summary>
            public string SName { get; set; }
            /// <summary>
            /// 構造物類別代碼
            /// </summary>
            public int TypeId { get; set; }
            /// <summary>
            /// 構造物類別名稱
            /// </summary>
            public string TypeName { get; set; }
            /// <summary>
            /// 經度
            /// </summary>
            public double Longitude { get; set; }
            /// <summary>
            /// 緯度
            /// </summary>
            public double Latitude { get; set; }
            /// <summary>
            /// 灌溉面積
            /// </summary>
            public decimal Area { get; set; }
            /// <summary>
            /// 圳路長度
            /// </summary>
            public decimal Length { get; set; }
            /// <summary>
            /// 渠道容量
            /// </summary>
            public decimal Capacity { get; set; }
            /// <summary>
            /// 管理單位代碼
            /// </summary>
            public short Unit_Id { get; set; }
            /// <summary>
            /// 管理單位名稱
            /// </summary>
            public string UnitName { get; set; }
        }
    }
}
