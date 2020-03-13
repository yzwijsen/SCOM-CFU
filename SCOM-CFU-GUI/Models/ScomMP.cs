using System;
using System.Collections.ObjectModel;

namespace SCOM_CFU_GUI.Models
{
    public class ScomMP : IConfigurationTarget
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid MpId { get { return Id; } }
        public ConfigurationTargetType ConfigTargetType { get { return ConfigurationTargetType.ManagementPack; } }
        public ObservableCollection<ScomTarget> ScomTargets { get; set; }

        public ScomMP(Guid id, string name, ObservableCollection<ScomTarget> scomTargets)
        {
            Id = id;
            Name = name;
            ScomTargets = scomTargets;
        }
    }
}
