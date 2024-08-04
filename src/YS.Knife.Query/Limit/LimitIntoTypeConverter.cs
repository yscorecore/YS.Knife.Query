using System.ComponentModel;

namespace YS.Knife.Query
{
    public class LimitIntoTypeConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return (value is string limttInfo) ? LimitInfo.Parse(limttInfo) : base.ConvertFrom(context, culture, value);
        }
    }
}
