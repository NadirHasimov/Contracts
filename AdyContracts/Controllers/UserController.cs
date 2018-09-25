using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdyContracts.Models;
using AdyContracts.DomainModels;
using AdyContracts.DALC;
using System.Web.Security;
using System.Net;
using System.Text;
using AdyContracts.Utils;
using DocumentFormat.OpenXml.Wordprocessing;
using System.IO;
using System.Globalization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.DirectoryServices;
using System.Web;
using System.Net.Sockets;

namespace AdyContracts.Controllers
{
    public class UserController : Controller
    {
        public bool LoginTest(string username, string password, string returnUrl)
        {
            const string localIp = "192.168.";
            string path = AppConfig.Path;
            bool status = false;
            string description = "";
            string ip = string.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"])
                    ? Request.UserHostAddress
                    : Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip) || ip.Trim() == "::1")
            { // still can't decide or is LAN
                var lan = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(r => r.AddressFamily == AddressFamily.InterNetwork);
                ip = lan == null ? string.Empty : lan.ToString();
            }
            if (UserDALC.CheckPermission(username) || ip.Contains(localIp))
            {
                using (DirectoryEntry entry = new DirectoryEntry(path, username, password))
                {
                    DirectorySearcher searcher = new DirectorySearcher(entry);
                    searcher.Filter = "(objectclass=user)";
                    try
                    {
                        var ent = searcher.FindOne();
                        status = true;
                        return true;
                    }
                    catch (Exception exc)
                    {
                        description = exc.Message;
                        return false;
                    }
                    finally
                    {
                        UserDALC.addLog(HttpContext, status, description);
                    }
                }
            }
            return false;
        }

        [HttpPost]
        public ActionResult signUp(UserViewModel model)
        {
            string result =
            UserDALC.add(MapToUserModel(model)) ?
            "#successful" : "#error";

            if (model.roleId != null)
            {
                if (String.Equals(result, "#successful") && model.emailSendStatus)
                {
                    Email.SendEmail(model.email, "ADY Express", "Admin sizin hesabı təsdiqlədi! Sistemə daxil ola bilərsiniz!");
                }
                return new RedirectResult(Url.Action("Index", "Admin") + result);
            }
            return new RedirectResult(Url.Action("signIn", "User") + result);
        }

        public ActionResult signIn()
        {
            string ip = Request.UserHostAddress;
            ViewBag.Departments = UserDALC.GetDepartments();
            return View();
        }
        [HttpPost]
        public ActionResult signIn(string username, string password, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            ReturnUrl = ReturnUrl ?? Url.Action("Index", "Home");
            if (UserDALC.Login(username, password))
            {
                FormsAuthentication.SetAuthCookie(username, false);
                UserDALC.InsertLog(username, password, true);
                return Redirect(ReturnUrl);
            }
            UserDALC.InsertLog(username, password, false);
            ViewBag.Departments = UserDALC.GetDepartments();
            ViewBag.InfoMessage = "İstifadəçi adı və ya parol səhvdi.";
            return View();
        }
        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return Redirect(HttpContext.Request.UrlReferrer.ToString());
        }

        public ActionResult CheckUsernameExists(string username)
        {
            if (!UserDALC.CheckUsername(username))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeLanguage(string lang)
        {
            new LanguageManager().SetLanguage(lang);
            return RedirectToAction("Index", "Home");
        }

        private UserViewModel MapToUserViewModel(UserModel model)
        {
            return new UserViewModel()
            {
                Id = model.Id,
                firstName = model.firstName,
                lastName = model.lastName,
                email = model.email,
                birthdate = model.birthdate,
                departId = model.departId,
                gender = model.gender,
                username = model.username,
                password = model.password
            };
        }
        private UserModel MapToUserModel(UserViewModel model)
        {
            return new UserModel()
            {
                firstName = model.firstName,
                lastName = model.lastName,
                gender = model.gender,
                birthdate = model.birthdate.Date,
                email = model.email,
                password = model.password,
                departId = model.departId,
                Id = model.Id,
                username = model.username,
                roleId = model.roleId
            };
        }

        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            string lang = null;
            HttpCookie langCookie = Request.Cookies["culture"];
            if (langCookie != null)
            {
                lang = langCookie.Value;
            }
            else
            {
                var userLanguage = Request.UserLanguages;
                var userLang = userLanguage != null ? userLanguage[0] : "";
                if (userLang != "")
                {
                    lang = userLang;
                }
                else
                {
                    lang = LanguageManager.GetDefaultLanguage();
                }
            }
            new LanguageManager().SetLanguage(lang);
            return base.BeginExecuteCore(callback, state);
        }
    }


}
