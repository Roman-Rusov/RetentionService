using Autofac;
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
        public IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();

            var logConfig = LogConfiguration.BuildConfig();
            builder.RegisterModule(new LoggingModule(logConfig));

            RegisterConfigSingleton(builder);
            RegisterStorage(builder);
            RegisterRetentionRules(builder);

            builder.RegisterType<CleanupExecutor>().AsSelf();
            builder.RegisterType<App>().AsSelf();

            return builder.Build();
        }

        private static void RegisterConfigSingleton(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigurationBuilder>().AsSelf();

            builder
                .Register(ctx => ctx.Resolve<ConfigurationBuilder>().BuildConfig())
                .SingleInstance();
        }

        private static void RegisterStorage(ContainerBuilder builder) =>
            builder
                .RegisterType<DirectoryFileStorage>()
                .As<IResourceStorage>()
                .WithParameter(
                    // Note: This approach is rather fragile.
                    (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "directoryPath",
                    (pi, ctx) => ctx.Resolve<Config>().CleanupDirectoryPath);

        private static void RegisterRetentionRules(ContainerBuilder builder) =>
            builder
                .Register(ctx => new RetentionRuleSet(ctx.Resolve<Config>().RetentionRules))
                .As<IStaleItemsDetector>();
    }
}
