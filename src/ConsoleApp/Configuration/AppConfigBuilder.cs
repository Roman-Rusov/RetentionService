using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Common;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

using RetentionService.RetentionRules;

namespace RetentionService.ConsoleApp.Configuration
{
    /// <summary>
    /// Represents the builder of application configuration.
    /// </summary>
    public class AppConfigBuilder
    {
        private const string ConfigName = nameof(AppConfig);
        private const string CleanupDirectoryPathSettingName = nameof(AppConfig.CleanupDirectoryPath);
        private const string RetentionRulesSettingName = nameof(AppConfig.RetentionRules);

        private const string AppConfigRootSectionName = "cleanup";

        private const string NotSpecifiedPhrase = "<not specified>";

        [CanBeNull] private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigBuilder"/> class.
        /// </summary>
        public AppConfigBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigBuilder"/> class.
        /// </summary>
        /// <param name="log">
        /// A log where to write log messages into.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="log"/> is <see langword="null"/>.
        /// </exception>
        public AppConfigBuilder([NotNull] ILog log) : this()
        {
            AssertArg.NotNull(log, nameof(log));

            _log = log;
        }

        /// <summary>
        /// Reads application configuration settings and builds a new instance
        /// of the <see cref="AppConfig"/> class.
        /// </summary>
        /// <returns> An instance of <see cref="AppConfig"/> class. </returns>
        [NotNull]
        public AppConfig Build()
        {
            try
            {
                var config = BuildConfig();

                // Note: Unfortunately, config binding doesn't support non-parameterless constructors.
                var cleanupDirectoryPath = ReadCleanupDirectoryPath(config);
                var retentionRules = ReadRetentionRules(config);

                return new AppConfig(
                    cleanupDirectoryPath,
                    retentionRules);
            }
            catch (Exception ex)
            {
                _log?.Error("An application configuration error occurred.", ex);

                throw;
            }
        }

        private static IConfigurationRoot BuildConfig()
        {
            var assemblyLocation = Assembly.GetEntryAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);

            return new ConfigurationBuilder()
                .SetBasePath(assemblyDirectory)
                .AddJsonFile("app.config.json", optional: false)
                .Build();
        }

        private string ReadCleanupDirectoryPath(IConfiguration config)
        {
            var configKey = $"{AppConfigRootSectionName}:{CleanupDirectoryPathSettingName}";

            var result = config.GetSection(configKey).Get<string>();

            var value = result != null
                ? $"\"{result}\""
                : NotSpecifiedPhrase;

            _log?.Debug($@"{ConfigName}: {CleanupDirectoryPathSettingName} = {value}");

            return result;
        }

        private RetentionRule[] ReadRetentionRules(IConfiguration config)
        {
            var configKey = $"{AppConfigRootSectionName}:{RetentionRulesSettingName}";

            var retentionRules = config
                .GetSection(configKey)
                .GetChildren()
                .Select(ReadRetentionRule)
                .ToArray();

            var value = retentionRules.Any()
                ? retentionRules.JoinThrough(", ")
                : NotSpecifiedPhrase;

            _log?.Debug($"{ConfigName}: {RetentionRulesSettingName} = {value}");

            return retentionRules;
        }

        private static RetentionRule ReadRetentionRule(IConfigurationSection retentionRuleSection)
        {
            var olderThanDays = ReadRetentionRuleProperty<int>(
                retentionRuleSection,
                nameof(RetentionRule.OlderThan));

            var allowedAmount = ReadRetentionRuleProperty<int>(
                retentionRuleSection,
                nameof(RetentionRule.AllowedAmount));

            var olderThan = TimeSpan.FromDays(olderThanDays);

            return new RetentionRule(olderThan, allowedAmount);
        }

        private static T ReadRetentionRuleProperty<T>(
            IConfiguration retentionRuleSection,
            string propertyName)
        {
            var propertySection = retentionRuleSection.GetSection(propertyName);

            return propertySection.Exists()
                ? propertySection.Get<T>()
                : throw new Exception(
                    $"{propertyName} property is not specified for a {nameof(RetentionRule)}.");
        }
    }
}