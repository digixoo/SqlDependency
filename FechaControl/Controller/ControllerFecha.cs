using ApiFechaControl.Service;
using System;
using System.Threading.Tasks;

namespace ApiFechaControl.Controller
{
    public class ControllerFecha : IControllerFecha
    {
        private ISqlDependencyService _sqlDependency;
        private delegate void VerificaFecha();
        private Task TaskError;
        #region Eventos
        public event EventHandler<FechaControlEventArgs> OnCambioFecha;
        public event EventHandler<ControlErroresEventArgs> OnError;
        #endregion

        #region Propiedades publicas
        public int CantidadReconecciones { set => _sqlDependency.CantidadReconecciones = value; }
        public int TimeOutConexion { set => _sqlDependency.Timeout = value; get => _sqlDependency.Timeout; }
        public string Query { set => _sqlDependency.Query = value; get => _sqlDependency.Query; }
        #endregion

        #region Constructores
        public ControllerFecha(ISqlDependencyService sqlDependency)
        {
            _sqlDependency = sqlDependency;
        }
        public ControllerFecha(string connectionStirng)
        {
            _sqlDependency = new SqlDependencyService(new SqlConnectService(connectionStirng));
        }
        #endregion

        #region Métodos publicos
        public void Stop()
        {
            _sqlDependency.Dispose();
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void Start()
        {
            _sqlDependency.OnVerificaFecha += new SqlDependencyService.VerificaFecha(OnNuevaFecha);

            try
            {
                _sqlDependency.Start();
                TaskError = Task.Run(() => ControlError());
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
     
        #endregion

        #region Métodos privados
        private void ControlError()
        {
            while(true)
            {
                if (_sqlDependency?.Activo == false)
                {
                    OnNuevoError();
                    break;
                }
            }

        }
        private void OnNuevaFecha()
        {
            FechaControlEventArgs args = new FechaControlEventArgs();
            args.FechaControl = _sqlDependency.Fecha;
            CambioFecha(args);
        }

        private void OnNuevoError()
        {
            ControlErroresEventArgs args = new ControlErroresEventArgs();
            args.MensajeError = "Se ha perdido la conexión con el Servidor de Base de Datos";
            Error(args);
        }

        private void Error(ControlErroresEventArgs args)
        {
            if (OnCambioFecha != null)
            {
                OnError.Invoke(this, args);
            }
            else
            {
                throw new NullReferenceException("No se ha establecido suscripción para evento OnError");
            }

        }

        private void CambioFecha(FechaControlEventArgs args)
        {        
            if(OnCambioFecha != null)
            {
                OnCambioFecha.Invoke(this, args);
            }
            else
            {
                throw new NullReferenceException("No se ha establecido suscripción para evento OnCambioFecha");
            }
            
        }
        #endregion
    }

    public class FechaControlEventArgs : EventArgs
    {
        public DateTime FechaControl { get; set; }
    }

    public class ControlErroresEventArgs : EventArgs
    {
        public string MensajeError { get; set; }
    }
}
