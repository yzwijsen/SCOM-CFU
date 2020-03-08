using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.Monitoring;
using SCOM_CFU_GUI.Models;
using SCOM_CFU_GUI.Views;

namespace SCOM_CFU_GUI.ViewModels
{
    class ScomDataViewModel : ViewModelBase
    {
        //private List<ScomMP> scomMPs;
        private List<ScomGroup> scomGroups;
        private ManagementGroup mg;

        #region Properties

        private ObservableCollection<ScomFlatWorkflow> scomFlatWorkflows;
        public ObservableCollection<ScomFlatWorkflow> ScomFlatWorkflows
        {
            get { return scomFlatWorkflows; }
            set
            {
                scomFlatWorkflows = value;
                OnPropertyChanged("ScomFlatWorkflows");
            }
        }

        private bool isInitActionInProgress;
        public bool IsInitActionInProgress
        {
            get
            {
                return isInitActionInProgress;
            }
            set
            {
                isInitActionInProgress = value;
                OnPropertyChanged("IsInitActionInProgress");
            }
        }
        private bool isConnectActionAvailable = true;
        public bool IsConnectActionAvailable
        {
            get
            {
                return isConnectActionAvailable;
            }
            set
            {
                isConnectActionAvailable = value;
                OnPropertyChanged("IsConnectActionAvailable");
            }
        }

        private string initStatus;
        public string InitStatus
        {
            get
            {
                return initStatus;
            }
            set
            {
                initStatus = value;
                OnPropertyChanged("InitStatus");
            }
        }


        public string ScomHostname
        {
            get
            {
                return Properties.Settings.Default.scomHost;
            }

            set
            {
                Properties.Settings.Default.scomHost = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged("ScomHostname");
            }
        }
        #endregion


        public async Task InitializeScomDataGathering()
        {
            IsConnectActionAvailable = false;
            IsInitActionInProgress = true;

            InitStatus = $"Connecting to {ScomHostname} ...";
            await ConnectToScom();
            if (mg == null || !mg.IsConnected)
            {
                InitStatus = "Failed to Connect";
                IsConnectActionAvailable = true;
                IsInitActionInProgress = false;
                return;
            }

            InitStatus = "Getting SCOM workflows...";
            await GetScomWorkflows();

            InitStatus = "Getting SCOM Groups...";
            await GetScomGroups();

            InitStatus = "Ordering data...";
            //build hierarchical data out of flat workflow data

            InitStatus = "Finished";
            IsInitActionInProgress = false;
        }

        async Task GetScomWorkflows()
        {
            //make sure we start with an empty list
            ScomFlatWorkflows = new ObservableCollection<ScomFlatWorkflow>();

            //Get All Rules
            IList<ManagementPackRule> scomRules = await Task.Run(() => mg.Monitoring.GetRules());

            foreach (var scomRule in scomRules)
            {
                //get management pack
                var mp = scomRule.GetManagementPack();

                //Gather additional data and create workflow item
                var workflowItem = CreateFlatWorkflowItem(scomRule.Id, scomRule.DisplayName, WorkflowType.Rule, scomRule.Target.ToString(), mp);

                //add the item to our list
                ScomFlatWorkflows.Add(workflowItem);
            }

            //Get All Monitors
            IList<ManagementPackMonitor> scomMonitors = await Task.Run(() => mg.Monitoring.GetMonitors());

            foreach (var scomMonitor in scomMonitors)
            {
                //get management pack
                var mp = scomMonitor.GetManagementPack();

                //Gather additional data and create workflow item
                var workflowItem = CreateFlatWorkflowItem(scomMonitor.Id, scomMonitor.DisplayName, WorkflowType.Monitor, scomMonitor.Target.ToString(), mp);

                //add the item to our list
                ScomFlatWorkflows.Add(workflowItem);
            }
        }

        ScomFlatWorkflow CreateFlatWorkflowItem(Guid id, string name, WorkflowType type, string targetText, ManagementPack mp)
        {
            //Get GUID out of Alert Target field
            var targetID = Guid.Parse(targetText.Substring(targetText.LastIndexOf('=') + 1));

            //get target class
            var target = mg.EntityTypes.GetClass(targetID);

            var workflowItem = new ScomFlatWorkflow(id, name, type, target.Id, target.DisplayName, mp.Id, mp.DisplayName);

            return workflowItem;
        }

        async Task GetScomGroups()
        {
            //make sure we start with an empty list
            scomGroups = new List<ScomGroup>();

            //Get All Groups
            IList<MonitoringObjectGroup> groups = await Task.Run(() => mg.EntityObjects.GetRootObjectGroups<MonitoringObjectGroup>(ObjectQueryOptions.Default));

            foreach (var group in groups)
            {
                scomGroups.Add(new ScomGroup(group.Id, group.DisplayName));
            }
        }


        async Task ConnectToScom()
        {
            try
            {
                mg = await Task.Run(() => ManagementGroup.Connect(ScomHostname));
                //NEED TO HANDLE EXCEPTION
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        #region ScomConnectCommand

        private ICommand scomConnectCommand;
        public ICommand ScomConnectCommand
        {
            get
            {
                if (scomConnectCommand == null)
                {
                    scomConnectCommand = new ScomConnectCommand(this);
                }
                return scomConnectCommand;
            }
            set
            {
                scomConnectCommand = value;
            }
        }

        #endregion

    }
}
