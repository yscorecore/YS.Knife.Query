using System.ComponentModel;
using System.Globalization;

namespace YS.Knife.Query
{
    public class FilterInfoTypeConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return value is string ? FilterInfo.Parse(value as string, culture) : base.ConvertFrom(context, culture, value);
        }
    }
}
