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

public class UnsignedLongIdTests
{
    [Fact]
    public void RecordHaveEmpty()
    {
        _ = RecordUnsignedLongId.Empty;
    }

    [Fact]
    public void SameValuesAreEqual()
    {
        var id = 123UL;
        var foo1 = new UnsignedLongId(id);
        var foo2 = new UnsignedLongId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0UL, UnsignedLongId.Empty.Value);
    }


    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new UnsignedLongId(1L);
        var foo2 = new UnsignedLongId(2L);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        var id = 12UL;
        var same1 = new UnsignedLongId(id);
        var same2 = new UnsignedLongId(id);
        var different = new UnsignedLongId(3L);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new UnsignedLongId(23L);

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object) bar, (object) foo);
    }

    [Fact]
    public void CanParseString()
    {
        var value = 1UL;
        var foo = UnsignedLongId.Parse(value.ToString());
        var bar = new UnsignedLongId(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => UnsignedLongId.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = UnsignedLongId.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        var value = 2UL;
        var result = UnsignedLongId.TryParse(value.ToString(), out var foo);
        var bar = new UnsignedLongId(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }


    [Fact]
    public void CanSerializeToLong_WithNewtonsoftJsonProvider()
    {
        var foo = new NewtonsoftJsonUnsignedLongId(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedLong = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedLong);
    }

    [Fact]
    public void CanSerializeToNullableInt_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId {Id = null};

        var json = NewtonsoftJsonSerializer.SerializeObject(entity);
        var deserialize =
            NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

    [Fact]
    public void CanSerializeToLong_WithSystemTextJsonProvider()
    {
        var foo = new SystemTextJsonUnsignedLongId(123L);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedLong = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedLong);
    }

    [Fact]
    public void CanDeserializeFromLong_WithNewtonsoftJsonProvider()
    {
        var value = 123UL;
        var foo = new NewtonsoftJsonUnsignedLongId(value);
        var serializedLong = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonUnsignedLongId>(serializedLong);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromLong_WithSystemTextJsonProvider()
    {
        var value = 123UL;
        var foo = new SystemTextJsonUnsignedLongId(value);
        var serializedLong = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonUnsignedLongId>(serializedLong);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToLong_WithBothJsonConverters()
    {
        var foo = new BothJsonUnsignedLongId(123L);

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedLong1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedLong2 = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo1, serializedLong1);
        Assert.Equal(serializedFoo2, serializedLong2);
    }

    [Fact]
    public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
    {
        var foo = new NoJsonUnsignedLongId(123);

        var serialized = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + foo.Value + "}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = new NoJsonUnsignedLongId(123);

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected = $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var foo = new NoConverterUnsignedLongId(123);

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

        var original = new TestEntity {Id = new EfCoreUnsignedLongId(123)};
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

        var results = await connection.QueryAsync<DapperUnsignedLongId>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(value, new DapperUnsignedLongId(123));
    }

    [Theory]
    [InlineData(123UL)]
    [InlineData("123")]
    public void TypeConverter_CanConvertToAndFrom(object value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonUnsignedLongId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonUnsignedLongId>(id);
        Assert.Equal(new NoJsonUnsignedLongId(123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableUnsignedLongId original = default;
        var other = ComparableUnsignedLongId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableUnsignedLongId original = default;
        var other = EquatableUnsignedLongId.Empty;

        var equals1 = (original as IEquatable<EquatableUnsignedLongId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableUnsignedLongId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothUnsignedLongId>>(BothUnsignedLongId.Empty);
        Assert.IsAssignableFrom<IComparable<BothUnsignedLongId>>(BothUnsignedLongId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableUnsignedLongId>>(EquatableUnsignedLongId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableUnsignedLongId>>(ComparableUnsignedLongId.Empty);

#pragma warning disable 184
        Assert.False(UnsignedLongId.Empty is IComparable<UnsignedLongId>);
        Assert.False(UnsignedLongId.Empty is IEquatable<UnsignedLongId>);
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
                new TestEntity {Id = new EfCoreUnsignedLongId(123)});
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
                .Properties<EfCoreUnsignedLongId>()
                .HaveConversion<EfCoreUnsignedLongId.EfValueConverter>();
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

        var idType = typeof(SwaggerUnsignedLongId);
        var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
        schemaFilter.Apply(schema,
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator,
                schemaRepository));

        Assert.Equal("integer", schema.Type);
        Assert.Equal(ulong.MinValue, schema.Minimum);
        Assert.Equal(ulong.MaxValue, schema.Maximum);
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
                        .HasConversion(new EfCoreUnsignedLongId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreUnsignedLongId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonUnsignedLongId? Id { get; set; }
    }
    
    [Fact]
    public void XorOperator()
    {
        UnsignedLongMath a = new(3);
        UnsignedLongMath b = new(10);
        UnsignedLongMath c = a ^ b;
        Assert.Equal(9u, c.Value);
    }
}