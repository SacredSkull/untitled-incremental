using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Data.Sqlite;
using Dapper;

namespace Incremental.Database {
    public class DatabaseConnection : IDisposable {

        private string connectionString = @"Data Source=Assets/Data/Game.db; FailIfMissing=True";
        private readonly SqliteConnection connection;

        public DatabaseConnection() {
            connection = new SqliteConnection(connectionString);
            connection.Open();
        }

        /*
         * Get All methods
         */

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

        /*
         * More specific methods
         */

        public ICollection<Research> GetResearchDependencies(Research r) {
            const string sql = @"SELECT r.* FROM Research as r 
                                INNER JOIN(
	                                SELECT rJunction.ResearchDependencyID 
	                                FROM Research_Dependencies as rJunction 
	                                WHERE rJunction.ResearchID = @RID
                                ) as rJunction ON rJunction.ResearchDependencyID = r.ID;";
            return connection.Query<Research>(sql, new {RID = r.ID}).ToList();
        }

        public ICollection<Research> GetProjectResearch(Project p) {
            // This wizardry results in the deriving classes using the tables as they should, assuming the table name is _exactly_ equal to the class name.
            string sql = string.Format(@"SELECT r.* FROM Research as r INNER JOIN( SELECT junction.ResearchID FROM {0}_Research as junction WHERE junction.{0}ID = @PID) as junction ON junction.ResearchID = ID;", p.GetType().Name);
            return connection.Query<Research>(sql, new {PID = p.ID}).ToList();
        }

        public ICollection<Part> GetHardwareParts(HardwareProject hw) {
            const string sql = @"SELECT p.ID ID, p.Name name, p.Cost cost, hwr.Quantity quantity FROM Part as p INNER JOIN( SELECT hwr.PartID, hwr.Quantity FROM HardwareProject_Parts as hwr WHERE hwr.HardwareProjectID = @PID) as hwr ON hwr.PartID = ID;";
            return connection.Query<Part>(sql, new {PID = hw.ID}).ToList();
        }

        public ICollection<Research> GetPartResearch(Part p) {
            const string sql = @"SELECT r.* FROM Research as r 
                                INNER JOIN(
	                                SELECT rJunction.ResearchID 
	                                FROM Part_Research as rJunction 
	                                WHERE rJunction.PartID = @PID
                                ) as rJunction ON rJunction.ResearchID = r.ID;";
            return connection.Query<Research>(sql, new { PID = p.ID }).ToList();
        }

        public ICollection<string> GetTypes(Startable s) {
           string sql = string.Format(@"SELECT t.Name name FROM {0}Types as st INNER JOIN( SELECT stt.TypeID, stt.{0}ID FROM {0}_Type as swt WHERE stt.{0}ID = @SID)as stt ON stt.TypeID = st.ID;", s.GetType().ToString());
            return connection.Query<string>(sql, new { SID = s.ID }).ToList();
        }

        public ICollection<string> GetFields(Startable s) {
            string sql = string.Format(@"SELECT f.Name name FROM Field as f INNER JOIN( SELECT sf.FieldID, sf.{0}ID FROM {0}_Field as sf WHERE sf.{0}ID = @SID)as sf ON sf.FieldID = f.ID;", s.GetType().ToString());
            return connection.Query<String>(sql, new { SID = s.ID }).ToList();
        } 

        public void Dispose() {
            connection.Close();
        }
    }
}