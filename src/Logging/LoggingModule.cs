using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Autofac.Core;
using Common;
using log4net;
using log4net.Config;

using Module = Autofac.Module;

namespace Logging
{
    /// <summary>
    /// The module that facilitates injecting properly configured log4net logger.
    /// </summary>
    /// <remarks>
    /// Please refer to the https://autofaccn.readthedocs.io/en/latest/examples/log4net.html page.
    /// </remarks>
    public class LoggingModule : Module
    {
        public const string DefaultConfigFileName = "log4net.config";

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingModule"/> class
        /// and applies default logger configuration.
        /// </summary>
        public LoggingModule() =>
            ConfigureLog4Net(DefaultConfigFileName);

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingModule"/> class
        /// and applies logger configuration specified by the <paramref name="config"/> argument.
        /// </summary>
        /// <param name="config">
        /// The configuration options required to configure log4net.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="config"/> is <see langword="null"/>.
        /// </exception>
        public LoggingModule(ILogConfiguration config)
        {
            AssertArg.NotNull(config, nameof(config));

            ConfigureLog4Net(config.ConfigFilePath);
        }

        private static void ConfigureLog4Net(string configFilePath)
        {
            var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());

            XmlConfigurator.Configure(repository, new FileInfo(configFilePath));
        }

        private static void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            e.Parameters = e.Parameters.Union(
                new[]
                {
                    new ResolvedParameter(
                        (p, i) => p.ParameterType == typeof(Common.ILog),
                        (p, i) => new Log4NetWrapper(LogManager.GetLogger(p.Member.DeclaringType))
                    ),
                });
        }

        protected override void AttachToComponentRegistration(
            IComponentRegistry componentRegistry,
            IComponentRegistration registration) =>
            registration.Preparing += OnComponentPreparing;
    }
}