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

public class UnsignedShortIdTests
{
    [Fact]
    public void RecordHaveEmpty()
    {
        _ = RecordUnsignedShortId.Empty;
    }

    [Fact]
    public void SameValuesAreEqual()
    {
        ushort id = 123;
        var foo1 = new UnsignedShortId(id);
        var foo2 = new UnsignedShortId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0, UnsignedShortId.Empty.Value);
    }


    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new UnsignedShortId(1);
        var foo2 = new UnsignedShortId(2);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        ushort id = 12;
        var same1 = new UnsignedShortId(id);
        var same2 = new UnsignedShortId(id);
        var different = new UnsignedShortId(3);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new UnsignedShortId(23);

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object)bar, (object)foo);
    }

    [Fact]
    public void CanParseString()
    {
        ushort value = 1;
        var foo = UnsignedShortId.Parse(value.ToString());
        var bar = new UnsignedShortId(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => UnsignedShortId.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = UnsignedShortId.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        ushort value = 2;
        var result = UnsignedShortId.TryParse(value.ToString(), out var foo);
        var bar = new UnsignedShortId(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }


    [Fact]
    public void CanSerializeToShort_WithNewtonsoftJsonProvider()
    {
        var foo = new NewtonsoftJsonUnsignedShortId(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedShort = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedShort);
    }

    [Fact]
    public void CanSerializeToNullableShort_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId { Id = null };

        var json = NewtonsoftJsonSerializer.SerializeObject(entity);
        var deserialize =
            NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

    [Fact]
    public void CanSerializeToShort_WithSystemTextJsonProvider()
    {
        var foo = new SystemTextJsonUnsignedShortId(123);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedShort = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedShort);
    }

    [Fact]
    public void CanDeserializeFromShort_WithNewtonsoftJsonProvider()
    {
        ushort value = 123;
        var foo = new NewtonsoftJsonUnsignedShortId(value);
        var serializedShort = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonUnsignedShortId>(serializedShort);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromShort_WithSystemTextJsonProvider()
    {
        ushort value = 123;
        var foo = new SystemTextJsonUnsignedShortId(value);
        var serializedShort = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonUnsignedShortId>(serializedShort);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToShort_WithBothJsonConverters()
    {
        var foo = new BothJsonUnsignedShortId(123);

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedShort1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedShort2 = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo1, serializedShort1);
        Assert.Equal(serializedFoo2, serializedShort2);
    }

    [Fact]
    public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
    {
        var foo = new NoJsonUnsignedShortId(123);

        var serialized = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + foo.Value + "}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = new NoJsonUnsignedShortId(123);

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected = $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var foo = new NoConverterUnsignedShortId(123);

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

        var original = new TestEntity { Id = new EfCoreUnsignedShortId(123) };
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

        var results = await connection.QueryAsync<DapperUnsignedShortId>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(new DapperUnsignedShortId(123), value);
    }

    [Theory]
    [InlineData((ushort)123)]
    [InlineData("123")]
    public void TypeConverter_CanConvertToAndFrom(object value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonUnsignedShortId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonUnsignedShortId>(id);
        Assert.Equal(new NoJsonUnsignedShortId(123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableUnsignedShortId original = default;
        var other = ComparableUnsignedShortId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableUnsignedShortId original = default;
        var other = EquatableUnsignedShortId.Empty;

        var equals1 = (original as IEquatable<EquatableUnsignedShortId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableUnsignedShortId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsShorterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothUnsignedShortId>>(BothUnsignedShortId.Empty);
        Assert.IsAssignableFrom<IComparable<BothUnsignedShortId>>(BothUnsignedShortId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableUnsignedShortId>>(EquatableUnsignedShortId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableUnsignedShortId>>(ComparableUnsignedShortId.Empty);

#pragma warning disable 184
        Assert.False(UnsignedShortId.Empty is IComparable<UnsignedShortId>);
        Assert.False(UnsignedShortId.Empty is IEquatable<UnsignedShortId>);
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
                new TestEntity { Id = new EfCoreUnsignedShortId(123) });
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
                .Properties<EfCoreUnsignedShortId>()
                .HaveConversion<EfCoreUnsignedShortId.EfValueConverter>();
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

        var idType = typeof(SwaggerUnsignedShortId);
        var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
        schemaFilter.Apply(schema,
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator,
                schemaRepository));

        Assert.Equal("integer", schema.Type);
        Assert.Equal(ushort.MinValue, schema.Minimum);
        Assert.Equal(ushort.MaxValue, schema.Maximum);
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
                        .HasConversion(new EfCoreUnsignedShortId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreUnsignedShortId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonUnsignedShortId? Id { get; set; }
    }
}