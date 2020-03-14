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
            throw new NotImplementedException();
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
