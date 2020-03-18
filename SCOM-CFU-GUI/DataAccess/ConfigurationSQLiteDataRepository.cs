using Dapper;
using SCOM_CFU_GUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCOM_CFU_GUI.DataAccess
{
    class ConfigurationSQLiteDataRepository : IConfigurationDataRepository
    {
        public List<CustomFieldDataSet> GetCustomFieldDataSets()
        {
            using (IDbConnection cn = new SQLiteConnection(LoadConnectionString()))
            {
                cn.Open();
                var result = cn.Query<CustomFieldDataSet>("select * from Datasets", new DynamicParameters());
                return result.ToList();
            }
        }

        public List<CustomFieldRule> GetCustomFieldRules(Guid targetId)
        {
            using (IDbConnection cn = new SQLiteConnection(LoadConnectionString()))
            {
                cn.Open();
                var result = cn.Query<CustomFieldRule>("select * from Rules", new DynamicParameters());
                return result.ToList();

                //var parameters = new { TargetId = targetId.ToString() };
                //var sql = "select * from Rules where targetId = @TargetId";
                //var result = cn.Query<CustomFieldRule>(sql, parameters);
                //return result.ToList();
            }
        }

        public void SaveDataset(CustomFieldDataSet dataset)
        {
            throw new NotImplementedException();
        }

        public void SaveRule(CustomFieldRule rule)
        {
            throw new NotImplementedException();
        }

        public void CheckDatabaseExists()
        {
        }
        private string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
        private void CreateDatabase()
        {

        }
    }
}
