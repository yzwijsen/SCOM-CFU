using System;
using System.Collections.ObjectModel;

namespace SCOM_CFU_GUI.Models
{
    public class ScomTarget : IConfigurationTarget
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MpId { get; set; }
        public ConfigurationTargetType ConfigTargetType { get { return ConfigurationTargetType.Target; } }

        public ObservableCollection<ScomWorkflow> ScomWorkflows { get; set; }

        public ScomTarget(Guid id, string name, Guid mpId, ObservableCollection<ScomWorkflow> scomWorkflows)
        {
            Id = id;
            Name = name;
            MpId = mpId;
            ScomWorkflows = scomWorkflows;
        }
    }
}
