using System;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Data;

namespace AgopBot.SQL
{
    static class DB
    {
        public static MySqlConnection connection;
        private static string server;
        private static string database;
        private static string uid;
        private static string password;

        public static void Initialize(string _server, string _database, string _uid, string _password)
        {
            server = _server;
            database = _database;
            uid = _uid;
            password = _password;

            string connectionString;
            connectionString = String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3}", server, database, uid, password);

            connection = new MySqlConnection(connectionString);
        }

        public static bool OpenConnection()
        {
            try {
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                return true;
            } catch (MySqlException e) {
                Console.WriteLine("MYSQL: " + e.Message);
                return false;
            }
        }

        public static bool CloseConnection()
        {
            try {
                if (connection.State != ConnectionState.Closed)
                    connection.Close();
                return true;
            } catch (MySqlException e) {
                Console.WriteLine("MYSQL: " + e.Message);
                return false;
            }
        }

        public static void QueryNoReturn(string query)
        {
            Thread T = new Thread(() =>
            {
                if (OpenConnection())
                {
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    CloseConnection();
                }
            });
            T.Start();
        }

        public static string EscapeString(string query)
        {
            return MySqlHelper.EscapeString(query);
        }
    }
}
