
public class EfValueConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<TYPENAME, uint>
{
    public EfValueConverter() : this(null) { }
    public EfValueConverter(Microsoft.EntityFrameworkCore.Storage.ValueConversion.ConverterMappingHints? mappingHints = null)
        : base(
            id => (uint) id.Value,
            value => new TYPENAME((nuint) value),
            mappingHints
        )
    { }
}
