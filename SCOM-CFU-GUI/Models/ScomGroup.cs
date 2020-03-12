using System;

namespace SCOM_CFU_GUI.Models
{
    public class ScomGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public ScomGroup(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
