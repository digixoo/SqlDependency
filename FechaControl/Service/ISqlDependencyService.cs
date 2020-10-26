using System;
using System.Data.SqlClient;
using static ApiFechaControl.Service.SqlDependencyService;

namespace ApiFechaControl.Service
{
    public interface ISqlDependencyService
    {
        event VerificaFecha OnVerificaFecha;
        DateTime Fecha { get; }
        int CantidadReconecciones { set; }
        bool Activo { get; }
        int Timeout { set; get; }
        string Query { set; get; }
        void RecargarFecha();
        void Start();
        void Dispose();       

    }
}
