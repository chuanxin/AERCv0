using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.ViewModel
{
    public class TownVIew
    {
        /// <summary>
        /// 鄉鎮代碼
        /// </summary>
        public short Town_Id { get; set; }
        /// <summary>
        /// 鄉鎮官碼
        /// </summary>
        public string Town_Code { get; set; }
        /// <summary>
        /// 鄉鎮名稱
        /// </summary>
        public string Town { get; set; }
        /// <summary>
        /// 郵遞區號
        /// </summary>
        public short Zip_Code { get; set; }
        /// <summary>
        /// 地政事務所代碼
        /// </summary>
        public short Land_Office_Id { get; set; }
        /// <summary>
        /// 地政事務所官碼
        /// </summary>
        public string Land_Office_Code { get; set; }
        /// <summary>
        /// 地政事務所名稱
        /// </summary>
        public string Land_Office { get; set; }
        /// <summary>
        /// 縣市代碼
        /// </summary>
        public string City_Code { get; set; }
    }
}
