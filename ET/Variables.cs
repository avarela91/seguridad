using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
namespace ET
{
    public class ContextVariables : models<Variables>
    {
        static Variables Variables = new Variables();
        public ContextVariables()
            : base("Variables_Id", "Variables")
        {
        }

        public void SUM(){
            string consulta="update Variables set Valor=Valor+1 where Variable_Id='CountLogin';";

            QueryRaw(consulta);
        }
    }

    [Serializable()]
    public class Variables : ICloneable
    {
        public string Variables_Id { get; set; }
        public string Descripcion { get; set; }
        public string Categoria_Id { get; set; }


        public object Clone()
        {
            return Utiles.Copia(this);
        }
    }
}
