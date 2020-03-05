using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCOM_CFU_GUI
{
    public class ScomTarget
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Dictionary<Guid, ScomWorkflow> ScomWorkflows { get; set; }
    }
}
