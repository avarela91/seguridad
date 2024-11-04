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

    public class ModuloContext : models<Modulo>
    {
        public ModuloContext()
            : base("Modulo_Id", "Modulo")
        {
            ActivarSoloRegistrosActivos = true;
            ActivarRegistrodeUsuarioEnTransaccion = true;
        }

        public List<Modulo> selectFiltro(string excluirModulos = null)
        {
            string filtro = "";
            if (excluirModulos != null)
                filtro = " AND Codigo not in (" + excluirModulos + ")";

            string query = "SELECT * FROM[dbo].[Modulo] m WHERE m.Active=1 "+filtro;
            return SelectRaw<Modulo>(query);
        }

        public List<Modulo2> getModulos(int UserId,string excluirModulos=null) {
                string filtro="";
                if (excluirModulos != null)
                    filtro = " AND Codigo not in (" + excluirModulos + ")";

                string query = "SELECT m.Modulo_Id" +
                                  ", m.Codigo" +
                                  ",m.Nombre" +
                                  ",m.Active" +
                                  ",m.Create_User" +
                                  ",m.Create_Date" +
                                  ",m.Modify_User" +
                                  ",m.Modify_Date" +
                                  ",(SELECT count(*) FROM UsersInPermisos up WHERE up.Modulo_Id = m.Modulo_Id AND up.UserId ="+ UserId + ") UserTienePermisos " +
                                  ",(SELECT count(*) FROM UsersInRoles ur WHERE ur.Modulo_Id = m.Modulo_Id AND ur.UserId =" + UserId + ") UserTieneRoles " +
                                  " FROM[dbo].[Modulo] m WHERE m.Active=1 " + filtro;
                return SelectRaw<Modulo2>(query);
            }

        public string getModuloByRoleId(int RoleId)
        {

            string query = "select Codigo from Modulo m inner join Rol r on r.Modulo_Id=m.Modulo_Id where RoleId="+ RoleId;
            List<ModuloName> Modulos= SelectRaw<ModuloName>(query);
            string ModuloName = "";
            if (Modulos.Count > 0)
                ModuloName = Modulos[0].Codigo;

            return ModuloName;
        }

    }

    [Serializable()]
    public class Modulo :ICloneable
    {
        public int Modulo_Id { get; set; }
        [Required]
        [Display(Name = "Código")]
        public string Codigo { get; set; }
        [Required]
        [Display(Name = "Nombre del modulo")]
        public string Nombre { get; set; }
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
    public class Modulo2 : ICloneable
    {
        public int Modulo_Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public int UserTienePermisos { get; set; }
        public int UserTieneRoles { get; set; }
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
    public class ModuloName : ICloneable
    {
        public string Codigo { get; set; }

        public object Clone()
        {
            return Utiles.Copia(this);
        }
    }
}
