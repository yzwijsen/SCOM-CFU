using System;

namespace SCOM_CFU_GUI.Models
{
    public class ScomWorkflow
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public WorkflowType Type { get; set; }

        public ScomWorkflow(Guid id, string name, WorkflowType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }

    }
}
