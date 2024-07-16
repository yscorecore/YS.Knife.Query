using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Query.ValueConverters
{
    internal class NullableParserConverter : ParserConverter
    {
        public override bool CanConvertTo(Type fromType, Type toType)
        {
            var toType2 = Nullable.GetUnderlyingType(toType);
            return toType2 != null && base.CanConvertTo(fromType, toType2);
        }

        public override object Convert(object fromValue, Type toType)
        {
            if (fromValue == null) return null;

            var toType2 = Nullable.GetUnderlyingType(toType);
            return base.Convert(fromValue, toType2);
        }
    }
}
