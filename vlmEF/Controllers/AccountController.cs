﻿using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using vlmEF.Interfaces;
using vlmEF.Models;
using vlmEF.Services;
using System.Linq;
using System.Collections;
using System.Net;

namespace vlmEF.Controllers
{
    public class AccountController : Controller
    {
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            if (User.Identity.IsAuthenticated) // they're logged in, so they were probably redirected because of a 401, so send to not allowed.
            {
                return RedirectToAction("NotAllowed", "Error", new { statusCode = HttpStatusCode.OK });
            }
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    using (var context = new UsersContext())
                    {
                        var user = context.Users.First(x => x.UserName == model.UserName);
                        if (user.Disabled || 
                            (user.SubscriptionStart.HasValue && DateTime.Today < user.SubscriptionStart.Value) || 
                            (user.SubscriptionEnd.HasValue && DateTime.Today > user.SubscriptionEnd.Value))
                        {
                            return RedirectToAction("InvalidSubscription", "Error");
                        }
                    }
                    SetupFormsAuthTicket(model.UserName, model.RememberMe);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        [Authorize (Roles="SuperAdmin,Admin")]
        public ActionResult Register()
        {
            var context = new UsersContext();
            var companies =
                            context.Companies.ToList().Select(
                                x => new SelectListItem() { Selected = false, Text = x.CompanyName, Value = x.CompanyId.ToString(CultureInfo.InvariantCulture) });
            ViewBag.CompanyOptions = companies;
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);
                using (var context = new UsersContext())
                {
                    var signedInUser = context.Users.First(x => x.UserName == User.Identity.Name);

                    if (!User.IsInRole("SuperAdmin"))
                    {
                        model.CompanyId = (int)signedInUser.CompanyId;
                    }
                    var user = context.Users.Single(u => u.UserName == model.UserName);
                    var companyId = model.CompanyId;

                    user.CreatedBy = signedInUser.UserName;

                    model.SubscriptionEnd = model.SubscriptionEnd < model.SubscriptionStart ? model.SubscriptionStart : model.SubscriptionEnd;
                    user.SubscriptionStart = model.SubscriptionStart;
                    user.SubscriptionEnd = model.SubscriptionEnd;
                    user.CompanyId = companyId;
                    context.SaveChanges();
                }

                if (createStatus == MembershipCreateStatus.Success)
                {
                    return RedirectToAction("Index", "Manage");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // ChangePassword method not implemented in CustomMembershipProvider.cs
        // Feel free to update!

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception e)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private User SetupFormsAuthTicket(string userName, bool persistanceFlag)
        {
            User user;
            using (var usersContext = new UsersContext())
            {
                user = usersContext.Users.First(x => x.UserName == userName);
            }
            var userId = user.UserId;
            var userData = userId.ToString(CultureInfo.InvariantCulture);
            var authTicket = new FormsAuthenticationTicket(1, //version
                                                        userName, // user name
                                                        DateTime.Now,             //creation
                                                        DateTime.Now.AddMinutes(30), //Expiration
                                                        persistanceFlag, //Persistent
                                                        userData);

            var encTicket = FormsAuthentication.Encrypt(authTicket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            return user;
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
