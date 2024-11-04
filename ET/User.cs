using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace ET
{
    public class ContextUsuario:models<User>
    {
        public ContextUsuario()
           : base("Id_Usuario", "User", "DefaultConnection")
        {
            ActivarRegistrodeUsuarioEnTransaccion = true;
            ActivarSoloRegistrosActivos = true;
        }
    }

    [Serializable()]
    public class User : IClonable
    {
        public int Id_User { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Nombre")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Usuario")]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Contraseña")]
        public string Password { get; set; }
        
        public string Create_User { get; set; }
        public DateTime Create_Date { get; set; }
        public string Modify_User { get; set; }
        public DateTime Modify_Date { get; set; }
        public bool Active { get; set; }
        public string Telefono { get; set; }
        [DisplayName("Contacto del Supervisor")]
        public string Supervisor { get; set; }
        public object Clone()
        {
            return Utiles.Copia(this);
        }

    }
}
