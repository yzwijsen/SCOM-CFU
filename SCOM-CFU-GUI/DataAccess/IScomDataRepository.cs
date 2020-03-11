using SCOM_CFU_GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCOM_CFU_GUI.DataAccess
{
    interface IScomDataRepository
    {
        Task<bool> ConnectToScomAsync(string hostname);
        Task<List<ScomGroup>> GetScomGroupsAsync();
        Task<List<ScomMP>> GetScomManagementPacksAsync();
        string GetScomManagementGroupInfo();
    }
}
