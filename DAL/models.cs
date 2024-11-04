using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Configuration;
using System.Data;

namespace DAL
{
    public class models<T>
    {
        public string primary_key = "";
        public string table_name = "";
        public T Entidad= (T)Activator.CreateInstance(typeof(T));
        public string ConnectionStringsName = "";
        public bool ActivarSoloRegistrosActivos = false;
        public bool ActivarRegistrodeUsuarioEnTransaccion = false;

        public models(string primary_key, string table_name, string ConnectionStringsName = "DefaultConnection")
        {
            this.primary_key = primary_key;
            this.table_name = table_name;
            this.ConnectionStringsName = ConnectionStringsName;
        }

        public List<T> Select(Dictionary<string, string> filtros=null)
        {
            if (filtros == null)
                filtros = new Dictionary<string, string>();

            if (ActivarSoloRegistrosActivos == true)
                filtros.Add("Active", "true");

            if (ActivarRegistrodeUsuarioEnTransaccion==true)
            {

            }

            DataTable dt = new DataTable();
            List<T> Resgistros = new List<T>();
            this.Clear();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[this.ConnectionStringsName].ToString()))
                {
                    conn.Open();

                    var query = new SqlCommand(table_name + "_Select", conn);
                    query.CommandType = CommandType.StoredProcedure;

                    Type t = Entidad.GetType();
                    System.Reflection.PropertyInfo[] properties = t.GetProperties();

                    foreach (System.Reflection.PropertyInfo p in properties)
                    {
                        string name = p.Name;
                        object p2 = p.GetValue(Entidad, null);
                        Type type = p.PropertyType;

                        if (filtros.ContainsKey(name))
                            query.Parameters.Add(new SqlParameter("@" + name, filtros[name]));
                    }

                    using (var dr = query.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                }

                Resgistros = Map<T>(dt, Entidad);
            }
            catch (Exception ex)
            {
                throw;
            }

