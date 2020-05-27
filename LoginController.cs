using Captivate.Adapters;
using Captivate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Captivate.Controllers
{
    public class LoginController : Controller
    {
        DriverAdapter driverAdapter;
        public LoginController()
        {
            driverAdapter = new DriverAdapter();
        }


        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View("~/Views/Admin/Login/AdminLogin.cshtml");
        }

        [HttpPost]
        public ActionResult Login(DriverModel adminLogin, string returnUrl)
        {
            DriverModel login = driverAdapter.SelectAdminLogins().Where(x => x.Username == adminLogin.Username && x.Password == adminLogin.Password).FirstOrDefault();
            
            if (login != null && adminLogin.Username != string.Empty && adminLogin.Password != string.Empty)
            {
                FormsAuthentication.SetAuthCookie(login.Username, false);
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return View("~/Views/Admin/Error/ErrorAdminLogin.cshtml");
                }
            }
            else
            {
                ModelState.AddModelError("", "");
                return View("~/Views/Admin/Error/ErrorAdminLogin.cshtml");
            }
        }
    }
}