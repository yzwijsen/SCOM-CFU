using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCOM_CFU_GUI
{
    public enum AlertSourceType
    {
        Rule,
        Monitor
    }

    public class ScomRuleMonitor
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public Guid ID { get; set; }
        public AlertSourceType Type { get; set; }

    }
}
