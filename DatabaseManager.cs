using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace CybersecurityChatbot
{
    public class DatabaseManager
    {
        private string connectionString;
        private MySqlConnection connection;

        public DatabaseManager(string host = "localhost", string database = "cybersecurity_chatbot",
                               string user = "root", string password = "your_password") // CHANGE PASSWORD
        {
            connectionString = $"Server={host};Database={database};Uid={user};Pwd={password};";
            Connect();
            CreateTables();
        }

        private void Connect()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection error: {ex.Message}");
                try
                {
                    string createDbString = connectionString.Replace($"Database={connection?.Database};", "");
                    using (var tempConn = new MySqlConnection(createDbString))
                    {
                        tempConn.Open();
                        string createDbQuery = "CREATE DATABASE IF NOT EXISTS cybersecurity_chatbot";
                        using (var cmd = new MySqlCommand(createDbQuery, tempConn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    connection = new MySqlConnection(connectionString);
                    connection.Open();
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"Cannot create database: {ex2.Message}");
                }
            }
        }

        private void CreateTables()
        {
            if (connection == null || connection.State != ConnectionState.Open) return;

            string createTasksTable = @"
                CREATE TABLE IF NOT EXISTS tasks (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    title VARCHAR(255) NOT NULL,
                    description TEXT,
                    reminder_date DATETIME,
                    status ENUM('pending', 'completed') DEFAULT 'pending',
                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                )";

            string createUserMemoryTable = @"
                CREATE TABLE IF NOT EXISTS user_memory (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    key_name VARCHAR(100) UNIQUE NOT NULL,
                    value TEXT,
                    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
                )";

            using (var cmd = new MySqlCommand(createTasksTable, connection))
            {
                cmd.ExecuteNonQuery();
            }
            using (var cmd = new MySqlCommand(createUserMemoryTable, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public int AddTask(string title, string description, DateTime? reminderDate = null)
        {
            if (connection == null || connection.State != ConnectionState.Open) return -1;

            string query = "INSERT INTO tasks (title, description, reminder_date) VALUES (@title, @description, @reminderDate)";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@reminderDate", reminderDate ?? (object)DBNull.Value);
                cmd.ExecuteNonQuery();
                return (int)cmd.LastInsertedId;
            }
        }

        public DataTable GetTasks(string status = null)
        {
            DataTable tasks = new DataTable();
            if (connection == null || connection.State != ConnectionState.Open) return tasks;

            string query = string.IsNullOrEmpty(status)
                ? "SELECT * FROM tasks ORDER BY created_at DESC"
                : "SELECT * FROM tasks WHERE status = @status ORDER BY created_at DESC";

            using (var cmd = new MySqlCommand(query, connection))
            {
                if (!string.IsNullOrEmpty(status))
                    cmd.Parameters.AddWithValue("@status", status);

                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    adapter.Fill(tasks);
                }
            }
            return tasks;
        }

        public bool UpdateTaskStatus(int taskId, string status)
        {
            if (connection == null || connection.State != ConnectionState.Open) return false;

            string query = "UPDATE tasks SET status = @status WHERE id = @id";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@id", taskId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool DeleteTask(int taskId)
        {
            if (connection == null || connection.State != ConnectionState.Open) return false;

            string query = "DELETE FROM tasks WHERE id = @id";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@id", taskId);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool SaveMemory(string key, string value)
        {
            if (connection == null || connection.State != ConnectionState.Open) return false;

            string query = "INSERT INTO user_memory (key_name, value) VALUES (@key, @value) ON DUPLICATE KEY UPDATE value = @value";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@key", key);
                cmd.Parameters.AddWithValue("@value", value);
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public string GetMemory(string key)
        {
            if (connection == null || connection.State != ConnectionState.Open) return null;

            string query = "SELECT value FROM user_memory WHERE key_name = @key";
            using (var cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@key", key);
                var result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }

        public void Close()
        {
            if (connection != null && connection.State == ConnectionState.Open)
                connection.Close();
        }
    }
}