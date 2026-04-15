using System.Collections.Generic;
using System.Web.Mvc;

namespace ResumeBuilder_1291763.Models.ViewModel
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string SelectedRole { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}