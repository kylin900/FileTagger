using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FileTagger.Converters
{
    public class FileDataConverter : IValueConverter, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var value = (object[])values.Clone();
            string tags = (string)value[0];       

            string[] stringSeparators = new string[] { "\r\n" };
            value[0] = new List<string>(tags.Split(stringSeparators, StringSplitOptions.None));
            return value;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var list = (List<string>)value;              
            if(list != null && list.Count > 0)
            {
                string delimiter = "\r\n";
                return list.Aggregate((i, j) => i + delimiter + j);
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
