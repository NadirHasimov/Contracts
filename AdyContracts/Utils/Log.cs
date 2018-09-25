using AdyContracts.DALC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdyContracts.Utils
{
    public class Log : ActionFilterAttribute, IActionFilter
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            UserDALC.addLog(filterContext.HttpContext, true, "");
            base.OnActionExecuted(filterContext);
        }
    }
}