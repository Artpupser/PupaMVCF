namespace PupaMVCF.Framework.Database;

public interface IDatabaseProcessor {
   public Task OpenConnection(CancellationToken cancellationToken);
   public Task CloseConnection(CancellationToken cancellationToken);

   public Task<IEnumerable<T>> SelectFromId<T>(string table, int id, CancellationToken cancellationToken);

   public Task<IEnumerable<T>> SelectFromColumn<T>(string table, string columnName, string value,
      CancellationToken cancellationToken);

   public Task<IEnumerable<T>> SelectAll<T>(string table, CancellationToken cancellationToken);
   public Task<int> Insert<T>(string table, T content, CancellationToken cancellationToken);
   public Task<int> Insert<T>(string table, IEnumerable<T> content, CancellationToken cancellationToken);
   public Task<int> UpdateFromId<T>(string table, int id, T content, CancellationToken cancellationToken);

   public Task<int> UpdateFromColumn<T>(string table, string columnName, string value, T content,
      CancellationToken cancellationToken);

   public Task<int> DeleteFromId(string table, int id, CancellationToken cancellationToken);
   public Task<int> Delete<T>(string table, T content, CancellationToken cancellationToken);
   public Task<int> Delete<T>(string table, IEnumerable<T> content, CancellationToken cancellationToken);
   public Task<int> DeleteAll(string table, CancellationToken cancellationToken);
}