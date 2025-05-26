/*
-- =============================================
-- Author: WEI
-- Create date: 2014-10-23
-- Description: 帳號登入、登出
-- =============================================
*/

using AERC.Models.CommonCls;
using AERC.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BotDetect.Web.Mvc;

namespace Dry.Controllers
{
    public class AccountController : Controller
    {
        
        Accounts account = new Accounts();
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string msg)
        {
            ViewBag.ReturnUrl = returnUrl;
            account.IsLogin = true;
            if (msg != null)
            {
                TempData["msg"] = msg;
            }
            return View(account);
        }
        [HttpPost]
        [CaptchaValidation("CaptchaCode", "ExampleCaptcha", "驗證碼錯誤!")]
        public ActionResult Login(Accounts accound, string returnUrl, bool captchaValid)
        {
            if (!ModelState.IsValid)
            {
                return View(account);
            }
            Session.RemoveAll();
            LoginView loginView = new LoginView();
            LoginData loginData = new LoginData();
            loginView = loginData.Login(accound.Account, accound.PassWord);
            if (loginView == null) 
            {
                account.IsLogin = false;
                return View(account);
            }
                
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                loginView.Account, 
                DateTime.Now, 
                DateTime.Now.AddDays(1d), 
                true, 
                loginView.Admin_Id.ToString(), 
                FormsAuthentication.FormsCookiePath);
            string endTicket = FormsAuthentication.Encrypt(ticket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, endTicket));
            //if (Request.Cookies["UnitName"] != null) { Request.Cookies.Remove("UnitName"); }
            //if (Request.Cookies["UnitID"] != null) { Request.Cookies.Remove("UnitID"); }
            //if (Request.Cookies["Status"] != null) { Request.Cookies.Remove("Status"); }

            Response.Cookies.Add(new HttpCookie("UnitName", loginView.UnitName));
            Response.Cookies.Add(new HttpCookie("UnitID", loginView.Unit_Id.ToString()));
            Response.Cookies.Add(new HttpCookie("Status", loginView.Status.ToString()));
            //HttpCookie myCookie = new HttpCookie("UserSettings");
            //myCookie.HttpOnly = true;
            ////myCookie["User"] = loginView.Admin_Id.ToString();
            //myCookie.Values.Add("UnitName", loginView.UnitName);
            //myCookie.Values.Add("UnitID", loginView.Unit_Id.ToString());
            //myCookie.Values.Add("Status", loginView.Status.ToString());
            //myCookie.Expires = DateTime.Now.AddDays(1d);
            //Response.Cookies.Add(myCookie);

            //string aa = Request.Cookies["UnitID"].Value;
            
            Session["User"] = loginView.Admin_Id;
            
            Session["UnitName"] = loginView.UnitName;
            
            Session["UnitID"] = loginView.Unit_Id;
            
            Session["Status"] = loginView.Status;
            //return Content("OK");

            
            if (loginView.UpdateDate == null)
            {
                TempData["msg"] = "密碼超過60天未更換，請更改密碼";
                return RedirectToAction("ChangePwd", "UserManage"/*,new { msg = "密碼超過60天未更換" }*/);

            }
            else if (DateTime.Now.Subtract((DateTime)loginView.UpdateDate).Days >= 60)
            {
                TempData["msg"] = "密碼超過60天未更換，請更改密碼";
                return RedirectToAction("ChangePwd", "UserManage"/*, new { msg = "密碼超過60天未更換" }*/);
            }

            if (returnUrl != null)
                return Redirect(returnUrl);
            else
               
            if (loginView.Unit_Id  == 0 || loginView.Unit_Id == 99)
            {
                return RedirectToAction("VIPPage", "Home");

            }else return RedirectToAction("Index", "ApplyIndex");
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            
            Session.Abandon();
            if (TempData["msg"] != null)
            {
                return RedirectToAction("Login", "Account", new {msg= TempData["msg"] });
            }else return RedirectToAction("Login", "Account");
        }
        public PartialViewResult UserInfo()
        {
            AERC.Models.ViewModel.UserInfo userInfo = new AERC.Models.ViewModel.UserInfo();
            AERC.Models.Service.AdminDBService adminDB = new AERC.Models.Service.AdminDBService();
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value);
            userInfo = adminDB.GetIndividualAdmin(Guid.Parse(ticket.UserData));
            return PartialView(userInfo);
        }
    }
}
