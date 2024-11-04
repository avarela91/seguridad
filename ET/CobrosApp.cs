using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace ET
{
    public class ContextCobrosApp : models<CobrosApp>
    {
        public ContextCobrosApp()
          : base("Id_Pago", "CobrosApp", "Cobros")
        {
            ActivarSoloRegistrosActivos = true;
            ActivarRegistrodeUsuarioEnTransaccion = true;
        }             
        
        
        public int addCobro(string Id_Pago, string Id_Prestamo,int Id_Cliente, string Cliente, string Grupo, int GrupoId, string FechaPago, decimal Monto, decimal ValorCuotaActual, string BaseDatos, int Id_PrestamoEstado, string Comentario, string CreateUser, string CreateDateTime, int Activo, int BD, int PagoPenalizado, string TipoCobro) // Guarda el cobro
        {
            string query = "exec ps_IEApp_SincronizarCobro @Id_Pago= '" + Id_Pago + "', @Id_Prestamo='" + Id_Prestamo + "', @Id_Cliente=" + Id_Cliente + ", @Cliente='" + Cliente + "',@Grupo='" + Grupo + "', @GrupoId=" + GrupoId + ", @FechaPago='" + DateTime.ParseExact(FechaPago, "yyyy-MM-dd", CultureInfo.InvariantCulture) + "', @Monto=" + Monto + ",@ValorCuotaActual= "+ValorCuotaActual+ ",@BaseDatos='"+BaseDatos+ "', @Id_PrestamoEstado="+Id_PrestamoEstado+ ",@Comentario='"+Comentario+ "',@CreateUser='"+CreateUser+ "',@CreateDateTime='"+ DateTime.ParseExact(CreateDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)+ "', @Activo="+Convert.ToBoolean(Activo) + ", @BD="+BD + ",@PagoPenalizado= " + Convert.ToBoolean(PagoPenalizado) + ",@TipoCobro= '" + TipoCobro+"'";


            return QueryRaw(query);

        }
    }

    [Serializable()]
    public class CobrosApp
    {
        public int Id { get; set; }
        public string Id_Pago { get; set; }
        public string Id_Prestamo { get; set; }
        public int Id_Cliente { get; set; }
        public string Cliente { get; set; }
        public string Grupo { get; set; }
        public int GrupoId { get; set; }
        public string FechaPago { get; set; }
        public decimal Monto { get; set; }
        public decimal ValorCuotaActual { get; set; }
        public string BaseDatos { get; set; }
        public int Id_PrestamoEstado { get; set; }
        public string Comentario { get; set; }
        public int Sincronizado { get; set; }
        public string CreateUser { get; set; }
        public string CreateDateTime { get; set; }
        public string CreateDate { get; set; }
        public int Activo { get; set; }
    }
}
