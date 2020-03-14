using SCOM_CFU_GUI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SCOM_CFU_GUI.Views
{
    class ConfigurationTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ConfigurationTargetType))
            {
                return null;
            }
            var s = (ConfigurationTargetType)value;
            var img = new BitmapImage();
            img.BeginInit();

            if (s == ConfigurationTargetType.ManagementPack)
            {
                img.UriSource = new Uri(@"/Icons/box24.png", UriKind.RelativeOrAbsolute);
            }
            else if (s == ConfigurationTargetType.Target)
            {
                img.UriSource = new Uri(@"/Icons/target24.png", UriKind.RelativeOrAbsolute);
            }
            else
            {
                img.UriSource = new Uri(@"/Icons/system-monitor24.png", UriKind.RelativeOrAbsolute);
            }
            img.EndInit();
            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
