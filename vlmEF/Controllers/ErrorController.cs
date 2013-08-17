using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace vlmEF.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            //TODO: Log Error
            return View();
        }

        public ActionResult NotFound()
        {
            //TODO: Log 404
            return View();
        }

        public ActionResult NotAllowed()
        {
            //TODO: Log 
            return View();
        }

    }
}
