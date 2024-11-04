using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace ET
{
    public class ContextRol : models<Rol>
    {
        public ContextRol()
            : base("RoleId", "Rol")
        {
            ActivarSoloRegistrosActivos = true;
            ActivarRegistrodeUsuarioEnTransaccion = true;
        }
        public bool RolesAccesoTotal(string modulo)
        {
            if (modulo == "seguridad")
            {
                string query = "select count(*) cantidad from Rol where AccesoTotal=1 and Active=1 and Modulo_Id =(select Modulo_Id from Modulo WHERE Codigo='" + modulo + "')";

                List<RolesCount> list = SelectRaw<RolesCount>(query);
                bool NoEliminarRol = false;
                if (list.Count > 0)
                {
                    if (list[0].cantidad > 1)
                        NoEliminarRol = true;
                }

                return NoEliminarRol;
            }
            else
                return true;
        }
    }

    [Serializable()]
    public class Rol : ICloneable
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool AccesoTotal { get; set; }
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
    public class RolesCount : ICloneable
    {
        public int cantidad { get; set; }

        public object Clone()
        {
            return Utiles.Copia(this);
        }
    }
}
