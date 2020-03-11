﻿using System;
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
using SCOM_CFU_GUI.DataAccess;
using SCOM_CFU_GUI.Models;
using SCOM_CFU_GUI.Views;

namespace SCOM_CFU_GUI.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        
        private IScomDataRepository scomDataRepo;

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


        public MainViewModel()
        {
            scomDataRepo = new ScomDummyDataRepository();
            //scomDataRepo = new ScomSDKDataRepository();
        }

        public async Task InitializeScomDataGathering()
        {
            IsConnectActionAvailable = false;
            IsInitActionInProgress = true;

            InitStatus = $"Connecting to {ScomHostname} ...";
            var connected = await scomDataRepo.ConnectToScomAsync(ScomHostname);
            if (!connected)
            {
                InitStatus = "Failed to Connect";
                IsConnectActionAvailable = true;
                IsInitActionInProgress = false;
                return;
            }

            InitStatus = "Getting SCOM Groups...";
            ScomGroups = new ObservableCollection<ScomGroup>(await scomDataRepo.GetScomGroupsAsync());

            InitStatus = "Getting SCOM Workflows...";
            ScomMPs = new ObservableCollection<ScomMP>(await scomDataRepo.GetScomManagementPacksAsync());

            InitStatus = "Finished";
            IsInitActionInProgress = false;

            //fire an event saying data init is finished, this allows the Init dialog window to close itself
            OnDataInitCompleted();
        }
    }
}