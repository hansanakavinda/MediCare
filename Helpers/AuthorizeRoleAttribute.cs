using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MediCare.Helpers
{
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userRole = session.GetString("Role");
            var userId = session.GetInt32("UserId");

            if (userId == null || string.IsNullOrEmpty(userRole))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (_roles.Length > 0 && !_roles.Contains(userRole))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}