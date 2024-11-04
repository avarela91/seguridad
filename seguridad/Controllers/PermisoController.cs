using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ET;
using seguridad.Filters;
namespace seguridad.Controllers
{
    [Authorize]
    public class PermisoController : Controller
    {
        PermisoContext permisoContext = new PermisoContext();

        //
        // GET: /Permiso/
        [CustomAuthorize("002")]
        public ActionResult Index(int id)
        {
            List<Permiso> Permisos= permisoContext.Select(
                new Dictionary<string, string> { {"Modulo_Id",id.ToString()} }
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
            return View(Permisos);
        }

        //
        // GET: /Permiso/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Permiso/Create

        public ActionResult Create(int id)
        {
            Permiso permiso = new Permiso();
            permiso.Modulo_Id = id;
            return View(permiso);
        }

        //
        // POST: /Permiso/Create

        [HttpPost]
        public ActionResult Create(Permiso Permiso)
        {
            var UserName = User.Identity.Name;
            try
            {
                if (ModelState.IsValid)
                {
                    permisoContext.Insert(Permiso, UserName);
                    TempData["Message"] = "Creado Correctamente";

                    return RedirectToAction("Index",new { id= Permiso.Modulo_Id });
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
            }

            return View();
        }

        //
        // GET: /Permiso/Edit/5

        public ActionResult Edit(int id)
        {
            List<Permiso> Permisos = permisoContext.Select(
                    new Dictionary<string, string> { {"Permiso_Id", id.ToString()} }
                );
            return View(Permisos[0]);
        }

        //
        // POST: /Permiso/Edit/5

        [HttpPost]
        public ActionResult Edit(Permiso Permiso)
        {
            var UserName = User.Identity.Name;
            try
            {
                if (ModelState.IsValid){
                    permisoContext.Update(Permiso, UserName);
                    TempData["Message"] = "Modificado Correctamente";
                    return RedirectToAction("Index",new { id=Permiso.Modulo_Id});
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
            }

            return View();
        }

        
        public ActionResult Delete(int id)
        {
            var UserName = User.Identity.Name;
            List<Permiso> Permisos = permisoContext.Select(
                    new Dictionary<string, string> { { "Permiso_Id", id.ToString() } }
                );
            Permisos[0].Active = false;
            try
            {
                if (ModelState.IsValid)
                {
                    permisoContext.Update(Permisos[0], UserName);
                    TempData["Message"] = "Eliminado Correctamente";
                    return RedirectToAction("Index", new { id = Permisos[0].Modulo_Id });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message.ToString());
            }

            return View();
        }
    }
}
