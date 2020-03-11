using SCOM_CFU_GUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
            groupList.Add(new ScomGroup(Guid.NewGuid(), "Windows 2016 Group"));
            groupList.Add(new ScomGroup(Guid.NewGuid(), "Windows 2012 Group"));
            groupList.Add(new ScomGroup(Guid.NewGuid(), "Some Test Group"));

            return groupList;
        }

        public async Task<List<ScomMP>> GetScomManagementPacksAsync()
        {
            await Task.Delay(2000);

            List<ScomMP> mpList = new List<ScomMP>();
            List<ScomTarget> targetList = new List<ScomTarget>();
            List<ScomWorkflow> workflowList;

            //create first dummy mp
            workflowList = CreateDummyWorkflows(8, "windows test workflow");
            targetList.Add(new ScomTarget(Guid.NewGuid(), "Target A", new ObservableCollection<ScomWorkflow>(workflowList)));
            workflowList = CreateDummyWorkflows(3, "other windows test workflow");
            targetList.Add(new ScomTarget(Guid.NewGuid(), "Target B", new ObservableCollection<ScomWorkflow>(workflowList)));
            mpList.Add(new ScomMP(Guid.NewGuid(), "Windows 2016 MP", new ObservableCollection<ScomTarget>(targetList)));

            //create second dummy mp
            workflowList = CreateDummyWorkflows(45, "some workflow");
            targetList.Clear();
            targetList.Add(new ScomTarget(Guid.NewGuid(), "test target", new ObservableCollection<ScomWorkflow>(workflowList)));
            mpList.Add(new ScomMP(Guid.NewGuid(), "Wintel Test MP", new ObservableCollection<ScomTarget>(targetList)));

            return mpList;
        }

        private List<ScomWorkflow> CreateDummyWorkflows(int amount, string nameBase)
        {
            List<ScomWorkflow> workflowList = new List<ScomWorkflow>();

            for (int i = 0; i < amount; i++)
            {
                workflowList.Add(new ScomWorkflow(Guid.NewGuid(), $"{nameBase} Rule {i}", WorkflowType.Rule));
                workflowList.Add(new ScomWorkflow(Guid.NewGuid(), $"{nameBase} Monitor {i}", WorkflowType.Rule));
                workflowCount += 2;
            }

            return workflowList;
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
