using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

using Dapper;

using PupaLib.Core;

namespace PupaMVCF.Framework.Database;

public abstract class Repository<T>(IDatabaseConnectionFactory databaseConnectionFactory) {
   protected IDatabaseConnectionFactory DatabaseConnectionFactory { get; init; } = databaseConnectionFactory;

   private static readonly string CachedTableName =
      typeof(T).GetCustomAttribute<TableAttribute>()?.Name ??
      throw new InvalidOperationException(
         $"{nameof(TableAttribute)} not found in {typeof(T).Name}");

   public string TableName => CachedTableName;

   #region EXECUTE

   public Task<Option<long>> DeleteWhereId(long id, CancellationToken cancellationToken) {
      return DeleteWhere("id", id, cancellationToken);
   }

   public async Task<Option<long>> DeleteWhere(string whereColumn, object whereValue,
      CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition = new CommandDefinition(
            $"DELETE FROM {TableName} WHERE {whereColumn}=@WhereValue RETURNING id",
            new { WhereValue = whereValue },
            cancellationToken: cancellationToken);
         var scalarId = await connection.ExecuteScalarAsync<long>(commandDefinition);
         return scalarId > 0 ? Option<long>.Ok(scalarId) : Option<long>.Fail();
      } catch {
         return Option<long>.Fail();
      }
   }

   public Task<Option<long>> ChangeWhereId(long id, string column, object value, CancellationToken cancellationToken) {
      return ChangeWhere("id", id, column, value, cancellationToken);
   }

   public async Task<Option<long>> ChangeWhere(string whereColumn, object whereValue, string column, object value,
      CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition = new CommandDefinition(
            $"UPDATE {TableName} SET {column}=@Value WHERE {whereColumn}=@WhereValue RETURNING id",
            new { Value = value, WhereValue = whereValue },
            cancellationToken: cancellationToken);
         var scalarId = await connection.ExecuteScalarAsync<long>(commandDefinition);
         return scalarId > 0 ? Option<long>.Ok(scalarId) : Option<long>.Fail();
      } catch {
         return Option<long>.Fail();
      }
   }

   #endregion

   #region QUERY

   public async Task<Option<IEnumerable<T>>> AllAsync(CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition =
            new CommandDefinition($"SELECT * FROM {TableName}", cancellationToken: cancellationToken);
         return Option<IEnumerable<T>>.Ok(await connection.QueryAsync<T>(commandDefinition));
      } catch {
         return Option<IEnumerable<T>>.Fail();
      }
   }

   public async Task<Option<IEnumerable<T>>> WhereAllAsync(string column, object value,
      CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition = new CommandDefinition($"SELECT * FROM {TableName} WHERE {column}=@Value",
            new { Value = value },
            cancellationToken: cancellationToken);
         return Option<IEnumerable<T>>.Ok(await connection.QueryAsync<T>(commandDefinition));
      } catch {
         return Option<IEnumerable<T>>.Fail();
      }
   }

   public async Task<Option> ExistsAsync(string column, object value, CancellationToken cancellationToken) {
      try {
         using var connection = DatabaseConnectionFactory.GetConnection();

         var command = new CommandDefinition(
            $"SELECT EXISTS(SELECT 1 FROM {TableName} WHERE {column} = @Value)",
            new { Value = value },
            cancellationToken: cancellationToken);
         var exists = await connection.ExecuteScalarAsync<bool>(command);
         return exists
            ? Option.Ok()
            : Option.Fail();
      } catch {
         return Option.Fail();
      }
   }

   public async Task<Option<T>> WhereOneAsync(string column, object value, CancellationToken cancellationToken) {
      try {
         var connection = DatabaseConnectionFactory.GetConnection();
         var commandDefinition = new CommandDefinition($"SELECT * FROM {TableName} WHERE {column}=@Value",
            new { Value = value },
            cancellationToken: cancellationToken);
         var result = await connection.QueryFirstOrDefaultAsync<T>(commandDefinition);
         return result is not null ? Option<T>.Ok(result) : Option<T>.Fail();
      } catch {
         return Option<T>.Fail();
      }
   }

   public Task<Option<T>> WhereOneIdAsync(long value, CancellationToken cancellationToken) {
      return WhereOneAsync("id", value, cancellationToken);
   }

   public Task<Option<T>> WhereOneNameAsync(string value, CancellationToken cancellationToken) {
      return WhereOneAsync("name", value, cancellationToken);
   }

   public Task<Option<T>> WhereOneAgeAsync(string value, CancellationToken cancellationToken) {
      return WhereOneAsync("name", value, cancellationToken);
   }

   public Task<Option<T>> WhereOneEmailAsync(string value, CancellationToken cancellationToken) {
      return WhereOneAsync("name", value, cancellationToken);
   }

   public Task<Option<T>> WhereOneUserIdAsync(long value, CancellationToken cancellationToken) {
      return WhereOneAsync("user_id", value, cancellationToken);
   }

   #endregion
}