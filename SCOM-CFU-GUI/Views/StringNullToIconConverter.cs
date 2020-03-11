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
    class StringNullToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            var img = new BitmapImage();
            img.BeginInit();

            if (string.IsNullOrWhiteSpace(s))
            {
                img.UriSource = new Uri(@"/Icons/target.png", UriKind.RelativeOrAbsolute);
            }
            else
            {
                img.UriSource = new Uri(@"/Icons/computer-network.png", UriKind.RelativeOrAbsolute);
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
