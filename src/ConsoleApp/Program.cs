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
            using (var container = new DIContainerBuilder().Build())
            {
                await container.Resolve<IApp>().Run();
            }
        }
    }
}
