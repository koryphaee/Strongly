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
            sbyte sbyteValue => new TYPENAME(sbyteValue),
            byte byteValue when byteValue < sbyte.MaxValue => new TYPENAME((sbyte)byteValue),
            short shortValue when shortValue < sbyte.MaxValue => new TYPENAME((sbyte)shortValue),
            int intValue when intValue < sbyte.MaxValue => new TYPENAME((sbyte)intValue),
            long longValue when longValue < sbyte.MaxValue => new TYPENAME((sbyte)longValue),
            string stringValue when !string.IsNullOrEmpty(stringValue) && sbyte.TryParse(stringValue, out var result) => new TYPENAME(result),
            _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
        };
    }
}
