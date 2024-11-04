using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ET;
using seguridad.Models;
using seguridad.Filters;
namespace seguridad.Controllers
{
    [Authorize]
    public class UsersPermisoController : Controller
    {
        ContextUsersInPermisos DB_Userspermiso = new ContextUsersInPermisos();
        ContextUsersInRoles db_webpages_Users_Roles = new ContextUsersInRoles();
        PermisoContext db_Permiso = new PermisoContext();
        ModuloContext db_Modulo = new ModuloContext();
        ModuloContext moduloContext = new ModuloContext();
        ContextUsuario DB_Users = new ContextUsuario();

        //
        // GET: /Modulo/
        [CustomAuthorize("004")]
        public ActionResult Modulo(int id)
        {
            List<Modulo2> Modulos;
            if (FilterAuth.Authorize_User("seguridad"))
            {
                Modulos = moduloContext.getModulos(id);
            }
            else {
                Modulos = moduloContext.getModulos(id, "'seguridad'");
            }

            var users = DB_Users.Select(new Dictionary<string, string>() { { "Id_User",id.ToString() } });
            string UserName = users[0].Name;


            if (TempData["Message"] != null)
            {
                ViewData["Message"] = TempData["Message"];
            }
            else
            {
                ViewData["Message"] = null;
            }

            ViewData["UserId"] = id;
            ViewData["UserName"] = UserName;


            return View(Modulos);
        }

        //
        // GET: /RolesPermiso/Create
        [CustomAuthorize("004")]
        public ActionResult Create(int UserId,int Modulo_Id)
        {
            List<Modulo> Modulo = db_Modulo.Select(
                    new Dictionary<string, string> { { "Modulo_Id", Modulo_Id.ToString() } }
                );
            var users = DB_Users.Select(new Dictionary<string, string>() { { "Id_User", UserId.ToString() } });
            string UserName = users[0].Name;
            ViewData["UserId"] = UserId;
            ViewData["UserName"] = UserName;

            //-----------------------Permisos------------------------------------
            List<Rol> RolesDisponibles = db_webpages_Users_Roles.rolesDisponiblesByRoles(UserId, Modulo_Id);
            //permisos seleccionados
            List<Rol> RolesSeleccionados = db_webpages_Users_Roles.rolesSeleccionadosByRoles(UserId, Modulo_Id);

            MultiSelectList ListRolesDisponibles = new MultiSelectList(RolesDisponibles, "RoleId", "RoleName");
            MultiSelectList ListRolesSeleccionados = new MultiSelectList(RolesSeleccionados, "RoleId", "RoleName");
            ViewData["ListRoles"] = ListRolesDisponibles;
            ViewData["ListRolesSeleccionados"] = ListRolesSeleccionados;
            ViewData["rolesSeleccionados"] = RolesSeleccionados;
            //-----------------------Permisos------------------------------------

            //-----------------------Permisos------------------------------------
            List<Permiso> PermisosDisponibles = DB_Userspermiso.permisosDisponiblesByRoles(UserId, Modulo_Id);
            //permisos seleccionados
            List<Permiso> PermisosSeleccionados = DB_Userspermiso.permisosSeleccionadosByRoles(UserId, Modulo_Id);

            MultiSelectList ListPermisosDisponibles = new MultiSelectList(PermisosDisponibles, "Permiso_Id", "Nombre");
            MultiSelectList ListPermisosSeleccionados = new MultiSelectList(PermisosSeleccionados, "Permiso_Id", "Nombre");
            ViewData["ListPermisos"] = ListPermisosDisponibles;
            ViewData["ListpermisosSeleccionados"] = ListPermisosSeleccionados;
            ViewData["permisosSeleccionados"] = PermisosSeleccionados;
            //-----------------------Permisos------------------------------------


            return View(Modulo[0]);
        }

