using ApiFechaControl.Service;
using Moq;
using System;
using System.Data.SqlClient;
using UnitTest.Factory;

namespace UnitTest.Fixtures
{
    public class ExecuteFicture : IDisposable
    {
        ISqlConnectService _sqlConnect;
        
        public ExecuteFicture()
        {
            _sqlConnect = Mock.Of<ISqlConnectService>(MockBehavior.Default);

            Mock.Get<ISqlConnectService>(_sqlConnect).SetupSequence(x => x.Execute(It.IsAny<SqlCommand>()))
              .Returns(new DateTime(2020, 01, 24))                
              .Returns(new DateTime(2020, 01, 25))
              .Throws (SqlExceptionFactory.NewSqlException())
              .Returns(new DateTime(2020, 02, 10));
        }

        public ISqlConnectService SqlConnect { get => _sqlConnect;  }

        public void Dispose()
        {

        }
    }
}
