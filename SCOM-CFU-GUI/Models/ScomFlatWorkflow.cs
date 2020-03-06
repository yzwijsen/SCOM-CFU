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
        public Guid ID { get; set; }
        public string Name { get; set; }
        public Guid TargetID { get; set; }
        public string TargetName { get; set; }
        public Guid MPID { get; set; }
        public string MPName { get; set; }

    }
}
