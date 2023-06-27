using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace 浪愛有家.Controllers
{
    public class Admin_LoginCheck : ActionFilterAttribute
    {
        void LoginState(HttpContext context)
        {
            if (context.Session["admin"] == null)
            {
                context.Response.Redirect("/Manager/Manager_Login");
            }
            
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            HttpContext context = HttpContext.Current;
            LoginState(context);
        }
    }
}