using System.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PupaMVCF.Framework.Database;

public sealed class DatabaseConnectionFactory<T>(IServiceProvider serviceProvider, IConfiguration configuration) : IDatabaseConnectionFactory
   where T : IDbConnection {
   private readonly string _databaseConnectionString = configuration.GetValue<string>("DB_CONNECTION_STRING") ??
                                                       throw new Exception("Undefined DB_CONNECTION_STRING.");

   public IDbConnection GetConnection() {
      return (T)(ActivatorUtilities.CreateInstance(serviceProvider, typeof(T), [_databaseConnectionString]) ??
                 throw new Exception($"{nameof(T)} failed instance creation"));
   }
}
