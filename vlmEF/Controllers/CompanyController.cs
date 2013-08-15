using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Data;

namespace vlmEF.Controllers
{
    [Authorize(Roles = "SuperAdmin")]  // Roles = "Role1, Role2, ..."  Putting the auth here protects all actions in the controller
    public class CompanyController : Controller
    {
        private readonly UsersContext _usersContext;

        public CompanyController()
        {
            _usersContext = new UsersContext();
        }


        public ActionResult Index()
        {
            return View(_usersContext.Companies.Where(u => u.CompanyId > 1).ToList());
        }

        //public JsonResult UpdateRole(int userId, short roleId)
        //{
        //    _usersContext.AddToUserRoles(new UserRole{ CompanyId = userId, RoleId = roleId});
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        // Alex was here
        public ActionResult Edit(int id)
        {
            var company = _usersContext.Companies.FirstOrDefault(x => x.CompanyId == id);
            if (company != null)
            {

                string roleIdAsString = company.CompanyId.ToString(CultureInfo.InvariantCulture);
                // Get a list of roles, but as SelectListItems (selected, text, value)
                var roles =
                    _usersContext.Roles.ToList().Select(
                        x => new SelectListItem() { Selected = false, Text = x.RoleDescription, Value = x.RoleId.ToString(CultureInfo.InvariantCulture) });

                var usersRole = roles.First(x => x.Value == roleIdAsString);
                usersRole.Selected = true;

                // set a dynamic property for the view (viewbag) with the roles to make them accessible in the view
                ViewBag.RoleOptions = roles;

                return View(company);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(Company company) //int userId, string userName, string password, string phoneNumber, string userEmailAddress
        {
            var existingCompany = _usersContext.Companies.First(x => x.CompanyId == company.CompanyId);
            existingCompany.CompanyName = company.CompanyName;
            existingCompany.CompanyAddress = company.CompanyAddress;
            existingCompany.CompanyCity = company.CompanyCity;
            existingCompany.CompanyState = company.CompanyState;
            existingCompany.CompanyZip = company.CompanyZip;

            // this is a 'trick' to set all the properties of an entity object at once. Add it, change it's state to modified
            //_usersContext.AddObject("Users", user);
            //_usersContext.ObjectStateManager.ChangeObjectState(user, EntityState.Modified);
            _usersContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var company = _usersContext.Companies.FirstOrDefault(x => x.CompanyId == id);
            if (company != null)
            {
                return View("Details", company); // view name is not required, it defauls to the same name as this method
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var company = _usersContext.Companies.FirstOrDefault(x => x.CompanyId == id);
            if (company != null)
            {
                return View(company);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult Delete_Post(int id) //int userId, string userName, string password, string phoneNumber, string userEmailAddress
        {
            var existingCompany = _usersContext.Companies.FirstOrDefault(x => x.CompanyId == id);
            _usersContext.DeleteObject(existingCompany);
            _usersContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View(new Company());
        }

        [HttpPost]
        public ActionResult Create(Company company)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                //var newCompany = _usersContext.Companies.CreateObject();
                _usersContext.AddObject("Companies", company);
                _usersContext.SaveChanges();
                //var createStatus = _usersContext.AddToCompanies(company);
                return RedirectToAction("Index");
                //if (createStatus == MembershipCreateStatus.Success)
                //{
                //    return RedirectToAction("LogOn", "Account");
                //}
                //ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index");
        }


    }

}
