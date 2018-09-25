using AdyContracts.DALC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace AdyContracts.Utils
{

    public class AuthorizeUsersRolesAttribute : AuthorizeAttribute
    {

        // This method must be thread-safe since it is called by the thread-safe OnCacheAuthorization() method.
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            base.AuthorizeCore(httpContext);
            // wish base._usersSplit were protected instead of private...
            InitializeSplits(httpContext);

            IPrincipal user = httpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            var userRequired = _usersSplit.Length > 0;
            var userValid = userRequired
                && _usersSplit.Contains(user.Identity.Name, StringComparer.OrdinalIgnoreCase);

            var roleRequired = _rolesSplit.Length > 0;
            var roleValid = (roleRequired)
                && _rolesSplit.Any(user.IsInRole);

            var userOrRoleRequired = userRequired || roleRequired;

            return userValid || roleValid;
        }

        private string[] _rolesSplit = new string[0];
        private string[] _usersSplit = new string[0];

        private void InitializeSplits(HttpContextBase httpContextBase)
        {
            var rd = httpContextBase.Request.RequestContext.RouteData;
            string currentAction = rd.GetRequiredString("action");
            lock (this)
            {
                if ((_rolesSplit.Length == 0) || (_usersSplit.Length == 0))
                {
                    _rolesSplit = UserDALC.GetRolesForMenu(currentAction).Split(',');
                    _usersSplit = UserDALC.GetUsersForMenu(currentAction).Split(',');
                }
            }
        }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
            base.OnResultExecuting(filterContext);
        }
    }
}