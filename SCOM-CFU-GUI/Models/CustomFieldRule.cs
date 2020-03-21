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
        public Guid MpId { get; set; }
        public int DatasetId { get; set; }
        public Guid GroupId { get; set; }

        public CustomFieldRule(int id, Guid targetId, ConfigurationTargetType targetType, Guid mpId, int datasetId, Guid groupId)
        {
            Id = id;
            TargetId = targetId;
            TargetType = targetType;
            MpId = mpId;
            DatasetId = datasetId;
            GroupId = groupId;
        }
    }
}
