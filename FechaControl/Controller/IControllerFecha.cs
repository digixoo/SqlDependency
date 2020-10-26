using ApiFechaControl.Controller;
using System;

namespace ApiFechaControl.Controller
{
    public interface IControllerFecha
    {
        /// <summary>
        /// Query de consulta a realizarse a la Base de datos para consultar por fecha de Sistema
        /// </summary>
        string Query { set; get; }
        /// <summary>
        /// Tiempo de espera máximo para apertura de conexión antes de generar un error (Exception)
        /// </summary>
        int TimeOutConexion { set; get; }
        /// <summary>
        /// Cantidad de intentos de conexión fallidos soportados antes de generar un error (Exception)
        /// ---
        /// Valores permitidos: 
        /// -- mayor a 0. Cantidad positiva de intentos antes de arrojar un error (Exception)
        /// -- igual a 0. Cantidad nula de intento, al ocurrir un fallo arrojará un error (Exception)
        /// -- -1. Cantidad ilimitada de intentos, intentará establecer conexión indefinidamente
        /// </summary>
        int CantidadReconecciones { set; }
        /// <summary>
        /// Evento desencadenado por cambio de fecha de tabla PRC de Base de Datos Factor
        /// ---
        /// Si no ha existido cambio por 1 hora, el evento se desencadenará atumáticamente
        /// </summary>
        event EventHandler<FechaControlEventArgs> OnCambioFecha;
        /// <summary>
        /// Evento desencadenado por un error interno del servicio SqlDependency
        /// </summary>
        event EventHandler<ControlErroresEventArgs> OnError;
        /// <summary>
        /// Inicia Servicio de SqlDependency
        /// </summary>
        void Start();
        /// <summary>
        /// Detiene Servicio de SqlDependency
        /// ---
        /// Puede provocar Exception si no logra establecer conexión a la BD
        /// </summary>
        void Stop();
        /// <summary>
        /// Reinicia Servicio de SqlDependency
        /// </summary>
        void Restart();
    }
}
