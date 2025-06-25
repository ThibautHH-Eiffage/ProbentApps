using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SqlExpressionTranslation = System.Func<System.Collections.Generic.IReadOnlyList<Microsoft.EntityFrameworkCore.Query.SqlExpressions.SqlExpression>, Microsoft.EntityFrameworkCore.Query.SqlExpressions.SqlExpression>;

namespace ProbentApps.Services.Database.Abstractions.Contexts;

public static class IServiceCollectionExtensions
{
    internal class MigrationsIdentityDbContext(DbContextOptions<MigrationsIdentityDbContext> options) : IdentityDbContext(options)
    {
        private static readonly SqlExpressionTranslation _dummyTranslation = _ => throw new NotImplementedException();

        public override SqlExpressionTranslation EncodeToASCII => _dummyTranslation;

        public override SqlExpressionTranslation ConcatenateByteArrays(ushort resultMaxLength) => _dummyTranslation;

        public override SqlExpressionTranslation HashBytesWithSHA(ushort size) => _dummyTranslation;
    }

    private class DbContextOptionsConfiguration<TContext>(Action<IServiceProvider, DbContextOptionsBuilder> configure) : IDbContextOptionsConfiguration<TContext> where TContext : DbContext
    {
        public void Configure(IServiceProvider serviceProvider, DbContextOptionsBuilder optionsBuilder)
            => configure(serviceProvider, optionsBuilder);
    }


    private const DynamicallyAccessedMemberTypes DbContextDynamicallyAccessedMemberTypes =
        DynamicallyAccessedMemberTypes.PublicConstructors
        | DynamicallyAccessedMemberTypes.NonPublicConstructors
        | DynamicallyAccessedMemberTypes.PublicProperties;

    internal const string MigrationsDbContextServiceKey = nameof(Migrations);

    public static IServiceCollection AddIdentityMigrationsDbContext(
        this IServiceCollection serviceCollection,
        Action<DbContextOptionsBuilder> optionsAction,
        ServiceLifetime contextLifetime = ServiceLifetime.Transient,
        ServiceLifetime optionsLifetime = ServiceLifetime.Transient) =>
        AddMigrationsDbContext<IdentityDbContext, MigrationsIdentityDbContext>(serviceCollection, optionsAction, contextLifetime, optionsLifetime);

    public static IServiceCollection AddMigrationsDbContext<[DynamicallyAccessedMembers(DbContextDynamicallyAccessedMemberTypes)] TContext>(
            this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Transient,
            ServiceLifetime optionsLifetime = ServiceLifetime.Transient)
        where TContext : DbContext =>
        AddMigrationsDbContext<TContext, TContext>(serviceCollection, optionsAction, contextLifetime, optionsLifetime);

    public static IServiceCollection AddMigrationsDbContext<TContextService, [DynamicallyAccessedMembers(DbContextDynamicallyAccessedMemberTypes)] TContextImplementation>(
            this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder>? optionsAction = null,
            ServiceLifetime contextLifetime = ServiceLifetime.Transient,
            ServiceLifetime optionsLifetime = ServiceLifetime.Transient)
        where TContextService : notnull
        where TContextImplementation : DbContext, TContextService =>
        AddMigrationsDbContext<TContextService, TContextImplementation>(
            serviceCollection,
            optionsAction == null
                ? null
                : (_, b) => optionsAction(b), contextLifetime, optionsLifetime);

    public static IServiceCollection AddMigrationsDbContext<[DynamicallyAccessedMembers(DbContextDynamicallyAccessedMemberTypes)] TContext>(
            this IServiceCollection serviceCollection,
            ServiceLifetime contextLifetime,
            ServiceLifetime optionsLifetime = ServiceLifetime.Transient)
        where TContext : DbContext =>
        AddMigrationsDbContext<TContext, TContext>(serviceCollection, contextLifetime, optionsLifetime);

    public static IServiceCollection AddMigrationsDbContext
        <TContextService, [DynamicallyAccessedMembers(DbContextDynamicallyAccessedMemberTypes)] TContextImplementation>(
            this IServiceCollection serviceCollection,
            ServiceLifetime contextLifetime,
            ServiceLifetime optionsLifetime = ServiceLifetime.Transient)
        where TContextImplementation : DbContext, TContextService
        where TContextService : class =>
        AddMigrationsDbContext<TContextService, TContextImplementation>(serviceCollection, (Action<IServiceProvider, DbContextOptionsBuilder>?)null, contextLifetime, optionsLifetime);

