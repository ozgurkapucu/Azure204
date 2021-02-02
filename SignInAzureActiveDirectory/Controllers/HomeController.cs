using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SignInAzureActiveDirectory.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
[Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            
            return View();
        }

        [Authorize(Roles ="Development")]
        public ActionResult Development()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        //Development

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public bool CheckIfAdmin()
        {
            if (User.Claims.FirstOrDefault(c => c.Type == "groups" &&
                c.Value.Equals("42f51be8-28c4-995d-a69f-f6f42f96a5cd", StringComparison.CurrentCultureIgnoreCase)) != null)
                return true;
            else
                return false;
        }
    }
}