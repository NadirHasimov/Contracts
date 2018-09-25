using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AdyContracts
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

            // Get the exception object.
            Exception exc = Server.GetLastError();

            // Handle HTTP errors
            if (exc.GetType() == typeof(HttpException))
            {
                // The Complete Error Handling Example generates
                // some errors using URLs with "NoCatch" in them;
                // ignore these here to simulate what would happen
                // if a global.asax handler were not implemented.
                if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
                    return;

                //Redirect HTTP errors to HttpError page

                // For other kinds of errors give the user some information
                // but stay on the default page
                Response.Write("<h2>Global Page Error</h2>\n");
                Response.Write(
                    "<p>Xəta baş verdi!</p>\n");
                Response.Write("<a href='../Home/Index'>" +
                    "Əsas səhifəy</a>ə qayıdın\n");
            }
            Utils.Email.SendEmail("hasimov.nadir@yandex.com", "Exception ADY CONTRACTS", exc.ToString());
            // Clear the error from the server
            Server.ClearError();
        }
    }
}
