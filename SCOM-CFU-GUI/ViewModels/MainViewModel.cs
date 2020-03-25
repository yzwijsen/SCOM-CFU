using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SCOM_CFU_GUI.DataAccess;
using SCOM_CFU_GUI.Models;

namespace SCOM_CFU_GUI.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        
        private IScomDataRepository scomDataRepo;
        private IConfigurationDataRepository configDataRepo;

        #region Properties
        private CustomFieldRule selectedRule;
        public CustomFieldRule SelectedRule
        {
            get { return selectedRule; }
            set
            {
                if (selectedRule != value)
                {
                    selectedRule = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<CustomFieldRule> rules;
        public ObservableCollection<CustomFieldRule> Rules
        {
            get { return rules; }
            set
            {
                rules = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<CustomFieldDataSet> datasets;
        public ObservableCollection<CustomFieldDataSet> Datasets
        {
            get { return datasets; }
            set
            {
                datasets = value;
                OnPropertyChanged();
            }
        }

        private IConfigurationTarget selectedConfigTarget;
        public IConfigurationTarget SelectedConfigTarget
        {
            get { return selectedConfigTarget; }
            set
            {
                if (selectedConfigTarget != value)
                {
                    selectedConfigTarget = value;
                    //update list of rules
                    GetConfigTargetRules();
                    OnPropertyChanged();
                }
            }
        }

        private string scomManagementGroupName;
        public string ScomManagementGroupName
        {
            get { return scomManagementGroupName; }
            set
            {
                scomManagementGroupName = value;
                OnPropertyChanged();
            }
        }

        private string scomManagementGroupInfo;
        public string ScomManagementGroupInfo
        {
            get { return scomManagementGroupInfo; }
            set
            {
                scomManagementGroupInfo = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ScomGroup> scomGroups;
        public ObservableCollection<ScomGroup> ScomGroups
        {
            get { return scomGroups; }
            set
            {
                scomGroups = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ScomMP> scomMPs;
        public ObservableCollection<ScomMP> ScomMPs
        {
            get { return scomMPs; }
            set
            {
                scomMPs = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
            IsConnectActionAvailable = false;
            IsInitActionInProgress = true;

            //set scom data repo
            scomDataRepo = SelectScomDataRepository(ScomHostname);
            configDataRepo = new ConfigurationSQLiteDataRepository();

            InitStatus = $"Connecting to {ScomHostname}...";
            var connected = await scomDataRepo.ConnectToScomAsync(ScomHostname);
            if (!connected)
            {
                InitStatus = "Failed to Connect";
                IsConnectActionAvailable = true;
                IsInitActionInProgress = false;
                return;
            }

            //Set connected management group name
            ScomManagementGroupName = scomDataRepo.GetScomManagementGroupName();

            InitStatus = "Getting SCOM Groups...";
            ScomGroups = new ObservableCollection<ScomGroup>(await scomDataRepo.GetScomGroupsAsync());

            InitStatus = "Getting SCOM Workflows...";
            ScomMPs = new ObservableCollection<ScomMP>(await scomDataRepo.GetScomManagementPacksAsync());

            Datasets = new ObservableCollection<CustomFieldDataSet>(configDataRepo.GetCustomFieldDataSets());

            InitStatus = "Finished";
            IsInitActionInProgress = false;

            //fire an event saying data init is finished, this allows the Init dialog window to close itself
            OnDataInitCompleted();

            //Collect management group info and update property
            ScomManagementGroupInfo = scomDataRepo.GetScomManagementGroupInfo();
        }

        private void GetConfigTargetRules()
        {
            if (SelectedConfigTarget == null)
            {
                return;
            }

            Rules = new ObservableCollection<CustomFieldRule>(configDataRepo.GetCustomFieldRules(SelectedConfigTarget.Id));
        }

        private IScomDataRepository SelectScomDataRepository(string hostname)
        {
            if (hostname == "$MockData")
            {
                //use mock data
                return new ScomMockDataRepository();
            }
            else
            {
                //use real scom data
                return new ScomSDKDataRepository();
            }
        }
    }
}
