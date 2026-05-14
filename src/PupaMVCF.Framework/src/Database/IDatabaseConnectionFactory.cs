using System.Data;

namespace PupaMVCF.Framework.Database;

public interface IDatabaseConnectionFactory {
   public IDbConnection GetConnection();
}