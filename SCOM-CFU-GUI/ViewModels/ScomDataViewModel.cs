using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Monitoring;
using SCOM_CFU_GUI.Models;

namespace SCOM_CFU_GUI.ViewModels
{
    class ScomDataViewModel
    {
        private List<ScomMP> scomMPs;
        private List<ScomGroup> scomGroups;
        private List<ScomFlatWorkflow> scomFlatWorkflows;
        private ManagementGroup mg;

        private string appStatus;


        public ScomDataViewModel()
        {
            InitializeScomDataGathering();
        }

        ScomFlatWorkflow CreateFlatWorkflowItem(Guid id, string name, string targetText, ManagementPack mp)
        {
            //Get GUID out of Alert Target field
            var targetID = Guid.Parse(targetText.Substring(targetText.LastIndexOf('=') + 1));

            //get target class
            var target = mg.EntityTypes.GetClass(targetID);

            var workflowItem = new ScomFlatWorkflow
            {
                ID = id,
                Name = name,
                TargetID = target.Id,
                TargetName = target.DisplayName,
                MPID = mp.Id,
                MPName = mp.DisplayName
            };

            return workflowItem;
        }

        void GetScomWorkflows()
        {
            //Get All Rules
            IList<ManagementPackRule> scomRules = mg.Monitoring.GetRules();
            scomFlatWorkflows = new List<ScomFlatWorkflow>();

            foreach (var scomRule in scomRules)
            {
                //get management pack
                var mp = scomRule.GetManagementPack();

                //Gather additional data and create workflow item
                var workflowItem = CreateFlatWorkflowItem(scomRule.Id, scomRule.DisplayName, scomRule.Target.ToString(), mp);

                //add the item to our list
                scomFlatWorkflows.Add(workflowItem);
            }

            //Get All Monitors
            IList<ManagementPackMonitor> scomMonitors = mg.Monitoring.GetMonitors();

            foreach (var scomMonitor in scomMonitors)
            {
                //get management pack
                var mp = scomMonitor.GetManagementPack();

                //Gather additional data and create workflow item
                var workflowItem = CreateFlatWorkflowItem(scomMonitor.Id, scomMonitor.DisplayName, scomMonitor.Target.ToString(), mp);

                //add the item to our list
                scomFlatWorkflows.Add(workflowItem);
            }
        }

        void GetScomGroups()
        {
            //Get All Groups
            IList<MonitoringObjectGroup> groups = mg.EntityObjects.GetRootObjectGroups<MonitoringObjectGroup>(ObjectQueryOptions.Default);

            foreach (var group in groups)
            {
                scomGroups.Add(new ScomGroup { ID = group.Id, Name = group.DisplayName });
            }
        }

        private void InitializeScomDataGathering()
        {
            appStatus = "Connecting...";
            if (ConnectToScom())
            {
                appStatus = "Connected";
                GetScomWorkflows();
                GetScomGroups();
            }
            else
            {
                appStatus = "Failed to connect.";
            }
        }

        bool ConnectToScom()
        {
            var scomHost = Properties.Settings.Default.scomHost;

            try
            {
                mg = ManagementGroup.Connect(scomHost);

                if (mg.IsConnected)
                {
                    return true;
                }
                else
                {
                    throw new InvalidOperationException("Unable to connect");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

    }
}
