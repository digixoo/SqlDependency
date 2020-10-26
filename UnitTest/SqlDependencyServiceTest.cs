using ApiFechaControl.Service;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnitTest.Factory;
using UnitTest.Fixtures;

using Xunit;
namespace UnitTest
{
    public class SqlDependencyServiceTest
    {        
        private readonly ISqlConnectService _sqlConnect;
        private ISqlDependencyService _sqlDependencyService;

        public SqlDependencyServiceTest()
        {
            _sqlConnect = Mock.Of<ISqlConnectService>();

            _sqlDependencyService = new SqlDependencyService(_sqlConnect);
        }

        /// <summary>
        /// Cuando se inicie el Servicio este debe prender el SqlDependency y suscribir el evento
        /// </summary>
        [Fact]
        public void Start_ShouldBeActivedAndConnectedToBd()
        {
            //Arrange
            Mock.Get<ISqlConnectService>(_sqlConnect).Setup(x => x.ActivarSqlDependency()).Verifiable("No se Activo SqlDependency");
            Mock.Get<ISqlConnectService>(_sqlConnect).Setup(x => x.Execute(It.IsAny<SqlCommand>())).Verifiable("No se invoca la ejecución de la consulta a BD");

            //Act
            _sqlDependencyService.Start();

            //Assert
            Mock.VerifyAll(Mock.Get(_sqlConnect));
        }

        /// <summary>
        /// Tras caida de conexión el servicio debe retornar la última fecha que haya conseguido
        /// </summary>
        [Fact]
        public void Fecha_ShouldReturnDateAfterRecargarFecha()
        {
            //Arrange
            bool isCorrect = true;
            DateTime Fecha = new DateTime();

            Mock.Get<ISqlConnectService>(_sqlConnect).SetupSequence(x => x.Execute(It.IsAny<SqlCommand>()))
             .Returns(new DateTime(2020, 01, 24))
             .Returns(new DateTime(2020, 01, 25))
             .Throws(SqlExceptionFactory.NewSqlException())
             .Returns(new DateTime(2020, 02, 10))
             .Returns(new DateTime(2020, 02, 10));
                        
            Queue<DateTime> lista = new Queue<DateTime>();
            lista.Enqueue(new DateTime(2020, 01, 24));
            lista.Enqueue(new DateTime(2020, 01, 25));
            lista.Enqueue(new DateTime(2020, 02, 10));
            lista.Enqueue(new DateTime(2020, 02, 10));
                        
            _sqlDependencyService.CantidadReconecciones = -1;
            _sqlDependencyService.Timeout = 0;
            
            //Act
            _sqlDependencyService.Start();
            for (int i = 4; i > 0; i--)
            {
                Fecha = _sqlDependencyService.Fecha;

                if (Fecha != lista.Dequeue())
                {
                    isCorrect = false;
                    break;
                }

                _sqlDependencyService.RecargarFecha();
            }

            //Assert
            isCorrect.Should().BeTrue();
        }

        [Fact]
        public void Fecha_ShouldReturnFecha()
        {
            //Arrange
            var fecha = 24.January(2020);
            Mock.Get<ISqlConnectService>(_sqlConnect).Setup(x => x.Execute(It.IsAny<SqlCommand>())).Returns(fecha);            

            //Act
            _sqlDependencyService.Start();
         
            var retornoFecha = _sqlDependencyService.Fecha;


            //Assert
            retornoFecha.Should().Equals(fecha);
        }


