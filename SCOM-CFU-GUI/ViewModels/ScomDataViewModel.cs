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
        
        private ManagementGroup mg;

        #region Properties

        private ObservableCollection<ScomGroup> scomGroups;
        public ObservableCollection<ScomGroup> ScomGroups
        {
            get { return scomGroups; }
            set
            {
                scomGroups = value;
                OnPropertyChanged(nameof(ScomGroups));
            }
        }

        private ObservableCollection<ScomMP> scomMPs;
        public ObservableCollection<ScomMP> ScomMPs
        {
            get { return scomMPs; }
            set
            {
                scomMPs = value;
                OnPropertyChanged(nameof(ScomMPs));
            }
        }

        private ObservableCollection<ScomFlatWorkflow> scomFlatWorkflows;
        public ObservableCollection<ScomFlatWorkflow> ScomFlatWorkflows
        {
            get { return scomFlatWorkflows; }
            set
            {
                scomFlatWorkflows = value;
                OnPropertyChanged(nameof(ScomFlatWorkflows));
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
                OnPropertyChanged(nameof(IsInitActionInProgress));
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
                OnPropertyChanged(nameof(IsConnectActionAvailable));
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
                OnPropertyChanged(nameof(InitStatus));
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
                OnPropertyChanged(nameof(ScomHostname));
            }
        }
        #endregion

        #region Events / Commands

        public event EventHandler DataInitCompleted;
        void OnDataInitCompleted()
        {
            EventHandler handler = this.DataInitCompleted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        RelayCommand<object> connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                if (connectCommand == null)
                {
                    connectCommand = new RelayCommand<object>(async param => await this.InitializeScomDataGathering(), param => IsConnectActionAvailable);
                }
                return connectCommand;
            }
        }

        #endregion


        public async Task InitializeScomDataGathering()
        {
            //make sure we start with empty collections
            ScomFlatWorkflows = new ObservableCollection<ScomFlatWorkflow>();
            ScomMPs = new ObservableCollection<ScomMP>();
            scomGroups = new ObservableCollection<ScomGroup>();

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

            InitStatus = "Getting SCOM Rules...";
            await GetScomRules();

            InitStatus = "Getting SCOM Monitors...";
            await GetScomMonitors();

            InitStatus = "Getting SCOM Groups...";
            await GetScomGroups();

            InitStatus = "Ordering data...";
            BuildHierarchicalScomData();

            InitStatus = "Finished";
            IsInitActionInProgress = false;

            OnDataInitCompleted();
        }

        void BuildHierarchicalScomData()
        {

            //query and group flat workflow items by management pack
            var queryMp =
                from workflow in ScomFlatWorkflows
                group workflow by new { workflow.MpId, workflow.MpName } into g
                orderby g.Key.MpId
                select g;

            try
            {
                //foreach mp we find...
                foreach (var mpGroup in queryMp)
                {

                    //we query again to group all items by target
                    var queryTarget =
                        from item in mpGroup
                        group item by new { item.TargetId, item.TargetName } into g
                        orderby g.Key.TargetId
                        select g;

                    var targetObservableCollection = new ObservableCollection<ScomTarget>();

                    foreach (var targetGroup in queryTarget)
                    {
                        //targetGroup.Key.TargetId
                        //Here we have a list of workflows per target
                        var workflowObservableCollection = new ObservableCollection<ScomWorkflow>();
                        //workflowObservableCollection = targetGroup.ToObservableCollection<>
                        foreach (var flow in targetGroup)
                        {
                            workflowObservableCollection.Add(new ScomWorkflow(flow.Id, flow.Name, flow.Type));
                        }
                        targetObservableCollection.Add(new ScomTarget(targetGroup.Key.TargetId, targetGroup.Key.TargetName, workflowObservableCollection));
                    }

                    //MP level
                    ScomMPs.Add(new ScomMP(mpGroup.Key.MpId, mpGroup.Key.MpName, targetObservableCollection));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        async Task GetScomRules()
        {
            

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
        }

        async Task GetScomMonitors()
        {
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
            //Get All Groups
            IList<MonitoringObjectGroup> groups = await Task.Run(() => mg.EntityObjects.GetRootObjectGroups<MonitoringObjectGroup>(ObjectQueryOptions.Default));

            foreach (var group in groups)
            {
                scomGroups.Add(new ScomGroup(group.Id, group.DisplayName));
            }
        }


        async Task ConnectToScom()
        {
            await Task.Run(() =>
            {
                try
                {
                    mg = ManagementGroup.Connect(ScomHostname);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            });
        }
    }
}
