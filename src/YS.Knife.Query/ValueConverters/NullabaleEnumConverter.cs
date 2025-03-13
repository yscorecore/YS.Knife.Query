using System;

namespace YS.Knife.Query.ValueConverters
{
    internal class NullabaleEnumConverter : EnumConverter, IValueConverter
    {
        public override bool CanConvertTo(Type fromType, Type toType)
        {
            var toType2 = Nullable.GetUnderlyingType(toType);
            var fromType2 = Nullable.GetUnderlyingType(fromType);
            return base.CanConvertTo(fromType2 ?? fromType, toType2 ?? toType);
        }
        public override object Convert(object fromValue, Type toType)
        {
            if (fromValue == null) return null;
            var toType2 = Nullable.GetUnderlyingType(toType);
            return base.Convert(fromValue, toType2 ?? toType);
        }
    }
}
