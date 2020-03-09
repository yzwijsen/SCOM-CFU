using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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
