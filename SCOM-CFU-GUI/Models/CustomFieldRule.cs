using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCOM_CFU_GUI.Models
{
    class CustomFieldRule
    {
        public int Id { get; set; }
        public Guid TargetId { get; set; }
        public ConfigurationTargetType TargetType { get; set; }
        public int DatasetId { get; set; }
        public Guid GroupId { get; set; }
    }
}
