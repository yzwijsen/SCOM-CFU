using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;

namespace SCOM_CFU_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<Guid, ScomMP> scomMPs = new Dictionary<Guid, ScomMP>();

        public MainWindow()
        {
            InitializeComponent();
        }



        void GetScomWorkflows(ManagementGroup mg)
        {
            //Get All Rules
            IList<ManagementPackRule> scomRules = mg.Monitoring.GetRules();

            foreach (var scomRule in scomRules)
            {
                //Get GUID out of Alert Target field
                var ruleTargetText = scomRule.Target.ToString();
                var targetID = ruleTargetText.Substring(ruleTargetText.LastIndexOf('=') + 1);

                //get reference to target class
                var testTargetClass = mg.EntityTypes.GetClass(Guid.Parse(targetID));

                //get reference to management pack
                var mp = scomRule.GetManagementPack();

                ScomWorkflow workflowItem = new ScomWorkflow { ID = scomRule.Id, Name = scomRule.DisplayName, Type = WorkflowType.Rule };
                ScomTarget targetItem = new ScomTarget { ID = testTargetClass.Id, Name = testTargetClass.Name, ScomWorkflows = new Dictionary<Guid, ScomWorkflow>() };
                ScomMP mpItem = new ScomMP { ID = mp.Id, Name = mp.DisplayName, ScomTargets = new Dictionary<Guid, ScomTarget>() };

                if (!scomMPs.ContainsKey(mp.Id))
                {
                    scomMPs.Add(mp.Id, mpItem);
                }
            }

            //Get All Monitors
            IList<ManagementPackMonitor> scomMonitors = mg.Monitoring.GetMonitors();

            foreach (var scomMonitor in scomMonitors)
            {
                
            }
        }

        void GetScomGroups(ManagementGroup mg)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //a little hack to make sure the window is done rendering before we start gathering scom data.
            Dispatcher.BeginInvoke(new Action(() => GetScomData()), DispatcherPriority.ContextIdle, null);
        }



        private void MPList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MPList.SelectedIndex < 0)
            {
                return;
            }

            scomRulesAndMonitors.Clear();
            statusText.Text = "Getting Rules and Monitors...";
            //GetRulesAndMonitors(ScomMPs[MPList.SelectedIndex].ID);
            statusText.Text = "Done";
            AlertList.ItemsSource = scomRulesAndMonitors;
        }

        private void AlertList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // return if nothing is selected
            if (AlertList.SelectedIndex < 0)
            {
                txtDisplayName.Text = "";
                txtName.Text = "";
                txtId.Text = "";
                return;
            }

            txtDisplayName.Text = scomRulesAndMonitors[AlertList.SelectedIndex].Name;
            txtName.Text = scomRulesAndMonitors[AlertList.SelectedIndex].Name;
            txtId.Text = scomRulesAndMonitors[AlertList.SelectedIndex].ID.ToString();
        }

        private void GetScomData()
        {
            statusText.Text = "Connecting...";
            var mg = ConnectToScom();

            if (mg != null)
            {
                GetScomWorkflows(mg);
                GetScomGroups(mg);
            }
        }

        ManagementGroup ConnectToScom()
        {
            var scomHost = Properties.Settings.Default.scomHost;
            ManagementGroup mg;

            try
            {
                mg = ManagementGroup.Connect(scomHost);

                if (mg.IsConnected)
                {
                    Debug.Write("Connection succeeded.");
                    statusText.Text = "Connected";
                    return mg;
                }
                else
                {
                    throw new InvalidOperationException("Not connected to an SDK Service.");
                }
            }
            catch (Exception ex)
            {
                statusText.Text = "Failed to connect";
                MessageBox.Show(ex.Message);
            }
            return null;
        }
    }
}
