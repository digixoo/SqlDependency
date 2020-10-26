using System;
using System.Data.SqlClient;

namespace ApiFechaControl.Service
{
    public interface ISqlConnectService
    {
        DateTime Execute(SqlCommand cmd);
        void ActivarSqlDependency();
        void DesactivarSqlDependency();
    }
}
