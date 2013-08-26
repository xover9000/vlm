using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace vlmEF.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            try { Response.StatusCode = (int)HttpStatusCode.InternalServerError; }
            catch { }
            return View();
        }

        public ActionResult NotFound()
        {
            try
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;

                // log the 404 so we can keep an eye on them
                var Exception = new HttpException((int)HttpStatusCode.NotFound, "Destination not found: " + Request.RawUrl);
                var Error = new Elmah.Error(Exception);
                var Logger = Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current);

                Logger.Log(Error);
            }
            catch { }

            return View();
        }

        // in .net 4.0, when you return a 401 (unauthorized), it automatically redirects to the login page. The login page has logic to redirect BACK here if the user is logged in, with
        // status code 200 (ok) to allow this page to be displayed properly. So this gets hit twice, once it logs the error and returns 401, once it returns 200 and displays the page
        public ActionResult NotAllowed(HttpStatusCode statusCode = HttpStatusCode.Unauthorized)
        {
            try
            {
                Response.StatusCode = (int)statusCode;
                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    // log the 401 so we can keep an eye on them
                    var Exception = new HttpException((int)HttpStatusCode.Unauthorized, "Unauthorized action at: " + Request.RawUrl);
                    var Error = new Elmah.Error(Exception);
                    var Logger = Elmah.ErrorLog.GetDefault(System.Web.HttpContext.Current);

                    Logger.Log(Error);
                }
            }
            catch { }

            return View();
        }

        public ActionResult InvalidSubscription()
        {
            return View();
        }
    }
}
