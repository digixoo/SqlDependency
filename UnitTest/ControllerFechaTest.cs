using ApiFechaControl.Controller;
using ApiFechaControl.Service;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UnitTest
{
    public class ControllerFechaTest
    {
        private IControllerFecha _controller;
        private ISqlDependencyService _sqlDevendencyService;

        public ControllerFechaTest()
        {
            _sqlDevendencyService = Mock.Of<ISqlDependencyService>();
            _controller = new ControllerFecha(_sqlDevendencyService);
            
        }

        [Fact]
        public void Start_ShouldReturnFecha()
        {
            //Arrange
            bool isActived = false;
            Mock.Get<ISqlDependencyService>(_sqlDevendencyService).Setup(x => x.Start()).Raises(x => x.OnVerificaFecha += null);
           
            //Act            
            _controller.OnCambioFecha += (_, y) => isActived = true;
            _controller.Start();
            
            //Assert
            isActived.Should().BeTrue();
        }

        [Fact]
        public void Restart_ShouldActivedStartAndDispose()
        {
            //Arrange
            Mock.Get<ISqlDependencyService>(_sqlDevendencyService).Setup(x => x.Start()).Verifiable("No se ha ejecutado el método Start de SqlDependencyService");
            Mock.Get<ISqlDependencyService>(_sqlDevendencyService).Setup(x => x.Dispose()).Verifiable("No se ha ejecutado el método Dispose de SqlDependencyService");

            //Act
            _controller.Restart();

            //Assert
            Mock.VerifyAll(Mock.Get<ISqlDependencyService>(_sqlDevendencyService));
        }

        [Fact]
        public void Stop_ShouldActiveDispose()
        {
            //Arrange
            Mock.Get<ISqlDependencyService>(_sqlDevendencyService).Setup(x => x.Dispose()).Verifiable("No se ha ejecutado el método dispose de SqlDependencyService");

            //Atc
            _controller.Stop();

            //Assert
            Mock.VerifyAll(Mock.Get<ISqlDependencyService>(_sqlDevendencyService));
        }

        [Fact]
        public void Fecha_ShouldReturnFecha()
        {
            //Arrange
            DateTime fecha = new DateTime();
            Mock.Get<ISqlDependencyService>(_sqlDevendencyService).Setup(x => x.Start()).Raises(x => x.OnVerificaFecha += null);
            Mock.Get<ISqlDependencyService>(_sqlDevendencyService).SetupGet(x => x.Fecha).Returns(2.March(2021));

            //Act
            _controller.OnCambioFecha += (_, x) => { fecha = x.FechaControl; };
            _controller.Start();
            

            //Assert
            fecha.Should().Equals(2.March(2021));
        }

        [Fact]
        public void Fecha_ShouldThrowNullReferenceException()
        {
            //Arrange            
            Mock.Get<ISqlDependencyService>(_sqlDevendencyService).Setup(x => x.Start()).Raises(x => x.OnVerificaFecha += null);

            //Act            
            Action action = () =>
            {
                _controller.Start();
            };

            //Assert
            action.Should().Throw<Exception>();
        }
    }
}
