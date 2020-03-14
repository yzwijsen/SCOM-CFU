using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SCOM_CFU_GUI.Models
{
    interface IConfigurationTarget
    {
        Guid Id { get;}
        Guid MpId { get;}
        string Name { get;}
        ConfigurationTargetType ConfigTargetType { get;}
    }
}
