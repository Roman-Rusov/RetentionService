using System.Threading.Tasks;

using Autofac;

namespace RetentionService.ConsoleApp
{
    /// <summary>
    /// Represents a program that executes the application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The entry point to the application.
        /// </summary>
        private static async Task Main()
        {
            var container = new DIContainerBuilder().BuildContainer();

            await container.Resolve<App>().Run();
        }
    }
}
