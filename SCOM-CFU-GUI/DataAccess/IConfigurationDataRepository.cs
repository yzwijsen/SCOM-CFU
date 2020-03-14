using SCOM_CFU_GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCOM_CFU_GUI.DataAccess
{
    interface IConfigurationDataRepository
    {
        List<CustomFieldDataSet> GetCustomFieldDataSets();
        List<CustomFieldRule> GetCustomFieldRules(Guid id);
        void SaveDataset(CustomFieldDataSet dataset);
        void SaveRule(CustomFieldRule rule);

    }
}
