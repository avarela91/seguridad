using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ET;
using seguridad.Filters;

namespace seguridad.Controllers
{
    
    public class ModuloController : Controller
    {
        ModuloContext moduloContext = new ModuloContext();

        //
        // GET: /Modulo/
        [CustomAuthorize("009")]
        public ActionResult Index()
        {
            List<Modulo> Modulos=moduloContext.Select(); ;
            //if (FilterAuth.Authorize_User("seguridad"))
            //{
            //    Modulos = moduloContext.Select();
            //}
            //else
            //{
            //    Modulos = moduloContext.selectFiltro("'seguridad'");
            //}

            if (TempData["Message"] != null)
            {
                ViewData["Message"] = TempData["Message"];
            }
            else
            {
                ViewData["Message"] = null;
            }

            return View(Modulos);
        }

        //
        // GET: /Modulo/Details/5
        [CustomAuthorize("009")]
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Modulo/Create
        [CustomAuthorize("005")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Modulo/Create

        [HttpPost]
        [CustomAuthorize("005")]
        public ActionResult Create(Modulo modulo)
        {
            var UserName = User.Identity.Name;
            try
            {
                if (ModelState.IsValid)
                {
                    moduloContext.Insert(modulo, UserName);
                    TempData["Message"] = "Creado Correctamente";

                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View();
        }

        //
        // GET: /Modulo/Edit/5
        [CustomAuthorize("007")]
        public ActionResult Edit(int id)
        {
            List<Modulo> Modulos = moduloContext.Select(
                    new Dictionary<string, string> { {"Modulo_Id", id.ToString()} }
                );
            return View(Modulos[0]);
        }

        //
        // POST: /Modulo/Edit/5

        [HttpPost]
        [CustomAuthorize("007")]
        public ActionResult Edit(Modulo modulo)
        {
            var UserName = User.Identity.Name;
            try
            {
                if (ModelState.IsValid){
                    moduloContext.Update(modulo, UserName);
                    TempData["Message"] = "Modificado Correctamente";
                    return RedirectToAction("Index");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("error", ex.Message);
            }

            return View();
        }

        [CustomAuthorize("006")]
        public ActionResult Delete(int id)
        {
           
            try
            {
                if (ModelState.IsValid)
                {
                    var Modulos = moduloContext.Select(new Dictionary<string, string> { { "Modulo_Id", id.ToString() } });
                    Modulos[0].Active = false;
                    moduloContext.Update(Modulos[0], User.Identity.Name);
                    TempData["Message"] = "Eliminado Correctamente";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.Message);
            }

            return View();
        }
    }
}
