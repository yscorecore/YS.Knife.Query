using System;

namespace YS.Knife.Query.ExpressionConverters
{
    internal class NullableEnumConverter : EnumConverter, IExpressionConverter
    {
        public override bool CanConvertTo(Type fromType, Type toType)
        {
            var toType2 = Nullable.GetUnderlyingType(toType) ?? toType;
            var fromType2 = Nullable.GetUnderlyingType(fromType) ?? fromType;
            return base.CanConvertTo(toType2, fromType2);

        }


    }
}
