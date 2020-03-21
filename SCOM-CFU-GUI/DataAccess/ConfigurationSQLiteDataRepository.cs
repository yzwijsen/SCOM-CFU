using SCOM_CFU_GUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SCOM_CFU_GUI.DataAccess
{
    class ConfigurationSQLiteDataRepository : IConfigurationDataRepository
    {
        public List<CustomFieldDataSet> GetCustomFieldDataSets()
        {
            using (var cn = new SQLiteConnection(LoadConnectionString()))
            {
                cn.Open();
                var query = "SELECT * FROM Datasets";
                var cmd = new SQLiteCommand(query, cn);
                List<CustomFieldDataSet> results = new List<CustomFieldDataSet>();

                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var id = Convert.ToInt32(rdr.GetInt64(0));
                        var name = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);
                        var cf1 = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2);
                        var cf2 = rdr.IsDBNull(3) ? string.Empty : rdr.GetString(3);
                        var cf3 = rdr.IsDBNull(4) ? string.Empty : rdr.GetString(4);
                        var cf4 = rdr.IsDBNull(5) ? string.Empty : rdr.GetString(5);
                        var cf5 = rdr.IsDBNull(6) ? string.Empty : rdr.GetString(6);
                        var cf6 = rdr.IsDBNull(7) ? string.Empty : rdr.GetString(7);
                        var cf7 = rdr.IsDBNull(8) ? string.Empty : rdr.GetString(8);
                        var cf8 = rdr.IsDBNull(9) ? string.Empty : rdr.GetString(9);
                        var cf9 = rdr.IsDBNull(10) ? string.Empty : rdr.GetString(10);
                        var cf10 = rdr.IsDBNull(11) ? string.Empty : rdr.GetString(11);


                        var dataset = new CustomFieldDataSet(id, name, cf1, cf2, cf3, cf4, cf5, cf6, cf7, cf8, cf9, cf10);
                        results.Add(dataset);
                    }
                    return results;
                }
            }
        }

        public List<CustomFieldRule> GetCustomFieldRules(Guid targetId)
        {
            using (var cn = new SQLiteConnection(LoadConnectionString()))
            {
                cn.Open();
                var query = $"select * from Rules where targetId = '{targetId.ToString()}'";
                //var query = $"select * from Rules";
                var cmd = new SQLiteCommand(query, cn);
                List<CustomFieldRule> results = new List<CustomFieldRule>();

                using (SQLiteDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        var id = Convert.ToInt32(rdr.GetInt64(0));
                        var target = rdr.IsDBNull(1) ? Guid.Empty : Guid.Parse(rdr.GetString(1));
                        var targetType = rdr.IsDBNull(2) ? ConfigurationTargetType.Workflow : (ConfigurationTargetType)rdr.GetInt64(2);
                        var mpId = rdr.IsDBNull(3) ? Guid.Empty : Guid.Parse(rdr.GetString(3));
                        var datasetId = rdr.IsDBNull(4) ? 0 : Convert.ToInt32(rdr.GetInt64(4));
                        var groupId = rdr.IsDBNull(5) ? Guid.Empty : Guid.Parse(rdr.GetString(5));

                        var rule = new CustomFieldRule(id, target, targetType, mpId, datasetId, groupId);
                        results.Add(rule);
                    }
                    return results;
                }
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
