using System;
using System.Data.SqlClient;

namespace Verona.Lib.Common.Utility
{
    public static class SqlUtility
    {
        public static bool TableExists(string connectionString, string tableName)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var cmdText = string.Format("SELECT COUNT(*) FROM information_schema.tables WHERE table_name = '{0}'", tableName);

                using (var command = new SqlCommand(cmdText, connection))
                {
                    var count = Convert.ToInt32(command.ExecuteScalar());
                    return (count > 0);
                }
            }
        }

        public static int ExecuteNonQuery(string connectionString, string cmdText)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(cmdText, connection))
                    return command.ExecuteNonQuery();
            }
        }
    }
}
