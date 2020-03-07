using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        private List<ScomMP> scomMPs;
        private List<ScomGroup> scomGroups;
        private List<ScomFlatWorkflow> scomFlatWorkflows;
        private ManagementGroup mg;

        #region Properties

        private bool isProgressbarVisible;
        public bool IsProgressbarVisible
        {
            get
            {
                return isProgressbarVisible;
            }
            set
            {
                isProgressbarVisible = value;
                OnPropertyChanged("IsProgressBarVisible");
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

        public ScomDataViewModel()
        {
            //iw = new InitializeWindow();
            //iw.ShowDialog();
        }

        ScomFlatWorkflow CreateFlatWorkflowItem(Guid id, string name, WorkflowType type, string targetText, ManagementPack mp)
        {
            //Get GUID out of Alert Target field
            var targetID = Guid.Parse(targetText.Substring(targetText.LastIndexOf('=') + 1));

            //get target class
            var target = mg.EntityTypes.GetClass(targetID);

            var workflowItem = new ScomFlatWorkflow
            {
                ID = id,
                Name = name,
                Type = type,
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
                var workflowItem = CreateFlatWorkflowItem(scomRule.Id, scomRule.DisplayName, WorkflowType.Rule, scomRule.Target.ToString(), mp);

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
                var workflowItem = CreateFlatWorkflowItem(scomMonitor.Id, scomMonitor.DisplayName, WorkflowType.Monitor, scomMonitor.Target.ToString(), mp);

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

        public void InitializeScomDataGathering()
        {
            IsConnectActionAvailable = false;
            IsProgressbarVisible = true;

            InitStatus = "Connecting...";
            ConnectToScom();
            if (mg == null)
            {
                InitStatus = "Failed to Connect";
                IsConnectActionAvailable = true;
                IsProgressbarVisible = false;
                return;
            }

            InitStatus = "Getting SCOM workflows...";
            GetScomWorkflows();

            InitStatus = "Getting SCOM Groups...";
            GetScomGroups();

            InitStatus = "Ordering data...";
            //build hierarchical data out of flat workflow data

            //InitStatus = "Connecting...";
            //if (ConnectToScom())
            //{
            //    InitStatus = "Connected";
            //    GetScomWorkflows();
            //    GetScomGroups();
            //}
            //else
            //{
            //    InitStatus = "Failed to connect.";
            //}
        }

        bool ConnectToScom()
        {
            try
            {
                mg = ManagementGroup.Connect(ScomHostname);

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
