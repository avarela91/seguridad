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
    public class ContextUsersInRoles : models<UsersInRoles>
    {
        public ContextUsersInRoles():base("UsersInRoles_Id", "UsersInRoles", "DefaultConnection")
        {
            ActivarSoloRegistrosActivos = true;
            ActivarRegistrodeUsuarioEnTransaccion = true;
        }

        public List<Rol> rolesSeleccionadosByRoles(int UserId, int Modulo_Id)
        {
            string query = "select rp.RoleId,p.RoleName,p.AccesoTotal,p.Modulo_Id,p.Active,p.Create_User,p.Create_Date,p.Modify_User,p.Modify_Date from UsersInRoles rp INNER JOIN Rol p on p.RoleId = rp.RoleId WHERE rp.Modulo_Id=" + Modulo_Id + " AND rp.Active=1 AND rp.UserId =" + UserId;
            return SelectRaw<Rol>(query);
        }
        public List<Rol> rolesDisponiblesByRoles(int UserId, int Modulo_Id)
        {
            string query = "select " +
                            "RoleId," +
                            "RoleName," +
                            "AccesoTotal," +
                            "Modulo_Id," +
                            "Active," +
                            "Create_User," +
                            "Create_Date," +
                            "Modify_User," +
                            "Modify_Date" +
                            " from " +
                            " Rol " +
                            " where Active=1 and Modulo_Id = " + Modulo_Id + " and RoleId not in( select ur.RoleId from UsersInRoles ur where ur.UserId =" + UserId + " and Modulo_Id = " + Modulo_Id + "   and Active = 1);";

            return SelectRaw<Rol>(query);
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
                ClausulaIn = " AND RoleId not in(" + ClausulaIn + ") ";
            }
            string query = "update UsersInRoles set Active=0,Modify_User='sistema',Modify_Date=GETDATE() where Modulo_Id=" + Modulo_Id + " AND UserId=" + UserId + " AND RoleId in (select RoleId from Rol where Modulo_Id=" + Modulo_Id + ") "+ClausulaIn;

            QueryRaw(query);
        }
        public bool Existe(int UserId, int RoleId, int Modulo_Id)
        {
            string query = "select count(*) 'count' from UsersInRoles where Modulo_Id=" + Modulo_Id + " AND active=1 and UserId=" + UserId + " AND RoleId=" + RoleId;
            List<Existe> Existe = SelectRaw<Existe>(query);
            bool existe = false;
            if (Existe.Count > 0)
            {
                if (Existe[0].count > 0)
                    existe = true;
            }

            return existe;
        }

        public bool ContieneRolAccesoTotal(int[] PermisosSeleccionados, int UserId, int Modulo_Id,string Modulo)
        {
            if (Modulo == "seguridad")
            {
                //variable que indica si actualmente hay seleccionado un rol con acceso total
                bool contieneRolAccesoTotalSeleccionado = false;

                string query = "select count(*) 'count' from UsersInRoles ur inner join Modulo m on ur.Modulo_Id = m.Modulo_Id inner join Rol r on r.RoleId = ur.RoleId where r.AccesoTotal = 1 and ur.Active = 1 and ur.Modulo_Id = '" + Modulo_Id + "' and ur.UserId =" + UserId + " and r.Active = 1";
                List<RolesAccesoTotalModulo> Roles = SelectRaw<RolesAccesoTotalModulo>(query);
                bool contieneRolAccesoTotal = false;
                if (Roles.Count > 0)
                {
                    if (Roles[0].count > 0)
                    {
                        contieneRolAccesoTotal = true;
                    }
                }

                if (contieneRolAccesoTotal)
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

                        string query2 = "select count(*) 'count' from Rol where AccesoTotal=1 AND RoleId in ( " + ClausulaIn + ");";
                        List<RolesAccesoTotalModulo> RolesSeleccionados = SelectRaw<RolesAccesoTotalModulo>(query2);


                        if (Roles.Count > 0)
                        {
                            if (Roles[0].count > 0)
                            {
                                contieneRolAccesoTotalSeleccionado = true;
                            }
                        }

                    }

                }
                else { contieneRolAccesoTotalSeleccionado = true; }
                return contieneRolAccesoTotalSeleccionado;
            }
            else{
                return true;
            }
        }

        public bool LimitUsersRolAccesoTotal(string Modulo){
            if (Modulo == "seguridad") {
                string query = "select count(*) 'count' from UsersInRoles ur inner join Modulo m on ur.Modulo_Id = m.Modulo_Id inner join Rol r on r.RoleId = ur.RoleId where r.AccesoTotal = 1 and ur.Active = 1 and m.Codigo='" + Modulo + "' and r.Active = 1";
                List<RolesAccesoTotalModulo> limitUsersRolAccesoTotal =SelectRaw<RolesAccesoTotalModulo>(query);
                bool limite = false;
                if (limitUsersRolAccesoTotal.Count > 0)
                {
                    if (limitUsersRolAccesoTotal[0].count > 1)
                        limite = true;
                }
                //se puede eliminar rol de acceso total de un usuario
                return limite;
            } else{
                return true;
            }
        }
    }

    [Serializable()]
    public class UsersInRoles : ICloneable
    {
        public int UsersInRoles_Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
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

    [Serializable()]
    public class RolesAccesoTotalModulo : ICloneable
    {
        public int count { set; get; }

        public object Clone()
        {
            return Utiles.Copia(this);
        }
    }
}