            return Resgistros;
        }

        public object Insert(T Entidad,string UserName)
        {
            DataTable dt = new DataTable();
            object result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[this.ConnectionStringsName].ToString()))
                {
                    conn.Open();

                    var query = new SqlCommand(table_name + "_Insert", conn);
                    query.CommandType = CommandType.StoredProcedure;

                    Type t = Entidad.GetType();
                    System.Reflection.PropertyInfo[] properties = t.GetProperties();

                    foreach (System.Reflection.PropertyInfo p in properties)
                    {
                        string name = p.Name;
                        //asignar valor a create user y create Date
                        if (ActivarRegistrodeUsuarioEnTransaccion == true)
                        {
                            if(name== "Create_User")
                                p.SetValue(Entidad,UserName);
                            if (name == "Create_Date")
                                p.SetValue(Entidad, DateTime.Now);
                            if (name == "Modify_Date")
                                p.SetValue(Entidad, DateTime.Parse(System.Data.SqlTypes.SqlDateTime.MinValue.ToString()));
                        }
                        if (ActivarSoloRegistrosActivos == true){
                            if (name == "Active")
                                p.SetValue(Entidad, true);
                        }

                        object p2 = p.GetValue(Entidad, null);
                        Type type = p.PropertyType;
                        if (!primary_key.Equals(name)){
                            query.Parameters.Add(new SqlParameter("@" + name, p2));
                        }
                    }

                        result = query.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public object Update(T Entidad,string UserName)
        {
            DataTable dt = new DataTable();
            object result = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[this.ConnectionStringsName].ToString()))
                {
                    conn.Open();

                    var query = new SqlCommand(table_name + "_Update", conn);
                    query.CommandType = CommandType.StoredProcedure;

                    Type t = Entidad.GetType();
                    System.Reflection.PropertyInfo[] properties = t.GetProperties();

                    foreach (System.Reflection.PropertyInfo p in properties)
                    {
                        string name = p.Name;

                        //asignar valor a modify user y modify Date
                        if (ActivarRegistrodeUsuarioEnTransaccion == true)
                        {
                            if (name == "Modify_User")
                                p.SetValue(Entidad, UserName);
                            if (name == "Modify_Date")
                                p.SetValue(Entidad, DateTime.Now);


                        }

                        object p2 = p.GetValue(Entidad, null);
                        Type type = p.PropertyType;

                        query.Parameters.Add(new SqlParameter("@" + name, p2));
                    }

                    result = query.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public List<T> SelectRaw<T>(string consulta)
        {
            DataTable dt = new DataTable();
            List<T> ObjetosMapeo = new List<T>();
            T ObjetoMapeo= (T)Activator.CreateInstance(typeof(T));

            try
            {

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[this.ConnectionStringsName].ToString()))
                {
                    conn.Open();

                    var query = new SqlCommand(consulta, conn);
                    query.CommandType = CommandType.Text;

                    using (var dr = query.ExecuteReader())
                    {
                        dt.Load(dr);
                    }
                }

                ObjetosMapeo=Map<T>(dt, ObjetoMapeo);
            }
            catch (Exception ex)
            {
                throw;
            }

            return ObjetosMapeo;
        }
        public int QueryRaw(string consulta)
        {
            int result;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[this.ConnectionStringsName].ToString()))
                {
                    conn.Open();

                    var query = new SqlCommand(consulta, conn);
                    query.CommandType = CommandType.Text;

                    result = query.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }
        public SqlDataReader consulta(){
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Server"].ToString());
            conn.Open();
            string consulta=" SELECT * FROM OPENROWSET('ADSDSOObject','adsdatasource','SELECT sAmaccountname  FROM ''LDAP://ferrebaratillo.com/DC=ferrebaratillo,DC=com'' WHERE givenname = ''Edwin'' ') ";
                    var query = new SqlCommand(consulta, conn);
                    query.CommandType = CommandType.Text;

                    var dr = query.ExecuteReader();
                    return dr;
        }

        public List<T> Map<T>(DataTable result,T Entidad)
        {
            Type t = Entidad.GetType();
            System.Reflection.PropertyInfo[] properties = t.GetProperties();
            List<T> Entidades = new List<T>();

            foreach (DataRow row in result.Rows)
            {
                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    string name = p.Name;
                    object p2 = p.GetValue(Entidad, null);
                    Type type = p.PropertyType;

                    if (type.Equals(typeof(string)))
                    {
                        if (row[name] is DBNull)
                            p.SetValue(Entidad, string.Empty);
                        else
                            p.SetValue(Entidad, (string)row[name]);
                    }
                    else if (type.Equals(typeof(int)))
                    {
                        if (row[name] is DBNull)
                            p.SetValue(Entidad, 0);
                        else
                            p.SetValue(Entidad, (int)row[name]);
                    }
                    else if (type.Equals(typeof(double)))
                    {
                        if (row[name] is DBNull)
                            p.SetValue(Entidad, 0);
                        else
                            p.SetValue(Entidad, (double)row[name]);
                    }
                    else if (type.Equals(typeof(DateTime)))
                    {
                        if (row[name] is DBNull)
                            p.SetValue(Entidad, (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue);
                        else
                            p.SetValue(Entidad, (DateTime)row[name]);

                    }
                    else if (type.Equals(typeof(short)))
                    {
                        if (row[name] is DBNull)
                            p.SetValue(Entidad, string.Empty);
                        else
                            p.SetValue(Entidad, (short)row[name]);
                    }
                    else if (type.Equals(typeof(byte)))
                    {
                        if (row[name] is DBNull)
                            p.SetValue(Entidad, 0);
                        else
                            p.SetValue(Entidad, (byte)row[name]);
                    }
                    else if (type.Equals(typeof(bool)))
                    {
                        if (row[name] is DBNull)
                            p.SetValue(Entidad, false);
                        else
                            p.SetValue(Entidad, (bool)row[name]);
                    }
                    else if (row[name] == null)
                    {
                        p.SetValue(Entidad, string.Empty);
                    }

                }
                Entidades.Add(Utiles.Copia(Entidad));

            }

            return Entidades;
        }

        public void Clear()
        {
            Type t = Entidad.GetType();
            System.Reflection.PropertyInfo[] properties = t.GetProperties();

            foreach (System.Reflection.PropertyInfo p in properties)
            {
                string name = p.Name;
                object p2 = p.GetValue(Entidad, null);
                Type type = p.PropertyType;

                if (type.Equals(typeof(string)))
                {
                    p.SetValue(Entidad, string.Empty);
                }
                else if (type.Equals(typeof(int)))
                {
                    p.SetValue(Entidad, 0);
                }
                else if (type.Equals(typeof(double)))
                {
                    p.SetValue(Entidad, 0);
                }
                else if (type.Equals(typeof(DateTime)))
                {
                    p.SetValue(Entidad, DateTime.MinValue);

                }
                else if (type.Equals(typeof(short)))
                {
                    p.SetValue(Entidad, string.Empty);
                }
                else if (type.Equals(typeof(byte)))
                {
                    p.SetValue(Entidad, 0);
                }
                else if (type.Equals(typeof(bool)))
                {
                    p.SetValue(Entidad, false);
                }

            }

        }
    }
}