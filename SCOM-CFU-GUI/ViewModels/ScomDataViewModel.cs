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

        public string InitStatus { get; private set; }

        private InitializeWindow iw;


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

        public ScomDataViewModel()
        {
            iw = new InitializeWindow();
            iw.ShowDialog();
            iw.DataContext = this;
            iw.hostnameTextBox.Text = ScomHostname;
            iw.connectButton.Command = this.ScomConnectCommand;
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

        public void InitializeScomDataGathering()
        {
            ScomHostname = iw.hostnameTextBox.Text;

            InitStatus = "Connecting...";
            if (ConnectToScom())
            {
                InitStatus = "Connected";
                GetScomWorkflows();
                GetScomGroups();
            }
            else
            {
                InitStatus = "Failed to connect.";
            }
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
