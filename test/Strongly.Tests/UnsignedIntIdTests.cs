using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Strongly.IntegrationTests.Types;
using Strongly.UnsignedIntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Strongly.IntegrationTests;

public class UnsignedIntIdTests
{
    [Fact]
    public void RecordHaveEmpty()
    {
        _ = RecordUnsignedIntId.Empty;
    }

    [Fact]
    public void SameValuesAreEqual()
    {
        uint id = 123;
        var foo1 = new UnsignedIntId(id);
        var foo2 = new UnsignedIntId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0u, UnsignedIntId.Empty.Value);
    }


    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new UnsignedIntId(1);
        var foo2 = new UnsignedIntId(2);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        uint id = 12;
        var same1 = new UnsignedIntId(id);
        var same2 = new UnsignedIntId(id);
        var different = new UnsignedIntId(3);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new UnsignedIntId(23);

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object)bar, (object)foo);
    }

    [Fact]
    public void CanParseString()
    {
        uint value = 1;
        var foo = UnsignedIntId.Parse(value.ToString());
        var bar = new UnsignedIntId(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => UnsignedIntId.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = UnsignedIntId.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        uint value = 2;
        var result = UnsignedIntId.TryParse(value.ToString(), out var foo);
        var bar = new UnsignedIntId(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }


    [Fact]
    public void CanSerializeToInt_WithNewtonsoftJsonProvider()
    {
        var foo = new NewtonsoftJsonUnsignedIntId(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedInt = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedInt);
    }

    [Fact]
    public void CanSerializeToNullableInt_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId { Id = null };

        var json = NewtonsoftJsonSerializer.SerializeObject(entity);
        var deserialize =
            NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

    [Fact]
    public void CanSerializeToInt_WithSystemTextJsonProvider()
    {
        var foo = new SystemTextJsonUnsignedIntId(123);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedInt = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedInt);
    }

    [Fact]
    public void CanDeserializeFromInt_WithNewtonsoftJsonProvider()
    {
        uint value = 123;
        var foo = new NewtonsoftJsonUnsignedIntId(value);
        var serializedInt = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonUnsignedIntId>(serializedInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromInt_WithSystemTextJsonProvider()
    {
        uint value = 123;
        var foo = new SystemTextJsonUnsignedIntId(value);
        var serializedInt = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonUnsignedIntId>(serializedInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToInt_WithBothJsonConverters()
    {
        var foo = new BothJsonUnsignedIntId(123);

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedInt1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedInt2 = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo1, serializedInt1);
        Assert.Equal(serializedFoo2, serializedInt2);
    }

    [Fact]
    public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
    {
        var foo = new NoJsonUnsignedIntId(123);

        var serialized = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + foo.Value + "}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = new NoJsonUnsignedIntId(123);

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected = $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var foo = new NoConverterUnsignedIntId(123);

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

        var original = new TestEntity { Id = new EfCoreUnsignedIntId(123) };
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

        var results = await connection.QueryAsync<DapperUnsignedIntId>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(new DapperUnsignedIntId(123), value);
    }

    [Theory]
    [InlineData(123u)]
    [InlineData("123")]
    public void TypeConverter_CanConvertToAndFrom(object value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonUnsignedIntId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonUnsignedIntId>(id);
        Assert.Equal(new NoJsonUnsignedIntId(123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableUnsignedIntId original = default;
        var other = ComparableUnsignedIntId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableUnsignedIntId original = default;
        var other = EquatableUnsignedIntId.Empty;

        var equals1 = (original as IEquatable<EquatableUnsignedIntId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableUnsignedIntId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothUnsignedIntId>>(BothUnsignedIntId.Empty);
        Assert.IsAssignableFrom<IComparable<BothUnsignedIntId>>(BothUnsignedIntId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableUnsignedIntId>>(EquatableUnsignedIntId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableUnsignedIntId>>(ComparableUnsignedIntId.Empty);

#pragma warning disable 184
        Assert.False(UnsignedIntId.Empty is IComparable<UnsignedIntId>);
        Assert.False(UnsignedIntId.Empty is IEquatable<UnsignedIntId>);
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
                new TestEntity { Id = new EfCoreUnsignedIntId(123) });
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
                .Properties<EfCoreUnsignedIntId>()
                .HaveConversion<EfCoreUnsignedIntId.EfValueConverter>();
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

        var idType = typeof(SwaggerUnsignedIntId);
        var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
        schemaFilter.Apply(schema,
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator,
                schemaRepository));

        Assert.Equal("integer", schema.Type);
        Assert.Equal(uint.MinValue, schema.Minimum);
        Assert.Equal(uint.MaxValue, schema.Maximum);
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
                        .HasConversion(new EfCoreUnsignedIntId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreUnsignedIntId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonUnsignedIntId? Id { get; set; }
    }
    
    [Fact]
    public void XorOperator()
    {
        UnsignedIntMath a = new(3);
        UnsignedIntMath b = new(10);
        UnsignedIntMath c = a ^ b;
        Assert.Equal(9u, c.Value);
    }
}