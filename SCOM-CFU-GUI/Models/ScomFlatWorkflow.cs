using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCOM_CFU_GUI.Models
{

    /// <summary>
    /// This model holds the original flat data we get out of SCOM
    /// </summary>
    class ScomFlatWorkflow
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public WorkflowType Type { get; set; }
        public Guid TargetId { get; set; }
        public string TargetName { get; set; }
        public Guid MpId { get; set; }
        public string MpName { get; set; }

        public ScomFlatWorkflow(Guid id, string name, WorkflowType type, Guid targetId, string targetName, Guid mpId, string mpName)
        {
            Id = id;
            Name = name;
            Type = type;
            TargetId = targetId;
            TargetName = targetName;
            MpId = mpId;
            MpName = mpName;
        }

    }
}
