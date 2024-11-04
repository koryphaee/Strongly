
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
            nuint intValue => new TYPENAME(intValue),
            int intValue => new TYPENAME((nuint)intValue),
            long longValue when longValue < int.MaxValue => new TYPENAME((nuint)longValue),
            string stringValue when !string.IsNullOrEmpty(stringValue) && nuint.TryParse(stringValue, out var result) => new TYPENAME((nuint)result),
            _ => throw new System.InvalidCastException($"Unable to cast object of type {value.GetType()} to TYPENAME"),
        };
    }
}
