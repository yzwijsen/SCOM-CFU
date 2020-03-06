using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SCOM_CFU_GUI.Models
{
    public class ScomTarget
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public List<ScomWorkflow> ScomWorkflows { get; set; }
    }
}
