using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCOM_CFU_GUI.DataAccess
{
    interface ICustomFieldDataRepository
    {
        Task GetCustomFieldDataSets();
        Task GetCustomFieldRules();
    }
}
