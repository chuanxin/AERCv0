using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AERC.Models.ViewModel;
using AERC.Models.CommonCls;
using AERC.Models;
using AERC.Models.Service;
using Newtonsoft.Json;
using Dry.Models.Service;
using EncryptStringV1;

namespace Dry.Controllers
{
    [Authorize][SessionCheck]
    public class UserManageController : Controller
    {
        // GET: /UserManage/
        public ActionResult UserManage()
        {
            AdminView admin = new AdminView();
            admin.TitleDDL = new GetTitle().GetTitles();
            admin.OrganDDL = new GetOrgan().GetOrgans();
            admin.UnitId = Convert.ToInt32(Session["UnitId"].ToString());
            return View(admin);
        }

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult UserManage(AdminView adminView)
        {
            Admin admin = new Admin();
            admin.Admin_Id = System.Guid.NewGuid();
            admin.Name = adminView.Name;
            admin.Account = adminView.Account;
            admin.Password = adminView.Password;
            admin.Unit_Id = (short)adminView.UnitId;
            admin.Title_Id = adminView.Title_Id;
            admin.Organization_Id = adminView.Organization_Id;
            admin.Telephone = adminView.Telephone;
            admin.Mobile = adminView.Mobile;
            admin.Fax = adminView.Fax;
            admin.Status = adminView.Status;
            admin.Enabled = adminView.Enabled ;
            admin.UpdateDate = DateTime.Now;
            new AdminDBService().InsertAdmin(admin);
            TempData["message"] = "會員[" + adminView.Name + "]新增成功!";
            return RedirectToAction("UserManage", "UserManage");
        }

        public ActionResult UserList()
        {
            AdminView admin = new AdminView();
            admin.UnitId = Convert.ToInt32(Session["UnitId"].ToString());
            return PartialView(admin);
        }

        public ActionResult UserEdit() 
        {
            string Admin_Id = Session["User"].ToString();            
            Admin admin = new AdminDBService().GetIndividualAdmin(Admin_Id);
            AdminView adminView = new AdminView();
            adminView.Name = admin.Name;
            adminView.Account = admin.Account;
            adminView.Password = admin.Password;
            adminView.TitleDDL = new GetTitle().GetTitles();
            adminView.OrganDDL = new GetOrgan().GetOrgans();
            adminView.Title_Id = admin.Title_Id ?? 0;
            adminView.Organization_Id = admin.Organization_Id ?? 0;
            adminView.Telephone = admin.Telephone;
            adminView.Mobile = admin.Mobile;
            adminView.Fax = admin.Fax;
            adminView.Status = admin.Status ?? false;
            adminView.Enabled = admin.Enabled ?? false;
            return PartialView(adminView);
        }

