using System.Threading.Tasks;

namespace RetentionService.ConsoleApp
{
    /// <summary>
    /// Represents the interface of an application.
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// Runs the application.
        /// </summary>
        Task Run();
    }
}