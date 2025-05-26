using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dry.Models.ViewModel
{
    public class EfileView
    {
        /// <summary>
        /// 檔案流水號
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// 案件編號
        /// </summary>
        public int MapNo { get; set; }
        /// <summary>
        /// 檔案類型
        /// </summary>
        public byte FileType { get; set; }
        /// <summary>
        /// 檔案類型名稱
        /// </summary>
        public string FileTypeCNS { get; set; }
        /// <summary>
        /// 檔案路徑
        /// </summary>
        public string Filepath { get; set; }
        /// <summary>
        /// 檔案描述
        /// </summary>
        public string FileDes { get; set; }
        /// <summary>
        /// 上傳時間
        /// </summary>
        public string UpldTime { get; set; }
    }
    public class EfileListView
    {
        public List<EfileView> EfileList { get; set; }
        public string ServerPath { get; set; }
    }
}
