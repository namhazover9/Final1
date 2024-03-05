using FinalWeb1.DataAccess.Data;
using FinalWeb1.Models;
using FinalWeb1.Models.ViewModels;
using FinalWeb1.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FinalWeb1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId; // get the role id of the user

            RoleManagementVM RoleVM = new RoleManagementVM() // create a new RoleManagementVM object
            {
                ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userId), // 
                RoleList = _db.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name, // the text of the dropdown list will be the name of the role
                    Value = i.Name // the value of the dropdown list will be the name of the role
                })
            };

            RoleVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name; // get the role name of the user
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {

            string RoleID = _db.UserRoles.FirstOrDefault(u => u.UserId == roleManagementVM.ApplicationUser.Id).RoleId; // get the role id of the user
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id == RoleID).Name; // get the role name of the user

            if (!(roleManagementVM.ApplicationUser.Role == oldRole)) 
            {
                //a role was updated
                // get the user from the database
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementVM.ApplicationUser.Id);
                //_db.SaveChanges();
                // RemoveFromRoleAsync is a method of the UserManager class that removes the specified user from the named role
                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult(); // remove the user from the old role

                // AddToRoleAsync is a method of the UserManager class that adds the specified user to the named role
                _userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult(); // add the user to the new role

            }

            return RedirectToAction("Index");
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _db.ApplicationUsers.ToList();

            var userRoles = _db.UserRoles.ToList(); // get the user roles
            var roles = _db.Roles.ToList(); // get the roles

            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId; //get the role id of the user
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name; // get the role name of the user
            }
            return Json(new { data = objUserList });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id) //[FromBody] is a parameter attribute that tells the framework to get the value from the request body
        {

            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id); // get the user from the database
            
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            /* LockoutEnd is a property of the IdentityUser class that represents the end of the lockout period for the user.
            If the user is not locked out, this value is null. */
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now) // if the user is currently locked
            { 
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now; // set the lockout end to the current time
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(100); // lock the user out for 100 years
            }
            _db.SaveChanges();

            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion
    }
}
