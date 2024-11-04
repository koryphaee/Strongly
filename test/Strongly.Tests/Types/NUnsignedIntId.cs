namespace Strongly.IntegrationTests.Types;

[Strongly(backingType: StronglyType.NativeUnsignedInt)]
public partial struct NativeUnsignedIntId
{
}

[Strongly(backingType: StronglyType.NativeUnsignedInt)]
public partial record struct RecordNativeUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.None, backingType: StronglyType.NativeUnsignedInt)]
public partial struct NoConverterNativeUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.TypeConverter, backingType: StronglyType.NativeUnsignedInt)]
public partial struct NoJsonNativeUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson, backingType: StronglyType.NativeUnsignedInt)]
public partial struct NewtonsoftJsonNativeUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.SystemTextJson, backingType: StronglyType.NativeUnsignedInt)]
public partial struct SystemTextJsonNativeUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.NewtonsoftJson | StronglyConverter.SystemTextJson,
    backingType: StronglyType.NativeUnsignedInt)]
public partial struct BothJsonNativeUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.EfValueConverter, backingType: StronglyType.NativeUnsignedInt)]
public partial struct EfCoreNativeUnsignedIntId
{
}

[Strongly(converters: StronglyConverter.DapperTypeHandler, backingType: StronglyType.NativeUnsignedInt)]
public partial struct DapperNativeUnsignedIntId
{
}

#if NET5_0_OR_GREATER
[Strongly(converters: StronglyConverter.SwaggerSchemaFilter, backingType: StronglyType.NativeUnsignedInt)]
public partial struct SwaggerNativeUnsignedIntId
{
}
#endif

[Strongly(backingType: StronglyType.NativeUnsignedInt,
    implementations: StronglyImplementations.IEquatable | StronglyImplementations.IComparable)]
public partial struct BothNativeUnsignedIntId
{
}

[Strongly(backingType: StronglyType.NativeUnsignedInt, implementations: StronglyImplementations.IEquatable)]
public partial struct EquatableNativeUnsignedIntId
{
}

[Strongly(backingType: StronglyType.NativeUnsignedInt,
    implementations: StronglyImplementations.IComparable)]
public partial struct ComparableNativeUnsignedIntId
{
}

[Strongly(backingType: StronglyType.NativeUnsignedInt, math: StronglyMath.All)]
public partial struct NativeUnsignedIntMath
{
}