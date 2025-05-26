using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AERC.Models.ViewModel
{
    public class LoginView
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public Guid Admin_Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 單位代碼
        /// </summary>
        public short Unit_Id { get; set; }
        /// <summary>
        /// 單位名稱
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// 承辦
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 密碼更新時間
        /// </summary>
        public DateTime? UpdateDate { get; set; }


    }
    public class Accounts
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required(ErrorMessage = "請輸入登入帳號")]
        public string Account { get; set; }
        /// <summary>
        /// 密碼
        /// </summary>
        [Required(ErrorMessage = "請輸入登入密碼")]
        public string PassWord { get; set; }

        /// <summary>
        /// 是否登入成功
        /// </summary>
        public bool IsLogin { get; set; }
    }
    public class UserInfo
    {
        /// <summary>
        /// 使用者代碼
        /// </summary>
        public Guid Admin_Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 登入帳號
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 單位名稱
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 職稱
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 組室
        /// </summary>
        public string Organization { get; set; }
        /// <summary>
        /// 電話
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// 手機
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// email
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 傳真
        /// </summary>
        public string Fax { get; set; }
        /// <summary>
        /// 是否為承辦人
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 密碼更新日期
        /// </summary>
        public DateTime UpdateTime { get; set; }

    }
}
