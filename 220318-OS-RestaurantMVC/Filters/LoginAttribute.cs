using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace _220318_OS_RestaurantMVC.Filters
{
    public class LoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var email = context.HttpContext.Session.GetString("KullaniciEmail");
            if (string.IsNullOrEmpty(email))
            {
                email = context.HttpContext.Request.Cookies["KullaniciEmail"];
                if (string.IsNullOrEmpty(email))
                {
                    context.Result = new RedirectToActionResult("Index", "Login", new { area = "Admin" });

                }
            }
        }
    }
}
