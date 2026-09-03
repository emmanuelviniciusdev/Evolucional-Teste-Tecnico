using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Escola.Aplicacao
{
    public sealed class IsoDateOnlyConverter : IsoDateTimeConverter
    {
        public IsoDateOnlyConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
            Culture = CultureInfo.InvariantCulture;
            DateTimeStyles = DateTimeStyles.None;
        }
    }
}
