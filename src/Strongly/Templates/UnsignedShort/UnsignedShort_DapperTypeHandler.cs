
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
            ushort ushortValue => new TYPENAME(ushortValue),
            short shortValue when shortValue < ushort.MaxValue => new TYPENAME((ushort)shortValue),
            int intValue when intValue < ushort.MaxValue => new TYPENAME((ushort)intValue),
            long longValue when longValue < ushort.MaxValue => new TYPENAME((ushort)longValue),
            string stringValue when !string.IsNullOrEmpty(stringValue) && ushort.TryParse(stringValue, out var result) => new TYPENAME(result),
            _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
        };
    }
}
