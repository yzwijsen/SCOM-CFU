using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCOM_CFU_GUI.Models
{
    public class ScomWorkflow
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public WorkflowType Type { get; set; }

    }
}
