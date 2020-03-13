using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Monitoring;
using SCOM_CFU_GUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SCOM_CFU_GUI.DataAccess
{
    class ScomSDKDataRepository : IScomDataRepository
    {
        ManagementGroup mg;

        List<ScomMP> scomMPs;
        List<ScomFlatWorkflow> scomFlatWorkflows;
        List<ScomGroup> scomGroups = new List<ScomGroup>();

        int scomWorkflowCount;

        public async Task<bool> ConnectToScomAsync(string hostname)
        {
            await Task.Run(() =>
            {
                try
                {
                    mg = ManagementGroup.Connect(hostname);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            });

            if (mg == null || !mg.IsConnected)
            {
                return false;
            }
            return true;

        }
        public async Task<List<ScomGroup>> GetScomGroupsAsync()
        {
            scomGroups = new List<ScomGroup>();

            //Get All Groups
            IList<MonitoringObjectGroup> groups = await Task.Run(() => mg.EntityObjects.GetRootObjectGroups<MonitoringObjectGroup>(ObjectQueryOptions.Default));

            foreach (var group in groups)
            {
                scomGroups.Add(new ScomGroup(group.Id, group.DisplayName));
            }
            return scomGroups;
        }

        public async Task<List<ScomMP>> GetScomManagementPacksAsync()
        {
            scomFlatWorkflows = new List<ScomFlatWorkflow>();
            scomMPs = new List<ScomMP>();

            await GetScomRules();
            await GetScomMonitors();
            await BuildHierarchicalScomData();

            //clear flat workflow list since we don't need it anymore
            scomFlatWorkflows.Clear();

            return scomMPs;
        }

        private async Task BuildHierarchicalScomData()
        {

            //group flat workflow items by management pack
            var queryMp = await Task.Run(() =>
                from workflow in scomFlatWorkflows
                group workflow by new { workflow.MpId, workflow.MpName } into g
                orderby g.Key.MpName
                select g);

            try
            {
                //foreach mp we find...
                foreach (var mpGroup in queryMp)
                {

                    //we query again to group all items by target
                    var queryTarget = await Task.Run(() =>
                        from item in mpGroup
                        group item by new { item.TargetId, item.TargetName } into g
                        orderby g.Key.TargetName
                        select g);

                    var targetList = new ObservableCollection<ScomTarget>();

                    //foreach target we find..
                    foreach (var targetGroup in queryTarget)
                    {
                        //we add the target to a list
                        var workflowList = new ObservableCollection<ScomWorkflow>();

                        //foreach workflow in the group...
                        foreach (var flow in targetGroup)
                        {
                            //we add the workflow to the targets workflow list
                            workflowList.Add(new ScomWorkflow(flow.Id, flow.Name, flow.Type));
                        }
                        //we add the current target + it's list of workflows to the target list
                        targetList.Add(new ScomTarget(targetGroup.Key.TargetId, targetGroup.Key.TargetName,mpGroup.Key.MpId, workflowList));
                    }

                    //we add the current mp + it's list of targets to the mp list
                    scomMPs.Add(new ScomMP(mpGroup.Key.MpId, mpGroup.Key.MpName, targetList));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private async Task GetScomRules()
        {

            //Get All Rules
            IList<ManagementPackRule> scomRules = await Task.Run(() => mg.Monitoring.GetRules());

            foreach (var scomRule in scomRules)
            {
                //don't include performance collection rules as they can't raise alerts
                //if (scomRule.Category == ManagementPackCategoryType.PerformanceCollection)
                //{
                //    continue;
                //}

                //get management pack
                var mp = scomRule.GetManagementPack();

                //Gather additional data and create workflow item
                var workflowItem = CreateFlatWorkflowItem(scomRule.Id, scomRule.DisplayName, WorkflowType.Rule, scomRule.Target.ToString(), mp);

                //add the item to our list
                scomFlatWorkflows.Add(workflowItem);
            }
        }

        private async Task GetScomMonitors()
        {
            //Get All Monitors
            IList<ManagementPackMonitor> scomMonitors = await Task.Run(() => mg.Monitoring.GetMonitors());

            foreach (var scomMonitor in scomMonitors)
            {
                //don't include performance collection monitors as they can't raise alerts
                //if (scomMonitor.Category == ManagementPackCategoryType.PerformanceCollection)
                //{
                //    continue;
                //}

                //get management pack
                var mp = scomMonitor.GetManagementPack();

                //Gather additional data and create workflow item
                var workflowItem = CreateFlatWorkflowItem(scomMonitor.Id, scomMonitor.DisplayName, WorkflowType.Monitor, scomMonitor.Target.ToString(), mp);

                //add the item to our list
                scomFlatWorkflows.Add(workflowItem);
            }
        }

        private ScomFlatWorkflow CreateFlatWorkflowItem(Guid id, string name, WorkflowType type, string targetText, ManagementPack mp)
        {
            //Get GUID out of Alert Target field
            var targetID = Guid.Parse(targetText.Substring(targetText.LastIndexOf('=') + 1));

            //get target class
            var target = mg.EntityTypes.GetClass(targetID);

            var workflowItem = new ScomFlatWorkflow(id, name, type, target.Id, target.DisplayName, mp.Id, mp.DisplayName);
            scomWorkflowCount++;

            return workflowItem;
        }

        public string GetScomManagementGroupInfo()
        {
            return $"Loaded {scomWorkflowCount} workflows from {scomMPs.Count()} Management Packs";
        }

        public string GetScomManagementGroupName()
        {
            return mg.Name;
        }
    }
}
