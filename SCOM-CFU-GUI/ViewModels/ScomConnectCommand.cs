using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SCOM_CFU_GUI.ViewModels
{
    class ScomConnectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private ScomDataViewModel scomVm;

        public ScomConnectCommand(ScomDataViewModel vm)
        {
            scomVm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            scomVm.InitializeScomDataGathering();
        }
    }
}
