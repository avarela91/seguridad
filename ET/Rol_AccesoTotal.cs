using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace ET
{
    public class COntextRol_AccesoTotal : models<Rol_AccesoTotal>
    {
        public COntextRol_AccesoTotal()
            : base("Rol_AccesoTotal_Id", "Rol_AccesoTotal")
        {
        }
    }

    [Serializable()]
    public class Rol_AccesoTotal : ICloneable
    {
       public int RoleId{get;set;}
       public string Create_User {get;set;}
      public DateTime Create_Date {get;set;}
      public string Modify_User {get;set;}
      public DateTime Modify_Date {get;set;}
      public bool Activo {get;set;}

      public object Clone()
      {
          return Utiles.Copia(this);
      }
    }

}
