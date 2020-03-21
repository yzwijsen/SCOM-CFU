using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCOM_CFU_GUI.Models
{
    class CustomFieldDataSet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cf1 { get; set; }
        public string Cf2 { get; set; }
        public string Cf3 { get; set; }
        public string Cf4 { get; set; }
        public string Cf5 { get; set; }
        public string Cf6 { get; set; }
        public string Cf7 { get; set; }
        public string Cf8 { get; set; }
        public string Cf9 { get; set; }
        public string Cf10 { get; set; }

        public CustomFieldDataSet(int id, string name, string cf1 = "", string cf2 = "", string cf3 = "", string cf4 = "", string cf5 = "", string cf6 = "", string cf7 = "", string cf8 = "", string cf9 = "", string cf10 = "")
        {
            Id = id;
            Name = name;
            Cf1 = cf1;
            Cf2 = cf2;
            Cf3 = cf3;
            Cf4 = cf4;
            Cf5 = cf5;
            Cf6 = cf6;
            Cf7 = cf7;
            Cf8 = cf8;
            Cf9 = cf9;
            Cf10 = cf10;
        }
    }
}
