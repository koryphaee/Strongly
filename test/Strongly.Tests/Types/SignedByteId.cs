namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.SignedByte)]
public partial struct SignedByteId
{
}

[Strongly(backingType: StronglyType.SignedByte)]
public partial record struct RecordSignedByteId
{
}

[Strongly(converters: StronglyConverter.None, backingType: StronglyType.SignedByte)]
public partial struct NoConverterSignedByteId
{
}

[Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.SignedByte)]
public partial struct NoJsonSignedByteId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.SignedByte)]
public partial struct NewtonsoftJsonSignedByteId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.SignedByte)]
public partial struct SystemTextJsonSignedByteId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
    backingType: StronglyType.SignedByte)]
public partial struct BothJsonSignedByteId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.SignedByte)]
public partial struct EfCoreSignedByteId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.SignedByte)]
public partial struct DapperSignedByteId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.SignedByte)]
public partial struct SwaggerSignedByteId
{
}
#endif

[Strongly(backingType: StronglyType.SignedByte,
    implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
public partial struct BothSignedByteId
{
}

[Strongly(backingType: StronglyType.SignedByte, implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableSignedByteId
{
}

[Strongly(backingType: StronglyType.SignedByte, implementations: StronglyImplementations.IComparable)]
public partial struct ComparableSignedByteId
{
}