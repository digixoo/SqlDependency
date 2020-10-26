using System;
using System.Data.SqlClient;
using System.Threading;

namespace ApiFechaControl.Service
{
    public sealed class SqlDependencyService : ISqlDependencyService, IDisposable
    {
        private int _timeout = 6000;
        private string _query = "select prc_fec_rea from dbo.prc";

        private ISqlConnectService _sqlConnection;

        private DateTime _fecha = new DateTime();
        private SqlDependency _dependency = null;
        private int _cantreconnect = 5;
        private int _cantreconnectConf = 5;
        private bool _isActivo = false;
        #region Eventos Publicos de salida        ;
        public delegate void VerificaFecha();
        public event VerificaFecha OnVerificaFecha;
        #endregion

        #region Propiedades publicas
        public DateTime Fecha { get => _fecha; }
        public string Query { set => _query = value; get => _query; }
        public bool Activo { get => _isActivo; }
        public int Timeout
        {
            set
            {
                if (value >= 0)
                {
                    _timeout = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("El tiempo de espera debe ser mayor a 0");
                }

            }
            get
            {
                return _timeout;
            }
        }
        public int CantidadReconecciones
        {
            set
            {
                var aux = value;
                if (value >= -1)
                {
                    _cantreconnect = aux;
                    _cantreconnectConf = aux;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Valor fuera de rango permitido");
                }
                    


            }

        }
        #endregion

        #region Construcuctor
        public SqlDependencyService(ISqlConnectService sqlConnect)
        {
            _sqlConnection = sqlConnect;
        }
        #endregion

        #region Métodos expuestos
        public void Start()
        {
            try
            {
                _sqlConnection.ActivarSqlDependency();
                DependenciaPrc();
                _isActivo = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
        public void RecargarFecha()
        {
            OnCambio(_dependency, null);
        }
        #endregion

        #region Métodos privados
        private void Stop()
        {
            _isActivo = false;
            _sqlConnection.DesactivarSqlDependency();
        }

        private void Reconnect()
        {
            if ((_cantreconnectConf >= 0 && _cantreconnect > 0) || _cantreconnectConf == -1)
            {
                Thread.Sleep(_timeout);
                DependenciaPrc();
            }
            else
            {
                throw new Exception("Se ha superado el máximo de intentos de reconección");
            }
        }

        private void DependenciaPrc()
        {
            SqlCommand sqlCommand = new SqlCommand(_query);
            _dependency = new SqlDependency(sqlCommand, timeout: 3600, options: null);

            _dependency.OnChange -= OnCambio;
            _dependency.OnChange += new OnChangeEventHandler(OnCambio);

            //Ejecución
            try
            {
                _fecha = _sqlConnection.Execute(sqlCommand);
                _cantreconnect = _cantreconnectConf;
                OnVerificaFecha?.Invoke();
            }
            catch (Exception)
            {
                _cantreconnect--;
                Reconnect();
            }

        }

        private void OnCambio(object sender, SqlNotificationEventArgs e)
        {
            try
            {
                DependenciaPrc();
            }
            catch (Exception)
            {
                Dispose();
            }


        }
        #endregion

    }
}
