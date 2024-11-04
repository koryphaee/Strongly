
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
            uint uintValue => new TYPENAME(uintValue),
            int intValue when intValue < uint.MaxValue => new TYPENAME((uint)intValue),
            long longValue when longValue < uint.MaxValue => new TYPENAME((uint)longValue),
            string stringValue when !string.IsNullOrEmpty(stringValue) && uint.TryParse(stringValue, out var result) => new TYPENAME(result),
            _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
        };
    }
}
