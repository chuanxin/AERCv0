using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.ViewModel
{
    public class CropView
    {
        /// <summary>
        /// 作物代碼
        /// </summary>
        public short Crop_Id { get; set; }
        /// <summary>
        /// 作物名
        /// </summary>
        public string Crop { get; set; }
        /// <summary>
        /// 作物用水系數
        /// </summary>
        public double Crop_Modulus { get; set; }
        /// <summary>
        /// 作物類別代碼
        /// </summary>
        public short Crop_Type_Id { get; set; }
        /// <summary>
        /// 作物類別
        /// </summary>
        public string Crop_Type { get; set; }
    }
}
