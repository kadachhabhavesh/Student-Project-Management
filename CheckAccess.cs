﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace StudentProjectManagement
{
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext.HttpContext.Session.GetString("Token") == null && filterContext.HttpContext.Session.GetString("UserId") == null)
            {
                filterContext.Result = new RedirectToActionResult("Login", "User", new { area = "" });
            }
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Expires"] = "-1";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            base.OnResultExecuting(context);
        }
    }
}
