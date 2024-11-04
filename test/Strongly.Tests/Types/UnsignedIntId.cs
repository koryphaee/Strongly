namespace Strongly.UnsignedIntegrationTests.Types;

[Strongly(backingType: StronglyType.UnsignedInt)]
public partial struct UnsignedIntId
{
}

[Strongly(backingType: StronglyType.UnsignedInt)]
public partial record struct RecordUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.None, backingType: StronglyType.UnsignedInt)]
public partial struct NoConverterUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.UnsignedInt)]
public partial struct NoJsonUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.UnsignedInt)]
public partial struct NewtonsoftJsonUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.UnsignedInt)]
public partial struct SystemTextJsonUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
    backingType: StronglyType.UnsignedInt)]
public partial struct BothJsonUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.UnsignedInt)]
public partial struct EfCoreUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.UnsignedInt)]
public partial struct DapperUnsignedIntId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.UnsignedInt)]
public partial struct SwaggerUnsignedIntId
{
}
#endif

[Strongly(backingType: StronglyType.UnsignedInt,
    implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
public partial struct BothUnsignedIntId
{
}

[Strongly(backingType: StronglyType.UnsignedInt, implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableUnsignedIntId
{
}

[Strongly(backingType: StronglyType.UnsignedInt, implementations: StronglyImplementations.IComparable)]
public partial struct ComparableUnsignedIntId
{
}