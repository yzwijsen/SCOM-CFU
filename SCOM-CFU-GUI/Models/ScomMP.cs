using System;
using System.Collections.ObjectModel;

namespace SCOM_CFU_GUI.Models
{
    public class ScomMP
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public ObservableCollection<ScomTarget> ScomTargets { get; set; }

        public ScomMP(Guid id, string name, ObservableCollection<ScomTarget> scomTargets)
        {
            ID = id;
            Name = name;
            ScomTargets = scomTargets;
        }
    }
}
