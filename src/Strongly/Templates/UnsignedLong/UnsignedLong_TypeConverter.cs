
class TYPENAMETypeConverter : System.ComponentModel.TypeConverter
{
    public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Type sourceType)
    {
        return sourceType == typeof(ulong) || sourceType == typeof(uint) || sourceType == typeof(ushort) || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
    {
        return value switch
        {
            ulong longValue => new TYPENAME(longValue),
            uint intValue => new TYPENAME(intValue),
            ushort shortValue => new TYPENAME(shortValue),
            string stringValue when !string.IsNullOrEmpty(stringValue) && ulong.TryParse(stringValue, out var result) => new TYPENAME(result),
            _ => base.ConvertFrom(context, culture, value),
        };
    }

    public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Type? sourceType)
    {
        return sourceType == typeof(ulong) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
    }

    public override object? ConvertTo(System.ComponentModel.ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object? value, System.Type destinationType)
    {
        if (value is TYPENAME idValue)
        {
            if (destinationType == typeof(ulong))
            {
                return idValue.Value;
            }

            if (destinationType == typeof(string))
            {
                return idValue.Value.ToString();
            }
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
