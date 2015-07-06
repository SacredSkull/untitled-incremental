using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using Dapper;

namespace Incremental.Database {
    public class DatabaseConnection : IDisposable {

        private string connectionString = @"Data Source=Assets/Data/Game.db; FailIfMissing=True";
        public readonly SqliteConnection connection;

        public DatabaseConnection() {
            connection = new SqliteConnection(connectionString);
            connection.Open();
        }

        public IEnumerable<Research> GetAllResearch() {
            const string sql = @"SELECT * FROM Research";
            return connection.Query<Research>(sql, null);
        }

        public IEnumerable<SoftwareProject> GetAllSoftwareProjects() {
            const string sql = @"SELECT * FROM SoftwareProject";
            return connection.Query<SoftwareProject>(sql, null);
        }

        public IEnumerable<HardwareProject> GetAllHardwareProjects() {
            const string sql = @"SELECT * FROM HardwareProject";
            return connection.Query<HardwareProject>(sql, null);
        } 


        public IEnumerable<Part> GetAllParts() {
            const string sql = @"SELECT * FROM Part";
            return connection.Query<Part>(sql, null);
        } 

        public void Dispose() {
            connection.Close();
        }
    }
}