using System.ComponentModel;
using System.Globalization;

namespace YS.Knife.Query
{
    public class OrderByInfoTypeConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (value is string orderText) ? OrderByInfo.Parse(orderText) : base.ConvertFrom(context, culture, value);
        }

    }
}
