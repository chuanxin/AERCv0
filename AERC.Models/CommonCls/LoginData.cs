/*
-- =============================================
-- Author:  Wei
-- Create date: 2014-10-5
-- Description: 讀取登入相關資料
-- =============================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models.ViewModel;

namespace AERC.Models.CommonCls
{
    public class LoginData
    {
        public LoginView Login(string Account, string PassWord)
        {
            CommonEntities comm = new CommonEntities();
            LoginView loginView = new LoginView();
            var admin = comm.Admin.ToList();
            var unit = comm.Unit.ToList();
            var loginData = (from Administrator in admin
                             join units in unit on Administrator.Unit_Id equals units.Unit_Id
                             where Administrator.Account.ToLower() == Account.ToLower() && Administrator.Password == PassWord && Administrator.Enabled == true
                             select new { Administrator.Admin_Id, Administrator.Name, Administrator.Account, Administrator.Unit_Id, units.Unit1, Administrator.Status, Administrator.UpdateDate }).ToList();
            if (loginData.Count <= 0)
                return null;
            foreach (var item in loginData)
            {
                loginView.Admin_Id = item.Admin_Id;
                loginView.Name = item.Name;
                loginView.Account = item.Account;
                loginView.Unit_Id = item.Unit_Id;
                loginView.UnitName = item.Unit1;
                loginView.Status = item.Status ?? false;
                loginView.UpdateDate = item.UpdateDate ?? null ;
            }
            return loginView;
        }
    }
}