        /// <summary>
        /// Prueb de reconección
        /// >  0 cantidad n de reconecciones
        /// 0  sin recconección        
        /// -1 recconección ilimitadas
        /// </summary>
        [Theory]      
        [InlineData(5, 5)]
        [InlineData(6, 5)]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        public void Fecha_ShouldReturnException(int cantIntentos, int paramIntentos)
        {
            //Arrange
            Mock.Get<ISqlConnectService>(_sqlConnect).Setup(x => x.Execute(It.IsAny<SqlCommand>())).Throws<Exception>();
            _sqlDependencyService.CantidadReconecciones = paramIntentos;


            //Act
            Action action = () =>
            {
                _sqlDependencyService.Start();
                for (int i = cantIntentos; i > 0; i--)
                {
                    _sqlDependencyService.RecargarFecha();
                }
            };

            //Assert
            action.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData(4)]
        [InlineData(15)]
        public void Fecha_ShouldReturnDateAfterException(int paramIntentos)
        {
            //Arrange       
            Mock.Get<ISqlConnectService>(_sqlConnect).SetupSequence(x => x.Execute(It.IsAny<SqlCommand>()))
             .Throws(SqlExceptionFactory.NewSqlException())
             .Throws(SqlExceptionFactory.NewSqlException())
             .Throws(SqlExceptionFactory.NewSqlException())
             .Returns(new DateTime(2020, 02, 10));

            _sqlDependencyService.CantidadReconecciones = paramIntentos;
            _sqlDependencyService.Timeout = 0;
            DateTime fecha;

            //Act
            _sqlDependencyService.Start();
                        
            fecha = _sqlDependencyService.Fecha;
           
            //Assert
            fecha.Should().Be(10.February(2020));
        }

        [Fact]        
        public void Fecha_ShouldReturnDateAfterNException()
        {
            //Arrange       
            Mock.Get<ISqlConnectService>(_sqlConnect).SetupSequence(x => x.Execute(It.IsAny<SqlCommand>()))
             .Throws(SqlExceptionFactory.NewSqlException())
             .Throws(SqlExceptionFactory.NewSqlException())
             .Throws(SqlExceptionFactory.NewSqlException())
             .Throws(SqlExceptionFactory.NewSqlException())
             .Throws(SqlExceptionFactory.NewSqlException())
             .Throws(SqlExceptionFactory.NewSqlException())
             .Throws(SqlExceptionFactory.NewSqlException())
             .Returns(10.February(2020));

            _sqlDependencyService.CantidadReconecciones = -1;
            _sqlDependencyService.Timeout = 0;

            DateTime fecha = new DateTime();

            //Act
            _sqlDependencyService.Start();          
            fecha = _sqlDependencyService.Fecha;

            //Assert
            fecha.Should().Be(10.February(2020));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void Timeout_ShouldBeValue(int TiempoEspera)
        {
            //Arrange
            _sqlDependencyService.Timeout = TiempoEspera;

            //Act
            var Tiempo = _sqlDependencyService.Timeout;

            //Assert
            Tiempo.Should().Equals(TiempoEspera);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        [InlineData(-10000)]
        public void Timeout_ShouldThrowArgumentOutOfRangeException(int TiempoEspera)
        {
            //Arrange
            

            //Act
            Action action = () =>
            {
                _sqlDependencyService.Timeout = TiempoEspera;
            };

            //Assert
            action.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Query_ShouldBeEqual()
        {
            //Arrange
            string query = "Select fecha from TablaFecha";
            _sqlDependencyService.Query = query;

            //Act
            var retornoQuery = _sqlDependencyService.Query;

            //Assert
            retornoQuery.Should().Equals(query);
        }

        [Fact]
        public void Dispose_ShouldBeActivedStop()
        {
            //Arrange
            Mock.Get<ISqlConnectService>(_sqlConnect).Setup(x => x.DesactivarSqlDependency()).Verifiable("No sé ha deshabilitado el Servicio de Sql Dependency");            

            //Act
            using (var service = new SqlDependencyService(_sqlConnect))
            {
                
            }

            //Assert
            Mock.VerifyAll(Mock.Get(_sqlConnect));
        }

        [Fact]
        public void Event_ShouldBeActived()
        {
            //Arrange
            bool isActived = false;
            _sqlDependencyService.OnVerificaFecha += () => isActived = true;

            //Act
            _sqlDependencyService.Start();
            _sqlDependencyService.RecargarFecha();

            //Assert
            isActived.Should().BeTrue();
        }
    }
}