        //
        // POST: /RolesPermiso/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize("004")]
        public ActionResult Create(int[] permisosSeleccionados, int[] rolesSeleccionados, int UserId, int Modulo_Id)
        {
            var Username = User.Identity.Name;
            string Codigo_Modulo = "";
            var Modulos = db_Modulo.Select(
                    new Dictionary<string, string> { { "Modulo_Id", Modulo_Id.ToString() } }
                );
            if (Modulos.Count > 0)
                Codigo_Modulo = Modulos[0].Codigo;

            try
            {
                bool PuedeQuitarRoles = db_webpages_Users_Roles.ContieneRolAccesoTotal(rolesSeleccionados, UserId, Modulo_Id, Codigo_Modulo);
                if (PuedeQuitarRoles)
                {
                    //------------- Roles -------------------------------------------------------------------
                    //eliminar los permisos que tenia y ya no estan
                    db_webpages_Users_Roles.EliminarPermisosQuitados(rolesSeleccionados, UserId, Modulo_Id);
                    //agregar permisos y verificar que no estban ingresados
                    if (rolesSeleccionados != null)
                    {
                        foreach (int RoleId in rolesSeleccionados)
                        {
                            if (!db_webpages_Users_Roles.Existe(UserId, RoleId, Modulo_Id))
                            {
                                UsersInRoles User_Roles = new UsersInRoles();
                                User_Roles.RoleId = RoleId;
                                User_Roles.UserId = UserId;
                                User_Roles.Modulo_Id = Modulo_Id;
                                db_webpages_Users_Roles.Insert(User_Roles, Username);
                            }
                        }
                    }
                }
                else{
                    TempData["Message"] = "No se puede eliminar Rol, Es obligatorio que al menos un usuario con rol Acceso Total";
                }
                //------------- Roles -------------------------------------------------------------------

                //------------- permisos -------------------------------------------------------------------
                //eliminar los permisos que tenia y ya no estan
                DB_Userspermiso.EliminarPermisosQuitados(permisosSeleccionados, UserId, Modulo_Id);
                //agregar permisos y verificar que no estban ingresados
                if (permisosSeleccionados != null) {
                    foreach(int Permiso_Id in permisosSeleccionados){
                        if (!DB_Userspermiso.Existe(UserId, Permiso_Id, Modulo_Id)){
                            UsersInPermisos User_Permiso = new UsersInPermisos();
                            User_Permiso.Permiso_Id = Permiso_Id;
                            User_Permiso.UserId = UserId;
                            User_Permiso.Modulo_Id = Modulo_Id;
                            DB_Userspermiso.Insert(User_Permiso, Username);
                        }
                    }
                }

                //------------- permisos -------------------------------------------------------------------
                if(PuedeQuitarRoles)
                    TempData["Message"] = "Guardado correctamente";

                return RedirectToAction("Modulo",new { Controller="UsersPermiso",id= UserId });
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("error", ex.Message);
            }

            List<Modulo> Modulo = db_Modulo.Select(
                    new Dictionary<string, string> { { "Modulo_Id", Modulo_Id.ToString() } }
                );

            var users = DB_Users.Select(new Dictionary<string, string>() { { "Id_User", UserId.ToString() } });
            string UserName = users[0].Name;
            ViewData["UserId"] = UserId;
            ViewData["UserName"] = UserName;

            //-----------------------Permisos------------------------------------
            List<Rol> RolesDisponibles = db_webpages_Users_Roles.rolesDisponiblesByRoles(UserId, Modulo_Id);
            //permisos seleccionados
            List<Rol> RolesSeleccionados = db_webpages_Users_Roles.rolesSeleccionadosByRoles(UserId, Modulo_Id);

            MultiSelectList ListRolesDisponibles = new MultiSelectList(RolesDisponibles, "RoleId", "RoleName");
            MultiSelectList ListRolesSeleccionados = new MultiSelectList(RolesSeleccionados, "RoleId", "RoleName");
            ViewData["ListRoles"] = ListRolesDisponibles;
            ViewData["ListRolesSeleccionados"] = ListRolesSeleccionados;
            ViewData["rolesSeleccionados"] = RolesSeleccionados;
            //-----------------------Permisos------------------------------------

            //-----------------------Permisos------------------------------------
            List<Permiso> PermisosDisponibles = DB_Userspermiso.permisosDisponiblesByRoles(UserId, Modulo_Id);
            //permisos seleccionados
            List<Permiso> PermisosSeleccionados = DB_Userspermiso.permisosSeleccionadosByRoles(UserId, Modulo_Id);

            MultiSelectList ListPermisosDisponibles = new MultiSelectList(PermisosDisponibles, "Permiso_Id", "Nombre");
            MultiSelectList ListPermisosSeleccionados = new MultiSelectList(PermisosSeleccionados, "Permiso_Id", "Nombre");
            ViewData["ListPermisos"] = ListPermisosDisponibles;
            ViewData["ListpermisosSeleccionados"] = ListPermisosSeleccionados;
            ViewData["permisosSeleccionados"] = PermisosSeleccionados;
            //-----------------------Permisos------------------------------------

            return View(Modulo[0]);
        }

    }
}
