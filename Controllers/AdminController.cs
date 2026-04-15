using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ResumeBuilder_1291763;
using ResumeBuilder_1291763.Models;
using ResumeBuilder_1291763.Models.ViewModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ResumeBuilder.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Helper to get UserManager
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        // GET: Admin/Index
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        // GET: Admin/ManageRoles/5
        public ActionResult ManageRoles(string id)
        {
            var user = UserManager.FindById(id);
            if (user == null) return HttpNotFound();

            // Get current role (assuming 1 role per user for simplicity)
            var currentRole = UserManager.GetRoles(user.Id).FirstOrDefault();

            var model = new UserRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                SelectedRole = currentRole,
                RoleList = db.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name,
                    Selected = r.Name == currentRole
                }).ToList()
            };

            return View(model);
        }

        // POST: Admin/ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageRoles(UserRoleViewModel model)
        {
            var user = UserManager.FindById(model.UserId);
            if (user == null) return HttpNotFound();

            // 1. Get existing roles
            var existingRoles = UserManager.GetRoles(user.Id);

            // 2. Remove them from existing roles
            if (existingRoles.Count > 0)
            {
                UserManager.RemoveFromRoles(user.Id, existingRoles.ToArray());
            }

            // 3. Add to the new selected role
            UserManager.AddToRole(user.Id, model.SelectedRole);

            TempData["Success"] = $"Role for {user.Email} updated to {model.SelectedRole}.";
            return RedirectToAction("Index");
        }
    }
}