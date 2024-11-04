using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace ET
{
    public class ContextRolPermiso : models<RolPermiso>
    {
        public ContextRolPermiso() : base("Rol_Permiso_Id", "RolPermiso","DefaultConnection")
        {
            ActivarSoloRegistrosActivos = true;
            ActivarRegistrodeUsuarioEnTransaccion = true;
        }

        public List<Permiso> permisosSeleccionadosByRoles(int RoleId)
        {
            //string query = "select rp.Permiso_Id,p.Codigo,p.Nombre,p.Modulo_Id,p.Activo,p.Create_User,p.Create_Date,p.Modify_User,p.Modify_Date from webpages_Roles_Permiso rp INNER JOIN Permiso p on p.Permiso_Id = rp.Permiso_Id where rp.Activo=1 AND rp.RoleId =" + RoleId;
            string query = "EXEC [dbo].[Seguridad_permisosSeleccionadosByRoles] @RolPermisoId = " + RoleId;
            return SelectRaw<Permiso>(query);
        }


        public List<Permiso> permisosDisponiblesByRoles(int RoleId, int Modulo_Id)
        {
            string query = "EXEC[dbo].[Seguridad_permisosDisponiblesByRoles] @RoleId =" + RoleId + ",@Modulo_Id =" + Modulo_Id;
            return SelectRaw<Permiso>(query);
        }

        public void EliminarPermisosQuitados(int[] PermisosSeleccionados, int RoleId, int Modulo_Id)
        {
            string ClausulaIn = "";
            if (PermisosSeleccionados != null)
            {
                foreach (int permiso in PermisosSeleccionados)
                {
                    ClausulaIn += permiso + ",";
                }
            }
            if (ClausulaIn.Length > 0)
            {
                ClausulaIn = ClausulaIn.Remove(ClausulaIn.Length - 1, 1);
                ClausulaIn = " AND Permiso_Id not in(" + ClausulaIn + ") ";
            }
            string query = "update RolPermiso set Active=0,Modify_User='sistema',Modify_Date=GETDATE() where RoleId=" + RoleId + " AND Permiso_Id in (select Permiso_Id from Permiso where Modulo_Id=" + Modulo_Id + ") ";

            QueryRaw(query);
        }

        public bool Existe(int RoleId, int Permiso_Id)
        {
            string query = "select count(*) 'count' from RolPermiso where active=1 and RoleId=" + RoleId + " AND Permiso_Id=" + Permiso_Id;
            List<Existe> Existe = SelectRaw<Existe>(query);
            bool existe = false;
            if (Existe.Count > 0)
            {
                if (Existe[0].count > 0)
                    existe = true;
            }

            return existe;
        }
    }
        
    [Serializable()]
    public class RolPermiso : ICloneable
    {
        public int Rol_Permiso_Id { set; get; }
        public int RoleId { set; get; }
        public int Permiso_Id { set; get; }
        public string Create_User { get; set; }
        public DateTime Create_Date { get; set; }
        public string Modify_User { get; set; }
        public DateTime Modify_Date { get; set; }
        public bool Active { get; set; }

        public object Clone()
        {
            return Utiles.Copia(this);
        }
    }

    [Serializable()]
    public class Existe : ICloneable
    {
        public int count { set; get; }

        public object Clone()
        {
            return Utiles.Copia(this);
        }
    }
}
