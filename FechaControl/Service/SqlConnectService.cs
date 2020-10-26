using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Permissions;

namespace ApiFechaControl.Service
{
    sealed class SqlConnectService : ISqlConnectService
    {
        private string _connectionString;

        public SqlConnectService(string connectionString)
        {
            _connectionString = connectionString;
        }


        public DateTime Execute(SqlCommand cmd)
        {
            DateTime fecha = new DateTime();
            try
            {
                using (var conexion = new SqlConnection(_connectionString))
                {
                    conexion.Open();
                    cmd.Connection = conexion;
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        fecha = (DateTime)reader[0];
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                throw new Exception("No es posible establecer conexión al Servidor de Base de Datos", sqlEx.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception("Problema en recepción de datos", ex.InnerException);
            }
            return fecha;
        }

        public void ActivarSqlDependency()
        {
            try
            {
                SqlClientPermission permission = new SqlClientPermission(PermissionState.Unrestricted);

                SqlDependency.Stop(_connectionString);
                if (!SqlDependency.Start(_connectionString))
                {
                    throw new Exception("No es posible iniciar el agente de escucha para recibir notificaciones de cambio de dependencia desde la instancia de SQL Server especificada por la cadena de conexión");
                }
            }
            catch (System.Security.SecurityException)
            {
                throw new Exception("No existe los permisos necesarios en el SqlClient");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                throw new Exception("El usuario utilizado no tiene los privilegios necesarios o no se logro establecer comunicación a la instancia de SQL Server");
            }
            catch(Exception)
            {
                throw new Exception("No se logro establecer comunicación a la instancia de SQL Server");
            }
        }

        public void DesactivarSqlDependency()
        {
            SqlDependency.Stop(_connectionString);
        }
    }
}
