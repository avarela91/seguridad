using ET;
using Newtonsoft.Json;
using seguridad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace seguridad.Controllers
{
    public class APIController : Controller
    {

        ContextUsuario DB_User = new ContextUsuario();
        PermisoContext db_permiso = new PermisoContext();

        [HttpPost]
        [AllowAnonymous]
        public JsonResult LoginApp(LoginModel model, string Modulo)
        {
            Dictionary<string, object> response = new Dictionary<string, object>();
            var jsonSerialiser = new JavaScriptSerializer();

            if (ModelState.IsValid)
            {
                //var users = UserDB.UserProfiles.Where(x => x.UserName == model.UserName).ToList();
                var userList = DB_User.Select(new Dictionary<string, string>() { { "UserName", model.UserName } });
                if (userList.Count > 0)
                {
                   
                    try
                    {
                        if((model.Password == userList[0].Password))
                        {
                            var permisos = db_permiso.PermisosByUserModulo(model.UserName, Modulo);
                            if (permisos.Count > 0)
                            {
                                response.Add("userId", userList[0].Id_User);
                                response.Add("permisos", permisos);
                                response.Add("login", true);
                                response.Add("error", "");
                                response.Add("telefono", userList[0].Telefono);
                                response.Add("supervisor", userList[0].Supervisor);
                                var json = jsonSerialiser.Serialize(response);

                                return Json(json);
                            }
                            else
                            {
                                response.Add("permisos", null);
                                response.Add("login", false);
                                response.Add("error", "No tiene permisos para acceder a esta aplicación");

                                var json3 = jsonSerialiser.Serialize(response);
                                return Json(json3);
                            }

                        }


                        
                    }
                    catch (Exception error)
                    {
                        //Response.Write(error.Message);
                        //ModelState.AddModelError("", "El nombre de usuario o la contraseña especificados son incorrectos.");
                    }
                }
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            response.Add("permisos", null);
            response.Add("login", false);
            response.Add("error", "El nombre de usuario o la contraseña especificados son incorrectos.");

            var json2 = jsonSerialiser.Serialize(response);
            return Json(json2);
        }


        [HttpPost]
        public JsonResult Update(int version, string package, string usuario)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var archivo = Server.MapPath("~/aplicacion/update.json");
            string outputJSON = System.IO.File.ReadAllText(archivo);

            List<update> ListUpdate = ser.Deserialize<List<update>>(outputJSON);

            updateResponse updateResponse = new updateResponse();
            updateResponse.package = package;
            updateResponse.updateavailable = false;
            updateResponse.version = version;
            updateResponse.url = "";

            

                foreach (update Update in ListUpdate.ToList())
                {
                    if (Update.version > version && Update.package == updateResponse.package)
                    {
                        updateResponse.updateavailable = true;
                        updateResponse.version = Update.version;
                        updateResponse.url = Update.url;
                    }
                }
            

            var jsonResponse = ser.Serialize(updateResponse);
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public FileResult Download() //descarga de la apk cuando hay una actualizacion
        {
            string ruta = "~/aplicacion/IEApp.apk";
            return File(ruta, "application/vnd.android.package-archive", "IEApp.apk");
        }

        [AllowAnonymous]
        public FileResult Downloadbd(string UserName)
        {   
                string ruta = "~/aplicacion/financiera.db";
                return File(ruta, "application/x-sqlite3", "financiera.db");
            
        }

        [AllowAnonymous]
        public FileResult Downloadbd2(string UserName)
        {
            string ruta = "~/aplicacion/financiera2.db";
            return File(ruta, "application/x-sqlite3", "financiera2.db");

        }

        [AllowAnonymous]
        public ViewResult app()
        {
            return View("app");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RecieveDB(HttpPostedFileBase file)
        {
            if (file!=null && file.ContentLength>0)
            {
                //file = Request.Files[0];
                file.SaveAs(Server.MapPath("/aplicacion/") +  file.FileName);
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult Sincronizar(string jsonData)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<respuesta> respuestaList = new List<respuesta>();
            List<jsonCobroToInsert> resultMaster = JsonConvert.DeserializeObject<List<jsonCobroToInsert>>(jsonData);


            ContextCobrosApp DB_Cobros = new ContextCobrosApp();

            foreach (jsonCobroToInsert item in resultMaster)
            {
                    try
                    {
                        int respMaster = DB_Cobros.addCobro(item.Id_Pago, item.Id_Prestamo, item.Id_Cliente, item.Cliente,item.Grupo,item.GrupoId,item.FechaPago,item.Monto,item.ValorCuotaActual,item.BaseDatos, item.Id_PrestamoEstado,item.Comentario,item.CreateUser,item.CreateDateTime,item.Activo, item.BD, item.PagoPenalizado, item.TipoCobro);

                        if (respMaster==-1)
                        {
                            respuestaList.Add(new respuesta(item.Id_Pago, true, "registro correcto"));
                        }

                    }
                    catch (Exception ex)
                    {
                        //retur.Add("response" + item.Codigo, "error: " + ex.Message);
                        respuestaList.Add(new respuesta(item.Id_Pago, false, "error:" + ex.Message));
                    }
                }
            var jsonResponse = ser.Serialize(respuestaList);
            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }
    }


    public class respuesta
    {
        public string idCobro { set; get; }
        public bool estado { set; get; }
        public string mensaje { set; get; }
        public respuesta(string idCobro, bool estado, string mensaje)
        {
            this.idCobro = idCobro;
            this.estado = estado;
            this.mensaje = mensaje;
        }
    }

    public class jsonCobroToInsert
    {
        public string Id_Pago { set; get; }
        public string Id_Prestamo { set; get; }
        public int Id_Cliente { set; get; }
        public string Cliente { set; get; }
        public string Grupo { set; get; }
        public int GrupoId { set; get; }
        public string FechaPago { set; get; }
        public decimal Monto { set; get; }
        public decimal ValorCuotaActual { set; get; }
        public string BaseDatos { set; get; }
        public int Id_PrestamoEstado { set; get; }
        public string Comentario { set; get; }
        public string CreateUser { set; get; }
        public string CreateDateTime { set; get; }
        public int Activo { set; get; }
        public int BD { set; get; }
        public int PagoPenalizado { set; get; }
        public string TipoCobro { set; get; }


    }

    public class update
    {
        public int version { set; get; }
        public string package { set; get; }
        public string url { set; get; }
    }
    public class updateResponse
    {
        public int version { set; get; }
        public string package { set; get; }
        public string url { set; get; }
        public bool updateavailable { set; get; }
    }
}

