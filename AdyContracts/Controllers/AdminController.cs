using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdyContracts.DALC;
using AdyContracts.DomainModels;
using AdyContracts.Models;
using AdyContracts.MyRoleProvider;
using AdyContracts.Utils;

namespace AdyContracts.Controllers
{
    [Log]
    public class AdminController : Controller
    {
        // GET: Admin
        [AuthorizeUsersRoles]
        public ActionResult Index(string userType = "False")
        {
            UserViewModel model = new UserViewModel();

            List<SelectListItem> departments = new List<SelectListItem>();
            model.userList = AdminDALC.GetAllUsers().Where(row => row.confirmationStatus == bool.Parse(userType.ToString())).Select(row => MapToUserViewModel(row)).ToList();
            model.menus = AdminDALC.GetMenus();

            ViewBag.departments = UserDALC.GetDepartments();
            ViewBag.Roles = AdminDALC.getAllRoleList();
            return View(model);
        }

        [HttpPost]
        [AuthorizeUsersRoles]
        public ActionResult DenyRequest(int[] ids)
        {
            UserDALC.Delete(ids);
            return Json(new { data = Url.Action("Index", "Admin") + "#successful" });
        }

        [HttpPost]
        [AuthorizeUsersRoles]
        public ActionResult Index(string[] selectedItems)
        {
            try
            {
                if (selectedItems.Length == 0)
                {
                    return View();
                }
                var list = new List<Tuple<int, string, int>>();
                int role;
                foreach (var item in selectedItems)
                {
                    role = 0;
                    string[] vs = item.Split(',');
                    int.TryParse(vs[2], out role);
                    list.Add(new Tuple<int, string, int>(int.Parse(vs[0]), vs[1], role));
                }
                foreach (var m in list)
                {
                    if (AdminDALC.approveUser(m.Item1, m.Item3))
                    {
                        Utils.Email.SendEmail(m.Item2, "Test", "Test message from tester");
                    }
                }
            }
            catch (Exception exc)
            {
                ViewBag.ErrorMessage = exc.Message;
            }
            return Json(new { result = true });
        }

        [HttpPost]
        public ActionResult CreateRole(string roleName, int[] ids)
        {
            string result = AdminDALC.CreateRole(ids, roleName) ? "#successful" : "#error";
            return new RedirectResult(Url.Action("Index", "Admin") + result);
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
                password = model.password,
                department = model.department,
                confirmationStatus = model.confirmationStatus
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

        protected override void OnException(ExceptionContext filterContext)
        {
            UserDALC.addLog(HttpContext, false, filterContext.Exception.Message);
            filterContext.ExceptionHandled = true;
            base.OnException(filterContext);
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