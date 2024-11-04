namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.UnsignedLong)]
partial struct UnsignedLongId
{
}

[Strongly(backingType: StronglyType.UnsignedLong)]
partial record struct RecordUnsignedLongId
{
}

[Strongly(converters: StronglyConverter.None, backingType: StronglyType.UnsignedLong)]
public partial struct NoConverterUnsignedLongId
{
}

[Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.UnsignedLong)]
public partial struct NoJsonUnsignedLongId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.UnsignedLong)]
public partial struct NewtonsoftJsonUnsignedLongId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.UnsignedLong)]
public partial struct SystemTextJsonUnsignedLongId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
    backingType: StronglyType.UnsignedLong)]
public partial struct BothJsonUnsignedLongId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.UnsignedLong)]
public partial struct EfCoreUnsignedLongId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.UnsignedLong)]
public partial struct DapperUnsignedLongId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.UnsignedLong)]
public partial struct SwaggerUnsignedLongId
{
}
#endif

[Strongly(backingType: StronglyType.UnsignedLong,
    implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
public partial struct BothUnsignedLongId
{
}

[Strongly(backingType: StronglyType.UnsignedLong, implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableUnsignedLongId
{
}

[Strongly(backingType: StronglyType.UnsignedLong, implementations: StronglyImplementations.IComparable)]
public partial struct ComparableUnsignedLongId
{
}