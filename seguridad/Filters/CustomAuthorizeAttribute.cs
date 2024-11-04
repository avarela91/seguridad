using ET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace seguridad.Filters
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedPermisos;

        public CustomAuthorizeAttribute(params string[] permisos)
        {
            this.allowedPermisos = permisos;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            
            bool authorize = false;
            foreach (string allowedPermiso in allowedPermisos)
            {
                foreach (PermisoUser permiso in (httpContext.Session["PermisosUser"] as List<PermisoUser>))
                {
                    if (permiso.Codigo == allowedPermiso)
                        authorize = true;
                }
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }

    public class FilterAuth {
        public static bool Authorize_User(string allowedPermiso)
        {

            bool authorize = false;
                foreach (PermisoUser permiso in (HttpContext.Current.Session["PermisosUser"] as List<PermisoUser>))
                {
                    if (permiso.Codigo == allowedPermiso)
                        authorize = true;
                }
            return authorize;
        }
    }
}