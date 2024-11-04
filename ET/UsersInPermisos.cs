using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL;

namespace ET
{
    public class ContextUsersInPermisos : models<UsersInPermisos>
    {
        public ContextUsersInPermisos():base("UsersInPermisos_Id", "UsersInPermisos","DefaultConnection")
        {
            ActivarSoloRegistrosActivos = true;
            ActivarRegistrodeUsuarioEnTransaccion = true;
        }

        public List<Permiso> permisosSeleccionadosByRoles(int UserId, int Modulo_Id)
        {
            string query = "select rp.Permiso_Id,p.Codigo,p.Nombre,p.Modulo_Id,p.Active,p.Create_User,p.Create_Date,p.Modify_User,p.Modify_Date from UsersInPermisos rp INNER JOIN Permiso p on p.Permiso_Id = rp.Permiso_Id where rp.Modulo_Id="+ Modulo_Id + " AND rp.Active=1 AND rp.UserId =" + UserId;
            return SelectRaw<Permiso>(query);
        }
        public List<Permiso> permisosDisponiblesByRoles(int UserId, int Modulo_Id)
        {
            string query = "select " +
                            "Permiso_Id," +
                            "Codigo," +
                            "Nombre," +
                            "Modulo_Id," +
                            "Active," +
                            "Create_User," +
                            "Create_Date," +
                            "Modify_User," +
                            "Modify_Date" +
                            " from " +
                            " Permiso " +
                            " where Active=1 and Modulo_Id = " + Modulo_Id + " and Permiso_Id not in( select ur.Permiso_Id from UsersInPermisos ur where ur.UserId =" + UserId + " and Modulo_Id = " + Modulo_Id + "   and Active = 1) AND Permiso_Id not in (select Permiso_Id from RolPermiso where Active=1 AND RoleId in (select RoleId from UsersInRoles where Active=1 AND UserId="+UserId+"));";

            return SelectRaw<Permiso>(query);
        }
        public void EliminarPermisosQuitados(int[] PermisosSeleccionados, int UserId, int Modulo_Id)
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
            string query = "update UsersInPermisos set Active=0,Modify_User='sistema',Modify_Date=GETDATE() where Modulo_Id=" + Modulo_Id + " AND UserId=" + UserId + " AND Permiso_Id in (select Permiso_Id from Permiso where Modulo_Id=" + Modulo_Id + ") ";

            QueryRaw(query);
        }
        public bool Existe(int UserId, int Permiso_Id, int Modulo_Id)
        {
            string query = "select count(*) 'count' from UsersInPermisos where Modulo_Id=" + Modulo_Id + " AND active=1 and UserId=" + UserId + " AND Permiso_Id=" + Permiso_Id;
            List<Existe> Existe = SelectRaw<Existe>(query);
            bool existe = false;
            if (Existe.Count > 0)
            {
                if(Existe[0].count > 0)
                    existe = true;
            }

            return existe;
        }
    }

    [Serializable()]
    public class UsersInPermisos : ICloneable
    {
        public int UsersInPermisos_Id { get; set; }
        public int UserId { get; set; }
        public int Permiso_Id { get; set; }
        public int Modulo_Id { get; set; }
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
}
