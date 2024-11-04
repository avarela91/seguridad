using ET;
using seguridad.Filters;
using seguridad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace seguridad.Controllers
{
    public class AccountController : Controller
    {
        UsersContext UserDB = new UsersContext();
        ContextUsuario DB_User = new ContextUsuario();
        PermisoContext db_permiso = new PermisoContext();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Session["CategoriasUsuario"] != null)
            {
                Session["CategoriasUsuario"] = null;
                Session["AccesoTotal"] = null;
            }
            if (User.Identity.IsAuthenticated)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Account");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try{
                    var userList = DB_User.Select(new Dictionary<string, string>() { { "UserName",model.UserName} });
                    if (userList.Count > 0)
                    {
                        if ((model.Password == userList[0].Password))
                        {
                            Session["PermisosUser"] = db_permiso.PermisosByUserModulo(model.UserName, "seguridad");
                            var permisos = Session["PermisosUser"];
                            if (FilterAuth.Authorize_User("001"))
                                FormsAuthentication.RedirectFromLoginPage(model.UserName, false);
                            else
                            {
                                ModelState.AddModelError("", "Usuario sin permisos");
                                return View(model);
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    //Response.Write(error.Message);
                    ModelState.AddModelError("", "Ocurrio un error:\n"+error.Message.ToString());
                    }
                }
            

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            ModelState.AddModelError("", "El nombre de usuario o la contraseña especificados son incorrectos.");
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult SessionDesactive()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}