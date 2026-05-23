using System.Data;

using Microsoft.Extensions.Configuration;

using PupaMVCF.Framework.Extensions;

namespace PupaMVCF.Framework.Database;

public sealed class DatabaseConnectionFactory<T>(IConfiguration configuration) : IDatabaseConnectionFactory
   where T : IDbConnection {
   private readonly string _databaseConnectionString = configuration.GetValue<string>("DB_CONNECTION_STRING") ??
                                                       throw new Exception("Undefined DB_CONNECTION_STRING.");

   public IDbConnection GetConnection() {
      return (T)(Activator.CreateInstance(typeof(T), _databaseConnectionString) ??
                 throw new Exception($"{nameof(T)} failed instance creation"));
   }
}