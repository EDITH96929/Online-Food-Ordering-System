using System.Configuration;
using System.Data.SqlClient;

namespace FoodOrderingSystem.DAL
{
    public class DatabaseHelper
    {
        private static string connectionString =
            ConfigurationManager.ConnectionStrings["FoodDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}