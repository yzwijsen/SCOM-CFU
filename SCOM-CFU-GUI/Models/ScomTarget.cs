using System;
using System.Collections.ObjectModel;

namespace SCOM_CFU_GUI.Models
{
    public class ScomTarget
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public ObservableCollection<ScomWorkflow> ScomWorkflows { get; set; }

        public ScomTarget(Guid id, string name, ObservableCollection<ScomWorkflow> scomWorkflows)
        {
            ID = id;
            Name = name;
            ScomWorkflows = scomWorkflows;
        }
    }
}
