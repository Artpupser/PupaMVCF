using System.Data;
using System.Reflection;

using Npgsql;

using Dapper;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PupaMVCF.Framework.Database.PgSql;

public sealed class DatabasePgSqlProcessor : IDatabaseProcessor {
   private readonly NpgsqlConnection _connection;
   private readonly ILogger<DatabasePgSqlProcessor> _logger;

   public DatabasePgSqlProcessor(IConfiguration configuration, ILogger<DatabasePgSqlProcessor> logger) {
      _connection =
         new NpgsqlConnection(configuration["DatabaseConnectionString"] ??
                              configuration["DATABASE_CONNECTION_STRING"] ??
                              throw new NullReferenceException(
                                 "Value [DatabaseConnectionString] not found in configuration"));
      _logger = logger;
   }

   #region OPEN_CLOSE

   public async Task OpenConnection(CancellationToken cancellationToken) {
      if (_connection.State != ConnectionState.Open) {
         _logger.LogInformation("Connection with database open...");
         await _connection.OpenAsync(cancellationToken);
      }
   }

   public async Task CloseConnection(CancellationToken cancellationToken) {
      if (_connection.State != ConnectionState.Closed) {
         _logger.LogWarning("Connection with database close...");
         await _connection.CloseAsync();
      }
   }

   #endregion

   #region CUSTOM_PROCEDURES_ASYNC

   private async Task<IEnumerable<T>> CustomQueryAsync<T>(string sql, object? args,
      CancellationToken cancellationToken) {
      await OpenConnection(cancellationToken);
      var commandDefinition =
         new CommandDefinition(sql, args,
            cancellationToken: cancellationToken);
      return await _connection.QueryAsync<T>(commandDefinition);
   }

   private async Task<int> CustomExecuteAsync(string sql, object? args,
      CancellationToken cancellationToken) {
      await OpenConnection(cancellationToken);
      var commandDefinition =
         new CommandDefinition(sql, args,
            cancellationToken: cancellationToken);
      return await _connection.ExecuteAsync(commandDefinition);
   }

   #endregion

   public async Task<IEnumerable<T>> SelectFromId<T>(string table, int id, CancellationToken cancellationToken) {
      await OpenConnection(cancellationToken);
      return await CustomQueryAsync<T>($"SELECT * FROM \"{table}\" WHERE \"id\"=@id", new { id }, cancellationToken);
   }

   public async Task<IEnumerable<T>> SelectFromColumn<T>(string table, string columnName, string value,
      CancellationToken cancellationToken) {
      await OpenConnection(cancellationToken);
      return await CustomQueryAsync<T>($"SELECT * FROM \"{table}\" WHERE \"{columnName}\"=@value",
         new { columnName, value },
         cancellationToken);
   }

   public async Task<IEnumerable<T>> SelectAll<T>(string table, CancellationToken cancellationToken) {
      await OpenConnection(cancellationToken);
      return await CustomQueryAsync<T>($"SELECT * FROM \"{table}\"", new { }, cancellationToken);
   }

   public async Task<int> Insert<T>(string table, T content, CancellationToken cancellationToken) {
      if (content == null) return 0;
      await OpenConnection(cancellationToken);
      var props = content.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
      var columns = string.Join(',', props.Select(x => $"\"{x.Name}\""));
      var values = string.Join(',', props.Select(x => $"@{x.Name}"));
      var parameters = new DynamicParameters(content);
      return await CustomExecuteAsync($"INSERT INTO \"{table}\" ({columns}) VALUES {values}",
         parameters,
         cancellationToken);
   }

   public async Task<int> Insert<T>(string table, IEnumerable<T> content, CancellationToken cancellationToken) {
      await OpenConnection(cancellationToken);
      var sum = 0;
      foreach (var item in content)
         sum += await Insert(table, item, cancellationToken);
      return sum;
   }

   public async Task<int> UpdateFromId<T>(string table, int id, T content, CancellationToken cancellationToken) {
      if (content == null) return 0;
      await OpenConnection(cancellationToken);
      var props = content.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
      var sets = string.Join(',', props.Select(x => $"\"{x.Name}\"=@{x.Name}"));
      var parameters = new DynamicParameters(content);
      parameters.Add("_id", id);
      return await CustomExecuteAsync($"UPDATE \"{table}\" SET {sets} WHERE id=@_id",
         parameters,
         cancellationToken);
   }

   public async Task<int> UpdateFromColumn<T>(string table, string columnName, string value, T content,
      CancellationToken cancellationToken) {
      if (content == null) return 0;
      await OpenConnection(cancellationToken);
      var props = content.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
      var sets = string.Join(',', props.Select(x => $"\"{x.Name}\"=@{x.Name}"));
      var parameters = new DynamicParameters(content);
      parameters.Add("_where", value);
      return await CustomExecuteAsync($"UPDATE \"{table}\" SET {sets} WHERE \"{columnName}\"=@_where",
         parameters,
         cancellationToken);
   }

   public async Task<int> DeleteFromId(string table, int id, CancellationToken cancellationToken) {
      await OpenConnection(cancellationToken);
      return await CustomExecuteAsync($"DELETE FROM \"{table}\" WHERE \"id\"=@id", new { id }, cancellationToken);
   }

   public async Task<int> Delete<T>(string table, T content, CancellationToken cancellationToken) {
      if (content == null) return 0;
      await OpenConnection(cancellationToken);
      var props = content.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
      var sets = string.Join(',', props.Select(x => $"\"{x.Name}\"=@{x.Name}"));
      var parameters = new DynamicParameters(content);
      return await CustomExecuteAsync($"DELETE FROM \"{table}\" WHERE {sets}", parameters, cancellationToken);
   }

   public async Task<int> Delete<T>(string table, IEnumerable<T> content, CancellationToken cancellationToken) {
      await OpenConnection(cancellationToken);
      var sum = 0;
      foreach (var item in content)
         sum += await Delete(table, item, cancellationToken);
      return sum;
   }

   public async Task<int> DeleteAll(string table, CancellationToken cancellationToken) {
      await OpenConnection(cancellationToken);
      return await CustomExecuteAsync($"TRUNCATE TABLE \"{table}\"", new { }, cancellationToken);
   }
}