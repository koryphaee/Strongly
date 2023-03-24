﻿
public class EfValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TYPENAME, long>
{
    public EfValueConverter() : this(null) { }
    public EfValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints mappingHints = null)
        : base(
            id => id.Value,
            value => new TYPENAME(value),
            mappingHints
        )
    { }
}
