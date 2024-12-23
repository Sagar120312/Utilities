using System.Data;

namespace DatabaseUtility.Operation.Interface
{
    public interface IDatabaseOperations
    {
        DataTable ExecuteStoredProcedure(string procedureName, Dictionary<string, object>? parameters = null);
        DataTable ExecuteRawQuery(string query);
    }
}
