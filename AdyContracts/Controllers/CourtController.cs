using AdyContracts.DALC;
using AdyContracts.DomainModels;
using AdyContracts.Models;
using AdyContracts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdyContracts.Controllers
{
    [Authorize]
    [Log]
    public class CourtController : Controller
    {
        // GET: Court
        [AuthorizeUsersRoles]
        public ActionResult CourtIndex()
        {
            CourtViewModel viewModel = new CourtViewModel();
            viewModel.CourtList = CourtDALC.GetTrials()
                                            .Select(trial => MapToCourtViewModel(trial)).ToList();
            ViewBag.UserList = UserDALC.GetUsers();
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeUsersRoles]
        public ActionResult CreateTrial(CourtViewModel courtViewModel)
        {
            string result = "#error";
            if (ModelState.IsValid)
            {
                courtViewModel.trialDate = DateTime.Parse(courtViewModel.time);
                if (CourtDALC.CreateTrial(MapToCourtDomainModel(courtViewModel)))
                {
                    result = "#successful";
                }
            }
            return new RedirectResult(Url.Action("CourtIndex", "Court") + result);
        }

        //[AuthorizeUsersRoles]
        public ActionResult AllMessages()
        {
            List<CourtNotificationModel> courtNotificationModel = CourtDALC.GetCourtNotifications(User.Identity.Name);
            return View(courtNotificationModel);
        }

        private CourtViewModel MapToCourtViewModel(CourtDomainModel courtDomainModel)
        {
            return new CourtViewModel
            {
                fullname = courtDomainModel.fullname,
                location = courtDomainModel.location,
                time = courtDomainModel.time,
                trialDate = courtDomainModel.trialDate
            };
        }

        private CourtDomainModel MapToCourtDomainModel(CourtViewModel courtViewModel)
        {
            return new CourtDomainModel
            {
                fullname = courtViewModel.fullname,
                location = courtViewModel.location,
                time = courtViewModel.time,
                trialDate = courtViewModel.trialDate,
                userId = courtViewModel.userId
            };
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }
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