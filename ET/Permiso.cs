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
    public class PermisoContext:models<Permiso>
    {
        public PermisoContext() : base("Permiso_Id", "Permiso", "DefaultConnection") {
            ActivarSoloRegistrosActivos = true;
            ActivarRegistrodeUsuarioEnTransaccion = true;
        }

        public List<PermisoUser> PermisosByUserModulo(string UserName, string Modulo){
            string query = "PermisosByUserModulo '"+ UserName + "','"+ Modulo + "' ";
            return SelectRaw<PermisoUser>(query);
        }
    }

    [Serializable()]
    public class Permiso : ICloneable
    {
        public int Permiso_Id { get; set; }

        [Required]
        [Display(Name = "Código")]
        public string Codigo { get; set; }
        [Required]
        [Display(Name = "Descripcion")]
        public string Nombre { get; set; }
        [Required]
        [Display(Name = "Modulo")]
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
    public class PermisoUser : ICloneable
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }

        public object Clone()
        {
            return Utiles.Copia(this);
        }
    }
}
