using BlazorApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AdoRepository.Tests
{    
    public class TestDatabase : IDisposable
    {
        private readonly string databaseName;

        public TestDatabase(string databaseName)
        {
            this.databaseName = databaseName;
            DropIfExists();

            //ConnectionString = $"Data Source=DESKTOP-HBECEJQ\\SQLEXPRESS;Initial Catalog=Blazor;Integrated Security=True";
            var createDatabaseSQL = $"CREATE DATABASE {databaseName} ON PRIMARY \r\n" +
            $"(NAME = {databaseName}, \r\n" +
            $"FILENAME = 'C:\\Program Files\\Microsoft SQL Server\\MSSQL15.SQLEXPRESS\\MSSQL\\DATA\\{databaseName}.mdf', \r\n" +
            "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) \r\n" +
            $"LOG ON (NAME = {databaseName}_Log, \r\n" +
            $"FILENAME = 'C:\\Program Files\\Microsoft SQL Server\\MSSQL15.SQLEXPRESS\\MSSQL\\DATA\\{databaseName}_Log.ldf', \r\n" +
            "SIZE = 1MB, \r\n" +
            "MAXSIZE = 5MB, \r\n" +
            "FILEGROWTH = 10%);";

            var createTableSql =$"CREATE TABLE [{databaseName}].[dbo].[UserData](\r\n" +
            "	[Id] [int] IDENTITY(1,1) NOT NULL,\r\n" +
            "	[Email] [nvarchar](320) NOT NULL,\r\n" +
            "	[PasswordHash] [nvarchar](72) NOT NULL,\r\n" +
            "PRIMARY KEY CLUSTERED \r\n" +
            "(\r\n" +
            "	[Id] ASC\r\n" +
            ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],\r\n" +
            " CONSTRAINT [UC_User] UNIQUE NONCLUSTERED \r\n" +
            "(\r\n" +
            "	[Email] ASC\r\n" +
            ")WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]\r\n" +
            ") ON [PRIMARY];";
            
            CreateDatabase(createDatabaseSQL);
            CreateDatabase(createTableSql);
        }

        public List<UserData> GetUserData(string connectionString)
        {
            var userData = new List<UserData>();
            using (var sqlConnection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT * FROM [dbo].[UserData]";
                command.Connection = sqlConnection;
                sqlConnection.Open();

                using (SqlDataReader rdr = command.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var item = new UserData();
                        item.Email = rdr["Email"].ToString();
                        item.PasswordHash = rdr["PasswordHash"].ToString();
                        userData.Add(item);
                    }
                }
            }

            return userData;
        }

        private void CreateDatabase(string commandSql)
        {
            try
            {
                using (var connection = new SqlConnection("Server=(local)\\SQLEXPRESS;Integrated security=SSPI;database=master"))
                using (var sqlCommand = new SqlCommand(commandSql, connection))
                {
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    Console.WriteLine("DataBase is Created Successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void DropIfExists()
        {
            var dropDatabaseSql =
            "if (select DB_ID('{0}')) is not null\r\n"
            + "begin\r\n"
            + "alter database [{0}] set offline with rollback immediate;\r\n"
            + "alter database [{0}] set online;\r\n"
            + "drop database [{0}];\r\n"
            + "end";

            var connectionString = $"Server=(local)\\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog={databaseName}";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var sqlToExecute = string.Format(dropDatabaseSql, connection.Database);
                    using (var command = new SqlCommand(sqlToExecute, connection))
                    {
                        Console.WriteLine($"Attempting to drop database {connection.Database}");
                        command.ExecuteNonQuery();
                        Console.WriteLine("Database is dropped");
                    }
                }
            }
            catch (SqlException sqlException)
            {
                if (sqlException.Message.StartsWith("Cannot open database"))
                {
                    Console.WriteLine("Database did not exist.");
                    return;
                }

                throw;
            }
        }

        public void Dispose()
        {
            DropIfExists();
        }
    }
}
