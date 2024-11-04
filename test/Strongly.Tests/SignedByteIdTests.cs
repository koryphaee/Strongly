using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Strongly.IntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Strongly.IntegrationTests;

public class SignedByteIdTests
{
    [Fact]
    public void RecordHaveEmpty()
    {
        _ = SignedByteId.Empty;
    }

    [Fact]
    public void SameValuesAreEqual()
    {
        sbyte id = -123;
        var foo1 = new SignedByteId(id);
        var foo2 = new SignedByteId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0, SignedByteId.Empty.Value);
    }


    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new SignedByteId(-1);
        var foo2 = new SignedByteId(-2);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        sbyte id = -12;
        var same1 = new SignedByteId(id);
        var same2 = new SignedByteId(id);
        var different = new SignedByteId(3);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new SignedByteId(-23);

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object) bar, (object) foo);
    }

    [Fact]
    public void CanParseString()
    {
        sbyte value = -1;
        var foo = SignedByteId.Parse(value.ToString());
        var bar = new SignedByteId(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => SignedByteId.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = SignedByteId.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        sbyte value = -2;
        var result = SignedByteId.TryParse(value.ToString(), out var foo);
        var bar = new SignedByteId(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }


    [Fact]
    public void CanSerializeToByte_WithNewtonsoftJsonProvider()
    {
        var foo = new NewtonsoftJsonSignedByteId(-123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedByte = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedByte);
    }

    [Fact]
    public void CanSerializeToNullableByte_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId {Id = null};

        var json = NewtonsoftJsonSerializer.SerializeObject(entity);
        var deserialize =
            NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

    [Fact]
    public void CanSerializeToByte_WithSystemTextJsonProvider()
    {
        var foo = new SystemTextJsonSignedByteId(-123);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedByte = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedByte);
    }

    [Fact]
    public void CanDeserializeFromByte_WithNewtonsoftJsonProvider()
    {
        sbyte value = -123;
        var foo = new NewtonsoftJsonSignedByteId(value);
        var serializedByte = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonSignedByteId>(serializedByte);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromByte_WithSystemTextJsonProvider()
    {
        sbyte value = -123;
        var foo = new SystemTextJsonSignedByteId(value);
        var serializedByte = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonSignedByteId>(serializedByte);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToByte_WithBothJsonConverters()
    {
        var foo = new BothJsonSignedByteId(-123);

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedByte1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedByte2 = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo1, serializedByte1);
        Assert.Equal(serializedFoo2, serializedByte2);
    }

    [Fact]
    public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
    {
        var foo = new NoJsonSignedByteId(-123);

        var serialized = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + foo.Value + "}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = new NoJsonSignedByteId(-123);

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected = $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var foo = new NoConverterSignedByteId(-123);

        var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
        var systemText = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + foo.Value + "}";

        Assert.Equal(expected, newtonsoft);
        Assert.Equal(expected, systemText);
    }

    [Fact]
    public void WhenEfValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        var original = new TestEntity {Id = new EfCoreSignedByteId(-123)};
        using (var context = new TestDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities.Add(original);
            context.SaveChanges();
        }

        using (var context = new TestDbContext(options))
        {
            var all = context.Entities.ToList();
            var retrieved = Assert.Single(all);
            Assert.Equal(original.Id, retrieved.Id);
        }
    }

    [Fact]
    public async Task WhenDapperValueConverterUsesValueConverter()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var results = await connection.QueryAsync<DapperSignedByteId>("SELECT -123");

        var value = Assert.Single(results);
        Assert.Equal(new DapperSignedByteId(-123), value);
    }

    [Theory]
    [InlineData((sbyte) -123)]
    [InlineData("-123")]
    public void TypeConverter_CanConvertToAndFrom(object value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonSignedByteId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonSignedByteId>(id);
        Assert.Equal(new NoJsonSignedByteId(-123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableSignedByteId original = default;
        var other = ComparableSignedByteId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableSignedByteId original = default;
        var other = EquatableSignedByteId.Empty;

        var equals1 = (original as IEquatable<EquatableSignedByteId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableSignedByteId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsByteerfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothSignedByteId>>(BothSignedByteId.Empty);
        Assert.IsAssignableFrom<IComparable<BothSignedByteId>>(BothSignedByteId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableSignedByteId>>(EquatableSignedByteId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableSignedByteId>>(ComparableSignedByteId.Empty);

#pragma warning disable 184
        Assert.False(SignedByteId.Empty is IComparable<SignedByteId>);
        Assert.False(SignedByteId.Empty is IEquatable<SignedByteId>);
#pragma warning restore 184
    }


#if NET6_0_OR_GREATER
    [Fact]
    public void WhenConventionBasedEfValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ConventionsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ConventionsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities.Add(
                new TestEntity {Id = new EfCoreSignedByteId(-123)});
            context.SaveChanges();
        }

        using (var context = new ConventionsDbContext(options))
        {
            var all = context.Entities.ToList();
            Assert.Single(all);
        }
    }

    public class ConventionsDbContext : DbContext
    {
        public DbSet<TestEntity> Entities { get; set; }

        public ConventionsDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void ConfigureConventions(
            ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<EfCoreSignedByteId>()
                .HaveConversion<EfCoreSignedByteId.EfValueConverter>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TestEntity>(builder =>
                {
                    builder
                        .Property(x => x.Id)
                        .ValueGeneratedNever();
                });
        }
    }
#endif

#if NET5_0_OR_GREATER
    [Fact]
    public void CanShowImplementationTypeExample_WithSwaggerSchemaFilter()
    {
        var schemaGenerator = new Swashbuckle.AspNetCore.SwaggerGen.SchemaGenerator(
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaGeneratorOptions(),
            new Swashbuckle.AspNetCore.SwaggerGen.JsonSerializerDataContractResolver(
                new System.Text.Json.JsonSerializerOptions()));
        var provider = Microsoft.Extensions.DependencyInjection
            .ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(
                new Microsoft.Extensions.DependencyInjection.ServiceCollection());
        var schemaFilter =
            new Swashbuckle.AspNetCore.Annotations.AnnotationsSchemaFilter(provider);
        var schemaRepository = new Swashbuckle.AspNetCore.SwaggerGen.SchemaRepository();

        var idType = typeof(SwaggerSignedByteId);
        var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
        schemaFilter.Apply(schema,
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator,
                schemaRepository));

        Assert.Equal("integer", schema.Type);
        Assert.Equal(sbyte.MinValue, schema.Minimum);
        Assert.Equal(sbyte.MaxValue, schema.Maximum);
    }
#endif
    public class TestDbContext : DbContext
    {
        public DbSet<TestEntity> Entities { get; set; }

        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TestEntity>(builder =>
                {
                    builder
                        .Property(x => x.Id)
                        .HasConversion(new EfCoreSignedByteId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreSignedByteId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonSignedByteId? Id { get; set; }
    }
    
    [Fact]
    public void XorOperator()
    {
        SignedByteMath a = new(3);
        SignedByteMath b = new(10);
        SignedByteMath c = a ^ b;
        Assert.Equal(9, c.Value);
    }
}