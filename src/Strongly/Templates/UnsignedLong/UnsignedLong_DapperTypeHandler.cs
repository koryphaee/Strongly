
public class DapperTypeHandler : Dapper.SqlMapper.TypeHandler<TYPENAME>
{
    public override void SetValue(System.Data.IDbDataParameter parameter, TYPENAME value)
    {
        parameter.Value = value.Value;
    }

    public override TYPENAME Parse(object value)
    {
        return value switch
        {
            ulong ulongValue => new TYPENAME(ulongValue),
            long longValue => new TYPENAME((ulong) longValue),
            int intValue => new TYPENAME((ulong) intValue),
            short shortValue => new TYPENAME((ulong) shortValue),
            string stringValue when !string.IsNullOrEmpty(stringValue) && ulong.TryParse(stringValue, out var result) => new TYPENAME(result),
            _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
        };
    }
}
