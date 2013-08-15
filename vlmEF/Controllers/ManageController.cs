using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Data;

namespace vlmEF.Controllers
{
    [Authorize(Roles = "SuperAdmin")]  // Roles = "Role1, Role2, ..."  Putting the auth here protects all actions in the controller
    public class ManageController : Controller
    {
        private readonly UsersContext _usersContext;

        public ManageController()
        {
            _usersContext = new UsersContext();
        }

        
        public ActionResult Index()
        {
            return View(_usersContext.Users.Where(u => u.UserId > 1).ToList());
        }

        //public JsonResult UpdateRole(int userId, short roleId)
        //{
        //    _usersContext.AddToUserRoles(new UserRole{ UserId = userId, RoleId = roleId});
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        // Alex was here
        public ActionResult Edit(int id)
        {
            var user = _usersContext.Users.FirstOrDefault(x => x.UserId == id);
            if(user != null)
            {

                string roleIdAsString = user.RoleId.ToString(CultureInfo.InvariantCulture);
                // Get a list of roles, but as SelectListItems (selected, text, value)
                var roles =
                    _usersContext.Roles.ToList().Select(
                        x => new SelectListItem() {Selected = false, Text = x.RoleDescription, Value = x.RoleId.ToString(CultureInfo.InvariantCulture)});

                var usersRole = roles.First(x => x.Value == roleIdAsString);
                usersRole.Selected = true;

                // set a dynamic property for the view (viewbag) with the roles to make them accessible in the view
                ViewBag.RoleOptions = roles;

                string companyIdAsString = user.CompanyId.ToString(CultureInfo.InvariantCulture);
                // Get a list of roles, but as SelectListItems (selected, text, value)
                var companies =
                    _usersContext.Companies.ToList().Select(
                        x => new SelectListItem() { Selected = false, Text = x.CompanyName, Value = x.CompanyId.ToString(CultureInfo.InvariantCulture) });

                var usersCompany = companies.First(x => x.Value == roleIdAsString);
                usersCompany.Selected = true;

                // set a dynamic property for the view (viewbag) with the roles to make them accessible in the view
                ViewBag.CompanyOptions = companies;

                return View(user);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(User user) //int userId, string userName, string password, string phoneNumber, string userEmailAddress
        {
            var existingUser = _usersContext.Users.First(x => x.UserId == user.UserId);
            existingUser.UserName = user.UserName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.EmailAddress = user.EmailAddress;
            existingUser.RoleId = user.RoleId;
            existingUser.CompanyId = user.CompanyId;

            // this is a 'trick' to set all the properties of an entity object at once. Add it, change it's state to modified
            //_usersContext.AddObject("Users", user);
            //_usersContext.ObjectStateManager.ChangeObjectState(user, EntityState.Modified);
            _usersContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var user = _usersContext.Users.FirstOrDefault(x => x.UserId == id);
            if(user != null)
            {
                return View("Details", user); // view name is not required, it defauls to the same name as this method
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var user = _usersContext.Users.FirstOrDefault(x => x.UserId == id);
            if (user != null)
            {
                return View(user);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete_Post(int id) //int userId, string userName, string password, string phoneNumber, string userEmailAddress
        {
            var existingUser = _usersContext.Users.FirstOrDefault(x => x.UserId == id);
            _usersContext.DeleteObject(existingUser);
            _usersContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            //return View(new User());
            return RedirectToAction("Register", "Account");
        }

    }

    public static class ListProvider
    {
        public static List<SelectListItem> Roles = new List<SelectListItem>
                                                   {
                                                       new SelectListItem { Text = "Vendita Admin", Value = "0" },
                                                       new SelectListItem { Text = "Company Admin", Value = "1" },
                                                       new SelectListItem { Text = "Company User", Value = "2" }
                                                   };

        public static List<SelectListItem> GetRoles(short roleId)
        {
            Roles.ForEach(r => r.Selected = false);
            var role = Roles.Single(r => r.Value == roleId.ToString(CultureInfo.InvariantCulture));
            role.Selected = true;
            return Roles;
        }
    }
}
