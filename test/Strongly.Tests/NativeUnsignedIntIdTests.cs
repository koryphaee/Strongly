﻿using System;
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

public class NativeUnsignedIntIdTests
{
    [Fact]
    public void RecordHaveEmpty()
    {
        _ = RecordNativeUnsignedIntId.Empty;
    }

    [Fact]
    public void SameValuesAreEqual()
    {
        nuint id = 123;
        var foo1 = new NativeUnsignedIntId(id);
        var foo2 = new NativeUnsignedIntId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0u, NativeUnsignedIntId.Empty.Value);
    }


    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new NativeUnsignedIntId(1);
        var foo2 = new NativeUnsignedIntId(2);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        nuint id = 12;
        var same1 = new NativeUnsignedIntId(id);
        var same2 = new NativeUnsignedIntId(id);
        var different = new NativeUnsignedIntId(3);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new NativeUnsignedIntId(23);

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object) bar, (object) foo);
    }

    [Fact]
    public void CanParseString()
    {
        nuint value = 1;
        var foo = NativeUnsignedIntId.Parse(value.ToString());
        var bar = new NativeUnsignedIntId(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => NativeUnsignedIntId.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = NativeUnsignedIntId.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        nuint value = 2;
        var result = NativeUnsignedIntId.TryParse(value.ToString(), out var foo);
        var bar = new NativeUnsignedIntId(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }

    [Fact]
    public void CanSerializeToNativeUnsignedInt_WithNewtonsoftJsonProvider()
    {
        var foo = new NewtonsoftJsonNativeUnsignedIntId(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedNativeUnsignedInt = NewtonsoftJsonSerializer.SerializeObject((int) foo.Value);

        Assert.Equal(serializedFoo, serializedNativeUnsignedInt);
    }

    [Fact]
    public void CanSerializeToNullableNativeUnsignedInt_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId {Id = null};

        var json = NewtonsoftJsonSerializer.SerializeObject(entity);
        var deserialize =
            NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

    [Fact]
    public void CanSerializeToNativeUnsignedInt_WithSystemTextJsonProvider()
    {
        var foo = new SystemTextJsonNativeUnsignedIntId(123);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedNativeUnsignedInt = SystemTextJsonSerializer.Serialize((int) foo.Value);

        Assert.Equal(serializedFoo, serializedNativeUnsignedInt);
    }

    [Fact]
    public void CanDeserializeFromNativeUnsignedInt_WithNewtonsoftJsonProvider()
    {
        nuint value = 123;
        var foo = new NewtonsoftJsonNativeUnsignedIntId(value);
        var serializedNativeUnsignedInt = NewtonsoftJsonSerializer.SerializeObject((int) value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonNativeUnsignedIntId>(
                serializedNativeUnsignedInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromIntPtr_WithNewtonsoftJsonProvider()
    {
        var value = (UIntPtr) 123;
        var foo = new NewtonsoftJsonNativeUnsignedIntId(value);
        var serializedNativeUnsignedInt = NewtonsoftJsonSerializer.SerializeObject((int) value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonNativeUnsignedIntId>(
                serializedNativeUnsignedInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromNativeUnsignedInt_WithSystemTextJsonProvider()
    {
        nuint value = 123;
        var foo = new SystemTextJsonNativeUnsignedIntId(value);
        var serializedNativeUnsignedInt = SystemTextJsonSerializer.Serialize((int) value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonNativeUnsignedIntId>(serializedNativeUnsignedInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToNativeUnsignedInt_WithBothJsonConverters()
    {
        var foo = new BothJsonNativeUnsignedIntId(123);

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedNativeUnsignedInt1 = NewtonsoftJsonSerializer.SerializeObject((int) foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedNativeUnsignedInt2 = SystemTextJsonSerializer.Serialize((int) foo.Value);

        Assert.Equal(serializedFoo1, serializedNativeUnsignedInt1);
        Assert.Equal(serializedFoo2, serializedNativeUnsignedInt2);
    }

    [Fact]
    public void WhenEfValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        var original = new TestEntity {Id = new EfCoreNativeUnsignedIntId(123)};
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

        var results = await connection.QueryAsync<DapperNativeUnsignedIntId>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(new DapperNativeUnsignedIntId(123), value);
    }

    [Fact]
    public void TypeConverter_CanConvertToAndFromNInt()
    {
        nuint value = 123;
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonNativeUnsignedIntId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonNativeUnsignedIntId>(id);
        Assert.Equal(new NoJsonNativeUnsignedIntId(123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Theory]
    [InlineData(123u)]
    [InlineData("123")]
    public void TypeConverter_CanConvertToAndFrom(object value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonNativeUnsignedIntId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonNativeUnsignedIntId>(id);
        Assert.Equal(new NoJsonNativeUnsignedIntId(123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableNativeUnsignedIntId original = default;
        var other = ComparableNativeUnsignedIntId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableNativeUnsignedIntId original = default;
        var other = EquatableNativeUnsignedIntId.Empty;

        var equals1 = (original as IEquatable<EquatableNativeUnsignedIntId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableNativeUnsignedIntId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsNativeUnsignedInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothNativeUnsignedIntId>>(BothNativeUnsignedIntId.Empty);
        Assert.IsAssignableFrom<IComparable<BothNativeUnsignedIntId>>(BothNativeUnsignedIntId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableNativeUnsignedIntId>>(EquatableNativeUnsignedIntId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableNativeUnsignedIntId>>(ComparableNativeUnsignedIntId.Empty);

#pragma warning disable 184
        Assert.False(NativeUnsignedIntId.Empty is IComparable<NativeUnsignedIntId>);
        Assert.False(NativeUnsignedIntId.Empty is IEquatable<NativeUnsignedIntId>);
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
                new TestEntity {Id = new EfCoreNativeUnsignedIntId(123)});
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
                .Properties<EfCoreNativeUnsignedIntId>()
                .HaveConversion<EfCoreNativeUnsignedIntId.EfValueConverter>();
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

        var idType = typeof(SwaggerNativeUnsignedIntId);
        var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
        schemaFilter.Apply(schema,
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator,
                schemaRepository));

        Assert.Equal("integer", schema.Type);
        Assert.Equal("", schema.Format);
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
                        .HasConversion(new EfCoreNativeUnsignedIntId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreNativeUnsignedIntId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonNativeUnsignedIntId? Id { get; set; }
    }
    
    [Fact]
    public void XorOperator()
    {
        NativeUnsignedIntMath a = new(3);
        NativeUnsignedIntMath b = new(10);
        NativeUnsignedIntMath c = a ^ b;
        Assert.Equal(9u, c.Value);
    }
}