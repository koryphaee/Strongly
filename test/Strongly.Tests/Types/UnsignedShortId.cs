namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.UnsignedShort)]
public partial struct UnsignedShortId
{
}

[Strongly(backingType: StronglyType.UnsignedShort)]
public partial record struct RecordUnsignedShortId
{
}

[Strongly(converters: StronglyConverter.None, backingType: StronglyType.UnsignedShort)]
public partial struct NoConverterUnsignedShortId
{
}

[Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.UnsignedShort)]
public partial struct NoJsonUnsignedShortId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.UnsignedShort)]
public partial struct NewtonsoftJsonUnsignedShortId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.UnsignedShort)]
public partial struct SystemTextJsonUnsignedShortId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
    backingType: StronglyType.UnsignedShort)]
public partial struct BothJsonUnsignedShortId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.UnsignedShort)]
public partial struct EfCoreUnsignedShortId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.UnsignedShort)]
public partial struct DapperUnsignedShortId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.UnsignedShort)]
public partial struct SwaggerUnsignedShortId
{
}
#endif

[Strongly(backingType: StronglyType.UnsignedShort,
    implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
public partial struct BothUnsignedShortId
{
}

[Strongly(backingType: StronglyType.UnsignedShort, implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableUnsignedShortId
{
}

[Strongly(backingType: StronglyType.UnsignedShort, implementations: StronglyImplementations.IComparable)]
public partial struct ComparableUnsignedShortId
{
}