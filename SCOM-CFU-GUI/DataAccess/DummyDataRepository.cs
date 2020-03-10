using SCOM_CFU_GUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCOM_CFU_GUI.DataAccess
{
    class DummyDataRepository : IScomDataRepository
    {
        public async Task ConnectToScomAsync(string hostname)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ScomGroup>> GetScomGroupsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<ScomMP>> GetScomManagementPacksAsync()
        {
            throw new NotImplementedException();
        }
    }
}
