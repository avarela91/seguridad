using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using ET;
using seguridad.Filters;
namespace seguridad.Controllers
{
    [Authorize]
    public class RolesController : Controller
    {
        ContextRol DB_Rol = new ContextRol();
        private ModuloContext dbModulo = new ModuloContext();

        //
        // GET: /Roles/
        [CustomAuthorize("003")]
        public ActionResult Index(int id)
        {
            List<Rol> Roles = DB_Rol.Select(
                new Dictionary<string, string> { { "Modulo_Id", id.ToString() } }
                );
            if (TempData["Message"] != null)
            {
                ViewData["Message"] = TempData["Message"];
            }
            else
            {
                ViewData["Message"] = null;
            }
            ViewData["id"] = id;
            return View(Roles);
        }

        //
        // GET: /Roles/Details/5

        public ActionResult Details(int id = 0)
        {
            Rol webpages_roles = DB_Rol.Select(
                new Dictionary<string,string>{ {"RoleId",id.ToString()} }
                )[0];
            if (webpages_roles == null)
            {
                return HttpNotFound();
            }
            return View(webpages_roles);
        }

        //
        // GET: /Roles/Create

        public ActionResult Create(int id)
        {
            Rol role = new Rol();
            role.Modulo_Id = id;
            return View(role);
        }

        //
        // POST: /Roles/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Rol rol)
        {
            var UserName = User.Identity.Name;
            if (ModelState.IsValid)
            {
                DB_Rol.Insert(rol, UserName);
                TempData["Message"] = "Guardado Correctamente";
                return RedirectToAction("Index",new { id = rol.Modulo_Id});
            }

            return View(rol);
        }

        //
        // GET: /Roles/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Rol rol = DB_Rol.Select(
                new Dictionary<string, string> { { "RoleId", id.ToString() } }
                )[0];
            if (rol == null)
            {
                return HttpNotFound();
            }
            return View(rol);
        }

        //
        // POST: /Roles/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Rol rol)
        {
            var UserName = User.Identity.Name;
            if (ModelState.IsValid)
            {
                if (rol.AccesoTotal == false)
                {
                    if (DB_Rol.RolesAccesoTotal("seguridad"))
                    {
                        DB_Rol.Update(rol, UserName);
                        TempData["Message"] = "Modificado Correctamente";
                        return RedirectToAction("Index", new { id = rol.Modulo_Id });
                    }
                    else
                    {
                        ModelState.AddModelError("error", "No puede eliminar rol de acceso total, debe haber obligatoriamente un rol con acceso total");
                        rol.AccesoTotal = true;
                        return View(rol);
                    }
                }
                else {
                    DB_Rol.Update(rol, UserName);
                    TempData["Message"] = "Modificado Correctamente";
                    return RedirectToAction("Index", new { id = rol.Modulo_Id });
                }
            }
            return View(rol);
        }

        //
        // GET: /Roles/Delete/5

        public ActionResult Delete(int id = 0)
        {
            string ModuloName = "";
            ModuloName=dbModulo.getModuloByRoleId(id);

            Rol webpages_roles = DB_Rol.Select(
                    new Dictionary<string, string> { { "RoleId", id.ToString() } }
                    )[0];

            if (DB_Rol.RolesAccesoTotal(ModuloName))
            {
                webpages_roles.Active = false;
                var UserName = User.Identity.Name;
                DB_Rol.Update(webpages_roles, UserName);
                TempData["Message"] = "Eliminado Correctamente";
                return RedirectToAction("Index", new { id = webpages_roles.Modulo_Id });
            }
            else {
                ModelState.AddModelError("error", "No puede eliminar el rol, debe haber obligatoriamente un rol con acceso total");

                List<Rol> Roles = DB_Rol.Select(
                new Dictionary<string, string> { { "Modulo_Id", webpages_roles.Modulo_Id.ToString() } }
                );
                if (TempData["Message"] != null)
                {
                    ViewData["Message"] = TempData["Message"];
                }
                else
                {
                    ViewData["Message"] = null;
                }

                return View("Index", Roles);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