        public ActionResult ChangePwd(string msg)
        {
            string Admin_Id = Session["User"].ToString();
            
            RijndaelEnhanced encrypttools = new RijndaelEnhanced("alex", "aerc4521314#alex");
            string token = encrypttools.Encrypt(Admin_Id);
            ViewBag.token = token;
            //if (TempData["msg"] != null)
            //{
            //    ViewBag.message = TempData["msg"];
            //}
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(string token, string pwd1)
        {
            RijndaelEnhanced encrypttools = new RijndaelEnhanced("alex", "aerc4521314#alex");
            string gid = encrypttools.Decrypt(token);
            int rec = new AdminDBService().ChangeAdminPwd(gid, pwd1);
            //int rec = 1;
            if (rec == 0)
            {
                TempData["msg"] = "密碼無法修正";
                return View();
            }
            else
            {
                TempData["msg"] = "密碼修改成功，請使用新密碼請重新登入";
                return RedirectToAction("logout", "Account");
            }
            
            
            
        }
        public ActionResult UserEditByDuty(string Admin_Id)
        {
            Admin admin = new AdminDBService().GetIndividualAdmin(Admin_Id);
            AdminView adminView = new AdminView();
            adminView.Name = admin.Name;
            adminView.Account = admin.Account;
            adminView.Password = admin.Password;
            adminView.TitleDDL = new GetTitle().GetTitles();
            adminView.OrganDDL = new GetOrgan().GetOrgans();
            adminView.Title_Id = admin.Title_Id ?? 0;
            adminView.Organization_Id = admin.Organization_Id ?? 0;
            adminView.Telephone = admin.Telephone;
            adminView.Mobile = admin.Mobile;
            adminView.Fax = admin.Fax;
            adminView.Status = admin.Status ?? false;
            adminView.Enabled = admin.Enabled ?? false;
            return PartialView(adminView);
        }
        
        public ActionResult UserDataView(string Admin_Id)
        {
            Admin admin = new AdminDBService().GetIndividualAdmin(Admin_Id);
            AdminViewData adminView = new AdminViewData();
            adminView.Name = admin.Name;
            adminView.Account = admin.Account;
            adminView.Password = admin.Password;
            adminView.TitleDDL = new GetTitle().GetTitles();
            adminView.OrganDDL = new GetOrgan().GetOrgans();
            
            adminView.Title_Id = admin.Title_Id ?? 0;
            adminView.Title = new AERC.Models.CommonCls.GetTitle().GetTitleName(adminView.Title_Id) ?? "";
            adminView.Organization_Id = admin.Organization_Id ?? 0;
            adminView.Organization = new AERC.Models.CommonCls.GetOrgan().GetOrgans(adminView.Organization_Id) ?? "";
            adminView.Telephone = admin.Telephone;
            adminView.Mobile = admin.Mobile;
            adminView.Fax = admin.Fax;
            adminView.Status = admin.Status ?? false;
            adminView.Enabled = admin.Enabled ?? false;
            return PartialView(adminView);
        }

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult UserEditByDuty(AdminView adminView)
        {
            new AdminDBService().UpdateAdmin(adminView);
            TempData["message"] = "修改成功!";
            //return RedirectToAction("UserManage", "UserManage");
            return Redirect("UserEditSuccess");
        }

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult UserEdit(AdminView adminView) 
        {
            adminView.Admin_Id = Session["User"].ToString();
            new AdminDBService().UpdateAdmin(adminView);
            TempData["message"] = "修改成功!";
            return RedirectToAction("UserManage", "UserManage");
        }
        public ActionResult UserEditSuccess()
        {
            return PartialView();
        }
        #region 刪除管理者資料
        /// <summary>
        /// 刪除會員資料
        /// </summary>
        /// <param name="MemId"></param>
        /// <returns></returns>
        public JsonResult DelMember(string Admin_Id)
        {
            new AdminDBService().DelAdmin(Admin_Id);
            return Json(new { result = "刪除成功!" });
        }
        #endregion

        #region 取得會員列表
        public JsonResult GetMembers(string sidx, string sord, int? page, int? rows, string UnitId)
        {
            var adminDatas = new GetAdminData().GetAdmin(Convert.ToInt32(UnitId));
            int pageSize = rows.HasValue ? rows.Value : 15;
            int pageNum = page.HasValue ? page.Value : 1;
            int totalRecords = adminDatas.Count;
            int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);
            var jsonData = new
            {
                total = totalPages,
                page = pageNum,
                records = totalRecords,
                rows = adminDatas.Skip((pageNum - 1) * pageSize).Take(pageSize)
                .Select(x => new
                {
                    Admin_Id = x.Admin_Id,
                    Unit = x.Unit,
                    Name = x.Name,
                    Account = x.Account,
                    Password = x.Password,
                    Title = x.Title,
                    Organization = x.Organization,
                    Telephone = x.Telephone,
                    Mobile = x.Mobile,
                    Fax = x.Fax,
                    Status = (x.Status == true) ? "承辦" : "非承辦",
                    Enabled = (x.Enabled == true) ? "啟用" : "停用"
                })
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
