using System.ComponentModel;
using System.Globalization;

namespace YS.Knife.Query
{
    public class AggInfoTypeConverter: StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (value is string orderText) ? AggInfo.Parse(orderText) : base.ConvertFrom(context, culture, value);
        }
    }
}
