using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

using Dapper;

using PupaLib.Core;

using PupaMVCF.Framework.Core;
using PupaMVCF.Framework.Models;

namespace PupaMVCF.Framework.Database;

public abstract class Repository<T>(IDatabaseConnectionFactory databaseConnectionFactory) {
   protected IDatabaseConnectionFactory DatabaseConnectionFactory { get; init; } = databaseConnectionFactory;

   public string TableName { get; init; } = typeof(T).GetCustomAttribute<TableAttribute>()?.Name ??
                                            throw new Exception(
                                               $"{nameof(TableAttribute)} not found in {typeof(T).Name}");

   #region EXECUTE

   public async Task<Option> SaveAsync(T content, string whereColumn, CancellationToken cancellationToken) {
      try {
         var props = typeof(T).GetProperties();
         var set = string.Join(", ",
            props.Where(p => p.Name != whereColumn)
               .Select(p => $"{p.Name} = @{p.Name}"));

         var connection = DatabaseConnectionFactory.GetConnection();
         var sql = $"UPDATE {TableName} SET {set} WHERE {whereColumn} = @{whereColumn}";
         var commandDefinition =
            new CommandDefinition(sql, content, cancellationToken: cancellationToken);
         await connection.ExecuteAsync(commandDefinition);
         return Option.Ok();
      } catch {
         return Option.Fail();
      }
   }


   public async Task<Option> ChangeFromId(int id, string column, object value, CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition = new CommandDefinition($"UPDATE {TableName} SET {column}={value} WHERE id=\'{id}\'",
            cancellationToken: cancellationToken);
         await connection.ExecuteAsync(commandDefinition);
         return Option.Ok();
      } catch {
         return Option.Fail();
      }
   }

   public async Task<Option> ChangeFrom(string whereColumn, object whereValue, string column, object value,
      CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition = new CommandDefinition(
            $"UPDATE {TableName} SET {column}={value} WHERE {whereColumn}=\'{whereValue}\'",
            cancellationToken: cancellationToken);
         await connection.ExecuteAsync(commandDefinition);
         return Option.Ok();
      } catch {
         return Option.Fail();
      }
   }

   #endregion

   #region QUERY

   public async Task<Option<IEnumerable<T>>> GetAll(CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition =
            new CommandDefinition($"SELECT * FROM {TableName}", cancellationToken: cancellationToken);
         return Option<IEnumerable<T>>.Ok(await connection.QueryAsync<T>(commandDefinition));
      } catch {
         return Option<IEnumerable<T>>.Fail();
      }
   }

   public async Task<Option<IEnumerable<T>>>
      GetWhere(string column, object value, CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition = new CommandDefinition($"SELECT * FROM {TableName} WHERE {column}=\'{value}\'",
            cancellationToken: cancellationToken);
         return Option<IEnumerable<T>>.Ok(await connection.QueryAsync<T>(commandDefinition));
      } catch {
         return Option<IEnumerable<T>>.Fail();
      }
   }

   public async Task<Option> ExistsAsync(string column, object value, CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition = new CommandDefinition($"SELECT * FROM {TableName} WHERE {column}=\'{value}\'",
            cancellationToken: cancellationToken);
         var result = await connection.QueryAsync<T>(commandDefinition);
         return result.Any() ? Option.Ok() : Option.Fail();
      } catch {
         return Option.Fail();
      }
   }

   public async Task<Option<T>> FirstWhere(string column, object value, CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition = new CommandDefinition($"SELECT * FROM {TableName} WHERE {column}=\'{value}\'",
            cancellationToken: cancellationToken);
         var result = await connection.QueryFirstAsync<T>(commandDefinition);
         return Option<T>.Ok(result);
      } catch {
         return Option<T>.Fail();
      }
   }

   public async Task<Option<T>> GetFromId(int id, CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition = new CommandDefinition($"SELECT * FROM {TableName} WHERE id=\'{id}\'",
            cancellationToken: cancellationToken);
         return Option<T>.Ok(await connection.QuerySingleAsync<T>(commandDefinition));
      } catch {
         return Option<T>.Fail();
      }
   }

   #endregion
}