    public static IServiceCollection AddMigrationsDbContext<[DynamicallyAccessedMembers(DbContextDynamicallyAccessedMemberTypes)] TContext>(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction,
            ServiceLifetime contextLifetime = ServiceLifetime.Transient,
            ServiceLifetime optionsLifetime = ServiceLifetime.Transient)
        where TContext : DbContext =>
        AddMigrationsDbContext<TContext, TContext>(serviceCollection, optionsAction, contextLifetime, optionsLifetime);


    public static IServiceCollection AddMigrationsDbContext<TContextService, [DynamicallyAccessedMembers(DbContextDynamicallyAccessedMemberTypes)] TContextImplementation>(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction,
            ServiceLifetime contextLifetime = ServiceLifetime.Transient,
            ServiceLifetime optionsLifetime = ServiceLifetime.Transient)
        where TContextService : notnull
        where TContextImplementation : notnull, DbContext, TContextService
    {
        if (contextLifetime == ServiceLifetime.Singleton)
        {
            optionsLifetime = ServiceLifetime.Singleton;
        }

        if (optionsAction != null)
        {
            CheckContextConstructors<TContextImplementation>();
        }

        AddCoreServices<TContextImplementation>(serviceCollection, optionsAction, optionsLifetime);

        serviceCollection.TryAdd(new ServiceDescriptor(typeof(TContextService), MigrationsDbContextServiceKey, typeof(TContextImplementation), contextLifetime));

        if (typeof(TContextService) != typeof(TContextImplementation))
        {
            serviceCollection.TryAdd(
                new ServiceDescriptor(
                    typeof(TContextImplementation),
                    MigrationsDbContextServiceKey,
                    (p, k) => (TContextImplementation)p.GetRequiredKeyedService<TContextService>(k),
                    contextLifetime));
        }

        return serviceCollection;
    }

    public static IServiceCollection ConfigureDbContext<[DynamicallyAccessedMembers(DbContextDynamicallyAccessedMemberTypes)] TContext>(
            this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> optionsAction,
            ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        where TContext : DbContext =>
        ConfigureDbContext<TContext>(serviceCollection, (_, b) => optionsAction(b), optionsLifetime);

    public static IServiceCollection ConfigureDbContext<[DynamicallyAccessedMembers(DbContextDynamicallyAccessedMemberTypes)] TContext>(
            this IServiceCollection serviceCollection,
            Action<IServiceProvider, DbContextOptionsBuilder> optionsAction,
            ServiceLifetime optionsLifetime = ServiceLifetime.Singleton)
        where TContext : DbContext
    {
        serviceCollection.Add(
            new ServiceDescriptor(
                typeof(IDbContextOptionsConfiguration<TContext>),
                _ => new DbContextOptionsConfiguration<TContext>(optionsAction),
                optionsLifetime));

        return serviceCollection;
    }

    private static void AddCoreServices<TContextImplementation>(
        IServiceCollection serviceCollection,
        Action<IServiceProvider, DbContextOptionsBuilder>? optionsAction,
        ServiceLifetime optionsLifetime)
        where TContextImplementation : DbContext
    {
        serviceCollection.TryAddSingleton<ServiceProviderAccessor>();

        if (optionsAction != null)
        {
            ConfigureDbContext<TContextImplementation>(serviceCollection, optionsAction, optionsLifetime);
        }

        serviceCollection.TryAdd(
            new ServiceDescriptor(
                typeof(DbContextOptions<TContextImplementation>),
                CreateDbContextOptions<TContextImplementation>,
                optionsLifetime));

        serviceCollection.Add(
            new ServiceDescriptor(
                typeof(DbContextOptions),
                p => p.GetRequiredService<DbContextOptions<TContextImplementation>>(),
                optionsLifetime));
    }

    private static DbContextOptions<TContext> CreateDbContextOptions<TContext>(IServiceProvider applicationServiceProvider)
        where TContext : DbContext
    {
        var builder = new DbContextOptionsBuilder<TContext>(
            new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>()));

        builder.UseApplicationServiceProvider(applicationServiceProvider);

        foreach (var configuration in applicationServiceProvider.GetServices<IDbContextOptionsConfiguration<TContext>>())
        {
            configuration.Configure(applicationServiceProvider, builder);
        }

        return builder.Options;
    }

    private static void CheckContextConstructors<TContext>()
        where TContext : DbContext
    {
        var declaredConstructors = typeof(TContext).GetTypeInfo().DeclaredConstructors.ToList();
        if (declaredConstructors.Count == 1
            && declaredConstructors[0].GetParameters().Length == 0)
        {
            throw new ArgumentException(CoreStrings.DbContextMissingConstructor(typeof(TContext).ShortDisplayName()));
        }
    }
}
