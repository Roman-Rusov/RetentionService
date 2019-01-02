using Autofac;
using Common;
using Logging;

using RetentionService.Cleanup;
using RetentionService.Cleanup.Contracts;
using RetentionService.ConsoleApp.Configuration;
using RetentionService.FileSystemStorage;
using RetentionService.RetentionRules;

namespace RetentionService.ConsoleApp
{
    /// <summary>
    /// Represents the builder of a DI container.
    /// </summary>
    internal class DIContainerBuilder
    {
        /// <summary>
        /// Builds DI container.
        /// </summary>
        /// <returns> An instance of DI container. </returns>
        public IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<SystemClock>().As<ISystemClock>().SingleInstance();

            RegisterLogging(builder);
            RegisterConfiguration(builder);
            RegisterApplication<string>(builder);

            return builder.Build();
        }

        private static void RegisterLogging(ContainerBuilder builder)
        {
            var logConfig = LogConfigBuilder.Build();
            builder.RegisterModule(new LoggingModule(logConfig));
        }

        private static void RegisterConfiguration(ContainerBuilder builder)
        {
            builder.RegisterType<AppConfigBuilder>().AsSelf();

            builder
                .Register(ctx => ctx.Resolve<AppConfigBuilder>().Build())
                .SingleInstance();
        }

        private static void RegisterApplication<TResourceId>(ContainerBuilder builder)
        {
            RegisterStorage<TResourceId>(builder);
            RegisterRetentionPolicy(builder);

            builder.RegisterType<CleanupExecutor>().AsSelf();
            builder.RegisterType<App<TResourceId>>().As<IApp>();
        }

        private static void RegisterStorage<TResourceId>(ContainerBuilder builder)
        {
            builder
                .RegisterType<DirectoryFileStorageSettings>()
                .AsSelf()
                .WithParameter(
                    // Note: This approach is rather fragile.
                    (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "directoryPath",
                    (pi, ctx) => ctx.Resolve<AppConfig>().CleanupDirectoryPath);

            builder
                .RegisterType<DirectoryFileStorage>()
                .As<IResourceStorage<TResourceId>>();
        }

        private static void RegisterRetentionPolicy(ContainerBuilder builder) =>
            builder
                .Register(ctx => new RetentionPolicy(ctx.Resolve<AppConfig>().RetentionRules))
                .As<IResourceExpirationPolicy>();
    }
}
