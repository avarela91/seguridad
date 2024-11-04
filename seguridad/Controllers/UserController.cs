using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ET;
using seguridad.Filters;

namespace seguridad.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        ContextUsuario DB_Users = new ContextUsuario();
        // GET: User
        [CustomAuthorize("010")]
        public ActionResult Index()
        {

            var users = DB_Users.Select().OrderBy(x=>x.Name);
            //Dictionary<string, string> User_Rol = new Dictionary<string, string>();
            //foreach (User user in users)
            //{
            //    string[] Rols = Roles.GetRolesForUser(user.UserName);
            //    if (Rols.Count() > 0)
            //        User_Rol.Add(user.UserName, Rols[0]);
            //}

            //ViewData["Rol"] = User_Rol;
            return View(users);
        }

        [CustomAuthorize("010")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User model)
        {
                // Intento de registrar al usuario
                try
                {
                    if (ModelState.IsValid)
                    {
                        var users = DB_Users.Select();
                        var x = 0;
                        foreach(User user in users)
                        {
                            if (user.UserName == model.UserName)
                            {
                                x++;
                            }
                        }
                        if (x > 0)
                        {
                            ModelState.AddModelError("", "El nombre del usuario '" + model.UserName + "' ya existe");
                        }
                        else
                        {
                            model.Active = true;
                            object idUser = DB_Users.Insert(model, User.Identity.Name);
                            TempData["Message"] = "Guardado Correctamente";
                            return RedirectToAction("Index", "User");
                        }
                    }
                        
                }
                catch (Exception e)
                {
                        ModelState.AddModelError("", e.Message.ToString());
                }
            return View();
        }

        [CustomAuthorize("010")]
        public ActionResult Edit(int id)
        {
            var user = DB_Users.Select(new Dictionary<string, string>() { { "Id_User", id.ToString() } });
            return View(user[0]);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User model)
        {
            // Intento de registrar al usuario
            try
            {
                if (ModelState.IsValid)
                {
                        DB_Users.Update(model, User.Identity.Name);
                        TempData["Message"] = "Modificado Correctamente";
                        return RedirectToAction("Index", "User");
                }

            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message.ToString());
            }
            return View();
        }

        [HttpPost]
        [CustomAuthorize("010")]
        public ActionResult Delete(int id)
        {
            try
            {
                var user = DB_Users.Select(new Dictionary<string, string>() { { "Id_User", id.ToString() } });
                user[0].Active = false;
                DB_Users.Update(user[0], User.Identity.Name);
                TempData["Message"] = "Eliminado Correctamente";
                return RedirectToAction("Index", "User");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.Message);
                return View();
            }
        }

    }
}