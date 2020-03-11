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
    class BooleanToConnectionImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolean = false;
            if (value is bool) boolean = (bool)value;
            var connectImg = new BitmapImage();
            var disconnectImg = new BitmapImage();
            connectImg.UriSource = new Uri(@"/Icons/computer-network.png", UriKind.RelativeOrAbsolute);
            disconnectImg.UriSource = new Uri(@"/Icons/target.png", UriKind.RelativeOrAbsolute);
            return boolean ? connectImg : disconnectImg;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
