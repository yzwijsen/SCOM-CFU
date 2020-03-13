using System;

namespace SCOM_CFU_GUI.Models
{
    public class ScomWorkflow : IConfigurationTarget
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MpId { get; set; }
        public WorkflowType Type { get; set; }
        public ConfigurationTargetType ConfigTargetType { get { return ConfigurationTargetType.Workflow; } }
        
        public ScomWorkflow(Guid id, string name, WorkflowType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

    }
}
