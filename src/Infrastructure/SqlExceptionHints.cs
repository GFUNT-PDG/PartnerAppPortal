using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace PartnerAppPortal.Infrastructure;

public static class SqlExceptionHints
{
    public static bool IsMissingTable(Exception ex)
    {
        for (var e = ex; e != null; e = e.InnerException!)
        {
            if (e is SqlException sql && sql.Number == 208) // Invalid object name
                return true;

            if (e.Message.Contains("Invalid object name", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
