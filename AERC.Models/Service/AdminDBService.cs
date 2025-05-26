using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AERC.Models.ViewModel;

namespace AERC.Models.Service
{
    public class AdminDBService
    {
        private CommonEntities _entity = new CommonEntities();
        DBCommon.DbEvent status = new DBCommon.DbEvent();

        /// <summary>
        /// 新增管理者
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        public int InsertAdmin(Admin admin)
        {
            try
            {
                _entity.Admin.Add(admin);
                return _entity.SaveChanges();
            }
            catch (Exception e)
            {
                status.DbMessage = "Create Admin Failed : " + e.Message;
                return 0;
            }
        }

        /// <summary>
        /// 取得個人管理者資料
        /// </summary>
        /// <param name="Admin_Id"></param>
        /// <returns></returns>
        public Admin GetIndividualAdmin(string Admin_Id)
        {
            Guid Id = Guid.Parse(Admin_Id);
            return _entity.Admin.Where(x => x.Admin_Id == Id).FirstOrDefault();
        }
        /// <summary>
        /// 取得個人資訊
        /// </summary>
        /// <param name="AdminId"></param>
        /// <returns></returns>
        public UserInfo GetIndividualAdmin(Guid AdminId)
        {
            UserInfo result = new UserInfo();
            var data = (from admin in _entity.Admin
                         join units in _entity.Unit on admin.Unit_Id equals units.Unit_Id
                         where admin.Admin_Id == AdminId
                         select new
                         {
                             admin.Admin_Id,
                             admin.Name,
                             admin.Account,
                             units.Unit1,
                             admin.Telephone,
                             admin.Mobile,
                             admin.email,
                             admin.Fax,
                             admin.Status
                         });
            foreach(var item in data)
            {
                //result.Admin_Id = item.Admin_Id;
                result.Name = item.Name;
                result.Account = item.Account;
                result.Unit = item.Unit1;
                result.Telephone = item.Telephone;
                result.Mobile = item.Mobile;
                result.email = item.email;
                result.Fax = item.Fax;
                result.Status = item.Status.Value;
            }
            return result;
        }

        /// <summary>
        /// 更新管理者
        /// </summary>
        /// <param name="adminView"></param>
        /// <returns></returns>
        public int UpdateAdmin(AdminView adminView) 
        {
            Guid id = Guid.Parse(adminView.Admin_Id);
            Admin admin = _entity.Admin.Where(x => x.Admin_Id == id).FirstOrDefault();
            admin.Name = adminView.Name;
            admin.Account = adminView.Account;
            admin.Password = adminView.Password;
            admin.Title_Id = adminView.Title_Id;
            admin.Organization_Id = adminView.Organization_Id;
            admin.Telephone = adminView.Telephone;
            admin.Mobile = adminView.Mobile;
            admin.Fax = adminView.Fax;
            admin.Status = adminView.Status;
            admin.Enabled = adminView.Enabled;
            try
            {
                return _entity.SaveChanges();
            }
            catch (Exception e)
            {
                status.DbMessage = "Update Admin Failed : " + e.Message;
                return 0;
            }
        }

        /// <summary>
        /// 刪除管理者
        /// </summary>
        /// <param name="Admin_Id"></param>
        /// <returns></returns>
        public int DelAdmin(string Admin_Id) 
        {
            Guid id = Guid.Parse(Admin_Id);
            _entity.Admin.Remove(_entity.Admin.Where(x => x.Admin_Id == id).FirstOrDefault());
            return _entity.SaveChanges();
           
        }
        public int ChangeAdminPwd(string Admin_id,/*string pwd,*/string pwd1)
        {
            Guid id = Guid.Parse(Admin_id);
            Admin admin = _entity.Admin.Where(x => x.Admin_Id == id).FirstOrDefault();
            //if (admin.Password != pwd)
            //{
            //    status.DbMessage = "使用者密碼不正確";
            //    return 0;
            //}

            admin.Password = pwd1;
            admin.UpdateDate = DateTime.Now;
            try
            {
                return _entity.SaveChanges();
            }
            catch (Exception e)
            {
                status.DbMessage = "Update Admin Failed : " + e.Message;
                return 0;
            }
        }
    }
}
