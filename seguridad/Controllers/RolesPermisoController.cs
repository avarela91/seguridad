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
    public class RolesPermisoController : Controller
    {
        ContextRolPermiso DB_roles_permiso = new ContextRolPermiso();
        PermisoContext db_Permiso = new PermisoContext();
        ContextRol DB_Rol = new ContextRol();

        //
        // GET: /RolesPermiso/Create
        [CustomAuthorize("003")]
        public ActionResult Create(int id)
        {
            List<Rol> Rol = DB_Rol.Select(
                    new Dictionary<string, string> { { "RoleId",id.ToString() } }
                );

            List<Permiso> PermisosDisponibles = DB_roles_permiso.permisosDisponiblesByRoles(id, Rol[0].Modulo_Id);

            //permisos seleccionados
            List<Permiso> PermisosSeleccionados = DB_roles_permiso.permisosSeleccionadosByRoles(id);

            MultiSelectList ListPermisosDisponibles = new MultiSelectList(PermisosDisponibles, "Permiso_Id", "Nombre");
            MultiSelectList ListPermisosSeleccionados = new MultiSelectList(PermisosSeleccionados, "Permiso_Id", "Nombre");
            ViewData["ListPermisos"] = ListPermisosDisponibles;
            ViewData["ListpermisosSeleccionados"] = ListPermisosSeleccionados;

            ViewData["permisosSeleccionados"] = PermisosSeleccionados;

            return View(Rol[0]);
        }

        //
        // POST: /RolesPermiso/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize("003")]
        public ActionResult Create(int[] permisosSeleccionados,int RoleId,int Modulo_Id)
        {
            var Username = User.Identity.Name;
            try
            {

                //eliminar los permisos que tenia y ya no estan
                DB_roles_permiso.EliminarPermisosQuitados(permisosSeleccionados, RoleId, Modulo_Id);
                //agregar permisos y verificar que no estban ingresados
                if (permisosSeleccionados != null) {
                    foreach(int Permiso_Id in permisosSeleccionados){
                        if (!DB_roles_permiso.Existe(RoleId, Permiso_Id)){
                            RolPermiso Roles_Permiso = new RolPermiso();
                            Roles_Permiso.Permiso_Id = Permiso_Id;
                            Roles_Permiso.RoleId = RoleId;
                            DB_roles_permiso.Insert(Roles_Permiso, Username);
                        }
                    }
                }
                return RedirectToAction("Index",new { Controller="Roles",id= Modulo_Id });
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("error", ex.Message);
            }

            List<Rol> Rol = DB_Rol.Select(
                    new Dictionary<string, string> { { "RoleId", RoleId.ToString() } }
                );

            List<Permiso> PermisosDisponibles = DB_roles_permiso.permisosDisponiblesByRoles(RoleId, Rol[0].Modulo_Id);

            //permisos seleccionados
            List<Permiso> PermisosSeleccionados = DB_roles_permiso.permisosSeleccionadosByRoles(RoleId);

            MultiSelectList ListPermisosDisponibles = new MultiSelectList(PermisosDisponibles, "Permiso_Id", "Nombre");
            MultiSelectList ListPermisosSeleccionados = new MultiSelectList(PermisosSeleccionados, "Permiso_Id", "Nombre");
            ViewData["ListPermisos"] = ListPermisosDisponibles;
            ViewData["ListpermisosSeleccionados"] = ListPermisosSeleccionados;

            ViewData["permisosSeleccionados"] = PermisosSeleccionados;

            return View(Rol[0]);
        }
    }
}
