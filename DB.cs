using MySql.Data.MySqlClient;

namespace HardwareStore.Data
{
    public class DB
    {
        public static MySqlConnection GetConnection()
        {
            string connStr = "server=localhost;user=root;password=;database=pos_hardware_store;";
            return new MySqlConnection(connStr);
        }
    }
}
