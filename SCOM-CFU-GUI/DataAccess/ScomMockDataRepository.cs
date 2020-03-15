using SCOM_CFU_GUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SCOM_CFU_GUI.DataAccess
{
    class ScomMockDataRepository : IScomDataRepository
    {
        private int workflowCount;

        public async Task<bool> ConnectToScomAsync(string hostname)
        {
            await Task.Delay(1000);
            return true;
        }

        public async Task<List<ScomGroup>> GetScomGroupsAsync()
        {
            await Task.Delay(2000);

            List<ScomGroup> groupList = new List<ScomGroup>();

            groupList.Add(new ScomGroup(new Guid("b564aa9d2d674d05a3177658df88ce79"), "Windows 2016 Group"));
            groupList.Add(new ScomGroup(new Guid("f968f6d7da904fe2a75d6ae0af8eecf3"), "Windows 2012 Group"));
            groupList.Add(new ScomGroup(new Guid("9704a190648c455fb84381a4c91a1bce"), "Some Test Group"));

            return groupList;
        }

        public async Task<List<ScomMP>> GetScomManagementPacksAsync()
        {
            await Task.Delay(2000);

            List<ScomMP> mpList = new List<ScomMP>();
            List<ScomTarget> targetList = new List<ScomTarget>();
            List<ScomWorkflow> workflowList = new List<ScomWorkflow>();

            //create first dummy mp
            var mpId = new Guid("c870e9207d344864b7283ce135180ef2");
            var mpName = "Windows 2016 Mp";

            var targetId = new Guid("5e42b6ba571948b8975ad6bbd59bdd22");
            var targetName = "Logical Disk";

            workflowList.Add(new ScomWorkflow(new Guid("36d8369ddaf648d2a8c4e9e0d85d368a"), "Logical Drive Free Space", WorkflowType.Monitor));
            workflowList.Add(new ScomWorkflow(new Guid("4345c647e8184a74b4b5c1d65b675618"), "Logical Drive Corrupted", WorkflowType.Monitor));

            targetList.Add(new ScomTarget(targetId, targetName, mpId, new ObservableCollection<ScomWorkflow>(workflowList)));
            mpList.Add(new ScomMP(mpId, mpName, new ObservableCollection<ScomTarget>(targetList)));

            workflowList.Clear();
            targetList.Clear();

            //create 2nd dummy mp
            mpId = new Guid("c1b8ecab4423411d84bdaf8d0afb2996");
            mpName = "Windows 2012 Mp";

            targetId = new Guid("347a7bf237ab4b2397698428d41660f4");
            targetName = "Logical Disk";

            workflowList.Add(new ScomWorkflow(new Guid("a3fef6952716485cb3fc82a956788e11"), "Logical Drive Free Space", WorkflowType.Monitor));
            workflowList.Add(new ScomWorkflow(new Guid("e19bccbd528d45ac96b5ecc5a3625644"), "Logical Drive Corrupted", WorkflowType.Monitor));

            targetList.Add(new ScomTarget(targetId, targetName, mpId, new ObservableCollection<ScomWorkflow>(workflowList)));
            mpList.Add(new ScomMP(mpId, mpName, new ObservableCollection<ScomTarget>(targetList)));

            workflowList.Clear();
            targetList.Clear();

            //create 3rd dummy mp
            mpId = new Guid("4254fb437b1c4c24af082848450d75cc");
            mpName = "Custom Service Monitoring";

            targetId = new Guid("b5c6e16c0e954a87b764cc5189b115ae");
            targetName = "Pulse Application";

            workflowList.Add(new ScomWorkflow(new Guid("7a0dee22f0e04da2925660016e99b271"), "Pulse Service Monitor", WorkflowType.Monitor));
            workflowList.Add(new ScomWorkflow(new Guid("aa6183397ed441a5bd4d0812a0d6b142"), "Pulse Error Event 455", WorkflowType.Rule));

            targetList.Add(new ScomTarget(targetId, targetName, mpId, new ObservableCollection<ScomWorkflow>(workflowList)));
            mpList.Add(new ScomMP(mpId, mpName, new ObservableCollection<ScomTarget>(targetList)));

            workflowList.Clear();
            targetList.Clear();

            return mpList;
        }

        public string GetScomManagementGroupInfo()
        {
            return $"Loaded {workflowCount} workflows from 2 Management Packs";
        }
        public string GetScomManagementGroupName()
        {
            return "$MockData";
        }
    }
}
